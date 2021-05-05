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
            this.routineIdentificationStatusTrackBar = new System.Windows.Forms.TrackBar();
            this.routineIdentificationDetailsTextBox = new System.Windows.Forms.TextBox();
            this.routineIdentificationLabel = new System.Windows.Forms.Label();
            this.routineCateg3ComboBox = new System.Windows.Forms.ComboBox();
            this.routineCateg2ComboBox = new System.Windows.Forms.ComboBox();
            this.label71 = new System.Windows.Forms.Label();
            this.routineCategComboBox = new System.Windows.Forms.ComboBox();
            this.label72 = new System.Windows.Forms.Label();
            this.routineDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label73 = new System.Windows.Forms.Label();
            this.routineDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label70 = new System.Windows.Forms.Label();
            this.inputArgDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label83 = new System.Windows.Forms.Label();
            this.inputArgDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.inputArgEncryptionComboBox = new System.Windows.Forms.ComboBox();
            this.label60 = new System.Windows.Forms.Label();
            this.inputArgPointerCheckBox = new System.Windows.Forms.CheckBox();
            this.inputArgWordCheckBox = new System.Windows.Forms.CheckBox();
            this.label59 = new System.Windows.Forms.Label();
            this.inputArgPositionTextBox = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.inputArgCodeTextBox = new System.Windows.Forms.TextBox();
            this.inputStructureTabPage = new System.Windows.Forms.TabPage();
            this.label84 = new System.Windows.Forms.Label();
            this.inputStructureDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label85 = new System.Windows.Forms.Label();
            this.inputStructureDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label86 = new System.Windows.Forms.Label();
            this.inputTableDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label87 = new System.Windows.Forms.Label();
            this.inputTableDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label88 = new System.Windows.Forms.Label();
            this.inputFunctionDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label89 = new System.Windows.Forms.Label();
            this.inputFunctionDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label90 = new System.Windows.Forms.Label();
            this.inputScalarDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label91 = new System.Windows.Forms.Label();
            this.inputScalarDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.inputScalarScalePrecNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.inputScalarUnitsTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.inputScalarByteCheckBox = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.inputScalarScaleTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.inputScalarAddrTextBox = new System.Windows.Forms.TextBox();
            this.inputScalarSignedCheckBox = new System.Windows.Forms.CheckBox();
            this.internalStructureTabPage = new System.Windows.Forms.TabPage();
            this.internalStructureIdentificationStatusTrackBar = new System.Windows.Forms.TrackBar();
            this.internalStructureIdentificationDetailsTextBox = new System.Windows.Forms.TextBox();
            this.internalStructureIdentificationLabel = new System.Windows.Forms.Label();
            this.internalStructureCateg3ComboBox = new System.Windows.Forms.ComboBox();
            this.internalStructureCateg2ComboBox = new System.Windows.Forms.ComboBox();
            this.label80 = new System.Windows.Forms.Label();
            this.internalStructureCategComboBox = new System.Windows.Forms.ComboBox();
            this.label81 = new System.Windows.Forms.Label();
            this.internalStructureDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label82 = new System.Windows.Forms.Label();
            this.internalStructureDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label92 = new System.Windows.Forms.Label();
            this.internalTableCellsMaxTextBox = new System.Windows.Forms.TextBox();
            this.label93 = new System.Windows.Forms.Label();
            this.internalTableCellsMinTextBox = new System.Windows.Forms.TextBox();
            this.internalTableIdentificationStatusTrackBar = new System.Windows.Forms.TrackBar();
            this.internalTableIdentificationDetailsTextBox = new System.Windows.Forms.TextBox();
            this.internalTableIdentificationLabel = new System.Windows.Forms.Label();
            this.internalTableCateg3ComboBox = new System.Windows.Forms.ComboBox();
            this.internalTableCateg2ComboBox = new System.Windows.Forms.ComboBox();
            this.sharedCategsLabel = new System.Windows.Forms.Label();
            this.internalTableCategComboBox = new System.Windows.Forms.ComboBox();
            this.label68 = new System.Windows.Forms.Label();
            this.internalTableDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label69 = new System.Windows.Forms.Label();
            this.internalTableDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label98 = new System.Windows.Forms.Label();
            this.internalFunctionMaxOutputTextBox = new System.Windows.Forms.TextBox();
            this.label99 = new System.Windows.Forms.Label();
            this.internalFunctionMinOutputTextBox = new System.Windows.Forms.TextBox();
            this.label96 = new System.Windows.Forms.Label();
            this.internalFunctionMaxInputTextBox = new System.Windows.Forms.TextBox();
            this.label97 = new System.Windows.Forms.Label();
            this.internalFunctionMinInputTextBox = new System.Windows.Forms.TextBox();
            this.internalFunctionIdentificationStatusTrackBar = new System.Windows.Forms.TrackBar();
            this.internalFunctionIdentificationDetailsTextBox = new System.Windows.Forms.TextBox();
            this.internalFunctionIdentificationLabel = new System.Windows.Forms.Label();
            this.internalFunctionCateg3ComboBox = new System.Windows.Forms.ComboBox();
            this.internalFunctionCateg2ComboBox = new System.Windows.Forms.ComboBox();
            this.label74 = new System.Windows.Forms.Label();
            this.internalFunctionCategComboBox = new System.Windows.Forms.ComboBox();
            this.label75 = new System.Windows.Forms.Label();
            this.internalFunctionDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label76 = new System.Windows.Forms.Label();
            this.internalFunctionDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            this.label94 = new System.Windows.Forms.Label();
            this.internalScalarMaxTextBox = new System.Windows.Forms.TextBox();
            this.label95 = new System.Windows.Forms.Label();
            this.internalScalarMinTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarIdentificationStatusTrackBar = new System.Windows.Forms.TrackBar();
            this.internalScalarIdentificationDetailsTextBox = new System.Windows.Forms.TextBox();
            this.internalScalarIdentificationLabel = new System.Windows.Forms.Label();
            this.internalScalarCateg3ComboBox = new System.Windows.Forms.ComboBox();
            this.internalScalarCateg2ComboBox = new System.Windows.Forms.ComboBox();
            this.label77 = new System.Windows.Forms.Label();
            this.internalScalarCategComboBox = new System.Windows.Forms.ComboBox();
            this.label78 = new System.Windows.Forms.Label();
            this.internalScalarDateUpdatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label79 = new System.Windows.Forms.Label();
            this.internalScalarDateCreatedDateTimePicker = new System.Windows.Forms.DateTimePicker();
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
            ((System.ComponentModel.ISupportInitialize)(this.routineIdentificationStatusTrackBar)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.internalStructureIdentificationStatusTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalStructureTipPictureBox)).BeginInit();
            this.internalTableTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalTableIdentificationStatusTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalTableScalePrecNumericUpDown)).BeginInit();
            this.internalFunctionTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionIdentificationStatusTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecOutputNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecInputNumericUpDown)).BeginInit();
            this.internalScalarTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalScalarIdentificationStatusTrackBar)).BeginInit();
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
            this.advMainPanel.Size = new System.Drawing.Size(984, 618);
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
            this.advMainSplitContainer.Size = new System.Drawing.Size(984, 436);
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
            this.advElemsTreeView.Size = new System.Drawing.Size(327, 436);
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
            this.elemTabControl.Size = new System.Drawing.Size(653, 436);
            this.elemTabControl.TabIndex = 2;
            // 
            // routineTabPage
            // 
            this.routineTabPage.AutoScroll = true;
            this.routineTabPage.Controls.Add(this.routineIdentificationStatusTrackBar);
            this.routineTabPage.Controls.Add(this.routineIdentificationDetailsTextBox);
            this.routineTabPage.Controls.Add(this.routineIdentificationLabel);
            this.routineTabPage.Controls.Add(this.routineCateg3ComboBox);
            this.routineTabPage.Controls.Add(this.routineCateg2ComboBox);
            this.routineTabPage.Controls.Add(this.label71);
            this.routineTabPage.Controls.Add(this.routineCategComboBox);
            this.routineTabPage.Controls.Add(this.label72);
            this.routineTabPage.Controls.Add(this.routineDateUpdatedDateTimePicker);
            this.routineTabPage.Controls.Add(this.label73);
            this.routineTabPage.Controls.Add(this.routineDateCreatedDateTimePicker);
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
            this.routineTabPage.Size = new System.Drawing.Size(645, 410);
            this.routineTabPage.TabIndex = 14;
            this.routineTabPage.Text = "Routine Creation";
            this.routineTabPage.UseVisualStyleBackColor = true;
            // 
            // routineIdentificationStatusTrackBar
            // 
            this.routineIdentificationStatusTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.routineIdentificationStatusTrackBar.LargeChange = 10;
            this.routineIdentificationStatusTrackBar.Location = new System.Drawing.Point(3, 363);
            this.routineIdentificationStatusTrackBar.Maximum = 100;
            this.routineIdentificationStatusTrackBar.Name = "routineIdentificationStatusTrackBar";
            this.routineIdentificationStatusTrackBar.Size = new System.Drawing.Size(248, 45);
            this.routineIdentificationStatusTrackBar.TabIndex = 82;
            this.routineIdentificationStatusTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // routineIdentificationDetailsTextBox
            // 
            this.routineIdentificationDetailsTextBox.Location = new System.Drawing.Point(257, 363);
            this.routineIdentificationDetailsTextBox.Multiline = true;
            this.routineIdentificationDetailsTextBox.Name = "routineIdentificationDetailsTextBox";
            this.routineIdentificationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.routineIdentificationDetailsTextBox.Size = new System.Drawing.Size(380, 45);
            this.routineIdentificationDetailsTextBox.TabIndex = 83;
            // 
            // routineIdentificationLabel
            // 
            this.routineIdentificationLabel.AutoSize = true;
            this.routineIdentificationLabel.Location = new System.Drawing.Point(0, 347);
            this.routineIdentificationLabel.Name = "routineIdentificationLabel";
            this.routineIdentificationLabel.Size = new System.Drawing.Size(67, 13);
            this.routineIdentificationLabel.TabIndex = 122;
            this.routineIdentificationLabel.Tag = "Identification";
            this.routineIdentificationLabel.Text = "Identification";
            // 
            // routineCateg3ComboBox
            // 
            this.routineCateg3ComboBox.FormattingEnabled = true;
            this.routineCateg3ComboBox.Location = new System.Drawing.Point(517, 308);
            this.routineCateg3ComboBox.Name = "routineCateg3ComboBox";
            this.routineCateg3ComboBox.Size = new System.Drawing.Size(120, 21);
            this.routineCateg3ComboBox.Sorted = true;
            this.routineCateg3ComboBox.TabIndex = 81;
            // 
            // routineCateg2ComboBox
            // 
            this.routineCateg2ComboBox.FormattingEnabled = true;
            this.routineCateg2ComboBox.Location = new System.Drawing.Point(257, 308);
            this.routineCateg2ComboBox.Name = "routineCateg2ComboBox";
            this.routineCateg2ComboBox.Size = new System.Drawing.Size(120, 21);
            this.routineCateg2ComboBox.Sorted = true;
            this.routineCateg2ComboBox.TabIndex = 80;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(254, 292);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(57, 13);
            this.label71.TabIndex = 121;
            this.label71.Text = "Categories";
            // 
            // routineCategComboBox
            // 
            this.routineCategComboBox.FormattingEnabled = true;
            this.routineCategComboBox.Location = new System.Drawing.Point(3, 308);
            this.routineCategComboBox.Name = "routineCategComboBox";
            this.routineCategComboBox.Size = new System.Drawing.Size(120, 21);
            this.routineCategComboBox.Sorted = true;
            this.routineCategComboBox.TabIndex = 79;
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(565, 245);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(72, 13);
            this.label72.TabIndex = 120;
            this.label72.Text = "Date updated";
            // 
            // routineDateUpdatedDateTimePicker
            // 
            this.routineDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.routineDateUpdatedDateTimePicker.Enabled = false;
            this.routineDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.routineDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.routineDateUpdatedDateTimePicker.Name = "routineDateUpdatedDateTimePicker";
            this.routineDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.routineDateUpdatedDateTimePicker.TabIndex = 78;
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(0, 245);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(69, 13);
            this.label73.TabIndex = 119;
            this.label73.Text = "Date created";
            // 
            // routineDateCreatedDateTimePicker
            // 
            this.routineDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.routineDateCreatedDateTimePicker.Enabled = false;
            this.routineDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.routineDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.routineDateCreatedDateTimePicker.Name = "routineDateCreatedDateTimePicker";
            this.routineDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.routineDateCreatedDateTimePicker.TabIndex = 77;
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
            this.signatureForBankComboBox.Location = new System.Drawing.Point(449, 65);
            this.signatureForBankComboBox.Name = "signatureForBankComboBox";
            this.signatureForBankComboBox.Size = new System.Drawing.Size(188, 21);
            this.signatureForBankComboBox.TabIndex = 81;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(0, 184);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(56, 13);
            this.label67.TabIndex = 79;
            this.label67.Text = "Comments";
            // 
            // routineOutputCommentsCheckBox
            // 
            this.routineOutputCommentsCheckBox.AutoSize = true;
            this.routineOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.routineOutputCommentsCheckBox.Location = new System.Drawing.Point(527, 183);
            this.routineOutputCommentsCheckBox.Name = "routineOutputCommentsCheckBox";
            this.routineOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.routineOutputCommentsCheckBox.TabIndex = 76;
            this.routineOutputCommentsCheckBox.Text = "Output Comments";
            this.routineOutputCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(576, 14);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(61, 13);
            this.label65.TabIndex = 78;
            this.label65.Text = "Short Label";
            // 
            // routineSLabelTextBox
            // 
            this.routineSLabelTextBox.Location = new System.Drawing.Point(449, 30);
            this.routineSLabelTextBox.Name = "routineSLabelTextBox";
            this.routineSLabelTextBox.Size = new System.Drawing.Size(188, 20);
            this.routineSLabelTextBox.TabIndex = 74;
            // 
            // routineCommentsTextBox
            // 
            this.routineCommentsTextBox.Location = new System.Drawing.Point(3, 200);
            this.routineCommentsTextBox.Multiline = true;
            this.routineCommentsTextBox.Name = "routineCommentsTextBox";
            this.routineCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.routineCommentsTextBox.Size = new System.Drawing.Size(634, 42);
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
            this.inputArgumentTabPage.Controls.Add(this.label70);
            this.inputArgumentTabPage.Controls.Add(this.inputArgDateUpdatedDateTimePicker);
            this.inputArgumentTabPage.Controls.Add(this.label83);
            this.inputArgumentTabPage.Controls.Add(this.inputArgDateCreatedDateTimePicker);
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
            this.inputArgumentTabPage.Size = new System.Drawing.Size(645, 410);
            this.inputArgumentTabPage.TabIndex = 13;
            this.inputArgumentTabPage.Text = "Input Argument";
            this.inputArgumentTabPage.UseVisualStyleBackColor = true;
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(565, 245);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(72, 13);
            this.label70.TabIndex = 113;
            this.label70.Text = "Date updated";
            // 
            // inputArgDateUpdatedDateTimePicker
            // 
            this.inputArgDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputArgDateUpdatedDateTimePicker.Enabled = false;
            this.inputArgDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputArgDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.inputArgDateUpdatedDateTimePicker.Name = "inputArgDateUpdatedDateTimePicker";
            this.inputArgDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputArgDateUpdatedDateTimePicker.TabIndex = 8;
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Location = new System.Drawing.Point(0, 245);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(69, 13);
            this.label83.TabIndex = 112;
            this.label83.Text = "Date created";
            // 
            // inputArgDateCreatedDateTimePicker
            // 
            this.inputArgDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputArgDateCreatedDateTimePicker.Enabled = false;
            this.inputArgDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputArgDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.inputArgDateCreatedDateTimePicker.Name = "inputArgDateCreatedDateTimePicker";
            this.inputArgDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputArgDateCreatedDateTimePicker.TabIndex = 7;
            // 
            // inputArgEncryptionComboBox
            // 
            this.inputArgEncryptionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputArgEncryptionComboBox.FormattingEnabled = true;
            this.inputArgEncryptionComboBox.Location = new System.Drawing.Point(484, 31);
            this.inputArgEncryptionComboBox.Name = "inputArgEncryptionComboBox";
            this.inputArgEncryptionComboBox.Size = new System.Drawing.Size(153, 21);
            this.inputArgEncryptionComboBox.TabIndex = 4;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(580, 15);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(57, 13);
            this.label60.TabIndex = 61;
            this.label60.Text = "Encryption";
            // 
            // inputArgPointerCheckBox
            // 
            this.inputArgPointerCheckBox.AutoSize = true;
            this.inputArgPointerCheckBox.Location = new System.Drawing.Point(484, 72);
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
            this.label59.Location = new System.Drawing.Point(246, 15);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(44, 13);
            this.label59.TabIndex = 50;
            this.label59.Text = "Position";
            // 
            // inputArgPositionTextBox
            // 
            this.inputArgPositionTextBox.Location = new System.Drawing.Point(249, 31);
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
            this.inputStructureTabPage.Controls.Add(this.label84);
            this.inputStructureTabPage.Controls.Add(this.inputStructureDateUpdatedDateTimePicker);
            this.inputStructureTabPage.Controls.Add(this.label85);
            this.inputStructureTabPage.Controls.Add(this.inputStructureDateCreatedDateTimePicker);
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
            this.inputStructureTabPage.Size = new System.Drawing.Size(645, 410);
            this.inputStructureTabPage.TabIndex = 8;
            this.inputStructureTabPage.Text = "Input Structure";
            this.inputStructureTabPage.UseVisualStyleBackColor = true;
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(565, 245);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(72, 13);
            this.label84.TabIndex = 117;
            this.label84.Text = "Date updated";
            // 
            // inputStructureDateUpdatedDateTimePicker
            // 
            this.inputStructureDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputStructureDateUpdatedDateTimePicker.Enabled = false;
            this.inputStructureDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputStructureDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.inputStructureDateUpdatedDateTimePicker.Name = "inputStructureDateUpdatedDateTimePicker";
            this.inputStructureDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputStructureDateUpdatedDateTimePicker.TabIndex = 7;
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(0, 245);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(69, 13);
            this.label85.TabIndex = 116;
            this.label85.Text = "Date created";
            // 
            // inputStructureDateCreatedDateTimePicker
            // 
            this.inputStructureDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputStructureDateCreatedDateTimePicker.Enabled = false;
            this.inputStructureDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputStructureDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.inputStructureDateCreatedDateTimePicker.Name = "inputStructureDateCreatedDateTimePicker";
            this.inputStructureDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputStructureDateCreatedDateTimePicker.TabIndex = 6;
            // 
            // inputStructureTipPictureBox
            // 
            this.inputStructureTipPictureBox.Image = global::SAD806x.Properties.Resources.question;
            this.inputStructureTipPictureBox.Location = new System.Drawing.Point(587, 14);
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
            this.inputStructureStructTextBox.Size = new System.Drawing.Size(634, 71);
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
            this.inputTableTabPage.Controls.Add(this.label86);
            this.inputTableTabPage.Controls.Add(this.inputTableDateUpdatedDateTimePicker);
            this.inputTableTabPage.Controls.Add(this.label87);
            this.inputTableTabPage.Controls.Add(this.inputTableDateCreatedDateTimePicker);
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
            this.inputTableTabPage.Size = new System.Drawing.Size(645, 410);
            this.inputTableTabPage.TabIndex = 3;
            this.inputTableTabPage.Text = "Input Table";
            this.inputTableTabPage.UseVisualStyleBackColor = true;
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(565, 245);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(72, 13);
            this.label86.TabIndex = 121;
            this.label86.Text = "Date updated";
            // 
            // inputTableDateUpdatedDateTimePicker
            // 
            this.inputTableDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputTableDateUpdatedDateTimePicker.Enabled = false;
            this.inputTableDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputTableDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.inputTableDateUpdatedDateTimePicker.Name = "inputTableDateUpdatedDateTimePicker";
            this.inputTableDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputTableDateUpdatedDateTimePicker.TabIndex = 17;
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(0, 245);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(69, 13);
            this.label87.TabIndex = 120;
            this.label87.Text = "Date created";
            // 
            // inputTableDateCreatedDateTimePicker
            // 
            this.inputTableDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputTableDateCreatedDateTimePicker.Enabled = false;
            this.inputTableDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputTableDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.inputTableDateCreatedDateTimePicker.Name = "inputTableDateCreatedDateTimePicker";
            this.inputTableDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputTableDateCreatedDateTimePicker.TabIndex = 16;
            // 
            // inputTableScalePrecNumericUpDown
            // 
            this.inputTableScalePrecNumericUpDown.Location = new System.Drawing.Point(606, 156);
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
            this.label25.Location = new System.Drawing.Point(556, 65);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(81, 13);
            this.label25.TabIndex = 53;
            this.label25.Text = "Output Register";
            // 
            // inputTableOutputTextBox
            // 
            this.inputTableOutputTextBox.Location = new System.Drawing.Point(517, 81);
            this.inputTableOutputTextBox.Name = "inputTableOutputTextBox";
            this.inputTableOutputTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputTableOutputTextBox.TabIndex = 6;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(250, 65);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(153, 13);
            this.label23.TabIndex = 51;
            this.label23.Text = "Rows Input Register/Argument";
            // 
            // inputTableRowsRegTextBox
            // 
            this.inputTableRowsRegTextBox.Location = new System.Drawing.Point(253, 81);
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
            this.label22.Location = new System.Drawing.Point(0, 140);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(123, 13);
            this.label22.TabIndex = 47;
            this.label22.Text = "Forced Columns Number";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(458, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(179, 13);
            this.label17.TabIndex = 46;
            this.label17.Text = "Columns Number Register/Argument";
            // 
            // inputTableColsNumFixTextBox
            // 
            this.inputTableColsNumFixTextBox.Location = new System.Drawing.Point(3, 156);
            this.inputTableColsNumFixTextBox.Name = "inputTableColsNumFixTextBox";
            this.inputTableColsNumFixTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableColsNumFixTextBox.TabIndex = 9;
            // 
            // inputTableWordCheckBox
            // 
            this.inputTableWordCheckBox.AutoSize = true;
            this.inputTableWordCheckBox.Location = new System.Drawing.Point(517, 117);
            this.inputTableWordCheckBox.Name = "inputTableWordCheckBox";
            this.inputTableWordCheckBox.Size = new System.Drawing.Size(52, 17);
            this.inputTableWordCheckBox.TabIndex = 7;
            this.inputTableWordCheckBox.Text = "Word";
            this.inputTableWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputTableCellsUnitsTextBox
            // 
            this.inputTableCellsUnitsTextBox.Location = new System.Drawing.Point(482, 212);
            this.inputTableCellsUnitsTextBox.Name = "inputTableCellsUnitsTextBox";
            this.inputTableCellsUnitsTextBox.Size = new System.Drawing.Size(155, 20);
            this.inputTableCellsUnitsTextBox.TabIndex = 15;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(545, 196);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 44;
            this.label12.Text = "Forced Cells Units";
            // 
            // inputTableColsNumRegTextBox
            // 
            this.inputTableColsNumRegTextBox.Location = new System.Drawing.Point(568, 32);
            this.inputTableColsNumRegTextBox.Name = "inputTableColsNumRegTextBox";
            this.inputTableColsNumRegTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableColsNumRegTextBox.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(567, 140);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 37;
            this.label13.Text = "Forced Scale";
            // 
            // inputTableScaleTextBox
            // 
            this.inputTableScaleTextBox.Location = new System.Drawing.Point(531, 156);
            this.inputTableScaleTextBox.Name = "inputTableScaleTextBox";
            this.inputTableScaleTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableScaleTextBox.TabIndex = 11;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(250, 140);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 13);
            this.label14.TabIndex = 35;
            this.label14.Text = "Forced Rows Number";
            // 
            // inputTableRowsNumTextBox
            // 
            this.inputTableRowsNumTextBox.Location = new System.Drawing.Point(253, 156);
            this.inputTableRowsNumTextBox.Name = "inputTableRowsNumTextBox";
            this.inputTableRowsNumTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputTableRowsNumTextBox.TabIndex = 10;
            // 
            // inputTableRowsUnitsTextBox
            // 
            this.inputTableRowsUnitsTextBox.Location = new System.Drawing.Point(253, 212);
            this.inputTableRowsUnitsTextBox.Name = "inputTableRowsUnitsTextBox";
            this.inputTableRowsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.inputTableRowsUnitsTextBox.TabIndex = 14;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(250, 196);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(97, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "Forced Rows Units";
            // 
            // inputTableColsUnitsTextBox
            // 
            this.inputTableColsUnitsTextBox.Location = new System.Drawing.Point(3, 212);
            this.inputTableColsUnitsTextBox.Name = "inputTableColsUnitsTextBox";
            this.inputTableColsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.inputTableColsUnitsTextBox.TabIndex = 13;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(0, 196);
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
            this.inputTableSignedCheckBox.Location = new System.Drawing.Point(578, 117);
            this.inputTableSignedCheckBox.Name = "inputTableSignedCheckBox";
            this.inputTableSignedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.inputTableSignedCheckBox.TabIndex = 8;
            this.inputTableSignedCheckBox.Text = "Signed";
            this.inputTableSignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputFunctionTabPage
            // 
            this.inputFunctionTabPage.AutoScroll = true;
            this.inputFunctionTabPage.Controls.Add(this.label88);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionDateUpdatedDateTimePicker);
            this.inputFunctionTabPage.Controls.Add(this.label89);
            this.inputFunctionTabPage.Controls.Add(this.inputFunctionDateCreatedDateTimePicker);
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
            this.inputFunctionTabPage.Size = new System.Drawing.Size(645, 410);
            this.inputFunctionTabPage.TabIndex = 2;
            this.inputFunctionTabPage.Text = "Input Function";
            this.inputFunctionTabPage.UseVisualStyleBackColor = true;
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(565, 245);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(72, 13);
            this.label88.TabIndex = 125;
            this.label88.Text = "Date updated";
            // 
            // inputFunctionDateUpdatedDateTimePicker
            // 
            this.inputFunctionDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputFunctionDateUpdatedDateTimePicker.Enabled = false;
            this.inputFunctionDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputFunctionDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.inputFunctionDateUpdatedDateTimePicker.Name = "inputFunctionDateUpdatedDateTimePicker";
            this.inputFunctionDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputFunctionDateUpdatedDateTimePicker.TabIndex = 16;
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(0, 245);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(69, 13);
            this.label89.TabIndex = 124;
            this.label89.Text = "Date created";
            // 
            // inputFunctionDateCreatedDateTimePicker
            // 
            this.inputFunctionDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputFunctionDateCreatedDateTimePicker.Enabled = false;
            this.inputFunctionDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputFunctionDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.inputFunctionDateCreatedDateTimePicker.Name = "inputFunctionDateCreatedDateTimePicker";
            this.inputFunctionDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputFunctionDateCreatedDateTimePicker.TabIndex = 15;
            // 
            // inputFunctionScalePrecOutputNumericUpDown
            // 
            this.inputFunctionScalePrecOutputNumericUpDown.Location = new System.Drawing.Point(606, 179);
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
            this.inputFunctionScalePrecInputNumericUpDown.Location = new System.Drawing.Point(152, 179);
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
            this.label5.Location = new System.Drawing.Point(556, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Output Register";
            // 
            // inputFunctionOutputTextBox
            // 
            this.inputFunctionOutputTextBox.Location = new System.Drawing.Point(517, 33);
            this.inputFunctionOutputTextBox.Name = "inputFunctionOutputTextBox";
            this.inputFunctionOutputTextBox.Size = new System.Drawing.Size(120, 20);
            this.inputFunctionOutputTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Input Register/Argument";
            // 
            // inputFunctionInputTextBox
            // 
            this.inputFunctionInputTextBox.Location = new System.Drawing.Point(279, 33);
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
            this.label11.Location = new System.Drawing.Point(532, 164);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Forced Output Scale";
            // 
            // inputFunctionScaleOutputTextBox
            // 
            this.inputFunctionScaleOutputTextBox.Location = new System.Drawing.Point(531, 179);
            this.inputFunctionScaleOutputTextBox.Name = "inputFunctionScaleOutputTextBox";
            this.inputFunctionScaleOutputTextBox.Size = new System.Drawing.Size(69, 20);
            this.inputFunctionScaleOutputTextBox.TabIndex = 11;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(86, 164);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Forced Input Scale";
            // 
            // inputFunctionScaleInputTextBox
            // 
            this.inputFunctionScaleInputTextBox.Location = new System.Drawing.Point(77, 179);
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
            this.inputFunctionUnitsOutputTextBox.Location = new System.Drawing.Point(457, 219);
            this.inputFunctionUnitsOutputTextBox.Name = "inputFunctionUnitsOutputTextBox";
            this.inputFunctionUnitsOutputTextBox.Size = new System.Drawing.Size(180, 20);
            this.inputFunctionUnitsOutputTextBox.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(454, 203);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Forced Output Units";
            // 
            // inputFunctionSignedOutputCheckBox
            // 
            this.inputFunctionSignedOutputCheckBox.AutoSize = true;
            this.inputFunctionSignedOutputCheckBox.Location = new System.Drawing.Point(517, 59);
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
            this.inputFunctionUnitsInputTextBox.Size = new System.Drawing.Size(180, 20);
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
            this.inputFunctionSignedInputCheckBox.Location = new System.Drawing.Point(279, 59);
            this.inputFunctionSignedInputCheckBox.Name = "inputFunctionSignedInputCheckBox";
            this.inputFunctionSignedInputCheckBox.Size = new System.Drawing.Size(86, 17);
            this.inputFunctionSignedInputCheckBox.TabIndex = 6;
            this.inputFunctionSignedInputCheckBox.Text = "Signed Input";
            this.inputFunctionSignedInputCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputScalarTabPage
            // 
            this.inputScalarTabPage.AutoScroll = true;
            this.inputScalarTabPage.Controls.Add(this.label90);
            this.inputScalarTabPage.Controls.Add(this.inputScalarDateUpdatedDateTimePicker);
            this.inputScalarTabPage.Controls.Add(this.label91);
            this.inputScalarTabPage.Controls.Add(this.inputScalarDateCreatedDateTimePicker);
            this.inputScalarTabPage.Controls.Add(this.inputScalarScalePrecNumericUpDown);
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
            this.inputScalarTabPage.Size = new System.Drawing.Size(645, 410);
            this.inputScalarTabPage.TabIndex = 0;
            this.inputScalarTabPage.Text = "Input Scalar";
            this.inputScalarTabPage.UseVisualStyleBackColor = true;
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(565, 245);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(72, 13);
            this.label90.TabIndex = 129;
            this.label90.Text = "Date updated";
            // 
            // inputScalarDateUpdatedDateTimePicker
            // 
            this.inputScalarDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputScalarDateUpdatedDateTimePicker.Enabled = false;
            this.inputScalarDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputScalarDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.inputScalarDateUpdatedDateTimePicker.Name = "inputScalarDateUpdatedDateTimePicker";
            this.inputScalarDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputScalarDateUpdatedDateTimePicker.TabIndex = 9;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(0, 245);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(69, 13);
            this.label91.TabIndex = 128;
            this.label91.Text = "Date created";
            // 
            // inputScalarDateCreatedDateTimePicker
            // 
            this.inputScalarDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.inputScalarDateCreatedDateTimePicker.Enabled = false;
            this.inputScalarDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inputScalarDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.inputScalarDateCreatedDateTimePicker.Name = "inputScalarDateCreatedDateTimePicker";
            this.inputScalarDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.inputScalarDateCreatedDateTimePicker.TabIndex = 8;
            // 
            // inputScalarScalePrecNumericUpDown
            // 
            this.inputScalarScalePrecNumericUpDown.Location = new System.Drawing.Point(606, 104);
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
            this.inputScalarByteCheckBox.Location = new System.Drawing.Point(529, 33);
            this.inputScalarByteCheckBox.Name = "inputScalarByteCheckBox";
            this.inputScalarByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.inputScalarByteCheckBox.TabIndex = 4;
            this.inputScalarByteCheckBox.Text = "Byte";
            this.inputScalarByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(567, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Forced Scale";
            // 
            // inputScalarScaleTextBox
            // 
            this.inputScalarScaleTextBox.Location = new System.Drawing.Point(529, 104);
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
            this.inputScalarSignedCheckBox.Location = new System.Drawing.Point(529, 14);
            this.inputScalarSignedCheckBox.Name = "inputScalarSignedCheckBox";
            this.inputScalarSignedCheckBox.Size = new System.Drawing.Size(59, 17);
            this.inputScalarSignedCheckBox.TabIndex = 3;
            this.inputScalarSignedCheckBox.Text = "Signed";
            this.inputScalarSignedCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalStructureTabPage
            // 
            this.internalStructureTabPage.AutoScroll = true;
            this.internalStructureTabPage.Controls.Add(this.internalStructureIdentificationStatusTrackBar);
            this.internalStructureTabPage.Controls.Add(this.internalStructureIdentificationDetailsTextBox);
            this.internalStructureTabPage.Controls.Add(this.internalStructureIdentificationLabel);
            this.internalStructureTabPage.Controls.Add(this.internalStructureCateg3ComboBox);
            this.internalStructureTabPage.Controls.Add(this.internalStructureCateg2ComboBox);
            this.internalStructureTabPage.Controls.Add(this.label80);
            this.internalStructureTabPage.Controls.Add(this.internalStructureCategComboBox);
            this.internalStructureTabPage.Controls.Add(this.label81);
            this.internalStructureTabPage.Controls.Add(this.internalStructureDateUpdatedDateTimePicker);
            this.internalStructureTabPage.Controls.Add(this.label82);
            this.internalStructureTabPage.Controls.Add(this.internalStructureDateCreatedDateTimePicker);
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
            this.internalStructureTabPage.Size = new System.Drawing.Size(645, 410);
            this.internalStructureTabPage.TabIndex = 9;
            this.internalStructureTabPage.Text = "Internal Structure";
            this.internalStructureTabPage.UseVisualStyleBackColor = true;
            // 
            // internalStructureIdentificationStatusTrackBar
            // 
            this.internalStructureIdentificationStatusTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.internalStructureIdentificationStatusTrackBar.LargeChange = 10;
            this.internalStructureIdentificationStatusTrackBar.Location = new System.Drawing.Point(3, 363);
            this.internalStructureIdentificationStatusTrackBar.Maximum = 100;
            this.internalStructureIdentificationStatusTrackBar.Name = "internalStructureIdentificationStatusTrackBar";
            this.internalStructureIdentificationStatusTrackBar.Size = new System.Drawing.Size(249, 45);
            this.internalStructureIdentificationStatusTrackBar.TabIndex = 14;
            this.internalStructureIdentificationStatusTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // internalStructureIdentificationDetailsTextBox
            // 
            this.internalStructureIdentificationDetailsTextBox.Location = new System.Drawing.Point(258, 363);
            this.internalStructureIdentificationDetailsTextBox.Multiline = true;
            this.internalStructureIdentificationDetailsTextBox.Name = "internalStructureIdentificationDetailsTextBox";
            this.internalStructureIdentificationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalStructureIdentificationDetailsTextBox.Size = new System.Drawing.Size(379, 45);
            this.internalStructureIdentificationDetailsTextBox.TabIndex = 15;
            // 
            // internalStructureIdentificationLabel
            // 
            this.internalStructureIdentificationLabel.AutoSize = true;
            this.internalStructureIdentificationLabel.Location = new System.Drawing.Point(0, 347);
            this.internalStructureIdentificationLabel.Name = "internalStructureIdentificationLabel";
            this.internalStructureIdentificationLabel.Size = new System.Drawing.Size(67, 13);
            this.internalStructureIdentificationLabel.TabIndex = 122;
            this.internalStructureIdentificationLabel.Tag = "Identification";
            this.internalStructureIdentificationLabel.Text = "Identification";
            // 
            // internalStructureCateg3ComboBox
            // 
            this.internalStructureCateg3ComboBox.FormattingEnabled = true;
            this.internalStructureCateg3ComboBox.Location = new System.Drawing.Point(517, 308);
            this.internalStructureCateg3ComboBox.Name = "internalStructureCateg3ComboBox";
            this.internalStructureCateg3ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalStructureCateg3ComboBox.Sorted = true;
            this.internalStructureCateg3ComboBox.TabIndex = 13;
            // 
            // internalStructureCateg2ComboBox
            // 
            this.internalStructureCateg2ComboBox.FormattingEnabled = true;
            this.internalStructureCateg2ComboBox.Location = new System.Drawing.Point(258, 308);
            this.internalStructureCateg2ComboBox.Name = "internalStructureCateg2ComboBox";
            this.internalStructureCateg2ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalStructureCateg2ComboBox.Sorted = true;
            this.internalStructureCateg2ComboBox.TabIndex = 12;
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Location = new System.Drawing.Point(255, 292);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(57, 13);
            this.label80.TabIndex = 121;
            this.label80.Text = "Categories";
            // 
            // internalStructureCategComboBox
            // 
            this.internalStructureCategComboBox.FormattingEnabled = true;
            this.internalStructureCategComboBox.Location = new System.Drawing.Point(3, 308);
            this.internalStructureCategComboBox.Name = "internalStructureCategComboBox";
            this.internalStructureCategComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalStructureCategComboBox.Sorted = true;
            this.internalStructureCategComboBox.TabIndex = 11;
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Location = new System.Drawing.Point(565, 245);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(72, 13);
            this.label81.TabIndex = 120;
            this.label81.Text = "Date updated";
            // 
            // internalStructureDateUpdatedDateTimePicker
            // 
            this.internalStructureDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalStructureDateUpdatedDateTimePicker.Enabled = false;
            this.internalStructureDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalStructureDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.internalStructureDateUpdatedDateTimePicker.Name = "internalStructureDateUpdatedDateTimePicker";
            this.internalStructureDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalStructureDateUpdatedDateTimePicker.TabIndex = 10;
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Location = new System.Drawing.Point(0, 245);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(69, 13);
            this.label82.TabIndex = 119;
            this.label82.Text = "Date created";
            // 
            // internalStructureDateCreatedDateTimePicker
            // 
            this.internalStructureDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalStructureDateCreatedDateTimePicker.Enabled = false;
            this.internalStructureDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalStructureDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.internalStructureDateCreatedDateTimePicker.Name = "internalStructureDateCreatedDateTimePicker";
            this.internalStructureDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalStructureDateCreatedDateTimePicker.TabIndex = 9;
            // 
            // internalStructureTipPictureBox
            // 
            this.internalStructureTipPictureBox.Image = global::SAD806x.Properties.Resources.question;
            this.internalStructureTipPictureBox.Location = new System.Drawing.Point(587, 14);
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
            this.internalStructureOutputCommentsCheckBox.Location = new System.Drawing.Point(527, 183);
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
            this.internalStructureStructTextBox.Size = new System.Drawing.Size(634, 71);
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
            this.internalStructureCommentsTextBox.Location = new System.Drawing.Point(3, 200);
            this.internalStructureCommentsTextBox.Multiline = true;
            this.internalStructureCommentsTextBox.Name = "internalStructureCommentsTextBox";
            this.internalStructureCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalStructureCommentsTextBox.Size = new System.Drawing.Size(634, 42);
            this.internalStructureCommentsTextBox.TabIndex = 7;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(0, 184);
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
            this.internalTableTabPage.Controls.Add(this.label92);
            this.internalTableTabPage.Controls.Add(this.internalTableCellsMaxTextBox);
            this.internalTableTabPage.Controls.Add(this.label93);
            this.internalTableTabPage.Controls.Add(this.internalTableCellsMinTextBox);
            this.internalTableTabPage.Controls.Add(this.internalTableIdentificationStatusTrackBar);
            this.internalTableTabPage.Controls.Add(this.internalTableIdentificationDetailsTextBox);
            this.internalTableTabPage.Controls.Add(this.internalTableIdentificationLabel);
            this.internalTableTabPage.Controls.Add(this.internalTableCateg3ComboBox);
            this.internalTableTabPage.Controls.Add(this.internalTableCateg2ComboBox);
            this.internalTableTabPage.Controls.Add(this.sharedCategsLabel);
            this.internalTableTabPage.Controls.Add(this.internalTableCategComboBox);
            this.internalTableTabPage.Controls.Add(this.label68);
            this.internalTableTabPage.Controls.Add(this.internalTableDateUpdatedDateTimePicker);
            this.internalTableTabPage.Controls.Add(this.label69);
            this.internalTableTabPage.Controls.Add(this.internalTableDateCreatedDateTimePicker);
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
            this.internalTableTabPage.Size = new System.Drawing.Size(645, 410);
            this.internalTableTabPage.TabIndex = 10;
            this.internalTableTabPage.Text = "Internal Table";
            this.internalTableTabPage.UseVisualStyleBackColor = true;
            // 
            // label92
            // 
            this.label92.AutoSize = true;
            this.label92.Location = new System.Drawing.Point(586, 97);
            this.label92.Name = "label92";
            this.label92.Size = new System.Drawing.Size(51, 13);
            this.label92.TabIndex = 115;
            this.label92.Text = "Maximum";
            // 
            // internalTableCellsMaxTextBox
            // 
            this.internalTableCellsMaxTextBox.Location = new System.Drawing.Point(568, 113);
            this.internalTableCellsMaxTextBox.Name = "internalTableCellsMaxTextBox";
            this.internalTableCellsMaxTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableCellsMaxTextBox.TabIndex = 113;
            // 
            // label93
            // 
            this.label93.AutoSize = true;
            this.label93.Location = new System.Drawing.Point(479, 97);
            this.label93.Name = "label93";
            this.label93.Size = new System.Drawing.Size(48, 13);
            this.label93.TabIndex = 114;
            this.label93.Text = "Minimum";
            // 
            // internalTableCellsMinTextBox
            // 
            this.internalTableCellsMinTextBox.Location = new System.Drawing.Point(482, 113);
            this.internalTableCellsMinTextBox.Name = "internalTableCellsMinTextBox";
            this.internalTableCellsMinTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableCellsMinTextBox.TabIndex = 112;
            // 
            // internalTableIdentificationStatusTrackBar
            // 
            this.internalTableIdentificationStatusTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.internalTableIdentificationStatusTrackBar.LargeChange = 10;
            this.internalTableIdentificationStatusTrackBar.Location = new System.Drawing.Point(3, 363);
            this.internalTableIdentificationStatusTrackBar.Maximum = 100;
            this.internalTableIdentificationStatusTrackBar.Name = "internalTableIdentificationStatusTrackBar";
            this.internalTableIdentificationStatusTrackBar.Size = new System.Drawing.Size(241, 45);
            this.internalTableIdentificationStatusTrackBar.TabIndex = 22;
            this.internalTableIdentificationStatusTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // internalTableIdentificationDetailsTextBox
            // 
            this.internalTableIdentificationDetailsTextBox.Location = new System.Drawing.Point(250, 363);
            this.internalTableIdentificationDetailsTextBox.Multiline = true;
            this.internalTableIdentificationDetailsTextBox.Name = "internalTableIdentificationDetailsTextBox";
            this.internalTableIdentificationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalTableIdentificationDetailsTextBox.Size = new System.Drawing.Size(387, 45);
            this.internalTableIdentificationDetailsTextBox.TabIndex = 23;
            // 
            // internalTableIdentificationLabel
            // 
            this.internalTableIdentificationLabel.AutoSize = true;
            this.internalTableIdentificationLabel.Location = new System.Drawing.Point(0, 347);
            this.internalTableIdentificationLabel.Name = "internalTableIdentificationLabel";
            this.internalTableIdentificationLabel.Size = new System.Drawing.Size(67, 13);
            this.internalTableIdentificationLabel.TabIndex = 111;
            this.internalTableIdentificationLabel.Tag = "Identification";
            this.internalTableIdentificationLabel.Text = "Identification";
            // 
            // internalTableCateg3ComboBox
            // 
            this.internalTableCateg3ComboBox.FormattingEnabled = true;
            this.internalTableCateg3ComboBox.Location = new System.Drawing.Point(517, 308);
            this.internalTableCateg3ComboBox.Name = "internalTableCateg3ComboBox";
            this.internalTableCateg3ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalTableCateg3ComboBox.Sorted = true;
            this.internalTableCateg3ComboBox.TabIndex = 21;
            // 
            // internalTableCateg2ComboBox
            // 
            this.internalTableCateg2ComboBox.FormattingEnabled = true;
            this.internalTableCateg2ComboBox.Location = new System.Drawing.Point(250, 308);
            this.internalTableCateg2ComboBox.Name = "internalTableCateg2ComboBox";
            this.internalTableCateg2ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalTableCateg2ComboBox.Sorted = true;
            this.internalTableCateg2ComboBox.TabIndex = 20;
            // 
            // sharedCategsLabel
            // 
            this.sharedCategsLabel.AutoSize = true;
            this.sharedCategsLabel.Location = new System.Drawing.Point(252, 292);
            this.sharedCategsLabel.Name = "sharedCategsLabel";
            this.sharedCategsLabel.Size = new System.Drawing.Size(57, 13);
            this.sharedCategsLabel.TabIndex = 110;
            this.sharedCategsLabel.Text = "Categories";
            // 
            // internalTableCategComboBox
            // 
            this.internalTableCategComboBox.FormattingEnabled = true;
            this.internalTableCategComboBox.Location = new System.Drawing.Point(3, 308);
            this.internalTableCategComboBox.Name = "internalTableCategComboBox";
            this.internalTableCategComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalTableCategComboBox.Sorted = true;
            this.internalTableCategComboBox.TabIndex = 19;
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(565, 245);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(72, 13);
            this.label68.TabIndex = 109;
            this.label68.Text = "Date updated";
            // 
            // internalTableDateUpdatedDateTimePicker
            // 
            this.internalTableDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalTableDateUpdatedDateTimePicker.Enabled = false;
            this.internalTableDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalTableDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.internalTableDateUpdatedDateTimePicker.Name = "internalTableDateUpdatedDateTimePicker";
            this.internalTableDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalTableDateUpdatedDateTimePicker.TabIndex = 18;
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(0, 245);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(69, 13);
            this.label69.TabIndex = 108;
            this.label69.Text = "Date created";
            // 
            // internalTableDateCreatedDateTimePicker
            // 
            this.internalTableDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalTableDateCreatedDateTimePicker.Enabled = false;
            this.internalTableDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalTableDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.internalTableDateCreatedDateTimePicker.Name = "internalTableDateCreatedDateTimePicker";
            this.internalTableDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalTableDateCreatedDateTimePicker.TabIndex = 17;
            // 
            // internalTableScalePrecNumericUpDown
            // 
            this.internalTableScalePrecNumericUpDown.Location = new System.Drawing.Point(606, 69);
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
            this.internalTableOutputCommentsCheckBox.Location = new System.Drawing.Point(527, 183);
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
            this.internalTableCellsUnitsTextBox.Location = new System.Drawing.Point(482, 156);
            this.internalTableCellsUnitsTextBox.Name = "internalTableCellsUnitsTextBox";
            this.internalTableCellsUnitsTextBox.Size = new System.Drawing.Size(155, 20);
            this.internalTableCellsUnitsTextBox.TabIndex = 13;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(581, 140);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(56, 13);
            this.label30.TabIndex = 73;
            this.label30.Text = "Cells Units";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(410, 14);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(87, 13);
            this.label31.TabIndex = 72;
            this.label31.Text = "Columns Number";
            // 
            // internalTableColsTextBox
            // 
            this.internalTableColsTextBox.Location = new System.Drawing.Point(413, 30);
            this.internalTableColsTextBox.Name = "internalTableColsTextBox";
            this.internalTableColsTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableColsTextBox.TabIndex = 5;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(479, 53);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(34, 13);
            this.label32.TabIndex = 71;
            this.label32.Text = "Scale";
            // 
            // internalTableScaleTextBox
            // 
            this.internalTableScaleTextBox.Location = new System.Drawing.Point(482, 68);
            this.internalTableScaleTextBox.Name = "internalTableScaleTextBox";
            this.internalTableScaleTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableScaleTextBox.TabIndex = 9;
            this.internalTableScaleTextBox.Text = "X";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(563, 14);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(74, 13);
            this.label35.TabIndex = 70;
            this.label35.Text = "Rows Number";
            // 
            // internalTableRowsTextBox
            // 
            this.internalTableRowsTextBox.Location = new System.Drawing.Point(568, 30);
            this.internalTableRowsTextBox.Name = "internalTableRowsTextBox";
            this.internalTableRowsTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalTableRowsTextBox.TabIndex = 6;
            this.internalTableRowsTextBox.Text = "0";
            // 
            // internalTableRowsUnitsTextBox
            // 
            this.internalTableRowsUnitsTextBox.Location = new System.Drawing.Point(250, 156);
            this.internalTableRowsUnitsTextBox.Name = "internalTableRowsUnitsTextBox";
            this.internalTableRowsUnitsTextBox.Size = new System.Drawing.Size(153, 20);
            this.internalTableRowsUnitsTextBox.TabIndex = 12;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(247, 140);
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
            this.internalTableCommentsTextBox.Location = new System.Drawing.Point(3, 200);
            this.internalTableCommentsTextBox.Multiline = true;
            this.internalTableCommentsTextBox.Name = "internalTableCommentsTextBox";
            this.internalTableCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalTableCommentsTextBox.Size = new System.Drawing.Size(634, 42);
            this.internalTableCommentsTextBox.TabIndex = 14;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(0, 184);
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
            this.internalFunctionTabPage.Controls.Add(this.label98);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionMaxOutputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label99);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionMinOutputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label96);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionMaxInputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.label97);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionMinInputTextBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionIdentificationStatusTrackBar);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionIdentificationDetailsTextBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionIdentificationLabel);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionCateg3ComboBox);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionCateg2ComboBox);
            this.internalFunctionTabPage.Controls.Add(this.label74);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionCategComboBox);
            this.internalFunctionTabPage.Controls.Add(this.label75);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionDateUpdatedDateTimePicker);
            this.internalFunctionTabPage.Controls.Add(this.label76);
            this.internalFunctionTabPage.Controls.Add(this.internalFunctionDateCreatedDateTimePicker);
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
            this.internalFunctionTabPage.Size = new System.Drawing.Size(645, 410);
            this.internalFunctionTabPage.TabIndex = 11;
            this.internalFunctionTabPage.Text = "Internal Function";
            this.internalFunctionTabPage.UseVisualStyleBackColor = true;
            // 
            // label98
            // 
            this.label98.AutoSize = true;
            this.label98.Location = new System.Drawing.Point(426, 139);
            this.label98.Name = "label98";
            this.label98.Size = new System.Drawing.Size(62, 13);
            this.label98.TabIndex = 130;
            this.label98.Text = "Output Max";
            // 
            // internalFunctionMaxOutputTextBox
            // 
            this.internalFunctionMaxOutputTextBox.Location = new System.Drawing.Point(429, 155);
            this.internalFunctionMaxOutputTextBox.Name = "internalFunctionMaxOutputTextBox";
            this.internalFunctionMaxOutputTextBox.Size = new System.Drawing.Size(45, 20);
            this.internalFunctionMaxOutputTextBox.TabIndex = 16;
            // 
            // label99
            // 
            this.label99.AutoSize = true;
            this.label99.Location = new System.Drawing.Point(336, 139);
            this.label99.Name = "label99";
            this.label99.Size = new System.Drawing.Size(59, 13);
            this.label99.TabIndex = 129;
            this.label99.Text = "Output Min";
            // 
            // internalFunctionMinOutputTextBox
            // 
            this.internalFunctionMinOutputTextBox.Location = new System.Drawing.Point(339, 155);
            this.internalFunctionMinOutputTextBox.Name = "internalFunctionMinOutputTextBox";
            this.internalFunctionMinOutputTextBox.Size = new System.Drawing.Size(45, 20);
            this.internalFunctionMinOutputTextBox.TabIndex = 15;
            // 
            // label96
            // 
            this.label96.AutoSize = true;
            this.label96.Location = new System.Drawing.Point(222, 139);
            this.label96.Name = "label96";
            this.label96.Size = new System.Drawing.Size(54, 13);
            this.label96.TabIndex = 126;
            this.label96.Text = "Input Max";
            // 
            // internalFunctionMaxInputTextBox
            // 
            this.internalFunctionMaxInputTextBox.Location = new System.Drawing.Point(225, 155);
            this.internalFunctionMaxInputTextBox.Name = "internalFunctionMaxInputTextBox";
            this.internalFunctionMaxInputTextBox.Size = new System.Drawing.Size(45, 20);
            this.internalFunctionMaxInputTextBox.TabIndex = 14;
            // 
            // label97
            // 
            this.label97.AutoSize = true;
            this.label97.Location = new System.Drawing.Point(143, 139);
            this.label97.Name = "label97";
            this.label97.Size = new System.Drawing.Size(51, 13);
            this.label97.TabIndex = 125;
            this.label97.Text = "Input Min";
            // 
            // internalFunctionMinInputTextBox
            // 
            this.internalFunctionMinInputTextBox.Location = new System.Drawing.Point(146, 155);
            this.internalFunctionMinInputTextBox.Name = "internalFunctionMinInputTextBox";
            this.internalFunctionMinInputTextBox.Size = new System.Drawing.Size(45, 20);
            this.internalFunctionMinInputTextBox.TabIndex = 13;
            // 
            // internalFunctionIdentificationStatusTrackBar
            // 
            this.internalFunctionIdentificationStatusTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.internalFunctionIdentificationStatusTrackBar.LargeChange = 10;
            this.internalFunctionIdentificationStatusTrackBar.Location = new System.Drawing.Point(3, 363);
            this.internalFunctionIdentificationStatusTrackBar.Maximum = 100;
            this.internalFunctionIdentificationStatusTrackBar.Name = "internalFunctionIdentificationStatusTrackBar";
            this.internalFunctionIdentificationStatusTrackBar.Size = new System.Drawing.Size(252, 45);
            this.internalFunctionIdentificationStatusTrackBar.TabIndex = 25;
            this.internalFunctionIdentificationStatusTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // internalFunctionIdentificationDetailsTextBox
            // 
            this.internalFunctionIdentificationDetailsTextBox.Location = new System.Drawing.Point(261, 363);
            this.internalFunctionIdentificationDetailsTextBox.Multiline = true;
            this.internalFunctionIdentificationDetailsTextBox.Name = "internalFunctionIdentificationDetailsTextBox";
            this.internalFunctionIdentificationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalFunctionIdentificationDetailsTextBox.Size = new System.Drawing.Size(376, 45);
            this.internalFunctionIdentificationDetailsTextBox.TabIndex = 26;
            // 
            // internalFunctionIdentificationLabel
            // 
            this.internalFunctionIdentificationLabel.AutoSize = true;
            this.internalFunctionIdentificationLabel.Location = new System.Drawing.Point(0, 347);
            this.internalFunctionIdentificationLabel.Name = "internalFunctionIdentificationLabel";
            this.internalFunctionIdentificationLabel.Size = new System.Drawing.Size(67, 13);
            this.internalFunctionIdentificationLabel.TabIndex = 122;
            this.internalFunctionIdentificationLabel.Tag = "Identification";
            this.internalFunctionIdentificationLabel.Text = "Identification";
            // 
            // internalFunctionCateg3ComboBox
            // 
            this.internalFunctionCateg3ComboBox.FormattingEnabled = true;
            this.internalFunctionCateg3ComboBox.Location = new System.Drawing.Point(517, 308);
            this.internalFunctionCateg3ComboBox.Name = "internalFunctionCateg3ComboBox";
            this.internalFunctionCateg3ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalFunctionCateg3ComboBox.Sorted = true;
            this.internalFunctionCateg3ComboBox.TabIndex = 24;
            // 
            // internalFunctionCateg2ComboBox
            // 
            this.internalFunctionCateg2ComboBox.FormattingEnabled = true;
            this.internalFunctionCateg2ComboBox.Location = new System.Drawing.Point(261, 308);
            this.internalFunctionCateg2ComboBox.Name = "internalFunctionCateg2ComboBox";
            this.internalFunctionCateg2ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalFunctionCateg2ComboBox.Sorted = true;
            this.internalFunctionCateg2ComboBox.TabIndex = 23;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(258, 292);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(57, 13);
            this.label74.TabIndex = 121;
            this.label74.Text = "Categories";
            // 
            // internalFunctionCategComboBox
            // 
            this.internalFunctionCategComboBox.FormattingEnabled = true;
            this.internalFunctionCategComboBox.Location = new System.Drawing.Point(3, 308);
            this.internalFunctionCategComboBox.Name = "internalFunctionCategComboBox";
            this.internalFunctionCategComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalFunctionCategComboBox.Sorted = true;
            this.internalFunctionCategComboBox.TabIndex = 22;
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(565, 245);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(72, 13);
            this.label75.TabIndex = 120;
            this.label75.Text = "Date updated";
            // 
            // internalFunctionDateUpdatedDateTimePicker
            // 
            this.internalFunctionDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalFunctionDateUpdatedDateTimePicker.Enabled = false;
            this.internalFunctionDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalFunctionDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.internalFunctionDateUpdatedDateTimePicker.Name = "internalFunctionDateUpdatedDateTimePicker";
            this.internalFunctionDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalFunctionDateUpdatedDateTimePicker.TabIndex = 21;
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(0, 245);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(69, 13);
            this.label76.TabIndex = 119;
            this.label76.Text = "Date created";
            // 
            // internalFunctionDateCreatedDateTimePicker
            // 
            this.internalFunctionDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalFunctionDateCreatedDateTimePicker.Enabled = false;
            this.internalFunctionDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalFunctionDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.internalFunctionDateCreatedDateTimePicker.Name = "internalFunctionDateCreatedDateTimePicker";
            this.internalFunctionDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalFunctionDateCreatedDateTimePicker.TabIndex = 20;
            // 
            // internalFunctionScalePrecOutputNumericUpDown
            // 
            this.internalFunctionScalePrecOutputNumericUpDown.Location = new System.Drawing.Point(606, 116);
            this.internalFunctionScalePrecOutputNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.internalFunctionScalePrecOutputNumericUpDown.Name = "internalFunctionScalePrecOutputNumericUpDown";
            this.internalFunctionScalePrecOutputNumericUpDown.Size = new System.Drawing.Size(31, 20);
            this.internalFunctionScalePrecOutputNumericUpDown.TabIndex = 11;
            this.internalFunctionScalePrecOutputNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // internalFunctionScalePrecInputNumericUpDown
            // 
            this.internalFunctionScalePrecInputNumericUpDown.Location = new System.Drawing.Point(239, 114);
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
            this.internalFunctionBankTextBox.TabIndex = 2;
            // 
            // internalFunctionOutputCommentsCheckBox
            // 
            this.internalFunctionOutputCommentsCheckBox.AutoSize = true;
            this.internalFunctionOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalFunctionOutputCommentsCheckBox.Location = new System.Drawing.Point(527, 183);
            this.internalFunctionOutputCommentsCheckBox.Name = "internalFunctionOutputCommentsCheckBox";
            this.internalFunctionOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.internalFunctionOutputCommentsCheckBox.TabIndex = 19;
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
            this.internalFunctionSLabelTextBox.TabIndex = 5;
            // 
            // internalFunctionByteCheckBox
            // 
            this.internalFunctionByteCheckBox.AutoSize = true;
            this.internalFunctionByteCheckBox.Location = new System.Drawing.Point(239, 33);
            this.internalFunctionByteCheckBox.Name = "internalFunctionByteCheckBox";
            this.internalFunctionByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.internalFunctionByteCheckBox.TabIndex = 1;
            this.internalFunctionByteCheckBox.Text = "Byte";
            this.internalFunctionByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(568, 100);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(69, 13);
            this.label45.TabIndex = 100;
            this.label45.Text = "Output Scale";
            // 
            // internalFunctionScaleOutputTextBox
            // 
            this.internalFunctionScaleOutputTextBox.Location = new System.Drawing.Point(517, 115);
            this.internalFunctionScaleOutputTextBox.Name = "internalFunctionScaleOutputTextBox";
            this.internalFunctionScaleOutputTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalFunctionScaleOutputTextBox.TabIndex = 10;
            this.internalFunctionScaleOutputTextBox.Text = "X";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(209, 99);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(61, 13);
            this.label46.TabIndex = 99;
            this.label46.Text = "Input Scale";
            // 
            // internalFunctionScaleInputTextBox
            // 
            this.internalFunctionScaleInputTextBox.Location = new System.Drawing.Point(146, 115);
            this.internalFunctionScaleInputTextBox.Name = "internalFunctionScaleInputTextBox";
            this.internalFunctionScaleInputTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalFunctionScaleInputTextBox.TabIndex = 7;
            this.internalFunctionScaleInputTextBox.Text = "X";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(563, 15);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(74, 13);
            this.label47.TabIndex = 98;
            this.label47.Text = "Rows Number";
            // 
            // internalFunctionRowsTextBox
            // 
            this.internalFunctionRowsTextBox.Location = new System.Drawing.Point(568, 31);
            this.internalFunctionRowsTextBox.Name = "internalFunctionRowsTextBox";
            this.internalFunctionRowsTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalFunctionRowsTextBox.TabIndex = 3;
            this.internalFunctionRowsTextBox.Text = "0";
            // 
            // internalFunctionUnitsOutputTextBox
            // 
            this.internalFunctionUnitsOutputTextBox.Location = new System.Drawing.Point(517, 155);
            this.internalFunctionUnitsOutputTextBox.Name = "internalFunctionUnitsOutputTextBox";
            this.internalFunctionUnitsOutputTextBox.Size = new System.Drawing.Size(120, 20);
            this.internalFunctionUnitsOutputTextBox.TabIndex = 17;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(571, 139);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(66, 13);
            this.label48.TabIndex = 97;
            this.label48.Text = "Output Units";
            // 
            // internalFunctionSignedOutputCheckBox
            // 
            this.internalFunctionSignedOutputCheckBox.AutoSize = true;
            this.internalFunctionSignedOutputCheckBox.Location = new System.Drawing.Point(339, 115);
            this.internalFunctionSignedOutputCheckBox.Name = "internalFunctionSignedOutputCheckBox";
            this.internalFunctionSignedOutputCheckBox.Size = new System.Drawing.Size(94, 17);
            this.internalFunctionSignedOutputCheckBox.TabIndex = 9;
            this.internalFunctionSignedOutputCheckBox.Text = "Signed Output";
            this.internalFunctionSignedOutputCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalFunctionUnitsInputTextBox
            // 
            this.internalFunctionUnitsInputTextBox.Location = new System.Drawing.Point(3, 155);
            this.internalFunctionUnitsInputTextBox.Name = "internalFunctionUnitsInputTextBox";
            this.internalFunctionUnitsInputTextBox.Size = new System.Drawing.Size(120, 20);
            this.internalFunctionUnitsInputTextBox.TabIndex = 12;
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
            this.internalFunctionCommentsTextBox.Size = new System.Drawing.Size(634, 42);
            this.internalFunctionCommentsTextBox.TabIndex = 18;
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
            this.internalFunctionLabelTextBox.TabIndex = 4;
            // 
            // internalFunctionSignedInputCheckBox
            // 
            this.internalFunctionSignedInputCheckBox.AutoSize = true;
            this.internalFunctionSignedInputCheckBox.Location = new System.Drawing.Point(3, 115);
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
            this.internalFunctionAddrTextBox.TabIndex = 0;
            // 
            // internalScalarTabPage
            // 
            this.internalScalarTabPage.AutoScroll = true;
            this.internalScalarTabPage.Controls.Add(this.label94);
            this.internalScalarTabPage.Controls.Add(this.internalScalarMaxTextBox);
            this.internalScalarTabPage.Controls.Add(this.label95);
            this.internalScalarTabPage.Controls.Add(this.internalScalarMinTextBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarIdentificationStatusTrackBar);
            this.internalScalarTabPage.Controls.Add(this.internalScalarIdentificationDetailsTextBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarIdentificationLabel);
            this.internalScalarTabPage.Controls.Add(this.internalScalarCateg3ComboBox);
            this.internalScalarTabPage.Controls.Add(this.internalScalarCateg2ComboBox);
            this.internalScalarTabPage.Controls.Add(this.label77);
            this.internalScalarTabPage.Controls.Add(this.internalScalarCategComboBox);
            this.internalScalarTabPage.Controls.Add(this.label78);
            this.internalScalarTabPage.Controls.Add(this.internalScalarDateUpdatedDateTimePicker);
            this.internalScalarTabPage.Controls.Add(this.label79);
            this.internalScalarTabPage.Controls.Add(this.internalScalarDateCreatedDateTimePicker);
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
            this.internalScalarTabPage.Size = new System.Drawing.Size(645, 410);
            this.internalScalarTabPage.TabIndex = 12;
            this.internalScalarTabPage.Text = "Internal Scalar";
            this.internalScalarTabPage.UseVisualStyleBackColor = true;
            // 
            // label94
            // 
            this.label94.AutoSize = true;
            this.label94.Location = new System.Drawing.Point(586, 141);
            this.label94.Name = "label94";
            this.label94.Size = new System.Drawing.Size(51, 13);
            this.label94.TabIndex = 137;
            this.label94.Text = "Maximum";
            // 
            // internalScalarMaxTextBox
            // 
            this.internalScalarMaxTextBox.Location = new System.Drawing.Point(568, 157);
            this.internalScalarMaxTextBox.Name = "internalScalarMaxTextBox";
            this.internalScalarMaxTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalScalarMaxTextBox.TabIndex = 11;
            // 
            // label95
            // 
            this.label95.AutoSize = true;
            this.label95.Location = new System.Drawing.Point(446, 141);
            this.label95.Name = "label95";
            this.label95.Size = new System.Drawing.Size(48, 13);
            this.label95.TabIndex = 136;
            this.label95.Text = "Minimum";
            // 
            // internalScalarMinTextBox
            // 
            this.internalScalarMinTextBox.Location = new System.Drawing.Point(449, 157);
            this.internalScalarMinTextBox.Name = "internalScalarMinTextBox";
            this.internalScalarMinTextBox.Size = new System.Drawing.Size(69, 20);
            this.internalScalarMinTextBox.TabIndex = 10;
            // 
            // internalScalarIdentificationStatusTrackBar
            // 
            this.internalScalarIdentificationStatusTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.internalScalarIdentificationStatusTrackBar.LargeChange = 10;
            this.internalScalarIdentificationStatusTrackBar.Location = new System.Drawing.Point(3, 363);
            this.internalScalarIdentificationStatusTrackBar.Maximum = 100;
            this.internalScalarIdentificationStatusTrackBar.Name = "internalScalarIdentificationStatusTrackBar";
            this.internalScalarIdentificationStatusTrackBar.Size = new System.Drawing.Size(248, 45);
            this.internalScalarIdentificationStatusTrackBar.TabIndex = 20;
            this.internalScalarIdentificationStatusTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // internalScalarIdentificationDetailsTextBox
            // 
            this.internalScalarIdentificationDetailsTextBox.Location = new System.Drawing.Point(257, 363);
            this.internalScalarIdentificationDetailsTextBox.Multiline = true;
            this.internalScalarIdentificationDetailsTextBox.Name = "internalScalarIdentificationDetailsTextBox";
            this.internalScalarIdentificationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalScalarIdentificationDetailsTextBox.Size = new System.Drawing.Size(380, 45);
            this.internalScalarIdentificationDetailsTextBox.TabIndex = 21;
            // 
            // internalScalarIdentificationLabel
            // 
            this.internalScalarIdentificationLabel.AutoSize = true;
            this.internalScalarIdentificationLabel.Location = new System.Drawing.Point(0, 347);
            this.internalScalarIdentificationLabel.Name = "internalScalarIdentificationLabel";
            this.internalScalarIdentificationLabel.Size = new System.Drawing.Size(67, 13);
            this.internalScalarIdentificationLabel.TabIndex = 133;
            this.internalScalarIdentificationLabel.Tag = "Identification";
            this.internalScalarIdentificationLabel.Text = "Identification";
            // 
            // internalScalarCateg3ComboBox
            // 
            this.internalScalarCateg3ComboBox.FormattingEnabled = true;
            this.internalScalarCateg3ComboBox.Location = new System.Drawing.Point(517, 308);
            this.internalScalarCateg3ComboBox.Name = "internalScalarCateg3ComboBox";
            this.internalScalarCateg3ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalScalarCateg3ComboBox.Sorted = true;
            this.internalScalarCateg3ComboBox.TabIndex = 19;
            // 
            // internalScalarCateg2ComboBox
            // 
            this.internalScalarCateg2ComboBox.FormattingEnabled = true;
            this.internalScalarCateg2ComboBox.Location = new System.Drawing.Point(257, 308);
            this.internalScalarCateg2ComboBox.Name = "internalScalarCateg2ComboBox";
            this.internalScalarCateg2ComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalScalarCateg2ComboBox.Sorted = true;
            this.internalScalarCateg2ComboBox.TabIndex = 18;
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Location = new System.Drawing.Point(254, 292);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(57, 13);
            this.label77.TabIndex = 132;
            this.label77.Text = "Categories";
            // 
            // internalScalarCategComboBox
            // 
            this.internalScalarCategComboBox.FormattingEnabled = true;
            this.internalScalarCategComboBox.Location = new System.Drawing.Point(3, 308);
            this.internalScalarCategComboBox.Name = "internalScalarCategComboBox";
            this.internalScalarCategComboBox.Size = new System.Drawing.Size(120, 21);
            this.internalScalarCategComboBox.Sorted = true;
            this.internalScalarCategComboBox.TabIndex = 17;
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(565, 245);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(72, 13);
            this.label78.TabIndex = 131;
            this.label78.Text = "Date updated";
            // 
            // internalScalarDateUpdatedDateTimePicker
            // 
            this.internalScalarDateUpdatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalScalarDateUpdatedDateTimePicker.Enabled = false;
            this.internalScalarDateUpdatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalScalarDateUpdatedDateTimePicker.Location = new System.Drawing.Point(457, 261);
            this.internalScalarDateUpdatedDateTimePicker.Name = "internalScalarDateUpdatedDateTimePicker";
            this.internalScalarDateUpdatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalScalarDateUpdatedDateTimePicker.TabIndex = 16;
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Location = new System.Drawing.Point(0, 245);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(69, 13);
            this.label79.TabIndex = 130;
            this.label79.Text = "Date created";
            // 
            // internalScalarDateCreatedDateTimePicker
            // 
            this.internalScalarDateCreatedDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.internalScalarDateCreatedDateTimePicker.Enabled = false;
            this.internalScalarDateCreatedDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.internalScalarDateCreatedDateTimePicker.Location = new System.Drawing.Point(3, 261);
            this.internalScalarDateCreatedDateTimePicker.Name = "internalScalarDateCreatedDateTimePicker";
            this.internalScalarDateCreatedDateTimePicker.Size = new System.Drawing.Size(180, 20);
            this.internalScalarDateCreatedDateTimePicker.TabIndex = 15;
            // 
            // internalScalarInlineCommentsCheckBox
            // 
            this.internalScalarInlineCommentsCheckBox.AutoSize = true;
            this.internalScalarInlineCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalScalarInlineCommentsCheckBox.Location = new System.Drawing.Point(418, 183);
            this.internalScalarInlineCommentsCheckBox.Name = "internalScalarInlineCommentsCheckBox";
            this.internalScalarInlineCommentsCheckBox.Size = new System.Drawing.Size(103, 17);
            this.internalScalarInlineCommentsCheckBox.TabIndex = 13;
            this.internalScalarInlineCommentsCheckBox.Text = "Inline Comments";
            this.internalScalarInlineCommentsCheckBox.UseVisualStyleBackColor = true;
            // 
            // internalScalarScalePrecNumericUpDown
            // 
            this.internalScalarScalePrecNumericUpDown.Location = new System.Drawing.Point(606, 114);
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
            this.internalScalarBitFlagsButton.Location = new System.Drawing.Point(257, 28);
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
            this.internalScalarBitFlagsCheckBox.Location = new System.Drawing.Point(257, 14);
            this.internalScalarBitFlagsCheckBox.Name = "internalScalarBitFlagsCheckBox";
            this.internalScalarBitFlagsCheckBox.Size = new System.Drawing.Size(66, 17);
            this.internalScalarBitFlagsCheckBox.TabIndex = 107;
            this.internalScalarBitFlagsCheckBox.Text = "Bit Flags";
            this.internalScalarBitFlagsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(605, 15);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(32, 13);
            this.label64.TabIndex = 105;
            this.label64.Text = "Bank";
            // 
            // internalScalarBankTextBox
            // 
            this.internalScalarBankTextBox.Location = new System.Drawing.Point(553, 31);
            this.internalScalarBankTextBox.Name = "internalScalarBankTextBox";
            this.internalScalarBankTextBox.Size = new System.Drawing.Size(84, 20);
            this.internalScalarBankTextBox.TabIndex = 3;
            // 
            // internalScalarOutputCommentsCheckBox
            // 
            this.internalScalarOutputCommentsCheckBox.AutoSize = true;
            this.internalScalarOutputCommentsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.internalScalarOutputCommentsCheckBox.Location = new System.Drawing.Point(527, 183);
            this.internalScalarOutputCommentsCheckBox.Name = "internalScalarOutputCommentsCheckBox";
            this.internalScalarOutputCommentsCheckBox.Size = new System.Drawing.Size(110, 17);
            this.internalScalarOutputCommentsCheckBox.TabIndex = 14;
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
            this.internalScalarByteCheckBox.Location = new System.Drawing.Point(257, 115);
            this.internalScalarByteCheckBox.Name = "internalScalarByteCheckBox";
            this.internalScalarByteCheckBox.Size = new System.Drawing.Size(47, 17);
            this.internalScalarByteCheckBox.TabIndex = 7;
            this.internalScalarByteCheckBox.Text = "Byte";
            this.internalScalarByteCheckBox.UseVisualStyleBackColor = true;
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(603, 100);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(34, 13);
            this.label54.TabIndex = 93;
            this.label54.Text = "Scale";
            // 
            // internalScalarScaleTextBox
            // 
            this.internalScalarScaleTextBox.Location = new System.Drawing.Point(517, 113);
            this.internalScalarScaleTextBox.Name = "internalScalarScaleTextBox";
            this.internalScalarScaleTextBox.Size = new System.Drawing.Size(71, 20);
            this.internalScalarScaleTextBox.TabIndex = 8;
            this.internalScalarScaleTextBox.Text = "X";
            // 
            // internalScalarUnitsTextBox
            // 
            this.internalScalarUnitsTextBox.Location = new System.Drawing.Point(449, 73);
            this.internalScalarUnitsTextBox.Name = "internalScalarUnitsTextBox";
            this.internalScalarUnitsTextBox.Size = new System.Drawing.Size(188, 20);
            this.internalScalarUnitsTextBox.TabIndex = 4;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(606, 57);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(31, 13);
            this.label55.TabIndex = 90;
            this.label55.Text = "Units";
            // 
            // internalScalarCommentsTextBox
            // 
            this.internalScalarCommentsTextBox.Location = new System.Drawing.Point(3, 200);
            this.internalScalarCommentsTextBox.Multiline = true;
            this.internalScalarCommentsTextBox.Name = "internalScalarCommentsTextBox";
            this.internalScalarCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.internalScalarCommentsTextBox.Size = new System.Drawing.Size(634, 42);
            this.internalScalarCommentsTextBox.TabIndex = 12;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(0, 184);
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
            this.advFooterPanel.Location = new System.Drawing.Point(0, 591);
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
            this.elemUpdateButton.TabIndex = 100;
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
            this.ClientSize = new System.Drawing.Size(984, 647);
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
            ((System.ComponentModel.ISupportInitialize)(this.routineIdentificationStatusTrackBar)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.internalStructureIdentificationStatusTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalStructureTipPictureBox)).EndInit();
            this.internalTableTabPage.ResumeLayout(false);
            this.internalTableTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalTableIdentificationStatusTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalTableScalePrecNumericUpDown)).EndInit();
            this.internalFunctionTabPage.ResumeLayout(false);
            this.internalFunctionTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionIdentificationStatusTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecOutputNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.internalFunctionScalePrecInputNumericUpDown)).EndInit();
            this.internalScalarTabPage.ResumeLayout(false);
            this.internalScalarTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.internalScalarIdentificationStatusTrackBar)).EndInit();
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
        private System.Windows.Forms.TrackBar internalTableIdentificationStatusTrackBar;
        private System.Windows.Forms.TextBox internalTableIdentificationDetailsTextBox;
        private System.Windows.Forms.Label internalTableIdentificationLabel;
        private System.Windows.Forms.ComboBox internalTableCateg3ComboBox;
        private System.Windows.Forms.ComboBox internalTableCateg2ComboBox;
        private System.Windows.Forms.Label sharedCategsLabel;
        private System.Windows.Forms.ComboBox internalTableCategComboBox;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.DateTimePicker internalTableDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.DateTimePicker internalTableDateCreatedDateTimePicker;
        private System.Windows.Forms.TrackBar routineIdentificationStatusTrackBar;
        private System.Windows.Forms.TextBox routineIdentificationDetailsTextBox;
        private System.Windows.Forms.Label routineIdentificationLabel;
        private System.Windows.Forms.ComboBox routineCateg3ComboBox;
        private System.Windows.Forms.ComboBox routineCateg2ComboBox;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.ComboBox routineCategComboBox;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.DateTimePicker routineDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.DateTimePicker routineDateCreatedDateTimePicker;
        private System.Windows.Forms.TrackBar internalFunctionIdentificationStatusTrackBar;
        private System.Windows.Forms.TextBox internalFunctionIdentificationDetailsTextBox;
        private System.Windows.Forms.Label internalFunctionIdentificationLabel;
        private System.Windows.Forms.ComboBox internalFunctionCateg3ComboBox;
        private System.Windows.Forms.ComboBox internalFunctionCateg2ComboBox;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.ComboBox internalFunctionCategComboBox;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.DateTimePicker internalFunctionDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.DateTimePicker internalFunctionDateCreatedDateTimePicker;
        private System.Windows.Forms.TrackBar internalScalarIdentificationStatusTrackBar;
        private System.Windows.Forms.TextBox internalScalarIdentificationDetailsTextBox;
        private System.Windows.Forms.Label internalScalarIdentificationLabel;
        private System.Windows.Forms.ComboBox internalScalarCateg3ComboBox;
        private System.Windows.Forms.ComboBox internalScalarCateg2ComboBox;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.ComboBox internalScalarCategComboBox;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.DateTimePicker internalScalarDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.DateTimePicker internalScalarDateCreatedDateTimePicker;
        private System.Windows.Forms.TrackBar internalStructureIdentificationStatusTrackBar;
        private System.Windows.Forms.TextBox internalStructureIdentificationDetailsTextBox;
        private System.Windows.Forms.Label internalStructureIdentificationLabel;
        private System.Windows.Forms.ComboBox internalStructureCateg3ComboBox;
        private System.Windows.Forms.ComboBox internalStructureCateg2ComboBox;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.ComboBox internalStructureCategComboBox;
        private System.Windows.Forms.Label label81;
        private System.Windows.Forms.DateTimePicker internalStructureDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.DateTimePicker internalStructureDateCreatedDateTimePicker;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.DateTimePicker inputArgDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label83;
        private System.Windows.Forms.DateTimePicker inputArgDateCreatedDateTimePicker;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.DateTimePicker inputStructureDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.DateTimePicker inputStructureDateCreatedDateTimePicker;
        private System.Windows.Forms.Label label86;
        private System.Windows.Forms.DateTimePicker inputTableDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label87;
        private System.Windows.Forms.DateTimePicker inputTableDateCreatedDateTimePicker;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.DateTimePicker inputFunctionDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.DateTimePicker inputFunctionDateCreatedDateTimePicker;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.DateTimePicker inputScalarDateUpdatedDateTimePicker;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.DateTimePicker inputScalarDateCreatedDateTimePicker;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.TextBox internalTableCellsMaxTextBox;
        private System.Windows.Forms.Label label93;
        private System.Windows.Forms.TextBox internalTableCellsMinTextBox;
        private System.Windows.Forms.Label label94;
        private System.Windows.Forms.TextBox internalScalarMaxTextBox;
        private System.Windows.Forms.Label label95;
        private System.Windows.Forms.TextBox internalScalarMinTextBox;
        private System.Windows.Forms.Label label96;
        private System.Windows.Forms.TextBox internalFunctionMaxInputTextBox;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.TextBox internalFunctionMinInputTextBox;
        private System.Windows.Forms.Label label98;
        private System.Windows.Forms.TextBox internalFunctionMaxOutputTextBox;
        private System.Windows.Forms.Label label99;
        private System.Windows.Forms.TextBox internalFunctionMinOutputTextBox;
    }
}