namespace SAD806x
{
    partial class CompareForm
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
            this.searchPanel = new System.Windows.Forms.Panel();
            this.searchButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.searchTreeView = new System.Windows.Forms.TreeView();
            this.openFileDialogBin = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialogS6x = new System.Windows.Forms.OpenFileDialog();
            this.resultCategContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultElemContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchPanel.SuspendLayout();
            this.resultCategContextMenuStrip.SuspendLayout();
            this.resultElemContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchPanel
            // 
            this.searchPanel.Controls.Add(this.searchButton);
            this.searchPanel.Controls.Add(this.label1);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(284, 35);
            this.searchPanel.TabIndex = 0;
            // 
            // searchButton
            // 
            this.searchButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchButton.Location = new System.Drawing.Point(0, 0);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(284, 22);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = "Select Binary to compare";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Result gives differences";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // searchTreeView
            // 
            this.searchTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTreeView.Location = new System.Drawing.Point(0, 35);
            this.searchTreeView.Name = "searchTreeView";
            this.searchTreeView.ShowNodeToolTips = true;
            this.searchTreeView.Size = new System.Drawing.Size(284, 226);
            this.searchTreeView.TabIndex = 2;
            // 
            // openFileDialogBin
            // 
            this.openFileDialogBin.Filter = "Binary File|*.bin|All Files|*.*";
            // 
            // openFileDialogS6x
            // 
            this.openFileDialogS6x.CheckFileExists = false;
            this.openFileDialogS6x.Filter = "SAD 806x File|*.s6x|All Files|*.*";
            // 
            // resultCategContextMenuStrip
            // 
            this.resultCategContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.collapseToolStripMenuItem});
            this.resultCategContextMenuStrip.Name = "resultCategContextMenuStrip";
            this.resultCategContextMenuStrip.ShowCheckMargin = true;
            this.resultCategContextMenuStrip.Size = new System.Drawing.Size(159, 48);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.expandToolStripMenuItem.Text = "Expand All";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.collapseToolStripMenuItem.Text = "Collapse All";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
            // 
            // resultElemContextMenuStrip
            // 
            this.resultElemContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importElementToolStripMenuItem});
            this.resultElemContextMenuStrip.Name = "resultCategContextMenuStrip";
            this.resultElemContextMenuStrip.Size = new System.Drawing.Size(157, 48);
            // 
            // importElementToolStripMenuItem
            // 
            this.importElementToolStripMenuItem.Name = "importElementToolStripMenuItem";
            this.importElementToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.importElementToolStripMenuItem.Text = "Import Element";
            this.importElementToolStripMenuItem.Click += new System.EventHandler(this.importElementToolStripMenuItem_Click);
            // 
            // CompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.searchTreeView);
            this.Controls.Add(this.searchPanel);
            this.Name = "CompareForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Compare Binaries";
            this.searchPanel.ResumeLayout(false);
            this.resultCategContextMenuStrip.ResumeLayout(false);
            this.resultElemContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TreeView searchTreeView;
        private System.Windows.Forms.OpenFileDialog openFileDialogBin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialogS6x;
        private System.Windows.Forms.ContextMenuStrip resultCategContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip resultElemContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem importElementToolStripMenuItem;
    }
}