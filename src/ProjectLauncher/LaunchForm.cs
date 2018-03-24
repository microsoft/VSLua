// Copyright (c) Microsoft. All rights reserved.

namespace ProjectLauncher
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using EnvDTE;
    using EnvDTE80;

    public partial class LaunchForm : Form
    {
        private DTE2 dte;
        private string filePath;

        public LaunchForm(DTE2 dte)
        {
            this.dte = dte;
            this.InitializeComponent();
        }

        public string FilePath
        {
            get { return this.filePath; }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            if (this.filePath != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void LaunchForm_Load(object sender, EventArgs e)
        {
            if (this.dte.ActiveDocument != null)
            {
                string activeDocName = this.dte.ActiveDocument.Name;
                this.cmbProjects.Items.Add(activeDocName);
            }

            if (this.cmbProjects.Items.Count > 0)
            {
                this.cmbProjects.SelectedIndex = 0;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lua Hosts|*.exe";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.lblPath.Text = openFileDialog.FileName;
                this.filePath = openFileDialog.FileName;
            }
        }
    }
}
