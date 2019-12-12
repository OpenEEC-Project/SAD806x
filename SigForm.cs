using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

        public SigForm(ref SADS6x s6x, ref S6xSignature sig)
        {
            S6x = s6x;
            s6xSig = sig;

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

            InitializeComponent();
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

            advLabelTextBox.Text = s6xSig.Label;
            advSLabelTextBox.Text = s6xSig.ShortLabel;
            advSigTextBox.Text = s6xSig.Signature;

            mainTipPictureBox.Tag = Tools.SignatureTip();
            mainTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            mainTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            inputStructureTipPictureBox.Tag = Tools.StructureTip();
            inputStructureTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            inputStructureTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            internalStructureTipPictureBox.Tag = Tools.StructureTip();
            internalStructureTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            internalStructureTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            repoContextMenuStrip.Opening += new CancelEventHandler(repoContextMenuStrip_Opening);
            repoToolStripTextBox.TextChanged += new EventHandler(repoToolStripTextBox_TextChanged);
            repoToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(repoToolStripMenuItem_DropDownItemClicked);

            Control.ControlCollection controls = null;
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
        }

        private void inputArgPositionTextBox_TextChanged(object sender, EventArgs e)
        {
            inputArgCodeTextBox.Text = Tools.ArgumentCode(inputArgPositionTextBox.Text);
        }

        private void loadElemsTreeView()
        {
            TreeNode tnNode = null;

            foreach (S6xRoutineInputArgument s6xObject in slInputArguments.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Position.ToString() + " - " + s6xObject.Code;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputArgumentsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputStructure s6xObject in slInputStructures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputStructuresNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputTable s6xObject in slInputTables.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputTablesNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputFunction s6xObject in slInputFunctions.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputFunctionsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInputScalar s6xObject in slInputScalars.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.VariableAddress;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInputScalarsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }

            foreach (S6xRoutineInternalStructure s6xObject in slInternalStructures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalStructuresNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInternalTable s6xObject in slInternalTables.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalTablesNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInternalFunction s6xObject in slInternalFunctions.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes[TreeCategInternalFunctionsNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }
            foreach (S6xRoutineInternalScalar s6xObject in slInternalScalars.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                tnNode.Text = s6xObject.Label;
                tnNode.ToolTipText = s6xObject.Comments;
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
                    inputTableSignedCheckBox.Checked = sigInpTbl.SignedOutput;
                    inputTableWordCheckBox.Checked = sigInpTbl.WordOutput;
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
                    inputFunctionSignedInputCheckBox.Checked = sigInpFunc.SignedInput;
                    inputFunctionSignedOutputCheckBox.Checked = sigInpFunc.SignedOutput;
                    inputFunctionUnitsInputTextBox.Text = sigInpFunc.ForcedInputUnits;
                    inputFunctionUnitsOutputTextBox.Text = sigInpFunc.ForcedOutputUnits;
                    sigInpFunc = null;
                    break;
                case TreeCategInputScalarsNodeName:
                    S6xRoutineInputScalar sigInpScal = (S6xRoutineInputScalar)slInputScalars[currentTreeNode.Name];
                    inputScalarAddrTextBox.Text = sigInpScal.VariableAddress;
                    inputScalarByteCheckBox.Checked = sigInpScal.Byte;
                    inputScalarScaleTextBox.Text = sigInpScal.ForcedScaleExpression;
                    inputScalarSignedCheckBox.Checked = sigInpScal.Signed;
                    inputScalarUnitsTextBox.Text = sigInpScal.ForcedUnits;
                    inputScalarBitFlagsCheckBox.Checked = sigInpScal.isBitFlags;

                    inputScalarBitFlagsButton.Tag = null;
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
                    internalTableSignedCheckBox.Checked = sigIntTbl.SignedOutput;
                    internalTableSLabelTextBox.Text = sigIntTbl.ShortLabel;
                    internalTableWordCheckBox.Checked = sigIntTbl.WordOutput;
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
                    internalFunctionSignedInputCheckBox.Checked = sigIntFunc.SignedInput;
                    internalFunctionSignedOutputCheckBox.Checked = sigIntFunc.SignedOutput;
                    internalFunctionSLabelTextBox.Text = sigIntFunc.ShortLabel;
                    internalFunctionUnitsInputTextBox.Text = sigIntFunc.InputUnits;
                    internalFunctionUnitsOutputTextBox.Text = sigIntFunc.OutputUnits;
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
                    internalScalarScaleTextBox.Text = sigIntScal.ScaleExpression;
                    internalScalarSignedCheckBox.Checked = sigIntScal.Signed;
                    internalScalarSLabelTextBox.Text = sigIntScal.ShortLabel;
                    internalScalarUnitsTextBox.Text = sigIntScal.Units;
                    internalScalarBitFlagsCheckBox.Checked = sigIntScal.isBitFlags;

                    internalScalarBitFlagsButton.Tag = null;
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
            
            currentTreeNode.Parent.Nodes.Remove(currentTreeNode);
            currentTreeNode = null;
        }

        private void updateElem()
        {
            string categ = string.Empty;
            string uniqueKey = string.Empty;
            string label = string.Empty;
            string comments = string.Empty;

            switch (elemTabControl.SelectedTab.Name)
            {
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
                    sigInpFunc.ForcedInputUnits = inputFunctionUnitsInputTextBox.Text;
                    sigInpFunc.ForcedOutputUnits = inputFunctionUnitsOutputTextBox.Text;

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
                    sigInpScal.ForcedUnits = inputScalarUnitsTextBox.Text;

                    if (inputScalarBitFlagsButton.Tag != null)
                    {
                        S6xScalar s6xScalar = (S6xScalar)inputScalarBitFlagsButton.Tag;
                        inputScalarBitFlagsButton.Tag = null;
                        if (s6xScalar.BitFlags != null) sigInpScal.BitFlags = (S6xBitFlag[])s6xScalar.BitFlags.Clone();
                        s6xScalar = null;
                    }

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

                    uniqueKey = sigIntStr.UniqueKey;
                    label = sigIntStr.Label;
                    comments = sigIntStr.Comments;

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
                    sigIntTbl.SignedOutput = internalTableSignedCheckBox.Checked;
                    sigIntTbl.ShortLabel = internalTableSLabelTextBox.Text;
                    sigIntTbl.WordOutput = internalTableWordCheckBox.Checked;

                    uniqueKey = sigIntTbl.UniqueKey;
                    label = sigIntTbl.Label;
                    comments = sigIntTbl.Comments;

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
                    sigIntFunc.SignedInput = internalFunctionSignedInputCheckBox.Checked;
                    sigIntFunc.SignedOutput = internalFunctionSignedOutputCheckBox.Checked;
                    sigIntFunc.ShortLabel = internalFunctionSLabelTextBox.Text;
                    sigIntFunc.InputUnits = internalFunctionUnitsInputTextBox.Text;
                    sigIntFunc.OutputUnits = internalFunctionUnitsOutputTextBox.Text;

                    uniqueKey = sigIntFunc.UniqueKey;
                    label = sigIntFunc.Label;
                    comments = sigIntFunc.Comments;

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
                    sigIntScal.ScaleExpression = internalScalarScaleTextBox.Text;
                    sigIntScal.Signed = internalScalarSignedCheckBox.Checked;
                    sigIntScal.ShortLabel = internalScalarSLabelTextBox.Text;
                    sigIntScal.Units = internalScalarUnitsTextBox.Text;

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
            currentTreeNode.Text = label;
            currentTreeNode.ToolTipText = comments;

            clearElem();

            advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName];
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
                    checkPassed &= checkScaleExpression(inputTableScaleTextBox.Text);
                    return checkPassed;
                case TreeCategInputFunctionsNodeName:
                    checkPassed &= checkVariableValue(inputFunctionAddrTextBox.Text);
                    checkPassed &= checkVariableValue(inputFunctionInputTextBox.Text);
                    checkPassed &= checkVariableValue(inputFunctionOutputTextBox.Text);
                    checkPassed &= checkForcedColsRowsNumber(inputFunctionRowsTextBox.Text);
                    checkPassed &= checkScaleExpression(inputFunctionScaleInputTextBox.Text);
                    checkPassed &= checkScaleExpression(inputFunctionScaleOutputTextBox.Text);
                    return checkPassed;
                case TreeCategInputScalarsNodeName:
                    checkPassed &= checkVariableValue(inputScalarAddrTextBox.Text);
                    checkPassed &= checkScaleExpression(inputScalarScaleTextBox.Text);
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
                    checkPassed &= checkScaleExpression(internalTableScaleTextBox.Text);
                    return checkPassed;
                case TreeCategInternalFunctionsNodeName:
                    checkPassed &= checkVariableValue(internalFunctionAddrTextBox.Text);
                    checkPassed &= checkColsRowsNumber(internalFunctionRowsTextBox.Text);
                    checkPassed &= checkScaleExpression(internalFunctionScaleInputTextBox.Text);
                    checkPassed &= checkScaleExpression(internalFunctionScaleOutputTextBox.Text);
                    return checkPassed;
                case TreeCategInternalScalarsNodeName:
                    checkPassed &= checkVariableValue(internalScalarAddrTextBox.Text);
                    checkPassed &= checkScaleExpression(internalScalarScaleTextBox.Text);
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
                if (num <= 0 || num > 99) return false;
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

        private void inputScalarBitFlagsButton_Click(object sender, EventArgs e)
        {
            string uniqueAddress = string.Empty;
            S6xScalar tempScalar = null;
            S6xScalar s6xScalar = null;

            if (currentTreeNode == null) return;

            if (currentTreeNode.Parent != null)
            {
                if (currentTreeNode.Parent.Name != TreeCategInputScalarsNodeName) return;
            }

            S6xRoutineInputScalar sigInpScal = (S6xRoutineInputScalar)slInputScalars[currentTreeNode.Name];

            if (inputScalarBitFlagsButton.Tag != null)
            {
                s6xScalar = (S6xScalar)inputScalarBitFlagsButton.Tag;
            }

            if (s6xScalar == null)
            {
                tempScalar = new S6xScalar();
                if (sigInpScal != null)
                {
                    if (sigInpScal.BitFlags != null) tempScalar.BitFlags = (S6xBitFlag[])sigInpScal.BitFlags.Clone();
                }
            }
            else
            {
                tempScalar = s6xScalar.Clone();
            }
            s6xScalar = null;

            tempScalar.Label = "Input Scalar";
            tempScalar.Byte = inputScalarByteCheckBox.Checked;

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref S6x, ref tempScalar);
            bitFlagsForm.ShowDialog();
            bitFlagsForm = null;

            inputScalarBitFlagsCheckBox.Checked = tempScalar.isBitFlags;

            // To be reused on Update
            inputScalarBitFlagsButton.Tag = tempScalar;

            tempScalar = null;
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

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref S6x, ref tempScalar);
            bitFlagsForm.ShowDialog();
            bitFlagsForm = null;

            internalScalarBitFlagsCheckBox.Checked = tempScalar.isBitFlags;

            // To be reused on Update
            internalScalarBitFlagsButton.Tag = tempScalar;

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
