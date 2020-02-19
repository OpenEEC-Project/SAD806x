namespace SAD806x
{
    partial class SettingsForm
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
            this.settingsListBox = new System.Windows.Forms.ListBox();
            this.listContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.categComboBox = new System.Windows.Forms.ComboBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.detailsPanel = new System.Windows.Forms.Panel();
            this.settingValuePanel = new System.Windows.Forms.Panel();
            this.informationTextBox = new System.Windows.Forms.TextBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.settingsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.listPanel.SuspendLayout();
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
            this.splitContainerMain.Size = new System.Drawing.Size(884, 461);
            this.splitContainerMain.SplitterDistance = 290;
            this.splitContainerMain.TabIndex = 1;
            // 
            // listPanel
            // 
            this.listPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listPanel.Controls.Add(this.settingsListBox);
            this.listPanel.Controls.Add(this.categComboBox);
            this.listPanel.Controls.Add(this.searchTextBox);
            this.listPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPanel.Location = new System.Drawing.Point(0, 0);
            this.listPanel.Name = "listPanel";
            this.listPanel.Size = new System.Drawing.Size(290, 439);
            this.listPanel.TabIndex = 1;
            // 
            // settingsListBox
            // 
            this.settingsListBox.ContextMenuStrip = this.listContextMenuStrip;
            this.settingsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsListBox.FormattingEnabled = true;
            this.settingsListBox.Location = new System.Drawing.Point(0, 41);
            this.settingsListBox.Name = "settingsListBox";
            this.settingsListBox.Size = new System.Drawing.Size(290, 394);
            this.settingsListBox.TabIndex = 2;
            // 
            // listContextMenuStrip
            // 
            this.listContextMenuStrip.Name = "listContextMenuStrip";
            this.listContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // categComboBox
            // 
            this.categComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.categComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.categComboBox.FormattingEnabled = true;
            this.categComboBox.Location = new System.Drawing.Point(0, 20);
            this.categComboBox.Name = "categComboBox";
            this.categComboBox.Size = new System.Drawing.Size(290, 21);
            this.categComboBox.TabIndex = 1;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(290, 20);
            this.searchTextBox.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.saveButton.Location = new System.Drawing.Point(0, 439);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(290, 22);
            this.saveButton.TabIndex = 100;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // detailsPanel
            // 
            this.detailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.detailsPanel.Controls.Add(this.settingValuePanel);
            this.detailsPanel.Controls.Add(this.informationTextBox);
            this.detailsPanel.Controls.Add(this.infoLabel);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(590, 461);
            this.detailsPanel.TabIndex = 0;
            // 
            // settingValuePanel
            // 
            this.settingValuePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingValuePanel.Location = new System.Drawing.Point(0, 154);
            this.settingValuePanel.Name = "settingValuePanel";
            this.settingValuePanel.Size = new System.Drawing.Size(590, 307);
            this.settingValuePanel.TabIndex = 18;
            // 
            // informationTextBox
            // 
            this.informationTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationTextBox.Location = new System.Drawing.Point(0, 13);
            this.informationTextBox.Multiline = true;
            this.informationTextBox.Name = "informationTextBox";
            this.informationTextBox.ReadOnly = true;
            this.informationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.informationTextBox.Size = new System.Drawing.Size(590, 141);
            this.informationTextBox.TabIndex = 3;
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.infoLabel.Location = new System.Drawing.Point(0, 0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(59, 13);
            this.infoLabel.TabIndex = 17;
            this.infoLabel.Text = "Information";
            // 
            // settingsToolTip
            // 
            this.settingsToolTip.AutoPopDelay = 60000;
            this.settingsToolTip.InitialDelay = 500;
            this.settingsToolTip.ReshowDelay = 100;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingsForm";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.detailsPanel.ResumeLayout(false);
            this.detailsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel listPanel;
        private System.Windows.Forms.ListBox settingsListBox;
        private System.Windows.Forms.ContextMenuStrip listContextMenuStrip;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel detailsPanel;
        private System.Windows.Forms.TextBox informationTextBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.ToolTip settingsToolTip;
        private System.Windows.Forms.Panel settingValuePanel;
        private System.Windows.Forms.ComboBox categComboBox;
    }
}