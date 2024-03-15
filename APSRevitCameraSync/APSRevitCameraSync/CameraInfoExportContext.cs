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
using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Text;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace APSRevitCameraSync
{
    internal class CameraInfoExportContext : IExportContext
    {
        private ElementId viewId;
        private Document document;
        private UIDocument uIDocument;
        private WebViewerViewState cameraDefinition;

        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public CameraInfoExportContext(ElementId view3DId, UIDocument uiDocument)
        {
            this.viewId = view3DId;
            this.uIDocument = uiDocument;
            this.document = uiDocument.Document;
        }

        public void Finish()
        {
            System.Diagnostics.Trace.WriteLine("Sending Revit camera info to APS Viewer");
            var data = JsonConvert.SerializeObject(this.cameraDefinition, this.jsonSerializerSettings);

            var url = "http://localhost:8080/api/viewStates:sync";

            try
            {
                var client = new HttpClient();
                //client.BaseAddress = new Uri(url);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;
            }
            catch (Exception ex)
            {
                // Log
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        public bool IsCanceled()
        {
            return false;
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            return RenderNodeAction.Skip;
        }

        public void OnElementEnd(ElementId elementId) { }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnFaceEnd(FaceNode node) { }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnInstanceEnd(InstanceNode node) { }

        public void OnLight(LightNode node) { }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnLinkEnd(LinkNode node) { }

        public void OnMaterial(MaterialNode node) { }

        public void OnPolymesh(PolymeshTopology node) { }

        public void OnRPC(RPCNode node) { }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            if (node.ViewId.IntegerValue != this.viewId.IntegerValue)
                return RenderNodeAction.Skip;


            var view3dElem = this.document.GetElement(node.ViewId);
            if (!(view3dElem is View3D))
                throw new InvalidOperationException(string.Format("Input view `{0}` is not a View3D type", viewId.IntegerValue));

            var view3d = view3dElem as View3D;
            var cameraInfo = node.GetCameraInfo();
            var isPerspective = cameraInfo.IsPerspective;

            double fov = 0;
            if (isPerspective)
            {
                // https://thebuildingcoder.typepad.com/blog/2020/04/revit-camera-fov-forge-partner-talks-and-jobs.html
                fov = 2 * Math.Atan(cameraInfo.HorizontalExtent / (2 * cameraInfo.TargetDistance)) * 180 / Math.PI;
            }

            var aspect = cameraInfo.HorizontalExtent / cameraInfo.VerticalExtent;

            var viewOrientation = view3d.GetOrientation();
            var up = viewOrientation.UpDirection;
            var eye = viewOrientation.EyePosition;
            var forwardDirection = viewOrientation.ForwardDirection;
            var rightDirection = forwardDirection.CrossProduct(up);

            IList<UIView> views = this.uIDocument.GetOpenUIViews();
            UIView currentView = views.FirstOrDefault(t => t.ViewId == this.viewId);

            if (currentView == null)
            {
                throw new InvalidOperationException("selected view is not opned");
            }

            IList<XYZ> corners = currentView.GetZoomCorners();
            XYZ corner1 = corners[0];
            XYZ corner2 = corners[1];

            ////center of the UI view
            //double x = (corner1.X + corner2.X) / 2;
            //double y = (corner1.Y + corner2.Y) / 2;
            //double z = (corner1.Z + corner2.Z) / 2;
            //XYZ viewCenter = new XYZ(x, y, z);
            ////XYZ target = viewCenter;
            //eye = viewCenter;

            //// Calculate diagonal vector
            //XYZ diagVector = corner1 - corner2;

            //double dist = corner1.DistanceTo(viewCenter) / 2;
            //var orthoHeight = cameraInfo.VerticalExtent / 2;  //dist * Math.Sin(diagVector.AngleTo(view3d.RightDirection));

            ////eye = target - forwardDirection * orthoHeight;
            //XYZ target = eye + forwardDirection * orthoHeight;

            // center of the UI view
            double x = (corner1.X + corner2.X) / 2;
            double y = (corner1.Y + corner2.Y) / 2;
            double z = (corner1.Z + corner2.Z) / 2;
            XYZ viewCenter = new XYZ(x, y, z);
            XYZ target = viewCenter;

            XYZ diagVector = corner1 - target;
            double dist = corner1.DistanceTo(viewCenter) / 2;
            var orthoHeight = dist * Math.Sin(diagVector.AngleTo(rightDirection)) * 2;

            eye = target - forwardDirection * orthoHeight;

            var cameraDef = new WebViewerViewState
            {
                Aspect = aspect,
                IsPerspective = isPerspective,
                FieldOfView = fov,
                Position = new double[] { eye.X, eye.Y, eye.Z },
                Target = new double[] { target.X, target.Y, target.Z },
                Up = new double[] { up.X, up.Y, up.Z },
                OrthoScale = orthoHeight
            };
            this.cameraDefinition = cameraDef;

            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId) { }

        public bool Start()
        {
            System.Diagnostics.Trace.WriteLine("Starting export view camera info");
            return true;
        }
    }
}