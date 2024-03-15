// (C) Copyright 2024 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software
// in object code form for any purpose and without fee is hereby
// granted, provided that the above copyright notice appears in
// all copies and that both that copyright notice and the limited
// warranty and restricted rights notice below appear in all
// supporting documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK,
// INC. DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL
// BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is
// subject to restrictions set forth in FAR 52.227-19 (Commercial
// Computer Software - Restricted Rights) and DFAR 252.227-7013(c)
// (1)(ii)(Rights in Technical Data and Computer Software), as
// applicable.
//

using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace APSRevitCameraSync
{
    class ViewStateRestoringEventHandler : IExternalEventHandler
    {
        private ViewStateRestoringDialog parent;

		public WebViewerViewState ViewState { get; set; }

		public ViewStateRestoringEventHandler(ViewStateRestoringDialog parent)
        {
            this.parent = parent;
        }

        public void Execute(UIApplication uiApp)
        {
			UIDocument uiDoc = uiApp.ActiveUIDocument;
			Document doc = uiApp.ActiveUIDocument.Document;
			var view3D = doc.ActiveView as View3D;

			if (this.ViewState == null) throw new InvalidDataException("ViewState is not set.");
			if (view3D == null) throw new InvalidDataException("Current view is not a 3D view.");

			using (var trans = new Transaction(doc, "Restore Web Viewer Camera"))
			{
				try
				{
					if (trans.Start() == TransactionStatus.Started)
					{
						var isPerspective = this.ViewState.IsPerspective;

						if (!view3D.CanToggleBetweenPerspectiveAndIsometric())
							throw new InvalidOperationException("Cannot change view projection");

						if (view3D.IsPerspective && !isPerspective)
							view3D.ToggleToIsometric();

						if (!view3D.IsPerspective && isPerspective)
							view3D.ToggleToPerspective();

						// By default, the 3D view uses a default orientation.
						// Change the orientation by creating and setting a ViewOrientation3D 
						var position = new XYZ(this.ViewState.Position[0], this.ViewState.Position[1], this.ViewState.Position[2]);
						var target = new XYZ(this.ViewState.Target[0], this.ViewState.Target[1], this.ViewState.Target[2]);
						var up = new XYZ(this.ViewState.Up[0], this.ViewState.Up[1], this.ViewState.Up[2]);
						var fov = this.ViewState.FieldOfView;
						var aspectRatio = this.ViewState.Aspect;
						var orthographicHeight = this.ViewState.OrthoScale;

						var sightDir = target.Subtract(position).Normalize();
						var right = sightDir.CrossProduct(up);
						var adjustedUp = right.CrossProduct(sightDir);

						var orientation = new ViewOrientation3D(position, adjustedUp, sightDir);
						view3D.SetOrientation(orientation);

						XYZ[] zoomCorners = null;
						if (!isPerspective)
						{
							zoomCorners = this.CalculateZoomCorners(view3D, position, target, orthographicHeight);
						}
						else
						{
							zoomCorners = this.CalculateZoomCorners(view3D, position, target, fov, aspectRatio);
						}


						var uiView = uiApp.ActiveUIDocument.GetOpenUIViews().FirstOrDefault(v => v.ViewId == view3D.Id);
						uiView.ZoomAndCenterRectangle(zoomCorners[0], zoomCorners[1]);

						uiApp.ActiveUIDocument.RefreshActiveView();
						uiApp.ActiveUIDocument.UpdateAllOpenViews();

						trans.Commit();
					}
				}
				catch (Exception ex)
				{
					trans.RollBack();
					TaskDialog.Show("Revit", "Failed to restore view state!");
				}
			}

			this.parent.BringWindowToFront();
        }

        public string GetName()
        {
            return "Viewpoint Restoring event hanlder";
        }

		private XYZ[] CalculateZoomCorners(View3D view, XYZ position, XYZ target, double fov, double aspectRatio)
		{
			var sightVec = position - target; //new XYZ(position.X - target.X, position.Y - target.Y, position.Z - target.Z);
			var halfHeight = Math.Tan((fov / 2) * Math.PI / 180) * sightVec.GetLength();
			var halfWidth = halfHeight * aspectRatio;

			var upDirectionVec = sightVec.CrossProduct(view.RightDirection).Normalize() * halfHeight;
			var rightDirectionVecOnPlane = sightVec.CrossProduct(upDirectionVec).Normalize() * halfWidth;
			var diagonalVec = upDirectionVec.Add(rightDirectionVecOnPlane);

			var corner1 = target.Add(diagonalVec);
			var corner2 = target.Add(diagonalVec.Negate());

			return new XYZ[]
			{
				corner1,
				corner2
			};
		}

		private XYZ[] CalculateZoomCorners(View3D view, XYZ position, XYZ target, double orthographicHeight)
		{
			var sightVec = position - target;
			var halfHeight = orthographicHeight / 2;
			var halfWidth = halfHeight;

			var upDirectionVec = sightVec.CrossProduct(view.RightDirection).Normalize() * halfHeight;
			var rightDirectionVecOnNearPlane = sightVec.CrossProduct(upDirectionVec).Normalize() * halfWidth;
			var diagonalVec = upDirectionVec.Add(rightDirectionVecOnNearPlane);

			var corner1 = target.Add(diagonalVec);
			var corner2 = target.Add(diagonalVec.Negate());

			return new XYZ[] {
			corner1,
			corner2
		  };
		}
	}
}
