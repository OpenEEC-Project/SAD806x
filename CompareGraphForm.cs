using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;

namespace SAD806x
{
    public partial class CompareGraphForm : Form
    {
        private SADBin sadBin = null;
        private SADS6x sadS6x = null;
        private byte[] arrCurBytes = null;
        private SADBin cmpSadBin = null;
        private SADS6x cmpSadS6x = null;
        private byte[] arrCmpBytes = null;
        private TreeView elemsTreeView = null;

        public CompareGraphForm(ref SADBin mainSadBin, ref SADS6x mainSadS6x, ref TreeView mainElemsTreeView)
        {
            sadBin = mainSadBin;
            sadS6x = mainSadS6x;
            elemsTreeView = mainElemsTreeView;

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }


            string[] chartTypes = Enum.GetNames(typeof(SeriesChartType));
            foreach (string chartType in chartTypes)
            {
                switch ((SeriesChartType)Enum.Parse(typeof(SeriesChartType), chartType, true))
                {
                    case SeriesChartType.Bubble:
                    case SeriesChartType.StackedArea100:
                    case SeriesChartType.StackedBar100:
                    case SeriesChartType.StackedColumn100:
                    case SeriesChartType.SplineArea:
                    case SeriesChartType.Pie:
                    case SeriesChartType.Doughnut:
                    case SeriesChartType.SplineRange:
                    case SeriesChartType.Radar:
                    case SeriesChartType.Polar:
                    case SeriesChartType.ErrorBar:
                    case SeriesChartType.Renko:
                    case SeriesChartType.Funnel:
                    case SeriesChartType.Pyramid:
                        // Not working
                        continue;
                }
                chartStyleToolStripMenuItem.DropDownItems.Add(chartType, null, chartStyle_Click);
            }
            chartStyleToolStripMenuItem.DropDownOpening += new EventHandler(chartStyleToolStripMenuItem_DropDownOpening);

            List<Color> allColors = new List<Color>();
            foreach (PropertyInfo property in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                if (property.PropertyType == typeof(Color)) allColors.Add((Color)property.GetValue(null, null));
            }
            List<Color> lColors = new List<Color>();
            foreach (Color cColor in allColors)
            {
                switch (cColor.Name)
                {
                    case "Black":
                    case "Blue":
                    case "Cyan":
                    case "Gray":
                    case "Green":
                    case "Lime":
                    case "Magenta":
                    case "Maroon":
                    case "Navy":
                    case "Olive":
                    case "Orange":
                    case "Pink":
                    case "Purple":
                    case "Red":
                    case "Silver":
                    case "Teal":
                    case "Transparent":
                    case "White":
                    case "Yellow":
                        lColors.Add(cColor);
                        break;
                }
            }
            foreach (Color chartColor in lColors) chartColorToolStripMenuItem.DropDownItems.Add(chartColor.Name, null, chartColor_Click);
            chartColorToolStripMenuItem.DropDownOpening += new EventHandler(chartColorToolStripMenuItem_DropDownOpening);
            foreach (Color chartColor in lColors) chartBackColorToolStripMenuItem.DropDownItems.Add(chartColor.Name, null, chartBackColor_Click);
            chartBackColorToolStripMenuItem.DropDownOpening += new EventHandler(chartBackColorToolStripMenuItem_DropDownOpening);

            calibCurrentChart.Visible = false;
            calibComparedChart.Visible = false;

            calibCurrentChart.Series[0].Color = Color.Blue;
            calibComparedChart.Series[0].Color = Color.Green;

            calibCurrentChart.ChartAreas[0].BackColor = Color.White;
            calibComparedChart.ChartAreas[0].BackColor = Color.White;

            calibCurrentChart.MouseWheel += new MouseEventHandler(calibChart_MouseWheel);
            calibCurrentChart.MouseClick += new MouseEventHandler(calibChart_MouseClick);

            calibComparedChart.MouseWheel += new MouseEventHandler(calibChart_MouseWheel);
            calibComparedChart.MouseClick += new MouseEventHandler(calibChart_MouseClick);

            chartContextMenuStrip.Opening += new CancelEventHandler(chartContextMenuStrip_Opening);
        }

        private void chartContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ((ContextMenuStrip)sender).Tag = ((ContextMenuStrip)sender).SourceControl;
        }

        private void chartStyleToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            Chart cChart = (Chart)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).Tag;
            foreach (ToolStripMenuItem mItem in ((ToolStripMenuItem)sender).DropDownItems) mItem.Checked = ((SeriesChartType)Enum.Parse(typeof(SeriesChartType), mItem.Text, true) == cChart.Series[0].ChartType);
        }

        private void chartStyle_Click(object sender, EventArgs e)
        {
            calibCurrentChart.Series[0].ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), ((ToolStripMenuItem)sender).Text, true);
            calibComparedChart.Series[0].ChartType = calibCurrentChart.Series[0].ChartType;
            foreach (ToolStripMenuItem mItem in chartStyleToolStripMenuItem.DropDownItems) mItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void chartColorToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            Chart cChart = (Chart)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).Tag;
            foreach (ToolStripMenuItem mItem in ((ToolStripMenuItem)sender).DropDownItems) mItem.Checked = (mItem.Text == cChart.Series[0].Color.Name);
        }

        private void chartBackColorToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            Chart cChart = (Chart)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).Tag;
            foreach (ToolStripMenuItem mItem in ((ToolStripMenuItem)sender).DropDownItems) mItem.Checked = (mItem.Text == cChart.ChartAreas[0].BackColor.Name);
        }

        private void chartColor_Click(object sender, EventArgs e)
        {
            Chart cChart = (Chart)((ContextMenuStrip)((ToolStripMenuItem)sender).OwnerItem.Owner).Tag;
            cChart.Series[0].Color = Color.FromName(((ToolStripMenuItem)sender).Text);
        }

        private void chartBackColor_Click(object sender, EventArgs e)
        {
            Chart cChart = (Chart)((ContextMenuStrip)((ToolStripMenuItem)sender).OwnerItem.Owner).Tag;
            cChart.ChartAreas[0].BackColor = Color.FromName(((ToolStripMenuItem)sender).Text);
        }

        private void calibChart_MouseClick(object sender, MouseEventArgs e)
        {
            Chart cChart = (Chart)sender;
            Axis xAxis = cChart.ChartAreas[0].AxisX;
            double pPos = (int)xAxis.PixelPositionToValue(e.Location.X);
            foreach (CustomLabel cLabel in xAxis.CustomLabels)
            {
                if (pPos >= cLabel.FromPosition && pPos <= cLabel.ToPosition)
                {
                    if (cLabel.Tag == null) return;
                    if (cLabel.Tag.GetType() == typeof(S6xTable))
                    {
                        try
                        {
                            S6xTable s6xTable = (S6xTable)cLabel.Tag;
                            mainToolTip.SetToolTip((Control)sender, s6xTable.Label + "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments);
                            if (cChart == calibCurrentChart) elemsTreeView.SelectedNode = elemsTreeView.Nodes["TABLES"].Nodes[s6xTable.UniqueAddress];
                        }
                        catch { }
                    }
                    else if (cLabel.Tag.GetType() == typeof(S6xFunction))
                    {
                        try
                        {
                            S6xFunction s6xFunction = (S6xFunction)cLabel.Tag;
                            mainToolTip.SetToolTip((Control)sender, s6xFunction.Label + "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments);
                            if (cChart == calibCurrentChart) elemsTreeView.SelectedNode = elemsTreeView.Nodes["FUNCTIONS"].Nodes[s6xFunction.UniqueAddress];
                        }
                        catch { }
                    }
                    return;
                }
            }
        }

        private void CompareGraphForm_Load(object sender, EventArgs e)
        {
            this.Text = getTitle();

            string sError = string.Empty;

            bool bError = false;

            bError = (!sadBin.isDisassembled);
            if (bError)
            {
                sError += "Binary is not disassembled.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Windows.Forms.Cursor processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string[] arrSBytes = sadBin.Calibration.getBytesArray(sadBin.Calibration.AddressBankInt, sadBin.Calibration.Size);
            arrCurBytes = new byte[arrSBytes.Length];
            for (int iPos = 0; iPos < arrCurBytes.Length; iPos++) arrCurBytes[iPos] = Convert.ToByte(arrSBytes[iPos], 16);

            setupChart(ref calibCurrentChart, ref arrCurBytes, ref sadBin, ref sadS6x);

            Cursor = processPreviousCursor;

            calibCurrentChart.Visible = true;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sadBin = null;
            sadS6x = null;
            arrCurBytes = null;
            cmpSadBin = null;
            cmpSadS6x = null;
            arrCmpBytes = null;

            Dispose();

            GC.Collect();
        }

        private void openBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cmpBinaryFilePath = string.Empty;
            string cmpS6xFilePath = string.Empty;

            string sError = string.Empty;

            bool bError = false;

            if (openFileDialogBin.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(openFileDialogBin.FileName))
            {
                sError += openFileDialogBin.FileName + "\r\n";
                sError += "Not existing Binary.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmpBinaryFilePath = openFileDialogBin.FileName;

            FileInfo fiFI = new FileInfo(cmpBinaryFilePath);
            cmpS6xFilePath = fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".s6x";
            fiFI = null;

            if (!File.Exists(cmpS6xFilePath)) cmpS6xFilePath = string.Empty;

            cmpSadBin = null;
            cmpSadS6x = null;

            System.Windows.Forms.Cursor processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            cmpSadBin = new SADBin(cmpBinaryFilePath, cmpS6xFilePath);
            cmpSadS6x = cmpSadBin.S6x;

            bError = (cmpSadBin == null);
            if (bError)
            {
                sError += cmpBinaryFilePath + "\r\n";
                sError += "Unrecognized Binary.";
                Cursor = processPreviousCursor;
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = getTitle();
                return;
            }

            bError = (!cmpSadBin.isLoaded || !cmpSadBin.isValid);
            if (bError)
            {
                sError += cmpBinaryFilePath + "\r\n";
                sError += "Unrecognized Binary.";
                Cursor = processPreviousCursor;
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = getTitle();
                return;
            }

            cmpSadBin.processBin();
            bError = (!cmpSadBin.isDisassembled);
            if (bError)
            {
                sError += cmpBinaryFilePath + "\r\n";
                sError += "Binary can not be disassembled.";
                Cursor = processPreviousCursor;
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = getTitle();
                return;
            }

            string[] arrSBytes = cmpSadBin.Calibration.getBytesArray(cmpSadBin.Calibration.AddressBankInt, cmpSadBin.Calibration.Size);
            arrCmpBytes = new byte[arrSBytes.Length];
            for (int iPos = 0; iPos < arrCmpBytes.Length; iPos++) arrCmpBytes[iPos] = Convert.ToByte(arrSBytes[iPos], 16);
            arrSBytes = null;

            setupChart(ref calibComparedChart, ref arrCmpBytes, ref cmpSadBin, ref cmpSadS6x);

            Cursor = processPreviousCursor;

            calibComparedChart.Visible = true;

            this.Text = getTitle();
        }

        private string getTitle()
        {
            string bseAppTitle = "Calibration Chart View";
            string addAppTitle = string.Empty;

            if (sadBin == null) return bseAppTitle;

            addAppTitle += sadBin.BinaryFileName;
            if (sadS6x.isValid && sadS6x.isSaved) addAppTitle += " / " + sadS6x.FileName;
            if (sadBin.isValid)
            {
                if (sadBin.Calibration.Info.VidStrategy != string.Empty) addAppTitle += " / " + sadBin.Calibration.Info.VidStrategy + "(" + sadBin.Calibration.Info.VidStrategyVersion + ")";
            }

            if (cmpSadBin != null)
            {
                addAppTitle += " VS " + cmpSadBin.BinaryFileName;
                if (cmpSadS6x.isValid && cmpSadS6x.isSaved) addAppTitle += " / " + cmpSadS6x.FileName;
                if (cmpSadBin.isValid)
                {
                    if (cmpSadBin.Calibration.Info.VidStrategy != string.Empty) addAppTitle += " / " + cmpSadBin.Calibration.Info.VidStrategy + "(" + cmpSadBin.Calibration.Info.VidStrategyVersion + ")";
                }
            }

            return string.Format("{0} - {1}", bseAppTitle, addAppTitle);
        }

        private void setupChart(ref Chart cChart, ref byte[] arrBytes, ref SADBin cBin, ref SADS6x cS6x)
        {
            cChart.Series[0].Points.DataBindY(arrBytes);

            string legendText = cBin.BinaryFileName;
            if (cBin.Calibration.Info.VidStrategy != string.Empty) legendText += " - " + cBin.Calibration.Info.VidStrategy + "(" + cBin.Calibration.Info.VidStrategyVersion + ")";
            cChart.Series[0].LegendText = legendText;

            string legentToolTip = string.Empty;
            if (cBin.Calibration.Info.is8061) legentToolTip += "8061";
            else legentToolTip += "8065";
            legentToolTip += " binary.\r\nBanks :";
            foreach (string[] bankInfos in cBin.Calibration.Info.slBanksInfos.Values) legentToolTip += string.Format("\r\n{0,4}{1,8}{2,3}{3,6}", bankInfos[0], bankInfos[1], "=>", bankInfos[2]);
            legentToolTip += "\r\n\r\n";
            if (cS6x.isValid) legentToolTip += cS6x.FileName;
            else legentToolTip += "No SAD 806x File";

            cChart.Series[0].LegendToolTip = legentToolTip;
            if (cChart == calibComparedChart) cChart.Series[0].Color = Color.Green;

            cChart.ChartAreas[0].AxisY.Minimum = 0.0;
            cChart.ChartAreas[0].AxisY.Maximum = 255.0;
            cChart.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
            cChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            cChart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;

            cChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            cChart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            cChart.ChartAreas[0].AxisX.CustomLabels.Clear();
            cChart.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;   // To remove non interesting buttons

            cChart.ChartAreas[0].AxisX.ScaleView.Size = (double)(sadBin.Calibration.Size / 10);  // For both Charts to be aligned

            setupChartLabels(ref cChart, ref arrBytes, ref cBin, ref cS6x);
        }

        private void setupChartLabels(ref Chart cChart, ref byte[] arrBytes, ref SADBin cBin, ref SADS6x cS6x)
        {
            foreach (S6xTable s6xTable in cS6x.slTables.Values)
            {
                if (s6xTable.Skip) continue;
                if (!s6xTable.isCalibrationElement) continue;
                if (s6xTable.ColsNumber <= 0 || s6xTable.RowsNumber <= 0) continue;

                int arrBytesAddressInt = s6xTable.AddressInt - cBin.Calibration.AddressBankInt;
                int size = (s6xTable.ColsNumber * s6xTable.RowsNumber) * ((s6xTable.WordOutput) ? 2 : 1);

                if (arrBytesAddressInt >= arrBytes.Length) continue;
                if (arrBytesAddressInt + size >= arrBytes.Length) continue;

                CustomLabel cLabel = new CustomLabel();
                cLabel.FromPosition = arrBytesAddressInt;
                cLabel.ToPosition = arrBytesAddressInt + size;
                cLabel.Text = s6xTable.ShortLabel;
                cLabel.ToolTip = s6xTable.Label;
                cLabel.Tag = s6xTable;
                cChart.ChartAreas[0].AxisX.CustomLabels.Add(cLabel);
            }

            foreach (S6xFunction s6xFunction in cS6x.slFunctions.Values)
            {
                if (s6xFunction.Skip) continue;
                if (!s6xFunction.isCalibrationElement) continue;
                if (s6xFunction.RowsNumber <= 0) continue;

                int arrBytesAddressInt = s6xFunction.AddressInt - cBin.Calibration.AddressBankInt;
                int size = s6xFunction.RowsNumber * (((s6xFunction.ByteInput) ? 1 : 2) + ((s6xFunction.ByteOutput) ? 1 : 2));

                if (arrBytesAddressInt >= arrBytes.Length) continue;
                if (arrBytesAddressInt + size >= arrBytes.Length) continue;

                CustomLabel cLabel = new CustomLabel();
                cLabel.FromPosition = arrBytesAddressInt;
                cLabel.ToPosition = arrBytesAddressInt + size;
                cLabel.Text = s6xFunction.ShortLabel;
                cLabel.ToolTip = s6xFunction.Label;
                cLabel.Tag = s6xFunction;
                cChart.ChartAreas[0].AxisX.CustomLabels.Add(cLabel);
            }
        }
        
        private void calibChart_MouseWheel(object sender, MouseEventArgs e)
        {
            Chart cChart = (Chart)sender;
            Axis xAxis = cChart.ChartAreas[0].AxisX;
            double dWheelEffect = 2.0;
            double maxSize = (double)(sadBin.Calibration.Size / 10);  // For both Charts to be aligned
            double originalViewPos = xAxis.ScaleView.Position;
            double originalMousePos = xAxis.PixelPositionToValue(e.Location.X) ;
            double originalViewSize = xAxis.ScaleView.Size;
            double relativeMousePos = (originalMousePos - originalViewPos) / originalViewSize;

            if (e.Delta > 0) xAxis.ScaleView.Size = originalViewSize / dWheelEffect;
            else if (originalViewSize * dWheelEffect > maxSize) return;
            else xAxis.ScaleView.Size = originalViewSize * dWheelEffect;
            xAxis.ScaleView.Position = originalMousePos - relativeMousePos * xAxis.ScaleView.Size;
        }
    }
}
