using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xTools
    {
        public static string TableNameFromType(Type tType)
        {
            if (tType.Namespace != typeof(SQLite806xTools).Namespace) return string.Empty;
            if (!tType.Name.StartsWith("R_")) return string.Empty;

            return tType.Name.Substring(2);
        }

        public static T_SQLiteTable initTable<T>(string label, string description) where T : new()
        {
            return initTable<T>(label, description, false, true);
        }

        public static T_SQLiteTable initTable<T>(string label, string description, bool readOnly, bool visible) where T : new()
        {
            if (typeof(T).Namespace != typeof(SQLite806xTools).Namespace) return null;
            if (!typeof(T).Name.StartsWith("R_")) return null;

            string tableName = typeof(T).Name.Substring(2);

            return new T_SQLiteTable(tableName, typeof(T), label, description, readOnly, visible, true);
        }

        // MainForm Part
        // Select UniDB 806x (.86x) file
        public static bool selectUniDb806x(ref ToolStripTextBox fileUniDb806xToolStripTextBox, string sFilePath, ref ToolStripMenuItem binariesUniDb806xToolStripMenuItem, ref ToolStripMenuItem filesUniDb806xToolStripMenuItem, EventHandler eventHandler)
        {
            fileUniDb806xToolStripTextBox.Tag = sFilePath;
            if (fileUniDb806xToolStripTextBox.Tag == null) return false;
            string filePath = (string)fileUniDb806xToolStripTextBox.Tag;
            if (filePath == string.Empty) return false;

            FileInfo fiFI = new FileInfo(filePath);
            fileUniDb806xToolStripTextBox.Text = fiFI.Name;
            fileUniDb806xToolStripTextBox.ToolTipText = filePath;
            if (fiFI.Exists)
            {
                fileUniDb806xToolStripTextBox.ToolTipText += "\r\nModified on" + fiFI.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                fileUniDb806xToolStripTextBox.ToolTipText += "\r\nWith size " + fiFI.Length / 1024 + " kB.";
            }
            else
            {
                fileUniDb806xToolStripTextBox.ToolTipText += "\r\nNew file";
            }
            fiFI = null;

            if (!openUniDb806x(filePath, ref binariesUniDb806xToolStripMenuItem, ref filesUniDb806xToolStripMenuItem, eventHandler))
            //  Invalid UniDb806x file
            {
                fileUniDb806xToolStripTextBox.Text = string.Empty;
                fileUniDb806xToolStripTextBox.ToolTipText = string.Empty;
                fileUniDb806xToolStripTextBox.Tag = null;
                MessageBox.Show("Selected file can not be used.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        // Read details from UniDB 806x (.86x) file
        public static bool openUniDb806x(string uniDb806xFilePath, ref ToolStripMenuItem binariesUniDb806xToolStripMenuItem, ref ToolStripMenuItem filesUniDb806xToolStripMenuItem, EventHandler eventHandler)
        {
            bool openingResult = true;

            SQLite806xDB db806x = null;
            List<R_806x_Strategy_Binaries> lstBinaries = null;
            List<R_806x_Strategy_Files> lstFiles = null;

            try { db806x = new SQLite806xDB(uniDb806xFilePath); }
            catch { openingResult = false; }

            if (db806x != null)
            {
                if (db806x.ValidDB)
                {
                    lstBinaries = db806x.Read<R_806x_Strategy_Binaries>(-1, new List<string>() { "File" }, string.Empty, "SortNumber ASC");
                    lstFiles = db806x.Read<R_806x_Strategy_Files>(-1, new List<string>() { "File" }, string.Empty, "SortNumber ASC");
                }
                else
                {
                    string errorMessage = string.Empty;
                    if (db806x.MissingTables != null)
                    {
                        if (errorMessage != string.Empty) errorMessage += "\r\n\r\n";
                        errorMessage += "Following table are required:";
                        foreach (T_SQLiteTable tTable in db806x.MissingTables) errorMessage += "\r\n\t- " + tTable.Name;
                    }
                    if (db806x.MissingFields != null)
                    {
                        if (errorMessage != string.Empty) errorMessage += "\r\n\r\n";
                        errorMessage += "Following fields are required:";
                        foreach (F_SQLiteField fField in db806x.MissingFields) errorMessage += "\r\n\t- " + fField.TableName + "." + fField.Name;
                    }
                    if (db806x.DifferentFields != null)
                    {
                        if (errorMessage != string.Empty) errorMessage += "\r\n\r\n";
                        errorMessage += "Following fields are not fully compliant:";
                        foreach (F_SQLiteField fField in db806x.DifferentFields) errorMessage += "\r\n\t- " + fField.TableName + "." + fField.Name;
                    }

                    if (errorMessage != string.Empty) MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    openingResult = false;
                }
            }

            refreshFilesUniDb806xMenu<R_806x_Strategy_Binaries>(ref binariesUniDb806xToolStripMenuItem, ref lstBinaries, eventHandler);
            refreshFilesUniDb806xMenu<R_806x_Strategy_Files>(ref filesUniDb806xToolStripMenuItem, ref lstFiles, eventHandler);

            return openingResult;
        }

        public static void refreshFilesUniDb806xMenu<T>(ref ToolStripMenuItem filesToolStripMenuItem, string uniDb806xFilePath, EventHandler eventHandler) where T : new()
        {
            SQLite806xDB db806x = null;
            List<T> lstFiles = null;

            try { db806x = new SQLite806xDB(uniDb806xFilePath); }
            catch { }

            if (db806x != null)
            {
                if (db806x.ValidDB) lstFiles = db806x.Read<T>(-1, new List<string>() { "File" }, string.Empty, "SortNumber ASC");
                db806x = null;
            }

            refreshFilesUniDb806xMenu(ref filesToolStripMenuItem, ref lstFiles, eventHandler);
        }

        public static void refreshFilesUniDb806xMenu<T>(ref ToolStripMenuItem filesToolStripMenuItem, ref List<T> lstFiles, EventHandler eventHandler)
        {
            filesToolStripMenuItem.DropDownItems.Clear();

            if (lstFiles == null) return;

            ToolStripMenuItem tsItem = null;
            if (typeof(T) == typeof(R_806x_Strategy_Binaries))
            {
                tsItem = new ToolStripMenuItem("Add current binary");
                tsItem.Name = "addCurrentToBinariesUniDb806xToolStripMenuItem";
                tsItem.ToolTipText = "Add current binary to data base.";
                tsItem.Click += eventHandler;
                filesToolStripMenuItem.DropDownItems.Add(tsItem);

                tsItem = new ToolStripMenuItem("Add another binary");
                tsItem.Name = "addAnotherToBinariesUniDb806xToolStripMenuItem";
                tsItem.ToolTipText = "Add another binary to data base.";
                tsItem.Click += eventHandler;
                filesToolStripMenuItem.DropDownItems.Add(tsItem);
            }
            else if (typeof(T) == typeof(R_806x_Strategy_Files))
            {
                tsItem = new ToolStripMenuItem("Add file");
                tsItem.Name = "addFileToFilesUniDb806xToolStripMenuItem";
                tsItem.ToolTipText = "Add file to data base.";
                tsItem.Click += eventHandler;
                filesToolStripMenuItem.DropDownItems.Add(tsItem);
            }
            tsItem = null;


            if (lstFiles.Count == 0) return;

            filesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

            foreach (object oFile in lstFiles)
            {
                R_SQLite_Files rFile = (R_SQLite_Files)oFile;
                tsItem = new ToolStripMenuItem((string)rFile.Description.Value);
                tsItem.Tag = rFile;
                tsItem.Name = rFile.RowId.Value.ToString();
                tsItem.ToolTipText = (string)rFile.Description.Value;
                if ((string)rFile.Description.Value != (string)rFile.FileName.Value)
                {
                    tsItem.ToolTipText += "\r\n" + rFile.FileName.Value;
                }
                tsItem.ToolTipText += "\r\nSize : " + (((int)rFile.FileSize.Value) / 1024).ToString() + " kB";
                tsItem.ToolTipText += "\r\nAdded on : " + ((DateTime)rFile.EntryCreation.Value).ToString("yyyy-MM-dd HH:mm:ss");
                if ((DateTime)rFile.EntryCreation.Value != (DateTime)rFile.EntryModification.Value)
                {
                    tsItem.ToolTipText += "\r\nUpdated on : " + ((DateTime)rFile.EntryCreation.Value).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (rFile.Comments.Value != null && (string)rFile.Comments.Value != string.Empty) tsItem.ToolTipText += "\r\n" + rFile.Comments.Value;
                tsItem.Click += eventHandler;
                filesToolStripMenuItem.DropDownItems.Add(tsItem);
                tsItem = null;
            }
        }

        public static bool isForStrategyFileAdd(ToolStripMenuItem fileMenuItem)
        {
            if (fileMenuItem == null) return false;

            switch (fileMenuItem.Name)
            {
                case "addCurrentToBinariesUniDb806xToolStripMenuItem":
                case "addAnotherToBinariesUniDb806xToolStripMenuItem":
                case "addFileToFilesUniDb806xToolStripMenuItem":
                    return true;
            }
            return false;
        }

        public static void strategyFileRemove(Dictionary<string, F_SQLiteField> attachmentDetails)
        {
            if (attachmentDetails == null) return;
            if (!attachmentDetails.ContainsKey("Binary")) return;
            if (!attachmentDetails.ContainsKey("Name")) return;

            attachmentDetails["Binary"].Value = null;
            attachmentDetails["Name"].Value = null;
            if (attachmentDetails.ContainsKey("Size")) attachmentDetails["Size"].Value = 0;
            if (attachmentDetails.ContainsKey("Creation")) attachmentDetails["Creation"].Value = DateTime.Now;
            if (attachmentDetails.ContainsKey("Modification")) attachmentDetails["Modification"].Value = DateTime.Now;
            if (attachmentDetails.ContainsKey("Extension")) attachmentDetails["Extension"].Value = null;
        }

        public static bool strategyFileAdd(ToolStripMenuItem fileMenuItem, string dbFilePath, ref SADBin sadBin, ref ToolStripMenuItem binariesUniDb806xToolStripMenuItem, ref ToolStripMenuItem filesUniDb806xToolStripMenuItem, EventHandler eventHandler)
        {
            if (fileMenuItem == null) return false;

            bool bRefreshBinaries = false;
            bool bRefreshFiles = false;

            switch (fileMenuItem.Name)
            {
                case "addCurrentToBinariesUniDb806xToolStripMenuItem":
                    if (sadBin == null) return false;
                    bRefreshBinaries = new SQLite806xFileForm(dbFilePath, true, sadBin.BinaryFilePath, -1).ShowDialog() == DialogResult.OK;
                    break;
                case "addAnotherToBinariesUniDb806xToolStripMenuItem":
                    bRefreshBinaries = new SQLite806xFileForm(dbFilePath, true, string.Empty, -1).ShowDialog() == DialogResult.OK;
                    break;
                case "addFileToFilesUniDb806xToolStripMenuItem":
                    bRefreshFiles = new SQLite806xFileForm(dbFilePath, false, string.Empty, -1).ShowDialog() == DialogResult.OK;
                    break;
            }

            if (bRefreshBinaries) SQLite806xTools.refreshFilesUniDb806xMenu<R_806x_Strategy_Binaries>(ref binariesUniDb806xToolStripMenuItem, dbFilePath, eventHandler);
            if (bRefreshFiles) SQLite806xTools.refreshFilesUniDb806xMenu<R_806x_Strategy_Files>(ref filesUniDb806xToolStripMenuItem, dbFilePath, eventHandler);

            return bRefreshBinaries || bRefreshFiles;
        }

        public static bool strategyFileUpdate(Dictionary<string, F_SQLiteField> attachmentDetails, ref OpenFileDialog openFileDialogGeneric)
        {
            if (attachmentDetails == null) return false;
            if (openFileDialogGeneric == null) return false;
            if (!attachmentDetails.ContainsKey("Binary")) return false;
            if (!attachmentDetails.ContainsKey("Name")) return false;

            if (openFileDialogGeneric.ShowDialog() != DialogResult.OK) return false;
            if (!File.Exists(openFileDialogGeneric.FileName)) return false;

            string filePath = openFileDialogGeneric.FileName;
            bool bResult = false;
            FileInfo fiFI = new FileInfo(filePath);
            if (fiFI.Length <= 256 * 1024 * 1024)    // 256MB limit
            {
                byte[] arrBytes = null;

                using (FileStream fsFS = new FileStream(filePath, FileMode.Open))
                {
                    byte[] bBuffer = new byte[16 * 1024];
                    using (MemoryStream msMS = new MemoryStream())
                    {
                        int iRead;
                        while ((iRead = fsFS.Read(bBuffer, 0, bBuffer.Length)) > 0) msMS.Write(bBuffer, 0, iRead);
                        arrBytes = msMS.ToArray();
                    }
                }

                attachmentDetails["Binary"].Value = arrBytes;
                arrBytes = null;

                attachmentDetails["Name"].Value = fiFI.Name;
                if (attachmentDetails.ContainsKey("Size")) attachmentDetails["Size"].Value = fiFI.Length;
                if (attachmentDetails.ContainsKey("Creation")) attachmentDetails["Creation"].Value = fiFI.CreationTime;
                if (attachmentDetails.ContainsKey("Modification")) attachmentDetails["Modification"].Value = fiFI.LastWriteTime;
                if (attachmentDetails.ContainsKey("Extension")) attachmentDetails["Extension"].Value = fiFI.Extension;
                bResult = true;
            }
            fiFI = null;

            return bResult;
        }

        public static bool strategyFileUpdate<T>(object rowFileRecord, string dbFilePath, ref ToolStripMenuItem filesUniDb806xToolStripMenuItem, EventHandler eventHandler) where T : new()
        {
            if (rowFileRecord == null) return false;
            if (rowFileRecord.GetType() != typeof(T)) return false;

            bool bRefresh = false;

            if (((R_SQLite_Files)rowFileRecord).RowId.Value != null)
            {
                bRefresh = new SQLite806xFileForm(dbFilePath, false, string.Empty, Convert.ToInt32(((R_SQLite_Files)rowFileRecord).RowId.Value)).ShowDialog() == DialogResult.OK;
                if (bRefresh) SQLite806xTools.refreshFilesUniDb806xMenu<T>(ref filesUniDb806xToolStripMenuItem, dbFilePath, eventHandler);
            }

            return bRefresh;
        }

        public static void strategyFileDownload(Dictionary<string, F_SQLiteField> attachmentDetails,  ref SaveFileDialog saveFileDialogGeneric)
        {
            if (attachmentDetails == null) return;
            if (saveFileDialogGeneric == null) return;
            if (!attachmentDetails.ContainsKey("Binary")) return;
            if (attachmentDetails["Binary"].Value == null) return;
            if (!attachmentDetails.ContainsKey("Name")) return;
            if (attachmentDetails["Name"].Value == null) return;

            string filePath = string.Empty;
            byte[] arrBytes = null;

            saveFileDialogGeneric.FileName = (string)attachmentDetails["Name"].Value;
            if (saveFileDialogGeneric.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialogGeneric.FileName;
                arrBytes = (byte[])attachmentDetails["Binary"].Value;
            }

            if (filePath != string.Empty && arrBytes != null)
            {
                File.WriteAllBytes(filePath, arrBytes);
                arrBytes = null;
            }
        }

        public static void strategyFileDownload<T>(object rowFileRecord, string dbFilePath, ref SaveFileDialog saveFileDialogFile) where T : new()
        {
            if (rowFileRecord == null) return;
            if (rowFileRecord.GetType() != typeof(T)) return;

            string filePath = string.Empty;
            byte[] arrBytes = null;
            SQLite806xDB db806x = null;

            if (((R_SQLite_Files)rowFileRecord).RowId.Value != null)
            {
                try
                {
                    db806x = new SQLite806xDB(dbFilePath);
                    if (db806x == null) return;
                    if (!db806x.ValidDB) return;
                }
                catch { return; }

                int rowId = Convert.ToInt32(((R_SQLite_Files)rowFileRecord).RowId.Value);

                System.Collections.IList lstFiles = null;
                lstFiles = db806x.Read<T>(rowId, null, string.Empty, string.Empty);
                if (lstFiles != null)
                {
                    if (lstFiles.Count == 1)
                    {
                        R_SQLite_Files rFile = (R_SQLite_Files)lstFiles[0];
                        saveFileDialogFile.FileName = (string)rFile.FileName.Value;
                        if (saveFileDialogFile.ShowDialog() == DialogResult.OK)
                        {
                            filePath = saveFileDialogFile.FileName;
                            arrBytes = (byte[])rFile.File.Value;
                        }
                    }
                }
                lstFiles = null;
                db806x = null;
            }

            if (filePath != string.Empty && arrBytes != null)
            {
                File.WriteAllBytes(filePath, arrBytes);
                arrBytes = null;
            }
        }

        public static bool strategyFileRemove<T>(object rowFileRecord, string dbFilePath, ref ToolStripMenuItem filesUniDb806xToolStripMenuItem, EventHandler eventHandler) where T : new()
        {
            if (rowFileRecord == null) return false;
            if (rowFileRecord.GetType() != typeof(T)) return false;

            string sQuestion = string.Empty;
            if (typeof(T) == typeof(R_806x_Strategy_Binaries)) sQuestion = "Binary will be removed from database, continue ?";
            else if (typeof(T) == typeof(R_806x_Strategy_Files)) sQuestion = "File will be removed from database, continue ?";

            if (sQuestion == string.Empty) return false;

            SQLite806xDB db806x = null;
            bool bError = false;
            bool bRemoved = false;

            R_SQLite_Files rFile = (R_SQLite_Files)rowFileRecord;
            if (rFile.RowId.Value != null)
            {
                try
                {
                    db806x = new SQLite806xDB(dbFilePath);
                    if (db806x == null) return false;
                    if (!db806x.ValidDB) return false;
                }
                catch { return false; }

                if (MessageBox.Show(sQuestion, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<T> lstRemoval = new List<T>() { (T)rowFileRecord };
                    bRemoved = db806x.Delete<T>(lstRemoval);
                    bError = !bRemoved;
                }

                db806x = null;

                if (bRemoved) refreshFilesUniDb806xMenu<T>(ref filesUniDb806xToolStripMenuItem, dbFilePath, eventHandler);
            }

            if (bError) MessageBox.Show("Removinig file from database has failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            return bRemoved;
        }

        public static string[] ElementsUniqueAddresses(R_806x_Elements_Core rElementRow)
        {
            int bankNum = (int)rElementRow.Bank.ValueConverted;
            int addressInt = (int)rElementRow.Address.ValueConverted;
            int duplicateNum = ((string)rElementRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rElementRow.UniqueAddCode.ValueConverted);

            return new string[] { Tools.UniqueAddress(bankNum, addressInt), duplicateNum == 0 ? string.Empty : Tools.DuplicateUniqueAddress(bankNum, addressInt, duplicateNum) };
        }

        public static bool exportProperties(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Properties> tSAD806xProperties = new List<R_SAD806x_Properties>();
                SQLite806xToolsSAD806x.addPropertiesRow(ref db806x, ref sadS6x, ref tSAD806xProperties, sadS6x.Properties);
                db806x.Truncate<R_SAD806x_Properties>();
                db806x.Write<R_SAD806x_Properties>(ref tSAD806xProperties);
                tSAD806xProperties = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Properties export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importProperties(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_SAD806x_Properties> tSAD806xProperties = db806x.Read<R_SAD806x_Properties>();
                if (tSAD806xProperties.Count > 0) SQLite806xToolsSAD806x.setPropertiesS6x(ref sadS6x, tSAD806xProperties[0]);
                tSAD806xProperties = null;

                lResult++;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Properties import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncProperties(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                List<R_SAD806x_Properties> tSAD806xProperties = db806x.Read<R_SAD806x_Properties>();
                
                if (tSAD806xProperties.Count == 0) SQLite806xToolsSAD806x.addPropertiesRow(ref db806x, ref sadS6x, ref tSAD806xProperties, sadS6x.Properties);
                else if ((DateTime)tSAD806xProperties[0].DateUpdated.ValueConverted > sadS6x.Properties.DateUpdated) SQLite806xToolsSAD806x.setPropertiesS6x(ref sadS6x, tSAD806xProperties[0]);
                else if (sadS6x.Properties.DateUpdated > (DateTime)tSAD806xProperties[0].DateUpdated.ValueConverted) SQLite806xToolsSAD806x.updatePropertiesRow(ref db806x, ref sadS6x, tSAD806xProperties[0], sadS6x.Properties);

                tSAD806xProperties = null;

                lResult++;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Properties import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportSyncStates(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_SyncStates> tSAD806xSyncStates = new List<R_SAD806x_SyncStates>();
                foreach (S6xSyncState s6xObject in sadS6x.Properties.SyncStates)
                {
                    SQLite806xToolsSAD806x.addSyncStateRow(ref db806x, ref sadS6x, ref tSAD806xSyncStates, s6xObject);
                }
                db806x.Truncate<R_SAD806x_SyncStates>();
                db806x.Write<R_SAD806x_SyncStates>(ref tSAD806xSyncStates);
                tSAD806xSyncStates = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite SyncStates export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static bool exportTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Tables> tSAD806xTables = new List<R_SAD806x_Def_Tables>();
                List<R_806x_Def_Tables> tTables = new List<R_806x_Def_Tables>();
                foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                {
                    SQLite806xToolsSAD806x.addTableRow(ref db806x, ref sadS6x, ref tSAD806xTables, s6xObject);
                    SQLite806xTools806x.addTableRow(ref db806x, ref sadS6x, ref tTables, s6xObject);
                }
                foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                {
                    SQLite806xToolsSAD806x.addTableRow(ref db806x, ref sadS6x, ref tSAD806xTables, s6xObject);
                    SQLite806xTools806x.addTableRow(ref db806x, ref sadS6x, ref tTables, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Tables>();
                db806x.Write<R_SAD806x_Def_Tables>(ref tSAD806xTables);
                tSAD806xTables = null;
                db806x.Truncate<R_806x_Def_Tables>();
                db806x.Write<R_806x_Def_Tables>(ref tTables);
                tTables = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Tables export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                sadS6x.slTables.Clear();
                sadS6x.slDupTables.Clear();

                List<R_806x_Def_Tables> tTables = db806x.Read<R_806x_Def_Tables>();
                lResult = tTables.Count;
                foreach (R_806x_Def_Tables rRow in tTables) SQLite806xTools806x.setTableS6x(ref sadS6x, rRow);
                tTables = null;

                List<R_SAD806x_Def_Tables> tSAD806xTables = db806x.Read<R_SAD806x_Def_Tables>();
                foreach (R_SAD806x_Def_Tables rRow in tSAD806xTables) SQLite806xToolsSAD806x.setTableS6x(ref sadS6x, rRow);
                tSAD806xTables = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Tables synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();
                
                List<R_806x_Def_Tables> t806xRows = db806x.Read<R_806x_Def_Tables>();
                foreach (R_806x_Def_Tables rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    
                }
                t806xRows = null;

                List<R_SAD806x_Def_Tables> tSAD806xRows = db806x.Read<R_SAD806x_Def_Tables>();
                foreach (R_SAD806x_Def_Tables rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Tables>();
                tSAD806xRows = new List<R_SAD806x_Def_Tables>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;
                    
                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateTableRow(ref db806x, ref sadS6x, (R_806x_Def_Tables)syncObject.SqLite806xObject, (S6xTable)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateTableRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Tables)syncObject.SqLiteSAD806xObject, (S6xTable)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Tables)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Tables)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setTableS6x(ref sadS6x, (R_806x_Def_Tables)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setTableS6x(ref sadS6x, (R_SAD806x_Def_Tables)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setTableS6x(ref sadS6x, (R_806x_Def_Tables)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setTableS6x(ref sadS6x, (R_SAD806x_Def_Tables)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addTableRow(ref db806x, ref sadS6x, ref t806xRows, (S6xTable)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addTableRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xTable)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Tables>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Tables>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Tables synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Functions> tSAD806xFunctions = new List<R_SAD806x_Def_Functions>();
                List<R_806x_Def_Functions> tFunctions = new List<R_806x_Def_Functions>();
                foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                {
                    SQLite806xToolsSAD806x.addFunctionRow(ref db806x, ref sadS6x, ref tSAD806xFunctions, s6xObject);
                    SQLite806xTools806x.addFunctionRow(ref db806x, ref sadS6x, ref tFunctions, s6xObject);
                }
                foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                {
                    SQLite806xToolsSAD806x.addFunctionRow(ref db806x, ref sadS6x, ref tSAD806xFunctions, s6xObject);
                    SQLite806xTools806x.addFunctionRow(ref db806x, ref sadS6x, ref tFunctions, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Functions>();
                db806x.Write<R_SAD806x_Def_Functions>(ref tSAD806xFunctions);
                tSAD806xFunctions = null;
                db806x.Truncate<R_806x_Def_Functions>();
                db806x.Write<R_806x_Def_Functions>(ref tFunctions);
                tFunctions = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Functions export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Functions> tFunctions = db806x.Read<R_806x_Def_Functions>();
                lResult = tFunctions.Count;
                foreach (R_806x_Def_Functions rRow in tFunctions) SQLite806xTools806x.setFunctionS6x(ref sadS6x, rRow);
                tFunctions = null;

                List<R_SAD806x_Def_Functions> tSAD806xFunctions = db806x.Read<R_SAD806x_Def_Functions>();
                foreach (R_SAD806x_Def_Functions rRow in tSAD806xFunctions) SQLite806xToolsSAD806x.setFunctionS6x(ref sadS6x, rRow);
                tSAD806xFunctions = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Functions import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Functions> t806xRows = db806x.Read<R_806x_Def_Functions>();
                foreach (R_806x_Def_Functions rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Functions> tSAD806xRows = db806x.Read<R_SAD806x_Def_Functions>();
                foreach (R_SAD806x_Def_Functions rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Functions>();
                tSAD806xRows = new List<R_SAD806x_Def_Functions>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateFunctionRow(ref db806x, ref sadS6x, (R_806x_Def_Functions)syncObject.SqLite806xObject, (S6xFunction)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateFunctionRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Functions)syncObject.SqLiteSAD806xObject, (S6xFunction)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Functions)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Functions)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setFunctionS6x(ref sadS6x, (R_806x_Def_Functions)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setFunctionS6x(ref sadS6x, (R_SAD806x_Def_Functions)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setFunctionS6x(ref sadS6x, (R_806x_Def_Functions)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setFunctionS6x(ref sadS6x, (R_SAD806x_Def_Functions)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addFunctionRow(ref db806x, ref sadS6x, ref t806xRows, (S6xFunction)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addFunctionRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xFunction)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Functions>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Functions>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Functions synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Scalars> tSAD806xScalars = new List<R_SAD806x_Def_Scalars>();
                List<R_SAD806x_Def_ScalarsBitFlags> tSAD806xScalarsBF = new List<R_SAD806x_Def_ScalarsBitFlags>();
                List<R_806x_Def_Scalars> tScalars = new List<R_806x_Def_Scalars>();
                List<R_806x_Def_ScalarsBitFlags> tScalarsBF = new List<R_806x_Def_ScalarsBitFlags>();
                foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                {
                    SQLite806xToolsSAD806x.addScalarRow(ref db806x, ref sadS6x, ref tSAD806xScalars, s6xObject);
                    SQLite806xToolsSAD806x.addScalarBitFlagRows(ref db806x, ref sadS6x, ref tSAD806xScalarsBF, s6xObject);
                    SQLite806xTools806x.addScalarRow(ref db806x, ref sadS6x, ref tScalars, s6xObject);
                    SQLite806xTools806x.addScalarBitFlagRows(ref db806x, ref sadS6x, ref tScalarsBF, s6xObject);
                }
                foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                {
                    SQLite806xToolsSAD806x.addScalarRow(ref db806x, ref sadS6x, ref tSAD806xScalars, s6xObject);
                    SQLite806xToolsSAD806x.addScalarBitFlagRows(ref db806x, ref sadS6x, ref tSAD806xScalarsBF, s6xObject);
                    SQLite806xTools806x.addScalarRow(ref db806x, ref sadS6x, ref tScalars, s6xObject);
                    SQLite806xTools806x.addScalarBitFlagRows(ref db806x, ref sadS6x, ref tScalarsBF, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Scalars>();
                db806x.Write<R_SAD806x_Def_Scalars>(ref tSAD806xScalars);
                tSAD806xScalars = null;
                db806x.Truncate<R_SAD806x_Def_ScalarsBitFlags>();
                db806x.Write<R_SAD806x_Def_ScalarsBitFlags>(ref tSAD806xScalarsBF);
                tSAD806xScalarsBF = null;
                db806x.Truncate<R_806x_Def_Scalars>();
                db806x.Write<R_806x_Def_Scalars>(ref tScalars);
                tScalars = null;
                db806x.Truncate<R_806x_Def_ScalarsBitFlags>();
                db806x.Write<R_806x_Def_ScalarsBitFlags>(ref tScalarsBF);
                tScalarsBF = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Scalars export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Scalars> tScalars = db806x.Read<R_806x_Def_Scalars>();
                lResult = tScalars.Count;
                foreach (R_806x_Def_Scalars rRow in tScalars) SQLite806xTools806x.setScalarS6x(ref sadS6x, rRow);
                tScalars = null;
                List<R_806x_Def_ScalarsBitFlags> tScalarsBF = db806x.Read<R_806x_Def_ScalarsBitFlags>();
                foreach (R_806x_Def_ScalarsBitFlags rRow in tScalarsBF) SQLite806xTools806x.setScalarBitFlagS6x(ref sadS6x, rRow);
                tScalarsBF = null;

                List<R_SAD806x_Def_Scalars> tSAD806xScalars = db806x.Read<R_SAD806x_Def_Scalars>();
                foreach (R_SAD806x_Def_Scalars rRow in tSAD806xScalars) SQLite806xToolsSAD806x.setScalarS6x(ref sadS6x, rRow);
                tSAD806xScalars = null;
                List<R_SAD806x_Def_ScalarsBitFlags> tSAD806xScalarsBF = db806x.Read<R_SAD806x_Def_ScalarsBitFlags>();
                foreach (R_SAD806x_Def_ScalarsBitFlags rRow in tSAD806xScalarsBF) SQLite806xToolsSAD806x.setScalarBitFlagS6x(ref sadS6x, rRow);
                tSAD806xScalarsBF = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Scalars import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Scalars> t806xRows = db806x.Read<R_806x_Def_Scalars>();
                foreach (R_806x_Def_Scalars rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Scalars> tSAD806xRows = db806x.Read<R_SAD806x_Def_Scalars>();
                foreach (R_SAD806x_Def_Scalars rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Scalars>();
                tSAD806xRows = new List<R_SAD806x_Def_Scalars>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateScalarRow(ref db806x, ref sadS6x, (R_806x_Def_Scalars)syncObject.SqLite806xObject, (S6xScalar)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateScalarRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Scalars)syncObject.SqLiteSAD806xObject, (S6xScalar)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Scalars)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Scalars)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setScalarS6x(ref sadS6x, (R_806x_Def_Scalars)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setScalarS6x(ref sadS6x, (R_SAD806x_Def_Scalars)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setScalarS6x(ref sadS6x, (R_806x_Def_Scalars)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setScalarS6x(ref sadS6x, (R_SAD806x_Def_Scalars)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addScalarRow(ref db806x, ref sadS6x, ref t806xRows, (S6xScalar)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addScalarRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xScalar)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Scalars>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Scalars>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Scalars synchronization part has failed.\r\n" + ex.Message);
            }

            syncScalarsBitFlags(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncScalarsBitFlags(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_ScalarsBitFlags> t806xRows = db806x.Read<R_806x_Def_ScalarsBitFlags>();
                foreach (R_806x_Def_ScalarsBitFlags rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_ScalarsBitFlags> tSAD806xRows = db806x.Read<R_SAD806x_Def_ScalarsBitFlags>();
                foreach (R_SAD806x_Def_ScalarsBitFlags rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xScalar s6xParentObject in sadS6x.slScalars.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (!s6xParentObject.isBitFlags) continue;

                    foreach (S6xBitFlag s6xObject in s6xParentObject.BitFlags)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                foreach (S6xScalar s6xParentObject in sadS6x.slDupScalars.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (!s6xParentObject.isBitFlags) continue;

                    foreach (S6xBitFlag s6xObject in s6xParentObject.BitFlags)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_ScalarsBitFlags>();
                tSAD806xRows = new List<R_SAD806x_Def_ScalarsBitFlags>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateScalarBitFlagRow(ref db806x, ref sadS6x, (R_806x_Def_ScalarsBitFlags)syncObject.SqLite806xObject, (S6xScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateScalarBitFlagRow(ref db806x, ref sadS6x, (R_SAD806x_Def_ScalarsBitFlags)syncObject.SqLiteSAD806xObject, (S6xScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_ScalarsBitFlags)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_ScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setScalarBitFlagS6x(ref sadS6x, (R_806x_Def_ScalarsBitFlags)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setScalarBitFlagS6x(ref sadS6x, (R_SAD806x_Def_ScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setScalarBitFlagS6x(ref sadS6x, (R_806x_Def_ScalarsBitFlags)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setScalarBitFlagS6x(ref sadS6x, (R_SAD806x_Def_ScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addScalarBitFlagRow(ref db806x, ref sadS6x, ref t806xRows, (S6xScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addScalarBitFlagRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_ScalarsBitFlags>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_ScalarsBitFlags>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Scalars Bit Flags synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Structures> tSAD806xStructures = new List<R_SAD806x_Def_Structures>();
                List<R_806x_Def_Structures> tStructures = new List<R_806x_Def_Structures>();
                foreach (S6xStructure s6xObject in sadS6x.slStructures.Values) SQLite806xToolsSAD806x.addStructureRow(ref db806x, ref sadS6x, ref tSAD806xStructures, s6xObject);
                foreach (S6xStructure s6xObject in sadS6x.slDupStructures.Values) SQLite806xToolsSAD806x.addStructureRow(ref db806x, ref sadS6x, ref tSAD806xStructures, s6xObject);
                foreach (S6xStructure s6xObject in sadS6x.slStructures.Values) SQLite806xTools806x.addStructureRow(ref db806x, ref sadS6x, ref tStructures, s6xObject);
                foreach (S6xStructure s6xObject in sadS6x.slDupStructures.Values) SQLite806xTools806x.addStructureRow(ref db806x, ref sadS6x, ref tStructures, s6xObject);
                db806x.Truncate<R_SAD806x_Def_Structures>();
                db806x.Write<R_SAD806x_Def_Structures>(ref tSAD806xStructures);
                tSAD806xStructures = null;
                db806x.Truncate<R_806x_Def_Structures>();
                db806x.Write<R_806x_Def_Structures>(ref tStructures);
                tStructures = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Structures export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Structures> tStructures = db806x.Read<R_806x_Def_Structures>();
                lResult = tStructures.Count;
                foreach (R_806x_Def_Structures rRow in tStructures) SQLite806xTools806x.setStructureS6x(ref sadS6x, rRow);
                tStructures = null;

                List<R_SAD806x_Def_Structures> tSAD806xStructures = db806x.Read<R_SAD806x_Def_Structures>();
                foreach (R_SAD806x_Def_Structures rRow in tSAD806xStructures) SQLite806xToolsSAD806x.setStructureS6x(ref sadS6x, rRow);
                tSAD806xStructures = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Structures import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Structures> t806xRows = db806x.Read<R_806x_Def_Structures>();
                foreach (R_806x_Def_Structures rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Structures> tSAD806xRows = db806x.Read<R_SAD806x_Def_Structures>();
                foreach (R_SAD806x_Def_Structures rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                foreach (S6xStructure s6xObject in sadS6x.slDupStructures.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Structures>();
                tSAD806xRows = new List<R_SAD806x_Def_Structures>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateStructureRow(ref db806x, ref sadS6x, (R_806x_Def_Structures)syncObject.SqLite806xObject, (S6xStructure)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateStructureRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Structures)syncObject.SqLiteSAD806xObject, (S6xStructure)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Structures)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Structures)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setStructureS6x(ref sadS6x, (R_806x_Def_Structures)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setStructureS6x(ref sadS6x, (R_SAD806x_Def_Structures)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setStructureS6x(ref sadS6x, (R_806x_Def_Structures)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setStructureS6x(ref sadS6x, (R_SAD806x_Def_Structures)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addStructureRow(ref db806x, ref sadS6x, ref t806xRows, (S6xStructure)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addStructureRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xStructure)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Structures>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Structures>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Structures synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportRoutines(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Routines> tSAD806xRoutines = new List<R_SAD806x_Def_Routines>();
                List<R_SAD806x_Def_RoutinesInputArgs> tSAD806xRoutinesInputArgs = new List<R_SAD806x_Def_RoutinesInputArgs>();
                List<R_SAD806x_Def_RoutinesInputScalars> tSAD806xRoutinesInputScalars = new List<R_SAD806x_Def_RoutinesInputScalars>();
                List<R_SAD806x_Def_RoutinesInputFunctions> tSAD806xRoutinesInputFunctions = new List<R_SAD806x_Def_RoutinesInputFunctions>();
                List<R_SAD806x_Def_RoutinesInputTables> tSAD806xRoutinesInputTables = new List<R_SAD806x_Def_RoutinesInputTables>();
                List<R_SAD806x_Def_RoutinesInputStructures> tSAD806xRoutinesInputStructures = new List<R_SAD806x_Def_RoutinesInputStructures>();

                List<R_806x_Def_Routines> tRoutines = new List<R_806x_Def_Routines>();
                List<R_806x_Def_RoutinesArgs> tRoutinesArgs = new List<R_806x_Def_RoutinesArgs>();

                foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
                {
                    SQLite806xToolsSAD806x.addRoutineRow(ref db806x, ref sadS6x, ref tSAD806xRoutines, s6xObject);
                    SQLite806xToolsSAD806x.addRoutineInputArgRows(ref db806x, ref sadS6x, ref tSAD806xRoutinesInputArgs, s6xObject);
                    SQLite806xToolsSAD806x.addRoutineInputScalarRows(ref db806x, ref sadS6x, ref tSAD806xRoutinesInputScalars, s6xObject);
                    SQLite806xToolsSAD806x.addRoutineInputFunctionRows(ref db806x, ref sadS6x, ref tSAD806xRoutinesInputFunctions, s6xObject);
                    SQLite806xToolsSAD806x.addRoutineInputTableRows(ref db806x, ref sadS6x, ref tSAD806xRoutinesInputTables, s6xObject);
                    SQLite806xToolsSAD806x.addRoutineInputStructureRows(ref db806x, ref sadS6x, ref tSAD806xRoutinesInputStructures, s6xObject);

                    SQLite806xTools806x.addRoutineRow(ref db806x, ref sadS6x, ref tRoutines, s6xObject);
                    SQLite806xTools806x.addRoutineInputArgRows(ref db806x, ref sadS6x, ref tRoutinesArgs, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Routines>();
                db806x.Write<R_SAD806x_Def_Routines>(ref tSAD806xRoutines);
                tSAD806xRoutines = null;
                db806x.Truncate<R_SAD806x_Def_RoutinesInputArgs>();
                db806x.Write<R_SAD806x_Def_RoutinesInputArgs>(ref tSAD806xRoutinesInputArgs);
                tSAD806xRoutinesInputArgs = null;
                db806x.Truncate<R_SAD806x_Def_RoutinesInputScalars>();
                db806x.Write<R_SAD806x_Def_RoutinesInputScalars>(ref tSAD806xRoutinesInputScalars);
                tSAD806xRoutinesInputScalars = null;
                db806x.Truncate<R_SAD806x_Def_RoutinesInputFunctions>();
                db806x.Write<R_SAD806x_Def_RoutinesInputFunctions>(ref tSAD806xRoutinesInputFunctions);
                tSAD806xRoutinesInputFunctions = null;
                db806x.Truncate<R_SAD806x_Def_RoutinesInputTables>();
                db806x.Write<R_SAD806x_Def_RoutinesInputTables>(ref tSAD806xRoutinesInputTables);
                tSAD806xRoutinesInputTables = null;
                db806x.Truncate<R_SAD806x_Def_RoutinesInputStructures>();
                db806x.Write<R_SAD806x_Def_RoutinesInputStructures>(ref tSAD806xRoutinesInputStructures);
                tSAD806xRoutinesInputStructures = null;

                db806x.Truncate<R_806x_Def_Routines>();
                db806x.Write<R_806x_Def_Routines>(ref tRoutines);
                tRoutines = null;
                db806x.Truncate<R_806x_Def_RoutinesArgs>();
                db806x.Write<R_806x_Def_RoutinesArgs>(ref tRoutinesArgs);
                tRoutinesArgs = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importRoutines(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Routines> tRoutines = db806x.Read<R_806x_Def_Routines>();
                lResult = tRoutines.Count;
                foreach (R_806x_Def_Routines rRow in tRoutines) SQLite806xTools806x.setRoutineS6x(ref sadS6x, rRow);
                tRoutines = null;
                List<R_806x_Def_RoutinesArgs> tRoutinesArgs = db806x.Read<R_806x_Def_RoutinesArgs>();
                foreach (R_806x_Def_RoutinesArgs rRow in tRoutinesArgs) SQLite806xTools806x.setRoutineInputArgS6x(ref sadS6x, rRow);
                tRoutinesArgs = null;

                List<R_SAD806x_Def_Routines> tSAD806xRoutines = db806x.Read<R_SAD806x_Def_Routines>();
                foreach (R_SAD806x_Def_Routines rRow in tSAD806xRoutines) SQLite806xToolsSAD806x.setRoutineS6x(ref sadS6x, rRow);
                tSAD806xRoutines = null;
                List<R_SAD806x_Def_RoutinesInputArgs> tSAD806xRoutinesInputArgs = db806x.Read<R_SAD806x_Def_RoutinesInputArgs>();
                foreach (R_SAD806x_Def_RoutinesInputArgs rRow in tSAD806xRoutinesInputArgs) SQLite806xToolsSAD806x.setRoutineInputArgS6x(ref sadS6x, rRow);
                tSAD806xRoutinesInputArgs = null;
                List<R_SAD806x_Def_RoutinesInputScalars> tSAD806xRoutinesInputScalars = db806x.Read<R_SAD806x_Def_RoutinesInputScalars>();
                foreach (R_SAD806x_Def_RoutinesInputScalars rRow in tSAD806xRoutinesInputScalars) SQLite806xToolsSAD806x.setRoutineInputScalarS6x(ref sadS6x, rRow);
                tSAD806xRoutinesInputScalars = null;
                List<R_SAD806x_Def_RoutinesInputFunctions> tSAD806xRoutinesInputFunctions = db806x.Read<R_SAD806x_Def_RoutinesInputFunctions>();
                foreach (R_SAD806x_Def_RoutinesInputFunctions rRow in tSAD806xRoutinesInputFunctions) SQLite806xToolsSAD806x.setRoutineInputFunctionS6x(ref sadS6x, rRow);
                tSAD806xRoutinesInputFunctions = null;
                List<R_SAD806x_Def_RoutinesInputTables> tSAD806xRoutinesInputTables = db806x.Read<R_SAD806x_Def_RoutinesInputTables>();
                foreach (R_SAD806x_Def_RoutinesInputTables rRow in tSAD806xRoutinesInputTables) SQLite806xToolsSAD806x.setRoutineInputTableS6x(ref sadS6x, rRow);
                tSAD806xRoutinesInputTables = null;
                List<R_SAD806x_Def_RoutinesInputStructures> tSAD806xRoutinesInputStructures = db806x.Read<R_SAD806x_Def_RoutinesInputStructures>();
                foreach (R_SAD806x_Def_RoutinesInputStructures rRow in tSAD806xRoutinesInputStructures) SQLite806xToolsSAD806x.setRoutineInputStructureS6x(ref sadS6x, rRow);
                tSAD806xRoutinesInputStructures = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRoutines(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Routines> t806xRows = db806x.Read<R_806x_Def_Routines>();
                foreach (R_806x_Def_Routines rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Routines> tSAD806xRows = db806x.Read<R_SAD806x_Def_Routines>();
                foreach (R_SAD806x_Def_Routines rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Routines>();
                tSAD806xRows = new List<R_SAD806x_Def_Routines>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateRoutineRow(ref db806x, ref sadS6x, (R_806x_Def_Routines)syncObject.SqLite806xObject, (S6xRoutine)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateRoutineRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Routines)syncObject.SqLiteSAD806xObject, (S6xRoutine)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Routines)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Routines)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setRoutineS6x(ref sadS6x, (R_806x_Def_Routines)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setRoutineS6x(ref sadS6x, (R_SAD806x_Def_Routines)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setRoutineS6x(ref sadS6x, (R_806x_Def_Routines)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRoutineS6x(ref sadS6x, (R_SAD806x_Def_Routines)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addRoutineRow(ref db806x, ref sadS6x, ref t806xRows, (S6xRoutine)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRoutineRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRoutine)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Routines>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Routines>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines synchronization part has failed.\r\n" + ex.Message);
            }

            syncRoutinesInputArgs(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncRoutinesInputScalars(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncRoutinesInputFunctions(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncRoutinesInputTables(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncRoutinesInputStructures(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncRoutinesInputArgs(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_RoutinesArgs> t806xRows = db806x.Read<R_806x_Def_RoutinesArgs>();
                foreach (R_806x_Def_RoutinesArgs rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_RoutinesInputArgs> tSAD806xRows = db806x.Read<R_SAD806x_Def_RoutinesInputArgs>();
                foreach (R_SAD806x_Def_RoutinesInputArgs rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRoutine s6xParentObject in sadS6x.slRoutines.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (s6xParentObject.InputArguments == null) continue;
                    if (s6xParentObject.InputArguments.Length == 0) continue;

                    foreach (S6xRoutineInputArgument s6xObject in s6xParentObject.InputArguments)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_RoutinesArgs>();
                tSAD806xRows = new List<R_SAD806x_Def_RoutinesInputArgs>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateRoutineInputArgRow(ref db806x, ref sadS6x, (R_806x_Def_RoutinesArgs)syncObject.SqLite806xObject, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputArgument)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateRoutineInputArgRow(ref db806x, ref sadS6x, (R_SAD806x_Def_RoutinesInputArgs)syncObject.SqLiteSAD806xObject, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputArgument)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_RoutinesArgs)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_RoutinesInputArgs)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setRoutineInputArgS6x(ref sadS6x, (R_806x_Def_RoutinesArgs)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setRoutineInputArgS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputArgs)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setRoutineInputArgS6x(ref sadS6x, (R_806x_Def_RoutinesArgs)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRoutineInputArgS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputArgs)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addRoutineInputArgRow(ref db806x, ref sadS6x, ref t806xRows, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputArgument)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRoutineInputArgRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputArgument)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_RoutinesArgs>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_RoutinesInputArgs>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Arguments synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRoutinesInputScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_Def_RoutinesInputScalars> tSAD806xRows = db806x.Read<R_SAD806x_Def_RoutinesInputScalars>();
                foreach (R_SAD806x_Def_RoutinesInputScalars rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRoutine s6xParentObject in sadS6x.slRoutines.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (s6xParentObject.InputScalars == null) continue;
                    if (s6xParentObject.InputScalars.Length == 0) continue;

                    foreach (S6xRoutineInputScalar s6xObject in s6xParentObject.InputScalars)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_Def_RoutinesInputScalars>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateRoutineInputScalarRow(ref db806x, ref sadS6x, (R_SAD806x_Def_RoutinesInputScalars)syncObject.SqLiteSAD806xObject, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputScalar)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_RoutinesInputScalars)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setRoutineInputScalarS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputScalars)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRoutineInputScalarS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputScalars)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRoutineInputScalarRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputScalar)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_Def_RoutinesInputScalars>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Input Scalars synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRoutinesInputFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_Def_RoutinesInputFunctions> tSAD806xRows = db806x.Read<R_SAD806x_Def_RoutinesInputFunctions>();
                foreach (R_SAD806x_Def_RoutinesInputFunctions rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRoutine s6xParentObject in sadS6x.slRoutines.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (s6xParentObject.InputFunctions == null) continue;
                    if (s6xParentObject.InputFunctions.Length == 0) continue;

                    foreach (S6xRoutineInputFunction s6xObject in s6xParentObject.InputFunctions)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_Def_RoutinesInputFunctions>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateRoutineInputFunctionRow(ref db806x, ref sadS6x, (R_SAD806x_Def_RoutinesInputFunctions)syncObject.SqLiteSAD806xObject, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputFunction)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_RoutinesInputFunctions)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setRoutineInputFunctionS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputFunctions)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRoutineInputFunctionS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputFunctions)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRoutineInputFunctionRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputFunction)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_Def_RoutinesInputFunctions>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Input Functions synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRoutinesInputTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_Def_RoutinesInputTables> tSAD806xRows = db806x.Read<R_SAD806x_Def_RoutinesInputTables>();
                foreach (R_SAD806x_Def_RoutinesInputTables rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRoutine s6xParentObject in sadS6x.slRoutines.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (s6xParentObject.InputTables == null) continue;
                    if (s6xParentObject.InputTables.Length == 0) continue;

                    foreach (S6xRoutineInputTable s6xObject in s6xParentObject.InputTables)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_Def_RoutinesInputTables>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateRoutineInputTableRow(ref db806x, ref sadS6x, (R_SAD806x_Def_RoutinesInputTables)syncObject.SqLiteSAD806xObject, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputTable)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_RoutinesInputTables)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setRoutineInputTableS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputTables)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRoutineInputTableS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputTables)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRoutineInputTableRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputTable)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_Def_RoutinesInputTables>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Input Tables synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRoutinesInputStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_Def_RoutinesInputStructures> tSAD806xRows = db806x.Read<R_SAD806x_Def_RoutinesInputStructures>();
                foreach (R_SAD806x_Def_RoutinesInputStructures rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRoutine s6xParentObject in sadS6x.slRoutines.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (s6xParentObject.InputStructures == null) continue;
                    if (s6xParentObject.InputStructures.Length == 0) continue;

                    foreach (S6xRoutineInputStructure s6xObject in s6xParentObject.InputStructures)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_Def_RoutinesInputStructures>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateRoutineInputStructureRow(ref db806x, ref sadS6x, (R_SAD806x_Def_RoutinesInputStructures)syncObject.SqLiteSAD806xObject, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputStructure)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_RoutinesInputStructures)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setRoutineInputStructureS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputStructures)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRoutineInputStructureS6x(ref sadS6x, (R_SAD806x_Def_RoutinesInputStructures)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRoutineInputStructureRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRoutine)syncObject.S6xParentObject, (S6xRoutineInputStructure)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_Def_RoutinesInputStructures>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Input Structures synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportOperations(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Operations> tSAD806xOperations = new List<R_SAD806x_Def_Operations>();
                List<R_806x_Def_Operations> tOperations = new List<R_806x_Def_Operations>();
                foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
                {
                    SQLite806xToolsSAD806x.addOperationRow(ref db806x, ref sadS6x, ref tSAD806xOperations, s6xObject);
                    SQLite806xTools806x.addOperationRow(ref db806x, ref sadS6x, ref tOperations, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Operations>();
                db806x.Write<R_SAD806x_Def_Operations>(ref tSAD806xOperations);
                tSAD806xOperations = null;
                db806x.Truncate<R_806x_Def_Operations>();
                db806x.Write<R_806x_Def_Operations>(ref tOperations);
                tOperations = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Operations export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importOperations(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Operations> tOperations = db806x.Read<R_806x_Def_Operations>();
                lResult = tOperations.Count;
                foreach (R_806x_Def_Operations rRow in tOperations) SQLite806xTools806x.setOperationS6x(ref sadS6x, rRow);
                tOperations = null;

                List<R_SAD806x_Def_Operations> tSAD806xOperations = db806x.Read<R_SAD806x_Def_Operations>();
                foreach (R_SAD806x_Def_Operations rRow in tSAD806xOperations) SQLite806xToolsSAD806x.setOperationS6x(ref sadS6x, rRow);
                tSAD806xOperations = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Operations import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncOperations(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Operations> t806xRows = db806x.Read<R_806x_Def_Operations>();
                foreach (R_806x_Def_Operations rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Operations> tSAD806xRows = db806x.Read<R_SAD806x_Def_Operations>();
                foreach (R_SAD806x_Def_Operations rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Operations>();
                tSAD806xRows = new List<R_SAD806x_Def_Operations>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateOperationRow(ref db806x, ref sadS6x, (R_806x_Def_Operations)syncObject.SqLite806xObject, (S6xOperation)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateOperationRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Operations)syncObject.SqLiteSAD806xObject, (S6xOperation)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Operations)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Operations)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setOperationS6x(ref sadS6x, (R_806x_Def_Operations)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setOperationS6x(ref sadS6x, (R_SAD806x_Def_Operations)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setOperationS6x(ref sadS6x, (R_806x_Def_Operations)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setOperationS6x(ref sadS6x, (R_SAD806x_Def_Operations)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addOperationRow(ref db806x, ref sadS6x, ref t806xRows, (S6xOperation)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addOperationRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xOperation)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Operations>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Operations>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Operations synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }
        
        public static bool exportOthers(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Others> tSAD806xOthers = new List<R_SAD806x_Def_Others>();
                List<R_806x_Def_Others> tOthers = new List<R_806x_Def_Others>();
                foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
                {
                    SQLite806xToolsSAD806x.addOtherRow(ref db806x, ref sadS6x, ref tSAD806xOthers, s6xObject);
                    SQLite806xTools806x.addOtherRow(ref db806x, ref sadS6x, ref tOthers, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Others>();
                db806x.Write<R_SAD806x_Def_Others>(ref tSAD806xOthers);
                tSAD806xOthers = null;
                db806x.Truncate<R_806x_Def_Others>();
                db806x.Write<R_806x_Def_Others>(ref tOthers);
                tOthers = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Other addresses export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importOthers(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Others> tOthers = db806x.Read<R_806x_Def_Others>();
                lResult = tOthers.Count;
                foreach (R_806x_Def_Others rRow in tOthers) SQLite806xTools806x.setOtherS6x(ref sadS6x, rRow);
                tOthers = null;

                List<R_SAD806x_Def_Others> tSAD806xOthers = db806x.Read<R_SAD806x_Def_Others>();
                foreach (R_SAD806x_Def_Others rRow in tSAD806xOthers) SQLite806xToolsSAD806x.setOtherS6x(ref sadS6x, rRow);
                tSAD806xOthers = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Other addresses import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncOthers(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Others> t806xRows = db806x.Read<R_806x_Def_Others>();
                foreach (R_806x_Def_Others rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Others> tSAD806xRows = db806x.Read<R_SAD806x_Def_Others>();
                foreach (R_SAD806x_Def_Others rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Others>();
                tSAD806xRows = new List<R_SAD806x_Def_Others>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateOtherRow(ref db806x, ref sadS6x, (R_806x_Def_Others)syncObject.SqLite806xObject, (S6xOtherAddress)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateOtherRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Others)syncObject.SqLiteSAD806xObject, (S6xOtherAddress)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Others)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Others)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setOtherS6x(ref sadS6x, (R_806x_Def_Others)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setOtherS6x(ref sadS6x, (R_SAD806x_Def_Others)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setOtherS6x(ref sadS6x, (R_806x_Def_Others)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setOtherS6x(ref sadS6x, (R_SAD806x_Def_Others)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addOtherRow(ref db806x, ref sadS6x, ref t806xRows, (S6xOtherAddress)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addOtherRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xOtherAddress)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Others>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Others>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Other Addresses synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static bool exportRegisters(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_Def_Registers> tSAD806xRegisters = new List<R_SAD806x_Def_Registers>();
                List<R_SAD806x_Def_RegistersBitFlags> tSAD806xRegistersBF = new List<R_SAD806x_Def_RegistersBitFlags>();
                List<R_806x_Def_Registers> tRegisters = new List<R_806x_Def_Registers>();
                List<R_806x_Def_RegistersBitFlags> tRegistersBF = new List<R_806x_Def_RegistersBitFlags>();
                foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
                {
                    SQLite806xToolsSAD806x.addRegisterRow(ref db806x, ref sadS6x, ref tSAD806xRegisters, s6xObject);
                    SQLite806xToolsSAD806x.addRegisterBitFlagRows(ref db806x, ref sadS6x, ref tSAD806xRegistersBF, s6xObject);
                    SQLite806xTools806x.addRegisterRow(ref db806x, ref sadS6x, ref tRegisters, s6xObject);
                    SQLite806xTools806x.addRegisterBitFlagRows(ref db806x, ref sadS6x, ref tRegistersBF, s6xObject);
                }
                db806x.Truncate<R_SAD806x_Def_Registers>();
                db806x.Write<R_SAD806x_Def_Registers>(ref tSAD806xRegisters);
                tSAD806xRegisters = null;
                db806x.Truncate<R_SAD806x_Def_RegistersBitFlags>();
                db806x.Write<R_SAD806x_Def_RegistersBitFlags>(ref tSAD806xRegistersBF);
                tSAD806xRegistersBF = null;
                db806x.Truncate<R_806x_Def_Registers>();
                db806x.Write<R_806x_Def_Registers>(ref tRegisters);
                tRegisters = null;
                db806x.Truncate<R_806x_Def_RegistersBitFlags>();
                db806x.Write<R_806x_Def_RegistersBitFlags>(ref tRegistersBF);
                tRegistersBF = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Registers export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importRegisters(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_806x_Def_Registers> tRegisters = db806x.Read<R_806x_Def_Registers>();
                lResult = tRegisters.Count;
                foreach (R_806x_Def_Registers rRow in tRegisters) SQLite806xTools806x.setRegisterS6x(ref sadS6x, rRow);
                tRegisters = null;
                List<R_806x_Def_RegistersBitFlags> tRegistersBF = db806x.Read<R_806x_Def_RegistersBitFlags>();
                foreach (R_806x_Def_RegistersBitFlags rRow in tRegistersBF) SQLite806xTools806x.setRegisterBitFlagS6x(ref sadS6x, rRow);
                tRegistersBF = null;

                List<R_SAD806x_Def_Registers> tSAD806xRegisters = db806x.Read<R_SAD806x_Def_Registers>();
                foreach (R_SAD806x_Def_Registers rRow in tSAD806xRegisters) SQLite806xToolsSAD806x.setRegisterS6x(ref sadS6x, rRow);
                tSAD806xRegisters = null;
                List<R_SAD806x_Def_RegistersBitFlags> tSAD806xRegistersBF = db806x.Read<R_SAD806x_Def_RegistersBitFlags>();
                foreach (R_SAD806x_Def_RegistersBitFlags rRow in tSAD806xRegistersBF) SQLite806xToolsSAD806x.setRegisterBitFlagS6x(ref sadS6x, rRow);
                tSAD806xRegistersBF = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Registers import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRegisters(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_Registers> t806xRows = db806x.Read<R_806x_Def_Registers>();
                foreach (R_806x_Def_Registers rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_Registers> tSAD806xRows = db806x.Read<R_SAD806x_Def_Registers>();
                foreach (R_SAD806x_Def_Registers rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
                {
                    if (!s6xObject.Store) continue;
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_Registers>();
                tSAD806xRows = new List<R_SAD806x_Def_Registers>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateRegisterRow(ref db806x, ref sadS6x, (R_806x_Def_Registers)syncObject.SqLite806xObject, (S6xRegister)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateRegisterRow(ref db806x, ref sadS6x, (R_SAD806x_Def_Registers)syncObject.SqLiteSAD806xObject, (S6xRegister)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_Registers)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_Registers)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setRegisterS6x(ref sadS6x, (R_806x_Def_Registers)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setRegisterS6x(ref sadS6x, (R_SAD806x_Def_Registers)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setRegisterS6x(ref sadS6x, (R_806x_Def_Registers)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRegisterS6x(ref sadS6x, (R_SAD806x_Def_Registers)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addRegisterRow(ref db806x, ref sadS6x, ref t806xRows, (S6xRegister)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRegisterRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRegister)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_Registers>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_Registers>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Registers synchronization part has failed.\r\n" + ex.Message);
            }

            syncRegistersBitFlags(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncRegistersBitFlags(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_806x_Def_RegistersBitFlags> t806xRows = db806x.Read<R_806x_Def_RegistersBitFlags>();
                foreach (R_806x_Def_RegistersBitFlags rRow in t806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, true);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLite806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);

                }
                t806xRows = null;

                List<R_SAD806x_Def_RegistersBitFlags> tSAD806xRows = db806x.Read<R_SAD806x_Def_RegistersBitFlags>();
                foreach (R_SAD806x_Def_RegistersBitFlags rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xRegister s6xParentObject in sadS6x.slRegisters.Values)
                {
                    if (!s6xParentObject.Store) continue;
                    if (!s6xParentObject.isBitFlags) continue;

                    foreach (S6xBitFlag s6xObject in s6xParentObject.BitFlags)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                t806xRows = new List<R_806x_Def_RegistersBitFlags>();
                tSAD806xRows = new List<R_SAD806x_Def_RegistersBitFlags>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xTools806x.updateRegisterBitFlagRow(ref db806x, ref sadS6x, (R_806x_Def_RegistersBitFlags)syncObject.SqLite806xObject, (S6xRegister)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                            SQLite806xToolsSAD806x.updateRegisterBitFlagRow(ref db806x, ref sadS6x, (R_SAD806x_Def_RegistersBitFlags)syncObject.SqLiteSAD806xObject, (S6xRegister)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                            t806xRows.Add((R_806x_Def_RegistersBitFlags)syncObject.SqLite806xObject);
                            tSAD806xRows.Add((R_SAD806x_Def_RegistersBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xTools806x.setRegisterBitFlagS6x(ref sadS6x, (R_806x_Def_RegistersBitFlags)syncObject.SqLite806xObject);
                            SQLite806xToolsSAD806x.setRegisterBitFlagS6x(ref sadS6x, (R_SAD806x_Def_RegistersBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xTools806x.setRegisterBitFlagS6x(ref sadS6x, (R_806x_Def_RegistersBitFlags)syncObject.SqLite806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setRegisterBitFlagS6x(ref sadS6x, (R_SAD806x_Def_RegistersBitFlags)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLite806xObject = SQLite806xTools806x.addRegisterBitFlagRow(ref db806x, ref sadS6x, ref t806xRows, (S6xRegister)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addRegisterBitFlagRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xRegister)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_806x_Def_RegistersBitFlags>(ref t806xRows);
                db806x.Write<R_SAD806x_Def_RegistersBitFlags>(ref tSAD806xRows);

                t806xRows = null;
                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Registers Bit Flags synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }
        
        public static bool exportSignatures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            try
            {
                List<R_SAD806x_SignaturesRoutines> tSAD806xSignaturesRoutines = new List<R_SAD806x_SignaturesRoutines>();
                List<R_SAD806x_SignaturesRoutinesInputArgs> tSAD806xSignaturesRoutinesInputArgs = new List<R_SAD806x_SignaturesRoutinesInputArgs>();
                List<R_SAD806x_SignaturesRoutinesInputScalars> tSAD806xSignaturesRoutinesInputScalars = new List<R_SAD806x_SignaturesRoutinesInputScalars>();
                List<R_SAD806x_SignaturesRoutinesInputFunctions> tSAD806xSignaturesRoutinesInputFunctions = new List<R_SAD806x_SignaturesRoutinesInputFunctions>();
                List<R_SAD806x_SignaturesRoutinesInputTables> tSAD806xSignaturesRoutinesInputTables = new List<R_SAD806x_SignaturesRoutinesInputTables>();
                List<R_SAD806x_SignaturesRoutinesInputStructures> tSAD806xSignaturesRoutinesInputStructures = new List<R_SAD806x_SignaturesRoutinesInputStructures>();
                List<R_SAD806x_SignaturesRoutinesInternalScalars> tSAD806xSignaturesRoutinesInternalScalars = new List<R_SAD806x_SignaturesRoutinesInternalScalars>();
                List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> tSAD806xSignaturesRoutinesInternalScalarsBitFlags = new List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                List<R_SAD806x_SignaturesRoutinesInternalFunctions> tSAD806xSignaturesRoutinesInternalFunctions = new List<R_SAD806x_SignaturesRoutinesInternalFunctions>();
                List<R_SAD806x_SignaturesRoutinesInternalTables> tSAD806xSignaturesRoutinesInternalTables = new List<R_SAD806x_SignaturesRoutinesInternalTables>();
                List<R_SAD806x_SignaturesRoutinesInternalStructures> tSAD806xSignaturesRoutinesInternalStructures = new List<R_SAD806x_SignaturesRoutinesInternalStructures>();
                
                List<R_SAD806x_SignaturesElements> tSAD806xSignaturesElements = new List<R_SAD806x_SignaturesElements>();
                List<R_SAD806x_SignaturesElementsInternalScalars> tSAD806xSignaturesElementsInternalScalars = new List<R_SAD806x_SignaturesElementsInternalScalars>();
                List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> tSAD806xSignaturesElementsInternalScalarsBitFlags = new List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
                List<R_SAD806x_SignaturesElementsInternalFunctions> tSAD806xSignaturesElementsInternalFunctions = new List<R_SAD806x_SignaturesElementsInternalFunctions>();
                List<R_SAD806x_SignaturesElementsInternalTables> tSAD806xSignaturesElementsInternalTables = new List<R_SAD806x_SignaturesElementsInternalTables>();
                List<R_SAD806x_SignaturesElementsInternalStructures> tSAD806xSignaturesElementsInternalStructures = new List<R_SAD806x_SignaturesElementsInternalStructures>();
                
                foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
                {
                    SQLite806xToolsSAD806x.addSignatureRoutineRow(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutines, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInputArgRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInputArgs, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInputScalarRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInputScalars, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInputFunctionRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInputFunctions, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInputTableRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInputTables, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInputStructureRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInputStructures, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInternalScalarRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInternalScalars, s6xObject);
                    if (s6xObject.isAdvanced && s6xObject.InternalScalars != null)
                    {
                        foreach (S6xRoutineInternalScalar s6xSubObject in s6xObject.InternalScalars)
                        {
                            SQLite806xToolsSAD806x.addSignatureRoutineInternalScalarBitFlagRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInternalScalarsBitFlags, s6xObject, s6xSubObject);
                        }
                    }
                    SQLite806xToolsSAD806x.addSignatureRoutineInternalFunctionRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInternalFunctions, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInternalTableRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInternalTables, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureRoutineInternalStructureRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesRoutinesInternalStructures, s6xObject);
                }

                foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
                {
                    SQLite806xToolsSAD806x.addSignatureElementRow(ref db806x, ref sadS6x, ref tSAD806xSignaturesElements, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureElementInternalScalarRow(ref db806x, ref sadS6x, ref tSAD806xSignaturesElementsInternalScalars, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureElementInternalScalarBitFlagRows(ref db806x, ref sadS6x, ref tSAD806xSignaturesElementsInternalScalarsBitFlags, s6xObject, s6xObject.Scalar);
                    SQLite806xToolsSAD806x.addSignatureElementInternalFunctionRow(ref db806x, ref sadS6x, ref tSAD806xSignaturesElementsInternalFunctions, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureElementInternalTableRow(ref db806x, ref sadS6x, ref tSAD806xSignaturesElementsInternalTables, s6xObject);
                    SQLite806xToolsSAD806x.addSignatureElementInternalStructureRow(ref db806x, ref sadS6x, ref tSAD806xSignaturesElementsInternalStructures, s6xObject);
                }
                db806x.Truncate<R_SAD806x_SignaturesRoutines>();
                db806x.Write<R_SAD806x_SignaturesRoutines>(ref tSAD806xSignaturesRoutines);
                tSAD806xSignaturesRoutines = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInputArgs>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInputArgs>(ref tSAD806xSignaturesRoutinesInputArgs);
                tSAD806xSignaturesRoutinesInputArgs = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInputScalars>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInputScalars>(ref tSAD806xSignaturesRoutinesInputScalars);
                tSAD806xSignaturesRoutinesInputScalars = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInputFunctions>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInputFunctions>(ref tSAD806xSignaturesRoutinesInputFunctions);
                tSAD806xSignaturesRoutinesInputFunctions = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInputTables>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInputTables>(ref tSAD806xSignaturesRoutinesInputTables);
                tSAD806xSignaturesRoutinesInputTables = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInputStructures>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInputStructures>(ref tSAD806xSignaturesRoutinesInputStructures);
                tSAD806xSignaturesRoutinesInputStructures = null;
                db806x.Write<R_SAD806x_SignaturesRoutinesInternalScalars>(ref tSAD806xSignaturesRoutinesInternalScalars);
                tSAD806xSignaturesRoutinesInternalScalars = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(ref tSAD806xSignaturesRoutinesInternalScalarsBitFlags);
                tSAD806xSignaturesRoutinesInternalScalarsBitFlags = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInternalFunctions>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInternalFunctions>(ref tSAD806xSignaturesRoutinesInternalFunctions);
                tSAD806xSignaturesRoutinesInternalFunctions = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInternalTables>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInternalTables>(ref tSAD806xSignaturesRoutinesInternalTables);
                tSAD806xSignaturesRoutinesInternalTables = null;
                db806x.Truncate<R_SAD806x_SignaturesRoutinesInternalStructures>();
                db806x.Write<R_SAD806x_SignaturesRoutinesInternalStructures>(ref tSAD806xSignaturesRoutinesInternalStructures);
                tSAD806xSignaturesRoutinesInternalStructures = null;

                db806x.Truncate<R_SAD806x_SignaturesElements>();
                db806x.Write<R_SAD806x_SignaturesElements>(ref tSAD806xSignaturesElements);
                tSAD806xSignaturesElements = null;
                db806x.Write<R_SAD806x_SignaturesElementsInternalScalars>(ref tSAD806xSignaturesElementsInternalScalars);
                tSAD806xSignaturesElementsInternalScalars = null;
                db806x.Truncate<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
                db806x.Write<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(ref tSAD806xSignaturesElementsInternalScalarsBitFlags);
                tSAD806xSignaturesElementsInternalScalarsBitFlags = null;
                db806x.Truncate<R_SAD806x_SignaturesElementsInternalFunctions>();
                db806x.Write<R_SAD806x_SignaturesElementsInternalFunctions>(ref tSAD806xSignaturesElementsInternalFunctions);
                tSAD806xSignaturesElementsInternalFunctions = null;
                db806x.Truncate<R_SAD806x_SignaturesElementsInternalTables>();
                db806x.Write<R_SAD806x_SignaturesElementsInternalTables>(ref tSAD806xSignaturesElementsInternalTables);
                tSAD806xSignaturesElementsInternalTables = null;
                db806x.Truncate<R_SAD806x_SignaturesElementsInternalStructures>();
                db806x.Write<R_SAD806x_SignaturesElementsInternalStructures>(ref tSAD806xSignaturesElementsInternalStructures);
                tSAD806xSignaturesElementsInternalStructures = null;

                return true;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Signatures export part has failed.\r\n" + ex.Message);
                return false;
            }
        }

        public static long importSignatures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors)
        {
            long lResult = 0;

            try
            {
                List<R_SAD806x_SignaturesRoutines> tSAD806xSignaturesRoutines = db806x.Read<R_SAD806x_SignaturesRoutines>();
                lResult = tSAD806xSignaturesRoutines.Count;
                foreach (R_SAD806x_SignaturesRoutines rRow in tSAD806xSignaturesRoutines) SQLite806xToolsSAD806x.setSignatureRoutineS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutines = null;
                List<R_SAD806x_SignaturesRoutinesInputArgs> tSAD806xSignaturesRoutinesInputArgs = db806x.Read<R_SAD806x_SignaturesRoutinesInputArgs>();
                foreach (R_SAD806x_SignaturesRoutinesInputArgs rRow in tSAD806xSignaturesRoutinesInputArgs) SQLite806xToolsSAD806x.setSignatureRoutineInputArgS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInputArgs = null;
                List<R_SAD806x_SignaturesRoutinesInputScalars> tSAD806xSignaturesRoutinesInputScalars = db806x.Read<R_SAD806x_SignaturesRoutinesInputScalars>();
                foreach (R_SAD806x_SignaturesRoutinesInputScalars rRow in tSAD806xSignaturesRoutinesInputScalars) SQLite806xToolsSAD806x.setSignatureRoutineInputScalarS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInputScalars = null;
                List<R_SAD806x_SignaturesRoutinesInputFunctions> tSAD806xSignaturesRoutinesInputFunctions = db806x.Read<R_SAD806x_SignaturesRoutinesInputFunctions>();
                foreach (R_SAD806x_SignaturesRoutinesInputFunctions rRow in tSAD806xSignaturesRoutinesInputFunctions) SQLite806xToolsSAD806x.setSignatureRoutineInputFunctionS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInputFunctions = null;
                List<R_SAD806x_SignaturesRoutinesInputTables> tSAD806xSignaturesRoutinesInputTables = db806x.Read<R_SAD806x_SignaturesRoutinesInputTables>();
                foreach (R_SAD806x_SignaturesRoutinesInputTables rRow in tSAD806xSignaturesRoutinesInputTables) SQLite806xToolsSAD806x.setSignatureRoutineInputTableS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInputTables = null;
                List<R_SAD806x_SignaturesRoutinesInputStructures> tSAD806xSignaturesRoutinesInputStructures = db806x.Read<R_SAD806x_SignaturesRoutinesInputStructures>();
                foreach (R_SAD806x_SignaturesRoutinesInputStructures rRow in tSAD806xSignaturesRoutinesInputStructures) SQLite806xToolsSAD806x.setSignatureRoutineInputStructureS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInputStructures = null;
                List<R_SAD806x_SignaturesRoutinesInternalScalars> tSAD806xSignaturesRoutinesInternalScalars = db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalars>();
                foreach (R_SAD806x_SignaturesRoutinesInternalScalars rRow in tSAD806xSignaturesRoutinesInternalScalars) SQLite806xToolsSAD806x.setSignatureRoutineInternalScalarS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInternalScalars = null;
                List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> tSAD806xSignaturesRoutinesInternalScalarsBitFlags = db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                foreach (R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow in tSAD806xSignaturesRoutinesInternalScalarsBitFlags) SQLite806xToolsSAD806x.setSignatureRoutineInternalScalarBitFlagS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInternalScalarsBitFlags = null;
                List<R_SAD806x_SignaturesRoutinesInternalFunctions> tSAD806xSignaturesRoutinesInternalFunctions = db806x.Read<R_SAD806x_SignaturesRoutinesInternalFunctions>();
                foreach (R_SAD806x_SignaturesRoutinesInternalFunctions rRow in tSAD806xSignaturesRoutinesInternalFunctions) SQLite806xToolsSAD806x.setSignatureRoutineInternalFunctionS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInternalFunctions = null;
                List<R_SAD806x_SignaturesRoutinesInternalTables> tSAD806xSignaturesRoutinesInternalTables = db806x.Read<R_SAD806x_SignaturesRoutinesInternalTables>();
                foreach (R_SAD806x_SignaturesRoutinesInternalTables rRow in tSAD806xSignaturesRoutinesInternalTables) SQLite806xToolsSAD806x.setSignatureRoutineInternalTableS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInternalTables = null;
                List<R_SAD806x_SignaturesRoutinesInternalStructures> tSAD806xSignaturesRoutinesInternalStructures = db806x.Read<R_SAD806x_SignaturesRoutinesInternalStructures>();
                foreach (R_SAD806x_SignaturesRoutinesInternalStructures rRow in tSAD806xSignaturesRoutinesInternalStructures) SQLite806xToolsSAD806x.setSignatureRoutineInternalStructureS6x(ref sadS6x, rRow);
                tSAD806xSignaturesRoutinesInternalStructures = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Signatures import part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncSignatures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            lResult += syncSignaturesRoutines(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            lResult += syncSignaturesElements(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        public static long syncSignaturesRoutines(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutines> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutines>();
                foreach (R_SAD806x_SignaturesRoutines rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutines>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutines)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutines)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineS6x(ref sadS6x, (R_SAD806x_SignaturesRoutines)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineS6x(ref sadS6x, (R_SAD806x_SignaturesRoutines)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLiteSAD806xCreation)
                    {
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xObject);
                        if (syncObject.SyncSqLiteSAD806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutines>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Signatures synchronization part has failed.\r\n" + ex.Message);
            }

            syncSignaturesRoutinesInputArgs(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInputScalars(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInputFunctions(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInputTables(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInputStructures(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInternalScalars(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInternalFunctions(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInternalTables(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesRoutinesInternalStructures(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncSignaturesRoutinesInputArgs(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInputArgs> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInputArgs>();
                foreach (R_SAD806x_SignaturesRoutinesInputArgs rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InputArguments == null) continue;
                    if (s6xParentObject.InputArguments.Length == 0) continue;

                    foreach (S6xRoutineInputArgument s6xObject in s6xParentObject.InputArguments)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInputArgs>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInputArgRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInputArgs)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputArgument)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInputArgs)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInputArgS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputArgs)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInputArgS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputArgs)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInputArgRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputArgument)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInputArgs>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Input Arguments synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInputScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInputScalars> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInputScalars>();
                foreach (R_SAD806x_SignaturesRoutinesInputScalars rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InputScalars == null) continue;
                    if (s6xParentObject.InputScalars.Length == 0) continue;

                    foreach (S6xRoutineInputScalar s6xObject in s6xParentObject.InputScalars)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInputScalars>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInputScalarRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInputScalars)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputScalar)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInputScalars)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInputScalarS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputScalars)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInputScalarS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputScalars)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInputScalarRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputScalar)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInputScalars>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Input Scalars synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInputFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInputFunctions> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInputFunctions>();
                foreach (R_SAD806x_SignaturesRoutinesInputFunctions rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InputFunctions == null) continue;
                    if (s6xParentObject.InputFunctions.Length == 0) continue;

                    foreach (S6xRoutineInputFunction s6xObject in s6xParentObject.InputFunctions)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInputFunctions>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInputFunctionRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInputFunctions)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputFunction)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInputFunctions)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInputFunctionS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputFunctions)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInputFunctionS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputFunctions)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInputFunctionRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputFunction)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInputFunctions>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Input Functions synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInputTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInputTables> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInputTables>();
                foreach (R_SAD806x_SignaturesRoutinesInputTables rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InputTables == null) continue;
                    if (s6xParentObject.InputTables.Length == 0) continue;

                    foreach (S6xRoutineInputTable s6xObject in s6xParentObject.InputTables)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInputTables>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInputTableRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInputTables)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputTable)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInputTables)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInputTableS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputTables)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInputTableS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputTables)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInputTableRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputTable)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInputTables>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Input Tables synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInputStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInputStructures> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInputStructures>();
                foreach (R_SAD806x_SignaturesRoutinesInputStructures rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InputStructures == null) continue;
                    if (s6xParentObject.InputStructures.Length == 0) continue;

                    foreach (S6xRoutineInputStructure s6xObject in s6xParentObject.InputStructures)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInputStructures>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInputStructureRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInputStructures)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputStructure)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInputStructures)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInputStructureS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputStructures)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInputStructureS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInputStructures)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInputStructureRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInputStructure)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInputStructures>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Input Structures synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInternalScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInternalScalars> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalars>();
                foreach (R_SAD806x_SignaturesRoutinesInternalScalars rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InternalScalars == null) continue;
                    if (s6xParentObject.InternalScalars.Length == 0) continue;

                    foreach (S6xRoutineInternalScalar s6xObject in s6xParentObject.InternalScalars)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInternalScalars>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInternalScalarRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalScalars)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalScalar)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInternalScalars)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInternalScalarS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalScalars)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInternalScalarS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalScalars)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInternalScalarRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalScalar)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInternalScalars>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Internal Scalars synchronization part has failed.\r\n" + ex.Message);
            }

            syncSignaturesRoutinesInternalScalarsBitFlags(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncSignaturesRoutinesInternalScalarsBitFlags(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                foreach (R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentParentObject.InternalScalars == null) continue;
                    if (s6xParentParentObject.InternalScalars.Length == 0) continue;

                    foreach (S6xRoutineInternalScalar s6xParentObject in s6xParentParentObject.InternalScalars)
                    {
                        if (!s6xParentObject.isBitFlags) continue;

                        foreach (S6xBitFlag s6xObject in s6xParentObject.BitFlags)
                        {
                            S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentParentObject, s6xParentObject, s6xObject);
                            if (!syncObject.SyncValid) continue;
                            if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                            {
                                syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                                syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                                syncObjects[syncObject.SyncUniqueKey].S6xParentParentObject = s6xParentParentObject;
                            }
                            else
                            {
                                syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                            }
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInternalScalarBitFlagRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentParentObject, (S6xRoutineInternalScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInternalScalarBitFlagS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInternalScalarBitFlagS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInternalScalarBitFlagRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentParentObject, (S6xRoutineInternalScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Signatures Routines Internal Scalars Bit Flags synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInternalFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInternalFunctions> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInternalFunctions>();
                foreach (R_SAD806x_SignaturesRoutinesInternalFunctions rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InternalFunctions == null) continue;
                    if (s6xParentObject.InternalFunctions.Length == 0) continue;

                    foreach (S6xRoutineInternalFunction s6xObject in s6xParentObject.InternalFunctions)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInternalFunctions>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInternalFunctionRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalFunctions)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalFunction)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInternalFunctions)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInternalFunctionS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalFunctions)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInternalFunctionS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalFunctions)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInternalFunctionRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalFunction)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInternalFunctions>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Internal Functions synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInternalTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInternalTables> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInternalTables>();
                foreach (R_SAD806x_SignaturesRoutinesInternalTables rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InternalTables == null) continue;
                    if (s6xParentObject.InternalTables.Length == 0) continue;

                    foreach (S6xRoutineInternalTable s6xObject in s6xParentObject.InternalTables)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInternalTables>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInternalTableRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalTables)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalTable)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInternalTables)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInternalTableS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalTables)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInternalTableS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalTables)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInternalTableRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalTable)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInternalTables>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Internal Tables synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesRoutinesInternalStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesRoutinesInternalStructures> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesRoutinesInternalStructures>();
                foreach (R_SAD806x_SignaturesRoutinesInternalStructures rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xSignature s6xParentObject in sadS6x.slSignatures.Values)
                {
                    if (s6xParentObject.InternalStructures == null) continue;
                    if (s6xParentObject.InternalStructures.Length == 0) continue;

                    foreach (S6xRoutineInternalStructure s6xObject in s6xParentObject.InternalStructures)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesRoutinesInternalStructures>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureRoutineInternalStructureRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalStructures)syncObject.SqLiteSAD806xObject, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalStructure)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesRoutinesInternalStructures)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureRoutineInternalStructureS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalStructures)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureRoutineInternalStructureS6x(ref sadS6x, (R_SAD806x_SignaturesRoutinesInternalStructures)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureRoutineInternalStructureRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xSignature)syncObject.S6xParentObject, (S6xRoutineInternalStructure)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesRoutinesInternalStructures>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Routines Signatures Internal Structures synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncSignaturesElements(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesElements> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesElements>();
                foreach (R_SAD806x_SignaturesElements rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesElements>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureElementRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesElements)syncObject.SqLiteSAD806xObject, (S6xElementSignature)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesElements)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureElementS6x(ref sadS6x, (R_SAD806x_SignaturesElements)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureElementS6x(ref sadS6x, (R_SAD806x_SignaturesElements)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureElementRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xElementSignature)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesElements>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Elements Signatures synchronization part has failed.\r\n" + ex.Message);
            }

            syncSignaturesElementsInternalScalars(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesElementsInternalFunctions(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesElementsInternalTables(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);
            syncSignaturesElementsInternalStructures(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncSignaturesElementsInternalScalars(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesElementsInternalScalars> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesElementsInternalScalars>();
                foreach (R_SAD806x_SignaturesElementsInternalScalars rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xElementSignature s6xParentObject in sadS6x.slElementsSignatures.Values)
                {
                    object s6xObject = s6xParentObject.Scalar;
                    if (s6xObject == null) continue;

                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                    {
                        syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                    }
                    else
                    {
                        syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesElementsInternalScalars>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureElementInternalScalarRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesElementsInternalScalars)syncObject.SqLiteSAD806xObject, (S6xElementSignature)syncObject.S6xParentObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesElementsInternalScalars)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureElementInternalScalarS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalScalars)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureElementInternalScalarS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalScalars)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureElementInternalScalarRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xElementSignature)syncObject.S6xParentObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesElementsInternalScalars>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Elements Signatures Internal Scalars synchronization part has failed.\r\n" + ex.Message);
            }

            syncSignaturesElementsInternalScalarsBitFlags(ref db806x, ref sadS6x, ref processErrors, ref unSyncObjects, dtLastSync);

            return lResult;
        }

        private static long syncSignaturesElementsInternalScalarsBitFlags(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
                foreach (R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xElementSignature s6xParentParentObject in sadS6x.slElementsSignatures.Values)
                {
                    S6xRoutineInternalScalar s6xParentObject = s6xParentParentObject.Scalar;
                    if (s6xParentObject == null) continue;
                    if (!s6xParentObject.isBitFlags) continue;

                    foreach (S6xBitFlag s6xObject in s6xParentObject.BitFlags)
                    {
                        S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentParentObject, s6xParentObject, s6xObject);
                        if (!syncObject.SyncValid) continue;
                        if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                        {
                            syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                            syncObjects[syncObject.SyncUniqueKey].S6xParentParentObject = s6xParentParentObject;
                        }
                        else
                        {
                            syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                        }
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureElementInternalScalarBitFlagRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesElementsInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject, (S6xElementSignature)syncObject.S6xParentParentObject, (S6xRoutineInternalScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesElementsInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureElementInternalScalarBitFlagS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureElementInternalScalarBitFlagS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalScalarsBitFlags)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureElementInternalScalarBitFlagRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xElementSignature)syncObject.S6xParentParentObject, (S6xRoutineInternalScalar)syncObject.S6xParentObject, (S6xBitFlag)syncObject.S6xObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Signatures Elements Internal Scalars Bit Flags synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesElementsInternalFunctions(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesElementsInternalFunctions> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesElementsInternalFunctions>();
                foreach (R_SAD806x_SignaturesElementsInternalFunctions rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xElementSignature s6xParentObject in sadS6x.slElementsSignatures.Values)
                {
                    object s6xObject = s6xParentObject.Function;
                    if (s6xObject == null) continue;

                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                    {
                        syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                    }
                    else
                    {
                        syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesElementsInternalFunctions>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureElementInternalFunctionRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesElementsInternalFunctions)syncObject.SqLiteSAD806xObject, (S6xElementSignature)syncObject.S6xParentObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesElementsInternalFunctions)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureElementInternalFunctionS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalFunctions)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureElementInternalFunctionS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalFunctions)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureElementInternalFunctionRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xElementSignature)syncObject.S6xParentObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesElementsInternalFunctions>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Elements Signatures Internal Functions synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesElementsInternalTables(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesElementsInternalTables> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesElementsInternalTables>();
                foreach (R_SAD806x_SignaturesElementsInternalTables rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xElementSignature s6xParentObject in sadS6x.slElementsSignatures.Values)
                {
                    object s6xObject = s6xParentObject.Table;
                    if (s6xObject == null) continue;

                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                    {
                        syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                    }
                    else
                    {
                        syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesElementsInternalTables>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureElementInternalTableRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesElementsInternalTables)syncObject.SqLiteSAD806xObject, (S6xElementSignature)syncObject.S6xParentObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesElementsInternalTables)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureElementInternalTableS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalTables)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureElementInternalTableS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalTables)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureElementInternalTableRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xElementSignature)syncObject.S6xParentObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesElementsInternalTables>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Elements Signatures Internal Tables synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        private static long syncSignaturesElementsInternalStructures(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref ArrayList processErrors, ref List<S_SQLiteSyncS6x> unSyncObjects, DateTime dtLastSync)
        {
            long lResult = 0;

            try
            {
                Dictionary<string, S_SQLiteSyncS6x> syncObjects = new Dictionary<string, S_SQLiteSyncS6x>();

                List<R_SAD806x_SignaturesElementsInternalStructures> tSAD806xRows = db806x.Read<R_SAD806x_SignaturesElementsInternalStructures>();
                foreach (R_SAD806x_SignaturesElementsInternalStructures rRow in tSAD806xRows)
                {
                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(rRow, false);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey)) syncObjects[syncObject.SyncUniqueKey].SqLiteSAD806xObject = rRow;
                    else syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                }
                tSAD806xRows = null;

                foreach (S6xElementSignature s6xParentObject in sadS6x.slElementsSignatures.Values)
                {
                    object s6xObject = s6xParentObject.Structure;
                    if (s6xObject == null) continue;

                    S_SQLiteSyncS6x syncObject = new S_SQLiteSyncS6x(s6xParentObject, s6xObject);
                    if (!syncObject.SyncValid) continue;
                    if (syncObjects.ContainsKey(syncObject.SyncUniqueKey))
                    {
                        syncObjects[syncObject.SyncUniqueKey].S6xObject = s6xObject;
                        syncObjects[syncObject.SyncUniqueKey].S6xParentObject = s6xParentObject;
                    }
                    else
                    {
                        syncObjects.Add(syncObject.SyncUniqueKey, syncObject);
                    }
                }

                lResult = syncObjects.Count;

                tSAD806xRows = new List<R_SAD806x_SignaturesElementsInternalStructures>();

                if (unSyncObjects == null) unSyncObjects = new List<S_SQLiteSyncS6x>();
                foreach (S_SQLiteSyncS6x syncObject in syncObjects.Values)
                {
                    // Nothing to manage
                    if (syncObject.SyncIgnore) continue;

                    // S6x and SqLite806x object are available
                    if (syncObject.SyncCompare)
                    {
                        if (syncObject.SyncS6xMaster)
                        {
                            SQLite806xToolsSAD806x.updateSignatureElementInternalStructureRow(ref db806x, ref sadS6x, (R_SAD806x_SignaturesElementsInternalStructures)syncObject.SqLiteSAD806xObject, (S6xElementSignature)syncObject.S6xParentObject);
                            tSAD806xRows.Add((R_SAD806x_SignaturesElementsInternalStructures)syncObject.SqLiteSAD806xObject);
                        }
                        else if (syncObject.SyncSqLite806xMaster)
                        {
                            SQLite806xToolsSAD806x.setSignatureElementInternalStructureS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalStructures)syncObject.SqLiteSAD806xObject);
                        }
                    }
                    else if (syncObject.SyncS6xCreation)
                    {
                        if (syncObject.SyncS6xCreationCancelled(dtLastSync)) continue;
                        object[] s6xCreatedObjects = null;
                        s6xCreatedObjects = SQLite806xToolsSAD806x.setSignatureElementInternalStructureS6x(ref sadS6x, (R_SAD806x_SignaturesElementsInternalStructures)syncObject.SqLiteSAD806xObject);
                        if (s6xCreatedObjects != null)
                        {
                            syncObject.S6xObject = s6xCreatedObjects[0];
                            syncObject.S6xParentObject = s6xCreatedObjects[1];
                            syncObject.S6xParentParentObject = s6xCreatedObjects[2];
                        }
                        if (syncObject.SyncS6xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                    else if (syncObject.SyncSqLite806xCreation)
                    {
                        if (syncObject.SyncSqLite806xCreationCancelled(dtLastSync)) continue;
                        syncObject.SqLiteSAD806xObject = SQLite806xToolsSAD806x.addSignatureElementInternalStructureRow(ref db806x, ref sadS6x, ref tSAD806xRows, (S6xElementSignature)syncObject.S6xParentObject);
                        if (syncObject.SyncSqLite806xCreationDoubt(dtLastSync)) unSyncObjects.Add(syncObject);
                    }
                }

                syncObjects = null;

                db806x.Write<R_SAD806x_SignaturesElementsInternalStructures>(ref tSAD806xRows);

                tSAD806xRows = null;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Elements Signatures Internal Structures synchronization part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRemovalStep1(S_SQLiteSyncS6x syncObjectToRemove, ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<object> dbRowsToDelete, ref ArrayList processErrors)
        {
            long lResult = 0;

            if (syncObjectToRemove == null || db806x == null) return lResult;

            try
            {
                List<object> oSubRows = null;

                if (syncObjectToRemove.S6xObject != null)
                {
                    try
                    {
                        if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xScalar))
                        {
                            if (syncObjectToRemove.isDuplicate) sadS6x.slDupScalars.Remove(syncObjectToRemove.DuplicateAddress);
                            else sadS6x.slScalars.Remove(syncObjectToRemove.UniqueAddress);
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xFunction))
                        {
                            if (syncObjectToRemove.isDuplicate) sadS6x.slDupFunctions.Remove(syncObjectToRemove.DuplicateAddress);
                            else sadS6x.slFunctions.Remove(syncObjectToRemove.UniqueAddress);
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xTable))
                        {
                            if (syncObjectToRemove.isDuplicate) sadS6x.slDupTables.Remove(syncObjectToRemove.DuplicateAddress);
                            else sadS6x.slTables.Remove(syncObjectToRemove.UniqueAddress);
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xStructure))
                        {
                            if (syncObjectToRemove.isDuplicate) sadS6x.slDupStructures.Remove(syncObjectToRemove.DuplicateAddress);
                            else sadS6x.slStructures.Remove(syncObjectToRemove.UniqueAddress);
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutine)) sadS6x.slRoutines.Remove(syncObjectToRemove.UniqueAddress);
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xOperation)) sadS6x.slOperations.Remove(syncObjectToRemove.UniqueAddress);
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRegister)) sadS6x.slRegisters.Remove(syncObjectToRemove.RegisterUniqueAddress);
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xOtherAddress)) sadS6x.slOtherAddresses.Remove(syncObjectToRemove.UniqueAddress);
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xSignature)) sadS6x.slSignatures.Remove(syncObjectToRemove.UniqueKey);
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xElementSignature)) sadS6x.slElementsSignatures.Remove(syncObjectToRemove.UniqueKey);
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xBitFlag))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xScalar)) ((S6xScalar)syncObjectToRemove.S6xParentObject).DelBitFlag((S6xBitFlag)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRegister)) ((S6xRegister)syncObjectToRemove.S6xParentObject).DelBitFlag((S6xBitFlag)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRoutineInternalScalar)) ((S6xRoutineInternalScalar)syncObjectToRemove.S6xParentObject).DelBitFlag((S6xBitFlag)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInputArgument))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRoutine)) ((S6xRoutine)syncObjectToRemove.S6xParentObject).DelInputArgument((S6xRoutineInputArgument)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInputArgument((S6xRoutineInputArgument)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInputStructure))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRoutine)) ((S6xRoutine)syncObjectToRemove.S6xParentObject).DelInputStructure((S6xRoutineInputStructure)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInputStructure((S6xRoutineInputStructure)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInputTable))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRoutine)) ((S6xRoutine)syncObjectToRemove.S6xParentObject).DelInputTable((S6xRoutineInputTable)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInputTable((S6xRoutineInputTable)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInputFunction))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRoutine)) ((S6xRoutine)syncObjectToRemove.S6xParentObject).DelInputFunction((S6xRoutineInputFunction)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInputFunction((S6xRoutineInputFunction)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInputScalar))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xRoutine)) ((S6xRoutine)syncObjectToRemove.S6xParentObject).DelInputScalar((S6xRoutineInputScalar)syncObjectToRemove.S6xObject);
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInputScalar((S6xRoutineInputScalar)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInternalStructure))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xElementSignature)) ((S6xElementSignature)syncObjectToRemove.S6xParentObject).Structure = null;
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInternalStructure((S6xRoutineInternalStructure)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInternalTable))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xElementSignature)) ((S6xElementSignature)syncObjectToRemove.S6xParentObject).Table = null;
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInternalTable((S6xRoutineInternalTable)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInternalFunction))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xElementSignature)) ((S6xElementSignature)syncObjectToRemove.S6xParentObject).Function = null;
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInternalFunction((S6xRoutineInternalFunction)syncObjectToRemove.S6xObject);
                            }
                        }
                        else if (syncObjectToRemove.S6xObject.GetType() == typeof(S6xRoutineInternalScalar))
                        {
                            if (syncObjectToRemove.S6xParentObject != null)
                            {
                                if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xElementSignature)) ((S6xElementSignature)syncObjectToRemove.S6xParentObject).Scalar = null;
                                else if (syncObjectToRemove.S6xParentObject.GetType() == typeof(S6xSignature)) ((S6xSignature)syncObjectToRemove.S6xParentObject).DelInternalScalar((S6xRoutineInternalScalar)syncObjectToRemove.S6xObject);
                            }
                        }
                    }
                    catch
                    {
                        throw new Exception("S6x object definition removal has failed.");
                    }
                }

                if (syncObjectToRemove.SqLite806xObject != null)
                {
                    dbRowsToDelete.Add(syncObjectToRemove.SqLite806xObject);
                    if (syncObjectToRemove.SqLite806xObject.GetType() == typeof(R_806x_Def_Scalars))
                    {
                        oSubRows = SQLite806xTools806x.getScalarSubRows(ref db806x, (R_806x_Def_Scalars)syncObjectToRemove.SqLite806xObject);
                        if (oSubRows == null) throw new Exception("Reading 806x Scalars linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLite806xObject.GetType() == typeof(R_806x_Def_Routines))
                    {
                        oSubRows = SQLite806xTools806x.getRoutineSubRows(ref db806x, (R_806x_Def_Routines)syncObjectToRemove.SqLite806xObject);
                        if (oSubRows == null) throw new Exception("Reading 806x Routines linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLite806xObject.GetType() == typeof(R_806x_Def_Registers))
                    {
                        oSubRows = SQLite806xTools806x.getRegisterSubRows(ref db806x, (R_806x_Def_Registers)syncObjectToRemove.SqLite806xObject);
                        if (oSubRows == null) throw new Exception("Reading 806x Registers linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                }

                if (syncObjectToRemove.SqLiteSAD806xObject != null)
                {
                    dbRowsToDelete.Add(syncObjectToRemove.SqLiteSAD806xObject);
                    if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Scalars))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getScalarSubRows(ref db806x, (R_SAD806x_Def_Scalars)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Scalars linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Routines))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getRoutineSubRows(ref db806x, (R_SAD806x_Def_Routines)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Routines linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Registers))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getRegisterSubRows(ref db806x, (R_SAD806x_Def_Registers)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Registers linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutines))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getSignatureRoutineSubRows(ref db806x, (R_SAD806x_SignaturesRoutines)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Routines Signatures linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalScalars))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getSignatureRoutineInternalScalarSubRows(ref db806x, (R_SAD806x_SignaturesRoutinesInternalScalars)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Routines Signatures Internal Scalars linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElements))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getSignatureElementSubRows(ref db806x, (R_SAD806x_SignaturesElements)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Elements Signatures linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                    else if (syncObjectToRemove.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElementsInternalScalars))
                    {
                        oSubRows = SQLite806xToolsSAD806x.getSignatureElementInternalScalarSubRows(ref db806x, (R_SAD806x_SignaturesElementsInternalScalars)syncObjectToRemove.SqLiteSAD806xObject);
                        if (oSubRows == null) throw new Exception("Reading SAD806x Elements Signatures Scalars linked records has failed.");
                        dbRowsToDelete.AddRange(oSubRows);
                    }
                }

                lResult++;
            }
            catch (Exception ex)
            {
                processErrors.Add("Synchronization object removal part has failed.\r\n" + ex.Message);
            }

            return lResult;
        }

        public static long syncRemovalStep2(ref List<object> dbRowsToDelete, ref SQLite806xDB db806x, ref ArrayList processErrors)
        {
            long lResult = 0;

            if (dbRowsToDelete == null || db806x == null) return lResult;

            MethodInfo deleteMethod = null;
            foreach (MethodInfo mInfo in typeof(SQLite806xDB).GetMethods())
            {
                if (mInfo.Name != "Delete") continue;
                if (mInfo.GetParameters().Length != 1) continue;
                deleteMethod = mInfo;
                break;
            }
            if (deleteMethod == null) return lResult;

            SortedList slRowsByType = new SortedList();
            
            foreach (object oRow in dbRowsToDelete)
            {
                if (oRow == null) continue;
                Type tType = oRow.GetType();
                if (!slRowsByType.ContainsKey(tType.Name)) slRowsByType.Add(tType.Name, (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(tType)));
                ((System.Collections.IList)slRowsByType[tType.Name]).Add(oRow);
            }

            foreach (string sType in slRowsByType.Keys)
            {
                if (((System.Collections.IList)slRowsByType[sType]).Count == 0) continue;
                Type tType = ((System.Collections.IList)slRowsByType[sType])[0].GetType();
                MethodInfo deleteTypeMethod = deleteMethod.MakeGenericMethod(tType);
                try
                {
                    bool bResult = (bool)deleteTypeMethod.Invoke(db806x, new object[] { slRowsByType[sType] });
                    if (!bResult) throw new Exception(sType + " rows removal has failed.");
                }
                catch (Exception ex)
                {
                    processErrors.Add("Synchronization object removal part has failed.\r\n" + ex.Message);
                }
            }

            lResult += dbRowsToDelete.Count;

            return lResult;
        }

        public static long importCount(ref SQLite806xDB db806x, ref bool[] iesOpt, ref ArrayList processErrors)
        {
            long lResult = 0;

            bool iesOptProperties = false;
            bool iesOptTables = false;
            bool iesOptFunctions = false;
            bool iesOptScalars = false;
            bool iesOptStructures = false;
            bool iesOptRoutines = false;
            bool iesOptOperations = false;
            bool iesOptOther = false;
            bool iesOptRegisters = false;
            bool iesOptSignatures = false;
            
            try
            {
                iesOptProperties = iesOpt[0];
                iesOptTables = iesOpt[1];
                iesOptFunctions = iesOpt[2];
                iesOptScalars = iesOpt[3];
                iesOptStructures = iesOpt[4];
                iesOptRoutines = iesOpt[5];
                iesOptOperations = iesOpt[6];
                iesOptOther = iesOpt[7];
                iesOptRegisters = iesOpt[8];
                iesOptSignatures = iesOpt[9];

                if (iesOptTables) lResult += db806x.Count<R_806x_Def_Tables>();
                if (iesOptFunctions) lResult += db806x.Count<R_806x_Def_Functions>();
                if (iesOptScalars) lResult += db806x.Count<R_806x_Def_Scalars>();
                if (iesOptStructures) lResult += db806x.Count<R_806x_Def_Structures>();
                if (iesOptRoutines) lResult += db806x.Count<R_806x_Def_Routines>();
                if (iesOptOperations) lResult += db806x.Count<R_806x_Def_Operations>();
                if (iesOptOther) lResult += db806x.Count<R_806x_Def_Others>();
                if (iesOptRegisters) lResult += db806x.Count<R_806x_Def_Registers>();
                if (iesOptSignatures)
                {
                    lResult += db806x.Count<R_SAD806x_SignaturesRoutines>();
                    lResult += db806x.Count<R_SAD806x_SignaturesElements>();
                }

                return lResult;
            }
            catch (Exception ex)
            {
                processErrors.Add("SQLite Count part has failed.\r\n" + ex.Message);
                return -1;
            }
        }
    }
}
