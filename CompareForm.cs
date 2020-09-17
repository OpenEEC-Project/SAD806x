using System;
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

        private System.Windows.Forms.Timer mainUpdateTimer = null;

        public CompareForm(ref SADBin mainSadBin, ref SADS6x mainSadS6x, ref TreeView mainElemsTreeView, ref ContextMenuStrip mainElemsContextMenuStrip, bool bBinaryComparison, bool bBinariesSameDefinition)
        {
            compareMode = bBinaryComparison ? "Bin" : "S6x";
            binariesSameDefinition = bBinariesSameDefinition;
            sadBin = mainSadBin;
            sadS6x = mainSadS6x;
            elemsTreeView = mainElemsTreeView;
            elemsContextMenuStrip = mainElemsContextMenuStrip; // For Nodes creation in Main Elems Tree View
            
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

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
            if (!niMFHeaderCateg.isValid) return;
            TreeNode tnMFNode = niMFHeaderCateg.FindElement(e.Node.Name);
            if (tnMFNode == null) return;
            try { elemsTreeView.SelectedNode = tnMFNode; }
            catch { }
            tnMFNode = null;
            niMFHeaderCateg = null;
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

            switch (Mode)
            {
                case "S6x":
                    foreach (object[] result in results)
                    {
                        TreeNode parentNode = null;

                        S6xStructure cmpStruct = null;
                        S6xTable cmpTable = null;
                        S6xFunction cmpFunction = null;
                        S6xScalar cmpScalar = null;

                        TreeNode tnNode = null;
                        TreeNode tnCmpNode = null;

                        if (result[2] == null) // Missing on S6x, Missing in Source
                        {
                            if (!searchTreeView.Nodes.ContainsKey("MISSING_IN_SOURCE")) continue;
                            parentNode = searchTreeView.Nodes["MISSING_IN_SOURCE"].Nodes[elemsTreeView.Nodes[result[0].ToString()].Name];
                            if (parentNode == null) continue;

                            if (result[3] != null)
                            {
                                if (result[3].GetType() == typeof(S6xStructure)) cmpStruct = (S6xStructure)result[3];
                                else if (result[3].GetType() == typeof(S6xTable)) cmpTable = (S6xTable)result[3];
                                else if (result[3].GetType() == typeof(S6xFunction)) cmpFunction = (S6xFunction)result[3];
                                else if (result[3].GetType() == typeof(S6xScalar)) cmpScalar = (S6xScalar)result[3];
                            }

                            tnCmpNode = new TreeNode();
                            if (cmpStruct != null)
                            {
                                tnCmpNode.Name = cmpStruct.UniqueAddress;
                                tnCmpNode.Text = cmpStruct.Label;
                                tnCmpNode.ToolTipText = cmpStruct.UniqueAddressHex + "\r\n" + cmpStruct.ShortLabel + "\r\n\r\n" + cmpStruct.Comments;
                            }
                            else if (cmpTable != null)
                            {
                                tnCmpNode.Name = cmpTable.UniqueAddress;
                                tnCmpNode.Text = cmpTable.Label;
                                tnCmpNode.ToolTipText = cmpTable.UniqueAddressHex + "\r\n" + cmpTable.ShortLabel + "\r\n\r\n" + cmpTable.Comments;
                            }
                            else if (cmpFunction != null)
                            {
                                tnCmpNode.Name = cmpFunction.UniqueAddress;
                                tnCmpNode.Text = cmpFunction.Label;
                                tnCmpNode.ToolTipText = cmpFunction.UniqueAddressHex + "\r\n" + cmpFunction.ShortLabel + "\r\n\r\n" + cmpFunction.Comments;
                            }
                            else if (cmpScalar != null)
                            {
                                tnCmpNode.Name = cmpScalar.UniqueAddress;
                                tnCmpNode.Text = cmpScalar.Label;
                                tnCmpNode.ToolTipText = cmpScalar.UniqueAddressHex + "\r\n" + cmpScalar.ShortLabel + "\r\n\r\n" + cmpScalar.Comments;
                            }
                            else
                            {
                                tnCmpNode = null;
                            }
                            if (tnCmpNode == null) continue;

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

                            if (!elemsTreeView.Nodes[result[0].ToString()].Nodes.ContainsKey(result[2].ToString())) continue;
                            if (parentNode.Nodes.ContainsKey(result[2].ToString())) continue;

                            tnNode = new TreeNode();
                            tnNode.Name = elemsTreeView.Nodes[result[0].ToString()].Nodes[result[2].ToString()].Name;
                            tnNode.Text = elemsTreeView.Nodes[result[0].ToString()].Nodes[result[2].ToString()].Text;
                            tnNode.ToolTipText = elemsTreeView.Nodes[result[0].ToString()].Nodes[result[2].ToString()].ToolTipText;
                            parentNode.Nodes.Add(tnNode);

                            if (result[3] != null)
                            {
                                if (result[3].GetType() == typeof(S6xStructure)) cmpStruct = (S6xStructure)result[3];
                                else if (result[3].GetType() == typeof(S6xTable)) cmpTable = (S6xTable)result[3];
                                else if (result[3].GetType() == typeof(S6xFunction)) cmpFunction = (S6xFunction)result[3];
                                else if (result[3].GetType() == typeof(S6xScalar)) cmpScalar = (S6xScalar)result[3];
                            }

                            tnCmpNode = new TreeNode();

                            if (cmpStruct != null)
                            {
                                tnCmpNode.Name = cmpStruct.UniqueAddress;
                                tnCmpNode.Text = cmpStruct.Label;
                                tnCmpNode.ToolTipText = cmpStruct.UniqueAddressHex + "\r\n" + cmpStruct.ShortLabel + "\r\n\r\n" + cmpStruct.Comments;
                            }
                            else if (cmpTable != null)
                            {
                                tnCmpNode.Name = cmpTable.UniqueAddress;
                                tnCmpNode.Text = cmpTable.Label;
                                tnCmpNode.ToolTipText = cmpTable.UniqueAddressHex + "\r\n" + cmpTable.ShortLabel + "\r\n\r\n" + cmpTable.Comments;
                            }
                            else if (cmpFunction != null)
                            {
                                tnCmpNode.Name = cmpFunction.UniqueAddress;
                                tnCmpNode.Text = cmpFunction.Label;
                                tnCmpNode.ToolTipText = cmpFunction.UniqueAddressHex + "\r\n" + cmpFunction.ShortLabel + "\r\n\r\n" + cmpFunction.Comments;
                            }
                            else if (cmpScalar != null)
                            {
                                tnCmpNode.Name = cmpScalar.UniqueAddress;
                                tnCmpNode.Text = cmpScalar.Label;
                                tnCmpNode.ToolTipText = cmpScalar.UniqueAddressHex + "\r\n" + cmpScalar.ShortLabel + "\r\n\r\n" + cmpScalar.Comments;                            }
                            else
                            {
                                tnCmpNode = null;
                            }
                            if (diffMessage != string.Empty)
                            {
                                tnNode.ToolTipText = diffMessage + "\r\n" + tnNode.ToolTipText;
                                if (tnCmpNode != null) tnCmpNode.ToolTipText = diffMessage + "\r\n" + tnCmpNode.ToolTipText;
                            }
                            if (tnCmpNode != null)
                            {
                                tnCmpNode.Tag = result[3];
                                tnCmpNode.ContextMenuStrip = resultElemContextMenuStrip;
                                tnNode.Nodes.Add(tnCmpNode);
                            }
                        }
                    }
                    break;
                case "Bin":
                default:
                    foreach (object[] result in results)
                    {
                        TreeNode parentNode = searchTreeView.Nodes[elemsTreeView.Nodes[result[0].ToString()].Name];
                        if (parentNode == null) continue;

                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[result[0].ToString()]);
                        if (!niMFHeaderCateg.isValid) continue;
                        TreeNode tnMFNode = niMFHeaderCateg.FindElement(result[1].ToString());
                        if (tnMFNode == null) continue;

                        if (parentNode.Nodes.ContainsKey(result[1].ToString())) continue;

                        TreeNode tnNode = new TreeNode();
                        tnNode.Name = tnMFNode.Name;
                        tnNode.Text = tnMFNode.Text;
                        tnNode.ToolTipText = tnMFNode.ToolTipText;
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
                    if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), extObject.UniqueAddress });
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
                    if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), extObject.UniqueAddress });
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
                    if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), extObject.UniqueAddress });
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
                    if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), extObject.UniqueAddress });
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
                    if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), extObject.UniqueAddress });
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
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), srcObject.UniqueAddress });

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
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), srcObject.UniqueAddress });

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
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), srcObject.UniqueAddress });

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
                        if (isResult) results.Add(new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), srcObject.UniqueAddress });

                        break;
                    }
                }
            }

            return results;
        }

        private List<object[]> CompareNewS6x()
        {
            List<object[]> results = new List<object[]>();

            string cmpS6xFilePath = string.Empty;

            string sError = string.Empty;

            bool bError = false;

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
                if (s6xObject.Skip) continue;
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slStructures[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xStructure cmpObject = (S6xStructure)uResult[3];

                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.Number != cmpObject.Number) uResult[4] = uResult[4].ToString() + "Different numbers\r\n";
                    if (s6xObject.StructDef != cmpObject.StructDef) uResult[4] = uResult[4].ToString() + "Different structure definitions\r\n";

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

                    if (uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                if (s6xObject.Skip) continue;
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slTables[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xTable cmpObject = (S6xTable)uResult[3];

                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.ColsNumber != cmpObject.ColsNumber) uResult[4] = uResult[4].ToString() + "Different columns numbers\r\n";
                    if (s6xObject.RowsNumber != cmpObject.RowsNumber) uResult[4] = uResult[4].ToString() + "Different rows numbers\r\n";
                    if (s6xObject.WordOutput != cmpObject.WordOutput) uResult[4] = uResult[4].ToString() + "Different type\r\n";
                    if (s6xObject.SignedOutput != cmpObject.SignedOutput) uResult[4] = uResult[4].ToString() + "Different sign\r\n";

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

                    if (uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                if (s6xObject.Skip) continue;
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slFunctions[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xFunction cmpObject = (S6xFunction)uResult[3];

                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.RowsNumber != cmpObject.RowsNumber) uResult[4] = uResult[4].ToString() + "Different rows numbers\r\n";
                    if (s6xObject.ByteInput != cmpObject.ByteInput) uResult[4] = uResult[4].ToString() + "Different input type\r\n";
                    if (s6xObject.ByteOutput != cmpObject.ByteOutput) uResult[4] = uResult[4].ToString() + "Different output type\r\n";
                    if (s6xObject.SignedInput != cmpObject.SignedInput) uResult[4] = uResult[4].ToString() + "Different input sign\r\n";
                    if (s6xObject.SignedOutput != cmpObject.SignedOutput) uResult[4] = uResult[4].ToString() + "Different output sign\r\n";

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

                    if (uResult[3] == null) uResult[1] = "MISSING";
                }
                if (uResult[1].ToString() == string.Empty) continue;
                results.Add(uResult);
            }

            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                if (s6xObject.Skip) continue;
                if (!s6xObject.Store) continue;

                object[] uResult = new object[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), string.Empty, s6xObject.UniqueAddress, null, string.Empty };
                uResult[3] = cmpSadS6x.slScalars[s6xObject.UniqueAddress];

                if (uResult[3] != null)
                {
                    S6xScalar cmpObject = (S6xScalar)uResult[3];

                    if (s6xObject.ShortLabel != cmpObject.ShortLabel) uResult[4] = uResult[4].ToString() + "Different short labels\r\n";
                    if (s6xObject.Byte != cmpObject.Byte) uResult[4] = uResult[4].ToString() + "Different type\r\n";
                    if (s6xObject.isBitFlags != cmpObject.isBitFlags) uResult[4] = uResult[4].ToString() + "Different bit flags type\r\n";
                    if (s6xObject.Signed != cmpObject.Signed) uResult[4] = uResult[4].ToString() + "Different sign\r\n";

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

                    if (uResult[3] == null) uResult[1] = "MISSING";
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
                            case S6xNavHeaderCategory.REGISTERS:
                            case S6xNavHeaderCategory.OPERATIONS:
                            case S6xNavHeaderCategory.OTHER:
                            case S6xNavHeaderCategory.SIGNATURES:
                            case S6xNavHeaderCategory.ELEMSSIGNATURES:
                            case S6xNavHeaderCategory.ROUTINES:
                                break;
                            case S6xNavHeaderCategory.TABLES:
                            case S6xNavHeaderCategory.FUNCTIONS:
                            case S6xNavHeaderCategory.SCALARS:
                            case S6xNavHeaderCategory.STRUCTURES:
                                TreeNode tnParent = new TreeNode();
                                tnParent.Name = tnMainParent.Name;
                                tnParent.Text = S6xNav.getHeaderCategLabel(niMainHeaderCateg.HeaderCategory);
                                tnParent.ToolTipText = S6xNav.getHeaderCategToolTip(niMainHeaderCateg.HeaderCategory);
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
                            TreeNode tnParent = new TreeNode();
                            tnParent.Name = tnMainParent.Name;
                            tnParent.Text = S6xNav.getHeaderCategLabel(niMainHeaderCateg.HeaderCategory);
                            tnParent.ToolTipText = S6xNav.getHeaderCategToolTip(niMainHeaderCateg.HeaderCategory);
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
                    string categLabel = S6xNav.getHeaderCategLabel(tnParent.Name);
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

        private void importElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (compareMode != "S6x") return;
            // Missing on Source & Differences Import

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
                    // Its is for Missing in Source
                    bCreate = (parentNode.Parent.Name == "MISSING_IN_SOURCE");
                    break;
                default:
                    // Its is Source Node
                    switch (S6xNav.getHeaderCateg(parentNode.Parent.Name))
                    {
                        case S6xNavHeaderCategory.STRUCTURES:
                            if (cmpNode.Tag.GetType() != typeof(S6xStructure))
                            {
                                bCreate = true;
                                sadS6x.slStructures.Remove(parentNode.Name);
                                if (elemsTreeView.Nodes.ContainsKey(parentNode.Parent.Name))
                                {
                                    if (elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.ContainsKey(parentNode.Name)) elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.RemoveByKey(parentNode.Name);
                                }
                            }
                            break;
                        case S6xNavHeaderCategory.TABLES:
                            if (cmpNode.Tag.GetType() != typeof(S6xTable))
                            {
                                bCreate = true;
                                sadS6x.slTables.Remove(parentNode.Name);
                                if (elemsTreeView.Nodes.ContainsKey(parentNode.Parent.Name))
                                {
                                    if (elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.ContainsKey(parentNode.Name)) elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.RemoveByKey(parentNode.Name);
                                }
                            }
                            break;
                        case S6xNavHeaderCategory.FUNCTIONS:
                            if (cmpNode.Tag.GetType() != typeof(S6xFunction))
                            {
                                bCreate = true;
                                sadS6x.slFunctions.Remove(parentNode.Name);
                                if (elemsTreeView.Nodes.ContainsKey(parentNode.Parent.Name))
                                {
                                    if (elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.ContainsKey(parentNode.Name)) elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.RemoveByKey(parentNode.Name);
                                }
                            }
                            break;
                        case S6xNavHeaderCategory.SCALARS:
                            if (cmpNode.Tag.GetType() != typeof(S6xScalar))
                            {
                                bCreate = true;
                                sadS6x.slScalars.Remove(parentNode.Name);
                                if (elemsTreeView.Nodes.ContainsKey(parentNode.Parent.Name))
                                {
                                    if (elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.ContainsKey(parentNode.Name)) elemsTreeView.Nodes[parentNode.Parent.Name].Nodes.RemoveByKey(parentNode.Name);
                                }
                            }
                            break;
                    }
                    break;
            }

            if (cmpNode.Tag.GetType() == typeof(S6xStructure))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);
                if (bCreate) sadS6x.slStructures.Add(cmpNode.Name, cmpNode.Tag);
                else sadS6x.slStructures[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xTable))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);
                if (bCreate) sadS6x.slTables.Add(cmpNode.Name, cmpNode.Tag);
                else sadS6x.slTables[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xFunction))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);
                if (bCreate) sadS6x.slFunctions.Add(cmpNode.Name, cmpNode.Tag);
                else sadS6x.slFunctions[cmpNode.Name] = cmpNode.Tag;
            }
            else if (cmpNode.Tag.GetType() == typeof(S6xScalar))
            {
                categName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);
                if (bCreate) sadS6x.slScalars.Add(cmpNode.Name, cmpNode.Tag);
                else sadS6x.slScalars[cmpNode.Name] = cmpNode.Tag;
            }

            S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[categName]);
            TreeNode s6xNode = niHeaderCateg.FindElement(cmpNode.Name);
            if (s6xNode == null)
            {
                s6xNode = new TreeNode();
                s6xNode.Name = cmpNode.Name;
                s6xNode.Text = cmpNode.Text;
                s6xNode.ToolTipText = cmpNode.ToolTipText;
                s6xNode.ForeColor = Color.Purple;
                s6xNode.ContextMenuStrip = elemsContextMenuStrip;
                niHeaderCateg.AddNode(s6xNode, null, null, null, false);
                niHeaderCateg.HeaderCategoryNode.Text = S6xNav.getHeaderCategLabel(niHeaderCateg.HeaderCategory) + " (" + niHeaderCateg.ElementsCount.ToString() + ")";
            }
            else
            {
                s6xNode.Text = cmpNode.Text;
                s6xNode.ToolTipText = cmpNode.ToolTipText;
                s6xNode.ForeColor = Color.Purple;
            }
            try { elemsTreeView.SelectedNode = s6xNode; }
            catch { }

            switch (S6xNav.getHeaderCateg(parentNode.Name))
            {
                case S6xNavHeaderCategory.STRUCTURES:
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.FUNCTIONS:
                case S6xNavHeaderCategory.SCALARS:
                    // Its is for Missing in Source
                    parentNode.Nodes.Remove(cmpNode);
                    break;
                default:
                    // Its is Source Node
                    parentNode.Parent.Nodes.Remove(parentNode);
                    break;
            }

            searchTreeViewCount(compareMode);

            searchTreeView.SelectedNode = null;

            sadS6x.isSaved = false;
        }
    }
}
