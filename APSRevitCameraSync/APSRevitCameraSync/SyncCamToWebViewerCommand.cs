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

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace APSRevitCameraSync
{
    [Transaction(TransactionMode.Manual)]
    public class SyncCamToWebViewerCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            var view = doc.ActiveView as View3D;

            if (null == view)
            {
                message = "Please run this command in a 3D view.";
                return Result.Failed;
            }

            var context = new CameraInfoExportContext(view.Id, uiDoc);
            using (var exporter = new CustomExporter(doc, context))
            {
                exporter.IncludeGeometricObjects = false;
                exporter.ShouldStopOnError = true;
                exporter.Export((View)view);
            }

            return Result.Succeeded;
        }
    }
}
