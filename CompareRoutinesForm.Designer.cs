namespace SAD806x
{
    partial class CompareRoutinesForm
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
            this.selectButton = new System.Windows.Forms.Button();
            this.compareTreeView = new System.Windows.Forms.TreeView();
            this.comparePanel = new System.Windows.Forms.Panel();
            this.compareButton = new System.Windows.Forms.Button();
            this.optionPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.distToleranceTextBox = new System.Windows.Forms.TextBox();
            this.gapToleranceTextBox = new System.Windows.Forms.TextBox();
            this.minOpsCountTextBox = new System.Windows.Forms.TextBox();
            this.sFileNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialogSkt = new System.Windows.Forms.OpenFileDialog();
            this.exportReportButton = new System.Windows.Forms.Button();
            this.saveFileDialogSkr = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogBin = new System.Windows.Forms.OpenFileDialog();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBarMain = new System.Windows.Forms.ToolStripProgressBar();
            this.resultCategContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.filterDefinedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterShortLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterUniqueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultElemContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comparePanel.SuspendLayout();
            this.optionPanel.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.resultCategContextMenuStrip.SuspendLayout();
            this.resultElemContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectButton
            // 
            this.selectButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectButton.Location = new System.Drawing.Point(0, 0);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(532, 22);
            this.selectButton.TabIndex = 1;
            this.selectButton.Text = "Select Skeleton";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // compareTreeView
            // 
            this.compareTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compareTreeView.Location = new System.Drawing.Point(0, 141);
            this.compareTreeView.Name = "compareTreeView";
            this.compareTreeView.ShowNodeToolTips = true;
            this.compareTreeView.Size = new System.Drawing.Size(532, 263);
            this.compareTreeView.TabIndex = 6;
            // 
            // comparePanel
            // 
            this.comparePanel.Controls.Add(this.compareButton);
            this.comparePanel.Controls.Add(this.optionPanel);
            this.comparePanel.Controls.Add(this.selectButton);
            this.comparePanel.Controls.Add(this.label1);
            this.comparePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.comparePanel.Location = new System.Drawing.Point(0, 0);
            this.comparePanel.Name = "comparePanel";
            this.comparePanel.Size = new System.Drawing.Size(532, 141);
            this.comparePanel.TabIndex = 3;
            // 
            // compareButton
            // 
            this.compareButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.compareButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.compareButton.Enabled = false;
            this.compareButton.Location = new System.Drawing.Point(0, 105);
            this.compareButton.Name = "compareButton";
            this.compareButton.Size = new System.Drawing.Size(532, 22);
            this.compareButton.TabIndex = 5;
            this.compareButton.Text = "Compare";
            this.compareButton.UseVisualStyleBackColor = true;
            this.compareButton.Click += new System.EventHandler(this.compareButton_Click);
            // 
            // optionPanel
            // 
            this.optionPanel.Controls.Add(this.splitContainer1);
            this.optionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.optionPanel.Location = new System.Drawing.Point(0, 22);
            this.optionPanel.Name = "optionPanel";
            this.optionPanel.Size = new System.Drawing.Size(532, 83);
            this.optionPanel.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.fileNameLabel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Panel2.Controls.Add(this.distToleranceTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.gapToleranceTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.minOpsCountTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.sFileNameTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(532, 83);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Distance Minimum Tolerance %";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(167, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Count Gap Maximum Tolerance %";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Minimum Operations Count";
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(0, 3);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(65, 13);
            this.fileNameLabel.TabIndex = 0;
            this.fileNameLabel.Text = "Skeleton file";
            // 
            // distToleranceTextBox
            // 
            this.distToleranceTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.distToleranceTextBox.Location = new System.Drawing.Point(0, 60);
            this.distToleranceTextBox.Name = "distToleranceTextBox";
            this.distToleranceTextBox.Size = new System.Drawing.Size(265, 20);
            this.distToleranceTextBox.TabIndex = 4;
            this.distToleranceTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // gapToleranceTextBox
            // 
            this.gapToleranceTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.gapToleranceTextBox.Location = new System.Drawing.Point(0, 40);
            this.gapToleranceTextBox.Name = "gapToleranceTextBox";
            this.gapToleranceTextBox.Size = new System.Drawing.Size(265, 20);
            this.gapToleranceTextBox.TabIndex = 3;
            this.gapToleranceTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // minOpsCountTextBox
            // 
            this.minOpsCountTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.minOpsCountTextBox.Location = new System.Drawing.Point(0, 20);
            this.minOpsCountTextBox.Name = "minOpsCountTextBox";
            this.minOpsCountTextBox.Size = new System.Drawing.Size(265, 20);
            this.minOpsCountTextBox.TabIndex = 2;
            this.minOpsCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // sFileNameTextBox
            // 
            this.sFileNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.sFileNameTextBox.Location = new System.Drawing.Point(0, 0);
            this.sFileNameTextBox.Name = "sFileNameTextBox";
            this.sFileNameTextBox.ReadOnly = true;
            this.sFileNameTextBox.Size = new System.Drawing.Size(265, 20);
            this.sFileNameTextBox.TabIndex = 0;
            this.sFileNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(532, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Result gives possible matchings";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // openFileDialogSkt
            // 
            this.openFileDialogSkt.Filter = "Skeleton File|*.skt|All Files|*.*";
            // 
            // exportReportButton
            // 
            this.exportReportButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.exportReportButton.Enabled = false;
            this.exportReportButton.Location = new System.Drawing.Point(0, 404);
            this.exportReportButton.Name = "exportReportButton";
            this.exportReportButton.Size = new System.Drawing.Size(532, 22);
            this.exportReportButton.TabIndex = 7;
            this.exportReportButton.Text = "Export report";
            this.exportReportButton.UseVisualStyleBackColor = true;
            this.exportReportButton.Click += new System.EventHandler(this.exportReportButton_Click);
            // 
            // saveFileDialogSkr
            // 
            this.saveFileDialogSkr.Filter = "Skeleton Report File|*.skr|All Files|*.*";
            // 
            // openFileDialogBin
            // 
            this.openFileDialogBin.Filter = "Binary File|*.bin|All Files|*.*";
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBarMain});
            this.statusStripMain.Location = new System.Drawing.Point(0, 426);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(532, 22);
            this.statusStripMain.TabIndex = 6;
            this.statusStripMain.Text = "statusStripMain";
            // 
            // toolStripProgressBarMain
            // 
            this.toolStripProgressBarMain.Name = "toolStripProgressBarMain";
            this.toolStripProgressBarMain.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBarMain.Value = 100;
            // 
            // resultCategContextMenuStrip
            // 
            this.resultCategContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.collapseToolStripMenuItem,
            this.toolStripSeparator1,
            this.filterDefinedToolStripMenuItem,
            this.filterShortLabelToolStripMenuItem,
            this.filterUniqueToolStripMenuItem,
            this.toolStripSeparator2,
            this.importElementsToolStripMenuItem});
            this.resultCategContextMenuStrip.Name = "resultCategContextMenuStrip";
            this.resultCategContextMenuStrip.ShowCheckMargin = true;
            this.resultCategContextMenuStrip.Size = new System.Drawing.Size(259, 170);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.expandToolStripMenuItem.Text = "Expand All";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.collapseToolStripMenuItem.Text = "Collapse All";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(255, 6);
            // 
            // filterDefinedToolStripMenuItem
            // 
            this.filterDefinedToolStripMenuItem.Name = "filterDefinedToolStripMenuItem";
            this.filterDefinedToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.filterDefinedToolStripMenuItem.Text = "Filter on defined Elements";
            this.filterDefinedToolStripMenuItem.Click += new System.EventHandler(this.definedFilterToolStripMenuItem_Click);
            // 
            // filterShortLabelToolStripMenuItem
            // 
            this.filterShortLabelToolStripMenuItem.Name = "filterShortLabelToolStripMenuItem";
            this.filterShortLabelToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.filterShortLabelToolStripMenuItem.Text = "Filter on Short Label Difference";
            this.filterShortLabelToolStripMenuItem.Click += new System.EventHandler(this.shortLabelFilterToolStripMenuItem_Click);
            // 
            // filterUniqueToolStripMenuItem
            // 
            this.filterUniqueToolStripMenuItem.Name = "filterUniqueToolStripMenuItem";
            this.filterUniqueToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.filterUniqueToolStripMenuItem.Text = "Filter on unique matching";
            this.filterUniqueToolStripMenuItem.Click += new System.EventHandler(this.filterUniqueToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(255, 6);
            // 
            // importElementsToolStripMenuItem
            // 
            this.importElementsToolStripMenuItem.Name = "importElementsToolStripMenuItem";
            this.importElementsToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.importElementsToolStripMenuItem.Text = "Import secured elements only";
            this.importElementsToolStripMenuItem.Click += new System.EventHandler(this.importElementsToolStripMenuItem_Click);
            // 
            // resultElemContextMenuStrip
            // 
            this.resultElemContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importElementToolStripMenuItem});
            this.resultElemContextMenuStrip.Name = "resultCategContextMenuStrip";
            this.resultElemContextMenuStrip.Size = new System.Drawing.Size(157, 26);
            // 
            // importElementToolStripMenuItem
            // 
            this.importElementToolStripMenuItem.Name = "importElementToolStripMenuItem";
            this.importElementToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.importElementToolStripMenuItem.Text = "Import Element";
            this.importElementToolStripMenuItem.Click += new System.EventHandler(this.importElementToolStripMenuItem_Click);
            // 
            // CompareRoutinesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 448);
            this.Controls.Add(this.compareTreeView);
            this.Controls.Add(this.exportReportButton);
            this.Controls.Add(this.comparePanel);
            this.Controls.Add(this.statusStripMain);
            this.Name = "CompareRoutinesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Compare Routines";
            this.comparePanel.ResumeLayout(false);
            this.optionPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.resultCategContextMenuStrip.ResumeLayout(false);
            this.resultElemContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.TreeView compareTreeView;
        private System.Windows.Forms.Panel comparePanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel optionPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.TextBox sFileNameTextBox;
        private System.Windows.Forms.TextBox minOpsCountTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox gapToleranceTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox distToleranceTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialogSkt;
        private System.Windows.Forms.Button exportReportButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialogSkr;
        private System.Windows.Forms.OpenFileDialog openFileDialogBin;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarMain;
        private System.Windows.Forms.Button compareButton;
        private System.Windows.Forms.ContextMenuStrip resultCategContextMenuStrip;
        private System.Windows.Forms.ContextMenuStrip resultElemContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem importElementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem filterDefinedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem filterShortLabelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterUniqueToolStripMenuItem;
    }
}