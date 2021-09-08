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

$(document).ready(function () {
    const baseUrl = window.location.href.split('?')[0];
    Autodesk.Viewing.theExtensionManager.registerExternalExtension('Autodesk.ADN.RevitCameraSync', `${baseUrl}js/revit-camera-sync.js`);

    const viewerOptions = {
        extensions: ['Autodesk.ADN.RevitCameraSync']
    };

    destroyViewer();
    launchViewer('dXJuOmFkc2sub2JqZWN0czpvcy5vYmplY3Q6OWR5YXI1enNtZ2NsaWJiYWVuaHQ5YmFjaWYyMnpvN3ctc2FuZGJveC9ydnRfc2FtcGxlX2hvdXNlX2NvbXBvc2l0ZS56aXA', null, viewerOptions);
});