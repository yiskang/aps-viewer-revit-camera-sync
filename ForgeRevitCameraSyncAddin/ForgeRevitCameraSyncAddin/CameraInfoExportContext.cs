/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;

using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Text;

namespace ForgeRevitCameraSyncAddin
{
    internal class CameraInfoExportContext : IExportContext
    {
        private ElementId viewId;
        private Document document;
        private ForgeCameraDefinition cameraDefinition;

        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        private class ForgeCameraDefinition
        {
            [JsonProperty("aspect")]
            public double Aspect { get; set; }
            [JsonProperty("isPerspective")]
            public bool IsPerspective { get; set; }
            [JsonProperty("fov")]
            public double FOV { get; set; }
            [JsonProperty("position")]
            public double[] Position { get; set; }
            [JsonProperty("target")]
            public double[] Target { get; set; }
            [JsonProperty("up")]
            public double[] Up { get; set; }
            [JsonProperty("orthoScale")]
            public double OrthoScale { get; set; }
        }
        
        public CameraInfoExportContext(ElementId view3DId, Document document)
        {
            this.viewId = view3DId;
            this.document = document;
        }

        public void Finish()
        {
            System.Diagnostics.Trace.WriteLine("Sending view camera info to Forge");
            var data = JsonConvert.SerializeObject(this.cameraDefinition, this.jsonSerializerSettings);

            var url = "http://1ocalhost:3000/api/forge/views/sync";
            

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(url);
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

        public void OnElementEnd(ElementId elementId) {}

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnFaceEnd(FaceNode node) {}

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnInstanceEnd(InstanceNode node) {}

        public void OnLight(LightNode node) {}

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            return RenderNodeAction.Skip;
        }

        public void OnLinkEnd(LinkNode node) {}

        public void OnMaterial(MaterialNode node) {}

        public void OnPolymesh(PolymeshTopology node) {}

        public void OnRPC(RPCNode node) {}

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
                fov = 2 * Math.Atan(cameraInfo.HorizontalExtent) / (2 * cameraInfo.TargetDistance);
            }

            var aspect = cameraInfo.HorizontalExtent / cameraInfo.VerticalExtent;

            var viewOrientation = view3d.GetOrientation();
            var up = viewOrientation.UpDirection;
            var eye = viewOrientation.EyePosition;
            var target = eye + viewOrientation.ForwardDirection.Normalize() * Math.Abs(cameraInfo.TargetDistance);
            var orthoHeight = cameraInfo.TargetDistance;

            var cameraDef = new ForgeCameraDefinition
            {
                Aspect = aspect,
                IsPerspective = isPerspective,
                FOV = fov,
                Position = new double[] { eye.X, eye.Y, eye.Z },
                Target = new double[] { target.X, target.Y, target.Z },
                Up = new double[] { up.X, up.Y, up.Z },
                OrthoScale = orthoHeight
            };
            this.cameraDefinition = cameraDef;

            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId) {}

        public bool Start()
        {
            System.Diagnostics.Trace.WriteLine("Starting export view camera info");
            return true;
        }
    }
}
