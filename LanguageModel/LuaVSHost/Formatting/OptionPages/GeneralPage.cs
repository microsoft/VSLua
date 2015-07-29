using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LuaLanguageService.Shared;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting.OptionPages
{
    [Guid(Guids.GeneralPageString)]
    internal class GeneralPage : BaseDialogPage
    {
        private System.Windows.Forms.CheckBox TestCheckBox;

        public override void SaveSettingsToStorage()
        {
            //base.SaveSettingsToStorage();
        }

        private void InitializeComponent()
        {
            this.TestCheckBox = new System.Windows.Forms.CheckBox();
            // 
            // TestCheckBox
            // 
            this.TestCheckBox.AutoSize = true;
            this.TestCheckBox.Location = new System.Drawing.Point(0, 0);
            this.TestCheckBox.Name = "TestCheckBox";
            this.TestCheckBox.Size = new System.Drawing.Size(104, 24);
            this.TestCheckBox.TabIndex = 0;
            this.TestCheckBox.Text = "TestCheckBox";
            this.TestCheckBox.UseVisualStyleBackColor = true;
            this.TestCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
