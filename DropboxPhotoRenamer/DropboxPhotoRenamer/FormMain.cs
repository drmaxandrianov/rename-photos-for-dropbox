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

namespace DropboxPhotoRenamer
{
    public partial class FormMain : Form
    {
        private string _rootFolderPath = "";

        public FormMain()
        {
            InitializeComponent();
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ButtonSelectFolder.Enabled = true;
            ButtonRename.Enabled = true;
        }

        void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var files = Directory.GetFiles(_rootFolderPath);

            for (var i = 0; i < files.Length; i++)
            {
                // Photo 20-05-15 15 51 02.jpg -> 2015-03-20 11.53.05.jpg
                try { 
                    var fileName = Path.GetFileNameWithoutExtension(files[i]);
                    var filePath = files[i].Replace(Path.GetFileName(files[i]), "");
                    var parts = fileName.Split(new char[]{' ', '-', '.'});
                    var newName = "20" + parts[3] + "-" + parts[2] + "-" + parts[1];
                    newName += " " + parts[4] + "." + parts[5] + "." + parts[6];


                    Console.WriteLine(files[i] + "\t - \t" + filePath + newName);
                    try
                    {
                        File.Move(files[i], filePath + newName + Path.GetExtension(files[i]));
                    }
                    catch
                    {
                        try
                        {
                            File.Move(files[i], filePath + newName + " duplicated" + Path.GetExtension(files[i]));
                        } catch {}
                    }
                } catch {
                    continue;
                }

                BackgroundWorker.ReportProgress(i + 1);
            }
        }

        private void ButtonSelectFolder_Click(object sender, EventArgs e)
        {
            var result = FolderBrowserDialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _rootFolderPath = FolderBrowserDialog.SelectedPath;
            }
        }

        private void ButtonRename_Click(object sender, EventArgs e)
        {
            if (_rootFolderPath == "")
            {
                MessageBox.Show(this, "Select folder with files!", "Select Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var files = Directory.GetFiles(_rootFolderPath);

            ProgressBar.Value = 0;
            ProgressBar.Maximum = files.Length;

            ButtonSelectFolder.Enabled = false;
            ButtonRename.Enabled = false;

            BackgroundWorker.RunWorkerAsync();
        }
    }
}
