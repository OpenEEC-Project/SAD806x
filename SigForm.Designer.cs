namespace SAD806x
{
    partial class SigForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Input Arguments");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Input Structures");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Input Tables");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Input Functions");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Input Scalars");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Internal Structures");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Internal Tables");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Internal Functions");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Internal Scalars");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Signature Elements Detection", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            this.advHeaderPanel = new System.Windows.Forms.Panel();
            this.advSLabelTextBox = new System.Windows.Forms.TextBox();
            this.advLabelTextBox = new System.Windows.Forms.TextBox();
            this.advMainPanel = new System.Windows.Forms.Panel();
            this.advMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.advElemsTreeView = new System.Windows.Forms.TreeView();
            this.advElemsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elemTabControl = new System.Windows.Forms.TabControl();
            this.routineTabPage = new System.Windows.Forms.TabPage();
            this.signatureFor806xComboBox = new System.Windows.Forms.ComboBox();
            this.signatureForBankComboBox = new System.Windows.Forms.ComboBox();
            this.label67 = new System.Windows.Forms.Label();
            this.routineOutputCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.label65 = new System.Windows.Forms.Label();
            this.routineSLabelTextBox = new System.Windows.Forms.TextBox();
            this.routineCommentsTextBox = new System.Windows.Forms.TextBox();
            this.label66 = new System.Windows.Forms.Label();
            this.routineLabelTextBox = new System.Windows.Forms.TextBox();
            this.inputArgumentTabPage = new System.Windows.Forms.TabPage();
            this.inputArgEncryptionComboBox = new System.Windows.Forms.ComboBox();
            this.label60 = new System.Windows.Forms.Label();
            this.inputArgPointerCheckBox = new System.Windows.Forms.CheckBox();
            this.inputArgWordCheckBox = new System.Windows.Forms.CheckBox();
            this.label59 = new System.Windows.Forms.Label();
            this.inputArgPositionTextBox = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.inputArgCodeTextBox = new System.Windows.Forms.TextBox();
            this.inputStructureTabPage = new System.Windows.Forms.TabPage();
            this.inputStructureTipPictureBox = new System.Windows.Forms.PictureBox();
            this.label19 = new System.Windows.Forms.Label();
            this.inputStructureNumFixTextBox = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.inputStructureNumRegTextBox = new System.Windows.Forms.TextBox();
            this.inputStructureStructTextBox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.inputStructureAddrTextBox = new System.Windows.Forms.TextBox();
            this.inputTableTabPage = new System.Windows.Forms.TabPage();
            this.inputTableScalePrecNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.inputTableOutputTextBox = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.inputTableRowsRegTextBox = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.inputTableColsRegTextBox = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.inputTableColsNumFixTextBox = new System.Windows.Forms.TextBox();
            this.inputTableWordCheckBox = new System.Windows.Forms.CheckBox();
            this.inputTableCellsUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.inputTableColsNumRegTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.inputTableScaleTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.inputTableRowsNumTextBox = new System.Windows.Forms.TextBox();
            this.inputTableRowsUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.inputTableColsUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.inputTableAddrTextBox = new System.Windows.Forms.TextBox();
            this.inputTableSignedCheckBox = new System.Windows.Forms.CheckBox();
            this.inputFunctionTabPage = new System.Windows.Forms.TabPage();
            this.inputFunctionScalePrecOutputNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.inputFunctionScalePrecInputNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.inputFunctionOutputTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.inputFunctionInputTextBox = new System.Windows.Forms.TextBox();
            this.inputFunctionByteCheckBox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.inputFunctionScaleOutputTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.inputFunctionScaleInputTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.inputFunctionRowsTextBox = new System.Windows.Forms.TextBox();
            this.inputFunctionUnitsOutputTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.inputFunctionSignedOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.inputFunctionUnitsInputTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.inputFunctionAddrTextBox = new System.Windows.Forms.TextBox();
            this.inputFunctionSignedInputCheckBox = new System.Windows.Forms.CheckBox();
            this.inputScalarTabPage = new System.Windows.Forms.TabPage();
            this.inputScalarScalePrecNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.inputScalarBitFlagsButton = new System.Windows.Forms.Button();
            this.inputScalarBitFlagsCheckBox = new System.Windows.Forms.CheckBox();
            this.inputScalarUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inputScalarByteCheckBox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.inputScalarScaleTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.inputScalarAddrTextBox = new System.Windows.Forms.TextBox();
            this.inputScalarSignedCheckBox = new System.Windows.Forms.CheckBox();
            this.internalStructureTabPage = new System.Windows.Forms.TabPage();
            this.internalStructureTipPictureBox = new System.Windows.Forms.PictureBox();
            this.label61 = new System.Windows.Forms.Label();
            this.internalStructureBankTextBox = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.internalStructureAddrTextBox = new System.Windows.Forms.TextBox();
            this.internalStructureOutputCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.label38 = new System.Windows.Forms.Label();
            this.internalStructureSLabelTextBox = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.internalStructureNumTextBox = new System.Windows.Forms.TextBox();
            this.internalStructureStructTextBox = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.internalStructureCommentsTextBox = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.internalStructureLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalTableTabPage = new System.Windows.Forms.TabPage();
            this.internalTableScalePrecNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label62 = new System.Windows.Forms.Label();
            this.internalTableBankTextBox = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.internalTableAddrTextBox = new System.Windows.Forms.TextBox();
            this.internalTableWordCheckBox = new System.Windows.Forms.CheckBox();
            this.internalTableOutputCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.label37 = new System.Windows.Forms.Label();
            this.internalTableSLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalTableCellsUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.internalTableColsTextBox = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.internalTableScaleTextBox = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.internalTableRowsTextBox = new System.Windows.Forms.TextBox();
            this.internalTableRowsUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.internalTableColsUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.internalTableCommentsTextBox = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.internalTableLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalTableSignedCheckBox = new System.Windows.Forms.CheckBox();
            this.internalFunctionTabPage = new System.Windows.Forms.TabPage();
            this.internalFunctionScalePrecOutputNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.internalFunctionScalePrecInputNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label63 = new System.Windows.Forms.Label();
            this.internalFunctionBankTextBox = new System.Windows.Forms.TextBox();
            this.internalFunctionOutputCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.label44 = new System.Windows.Forms.Label();
            this.internalFunctionSLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalFunctionByteCheckBox = new System.Windows.Forms.CheckBox();
            this.label45 = new System.Windows.Forms.Label();
            this.internalFunctionScaleOutputTextBox = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.internalFunctionScaleInputTextBox = new System.Windows.Forms.TextBox();
            this.label47 = new System.Windows.Forms.Label();
            this.internalFunctionRowsTextBox = new System.Windows.Forms.TextBox();
            this.internalFunctionUnitsOutputTextBox = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.internalFunctionSignedOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.internalFunctionUnitsInputTextBox = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.internalFunctionCommentsTextBox = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.internalFunctionLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalFunctionSignedInputCheckBox = new System.Windows.Forms.CheckBox();
            this.label43 = new System.Windows.Forms.Label();
            this.internalFunctionAddrTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarTabPage = new System.Windows.Forms.TabPage();
            this.internalScalarInlineCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.internalScalarScalePrecNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.internalScalarBitFlagsButton = new System.Windows.Forms.Button();
            this.internalScalarBitFlagsCheckBox = new System.Windows.Forms.CheckBox();
            this.label64 = new System.Windows.Forms.Label();
            this.internalScalarBankTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarOutputCommentsCheckBox = new System.Windows.Forms.CheckBox();
            this.label53 = new System.Windows.Forms.Label();
            this.internalScalarSLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarByteCheckBox = new System.Windows.Forms.CheckBox();
            this.label54 = new System.Windows.Forms.Label();
            this.internalScalarScaleTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.internalScalarCommentsTextBox = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.internalScalarLabelTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarSignedCheckBox = new System.Windows.Forms.CheckBox();
            this.label52 = new System.Windows.Forms.Label();
            this.internalScalarAddrTextBox = new System.Windows.Forms.TextBox();
            this.advSigPanel = new System.Windows.Forms.Panel();
            this.advSigTextBox = new System.Windows.Forms.TextBox();
            this.advFooterPanel = new System.Windows.Forms.Panel();
            this.mainTipPictureBox = new System.Windows.Forms.PictureBox();
            this.elemUpdateButton = new System.Windows.Forms.Button();
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.repoContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.repoToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.repoToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.repoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advHeaderPanel.SuspendLayout();
            this.advMainPanel.SuspendLayout();
            this.advMainSplitContainer.Panel1.SuspendLayout();
            this.advMainSplitContainer.Panel2.SuspendLayout();
            this.advMainSplitContainer.SuspendLayout();
            this.advElemsContextMenuStrip.SuspendLayout();
            this.elemTabControl.SuspendLayout();
            this.routineTabPage.SuspendLayout();
            this.inputArgumentTabPage.SuspendLayout();
            this.inputStructureTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputStructureTipPictureBox)).BeginInit();
            this.inputTableTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputTableScalePrecNumericUpDown)).BeginInit();
            this.inputFunctionTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputFunctionScalePrecOutputNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputFunctionScalePrecInputNumericUpDown)).BeginInit();
            this.inputScalarTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputScalarScalePrecNumericUpDown)).BeginInit();
            this.internalStructureTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalStructureTipPictureBox)).BeginInit();
            this.internalTableTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalTableScalePrecNumericUpDown)).BeginInit();
            this.internalFunctionTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecOutputNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecInputNumericUpDown)).BeginInit();
            this.internalScalarTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalScalarScalePrecNumericUpDown)).BeginInit();
            this.advSigPanel.SuspendLayout();
            this.advFooterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainTipPictureBox)).BeginInit();
            this.repoContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // advHeaderPanel
            // 
            this.advHeaderPanel.BackColor = System.Drawing.SystemColors.Window;
            this.advHeaderPanel.Controls.Add(this.advSLabelTextBox);
            this.advHeaderPanel.Controls.Add(this.advLabelTextBox);
            this.advHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.advHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.advHeaderPanel.Name = "advHeaderPanel";
            this.advHeaderPanel.Size = new System.Drawing.Size(984, 29);
            this.advHeaderPanel.TabIndex = 0;
            // 
            // advSLabelTextBox
            // 
            this.advSLabelTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.advSLabelTextBox.Location = new System.Drawing.Point(884, 0);
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
            this.advLabelTextBox.Size = new System.Drawing.Size(984, 20);
            this.advLabelTextBox.TabIndex = 0;
            // 
            // advMainPanel
            // 
            this.advMainPanel.BackColor = System.Drawing.SystemColors.Window;
            this.advMainPanel.Controls.Add(this.advMainSplitContainer);
            this.advMainPanel.Controls.Add(this.advSigPanel);
            this.advMainPanel.Controls.Add(this.advFooterPanel);
            this.advMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advMainPanel.Location = new System.Drawing.Point(0, 29);
            this.advMainPanel.Name = "advMainPanel";
            this.advMainPanel.Size = new System.Drawing.Size(984, 471);
            this.advMainPanel.TabIndex = 1;
            // 
            // advMainSplitContainer
            // 
            this.advMainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advMainSplitContainer.Location = new System.Drawing.Point(0, 155);
            this.advMainSplitContainer.Name = "advMainSplitContainer";
            // 
            // advMainSplitContainer.Panel1
            // 
            this.advMainSplitContainer.Panel1.Controls.Add(this.advElemsTreeView);
            // 
            // advMainSplitContainer.Panel2
            // 
            this.advMainSplitContainer.Panel2.Controls.Add(this.elemTabControl);
            this.advMainSplitContainer.Size = new System.Drawing.Size(984, 289);
            this.advMainSplitContainer.SplitterDistance = 327;
            this.advMainSplitContainer.TabIndex = 5;
            // 
            // advElemsTreeView
            // 
            this.advElemsTreeView.ContextMenuStrip = this.advElemsContextMenuStrip;
            this.advElemsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advElemsTreeView.Location = new System.Drawing.Point(0, 0);
            this.advElemsTreeView.Name = "advElemsTreeView";
            treeNode1.Name = "INPARGUMENTS";
            treeNode1.Text = "Input Arguments";
            treeNode2.Name = "INPSTRUCTURES";
            treeNode2.Text = "Input Structures";
            treeNode3.Name = "INPTABLES";
            treeNode3.Text = "Input Tables";
            treeNode4.Name = "INPFUNCTIONS";
            treeNode4.Text = "Input Functions";
            treeNode5.Name = "INPSCALARS";
            treeNode5.Text = "Input Scalars";
            treeNode6.Name = "INTSTRUCTURES";
            treeNode6.Text = "Internal Structures";
            treeNode7.Name = "INTTABLES";
            treeNode7.Text = "Internal Tables";
            treeNode8.Name = "INTFUNCTIONS";
            treeNode8.Text = "Internal Functions";
            treeNode9.Name = "INTSCALARS";
            treeNode9.Text = "Internal Scalars";
            treeNode10.Name = "SIGNATURE";
            treeNode10.Text = "Signature Elements Detection";
            this.advElemsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode10});
            this.advElemsTreeView.Size = new System.Drawing.Size(327, 289);
            this.advElemsTreeView.TabIndex = 1;
            // 
            // advElemsContextMenuStrip
            // 
            this.advElemsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newElementToolStripMenuItem,
            this.delElementToolStripMenuItem});
            this.advElemsContextMenuStrip.Name = "advSigElemsContextMenuStrip";
            this.advElemsContextMenuStrip.Size = new System.Drawing.Size(145, 48);
            // 
            // newElementToolStripMenuItem
            // 
            this.newElementToolStripMenuItem.Name = "newElementToolStripMenuItem";
            this.newElementToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.newElementToolStripMenuItem.Text = "New Element";
            this.newElementToolStripMenuItem.Click += new System.EventHandler(this.newElementToolStripMenuItem_Click);
            // 
            // delElementToolStripMenuItem
            // 
            this.delElementToolStripMenuItem.Name = "delElementToolStripMenuItem";
            this.delElementToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.delElementToolStripMenuItem.Text = "Remove";
            this.delElementToolStripMenuItem.Click += new System.EventHandler(this.delElementToolStripMenuItem_Click);
            // 
            // elemTabControl
            // 
            this.elemTabControl.Controls.Add(this.routineTabPage);
            this.elemTabControl.Controls.Add(this.inputArgumentTabPage);
            this.elemTabControl.Controls.Add(this.inputStructureTabPage);
            this.elemTabControl.Controls.Add(this.inputTableTabPage);
            this.elemTabControl.Controls.Add(this.inputFunctionTabPage);
            this.elemTabControl.Controls.Add(this.inputScalarTabPage);
            this.elemTabControl.Controls.Add(this.internalStructureTabPage);
            this.elemTabControl.Controls.Add(this.internalTableTabPage);
            this.elemTabControl.Controls.Add(this.internalFunctionTabPage);
            this.elemTabControl.Controls.Add(this.internalScalarTabPage);
            this.elemTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elemTabControl.Location = new System.Drawing.Point(0, 0);
            this.elemTabControl.Name = "elemTabControl";
            this.elemTabControl.SelectedIndex = 0;
            this.elemTabControl.Size = new System.Drawing.Size(653, 289);
            this.elemTabControl.TabIndex = 2;
            // 
            // routineTabPage
            // 
            this.routineTabPage.AutoScroll = true;
            this.routineTabPage.Controls.Add(this.signatureFor806xComboBox);
            this.routineTabPage.Controls.Add(this.signatureForBankComboBox);
            this.routineTabPage.Controls.Add(this.label67);
            this.routineTabPage.Controls.Add(this.routineOutputCommentsCheckBox);
            this.routineTabPage.Controls.Add(this.label65);
            this.routineTabPage.Controls.Add(this.routineSLabelTextBox);
            this.routineTabPage.Controls.Add(this.routineCommentsTextBox);
            this.routineTabPage.Controls.Add(this.label66);
            this.routineTabPage.Controls.Add(this.routineLabelTextBox);
            this.routineTabPage.Location = new System.Drawing.Point(4, 22);
            this.routineTabPage.Name = "routineTabPage";
            this.routineTabPage.Size = new System.Drawing.Size(645, 263);
            this.routineTabPage.TabIndex = 14;
            this.routineTabPage.Text = "Routine Creation";
            this.routineTabPage.UseVisualStyleBackColor = true;
            // 
            // signatureFor806xComboBox
            // 
            this.signatureFor806xComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signatureFor806xComboBox.FormattingEnabled = true;
            this.signatureFor806xComboBox.Items.AddRange(new object[] {
            "",
            "8061 only",
            "8065 only"});
            this.signatureFor806xComboBox.Location = new System.Drawing.Point(3, 65);
            this.signatureFor806xComboBox.Name = "signatureFor806xComboBox";
            this.signatureFor806xComboBox.Size = new System.Drawing.Size(188, 21);
            this.signatureFor806xComboBox.TabIndex = 80;
            // 
            // signatureForBankComboBox
            // 
            this.signatureForBankComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signatureForBankComboBox.FormattingEnabled = true;
            this.signatureForBankComboBox.Items.AddRange(new object[] {
            "",
            "Bank 8 only",
            "Bank 1 only",
            "Bank 9 only",
            "Bank 0 only"});
            this.signatureForBankComboBox.Location = new System.Drawing.Point(327, 65);
            this.signatureForBankComboBox.Name = "signatureForBankComboBox";
            this.signatureForBankComboBox.Size = new System.Drawing.Size(188, 21);
            this.signatureForBankComboBox.TabIndex = 81;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(3, 145);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(56, 13);
            this.label67.TabIndex = 79;
            this.label67.Text = "Comments";
            // 
            // routineOutputCommentsCheckBox
            // 
            this.routineOutputCommentsCheckBox.AutoSize = true;
            this.routineOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.routineOutputCommentsCheckBox.Location = new System.Drawing.Point(405, 144);
            this.routineOutputCommentsCheckBox.Name = "routineOutputCommentsCheckBox";
            this.routineOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.routineOutputCommentsCheckBox.TabIndex = 76;
            this.routineOutputCommentsCheckBox.Text = "Output Comments";
            this.routineOutputCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(454, 14);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(61, 13);
            this.label65.TabIndex = 78;
            this.label65.Text = "Short Label";
            // 
            // routineSLabelTextBox
            // 
            this.routineSLabelTextBox.Location = new System.Drawing.Point(327, 30);
            this.routineSLabelTextBox.Name = "routineSLabelTextBox";
            this.routineSLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.routineSLabelTextBox.TabIndex = 74;
            // 
            // routineCommentsTextBox
            // 
            this.routineCommentsTextBox.Location = new System.Drawing.Point(3, 161);
            this.routineCommentsTextBox.Multiline = true;
            this.routineCommentsTextBox.Name = "routineCommentsTextBox";
            this.routineCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.routineCommentsTextBox.Size = new System.Drawing.Size(512, 42);
            this.routineCommentsTextBox.TabIndex = 75;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(3, 14);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(33, 13);
            this.label66.TabIndex = 77;
            this.label66.Text = "Label";
            // 
            // routineLabelTextBox
            // 
            this.routineLabelTextBox.Location = new System.Drawing.Point(3, 30);
            this.routineLabelTextBox.Name = "routineLabelTextBox";
            this.routineLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.routineLabelTextBox.TabIndex = 73;
            // 
            // inputArgumentTabPage
            // 
            this.inputArgumentTabPage.AutoScroll = true;
            this.inputArgumentTabPage.Controls.Add(this.inputArgEncryptionComboBox);
            this.inputArgumentTabPage.Controls.Add(this.label60);
            this.inputArgumentTabPage.Controls.Add(this.inputArgPointerCheckBox);
            this.inputArgumentTabPage.Controls.Add(this.inputArgWordCheckBox);
            this.inputArgumentTabPage.Controls.Add(this.label59);
            this.inputArgumentTabPage.Controls.Add(this.inputArgPositionTextBox);
            this.inputArgumentTabPage.Controls.Add(this.label58);
            this.inputArgumentTabPage.Controls.Add(this.inputArgCodeTextBox);
            this.inputArgumentTabPage.Location = new System.Drawing.Point(4, 22);
            this.inputArgumentTabPage.Name = "inputArgumentTabPage";
            this.inputArgumentTabPage.Size = new System.Drawing.Size(645, 263);
            this.inputArgumentTabPage.TabIndex = 13;
            this.inputArgumentTabPage.Text = "Input Argument";
            this.inputArgumentTabPage.UseVisualStyleBackColor = true;
            // 
            // inputArgEncryptionComboBox
            // 
            this.inputArgEncryptionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputArgEncryptionComboBox.FormattingEnabled = true;
            this.inputArgEncryptionComboBox.Location = new System.Drawing.Point(268, 31);
            this.inputArgEncryptionComboBox.Name = "inputArgEncryptionComboBox";
            this.inputArgEncryptionComboBox.Size = new System.Drawing.Size(153, 21);
            this.inputArgEncryptionComboBox.TabIndex = 4;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(364, 15);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(57, 13);
            this.label60.TabIndex = 61;
            this.label60.Text = "Encryption";
            // 
            // inputArgPointerCheckBox
            // 
            this.inputArgPointerCheckBox.AutoSize = true;
            this.inputArgPointerCheckBox.Location = new System.Drawing.Point(313, 72);
            this.inputArgPointerCheckBox.Name = "inputArgPointerCheckBox";
            this.inputArgPointerCheckBox.Size = new System.Drawing.Size(108, 17);
            this.inputArgPointerCheckBox.TabIndex = 6;
            this.inputArgPointerCheckBox.Text = "Output as Pointer";
            this.inputArgPointerCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputArgWordCheckBox
            // 
            this.inputArgWordCheckBox.AutoSize = true;
            this.inputArgWordCheckBox.Location = new System.Drawing.Point(20, 72);
            this.inputArgWordCheckBox.Name = "inputArgWordCheckBox";
            this.inputArgWordCheckBox.Size = new System.Drawing.Size(52, 17);
            this.inputArgWordCheckBox.TabIndex = 5;
            this.inputArgWordCheckBox.Text = "Word";
            this.inputArgWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(166, 15);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(44, 13);
            this.label59.TabIndex = 50;
            this.label59.Text = "Position";
            // 
            // inputArgPositionTextBox
            // 
            this.inputArgPositionTextBox.Location = new System.Drawing.Point(141, 31);
            this.inputArgPositionTextBox.Name = "inputArgPositionTextBox";
            this.inputArgPositionTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputArgPositionTextBox.TabIndex = 3;
            this.inputArgPositionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(0, 15);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(79, 13);
            this.label58.TabIndex = 48;
            this.label58.Text = "Matching Code";
            // 
            // inputArgCodeTextBox
            // 
            this.inputArgCodeTextBox.Location = new System.Drawing.Point(3, 31);
            this.inputArgCodeTextBox.Name = "inputArgCodeTextBox";
            this.inputArgCodeTextBox.ReadOnly = true;
            this.inputArgCodeTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputArgCodeTextBox.TabIndex = 2;
            // 
            // inputStructureTabPage
            // 
            this.inputStructureTabPage.AutoScroll = true;
            this.inputStructureTabPage.Controls.Add(this.inputStructureTipPictureBox);
            this.inputStructureTabPage.Controls.Add(this.label19);
            this.inputStructureTabPage.Controls.Add(this.inputStructureNumFixTextBox);
            this.inputStructureTabPage.Controls.Add(this.label21);
            this.inputStructureTabPage.Controls.Add(this.inputStructureNumRegTextBox);
            this.inputStructureTabPage.Controls.Add(this.inputStructureStructTextBox);
            this.inputStructureTabPage.Controls.Add(this.label20);
            this.inputStructureTabPage.Controls.Add(this.label34);
            this.inputStructureTabPage.Controls.Add(this.inputStructureAddrTextBox);
            this.inputStructureTabPage.Location = new System.Drawing.Point(4, 22);
            this.inputStructureTabPage.Name = "inputStructureTabPage";
            this.inputStructureTabPage.Size = new System.Drawing.Size(645, 263);
            this.inputStructureTabPage.TabIndex = 8;
            this.inputStructureTabPage.Text = "Input Structure";
            this.inputStructureTabPage.UseVisualStyleBackColor = true;
            // 
            // inputStructureTipPictureBox
            // 
            this.inputStructureTipPictureBox.Image = global::SAD806x.Properties.Resources.question;
            this.inputStructureTipPictureBox.Location = new System.Drawing.Point(409, 14);
            this.inputStructureTipPictureBox.Name = "inputStructureTipPictureBox";
            this.inputStructureTipPictureBox.Size = new System.Drawing.Size(50, 50);
            this.inputStructureTipPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.inputStructureTipPictureBox.TabIndex = 55;
            this.inputStructureTipPictureBox.TabStop = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(298, 60);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(80, 13);
            this.label19.TabIndex = 40;
            this.label19.Text = "Forced Number";
            // 
            // inputStructureNumFixTextBox
            // 
            this.inputStructureNumFixTextBox.Location = new System.Drawing.Point(337, 76);
            this.inputStructureNumFixTextBox.Name = "inputStructureNumFixTextBox";
            this.inputStructureNumFixTextBox.Size = new System.Drawing.Size(41, 20);
            this.inputStructureNumFixTextBox.TabIndex = 4;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(242, 14);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(136, 13);
            this.label21.TabIndex = 37;
            this.label21.Text = "Number Register/Argument";
            // 
            // inputStructureNumRegTextBox
            // 
            this.inputStructureNumRegTextBox.Location = new System.Drawing.Point(337, 30);
            this.inputStructureNumRegTextBox.Name = "inputStructureNumRegTextBox";
            this.inputStructureNumRegTextBox.Size = new System.Drawing.Size(41, 20);
            this.inputStructureNumRegTextBox.TabIndex = 3;
            // 
            // inputStructureStructTextBox
            // 
            this.inputStructureStructTextBox.Location = new System.Drawing.Point(3, 111);
            this.inputStructureStructTextBox.Multiline = true;
            this.inputStructureStructTextBox.Name = "inputStructureStructTextBox";
            this.inputStructureStructTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.inputStructureStructTextBox.Size = new System.Drawing.Size(456, 71);
            this.inputStructureStructTextBox.TabIndex = 5;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(0, 95);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(50, 13);
            this.label20.TabIndex = 22;
            this.label20.Text = "Structure";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(0, 14);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(164, 13);
            this.label34.TabIndex = 10;
            this.label34.Text = "Address Input Register/Argument";
            // 
            // inputStructureAddrTextBox
            // 
            this.inputStructureAddrTextBox.Location = new System.Drawing.Point(3, 30);
            this.inputStructureAddrTextBox.Name = "inputStructureAddrTextBox";
            this.inputStructureAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.inputStructureAddrTextBox.TabIndex = 2;
            // 
            // inputTableTabPage
            // 
            this.inputTableTabPage.AutoScroll = true;
            this.inputTableTabPage.Controls.Add(this.inputTableScalePrecNumericUpDown);
            this.inputTableTabPage.Controls.Add(this.label25);
            this.inputTableTabPage.Controls.Add(this.inputTableOutputTextBox);
            this.inputTableTabPage.Controls.Add(this.label23);
            this.inputTableTabPage.Controls.Add(this.inputTableRowsRegTextBox);
            this.inputTableTabPage.Controls.Add(this.label24);
            this.inputTableTabPage.Controls.Add(this.inputTableColsRegTextBox);
            this.inputTableTabPage.Controls.Add(this.label22);
            this.inputTableTabPage.Controls.Add(this.label17);
            this.inputTableTabPage.Controls.Add(this.inputTableColsNumFixTextBox);
            this.inputTableTabPage.Controls.Add(this.inputTableWordCheckBox);
            this.inputTableTabPage.Controls.Add(this.inputTableCellsUnitsTextBox);
            this.inputTableTabPage.Controls.Add(this.label12);
            this.inputTableTabPage.Controls.Add(this.inputTableColsNumRegTextBox);
            this.inputTableTabPage.Controls.Add(this.label13);
            this.inputTableTabPage.Controls.Add(this.inputTableScaleTextBox);
            this.inputTableTabPage.Controls.Add(this.label14);
            this.inputTableTabPage.Controls.Add(this.inputTableRowsNumTextBox);
            this.inputTableTabPage.Controls.Add(this.inputTableRowsUnitsTextBox);
            this.inputTableTabPage.Controls.Add(this.label15);
            this.inputTableTabPage.Controls.Add(this.inputTableColsUnitsTextBox);
            this.inputTableTabPage.Controls.Add(this.label16);
            this.inputTableTabPage.Controls.Add(this.label18);
            this.inputTableTabPage.Controls.Add(this.inputTableAddrTextBox);
            this.inputTableTabPage.Controls.Add(this.inputTableSignedCheckBox);
            this.inputTableTabPage.Location = new System.Drawing.Point(4, 22);
            this.inputTableTabPage.Name = "inputTableTabPage";
            this.inputTableTabPage.Size = new System.Drawing.Size(645, 263);
            this.inputTableTabPage.TabIndex = 3;
            this.inputTableTabPage.Text = "Input Table";
            this.inputTableTabPage.UseVisualStyleBackColor = true;
            // 
            // inputTableScalePrecNumericUpDown
            // 
            this.inputTableScalePrecNumericUpDown.Location = new System.Drawing.Point(490, 183);
            this.inputTableScalePrecNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.inputTableScalePrecNumericUpDown.Name = "inputTableScalePrecNumericUpDown";
            this.inputTableScalePrecNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.inputTableScalePrecNumericUpDown.TabIndex = 12;
            this.inputTableScalePrecNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(398, 65);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(81, 13);
            this.label25.TabIndex = 53;
            this.label25.Text = "Output Register";
            // 
            // inputTableOutputTextBox
            // 
            this.inputTableOutputTextBox.Location = new System.Drawing.Point(401, 81);
            this.inputTableOutputTextBox.Name = "inputTableOutputTextBox";
            this.inputTableOutputTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputTableOutputTextBox.TabIndex = 6;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(193, 65);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(153, 13);
            this.label23.TabIndex = 51;
            this.label23.Text = "Rows Input Register/Argument";
            // 
            // inputTableRowsRegTextBox
            // 
            this.inputTableRowsRegTextBox.Location = new System.Drawing.Point(196, 81);
            this.inputTableRowsRegTextBox.Name = "inputTableRowsRegTextBox";
            this.inputTableRowsRegTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputTableRowsRegTextBox.TabIndex = 5;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(0, 65);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(166, 13);
            this.label24.TabIndex = 49;
            this.label24.Text = "Columns Input Register/Argument";
            // 
            // inputTableColsRegTextBox
            // 
            this.inputTableColsRegTextBox.Location = new System.Drawing.Point(3, 81);
            this.inputTableColsRegTextBox.Name = "inputTableColsRegTextBox";
            this.inputTableColsRegTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputTableColsRegTextBox.TabIndex = 4;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(0, 167);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(123, 13);
            this.label22.TabIndex = 47;
            this.label22.Text = "Forced Columns Number";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(342, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(179, 13);
            this.label17.TabIndex = 46;
            this.label17.Text = "Columns Number Register/Argument";
            // 
            // inputTableColsNumFixTextBox
            // 
            this.inputTableColsNumFixTextBox.Location = new System.Drawing.Point(3, 183);
            this.inputTableColsNumFixTextBox.Name = "inputTableColsNumFixTextBox";
            this.inputTableColsNumFixTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableColsNumFixTextBox.TabIndex = 9;
            // 
            // inputTableWordCheckBox
            // 
            this.inputTableWordCheckBox.AutoSize = true;
            this.inputTableWordCheckBox.Location = new System.Drawing.Point(401, 117);
            this.inputTableWordCheckBox.Name = "inputTableWordCheckBox";
            this.inputTableWordCheckBox.Size = new System.Drawing.Size(52, 17);
            this.inputTableWordCheckBox.TabIndex = 7;
            this.inputTableWordCheckBox.Text = "Word";
            this.inputTableWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputTableCellsUnitsTextBox
            // 
            this.inputTableCellsUnitsTextBox.Location = new System.Drawing.Point(366, 235);
            this.inputTableCellsUnitsTextBox.Name = "inputTableCellsUnitsTextBox";
            this.inputTableCellsUnitsTextBox.Size = new System.Drawing.Size(155, 20);
            this.inputTableCellsUnitsTextBox.TabIndex = 15;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(363, 219);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 44;
            this.label12.Text = "Forced Cells Units";
            // 
            // inputTableColsNumRegTextBox
            // 
            this.inputTableColsNumRegTextBox.Location = new System.Drawing.Point(452, 32);
            this.inputTableColsNumRegTextBox.Name = "inputTableColsNumRegTextBox";
            this.inputTableColsNumRegTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableColsNumRegTextBox.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(451, 167);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 37;
            this.label13.Text = "Forced Scale";
            // 
            // inputTableScaleTextBox
            // 
            this.inputTableScaleTextBox.Location = new System.Drawing.Point(415, 183);
            this.inputTableScaleTextBox.Name = "inputTableScaleTextBox";
            this.inputTableScaleTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableScaleTextBox.TabIndex = 11;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(193, 167);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 13);
            this.label14.TabIndex = 35;
            this.label14.Text = "Forced Rows Number";
            // 
            // inputTableRowsNumTextBox
            // 
            this.inputTableRowsNumTextBox.Location = new System.Drawing.Point(196, 183);
            this.inputTableRowsNumTextBox.Name = "inputTableRowsNumTextBox";
            this.inputTableRowsNumTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableRowsNumTextBox.TabIndex = 10;
            // 
            // inputTableRowsUnitsTextBox
            // 
            this.inputTableRowsUnitsTextBox.Location = new System.Drawing.Point(196, 235);
            this.inputTableRowsUnitsTextBox.Name = "inputTableRowsUnitsTextBox";
            this.inputTableRowsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.inputTableRowsUnitsTextBox.TabIndex = 14;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(193, 219);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(97, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "Forced Rows Units";
            // 
            // inputTableColsUnitsTextBox
            // 
            this.inputTableColsUnitsTextBox.Location = new System.Drawing.Point(3, 235);
            this.inputTableColsUnitsTextBox.Name = "inputTableColsUnitsTextBox";
            this.inputTableColsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.inputTableColsUnitsTextBox.TabIndex = 13;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(0, 219);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(110, 13);
            this.label16.TabIndex = 29;
            this.label16.Text = "Forced Columns Units";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(0, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(164, 13);
            this.label18.TabIndex = 26;
            this.label18.Text = "Address Input Register/Argument";
            // 
            // inputTableAddrTextBox
            // 
            this.inputTableAddrTextBox.Location = new System.Drawing.Point(3, 32);
            this.inputTableAddrTextBox.Name = "inputTableAddrTextBox";
            this.inputTableAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.inputTableAddrTextBox.TabIndex = 2;
            // 
            // inputTableSignedCheckBox
            // 
            this.inputTableSignedCheckBox.AutoSize = true;
            this.inputTableSignedCheckBox.Location = new System.Drawing.Point(462, 117);
            this.inputTableSignedCheckBox.Name = "inputTableSignedCheckBox";
            this.inputTableSignedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.inputTableSignedCheckBox.TabIndex = 8;
            this.inputTableSignedCheckBox.Text = "Signed";
            this.inputTableSignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputFunctionTabPage
            // 
            this.inputFunctionTabPage.AutoScroll = true;
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionScalePrecOutputNumericUpDown);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionScalePrecInputNumericUpDown);
            this.inputFunctionTabPage.Controls.Add(this.label5);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionOutputTextBox);
            this.inputFunctionTabPage.Controls.Add(this.label3);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionInputTextBox);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionByteCheckBox);
            this.inputFunctionTabPage.Controls.Add(this.label11);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionScaleOutputTextBox);
            this.inputFunctionTabPage.Controls.Add(this.label10);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionScaleInputTextBox);
            this.inputFunctionTabPage.Controls.Add(this.label8);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionRowsTextBox);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionUnitsOutputTextBox);
            this.inputFunctionTabPage.Controls.Add(this.label7);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionSignedOutputCheckBox);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionUnitsInputTextBox);
            this.inputFunctionTabPage.Controls.Add(this.label4);
            this.inputFunctionTabPage.Controls.Add(this.label6);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionAddrTextBox);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionSignedInputCheckBox);
            this.inputFunctionTabPage.Location = new System.Drawing.Point(4, 22);
            this.inputFunctionTabPage.Name = "inputFunctionTabPage";
            this.inputFunctionTabPage.Size = new System.Drawing.Size(645, 263);
            this.inputFunctionTabPage.TabIndex = 2;
            this.inputFunctionTabPage.Text = "Input Function";
            this.inputFunctionTabPage.UseVisualStyleBackColor = true;
            // 
            // inputFunctionScalePrecOutputNumericUpDown
            // 
            this.inputFunctionScalePrecOutputNumericUpDown.Location = new System.Drawing.Point(490, 180);
            this.inputFunctionScalePrecOutputNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.inputFunctionScalePrecOutputNumericUpDown.Name = "inputFunctionScalePrecOutputNumericUpDown";
            this.inputFunctionScalePrecOutputNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.inputFunctionScalePrecOutputNumericUpDown.TabIndex = 12;
            this.inputFunctionScalePrecOutputNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // inputFunctionScalePrecInputNumericUpDown
            // 
            this.inputFunctionScalePrecInputNumericUpDown.Location = new System.Drawing.Point(160, 180);
            this.inputFunctionScalePrecInputNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.inputFunctionScalePrecInputNumericUpDown.Name = "inputFunctionScalePrecInputNumericUpDown";
            this.inputFunctionScalePrecInputNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.inputFunctionScalePrecInputNumericUpDown.TabIndex = 10;
            this.inputFunctionScalePrecInputNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(398, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Output Register";
            // 
            // inputFunctionOutputTextBox
            // 
            this.inputFunctionOutputTextBox.Location = new System.Drawing.Point(401, 33);
            this.inputFunctionOutputTextBox.Name = "inputFunctionOutputTextBox";
            this.inputFunctionOutputTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputFunctionOutputTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Input Register/Argument";
            // 
            // inputFunctionInputTextBox
            // 
            this.inputFunctionInputTextBox.Location = new System.Drawing.Point(213, 33);
            this.inputFunctionInputTextBox.Name = "inputFunctionInputTextBox";
            this.inputFunctionInputTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputFunctionInputTextBox.TabIndex = 3;
            // 
            // inputFunctionByteCheckBox
            // 
            this.inputFunctionByteCheckBox.AutoSize = true;
            this.inputFunctionByteCheckBox.Location = new System.Drawing.Point(3, 59);
            this.inputFunctionByteCheckBox.Name = "inputFunctionByteCheckBox";
            this.inputFunctionByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.inputFunctionByteCheckBox.TabIndex = 5;
            this.inputFunctionByteCheckBox.Text = "Byte";
            this.inputFunctionByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(416, 164);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Forced Output Scale";
            // 
            // inputFunctionScaleOutputTextBox
            // 
            this.inputFunctionScaleOutputTextBox.Location = new System.Drawing.Point(415, 180);
            this.inputFunctionScaleOutputTextBox.Name = "inputFunctionScaleOutputTextBox";
            this.inputFunctionScaleOutputTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputFunctionScaleOutputTextBox.TabIndex = 11;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(94, 164);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Forced Input Scale";
            // 
            // inputFunctionScaleInputTextBox
            // 
            this.inputFunctionScaleInputTextBox.Location = new System.Drawing.Point(85, 180);
            this.inputFunctionScaleInputTextBox.Name = "inputFunctionScaleInputTextBox";
            this.inputFunctionScaleInputTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputFunctionScaleInputTextBox.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Forced Rows Number";
            // 
            // inputFunctionRowsTextBox
            // 
            this.inputFunctionRowsTextBox.Location = new System.Drawing.Point(3, 130);
            this.inputFunctionRowsTextBox.Name = "inputFunctionRowsTextBox";
            this.inputFunctionRowsTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputFunctionRowsTextBox.TabIndex = 8;
            // 
            // inputFunctionUnitsOutputTextBox
            // 
            this.inputFunctionUnitsOutputTextBox.Location = new System.Drawing.Point(339, 219);
            this.inputFunctionUnitsOutputTextBox.Name = "inputFunctionUnitsOutputTextBox";
            this.inputFunctionUnitsOutputTextBox.Size = new System.Drawing.Size(182, 20);
            this.inputFunctionUnitsOutputTextBox.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(336, 203);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Forced Output Units";
            // 
            // inputFunctionSignedOutputCheckBox
            // 
            this.inputFunctionSignedOutputCheckBox.AutoSize = true;
            this.inputFunctionSignedOutputCheckBox.Location = new System.Drawing.Point(401, 59);
            this.inputFunctionSignedOutputCheckBox.Name = "inputFunctionSignedOutputCheckBox";
            this.inputFunctionSignedOutputCheckBox.Size = new System.Drawing.Size(94, 17);
            this.inputFunctionSignedOutputCheckBox.TabIndex = 7;
            this.inputFunctionSignedOutputCheckBox.Text = "Signed Output";
            this.inputFunctionSignedOutputCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputFunctionUnitsInputTextBox
            // 
            this.inputFunctionUnitsInputTextBox.Location = new System.Drawing.Point(3, 219);
            this.inputFunctionUnitsInputTextBox.Name = "inputFunctionUnitsInputTextBox";
            this.inputFunctionUnitsInputTextBox.Size = new System.Drawing.Size(188, 20);
            this.inputFunctionUnitsInputTextBox.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Forced Input Units";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(164, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Address Input Register/Argument";
            // 
            // inputFunctionAddrTextBox
            // 
            this.inputFunctionAddrTextBox.Location = new System.Drawing.Point(3, 33);
            this.inputFunctionAddrTextBox.Name = "inputFunctionAddrTextBox";
            this.inputFunctionAddrTextBox.Size = new System.Drawing.Size(161, 20);
            this.inputFunctionAddrTextBox.TabIndex = 2;
            // 
            // inputFunctionSignedInputCheckBox
            // 
            this.inputFunctionSignedInputCheckBox.AutoSize = true;
            this.inputFunctionSignedInputCheckBox.Location = new System.Drawing.Point(213, 59);
            this.inputFunctionSignedInputCheckBox.Name = "inputFunctionSignedInputCheckBox";
            this.inputFunctionSignedInputCheckBox.Size = new System.Drawing.Size(86, 17);
            this.inputFunctionSignedInputCheckBox.TabIndex = 6;
            this.inputFunctionSignedInputCheckBox.Text = "Signed Input";
            this.inputFunctionSignedInputCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputScalarTabPage
            // 
            this.inputScalarTabPage.AutoScroll = true;
            this.inputScalarTabPage.Controls.Add(this.inputScalarScalePrecNumericUpDown);
            this.inputScalarTabPage.Controls.Add(this.inputScalarBitFlagsButton);
            this.inputScalarTabPage.Controls.Add(this.inputScalarBitFlagsCheckBox);
            this.inputScalarTabPage.Controls.Add(this.inputScalarUnitsTextBox);
            this.inputScalarTabPage.Controls.Add(this.label2);
            this.inputScalarTabPage.Controls.Add(this.inputScalarByteCheckBox);
            this.inputScalarTabPage.Controls.Add(this.label9);
            this.inputScalarTabPage.Controls.Add(this.inputScalarScaleTextBox);
            this.inputScalarTabPage.Controls.Add(this.label1);
            this.inputScalarTabPage.Controls.Add(this.inputScalarAddrTextBox);
            this.inputScalarTabPage.Controls.Add(this.inputScalarSignedCheckBox);
            this.inputScalarTabPage.Location = new System.Drawing.Point(4, 22);
            this.inputScalarTabPage.Name = "inputScalarTabPage";
            this.inputScalarTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.inputScalarTabPage.Size = new System.Drawing.Size(645, 263);
            this.inputScalarTabPage.TabIndex = 0;
            this.inputScalarTabPage.Text = "Input Scalar";
            this.inputScalarTabPage.UseVisualStyleBackColor = true;
            // 
            // inputScalarScalePrecNumericUpDown
            // 
            this.inputScalarScalePrecNumericUpDown.Location = new System.Drawing.Point(387, 104);
            this.inputScalarScalePrecNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.inputScalarScalePrecNumericUpDown.Name = "inputScalarScalePrecNumericUpDown";
            this.inputScalarScalePrecNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.inputScalarScalePrecNumericUpDown.TabIndex = 7;
            this.inputScalarScalePrecNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // inputScalarBitFlagsButton
            // 
            this.inputScalarBitFlagsButton.Location = new System.Drawing.Point(315, 30);
            this.inputScalarBitFlagsButton.Name = "inputScalarBitFlagsButton";
            this.inputScalarBitFlagsButton.Size = new System.Drawing.Size(66, 23);
            this.inputScalarBitFlagsButton.TabIndex = 95;
            this.inputScalarBitFlagsButton.Text = "Bit Flags";
            this.inputScalarBitFlagsButton.UseVisualStyleBackColor = true;
            this.inputScalarBitFlagsButton.Click += new System.EventHandler(this.inputScalarBitFlagsButton_Click);
            // 
            // inputScalarBitFlagsCheckBox
            // 
            this.inputScalarBitFlagsCheckBox.AutoSize = true;
            this.inputScalarBitFlagsCheckBox.Enabled = false;
            this.inputScalarBitFlagsCheckBox.Location = new System.Drawing.Point(315, 14);
            this.inputScalarBitFlagsCheckBox.Name = "inputScalarBitFlagsCheckBox";
            this.inputScalarBitFlagsCheckBox.Size = new System.Drawing.Size(66, 17);
            this.inputScalarBitFlagsCheckBox.TabIndex = 96;
            this.inputScalarBitFlagsCheckBox.Text = "Bit Flags";
            this.inputScalarBitFlagsCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputScalarUnitsTextBox
            // 
            this.inputScalarUnitsTextBox.Location = new System.Drawing.Point(3, 104);
            this.inputScalarUnitsTextBox.Name = "inputScalarUnitsTextBox";
            this.inputScalarUnitsTextBox.Size = new System.Drawing.Size(188, 20);
            this.inputScalarUnitsTextBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Forced Units";
            // 
            // inputScalarByteCheckBox
            // 
            this.inputScalarByteCheckBox.AutoSize = true;
            this.inputScalarByteCheckBox.Location = new System.Drawing.Point(223, 34);
            this.inputScalarByteCheckBox.Name = "inputScalarByteCheckBox";
            this.inputScalarByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.inputScalarByteCheckBox.TabIndex = 4;
            this.inputScalarByteCheckBox.Text = "Byte";
            this.inputScalarByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(311, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Forced Scale";
            // 
            // inputScalarScaleTextBox
            // 
            this.inputScalarScaleTextBox.Location = new System.Drawing.Point(310, 104);
            this.inputScalarScaleTextBox.Name = "inputScalarScaleTextBox";
            this.inputScalarScaleTextBox.Size = new System.Drawing.Size(71, 20);
            this.inputScalarScaleTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Address Input Register/Argument";
            // 
            // inputScalarAddrTextBox
            // 
            this.inputScalarAddrTextBox.Location = new System.Drawing.Point(3, 31);
            this.inputScalarAddrTextBox.Name = "inputScalarAddrTextBox";
            this.inputScalarAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.inputScalarAddrTextBox.TabIndex = 2;
            // 
            // inputScalarSignedCheckBox
            // 
            this.inputScalarSignedCheckBox.AutoSize = true;
            this.inputScalarSignedCheckBox.Location = new System.Drawing.Point(223, 14);
            this.inputScalarSignedCheckBox.Name = "inputScalarSignedCheckBox";
            this.inputScalarSignedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.inputScalarSignedCheckBox.TabIndex = 3;
            this.inputScalarSignedCheckBox.Text = "Signed";
            this.inputScalarSignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalStructureTabPage
            // 
            this.internalStructureTabPage.AutoScroll = true;
            this.internalStructureTabPage.Controls.Add(this.internalStructureTipPictureBox);
            this.internalStructureTabPage.Controls.Add(this.label61);
            this.internalStructureTabPage.Controls.Add(this.internalStructureBankTextBox);
            this.internalStructureTabPage.Controls.Add(this.label26);
            this.internalStructureTabPage.Controls.Add(this.internalStructureAddrTextBox);
            this.internalStructureTabPage.Controls.Add(this.internalStructureOutputCommentsCheckBox);
            this.internalStructureTabPage.Controls.Add(this.label38);
            this.internalStructureTabPage.Controls.Add(this.internalStructureSLabelTextBox);
            this.internalStructureTabPage.Controls.Add(this.label27);
            this.internalStructureTabPage.Controls.Add(this.internalStructureNumTextBox);
            this.internalStructureTabPage.Controls.Add(this.internalStructureStructTextBox);
            this.internalStructureTabPage.Controls.Add(this.label28);
            this.internalStructureTabPage.Controls.Add(this.internalStructureCommentsTextBox);
            this.internalStructureTabPage.Controls.Add(this.label33);
            this.internalStructureTabPage.Controls.Add(this.label29);
            this.internalStructureTabPage.Controls.Add(this.internalStructureLabelTextBox);
            this.internalStructureTabPage.Location = new System.Drawing.Point(4, 22);
            this.internalStructureTabPage.Name = "internalStructureTabPage";
            this.internalStructureTabPage.Size = new System.Drawing.Size(645, 263);
            this.internalStructureTabPage.TabIndex = 9;
            this.internalStructureTabPage.Text = "Internal Structure";
            this.internalStructureTabPage.UseVisualStyleBackColor = true;
            // 
            // internalStructureTipPictureBox
            // 
            this.internalStructureTipPictureBox.Image = global::SAD806x.Properties.Resources.question;
            this.internalStructureTipPictureBox.Location = new System.Drawing.Point(465, 14);
            this.internalStructureTipPictureBox.Name = "internalStructureTipPictureBox";
            this.internalStructureTipPictureBox.Size = new System.Drawing.Size(50, 50);
            this.internalStructureTipPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.internalStructureTipPictureBox.TabIndex = 71;
            this.internalStructureTipPictureBox.TabStop = false;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(241, 14);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(32, 13);
            this.label61.TabIndex = 70;
            this.label61.Text = "Bank";
            // 
            // internalStructureBankTextBox
            // 
            this.internalStructureBankTextBox.Location = new System.Drawing.Point(244, 30);
            this.internalStructureBankTextBox.Name = "internalStructureBankTextBox";
            this.internalStructureBankTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalStructureBankTextBox.TabIndex = 3;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(0, 14);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(45, 13);
            this.label26.TabIndex = 68;
            this.label26.Text = "Address";
            // 
            // internalStructureAddrTextBox
            // 
            this.internalStructureAddrTextBox.Location = new System.Drawing.Point(3, 30);
            this.internalStructureAddrTextBox.Name = "internalStructureAddrTextBox";
            this.internalStructureAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalStructureAddrTextBox.TabIndex = 2;
            // 
            // internalStructureOutputCommentsCheckBox
            // 
            this.internalStructureOutputCommentsCheckBox.AutoSize = true;
            this.internalStructureOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalStructureOutputCommentsCheckBox.Location = new System.Drawing.Point(405, 181);
            this.internalStructureOutputCommentsCheckBox.Name = "internalStructureOutputCommentsCheckBox";
            this.internalStructureOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.internalStructureOutputCommentsCheckBox.TabIndex = 8;
            this.internalStructureOutputCommentsCheckBox.Text = "Output Comments";
            this.internalStructureOutputCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(241, 54);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(61, 13);
            this.label38.TabIndex = 66;
            this.label38.Text = "Short Label";
            // 
            // internalStructureSLabelTextBox
            // 
            this.internalStructureSLabelTextBox.Location = new System.Drawing.Point(244, 69);
            this.internalStructureSLabelTextBox.Name = "internalStructureSLabelTextBox";
            this.internalStructureSLabelTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalStructureSLabelTextBox.TabIndex = 4;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(334, 53);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(44, 13);
            this.label27.TabIndex = 64;
            this.label27.Text = "Number";
            // 
            // internalStructureNumTextBox
            // 
            this.internalStructureNumTextBox.Location = new System.Drawing.Point(337, 69);
            this.internalStructureNumTextBox.Name = "internalStructureNumTextBox";
            this.internalStructureNumTextBox.Size = new System.Drawing.Size(41, 20);
            this.internalStructureNumTextBox.TabIndex = 5;
            this.internalStructureNumTextBox.Text = "0";
            // 
            // internalStructureStructTextBox
            // 
            this.internalStructureStructTextBox.Location = new System.Drawing.Point(3, 108);
            this.internalStructureStructTextBox.Multiline = true;
            this.internalStructureStructTextBox.Name = "internalStructureStructTextBox";
            this.internalStructureStructTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalStructureStructTextBox.Size = new System.Drawing.Size(512, 71);
            this.internalStructureStructTextBox.TabIndex = 6;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(0, 92);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(50, 13);
            this.label28.TabIndex = 63;
            this.label28.Text = "Structure";
            // 
            // internalStructureCommentsTextBox
            // 
            this.internalStructureCommentsTextBox.Location = new System.Drawing.Point(3, 198);
            this.internalStructureCommentsTextBox.Multiline = true;
            this.internalStructureCommentsTextBox.Name = "internalStructureCommentsTextBox";
            this.internalStructureCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalStructureCommentsTextBox.Size = new System.Drawing.Size(512, 42);
            this.internalStructureCommentsTextBox.TabIndex = 7;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(0, 182);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(56, 13);
            this.label33.TabIndex = 62;
            this.label33.Text = "Comments";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(0, 53);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(33, 13);
            this.label29.TabIndex = 61;
            this.label29.Text = "Label";
            // 
            // internalStructureLabelTextBox
            // 
            this.internalStructureLabelTextBox.Location = new System.Drawing.Point(3, 69);
            this.internalStructureLabelTextBox.Name = "internalStructureLabelTextBox";
            this.internalStructureLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalStructureLabelTextBox.TabIndex = 3;
            // 
            // internalTableTabPage
            // 
            this.internalTableTabPage.AutoScroll = true;
            this.internalTableTabPage.Controls.Add(this.internalTableScalePrecNumericUpDown);
            this.internalTableTabPage.Controls.Add(this.label62);
            this.internalTableTabPage.Controls.Add(this.internalTableBankTextBox);
            this.internalTableTabPage.Controls.Add(this.label42);
            this.internalTableTabPage.Controls.Add(this.internalTableAddrTextBox);
            this.internalTableTabPage.Controls.Add(this.internalTableWordCheckBox);
            this.internalTableTabPage.Controls.Add(this.internalTableOutputCommentsCheckBox);
            this.internalTableTabPage.Controls.Add(this.label37);
            this.internalTableTabPage.Controls.Add(this.internalTableSLabelTextBox);
            this.internalTableTabPage.Controls.Add(this.internalTableCellsUnitsTextBox);
            this.internalTableTabPage.Controls.Add(this.label30);
            this.internalTableTabPage.Controls.Add(this.label31);
            this.internalTableTabPage.Controls.Add(this.internalTableColsTextBox);
            this.internalTableTabPage.Controls.Add(this.label32);
            this.internalTableTabPage.Controls.Add(this.internalTableScaleTextBox);
            this.internalTableTabPage.Controls.Add(this.label35);
            this.internalTableTabPage.Controls.Add(this.internalTableRowsTextBox);
            this.internalTableTabPage.Controls.Add(this.internalTableRowsUnitsTextBox);
            this.internalTableTabPage.Controls.Add(this.label36);
            this.internalTableTabPage.Controls.Add(this.internalTableColsUnitsTextBox);
            this.internalTableTabPage.Controls.Add(this.label39);
            this.internalTableTabPage.Controls.Add(this.internalTableCommentsTextBox);
            this.internalTableTabPage.Controls.Add(this.label40);
            this.internalTableTabPage.Controls.Add(this.label41);
            this.internalTableTabPage.Controls.Add(this.internalTableLabelTextBox);
            this.internalTableTabPage.Controls.Add(this.internalTableSignedCheckBox);
            this.internalTableTabPage.Location = new System.Drawing.Point(4, 22);
            this.internalTableTabPage.Name = "internalTableTabPage";
            this.internalTableTabPage.Size = new System.Drawing.Size(645, 263);
            this.internalTableTabPage.TabIndex = 10;
            this.internalTableTabPage.Text = "Internal Table";
            this.internalTableTabPage.UseVisualStyleBackColor = true;
            // 
            // internalTableScalePrecNumericUpDown
            // 
            this.internalTableScalePrecNumericUpDown.Location = new System.Drawing.Point(490, 113);
            this.internalTableScalePrecNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.internalTableScalePrecNumericUpDown.Name = "internalTableScalePrecNumericUpDown";
            this.internalTableScalePrecNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.internalTableScalePrecNumericUpDown.TabIndex = 10;
            this.internalTableScalePrecNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(247, 14);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(32, 13);
            this.label62.TabIndex = 80;
            this.label62.Text = "Bank";
            // 
            // internalTableBankTextBox
            // 
            this.internalTableBankTextBox.Location = new System.Drawing.Point(250, 30);
            this.internalTableBankTextBox.Name = "internalTableBankTextBox";
            this.internalTableBankTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalTableBankTextBox.TabIndex = 3;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(0, 14);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(45, 13);
            this.label42.TabIndex = 78;
            this.label42.Text = "Address";
            // 
            // internalTableAddrTextBox
            // 
            this.internalTableAddrTextBox.Location = new System.Drawing.Point(3, 30);
            this.internalTableAddrTextBox.Name = "internalTableAddrTextBox";
            this.internalTableAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalTableAddrTextBox.TabIndex = 2;
            // 
            // internalTableWordCheckBox
            // 
            this.internalTableWordCheckBox.AutoSize = true;
            this.internalTableWordCheckBox.Location = new System.Drawing.Point(250, 115);
            this.internalTableWordCheckBox.Name = "internalTableWordCheckBox";
            this.internalTableWordCheckBox.Size = new System.Drawing.Size(52, 17);
            this.internalTableWordCheckBox.TabIndex = 8;
            this.internalTableWordCheckBox.Text = "Word";
            this.internalTableWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalTableOutputCommentsCheckBox
            // 
            this.internalTableOutputCommentsCheckBox.AutoSize = true;
            this.internalTableOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalTableOutputCommentsCheckBox.Location = new System.Drawing.Point(411, 178);
            this.internalTableOutputCommentsCheckBox.Name = "internalTableOutputCommentsCheckBox";
            this.internalTableOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.internalTableOutputCommentsCheckBox.TabIndex = 15;
            this.internalTableOutputCommentsCheckBox.Text = "Output Comments";
            this.internalTableOutputCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(247, 53);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(61, 13);
            this.label37.TabIndex = 76;
            this.label37.Text = "Short Label";
            // 
            // internalTableSLabelTextBox
            // 
            this.internalTableSLabelTextBox.Location = new System.Drawing.Point(250, 68);
            this.internalTableSLabelTextBox.Name = "internalTableSLabelTextBox";
            this.internalTableSLabelTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalTableSLabelTextBox.TabIndex = 4;
            // 
            // internalTableCellsUnitsTextBox
            // 
            this.internalTableCellsUnitsTextBox.Location = new System.Drawing.Point(366, 156);
            this.internalTableCellsUnitsTextBox.Name = "internalTableCellsUnitsTextBox";
            this.internalTableCellsUnitsTextBox.Size = new System.Drawing.Size(155, 20);
            this.internalTableCellsUnitsTextBox.TabIndex = 13;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(363, 140);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(56, 13);
            this.label30.TabIndex = 73;
            this.label30.Text = "Cells Units";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(338, 53);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(87, 13);
            this.label31.TabIndex = 72;
            this.label31.Text = "Columns Number";
            // 
            // internalTableColsTextBox
            // 
            this.internalTableColsTextBox.Location = new System.Drawing.Point(356, 69);
            this.internalTableColsTextBox.Name = "internalTableColsTextBox";
            this.internalTableColsTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableColsTextBox.TabIndex = 5;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(449, 99);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(34, 13);
            this.label32.TabIndex = 71;
            this.label32.Text = "Scale";
            // 
            // internalTableScaleTextBox
            // 
            this.internalTableScaleTextBox.Location = new System.Drawing.Point(415, 113);
            this.internalTableScaleTextBox.Name = "internalTableScaleTextBox";
            this.internalTableScaleTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableScaleTextBox.TabIndex = 9;
            this.internalTableScaleTextBox.Text = "X";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(447, 53);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(74, 13);
            this.label35.TabIndex = 70;
            this.label35.Text = "Rows Number";
            // 
            // internalTableRowsTextBox
            // 
            this.internalTableRowsTextBox.Location = new System.Drawing.Point(452, 69);
            this.internalTableRowsTextBox.Name = "internalTableRowsTextBox";
            this.internalTableRowsTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableRowsTextBox.TabIndex = 6;
            this.internalTableRowsTextBox.Text = "0";
            // 
            // internalTableRowsUnitsTextBox
            // 
            this.internalTableRowsUnitsTextBox.Location = new System.Drawing.Point(196, 156);
            this.internalTableRowsUnitsTextBox.Name = "internalTableRowsUnitsTextBox";
            this.internalTableRowsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.internalTableRowsUnitsTextBox.TabIndex = 12;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(193, 140);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(61, 13);
            this.label36.TabIndex = 69;
            this.label36.Text = "Rows Units";
            // 
            // internalTableColsUnitsTextBox
            // 
            this.internalTableColsUnitsTextBox.Location = new System.Drawing.Point(3, 156);
            this.internalTableColsUnitsTextBox.Name = "internalTableColsUnitsTextBox";
            this.internalTableColsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.internalTableColsUnitsTextBox.TabIndex = 11;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(0, 140);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(74, 13);
            this.label39.TabIndex = 68;
            this.label39.Text = "Columns Units";
            // 
            // internalTableCommentsTextBox
            // 
            this.internalTableCommentsTextBox.Location = new System.Drawing.Point(3, 195);
            this.internalTableCommentsTextBox.Multiline = true;
            this.internalTableCommentsTextBox.Name = "internalTableCommentsTextBox";
            this.internalTableCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalTableCommentsTextBox.Size = new System.Drawing.Size(518, 42);
            this.internalTableCommentsTextBox.TabIndex = 14;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(0, 179);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(56, 13);
            this.label40.TabIndex = 67;
            this.label40.Text = "Comments";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(0, 53);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(33, 13);
            this.label41.TabIndex = 66;
            this.label41.Text = "Label";
            // 
            // internalTableLabelTextBox
            // 
            this.internalTableLabelTextBox.Location = new System.Drawing.Point(3, 69);
            this.internalTableLabelTextBox.Name = "internalTableLabelTextBox";
            this.internalTableLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalTableLabelTextBox.TabIndex = 3;
            // 
            // internalTableSignedCheckBox
            // 
            this.internalTableSignedCheckBox.AutoSize = true;
            this.internalTableSignedCheckBox.Location = new System.Drawing.Point(250, 96);
            this.internalTableSignedCheckBox.Name = "internalTableSignedCheckBox";
            this.internalTableSignedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.internalTableSignedCheckBox.TabIndex = 7;
            this.internalTableSignedCheckBox.Text = "Signed";
            this.internalTableSignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalFunctionTabPage
            // 
            this.internalFunctionTabPage.AutoScroll = true;
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionScalePrecOutputNumericUpDown);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionScalePrecInputNumericUpDown);
            this.internalFunctionTabPage.Controls.Add(this.label63);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionBankTextBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionOutputCommentsCheckBox);
            this.internalFunctionTabPage.Controls.Add(this.label44);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionSLabelTextBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionByteCheckBox);
            this.internalFunctionTabPage.Controls.Add(this.label45);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionScaleOutputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label46);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionScaleInputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label47);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionRowsTextBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionUnitsOutputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label48);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionSignedOutputCheckBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionUnitsInputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label49);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionCommentsTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label50);
            this.internalFunctionTabPage.Controls.Add(this.label51);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionLabelTextBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionSignedInputCheckBox);
            this.internalFunctionTabPage.Controls.Add(this.label43);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionAddrTextBox);
            this.internalFunctionTabPage.Location = new System.Drawing.Point(4, 22);
            this.internalFunctionTabPage.Name = "internalFunctionTabPage";
            this.internalFunctionTabPage.Size = new System.Drawing.Size(645, 263);
            this.internalFunctionTabPage.TabIndex = 11;
            this.internalFunctionTabPage.Text = "Internal Function";
            this.internalFunctionTabPage.UseVisualStyleBackColor = true;
            // 
            // internalFunctionScalePrecOutputNumericUpDown
            // 
            this.internalFunctionScalePrecOutputNumericUpDown.Location = new System.Drawing.Point(490, 116);
            this.internalFunctionScalePrecOutputNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.internalFunctionScalePrecOutputNumericUpDown.Name = "internalFunctionScalePrecOutputNumericUpDown";
            this.internalFunctionScalePrecOutputNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.internalFunctionScalePrecOutputNumericUpDown.TabIndex = 12;
            this.internalFunctionScalePrecOutputNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // internalFunctionScalePrecInputNumericUpDown
            // 
            this.internalFunctionScalePrecInputNumericUpDown.Location = new System.Drawing.Point(160, 116);
            this.internalFunctionScalePrecInputNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.internalFunctionScalePrecInputNumericUpDown.Name = "internalFunctionScalePrecInputNumericUpDown";
            this.internalFunctionScalePrecInputNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.internalFunctionScalePrecInputNumericUpDown.TabIndex = 8;
            this.internalFunctionScalePrecInputNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(336, 15);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(32, 13);
            this.label63.TabIndex = 103;
            this.label63.Text = "Bank";
            // 
            // internalFunctionBankTextBox
            // 
            this.internalFunctionBankTextBox.Location = new System.Drawing.Point(339, 31);
            this.internalFunctionBankTextBox.Name = "internalFunctionBankTextBox";
            this.internalFunctionBankTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalFunctionBankTextBox.TabIndex = 3;
            // 
            // internalFunctionOutputCommentsCheckBox
            // 
            this.internalFunctionOutputCommentsCheckBox.AutoSize = true;
            this.internalFunctionOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalFunctionOutputCommentsCheckBox.Location = new System.Drawing.Point(411, 183);
            this.internalFunctionOutputCommentsCheckBox.Name = "internalFunctionOutputCommentsCheckBox";
            this.internalFunctionOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.internalFunctionOutputCommentsCheckBox.TabIndex = 16;
            this.internalFunctionOutputCommentsCheckBox.Text = "Output Comments";
            this.internalFunctionOutputCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(336, 57);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(61, 13);
            this.label44.TabIndex = 101;
            this.label44.Text = "Short Label";
            // 
            // internalFunctionSLabelTextBox
            // 
            this.internalFunctionSLabelTextBox.Location = new System.Drawing.Point(339, 72);
            this.internalFunctionSLabelTextBox.Name = "internalFunctionSLabelTextBox";
            this.internalFunctionSLabelTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalFunctionSLabelTextBox.TabIndex = 4;
            // 
            // internalFunctionByteCheckBox
            // 
            this.internalFunctionByteCheckBox.AutoSize = true;
            this.internalFunctionByteCheckBox.Location = new System.Drawing.Point(240, 100);
            this.internalFunctionByteCheckBox.Name = "internalFunctionByteCheckBox";
            this.internalFunctionByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.internalFunctionByteCheckBox.TabIndex = 9;
            this.internalFunctionByteCheckBox.Text = "Byte";
            this.internalFunctionByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(452, 99);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(69, 13);
            this.label45.TabIndex = 100;
            this.label45.Text = "Output Scale";
            // 
            // internalFunctionScaleOutputTextBox
            // 
            this.internalFunctionScaleOutputTextBox.Location = new System.Drawing.Point(415, 116);
            this.internalFunctionScaleOutputTextBox.Name = "internalFunctionScaleOutputTextBox";
            this.internalFunctionScaleOutputTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalFunctionScaleOutputTextBox.TabIndex = 11;
            this.internalFunctionScaleOutputTextBox.Text = "X";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(130, 100);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(61, 13);
            this.label46.TabIndex = 99;
            this.label46.Text = "Input Scale";
            // 
            // internalFunctionScaleInputTextBox
            // 
            this.internalFunctionScaleInputTextBox.Location = new System.Drawing.Point(85, 116);
            this.internalFunctionScaleInputTextBox.Name = "internalFunctionScaleInputTextBox";
            this.internalFunctionScaleInputTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalFunctionScaleInputTextBox.TabIndex = 7;
            this.internalFunctionScaleInputTextBox.Text = "X";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(447, 57);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(74, 13);
            this.label47.TabIndex = 98;
            this.label47.Text = "Rows Number";
            // 
            // internalFunctionRowsTextBox
            // 
            this.internalFunctionRowsTextBox.Location = new System.Drawing.Point(452, 73);
            this.internalFunctionRowsTextBox.Name = "internalFunctionRowsTextBox";
            this.internalFunctionRowsTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalFunctionRowsTextBox.TabIndex = 5;
            this.internalFunctionRowsTextBox.Text = "0";
            // 
            // internalFunctionUnitsOutputTextBox
            // 
            this.internalFunctionUnitsOutputTextBox.Location = new System.Drawing.Point(339, 155);
            this.internalFunctionUnitsOutputTextBox.Name = "internalFunctionUnitsOutputTextBox";
            this.internalFunctionUnitsOutputTextBox.Size = new System.Drawing.Size(182, 20);
            this.internalFunctionUnitsOutputTextBox.TabIndex = 14;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(336, 139);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(66, 13);
            this.label48.TabIndex = 97;
            this.label48.Text = "Output Units";
            // 
            // internalFunctionSignedOutputCheckBox
            // 
            this.internalFunctionSignedOutputCheckBox.AutoSize = true;
            this.internalFunctionSignedOutputCheckBox.Location = new System.Drawing.Point(339, 99);
            this.internalFunctionSignedOutputCheckBox.Name = "internalFunctionSignedOutputCheckBox";
            this.internalFunctionSignedOutputCheckBox.Size = new System.Drawing.Size(94, 17);
            this.internalFunctionSignedOutputCheckBox.TabIndex = 10;
            this.internalFunctionSignedOutputCheckBox.Text = "Signed Output";
            this.internalFunctionSignedOutputCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalFunctionUnitsInputTextBox
            // 
            this.internalFunctionUnitsInputTextBox.Location = new System.Drawing.Point(3, 155);
            this.internalFunctionUnitsInputTextBox.Name = "internalFunctionUnitsInputTextBox";
            this.internalFunctionUnitsInputTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalFunctionUnitsInputTextBox.TabIndex = 13;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(0, 139);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(58, 13);
            this.label49.TabIndex = 94;
            this.label49.Text = "Input Units";
            // 
            // internalFunctionCommentsTextBox
            // 
            this.internalFunctionCommentsTextBox.Location = new System.Drawing.Point(3, 200);
            this.internalFunctionCommentsTextBox.Multiline = true;
            this.internalFunctionCommentsTextBox.Name = "internalFunctionCommentsTextBox";
            this.internalFunctionCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalFunctionCommentsTextBox.Size = new System.Drawing.Size(518, 42);
            this.internalFunctionCommentsTextBox.TabIndex = 15;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(0, 184);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(56, 13);
            this.label50.TabIndex = 91;
            this.label50.Text = "Comments";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(0, 57);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(33, 13);
            this.label51.TabIndex = 89;
            this.label51.Text = "Label";
            // 
            // internalFunctionLabelTextBox
            // 
            this.internalFunctionLabelTextBox.Location = new System.Drawing.Point(3, 73);
            this.internalFunctionLabelTextBox.Name = "internalFunctionLabelTextBox";
            this.internalFunctionLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalFunctionLabelTextBox.TabIndex = 3;
            // 
            // internalFunctionSignedInputCheckBox
            // 
            this.internalFunctionSignedInputCheckBox.AutoSize = true;
            this.internalFunctionSignedInputCheckBox.Location = new System.Drawing.Point(3, 99);
            this.internalFunctionSignedInputCheckBox.Name = "internalFunctionSignedInputCheckBox";
            this.internalFunctionSignedInputCheckBox.Size = new System.Drawing.Size(86, 17);
            this.internalFunctionSignedInputCheckBox.TabIndex = 6;
            this.internalFunctionSignedInputCheckBox.Text = "Signed Input";
            this.internalFunctionSignedInputCheckBox.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(0, 15);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(45, 13);
            this.label43.TabIndex = 80;
            this.label43.Text = "Address";
            // 
            // internalFunctionAddrTextBox
            // 
            this.internalFunctionAddrTextBox.Location = new System.Drawing.Point(3, 31);
            this.internalFunctionAddrTextBox.Name = "internalFunctionAddrTextBox";
            this.internalFunctionAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalFunctionAddrTextBox.TabIndex = 2;
            // 
            // internalScalarTabPage
            // 
            this.internalScalarTabPage.AutoScroll = true;
            this.internalScalarTabPage.Controls.Add(this.internalScalarInlineCommentsCheckBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarScalePrecNumericUpDown);
            this.internalScalarTabPage.Controls.Add(this.internalScalarBitFlagsButton);
            this.internalScalarTabPage.Controls.Add(this.internalScalarBitFlagsCheckBox);
            this.internalScalarTabPage.Controls.Add(this.label64);
            this.internalScalarTabPage.Controls.Add(this.internalScalarBankTextBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarOutputCommentsCheckBox);
            this.internalScalarTabPage.Controls.Add(this.label53);
            this.internalScalarTabPage.Controls.Add(this.internalScalarSLabelTextBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarByteCheckBox);
            this.internalScalarTabPage.Controls.Add(this.label54);
            this.internalScalarTabPage.Controls.Add(this.internalScalarScaleTextBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarUnitsTextBox);
            this.internalScalarTabPage.Controls.Add(this.label55);
            this.internalScalarTabPage.Controls.Add(this.internalScalarCommentsTextBox);
            this.internalScalarTabPage.Controls.Add(this.label56);
            this.internalScalarTabPage.Controls.Add(this.label57);
            this.internalScalarTabPage.Controls.Add(this.internalScalarLabelTextBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarSignedCheckBox);
            this.internalScalarTabPage.Controls.Add(this.label52);
            this.internalScalarTabPage.Controls.Add(this.internalScalarAddrTextBox);
            this.internalScalarTabPage.Location = new System.Drawing.Point(4, 22);
            this.internalScalarTabPage.Name = "internalScalarTabPage";
            this.internalScalarTabPage.Size = new System.Drawing.Size(645, 263);
            this.internalScalarTabPage.TabIndex = 12;
            this.internalScalarTabPage.Text = "Internal Scalar";
            this.internalScalarTabPage.UseVisualStyleBackColor = true;
            // 
            // internalScalarInlineCommentsCheckBox
            // 
            this.internalScalarInlineCommentsCheckBox.AutoSize = true;
            this.internalScalarInlineCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalScalarInlineCommentsCheckBox.Location = new System.Drawing.Point(296, 144);
            this.internalScalarInlineCommentsCheckBox.Name = "internalScalarInlineCommentsCheckBox";
            this.internalScalarInlineCommentsCheckBox.Size = new System.Drawing.Size(103, 17);
            this.internalScalarInlineCommentsCheckBox.TabIndex = 108;
            this.internalScalarInlineCommentsCheckBox.Text = "Inline Comments";
            this.internalScalarInlineCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalScalarScalePrecNumericUpDown
            // 
            this.internalScalarScalePrecNumericUpDown.Location = new System.Drawing.Point(484, 115);
            this.internalScalarScalePrecNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.internalScalarScalePrecNumericUpDown.Name = "internalScalarScalePrecNumericUpDown";
            this.internalScalarScalePrecNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.internalScalarScalePrecNumericUpDown.TabIndex = 9;
            this.internalScalarScalePrecNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // internalScalarBitFlagsButton
            // 
            this.internalScalarBitFlagsButton.Location = new System.Drawing.Point(232, 29);
            this.internalScalarBitFlagsButton.Name = "internalScalarBitFlagsButton";
            this.internalScalarBitFlagsButton.Size = new System.Drawing.Size(66, 23);
            this.internalScalarBitFlagsButton.TabIndex = 106;
            this.internalScalarBitFlagsButton.Text = "Bit Flags";
            this.internalScalarBitFlagsButton.UseVisualStyleBackColor = true;
            this.internalScalarBitFlagsButton.Click += new System.EventHandler(this.internalScalarBitFlagsButton_Click);
            // 
            // internalScalarBitFlagsCheckBox
            // 
            this.internalScalarBitFlagsCheckBox.AutoSize = true;
            this.internalScalarBitFlagsCheckBox.Enabled = false;
            this.internalScalarBitFlagsCheckBox.Location = new System.Drawing.Point(232, 14);
            this.internalScalarBitFlagsCheckBox.Name = "internalScalarBitFlagsCheckBox";
            this.internalScalarBitFlagsCheckBox.Size = new System.Drawing.Size(66, 17);
            this.internalScalarBitFlagsCheckBox.TabIndex = 107;
            this.internalScalarBitFlagsCheckBox.Text = "Bit Flags";
            this.internalScalarBitFlagsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(324, 15);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(32, 13);
            this.label64.TabIndex = 105;
            this.label64.Text = "Bank";
            // 
            // internalScalarBankTextBox
            // 
            this.internalScalarBankTextBox.Location = new System.Drawing.Point(327, 31);
            this.internalScalarBankTextBox.Name = "internalScalarBankTextBox";
            this.internalScalarBankTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalScalarBankTextBox.TabIndex = 3;
            // 
            // internalScalarOutputCommentsCheckBox
            // 
            this.internalScalarOutputCommentsCheckBox.AutoSize = true;
            this.internalScalarOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalScalarOutputCommentsCheckBox.Location = new System.Drawing.Point(405, 144);
            this.internalScalarOutputCommentsCheckBox.Name = "internalScalarOutputCommentsCheckBox";
            this.internalScalarOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.internalScalarOutputCommentsCheckBox.TabIndex = 11;
            this.internalScalarOutputCommentsCheckBox.Text = "Output Comments";
            this.internalScalarOutputCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(104, 100);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(61, 13);
            this.label53.TabIndex = 96;
            this.label53.Text = "Short Label";
            // 
            // internalScalarSLabelTextBox
            // 
            this.internalScalarSLabelTextBox.Location = new System.Drawing.Point(107, 115);
            this.internalScalarSLabelTextBox.Name = "internalScalarSLabelTextBox";
            this.internalScalarSLabelTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalScalarSLabelTextBox.TabIndex = 6;
            // 
            // internalScalarByteCheckBox
            // 
            this.internalScalarByteCheckBox.AutoSize = true;
            this.internalScalarByteCheckBox.Location = new System.Drawing.Point(232, 115);
            this.internalScalarByteCheckBox.Name = "internalScalarByteCheckBox";
            this.internalScalarByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.internalScalarByteCheckBox.TabIndex = 7;
            this.internalScalarByteCheckBox.Text = "Byte";
            this.internalScalarByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(481, 100);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(34, 13);
            this.label54.TabIndex = 93;
            this.label54.Text = "Scale";
            // 
            // internalScalarScaleTextBox
            // 
            this.internalScalarScaleTextBox.Location = new System.Drawing.Point(407, 115);
            this.internalScalarScaleTextBox.Name = "internalScalarScaleTextBox";
            this.internalScalarScaleTextBox.Size = new System.Drawing.Size(71, 20);
            this.internalScalarScaleTextBox.TabIndex = 8;
            this.internalScalarScaleTextBox.Text = "X";
            // 
            // internalScalarUnitsTextBox
            // 
            this.internalScalarUnitsTextBox.Location = new System.Drawing.Point(327, 73);
            this.internalScalarUnitsTextBox.Name = "internalScalarUnitsTextBox";
            this.internalScalarUnitsTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalScalarUnitsTextBox.TabIndex = 4;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(324, 57);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(31, 13);
            this.label55.TabIndex = 90;
            this.label55.Text = "Units";
            // 
            // internalScalarCommentsTextBox
            // 
            this.internalScalarCommentsTextBox.Location = new System.Drawing.Point(3, 161);
            this.internalScalarCommentsTextBox.Multiline = true;
            this.internalScalarCommentsTextBox.Name = "internalScalarCommentsTextBox";
            this.internalScalarCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalScalarCommentsTextBox.Size = new System.Drawing.Size(512, 42);
            this.internalScalarCommentsTextBox.TabIndex = 10;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(0, 145);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(56, 13);
            this.label56.TabIndex = 86;
            this.label56.Text = "Comments";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(0, 57);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(33, 13);
            this.label57.TabIndex = 85;
            this.label57.Text = "Label";
            // 
            // internalScalarLabelTextBox
            // 
            this.internalScalarLabelTextBox.Location = new System.Drawing.Point(3, 73);
            this.internalScalarLabelTextBox.Name = "internalScalarLabelTextBox";
            this.internalScalarLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalScalarLabelTextBox.TabIndex = 3;
            // 
            // internalScalarSignedCheckBox
            // 
            this.internalScalarSignedCheckBox.AutoSize = true;
            this.internalScalarSignedCheckBox.Location = new System.Drawing.Point(3, 115);
            this.internalScalarSignedCheckBox.Name = "internalScalarSignedCheckBox";
            this.internalScalarSignedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.internalScalarSignedCheckBox.TabIndex = 5;
            this.internalScalarSignedCheckBox.Text = "Signed";
            this.internalScalarSignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(0, 15);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(101, 13);
            this.label52.TabIndex = 82;
            this.label52.Text = "Address Parameters";
            // 
            // internalScalarAddrTextBox
            // 
            this.internalScalarAddrTextBox.Location = new System.Drawing.Point(3, 31);
            this.internalScalarAddrTextBox.Name = "internalScalarAddrTextBox";
            this.internalScalarAddrTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalScalarAddrTextBox.TabIndex = 2;
            // 
            // advSigPanel
            // 
            this.advSigPanel.BackColor = System.Drawing.SystemColors.Window;
            this.advSigPanel.Controls.Add(this.advSigTextBox);
            this.advSigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.advSigPanel.Location = new System.Drawing.Point(0, 0);
            this.advSigPanel.Name = "advSigPanel";
            this.advSigPanel.Size = new System.Drawing.Size(984, 155);
            this.advSigPanel.TabIndex = 3;
            // 
            // advSigTextBox
            // 
            this.advSigTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advSigTextBox.Location = new System.Drawing.Point(0, 0);
            this.advSigTextBox.Multiline = true;
            this.advSigTextBox.Name = "advSigTextBox";
            this.advSigTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.advSigTextBox.Size = new System.Drawing.Size(984, 155);
            this.advSigTextBox.TabIndex = 0;
            // 
            // advFooterPanel
            // 
            this.advFooterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.advFooterPanel.Controls.Add(this.mainTipPictureBox);
            this.advFooterPanel.Controls.Add(this.elemUpdateButton);
            this.advFooterPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.advFooterPanel.Location = new System.Drawing.Point(0, 444);
            this.advFooterPanel.Name = "advFooterPanel";
            this.advFooterPanel.Size = new System.Drawing.Size(984, 27);
            this.advFooterPanel.TabIndex = 7;
            // 
            // mainTipPictureBox
            // 
            this.mainTipPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.mainTipPictureBox.Image = global::SAD806x.Properties.Resources.question;
            this.mainTipPictureBox.Location = new System.Drawing.Point(0, 0);
            this.mainTipPictureBox.Name = "mainTipPictureBox";
            this.mainTipPictureBox.Size = new System.Drawing.Size(29, 27);
            this.mainTipPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mainTipPictureBox.TabIndex = 2;
            this.mainTipPictureBox.TabStop = false;
            // 
            // elemUpdateButton
            // 
            this.elemUpdateButton.AutoSize = true;
            this.elemUpdateButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.elemUpdateButton.Location = new System.Drawing.Point(902, 0);
            this.elemUpdateButton.Name = "elemUpdateButton";
            this.elemUpdateButton.Size = new System.Drawing.Size(82, 27);
            this.elemUpdateButton.TabIndex = 20;
            this.elemUpdateButton.Text = "Add / Update";
            this.elemUpdateButton.UseVisualStyleBackColor = true;
            this.elemUpdateButton.Click += new System.EventHandler(this.elemUpdateButton_Click);
            // 
            // mainToolTip
            // 
            this.mainToolTip.AutoPopDelay = 60000;
            this.mainToolTip.InitialDelay = 500;
            this.mainToolTip.ReshowDelay = 100;
            // 
            // repoContextMenuStrip
            // 
            this.repoContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.repoToolStripTextBox,
            this.repoToolStripSeparator,
            this.repoToolStripMenuItem});
            this.repoContextMenuStrip.Name = "repoContextMenuStrip";
            this.repoContextMenuStrip.ShowCheckMargin = true;
            this.repoContextMenuStrip.ShowImageMargin = false;
            this.repoContextMenuStrip.Size = new System.Drawing.Size(173, 57);
            // 
            // repoToolStripTextBox
            // 
            this.repoToolStripTextBox.Name = "repoToolStripTextBox";
            this.repoToolStripTextBox.Size = new System.Drawing.Size(100, 23);
            this.repoToolStripTextBox.ToolTipText = "Searched text";
            // 
            // repoToolStripSeparator
            // 
            this.repoToolStripSeparator.Name = "repoToolStripSeparator";
            this.repoToolStripSeparator.Size = new System.Drawing.Size(169, 6);
            // 
            // repoToolStripMenuItem
            // 
            this.repoToolStripMenuItem.Name = "repoToolStripMenuItem";
            this.repoToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.repoToolStripMenuItem.Text = "Related Repository";
            this.repoToolStripMenuItem.ToolTipText = "Results";
            // 
            // SigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 500);
            this.Controls.Add(this.advMainPanel);
            this.Controls.Add(this.advHeaderPanel);
            this.Name = "SigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Signature";
            this.Load += new System.EventHandler(this.SigForm_Load);
            this.advHeaderPanel.ResumeLayout(false);
            this.advHeaderPanel.PerformLayout();
            this.advMainPanel.ResumeLayout(false);
            this.advMainSplitContainer.Panel1.ResumeLayout(false);
            this.advMainSplitContainer.Panel2.ResumeLayout(false);
            this.advMainSplitContainer.ResumeLayout(false);
            this.advElemsContextMenuStrip.ResumeLayout(false);
            this.elemTabControl.ResumeLayout(false);
            this.routineTabPage.ResumeLayout(false);
            this.routineTabPage.PerformLayout();
            this.inputArgumentTabPage.ResumeLayout(false);
            this.inputArgumentTabPage.PerformLayout();
            this.inputStructureTabPage.ResumeLayout(false);
            this.inputStructureTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputStructureTipPictureBox)).EndInit();
            this.inputTableTabPage.ResumeLayout(false);
            this.inputTableTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputTableScalePrecNumericUpDown)).EndInit();
            this.inputFunctionTabPage.ResumeLayout(false);
            this.inputFunctionTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputFunctionScalePrecOutputNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputFunctionScalePrecInputNumericUpDown)).EndInit();
            this.inputScalarTabPage.ResumeLayout(false);
            this.inputScalarTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputScalarScalePrecNumericUpDown)).EndInit();
            this.internalStructureTabPage.ResumeLayout(false);
            this.internalStructureTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalStructureTipPictureBox)).EndInit();
            this.internalTableTabPage.ResumeLayout(false);
            this.internalTableTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalTableScalePrecNumericUpDown)).EndInit();
            this.internalFunctionTabPage.ResumeLayout(false);
            this.internalFunctionTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecOutputNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecInputNumericUpDown)).EndInit();
            this.internalScalarTabPage.ResumeLayout(false);
            this.internalScalarTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalScalarScalePrecNumericUpDown)).EndInit();
            this.advSigPanel.ResumeLayout(false);
            this.advSigPanel.PerformLayout();
            this.advFooterPanel.ResumeLayout(false);
            this.advFooterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainTipPictureBox)).EndInit();
            this.repoContextMenuStrip.ResumeLayout(false);
            this.repoContextMenuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel advHeaderPanel;
        private System.Windows.Forms.TextBox advSLabelTextBox;
        private System.Windows.Forms.TextBox advLabelTextBox;
        private System.Windows.Forms.Panel advMainPanel;
        private System.Windows.Forms.TextBox advSigTextBox;
        private System.Windows.Forms.Panel advSigPanel;
        private System.Windows.Forms.TreeView advElemsTreeView;
        private System.Windows.Forms.SplitContainer advMainSplitContainer;
        private System.Windows.Forms.TabControl elemTabControl;
        private System.Windows.Forms.TabPage inputScalarTabPage;
        private System.Windows.Forms.CheckBox inputScalarByteCheckBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox inputScalarScaleTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputScalarAddrTextBox;
        private System.Windows.Forms.CheckBox inputScalarSignedCheckBox;
        private System.Windows.Forms.TabPage inputFunctionTabPage;
        private System.Windows.Forms.CheckBox inputFunctionByteCheckBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox inputFunctionScaleOutputTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox inputFunctionScaleInputTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox inputFunctionRowsTextBox;
        private System.Windows.Forms.TextBox inputFunctionUnitsOutputTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox inputFunctionSignedOutputCheckBox;
        private System.Windows.Forms.TextBox inputFunctionUnitsInputTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox inputFunctionAddrTextBox;
        private System.Windows.Forms.CheckBox inputFunctionSignedInputCheckBox;
        private System.Windows.Forms.TabPage inputTableTabPage;
        private System.Windows.Forms.CheckBox inputTableWordCheckBox;
        private System.Windows.Forms.TextBox inputTableCellsUnitsTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox inputTableColsNumRegTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox inputTableScaleTextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox inputTableRowsNumTextBox;
        private System.Windows.Forms.TextBox inputTableRowsUnitsTextBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox inputTableColsUnitsTextBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox inputTableAddrTextBox;
        private System.Windows.Forms.CheckBox inputTableSignedCheckBox;
        private System.Windows.Forms.TabPage inputStructureTabPage;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox inputStructureNumRegTextBox;
        private System.Windows.Forms.TextBox inputStructureStructTextBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox inputStructureAddrTextBox;
        private System.Windows.Forms.TextBox inputScalarUnitsTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox inputFunctionOutputTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox inputFunctionInputTextBox;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox inputTableColsNumFixTextBox;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox inputTableOutputTextBox;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox inputTableRowsRegTextBox;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox inputTableColsRegTextBox;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox inputStructureNumFixTextBox;
        private System.Windows.Forms.TabPage internalStructureTabPage;
        private System.Windows.Forms.TabPage internalTableTabPage;
        private System.Windows.Forms.TabPage internalFunctionTabPage;
        private System.Windows.Forms.TabPage internalScalarTabPage;
        private System.Windows.Forms.CheckBox internalStructureOutputCommentsCheckBox;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox internalStructureSLabelTextBox;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox internalStructureNumTextBox;
        private System.Windows.Forms.TextBox internalStructureStructTextBox;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox internalStructureCommentsTextBox;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox internalStructureLabelTextBox;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox internalStructureAddrTextBox;
        private System.Windows.Forms.CheckBox internalTableWordCheckBox;
        private System.Windows.Forms.CheckBox internalTableOutputCommentsCheckBox;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox internalTableSLabelTextBox;
        private System.Windows.Forms.TextBox internalTableCellsUnitsTextBox;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox internalTableColsTextBox;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox internalTableScaleTextBox;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox internalTableRowsTextBox;
        private System.Windows.Forms.TextBox internalTableRowsUnitsTextBox;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox internalTableColsUnitsTextBox;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.TextBox internalTableCommentsTextBox;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.TextBox internalTableLabelTextBox;
        private System.Windows.Forms.CheckBox internalTableSignedCheckBox;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox internalTableAddrTextBox;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox internalFunctionAddrTextBox;
        private System.Windows.Forms.CheckBox internalFunctionOutputCommentsCheckBox;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox internalFunctionSLabelTextBox;
        private System.Windows.Forms.CheckBox internalFunctionByteCheckBox;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.TextBox internalFunctionScaleOutputTextBox;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TextBox internalFunctionScaleInputTextBox;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.TextBox internalFunctionRowsTextBox;
        private System.Windows.Forms.TextBox internalFunctionUnitsOutputTextBox;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.CheckBox internalFunctionSignedOutputCheckBox;
        private System.Windows.Forms.TextBox internalFunctionUnitsInputTextBox;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox internalFunctionCommentsTextBox;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TextBox internalFunctionLabelTextBox;
        private System.Windows.Forms.CheckBox internalFunctionSignedInputCheckBox;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox internalScalarAddrTextBox;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.TextBox internalScalarSLabelTextBox;
        private System.Windows.Forms.CheckBox internalScalarByteCheckBox;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.TextBox internalScalarScaleTextBox;
        private System.Windows.Forms.TextBox internalScalarUnitsTextBox;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.TextBox internalScalarCommentsTextBox;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.TextBox internalScalarLabelTextBox;
        private System.Windows.Forms.CheckBox internalScalarSignedCheckBox;
        private System.Windows.Forms.ContextMenuStrip advElemsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem delElementToolStripMenuItem;
        private System.Windows.Forms.TabPage inputArgumentTabPage;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.TextBox inputArgCodeTextBox;
        private System.Windows.Forms.CheckBox inputArgWordCheckBox;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TextBox inputArgPositionTextBox;
        private System.Windows.Forms.CheckBox inputArgPointerCheckBox;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.ComboBox inputArgEncryptionComboBox;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.TextBox internalStructureBankTextBox;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.TextBox internalTableBankTextBox;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.TextBox internalFunctionBankTextBox;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox internalScalarBankTextBox;
        private System.Windows.Forms.PictureBox inputStructureTipPictureBox;
        private System.Windows.Forms.PictureBox internalStructureTipPictureBox;
        private System.Windows.Forms.ToolTip mainToolTip;
        private System.Windows.Forms.Button inputScalarBitFlagsButton;
        private System.Windows.Forms.CheckBox inputScalarBitFlagsCheckBox;
        private System.Windows.Forms.Button internalScalarBitFlagsButton;
        private System.Windows.Forms.CheckBox internalScalarBitFlagsCheckBox;
        private System.Windows.Forms.ContextMenuStrip repoContextMenuStrip;
        private System.Windows.Forms.ToolStripTextBox repoToolStripTextBox;
        private System.Windows.Forms.ToolStripSeparator repoToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem repoToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown inputTableScalePrecNumericUpDown;
        private System.Windows.Forms.NumericUpDown inputFunctionScalePrecInputNumericUpDown;
        private System.Windows.Forms.NumericUpDown inputFunctionScalePrecOutputNumericUpDown;
        private System.Windows.Forms.NumericUpDown inputScalarScalePrecNumericUpDown;
        private System.Windows.Forms.NumericUpDown internalTableScalePrecNumericUpDown;
        private System.Windows.Forms.NumericUpDown internalFunctionScalePrecInputNumericUpDown;
        private System.Windows.Forms.NumericUpDown internalFunctionScalePrecOutputNumericUpDown;
        private System.Windows.Forms.NumericUpDown internalScalarScalePrecNumericUpDown;
        private System.Windows.Forms.CheckBox internalScalarInlineCommentsCheckBox;
        private System.Windows.Forms.CheckBox internalScalarOutputCommentsCheckBox;
        private System.Windows.Forms.Panel advFooterPanel;
        private System.Windows.Forms.PictureBox mainTipPictureBox;
        private System.Windows.Forms.Button elemUpdateButton;
        private System.Windows.Forms.TabPage routineTabPage;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.CheckBox routineOutputCommentsCheckBox;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.TextBox routineSLabelTextBox;
        private System.Windows.Forms.TextBox routineCommentsTextBox;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.TextBox routineLabelTextBox;
        private System.Windows.Forms.ComboBox signatureFor806xComboBox;
        private System.Windows.Forms.ComboBox signatureForBankComboBox;
    }
}