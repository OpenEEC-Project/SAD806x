using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class ElemSigForm : Form
    {
        private const string TreeCategStructureNodeName = "STRUCTURE";
        private const string TreeCategTableNodeName = "TABLE";
        private const string TreeCategFunctionNodeName = "FUNCTION";
        private const string TreeCategScalarNodeName = "SCALAR";

        private SADS6x S6x = null;
        private S6xElementSignature s6xESig = null;

        private TreeNode currentTreeNode = null;

        private Repository repoTables = null;
        private Repository repoFunctions = null;
        private Repository repoScalars = null;
        private Repository repoStructures = null;

        private Repository repoUnits = null;

        private RepositoryConversion repoConversion = null;

        public ElemSigForm(ref SADS6x s6x, ref S6xElementSignature eSig)
        {
            S6x = s6x;
            s6xESig = eSig;

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            InitializeComponent();
        }

        private void ElemSigForm_Load(object sender, EventArgs e)
        {
            advElemsTreeView.BeforeCheck += new TreeViewCancelEventHandler(advElemsTreeView_BeforeCheck);
            advElemsTreeView.AfterCheck += new TreeViewEventHandler(advElemsTreeView_AfterCheck);

            advSigTextBox.TextChanged += new EventHandler(advSigTextBox_TextChanged);
            
            advLabelTextBox.Text = s6xESig.SignatureLabel;
            advSigTextBox.Text = s6xESig.Signature;

            mainTipPictureBox.Tag = SharedUI.ElementSignatureTip();
            mainTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            mainTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            structureTipPictureBox.Tag = SharedUI.StructureTip();
            structureTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            structureTipPictureBox.Click += new EventHandler(TipPictureBox_Click);

            repoContextMenuStrip.Opening += new CancelEventHandler(repoContextMenuStrip_Opening);
            repoToolStripTextBox.TextChanged += new EventHandler(repoToolStripTextBox_TextChanged);
            repoToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(repoToolStripMenuItem_DropDownItemClicked);

            scalarScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            scalarScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            scalarScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            functionScalePrecInputNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            functionScalePrecInputNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            functionScalePrecInputNumericUpDown.Value = SADDef.DefaultScalePrecision;
            functionScalePrecOutputNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            functionScalePrecOutputNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            functionScalePrecOutputNumericUpDown.Value = SADDef.DefaultScalePrecision;

            tableScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            tableScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            tableScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            Control.ControlCollection controls = null;
            controls = (Control.ControlCollection)elemHeaderPanel.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)advSigPanel.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)structureTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)tableTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)functionTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)scalarTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = null;

            initRepositories();

            loadElemsTreeView();
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

        private void advElemsTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == currentTreeNode) e.Cancel = true;
        }

        private void advElemsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                currentTreeNode = e.Node;

                foreach (TreeNode tnNode in advElemsTreeView.Nodes) if (tnNode != currentTreeNode && tnNode.Checked) tnNode.Checked = false;

                switch (currentTreeNode.Name)
                {
                    case TreeCategScalarNodeName:
                        if (s6xESig.Scalar == null)
                        {
                            s6xESig.Scalar = new S6xRoutineInternalScalar();
                            s6xESig.Scalar.Label = "New Scalar";
                            s6xESig.Scalar.ShortLabel = SADDef.ShortScalarPrefix + string.Format("{0:d4}", S6x.slScalars.Count + 1);
                            s6xESig.Scalar.Byte = true;
                            s6xESig.Scalar.ScaleExpression = "X";
                            s6xESig.Scalar.ScalePrecision = SADDef.DefaultScalePrecision;
                        }
                        s6xESig.Function = null;
                        s6xESig.Table = null;
                        s6xESig.Structure = null;
                        break;
                    case TreeCategFunctionNodeName:
                        if (s6xESig.Function == null)
                        {
                            s6xESig.Function = new S6xRoutineInternalFunction();
                            s6xESig.Function.Label = "New Function";
                            s6xESig.Function.ShortLabel = SADDef.ShortFunctionPrefix + string.Format("{0:d3}", S6x.slFunctions.Count + 1);
                            s6xESig.Function.RowsNumber = 0;
                            s6xESig.Function.ByteInput = true;
                            s6xESig.Function.ByteOutput = true;
                            s6xESig.Function.InputScaleExpression = "X";
                            s6xESig.Function.OutputScaleExpression = "X";
                            s6xESig.Function.InputScalePrecision = SADDef.DefaultScalePrecision;
                            s6xESig.Function.OutputScalePrecision = SADDef.DefaultScalePrecision;
                        }
                        s6xESig.Scalar = null;
                        s6xESig.Table = null;
                        s6xESig.Structure = null;
                        break;
                    case TreeCategTableNodeName:
                        if (s6xESig.Table == null)
                        {
                            s6xESig.Table = new S6xRoutineInternalTable();
                            s6xESig.Table.Label = "New Table";
                            s6xESig.Table.ShortLabel = SADDef.ShortTablePrefix + string.Format("{0:d3}", S6x.slTables.Count + 1);
                            s6xESig.Table.VariableColsNumber = "0";
                            s6xESig.Table.RowsNumber = 0;
                            s6xESig.Table.CellsScaleExpression = "X";
                            s6xESig.Table.CellsScalePrecision = SADDef.DefaultScalePrecision;
                        }
                        s6xESig.Scalar = null;
                        s6xESig.Function = null;
                        s6xESig.Structure = null;
                        break;
                    case TreeCategStructureNodeName:
                        if (s6xESig.Structure == null)
                        {
                            s6xESig.Structure = new S6xRoutineInternalStructure();
                            s6xESig.Structure.Label = "New Structure";
                            s6xESig.Structure.ShortLabel = SADDef.ShortStructurePrefix + string.Format("{0:d3}", S6x.slStructures.Count + 1);
                            s6xESig.Structure.Number = 0;
                        }
                        s6xESig.Scalar = null;
                        s6xESig.Function = null;
                        s6xESig.Table = null;
                        break;
                }
                
                showCateg();
            }
        }

        private void elemUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void advSigTextBox_TextChanged(object sender, EventArgs e)
        {
            s6xESig.Signature = advSigTextBox.Text;
        }

        private void loadElemsTreeView()
        {
            if (s6xESig.Scalar != null) advElemsTreeView.Nodes[TreeCategScalarNodeName].Checked = true;
            else if (s6xESig.Function != null) advElemsTreeView.Nodes[TreeCategFunctionNodeName].Checked = true;
            else if (s6xESig.Table != null) advElemsTreeView.Nodes[TreeCategTableNodeName].Checked = true;
            else if (s6xESig.Structure != null) advElemsTreeView.Nodes[TreeCategStructureNodeName].Checked = true;
        }

        private void showCateg()
        {
            TabPage selectedTabPage = null;
            ArrayList removeTabPages = null;

            if (currentTreeNode == null) return;
            switch (currentTreeNode.Name)
            {
                case TreeCategStructureNodeName:
                    selectedTabPage = structureTabPage;
                    break;
                case TreeCategTableNodeName:
                    selectedTabPage = tableTabPage;
                    break;
                case TreeCategFunctionNodeName:
                    selectedTabPage = functionTabPage;
                    break;
                case TreeCategScalarNodeName:
                    selectedTabPage = scalarTabPage;
                    break;
                default:
                    return;
            }

            removeTabPages = new ArrayList();
            foreach (TabPage tabPage in elemTabControl.TabPages)
            {
                if (tabPage != selectedTabPage) removeTabPages.Add(tabPage);
            }
            foreach (TabPage tabPage in removeTabPages) elemTabControl.TabPages.Remove(tabPage);
            removeTabPages = null;

            if (selectedTabPage != null)
            {
                if (!elemTabControl.TabPages.Contains(selectedTabPage)) elemTabControl.TabPages.Add(selectedTabPage);
            }
            elemTabControl.SelectedTab = selectedTabPage;

            selectedTabPage = null;

            showElem();
        }

        private void showElem()
        {
            if (currentTreeNode == null) return;
            if (!currentTreeNode.Checked) return;

            if (s6xESig.for8061) for8061ComboBox.SelectedIndex = 0;
            else for8061ComboBox.SelectedIndex = 1;
            forBankTextBox.Text = (string)s6xESig.forBankNum;

            switch (currentTreeNode.Name)
            {
                case TreeCategStructureNodeName:
                    S6xRoutineInternalStructure sigIntStr = s6xESig.Structure;
                    structureNumTextBox.Text = sigIntStr.Number.ToString();

                    // Windows 10 1809 (10.0.17763) Issue
                    structureStructTextBox.Clear();
                    structureStructTextBox.Multiline = false;
                    structureStructTextBox.Multiline = true;

                    structureStructTextBox.Text = sigIntStr.StructDef;

                    shortLabelTextBox.Text = sigIntStr.ShortLabel;
                    labelTextBox.Text = sigIntStr.Label;

                    // Windows 10 1809 (10.0.17763) Issue
                    commentsTextBox.Clear();
                    commentsTextBox.Multiline = false;
                    commentsTextBox.Multiline = true;
                    
                    commentsTextBox.Text = sigIntStr.Comments;
                    outputCommentsCheckBox.Checked = sigIntStr.OutputComments;

                    sigIntStr = null;
                    break;
                case TreeCategTableNodeName:
                    S6xRoutineInternalTable sigIntTbl = s6xESig.Table;
                    tableCellsUnitsTextBox.Text = sigIntTbl.CellsUnits;
                    tableColsTextBox.Text = sigIntTbl.VariableColsNumber;
                    tableColsUnitsTextBox.Text = sigIntTbl.ColsUnits;
                    tableRowsTextBox.Text = sigIntTbl.RowsNumber.ToString();
                    tableRowsUnitsTextBox.Text = sigIntTbl.RowsUnits;
                    tableScaleTextBox.Text = sigIntTbl.CellsScaleExpression;
                    tableScalePrecNumericUpDown.Value = sigIntTbl.CellsScalePrecision;
                    tableSignedCheckBox.Checked = sigIntTbl.SignedOutput;
                    tableWordCheckBox.Checked = sigIntTbl.WordOutput;
                    
                    shortLabelTextBox.Text = sigIntTbl.ShortLabel;
                    labelTextBox.Text = sigIntTbl.Label;

                    // Windows 10 1809 (10.0.17763) Issue
                    commentsTextBox.Clear();
                    commentsTextBox.Multiline = false;
                    commentsTextBox.Multiline = true;
                    
                    commentsTextBox.Text = sigIntTbl.Comments;
                    outputCommentsCheckBox.Checked = sigIntTbl.OutputComments;

                    sigIntTbl = null;
                    break;
                case TreeCategFunctionNodeName:
                    S6xRoutineInternalFunction sigIntFunc = s6xESig.Function;
                    functionByteCheckBox.Checked = sigIntFunc.ByteInput;
                    functionRowsTextBox.Text = sigIntFunc.RowsNumber.ToString();
                    functionScaleInputTextBox.Text = sigIntFunc.InputScaleExpression;
                    functionScaleOutputTextBox.Text = sigIntFunc.OutputScaleExpression;
                    functionScalePrecInputNumericUpDown.Value = sigIntFunc.InputScalePrecision;
                    functionScalePrecOutputNumericUpDown.Value = sigIntFunc.OutputScalePrecision;
                    functionSignedInputCheckBox.Checked = sigIntFunc.SignedInput;
                    functionSignedOutputCheckBox.Checked = sigIntFunc.SignedOutput;
                    functionUnitsInputTextBox.Text = sigIntFunc.InputUnits;
                    functionUnitsOutputTextBox.Text = sigIntFunc.OutputUnits;

                    shortLabelTextBox.Text = sigIntFunc.ShortLabel;
                    labelTextBox.Text = sigIntFunc.Label;

                    // Windows 10 1809 (10.0.17763) Issue
                    commentsTextBox.Clear();
                    commentsTextBox.Multiline = false;
                    commentsTextBox.Multiline = true;

                    commentsTextBox.Text = sigIntFunc.Comments;
                    outputCommentsCheckBox.Checked = sigIntFunc.OutputComments;

                    sigIntFunc = null;
                    break;
                case TreeCategScalarNodeName:
                    S6xRoutineInternalScalar sigIntScal = s6xESig.Scalar;
                    scalarBitFlagsCheckBox.Checked = sigIntScal.isBitFlags;
                    scalarByteCheckBox.Checked = sigIntScal.Byte;
                    scalarScaleTextBox.Text = sigIntScal.ScaleExpression;
                    scalarScalePrecNumericUpDown.Value = sigIntScal.ScalePrecision;
                    scalarSignedCheckBox.Checked = sigIntScal.Signed;
                    scalarUnitsTextBox.Text = sigIntScal.Units;

                    shortLabelTextBox.Text = sigIntScal.ShortLabel;
                    labelTextBox.Text = sigIntScal.Label;

                    // Windows 10 1809 (10.0.17763) Issue
                    commentsTextBox.Clear();
                    commentsTextBox.Multiline = false;
                    commentsTextBox.Multiline = true;
                    
                    commentsTextBox.Text = sigIntScal.Comments;
                    outputCommentsCheckBox.Checked = sigIntScal.OutputComments;
                    scalarInlineCommentsCheckBox.Checked = sigIntScal.InlineComments;

                    scalarBitFlagsButton.Tag = null;

                    sigIntScal = null;
                    break;
                default:
                    return;
            }
        }

        private void updateElem()
        {
            string categ = string.Empty;
            string uniqueKey = string.Empty;
            string label = string.Empty;
            string comments = string.Empty;

            switch (elemTabControl.SelectedTab.Name)
            {
                case "structureTabPage":
                    categ = TreeCategStructureNodeName;
                    break;
                case "tableTabPage":
                    categ = TreeCategTableNodeName;
                    break;
                case "functionTabPage":
                    categ = TreeCategFunctionNodeName;
                    break;
                case "scalarTabPage":
                    categ = TreeCategScalarNodeName;
                    break;
                default:
                    return;
            }

            if (!checkElem(categ))
            {
                MessageBox.Show("Invalid values are present, please correct them to continue.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            s6xESig.for8061 = (for8061ComboBox.SelectedIndex == 0);
            s6xESig.forBankNum = forBankTextBox.Text;
            
            switch (categ)
            {
                case TreeCategStructureNodeName:
                    S6xRoutineInternalStructure sigIntStr = s6xESig.Structure;
                    sigIntStr.Number = Convert.ToInt32(structureNumTextBox.Text);
                    sigIntStr.StructDef = structureStructTextBox.Text;

                    sigIntStr.ShortLabel = shortLabelTextBox.Text;
                    sigIntStr.Label = labelTextBox.Text;
                    sigIntStr.Comments = commentsTextBox.Text;
                    sigIntStr.OutputComments = outputCommentsCheckBox.Checked;

                    uniqueKey = sigIntStr.UniqueKey;
                    label = sigIntStr.Label;
                    comments = sigIntStr.Comments;

                    sigIntStr = null;
                    break;
                case TreeCategTableNodeName:
                    S6xRoutineInternalTable sigIntTbl = s6xESig.Table;
                    sigIntTbl.VariableColsNumber = tableColsTextBox.Text;
                    sigIntTbl.CellsUnits = tableCellsUnitsTextBox.Text;
                    sigIntTbl.ColsUnits = tableColsUnitsTextBox.Text;
                    sigIntTbl.RowsNumber = Convert.ToInt32(tableRowsTextBox.Text);
                    sigIntTbl.RowsUnits = tableRowsUnitsTextBox.Text;
                    sigIntTbl.CellsScaleExpression = tableScaleTextBox.Text;
                    sigIntTbl.CellsScalePrecision = (int)tableScalePrecNumericUpDown.Value;
                    sigIntTbl.SignedOutput = tableSignedCheckBox.Checked;
                    sigIntTbl.WordOutput = tableWordCheckBox.Checked;

                    sigIntTbl.ShortLabel = shortLabelTextBox.Text;
                    sigIntTbl.Label = labelTextBox.Text;
                    sigIntTbl.Comments = commentsTextBox.Text;
                    sigIntTbl.OutputComments = outputCommentsCheckBox.Checked;

                    uniqueKey = sigIntTbl.UniqueKey;
                    label = sigIntTbl.Label;
                    comments = sigIntTbl.Comments;

                    sigIntTbl = null;
                    break;
                case TreeCategFunctionNodeName:
                    S6xRoutineInternalFunction sigIntFunc = s6xESig.Function;
                    sigIntFunc.ByteInput = functionByteCheckBox.Checked;
                    sigIntFunc.RowsNumber = Convert.ToInt32(functionRowsTextBox.Text);
                    sigIntFunc.InputScaleExpression = functionScaleInputTextBox.Text;
                    sigIntFunc.OutputScaleExpression = functionScaleOutputTextBox.Text;
                    sigIntFunc.InputScalePrecision = (int)functionScalePrecInputNumericUpDown.Value;
                    sigIntFunc.OutputScalePrecision = (int)functionScalePrecOutputNumericUpDown.Value;
                    sigIntFunc.SignedInput = functionSignedInputCheckBox.Checked;
                    sigIntFunc.SignedOutput = functionSignedOutputCheckBox.Checked;
                    sigIntFunc.InputUnits = functionUnitsInputTextBox.Text;
                    sigIntFunc.OutputUnits = functionUnitsOutputTextBox.Text;

                    sigIntFunc.ShortLabel = shortLabelTextBox.Text;
                    sigIntFunc.Label = labelTextBox.Text;
                    sigIntFunc.Comments = commentsTextBox.Text;
                    sigIntFunc.OutputComments = outputCommentsCheckBox.Checked;

                    uniqueKey = sigIntFunc.UniqueKey;
                    label = sigIntFunc.Label;
                    comments = sigIntFunc.Comments;

                    sigIntFunc = null;
                    break;
                case TreeCategScalarNodeName:
                    S6xRoutineInternalScalar sigIntScal = s6xESig.Scalar;
                    sigIntScal.Byte = scalarByteCheckBox.Checked;
                    sigIntScal.ScaleExpression = scalarScaleTextBox.Text;
                    sigIntScal.ScalePrecision = (int)scalarScalePrecNumericUpDown.Value;
                    sigIntScal.Signed = scalarSignedCheckBox.Checked;
                    sigIntScal.Units = scalarUnitsTextBox.Text;

                    sigIntScal.ShortLabel = shortLabelTextBox.Text;
                    sigIntScal.Label = labelTextBox.Text;
                    sigIntScal.Comments = commentsTextBox.Text;
                    sigIntScal.OutputComments = outputCommentsCheckBox.Checked;
                    sigIntScal.InlineComments = scalarInlineCommentsCheckBox.Checked;

                    uniqueKey = sigIntScal.UniqueKey;
                    label = sigIntScal.Label;
                    comments = sigIntScal.Comments;

                    if (scalarBitFlagsButton.Tag != null)
                    {
                        S6xScalar s6xScalar = (S6xScalar)scalarBitFlagsButton.Tag;
                        scalarBitFlagsButton.Tag = null;
                        if (s6xScalar.BitFlags != null) sigIntScal.BitFlags = (S6xBitFlag[])s6xScalar.BitFlags.Clone();
                        s6xScalar = null;
                    }

                    sigIntScal = null;
                    break;
                default:
                    return;
            }
        }

        private bool checkElem(string categ)
        {
            bool checkPassed = true;

            switch (categ)
            {
                case TreeCategStructureNodeName:
                    checkPassed &= checkColsRowsNumber(structureNumTextBox.Text);
                    checkPassed &= checkStructDef(structureStructTextBox.Text);
                    return checkPassed;
                case TreeCategTableNodeName:
                    checkPassed &= checkVariableValue(tableColsTextBox.Text);
                    checkPassed &= checkColsRowsNumber(tableRowsTextBox.Text);
                    checkPassed &= checkScaleExpression(tableScaleTextBox.Text);
                    return checkPassed;
                case TreeCategFunctionNodeName:
                    checkPassed &= checkColsRowsNumber(functionRowsTextBox.Text);
                    checkPassed &= checkScaleExpression(functionScaleInputTextBox.Text);
                    checkPassed &= checkScaleExpression(functionScaleOutputTextBox.Text);
                    return checkPassed;
                case TreeCategScalarNodeName:
                    checkPassed &= checkScaleExpression(scalarScaleTextBox.Text);
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

        private void scalarBitFlagsButton_Click(object sender, EventArgs e)
        {
            string uniqueAddress = string.Empty;
            S6xScalar tempScalar = null;
            S6xScalar s6xScalar = null;

            if (scalarBitFlagsButton.Tag != null)
            {
                s6xScalar = (S6xScalar)scalarBitFlagsButton.Tag;
            }

            if (s6xScalar == null)
            {
                tempScalar = new S6xScalar();
                if (s6xESig.Scalar != null)
                {
                    if (s6xESig.Scalar.BitFlags != null) tempScalar.BitFlags = (S6xBitFlag[])s6xESig.Scalar.BitFlags.Clone();
                }
            }
            else
            {
                tempScalar = s6xScalar.Clone();
            }
            s6xScalar = null;

            tempScalar.Label = labelTextBox.Text;
            tempScalar.Byte = scalarByteCheckBox.Checked;

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref S6x, ref tempScalar);
            bitFlagsForm.ShowDialog();
            bitFlagsForm = null;

            scalarBitFlagsCheckBox.Checked = tempScalar.isBitFlags;

            // To be reused on Update
            scalarBitFlagsButton.Tag = tempScalar;

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
                case "tableCellsUnitsTextBox":
                case "tableColsUnitsTextBox":
                case "tableRowsUnitsTextBox":
                case "functionUnitsInputTextBox":
                case "functionUnitsOutputTextBox":
                case "scalarUnitsTextBox":
                case "tableScaleTextBox":
                case "functionScaleInputTextBox":
                case "functionScaleOutputTextBox":
                case "scalarScaleTextBox":
                    repoContextMenuStrip.Tag = sender.Name;
                    break;
                default:
                    repoContextMenuStrip.Tag = sender.Parent.Name;
                    break;
            }
            switch (repoContextMenuStrip.Tag.ToString())
            {
                case "elemHeaderPanel":
                    searchLabel = shortLabelTextBox.Text;
                    break;
                case "tableColsUnitsTextBox":
                    searchLabel = tableColsUnitsTextBox.Text;
                    break;
                case "tableRowsUnitsTextBox":
                    searchLabel = tableRowsUnitsTextBox.Text;
                    break;
                case "tableCellsUnitsTextBox":
                case "tableScaleTextBox":
                    searchLabel = tableCellsUnitsTextBox.Text;
                    break;
                case "functionUnitsInputTextBox":
                case "functionScaleInputTextBox":
                    searchLabel = functionUnitsInputTextBox.Text;
                    break;
                case "functionUnitsOutputTextBox":
                case "functionScaleOutputTextBox":
                    searchLabel = functionUnitsOutputTextBox.Text;
                    break;
                case "scalarUnitsTextBox":
                case "scalarScaleTextBox":
                    searchLabel = scalarUnitsTextBox.Text;
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
                case "elemHeaderPanel":
                    switch (elemTabControl.SelectedTab.Name)
                    {
                        case "tableTabPage":
                            repoRepository = repoTables;
                            break;
                        case "functionTabPage":
                            repoRepository = repoFunctions;
                            break;
                        case "scalarTabPage":
                            repoRepository = repoScalars;
                            break;
                        case "structureTabPage":
                            repoRepository = repoStructures;
                            break;
                        default:
                            return;
                    }
                    break;
                case "tableCellsUnitsTextBox":
                case "tableColsUnitsTextBox":
                case "tableRowsUnitsTextBox":
                case "functionUnitsInputTextBox":
                case "functionUnitsOutputTextBox":
                case "scalarUnitsTextBox":
                    repoRepository = repoUnits;
                    break;
                case "tableScaleTextBox":
                case "functionScaleInputTextBox":
                case "functionScaleOutputTextBox":
                case "scalarScaleTextBox":
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
                case "elemHeaderPanel":
                    shortLabelTextBox.Text = sSLabel;
                    labelTextBox.Text = sLLabel;
                    commentsTextBox.Text = sComments;
                    break;
                case "tableCellsUnitsTextBox":
                    tableCellsUnitsTextBox.Text = sSLabel;
                    break;
                case "tableColsUnitsTextBox":
                    tableColsUnitsTextBox.Text = sSLabel;
                    break;
                case "tableRowsUnitsTextBox":
                    tableRowsUnitsTextBox.Text = sSLabel;
                    break;
                case "functionUnitsInputTextBox":
                    functionUnitsInputTextBox.Text = sSLabel;
                    break;
                case "functionUnitsOutputTextBox":
                    functionUnitsOutputTextBox.Text = sSLabel;
                    break;
                case "scalarUnitsTextBox":
                    scalarUnitsTextBox.Text = sSLabel;
                    break;
                case "tableScaleTextBox":
                    tableScaleTextBox.Text = sLLabel;
                    break;
                case "functionScaleInputTextBox":
                    functionScaleInputTextBox.Text = sLLabel;
                    break;
                case "functionScaleOutputTextBox":
                    functionScaleOutputTextBox.Text = sLLabel;
                    break;
                case "scalarScaleTextBox":
                    scalarScaleTextBox.Text = sLLabel;
                    break;
                default:
                    repoContextMenuStrip.Tag = null;
                    return;
            }
        }
    }
}
