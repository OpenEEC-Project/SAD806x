using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SAD806x
{
    public partial class CompareForm : Form
    {
        private string compareMode = string.Empty;
        private bool binariesSameDefinition = false;
        private SADBin sadBin = null;
        private SADS6x sadS6x = null;
        private SADBin cmpSadBin = null;
        private SADS6x cmpSadS6x = null;
        private TreeView elemsTreeView = null;
        private ContextMenuStrip elemsContextMenuStrip = null;
        private ContextMenuStrip elemContextMenuStrip = null;
        private ElemDataForm elemDataForm = null;

        private System.Windows.Forms.Timer mainUpdateTimer = null;

        public CompareForm(ref SADBin mainSadBin, ref SADS6x mainSadS6x, ref TreeView mainElemsTreeView, ref ContextMenuStrip mainElemsContextMenuStrip, ref ContextMenuStrip mainElemContextMenuStrip, bool bBinaryComparison, bool bBinariesSameDefinition)
        {
            compareMode = bBinaryComparison ? "Bin" : "S6x";
            binariesSameDefinition = bBinariesSameDefinition;
            sadBin = mainSadBin;
            sadS6x = mainSadS6x;
            elemsTreeView = mainElemsTreeView;
            elemsContextMenuStrip = mainElemsContextMenuStrip; // For Nodes creation in Main Elems Tree View
            elemContextMenuStrip = mainElemContextMenuStrip; // For Options at ShowElemDataForm level

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            searchTreeView.StateImageList = elemsTreeView.StateImageList;

            mainUpdateTimer = new System.Windows.Forms.Timer();
            mainUpdateTimer.Enabled = false;
            mainUpdateTimer.Interval = 500;
            mainUpdateTimer.Tick += new EventHandler(mainUpdateTimer_Tick);

            if (compareMode == "Bin")
            {
                this.Text = "Compare Binaries";
                if (!binariesSameDefinition) this.Text += " (different definition)";
                searchButton.Text = "Select Binary to compare";
            }
            else
            {
                this.Text = "Compare SAD 806x";
                searchButton.Text = "Select SAD 806x to compare";
            }
            
            searchTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(searchTreeView_NodeMouseClick);
            searchTreeView.AfterSelect += new TreeViewEventHandler(searchTreeView_AfterSelect);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainUpdateTimer.Enabled = false;

            closeElemDataForm();
        }

        private void mainUpdateTimer_Tick(object sender, EventArgs e)
        {
            mainUpdateTimer.Enabled = false;

            try
            {
                bool refreshCount = false;

                TreeNodeCollection[] tnCollections = new TreeNodeCollection[] { };
                if (compareMode == "Bin") tnCollections = new TreeNodeCollection[] { searchTreeView.Nodes };
                else if (compareMode == "S6x") tnCollections = new TreeNodeCollection[] { searchTreeView.Nodes["MISSING_IN_COMPARED"].Nodes, searchTreeView.Nodes["DIFFERENCES"].Nodes };

                foreach (TreeNodeCollection tnCollection in tnCollections)
                {
                    foreach (TreeNode tnCateg in tnCollection)
                    {
                        // 20210114 - PYM - Now managing remaining addresses
                        if (tnCateg.Name == S6xNav.getHeaderCategName(S6xNavHeaderCategory.UNDEFINED)) continue;

                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[tnCateg.Name]);
                        if (!niMFHeaderCateg.isValid)
                        {
                            tnCateg.Nodes.Clear();
                            continue;
                        }

                        List<TreeNode> lsRemoval = new List<TreeNode>();

                        foreach (TreeNode tnNode in tnCateg.Nodes)
                        {
                            TreeNode tnMainNode = niMFHeaderCateg.FindElement(tnNode.Name);
                            if (tnMainNode == null)
                            {
                                lsRemoval.Add(tnNode);
                                continue;
                            }
                            if (tnMainNode.Text != tnNode.Text) tnNode.Text = tnMainNode.Text;
                            if (tnMainNode.ToolTipText != tnNode.ToolTipText) tnNode.ToolTipText = tnMainNode.ToolTipText;
                            if (tnMainNode.StateImageKey != tnNode.StateImageKey) tnNode.StateImageKey = tnMainNode.StateImageKey;
                        }

                        foreach (TreeNode tnNode in lsRemoval) tnCateg.Nodes.Remove(tnNode);
                        refreshCount |= lsRemoval.Count > 0;
                    }
                }

                if (refreshCount) searchTreeViewCount(compareMode);
                mainUpdateTimer.Enabled = true;
            }
            catch { }
        }

        private void searchTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void searchTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null) return;
            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[e.Node.Parent.Name]);
            if (!niMFHeaderCateg.isValid)
            {
                closeElemDataForm();
                return;
            }
            TreeNode tnMFNode = niMFHeaderCateg.FindElement(e.Node.Name);
            if (tnMFNode == null)
            {
                closeElemDataForm();
                return;
            }
            try
            {
                elemsTreeView.SelectedNode = tnMFNode;
                string differentDefinitionUniqueAddress = string.Empty;
                if (!binariesSameDefinition)
                {
                    if (e.Node.Tag != null) differentDefinitionUniqueAddress = e.Node.Tag.ToString();
                }
                showElemDataForm(new S6xNavInfo(tnMFNode), differentDefinitionUniqueAddress);
            }
            catch
            {
                closeElemDataForm();
            }
            tnMFNode = null;
            niMFHeaderCateg = null;
        }

        private void showElemDataForm(S6xNavInfo mfElemS6xNavInfo, string differentDefinitionUniqueAddress)
        {
            if (compareMode.ToUpper() != "BIN") return;

            if (cmpSadBin == null || mfElemS6xNavInfo == null)
            {
                closeElemDataForm();
                return;
            }

            object s6xObject = null;
            string uniqueAddress = (differentDefinitionUniqueAddress == string.Empty ? mfElemS6xNavInfo.Node.Name : differentDefinitionUniqueAddress);

            if (binariesSameDefinition) cmpSadBin.S6x = sadS6x;
            if (cmpSadBin.S6x != null)
            {
                switch (mfElemS6xNavInfo.HeaderCategory)
                {
                    case S6xNavHeaderCategory.SCALARS:
                        if (mfElemS6xNavInfo.isDuplicate) s6xObject = cmpSadBin.S6x.slDupScalars[uniqueAddress];
                        else s6xObject = cmpSadBin.S6x.slScalars[uniqueAddress];
                        break;
                    case S6xNavHeaderCategory.FUNCTIONS:
                        if (mfElemS6xNavInfo.isDuplicate) s6xObject = cmpSadBin.S6x.slDupFunctions[uniqueAddress];
                        else s6xObject = cmpSadBin.S6x.slFunctions[uniqueAddress];
                        break;
                    case S6xNavHeaderCategory.TABLES:
                        if (mfElemS6xNavInfo.isDuplicate) s6xObject = cmpSadBin.S6x.slDupTables[uniqueAddress];
                        else s6xObject = cmpSadBin.S6x.slTables[uniqueAddress];
                        break;
                    case S6xNavHeaderCategory.STRUCTURES:
                        if (mfElemS6xNavInfo.isDuplicate) s6xObject = cmpSadBin.S6x.slDupStructures[uniqueAddress];
                        else s6xObject = cmpSadBin.S6x.slStructures[uniqueAddress];
                        break;
                }
            }

            if (s6xObject == null)
            {
                closeElemDataForm();
                return;
            }

            if (elemDataForm == null)
            {
                elemDataForm = new ElemDataForm(ref cmpSadBin, s6xObject, ref elemContextMenuStrip);
                elemDataForm.FormClosed += new FormClosedEventHandler(elemDataForm_FormClosed);
            }
            else
            {
                elemDataForm.SetElemObject(s6xObject);
            }
            elemDataForm.Show();
        }

        private void elemDataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            elemDataForm = null;
        }

        private void closeElemDataForm()
        {
            if (elemDataForm != null)
            {
                elemDataForm.Close();
                elemDataForm = null;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            mainUpdateTimer.Enabled = false;
            Compare(compareMode);
            mainUpdateTimer.Enabled = true;
        }

        private void Compare(string Mode)
        {
            if (sadS6x == null) return;
            if (elemsTreeView == null) return;

            List<object[]> results = null;

            Cursor processPreviousCursor = Cursor;

            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            switch (Mode)
            {
                case "S6x":
                    try { results = CompareNewS6x(); }
                    catch { results = null; }
                    break;
                case "Bin":
                default:
                    try { results = CompareNewBinary(); }
                    catch { results = null; }
                    break;
            }

            Cursor = processPreviousCursor;

            if (results == null) return;

            searchTreeView.BeginUpdate();
            
            searchTreeViewInit(Mode);

            S6xNavInfo niMFHeaderCateg = null;
            TreeNode parentNode = null;
            TreeNode tnNode = null;
            TreeNode tnMFNode = null;
            TreeNode tnCmpNode = null;

            string cmpNodeName = string.Empty;
            string cmpNodeText = string.Empty;
            string cmpNodeToolTipText = string.Empty;

            switch (Mode)
            {
                case "S6x":
                    foreach (object[] result in results)
                    {
                        if (result[2] == null) // Missing on S6x, Missing in Source
                        {
                            if (!searchTreeView.Nodes.ContainsKey("MISSING_IN_SOURCE")) continue;
                            parentNode = searchTreeView.Nodes["MISSING_IN_SOURCE"].Nodes[elemsTreeView.Nodes[result[0].ToString()].Name];
                            if (parentNode == null) continue;

                            if (result[3] != null)
                            {
                                if (result[3].GetType() == typeof(S6xStructure))
                                {
                                    cmpNodeName = ((S6xStructure)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xStructure)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xStructure)result[3]).UniqueAddressHex + "\r\n" + ((S6xStructure)result[3]).ShortLabel + "\r\n\r\n" + ((S6xStructure)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xTable))
                                {
                                    cmpNodeName = ((S6xTable)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xTable)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xTable)result[3]).UniqueAddressHex + "\r\n" + ((S6xTable)result[3]).ShortLabel + "\r\n\r\n" + ((S6xTable)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xFunction))
                                {
                                    cmpNodeName = ((S6xFunction)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xFunction)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xFunction)result[3]).UniqueAddressHex + "\r\n" + ((S6xFunction)result[3]).ShortLabel + "\r\n\r\n" + ((S6xFunction)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xScalar))
                                {
                                    cmpNodeName = ((S6xScalar)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xScalar)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xScalar)result[3]).UniqueAddressHex + "\r\n" + ((S6xScalar)result[3]).ShortLabel + "\r\n\r\n" + ((S6xScalar)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xRoutine))
                                {
                                    cmpNodeName = ((S6xRoutine)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xRoutine)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xRoutine)result[3]).UniqueAddressHex + "\r\n" + ((S6xRoutine)result[3]).ShortLabel + "\r\n\r\n" + ((S6xRoutine)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xOperation))
                                {
                                    cmpNodeName = ((S6xOperation)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xOperation)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xOperation)result[3]).UniqueAddressHex + "\r\n" + ((S6xOperation)result[3]).ShortLabel + "\r\n\r\n" + ((S6xOperation)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xRegister))
                                {
                                    cmpNodeName = ((S6xRegister)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xRegister)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xRegister)result[3]).Address + "\r\n" + ((S6xRegister)result[3]).AdditionalAddress10 + "\r\n\r\n" + ((S6xRegister)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xOtherAddress))
                                {
                                    cmpNodeName = ((S6xOtherAddress)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xOtherAddress)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xOtherAddress)result[3]).UniqueAddressHex + "\r\n\r\n" + ((S6xOtherAddress)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xSignature))
                                {
                                    cmpNodeName = ((S6xSignature)result[3]).UniqueKey;
                                    cmpNodeText = ((S6xSignature)result[3]).SignatureLabel;
                                    cmpNodeToolTipText = ((S6xSignature)result[3]).SignatureLabel + "\r\n\r\n" + ((S6xSignature)result[3]).SignatureComments;
                                }
                                else if (result[3].GetType() == typeof(S6xElementSignature))
                                {
                                    cmpNodeName = ((S6xElementSignature)result[3]).UniqueKey;
                                    cmpNodeText = ((S6xElementSignature)result[3]).SignatureLabel;
                                    cmpNodeToolTipText = ((S6xElementSignature)result[3]).SignatureLabel + "\r\n\r\n" + ((S6xElementSignature)result[3]).SignatureComments;
                                }
                            }

                            if (cmpNodeName == string.Empty) continue;

                            tnCmpNode = new TreeNode();
                            tnCmpNode.Name = cmpNodeName;
                            tnCmpNode.Text = cmpNodeText;
                            tnCmpNode.ToolTipText = cmpNodeToolTipText;
                            tnCmpNode.Tag = result[3];
                            tnCmpNode.ContextMenuStrip = resultElemContextMenuStrip;
                            parentNode.Nodes.Add(tnCmpNode);
                        }
                        else
                        {
                            string diffMessage = string.Empty;

                            switch (result[1].ToString())
                            {
                                case "MISSING":
                                    if (!searchTreeView.Nodes.ContainsKey("MISSING_IN_COMPARED")) continue;
                                    parentNode = searchTreeView.Nodes["MISSING_IN_COMPARED"].Nodes[elemsTreeView.Nodes[result[0].ToString()].Name];
                                    if (parentNode == null) continue;
                                    break;
                                default:
                                    if (!searchTreeView.Nodes.ContainsKey("DIFFERENCES")) continue;
                                    parentNode = searchTreeView.Nodes["DIFFERENCES"].Nodes[elemsTreeView.Nodes[result[0].ToString()].Name];
                                    if (parentNode == null) continue;
                                    break;
                            }

                            switch (result[1].ToString())
                            {
                                case "MISSING":
                                    break;
                                case "DIFF":
                                    if (result[4].ToString() == string.Empty) diffMessage = "Different setup\r\n";
                                    else diffMessage = result[4].ToString();
                                    break;
                                default:
                                    diffMessage = "Different type\r\n";
                                    break;
                            }

                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[(string)result[0]]);
                            if (!niMFHeaderCateg.isValid) continue;
                            tnMFNode = niMFHeaderCateg.FindElement(result[2].ToString());
                            if (tnMFNode == null) continue;

                            if (parentNode.Nodes.ContainsKey(result[2].ToString())) continue;

                            tnNode = new TreeNode();
                            tnNode.Name = tnMFNode.Name;
                            tnNode.Text = tnMFNode.Text;
                            tnNode.ToolTipText = tnMFNode.ToolTipText;
                            tnNode.StateImageKey = tnMFNode.StateImageKey;
                            parentNode.Nodes.Add(tnNode);
                            
                            tnMFNode = null;

                            if (result[3] != null)
                            {
                                if (result[3].GetType() == typeof(S6xStructure))
                                {
                                    cmpNodeName = ((S6xStructure)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xStructure)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xStructure)result[3]).UniqueAddressHex + "\r\n" + ((S6xStructure)result[3]).ShortLabel + "\r\n\r\n" + ((S6xStructure)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xTable))
                                {
                                    cmpNodeName = ((S6xTable)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xTable)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xTable)result[3]).UniqueAddressHex + "\r\n" + ((S6xTable)result[3]).ShortLabel + "\r\n\r\n" + ((S6xTable)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xFunction))
                                {
                                    cmpNodeName = ((S6xFunction)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xFunction)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xFunction)result[3]).UniqueAddressHex + "\r\n" + ((S6xFunction)result[3]).ShortLabel + "\r\n\r\n" + ((S6xFunction)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xScalar))
                                {
                                    cmpNodeName = ((S6xScalar)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xScalar)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xScalar)result[3]).UniqueAddressHex + "\r\n" + ((S6xScalar)result[3]).ShortLabel + "\r\n\r\n" + ((S6xScalar)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xRoutine))
                                {
                                    cmpNodeName = ((S6xRoutine)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xRoutine)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xRoutine)result[3]).UniqueAddressHex + "\r\n" + ((S6xRoutine)result[3]).ShortLabel + "\r\n\r\n" + ((S6xRoutine)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xOperation))
                                {
                                    cmpNodeName = ((S6xOperation)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xOperation)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xOperation)result[3]).UniqueAddressHex + "\r\n" + ((S6xOperation)result[3]).ShortLabel + "\r\n\r\n" + ((S6xOperation)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xRegister))
                                {
                                    cmpNodeName = ((S6xRegister)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xRegister)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xRegister)result[3]).Address + "\r\n" + ((S6xRegister)result[3]).AdditionalAddress10 + "\r\n\r\n" + ((S6xRegister)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xOtherAddress))
                                {
                                    cmpNodeName = ((S6xOtherAddress)result[3]).UniqueAddress;
                                    cmpNodeText = ((S6xOtherAddress)result[3]).Label;
                                    cmpNodeToolTipText = ((S6xOtherAddress)result[3]).UniqueAddressHex + "\r\n\r\n" + ((S6xOtherAddress)result[3]).Comments;
                                }
                                else if (result[3].GetType() == typeof(S6xSignature))
                                {
                                    cmpNodeName = ((S6xSignature)result[3]).UniqueKey;
                                    cmpNodeText = ((S6xSignature)result[3]).SignatureLabel;
                                    cmpNodeToolTipText = ((S6xSignature)result[3]).SignatureLabel + "\r\n\r\n" + ((S6xSignature)result[3]).SignatureComments;
                                }
                                else if (result[3].GetType() == typeof(S6xElementSignature))
                                {
                                    cmpNodeName = ((S6xElementSignature)result[3]).UniqueKey;
                                    cmpNodeText = ((S6xElementSignature)result[3]).SignatureLabel;
                                    cmpNodeToolTipText = ((S6xElementSignature)result[3]).SignatureLabel + "\r\n\r\n" + ((S6xElementSignature)result[3]).SignatureComments;
                                }
                            }

                            if (diffMessage != string.Empty)
                            {
                                tnNode.ToolTipText = diffMessage + "\r\n" + tnNode.ToolTipText;
                                cmpNodeToolTipText = diffMessage + "\r\n" + cmpNodeToolTipText;
                            }

                            if (cmpNodeName == string.Empty) continue;

                            tnCmpNode = new TreeNode();
                            tnCmpNode.Name = cmpNodeName;
                            tnCmpNode.Text = cmpNodeText;
                            tnCmpNode.ToolTipText = cmpNodeToolTipText;
                            tnCmpNode.Tag = result[3];
                            tnCmpNode.ContextMenuStrip = resultElemContextMenuStrip;
                            tnNode.Nodes.Add(tnCmpNode);
                        }
                    }
                    break;
                case "Bin":
                default:
                    foreach (object[] result in results)
                    {
                        // 20210114 - PYM - Now managing remaining addresses
                        if ((string)result[0] == S6xNav.getHeaderCategName(S6xNavHeaderCategory.UNDEFINED) && result.Length > 2)
                        {
                            parentNode = searchTreeView.Nodes[(string)result[0]];
                            if (parentNode == null)
                            {
                                parentNode = new TreeNode();
                                parentNode.Name = (string)result[0];
                                parentNode.Text = "Remaining differences";
                                parentNode.ToolTipText = "Remaining differences including unknown/unidentified elements, address by address.";
                                searchTreeView.Nodes.Add(parentNode);
                            }

                            if (parentNode.Nodes.ContainsKey((string)result[1])) continue;

                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.RESERVED)]);
                            if (niMFHeaderCateg.isValid) tnMFNode = niMFHeaderCateg.FindElement((string)result[1]);

                            tnNode = new TreeNode();
                            tnNode.Name = (string)result[1];
                            if (tnMFNode == null)
                            {
                                tnNode.Text = Tools.UniqueAddressHex(((int[])result[2])[0], ((int[])result[2])[1]);
                                tnNode.ToolTipText = tnNode.Text;
                            }
                            else
                            {
                                tnNode.Text = tnMFNode.Text;
                                tnNode.ToolTipText = tnMFNode.ToolTipText;
                                tnNode.StateImageKey = tnMFNode.StateImageKey;
                            }
                            parentNode.Nodes.Add(tnNode);

                            continue;
                        }

                        parentNode = searchTreeView.Nodes[elemsTreeView.Nodes[(string)result[0]].Name];
                        if (parentNode == null) continue;

                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[(string)result[0]]);
                        if (!niMFHeaderCateg.isValid) continue;
                        tnMFNode = niMFHeaderCateg.FindElement((string)result[1]);
                        if (tnMFNode == null) continue;

                        if (parentNode.Nodes.ContainsKey((string)result[1])) continue;

                        tnNode = new TreeNode();
                        tnNode.Name = tnMFNode.Name;
                        tnNode.Text = tnMFNode.Text;
                        tnNode.ToolTipText = tnMFNode.ToolTipText;
                        tnNode.StateImageKey = tnMFNode.StateImageKey;
                        if (result.Length > 2) tnNode.Tag = result[2];
                        parentNode.Nodes.Add(tnNode);
                    }
                    break;
            }

            searchTreeViewCount(Mode);

            searchTreeView.EndUpdate();
        }

        private List<object[]> CompareNewBinary()
        {
            List<object[]> results = new List<object[]>();

            string cmpBinaryFilePath = string.Empty;
            string cmpS6xFilePath = string.Empty;

            string sError = string.Empty;

            bool bError = false;

            // Close the Element Data Form if already opened
            closeElemDataForm();

            if (openFileDialogBin.ShowDialog() != DialogResult.OK) return null;
            if (!File.Exists(openFileDialogBin.FileName))
            {
                sError += openFileDialogBin.FileName + "\r\n";
                sError += "Not existing Binary.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            cmpBinaryFilePath = openFileDialogBin.FileName;

            FileInfo fiFI = new FileInfo(cmpBinaryFilePath);
            cmpS6xFilePath = fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".s6x";
            fiFI = null;

            if (!File.Exists(cmpS6xFilePath)) cmpS6xFilePath = string.Empty;

            cmpSadBin = null;
            cmpSadS6x = null;

            cmpSadBin = new SADBin(cmpBinaryFilePath, cmpS6xFilePath);
            cmpSadS6x = cmpSadBin.S6x;

            bError = (cmpSadBin == null);
            if (bError)
            {
                sError += cmpBinaryFilePath + "\r\n";
                sError += "Unrecognized Binary.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            bError = (!cmpSadBin.isLoaded || !cmpSadBin.isValid);
            if (bError)
            {
                sError += cmpBinaryFilePath + "\r\n";
                sError += "Unrecognized Binary.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (binariesSameDefinition)
            {
                bError = (sadBin.Calibration.Info.is8061 != cmpSadBin.Calibration.Info.is8061 || sadBin.Calibration.Info.isEarly != cmpSadBin.Calibration.Info.isEarly);
                if (!bError) bError = (sadBin.Calibration.Info.slBanksInfos.Count != cmpSadBin.Calibration.Info.slBanksInfos.Count);
                if (bError)
                {
                    sError += cmpBinaryFilePath + "\r\n";
                    sError += "Uncompatible Binary.";
                    MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                string srcStrategy = string.Empty;
                string cmpStrategy = string.Empty;
                if (sadBin.Calibration.Info.VidStrategy != null && sadBin.Calibration.Info.VidStrategyVersion != null) srcStrategy = sadBin.Calibration.Info.VidStrategy.Trim() + sadBin.Calibration.Info.VidStrategyVersion.Trim();
                if (cmpSadBin.Calibration.Info.VidStrategy != null && cmpSadBin.Calibration.Info.VidStrategyVersion != null) cmpStrategy = cmpSadBin.Calibration.Info.VidStrategy.Trim() + cmpSadBin.Calibration.Info.VidStrategyVersion.Trim();
                bError = (srcStrategy != cmpStrategy);
                if (bError)
                {
                    sError += cmpBinaryFilePath + "\r\n";
                    sError += "Strategy is different.\r\n";
                    sError += "\t" + cmpStrategy.Replace("\0", "") + ", " + srcStrategy.Replace("\0", "") + " was expected.";
                    MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                cmpSadBin.processBin();
                bError = (!cmpSadBin.isDisassembled);
                if (bError)
                {
                    sError += cmpBinaryFilePath + "\r\n";
                    sError += "Binary can not be disassembled.";
                    MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
            }

            if (binariesSameDefinition)
            //  Working on Same Definition - All Adresses are the same
            {
                // 20210114 - PYM - Now managing remaining addresses
                SortedList<string, int[]> slDifferentBanksBytes = new SortedList<string, int[]>();
                SADBank[] arrSadBanks = new SADBank[] { sadBin.Bank8, sadBin.Bank1, sadBin.Bank9, sadBin.Bank0 };
                SADBank[] arrCmpBanks = new SADBank[] { cmpSadBin.Bank8, cmpSadBin.Bank1, cmpSadBin.Bank9, cmpSadBin.Bank0 };
                for (int iBank = 0; iBank < arrSadBanks.Length; iBank++)
                {
                    SADBank sadBank = arrSadBanks[iBank];
                    if (sadBank == null) continue;
                    SADBank cmpBank = arrCmpBanks[iBank];
                    if (cmpBank == null) continue;

                    string[] arrSadBytes = sadBank.getBytesArray(0, sadBank.Size);
                    string[] arrCmpBytes = cmpBank.getBytesArray(0, cmpBank.Size);
                    int iMinSize = sadBank.Size;
                    if (iMinSize > cmpBank.Size) iMinSize = cmpBank.Size;
                    for (int iAddress = 0; iAddress < iMinSize; iAddress++)
                    {
                        if (arrSadBytes[iAddress] != arrCmpBytes[iAddress]) slDifferentBanksBytes.Add(Tools.UniqueAddress(sadBank.Num, iAddress), new int[] { sadBank.Num, iAddress });
                    }

                    // Reserved Addresses are kept, but only the first one for a word (or more) Reserved address
                    foreach (ReservedAddress rsRA in sadBank.slReserved.Values)
                    {
                        if (rsRA.AddressInt == rsRA.AddressEndInt) continue;
                        for (int iAddress = rsRA.AddressInt + 1; iAddress < rsRA.AddressEndInt + 1; iAddress++)
                        {
                            if (slDifferentBanksBytes.ContainsKey(Tools.UniqueAddress(sadBank.Num, iAddress)))
                            {
                                if (!slDifferentBanksBytes.ContainsKey(rsRA.UniqueAddress)) slDifferentBanksBytes.Add(rsRA.UniqueAddress, new int[] { sadBank.Num, rsRA.AddressInt });
                                slDifferentBanksBytes.Remove(Tools.UniqueAddress(sadBank.Num, iAddress));
                            }
                        }
                    }
                }
                arrSadBanks = null;
                arrCmpBanks = null;

                foreach (CalibrationElement calElem in sadBin.Calibration.slCalibrationElements.Values)
                {
                    bool isResult = false;
                    SADBank srcReadBank = null;
                    SADBank cmpReadBank = null;
                    switch (calElem.BankNum)
                    {
                        case 8:
                            srcReadBank = sadBin.Bank8;
                            cmpReadBank = cmpSadBin.Bank8;
                            break;
                        case 1:
                            srcReadBank = sadBin.Bank1;
                            cmpReadBank = cmpSadBin.Bank1;
                            break;
                        case 9:
                            srcReadBank = sadBin.Bank9;
                            cmpReadBank = cmpSadBin.Bank9;
                            break;
                        case 0:
                            srcReadBank = sadBin.Bank0;
                            cmpReadBank = cmpSadBin.Bank0;
                            break;
                        default:
                            continue;
                    }
                    isResult = (srcReadBank.getBytes(calElem.AddressInt, calElem.Size) != cmpReadBank.getBytes(calElem.AddressInt, calElem.Size));
                    if (isResult)
                    {
                        if (calElem.isScalar) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), calElem.UniqueAddress });
                        else if (calElem.isFunction) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), calElem.UniqueAddress });
                        else if (calElem.isTable) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), calElem.UniqueAddress });
                        else if (calElem.isStructure) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), calElem.UniqueAddress });

                        // 20210114 - PYM - Now managing remaining addresses
                        for (int iAddress = calElem.AddressInt; iAddress < calElem.AddressInt + calElem.Size; iAddress++) slDifferentBanksBytes.Remove(Tools.UniqueAddress(srcReadBank.Num, iAddress));
                    }
                }
                foreach (Scalar extObject in sadBin.Calibration.slExtScalars.Values)
                {
                    bool isResult = false;
                    SADBank srcReadBank = null;
                    SADBank cmpReadBank = null;
                    switch (extObject.BankNum)
                    {
                        case 8:
                            srcReadBank = sadBin.Bank8;
                            cmpReadBank = cmpSadBin.Bank8;
                            break;
                        case 1:
                            srcReadBank = sadBin.Bank1;
                            cmpReadBank = cmpSadBin.Bank1;
                            break;
                        case 9:
                            srcReadBank = sadBin.Bank9;
                            cmpReadBank = cmpSadBin.Bank9;
                            break;
                        case 0:
                            srcReadBank = sadBin.Bank0;
                            cmpReadBank = cmpSadBin.Bank0;
                            break;
                        default:
                            continue;
                    }
                    isResult = (srcReadBank.getBytes(extObject.AddressInt, extObject.Size) != cmpReadBank.getBytes(extObject.AddressInt, extObject.Size));
                    if (isResult)
                    {
                        results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), extObject.UniqueAddress });

                        // 20210114 - PYM - Now managing remaining addresses
                        for (int iAddress = extObject.AddressInt; iAddress < extObject.AddressInt + extObject.Size; iAddress++) slDifferentBanksBytes.Remove(Tools.UniqueAddress(srcReadBank.Num, iAddress));
                    }
                }
                foreach (Function extObject in sadBin.Calibration.slExtFunctions.Values)
                {
                    bool isResult = false;
                    SADBank srcReadBank = null;
                    SADBank cmpReadBank = null;
                    switch (extObject.BankNum)
                    {
                        case 8:
                            srcReadBank = sadBin.Bank8;
                            cmpReadBank = cmpSadBin.Bank8;
                            break;
                        case 1:
                            srcReadBank = sadBin.Bank1;
                            cmpReadBank = cmpSadBin.Bank1;
                            break;
                        case 9:
                            srcReadBank = sadBin.Bank9;
                            cmpReadBank = cmpSadBin.Bank9;
                            break;
                        case 0:
                            srcReadBank = sadBin.Bank0;
                            cmpReadBank = cmpSadBin.Bank0;
                            break;
                        default:
                            continue;
                    }
                    isResult = (srcReadBank.getBytes(extObject.AddressInt, extObject.Size) != cmpReadBank.getBytes(extObject.AddressInt, extObject.Size));
                    if (isResult) 
                    {
                        results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), extObject.UniqueAddress });

                        // 20210114 - PYM - Now managing remaining addresses
                        for (int iAddress = extObject.AddressInt; iAddress < extObject.AddressInt + extObject.Size; iAddress++) slDifferentBanksBytes.Remove(Tools.UniqueAddress(srcReadBank.Num, iAddress));
                    }
                }
                foreach (Table extObject in sadBin.Calibration.slExtTables.Values)
                {
                    bool isResult = false;
                    SADBank srcReadBank = null;
                    SADBank cmpReadBank = null;
                    switch (extObject.BankNum)
                    {
                        case 8:
                            srcReadBank = sadBin.Bank8;
                            cmpReadBank = cmpSadBin.Bank8;
                            break;
                        case 1:
                            srcReadBank = sadBin.Bank1;
                            cmpReadBank = cmpSadBin.Bank1;
                            break;
                        case 9:
                            srcReadBank = sadBin.Bank9;
                            cmpReadBank = cmpSadBin.Bank9;
                            break;
                        case 0:
                            srcReadBank = sadBin.Bank0;
                            cmpReadBank = cmpSadBin.Bank0;
                            break;
                        default:
                            continue;
                    }
                    isResult = (srcReadBank.getBytes(extObject.AddressInt, extObject.Size) != cmpReadBank.getBytes(extObject.AddressInt, extObject.Size));
                    if (isResult)
                    {
                        results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), extObject.UniqueAddress });

                        // 20210114 - PYM - Now managing remaining addresses
                        for (int iAddress = extObject.AddressInt; iAddress < extObject.AddressInt + extObject.Size; iAddress++) slDifferentBanksBytes.Remove(Tools.UniqueAddress(srcReadBank.Num, iAddress));
                    }
                }
                foreach (Structure extObject in sadBin.Calibration.slExtStructures.Values)
                {
                    bool isResult = false;
                    SADBank srcReadBank = null;
                    SADBank cmpReadBank = null;
                    switch (extObject.BankNum)
                    {
                        case 8:
                            srcReadBank = sadBin.Bank8;
                            cmpReadBank = cmpSadBin.Bank8;
                            break;
                        case 1:
                            srcReadBank = sadBin.Bank1;
                            cmpReadBank = cmpSadBin.Bank1;
                            break;
                        case 9:
                            srcReadBank = sadBin.Bank9;
                            cmpReadBank = cmpSadBin.Bank9;
                            break;
                        case 0:
                            srcReadBank = sadBin.Bank0;
                            cmpReadBank = cmpSadBin.Bank0;
                            break;
                        default:
                            continue;
                    }
                    isResult = (srcReadBank.getBytes(extObject.AddressInt, extObject.Size) != cmpReadBank.getBytes(extObject.AddressInt, extObject.Size));
                    if (isResult)
                    {
                        results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), extObject.UniqueAddress });

                        // 20210114 - PYM - Now managing remaining addresses
                        for (int iAddress = extObject.AddressInt; iAddress < extObject.AddressInt + extObject.Size; iAddress++) slDifferentBanksBytes.Remove(Tools.UniqueAddress(srcReadBank.Num, iAddress));
                    }
                }
                foreach (Call extObject in sadBin.Calibration.slCalls.Values)
                {
                    if (extObject.S6xRoutine == null) continue;
                    if (extObject.AddressEndInt <= extObject.AddressInt) continue;

                    bool isResult = false;
                    SADBank srcReadBank = null;
                    SADBank cmpReadBank = null;
                    switch (extObject.BankNum)
                    {
                        case 8:
                            srcReadBank = sadBin.Bank8;
                            cmpReadBank = cmpSadBin.Bank8;
                            break;
                        case 1:
                            srcReadBank = sadBin.Bank1;
                            cmpReadBank = cmpSadBin.Bank1;
                            break;
                        case 9:
                            srcReadBank = sadBin.Bank9;
                            cmpReadBank = cmpSadBin.Bank9;
                            break;
                        case 0:
                            srcReadBank = sadBin.Bank0;
                            cmpReadBank = cmpSadBin.Bank0;
                            break;
                        default:
                            continue;
                    }
                    isResult = (srcReadBank.getBytes(extObject.AddressInt, extObject.AddressEndInt - extObject.AddressInt) != cmpReadBank.getBytes(extObject.AddressInt, extObject.AddressEndInt - extObject.AddressInt));
                    if (isResult)
                    {
                        results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), extObject.UniqueAddress });

                        // 20210114 - PYM - Now managing remaining addresses
                        for (int iAddress = extObject.AddressInt; iAddress < extObject.AddressEndInt + 1; iAddress++) slDifferentBanksBytes.Remove(Tools.UniqueAddress(srcReadBank.Num, iAddress));
                    }
                }

                // 20210114 - PYM - Now managing remaining addresses
                foreach (string uniqueAddress in slDifferentBanksBytes.Keys)
                {
                    results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.UNDEFINED), uniqueAddress, slDifferentBanksBytes[uniqueAddress] });
                }
            }
            else
            //  Working on Different Definition - Only working on defined Elements
            {
                foreach (S6xScalar srcObject in sadS6x.slScalars.Values)
                {
                    if (srcObject == null) continue;
                    if (srcObject.Skip || !srcObject.Store) continue;
                    if (!srcObject.isUserDefined) continue;

                    foreach (S6xScalar cmpObject in cmpSadS6x.slScalars.Values)
                    {
                        if (cmpObject == null) continue;
                        if (cmpObject.Skip || !cmpObject.Store) continue;
                        if (cmpObject.ShortLabel != srcObject.ShortLabel) continue;

                        bool isResult = false;
                        SADBank srcReadBank = null;
                        SADBank cmpReadBank = null;
                        switch (srcObject.BankNum)
                        {
                            case 8:
                                srcReadBank = sadBin.Bank8;
                                break;
                            case 1:
                                srcReadBank = sadBin.Bank1;
                                break;
                            case 9:
                                srcReadBank = sadBin.Bank9;
                                break;
                            case 0:
                                srcReadBank = sadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        switch (cmpObject.BankNum)
                        {
                            case 8:
                                cmpReadBank = cmpSadBin.Bank8;
                                break;
                            case 1:
                                cmpReadBank = cmpSadBin.Bank1;
                                break;
                            case 9:
                                cmpReadBank = cmpSadBin.Bank9;
                                break;
                            case 0:
                                cmpReadBank = cmpSadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        string srcBytes = string.Empty;
                        string cmpBytes = string.Empty;

                        if (srcObject.isCalibrationElement)
                        {
                            if (sadBin.Calibration.slCalibrationElements.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((CalibrationElement)sadBin.Calibration.slCalibrationElements[srcObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (sadBin.Calibration.slExtScalars.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((Scalar)sadBin.Calibration.slExtScalars[srcObject.UniqueAddress]).Size);
                        }
                        if (cmpObject.isCalibrationElement)
                        {
                            if (cmpSadBin.Calibration.slCalibrationElements.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((CalibrationElement)cmpSadBin.Calibration.slCalibrationElements[cmpObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (cmpSadBin.Calibration.slExtScalars.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((Scalar)cmpSadBin.Calibration.slExtScalars[cmpObject.UniqueAddress]).Size);
                        }

                        isResult = (srcBytes != cmpBytes);
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), srcObject.UniqueAddress, cmpObject.UniqueAddress });

                        break;
                    }
                }

                foreach (S6xFunction srcObject in sadS6x.slFunctions.Values)
                {
                    if (srcObject == null) continue;
                    if (srcObject.Skip || !srcObject.Store) continue;
                    if (!srcObject.isUserDefined) continue;

                    foreach (S6xFunction cmpObject in cmpSadS6x.slFunctions.Values)
                    {
                        if (cmpObject == null) continue;
                        if (cmpObject.Skip || !cmpObject.Store) continue;
                        if (cmpObject.ShortLabel != srcObject.ShortLabel) continue;

                        bool isResult = false;
                        SADBank srcReadBank = null;
                        SADBank cmpReadBank = null;
                        switch (srcObject.BankNum)
                        {
                            case 8:
                                srcReadBank = sadBin.Bank8;
                                break;
                            case 1:
                                srcReadBank = sadBin.Bank1;
                                break;
                            case 9:
                                srcReadBank = sadBin.Bank9;
                                break;
                            case 0:
                                srcReadBank = sadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        switch (cmpObject.BankNum)
                        {
                            case 8:
                                cmpReadBank = cmpSadBin.Bank8;
                                break;
                            case 1:
                                cmpReadBank = cmpSadBin.Bank1;
                                break;
                            case 9:
                                cmpReadBank = cmpSadBin.Bank9;
                                break;
                            case 0:
                                cmpReadBank = cmpSadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        string srcBytes = string.Empty;
                        string cmpBytes = string.Empty;

                        if (srcObject.isCalibrationElement)
                        {
                            if (sadBin.Calibration.slCalibrationElements.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((CalibrationElement)sadBin.Calibration.slCalibrationElements[srcObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (sadBin.Calibration.slExtFunctions.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((Function)sadBin.Calibration.slExtFunctions[srcObject.UniqueAddress]).Size);
                        }
                        if (cmpObject.isCalibrationElement)
                        {
                            if (cmpSadBin.Calibration.slCalibrationElements.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((CalibrationElement)cmpSadBin.Calibration.slCalibrationElements[cmpObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (cmpSadBin.Calibration.slExtFunctions.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((Function)cmpSadBin.Calibration.slExtFunctions[cmpObject.UniqueAddress]).Size);
                        }

                        isResult = (srcBytes != cmpBytes);
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), srcObject.UniqueAddress, cmpObject.UniqueAddress });

                        break;
                    }
                }

                foreach (S6xTable srcObject in sadS6x.slTables.Values)
                {
                    if (srcObject == null) continue;
                    if (srcObject.Skip || !srcObject.Store) continue;
                    if (!srcObject.isUserDefined) continue;

                    foreach (S6xTable cmpObject in cmpSadS6x.slTables.Values)
                    {
                        if (cmpObject == null) continue;
                        if (cmpObject.Skip || !cmpObject.Store) continue;
                        if (cmpObject.ShortLabel != srcObject.ShortLabel) continue;

                        bool isResult = false;
                        SADBank srcReadBank = null;
                        SADBank cmpReadBank = null;
                        switch (srcObject.BankNum)
                        {
                            case 8:
                                srcReadBank = sadBin.Bank8;
                                break;
                            case 1:
                                srcReadBank = sadBin.Bank1;
                                break;
                            case 9:
                                srcReadBank = sadBin.Bank9;
                                break;
                            case 0:
                                srcReadBank = sadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        switch (cmpObject.BankNum)
                        {
                            case 8:
                                cmpReadBank = cmpSadBin.Bank8;
                                break;
                            case 1:
                                cmpReadBank = cmpSadBin.Bank1;
                                break;
                            case 9:
                                cmpReadBank = cmpSadBin.Bank9;
                                break;
                            case 0:
                                cmpReadBank = cmpSadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        string srcBytes = string.Empty;
                        string cmpBytes = string.Empty;

                        if (srcObject.isCalibrationElement)
                        {
                            if (sadBin.Calibration.slCalibrationElements.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((CalibrationElement)sadBin.Calibration.slCalibrationElements[srcObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (sadBin.Calibration.slExtTables.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((Table)sadBin.Calibration.slExtTables[srcObject.UniqueAddress]).Size);
                        }
                        if (cmpObject.isCalibrationElement)
                        {
                            if (cmpSadBin.Calibration.slCalibrationElements.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((CalibrationElement)cmpSadBin.Calibration.slCalibrationElements[cmpObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (cmpSadBin.Calibration.slExtTables.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((Table)cmpSadBin.Calibration.slExtTables[cmpObject.UniqueAddress]).Size);
                        }

                        isResult = (srcBytes != cmpBytes);
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), srcObject.UniqueAddress, cmpObject.UniqueAddress });

                        break;
                    }
                }

                foreach (S6xStructure srcObject in sadS6x.slStructures.Values)
                {
                    if (srcObject == null) continue;
                    if (srcObject.Skip || !srcObject.Store) continue;
                    if (!srcObject.isUserDefined) continue;

                    foreach (S6xStructure cmpObject in cmpSadS6x.slStructures.Values)
                    {
                        if (cmpObject == null) continue;
                        if (cmpObject.Skip || !cmpObject.Store) continue;
                        if (cmpObject.ShortLabel != srcObject.ShortLabel) continue;

                        bool isResult = false;
                        SADBank srcReadBank = null;
                        SADBank cmpReadBank = null;
                        switch (srcObject.BankNum)
                        {
                            case 8:
                                srcReadBank = sadBin.Bank8;
                                break;
                            case 1:
                                srcReadBank = sadBin.Bank1;
                                break;
                            case 9:
                                srcReadBank = sadBin.Bank9;
                                break;
                            case 0:
                                srcReadBank = sadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        switch (cmpObject.BankNum)
                        {
                            case 8:
                                cmpReadBank = cmpSadBin.Bank8;
                                break;
                            case 1:
                                cmpReadBank = cmpSadBin.Bank1;
                                break;
                            case 9:
                                cmpReadBank = cmpSadBin.Bank9;
                                break;
                            case 0:
                                cmpReadBank = cmpSadBin.Bank0;
                                break;
                            default:
                                continue;
                        }
                        string srcBytes = string.Empty;
                        string cmpBytes = string.Empty;

                        if (srcObject.isCalibrationElement)
                        {
                            if (sadBin.Calibration.slCalibrationElements.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((CalibrationElement)sadBin.Calibration.slCalibrationElements[srcObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (sadBin.Calibration.slExtStructures.ContainsKey(srcObject.UniqueAddress)) srcBytes = srcReadBank.getBytes(srcObject.AddressInt, ((Structure)sadBin.Calibration.slExtStructures[srcObject.UniqueAddress]).Size);
                        }
                        if (cmpObject.isCalibrationElement)
                        {
                            if (cmpSadBin.Calibration.slCalibrationElements.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((CalibrationElement)cmpSadBin.Calibration.slCalibrationElements[cmpObject.UniqueAddress]).Size);
                        }
                        else
                        {
                            if (cmpSadBin.Calibration.slExtStructures.ContainsKey(cmpObject.UniqueAddress)) cmpBytes = cmpReadBank.getBytes(cmpObject.AddressInt, ((Structure)cmpSadBin.Calibration.slExtStructures[cmpObject.UniqueAddress]).Size);
                        }

                        isResult = (srcBytes != cmpBytes);
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), srcObject.UniqueAddress, cmpObject.UniqueAddress });

                        break;
                    }
                }
            }

            return results;
        }

        // CompareNewS6x
        //      20210119 - No management for Duplicates, no real interest for comparing definitions
        //      20210119 - No management for Signatures & ElementSignatures, because based on Unique keys, without needed Unique matching address
        private List<object[]> CompareNewS6x()
        {
            List<object[]> results = new List<object[]>();

            string cmpS6xFilePath = string.Empty;

            string sError = string.Empty;

            bool bError = false;

            bool findGenericDifferences = true;

            if (openFileDialogS6x.ShowDialog() != DialogResult.OK) return null;
            if (!File.Exists(openFileDialogS6x.FileName))
            {
                sError += openFileDialogS6x.FileName + "\r\n";
                sError += "Not existing Sad 806X.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            cmpS6xFilePath = openFileDialogS6x.FileName;

            cmpSadBin = null;
            cmpSadS6x = null;

            cmpSadBin = new SADBin(sadBin.BinaryFilePath, cmpS6xFilePath);
            cmpSadS6x = cmpSadBin.S6x;

            bError = (cmpSadBin == null);
            if (bError)
            {
                sError += sadBin.BinaryFilePath + "\r\n";
                sError += "Unrecognized SAD 806x.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            bError = (!cmpSadBin.isLoaded || !cmpSadBin.isValid);
            if (bError)
            {
                sError += sadBin.BinaryFilePath + "\r\n";
                sError += "Unrecognized SAD 806x.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Missing or Different on CmpS6x
            foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
            {
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slStructures[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xStructure cmpObject = (S6xStructure)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.Number != cmpObject.Number) uResult[4] = uResult[4].ToString() + "Different numbers\r\n";
                    if (s6xObject.StructDef != cmpObject.StructDef) uResult[4] = uResult[4].ToString() + "Different structure definitions\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Label != cmpObject.Label;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;
                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;
                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slStructures[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slTables[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slFunctions[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slScalars[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);

                    if (!s6xObject.Skip && uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slTables[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xTable cmpObject = (S6xTable)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.ColsNumber != cmpObject.ColsNumber) uResult[4] = uResult[4].ToString() + "Different columns numbers\r\n";
                    if (s6xObject.RowsNumber != cmpObject.RowsNumber) uResult[4] = uResult[4].ToString() + "Different rows numbers\r\n";
                    if (s6xObject.WordOutput != cmpObject.WordOutput) uResult[4] = uResult[4].ToString() + "Different type\r\n";
                    if (s6xObject.SignedOutput != cmpObject.SignedOutput) uResult[4] = uResult[4].ToString() + "Different sign\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Label != cmpObject.Label;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;
                        
                        if (!genericDifference) genericDifference = s6xObject.CellsScaleExpression != cmpObject.CellsScaleExpression;
                        if (!genericDifference) genericDifference = s6xObject.CellsScalePrecision != cmpObject.CellsScalePrecision;
                        if (!genericDifference) genericDifference = s6xObject.CellsUnits != cmpObject.CellsUnits;
                        if (!genericDifference) genericDifference = s6xObject.CellsMin != cmpObject.CellsMin;
                        if (!genericDifference) genericDifference = s6xObject.CellsMax != cmpObject.CellsMax;
                        if (!genericDifference) genericDifference = s6xObject.ColsScalerAddress != cmpObject.ColsScalerAddress;
                        if (!genericDifference) genericDifference = s6xObject.ColsUnits != cmpObject.ColsUnits;
                        if (!genericDifference) genericDifference = s6xObject.RowsScalerAddress != cmpObject.RowsScalerAddress;
                        if (!genericDifference) genericDifference = s6xObject.RowsUnits != cmpObject.RowsUnits;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;
                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slStructures[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slTables[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slFunctions[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slScalars[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);

                    if (!s6xObject.Skip && uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slFunctions[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xFunction cmpObject = (S6xFunction)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.RowsNumber != cmpObject.RowsNumber) uResult[4] = uResult[4].ToString() + "Different rows numbers\r\n";
                    if (s6xObject.ByteInput != cmpObject.ByteInput) uResult[4] = uResult[4].ToString() + "Different input type\r\n";
                    if (s6xObject.ByteOutput != cmpObject.ByteOutput) uResult[4] = uResult[4].ToString() + "Different output type\r\n";
                    if (s6xObject.SignedInput != cmpObject.SignedInput) uResult[4] = uResult[4].ToString() + "Different input sign\r\n";
                    if (s6xObject.SignedOutput != cmpObject.SignedOutput) uResult[4] = uResult[4].ToString() + "Different output sign\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Label != cmpObject.Label;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;

                        if (!genericDifference) genericDifference = s6xObject.ByteInput != cmpObject.ByteInput;
                        if (!genericDifference) genericDifference = s6xObject.ByteOutput != cmpObject.ByteOutput;
                        if (!genericDifference) genericDifference = s6xObject.InputScaleExpression != cmpObject.InputScaleExpression;
                        if (!genericDifference) genericDifference = s6xObject.InputScalePrecision != cmpObject.InputScalePrecision;
                        if (!genericDifference) genericDifference = s6xObject.InputUnits != cmpObject.InputUnits;
                        if (!genericDifference) genericDifference = s6xObject.InputMin != cmpObject.InputMin;
                        if (!genericDifference) genericDifference = s6xObject.InputMax != cmpObject.InputMax;
                        if (!genericDifference) genericDifference = s6xObject.OutputScaleExpression != cmpObject.OutputScaleExpression;
                        if (!genericDifference) genericDifference = s6xObject.OutputScalePrecision != cmpObject.OutputScalePrecision;
                        if (!genericDifference) genericDifference = s6xObject.OutputUnits != cmpObject.OutputUnits;
                        if (!genericDifference) genericDifference = s6xObject.OutputMin != cmpObject.OutputMin;
                        if (!genericDifference) genericDifference = s6xObject.OutputMax != cmpObject.OutputMax;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;
                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slStructures[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slTables[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slFunctions[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slScalars[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);

                    if (!s6xObject.Skip && uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slScalars[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xScalar cmpObject = (S6xScalar)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.Byte != cmpObject.Byte) uResult[4] = uResult[4].ToString() + "Different type\r\n";
                    if (s6xObject.isBitFlags != cmpObject.isBitFlags) uResult[4] = uResult[4].ToString() + "Different bit flags type\r\n";
                    if (s6xObject.Signed != cmpObject.Signed) uResult[4] = uResult[4].ToString() + "Different sign\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Label != cmpObject.Label;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;

                        if (!genericDifference) genericDifference = s6xObject.InlineComments != cmpObject.InlineComments;
                        if (!genericDifference) genericDifference = s6xObject.ScaleExpression != cmpObject.ScaleExpression;
                        if (!genericDifference) genericDifference = s6xObject.ScalePrecision != cmpObject.ScalePrecision;
                        if (!genericDifference) genericDifference = s6xObject.Units != cmpObject.Units;
                        if (!genericDifference) genericDifference = s6xObject.Min != cmpObject.Min;
                        if (!genericDifference) genericDifference = s6xObject.Max != cmpObject.Max;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;

                        SortedList s6xBitFlags = null;
                        SortedList cmpBitFlags = null;
                        if (!genericDifference && s6xObject.BitFlags != null && cmpObject.BitFlags != null)
                        {
                            genericDifference = s6xObject.BitFlags.Length != cmpObject.BitFlags.Length;
                            if (!genericDifference)
                            {
                                s6xBitFlags = new SortedList();
                                cmpBitFlags = new SortedList();
                                foreach (S6xBitFlag s6xBF in s6xObject.BitFlags) s6xBitFlags.Add(s6xBF.UniqueKey, s6xBF);
                                foreach (S6xBitFlag s6xBF in cmpObject.BitFlags) cmpBitFlags.Add(s6xBF.UniqueKey, s6xBF);

                                foreach (S6xBitFlag s6xBF in s6xBitFlags.Values)
                                {
                                    genericDifference = !cmpBitFlags.ContainsKey(s6xBF.UniqueKey);
                                    if (genericDifference) break;
                                    
                                    S6xBitFlag cmpBF = (S6xBitFlag)cmpBitFlags[s6xBF.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xBF.ShortLabel != cmpBF.ShortLabel;
                                    if (!genericDifference) genericDifference = s6xBF.Label != cmpBF.Label;
                                    if (!genericDifference) genericDifference = s6xBF.Comments != cmpBF.Comments;

                                    if (!genericDifference) genericDifference = s6xBF.HideParent != cmpBF.HideParent;
                                    if (!genericDifference) genericDifference = s6xBF.NotSetValue != cmpBF.NotSetValue;
                                    if (!genericDifference) genericDifference = s6xBF.SetValue != cmpBF.SetValue;

                                    if (!genericDifference) genericDifference = s6xBF.Category != cmpBF.Category;
                                    if (!genericDifference) genericDifference = s6xBF.Category2 != cmpBF.Category2;
                                    if (!genericDifference) genericDifference = s6xBF.Category3 != cmpBF.Category3;
                                    if (!genericDifference) genericDifference = s6xBF.IdentificationDetails != cmpBF.IdentificationDetails;
                                    if (!genericDifference) genericDifference = s6xBF.IdentificationStatus != cmpBF.IdentificationStatus;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slStructures[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slTables[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slFunctions[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);

                    if (uResult[3] == null) uResult[3] = cmpSadS6x.slScalars[s6xObject.UniqueAddress];
                    if (uResult[3] != null) uResult[1] = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);

                    if (!s6xObject.Skip && uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slRoutines[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xRoutine cmpObject = (S6xRoutine)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.isAdvanced != cmpObject.isAdvanced) uResult[4] = uResult[4].ToString() + "Different advanced definitions\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Label != cmpObject.Label;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;

                        SortedList s6xSubObjects = null;
                        SortedList cmpSubObjects = null;
                        if (!genericDifference && s6xObject.InputArguments != null && cmpObject.InputArguments != null)
                        {
                            genericDifference = s6xObject.InputArguments.Length != cmpObject.InputArguments.Length;
                            if (!genericDifference)
                            {
                                s6xSubObjects = new SortedList();
                                cmpSubObjects = new SortedList();
                                foreach (S6xRoutineInputArgument s6xSubObject in s6xObject.InputArguments) s6xSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
                                foreach (S6xRoutineInputArgument s6xSubObject in cmpObject.InputArguments) cmpSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);

                                foreach (S6xRoutineInputArgument s6xSubObject in s6xSubObjects.Values)
                                {
                                    genericDifference = !cmpSubObjects.ContainsKey(s6xSubObject.UniqueKey);
                                    if (genericDifference) break;

                                    S6xRoutineInputArgument cmpSubObject = (S6xRoutineInputArgument)cmpSubObjects[s6xSubObject.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xSubObject.Position != cmpSubObject.Position;
                                    if (!genericDifference) genericDifference = s6xSubObject.Encryption != cmpSubObject.Encryption;
                                    if (!genericDifference) genericDifference = s6xSubObject.Word != cmpSubObject.Word;
                                    if (!genericDifference) genericDifference = s6xSubObject.Pointer != cmpSubObject.Pointer;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (!genericDifference && s6xObject.InputStructures != null && cmpObject.InputStructures != null)
                        {
                            genericDifference = s6xObject.InputStructures.Length != cmpObject.InputStructures.Length;
                            if (!genericDifference)
                            {
                                s6xSubObjects = new SortedList();
                                cmpSubObjects = new SortedList();
                                foreach (S6xRoutineInputStructure s6xSubObject in s6xObject.InputStructures) s6xSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
                                foreach (S6xRoutineInputStructure s6xSubObject in cmpObject.InputStructures) cmpSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);

                                foreach (S6xRoutineInputStructure s6xSubObject in s6xSubObjects.Values)
                                {
                                    genericDifference = !cmpSubObjects.ContainsKey(s6xSubObject.UniqueKey);
                                    if (genericDifference) break;

                                    S6xRoutineInputStructure cmpSubObject = (S6xRoutineInputStructure)cmpSubObjects[s6xSubObject.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedNumber != cmpSubObject.ForcedNumber;
                                    if (!genericDifference) genericDifference = s6xSubObject.StructDef != cmpSubObject.StructDef;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableAddress != cmpSubObject.VariableAddress;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableNumber != cmpSubObject.VariableNumber;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (!genericDifference && s6xObject.InputTables != null && cmpObject.InputTables != null)
                        {
                            genericDifference = s6xObject.InputTables.Length != cmpObject.InputTables.Length;
                            if (!genericDifference)
                            {
                                s6xSubObjects = new SortedList();
                                cmpSubObjects = new SortedList();
                                foreach (S6xRoutineInputTable s6xSubObject in s6xObject.InputTables) s6xSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
                                foreach (S6xRoutineInputTable s6xSubObject in cmpObject.InputTables) cmpSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);

                                foreach (S6xRoutineInputTable s6xSubObject in s6xSubObjects.Values)
                                {
                                    genericDifference = !cmpSubObjects.ContainsKey(s6xSubObject.UniqueKey);
                                    if (genericDifference) break;

                                    S6xRoutineInputTable cmpSubObject = (S6xRoutineInputTable)cmpSubObjects[s6xSubObject.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedCellsScaleExpression != cmpSubObject.ForcedCellsScaleExpression;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedCellsScalePrecision != cmpSubObject.ForcedCellsScalePrecision;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedCellsUnits != cmpSubObject.ForcedCellsUnits;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedColsNumber != cmpSubObject.ForcedColsNumber;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedColsUnits != cmpSubObject.ForcedColsUnits;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedRowsNumber != cmpSubObject.ForcedRowsNumber;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedRowsUnits != cmpSubObject.ForcedRowsUnits;
                                    if (!genericDifference) genericDifference = s6xSubObject.SignedOutput != cmpSubObject.SignedOutput;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableAddress != cmpSubObject.VariableAddress;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableColsNumberReg != cmpSubObject.VariableColsNumberReg;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableColsReg != cmpSubObject.VariableColsReg;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableOutput != cmpSubObject.VariableOutput;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableRowsReg != cmpSubObject.VariableRowsReg;
                                    if (!genericDifference) genericDifference = s6xSubObject.WordOutput != cmpSubObject.WordOutput;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (!genericDifference && s6xObject.InputFunctions != null && cmpObject.InputFunctions != null)
                        {
                            genericDifference = s6xObject.InputFunctions.Length != cmpObject.InputFunctions.Length;
                            if (!genericDifference)
                            {
                                s6xSubObjects = new SortedList();
                                cmpSubObjects = new SortedList();
                                foreach (S6xRoutineInputFunction s6xSubObject in s6xObject.InputFunctions) s6xSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
                                foreach (S6xRoutineInputFunction s6xSubObject in cmpObject.InputFunctions) cmpSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);

                                foreach (S6xRoutineInputFunction s6xSubObject in s6xSubObjects.Values)
                                {
                                    genericDifference = !cmpSubObjects.ContainsKey(s6xSubObject.UniqueKey);
                                    if (genericDifference) break;

                                    S6xRoutineInputFunction cmpSubObject = (S6xRoutineInputFunction)cmpSubObjects[s6xSubObject.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xSubObject.ByteInput != cmpSubObject.ByteInput;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedInputScaleExpression != cmpSubObject.ForcedInputScaleExpression;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedInputScalePrecision != cmpSubObject.ForcedInputScalePrecision;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedInputUnits != cmpSubObject.ForcedInputUnits;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedOutputScaleExpression != cmpSubObject.ForcedOutputScaleExpression;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedOutputScalePrecision != cmpSubObject.ForcedOutputScalePrecision;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedOutputUnits != cmpSubObject.ForcedOutputUnits;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedRowsNumber != cmpSubObject.ForcedRowsNumber;
                                    if (!genericDifference) genericDifference = s6xSubObject.SignedInput != cmpSubObject.SignedInput;
                                    if (!genericDifference) genericDifference = s6xSubObject.SignedOutput != cmpSubObject.SignedOutput;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableAddress != cmpSubObject.VariableAddress;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableInput != cmpSubObject.VariableInput;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableOutput != cmpSubObject.VariableOutput;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (!genericDifference && s6xObject.InputScalars != null && cmpObject.InputScalars != null)
                        {
                            genericDifference = s6xObject.InputScalars.Length != cmpObject.InputScalars.Length;
                            if (!genericDifference)
                            {
                                s6xSubObjects = new SortedList();
                                cmpSubObjects = new SortedList();
                                foreach (S6xRoutineInputScalar s6xSubObject in s6xObject.InputScalars) s6xSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
                                foreach (S6xRoutineInputScalar s6xSubObject in cmpObject.InputScalars) cmpSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);

                                foreach (S6xRoutineInputScalar s6xSubObject in s6xSubObjects.Values)
                                {
                                    genericDifference = !cmpSubObjects.ContainsKey(s6xSubObject.UniqueKey);
                                    if (genericDifference) break;

                                    S6xRoutineInputScalar cmpSubObject = (S6xRoutineInputScalar)cmpSubObjects[s6xSubObject.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xSubObject.Byte != cmpSubObject.Byte;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedScaleExpression != cmpSubObject.ForcedScaleExpression;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedScalePrecision != cmpSubObject.ForcedScalePrecision;
                                    if (!genericDifference) genericDifference = s6xSubObject.ForcedUnits != cmpSubObject.ForcedUnits;
                                    if (!genericDifference) genericDifference = s6xSubObject.Signed != cmpSubObject.Signed;
                                    if (!genericDifference) genericDifference = s6xSubObject.VariableAddress != cmpSubObject.VariableAddress;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (!s6xObject.Skip) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
            {
                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slOperations[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xOperation cmpObject = (S6xOperation)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Label != cmpObject.Label;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;

                        if (!genericDifference) genericDifference = s6xObject.InlineComments != cmpObject.InlineComments;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;

                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (!s6xObject.Skip) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
            {
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slRegisters[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xRegister cmpObject = (S6xRegister)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.Label != cmpObject.Label) uResult[4] = uResult[4].ToString() + "Different labels\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;

                        if (!genericDifference) genericDifference = s6xObject.ByteLabel != cmpObject.ByteLabel;
                        if (!genericDifference) genericDifference = s6xObject.WordLabel != cmpObject.WordLabel;
                        if (!genericDifference) genericDifference = s6xObject.ConstValue != cmpObject.ConstValue;
                        if (!genericDifference) genericDifference = s6xObject.isRBase != cmpObject.isRBase;
                        if (!genericDifference) genericDifference = s6xObject.isRConst != cmpObject.isRConst;
                        if (!genericDifference) genericDifference = s6xObject.ScaleExpression != cmpObject.ScaleExpression;
                        if (!genericDifference) genericDifference = s6xObject.ScalePrecision != cmpObject.ScalePrecision;
                        if (!genericDifference) genericDifference = s6xObject.Units != cmpObject.Units;
                        if (!genericDifference) genericDifference = s6xObject.SizeStatus != cmpObject.SizeStatus;
                        if (!genericDifference) genericDifference = s6xObject.SignedStatus != cmpObject.SignedStatus;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;

                        SortedList s6xBitFlags = null;
                        SortedList cmpBitFlags = null;
                        if (!genericDifference && s6xObject.BitFlags != null && cmpObject.BitFlags != null)
                        {
                            genericDifference = s6xObject.BitFlags.Length != cmpObject.BitFlags.Length;
                            if (!genericDifference)
                            {
                                s6xBitFlags = new SortedList();
                                cmpBitFlags = new SortedList();
                                foreach (S6xBitFlag s6xBF in s6xObject.BitFlags) s6xBitFlags.Add(s6xBF.UniqueKey, s6xBF);
                                foreach (S6xBitFlag s6xBF in cmpObject.BitFlags) cmpBitFlags.Add(s6xBF.UniqueKey, s6xBF);

                                foreach (S6xBitFlag s6xBF in s6xBitFlags.Values)
                                {
                                    genericDifference = !cmpBitFlags.ContainsKey(s6xBF.UniqueKey);
                                    if (genericDifference) break;

                                    S6xBitFlag cmpBF = (S6xBitFlag)cmpBitFlags[s6xBF.UniqueKey];

                                    if (!genericDifference) genericDifference = s6xBF.ShortLabel != cmpBF.ShortLabel;
                                    if (!genericDifference) genericDifference = s6xBF.Label != cmpBF.Label;
                                    if (!genericDifference) genericDifference = s6xBF.Comments != cmpBF.Comments;

                                    if (!genericDifference) genericDifference = s6xBF.HideParent != cmpBF.HideParent;
                                    if (!genericDifference) genericDifference = s6xBF.NotSetValue != cmpBF.NotSetValue;
                                    if (!genericDifference) genericDifference = s6xBF.SetValue != cmpBF.SetValue;

                                    if (!genericDifference) genericDifference = s6xBF.Category != cmpBF.Category;
                                    if (!genericDifference) genericDifference = s6xBF.Category2 != cmpBF.Category2;
                                    if (!genericDifference) genericDifference = s6xBF.Category3 != cmpBF.Category3;
                                    if (!genericDifference) genericDifference = s6xBF.IdentificationDetails != cmpBF.IdentificationDetails;
                                    if (!genericDifference) genericDifference = s6xBF.IdentificationStatus != cmpBF.IdentificationStatus;

                                    if (genericDifference) break;
                                }
                            }
                        }

                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (!s6xObject.Skip) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
            {
                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slOtherAddresses[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xOtherAddress cmpObject = (S6xOtherAddress)uResult[3];

                    if (s6xObject.Skip != cmpObject.Skip) uResult[4] = uResult[4].ToString() + "Different skip flags\r\n";
                    if (s6xObject.Label != cmpObject.Label) uResult[4] = uResult[4].ToString() + "Different labels\r\n";

                    if (findGenericDifferences)
                    {
                        bool genericDifference = false;
                        if (!genericDifference) genericDifference = s6xObject.OutputLabel != cmpObject.OutputLabel;
                        if (!genericDifference) genericDifference = s6xObject.Comments != cmpObject.Comments;
                        if (!genericDifference) genericDifference = s6xObject.OutputComments != cmpObject.OutputComments;

                        if (!genericDifference) genericDifference = s6xObject.InlineComments != cmpObject.InlineComments;

                        if (!genericDifference) genericDifference = s6xObject.Category != cmpObject.Category;
                        if (!genericDifference) genericDifference = s6xObject.Category2 != cmpObject.Category2;
                        if (!genericDifference) genericDifference = s6xObject.Category3 != cmpObject.Category3;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationDetails != cmpObject.IdentificationDetails;
                        if (!genericDifference) genericDifference = s6xObject.IdentificationStatus != cmpObject.IdentificationStatus;

                        if (genericDifference) uResult[4] = uResult[4].ToString() + "Other differences\r\n";
                    }

                    if (uResult[4].ToString() != string.Empty) uResult[1] = "DIFF";
                }
                else
                {
                    // Other Type or Missing
                    if (!s6xObject.Skip) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            // Missing On Source S6x
            foreach (S6xStructure cmpObject in cmpSadS6x.slStructures.Values)
            {
                if (cmpObject.Skip) continue;
                if (!cmpObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slStructures[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slTables[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slFunctions[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slScalars[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }
            foreach (S6xTable cmpObject in cmpSadS6x.slTables.Values)
            {
                if (cmpObject.Skip) continue;
                if (!cmpObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slStructures[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slTables[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slFunctions[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slScalars[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }
            foreach (S6xFunction cmpObject in cmpSadS6x.slFunctions.Values)
            {
                if (cmpObject.Skip) continue;
                if (!cmpObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slStructures[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slTables[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slFunctions[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slScalars[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }
            foreach (S6xScalar cmpObject in cmpSadS6x.slScalars.Values)
            {
                if (cmpObject.Skip) continue;
                if (!cmpObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slStructures[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slTables[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slFunctions[cmpObject.UniqueAddress];
                if (uResult[2] == null) uResult[2] = sadS6x.slScalars[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }

            foreach (S6xRoutine cmpObject in cmpSadS6x.slRoutines.Values)
            {
                if (cmpObject.Skip) continue;
                if (!cmpObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slRoutines[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }

            foreach (S6xOperation cmpObject in cmpSadS6x.slOperations.Values)
            {
                if (cmpObject.Skip) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slOperations[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }

            foreach (S6xRegister cmpObject in cmpSadS6x.slRegisters.Values)
            {
                if (cmpObject.Skip) continue;
                if (!cmpObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slRegisters[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }

            foreach (S6xOtherAddress cmpObject in cmpSadS6x.slOtherAddresses.Values)
            {
                if (cmpObject.Skip) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER), "MISSING", null, cmpObject, string.Empty };
                if (uResult[2] == null) uResult[2] = sadS6x.slOtherAddresses[cmpObject.UniqueAddress];
                if (uResult[2] == null) results.Add(uResult);
            }

            return results;
        }

        private void searchTreeViewInit(string Mode)
        {
            searchTreeView.Nodes.Clear();

            if (Mode == "S6x")
            {
                TreeNode tnMissingIC = new TreeNode();
                tnMissingIC.Name = "MISSING_IN_COMPARED";
                tnMissingIC.Text = "Missing In Compared";
                tnMissingIC.ToolTipText = "Missing In Compared SAD 806x";
                searchTreeView.Nodes.Add(tnMissingIC);

                TreeNode tnDiff = new TreeNode();
                tnDiff.Name = "DIFFERENCES";
                tnDiff.Text = "Differences";
                tnDiff.ToolTipText = "Differences between SAD 806x";
                searchTreeView.Nodes.Add(tnDiff);

                TreeNode tnMissingIS = new TreeNode();
                tnMissingIS.Name = "MISSING_IN_SOURCE";
                tnMissingIS.Text = "Missing In Source";
                tnMissingIS.ToolTipText = "Missing In Source SAD 806x";
                searchTreeView.Nodes.Add(tnMissingIS);

                //      20210119 - No management for Signatures & ElementSignatures, because based on Unique keys, without needed Unique matching address
                foreach (TreeNode tnHeader in searchTreeView.Nodes)
                {
                    foreach (TreeNode tnMainParent in elemsTreeView.Nodes)
                    {
                        S6xNavInfo niMainHeaderCateg = new S6xNavInfo(tnMainParent);
                        if (!niMainHeaderCateg.isValid) continue;
                        switch (niMainHeaderCateg.HeaderCategory)
                        {
                            case S6xNavHeaderCategory.PROPERTIES:
                            case S6xNavHeaderCategory.RESERVED:
                            case S6xNavHeaderCategory.SIGNATURES:
                            case S6xNavHeaderCategory.ELEMSSIGNATURES:
                                break;
                            case S6xNavHeaderCategory.TABLES:
                            case S6xNavHeaderCategory.FUNCTIONS:
                            case S6xNavHeaderCategory.SCALARS:
                            case S6xNavHeaderCategory.STRUCTURES:
                            case S6xNavHeaderCategory.ROUTINES:
                            case S6xNavHeaderCategory.OPERATIONS:
                            case S6xNavHeaderCategory.REGISTERS:
                            case S6xNavHeaderCategory.OTHER:
                                TreeNode tnParent = new TreeNode();
                                tnParent.Name = tnMainParent.Name;
                                tnParent.Text = S6xNav.getHeaderCategLabel(niMainHeaderCateg.HeaderCategory);
                                tnParent.ToolTipText = S6xNav.getHeaderCategToolTip(niMainHeaderCateg.HeaderCategory);
                                tnParent.StateImageKey = tnMainParent.StateImageKey;
                                tnParent.ContextMenuStrip = resultCategContextMenuStrip;
                                tnHeader.Nodes.Add(tnParent);
                                tnHeader.ContextMenuStrip = resultCategContextMenuStrip;
                                break;
                        }
                    }
                }
                return;
            }

            if (Mode == "Bin")
            {
                foreach (TreeNode tnMainParent in elemsTreeView.Nodes)
                {
                    S6xNavInfo niMainHeaderCateg = new S6xNavInfo(tnMainParent);
                    if (!niMainHeaderCateg.isValid) continue;
                    switch (niMainHeaderCateg.HeaderCategory)
                    {
                        case S6xNavHeaderCategory.PROPERTIES:
                        case S6xNavHeaderCategory.RESERVED:
                        case S6xNavHeaderCategory.REGISTERS:
                        case S6xNavHeaderCategory.OPERATIONS:
                        case S6xNavHeaderCategory.OTHER:
                        case S6xNavHeaderCategory.SIGNATURES:
                        case S6xNavHeaderCategory.ELEMSSIGNATURES:
                            break;
                        case S6xNavHeaderCategory.TABLES:
                        case S6xNavHeaderCategory.FUNCTIONS:
                        case S6xNavHeaderCategory.SCALARS:
                        case S6xNavHeaderCategory.STRUCTURES:
                        case S6xNavHeaderCategory.ROUTINES:
                            if (!binariesSameDefinition && niMainHeaderCateg.HeaderCategory == S6xNavHeaderCategory.ROUTINES) break;
                            TreeNode tnParent = new TreeNode();
                            tnParent.Name = tnMainParent.Name;
                            tnParent.Text = S6xNav.getHeaderCategLabel(niMainHeaderCateg.HeaderCategory);
                            tnParent.ToolTipText = S6xNav.getHeaderCategToolTip(niMainHeaderCateg.HeaderCategory);
                            tnParent.StateImageKey = tnMainParent.StateImageKey;
                            tnParent.ContextMenuStrip = resultCategContextMenuStrip;
                            searchTreeView.Nodes.Add(tnParent);
                            break;
                    }
                }
                return;
            }
        }

        private void searchTreeViewCount(string Mode)
        {
            if (Mode == "S6x")
            {
                foreach (TreeNode tnHeader in searchTreeView.Nodes)
                {
                    string headerLabel = string.Empty;
                    switch (tnHeader.Name)
                    {
                        case "MISSING_IN_COMPARED":
                            headerLabel = "Missing In Compared";
                            break;
                        case "DIFFERENCES":
                            headerLabel = "Differences";
                            break;
                        case "MISSING_IN_SOURCE":
                            headerLabel = "Missing In Source";
                            break;
                    }

                    int iCnt = 0;
                    foreach (TreeNode tnParent in tnHeader.Nodes)
                    {
                        string categLabel = S6xNav.getHeaderCategLabel(tnParent.Name);
                        if (categLabel == string.Empty) return;
                        tnParent.Text = categLabel + " (" + tnParent.Nodes.Count.ToString() + ")";
                        iCnt += tnParent.Nodes.Count;
                    }

                    tnHeader.Text = headerLabel + " (" + iCnt.ToString() + ")";
                }
            }

            if (Mode == "Bin")
            {
                foreach (TreeNode tnParent in searchTreeView.Nodes)
                {
                    string categLabel = string.Empty;
                    // 20210114 - PYM - Now managing remaining addresses
                    if (tnParent.Name == S6xNav.getHeaderCategName(S6xNavHeaderCategory.UNDEFINED)) categLabel = "Remaining differences";
                    else categLabel = S6xNav.getHeaderCategLabel(tnParent.Name);
                    if (categLabel == string.Empty) return;
                    tnParent.Text = categLabel + " (" + tnParent.Nodes.Count.ToString() + ")";
                }
            }
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchTreeView.SelectedNode.ExpandAll();
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchTreeView.SelectedNode.Collapse();
        }

        // importElementToolStripMenuItem_Click
        //      20210119 - No management for Duplicates, no real interest for comparing definitions
        //      20210119 - No management for Signatures & ElementSignatures, because based on Unique keys, without needed Unique matching address
        private void importElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Only availble for S6x comparison
            if (compareMode != "S6x") return;

            // Missing on Source & Differences Import

            S6xNavInfo niMFHeaderCateg = null;
            TreeNode tnMFNode = null;
            TreeNode cmpNode = searchTreeView.SelectedNode;
            TreeNode parentNode = cmpNode.Parent;
            
            if (cmpNode.Tag == null) return;
            if (parentNode == null) return;

            bool bCreate = false;
            string categName = string.Empty;

            switch (S6xNav.getHeaderCateg(parentNode.Name))
            {
                case S6xNavHeaderCategory.STRUCTURES:
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.FUNCTIONS:
                case S6xNavHeaderCategory.SCALARS:
                case S6xNavHeaderCategory.ROUTINES:
                case S6xNavHeaderCategory.OPERATIONS:
                case S6xNavHeaderCategory.REGISTERS:
                case S6xNavHeaderCategory.OTHER:
                    // It is for Missing in Source
                    bCreate = (parentNode.Parent.Name == "MISSING_IN_SOURCE");
                    break;
                default:
                    // It is Source Node
                    switch (S6xNav.getHeaderCateg(parentNode.Parent.Name))
                    {
                        case S6xNavHeaderCategory.STRUCTURES:
                            if (cmpNode.Tag.GetType() != typeof(S6xStructure))
                            {
                                bCreate = true;
                                sadS6x.slStructures.Remove(parentNode.Name);
                            }
                            break;
                        case S6xNavHeaderCategory.TABLES:
                            if (cmpNode.Tag.GetType() != typeof(S6xTable))
                            {
                                bCreate = true;
                                sadS6x.slTables.Remove(parentNode.Name);
                            }
                            break;
                        case S6xNavHeaderCategory.FUNCTIONS:
                            if (cmpNode.Tag.GetType() != typeof(S6xFunction))
                            {
                                bCreate = true;
                                sadS6x.slFunctions.Remove(parentNode.Name);
                            }
                            break;
                        case S6xNavHeaderCategory.SCALARS:
                            if (cmpNode.Tag.GetType() != typeof(S6xScalar))
                            {
                                bCreate = true;
                                sadS6x.slScalars.Remove(parentNode.Name);
                            }
                            break;
                    }
                    if (bCreate)
                    {
                        if (elemsTreeView.Nodes.ContainsKey(parentNode.Parent.Name))
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[parentNode.Parent.Name]);
                            if (niMFHeaderCateg.isValid)
                            {
                                tnMFNode = niMFHeaderCateg.FindElement(parentNode.Name);
                                if (tnMFNode != null)
                                {
                                    tnMFNode.Parent.Nodes.Remove(tnMFNode);
                                    tnMFNode = null;
                                }
                            }
                            niMFHeaderCateg = null;
                        }
                    }
                    break;
            }

            if (cmpNode.Tag.GetType() == typeof(S6xStructure))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);
                ((S6xStructure)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xStructure)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slStructures.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slStructures[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xTable))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);
                ((S6xTable)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xTable)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slTables.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slTables[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xFunction))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);
                ((S6xFunction)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xFunction)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slFunctions.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slFunctions[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xScalar))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);
                ((S6xScalar)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xScalar)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slScalars.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slScalars[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xRoutine))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES);
                ((S6xRoutine)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xRoutine)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slRoutines.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slRoutines[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xOperation))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS);
                ((S6xOperation)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xOperation)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slOperations.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slOperations[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xRegister))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS);
                ((S6xRegister)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xRegister)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slRegisters.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slRegisters[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xOtherAddress))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER);
                ((S6xOtherAddress)cmpNode.Tag).DateUpdated = DateTime.UtcNow;
                if (bCreate)
                {
                    ((S6xOtherAddress)cmpNode.Tag).DateCreated = DateTime.UtcNow;
                    sadS6x.slOtherAddresses.Add(cmpNode.Name, cmpNode.Tag);
                }
                else sadS6x.slOtherAddresses[cmpNode.Name] = cmpNode.Tag;
            }

            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[categName]);
            tnMFNode = niMFHeaderCateg.FindElement(cmpNode.Name);

            object[] sharedDetails = null;
            S6xNavCategory nCateg = null;
            S6xNavCategory nCateg2 = null;
            S6xNavCategory nCateg3 = null;
            int idStatus = 0;

            if (cmpNode.Tag != null)
            {
                sharedDetails = S6xNav.getElemsTreeViewUpdateSharedDetails(cmpNode.Tag);
                if ((string)sharedDetails[0] != string.Empty && (string)sharedDetails[0] != null) nCateg = new S6xNavCategory((string)sharedDetails[0]);
                if ((string)sharedDetails[1] != string.Empty && (string)sharedDetails[1] != null) nCateg2 = new S6xNavCategory((string)sharedDetails[1]);
                if ((string)sharedDetails[2] != string.Empty && (string)sharedDetails[2] != null) nCateg3 = new S6xNavCategory((string)sharedDetails[2]);
                idStatus = (int)sharedDetails[3];
            }

            if (tnMFNode == null)
            {
                tnMFNode = new TreeNode();
                tnMFNode.Name = cmpNode.Name;
                tnMFNode.Text = cmpNode.Text;
                tnMFNode.ToolTipText = cmpNode.ToolTipText;
                tnMFNode.ForeColor = Color.Purple;
                tnMFNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(idStatus);
                tnMFNode.ContextMenuStrip = elemsContextMenuStrip;
                niMFHeaderCateg.AddNode(tnMFNode, nCateg, nCateg2, nCateg3, false, S6xNavCategoryDepth.MAXIMUM);
                niMFHeaderCateg.HeaderCategoryNode.Text = S6xNav.getHeaderCategLabel(niMFHeaderCateg.HeaderCategory) + " (" + niMFHeaderCateg.ElementsCount.ToString() + ")";
            }
            else
            {
                tnMFNode.Text = cmpNode.Text;
                tnMFNode.ToolTipText = cmpNode.ToolTipText;
                tnMFNode.ForeColor = Color.Purple;
                if (sharedDetails != null) tnMFNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(idStatus);
                S6xNavInfo niNI = new S6xNavInfo(tnMFNode);
                if (niNI.Category != nCateg || niNI.Category2 != nCateg2 || niNI.Category3 != nCateg3)
                // Node has to be moved
                {
                    tnMFNode.Parent.Nodes.Remove(tnMFNode);
                    niMFHeaderCateg.AddNode(tnMFNode, nCateg, nCateg2, nCateg3, false, S6xNavCategoryDepth.MAXIMUM);
                }
                niNI = null;
            }
            try { elemsTreeView.SelectedNode = tnMFNode; }
            catch { }

            switch (S6xNav.getHeaderCateg(parentNode.Name))
            {
                case S6xNavHeaderCategory.STRUCTURES:
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.FUNCTIONS:
                case S6xNavHeaderCategory.SCALARS:
                case S6xNavHeaderCategory.ROUTINES:
                case S6xNavHeaderCategory.OPERATIONS:
                case S6xNavHeaderCategory.REGISTERS:
                case S6xNavHeaderCategory.OTHER:
                    // It is for Missing in Source
                    parentNode.Nodes.Remove(cmpNode);
                    break;
                default:
                    // It is Source Node
                    parentNode.Parent.Nodes.Remove(parentNode);
                    break;
            }

            searchTreeViewCount(compareMode);

            searchTreeView.SelectedNode = null;

            sadS6x.isSaved = false;
        }
    }
}
