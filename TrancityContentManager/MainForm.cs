using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using Ionic.Zip;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrancityContentManager
{
    public partial class MainForm : Form
    {
        private INetworkModsProvider modsProvider;
        private ModInfo[] modsList;

        public MainForm()
        {
            InitializeComponent();
            typeSelectionBox.SelectedIndex = 0;
            modsProvider = new ObjBaseModProvider();
            PopulateModsList();
        }

        public async void PopulateModsList()
        {
            var progress = new Progress<int>(value =>
            {
                loadingInfoProgress.Value = value;
                loadingInfoProgress.Update();
            });
            modsList = await modsProvider.GetModsFromServer(progress);

            UpdateList();
        }

        private void UpdateList()
        {
            modsListView.Items.Clear();

            for (int i = 0; i < modsList.Length; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = modsList[i].Name;
                item.ImageIndex = i;
                modsListView.Items.Add(item);
            }

            ImageList images = new ImageList();
            for (int i = 0; i < modsList.Length; i++)
            {
                images.Images.Add(modsList[i].Image);
            }

            modsListView.SmallImageList = images;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.Show(this);
        }

        private void modsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = 0;

            if (modsListView.SelectedItems.Count > 0)
            {
                selectedIndex = modsList.ToList().FindIndex(x => x.Name == modsListView.SelectedItems[0].Text);
            }

            versionLabel.Text = $"Version: {modsList[selectedIndex].Version}";
            dateAddedLabel.Text = $"Date Added: {modsList[selectedIndex].DateUploaded}";
            modNameField.Text = modsList[selectedIndex].Name;
            ObjectPicturePreview.Image = modsList[selectedIndex].Image;
            sizeLabel.Text = $"Size: {modsList[selectedIndex].FileSize}";
            authorLabel.Text = $"Author: {modsList[selectedIndex].Uploader}";

            if (modsListView.SelectedIndices.Count < 2)
            {
                downloadButton.Text = "Download";
            }
            else
            {
                downloadButton.Text = $"Download({modsListView.SelectedIndices.Count})";
            }
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.ExecutablePath))
            {
                if (MessageBox.Show("Executable path is not defined! Do you want to set the trancity folder now?",
                        "Trancity Folder not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    settingsToolStripMenuItem_Click(this, null);
                }
                else
                {
                    return;
                }
            }

            for (int i = 0; i < modsListView.SelectedItems.Count; i++)
            {
                using WebClient client = new WebClient();
                
                int modIndex = modsList.ToList().FindIndex(x => x.Name == modsListView.SelectedItems[i].Text);
               
                string fileName = Settings.ExecutablePath + "/" + Path.GetFileName(modsList[modIndex].DownloadLink);
                
                statusLabel.Text = $"Downloading {i + 1}/{modsListView.SelectedItems.Count}";
                client.DownloadProgressChanged += OnDownloadProgressChanged;

                await client.DownloadFileTaskAsync(new Uri(modsList[modIndex].DownloadLink), fileName);
               
                statusLabel.Text = $"Extracting {i + 1}/{modsListView.SelectedItems.Count}";

                ExtractFile(fileName, Settings.ExecutablePath + "/data/objects/");
            }

            statusLabel.Text = "Ready.";
        }


        private async void ExtractFile(string fileName, string destination)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (ZipFile file = ZipFile.Read(fileName, new ReadOptions() { Encoding = Encoding.GetEncoding(866) }))
                    {

                        file.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
                    }
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Can't extract file! Skipping", "Error", MessageBoxButtons.OK);
            }

            File.Delete(fileName);
        }


        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress.Value = e.ProgressPercentage;
        }

        private void searchField_TextChanged(object sender, EventArgs e)
        {
            UpdateList();

            if (searchField.Text != "")
            {
                for (int i = modsListView.Items.Count - 1; i >= 0; i--)
                {
                    var item = modsListView.Items[i];
                    if (item.Text.ToLower().Contains(searchField.Text.ToLower()))
                    {
                        item.BackColor = SystemColors.Highlight;
                        item.ForeColor = SystemColors.HighlightText;
                    }
                    else
                    {
                        modsListView.Items.Remove(item);
                    }
                }
                if (modsListView.SelectedItems.Count == 1)
                {
                    modsListView.Focus();
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {

        }

        private void modsListView_Enter(object sender, EventArgs e)
        {
            foreach (ListViewItem item in modsListView.Items)
            {
                item.BackColor = SystemColors.Window;
                item.ForeColor = SystemColors.WindowText;
            }
        }
    }
}
