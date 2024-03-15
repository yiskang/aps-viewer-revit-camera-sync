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

namespace APSRevitCameraSync
{
    partial class ViewStateRestoringDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tbViewState = new System.Windows.Forms.TextBox();
            this.btnRestore = new System.Windows.Forms.Button();
            this.mainLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.ColumnCount = 1;
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutPanel.Controls.Add(this.tbViewState, 0, 0);
            this.mainLayoutPanel.Controls.Add(this.btnRestore, 0, 1);
            this.mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            this.mainLayoutPanel.RowCount = 2;
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.5F));
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.mainLayoutPanel.Size = new System.Drawing.Size(398, 393);
            this.mainLayoutPanel.TabIndex = 0;
            // 
            // tbViewState
            // 
            this.tbViewState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbViewState.Location = new System.Drawing.Point(3, 3);
            this.tbViewState.Multiline = true;
            this.tbViewState.Name = "tbViewState";
            this.tbViewState.Size = new System.Drawing.Size(392, 337);
            this.tbViewState.TabIndex = 0;
            // 
            // btnRestore
            // 
            this.btnRestore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestore.Location = new System.Drawing.Point(3, 346);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(392, 44);
            this.btnRestore.TabIndex = 1;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // ViewStateRestoringDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 393);
            this.Controls.Add(this.mainLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewStateRestoringDialog";
            this.Text = "Restore Web Viewer View State";
            this.mainLayoutPanel.ResumeLayout(false);
            this.mainLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.TextBox tbViewState;
        private System.Windows.Forms.Button btnRestore;
    }
}