namespace SAD806x
{
    partial class ConversionRepoForm
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
            this.informationTextBox = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.commentsTextBox = new System.Windows.Forms.TextBox();
            this.repoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.iFormulaTextBox = new System.Windows.Forms.TextBox();
            this.commentsLabel = new System.Windows.Forms.Label();
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.iFormulaLabel = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.listPanel = new System.Windows.Forms.Panel();
            this.repoListBox = new System.Windows.Forms.ListBox();
            this.listContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.listRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.detailsPanel.SuspendLayout();
            this.listPanel.SuspendLayout();
            this.listContextMenuStrip.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // informationTextBox
            // 
            this.informationTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationTextBox.Location = new System.Drawing.Point(0, 251);
            this.informationTextBox.Multiline = true;
            this.informationTextBox.Name = "informationTextBox";
            this.informationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.informationTextBox.Size = new System.Drawing.Size(438, 201);
            this.informationTextBox.TabIndex = 8;
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.infoLabel.Location = new System.Drawing.Point(0, 238);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(59, 13);
            this.infoLabel.TabIndex = 17;
            this.infoLabel.Text = "Information";
            // 
            // commentsTextBox
            // 
            this.commentsTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.commentsTextBox.Location = new System.Drawing.Point(0, 79);
            this.commentsTextBox.Multiline = true;
            this.commentsTextBox.Name = "commentsTextBox";
            this.commentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.commentsTextBox.Size = new System.Drawing.Size(438, 159);
            this.commentsTextBox.TabIndex = 6;
            // 
            // repoToolTip
            // 
            this.repoToolTip.AutoPopDelay = 60000;
            this.repoToolTip.InitialDelay = 500;
            this.repoToolTip.ReshowDelay = 100;
            // 
            // iFormulaTextBox
            // 
            this.iFormulaTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.iFormulaTextBox.Location = new System.Drawing.Point(0, 46);
            this.iFormulaTextBox.Name = "iFormulaTextBox";
            this.iFormulaTextBox.Size = new System.Drawing.Size(438, 20);
            this.iFormulaTextBox.TabIndex = 4;
            this.iFormulaTextBox.Text = "X*1";
            // 
            // commentsLabel
            // 
            this.commentsLabel.AutoSize = true;
            this.commentsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.commentsLabel.Location = new System.Drawing.Point(0, 66);
            this.commentsLabel.Name = "commentsLabel";
            this.commentsLabel.Size = new System.Drawing.Size(56, 13);
            this.commentsLabel.TabIndex = 13;
            this.commentsLabel.Text = "Comments";
            // 
            // detailsPanel
            // 
            this.detailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.detailsPanel.Controls.Add(this.informationTextBox);
            this.detailsPanel.Controls.Add(this.infoLabel);
            this.detailsPanel.Controls.Add(this.commentsTextBox);
            this.detailsPanel.Controls.Add(this.commentsLabel);
            this.detailsPanel.Controls.Add(this.iFormulaTextBox);
            this.detailsPanel.Controls.Add(this.iFormulaLabel);
            this.detailsPanel.Controls.Add(this.titleTextBox);
            this.detailsPanel.Controls.Add(this.titleLabel);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(438, 452);
            this.detailsPanel.TabIndex = 0;
            // 
            // iFormulaLabel
            // 
            this.iFormulaLabel.AutoSize = true;
            this.iFormulaLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.iFormulaLabel.Location = new System.Drawing.Point(0, 33);
            this.iFormulaLabel.Name = "iFormulaLabel";
            this.iFormulaLabel.Size = new System.Drawing.Size(82, 13);
            this.iFormulaLabel.TabIndex = 12;
            this.iFormulaLabel.Text = "Internal Formula";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleTextBox.Location = new System.Drawing.Point(0, 13);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(438, 20);
            this.titleTextBox.TabIndex = 2;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(27, 13);
            this.titleLabel.TabIndex = 16;
            this.titleLabel.Text = "Title";
            // 
            // listPanel
            // 
            this.listPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listPanel.Controls.Add(this.repoListBox);
            this.listPanel.Controls.Add(this.searchTextBox);
            this.listPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPanel.Location = new System.Drawing.Point(0, 0);
            this.listPanel.Name = "listPanel";
            this.listPanel.Size = new System.Drawing.Size(218, 430);
            this.listPanel.TabIndex = 1;
            // 
            // repoListBox
            // 
            this.repoListBox.ContextMenuStrip = this.listContextMenuStrip;
            this.repoListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repoListBox.FormattingEnabled = true;
            this.repoListBox.Location = new System.Drawing.Point(0, 20);
            this.repoListBox.Name = "repoListBox";
            this.repoListBox.Size = new System.Drawing.Size(218, 407);
            this.repoListBox.Sorted = true;
            this.repoListBox.TabIndex = 1;
            // 
            // listContextMenuStrip
            // 
            this.listContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listAddToolStripMenuItem,
            this.toolStripSeparator1,
            this.listRemoveToolStripMenuItem});
            this.listContextMenuStrip.Name = "listContextMenuStrip";
            this.listContextMenuStrip.Size = new System.Drawing.Size(140, 54);
            // 
            // listAddToolStripMenuItem
            // 
            this.listAddToolStripMenuItem.Name = "listAddToolStripMenuItem";
            this.listAddToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.listAddToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.listAddToolStripMenuItem.Text = "Add";
            this.listAddToolStripMenuItem.Click += new System.EventHandler(this.listAddToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(136, 6);
            // 
            // listRemoveToolStripMenuItem
            // 
            this.listRemoveToolStripMenuItem.Name = "listRemoveToolStripMenuItem";
            this.listRemoveToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.listRemoveToolStripMenuItem.Text = "Remove";
            this.listRemoveToolStripMenuItem.Click += new System.EventHandler(this.listRemoveToolStripMenuItem_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(218, 20);
            this.searchTextBox.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.saveButton.Location = new System.Drawing.Point(0, 430);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(218, 22);
            this.saveButton.TabIndex = 100;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.listPanel);
            this.splitContainerMain.Panel1.Controls.Add(this.saveButton);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.detailsPanel);
            this.splitContainerMain.Size = new System.Drawing.Size(660, 452);
            this.splitContainerMain.SplitterDistance = 218;
            this.splitContainerMain.TabIndex = 1;
            // 
            // ConversionRepoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 452);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "ConversionRepoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Conversion Repository";
            this.detailsPanel.ResumeLayout(false);
            this.detailsPanel.PerformLayout();
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.listContextMenuStrip.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox informationTextBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.TextBox commentsTextBox;
        private System.Windows.Forms.ToolTip repoToolTip;
        private System.Windows.Forms.TextBox iFormulaTextBox;
        private System.Windows.Forms.Label commentsLabel;
        private System.Windows.Forms.Panel detailsPanel;
        private System.Windows.Forms.Label iFormulaLabel;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel listPanel;
        private System.Windows.Forms.ListBox repoListBox;
        private System.Windows.Forms.ContextMenuStrip listContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem listAddToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem listRemoveToolStripMenuItem;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SplitContainer splitContainerMain;
    }
}