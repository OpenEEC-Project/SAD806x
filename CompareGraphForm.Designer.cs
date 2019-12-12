namespace SAD806x
{
    partial class CompareGraphForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.openFileDialogBin = new System.Windows.Forms.OpenFileDialog();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibCurrentChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chartStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartSplitContainer = new System.Windows.Forms.SplitContainer();
            this.calibComparedChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chartColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartBackColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calibCurrentChart)).BeginInit();
            this.chartContextMenuStrip.SuspendLayout();
            this.chartSplitContainer.Panel1.SuspendLayout();
            this.chartSplitContainer.Panel2.SuspendLayout();
            this.chartSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calibComparedChart)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialogBin
            // 
            this.openFileDialogBin.Filter = "Binary File|*.bin|All Files|*.*";
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1085, 24);
            this.mainMenuStrip.TabIndex = 4;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBinaryToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openBinaryToolStripMenuItem
            // 
            this.openBinaryToolStripMenuItem.Name = "openBinaryToolStripMenuItem";
            this.openBinaryToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.openBinaryToolStripMenuItem.Text = "Disassemble Comparison Binary";
            this.openBinaryToolStripMenuItem.Click += new System.EventHandler(this.openBinaryToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(240, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // calibCurrentChart
            // 
            chartArea1.AxisY.LabelStyle.TruncatedLabels = true;
            chartArea1.AxisY.ScaleView.MinSizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisY.ScaleView.SizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.AxisY.ScaleView.Zoomable = false;
            chartArea1.CursorY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.Name = "ChartArea1";
            this.calibCurrentChart.ChartAreas.Add(chartArea1);
            this.calibCurrentChart.ContextMenuStrip = this.chartContextMenuStrip;
            this.calibCurrentChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.calibCurrentChart.Legends.Add(legend1);
            this.calibCurrentChart.Location = new System.Drawing.Point(0, 0);
            this.calibCurrentChart.Name = "calibCurrentChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.calibCurrentChart.Series.Add(series1);
            this.calibCurrentChart.Size = new System.Drawing.Size(1085, 242);
            this.calibCurrentChart.TabIndex = 5;
            this.calibCurrentChart.Text = "Current Binary Calibration Chart";
            // 
            // chartContextMenuStrip
            // 
            this.chartContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chartStyleToolStripMenuItem,
            this.chartColorToolStripMenuItem,
            this.chartBackColorToolStripMenuItem});
            this.chartContextMenuStrip.Name = "chartContextMenuStrip";
            this.chartContextMenuStrip.Size = new System.Drawing.Size(127, 70);
            // 
            // chartStyleToolStripMenuItem
            // 
            this.chartStyleToolStripMenuItem.Name = "chartStyleToolStripMenuItem";
            this.chartStyleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.chartStyleToolStripMenuItem.Text = "Style";
            // 
            // chartSplitContainer
            // 
            this.chartSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartSplitContainer.Location = new System.Drawing.Point(0, 24);
            this.chartSplitContainer.Name = "chartSplitContainer";
            this.chartSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // chartSplitContainer.Panel1
            // 
            this.chartSplitContainer.Panel1.Controls.Add(this.calibCurrentChart);
            // 
            // chartSplitContainer.Panel2
            // 
            this.chartSplitContainer.Panel2.Controls.Add(this.calibComparedChart);
            this.chartSplitContainer.Size = new System.Drawing.Size(1085, 484);
            this.chartSplitContainer.SplitterDistance = 242;
            this.chartSplitContainer.TabIndex = 6;
            // 
            // calibComparedChart
            // 
            chartArea2.Name = "ChartArea1";
            this.calibComparedChart.ChartAreas.Add(chartArea2);
            this.calibComparedChart.ContextMenuStrip = this.chartContextMenuStrip;
            this.calibComparedChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.Name = "Legend1";
            this.calibComparedChart.Legends.Add(legend2);
            this.calibComparedChart.Location = new System.Drawing.Point(0, 0);
            this.calibComparedChart.Name = "calibComparedChart";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.calibComparedChart.Series.Add(series2);
            this.calibComparedChart.Size = new System.Drawing.Size(1085, 238);
            this.calibComparedChart.TabIndex = 6;
            this.calibComparedChart.Text = "Compared Binary Calibration Chart";
            // 
            // mainToolTip
            // 
            this.mainToolTip.Active = false;
            this.mainToolTip.AutoPopDelay = 60000;
            this.mainToolTip.InitialDelay = 500;
            this.mainToolTip.ReshowDelay = 100;
            // 
            // chartColorToolStripMenuItem
            // 
            this.chartColorToolStripMenuItem.Name = "chartColorToolStripMenuItem";
            this.chartColorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.chartColorToolStripMenuItem.Text = "Color";
            // 
            // chartBackColorToolStripMenuItem
            // 
            this.chartBackColorToolStripMenuItem.Name = "chartBackColorToolStripMenuItem";
            this.chartBackColorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.chartBackColorToolStripMenuItem.Text = "Backcolor";
            // 
            // CompareGraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1085, 508);
            this.Controls.Add(this.chartSplitContainer);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "CompareGraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CompareGraphForm";
            this.Load += new System.EventHandler(this.CompareGraphForm_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calibCurrentChart)).EndInit();
            this.chartContextMenuStrip.ResumeLayout(false);
            this.chartSplitContainer.Panel1.ResumeLayout(false);
            this.chartSplitContainer.Panel2.ResumeLayout(false);
            this.chartSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calibComparedChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogBin;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBinaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.DataVisualization.Charting.Chart calibCurrentChart;
        private System.Windows.Forms.SplitContainer chartSplitContainer;
        private System.Windows.Forms.DataVisualization.Charting.Chart calibComparedChart;
        private System.Windows.Forms.ContextMenuStrip chartContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem chartStyleToolStripMenuItem;
        private System.Windows.Forms.ToolTip mainToolTip;
        private System.Windows.Forms.ToolStripMenuItem chartColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartBackColorToolStripMenuItem;

    }
}