namespace SAD806x
{
    partial class BitFlagsForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Bit Flags");
            this.advElemsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyXdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elemUpdateButton = new System.Windows.Forms.Button();
            this.elemFooterPanel = new System.Windows.Forms.Panel();
            this.advElemsTreeView = new System.Windows.Forms.TreeView();
            this.advMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.elemTabControl = new System.Windows.Forms.TabControl();
            this.bitFlagTabPage = new System.Windows.Forms.TabPage();
            this.bitFlagHParentCheckBox = new System.Windows.Forms.CheckBox();
            this.bitFlagSkipCheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.bitFlagSLabelTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bitFlagLabelTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bitFlagNSetValueTextBox = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.bitFlagSetValueTextBox = new System.Windows.Forms.TextBox();
            this.bitFlagCommentsTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bitFlagPositionComboBox = new System.Windows.Forms.ComboBox();
            this.label60 = new System.Windows.Forms.Label();
            this.sharedDetailsTabPage = new System.Windows.Forms.TabPage();
            this.sharedIdentificationStatusTrackBar = new System.Windows.Forms.TrackBar();
            this.sharedIdentificationDetailsTextBox = new System.Windows.Forms.TextBox();
            this.sharedIdentificationLabel = new System.Windows.Forms.Label();
            this.sharedCateg3ComboBox = new System.Windows.Forms.ComboBox();
            this.sharedCateg2ComboBox = new System.Windows.Forms.ComboBox();
            this.sharedCategsLabel = new System.Windows.Forms.Label();
            this.sharedCategComboBox = new System.Windows.Forms.ComboBox();
            this.label53 = new System.Windows.Forms.Label();
            this.sharedDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label52 = new System.Windows.Forms.Label();
            this.sharedDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.advSLabelTextBox = new System.Windows.Forms.TextBox();
            this.advLabelTextBox = new System.Windows.Forms.TextBox();
            this.advHeaderPanel = new System.Windows.Forms.Panel();
            this.advSigPanel = new System.Windows.Forms.Panel();
            this.advMainPanel = new System.Windows.Forms.Panel();
            this.advElemsContextMenuStrip.SuspendLayout();
            this.elemFooterPanel.SuspendLayout();
            this.advMainSplitContainer.Panel1.SuspendLayout();
            this.advMainSplitContainer.Panel2.SuspendLayout();
            this.advMainSplitContainer.SuspendLayout();
            this.elemTabControl.SuspendLayout();
            this.bitFlagTabPage.SuspendLayout();
            this.sharedDetailsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sharedIdentificationStatusTrackBar)).BeginInit();
            this.advHeaderPanel.SuspendLayout();
            this.advMainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // advElemsContextMenuStrip
            // 
            this.advElemsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newElementToolStripMenuItem,
            this.createAllToolStripMenuItem,
            this.delElementToolStripMenuItem,
            this.removeAllToolStripMenuItem,
            this.copyXdfToolStripMenuItem});
            this.advElemsContextMenuStrip.Name = "advSigElemsContextMenuStrip";
            this.advElemsContextMenuStrip.Size = new System.Drawing.Size(145, 114);
            // 
            // newElementToolStripMenuItem
            // 
            this.newElementToolStripMenuItem.Name = "newElementToolStripMenuItem";
            this.newElementToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.newElementToolStripMenuItem.Text = "New Element";
            this.newElementToolStripMenuItem.Click += new System.EventHandler(this.newElementToolStripMenuItem_Click);
            // 
            // createAllToolStripMenuItem
            // 
            this.createAllToolStripMenuItem.Name = "createAllToolStripMenuItem";
            this.createAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.createAllToolStripMenuItem.Text = "Create All";
            this.createAllToolStripMenuItem.Click += new System.EventHandler(this.createAllToolStripMenuItem_Click);
            // 
            // delElementToolStripMenuItem
            // 
            this.delElementToolStripMenuItem.Name = "delElementToolStripMenuItem";
            this.delElementToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.delElementToolStripMenuItem.Text = "Remove";
            this.delElementToolStripMenuItem.Click += new System.EventHandler(this.delElementToolStripMenuItem_Click);
            // 
            // removeAllToolStripMenuItem
            // 
            this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.removeAllToolStripMenuItem.Text = "Remove All";
            this.removeAllToolStripMenuItem.Click += new System.EventHandler(this.removeAllToolStripMenuItem_Click);
            // 
            // copyXdfToolStripMenuItem
            // 
            this.copyXdfToolStripMenuItem.Name = "copyXdfToolStripMenuItem";
            this.copyXdfToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyXdfToolStripMenuItem.Text = "Copy (xdf)";
            this.copyXdfToolStripMenuItem.Click += new System.EventHandler(this.copyXdfToolStripMenuItem_Click);
            // 
            // elemUpdateButton
            // 
            this.elemUpdateButton.AutoSize = true;
            this.elemUpdateButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.elemUpdateButton.Location = new System.Drawing.Point(414, 0);
            this.elemUpdateButton.Name = "elemUpdateButton";
            this.elemUpdateButton.Size = new System.Drawing.Size(82, 27);
            this.elemUpdateButton.TabIndex = 100;
            this.elemUpdateButton.Text = "Add / Update";
            this.elemUpdateButton.UseVisualStyleBackColor = true;
            this.elemUpdateButton.Click += new System.EventHandler(this.elemUpdateButton_Click);
            // 
            // elemFooterPanel
            // 
            this.elemFooterPanel.Controls.Add(this.elemUpdateButton);
            this.elemFooterPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.elemFooterPanel.Location = new System.Drawing.Point(0, 268);
            this.elemFooterPanel.Name = "elemFooterPanel";
            this.elemFooterPanel.Size = new System.Drawing.Size(496, 27);
            this.elemFooterPanel.TabIndex = 3;
            // 
            // advElemsTreeView
            // 
            this.advElemsTreeView.ContextMenuStrip = this.advElemsContextMenuStrip;
            this.advElemsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advElemsTreeView.Location = new System.Drawing.Point(0, 0);
            this.advElemsTreeView.Name = "advElemsTreeView";
            treeNode1.Name = "BITFLAGS";
            treeNode1.Text = "Bit Flags";
            this.advElemsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.advElemsTreeView.Size = new System.Drawing.Size(161, 295);
            this.advElemsTreeView.TabIndex = 1;
            // 
            // advMainSplitContainer
            // 
            this.advMainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advMainSplitContainer.Location = new System.Drawing.Point(0, 38);
            this.advMainSplitContainer.Name = "advMainSplitContainer";
            // 
            // advMainSplitContainer.Panel1
            // 
            this.advMainSplitContainer.Panel1.Controls.Add(this.advElemsTreeView);
            // 
            // advMainSplitContainer.Panel2
            // 
            this.advMainSplitContainer.Panel2.Controls.Add(this.elemFooterPanel);
            this.advMainSplitContainer.Panel2.Controls.Add(this.elemTabControl);
            this.advMainSplitContainer.Size = new System.Drawing.Size(661, 295);
            this.advMainSplitContainer.SplitterDistance = 161;
            this.advMainSplitContainer.TabIndex = 5;
            // 
            // elemTabControl
            // 
            this.elemTabControl.Controls.Add(this.bitFlagTabPage);
            this.elemTabControl.Controls.Add(this.sharedDetailsTabPage);
            this.elemTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elemTabControl.Location = new System.Drawing.Point(0, 0);
            this.elemTabControl.Name = "elemTabControl";
            this.elemTabControl.SelectedIndex = 0;
            this.elemTabControl.Size = new System.Drawing.Size(496, 295);
            this.elemTabControl.TabIndex = 2;
            // 
            // bitFlagTabPage
            // 
            this.bitFlagTabPage.AutoScroll = true;
            this.bitFlagTabPage.Controls.Add(this.bitFlagHParentCheckBox);
            this.bitFlagTabPage.Controls.Add(this.bitFlagSkipCheckBox);
            this.bitFlagTabPage.Controls.Add(this.label6);
            this.bitFlagTabPage.Controls.Add(this.bitFlagSLabelTextBox);
            this.bitFlagTabPage.Controls.Add(this.label5);
            this.bitFlagTabPage.Controls.Add(this.bitFlagLabelTextBox);
            this.bitFlagTabPage.Controls.Add(this.label3);
            this.bitFlagTabPage.Controls.Add(this.bitFlagNSetValueTextBox);
            this.bitFlagTabPage.Controls.Add(this.label35);
            this.bitFlagTabPage.Controls.Add(this.bitFlagSetValueTextBox);
            this.bitFlagTabPage.Controls.Add(this.bitFlagCommentsTextBox);
            this.bitFlagTabPage.Controls.Add(this.label2);
            this.bitFlagTabPage.Controls.Add(this.bitFlagPositionComboBox);
            this.bitFlagTabPage.Controls.Add(this.label60);
            this.bitFlagTabPage.Location = new System.Drawing.Point(4, 22);
            this.bitFlagTabPage.Name = "bitFlagTabPage";
            this.bitFlagTabPage.Size = new System.Drawing.Size(488, 269);
            this.bitFlagTabPage.TabIndex = 13;
            this.bitFlagTabPage.Text = "Bit Flag";
            this.bitFlagTabPage.UseVisualStyleBackColor = true;
            // 
            // bitFlagHParentCheckBox
            // 
            this.bitFlagHParentCheckBox.AutoSize = true;
            this.bitFlagHParentCheckBox.Location = new System.Drawing.Point(228, 141);
            this.bitFlagHParentCheckBox.Name = "bitFlagHParentCheckBox";
            this.bitFlagHParentCheckBox.Size = new System.Drawing.Size(82, 17);
            this.bitFlagHParentCheckBox.TabIndex = 77;
            this.bitFlagHParentCheckBox.Text = "Hide Parent";
            this.bitFlagHParentCheckBox.UseVisualStyleBackColor = true;
            // 
            // bitFlagSkipCheckBox
            // 
            this.bitFlagSkipCheckBox.AutoSize = true;
            this.bitFlagSkipCheckBox.Location = new System.Drawing.Point(228, 30);
            this.bitFlagSkipCheckBox.Name = "bitFlagSkipCheckBox";
            this.bitFlagSkipCheckBox.Size = new System.Drawing.Size(47, 17);
            this.bitFlagSkipCheckBox.TabIndex = 6;
            this.bitFlagSkipCheckBox.Text = "Skip";
            this.bitFlagSkipCheckBox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(421, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 76;
            this.label6.Text = "Short Label";
            // 
            // bitFlagSLabelTextBox
            // 
            this.bitFlagSLabelTextBox.Location = new System.Drawing.Point(362, 28);
            this.bitFlagSLabelTextBox.Name = "bitFlagSLabelTextBox";
            this.bitFlagSLabelTextBox.Size = new System.Drawing.Size(120, 20);
            this.bitFlagSLabelTextBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 74;
            this.label5.Text = "Label";
            // 
            // bitFlagLabelTextBox
            // 
            this.bitFlagLabelTextBox.Location = new System.Drawing.Point(6, 28);
            this.bitFlagLabelTextBox.Name = "bitFlagLabelTextBox";
            this.bitFlagLabelTextBox.Size = new System.Drawing.Size(180, 20);
            this.bitFlagLabelTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(374, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 72;
            this.label3.Text = "Output Not Set Value";
            // 
            // bitFlagNSetValueTextBox
            // 
            this.bitFlagNSetValueTextBox.Location = new System.Drawing.Point(362, 138);
            this.bitFlagNSetValueTextBox.Name = "bitFlagNSetValueTextBox";
            this.bitFlagNSetValueTextBox.Size = new System.Drawing.Size(120, 20);
            this.bitFlagNSetValueTextBox.TabIndex = 8;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(3, 123);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(88, 13);
            this.label35.TabIndex = 68;
            this.label35.Text = "Output Set Value";
            // 
            // bitFlagSetValueTextBox
            // 
            this.bitFlagSetValueTextBox.Location = new System.Drawing.Point(6, 139);
            this.bitFlagSetValueTextBox.Name = "bitFlagSetValueTextBox";
            this.bitFlagSetValueTextBox.Size = new System.Drawing.Size(120, 20);
            this.bitFlagSetValueTextBox.TabIndex = 7;
            // 
            // bitFlagCommentsTextBox
            // 
            this.bitFlagCommentsTextBox.Location = new System.Drawing.Point(6, 195);
            this.bitFlagCommentsTextBox.Multiline = true;
            this.bitFlagCommentsTextBox.Name = "bitFlagCommentsTextBox";
            this.bitFlagCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.bitFlagCommentsTextBox.Size = new System.Drawing.Size(476, 45);
            this.bitFlagCommentsTextBox.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 64;
            this.label2.Text = "Comments";
            // 
            // bitFlagPositionComboBox
            // 
            this.bitFlagPositionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bitFlagPositionComboBox.FormattingEnabled = true;
            this.bitFlagPositionComboBox.Items.AddRange(new object[] {
            "B0",
            "B1",
            "B2",
            "B3",
            "B4",
            "B5",
            "B6",
            "B7",
            "B8",
            "B9",
            "B10",
            "B11",
            "B12",
            "B13",
            "B14",
            "B15"});
            this.bitFlagPositionComboBox.Location = new System.Drawing.Point(6, 82);
            this.bitFlagPositionComboBox.Name = "bitFlagPositionComboBox";
            this.bitFlagPositionComboBox.Size = new System.Drawing.Size(120, 21);
            this.bitFlagPositionComboBox.TabIndex = 5;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(3, 66);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(44, 13);
            this.label60.TabIndex = 61;
            this.label60.Text = "Position";
            // 
            // sharedDetailsTabPage
            // 
            this.sharedDetailsTabPage.AutoScroll = true;
            this.sharedDetailsTabPage.Controls.Add(this.sharedIdentificationStatusTrackBar);
            this.sharedDetailsTabPage.Controls.Add(this.sharedIdentificationDetailsTextBox);
            this.sharedDetailsTabPage.Controls.Add(this.sharedIdentificationLabel);
            this.sharedDetailsTabPage.Controls.Add(this.sharedCateg3ComboBox);
            this.sharedDetailsTabPage.Controls.Add(this.sharedCateg2ComboBox);
            this.sharedDetailsTabPage.Controls.Add(this.sharedCategsLabel);
            this.sharedDetailsTabPage.Controls.Add(this.sharedCategComboBox);
            this.sharedDetailsTabPage.Controls.Add(this.label53);
            this.sharedDetailsTabPage.Controls.Add(this.sharedDateUpdatedDateTimePicker);
            this.sharedDetailsTabPage.Controls.Add(this.label52);
            this.sharedDetailsTabPage.Controls.Add(this.sharedDateCreatedDateTimePicker);
            this.sharedDetailsTabPage.Location = new System.Drawing.Point(4, 22);
            this.sharedDetailsTabPage.Name = "sharedDetailsTabPage";
            this.sharedDetailsTabPage.Size = new System.Drawing.Size(488, 269);
            this.sharedDetailsTabPage.TabIndex = 14;
            this.sharedDetailsTabPage.Text = "Details";
            this.sharedDetailsTabPage.UseVisualStyleBackColor = true;
            // 
            // sharedIdentificationStatusTrackBar
            // 
            this.sharedIdentificationStatusTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.sharedIdentificationStatusTrackBar.LargeChange = 10;
            this.sharedIdentificationStatusTrackBar.Location = new System.Drawing.Point(6, 195);
            this.sharedIdentificationStatusTrackBar.Maximum = 100;
            this.sharedIdentificationStatusTrackBar.Name = "sharedIdentificationStatusTrackBar";
            this.sharedIdentificationStatusTrackBar.Size = new System.Drawing.Size(180, 45);
            this.sharedIdentificationStatusTrackBar.TabIndex = 40;
            this.sharedIdentificationStatusTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // sharedIdentificationDetailsTextBox
            // 
            this.sharedIdentificationDetailsTextBox.Location = new System.Drawing.Point(190, 195);
            this.sharedIdentificationDetailsTextBox.Multiline = true;
            this.sharedIdentificationDetailsTextBox.Name = "sharedIdentificationDetailsTextBox";
            this.sharedIdentificationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sharedIdentificationDetailsTextBox.Size = new System.Drawing.Size(292, 45);
            this.sharedIdentificationDetailsTextBox.TabIndex = 41;
            // 
            // sharedIdentificationLabel
            // 
            this.sharedIdentificationLabel.AutoSize = true;
            this.sharedIdentificationLabel.Location = new System.Drawing.Point(3, 179);
            this.sharedIdentificationLabel.Name = "sharedIdentificationLabel";
            this.sharedIdentificationLabel.Size = new System.Drawing.Size(67, 13);
            this.sharedIdentificationLabel.TabIndex = 89;
            this.sharedIdentificationLabel.Tag = "Identification";
            this.sharedIdentificationLabel.Text = "Identification";
            // 
            // sharedCateg3ComboBox
            // 
            this.sharedCateg3ComboBox.FormattingEnabled = true;
            this.sharedCateg3ComboBox.Location = new System.Drawing.Point(362, 120);
            this.sharedCateg3ComboBox.Name = "sharedCateg3ComboBox";
            this.sharedCateg3ComboBox.Size = new System.Drawing.Size(120, 21);
            this.sharedCateg3ComboBox.Sorted = true;
            this.sharedCateg3ComboBox.TabIndex = 32;
            // 
            // sharedCateg2ComboBox
            // 
            this.sharedCateg2ComboBox.FormattingEnabled = true;
            this.sharedCateg2ComboBox.Location = new System.Drawing.Point(190, 120);
            this.sharedCateg2ComboBox.Name = "sharedCateg2ComboBox";
            this.sharedCateg2ComboBox.Size = new System.Drawing.Size(120, 21);
            this.sharedCateg2ComboBox.Sorted = true;
            this.sharedCateg2ComboBox.TabIndex = 31;
            // 
            // sharedCategsLabel
            // 
            this.sharedCategsLabel.AutoSize = true;
            this.sharedCategsLabel.Location = new System.Drawing.Point(219, 104);
            this.sharedCategsLabel.Name = "sharedCategsLabel";
            this.sharedCategsLabel.Size = new System.Drawing.Size(57, 13);
            this.sharedCategsLabel.TabIndex = 88;
            this.sharedCategsLabel.Text = "Categories";
            // 
            // sharedCategComboBox
            // 
            this.sharedCategComboBox.FormattingEnabled = true;
            this.sharedCategComboBox.Location = new System.Drawing.Point(6, 120);
            this.sharedCategComboBox.Name = "sharedCategComboBox";
            this.sharedCategComboBox.Size = new System.Drawing.Size(120, 21);
            this.sharedCategComboBox.Sorted = true;
            this.sharedCategComboBox.TabIndex = 30;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(415, 12);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(72, 13);
            this.label53.TabIndex = 85;
            this.label53.Text = "Date updated";
            // 
            // sharedDateUpdatedDateTimePicker
            // 
            this.sharedDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.sharedDateUpdatedDateTimePicker.Enabled = false;
            this.sharedDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.sharedDateUpdatedDateTimePicker.Location = new System.Drawing.Point(302, 28);
            this.sharedDateUpdatedDateTimePicker.Name = "sharedDateUpdatedDateTimePicker";
            this.sharedDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.sharedDateUpdatedDateTimePicker.TabIndex = 21;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(3, 12);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(69, 13);
            this.label52.TabIndex = 84;
            this.label52.Text = "Date created";
            // 
            // sharedDateCreatedDateTimePicker
            // 
            this.sharedDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.sharedDateCreatedDateTimePicker.Enabled = false;
            this.sharedDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.sharedDateCreatedDateTimePicker.Location = new System.Drawing.Point(6, 28);
            this.sharedDateCreatedDateTimePicker.Name = "sharedDateCreatedDateTimePicker";
            this.sharedDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.sharedDateCreatedDateTimePicker.TabIndex = 20;
            // 
            // advSLabelTextBox
            // 
            this.advSLabelTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.advSLabelTextBox.Location = new System.Drawing.Point(561, 0);
            this.advSLabelTextBox.Name = "advSLabelTextBox";
            this.advSLabelTextBox.ReadOnly = true;
            this.advSLabelTextBox.Size = new System.Drawing.Size(100, 20);
            this.advSLabelTextBox.TabIndex = 1;
            this.advSLabelTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // advLabelTextBox
            // 
            this.advLabelTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advLabelTextBox.Location = new System.Drawing.Point(0, 0);
            this.advLabelTextBox.Name = "advLabelTextBox";
            this.advLabelTextBox.ReadOnly = true;
            this.advLabelTextBox.Size = new System.Drawing.Size(661, 20);
            this.advLabelTextBox.TabIndex = 0;
            // 
            // advHeaderPanel
            // 
            this.advHeaderPanel.BackColor = System.Drawing.SystemColors.Window;
            this.advHeaderPanel.Controls.Add(this.advSLabelTextBox);
            this.advHeaderPanel.Controls.Add(this.advLabelTextBox);
            this.advHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.advHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.advHeaderPanel.Name = "advHeaderPanel";
            this.advHeaderPanel.Size = new System.Drawing.Size(661, 29);
            this.advHeaderPanel.TabIndex = 4;
            // 
            // advSigPanel
            // 
            this.advSigPanel.BackColor = System.Drawing.SystemColors.Window;
            this.advSigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.advSigPanel.Location = new System.Drawing.Point(0, 0);
            this.advSigPanel.Name = "advSigPanel";
            this.advSigPanel.Size = new System.Drawing.Size(661, 38);
            this.advSigPanel.TabIndex = 3;
            // 
            // advMainPanel
            // 
            this.advMainPanel.BackColor = System.Drawing.SystemColors.Window;
            this.advMainPanel.Controls.Add(this.advMainSplitContainer);
            this.advMainPanel.Controls.Add(this.advSigPanel);
            this.advMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advMainPanel.Location = new System.Drawing.Point(0, 0);
            this.advMainPanel.Name = "advMainPanel";
            this.advMainPanel.Size = new System.Drawing.Size(661, 333);
            this.advMainPanel.TabIndex = 5;
            // 
            // BitFlagsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 333);
            this.Controls.Add(this.advHeaderPanel);
            this.Controls.Add(this.advMainPanel);
            this.Name = "BitFlagsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bit Flags";
            this.Load += new System.EventHandler(this.BitFlagsForm_Load);
            this.advElemsContextMenuStrip.ResumeLayout(false);
            this.elemFooterPanel.ResumeLayout(false);
            this.elemFooterPanel.PerformLayout();
            this.advMainSplitContainer.Panel1.ResumeLayout(false);
            this.advMainSplitContainer.Panel2.ResumeLayout(false);
            this.advMainSplitContainer.ResumeLayout(false);
            this.elemTabControl.ResumeLayout(false);
            this.bitFlagTabPage.ResumeLayout(false);
            this.bitFlagTabPage.PerformLayout();
            this.sharedDetailsTabPage.ResumeLayout(false);
            this.sharedDetailsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sharedIdentificationStatusTrackBar)).EndInit();
            this.advHeaderPanel.ResumeLayout(false);
            this.advHeaderPanel.PerformLayout();
            this.advMainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip advElemsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem delElementToolStripMenuItem;
        private System.Windows.Forms.Button elemUpdateButton;
        private System.Windows.Forms.Panel elemFooterPanel;
        private System.Windows.Forms.TreeView advElemsTreeView;
        private System.Windows.Forms.SplitContainer advMainSplitContainer;
        private System.Windows.Forms.TabControl elemTabControl;
        private System.Windows.Forms.TabPage bitFlagTabPage;
        private System.Windows.Forms.ComboBox bitFlagPositionComboBox;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.TextBox advSLabelTextBox;
        private System.Windows.Forms.TextBox advLabelTextBox;
        private System.Windows.Forms.Panel advHeaderPanel;
        private System.Windows.Forms.Panel advSigPanel;
        private System.Windows.Forms.Panel advMainPanel;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox bitFlagSetValueTextBox;
        private System.Windows.Forms.TextBox bitFlagCommentsTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox bitFlagNSetValueTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox bitFlagLabelTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox bitFlagSLabelTextBox;
        private System.Windows.Forms.CheckBox bitFlagSkipCheckBox;
        private System.Windows.Forms.ToolStripMenuItem createAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyXdfToolStripMenuItem;
        private System.Windows.Forms.CheckBox bitFlagHParentCheckBox;
        private System.Windows.Forms.TabPage sharedDetailsTabPage;
        private System.Windows.Forms.TrackBar sharedIdentificationStatusTrackBar;
        private System.Windows.Forms.TextBox sharedIdentificationDetailsTextBox;
        private System.Windows.Forms.Label sharedIdentificationLabel;
        private System.Windows.Forms.ComboBox sharedCateg3ComboBox;
        private System.Windows.Forms.ComboBox sharedCateg2ComboBox;
        private System.Windows.Forms.Label sharedCategsLabel;
        private System.Windows.Forms.ComboBox sharedCategComboBox;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.DateTimePicker sharedDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.DateTimePicker sharedDateCreatedDateTimePicker;
    }
}