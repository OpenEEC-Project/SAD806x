using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class HexForm : Form
    {
        private DataTable dtBytes = null;

        public HexForm(string[] binBytes, SortedList slOrderedBanks)
        {
            DataColumn dcDc = null;
            int bankIndex = -1;
            int bankNum = -1;
            int bankStartAddress = -1;
            int bankEndAddress = -1;
            bool bankMode = false;
            
            dtBytes = new DataTable();

            dcDc = new DataColumn();
            dcDc.ColumnName = "Offset";
            dcDc.DataType = typeof(string);
            dtBytes.Columns.Add(dcDc);

            if (slOrderedBanks.Count > 0)
            {
                dcDc = new DataColumn();
                dcDc.ColumnName = "Bank Offset";
                dcDc.DataType = typeof(string);
                dtBytes.Columns.Add(dcDc);
                
                bankMode = true;
                bankIndex = -1;
            }
            
            for (int iCol = 0; iCol < 16; iCol++)
            {
                dcDc = new DataColumn();
                dcDc.Caption = string.Format("{0:X2}", iCol);
                dcDc.ColumnName = dcDc.Caption;
                dcDc.DataType = typeof(string);
                dtBytes.Columns.Add(dcDc);
            }

            dcDc = new DataColumn();
            dcDc.ColumnName = "Ascii";
            dcDc.DataType = typeof(string);
            dtBytes.Columns.Add(dcDc);

            dcDc = null;

            for (int iPos = 0; iPos < binBytes.Length; iPos+=16)
            {
                string[] hexLine = null;
                if (bankMode) hexLine = new string[19];
                else hexLine = new string[18];
                string asciiLine = string.Empty;

                hexLine[0] = string.Format("{0:X5}", iPos);
                if (bankMode)
                {
                    while (iPos > bankEndAddress)
                    {
                        bankIndex++;
                        if (bankIndex > slOrderedBanks.Count - 1)
                        {
                            bankStartAddress = binBytes.Length;
                            bankEndAddress = binBytes.Length;
                            break;
                        }
                        else
                        {
                            bankNum = ((int[])slOrderedBanks.GetByIndex(bankIndex))[0];
                            bankStartAddress = ((int[])slOrderedBanks.GetByIndex(bankIndex))[1];
                            bankEndAddress = ((int[])slOrderedBanks.GetByIndex(bankIndex))[2];
                        }
                    }
                    hexLine[1] = string.Empty;
                    if (iPos >= bankStartAddress && iPos <= bankEndAddress)
                    {
                        hexLine[1] = string.Format("{0,1} {1:X4}", bankNum, iPos - bankStartAddress + SADDef.EecBankStartAddress);
                    }
                }
                for (int subPos = 0; subPos < 16 && subPos + iPos < binBytes.Length; subPos++)
                {
                    if (bankMode) hexLine[subPos + 2] = binBytes[subPos + iPos].ToUpper();
                    else hexLine[subPos + 1] = binBytes[subPos + iPos].ToUpper();
                    asciiLine += Convert.ToChar(Convert.ToByte(binBytes[subPos + iPos], 16));
                }
                hexLine[hexLine.Length - 1] = asciiLine;
                dtBytes.LoadDataRow(hexLine, false);
            }

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            InitializeComponent();
        }

        private void HexForm_Load(object sender, EventArgs e)
        {
            hexDataGridView.DataSource = dtBytes;
            for (int iCol = 0; iCol < hexDataGridView.Columns.Count; iCol++)
            {
                if (iCol == 0)
                {
                    hexDataGridView.Columns[iCol].Width = 45;
                    hexDataGridView.Columns[iCol].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    hexDataGridView.Columns[iCol].DefaultCellStyle.BackColor = hexDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
                }
                else if (iCol == 1 && hexDataGridView.Columns.Count == 19)
                {
                    hexDataGridView.Columns[iCol].Width = 45;
                    hexDataGridView.Columns[iCol].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    hexDataGridView.Columns[iCol].DefaultCellStyle.BackColor = hexDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
                }
                else if (iCol == hexDataGridView.Columns.Count - 1)
                {
                    hexDataGridView.Columns[iCol].Width = 150;
                    hexDataGridView.Columns[iCol].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    hexDataGridView.Columns[iCol].DefaultCellStyle.Font = new Font(FontFamily.GenericMonospace, hexDataGridView.Font.Size);
                    hexDataGridView.Columns[iCol].DefaultCellStyle.BackColor = hexDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
                }
                else hexDataGridView.Columns[iCol].Width = 25;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sB = new StringBuilder(string.Empty);
            SortedList slCells = null;

            if (hexDataGridView.SelectedCells != null)
            {
                slCells = new SortedList();
                foreach (DataGridViewCell cCell in hexDataGridView.SelectedCells)
                {
                    switch (hexDataGridView.Columns[cCell.ColumnIndex].Name.ToUpper())
                    {
                        case "OFFSET":
                        case "BANK OFFSET":
                        case "ASCII":
                            break;
                        default:
                            slCells.Add(cCell.ColumnIndex + cCell.RowIndex * hexDataGridView.RowCount, cCell.Value);
                            break;
                    }
                }
                foreach (string cellText in slCells.Values) sB.Append(cellText);
                slCells = null;
            }
            Clipboard.SetText(sB.ToString());
            sB = null;
        }
    }
}
