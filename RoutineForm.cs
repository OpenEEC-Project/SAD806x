using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class RoutineForm : Form
    {
        private const string TreeRootNodeName = "ROUTINE";
        private const string TreeCategInputArgumentsNodeName = "INPARGUMENTS";
        private const string TreeCategInputStructuresNodeName = "INPSTRUCTURES";
        private const string TreeCategInputTablesNodeName = "INPTABLES";
        private const string TreeCategInputFunctionsNodeName = "INPFUNCTIONS";
        private const string TreeCategInputScalarsNodeName = "INPSCALARS";

        private SADS6x S6x = null;
        private S6xRoutine s6xRoutine = null;

        private SortedList slInputArguments = null;
        private SortedList slInputStructures = null;
        private SortedList slInputTables = null;
        private SortedList slInputFunctions = null;
        private SortedList slInputScalars = null;

        private TreeNode currentTreeNode = null;

        private Repository repoUnits = null;

        private RepositoryConversion repoConversion = null;

        public RoutineForm(ref SADS6x s6x, ref S6xRoutine routine)
        {
            S6x = s6x;
            s6xRoutine = routine;

            slInputArguments = new SortedList();
            slInputStructures = new SortedList();
            slInputTables = new SortedList();
            slInputFunctions = new SortedList();
            slInputScalars = new SortedList();

            if (s6xRoutine.InputArguments != null) foreach (S6xRoutineInputArgument s6xObject in s6xRoutine.InputArguments) slInputArguments.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xRoutine.InputFunctions != null) foreach (S6xRoutineInputFunction s6xObject in s6xRoutine.InputFunctions) slInputFunctions.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xRoutine.InputScalars != null) foreach (S6xRoutineInputScalar s6xObject in s6xRoutine.InputScalars) slInputScalars.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xRoutine.InputStructures != null) foreach (S6xRoutineInputStructure s6xObject in s6xRoutine.InputStructures) slInputStructures.Add(s6xObject.UniqueKey, s6xObject);
            if (s6xRoutine.InputTables != null) foreach (S6xRoutineInputTable s6xObject in s6xRoutine.InputTables) slInputTables.Add(s6xObject.UniqueKey, s6xObject);

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            InitializeComponent();
        }

        private void RoutineForm_Load(object sender, EventArgs e)
        {
            advElemsTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(advElemsTreeView_NodeMouseClick);
            advElemsTreeView.AfterSelect += new TreeViewEventHandler(advElemsTreeView_AfterSelect);

            advElemsContextMenuStrip.Opening += new CancelEventHandler(advElemsContextMenuStrip_Opening);

            inputArgPositionTextBox.TextChanged += new EventHandler(inputArgPositionTextBox_TextChanged);
            
            inputArgEncryptionComboBox.DataSource = Enum.GetValues(typeof(CallArgsMode));
            inputArgEncryptionComboBox.SelectedItem = CallArgsMode.Standard;

            advLabelTextBox.Text = s6xRoutine.Label;
            advSLabelTextBox.Text = s6xRoutine.UniqueAddressHex;

            inputStructureTipPictureBox.Tag = Tools.StructureTip();
            inputStructureTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            inputStructureTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            repoContextMenuStrip.Opening += new CancelEventHandler(repoContextMenuStrip_Opening);
            repoToolStripTextBox.TextChanged += new EventHandler(repoToolStripTextBox_TextChanged);
            repoToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(repoToolStripMenuItem_DropDownItemClicked);

            Control.ControlCollection controls = null;
            controls = (Control.ControlCollection)inputTableTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputFunctionTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputScalarTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputStructureTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)inputArgumentTabPage.Controls;
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
            if (((Control)sender).Tag == null)

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

            advElemsTreeView.ExpandAll();

        }

        private void updateArray(string categ)
        {
            switch (categ)
            {
                case TreeCategInputArgumentsNodeName:
                    if (slInputArguments.Count == 0) s6xRoutine.InputArguments = null;
                    else
                    {
                        s6xRoutine.InputArguments = new S6xRoutineInputArgument[slInputArguments.Count];
                        slInputArguments.Values.CopyTo(s6xRoutine.InputArguments, 0);
                    }
                    break;
                case TreeCategInputStructuresNodeName:
                    if (slInputStructures.Count == 0) s6xRoutine.InputStructures = null;
                    else
                    {
                        s6xRoutine.InputStructures = new S6xRoutineInputStructure[slInputStructures.Count];
                        slInputStructures.Values.CopyTo(s6xRoutine.InputStructures, 0);
                    }
                    break;
                case TreeCategInputTablesNodeName:
                    if (slInputTables.Count == 0) s6xRoutine.InputTables = null;
                    else
                    {
                        s6xRoutine.InputTables = new S6xRoutineInputTable[slInputTables.Count];
                        slInputTables.Values.CopyTo(s6xRoutine.InputTables, 0);
                    }
                    break;
                case TreeCategInputFunctionsNodeName:
                    if (slInputFunctions.Count == 0) s6xRoutine.InputFunctions = null;
                    else
                    {
                        s6xRoutine.InputFunctions = new S6xRoutineInputFunction[slInputFunctions.Count];
                        slInputFunctions.Values.CopyTo(s6xRoutine.InputFunctions, 0);
                    }
                    break;
                case TreeCategInputScalarsNodeName:
                    if (slInputScalars.Count == 0) s6xRoutine.InputScalars = null;
                    else
                    {
                        s6xRoutine.InputScalars = new S6xRoutineInputScalar[slInputScalars.Count];
                        slInputScalars.Values.CopyTo(s6xRoutine.InputScalars, 0);
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
                    sigInpScal = null;
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

                    uniqueKey = sigInpScal.UniqueKey;
                    label = sigInpScal.VariableAddress;

                    sigInpScal = null;
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
                    checkPassed &= checkColsRowsNumber(inputArgPositionTextBox.Text);
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
            cnt += slInputStructures.Count;
            cnt += slInputTables.Count;
            cnt += slInputFunctions.Count;
            cnt += slInputScalars.Count;

            string uniqueKey = string.Empty;
            while (uniqueKey == string.Empty || !isValid)
            {
                isValid = true;
                uniqueKey = string.Format("Ra{0:d3}", cnt);
                cnt++;

                if (isValid) isValid &= !slInputArguments.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputStructures.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputTables.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputFunctions.ContainsKey(uniqueKey);
                if (isValid) isValid &= !slInputScalars.ContainsKey(uniqueKey);
            }
            return uniqueKey;
        }

        private void initRepositories()
        {
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
                    repoContextMenuStrip.Tag = sender.Name;
                    break;
                default:
                    return;
            }
            switch (repoContextMenuStrip.Tag.ToString())
            {
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
                case "inputTableCellsUnitsTextBox":
                case "inputTableColsUnitsTextBox":
                case "inputTableRowsUnitsTextBox":
                case "inputFunctionUnitsInputTextBox":
                case "inputFunctionUnitsOutputTextBox":
                case "inputScalarUnitsTextBox":
                    repoRepository = repoUnits;
                    break;
                case "inputTableScaleTextBox":
                case "inputFunctionScaleInputTextBox":
                case "inputFunctionScaleOutputTextBox":
                case "inputScalarScaleTextBox":
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
                default:
                    repoContextMenuStrip.Tag = null;
                    return;
            }
        }
    }
}
