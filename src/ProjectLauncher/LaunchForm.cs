using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using EnvDTE;
using EnvDTE80;

namespace ProjectLauncher
{
    public partial class LaunchForm : Form
    {
        DTE2 _dte;
        string _filePath;
        public LaunchForm(DTE2 dte)
        {
            _dte = dte;
            InitializeComponent();
        }

        public string FilePath
        {
            get { return this._filePath; }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            if(_filePath != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void LaunchForm_Load(object sender, EventArgs e)
        {
            if (_dte.ActiveDocument != null)
            {
                string activeDocName = _dte.ActiveDocument.Name;
                cmbProjects.Items.Add(activeDocName);
            }

            if(this.cmbProjects.Items.Count > 0)
                this.cmbProjects.SelectedIndex = 0;            
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lua Hosts|*.exe";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.lblPath.Text = openFileDialog.FileName;
                _filePath = openFileDialog.FileName;
            }
        }
    }
}
