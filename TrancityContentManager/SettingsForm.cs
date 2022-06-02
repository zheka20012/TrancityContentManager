using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrancityContentManager
{
    public partial class SettingsForm : Form
    {
        private string trancityExecutablePath = string.Empty;


        public SettingsForm()
        {
            InitializeComponent();
            
            trancityExecutablePath = textBox1.Text = Settings.ExecutablePath;
        }

        private void pickFolderButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() {Filter = "Trancity.exe|trancity.exe"};

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                trancityExecutablePath = textBox1.Text = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Settings.ExecutablePath = trancityExecutablePath;
            Settings.Save();
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            trancityExecutablePath = textBox1.Text;
        }
    }
}
