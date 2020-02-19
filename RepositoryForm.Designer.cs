namespace SAD806x
{
    partial class RepositoryForm
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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.listPanel = new System.Windows.Forms.Panel();
            this.repoListBox = new System.Windows.Forms.ListBox();
            this.listContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.listRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listLoadS6xToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.listLoadS6xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.informationTextBox = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.commentsTextBox = new System.Windows.Forms.TextBox();
            this.commentsLabel = new System.Windows.Forms.Label();
            this.labelTextBox = new System.Windows.Forms.TextBox();
            this.labelLabel = new System.Windows.Forms.Label();
            this.sLabelTextBox = new System.Windows.Forms.TextBox();
            this.sLabelLabel = new System.Windows.Forms.Label();
            this.repoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.listPanel.SuspendLayout();
            this.listContextMenuStrip.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainerMain.Size = new System.Drawing.Size(691, 436);
            this.splitContainerMain.SplitterDistance = 229;
            this.splitContainerMain.TabIndex = 0;
            // 
            // listPanel
            // 
            this.listPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listPanel.Controls.Add(this.repoListBox);
            this.listPanel.Controls.Add(this.searchTextBox);
            this.listPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPanel.Location = new System.Drawing.Point(0, 0);
            this.listPanel.Name = "listPanel";
            this.listPanel.Size = new System.Drawing.Size(229, 414);
            this.listPanel.TabIndex = 1;
            // 
            // repoListBox
            // 
            this.repoListBox.ContextMenuStrip = this.listContextMenuStrip;
            this.repoListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repoListBox.FormattingEnabled = true;
            this.repoListBox.Location = new System.Drawing.Point(0, 20);
            this.repoListBox.Name = "repoListBox";
            this.repoListBox.Size = new System.Drawing.Size(229, 394);
            this.repoListBox.Sorted = true;
            this.repoListBox.TabIndex = 1;
            // 
            // listContextMenuStrip
            // 
            this.listContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listAddToolStripMenuItem,
            this.toolStripSeparator1,
            this.listRemoveToolStripMenuItem,
            this.listLoadS6xToolStripSeparator,
            this.listLoadS6xToolStripMenuItem});
            this.listContextMenuStrip.Name = "listContextMenuStrip";
            this.listContextMenuStrip.Size = new System.Drawing.Size(151, 82);
            // 
            // listAddToolStripMenuItem
            // 
            this.listAddToolStripMenuItem.Name = "listAddToolStripMenuItem";
            this.listAddToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.listAddToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.listAddToolStripMenuItem.Text = "Add";
            this.listAddToolStripMenuItem.Click += new System.EventHandler(this.listAddToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // listRemoveToolStripMenuItem
            // 
            this.listRemoveToolStripMenuItem.Name = "listRemoveToolStripMenuItem";
            this.listRemoveToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.listRemoveToolStripMenuItem.Text = "Remove";
            this.listRemoveToolStripMenuItem.Click += new System.EventHandler(this.listRemoveToolStripMenuItem_Click);
            // 
            // listLoadS6xToolStripSeparator
            // 
            this.listLoadS6xToolStripSeparator.Name = "listLoadS6xToolStripSeparator";
            this.listLoadS6xToolStripSeparator.Size = new System.Drawing.Size(147, 6);
            // 
            // listLoadS6xToolStripMenuItem
            // 
            this.listLoadS6xToolStripMenuItem.Name = "listLoadS6xToolStripMenuItem";
            this.listLoadS6xToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.listLoadS6xToolStripMenuItem.Text = "Load from S6x";
            this.listLoadS6xToolStripMenuItem.Click += new System.EventHandler(this.listLoadS6xToolStripMenuItem_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(229, 20);
            this.searchTextBox.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.saveButton.Location = new System.Drawing.Point(0, 414);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(229, 22);
            this.saveButton.TabIndex = 100;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // detailsPanel
            // 
            this.detailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.detailsPanel.Controls.Add(this.informationTextBox);
            this.detailsPanel.Controls.Add(this.infoLabel);
            this.detailsPanel.Controls.Add(this.commentsTextBox);
            this.detailsPanel.Controls.Add(this.commentsLabel);
            this.detailsPanel.Controls.Add(this.labelTextBox);
            this.detailsPanel.Controls.Add(this.labelLabel);
            this.detailsPanel.Controls.Add(this.sLabelTextBox);
            this.detailsPanel.Controls.Add(this.sLabelLabel);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(458, 436);
            this.detailsPanel.TabIndex = 0;
            // 
            // informationTextBox
            // 
            this.informationTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationTextBox.Location = new System.Drawing.Point(0, 251);
            this.informationTextBox.Multiline = true;
            this.informationTextBox.Name = "informationTextBox";
            this.informationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.informationTextBox.Size = new System.Drawing.Size(458, 185);
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
            this.commentsTextBox.Size = new System.Drawing.Size(458, 159);
            this.commentsTextBox.TabIndex = 6;
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
            // labelTextBox
            // 
            this.labelTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTextBox.Location = new System.Drawing.Point(0, 46);
            this.labelTextBox.Name = "labelTextBox";
            this.labelTextBox.Size = new System.Drawing.Size(458, 20);
            this.labelTextBox.TabIndex = 4;
            // 
            // labelLabel
            // 
            this.labelLabel.AutoSize = true;
            this.labelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelLabel.Location = new System.Drawing.Point(0, 33);
            this.labelLabel.Name = "labelLabel";
            this.labelLabel.Size = new System.Drawing.Size(33, 13);
            this.labelLabel.TabIndex = 12;
            this.labelLabel.Text = "Label";
            // 
            // sLabelTextBox
            // 
            this.sLabelTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.sLabelTextBox.Location = new System.Drawing.Point(0, 13);
            this.sLabelTextBox.Name = "sLabelTextBox";
            this.sLabelTextBox.Size = new System.Drawing.Size(458, 20);
            this.sLabelTextBox.TabIndex = 2;
            // 
            // sLabelLabel
            // 
            this.sLabelLabel.AutoSize = true;
            this.sLabelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sLabelLabel.Location = new System.Drawing.Point(0, 0);
            this.sLabelLabel.Name = "sLabelLabel";
            this.sLabelLabel.Size = new System.Drawing.Size(61, 13);
            this.sLabelLabel.TabIndex = 16;
            this.sLabelLabel.Text = "Short Label";
            // 
            // repoToolTip
            // 
            this.repoToolTip.AutoPopDelay = 60000;
            this.repoToolTip.InitialDelay = 500;
            this.repoToolTip.ReshowDelay = 100;
            // 
            // RepositoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 436);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "RepositoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Repository";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.listContextMenuStrip.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.detailsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel detailsPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel listPanel;
        private System.Windows.Forms.ListBox repoListBox;
        private System.Windows.Forms.TextBox commentsTextBox;
        private System.Windows.Forms.Label commentsLabel;
        private System.Windows.Forms.TextBox labelTextBox;
        private System.Windows.Forms.Label labelLabel;
        private System.Windows.Forms.TextBox sLabelTextBox;
        private System.Windows.Forms.Label sLabelLabel;
        private System.Windows.Forms.ContextMenuStrip listContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem listAddToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem listRemoveToolStripMenuItem;
        private System.Windows.Forms.TextBox informationTextBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.ToolTip repoToolTip;
        private System.Windows.Forms.ToolStripSeparator listLoadS6xToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem listLoadS6xToolStripMenuItem;
        private System.Windows.Forms.TextBox searchTextBox;
    }
}