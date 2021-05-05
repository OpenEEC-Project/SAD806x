using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using SQLite806x;

namespace SAD806x
{
    public partial class SQLite806xForm : Form
    {
        string sqlDB806xFilePath = string.Empty;
        SQLite806xDB sqlDB806x = null;
        SQLite806xRecordForm[] arrRecordForms = new SQLite806xRecordForm[10];

        public SQLite806xForm(string db806xFilePath)
        {
            sqlDB806xFilePath = db806xFilePath;

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.Load += new EventHandler(Form_Load);
            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            this.dbListViewContextMenuStrip.Opening += new CancelEventHandler(dbListViewContextMenuStrip_Opening);
            
            this.dbListView.MouseDoubleClick += new MouseEventHandler(dbListView_MouseDoubleClick);

            foreach (View vView in Enum.GetValues(typeof(View)))
            {
                ToolStripMenuItem tsItem = new ToolStripMenuItem();
                tsItem.Tag = vView;
                tsItem.Name = vView.ToString();
                switch (vView)
                {
                    case View.Details:
                    case View.List:
                    case View.Tile:
                        tsItem.Text = Enum.GetName(typeof(View), vView);
                        break;
                    case View.LargeIcon:
                        tsItem.Text = "Large Icon";
                        break;
                    case View.SmallIcon:
                        tsItem.Text = "Small Icon";
                        break;
                }
                tsItem.Click += new EventHandler(dbListViewViewContextMenuStrip_Click);
                viewToolStripMenuItem.DropDownItems.Add(tsItem);
            }
            dbListViewRefreshView();
        }

        private void dbListViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            addToolStripMenuItem.Visible = false;
            removeToolStripMenuItem.Visible = false;
            tableItemsToolStripSeparator.Visible = false;
            if (dbListView.Tag != null)
            {
                if (dbListView.Tag.GetType() == typeof(T_SQLiteTable))
                {
                    if (!((T_SQLiteTable)dbListView.Tag).ReadOnly)
                    {
                        addToolStripMenuItem.Visible = true;
                        if (dbListView.SelectedItems.Count > 0)
                        {
                            ListViewItem[] lviSelected = new ListViewItem[dbListView.SelectedItems.Count];
                            dbListView.SelectedItems.CopyTo(lviSelected, 0);

                            foreach (ListViewItem lvItem in lviSelected)
                            {
                                if (lvItem.Tag != null)
                                {
                                    removeToolStripMenuItem.Visible = true;
                                    removeToolStripMenuItem.Tag = lviSelected;
                                    break;
                                }
                            }
                            lviSelected = null;
                        }
                        tableItemsToolStripSeparator.Visible = true;
                    }
                }
            }
        }

        private void dbListViewViewContextMenuStrip_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag == null) return;
            if (((ToolStripMenuItem)sender).Tag.GetType() == typeof(View)) dbListView.View = (View)((ToolStripMenuItem)sender).Tag;

            dbListViewRefreshView();

            if (dbListView.View == View.Details) dbListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            else if (dbListView.View == View.List)
            {
                dbListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
                dbListView.Columns[0].Width = dbListView.Width / 3;
            }
            else dbListView.AutoArrange = true;
        }

        private void dbListViewRefreshView()
        {
            foreach (ToolStripMenuItem tsItem in viewToolStripMenuItem.DropDownItems)
            {
                tsItem.Checked = false;
                if (tsItem.Tag == null) continue;
                if (tsItem.Tag.GetType() == typeof(View)) tsItem.Checked = (View)tsItem.Tag == dbListView.View;
            }
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

            readDB();
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

        private void dbListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvSelectedItem = dbListView.GetItemAt(e.X, e.Y);
            if (lvSelectedItem == null) return;

            if (lvSelectedItem.Tag == null)
            {
                dbListView.Tag = null;
                readDB();
                return;
            }

            if (lvSelectedItem.Tag.GetType() == typeof(T_SQLiteTable))
            {
                dbListView.Tag = (T_SQLiteTable)lvSelectedItem.Tag;
                readTable((T_SQLiteTable)lvSelectedItem.Tag);
                return;
            }

            if (lvSelectedItem.Tag.GetType() == typeof(int))
            {
                openRecord((T_SQLiteTable)dbListView.Tag, (int)lvSelectedItem.Tag);
                return;
            }
        }

        private void readDB()
        {
            dbListView.BeginUpdate();

            dbListView.Items.Clear();

            List<T_SQLiteTable> tTables = sqlDB806x.validatedSchema();
            string tableName806xPrefix = typeof(R_806x_DB_Information).Name.Substring(2).Replace("DB_Information", string.Empty);

            foreach (T_SQLiteTable tTable in tTables)
            {
                if (!tTable.Name.StartsWith(tableName806xPrefix)) continue;
                if (!tTable.Visible) continue;

                ListViewItem lvItem = new ListViewItem(tTable.Label, 0);
                lvItem.Name = tTable.Name;
                lvItem.Tag = tTable;
                lvItem.ToolTipText = tTable.Description;
                ListViewItem.ListViewSubItem[] lvSubItems = new ListViewItem.ListViewSubItem[]
                        { new ListViewItem.ListViewSubItem(lvItem, "Table"),
                            new ListViewItem.ListViewSubItem(lvItem,
                            tTable.Description) };
                lvItem.SubItems.AddRange(lvSubItems);
                dbListView.Items.Add(lvItem);
            }

            dbListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            dbListView.EndUpdate();

            this.Text = "806x SqLite Database";
        }

        private void readTable(T_SQLiteTable tTable)
        {
            dbListView.BeginUpdate();

            dbListView.Items.Clear();

            ListViewItem lvBackItem = new ListViewItem("..", 0);
            ListViewItem.ListViewSubItem[] lvBackSubItems = new ListViewItem.ListViewSubItem[]
                        { new ListViewItem.ListViewSubItem(lvBackItem, "Back to"),
                            new ListViewItem.ListViewSubItem(lvBackItem,
                            "Parent") };
            lvBackItem.SubItems.AddRange(lvBackSubItems);
            dbListView.Items.Add(lvBackItem);

            MethodInfo readLabelDescriptionMethod = typeof(SQLite806xDB).GetMethod("ReadLabelDescription");
            if (readLabelDescriptionMethod == null) return;

            MethodInfo readLabelDescriptionTypeMethod = readLabelDescriptionMethod.MakeGenericMethod(tTable.Type);
            Dictionary<int, string[]> dResult = (Dictionary<int, string[]>)readLabelDescriptionTypeMethod.Invoke(sqlDB806x, null);
            if (dResult != null)
            {
                foreach (int rowId in dResult.Keys)
                {
                    string[] arrLabelDescription = dResult[rowId];
                    ListViewItem lvItem = new ListViewItem(arrLabelDescription[0], 1);
                    lvItem.Name = rowId.ToString();
                    lvItem.Tag = rowId;
                    lvItem.ToolTipText = arrLabelDescription[1];
                    ListViewItem.ListViewSubItem[] lvSubItems = new ListViewItem.ListViewSubItem[]
                        { new ListViewItem.ListViewSubItem(lvItem, "Record"),
                           new ListViewItem.ListViewSubItem(lvItem,
                           arrLabelDescription[1]) };
                    lvItem.SubItems.AddRange(lvSubItems);
                    dbListView.Items.Add(lvItem);
                }
            }

            dbListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            dbListView.EndUpdate();

            this.Text = tTable.Label + " - " + (dbListView.Items.Count - 1).ToString() + " record(s)";
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openRecord((T_SQLiteTable)dbListView.Tag, -1);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (removeToolStripMenuItem.Tag == null) return;

            string recordRecords = "Record";
            if (((ListViewItem[])removeToolStripMenuItem.Tag).Length > 1) recordRecords += "s";
            if (MessageBox.Show(recordRecords + " will be removed from database, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            foreach (ListViewItem lvItem in (ListViewItem[])removeToolStripMenuItem.Tag)
            {
                if (lvItem.Tag == null) continue;
                if (lvItem.Tag.GetType() != typeof(int)) continue;
                removeRecord((T_SQLiteTable)dbListView.Tag, (int)lvItem.Tag);
            }
        }

        private void removeRecord(T_SQLiteTable tTable, int rowId)
        {
            // Searching for opened form
            for (int iForm = 0; iForm < arrRecordForms.Length; iForm++)
            {
                if (arrRecordForms[iForm] == null) continue;

                if (arrRecordForms[iForm].Tag.ToString() == tTable.Name + "." + rowId.ToString())
                {
                    arrRecordForms[iForm].Close();
                    break;
                }
            }

            MethodInfo newRowMethod = typeof(SQLite806xDB).GetMethod("newRow");
            if (newRowMethod == null) return;

            MethodInfo newRowTypeMethod = newRowMethod.MakeGenericMethod(tTable.Type);
            object recordForRowId = newRowTypeMethod.Invoke(sqlDB806x, null);
            F_SQLiteField recordRowIdField = (F_SQLiteField)recordForRowId.GetType().GetProperty("RowId").GetValue(recordForRowId, null);
            recordRowIdField.Value = rowId;
            recordRowIdField = null;

            MethodInfo deleteMethod = null;
            foreach (MethodInfo mInfo in typeof(SQLite806xDB).GetMethods())
            {
                if (mInfo.Name != "Delete") continue;
                if (mInfo.GetParameters().Length != 1) continue;
                deleteMethod = mInfo;
                break;
            }
            if (deleteMethod == null) return;

            MethodInfo deleteTypeMethod = deleteMethod.MakeGenericMethod(tTable.Type);
            try
            {
                System.Collections.IList rList = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(recordForRowId.GetType()));
                rList.Add(recordForRowId);

                bool bResult = (bool)deleteTypeMethod.Invoke(sqlDB806x, new object[] { rList });
                rList = null;

                if (!bResult) throw new Exception("Removal has failed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Refresh
            if (tTable == (T_SQLiteTable)dbListView.Tag) readTable(tTable);
        }

        private void openRecord(T_SQLiteTable tTable, int rowId)
        {
            object rRecord = null;
            int iFirstFreeFormIndex = -1;
            
            // Searching for free/opened form
            for (int iForm = 0; iForm < arrRecordForms.Length; iForm++)
            {
                if (arrRecordForms[iForm] == null)
                {
                    if (iFirstFreeFormIndex < 0) iFirstFreeFormIndex = iForm;
                    continue;
                }

                if (arrRecordForms[iForm].Tag.ToString() == tTable.Name + "." + rowId.ToString())
                {
                    arrRecordForms[iForm].Show();
                    arrRecordForms[iForm].Focus();
                    return;
                }
            }
            if (iFirstFreeFormIndex < 0) return;

            System.Collections.IList lstResult = null;
            if (rowId < 0)
            {
                MethodInfo newRowMethod = typeof(SQLite806xDB).GetMethod("newRow");
                if (newRowMethod == null) return;

                MethodInfo newRowTypeMethod = newRowMethod.MakeGenericMethod(tTable.Type);
                lstResult = new List<object>() {newRowTypeMethod.Invoke(sqlDB806x, null)};
            }
            else
            {
                MethodInfo readMethod = null;
                foreach (MethodInfo mInfo in typeof(SQLite806xDB).GetMethods())
                {
                    if (mInfo.Name != "Read") continue;
                    if (mInfo.GetParameters().Length == 0) continue;
                    readMethod = mInfo;
                    break;
                }
                if (readMethod == null) return;

                MethodInfo readTypeMethod = readMethod.MakeGenericMethod(tTable.Type);
                lstResult = (System.Collections.IList)readTypeMethod.Invoke(sqlDB806x, new object[] { rowId, new List<string>() {}, string.Empty, string.Empty });
            }

            if (lstResult.Count != 1) return;

            rRecord = lstResult[0];
            lstResult = null;
            
            arrRecordForms[iFirstFreeFormIndex] = new SQLite806xRecordForm(ref sqlDB806x, ref tTable, ref rRecord);
            arrRecordForms[iFirstFreeFormIndex].FormClosed += new FormClosedEventHandler(recordForm_FormClosed);
            arrRecordForms[iFirstFreeFormIndex].RecordUpdated += new EventHandler(recordForm_RecordUpdated);
            arrRecordForms[iFirstFreeFormIndex].RecordRemoved += new EventHandler(recordForm_RecordRemoved);
            arrRecordForms[iFirstFreeFormIndex].Show();
            arrRecordForms[iFirstFreeFormIndex].Focus();
        }

        private void recordForm_RecordUpdated(object sender, EventArgs e)
        {
            string tableName = ((SQLite806xRecordForm)sender).Tag.ToString().Split('.')[0];
            string rowId = ((SQLite806xRecordForm)sender).Tag.ToString().Split('.')[1];
            if (((T_SQLiteTable)dbListView.Tag).Name != tableName) return;

            readTable((T_SQLiteTable)dbListView.Tag);
        }

        private void recordForm_RecordRemoved(object sender, EventArgs e)
        {
            string tableName = ((SQLite806xRecordForm)sender).Tag.ToString().Split('.')[0];
            string rowId = ((SQLite806xRecordForm)sender).Tag.ToString().Split('.')[1];
            if (((T_SQLiteTable)dbListView.Tag).Name != tableName) return;

            readTable((T_SQLiteTable)dbListView.Tag);
        }

        private void recordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int iForm = 0; iForm < arrRecordForms.Length; iForm++)
            {
                if (arrRecordForms[iForm] == null) continue;

                if (arrRecordForms[iForm].Tag.ToString() == ((SQLite806xRecordForm)sender).Tag.ToString())
                {
                    arrRecordForms[iForm] = null;
                    return;
                }
            }
        }
    }
}
