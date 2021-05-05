using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SQLite806x;

namespace SAD806x
{
    public partial class SQLite806xFileForm : Form
    {
        string sqlDB806xFilePath = string.Empty;
        SQLite806xDB sqlDB806x = null;
        bool BinaryMode = false;
        int RowId = -1;

        public SQLite806xFileForm (string db806xFilePath, bool binaryMode, string givenFilePath, int rowId)
        {
            sqlDB806xFilePath = db806xFilePath;
            BinaryMode = binaryMode;
            RowId = rowId;

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.Load += new EventHandler(Form_Load);
            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            uploadButton.Text = (RowId >= 0 ? "Update" : "Upload");
            if (givenFilePath != null && givenFilePath != string.Empty) filePathTextBox.Text = givenFilePath;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (sqlDB806xFilePath == string.Empty)
            {
                this.Close();
                return;
            }
            
            try
            {
                sqlDB806x = new SQLite806xDB(sqlDB806xFilePath);
            }
            catch
            {
                this.Close();
                return;
            }

            if (sqlDB806x == null)
            {
                this.Close();
                return;
            }

            if (!sqlDB806x.ValidDB)
            {
                this.Close();
                return;
            }

            if (RowId >= 0 || filePathTextBox.Text != string.Empty) showFileInformation();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel) Exit();
        }

        private void Exit()
        {
            sqlDB806x = null;

            Dispose();

            GC.Collect();
        }

        private void showFileInformation()
        {
            uploadButton.Enabled = false;

            fileTreeView.Nodes.Clear();

            TreeNode tnFileName = new TreeNode("Name");
            tnFileName.Name = "FileName";
            fileTreeView.Nodes.Add(tnFileName);
            TreeNode tnFileSize = new TreeNode("Size");
            tnFileSize.Name = "FileSize";
            fileTreeView.Nodes.Add(tnFileSize);
            TreeNode tnFileCreation = new TreeNode("Creation");
            tnFileCreation.Name = "FileCreation";
            fileTreeView.Nodes.Add(tnFileCreation);
            TreeNode tnFileModification = new TreeNode("Modification");
            tnFileModification.Name = "FileModification";
            fileTreeView.Nodes.Add(tnFileModification);
            TreeNode tnFileExtension = new TreeNode("Extension");
            tnFileExtension.Name = "FileExtension";
            fileTreeView.Nodes.Add(tnFileExtension);

            string sInfo = string.Empty;
            int iInfo = 0;

            if (RowId >= 0)
            // Searching and displaying record information
            {
                TreeNode tnEntryCreation = new TreeNode("Creation in database");
                tnEntryCreation.Name = "EntryCreation";
                fileTreeView.Nodes.Add(tnEntryCreation);
                TreeNode tnEntryModification = new TreeNode("Modification in database");
                tnEntryModification.Name = "EntryModification";
                fileTreeView.Nodes.Add(tnEntryModification);

                // To Be Done
                //descriptionTextBox.Text = string.Empty;

                //sortNumberNumericUpDown.Value = 0;

                if (filePathTextBox.Text == string.Empty)
                {
                    if (BinaryMode)
                    {
                        List<R_806x_Strategy_Binaries> lstBinaries = sqlDB806x.Read<R_806x_Strategy_Binaries>(RowId, new List<string>() { "FileBinary" }, string.Empty, string.Empty);
                        if (lstBinaries != null)
                        {
                            if (lstBinaries.Count == 1)
                            {
                                sInfo = (string)lstBinaries[0].Description.Value;
                                descriptionTextBox.Text = sInfo;

                                iInfo = Convert.ToInt32(lstBinaries[0].SortNumber.Value);
                                if (iInfo > sortNumberNumericUpDown.Minimum) iInfo = (int)sortNumberNumericUpDown.Maximum;
                                if (iInfo < sortNumberNumericUpDown.Minimum) iInfo = (int)sortNumberNumericUpDown.Minimum;
                                sortNumberNumericUpDown.Value = iInfo;

                                sInfo = (string)lstBinaries[0].Comments.Value;
                                // Windows 10 1809 (10.0.17763) Issue
                                commentsTextBox.Clear();
                                commentsTextBox.Multiline = false;
                                commentsTextBox.Multiline = true;
                                commentsTextBox.Text = sInfo;
                                commentsTextBox.Text = commentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

                                sInfo = (string)lstBinaries[0].FileName.Value;
                                tnFileName.ToolTipText = sInfo;
                                tnFileName.Nodes.Add(sInfo);

                                sInfo = string.Format("{0} {1}", Convert.ToInt32(lstBinaries[0].FileSize.Value) / 1024, "kB");
                                tnFileSize.ToolTipText = sInfo;
                                tnFileSize.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstBinaries[0].FileCreation.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnFileCreation.ToolTipText = sInfo;
                                tnFileCreation.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstBinaries[0].FileModification.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnFileModification.ToolTipText = sInfo;
                                tnFileModification.Nodes.Add(sInfo);

                                sInfo = (string)lstBinaries[0].FileExtension.Value;
                                tnFileExtension.ToolTipText = sInfo;
                                tnFileExtension.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstBinaries[0].EntryCreation.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnEntryCreation.ToolTipText = sInfo;
                                tnEntryCreation.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstBinaries[0].EntryModification.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnEntryModification.ToolTipText = sInfo;
                                tnEntryModification.Nodes.Add(sInfo);
                            }
                        }
                        lstBinaries = null;
                    }
                    else
                    {
                        List<R_806x_Strategy_Files> lstFiles = sqlDB806x.Read<R_806x_Strategy_Files>(RowId, new List<string>() { "FileBinary" }, string.Empty, string.Empty);
                        if (lstFiles != null)
                        {
                            if (lstFiles.Count == 1)
                            {
                                sInfo = (string)lstFiles[0].Description.Value;
                                descriptionTextBox.Text = sInfo;

                                iInfo = Convert.ToInt32(lstFiles[0].SortNumber.Value);
                                if (iInfo > sortNumberNumericUpDown.Minimum) iInfo = (int)sortNumberNumericUpDown.Maximum;
                                if (iInfo < sortNumberNumericUpDown.Minimum) iInfo = (int)sortNumberNumericUpDown.Minimum;
                                sortNumberNumericUpDown.Value = iInfo;

                                sInfo = (string)lstFiles[0].Comments.Value;
                                // Windows 10 1809 (10.0.17763) Issue
                                commentsTextBox.Clear();
                                commentsTextBox.Multiline = false;
                                commentsTextBox.Multiline = true;
                                commentsTextBox.Text = sInfo;
                                commentsTextBox.Text = commentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

                                sInfo = (string)lstFiles[0].FileName.Value;
                                tnFileName.ToolTipText = sInfo;
                                tnFileName.Nodes.Add(sInfo);

                                sInfo = string.Format("{0} {1}", Convert.ToInt32(lstFiles[0].FileSize.Value) / 1024, "kB");
                                tnFileSize.ToolTipText = sInfo;
                                tnFileSize.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstFiles[0].FileCreation.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnFileCreation.ToolTipText = sInfo;
                                tnFileCreation.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstFiles[0].FileModification.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnFileModification.ToolTipText = sInfo;
                                tnFileModification.Nodes.Add(sInfo);

                                sInfo = (string)lstFiles[0].FileExtension.Value;
                                tnFileExtension.ToolTipText = sInfo;
                                tnFileExtension.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstFiles[0].EntryCreation.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnEntryCreation.ToolTipText = sInfo;
                                tnEntryCreation.Nodes.Add(sInfo);

                                sInfo = Convert.ToDateTime(lstFiles[0].EntryModification.Value).ToString("yyyy-MM-dd HH:mm:ss");
                                tnEntryModification.ToolTipText = sInfo;
                                tnEntryModification.Nodes.Add(sInfo);
                            }
                        }
                        lstFiles = null;
                    }
                }

                uploadButton.Enabled = true;
            }

            if (filePathTextBox.Text != string.Empty)
            {
                FileInfo fiFI = new FileInfo(filePathTextBox.Text);
                if (fiFI.Exists)
                {
                    sInfo = fiFI.Name;
                    tnFileName.ToolTipText = sInfo;
                    tnFileName.Nodes.Add(sInfo);
                    if (descriptionTextBox.Text == string.Empty) descriptionTextBox.Text = sInfo;

                    sInfo = string.Format("{0} {1}", fiFI.Length / 1024, "kB");
                    tnFileSize.ToolTipText = sInfo;
                    tnFileSize.Nodes.Add(sInfo);

                    sInfo = fiFI.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                    tnFileCreation.ToolTipText = sInfo;
                    tnFileCreation.Nodes.Add(sInfo);

                    sInfo = fiFI.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                    tnFileModification.ToolTipText = sInfo;
                    tnFileModification.Nodes.Add(sInfo);

                    sInfo = fiFI.Extension;
                    tnFileExtension.ToolTipText = sInfo;
                    tnFileExtension.Nodes.Add(sInfo);

                    uploadButton.Enabled = true;
                }
                fiFI = null;
            }
        }
        
        private void selectButton_Click(object sender, EventArgs e)
        {
            if (BinaryMode)
            {
                if (openFileDialogBin.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogBin.FileName)) return;
                filePathTextBox.Text = openFileDialogBin.FileName;
            }
            else
            {
                if (openFileDialogGeneric.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogGeneric.FileName)) return;
                filePathTextBox.Text = openFileDialogGeneric.FileName;
            }

            showFileInformation();
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            R_806x_Strategy_Binaries rBinary = null;
            R_806x_Strategy_Files rFile = null;
            List<R_806x_Strategy_Binaries> lstBinaries = null;
            List<R_806x_Strategy_Files> lstFiles = null;

            if (RowId >= 0)
            {
                if (BinaryMode) lstBinaries = sqlDB806x.Read<R_806x_Strategy_Binaries>(RowId, new List<string>() { "FileBinary" }, string.Empty, string.Empty);
                else lstFiles = sqlDB806x.Read<R_806x_Strategy_Files>(RowId, new List<string>() { "FileBinary" }, string.Empty, string.Empty);
            }
            else
            {
                if (BinaryMode)
                {
                    rBinary = sqlDB806x.newRow<R_806x_Strategy_Binaries>();
                    lstBinaries = new List<R_806x_Strategy_Binaries>() {rBinary};
                    rBinary.EntryCreation.Value = DateTime.Now;
                }
                else
                {
                    rFile = sqlDB806x.newRow<R_806x_Strategy_Files>();
                    lstFiles = new List<R_806x_Strategy_Files>() {rFile};
                    rFile.EntryCreation.Value = DateTime.Now;
                }
            }

            if (BinaryMode)
            {
                rBinary.EntryModification.Value = DateTime.Now;
                rBinary.SortNumber.Value = (int)sortNumberNumericUpDown.Value;
                rBinary.Description.Value = descriptionTextBox.Text;
                rBinary.Comments.Value = commentsTextBox.Text;
            }
            else
            {
                rFile.EntryModification.Value = DateTime.Now;
                rFile.SortNumber.Value = (int)sortNumberNumericUpDown.Value;
                rFile.Description.Value = descriptionTextBox.Text;
                rFile.Comments.Value = commentsTextBox.Text;
            }

            if (filePathTextBox.Text != string.Empty)
            {
                if (File.Exists(filePathTextBox.Text))
                {
                    FileInfo fiFI = new FileInfo(filePathTextBox.Text);
                    if (fiFI.Length <= 256 * 1024 * 1024)    // 256MB limit
                    {
                        byte[] arrBytes = null;

                        using (FileStream fsFS = new FileStream(filePathTextBox.Text, FileMode.Open))
                        {
                            byte[] bBuffer = new byte[16 * 1024];
                            using (MemoryStream msMS = new MemoryStream())
                            {
                                int iRead;
                                while ((iRead = fsFS.Read(bBuffer, 0, bBuffer.Length)) > 0) msMS.Write(bBuffer, 0, iRead);
                                arrBytes = msMS.ToArray();
                            }
                        }

                        if (BinaryMode)
                        {
                            rBinary.FileName.Value = fiFI.Name;
                            rBinary.FileSize.Value = fiFI.Length;
                            rBinary.FileCreation.Value = fiFI.CreationTime;
                            rBinary.FileModification.Value = fiFI.LastWriteTime;
                            rBinary.FileExtension.Value = fiFI.Extension;
                            rBinary.File.Value = arrBytes;
                        }
                        else
                        {
                            rFile.FileName.Value = fiFI.Name;
                            rFile.FileSize.Value = fiFI.Length;
                            rFile.FileCreation.Value = fiFI.CreationTime;
                            rFile.FileModification.Value = fiFI.LastWriteTime;
                            rFile.FileExtension.Value = fiFI.Extension;
                            rFile.File.Value = arrBytes;
                        }

                        arrBytes = null;
                    }
                    fiFI = null;
                }
            }

            bool bResult = false;
            
            if (BinaryMode) bResult = sqlDB806x.Write<R_806x_Strategy_Binaries>(ref lstBinaries);
            else bResult = sqlDB806x.Write<R_806x_Strategy_Files>(ref lstFiles);

            if (bResult)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Writting into database has failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
