namespace SAD806x
{
    partial class SearchForm
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
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.searchContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.routinesWithArgumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.routinesAdvancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTreeView = new System.Windows.Forms.TreeView();
            this.searchPanel.SuspendLayout();
            this.searchContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchPanel
            // 
            this.searchPanel.Controls.Add(this.searchTextBox);
            this.searchPanel.Controls.Add(this.searchButton);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(284, 20);
            this.searchPanel.TabIndex = 0;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(244, 20);
            this.searchTextBox.TabIndex = 0;
            // 
            // searchButton
            // 
            this.searchButton.ContextMenuStrip = this.searchContextMenuStrip;
            this.searchButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.searchButton.Location = new System.Drawing.Point(244, 0);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(40, 20);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = ">";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // searchContextMenuStrip
            // 
            this.searchContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.routinesWithArgumentsToolStripMenuItem,
            this.routinesAdvancedToolStripMenuItem});
            this.searchContextMenuStrip.Name = "searchContextMenuStrip";
            this.searchContextMenuStrip.Size = new System.Drawing.Size(209, 70);
            // 
            // routinesWithArgumentsToolStripMenuItem
            // 
            this.routinesWithArgumentsToolStripMenuItem.Name = "routinesWithArgumentsToolStripMenuItem";
            this.routinesWithArgumentsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.routinesWithArgumentsToolStripMenuItem.Text = "Routines with Arguments";
            this.routinesWithArgumentsToolStripMenuItem.Click += new System.EventHandler(this.routinesWithArgumentsToolStripMenuItem_Click);
            // 
            // routinesAdvancedToolStripMenuItem
            // 
            this.routinesAdvancedToolStripMenuItem.Name = "routinesAdvancedToolStripMenuItem";
            this.routinesAdvancedToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.routinesAdvancedToolStripMenuItem.Text = "Routines Advanced";
            this.routinesAdvancedToolStripMenuItem.Click += new System.EventHandler(this.routinesAdvancedToolStripMenuItem_Click);
            // 
            // searchTreeView
            // 
            this.searchTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTreeView.Location = new System.Drawing.Point(0, 20);
            this.searchTreeView.Name = "searchTreeView";
            this.searchTreeView.Size = new System.Drawing.Size(284, 241);
            this.searchTreeView.TabIndex = 2;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.searchTreeView);
            this.Controls.Add(this.searchPanel);
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search Objects";
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.searchContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TreeView searchTreeView;
        private System.Windows.Forms.ContextMenuStrip searchContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem routinesWithArgumentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routinesAdvancedToolStripMenuItem;
    }
}