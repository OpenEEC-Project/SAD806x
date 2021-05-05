using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace SAD806x
{
    public partial class SigForm : Form
    {
        private const string TreeRootNodeName = "SIGNATURE";
        private const string TreeCategInputArgumentsNodeName = "INPARGUMENTS";
        private const string TreeCategInputStructuresNodeName = "INPSTRUCTURES";
        private const string TreeCategInputTablesNodeName = "INPTABLES";
        private const string TreeCategInputFunctionsNodeName = "INPFUNCTIONS";
        private const string TreeCategInputScalarsNodeName = "INPSCALARS";
        private const string TreeCategInternalStructuresNodeName = "INTSTRUCTURES";
        private const string TreeCategInternalTablesNodeName = "INTTABLES";
        private const string TreeCategInternalFunctionsNodeName = "INTFUNCTIONS";
        private const string TreeCategInternalScalarsNodeName = "INTSCALARS";

        private SADS6x S6x = null;
        private S6xSignature s6xSig = null;

        private S6xNavCategories s6xNavCategories = null;

        private SortedList slInternalStructures = null;
        private SortedList slInternalTables = null;
        private SortedList slInternalFunctions = null;
        private SortedList slInternalScalars = null;

        private SortedList slInputArguments = null;
        private SortedList slInputStructures = null;
        private SortedList slInputTables = null;
        private SortedList slInputFunctions = null;
        private SortedList slInputScalars = null;

        private TreeNode currentTreeNode = null;

        private Repository repoTables = null;
        private Repository repoFunctions = null;
        private Repository repoScalars = null;
        private Repository repoStructures = null;

        private Repository repoUnits = null;

        private RepositoryConversion repoConversion = null;

        private DialogResult closingDialogResult = DialogResult.Cancel;

        public SigForm(ref SADS6x s6x, ref S6xSignature sig, ref ImageList stateImageList, ref S6xNavCategories navCategories)
        {
            S6x = s6x;
            s6xSig = sig;
            s6xNavCategories = navCategories;

            slInternalStructures  = new SortedList();
            slInternalTables  = new SortedList();
            slInternalFunctions  = new SortedList();
            slInternalScalars  = new SortedList();

            slInputArguments = new SortedList();
            slInputStructures = new SortedList();
            slInputTables  = new SortedList();
            slInputFunctions  = new SortedList();
            slInputScalars  = new SortedList();

            if (s6xSig.InternalFunctions != null) foreach (S6xRoutineInternalFunction s6xObject in s6xSig.InternalFunctions) slInternalFunctions.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InternalScalars != null) foreach (S6xRoutineInternalScalar s6xObject in s6xSig.InternalScalars) slInternalScalars.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InternalStructures != null) foreach (S6xRoutineInternalStructure s6xObject in s6xSig.InternalStructures) slInternalStructures.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InternalTables != null) foreach (S6xRoutineInternalTable s6xObject in s6xSig.InternalTables) slInternalTables.Add(s6xObject.UniqueKey, s6xObject);

            if (s6xSig.InputArguments != null) foreach (S6xRoutineInputArgument s6xObject in s6xSig.InputArguments) slInputArguments.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InputFunctions != null) foreach (S6xRoutineInputFunction s6xObject in s6xSig.InputFunctions) slInputFunctions.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InputScalars != null) foreach (S6xRoutineInputScalar s6xObject in s6xSig.InputScalars) slInputScalars.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InputStructures != null) foreach (S6xRoutineInputStructure s6xObject in s6xSig.InputStructures) slInputStructures.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xSig.InputTables != null) foreach (S6xRoutineInputTable s6xObject in s6xSig.InputTables) slInputTables.Add(s6xObject.UniqueKey, s6xObject);

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            InitializeComponent();

            advElemsTreeView.StateImageList = stateImageList;
            if (advElemsTreeView.Nodes.ContainsKey(TreeRootNodeName))
            {
                foreach (TreeNode tnNode in advElemsTreeView.Nodes[TreeRootNodeName].Nodes)
                {
                    S6xNavHeaderCategory headerCateg = S6xNavHeaderCategory.UNDEFINED;
                    switch (tnNode.Name)
                    {
                        case TreeCategInputArgumentsNodeName:
                            headerCateg = S6xNavHeaderCategory.REGISTERS;
                            break;
                        case TreeCategInputStructuresNodeName:
                        case TreeCategInternalStructuresNodeName:
                            headerCateg = S6xNavHeaderCategory.STRUCTURES;
                            break;
                        case TreeCategInputTablesNodeName:
                        case TreeCategInternalTablesNodeName:
                            headerCateg = S6xNavHeaderCategory.TABLES;
                            break;
                        case TreeCategInputFunctionsNodeName:
                        case TreeCategInternalFunctionsNodeName:
                            headerCateg = S6xNavHeaderCategory.FUNCTIONS;
                            break;
                        case TreeCategInputScalarsNodeName:
                        case TreeCategInternalScalarsNodeName:
                            headerCateg = S6xNavHeaderCategory.SCALARS;
                            break;
                    }
                    if (headerCateg != S6xNavHeaderCategory.UNDEFINED) tnNode.StateImageKey = S6xNav.getS6xNavHeaderCategoryStateImageKey(headerCateg);
                }
            }
        }

        private void SigForm_Load(object sender, EventArgs e)
        {
            advElemsTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(advElemsTreeView_NodeMouseClick);
            advElemsTreeView.AfterSelect += new TreeViewEventHandler(advElemsTreeView_AfterSelect);

            advElemsContextMenuStrip.Opening += new CancelEventHandler(advElemsContextMenuStrip_Opening);

            advSigTextBox.TextChanged += new EventHandler(advSigTextBox_TextChanged);

            inputArgPositionTextBox.TextChanged += new EventHandler(inputArgPositionTextBox_TextChanged);

            inputArgEncryptionComboBox.DataSource = Enum.GetValues(typeof(CallArgsMode));
            inputArgEncryptionComboBox.SelectedItem = CallArgsMode.Standard;

            routineIdentificationStatusTrackBar.ValueChanged += new EventHandler(identificationStatusTrackBar_ValueChanged);
            internalStructureIdentificationStatusTrackBar.ValueChanged += new EventHandler(identificationStatusTrackBar_ValueChanged);
            internalTableIdentificationStatusTrackBar.ValueChanged += new EventHandler(identificationStatusTrackBar_ValueChanged);
            internalFunctionIdentificationStatusTrackBar.ValueChanged += new EventHandler(identificationStatusTrackBar_ValueChanged);
            internalScalarIdentificationStatusTrackBar.ValueChanged += new EventHandler(identificationStatusTrackBar_ValueChanged);

            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.ROUTINES, routineCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.ROUTINES, routineCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.ROUTINES, routineCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.STRUCTURES, internalStructureCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.STRUCTURES, internalStructureCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.STRUCTURES, internalStructureCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.TABLES, internalTableCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.TABLES, internalTableCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.TABLES, internalTableCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.FUNCTIONS, internalFunctionCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.FUNCTIONS, internalFunctionCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.FUNCTIONS, internalFunctionCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.SCALARS, internalScalarCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.SCALARS, internalScalarCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
            S6xNav.s6xNavCategoriesLoad(S6xNavHeaderCategory.SCALARS, internalScalarCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

            advLabelTextBox.Text = s6xSig.SignatureLabel;
            advSLabelTextBox.Text = s6xSig.UniqueKey;
            
            advSigTextBox.Text = s6xSig.Signature;
            advSigTextBox.Text = advSigTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

            routineSLabelTextBox.Text = s6xSig.ShortLabel;
            routineLabelTextBox.Text = s6xSig.Label;
            routineCommentsTextBox.Text = s6xSig.Comments;
            routineCommentsTextBox.Text = routineCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            routineOutputCommentsCheckBox.Checked = s6xSig.OutputComments;

            routineDateCreatedDateTimePicker.Value = Tools.getValidDateTime(s6xSig.RoutineDateCreated, S6x.Properties.DateCreated).ToLocalTime();
            routineDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(s6xSig.RoutineDateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

            if (s6xSig.RoutineCategory == null) routineCategComboBox.Text = string.Empty;
            else routineCategComboBox.Text = s6xSig.RoutineCategory;
            if (s6xSig.RoutineCategory2 == null) routineCateg2ComboBox.Text = string.Empty;
            else routineCateg2ComboBox.Text = s6xSig.RoutineCategory2;
            if (s6xSig.RoutineCategory3 == null) routineCateg3ComboBox.Text = string.Empty;
            else routineCateg3ComboBox.Text = s6xSig.RoutineCategory3;

            if (s6xSig.RoutineIdentificationStatus < 0) routineIdentificationStatusTrackBar.Value = 0;
            else if (s6xSig.RoutineIdentificationStatus > 100) routineIdentificationStatusTrackBar.Value = 100;
            else routineIdentificationStatusTrackBar.Value = s6xSig.RoutineIdentificationStatus;

            // Windows 10 1809 (10.0.17763) Issue
            routineIdentificationDetailsTextBox.Clear();
            routineIdentificationDetailsTextBox.Multiline = false;
            routineIdentificationDetailsTextBox.Multiline = true;

            if (s6xSig.RoutineIdentificationDetails == null) routineIdentificationDetailsTextBox.Text = string.Empty;
            else routineIdentificationDetailsTextBox.Text = s6xSig.RoutineIdentificationDetails;
            routineIdentificationDetailsTextBox.Text = routineIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

            if (signatureFor806xComboBox.Items.Count == Enum.GetValues(typeof(Signature806xOptions)).Length)
            {
                if (s6xSig.for806x != null && s6xSig.for806x != string.Empty)
                {
                    try { signatureFor806xComboBox.SelectedIndex = (int)Enum.Parse(typeof(Signature806xOptions), s6xSig.for806x, true); }
                    catch { signatureFor806xComboBox.SelectedIndex = (int)Signature806xOptions.Undefined; }
                }
                else
                {
                    signatureFor806xComboBox.SelectedIndex = (int)Signature806xOptions.Undefined;
                }
            }
            if (signatureForBankComboBox.Items.Count == Enum.GetValues(typeof(SignatureBankOptions)).Length)
            {
                if (s6xSig.forBankNum != null && s6xSig.forBankNum != string.Empty)
                {
                    try { signatureForBankComboBox.SelectedIndex = (int)Enum.Parse(typeof(SignatureBankOptions), s6xSig.forBankNum, true); }
                    catch { signatureForBankComboBox.SelectedIndex = (int)SignatureBankOptions.Undefined; }
                }
                else
                {
                    signatureForBankComboBox.SelectedIndex = (int)SignatureBankOptions.Undefined;
                }
            }

            mainTipPictureBox.Tag = SharedUI.SignatureTip();
            mainTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            mainTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            inputStructureTipPictureBox.Tag = SharedUI.StructureTip();
            inputStructureTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            inputStructureTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            internalStructureTipPictureBox.Tag = SharedUI.StructureTip();
            internalStructureTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            internalStructureTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            repoContextMenuStrip.Opening += new CancelEventHandler(repoContextMenuStrip_Opening);
            repoToolStripTextBox.TextChanged += new EventHandler(repoToolStripTextBox_TextChanged);
            repoToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(repoToolStripMenuItem_DropDownItemClicked);

            inputScalarScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            inputScalarScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            inputScalarScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            inputFunctionScalePrecInputNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            inputFunctionScalePrecInputNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            inputFunctionScalePrecInputNumericUpDown.Value = SADDef.DefaultScalePrecision;
            inputFunctionScalePrecOutputNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            inputFunctionScalePrecOutputNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            inputFunctionScalePrecOutputNumericUpDown.Value = SADDef.DefaultScalePrecision;

            inputTableScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            inputTableScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            inputTableScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            internalScalarScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            internalScalarScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            internalScalarScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            internalFunctionScalePrecInputNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            internalFunctionScalePrecInputNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            internalFunctionScalePrecInputNumericUpDown.Value = SADDef.DefaultScalePrecision;
            internalFunctionScalePrecOutputNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            internalFunctionScalePrecOutputNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            internalFunctionScalePrecOutputNumericUpDown.Value = SADDef.DefaultScalePrecision;

            internalTableScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            internalTableScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            internalTableScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            Control.ControlCollection controls = null;
            controls = (Control.ControlCollection)routineTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputArgumentTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputStructureTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputTableTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputFunctionTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputScalarTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)internalStructureTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)internalTableTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)internalFunctionTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)internalScalarTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = null;

            initRepositories();

            loadElemsTreeView();
            foreach (TreeNode tnCateg in advElemsTreeView.Nodes[TreeRootNodeName].Nodes)
            {
                if (tnCateg.Nodes.Count > 0)
                {
                    advElemsTreeView.SelectedNode = tnCateg.Nodes[0];
                    return;
                }
            }
            clearElem();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = closingDialogResult;
        }

        private void attachPropertiesEventsControls(ref Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control.GetType().Name)
                {
                    case "TextBox":
                        ((TextBox)control).KeyDown += new KeyEventHandler(textBox_KeyDown);
                        break;
                }
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // UpperCase / LowerCase
                case Keys.U:
                    if (e.Control && e.Shift)
                    {
                        if (((TextBox)sender).SelectedText != string.Empty) ((TextBox)sender).SelectedText = ((TextBox)sender).SelectedText.ToUpper();
                    }
                    else if (e.Control)
                    {
                        if (((TextBox)sender).SelectedText != string.Empty) ((TextBox)sender).SelectedText = ((TextBox)sender).SelectedText.ToLower();
                    }
                    break;
                // Repository
                case Keys.R:
                    if (e.Control) showRepository((Control)sender);
                    break;
            }
        }

        private void TipPictureBox_MouseHover(object sender, EventArgs e)
        {
            if (((Control)sender).Tag == null) return;

            mainToolTip.SetToolTip((Control)sender, ((Control)sender).Tag.ToString());
        }

        private void TipPictureBox_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag == null) return;

            MessageBox.Show(((PictureBox)sender).Tag.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void advElemsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void advElemsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                currentTreeNode = null;
                return;
            }

            currentTreeNode = e.Node;

            clearElem();

            if (currentTreeNode.Parent.Name == TreeRootNodeName) return;

            showElem();
        }

        private void advElemsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (currentTreeNode == null)
            {
                e.Cancel = true;
                return;
            }

            if (currentTreeNode.Parent == null)
            {
                e.Cancel = true;
                return;
            }

            if (currentTreeNode.Parent.Name == TreeRootNodeName)
            {
                newElementToolStripMenuItem.Visible = true;
                delElementToolStripMenuItem.Visible = false;
            }
            else
            {
                newElementToolStripMenuItem.Visible = false;
                delElementToolStripMenuItem.Visible = true;
            }
        }

        private void newElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newElem();
        }

        private void delElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delElem();
        }

        private void elemUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void advSigTextBox_TextChanged(object sender, EventArgs e)
        {
            s6xSig.Signature = advSigTextBox.Text;

            closingDialogResult = DialogResult.OK;
        }

        private void inputArgPositionTextBox_TextChanged(object sender, EventArgs e)
        {
            inputArgCodeTextBox.Text = Tools.ArgumentCode(inputArgPositionTextBox.Text);
        }

        private void identificationStatusTrackBar_ValueChanged(object sender, EventArgs e)
        {
            TrackBar tbTB = (TrackBar)sender;
            Label lbLB = null;

            if (tbTB == routineIdentificationStatusTrackBar) lbLB = routineIdentificationLabel;
            else if (tbTB == internalStructureIdentificationStatusTrackBar) lbLB = internalStructureIdentificationLabel;
            else if (tbTB == internalTableIdentificationStatusTrackBar) lbLB = internalTableIdentificationLabel;
            else if (tbTB == internalFunctionIdentificationStatusTrackBar) lbLB = internalFunctionIdentificationLabel;
            else if (tbTB == internalScalarIdentificationStatusTrackBar) lbLB = internalScalarIdentificationLabel;

            if (lbLB == null) return;

            lbLB.Text = string.Format("{0} ({1:d2}%)", lbLB.Tag, tbTB.Value);
        }

        private void loadElemsTreeView()
        {
            TreeNode tnNode = null;

            string defaultStateImageKey = S6xNav.getIdentificationStatusStateImageKey(100);

            foreach (S6xRoutineInputArgument s6xObject in slInputArguments.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Position.ToString() + " - " + s6xObject.Code;
                tnNode.StateImageKey = defaultStateImageKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputArgumentsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputStructure s6xObject in slInputStructures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                tnNode.StateImageKey = defaultStateImageKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputStructuresNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputTable s6xObject in slInputTables.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                tnNode.StateImageKey = defaultStateImageKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputTablesNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputFunction s6xObject in slInputFunctions.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                tnNode.StateImageKey = defaultStateImageKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputFunctionsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputScalar s6xObject in slInputScalars.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                tnNode.StateImageKey = defaultStateImageKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputScalarsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }

            foreach (S6xRoutineInternalStructure s6xObject in slInternalStructures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xObject.IdentificationStatus);
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalStructuresNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInternalTable s6xObject in slInternalTables.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xObject.IdentificationStatus);
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalTablesNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInternalFunction s6xObject in slInternalFunctions.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xObject.IdentificationStatus);
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalFunctionsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInternalScalar s6xObject in slInternalScalars.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xObject.IdentificationStatus);
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalScalarsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }

            advElemsTreeView.ExpandAll();

        }

        private void updateArray(string categ)
        {
            switch (categ)
            {
                case TreeCategInputArgumentsNodeName:
                    if (slInputArguments.Count == 0) s6xSig.InputArguments = null;
                    else
                    {
                        s6xSig.InputArguments = new S6xRoutineInputArgument[slInputArguments.Count];
                        slInputArguments.Values.CopyTo(s6xSig.InputArguments, 0);
                    }
                    break;
                case TreeCategInputStructuresNodeName:
                    if (slInputStructures.Count == 0) s6xSig.InputStructures = null;
                    else
                    {
                        s6xSig.InputStructures = new S6xRoutineInputStructure[slInputStructures.Count];
                        slInputStructures.Values.CopyTo(s6xSig.InputStructures, 0);
                    }
                    break;
                case TreeCategInputTablesNodeName:
                    if (slInputTables.Count == 0) s6xSig.InputTables = null;
                    else
                    {
                    s6xSig.InputTables = new S6xRoutineInputTable[slInputTables.Count];
                    slInputTables.Values.CopyTo(s6xSig.InputTables, 0);
                    }
                    break;
                case TreeCategInputFunctionsNodeName:
                    if (slInputFunctions.Count == 0) s6xSig.InputFunctions = null;
                    else
                    {
                    s6xSig.InputFunctions = new S6xRoutineInputFunction[slInputFunctions.Count];
                    slInputFunctions.Values.CopyTo(s6xSig.InputFunctions, 0);
                    }
                    break;
                case TreeCategInputScalarsNodeName:
                    if (slInputScalars.Count == 0) s6xSig.InputScalars = null;
                    else
                    {
                    s6xSig.InputScalars = new S6xRoutineInputScalar[slInputScalars.Count];
                    slInputScalars.Values.CopyTo(s6xSig.InputScalars, 0);
                    }
                    break;
                case TreeCategInternalStructuresNodeName:
                    if (slInternalStructures.Count == 0) s6xSig.InternalStructures = null;
                    else
                    {
                    s6xSig.InternalStructures = new S6xRoutineInternalStructure[slInternalStructures.Count];
                    slInternalStructures.Values.CopyTo(s6xSig.InternalStructures, 0);
                    }
                    break;
                case TreeCategInternalTablesNodeName:
                    if (slInternalTables.Count == 0) s6xSig.InternalTables = null;
                    else
                    {
                    s6xSig.InternalTables = new S6xRoutineInternalTable[slInternalTables.Count];
                    slInternalTables.Values.CopyTo(s6xSig.InternalTables, 0);
                    }
                    break;
                case TreeCategInternalFunctionsNodeName:
                    if (slInternalFunctions.Count == 0) s6xSig.InternalFunctions = null;
                    else
                    {
                    s6xSig.InternalFunctions = new S6xRoutineInternalFunction[slInternalFunctions.Count];
                    slInternalFunctions.Values.CopyTo(s6xSig.InternalFunctions, 0);
                    }
                    break;
                case TreeCategInternalScalarsNodeName:
                    if (slInternalScalars.Count == 0) s6xSig.InternalScalars = null;
                    else
                    {
                        s6xSig.InternalScalars = new S6xRoutineInternalScalar[slInternalScalars.Count];
                        slInternalScalars.Values.CopyTo(s6xSig.InternalScalars, 0);
                    }
                    break;
                default:
                    return;
            }
        }
        
        private void clearElem()
        {
            foreach (TabPage tabPage in elemTabControl.TabPages)
            {
                // Not for routine
                if (tabPage == routineTabPage) continue;

                Control.ControlCollection controls = (Control.ControlCollection)tabPage.Controls;
                clearControls(ref controls);
            }
        }

        private void clearControls(ref Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control.GetType().Name)
                {
                    case "TextBox":
                        switch (control.Name)
                        {
                            case "internalStructureNumTextBox":
                            case "internalTableColsTextBox":
                            case "internalFunctionRowsTextBox":
                                ((TextBox)control).Text = "0";
                                break;
                            default:
                                ((TextBox)control).Text = string.Empty;
                                break;
                        }
                        break;
                    case "CheckBox":
                        if (((CheckBox)control).ThreeState) ((CheckBox)control).CheckState = CheckState.Indeterminate;
                        else ((CheckBox)control).Checked = false;
                        break;
                    case "ComboBox":
                        switch (control.Name)
                        {
                            case "inputArgEncryptionComboBox":
                                ((ComboBox)control).SelectedItem = CallArgsMode.Standard;
                                break;
                            default:
                                ((ComboBox)control).SelectedItem = null;
                                break;
                        }
                        break;
                    case "NumericUpDown":
                        ((NumericUpDown)control).Value = ((NumericUpDown)control).Minimum;
                        break;
                    case "DateTimePicker":
                        ((DateTimePicker)control).Value = DateTime.Now;
                        break;
                    case "TrackBar":
                        ((TrackBar)control).Value = ((TrackBar)control).Minimum;
                        break;
                    case "Button":
                        control.Tag = null;
                        break;
                }
            }
        }

        private void showCateg(string categ)
        {
            switch (categ)
            {
                case TreeCategInputArgumentsNodeName:
                    elemTabControl.SelectedTab = inputArgumentTabPage;
                    break;
                case TreeCategInputStructuresNodeName:
                    elemTabControl.SelectedTab = inputStructureTabPage;
                    break;
                case TreeCategInputTablesNodeName:
                    elemTabControl.SelectedTab = inputTableTabPage;
                    break;
                case TreeCategInputFunctionsNodeName:
                    elemTabControl.SelectedTab = inputFunctionTabPage;
                    break;
                case TreeCategInputScalarsNodeName:
                    elemTabControl.SelectedTab = inputScalarTabPage;
                    break;
                case TreeCategInternalStructuresNodeName:
                    elemTabControl.SelectedTab = internalStructureTabPage;
                    break;
                case TreeCategInternalTablesNodeName:
                    elemTabControl.SelectedTab = internalTableTabPage;
                    break;
                case TreeCategInternalFunctionsNodeName:
                    elemTabControl.SelectedTab = internalFunctionTabPage;
                    break;
                case TreeCategInternalScalarsNodeName:
                    elemTabControl.SelectedTab = internalScalarTabPage;
                    break;
                default:
                    return;
            }
        }

        private void showElem()
        {
            if (currentTreeNode == null) return;
            if (currentTreeNode.Parent == null) return;

            switch (currentTreeNode.Parent.Name)
            {
                case TreeCategInputArgumentsNodeName:
                    S6xRoutineInputArgument sigInpArg = (S6xRoutineInputArgument)slInputArguments[currentTreeNode.Name];
                    inputArgCodeTextBox.Text = sigInpArg.Code;
                    inputArgPositionTextBox.Text = sigInpArg.Position.ToString();
                    inputArgWordCheckBox.Checked = sigInpArg.Word;
                    inputArgPointerCheckBox.Checked = sigInpArg.Pointer;
                    try
                    {
                        inputArgEncryptionComboBox.SelectedItem = (CallArgsMode)sigInpArg.Encryption;
                    }
                    catch { }

                    inputArgDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigInpArg.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    inputArgDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigInpArg.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    sigInpArg = null;
                    break;
                case TreeCategInputStructuresNodeName:
                    S6xRoutineInputStructure sigInpStr = (S6xRoutineInputStructure)slInputStructures[currentTreeNode.Name];
                    inputStructureAddrTextBox.Text = sigInpStr.VariableAddress;
                    inputStructureNumFixTextBox.Text = sigInpStr.ForcedNumber;
                    inputStructureNumRegTextBox.Text = sigInpStr.VariableNumber;

                    // Windows 10 1809 (10.0.17763) Issue
                    inputStructureStructTextBox.Clear();
                    inputStructureStructTextBox.Multiline = false;
                    inputStructureStructTextBox.Multiline = true;

                    inputStructureStructTextBox.Text = sigInpStr.StructDef;

                    inputStructureDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigInpStr.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    inputStructureDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigInpStr.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    sigInpStr = null;
                    break;
                case TreeCategInputTablesNodeName:
                    S6xRoutineInputTable sigInpTbl = (S6xRoutineInputTable)slInputTables[currentTreeNode.Name];
                    inputTableAddrTextBox.Text = sigInpTbl.VariableAddress;
                    inputTableCellsUnitsTextBox.Text = sigInpTbl.ForcedCellsUnits;
                    inputTableColsNumFixTextBox.Text = sigInpTbl.ForcedColsNumber;
                    inputTableColsNumRegTextBox.Text = sigInpTbl.VariableColsNumberReg;
                    inputTableColsRegTextBox.Text = sigInpTbl.VariableColsReg;
                    inputTableColsUnitsTextBox.Text = sigInpTbl.ForcedColsUnits;
                    inputTableOutputTextBox.Text = sigInpTbl.VariableOutput;
                    inputTableRowsNumTextBox.Text = sigInpTbl.ForcedRowsNumber;
                    inputTableRowsRegTextBox.Text = sigInpTbl.VariableRowsReg;
                    inputTableRowsUnitsTextBox.Text = sigInpTbl.ForcedRowsUnits;
                    inputTableScaleTextBox.Text = sigInpTbl.ForcedCellsScaleExpression;
                    inputTableScalePrecNumericUpDown.Value = sigInpTbl.ForcedCellsScalePrecision;
                    inputTableSignedCheckBox.Checked = sigInpTbl.SignedOutput;
                    inputTableWordCheckBox.Checked = sigInpTbl.WordOutput;

                    inputTableDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigInpTbl.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    inputTableDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigInpTbl.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();
                    
                    sigInpTbl = null;
                    break;
                case TreeCategInputFunctionsNodeName:
                    S6xRoutineInputFunction sigInpFunc = (S6xRoutineInputFunction)slInputFunctions[currentTreeNode.Name];
                    inputFunctionAddrTextBox.Text = sigInpFunc.VariableAddress;
                    inputFunctionByteCheckBox.Checked = sigInpFunc.ByteInput;
                    inputFunctionInputTextBox.Text = sigInpFunc.VariableInput;
                    inputFunctionOutputTextBox.Text = sigInpFunc.VariableOutput;
                    inputFunctionRowsTextBox.Text = sigInpFunc.ForcedRowsNumber;
                    inputFunctionScaleInputTextBox.Text = sigInpFunc.ForcedInputScaleExpression;
                    inputFunctionScaleOutputTextBox.Text = sigInpFunc.ForcedOutputScaleExpression;
                    inputFunctionScalePrecInputNumericUpDown.Value = sigInpFunc.ForcedInputScalePrecision;
                    inputFunctionScalePrecOutputNumericUpDown.Value = sigInpFunc.ForcedOutputScalePrecision;
                    inputFunctionSignedInputCheckBox.Checked = sigInpFunc.SignedInput;
                    inputFunctionSignedOutputCheckBox.Checked = sigInpFunc.SignedOutput;
                    inputFunctionUnitsInputTextBox.Text = sigInpFunc.ForcedInputUnits;
                    inputFunctionUnitsOutputTextBox.Text = sigInpFunc.ForcedOutputUnits;

                    inputFunctionDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigInpFunc.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    inputFunctionDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigInpFunc.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();
                    
                    sigInpFunc = null;
                    break;
                case TreeCategInputScalarsNodeName:
                    S6xRoutineInputScalar sigInpScal = (S6xRoutineInputScalar)slInputScalars[currentTreeNode.Name];
                    inputScalarAddrTextBox.Text = sigInpScal.VariableAddress;
                    inputScalarByteCheckBox.Checked = sigInpScal.Byte;
                    inputScalarScaleTextBox.Text = sigInpScal.ForcedScaleExpression;
                    inputScalarScalePrecNumericUpDown.Value = sigInpScal.ForcedScalePrecision;
                    inputScalarSignedCheckBox.Checked = sigInpScal.Signed;
                    inputScalarUnitsTextBox.Text = sigInpScal.ForcedUnits;

                    inputScalarDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigInpScal.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    inputScalarDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigInpScal.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    sigInpScal = null;
                    break;
                case TreeCategInternalStructuresNodeName:
                    S6xRoutineInternalStructure sigIntStr = (S6xRoutineInternalStructure)slInternalStructures[currentTreeNode.Name];
                    internalStructureAddrTextBox.Text = sigIntStr.VariableAddress;
                    internalStructureBankTextBox.Text = sigIntStr.VariableBankNum;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalStructureCommentsTextBox.Clear();
                    internalStructureCommentsTextBox.Multiline = false;
                    internalStructureCommentsTextBox.Multiline = true;
                    
                    internalStructureCommentsTextBox.Text = sigIntStr.Comments;
                    internalStructureLabelTextBox.Text = sigIntStr.Label;
                    internalStructureNumTextBox.Text = sigIntStr.Number.ToString();
                    internalStructureOutputCommentsCheckBox.Checked = sigIntStr.OutputComments;
                    internalStructureSLabelTextBox.Text = sigIntStr.ShortLabel;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalStructureStructTextBox.Clear();
                    internalStructureStructTextBox.Multiline = false;
                    internalStructureStructTextBox.Multiline = true;
                    
                    internalStructureStructTextBox.Text = sigIntStr.StructDef;

                    internalStructureDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigIntStr.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    internalStructureDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigIntStr.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    if (sigIntStr.Category == null) internalStructureCategComboBox.Text = string.Empty;
                    else internalStructureCategComboBox.Text = sigIntStr.Category;
                    if (sigIntStr.Category2 == null) internalStructureCateg2ComboBox.Text = string.Empty;
                    else internalStructureCateg2ComboBox.Text = sigIntStr.Category2;
                    if (sigIntStr.Category3 == null) internalStructureCateg3ComboBox.Text = string.Empty;
                    else internalStructureCateg3ComboBox.Text = sigIntStr.Category3;

                    if (sigIntStr.IdentificationStatus < 0) internalStructureIdentificationStatusTrackBar.Value = 0;
                    else if (sigIntStr.IdentificationStatus > 100) internalStructureIdentificationStatusTrackBar.Value = 100;
                    else internalStructureIdentificationStatusTrackBar.Value = sigIntStr.IdentificationStatus;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalStructureIdentificationDetailsTextBox.Clear();
                    internalStructureIdentificationDetailsTextBox.Multiline = false;
                    internalStructureIdentificationDetailsTextBox.Multiline = true;

                    if (sigIntStr.IdentificationDetails == null) internalStructureIdentificationDetailsTextBox.Text = string.Empty;
                    else internalStructureIdentificationDetailsTextBox.Text = sigIntStr.IdentificationDetails;
                    internalStructureIdentificationDetailsTextBox.Text = internalStructureIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
                    
                    sigIntStr = null;
                    break;
                case TreeCategInternalTablesNodeName:
                    S6xRoutineInternalTable sigIntTbl = (S6xRoutineInternalTable)slInternalTables[currentTreeNode.Name];
                    internalTableAddrTextBox.Text = sigIntTbl.VariableAddress;
                    internalTableBankTextBox.Text = sigIntTbl.VariableBankNum;
                    internalTableCellsUnitsTextBox.Text = sigIntTbl.CellsUnits;
                    internalTableColsTextBox.Text = sigIntTbl.VariableColsNumber;
                    internalTableColsUnitsTextBox.Text = sigIntTbl.ColsUnits;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalTableCommentsTextBox.Clear();
                    internalTableCommentsTextBox.Multiline = false;
                    internalTableCommentsTextBox.Multiline = true;
                    
                    internalTableCommentsTextBox.Text = sigIntTbl.Comments;
                    internalTableLabelTextBox.Text = sigIntTbl.Label;
                    internalTableOutputCommentsCheckBox.Checked = sigIntTbl.OutputComments;
                    internalTableRowsTextBox.Text = sigIntTbl.RowsNumber.ToString();
                    internalTableRowsUnitsTextBox.Text = sigIntTbl.RowsUnits;
                    internalTableScaleTextBox.Text = sigIntTbl.CellsScaleExpression;
                    internalTableScalePrecNumericUpDown.Value = sigIntTbl.CellsScalePrecision;
                    internalTableSignedCheckBox.Checked = sigIntTbl.SignedOutput;
                    internalTableSLabelTextBox.Text = sigIntTbl.ShortLabel;
                    internalTableWordCheckBox.Checked = sigIntTbl.WordOutput;

                    internalTableCellsMinTextBox.Text = sigIntTbl.CellsMin;
                    internalTableCellsMaxTextBox.Text = sigIntTbl.CellsMax;

                    internalTableDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigIntTbl.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    internalTableDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigIntTbl.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    if (sigIntTbl.Category == null) internalTableCategComboBox.Text = string.Empty;
                    else internalTableCategComboBox.Text = sigIntTbl.Category;
                    if (sigIntTbl.Category2 == null) internalTableCateg2ComboBox.Text = string.Empty;
                    else internalTableCateg2ComboBox.Text = sigIntTbl.Category2;
                    if (sigIntTbl.Category3 == null) internalTableCateg3ComboBox.Text = string.Empty;
                    else internalTableCateg3ComboBox.Text = sigIntTbl.Category3;

                    if (sigIntTbl.IdentificationStatus < 0) internalTableIdentificationStatusTrackBar.Value = 0;
                    else if (sigIntTbl.IdentificationStatus > 100) internalTableIdentificationStatusTrackBar.Value = 100;
                    else internalTableIdentificationStatusTrackBar.Value = sigIntTbl.IdentificationStatus;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalTableIdentificationDetailsTextBox.Clear();
                    internalTableIdentificationDetailsTextBox.Multiline = false;
                    internalTableIdentificationDetailsTextBox.Multiline = true;

                    if (sigIntTbl.IdentificationDetails == null) internalTableIdentificationDetailsTextBox.Text = string.Empty;
                    else internalTableIdentificationDetailsTextBox.Text = sigIntTbl.IdentificationDetails;
                    internalTableIdentificationDetailsTextBox.Text = internalTableIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
                    
                    sigIntTbl = null;
                    break;
                case TreeCategInternalFunctionsNodeName:
                    S6xRoutineInternalFunction sigIntFunc = (S6xRoutineInternalFunction)slInternalFunctions[currentTreeNode.Name];
                    internalFunctionAddrTextBox.Text = sigIntFunc.VariableAddress;
                    internalFunctionBankTextBox.Text = sigIntFunc.VariableBankNum;
                    internalFunctionByteCheckBox.Checked = sigIntFunc.ByteInput;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalFunctionCommentsTextBox.Clear();
                    internalFunctionCommentsTextBox.Multiline = false;
                    internalFunctionCommentsTextBox.Multiline = true;
                    
                    internalFunctionCommentsTextBox.Text = sigIntFunc.Comments;
                    internalFunctionLabelTextBox.Text = sigIntFunc.Label;
                    internalFunctionOutputCommentsCheckBox.Checked = sigIntFunc.OutputComments;
                    internalFunctionRowsTextBox.Text = sigIntFunc.RowsNumber.ToString();
                    internalFunctionScaleInputTextBox.Text = sigIntFunc.InputScaleExpression;
                    internalFunctionScaleOutputTextBox.Text = sigIntFunc.OutputScaleExpression;
                    internalFunctionScalePrecInputNumericUpDown.Value = sigIntFunc.InputScalePrecision;
                    internalFunctionScalePrecOutputNumericUpDown.Value = sigIntFunc.OutputScalePrecision;
                    internalFunctionSignedInputCheckBox.Checked = sigIntFunc.SignedInput;
                    internalFunctionSignedOutputCheckBox.Checked = sigIntFunc.SignedOutput;
                    internalFunctionSLabelTextBox.Text = sigIntFunc.ShortLabel;
                    internalFunctionUnitsInputTextBox.Text = sigIntFunc.InputUnits;
                    internalFunctionUnitsOutputTextBox.Text = sigIntFunc.OutputUnits;

                    internalFunctionMinInputTextBox.Text = sigIntFunc.InputMin;
                    internalFunctionMaxInputTextBox.Text = sigIntFunc.InputMax;
                    internalFunctionMinOutputTextBox.Text = sigIntFunc.OutputMin;
                    internalFunctionMaxOutputTextBox.Text = sigIntFunc.OutputMax;

                    internalFunctionDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigIntFunc.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    internalFunctionDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigIntFunc.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    if (sigIntFunc.Category == null) internalFunctionCategComboBox.Text = string.Empty;
                    else internalFunctionCategComboBox.Text = sigIntFunc.Category;
                    if (sigIntFunc.Category2 == null) internalFunctionCateg2ComboBox.Text = string.Empty;
                    else internalFunctionCateg2ComboBox.Text = sigIntFunc.Category2;
                    if (sigIntFunc.Category3 == null) internalFunctionCateg3ComboBox.Text = string.Empty;
                    else internalFunctionCateg3ComboBox.Text = sigIntFunc.Category3;

                    if (sigIntFunc.IdentificationStatus < 0) internalFunctionIdentificationStatusTrackBar.Value = 0;
                    else if (sigIntFunc.IdentificationStatus > 100) internalFunctionIdentificationStatusTrackBar.Value = 100;
                    else internalFunctionIdentificationStatusTrackBar.Value = sigIntFunc.IdentificationStatus;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalFunctionIdentificationDetailsTextBox.Clear();
                    internalFunctionIdentificationDetailsTextBox.Multiline = false;
                    internalFunctionIdentificationDetailsTextBox.Multiline = true;

                    if (sigIntFunc.IdentificationDetails == null) internalFunctionIdentificationDetailsTextBox.Text = string.Empty;
                    else internalFunctionIdentificationDetailsTextBox.Text = sigIntFunc.IdentificationDetails;
                    internalFunctionIdentificationDetailsTextBox.Text = internalFunctionIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

                    sigIntFunc = null;
                    break;
                case TreeCategInternalScalarsNodeName:
                    S6xRoutineInternalScalar sigIntScal = (S6xRoutineInternalScalar)slInternalScalars[currentTreeNode.Name];
                    internalScalarAddrTextBox.Text = sigIntScal.VariableAddress;
                    internalScalarBankTextBox.Text = sigIntScal.VariableBankNum;
                    internalScalarByteCheckBox.Checked = sigIntScal.Byte;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalScalarCommentsTextBox.Clear();
                    internalScalarCommentsTextBox.Multiline = false;
                    internalScalarCommentsTextBox.Multiline = true;
                    
                    internalScalarCommentsTextBox.Text = sigIntScal.Comments;
                    internalScalarLabelTextBox.Text = sigIntScal.Label;
                    internalScalarOutputCommentsCheckBox.Checked = sigIntScal.OutputComments;
                    internalScalarInlineCommentsCheckBox.Checked = sigIntScal.InlineComments;
                    internalScalarScaleTextBox.Text = sigIntScal.ScaleExpression;
                    internalScalarScalePrecNumericUpDown.Value = sigIntScal.ScalePrecision;
                    internalScalarSignedCheckBox.Checked = sigIntScal.Signed;
                    internalScalarSLabelTextBox.Text = sigIntScal.ShortLabel;
                    internalScalarUnitsTextBox.Text = sigIntScal.Units;
                    internalScalarBitFlagsCheckBox.Checked = sigIntScal.isBitFlags;

                    internalScalarMinTextBox.Text = sigIntScal.Min;
                    internalScalarMaxTextBox.Text = sigIntScal.Max;

                    internalScalarBitFlagsButton.Tag = null;

                    internalScalarDateCreatedDateTimePicker.Value = Tools.getValidDateTime(sigIntScal.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    internalScalarDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(sigIntScal.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    if (sigIntScal.Category == null) internalScalarCategComboBox.Text = string.Empty;
                    else internalScalarCategComboBox.Text = sigIntScal.Category;
                    if (sigIntScal.Category2 == null) internalScalarCateg2ComboBox.Text = string.Empty;
                    else internalScalarCateg2ComboBox.Text = sigIntScal.Category2;
                    if (sigIntScal.Category3 == null) internalScalarCateg3ComboBox.Text = string.Empty;
                    else internalScalarCateg3ComboBox.Text = sigIntScal.Category3;

                    if (sigIntScal.IdentificationStatus < 0) internalScalarIdentificationStatusTrackBar.Value = 0;
                    else if (sigIntScal.IdentificationStatus > 100) internalScalarIdentificationStatusTrackBar.Value = 100;
                    else internalScalarIdentificationStatusTrackBar.Value = sigIntScal.IdentificationStatus;

                    // Windows 10 1809 (10.0.17763) Issue
                    internalScalarIdentificationDetailsTextBox.Clear();
                    internalScalarIdentificationDetailsTextBox.Multiline = false;
                    internalScalarIdentificationDetailsTextBox.Multiline = true;

                    if (sigIntScal.IdentificationDetails == null) internalScalarIdentificationDetailsTextBox.Text = string.Empty;
                    else internalScalarIdentificationDetailsTextBox.Text = sigIntScal.IdentificationDetails;
                    internalScalarIdentificationDetailsTextBox.Text = internalScalarIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
                    
                    sigIntScal = null;
                    break;
                default:
                    return;
            }

            showCateg(currentTreeNode.Parent.Name);
        }

        private void newElem()
        {
            if (currentTreeNode == null) return;
            if (currentTreeNode.Parent == null) return;

            clearElem();

            switch (currentTreeNode.Parent.Name)
            {
                case TreeRootNodeName:
                    showCateg(currentTreeNode.Name);
                    break;
                default:
                    showCateg(currentTreeNode.Parent.Name);
                    break;
            }
        }

        private void delElem()
        {
            if (currentTreeNode == null) return;
            if (currentTreeNode.Parent == null) return;

            clearElem();

            switch (currentTreeNode.Parent.Name)
            {
                case TreeCategInputArgumentsNodeName:
                    slInputArguments.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInputStructuresNodeName:
                    slInputStructures.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInputTablesNodeName:
                    slInputTables.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInputFunctionsNodeName:
                    slInputFunctions.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInputScalarsNodeName:
                    slInputScalars.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInternalStructuresNodeName:
                    slInternalStructures.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInternalTablesNodeName:
                    slInternalTables.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInternalFunctionsNodeName:
                    slInternalFunctions.Remove(currentTreeNode.Name);
                    break;
                case TreeCategInternalScalarsNodeName:
                    slInternalScalars.Remove(currentTreeNode.Name);
                    break;
                default:
                    return;
            }

            updateArray(currentTreeNode.Parent.Name);

            closingDialogResult = DialogResult.OK;

            currentTreeNode.Parent.Nodes.Remove(currentTreeNode);
            currentTreeNode = null;
        }

        private void updateElem()
        {
            string categ = string.Empty;
            string uniqueKey = string.Empty;
            string label = string.Empty;
            string comments = string.Empty;
            int identificationStatus = 100; // Maximum by default (for non internal elements)

            switch (elemTabControl.SelectedTab.Name)
            {
                case "routineTabPage":
                    updateRoutine();
                    return;
                case "inputArgumentTabPage":
                    categ = TreeCategInputArgumentsNodeName;
                    break;
                case "inputStructureTabPage":
                    categ = TreeCategInputStructuresNodeName;
                    break;
                case "inputTableTabPage":
                    categ = TreeCategInputTablesNodeName;
                    break;
                case "inputFunctionTabPage":
                    categ = TreeCategInputFunctionsNodeName;
                    break;
                case "inputScalarTabPage":
                    categ = TreeCategInputScalarsNodeName;
                    break;
                case "internalStructureTabPage":
                    categ = TreeCategInternalStructuresNodeName;
                    break;
                case "internalTableTabPage":
                    categ = TreeCategInternalTablesNodeName;
                    break;
                case "internalFunctionTabPage":
                    categ = TreeCategInternalFunctionsNodeName;
                    break;
                case "internalScalarTabPage":
                    categ = TreeCategInternalScalarsNodeName;
                    break;
                default:
                    return;
            }

            if (!checkElem(categ))
            {
                MessageBox.Show("Invalid values are present, please correct them to continue.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (currentTreeNode != null)
            {
                if (currentTreeNode.Parent != null)
                {
                    if (currentTreeNode.Parent.Name != categ) currentTreeNode = null;
                }
            }
            
            switch (categ)
            {
                case TreeCategInputArgumentsNodeName:
                    S6xRoutineInputArgument sigInpArg = null;
                    if (currentTreeNode == null)
                    {
                        sigInpArg = new S6xRoutineInputArgument();
                        sigInpArg.UniqueKey = getNewElemUniqueKey();
                        slInputArguments.Add(sigInpArg.UniqueKey, sigInpArg);
                    }
                    else
                    {
                        sigInpArg = (S6xRoutineInputArgument)slInputArguments[currentTreeNode.Name];
                    }
                    sigInpArg.Position = Convert.ToInt32(inputArgPositionTextBox.Text);
                    try
                    {
                        sigInpArg.Encryption = (int)(CallArgsMode)inputArgEncryptionComboBox.SelectedItem;
                    }
                    catch
                    {
                        sigInpArg.Encryption = 0;
                    }
                    sigInpArg.Word = inputArgWordCheckBox.Checked;
                    sigInpArg.Pointer = inputArgPointerCheckBox.Checked;

                    sigInpArg.DateCreated = inputArgDateCreatedDateTimePicker.Value.ToUniversalTime();
                    inputArgDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigInpArg.DateUpdated = inputArgDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    uniqueKey = sigInpArg.UniqueKey;
                    label = sigInpArg.Position.ToString() + " - " + sigInpArg.Code;

                    sigInpArg = null;
                    break;
                case TreeCategInputStructuresNodeName:
                    S6xRoutineInputStructure sigInpStr = null;
                    if (currentTreeNode == null)
                    {
                        sigInpStr = new S6xRoutineInputStructure();
                        sigInpStr.UniqueKey = getNewElemUniqueKey();
                        slInputStructures.Add(sigInpStr.UniqueKey, sigInpStr);
                    }
                    else
                    {
                        sigInpStr = (S6xRoutineInputStructure)slInputStructures[currentTreeNode.Name];
                    }
                    sigInpStr.VariableAddress = inputStructureAddrTextBox.Text;
                    sigInpStr.VariableNumber = inputStructureNumRegTextBox.Text;
                    sigInpStr.StructDef = inputStructureStructTextBox.Text;

                    sigInpStr.ForcedNumber = inputStructureNumFixTextBox.Text;

                    sigInpStr.DateCreated = inputStructureDateCreatedDateTimePicker.Value.ToUniversalTime();
                    inputStructureDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigInpStr.DateUpdated = inputStructureDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    uniqueKey = sigInpStr.UniqueKey;
                    label = sigInpStr.VariableAddress;
                    
                    sigInpStr = null;
                    break;
                case TreeCategInputTablesNodeName:
                    S6xRoutineInputTable sigInpTbl = null;
                    if (currentTreeNode == null)
                    {
                        sigInpTbl = new S6xRoutineInputTable();
                        sigInpTbl.UniqueKey = getNewElemUniqueKey();
                        slInputTables.Add(sigInpTbl.UniqueKey, sigInpTbl);
                    }
                    else
                    {
                        sigInpTbl = (S6xRoutineInputTable)slInputTables[currentTreeNode.Name];
                    }
                    sigInpTbl.VariableAddress = inputTableAddrTextBox.Text;
                    sigInpTbl.VariableColsNumberReg = inputTableColsNumRegTextBox.Text;
                    sigInpTbl.VariableColsReg = inputTableColsRegTextBox.Text;
                    sigInpTbl.VariableOutput = inputTableOutputTextBox.Text;
                    sigInpTbl.VariableRowsReg = inputTableRowsRegTextBox.Text;
                    sigInpTbl.SignedOutput = inputTableSignedCheckBox.Checked;
                    sigInpTbl.WordOutput = inputTableWordCheckBox.Checked;

                    sigInpTbl.ForcedCellsUnits = inputTableCellsUnitsTextBox.Text;
                    sigInpTbl.ForcedColsNumber = inputTableColsNumFixTextBox.Text;
                    sigInpTbl.ForcedColsUnits = inputTableColsUnitsTextBox.Text;
                    sigInpTbl.ForcedRowsNumber = inputTableRowsNumTextBox.Text;
                    sigInpTbl.ForcedRowsUnits = inputTableRowsUnitsTextBox.Text;
                    sigInpTbl.ForcedCellsScaleExpression = inputTableScaleTextBox.Text;
                    sigInpTbl.ForcedCellsScalePrecision = (int)inputTableScalePrecNumericUpDown.Value;

                    sigInpTbl.DateCreated = inputTableDateCreatedDateTimePicker.Value.ToUniversalTime();
                    inputTableDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigInpTbl.DateUpdated = inputTableDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    uniqueKey = sigInpTbl.UniqueKey;
                    label = sigInpTbl.VariableAddress;
                    
                    sigInpTbl = null;
                    break;
                case TreeCategInputFunctionsNodeName:
                    S6xRoutineInputFunction sigInpFunc = null;
                    if (currentTreeNode == null)
                    {
                        sigInpFunc = new S6xRoutineInputFunction();
                        sigInpFunc.UniqueKey = getNewElemUniqueKey();
                        slInputFunctions.Add(sigInpFunc.UniqueKey, sigInpFunc);
                    }
                    else
                    {
                        sigInpFunc = (S6xRoutineInputFunction)slInputFunctions[currentTreeNode.Name];
                    }
                    sigInpFunc.VariableAddress = inputFunctionAddrTextBox.Text;
                    sigInpFunc.ByteInput = inputFunctionByteCheckBox.Checked;
                    sigInpFunc.VariableInput = inputFunctionInputTextBox.Text;
                    sigInpFunc.VariableOutput = inputFunctionOutputTextBox.Text;
                    sigInpFunc.SignedInput = inputFunctionSignedInputCheckBox.Checked;
                    sigInpFunc.SignedOutput = inputFunctionSignedOutputCheckBox.Checked;

                    sigInpFunc.ForcedRowsNumber = inputFunctionRowsTextBox.Text;
                    sigInpFunc.ForcedInputScaleExpression = inputFunctionScaleInputTextBox.Text;
                    sigInpFunc.ForcedOutputScaleExpression = inputFunctionScaleOutputTextBox.Text;
                    sigInpFunc.ForcedInputScalePrecision = (int)inputFunctionScalePrecInputNumericUpDown.Value;
                    sigInpFunc.ForcedOutputScalePrecision = (int)inputFunctionScalePrecOutputNumericUpDown.Value;
                    sigInpFunc.ForcedInputUnits = inputFunctionUnitsInputTextBox.Text;
                    sigInpFunc.ForcedOutputUnits = inputFunctionUnitsOutputTextBox.Text;

                    sigInpFunc.DateCreated = inputFunctionDateCreatedDateTimePicker.Value.ToUniversalTime();
                    inputFunctionDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigInpFunc.DateUpdated = inputFunctionDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    uniqueKey = sigInpFunc.UniqueKey;
                    label = sigInpFunc.VariableAddress;

                    sigInpFunc = null;
                    break;
                case TreeCategInputScalarsNodeName:
                    S6xRoutineInputScalar sigInpScal = null;
                    if (currentTreeNode == null)
                    {
                        sigInpScal = new S6xRoutineInputScalar();
                        sigInpScal.UniqueKey = getNewElemUniqueKey();
                        slInputScalars.Add(sigInpScal.UniqueKey, sigInpScal);
                    }
                    else
                    {
                        sigInpScal = (S6xRoutineInputScalar)slInputScalars[currentTreeNode.Name];
                    }
                    sigInpScal.VariableAddress = inputScalarAddrTextBox.Text;
                    sigInpScal.Byte = inputScalarByteCheckBox.Checked;
                    sigInpScal.Signed = inputScalarSignedCheckBox.Checked;

                    sigInpScal.ForcedScaleExpression = inputScalarScaleTextBox.Text;
                    sigInpScal.ForcedScalePrecision = (int)inputScalarScalePrecNumericUpDown.Value;
                    sigInpScal.ForcedUnits = inputScalarUnitsTextBox.Text;

                    sigInpScal.DateCreated = inputScalarDateCreatedDateTimePicker.Value.ToUniversalTime();
                    inputScalarDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigInpScal.DateUpdated = inputScalarDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    uniqueKey = sigInpScal.UniqueKey;
                    label = sigInpScal.VariableAddress;

                    sigInpScal = null;
                    break;
                case TreeCategInternalStructuresNodeName:
                    S6xRoutineInternalStructure sigIntStr = null;
                    if (currentTreeNode == null)
                    {
                        sigIntStr = new S6xRoutineInternalStructure();
                        sigIntStr.UniqueKey = getNewElemUniqueKey();
                        slInternalStructures.Add(sigIntStr.UniqueKey, sigIntStr);
                    }
                    else
                    {
                        sigIntStr = (S6xRoutineInternalStructure)slInternalStructures[currentTreeNode.Name];
                    }
                    sigIntStr.VariableAddress = internalStructureAddrTextBox.Text;
                    sigIntStr.VariableBankNum = internalStructureBankTextBox.Text;
                    sigIntStr.Comments = internalStructureCommentsTextBox.Text;
                    sigIntStr.Label = internalStructureLabelTextBox.Text;
                    sigIntStr.Number = Convert.ToInt32(internalStructureNumTextBox.Text);
                    sigIntStr.OutputComments = internalStructureOutputCommentsCheckBox.Checked;
                    sigIntStr.ShortLabel = internalStructureSLabelTextBox.Text;
                    sigIntStr.StructDef = internalStructureStructTextBox.Text;

                    sigIntStr.IdentificationStatus = internalStructureIdentificationStatusTrackBar.Value;
                    sigIntStr.IdentificationDetails = internalStructureIdentificationDetailsTextBox.Text;

                    sigIntStr.DateCreated = internalStructureDateCreatedDateTimePicker.Value.ToUniversalTime();
                    internalStructureDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigIntStr.DateUpdated = internalStructureDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    sigIntStr.Category = internalStructureCategComboBox.Text;
                    sigIntStr.Category2 = internalStructureCateg2ComboBox.Text;
                    sigIntStr.Category3 = internalStructureCateg3ComboBox.Text;

                    uniqueKey = sigIntStr.UniqueKey;
                    label = sigIntStr.Label;
                    comments = sigIntStr.Comments;
                    identificationStatus = sigIntStr.IdentificationStatus;

                    sigIntStr = null;
                    break;
                case TreeCategInternalTablesNodeName:
                    S6xRoutineInternalTable sigIntTbl = null;
                    if (currentTreeNode == null)
                    {
                        sigIntTbl = new S6xRoutineInternalTable();
                        sigIntTbl.UniqueKey = getNewElemUniqueKey();
                        slInternalTables.Add(sigIntTbl.UniqueKey, sigIntTbl);
                    }
                    else
                    {
                        sigIntTbl = (S6xRoutineInternalTable)slInternalTables[currentTreeNode.Name];
                    }
                    sigIntTbl.VariableAddress = internalTableAddrTextBox.Text;
                    sigIntTbl.VariableBankNum = internalTableBankTextBox.Text;
                    sigIntTbl.VariableColsNumber = internalTableColsTextBox.Text;
                    sigIntTbl.CellsUnits = internalTableCellsUnitsTextBox.Text;
                    sigIntTbl.ColsUnits = internalTableColsUnitsTextBox.Text;
                    sigIntTbl.Comments = internalTableCommentsTextBox.Text;
                    sigIntTbl.Label = internalTableLabelTextBox.Text;
                    sigIntTbl.OutputComments = internalTableOutputCommentsCheckBox.Checked;
                    sigIntTbl.RowsNumber = Convert.ToInt32(internalTableRowsTextBox.Text);
                    sigIntTbl.RowsUnits = internalTableRowsUnitsTextBox.Text;
                    sigIntTbl.CellsScaleExpression = internalTableScaleTextBox.Text;
                    sigIntTbl.CellsScalePrecision = (int)internalTableScalePrecNumericUpDown.Value;
                    sigIntTbl.SignedOutput = internalTableSignedCheckBox.Checked;
                    sigIntTbl.ShortLabel = internalTableSLabelTextBox.Text;
                    sigIntTbl.WordOutput = internalTableWordCheckBox.Checked;

                    sigIntTbl.CellsMin = internalTableCellsMinTextBox.Text;
                    sigIntTbl.CellsMax = internalTableCellsMaxTextBox.Text;

                    sigIntTbl.IdentificationStatus = internalTableIdentificationStatusTrackBar.Value;
                    sigIntTbl.IdentificationDetails = internalTableIdentificationDetailsTextBox.Text;

                    sigIntTbl.DateCreated = internalTableDateCreatedDateTimePicker.Value.ToUniversalTime();
                    internalTableDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigIntTbl.DateUpdated = internalTableDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    sigIntTbl.Category = internalTableCategComboBox.Text;
                    sigIntTbl.Category2 = internalTableCateg2ComboBox.Text;
                    sigIntTbl.Category3 = internalTableCateg3ComboBox.Text;

                    uniqueKey = sigIntTbl.UniqueKey;
                    label = sigIntTbl.Label;
                    comments = sigIntTbl.Comments;
                    identificationStatus = sigIntTbl.IdentificationStatus;

                    sigIntTbl = null;
                    break;
                case TreeCategInternalFunctionsNodeName:
                    S6xRoutineInternalFunction sigIntFunc = null;
                    if (currentTreeNode == null)
                    {
                        sigIntFunc = new S6xRoutineInternalFunction();
                        sigIntFunc.UniqueKey = getNewElemUniqueKey();
                        slInternalFunctions.Add(sigIntFunc.UniqueKey, sigIntFunc);
                    }
                    else
                    {
                        sigIntFunc = (S6xRoutineInternalFunction)slInternalFunctions[currentTreeNode.Name];
                    }
                    sigIntFunc.VariableAddress = internalFunctionAddrTextBox.Text;
                    sigIntFunc.VariableBankNum = internalFunctionBankTextBox.Text;
                    sigIntFunc.ByteInput = internalFunctionByteCheckBox.Checked;
                    sigIntFunc.Comments = internalFunctionCommentsTextBox.Text;
                    sigIntFunc.Label = internalFunctionLabelTextBox.Text;
                    sigIntFunc.OutputComments = internalFunctionOutputCommentsCheckBox.Checked;
                    sigIntFunc.RowsNumber = Convert.ToInt32(internalFunctionRowsTextBox.Text);
                    sigIntFunc.InputScaleExpression = internalFunctionScaleInputTextBox.Text;
                    sigIntFunc.OutputScaleExpression = internalFunctionScaleOutputTextBox.Text;
                    sigIntFunc.InputScalePrecision = (int)internalFunctionScalePrecInputNumericUpDown.Value;
                    sigIntFunc.OutputScalePrecision = (int)internalFunctionScalePrecOutputNumericUpDown.Value;
                    sigIntFunc.SignedInput = internalFunctionSignedInputCheckBox.Checked;
                    sigIntFunc.SignedOutput = internalFunctionSignedOutputCheckBox.Checked;
                    sigIntFunc.ShortLabel = internalFunctionSLabelTextBox.Text;
                    sigIntFunc.InputUnits = internalFunctionUnitsInputTextBox.Text;
                    sigIntFunc.OutputUnits = internalFunctionUnitsOutputTextBox.Text;

                    sigIntFunc.InputMin = internalFunctionMinInputTextBox.Text;
                    sigIntFunc.InputMax = internalFunctionMaxInputTextBox.Text;
                    sigIntFunc.OutputMin = internalFunctionMinOutputTextBox.Text;
                    sigIntFunc.OutputMax = internalFunctionMaxOutputTextBox.Text;

                    sigIntFunc.IdentificationStatus = internalFunctionIdentificationStatusTrackBar.Value;
                    sigIntFunc.IdentificationDetails = internalFunctionIdentificationDetailsTextBox.Text;

                    sigIntFunc.DateCreated = internalFunctionDateCreatedDateTimePicker.Value.ToUniversalTime();
                    internalFunctionDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigIntFunc.DateUpdated = internalFunctionDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    sigIntFunc.Category = internalFunctionCategComboBox.Text;
                    sigIntFunc.Category2 = internalFunctionCateg2ComboBox.Text;
                    sigIntFunc.Category3 = internalFunctionCateg3ComboBox.Text;

                    uniqueKey = sigIntFunc.UniqueKey;
                    label = sigIntFunc.Label;
                    comments = sigIntFunc.Comments;
                    identificationStatus = sigIntFunc.IdentificationStatus;

                    sigIntFunc = null;
                    break;
                case TreeCategInternalScalarsNodeName:
                    S6xRoutineInternalScalar sigIntScal = null;
                    if (currentTreeNode == null)
                    {
                        sigIntScal = new S6xRoutineInternalScalar();
                        sigIntScal.UniqueKey = getNewElemUniqueKey();
                        slInternalScalars.Add(sigIntScal.UniqueKey, sigIntScal);
                    }
                    else
                    {
                        sigIntScal = (S6xRoutineInternalScalar)slInternalScalars[currentTreeNode.Name];
                    }
                    sigIntScal.VariableAddress = internalScalarAddrTextBox.Text;
                    sigIntScal.VariableBankNum = internalScalarBankTextBox.Text;
                    sigIntScal.Byte = internalScalarByteCheckBox.Checked;
                    sigIntScal.Comments = internalScalarCommentsTextBox.Text;
                    sigIntScal.Label = internalScalarLabelTextBox.Text;
                    sigIntScal.OutputComments = internalScalarOutputCommentsCheckBox.Checked;
                    sigIntScal.InlineComments = internalScalarInlineCommentsCheckBox.Checked;
                    sigIntScal.ScaleExpression = internalScalarScaleTextBox.Text;
                    sigIntScal.ScalePrecision = (int)internalScalarScalePrecNumericUpDown.Value;
                    sigIntScal.Signed = internalScalarSignedCheckBox.Checked;
                    sigIntScal.ShortLabel = internalScalarSLabelTextBox.Text;
                    sigIntScal.Units = internalScalarUnitsTextBox.Text;

                    sigIntScal.Min = internalScalarMinTextBox.Text;
                    sigIntScal.Max = internalScalarMaxTextBox.Text;

                    sigIntScal.IdentificationStatus = internalScalarIdentificationStatusTrackBar.Value;
                    sigIntScal.IdentificationDetails = internalScalarIdentificationDetailsTextBox.Text;

                    sigIntScal.DateCreated = internalScalarDateCreatedDateTimePicker.Value.ToUniversalTime();
                    internalScalarDateUpdatedDateTimePicker.Value = DateTime.Now;
                    sigIntScal.DateUpdated = internalScalarDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    sigIntScal.Category = internalScalarCategComboBox.Text;
                    sigIntScal.Category2 = internalScalarCateg2ComboBox.Text;
                    sigIntScal.Category3 = internalScalarCateg3ComboBox.Text;

                    if (internalScalarBitFlagsButton.Tag != null)
                    {
                        S6xScalar s6xScalar = (S6xScalar)internalScalarBitFlagsButton.Tag;
                        internalScalarBitFlagsButton.Tag = null;
                        if (s6xScalar.BitFlags != null) sigIntScal.BitFlags = (S6xBitFlag[])s6xScalar.BitFlags.Clone();
                        s6xScalar = null;
                    }

                    uniqueKey = sigIntScal.UniqueKey;
                    label = sigIntScal.Label;
                    comments = sigIntScal.Comments;
                    identificationStatus = sigIntScal.IdentificationStatus;

                    sigIntScal = null;
                    break;
                default:
                    return;
            }

            if (currentTreeNode == null)
            {
                updateArray(categ);
                
                currentTreeNode = new TreeNode();
                currentTreeNode.Name = uniqueKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[categ].Nodes.Add(currentTreeNode);
            }

            closingDialogResult = DialogResult.OK;

            currentTreeNode.Text = label;
            currentTreeNode.ToolTipText = comments;
            currentTreeNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(identificationStatus);

            clearElem();

            advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName];
        }

        private void updateRoutine()
        {
            s6xSig.ShortLabel = routineSLabelTextBox.Text;
            s6xSig.Label = routineLabelTextBox.Text;
            s6xSig.Comments = routineCommentsTextBox.Text;

            s6xSig.OutputComments = routineOutputCommentsCheckBox.Checked;

            if (signatureFor806xComboBox.Items.Count == Enum.GetValues(typeof(Signature806xOptions)).Length)
            {
                if (signatureFor806xComboBox.SelectedIndex == (int)Signature806xOptions.Undefined) s6xSig.for806x = null;
                else s6xSig.for806x = Enum.GetName(typeof(Signature806xOptions), signatureFor806xComboBox.SelectedIndex);
            }

            if (signatureForBankComboBox.Items.Count == Enum.GetValues(typeof(SignatureBankOptions)).Length)
            {
                if (signatureForBankComboBox.SelectedIndex == (int)SignatureBankOptions.Undefined) s6xSig.forBankNum = null;
                else s6xSig.forBankNum = Enum.GetName(typeof(SignatureBankOptions), signatureForBankComboBox.SelectedIndex);
            }

            s6xSig.RoutineIdentificationStatus = routineIdentificationStatusTrackBar.Value;
            s6xSig.RoutineIdentificationDetails = routineIdentificationDetailsTextBox.Text;

            s6xSig.RoutineDateCreated = routineDateCreatedDateTimePicker.Value.ToUniversalTime();
            routineDateUpdatedDateTimePicker.Value = DateTime.Now;
            s6xSig.RoutineDateUpdated = routineDateUpdatedDateTimePicker.Value.ToUniversalTime();

            s6xSig.RoutineCategory = routineCategComboBox.Text;
            s6xSig.RoutineCategory2 = routineCateg2ComboBox.Text;
            s6xSig.RoutineCategory3 = routineCateg3ComboBox.Text;
        }

        private bool checkElem(string categ)
        {
            bool checkPassed = true;

            switch (categ)
            {
                case TreeCategInputArgumentsNodeName:
                    checkPassed &= checkForcedColsRowsNumber(inputArgPositionTextBox.Text);
                    return checkPassed;
                case TreeCategInputStructuresNodeName:
                    checkPassed &= checkVariableValue(inputStructureAddrTextBox.Text);
                    checkPassed &= checkForcedColsRowsNumber(inputStructureNumFixTextBox.Text);
                    checkPassed &= checkVariableValue(inputStructureNumRegTextBox.Text);
                    checkPassed &= checkStructDef(inputStructureStructTextBox.Text);
                    return checkPassed;
                case TreeCategInputTablesNodeName:
                    checkPassed &= checkVariableValue(inputTableAddrTextBox.Text);
                    checkPassed &= checkForcedColsRowsNumber(inputTableColsNumFixTextBox.Text);
                    checkPassed &= checkVariableValue(inputTableColsNumRegTextBox.Text);
                    checkPassed &= checkVariableValue(inputTableColsRegTextBox.Text);
                    checkPassed &= checkVariableValue(inputTableOutputTextBox.Text);
                    checkPassed &= checkForcedColsRowsNumber(inputTableRowsNumTextBox.Text);
                    checkPassed &= checkVariableValue(inputTableRowsRegTextBox.Text);
                    checkScaleExpression(inputTableScaleTextBox.Text);                          // Warning Only
                    return checkPassed;
                case TreeCategInputFunctionsNodeName:
                    checkPassed &= checkVariableValue(inputFunctionAddrTextBox.Text);
                    checkPassed &= checkVariableValue(inputFunctionInputTextBox.Text);
                    checkPassed &= checkVariableValue(inputFunctionOutputTextBox.Text);
                    checkPassed &= checkForcedColsRowsNumber(inputFunctionRowsTextBox.Text);
                    checkScaleExpression(inputFunctionScaleInputTextBox.Text);                  // Warning Only
                    checkScaleExpression(inputFunctionScaleOutputTextBox.Text);                 // Warning Only
                    return checkPassed;
                case TreeCategInputScalarsNodeName:
                    checkPassed &= checkVariableValue(inputScalarAddrTextBox.Text);
                    checkScaleExpression(inputScalarScaleTextBox.Text);                         // Warning Only
                    return checkPassed;
                case TreeCategInternalStructuresNodeName:
                    checkPassed &= checkVariableValue(internalStructureAddrTextBox.Text);
                    checkPassed &= checkColsRowsNumber(internalStructureNumTextBox.Text);
                    checkPassed &= checkStructDef(internalStructureStructTextBox.Text);
                    return checkPassed;
                case TreeCategInternalTablesNodeName:
                    checkPassed &= checkVariableValue(internalTableAddrTextBox.Text);
                    checkPassed &= checkVariableValue(internalTableColsTextBox.Text);
                    checkPassed &= checkColsRowsNumber(internalTableRowsTextBox.Text);
                    checkScaleExpression(internalTableScaleTextBox.Text);                       // Warning Only
                    checkMinMax(internalTableCellsMinTextBox.Text);                             // Warning Only
                    checkMinMax(internalTableCellsMaxTextBox.Text);                             // Warning Only
                    return checkPassed;
                case TreeCategInternalFunctionsNodeName:
                    checkPassed &= checkVariableValue(internalFunctionAddrTextBox.Text);
                    checkPassed &= checkColsRowsNumber(internalFunctionRowsTextBox.Text);
                    checkScaleExpression(internalFunctionScaleInputTextBox.Text);               // Warning Only
                    checkScaleExpression(internalFunctionScaleOutputTextBox.Text);              // Warning Only
                    checkMinMax(internalFunctionMinInputTextBox.Text);                          // Warning Only
                    checkMinMax(internalFunctionMaxInputTextBox.Text);                          // Warning Only
                    checkMinMax(internalFunctionMinOutputTextBox.Text);                         // Warning Only
                    checkMinMax(internalFunctionMaxOutputTextBox.Text);                         // Warning Only
                    return checkPassed;
                case TreeCategInternalScalarsNodeName:
                    checkPassed &= checkVariableValue(internalScalarAddrTextBox.Text);
                    checkScaleExpression(internalScalarScaleTextBox.Text);                      // Warning Only
                    checkMinMax(internalScalarMinTextBox.Text);                                 // Warning Only
                    checkMinMax(internalScalarMaxTextBox.Text);                                 // Warning Only
                    return checkPassed;
                default:
                    return false;
            }
        }

        private bool checkVariableValue(string variableValue)
        {
            // TEMPORARY
            return variableValue.Length >= 2;
        }
        
        private bool checkScaleExpression(string scaleExpression)
        {
            if (Tools.ScaleExpressionCheck(scaleExpression))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Scale expression \"" + scaleExpression + "\" is not managed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private bool checkNumber(string number)
        {
            if (number.Contains(SADDef.SignatureParamBytePrefixSuffix)) number = number.Replace(SADDef.SignatureParamBytePrefixSuffix, string.Empty);

            try
            {
                int num = Convert.ToInt32(number);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool checkMinMax(string number)
        {
            if (number.Contains(SADDef.SignatureParamBytePrefixSuffix)) number = number.Replace(SADDef.SignatureParamBytePrefixSuffix, string.Empty);

            if (number == Tools.getValidMinMax(number)) return true;

            MessageBox.Show("Minimum or Maximum value \"" + number + "\" will not be seen as a valid expression.\r\nExpect format is '000000.0000'.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private bool checkForcedColsRowsNumber(string number)
        {
            if (number == string.Empty) return true;

            if (number.Contains(SADDef.SignatureParamBytePrefixSuffix)) number = number.Replace(SADDef.SignatureParamBytePrefixSuffix, string.Empty);

            try
            {
                int num = Convert.ToInt32(number);
                if (num <= 0 || num > 99) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool checkColsRowsNumber(string number)
        {
            if (number.Contains(SADDef.SignatureParamBytePrefixSuffix)) number = number.Replace(SADDef.SignatureParamBytePrefixSuffix, string.Empty);

            try
            {
                int num = Convert.ToInt32(number);
                // 20210311 - PYM - 0 is for Autodetection
                //if (num <= 0 || num > 99) return false;
                if (num < 0 || num > 99) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool checkStructDef(string sDef)
        {
            bool checkPassed = false;
            Structure sStruct = new Structure();
            sStruct.StructDefString = sDef;
            checkPassed = sStruct.isValid;

            if (!checkPassed) MessageBox.Show("Invalid Structure definition.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            sStruct = null;

            return checkPassed;
        }

        private CheckState CheckStateFromNullableBool(bool? value)
        {
            if (value == null) return CheckState.Indeterminate;
            else if (value.Value) return CheckState.Checked;
            else return CheckState.Unchecked;
        }

        private bool? NullableBoolFromCheckState(CheckState value)
        {
            switch (value)
            {
                case CheckState.Checked:
                    return true;
                case CheckState.Unchecked:
                    return false;
                default:
                    return null;
            }
        }

        private string getNewElemUniqueKey()
        {
            bool isValid = true;
            int cnt = 0;
            cnt += slInternalStructures.Count;
            cnt += slInternalTables.Count;
            cnt += slInternalFunctions.Count;
            cnt += slInternalScalars.Count;
            cnt += slInputArguments.Count;
            cnt += slInputStructures.Count;
            cnt += slInputTables.Count;
            cnt += slInputFunctions.Count;
            cnt += slInputScalars.Count;

            string uniqueKey = string.Empty;
            while (uniqueKey == string.Empty || !isValid)
            {
                isValid = true;
                uniqueKey = string.Format("Sa{0:d3}", cnt);
                cnt++;

                if (isValid) isValid &= !slInternalStructures.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInternalTables.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInternalFunctions.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInternalScalars.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputArguments.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputStructures.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputTables.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputFunctions.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputScalars.ContainsKey(uniqueKey);
            }
            return uniqueKey;
        }

        private void internalScalarBitFlagsButton_Click(object sender, EventArgs e)
        {
            string uniqueAddress = string.Empty;
            S6xScalar tempScalar = null;
            S6xScalar s6xScalar = null;

            if (currentTreeNode == null) return;

            if (currentTreeNode.Parent != null)
            {
                if (currentTreeNode.Parent.Name != TreeCategInternalScalarsNodeName) return;
            }

            S6xRoutineInternalScalar sigIntScal = (S6xRoutineInternalScalar)slInputScalars[currentTreeNode.Name];

            if (internalScalarBitFlagsButton.Tag != null)
            {
                s6xScalar = (S6xScalar)internalScalarBitFlagsButton.Tag;
            }

            if (s6xScalar == null)
            {
                tempScalar = new S6xScalar();
                if (sigIntScal != null)
                {
                    if (sigIntScal.BitFlags != null) tempScalar.BitFlags = (S6xBitFlag[])sigIntScal.BitFlags.Clone();
                }
            }
            else
            {
                tempScalar = s6xScalar.Clone();
            }
            s6xScalar = null;

            tempScalar.Label = "Internal Scalar";
            tempScalar.Byte = internalScalarByteCheckBox.Checked;

            ImageList stateImageList = advElemsTreeView.StateImageList;
            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref S6x, ref tempScalar, ref stateImageList, ref s6xNavCategories);
            bool updatedBitFlags = bitFlagsForm.ShowDialog() == DialogResult.OK;
            stateImageList = null;
            bitFlagsForm = null;

            if (updatedBitFlags)
            {
                internalScalarBitFlagsCheckBox.Checked = tempScalar.isBitFlags;

                // To be reused on Update
                internalScalarBitFlagsButton.Tag = tempScalar;
            }

            tempScalar = null;
        }

        private void initRepositories()
        {
            repoTables = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameTables, typeof(Repository));
            if (repoTables == null) repoTables = new Repository();

            repoFunctions = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameFunctions, typeof(Repository));
            if (repoFunctions == null) repoFunctions = new Repository();

            repoScalars = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameScalars, typeof(Repository));
            if (repoScalars == null) repoScalars = new Repository();

            repoStructures = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameStructures, typeof(Repository));
            if (repoStructures == null) repoStructures = new Repository();

            repoUnits = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameUnits, typeof(Repository));
            if (repoUnits == null) repoUnits = new Repository();

            repoConversion = (RepositoryConversion)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameConversion, typeof(RepositoryConversion));
            if (repoConversion == null) repoConversion = new RepositoryConversion();
        }

        private void showRepository(Control sender)
        {
            if (sender == null) return;
            if (sender.Parent == null) return;

            string searchLabel = string.Empty;

            switch (sender.Name)
            {
                case "inputTableCellsUnitsTextBox":
                case "inputTableColsUnitsTextBox":
                case "inputTableRowsUnitsTextBox":
                case "inputFunctionUnitsInputTextBox":
                case "inputFunctionUnitsOutputTextBox":
                case "inputScalarUnitsTextBox":
                case "inputTableScaleTextBox":
                case "inputFunctionScaleInputTextBox":
                case "inputFunctionScaleOutputTextBox":
                case "inputScalarScaleTextBox":
                case "internalTableCellsUnitsTextBox":
                case "internalTableColsUnitsTextBox":
                case "internalTableRowsUnitsTextBox":
                case "internalFunctionUnitsInputTextBox":
                case "internalFunctionUnitsOutputTextBox":
                case "internalScalarUnitsTextBox":
                case "internalTableScaleTextBox":
                case "internalFunctionScaleInputTextBox":
                case "internalFunctionScaleOutputTextBox":
                case "internalScalarScaleTextBox":
                    repoContextMenuStrip.Tag = sender.Name;
                    break;
                default:
                    repoContextMenuStrip.Tag = sender.Parent.Name;
                    break;
            }
            switch (repoContextMenuStrip.Tag.ToString())
            {
                case "internalStructureTabPage":
                    searchLabel = internalStructureSLabelTextBox.Text;
                    break;
                case "internalTableTabPage":
                    searchLabel = internalTableSLabelTextBox.Text;
                    break;
                case "internalFunctionTabPage":
                    searchLabel = internalFunctionSLabelTextBox.Text;
                    break;
                case "internalScalarTabPage":
                    searchLabel = internalScalarSLabelTextBox.Text;
                    break;
                case "inputTableColsUnitsTextBox":
                    searchLabel = inputTableColsUnitsTextBox.Text;
                    break;
                case "inputTableRowsUnitsTextBox":
                    searchLabel = inputTableRowsUnitsTextBox.Text;
                    break;
                case "inputTableCellsUnitsTextBox":
                case "inputTableScaleTextBox":
                    searchLabel = inputTableCellsUnitsTextBox.Text;
                    break;
                case "inputFunctionUnitsInputTextBox":
                case "inputFunctionScaleInputTextBox":
                    searchLabel = inputFunctionUnitsInputTextBox.Text;
                    break;
                case "inputFunctionUnitsOutputTextBox":
                case "inputFunctionScaleOutputTextBox":
                    searchLabel = inputFunctionUnitsOutputTextBox.Text;
                    break;
                case "inputScalarUnitsTextBox":
                case "inputScalarScaleTextBox":
                    searchLabel = inputScalarUnitsTextBox.Text;
                    break;
                case "internalTableColsUnitsTextBox":
                    searchLabel = internalTableColsUnitsTextBox.Text;
                    break;
                case "internalTableRowsUnitsTextBox":
                    searchLabel = internalTableRowsUnitsTextBox.Text;
                    break;
                case "internalTableCellsUnitsTextBox":
                case "internalTableScaleTextBox":
                    searchLabel = internalTableCellsUnitsTextBox.Text;
                    break;
                case "internalFunctionUnitsInputTextBox":
                case "internalFunctionScaleInputTextBox":
                    searchLabel = internalFunctionUnitsInputTextBox.Text;
                    break;
                case "internalFunctionUnitsOutputTextBox":
                case "internalFunctionScaleOutputTextBox":
                    searchLabel = internalFunctionUnitsOutputTextBox.Text;
                    break;
                case "internalScalarUnitsTextBox":
                case "internalScalarScaleTextBox":
                    searchLabel = internalScalarUnitsTextBox.Text;
                    break;
                default:
                    repoContextMenuStrip.Tag = null;
                    return;
            }

            repoToolStripTextBox.Text = searchLabel;

            repoContextMenuStrip.Show(Cursor.Position);
        }

        private void repoContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            repoToolStripTextBox_TextChanged(sender, e);
        }

        private void repoToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            Repository repoRepository = null;
            RepositoryConversion repoRepositoryConversion = null;

            repoToolStripMenuItem.DropDownItems.Clear();

            if (repoContextMenuStrip.Tag == null) return;

            switch (repoContextMenuStrip.Tag.ToString())
            {
                case "internalStructureTabPage":
                    repoRepository = repoStructures;
                    break;
                case "internalTableTabPage":
                    repoRepository = repoTables;
                    break;
                case "internalFunctionTabPage":
                    repoRepository = repoFunctions;
                    break;
                case "internalScalarTabPage":
                    repoRepository = repoScalars;
                    break;
                case "inputTableCellsUnitsTextBox":
                case "inputTableColsUnitsTextBox":
                case "inputTableRowsUnitsTextBox":
                case "inputFunctionUnitsInputTextBox":
                case "inputFunctionUnitsOutputTextBox":
                case "inputScalarUnitsTextBox":
                case "internalTableCellsUnitsTextBox":
                case "internalTableColsUnitsTextBox":
                case "internalTableRowsUnitsTextBox":
                case "internalFunctionUnitsInputTextBox":
                case "internalFunctionUnitsOutputTextBox":
                case "internalScalarUnitsTextBox":
                    repoRepository = repoUnits;
                    break;
                case "inputTableScaleTextBox":
                case "inputFunctionScaleInputTextBox":
                case "inputFunctionScaleOutputTextBox":
                case "inputScalarScaleTextBox":
                case "internalTableScaleTextBox":
                case "internalFunctionScaleInputTextBox":
                case "internalFunctionScaleOutputTextBox":
                case "internalScalarScaleTextBox":
                    repoRepositoryConversion = repoConversion;
                    break;
                default:
                    repoContextMenuStrip.Tag = null;
                    break;
            }

            string searchLabel = repoToolStripTextBox.Text.ToUpper();

            SortedList slFiltered = new SortedList();
            if (repoRepository != null)
            {
                foreach (RepositoryItem repoItem in repoRepository.Items)
                {
                    string sLabel = repoItem.ShortLabel;
                    string label = repoItem.Label;

                    if (sLabel == null) continue;
                    if (label == null) label = string.Empty;

                    if (searchLabel == string.Empty || sLabel.ToUpper().Contains(searchLabel) || label.ToUpper().Contains(searchLabel))
                    {
                        if (!slFiltered.ContainsKey(sLabel)) slFiltered.Add(sLabel, repoItem);
                    }
                }
                repoRepository = null;

                foreach (RepositoryItem repoItem in slFiltered.Values)
                {
                    ToolStripMenuItem tsMI = new ToolStripMenuItem();
                    tsMI.Tag = repoItem;
                    tsMI.Text = repoItem.ShortLabel;
                    tsMI.ToolTipText = string.Empty;
                    if (repoItem.Label != null && repoItem.Label != string.Empty) tsMI.ToolTipText += repoItem.Label + "\r\n\r\n";
                    if (repoItem.Comments != null && repoItem.Comments != string.Empty) tsMI.ToolTipText += repoItem.Comments + "\r\n\r\n";
                    if (repoItem.Information != null && repoItem.Information != string.Empty) tsMI.ToolTipText += repoItem.Information + "\r\n\r\n";
                    repoToolStripMenuItem.DropDownItems.Add(tsMI);
                    // Limited to 50
                    if (repoToolStripMenuItem.DropDownItems.Count >= 50) break;
                }
            }
            if (repoRepositoryConversion != null)
            {
                foreach (RepositoryConversionItem repoItem in repoRepositoryConversion.Items)
                {
                    string sLabel = repoItem.Title;

                    if (sLabel == null) continue;

                    if (searchLabel == string.Empty || sLabel.ToUpper().Contains(searchLabel))
                    {
                        if (!slFiltered.ContainsKey(sLabel)) slFiltered.Add(sLabel, repoItem);
                    }
                }
                repoRepositoryConversion = null;

                foreach (RepositoryConversionItem repoItem in slFiltered.Values)
                {
                    ToolStripMenuItem tsMI = new ToolStripMenuItem();
                    tsMI.Tag = repoItem;
                    tsMI.Text = repoItem.Title;
                    tsMI.ToolTipText = string.Empty;
                    if (repoItem.InternalFormula != null && repoItem.InternalFormula != string.Empty) tsMI.ToolTipText += repoItem.InternalFormula + "\r\n\r\n";
                    if (repoItem.Comments != null && repoItem.Comments != string.Empty) tsMI.ToolTipText += repoItem.Comments + "\r\n\r\n";
                    if (repoItem.Information != null && repoItem.Information != string.Empty) tsMI.ToolTipText += repoItem.Information + "\r\n\r\n";
                    repoToolStripMenuItem.DropDownItems.Add(tsMI);
                    // Limited to 50
                    if (repoToolStripMenuItem.DropDownItems.Count >= 50) break;
                }
            }
            slFiltered = null;
        }

        private void repoToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string sSLabel = string.Empty;
            string sLLabel = string.Empty;
            string sComments = string.Empty;

            if (e.ClickedItem.Tag == null) return;

            if (e.ClickedItem.Tag.GetType() == typeof(RepositoryItem))
            {
                sSLabel = ((RepositoryItem)e.ClickedItem.Tag).ShortLabel;
                sLLabel = ((RepositoryItem)e.ClickedItem.Tag).Label;
                sComments = ((RepositoryItem)e.ClickedItem.Tag).Comments;
            }
            else if (e.ClickedItem.Tag.GetType() == typeof(RepositoryConversionItem))
            {
                sSLabel = ((RepositoryConversionItem)e.ClickedItem.Tag).Title;
                sLLabel = ((RepositoryConversionItem)e.ClickedItem.Tag).InternalFormula;
                sComments = ((RepositoryConversionItem)e.ClickedItem.Tag).Comments;
            }
            else return;

            if (sSLabel == null) sSLabel = string.Empty;
            if (sLLabel == null) sLLabel = string.Empty;
            if (sComments == null) sComments = string.Empty;

            switch (repoContextMenuStrip.Tag.ToString())
            {
                case "internalStructureTabPage":
                    internalStructureSLabelTextBox.Text = sSLabel;
                    internalStructureLabelTextBox.Text = sLLabel;
                    internalStructureCommentsTextBox.Text = sComments;
                    break;
                case "internalTableTabPage":
                    internalTableSLabelTextBox.Text = sSLabel;
                    internalTableLabelTextBox.Text = sLLabel;
                    internalTableCommentsTextBox.Text = sComments;
                    break;
                case "internalFunctionTabPage":
                    internalFunctionSLabelTextBox.Text = sSLabel;
                    internalFunctionLabelTextBox.Text = sLLabel;
                    internalFunctionCommentsTextBox.Text = sComments;
                    break;
                case "internalScalarTabPage":
                    internalScalarSLabelTextBox.Text = sSLabel;
                    internalScalarLabelTextBox.Text = sLLabel;
                    internalScalarCommentsTextBox.Text = sComments;
                    break;
                case "inputTableCellsUnitsTextBox":
                    inputTableCellsUnitsTextBox.Text = sSLabel;
                    break;
                case "inputTableColsUnitsTextBox":
                    inputTableColsUnitsTextBox.Text = sSLabel;
                    break;
                case "inputTableRowsUnitsTextBox":
                    inputTableRowsUnitsTextBox.Text = sSLabel;
                    break;
                case "inputFunctionUnitsInputTextBox":
                    inputFunctionUnitsInputTextBox.Text = sSLabel;
                    break;
                case "inputFunctionUnitsOutputTextBox":
                    inputFunctionUnitsOutputTextBox.Text = sSLabel;
                    break;
                case "inputScalarUnitsTextBox":
                    inputScalarUnitsTextBox.Text = sSLabel;
                    break;
                case "internalTableCellsUnitsTextBox":
                    internalTableCellsUnitsTextBox.Text = sSLabel;
                    break;
                case "internalTableColsUnitsTextBox":
                    internalTableColsUnitsTextBox.Text = sSLabel;
                    break;
                case "internalTableRowsUnitsTextBox":
                    internalTableRowsUnitsTextBox.Text = sSLabel;
                    break;
                case "internalFunctionUnitsInputTextBox":
                    internalFunctionUnitsInputTextBox.Text = sSLabel;
                    break;
                case "internalFunctionUnitsOutputTextBox":
                    internalFunctionUnitsOutputTextBox.Text = sSLabel;
                    break;
                case "internalScalarUnitsTextBox":
                    internalScalarUnitsTextBox.Text = sSLabel;
                    break;
                case "inputTableScaleTextBox":
                    inputTableScaleTextBox.Text = sLLabel;
                    break;
                case "inputFunctionScaleInputTextBox":
                    inputFunctionScaleInputTextBox.Text = sLLabel;
                    break;
                case "inputFunctionScaleOutputTextBox":
                    inputFunctionScaleOutputTextBox.Text = sLLabel;
                    break;
                case "inputScalarScaleTextBox":
                    inputScalarScaleTextBox.Text = sLLabel;
                    break;
                case "internalTableScaleTextBox":
                    internalTableScaleTextBox.Text = sLLabel;
                    break;
                case "internalFunctionScaleInputTextBox":
                    internalFunctionScaleInputTextBox.Text = sLLabel;
                    break;
                case "internalFunctionScaleOutputTextBox":
                    internalFunctionScaleOutputTextBox.Text = sLLabel;
                    break;
                case "internalScalarScaleTextBox":
                    internalScalarScaleTextBox.Text = sLLabel;
                    break;
                default:
                    repoContextMenuStrip.Tag = null;
                    return;
            }
        }
    }
}
