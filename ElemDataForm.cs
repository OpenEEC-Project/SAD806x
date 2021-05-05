using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class ElemDataForm : Form
    {
        private SADBin sadBin = null;
        private object s6xObject = null;
        private ContextMenuStrip MFElemContextMenuStrip = null;

        public ElemDataForm(ref SADBin elemDataSADBin, object elemS6xObject, ref ContextMenuStrip mfElemContextMenuStrip)
        {
            sadBin = elemDataSADBin;
            s6xObject = elemS6xObject;
            MFElemContextMenuStrip = mfElemContextMenuStrip;
            
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.Load += new EventHandler(Form_Load);
            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            elemDataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(elemDataGridView_DataBindingComplete);
            elemContextMenuStrip.Opening += new CancelEventHandler(elemContextMenuStrip_Opening);
            decimalToolStripMenuItem.Click += new EventHandler(decimalToolStripMenuItem_Click);
            decimalNotConvertedToolStripMenuItem.Click += new EventHandler(decimalNotConvertedToolStripMenuItem_Click);
            reverseOrderToolStripMenuItem.Click += new EventHandler(reverseOrderToolStripMenuItem_Click);
            convertToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(convertToolStripMenuItem_DropDownItemClicked);
            convertInputToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(convertInputToolStripMenuItem_DropDownItemClicked);

            if (MFElemContextMenuStrip != null)
            {
                foreach (ToolStripItem tsMI in MFElemContextMenuStrip.Items)
                {
                    if (tsMI.Name == decimalToolStripMenuItem.Name) decimalToolStripMenuItem.Checked = ((ToolStripMenuItem)tsMI).Checked;
                    else if (tsMI.Name == decimalNotConvertedToolStripMenuItem.Name) decimalNotConvertedToolStripMenuItem.Checked = ((ToolStripMenuItem)tsMI).Checked;
                    else if (tsMI.Name == reverseOrderToolStripMenuItem.Name) reverseOrderToolStripMenuItem.Checked = ((ToolStripMenuItem)tsMI).Checked;
                    else if (tsMI.Name == convertToolStripMenuItem.Name)
                    {
                        foreach (ToolStripMenuItem subMFMI in ((ToolStripMenuItem)tsMI).DropDownItems)
                        {
                            ToolStripMenuItem subMI = new ToolStripMenuItem();
                            subMI.Tag = subMFMI.Tag;
                            subMI.Text = subMFMI.Text;
                            subMI.ToolTipText = subMFMI.ToolTipText;
                            convertToolStripMenuItem.DropDownItems.Add(subMI);
                        }
                    }
                    else if (tsMI.Name == convertInputToolStripMenuItem.Name)
                    {
                        foreach (ToolStripMenuItem subMFMI in ((ToolStripMenuItem)tsMI).DropDownItems)
                        {
                            ToolStripMenuItem subMI = new ToolStripMenuItem();
                            subMI.Tag = subMFMI.Tag;
                            subMI.Text = subMFMI.Text;
                            subMI.ToolTipText = subMFMI.ToolTipText;
                            convertInputToolStripMenuItem.DropDownItems.Add(subMI);
                        }
                    }
                }
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.Text = sadBin.BinaryFileName;

            showElemData();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel) Exit();
        }

        private void Exit()
        {
            Dispose();

            GC.Collect();
        }

        // For Updating Columns and Rows Headers after Binding
        private void elemDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn dgVC in elemDataGridView.Columns)
            {
                dgVC.HeaderText = ((DataTable)elemDataGridView.DataSource).Columns[dgVC.Index].Caption;
                dgVC.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (elemDataGridView.Tag == null) return;
            object[] arrDefColsRows = (object[])elemDataGridView.Tag;
            if (arrDefColsRows.Length < 2) return;
            object[] arrRowsHeaders = (object[])arrDefColsRows[1];
            if (arrRowsHeaders == null) return;

            foreach (DataGridViewRow dgVR in elemDataGridView.Rows) if (arrRowsHeaders.Length > dgVR.Index) dgVR.HeaderCell.Value = arrRowsHeaders[dgVR.Index];
            arrRowsHeaders = null;
        }

        private void elemContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool additionalConversion = (decimalToolStripMenuItem.Checked == true);
            bool forFunction = false;

            if (s6xObject != null)
            {
                if (s6xObject.GetType() == typeof(S6xFunction)) forFunction = true;
            }

            convertToolStripSeparator.Visible = additionalConversion;
            convertToolStripMenuItem.Visible = additionalConversion;

            convertInputToolStripMenuItem.Visible = forFunction;
        }

        private void decimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showElemData();
        }

        private void decimalNotConvertedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showElemData();
        }

        private void reverseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showElemData();
        }

        private void convertToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            convertToolStripMenuItem.Tag = e.ClickedItem.Tag;
            showElemData();
        }

        private void convertInputToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            convertInputToolStripMenuItem.Tag = e.ClickedItem.Tag;
            showElemData();
        }

        private void showElemData()
        {
            if (sadBin == null || s6xObject == null) this.Close();

            S6xNavHeaderCategory headerCategory = S6xNavHeaderCategory.UNDEFINED;
            if (s6xObject.GetType() == typeof(S6xScalar))
            {
                headerCategory = S6xNavHeaderCategory.SCALARS;
                elemLabelTextBox.Text = ((S6xScalar)s6xObject).Label;
                elemBankTextBox.Text = ((S6xScalar)s6xObject).BankNum.ToString();
                elemAddressTextBox.Text = ((S6xScalar)s6xObject).Address;
            }
            else if (s6xObject.GetType() == typeof(S6xFunction))
            {
                headerCategory = S6xNavHeaderCategory.FUNCTIONS;
                elemLabelTextBox.Text = ((S6xFunction)s6xObject).Label;
                elemBankTextBox.Text = ((S6xFunction)s6xObject).BankNum.ToString();
                elemAddressTextBox.Text = ((S6xFunction)s6xObject).Address;
            }
            else if (s6xObject.GetType() == typeof(S6xTable))
            {
                headerCategory = S6xNavHeaderCategory.TABLES;
                elemLabelTextBox.Text = ((S6xTable)s6xObject).Label;
                elemBankTextBox.Text = ((S6xTable)s6xObject).BankNum.ToString();
                elemAddressTextBox.Text = ((S6xTable)s6xObject).Address;
            }
            else if (s6xObject.GetType() == typeof(S6xStructure))
            {
                headerCategory = S6xNavHeaderCategory.STRUCTURES;
                elemLabelTextBox.Text = ((S6xStructure)s6xObject).Label;
                elemBankTextBox.Text = ((S6xStructure)s6xObject).BankNum.ToString();
                elemAddressTextBox.Text = ((S6xStructure)s6xObject).Address;
            }

            if (headerCategory == S6xNavHeaderCategory.UNDEFINED) this.Close();

            ToolsElemData.showElemData(ref elemDataGridView, ref sadBin, s6xObject, decimalToolStripMenuItem.Checked, decimalNotConvertedToolStripMenuItem.Checked, reverseOrderToolStripMenuItem.Checked, (RepositoryConversionItem)convertToolStripMenuItem.Tag, (RepositoryConversionItem)convertInputToolStripMenuItem.Tag);
        }

        public void SetElemObject(object elemS6xObject)
        {
            s6xObject = elemS6xObject;

            showElemData();
        }

    }
}
