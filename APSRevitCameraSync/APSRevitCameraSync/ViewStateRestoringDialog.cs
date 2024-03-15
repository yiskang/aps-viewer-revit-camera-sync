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
using Autodesk.Revit.UI;
using RvtApplication = Autodesk.Revit.ApplicationServices.Application;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace APSRevitCameraSync
{
    public partial class ViewStateRestoringDialog : System.Windows.Forms.Form
    {
        private UIApplication rvtUIApp = null;
        private ViewStateRestoringEventHandler modifierEventHandler = null;
        private ExternalEvent modifierEvent = null;

        // DLL imports from user32.dll to set focus to
        // Revit to force it to forward the external event
        // Raise to actually call the external event 
        // Execute.

        /// <summary>
        /// The GetForegroundWindow function returns a 
        /// handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Move the window associated with the passed 
        /// handle to the front.
        /// </summary>
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public ViewStateRestoringDialog(RvtApplication application)
        {
            InitializeComponent();

            this.rvtUIApp = new UIApplication(application);
            this.modifierEventHandler = new ViewStateRestoringEventHandler(this);
            this.modifierEvent = ExternalEvent.Create(this.modifierEventHandler);
        }

        public void SendWindowToBack()
        {
            // Set focus to Revit for a moment.
            // Otherwise, it may take a while before 
            // Revit forwards the event Raise to the
            // event handler Execute method.
            SetForegroundWindow(this.rvtUIApp.MainWindowHandle);
        }

        public void BringWindowToFront()
        {
            //IntPtr hBefore = GetForegroundWindow();
            SetForegroundWindow(this.Handle);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbViewState.Text))
                return;

            var viewState = JsonConvert.DeserializeObject<WebViewerViewState>(this.tbViewState.Text);

            if (viewState == null)
            {
                TaskDialog.Show("Revit", "Input view state data is not a valid JSON!");
                return;
            }

            this.modifierEventHandler.ViewState = viewState;
            this.modifierEvent.Raise();
            this.SendWindowToBack();
        }
    }
}
