using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using SQLite806x;

namespace SAD806x
{
    public partial class SQLite806xRecordForm : Form
    {
        private SQLite806xDB sqlDB806x = null;
        private T_SQLiteTable sqLiteTable = null;
        private object sqLiteRecord = null;

        public event EventHandler RecordUpdated;
        public event EventHandler RecordRemoved;

        public SQLite806xRecordForm(ref SQLite806xDB db806x, ref T_SQLiteTable tTable, ref object rRecord)
        {
            sqlDB806x = db806x;
            sqLiteTable = tTable;
            sqLiteRecord = rRecord;

            if (sqLiteRecord != null) this.Tag = sqLiteTable.Name + "." + ((R_SQLite_Core)sqLiteRecord).RowId.ToString();
            
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.Load += new EventHandler(Form_Load);
            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (sqLiteRecord == null) this.Close();

            displayRecord();
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

        private void controlValue_Modified(object sender, EventArgs e)
        {
            F_SQLiteField fField = (F_SQLiteField)((Control)sender).Tag;

            if (fField == null) return;

            object newValue = null;
            Exception exError = null;

            if (sender.GetType() == typeof(TextBox)) if (!((TextBox)sender).Modified) return;

            if (sender.GetType() == typeof(TextBox)) newValue = ((TextBox)sender).Text;
            else if (sender.GetType() == typeof(DateTimePicker)) newValue = ((DateTimePicker)sender).Value;
            else if (sender.GetType() == typeof(CheckBox)) newValue = ((CheckBox)sender).Checked;

            // Type and Length Check
            switch (fField.EDbType)
            {
                case DbType.Int32:
                    try { newValue = Convert.ToInt32(newValue); }
                    catch (Exception ex) { exError = ex; }
                    break;
                case DbType.Int64:
                    try { newValue = Convert.ToInt64(newValue); }
                    catch (Exception ex) { exError = ex; }
                    break;
                case DbType.Double:
                    try { newValue = Convert.ToDouble(newValue); }
                    catch (Exception ex) { exError = ex; }
                    break;
                case DbType.DateTime:
                    try { newValue = Convert.ToDateTime(newValue); }
                    catch (Exception ex) { exError = ex; }
                    break;
                case DbType.Boolean:
                    try { newValue = Convert.ToBoolean(newValue); }
                    catch (Exception ex) { exError = ex; }
                    break;
                case DbType.Binary:
                    break;
                case DbType.String:
                    try { newValue = Convert.ToString(newValue); }
                    catch (Exception ex) { exError = ex; }
                    break;
                case DbType.Object:
                    break;
            }

            F_SQLiteField fRecordField = (F_SQLiteField)sqLiteRecord.GetType().GetProperty(fField.Name).GetValue(sqLiteRecord, null);

            if (exError != null)
            // Rollback
            {
                if (sender.GetType() == typeof(TextBox)) ((TextBox)sender).Text = fRecordField.Value == null ? string.Empty : fRecordField.Value.ToString();
                else if (sender.GetType() == typeof(DateTimePicker)) ((DateTimePicker)sender).Value = fRecordField.Value == null ? DateTime.Today : Convert.ToDateTime(fRecordField.Value);
                else if (sender.GetType() == typeof(CheckBox)) ((CheckBox)sender).Checked = fRecordField.Value == null ? false : Convert.ToBoolean(fRecordField.Value);

                MessageBox.Show(exError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            // Change
            {
                // Change except for Attachment, already managed
                if (fField.EDbType != DbType.Binary) fRecordField.Value = newValue;
                if (sender.GetType() == typeof(TextBox)) ((TextBox)sender).Modified = false;
            }

            Control[] arrControls = null;
            arrControls = this.Controls.Find("updateButton", true);
            if (arrControls.Length == 1) arrControls[0].Enabled = true;
            arrControls = null;
        }

        private void refreshLabelDescription()
        {
            MethodInfo mGetLabel = sqLiteRecord.GetType().GetMethod("getRecordLabel");
            if (mGetLabel != null) this.Text = (string)mGetLabel.Invoke(sqLiteRecord, null);
            MethodInfo mGetDescription = sqLiteRecord.GetType().GetMethod("getRecordDescription");
            if (mGetDescription != null)
            {
                Control[] arrControls = null;
                arrControls = this.Controls.Find("descLabel", true);
                if (arrControls.Length == 1) ((Label)arrControls[0]).Text = (string)mGetDescription.Invoke(sqLiteRecord, null);
                arrControls = null;
            }
        }

        private void displayRecord()
        {
            this.Text = "New record";

            int rowId = Convert.ToInt32(((F_SQLiteField)sqLiteRecord.GetType().GetProperty("RowId").GetValue(sqLiteRecord, null)).Value);

            Panel centerPanel = new Panel();
            centerPanel.AutoScroll = true;
            centerPanel.Dock = DockStyle.Fill;

            Panel topPanel = new Panel();
            topPanel.BorderStyle = BorderStyle.Fixed3D;
            Label descLabel = new Label();
            descLabel.Name = "descLabel";
            descLabel.TabIndex = 0;
            descLabel.AutoSize = false;
            descLabel.Height *= 3;
            descLabel.TextAlign = ContentAlignment.TopLeft;
            descLabel.Dock = DockStyle.Left;
            Panel buttonsPanel = new Panel();
            Button updateButton = new Button();
            updateButton.Name = "updateButton";
            updateButton.TabIndex = 1;
            updateButton.Enabled = false;
            updateButton.ImageList = this.dbSmallImageList;
            updateButton.ImageIndex = rowId < 0 ? 2 : 3;
            updateButton.Dock = DockStyle.Top;
            updateButton.Size = updateButton.Image.Size;
            updateButton.Click += new EventHandler(updateButton_Click);
            Button removeButton = new Button();
            removeButton.Name = "removeButton";
            removeButton.TabIndex = 2;
            removeButton.ImageList = this.dbSmallImageList;
            removeButton.ImageIndex = 4;
            removeButton.Dock = DockStyle.Top;
            removeButton.Size = removeButton.Image.Size;
            removeButton.Visible = !sqLiteTable.ReadOnly && rowId > 0;
            removeButton.Click += new EventHandler(removeButton_Click);
            buttonsPanel.Dock = DockStyle.Right;
            buttonsPanel.Width = updateButton.Width > removeButton.Width ? updateButton.Width : removeButton.Width;
            buttonsPanel.Controls.Add(removeButton);
            buttonsPanel.Controls.Add(updateButton);
            topPanel.Controls.Add(buttonsPanel);
            topPanel.Controls.Add(descLabel);
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = updateButton.Height + removeButton.Height + topPanel.Margin.Bottom;

            this.Controls.Add(centerPanel);
            this.Controls.Add(topPanel);

            if (rowId > 0) refreshLabelDescription();

            List<Panel> lstInvertedPanels = new List<Panel>();

            Control ctlSelected = null;
            
            Dictionary<string, Dictionary<string, F_SQLiteField>> attachmentsDetails = new Dictionary<string,Dictionary<string,F_SQLiteField>>();
            Dictionary<string, F_SQLiteField> attachmentDetails = null;

            foreach (F_SQLiteField fField in sqLiteTable.Fields)
            {
                if (fField.isRowId) continue;
                object oValue = null;
                string attachmentName = string.Empty;
                if (fField.isAttachment)
                {
                    if (fField.EDbType != DbType.Binary) continue;
                    attachmentName = fField.Name;
                    if (!attachmentsDetails.ContainsKey(attachmentName))
                    {
                        attachmentDetails = new Dictionary<string, F_SQLiteField>();
                        attachmentDetails.Add("Binary", (F_SQLiteField)sqLiteRecord.GetType().GetProperty(attachmentName).GetValue(sqLiteRecord, null));
                        if (sqLiteRecord.GetType().GetProperty(attachmentName + "Name") != null) attachmentDetails.Add("Name", (F_SQLiteField)sqLiteRecord.GetType().GetProperty(attachmentName + "Name").GetValue(sqLiteRecord, null));
                        if (sqLiteRecord.GetType().GetProperty(attachmentName + "Size") != null) attachmentDetails.Add("Size", (F_SQLiteField)sqLiteRecord.GetType().GetProperty(attachmentName + "Size").GetValue(sqLiteRecord, null));
                        if (sqLiteRecord.GetType().GetProperty(attachmentName + "Creation") != null) attachmentDetails.Add("Creation", (F_SQLiteField)sqLiteRecord.GetType().GetProperty(attachmentName + "Creation").GetValue(sqLiteRecord, null));
                        if (sqLiteRecord.GetType().GetProperty(attachmentName + "Modification") != null) attachmentDetails.Add("Modification", (F_SQLiteField)sqLiteRecord.GetType().GetProperty(attachmentName + "Modification").GetValue(sqLiteRecord, null));
                        if (sqLiteRecord.GetType().GetProperty(attachmentName + "Extension") != null) attachmentDetails.Add("Extension", (F_SQLiteField)sqLiteRecord.GetType().GetProperty(attachmentName + "Extension").GetValue(sqLiteRecord, null));
                        // Minimum for attachment validity
                        if (attachmentDetails.ContainsKey("Name") && attachmentDetails.ContainsKey("Size")) attachmentsDetails.Add(attachmentName, attachmentDetails);
                        attachmentDetails = null;
                        attachmentName = string.Empty;
                    }
                }
                else
                {
                    oValue = ((F_SQLiteField)sqLiteRecord.GetType().GetProperty(fField.Name).GetValue(sqLiteRecord, null)).Value;
                }

                int iLabelTabIndex = 3 + lstInvertedPanels.Count * 2;
                int iControlTabIndex = iLabelTabIndex + 1;

                switch (fField.EDbType)
                {
                    case DbType.Int32:
                    case DbType.Int64:
                    case DbType.Double:
                        Panel intPanel = new Panel();
                        Label intLabel = new Label();
                        intLabel.TabIndex = iLabelTabIndex;
                        intLabel.Text = fField.Name;
                        intLabel.TextAlign = ContentAlignment.MiddleRight;
                        intLabel.Dock = DockStyle.Top;
                        TextBox intTextBox = new TextBox();
                        if (ctlSelected == null) ctlSelected = intTextBox;
                        intTextBox.Enabled = !fField.ReadOnly && !sqLiteTable.ReadOnly && !fField.isAttachment;
                        intTextBox.TabIndex = iControlTabIndex;
                        intTextBox.Tag = fField;
                        intTextBox.TextAlign = HorizontalAlignment.Center;
                        if (oValue != null && oValue != DBNull.Value) intTextBox.Text = oValue.ToString();
                        intTextBox.Dock = DockStyle.Top;
                        intTextBox.ModifiedChanged += new EventHandler(controlValue_Modified);
                        intPanel.Controls.Add(intTextBox);
                        intPanel.Controls.Add(intLabel);
                        intPanel.Dock = DockStyle.Top;
                        intPanel.AutoSize = true;
                        lstInvertedPanels.Add(intPanel);
                        break;
                    case DbType.DateTime:
                        Panel dtPanel = new Panel();
                        Label dtLabel = new Label();
                        dtLabel.TabIndex = iLabelTabIndex;
                        dtLabel.Text = fField.Name;
                        dtLabel.TextAlign = ContentAlignment.MiddleRight;
                        dtLabel.Dock = DockStyle.Top;
                        DateTimePicker dtDTP = new DateTimePicker();
                        if (ctlSelected == null) ctlSelected = dtDTP;
                        dtDTP.Enabled = !fField.ReadOnly && !sqLiteTable.ReadOnly && !fField.isAttachment;
                        dtDTP.TabIndex = iControlTabIndex;
                        dtDTP.Tag = fField;
                        dtDTP.Format = DateTimePickerFormat.Custom;
                        dtDTP.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                        dtDTP.Value = Tools.getValidDateTime(oValue);
                        dtDTP.Dock = DockStyle.Top;
                        dtDTP.ValueChanged += new EventHandler(controlValue_Modified);
                        dtPanel.Controls.Add(dtDTP);
                        dtPanel.Controls.Add(dtLabel);
                        dtPanel.Dock = DockStyle.Top;
                        dtPanel.AutoSize = true;
                        lstInvertedPanels.Add(dtPanel);
                        break;
                    case DbType.Boolean:
                        Panel blPanel = new Panel();
                        Label blLabel = new Label();
                        blLabel.TabIndex = iLabelTabIndex;
                        blLabel.Text = fField.Name;
                        blLabel.TextAlign = ContentAlignment.MiddleRight;
                        blLabel.Dock = DockStyle.Top;
                        CheckBox blCheckBox = new CheckBox();
                        if (ctlSelected == null) ctlSelected = blCheckBox;
                        blCheckBox.Enabled = !fField.ReadOnly && !sqLiteTable.ReadOnly && !fField.isAttachment;
                        blCheckBox.TabIndex = iControlTabIndex;
                        blCheckBox.Tag = fField;
                        blCheckBox.CheckAlign = ContentAlignment.MiddleCenter;
                        if (oValue != null && oValue != DBNull.Value) blCheckBox.Checked = (bool)oValue;
                        blCheckBox.Dock = DockStyle.Top;
                        blCheckBox.Click += new EventHandler(controlValue_Modified);
                        blPanel.Controls.Add(blCheckBox);
                        blPanel.Controls.Add(blLabel);
                        blPanel.Dock = DockStyle.Top;
                        blPanel.AutoSize = true;
                        lstInvertedPanels.Add(blPanel);
                        break;
                    case DbType.Binary:
                        if (!fField.isAttachment) continue;
                        if (!attachmentsDetails.ContainsKey(fField.Name)) continue;
                        attachmentDetails = attachmentsDetails[fField.Name];
                        Panel filePanel = new Panel();
                        Label fileLabel = new Label();
                        fileLabel.TabIndex = iLabelTabIndex;
                        fileLabel.Text = fField.Name;
                        fileLabel.TextAlign = ContentAlignment.MiddleRight;
                        fileLabel.Dock = DockStyle.Top;
                        Button fileButton = new Button();
                        if (ctlSelected == null) ctlSelected = fileButton;
                        fileButton.Name = attachmentDetails["Binary"].Name;
                        fileButton.Enabled = !fField.ReadOnly && !sqLiteTable.ReadOnly;
                        fileButton.TabIndex = iControlTabIndex;
                        fileButton.Tag = attachmentDetails;
                        fileButton.TextAlign = ContentAlignment.MiddleCenter;
                        fileButton.Text = attachmentDetails["Name"].Value == null ? string.Empty : (string)attachmentDetails["Name"].Value;
                        fileButton.Dock = DockStyle.Top;
                        fileButton.Click += new EventHandler(fileButton_Click);
                        filePanel.Controls.Add(fileButton);
                        filePanel.Controls.Add(fileLabel);
                        filePanel.Dock = DockStyle.Top;
                        filePanel.AutoSize = true;
                        lstInvertedPanels.Add(filePanel);
                        break;
                    default:
                        // DbType.String:
                        // DbType.Object:
                        Panel stringPanel = new Panel();
                        Label stringLabel = new Label();
                        stringLabel.TabIndex = iLabelTabIndex;
                        stringLabel.Text = fField.Name;
                        stringLabel.TextAlign = ContentAlignment.MiddleRight;
                        stringLabel.Dock = DockStyle.Top;
                        TextBox stringTextBox = new TextBox();
                        if (ctlSelected == null) ctlSelected = stringTextBox;
                        stringTextBox.Enabled = !fField.ReadOnly && !sqLiteTable.ReadOnly && !fField.isAttachment;
                        stringTextBox.TabIndex = iControlTabIndex;
                        stringTextBox.Tag = fField;
                        if (fField.MaxLength == 0)          // Text
                        {
                            stringTextBox.Multiline = true;
                            stringTextBox.Height *= 3;
                        }
                        else if (fField.MaxLength > 255)    // Bigger than Emails or Path
                        {
                            stringTextBox.Multiline = true;
                            stringTextBox.Height *= 2;
                        }
                        else
                        {
                            stringTextBox.MaxLength = fField.MaxLength;
                        }
                        if (oValue != null && oValue != DBNull.Value) stringTextBox.Text = oValue.ToString();
                        stringTextBox.Dock = DockStyle.Top;
                        stringTextBox.ModifiedChanged += new EventHandler(controlValue_Modified);
                        stringPanel.Controls.Add(stringTextBox);
                        stringPanel.Controls.Add(stringLabel);
                        stringPanel.Dock = DockStyle.Top;
                        stringPanel.AutoSize = true;
                        lstInvertedPanels.Add(stringPanel);
                        break;
                }
            }

            for (int iPanel = lstInvertedPanels.Count - 1; iPanel >= 0; iPanel--) centerPanel.Controls.Add(lstInvertedPanels[iPanel]);

            if (ctlSelected != null) ctlSelected.Select();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            MethodInfo writeMethod = null;
            foreach (MethodInfo mInfo in typeof(SQLite806xDB).GetMethods())
            {
                if (mInfo.Name != "Write") continue;
                if (mInfo.GetParameters().Length != 1) continue;
                writeMethod = mInfo;
                break;
            }
            if (writeMethod == null) return;

            MethodInfo writeTypeMethod = writeMethod.MakeGenericMethod(sqLiteTable.Type);
            bool bResult = false;
            try
            {
                System.Collections.IList rList = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(sqLiteRecord.GetType()));
                rList.Add(sqLiteRecord);

                bResult = (bool)writeTypeMethod.Invoke(sqlDB806x, new object[] { rList });
                rList = null;

                if (!bResult) throw new Exception("Database change has failed.");
            }
            catch (Exception ex)
            {
                bResult = false;
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!bResult) return;

            ((Button)sender).Enabled = false;

            refreshLabelDescription();
            
            EventHandler recordUpdatedEventSubscribers = RecordUpdated;
            if (recordUpdatedEventSubscribers != null) recordUpdatedEventSubscribers(this, new EventArgs());
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Record will be removed from database, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            
            MethodInfo deleteMethod = null;
            foreach (MethodInfo mInfo in typeof(SQLite806xDB).GetMethods())
            {
                if (mInfo.Name != "Delete") continue;
                if (mInfo.GetParameters().Length != 1) continue;
                deleteMethod = mInfo;
                break;
            }
            if (deleteMethod == null) return;

            MethodInfo deleteTypeMethod = deleteMethod.MakeGenericMethod(sqLiteTable.Type);
            try
            {
                System.Collections.IList rList = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(sqLiteRecord.GetType()));
                rList.Add(sqLiteRecord);

                bool bResult = (bool)deleteTypeMethod.Invoke(sqlDB806x, new object[] { rList });
                rList = null;

                if (!bResult) throw new Exception("Removal has failed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            EventHandler recordRemovedEventSubscribers = RecordRemoved;
            if (recordRemovedEventSubscribers != null) recordRemovedEventSubscribers(this, new EventArgs());

            this.Close();
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            fileLabelToolStripMenuItem.Text = ((Button)sender).Text;
            fileLabelToolStripMenuItem.ToolTipText = fileLabelToolStripMenuItem.Text;
            Dictionary<string, F_SQLiteField> attachmentDetails = (Dictionary<string, F_SQLiteField>)((Button)sender).Tag;
            if (attachmentDetails["Size"].Value != null) fileLabelToolStripMenuItem.ToolTipText += "\r\nSize : " + ((Convert.ToInt32(attachmentDetails["Size"].Value)) / 1024).ToString() + " kB";
            if (attachmentDetails.ContainsKey("Creation"))
            {
                if (attachmentDetails["Creation"].Value != null) fileLabelToolStripMenuItem.ToolTipText += "\r\nAdded on : " + ((DateTime)attachmentDetails["Creation"].Value).ToString("yyyy-MM-dd HH:mm:ss");
                if (attachmentDetails.ContainsKey("Modification"))
                {
                    if (attachmentDetails["Creation"].Value != attachmentDetails["Modification"].Value) fileLabelToolStripMenuItem.ToolTipText += "\r\nUpdated on : " + ((DateTime)attachmentDetails["Modification"].Value).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            fileContextMenuStrip.Tag = attachmentDetails;
            fileContextMenuStrip.Show(Cursor.Position);
        }

        private void fileDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileContextMenuStrip.Tag == null) return;
            Dictionary<string, F_SQLiteField> attachmentDetails = (Dictionary<string, F_SQLiteField>)fileContextMenuStrip.Tag;

            SQLite806xTools.strategyFileDownload(attachmentDetails, ref saveFileDialogGeneric);
        }

        private void fileUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileContextMenuStrip.Tag == null) return;
            Dictionary<string, F_SQLiteField> attachmentDetails = (Dictionary<string, F_SQLiteField>)fileContextMenuStrip.Tag;

            if (SQLite806xTools.strategyFileUpdate(attachmentDetails, ref openFileDialogGeneric)) fileButtonRefresh();
        }

        private void fileRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileContextMenuStrip.Tag == null) return;
            Dictionary<string, F_SQLiteField> attachmentDetails = (Dictionary<string, F_SQLiteField>)fileContextMenuStrip.Tag;

            SQLite806xTools.strategyFileRemove(attachmentDetails);
            fileButtonRefresh();
        }

        private void fileButtonRefresh()
        {
            if (fileContextMenuStrip.Tag == null) return;
            Dictionary<string, F_SQLiteField> attachmentDetails = (Dictionary<string, F_SQLiteField>)fileContextMenuStrip.Tag;

            if (!attachmentDetails.ContainsKey("Binary")) return;
            string buttonText = string.Empty;
            if (attachmentDetails.ContainsKey("Name")) buttonText = attachmentDetails["Name"].Value == null ? string.Empty : (string)attachmentDetails["Name"].Value;

            Control[] arrControls = null;

            arrControls = this.Controls.Find(attachmentDetails["Binary"].Name, true);
            if (arrControls.Length == 1) ((Button)arrControls[0]).Text = buttonText;

            arrControls = this.Controls.Find("updateButton", true);
            if (arrControls.Length == 1) arrControls[0].Enabled = true;
            
            arrControls = null;
            attachmentDetails = null;
        }
    }
}
