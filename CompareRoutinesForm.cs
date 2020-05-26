using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace SAD806x
{
    public partial class CompareRoutinesForm : Form
    {
        private SADBin sadBin = null;
        private SADBin cmpSadBin = null;
        private TreeView elemsTreeView = null;
        private ContextMenuStrip elemsContextMenuStrip = null;
        private SortedList currentSkeleton = null;
        private SortedList fileSkeleton = null;
        private bool skeletonMode = false;
        private string cmpSktFilePath = string.Empty;
        private string cmpBinFilePath = string.Empty;
        private string cmpS6xFilePath = string.Empty;
        private object[] lastExecutionOptions = null;

        private SortedList slPossibleMatchingRegisters = null;
        private SortedList slPossibleMatchingCalElements = null;
        private SortedList slPossibleMatchingOtherElements = null;

        private System.Windows.Forms.Timer processUpdateTimer = null;
        private Thread processThread = null;
        private bool processRunning = false;
        private string processError = string.Empty;
        private int progressGlobalStatus = 100;
        private Cursor processPreviousCursor = null;

        public CompareRoutinesForm(ref SADBin mainSadBin, ref TreeView mainElemsTreeView, ref ContextMenuStrip mainElemsContextMenuStrip, bool bSkeletonMode)
        {
            sadBin = mainSadBin;
            elemsTreeView = mainElemsTreeView;
            elemsContextMenuStrip = mainElemsContextMenuStrip; // For Nodes creation in Main Elems Tree View
            skeletonMode = bSkeletonMode;

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            processUpdateTimer = new System.Windows.Forms.Timer();
            processUpdateTimer.Enabled = false;
            processUpdateTimer.Interval = 100;
            processUpdateTimer.Tick += new EventHandler(processUpdateTimer_Tick);

            compareTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(compareTreeView_NodeMouseClick);
            compareTreeView.AfterSelect += new TreeViewEventHandler(compareTreeView_AfterSelect);

            filterDefinedToolStripMenuItem.CheckedChanged += new EventHandler(filterToolStripMenuItem_CheckedChanged);
            filterShortLabelToolStripMenuItem.CheckedChanged += new EventHandler(filterToolStripMenuItem_CheckedChanged);
            filterUniqueToolStripMenuItem.CheckedChanged += new EventHandler(filterToolStripMenuItem_CheckedChanged);

            selectButton.Text = skeletonMode ? "Select Skeleton" : "Select Binary";
            fileNameLabel.Text = skeletonMode ? "Skeleton file" : "Binary/S6x file(s)";
            
            minOpsCountTextBox.Text = "3";
            gapToleranceTextBox.Text = "10";
            distToleranceTextBox.Text = "70";
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processRunning)
            {
                MessageBox.Show("Please wait until the end of the process.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }

            if (!e.Cancel) Exit();
        }

        private void Exit()
        {
            if (processThread != null)
            {
                try { processThread.Abort(); }
                catch { }
            }
            processThread = null;

            sadBin = null;
            cmpSadBin = null;

            Dispose();

            GC.Collect();
        }

        private void processUpdateTimer_Tick(object sender, EventArgs e)
        {
            processUpdateTimer.Enabled = false;

            if (!processRunning) return;

            if (progressGlobalStatus > 100) progressGlobalStatus = 100;
            if (progressGlobalStatus < -1) progressGlobalStatus = -1;
            switch (progressGlobalStatus)
            {
                case 100:
                case -1:
                    endProcess();
                    break;
                default:
                    if (!skeletonMode && cmpSadBin != null)
                    {
                        if (cmpSadBin.ProgressStatus > 100) cmpSadBin.ProgressStatus = 100;
                        if (cmpSadBin.ProgressStatus < -1) cmpSadBin.ProgressStatus = -1;
                        switch (cmpSadBin.ProgressStatus)
                        {
                            case 100:
                            case -1:
                                if (progressGlobalStatus < 50) progressGlobalStatus = 50;
                                break;
                        }
                    }
                    toolStripProgressBarMain.Value = progressGlobalStatus;
                    processUpdateTimer.Enabled = true;
                    break;
            }
        }
        
        private void compareTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void compareTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null) return;
            if (e.Node.Parent.Parent != null) return;
            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[e.Node.Parent.Name]);
            if (!niMFHeaderCateg.isValid) return;
            TreeNode tnMFNode = niMFHeaderCateg.FindElement(e.Node.Name);
            if (tnMFNode == null) return;
            try { elemsTreeView.SelectedNode = tnMFNode; }
            catch { }
            tnMFNode = null;
            niMFHeaderCateg = null;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            if (skeletonMode) selectSkeleton();
            else selectBinary();
        }

        private void compareButton_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = false;
            compareButton.Enabled = false;
            exportReportButton.Enabled = false;

            processPreviousCursor = Cursor;
            Cursor = Cursors.WaitCursor;
            
            progressGlobalStatus = 0;
            toolStripProgressBarMain.Value = progressGlobalStatus;

            processRunning = true;
            processError = string.Empty;
            processUpdateTimer.Enabled = true;

            if (skeletonMode) processThread = new Thread(compareSkeleton);
            else processThread = new Thread(compareBinary);

            processThread.Start();
        }

        private void endProcess()
        {
            processThread = null;
            processRunning = false;

            compareAnalysis();
            compareOutput();

            selectButton.Enabled = true;
            compareButton.Enabled = true;
            exportReportButton.Enabled = true;

            toolStripProgressBarMain.Value = 100; 
            
            Cursor = processPreviousCursor;

            if (processError != string.Empty)
            {
                MessageBox.Show(processError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                processError = string.Empty;
            }
            
            GC.Collect();
        }
        
        private void selectSkeleton()
        {
            string sError = string.Empty;

            if (openFileDialogSkt.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(openFileDialogSkt.FileName))
            {
                sError += openFileDialogSkt.FileName + "\r\n";
                sError += "Not existing skeleton.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmpSktFilePath = openFileDialogSkt.FileName;
            FileInfo fiFI = new FileInfo(cmpSktFilePath);
            sFileNameTextBox.Text = fiFI.Name;
            fiFI = null;

            compareButton.Enabled = true;
        }

        private void selectBinary()
        {
            string sError = string.Empty;

            if (openFileDialogBin.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(openFileDialogBin.FileName))
            {
                sError += openFileDialogBin.FileName + "\r\n";
                sError += "Not existing binary.";
                MessageBox.Show(sError, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmpBinFilePath = openFileDialogBin.FileName;

            FileInfo fiFI = new FileInfo(cmpBinFilePath);
            cmpS6xFilePath = fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".s6x";
            sFileNameTextBox.Text = fiFI.Name;
            fiFI = null;

            if (File.Exists(cmpS6xFilePath))
            {
                fiFI = new FileInfo(cmpS6xFilePath);
                sFileNameTextBox.Text += " / " + fiFI.Name;
                fiFI = null;
            }
            else
            {
                cmpS6xFilePath = string.Empty;
            }

            compareButton.Enabled = true;
        }

        private void compareSkeleton()
        {
            int minOpsNumber = 3;      // 3 ops for minimum
            try { minOpsNumber = Convert.ToInt32(minOpsCountTextBox.Text); }
            catch { }
            if (minOpsNumber < 0) minOpsNumber = 0;

            double gapTolerance = 0.1; // 10%
            try { gapTolerance = Convert.ToDouble(gapToleranceTextBox.Text) / 100; }
            catch { }
            if (gapTolerance < 0.01 || gapTolerance > 1.0) gapTolerance = 0.1;

            double levTolerance = 0.7; // 70%
            try { levTolerance = Convert.ToDouble(distToleranceTextBox.Text) / 100; }
            catch { }
            if (levTolerance < 0.01 || levTolerance > 1.0) levTolerance = 0.7;

            lastExecutionOptions = new object[] { sadBin.BinaryFileName, cmpSktFilePath, minOpsNumber, gapTolerance, levTolerance };

            currentSkeleton = ToolsRoutinesComp.getRoutinesComparisonSkeleton(ref sadBin);
            progressGlobalStatus = 25;

            fileSkeleton = ToolsRoutinesComp.getRoutinesComparisonSkeleton(cmpSktFilePath);
            progressGlobalStatus = 50;

            ToolsRoutinesComp.compareRoutinesComparisonSkeletons(ref currentSkeleton, ref fileSkeleton, minOpsNumber, gapTolerance, levTolerance);
            progressGlobalStatus = 100;
        }

        private void compareBinary()
        {
            cmpSadBin = null;

            cmpSadBin = new SADBin(cmpBinFilePath, cmpS6xFilePath);

            bool bError = (cmpSadBin == null);
            if (bError)
            {
                processError += cmpBinFilePath + "\r\n";
                processError += "Unrecognized Binary.";
                progressGlobalStatus = -1;
                return;
            }

            bError = (!cmpSadBin.isLoaded || !cmpSadBin.isValid);
            if (bError)
            {
                processError += cmpBinFilePath + "\r\n";
                processError += "Unrecognized Binary.";
                progressGlobalStatus = -1;
                return;
            }

            cmpSadBin.processBin();
            bError = (!cmpSadBin.isDisassembled);
            if (bError)
            {
                processError += cmpBinFilePath + "\r\n";
                processError += "Binary can not be disassembled.";
                progressGlobalStatus = -1;
                return;
            }

            int minOpsNumber = 3;      // 3 ops for minimum
            try { minOpsNumber = Convert.ToInt32(minOpsCountTextBox.Text); }
            catch { }
            if (minOpsNumber < 0) minOpsNumber = 0;

            double gapTolerance = 0.1; // 10%
            try { gapTolerance = Convert.ToDouble(gapToleranceTextBox.Text) / 100; }
            catch { }
            if (gapTolerance < 0.01 || gapTolerance > 1.0) gapTolerance = 0.1;

            double levTolerance = 0.7; // 70%
            try { levTolerance = Convert.ToDouble(distToleranceTextBox.Text) / 100; }
            catch { }
            if (levTolerance < 0.01 || levTolerance > 1.0) levTolerance = 0.7;

            lastExecutionOptions = new object[] { sadBin.BinaryFileName, cmpBinFilePath, minOpsNumber, gapTolerance, levTolerance };

            currentSkeleton = ToolsRoutinesComp.getRoutinesComparisonSkeleton(ref sadBin);
            progressGlobalStatus = 10;

            fileSkeleton = ToolsRoutinesComp.getRoutinesComparisonSkeleton(ref cmpSadBin);
            progressGlobalStatus = 70;

            ToolsRoutinesComp.compareRoutinesComparisonSkeletons(ref currentSkeleton, ref fileSkeleton, minOpsNumber, gapTolerance, levTolerance);
            progressGlobalStatus = 100;
        }

        // Compare Analysis
        //      To move Routine level elements to top structures
        private void compareAnalysis()
        {
            if (currentSkeleton == null) return;
            if (currentSkeleton.Count == 0) return;

            if (!skeletonMode)
            {
                slPossibleMatchingCalElements = new SortedList();
                slPossibleMatchingOtherElements = new SortedList();
                slPossibleMatchingRegisters = new SortedList();

                foreach (RoutineSkeleton rsSkt in currentSkeleton.Values)
                {
                    if (rsSkt.alMatches == null) continue;
                    if (rsSkt.alMatches.Count == 0) continue;

                    if (rsSkt.slPossibleMatchingCalElements != null)
                    {
                        foreach (object[] matchingCalElem in rsSkt.slPossibleMatchingCalElements.Values)
                        {
                            string sEKey = (string)matchingCalElem[0];
                            SortedList slEMatches = null;
                            if (slPossibleMatchingCalElements.ContainsKey(sEKey))
                            {
                                slEMatches = (SortedList)((object[])slPossibleMatchingCalElements[sEKey])[1];
                                if (slEMatches.ContainsKey((string)matchingCalElem[1]))
                                {
                                    ((object[])slEMatches[(string)matchingCalElem[1]])[1] = (int)((object[])slEMatches[(string)matchingCalElem[1]])[1] + (int)matchingCalElem[2];
                                }
                                else
                                {
                                    slEMatches.Add((string)matchingCalElem[1], new object[] { (string)matchingCalElem[1], (int)matchingCalElem[2] });
                                }
                            }
                            else
                            {
                                slEMatches = new SortedList();
                                slEMatches.Add((string)matchingCalElem[1], new object[] { (string)matchingCalElem[1], (int)matchingCalElem[2] });
                                slPossibleMatchingCalElements.Add(sEKey, new object[] { sEKey, slEMatches });
                            }
                        }
                    }
                    if (rsSkt.slPossibleMatchingOtherElements != null)
                    {
                        foreach (object[] matchingOthElem in rsSkt.slPossibleMatchingOtherElements.Values)
                        {
                            string sEKey = (string)matchingOthElem[0];
                            SortedList slEMatches = null;
                            if (slPossibleMatchingOtherElements.ContainsKey(sEKey))
                            {
                                slEMatches = (SortedList)((object[])slPossibleMatchingOtherElements[sEKey])[1];
                                if (slEMatches.ContainsKey((string)matchingOthElem[1]))
                                {
                                    ((object[])slEMatches[(string)matchingOthElem[1]])[1] = (int)((object[])slEMatches[(string)matchingOthElem[1]])[1] + (int)matchingOthElem[2];
                                }
                                else
                                {
                                    slEMatches.Add((string)matchingOthElem[1], new object[] { (string)matchingOthElem[1], (int)matchingOthElem[2] });
                                }
                            }
                            else
                            {
                                slEMatches = new SortedList();
                                slEMatches.Add((string)matchingOthElem[1], new object[] { (string)matchingOthElem[1], (int)matchingOthElem[2] });
                                slPossibleMatchingOtherElements.Add(sEKey, new object[] { sEKey, slEMatches });
                            }
                        }
                    }
                    if (rsSkt.slPossibleMatchingRegisters != null)
                    {
                        foreach (object[] matchingReg in rsSkt.slPossibleMatchingRegisters.Values)
                        {
                            string sEKey = (string)matchingReg[0];
                            string slEKey = Tools.RegisterUniqueAddress(sEKey);
                            string slECKey = Tools.RegisterUniqueAddress((string)matchingReg[1]);
                            SortedList slEMatches = null;
                            SortedList slEBitFlags = null;
                            if (slPossibleMatchingRegisters.ContainsKey(slEKey))
                            {
                                slEMatches = (SortedList)((object[])slPossibleMatchingRegisters[slEKey])[1];
                                if (slEMatches.ContainsKey(slECKey))
                                {
                                    ((object[])slEMatches[slECKey])[1] = (int)((object[])slEMatches[slECKey])[1] + (int)matchingReg[2];
                                }
                                else
                                {
                                    slEMatches.Add(slECKey, new object[] { (string)matchingReg[1], (int)matchingReg[2] });
                                }

                                // BitFlags
                                if (matchingReg[3] != null)
                                {
                                    slEBitFlags = (SortedList)((object[])slPossibleMatchingRegisters[slEKey])[2];
                                    if (slEBitFlags == null)
                                    {
                                        slEBitFlags = new SortedList();
                                        ((object[])slPossibleMatchingRegisters[slEKey])[2] = slEBitFlags;
                                    }
                                    foreach (object[] arrBitFlags in ((SortedList)matchingReg[3]).Values)
                                    {
                                        SortedList slECBitFlags = (SortedList)slEBitFlags[arrBitFlags[0].ToString()];
                                        if (slECBitFlags == null)
                                        {
                                            slECBitFlags = new SortedList();
                                            slEBitFlags.Add(((int)arrBitFlags[0]).ToString(), slECBitFlags);
                                        }

                                        if (slECBitFlags.ContainsKey(slECKey + "." + ((int)arrBitFlags[1]).ToString()))
                                        {
                                            ((object[])slECBitFlags[slECKey + "." + ((int)arrBitFlags[1]).ToString()])[2] = (int)((object[])slECBitFlags[slECKey + "." + ((int)arrBitFlags[1]).ToString()])[2] + (int)arrBitFlags[2];
                                        }
                                        else
                                        {
                                            slECBitFlags.Add(slECKey + "." + ((int)arrBitFlags[1]).ToString(), new object[] { slECKey, arrBitFlags[1], arrBitFlags[2] });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                slEMatches = new SortedList();
                                slEMatches.Add(slECKey, new object[] { (string)matchingReg[1], (int)matchingReg[2] });

                                // BitFlags
                                if (matchingReg[3] != null)
                                {
                                    slEBitFlags = new SortedList();
                                    foreach (object[] arrBitFlags in ((SortedList)matchingReg[3]).Values)
                                    {
                                        SortedList slECBitFlags = new SortedList();
                                        slECBitFlags.Add(slECKey + "." + ((int)arrBitFlags[1]).ToString(), new object[] { slECKey, arrBitFlags[1], arrBitFlags[2] });
                                        if (!slEBitFlags.ContainsKey(((int)arrBitFlags[0]).ToString())) slEBitFlags.Add(((int)arrBitFlags[0]).ToString(), slECBitFlags);
                                    }
                                }

                                slPossibleMatchingRegisters.Add(slEKey, new object[] { sEKey, slEMatches, slEBitFlags });
                            }
                        }
                    }
                }
            }
        }
        
        private void compareOutput()
        {
            if (elemsTreeView == null) return;

            searchTreeViewInit();

            if (currentSkeleton == null)
            {
                searchTreeViewCount();
                return;
            }
            
            if (currentSkeleton.Count == 0)
            {
                searchTreeViewCount();
                return;
            }

            foreach (RoutineSkeleton rsSkt in currentSkeleton.Values)
            {
                if (rsSkt.alMatches == null) continue;
                if (rsSkt.alMatches.Count == 0) continue;
                if (filterUniqueToolStripMenuItem.Checked && rsSkt.alMatches.Count > 1) continue;

                TreeNode parentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES)];
                if (parentNode == null) break; ;

                if (parentNode.Nodes.ContainsKey(rsSkt.UniqueAddress)) continue;

                bool showNode = false;

                TreeNode tnNode = new TreeNode();
                tnNode.Name = rsSkt.UniqueAddress;
                tnNode.Text = rsSkt.UniqueAddressHex;
                S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[parentNode.Name]);
                TreeNode tnMFNode = null;
                if (niMFHeaderCateg.isValid)
                {
                    tnMFNode = niMFHeaderCateg.FindElement(rsSkt.UniqueAddress);
                    if (tnMFNode != null) tnNode.Text = tnMFNode.Text;
                }
                tnNode.ToolTipText = "Last Operation(" + Tools.UniqueAddressHex(rsSkt.BankNum, rsSkt.LastOperationAddressInt) + ")\nOperations Count(" + rsSkt.OpsNumber.ToString() + ")";
                tnNode.ContextMenuStrip = resultElemContextMenuStrip;

                // Elements Output Inside Routine
                if (!skeletonMode)
                {
                    if (rsSkt.alCalElements != null)
                    {
                        foreach (CalibrationElement calElem in rsSkt.alCalElements)
                        {
                            string rCategName = string.Empty;
                            TreeNode tnRCateg = null;
                            TreeNode tnRElem = null;

                            if (calElem.isStructure)
                            {
                                S6xStructure s6xStructure = (S6xStructure)sadBin.S6x.slStructures[calElem.UniqueAddress];
                                if (s6xStructure == null) continue;
                                if (s6xStructure.Skip) continue;

                                rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);
                                if (tnNode.Nodes.ContainsKey(rCategName))
                                {
                                    tnRCateg = tnNode.Nodes[rCategName];
                                }
                                else
                                {
                                    tnRCateg = new TreeNode();
                                    tnRCateg.Name = rCategName;
                                    tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                    tnNode.Nodes.Add(tnRCateg);
                                }

                                if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = calElem.UniqueAddress;
                                tnRElem.Text = s6xStructure.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                            if (calElem.isTable)
                            {
                                S6xTable s6xTable = (S6xTable)sadBin.S6x.slTables[calElem.UniqueAddress];
                                if (s6xTable == null) continue;
                                if (s6xTable.Skip) continue;

                                rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);
                                if (tnNode.Nodes.ContainsKey(rCategName))
                                {
                                    tnRCateg = tnNode.Nodes[rCategName];
                                }
                                else
                                {
                                    tnRCateg = new TreeNode();
                                    tnRCateg.Name = rCategName;
                                    tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                    tnNode.Nodes.Add(tnRCateg);
                                }

                                if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = calElem.UniqueAddress;
                                tnRElem.Text = s6xTable.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                            if (calElem.isFunction)
                            {
                                S6xFunction s6xFunction = (S6xFunction)sadBin.S6x.slFunctions[calElem.UniqueAddress];
                                if (s6xFunction == null) continue;
                                if (s6xFunction.Skip) continue;

                                rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);
                                if (tnNode.Nodes.ContainsKey(rCategName))
                                {
                                    tnRCateg = tnNode.Nodes[rCategName];
                                }
                                else
                                {
                                    tnRCateg = new TreeNode();
                                    tnRCateg.Name = rCategName;
                                    tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                    tnNode.Nodes.Add(tnRCateg);
                                }

                                if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = calElem.UniqueAddress;
                                tnRElem.Text = s6xFunction.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                            if (calElem.isScalar)
                            {
                                S6xScalar s6xScalar = (S6xScalar)sadBin.S6x.slScalars[calElem.UniqueAddress];
                                if (s6xScalar == null) continue;
                                if (s6xScalar.Skip) continue;

                                rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);
                                if (tnNode.Nodes.ContainsKey(rCategName))
                                {
                                    tnRCateg = tnNode.Nodes[rCategName];
                                }
                                else
                                {
                                    tnRCateg = new TreeNode();
                                    tnRCateg.Name = rCategName;
                                    tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                    tnNode.Nodes.Add(tnRCateg);
                                }

                                if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = calElem.UniqueAddress;
                                tnRElem.Text = s6xScalar.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                        }
                    }
                    if (rsSkt.alOtherElements != null)
                    {
                        // To be Done
                    }
                }
                
                // Matches Output
                foreach (object[] arrRes in rsSkt.alMatches)
                {
                    RoutineSkeleton rsMatchingSkt = (RoutineSkeleton)arrRes[0];

                    if (filterShortLabelToolStripMenuItem.Checked)
                    {
                        if (sadBin.S6x.slRoutines.ContainsKey(rsSkt.UniqueAddress))
                        {
                            if (((S6xRoutine)sadBin.S6x.slRoutines[rsSkt.UniqueAddress]).ShortLabel.ToUpper() == rsMatchingSkt.ShortLabel.ToUpper()) continue;
                        }
                    }
                    
                    S6xRoutine matchingRoutine = null;
                    if (!skeletonMode)
                    {
                        matchingRoutine = (S6xRoutine)cmpSadBin.S6x.slRoutines[rsMatchingSkt.UniqueAddress];
                        if (matchingRoutine == null) continue;
                        if (matchingRoutine.Skip) continue;
                        if (filterDefinedToolStripMenuItem.Checked && !matchingRoutine.Store) continue;
                    }

                    double dProximity = (double)arrRes[1];
                    TreeNode tnMatchingNode = new TreeNode();
                    tnMatchingNode.Name = rsSkt.UniqueAddress + "." + rsMatchingSkt.UniqueAddress;
                    tnMatchingNode.Text = rsMatchingSkt.UniqueAddressHex + " - " + rsMatchingSkt.FullLabel;
                    tnMatchingNode.ToolTipText = "% Chances : " + string.Format("{0:0.00}", dProximity * 100.0);
                    tnMatchingNode.ContextMenuStrip = resultElemContextMenuStrip;
                    tnMatchingNode.Tag = new object[] { rsMatchingSkt, matchingRoutine };
                    tnNode.Nodes.Add(tnMatchingNode);
                    showNode = true;

                    // Elements inside Match Output
                    if (!skeletonMode)
                    {
                        if (rsMatchingSkt.alCalElements != null)
                        {
                            foreach (CalibrationElement calElem in rsMatchingSkt.alCalElements)
                            {
                                string rCategName = string.Empty;
                                TreeNode tnRCateg = null;
                                TreeNode tnRElem = null;

                                if (calElem.isStructure)
                                {
                                    S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[calElem.UniqueAddress];
                                    if (s6xStructure == null) continue;
                                    if (s6xStructure.Skip) continue;

                                    rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);
                                    if (tnMatchingNode.Nodes.ContainsKey(rCategName))
                                    {
                                        tnRCateg = tnMatchingNode.Nodes[rCategName];
                                    }
                                    else
                                    {
                                        tnRCateg = new TreeNode();
                                        tnRCateg.Name = rCategName;
                                        tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                        tnMatchingNode.Nodes.Add(tnRCateg);
                                    }

                                    if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = calElem.UniqueAddress;
                                    tnRElem.Text = s6xStructure.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                                if (calElem.isTable)
                                {
                                    S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[calElem.UniqueAddress];
                                    if (s6xTable == null) continue;
                                    if (s6xTable.Skip) continue;

                                    rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);
                                    if (tnMatchingNode.Nodes.ContainsKey(rCategName))
                                    {
                                        tnRCateg = tnMatchingNode.Nodes[rCategName];
                                    }
                                    else
                                    {
                                        tnRCateg = new TreeNode();
                                        tnRCateg.Name = rCategName;
                                        tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                        tnMatchingNode.Nodes.Add(tnRCateg);
                                    }

                                    if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = calElem.UniqueAddress;
                                    tnRElem.Text = s6xTable.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                                if (calElem.isFunction)
                                {
                                    S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[calElem.UniqueAddress];
                                    if (s6xFunction == null) continue;
                                    if (s6xFunction.Skip) continue;

                                    rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);
                                    if (tnMatchingNode.Nodes.ContainsKey(rCategName))
                                    {
                                        tnRCateg = tnMatchingNode.Nodes[rCategName];
                                    }
                                    else
                                    {
                                        tnRCateg = new TreeNode();
                                        tnRCateg.Name = rCategName;
                                        tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                        tnMatchingNode.Nodes.Add(tnRCateg);
                                    }

                                    if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = calElem.UniqueAddress;
                                    tnRElem.Text = s6xFunction.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                                if (calElem.isScalar)
                                {
                                    S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[calElem.UniqueAddress];
                                    if (s6xScalar == null) continue;
                                    if (s6xScalar.Skip) continue;

                                    rCategName = S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);
                                    if (tnMatchingNode.Nodes.ContainsKey(rCategName))
                                    {
                                        tnRCateg = tnMatchingNode.Nodes[rCategName];
                                    }
                                    else
                                    {
                                        tnRCateg = new TreeNode();
                                        tnRCateg.Name = rCategName;
                                        tnRCateg.Text = S6xNav.getHeaderCategLabel(rCategName);
                                        tnMatchingNode.Nodes.Add(tnRCateg);
                                    }

                                    if (tnRCateg.Nodes.ContainsKey(calElem.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = calElem.UniqueAddress;
                                    tnRElem.Text = s6xScalar.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                            }
                        }
                        if (rsMatchingSkt.alOtherElements != null)
                        {
                            // To be Done
                        }
                    }
                }

                if (showNode) parentNode.Nodes.Add(tnNode);
            }

            if (!skeletonMode)
            {
                // Calibration Elements
                if (slPossibleMatchingCalElements != null)
                {
                    foreach (object[] EMatch in slPossibleMatchingCalElements.Values)
                    {
                        string EKey = (string)EMatch[0];
                        SortedList slEMatches = (SortedList)EMatch[1];

                        if (filterUniqueToolStripMenuItem.Checked && slEMatches.Count > 1) continue;

                        object s6xObject = null;
                        TreeNode tnCurNode = null;
                        string toolTipHeader = string.Empty;
                        string curShortLabel = string.Empty;
                        bool showNode = false;
                        S6xNavInfo niMFHeaderCateg = null;
                        S6xNavInfo niMFElement = null;

                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slStructures[EKey];
                                toolTipHeader = "Number : " + ((S6xStructure)s6xObject).Number;
                                curShortLabel = ((S6xStructure)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slTables[EKey];
                                toolTipHeader = "Colums : " + ((S6xTable)s6xObject).ColsNumber + "\r\n" + "Rows : " + ((S6xTable)s6xObject).RowsNumber + "\r\n";
                                toolTipHeader += ((S6xTable)s6xObject).SignedOutput ? "Signed" : "Unsigned" + " " + (((S6xTable)s6xObject).WordOutput ? "Word" : "Byte") + " Table";
                                curShortLabel = ((S6xTable)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slFunctions[EKey];
                                toolTipHeader = "Rows : " + ((S6xFunction)s6xObject).RowsNumber + "\r\n";
                                toolTipHeader += ((S6xFunction)s6xObject).SignedInput ? "Signed" : "Unsigned" + " " + (((S6xFunction)s6xObject).ByteInput ? "Byte" : "Word") + " Input\r\n";
                                toolTipHeader += ((S6xFunction)s6xObject).SignedOutput ? "Signed" : "Unsigned" + " " + (((S6xFunction)s6xObject).ByteOutput ? "Byte" : "Word") + " Output";
                                curShortLabel = ((S6xFunction)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slScalars[EKey];
                                toolTipHeader = ((S6xScalar)s6xObject).Signed ? "Signed" : "Unsigned" + " " + (((S6xScalar)s6xObject).Byte ? "Byte" : "Word") + " Scalar";
                                curShortLabel = ((S6xScalar)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null) continue;    // Bad Calibration Element

                        niMFElement = new S6xNavInfo(tnCurNode);
                        if (!niMFElement.isValid) continue; // Bad Calibration Element

                        TreeNode parentNode = compareTreeView.Nodes[niMFElement.HeaderCategoryName];
                        if (parentNode == null) break;
                        
                        if (parentNode.Nodes.ContainsKey(EKey)) continue;

                        TreeNode tnNewNode = new TreeNode();
                        tnNewNode.Name = tnCurNode.Name;
                        tnNewNode.Text = tnCurNode.Text;
                        tnNewNode.ToolTipText = tnCurNode.ToolTipText;
                        if (toolTipHeader != string.Empty) tnNewNode.ToolTipText = toolTipHeader + "\r\n" + tnNewNode.ToolTipText;
                        tnNewNode.ContextMenuStrip = resultElemContextMenuStrip;
                        tnNewNode.Tag = s6xObject;

                        foreach (object[] ECMatch in slEMatches.Values)
                        {
                            string ECKey = (string)ECMatch[0];
                            string ECOccurrences = ECMatch[1].ToString();

                            TreeNode tnMatchingNode = new TreeNode();
                            tnMatchingNode.Name = ECKey;
                            tnMatchingNode.Text = Tools.UniqueAddressHex(ECKey);
                            tnMatchingNode.ToolTipText = "Occurrences : " + ECOccurrences;
                            tnMatchingNode.ContextMenuStrip = resultElemContextMenuStrip;

                            S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[ECKey];
                            if (s6xStructure != null)
                            {
                                if (s6xStructure.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xStructure.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xStructure.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xStructure.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Number : " + s6xStructure.Number;
                                tnMatchingNode.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                tnMatchingNode.Tag = s6xStructure;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[ECKey];
                            if (s6xTable != null)
                            {
                                if (s6xTable.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xTable.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xTable.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xTable.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Colums : " + s6xTable.ColsNumber + "\r\n" + "Rows : " + s6xTable.RowsNumber + "\r\n";
                                tnMatchingNode.ToolTipText += s6xTable.SignedOutput ? "Signed" : "Unsigned" + " " + (s6xTable.WordOutput ? "Word" : "Byte") + " Table";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                                tnMatchingNode.Tag = s6xTable;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[ECKey];
                            if (s6xFunction != null)
                            {
                                if (s6xFunction.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xFunction.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xFunction.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xFunction.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Rows : " + s6xFunction.RowsNumber + "\r\n";
                                tnMatchingNode.ToolTipText += s6xFunction.SignedInput ? "Signed" : "Unsigned" + " " + (s6xFunction.ByteInput ? "Byte" : "Word") + " Input\r\n";
                                tnMatchingNode.ToolTipText += s6xFunction.SignedOutput ? "Signed" : "Unsigned" + " " + (s6xFunction.ByteOutput ? "Byte" : "Word") + " Output";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                                tnMatchingNode.Tag = s6xFunction;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[ECKey];
                            if (s6xScalar != null)
                            {
                                if (s6xScalar.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xScalar.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xScalar.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xScalar.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + (s6xScalar.Signed ? "Signed" : "Unsigned") + " " + (s6xScalar.Byte ? "Byte" : "Word") + " Scalar";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                tnMatchingNode.Tag = s6xScalar;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }

                        }

                        if (showNode) parentNode.Nodes.Add(tnNewNode);
                    }
                }

                // Other Elements
                if (slPossibleMatchingOtherElements != null)
                {
                    foreach (object[] EMatch in slPossibleMatchingOtherElements.Values)
                    {
                        string EKey = (string)EMatch[0];
                        SortedList slEMatches = (SortedList)EMatch[1];

                        if (filterUniqueToolStripMenuItem.Checked && slEMatches.Count > 1) continue;

                        object s6xObject = null;
                        TreeNode tnCurNode = null;
                        string toolTipHeader = string.Empty;
                        string curShortLabel = string.Empty;
                        bool showNode = false;
                        S6xNavInfo niMFHeaderCateg = null;
                        S6xNavInfo niMFElement = null;

                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slStructures[EKey];
                                toolTipHeader = "Number : " + ((S6xStructure)s6xObject).Number;
                                curShortLabel = ((S6xStructure)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slTables[EKey];
                                toolTipHeader = "Colums : " + ((S6xTable)s6xObject).ColsNumber + "\r\n" + "Rows : " + ((S6xTable)s6xObject).RowsNumber + "\r\n";
                                toolTipHeader += ((S6xTable)s6xObject).SignedOutput ? "Signed" : "Unsigned" + " " + (((S6xTable)s6xObject).WordOutput ? "Word" : "Byte") + " Table";
                                curShortLabel = ((S6xTable)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slFunctions[EKey];
                                toolTipHeader = "Rows : " + ((S6xFunction)s6xObject).RowsNumber + "\r\n";
                                toolTipHeader += ((S6xFunction)s6xObject).SignedInput ? "Signed" : "Unsigned" + " " + (((S6xFunction)s6xObject).ByteInput ? "Byte" : "Word") + " Input\r\n";
                                toolTipHeader += ((S6xFunction)s6xObject).SignedOutput ? "Signed" : "Unsigned" + " " + (((S6xFunction)s6xObject).ByteOutput ? "Byte" : "Word") + " Output";
                                curShortLabel = ((S6xFunction)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(EKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slScalars[EKey];
                                toolTipHeader = ((S6xScalar)s6xObject).Signed ? "Signed" : "Unsigned" + " " + (((S6xScalar)s6xObject).Byte ? "Byte" : "Word") + " Scalar";
                                curShortLabel = ((S6xScalar)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null) continue;   // Bad Calibration Element

                        niMFElement = new S6xNavInfo(tnCurNode);
                        if (!niMFElement.isValid) continue; // Bad Calibration Element

                        TreeNode parentNode = compareTreeView.Nodes[niMFElement.HeaderCategoryName];
                        if (parentNode == null) break;

                        if (parentNode.Nodes.ContainsKey(EKey)) continue;

                        TreeNode tnNewNode = new TreeNode();
                        tnNewNode.Name = tnCurNode.Name;
                        tnNewNode.Text = tnCurNode.Text;
                        tnNewNode.ToolTipText = tnCurNode.ToolTipText;
                        if (toolTipHeader != string.Empty) tnNewNode.ToolTipText = toolTipHeader + "\r\n" + tnNewNode.ToolTipText;
                        tnNewNode.ContextMenuStrip = resultElemContextMenuStrip;
                        tnNewNode.Tag = s6xObject;

                        foreach (object[] ECMatch in slEMatches.Values)
                        {
                            string ECKey = (string)ECMatch[0];
                            string ECOccurrences = ECMatch[1].ToString();

                            TreeNode tnMatchingNode = new TreeNode();
                            tnMatchingNode.Name = ECKey;
                            tnMatchingNode.Text = Tools.UniqueAddressHex(ECKey);
                            tnMatchingNode.ToolTipText = "Occurrences : " + ECOccurrences;
                            tnMatchingNode.ContextMenuStrip = resultElemContextMenuStrip;

                            S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[ECKey];
                            if (s6xStructure != null)
                            {
                                if (s6xStructure.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xStructure.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xStructure.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xStructure.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Number : " + s6xStructure.Number;
                                tnMatchingNode.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                tnMatchingNode.Tag = s6xStructure;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[ECKey];
                            if (s6xTable != null)
                            {
                                if (s6xTable.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xTable.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xTable.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xTable.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Colums : " + s6xTable.ColsNumber + "\r\n" + "Rows : " + s6xTable.RowsNumber + "\r\n";
                                tnMatchingNode.ToolTipText += s6xTable.SignedOutput ? "Signed" : "Unsigned" + " " + (s6xTable.WordOutput ? "Word" : "Byte") + " Table";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                                tnMatchingNode.Tag = s6xTable;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[ECKey];
                            if (s6xFunction != null)
                            {
                                if (s6xFunction.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xFunction.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xFunction.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xFunction.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Rows : " + s6xFunction.RowsNumber + "\r\n";
                                tnMatchingNode.ToolTipText += s6xFunction.SignedInput ? "Signed" : "Unsigned" + " " + (s6xFunction.ByteInput ? "Byte" : "Word") + " Input\r\n";
                                tnMatchingNode.ToolTipText += s6xFunction.SignedOutput ? "Signed" : "Unsigned" + " " + (s6xFunction.ByteOutput ? "Byte" : "Word") + " Output";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                                tnMatchingNode.Tag = s6xFunction;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[ECKey];
                            if (s6xScalar != null)
                            {
                                if (s6xScalar.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xScalar.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xScalar.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xScalar.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + (s6xScalar.Signed ? "Signed" : "Unsigned") + " " + (s6xScalar.Byte ? "Byte" : "Word") + " Scalar";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                tnMatchingNode.Tag = s6xScalar;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }

                        }

                        if (showNode) parentNode.Nodes.Add(tnNewNode);
                    }
                }

                // Registers
                if (slPossibleMatchingRegisters != null)
                {
                    foreach (object[] EMatch in slPossibleMatchingRegisters.Values)
                    {
                        string EKey = (string)EMatch[0];
                        SortedList slEMatches = (SortedList)EMatch[1];
                        SortedList slEBitFlags = (SortedList)EMatch[2];

                        if (filterUniqueToolStripMenuItem.Checked && slEMatches.Count > 1) continue;

                        TreeNode parentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS)];
                        if (parentNode == null) break; ;

                        if (parentNode.Nodes.ContainsKey(Tools.RegisterUniqueAddress(EKey))) continue;

                        TreeNode tnNewNode = new TreeNode();
                        tnNewNode.Name = Tools.RegisterUniqueAddress(EKey);
                        S6xRegister s6xReg = (S6xRegister)sadBin.S6x.slRegisters[tnNewNode.Name];

                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[parentNode.Name]);
                        if (!niMFHeaderCateg.isValid) continue;
                        TreeNode tnMFNode = niMFHeaderCateg.FindElement(tnNewNode.Name);
                        if (tnMFNode == null)
                        {
                            tnNewNode.Text = Tools.RegisterInstruction(EKey);
                        }
                        else
                        {
                            tnNewNode.Text = tnMFNode.Text;
                            if (s6xReg != null)
                            {
                                tnNewNode.Text = s6xReg.FullLabel;
                                if (s6xReg.MultipleMeanings)
                                {
                                    tnNewNode.ToolTipText = "Byte : " + s6xReg.ByteLabel + "\r\n";
                                    tnNewNode.ToolTipText += "Word : " + s6xReg.WordLabel + "\r\n";
                                }
                                tnNewNode.ToolTipText += s6xReg.FullComments;
                            }
                        }

                        if (slEBitFlags == null)
                        {
                            // No Bit Flags, classical Matching will be used, Context Menu is available
                            tnNewNode.ContextMenuStrip = resultElemContextMenuStrip;
                            
                            foreach (object[] ECMatch in slEMatches.Values)
                            {
                                string ECKey = (string)ECMatch[0];
                                string ECOccurrences = ECMatch[1].ToString();

                                S6xRegister s6xCmpReg = (S6xRegister)cmpSadBin.S6x.slRegisters[Tools.RegisterUniqueAddress(ECKey)];
                                if (s6xCmpReg == null) continue;
                                if (s6xCmpReg.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xCmpReg.Store) continue;

                                if (filterShortLabelToolStripMenuItem.Checked && s6xReg != null)
                                {
                                    if (s6xReg.Label.ToLower() == s6xCmpReg.Label.ToLower()) continue;
                                }

                                TreeNode tnMatchingNode = new TreeNode();
                                tnMatchingNode.Name = Tools.RegisterUniqueAddress(ECKey);
                                tnMatchingNode.Text = s6xCmpReg.FullLabel;
                                tnMatchingNode.ToolTipText = "Occurrences : " + ECOccurrences;
                                if (s6xCmpReg.MultipleMeanings)
                                {
                                    tnMatchingNode.ToolTipText = "\r\n" + "Byte : " + s6xCmpReg.ByteLabel + "\r\n";
                                    tnMatchingNode.ToolTipText += "Word : " + s6xCmpReg.WordLabel + "\r\n";
                                }
                                tnMatchingNode.ToolTipText += "\r\n" + s6xCmpReg.FullComments;
                                tnMatchingNode.Tag = s6xCmpReg;
                                tnMatchingNode.ContextMenuStrip = resultElemContextMenuStrip;
                                tnNewNode.Nodes.Add(tnMatchingNode);
                            }
                        }
                        else
                        {
                            // Bit Flags, specific Matching will be used
                            foreach (string sBFKey in slEBitFlags.Keys)
                            {
                                TreeNode tnBFNode = new TreeNode();
                                tnBFNode.Name = sBFKey;
                                tnBFNode.Text = "B" + sBFKey;
                                tnBFNode.ToolTipText = tnBFNode.Text;
                                if (s6xReg != null)
                                {
                                    if (s6xReg.isBitFlags)
                                    {
                                        foreach (S6xBitFlag s6xBf in s6xReg.BitFlags)
                                        {
                                            if (s6xBf.Position == Convert.ToInt32(sBFKey))
                                            {
                                                tnBFNode.Text = string.Format("B{0,-5}{1}", sBFKey, s6xBf.ShortLabel);
                                                tnBFNode.ToolTipText = tnBFNode.Text + "\r\n" + s6xBf.Label + "\r\n" + s6xBf.Comments;
                                                break;
                                            }
                                        }
                                    }
                                }

                                foreach (object[] ECBitFlag in ((SortedList)slEBitFlags[sBFKey]).Values)
                                {
                                    string slECKey = (string)ECBitFlag[0];
                                    int iECBf = (int)ECBitFlag[1];
                                    string ECOccurrences = ECBitFlag[2].ToString();

                                    S6xRegister s6xCmpReg = (S6xRegister)cmpSadBin.S6x.slRegisters[slECKey];
                                    if (s6xCmpReg == null) continue;
                                    if (s6xCmpReg.Skip) continue;
                                    if (filterDefinedToolStripMenuItem.Checked && !s6xCmpReg.Store) continue;

                                    if (filterShortLabelToolStripMenuItem.Checked && s6xReg != null)
                                    {
                                        if (s6xReg.Label.ToLower() == s6xCmpReg.Label.ToLower()) continue;
                                    }

                                    TreeNode tnMatchingNode = new TreeNode();
                                    tnMatchingNode.Name = slECKey;
                                    tnMatchingNode.Text = "B" + iECBf.ToString() + "." + s6xCmpReg.FullLabel;
                                    tnMatchingNode.ToolTipText = "Occurrences : " + ECOccurrences;
                                    tnMatchingNode.ToolTipText += "\r\n" + s6xCmpReg.FullComments;
                                    if (s6xCmpReg.isBitFlags)
                                    {
                                        foreach (S6xBitFlag s6xCmpBf in s6xCmpReg.BitFlags)
                                        {
                                            if (s6xCmpBf.Position == iECBf)
                                            {
                                                tnMatchingNode.Text = "B" + iECBf.ToString() + "." + s6xCmpReg.FullLabel + "." + s6xCmpBf.ShortLabel;
                                                tnMatchingNode.ToolTipText = "Occurrences : " + ECOccurrences;
                                                tnMatchingNode.ToolTipText += "\r\n" + tnMatchingNode.Text + "\r\n" + s6xCmpBf.Label + "\r\n" + s6xCmpBf.Comments;
                                                break;
                                            }
                                        }
                                    }
                                    tnBFNode.Nodes.Add(tnMatchingNode);
                                }

                                if (tnBFNode.Nodes.Count > 0) tnNewNode.Nodes.Add(tnBFNode);
                            }
                        }

                        if (tnNewNode.Nodes.Count > 0)
                        {
                            if (filterUniqueToolStripMenuItem.Checked && tnNewNode.Nodes.Count > 1) continue;
                            parentNode.Nodes.Add(tnNewNode);
                        }
                    }
                }
            }
            
            searchTreeViewCount();
        }

        private void exportReportButton_Click(object sender, EventArgs e)
        {
            saveFileDialogSkr.FileName = lastExecutionOptions[0].ToString() + ".skr";
            if (saveFileDialogSkr.ShowDialog() != DialogResult.OK) return;

            SortedList slRegistersReporting = null;
            SortedList slStructuresReporting = null;
            SortedList slTablesReporting = null;
            SortedList slFunctionsReporting = null;
            SortedList slScalarsReporting = null;

            if (!skeletonMode)
            {
                slRegistersReporting = new SortedList();
                slStructuresReporting = new SortedList();
                slTablesReporting = new SortedList();
                slFunctionsReporting = new SortedList();
                slScalarsReporting = new SortedList();

                // Calibration Elements
                foreach (object[] EMatch in slPossibleMatchingCalElements.Values)
                {
                    string EKey = (string)EMatch[0];
                    SortedList slEMatches = (SortedList)EMatch[1];

                    SortedList curSL = null;
                    string curType = string.Empty;
                    string curUniqueAddressHex = string.Empty;
                    string curFullLabel = string.Empty;
                    SortedList slMatches = new SortedList();

                    S6xNavInfo niMFHeaderCateg = null;
                    TreeNode tnCurNode = null;

                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slStructuresReporting;
                            curType = "STRUCTURE";
                            curUniqueAddressHex = ((S6xStructure)sadBin.S6x.slStructures[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xStructure)sadBin.S6x.slStructures[EKey]).ShortLabel + " - " + ((S6xStructure)sadBin.S6x.slStructures[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slTablesReporting;
                            curType = "TABLE";
                            curUniqueAddressHex = ((S6xTable)sadBin.S6x.slTables[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xTable)sadBin.S6x.slTables[EKey]).ShortLabel + " - " + ((S6xTable)sadBin.S6x.slTables[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slFunctionsReporting;
                            curType = "FUNCTION";
                            curUniqueAddressHex = ((S6xFunction)sadBin.S6x.slFunctions[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xFunction)sadBin.S6x.slFunctions[EKey]).ShortLabel + " - " + ((S6xFunction)sadBin.S6x.slFunctions[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slScalarsReporting;
                            curType = "SCALAR";
                            curUniqueAddressHex = ((S6xScalar)sadBin.S6x.slScalars[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xScalar)sadBin.S6x.slScalars[EKey]).ShortLabel + " - " + ((S6xScalar)sadBin.S6x.slScalars[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null) continue;   // Bad Calibration Element

                    foreach (object[] ECMatch in slEMatches.Values)
                    {
                        string ECKey = (string)ECMatch[0];
                        string ECOccurrences = ECMatch[1].ToString();

                        string cmpType = string.Empty;
                        string cmpUniqueAddressHex = string.Empty;
                        string cmpFullLabel = string.Empty;

                        S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[ECKey];
                        if (s6xStructure != null)
                        {
                            if (s6xStructure.Skip) continue;

                            cmpType = "STRUCTURE";
                            cmpUniqueAddressHex = ((S6xStructure)cmpSadBin.S6x.slStructures[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xStructure)cmpSadBin.S6x.slStructures[ECKey]).ShortLabel + " - " + ((S6xStructure)cmpSadBin.S6x.slStructures[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] {cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences});
                            continue;
                        }
                        S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[ECKey];
                        if (s6xTable != null)
                        {
                            if (s6xTable.Skip) continue;

                            cmpType = "TABLE";
                            cmpUniqueAddressHex = ((S6xTable)cmpSadBin.S6x.slTables[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xTable)cmpSadBin.S6x.slTables[ECKey]).ShortLabel + " - " + ((S6xTable)cmpSadBin.S6x.slTables[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                        S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[ECKey];
                        if (s6xFunction != null)
                        {
                            if (s6xFunction.Skip) continue;

                            cmpType = "FUNCTION";
                            cmpUniqueAddressHex = ((S6xFunction)cmpSadBin.S6x.slFunctions[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xFunction)cmpSadBin.S6x.slFunctions[ECKey]).ShortLabel + " - " + ((S6xFunction)cmpSadBin.S6x.slFunctions[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                        S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[ECKey];
                        if (s6xScalar != null)
                        {
                            if (s6xScalar.Skip) continue;

                            cmpType = "SCALAR";
                            cmpUniqueAddressHex = ((S6xScalar)cmpSadBin.S6x.slScalars[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xScalar)cmpSadBin.S6x.slScalars[ECKey]).ShortLabel + " - " + ((S6xScalar)cmpSadBin.S6x.slScalars[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                    }

                    if (slMatches.Count == 0) continue;

                    curSL.Add(EKey, new object[] {curType, curUniqueAddressHex, curFullLabel, slMatches });
                }

                // Other Elements
                foreach (object[] EMatch in slPossibleMatchingOtherElements.Values)
                {
                    string EKey = (string)EMatch[0];
                    SortedList slEMatches = (SortedList)EMatch[1];

                    SortedList curSL = null;
                    string curType = string.Empty;
                    string curUniqueAddressHex = string.Empty;
                    string curFullLabel = string.Empty;
                    SortedList slMatches = new SortedList();

                    S6xNavInfo niMFHeaderCateg = null;
                    TreeNode tnCurNode = null;

                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slStructuresReporting;
                            curType = "STRUCTURE";
                            curUniqueAddressHex = ((S6xStructure)sadBin.S6x.slStructures[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xStructure)sadBin.S6x.slStructures[EKey]).ShortLabel + " - " + ((S6xStructure)sadBin.S6x.slStructures[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slTablesReporting;
                            curType = "TABLE";
                            curUniqueAddressHex = ((S6xTable)sadBin.S6x.slTables[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xTable)sadBin.S6x.slTables[EKey]).ShortLabel + " - " + ((S6xTable)sadBin.S6x.slTables[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slFunctionsReporting;
                            curType = "FUNCTION";
                            curUniqueAddressHex = ((S6xFunction)sadBin.S6x.slFunctions[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xFunction)sadBin.S6x.slFunctions[EKey]).ShortLabel + " - " + ((S6xFunction)sadBin.S6x.slFunctions[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(EKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slScalarsReporting;
                            curType = "SCALAR";
                            curUniqueAddressHex = ((S6xScalar)sadBin.S6x.slScalars[EKey]).UniqueAddressHex;
                            curFullLabel = ((S6xScalar)sadBin.S6x.slScalars[EKey]).ShortLabel + " - " + ((S6xScalar)sadBin.S6x.slScalars[EKey]).Label;
                        }
                    }
                    if (tnCurNode == null) continue;   // Bad Calibration Element

                    foreach (object[] ECMatch in slEMatches.Values)
                    {
                        string ECKey = (string)ECMatch[0];
                        string ECOccurrences = ECMatch[1].ToString();

                        string cmpType = string.Empty;
                        string cmpUniqueAddressHex = string.Empty;
                        string cmpFullLabel = string.Empty;

                        S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[ECKey];
                        if (s6xStructure != null)
                        {
                            if (s6xStructure.Skip) continue;

                            cmpType = "STRUCTURE";
                            cmpUniqueAddressHex = ((S6xStructure)cmpSadBin.S6x.slStructures[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xStructure)cmpSadBin.S6x.slStructures[ECKey]).ShortLabel + " - " + ((S6xStructure)cmpSadBin.S6x.slStructures[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                        S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[ECKey];
                        if (s6xTable != null)
                        {
                            if (s6xTable.Skip) continue;

                            cmpType = "TABLE";
                            cmpUniqueAddressHex = ((S6xTable)cmpSadBin.S6x.slTables[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xTable)cmpSadBin.S6x.slTables[ECKey]).ShortLabel + " - " + ((S6xTable)cmpSadBin.S6x.slTables[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                        S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[ECKey];
                        if (s6xFunction != null)
                        {
                            if (s6xFunction.Skip) continue;

                            cmpType = "FUNCTION";
                            cmpUniqueAddressHex = ((S6xFunction)cmpSadBin.S6x.slFunctions[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xFunction)cmpSadBin.S6x.slFunctions[ECKey]).ShortLabel + " - " + ((S6xFunction)cmpSadBin.S6x.slFunctions[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                        S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[ECKey];
                        if (s6xScalar != null)
                        {
                            if (s6xScalar.Skip) continue;

                            cmpType = "SCALAR";
                            cmpUniqueAddressHex = ((S6xScalar)cmpSadBin.S6x.slScalars[ECKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xScalar)cmpSadBin.S6x.slScalars[ECKey]).ShortLabel + " - " + ((S6xScalar)cmpSadBin.S6x.slScalars[ECKey]).Label;

                            slMatches.Add(ECKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, ECOccurrences });
                            continue;
                        }
                    }

                    if (slMatches.Count == 0) continue;

                    curSL.Add(EKey, new object[] { curType, curUniqueAddressHex, curFullLabel, slMatches });
                }

                // Registers
                foreach (object[] EMatch in slPossibleMatchingRegisters.Values)
                {
                    string EKey = (string)EMatch[0];
                    SortedList slEMatches = (SortedList)EMatch[1];

                    string curFullLabel = string.Empty;
                    SortedList slMatches = new SortedList();

                    if (!sadBin.S6x.slRegisters.ContainsKey(Tools.RegisterUniqueAddress(EKey))) curFullLabel = Tools.RegisterInstruction(EKey);
                    else curFullLabel = ((S6xRegister)sadBin.S6x.slRegisters[Tools.RegisterUniqueAddress(EKey)]).FullLabel;

                    foreach (object[] ECMatch in slEMatches.Values)
                    {
                        string ECKey = (string)ECMatch[0];
                        string ECOccurrences = ECMatch[1].ToString();

                        string cmpFullLabel = string.Empty;

                        S6xRegister s6xCmpReg = (S6xRegister)cmpSadBin.S6x.slRegisters[Tools.RegisterUniqueAddress(ECKey)];
                        if (s6xCmpReg == null) continue; // No Interest for reporting
                        if (s6xCmpReg.Skip || !s6xCmpReg.Store) continue;

                        cmpFullLabel = s6xCmpReg.FullLabel;

                        slMatches.Add(ECKey, new string[] { cmpFullLabel, ECOccurrences });
                    }

                    if (slMatches.Count == 0) continue;

                    slRegistersReporting.Add(EKey, new object[] { curFullLabel, slMatches });
                }
            }

            ToolsRoutinesComp.exportReportRoutinesComparisonSkeletons(saveFileDialogSkr.FileName, ref currentSkeleton, lastExecutionOptions[0].ToString(), ref fileSkeleton, lastExecutionOptions[1].ToString(), (int)lastExecutionOptions[2], (double)lastExecutionOptions[3], (double)lastExecutionOptions[4], ref slRegistersReporting, ref slStructuresReporting, ref slTablesReporting, ref slFunctionsReporting, ref slScalarsReporting);
        }

        private void searchTreeViewInit()
        {
            compareTreeView.Nodes.Clear();

            if (elemsTreeView == null) return;

            foreach (TreeNode tnMainParent in elemsTreeView.Nodes)
            {
                string categLabel = string.Empty;

                switch (S6xNav.getHeaderCateg(tnMainParent.Name))
                {
                    case S6xNavHeaderCategory.PROPERTIES:
                    case S6xNavHeaderCategory.RESERVED:
                    case S6xNavHeaderCategory.OPERATIONS:
                    case S6xNavHeaderCategory.OTHER:
                    case S6xNavHeaderCategory.SIGNATURES:
                    case S6xNavHeaderCategory.ELEMSSIGNATURES:
                        break;
                    case S6xNavHeaderCategory.ROUTINES:
                        categLabel = S6xNav.getHeaderCategLabel(S6xNav.getHeaderCateg(tnMainParent.Name));
                        break;
                    case S6xNavHeaderCategory.REGISTERS:
                    case S6xNavHeaderCategory.TABLES:
                    case S6xNavHeaderCategory.FUNCTIONS:
                    case S6xNavHeaderCategory.SCALARS:
                    case S6xNavHeaderCategory.STRUCTURES:
                        if (!skeletonMode) categLabel = S6xNav.getHeaderCategLabel(S6xNav.getHeaderCateg(tnMainParent.Name));
                        break;
                }

                if (categLabel == string.Empty) continue;

                TreeNode tnParent = new TreeNode();
                tnParent.Name = tnMainParent.Name;
                tnParent.Text = categLabel;
                tnParent.ToolTipText = tnMainParent.ToolTipText;
                tnParent.ContextMenuStrip = resultCategContextMenuStrip;
                compareTreeView.Nodes.Add(tnParent);
            }
        }

        private void searchTreeViewCount()
        {
            foreach (TreeNode tnParent in compareTreeView.Nodes)
            {
                string categLabel = S6xNav.getHeaderCategLabel(tnParent.Name);
                if (categLabel == string.Empty) return;
                tnParent.Text = categLabel + " (" + tnParent.Nodes.Count.ToString() + ")";
            }
        }

        private void importElementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode categNode = compareTreeView.SelectedNode;
            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[categNode.Name]);
            if (!niMFHeaderCateg.isValid) return;

            ArrayList alNewNodes = new ArrayList();

            foreach (TreeNode topNode in categNode.Nodes)
            {
                if (topNode.Nodes == null) continue;

                TreeNode cmpNode = null;
                TreeNode tnNode = null;
                int iUnicityCount = 0;
                // Unicity Mngt 
                if (niMFHeaderCateg.HeaderCategory == S6xNavHeaderCategory.ROUTINES)
                {
                    // Routine Contains details on its Elements
                    foreach (TreeNode subNode in topNode.Nodes)
                    {
                        if (subNode.Tag != null) // Not an Element
                        {
                            iUnicityCount++;
                            if (iUnicityCount > 1) break;
                            cmpNode = subNode;
                        }
                    }
                    if (iUnicityCount != 1) continue; // Only when 100% sure
                }
                else
                {
                    if (topNode.Nodes.Count != 1) continue; // Only when 100% sure
                    cmpNode = topNode.Nodes[0];
                }

                bool elemsTreeViewUpdate = false;

                switch (niMFHeaderCateg.HeaderCategory)
                {
                    case S6xNavHeaderCategory.ROUTINES:
                        if (cmpNode.Tag == null) continue;
                        RoutineSkeleton cmpRsSkt = ((RoutineSkeleton)((object[])cmpNode.Tag)[0]);
                        S6xRoutine cmpRoutine = ((S6xRoutine)((object[])cmpNode.Tag)[1]);
                        
                        if (!skeletonMode)
                        {
                            if (cmpRoutine == null) continue;
                            if (cmpRoutine.Skip) continue;
                            if (!cmpRoutine.Store) continue;
                        }

                        RoutineSkeleton curRsSkt = (RoutineSkeleton)currentSkeleton[topNode.Name];
                        S6xRoutine s6xRoutine = (S6xRoutine)sadBin.S6x.slRoutines[topNode.Name];
                        if (s6xRoutine == null)
                        {
                            if (curRsSkt == null) continue;
                            s6xRoutine = new S6xRoutine();
                            s6xRoutine.BankNum = curRsSkt.BankNum;
                            s6xRoutine.AddressInt = curRsSkt.AddressInt;
                        }
                        s6xRoutine.ShortLabel = cmpRsSkt.ShortLabel;
                        s6xRoutine.Label = cmpRsSkt.Label;
                        s6xRoutine.Comments = cmpRsSkt.Comments;
                        s6xRoutine.Store = true;

                        if (sadBin.S6x.slRoutines.ContainsKey(topNode.Name)) sadBin.S6x.slRoutines[topNode.Name] = s6xRoutine;
                        else sadBin.S6x.slRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xRoutine.Label;
                        topNode.ToolTipText = s6xRoutine.UniqueAddressHex + "\r\n" + s6xRoutine.ShortLabel + "\r\n\r\n" + s6xRoutine.Comments;
                        elemsTreeViewUpdate = true;
                        break;
                    case S6xNavHeaderCategory.REGISTERS:
                        if (cmpNode.Tag == null) continue;
                        if (((S6xRegister)cmpNode.Tag).Skip) continue;
                        if (!((S6xRegister)cmpNode.Tag).Store) continue;
                        // AutoConstValue Registers (RBase, RConst) are ignored
                        if (((S6xRegister)cmpNode.Tag).AutoConstValue) continue;
                        // Bit Flags Should be managed one by one
                        if (((S6xRegister)cmpNode.Tag).isBitFlags) continue;
                        // Double Labels are ignored on glbal Import
                        if (((S6xRegister)cmpNode.Tag).ByteLabel != null && ((S6xRegister)cmpNode.Tag).WordLabel != null)
                        {
                            if (((S6xRegister)cmpNode.Tag).ByteLabel != ((S6xRegister)cmpNode.Tag).WordLabel) continue;
                        }
                        S6xRegister s6xReg = ((S6xRegister)cmpNode.Tag).Clone();
                        if (topNode.Name.Contains(SADDef.AdditionSeparator))
                        {
                            s6xReg.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim().Split(SADDef.AdditionSeparator[0])[0]);
                            s6xReg.AdditionalAddress10 = topNode.Name.Substring(1).Trim().Split(SADDef.AdditionSeparator[0])[1];
                        }
                        else
                        {
                            s6xReg.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        }
                        s6xReg.Store = true;
                        if (sadBin.S6x.slRegisters.ContainsKey(topNode.Name))
                        {
                            // AutoConstValue Registers (RBase, RConst) are protected
                            if (((S6xRegister)sadBin.S6x.slRegisters[topNode.Name]).AutoConstValue) continue;
                            sadBin.S6x.slRegisters[topNode.Name] = s6xReg;
                        }
                        else
                        {
                            sadBin.S6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xReg.FullLabel;
                        topNode.ToolTipText = s6xReg.FullComments;
                        elemsTreeViewUpdate = true;
                        break;
                    case S6xNavHeaderCategory.STRUCTURES:
                        if (topNode.Tag == null) continue;
                        if (cmpNode.Tag == null) continue;
                        if (typeof(S6xStructure) != topNode.Tag.GetType()) continue;
                        if (typeof(S6xStructure) != cmpNode.Tag.GetType()) continue;
                        if (((S6xStructure)cmpNode.Tag).Skip) continue;
                        if (!((S6xStructure)cmpNode.Tag).Store) continue;

                        S6xStructure s6xStruct = ((S6xStructure)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xStruct.Number != ((S6xStructure)topNode.Tag).Number) continue;

                        s6xStruct.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xStruct.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xStruct.Store = true;
                        if (sadBin.S6x.slStructures.ContainsKey(topNode.Name)) sadBin.S6x.slStructures[topNode.Name] = s6xStruct;
                        else sadBin.S6x.slStructures.Add(topNode.Name, s6xStruct);
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xStruct.Label;
                        topNode.ToolTipText = s6xStruct.UniqueAddressHex + "\r\n" + s6xStruct.ShortLabel + "\r\n\r\n" + s6xStruct.Comments;
                        elemsTreeViewUpdate = true;
                        break;
                    case S6xNavHeaderCategory.TABLES:
                        if (topNode.Tag == null) continue;
                        if (cmpNode.Tag == null) continue;
                        if (typeof(S6xTable) != topNode.Tag.GetType()) continue;
                        if (typeof(S6xTable) != cmpNode.Tag.GetType()) continue;
                        if (((S6xTable)cmpNode.Tag).Skip) continue;
                        if (!((S6xTable)cmpNode.Tag).Store) continue;

                        S6xTable s6xTable = ((S6xTable)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xTable.ColsNumber != ((S6xTable)topNode.Tag).ColsNumber) continue;
                        if (s6xTable.RowsNumber != ((S6xTable)topNode.Tag).RowsNumber) continue;
                        if (s6xTable.WordOutput != ((S6xTable)topNode.Tag).WordOutput) continue;

                        s6xTable.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xTable.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xTable.Store = true;
                        if (sadBin.S6x.slTables.ContainsKey(topNode.Name))
                        {
                            S6xTable s6xOldTable = (S6xTable)sadBin.S6x.slTables[topNode.Name];
                            s6xTable.ColsScalerAddress = s6xOldTable.ColsScalerAddress;
                            s6xTable.RowsScalerAddress = s6xOldTable.RowsScalerAddress;
                            s6xTable.XdfUniqueId = s6xOldTable.XdfUniqueId;
                            s6xTable.ColsScalerXdfUniqueId = s6xOldTable.ColsScalerXdfUniqueId;
                            s6xTable.RowsScalerXdfUniqueId = s6xOldTable.RowsScalerXdfUniqueId;
                            sadBin.S6x.slTables[topNode.Name] = s6xTable;
                        }
                        else
                        {
                            s6xTable.ColsScalerAddress = string.Empty;
                            s6xTable.RowsScalerAddress = string.Empty;
                            sadBin.S6x.slTables.Add(topNode.Name, s6xTable);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xTable.Label;
                        topNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                        elemsTreeViewUpdate = true;
                        break;
                    case S6xNavHeaderCategory.FUNCTIONS:
                        if (topNode.Tag == null) continue;
                        if (cmpNode.Tag == null) continue;
                        if (typeof(S6xFunction) != topNode.Tag.GetType()) continue;
                        if (typeof(S6xFunction) != cmpNode.Tag.GetType()) continue;
                        if (((S6xFunction)cmpNode.Tag).Skip) continue;
                        if (!((S6xFunction)cmpNode.Tag).Store) continue;

                        S6xFunction s6xFunction = ((S6xFunction)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xFunction.RowsNumber != ((S6xFunction)topNode.Tag).RowsNumber) continue;
                        if (s6xFunction.ByteInput != ((S6xFunction)topNode.Tag).ByteInput) continue;
                        if (s6xFunction.ByteOutput != ((S6xFunction)topNode.Tag).ByteOutput) continue;
                        
                        s6xFunction.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xFunction.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xFunction.Store = true;
                        if (sadBin.S6x.slFunctions.ContainsKey(topNode.Name))
                        {
                            s6xFunction.XdfUniqueId = ((S6xFunction)sadBin.S6x.slFunctions[topNode.Name]).XdfUniqueId;
                            sadBin.S6x.slFunctions[topNode.Name] = s6xFunction;
                        }
                        else
                        {
                            sadBin.S6x.slFunctions.Add(topNode.Name, s6xFunction);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xFunction.Label;
                        topNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                        elemsTreeViewUpdate = true;
                        break;
                    case S6xNavHeaderCategory.SCALARS:
                        if (topNode.Tag == null) continue;
                        if (cmpNode.Tag == null) continue;
                        if (typeof(S6xScalar) != topNode.Tag.GetType()) continue;
                        if (typeof(S6xScalar) != cmpNode.Tag.GetType()) continue;
                        if (((S6xScalar)cmpNode.Tag).Skip) continue;
                        if (!((S6xScalar)cmpNode.Tag).Store) continue;

                        S6xScalar s6xScalar = ((S6xScalar)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xScalar.Byte != ((S6xScalar)topNode.Tag).Byte) continue;

                        s6xScalar.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xScalar.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xScalar.Store = true;
                        if (sadBin.S6x.slScalars.ContainsKey(topNode.Name))
                        {
                            s6xScalar.XdfUniqueId = ((S6xScalar)sadBin.S6x.slScalars[topNode.Name]).XdfUniqueId;
                            sadBin.S6x.slScalars[topNode.Name] = s6xScalar;
                        }
                        else
                        {
                            sadBin.S6x.slScalars.Add(topNode.Name, s6xScalar);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xScalar.Label;
                        topNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                        elemsTreeViewUpdate = true;
                        break;
                }

                if (elemsTreeViewUpdate)
                {
                    S6xNavInfo niMFTopHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[topNode.Parent.Name]);
                    if (!niMFTopHeaderCateg.isValid) continue;

                    tnNode = niMFTopHeaderCateg.FindElement(topNode.Name);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = topNode.Name;
                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        alNewNodes.Add(tnNode);
                    }
                    tnNode.Text = topNode.Text;
                    tnNode.ToolTipText = topNode.ToolTipText;
                    tnNode.ForeColor = Color.Purple;
                }
            }

            if (alNewNodes.Count == 0) return;

            foreach (TreeNode addedNode in alNewNodes)
            {
                niMFHeaderCateg.AddNode(addedNode, null, null, null, false);
            }
            elemsTreeView.Nodes[categNode.Name].Text = S6xNav.getHeaderCategLabel(niMFHeaderCateg.HeaderCategory) + " (" + niMFHeaderCateg.ElementsCount.ToString() + ")";
        }

        private void importElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnNode = compareTreeView.SelectedNode;

            if (tnNode.Parent == null) return;

            bool subCategMode = (tnNode.Parent.Parent == null);
            int iUnicityCount = 0;

            TreeNode topNode = null;
            TreeNode cmpNode = null;
            if (subCategMode)
            {
                topNode = tnNode;

                // Unicity Mngt 
                if (S6xNav.getHeaderCateg(topNode.Parent.Name) == S6xNavHeaderCategory.ROUTINES)
                {
                    // Routine Contains details on its Elements
                    foreach (TreeNode subNode in topNode.Nodes)
                    {
                        if (subNode.Tag != null) // Not an Element
                        {
                            iUnicityCount++;
                            if (iUnicityCount > 1) break;
                            cmpNode = subNode;
                        }
                    }
                    if (iUnicityCount != 1)
                    {
                        MessageBox.Show("Please select right matching.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    if (topNode.Nodes.Count != 1)
                    {
                        MessageBox.Show("Please select right matching.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    cmpNode = topNode.Nodes[0];
                }
            }
            else
            {
                topNode = tnNode.Parent;
                cmpNode = tnNode;
            }
            tnNode = null;

            if (topNode.Parent == null) return;
            if (cmpNode.Tag == null) return;

            bool elemsTreeViewUpdate = false;
            bool differentTypes = false;

            switch (S6xNav.getHeaderCateg(topNode.Parent.Name))
            {
                case S6xNavHeaderCategory.ROUTINES:
                    RoutineSkeleton cmpRtSkt = ((RoutineSkeleton)((object[])cmpNode.Tag)[0]);
                    S6xRoutine cmpRoutine = ((S6xRoutine)((object[])cmpNode.Tag)[1]);
                    RoutineSkeleton curRtSkt = (RoutineSkeleton)currentSkeleton[topNode.Name];
                    S6xRoutine s6xRoutine = (S6xRoutine)sadBin.S6x.slRoutines[topNode.Name];
                    if (s6xRoutine == null)
                    {
                        if (curRtSkt == null) return;
                        s6xRoutine = new S6xRoutine();
                        s6xRoutine.BankNum = curRtSkt.BankNum;
                        s6xRoutine.AddressInt = curRtSkt.AddressInt;
                    }
                    s6xRoutine.ShortLabel = cmpRtSkt.ShortLabel;
                    s6xRoutine.Label = cmpRtSkt.Label;
                    s6xRoutine.Comments = cmpRtSkt.Comments;
                    s6xRoutine.Store = true;

                    if (sadBin.S6x.slRoutines.ContainsKey(topNode.Name)) sadBin.S6x.slRoutines[topNode.Name] = s6xRoutine;
                    else sadBin.S6x.slRoutines.Add(topNode.Name, s6xRoutine);
                    sadBin.S6x.isSaved = false;

                    topNode.Text = s6xRoutine.Label;
                    topNode.ToolTipText = s6xRoutine.UniqueAddressHex + "\r\n" + s6xRoutine.ShortLabel + "\r\n\r\n" + s6xRoutine.Comments;
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    S6xRegister s6xReg = ((S6xRegister)cmpNode.Tag).Clone();
                    if (topNode.Name.Contains(SADDef.AdditionSeparator))
                    {
                        s6xReg.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim().Split(SADDef.AdditionSeparator[0])[0]);
                        s6xReg.AdditionalAddress10 = topNode.Name.Substring(1).Trim().Split(SADDef.AdditionSeparator[0])[1];
                    }
                    else
                    {
                        s6xReg.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                    }
                    s6xReg.Store = true;
                    if (sadBin.S6x.slRegisters.ContainsKey(topNode.Name))
                    {
                        // AutoConstValue Registers (RBase, RConst) are protected
                        if (((S6xRegister)sadBin.S6x.slRegisters[topNode.Name]).AutoConstValue)
                        {
                            MessageBox.Show("RBase or RConst detected registers are protected. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        sadBin.S6x.slRegisters[topNode.Name] = s6xReg;
                    }
                    else sadBin.S6x.slRegisters.Add(topNode.Name, s6xReg);
                    sadBin.S6x.isSaved = false;

                    topNode.Text = s6xReg.FullLabel;
                    topNode.ToolTipText = s6xReg.FullComments;
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    if (topNode.Tag == null) return;
                    if (typeof(S6xStructure) != topNode.Tag.GetType())
                    {
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (typeof(S6xStructure) != cmpNode.Tag.GetType())
                    {
                        if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xStructure s6xStruct = ((S6xStructure)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xStruct.Number != ((S6xStructure)topNode.Tag).Number)
                        {
                            if (MessageBox.Show("Rows number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }

                        s6xStruct.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xStruct.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xStruct.Store = true;
                        if (sadBin.S6x.slStructures.ContainsKey(topNode.Name)) sadBin.S6x.slStructures[topNode.Name] = s6xStruct;
                        else sadBin.S6x.slStructures.Add(topNode.Name, s6xStruct);
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xStruct.Label;
                        topNode.ToolTipText = s6xStruct.UniqueAddressHex + "\r\n" + s6xStruct.ShortLabel + "\r\n\r\n" + s6xStruct.Comments;
                    }
                    else
                    {
                        if (cmpNode.Tag.GetType() == typeof(S6xStructure))
                        {
                            // No way Here
                            return;
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xTable))
                        {
                            S6xTable s6xTable = ((S6xTable)cmpNode.Tag).Clone();
                            s6xTable.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xTable.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xTable.Store = true;
                            s6xTable.ColsScalerAddress = string.Empty;
                            s6xTable.RowsScalerAddress = string.Empty;
                            sadBin.S6x.slTables.Add(topNode.Name, s6xTable);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xTable.Label;
                            topNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)].Nodes.Add(topNode);
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xFunction))
                        {
                            S6xFunction s6xFunction = ((S6xFunction)cmpNode.Tag).Clone();
                            s6xFunction.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xFunction.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xFunction.Store = true;
                            sadBin.S6x.slFunctions.Add(topNode.Name, s6xFunction);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xFunction.Label;
                            topNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)].Nodes.Add(topNode);
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xScalar))
                        {
                            S6xScalar s6xScalar = ((S6xScalar)cmpNode.Tag).Clone();
                            s6xScalar.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xScalar.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xScalar.Store = true;
                            sadBin.S6x.slScalars.Add(topNode.Name, s6xScalar);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xScalar.Label;
                            topNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes.Add(topNode);
                        }
                        else
                        {
                            return;
                        }

                        searchTreeViewCount();
                        sadBin.S6x.slStructures.Remove(((S6xStructure)topNode.Tag).UniqueAddress);
                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                        if (niMFHeaderCateg.isValid)
                        {
                            TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                            if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                            tnMFNode = null;
                        }
                        niMFHeaderCateg = null;
                    }
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.TABLES:
                    if (topNode.Tag == null) return;
                    if (typeof(S6xTable) != topNode.Tag.GetType())
                    {
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (typeof(S6xTable) != cmpNode.Tag.GetType())
                    {
                        if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xTable s6xTable = ((S6xTable)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xTable.ColsNumber != ((S6xTable)topNode.Tag).ColsNumber)
                        {
                            if (MessageBox.Show("Columns number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        if (s6xTable.RowsNumber != ((S6xTable)topNode.Tag).RowsNumber)
                        {
                            if (MessageBox.Show("Rows number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        if (s6xTable.WordOutput != ((S6xTable)topNode.Tag).WordOutput)
                        {
                            if (MessageBox.Show("Table types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }

                        s6xTable.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xTable.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xTable.Store = true;
                        if (sadBin.S6x.slTables.ContainsKey(topNode.Name))
                        {
                            S6xTable s6xOldTable = (S6xTable)sadBin.S6x.slTables[topNode.Name];
                            s6xTable.ColsScalerAddress = s6xOldTable.ColsScalerAddress;
                            s6xTable.RowsScalerAddress = s6xOldTable.RowsScalerAddress;
                            s6xTable.XdfUniqueId = s6xOldTable.XdfUniqueId;
                            s6xTable.ColsScalerXdfUniqueId = s6xOldTable.ColsScalerXdfUniqueId;
                            s6xTable.RowsScalerXdfUniqueId = s6xOldTable.RowsScalerXdfUniqueId;
                            sadBin.S6x.slTables[topNode.Name] = s6xTable;
                        }
                        else
                        {
                            s6xTable.ColsScalerAddress = string.Empty;
                            s6xTable.RowsScalerAddress = string.Empty;
                            sadBin.S6x.slTables.Add(topNode.Name, s6xTable);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xTable.Label;
                        topNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                    }
                    else
                    {
                        if (cmpNode.Tag.GetType() == typeof(S6xStructure))
                        {
                            S6xStructure s6xStruct = ((S6xStructure)cmpNode.Tag).Clone();
                            s6xStruct.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xStruct.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xStruct.Store = true;
                            sadBin.S6x.slStructures.Add(topNode.Name, s6xStruct);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xStruct.Label;
                            topNode.ToolTipText = s6xStruct.UniqueAddressHex + "\r\n" + s6xStruct.ShortLabel + "\r\n\r\n" + s6xStruct.Comments;
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xTable))
                        {
                            // No way Here
                            return;
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xFunction))
                        {
                            S6xFunction s6xFunction = ((S6xFunction)cmpNode.Tag).Clone();
                            s6xFunction.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xFunction.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xFunction.Store = true;
                            sadBin.S6x.slFunctions.Add(topNode.Name, s6xFunction);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xFunction.Label;
                            topNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)].Nodes.Add(topNode);
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xScalar))
                        {
                            S6xScalar s6xScalar = ((S6xScalar)cmpNode.Tag).Clone();
                            s6xScalar.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xScalar.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xScalar.Store = true;
                            sadBin.S6x.slScalars.Add(topNode.Name, s6xScalar);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xScalar.Label;
                            topNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes.Add(topNode);
                        }
                        else
                        {
                            return;
                        }

                        searchTreeViewCount();
                        sadBin.S6x.slTables.Remove(((S6xTable)topNode.Tag).UniqueAddress);
                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                        if (niMFHeaderCateg.isValid)
                        {
                            TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                            if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                            tnMFNode = null;
                        }
                        niMFHeaderCateg = null;
                    }
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    if (topNode.Tag == null) return;
                    if (typeof(S6xFunction) != topNode.Tag.GetType())
                    {
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (typeof(S6xFunction) != cmpNode.Tag.GetType())
                    {
                        if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xFunction s6xFunction = ((S6xFunction)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xFunction.RowsNumber != ((S6xFunction)topNode.Tag).RowsNumber)
                        {
                            if (MessageBox.Show("Rows number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        if (s6xFunction.ByteInput != ((S6xFunction)topNode.Tag).ByteInput)
                        {
                            if (MessageBox.Show("Input types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        if (s6xFunction.ByteOutput != ((S6xFunction)topNode.Tag).ByteOutput)
                        {
                            if (MessageBox.Show("Output types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }

                        s6xFunction.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xFunction.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xFunction.Store = true;
                        if (sadBin.S6x.slFunctions.ContainsKey(topNode.Name))
                        {
                            s6xFunction.XdfUniqueId = ((S6xFunction)sadBin.S6x.slFunctions[topNode.Name]).XdfUniqueId;
                            sadBin.S6x.slFunctions[topNode.Name] = s6xFunction;
                        }
                        else
                        {
                            sadBin.S6x.slFunctions.Add(topNode.Name, s6xFunction);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xFunction.Label;
                        topNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                    }
                    else
                    {
                        if (cmpNode.Tag.GetType() == typeof(S6xStructure))
                        {
                            S6xStructure s6xStruct = ((S6xStructure)cmpNode.Tag).Clone();
                            s6xStruct.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xStruct.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xStruct.Store = true;
                            sadBin.S6x.slStructures.Add(topNode.Name, s6xStruct);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xStruct.Label;
                            topNode.ToolTipText = s6xStruct.UniqueAddressHex + "\r\n" + s6xStruct.ShortLabel + "\r\n\r\n" + s6xStruct.Comments;
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xTable))
                        {
                            S6xTable s6xTable = ((S6xTable)cmpNode.Tag).Clone();
                            s6xTable.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xTable.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xTable.Store = true;
                            s6xTable.ColsScalerAddress = string.Empty;
                            s6xTable.RowsScalerAddress = string.Empty;
                            sadBin.S6x.slTables.Add(topNode.Name, s6xTable);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xTable.Label;
                            topNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)].Nodes.Add(topNode);
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xFunction))
                        {
                            // No Way Here
                            return;
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xScalar))
                        {
                            S6xScalar s6xScalar = ((S6xScalar)cmpNode.Tag).Clone();
                            s6xScalar.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xScalar.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xScalar.Store = true;
                            sadBin.S6x.slScalars.Add(topNode.Name, s6xScalar);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xScalar.Label;
                            topNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes.Add(topNode);
                        }
                        else
                        {
                            return;
                        }

                        searchTreeViewCount();
                        sadBin.S6x.slFunctions.Remove(((S6xFunction)topNode.Tag).UniqueAddress);
                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                        if (niMFHeaderCateg.isValid)
                        {
                            TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                            if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                            tnMFNode = null;
                        }
                        niMFHeaderCateg = null;
                    }
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    if (topNode.Tag == null) return;
                    if (typeof(S6xScalar) != topNode.Tag.GetType())
                    {
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (typeof(S6xScalar) != cmpNode.Tag.GetType())
                    {
                        if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xScalar s6xScalar = ((S6xScalar)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (s6xScalar.Byte != ((S6xScalar)topNode.Tag).Byte)
                        {
                            if (MessageBox.Show("Types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }

                        s6xScalar.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xScalar.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xScalar.Store = true;
                        if (sadBin.S6x.slScalars.ContainsKey(topNode.Name))
                        {
                            s6xScalar.XdfUniqueId = ((S6xScalar)sadBin.S6x.slScalars[topNode.Name]).XdfUniqueId;
                            sadBin.S6x.slScalars[topNode.Name] = s6xScalar;
                        }
                        else
                        {
                            sadBin.S6x.slScalars.Add(topNode.Name, s6xScalar);
                        }
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xScalar.Label;
                        topNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                    }
                    else
                    {
                        if (cmpNode.Tag.GetType() == typeof(S6xStructure))
                        {
                            S6xStructure s6xStruct = ((S6xStructure)cmpNode.Tag).Clone();
                            s6xStruct.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xStruct.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xStruct.Store = true;
                            sadBin.S6x.slStructures.Add(topNode.Name, s6xStruct);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xStruct.Label;
                            topNode.ToolTipText = s6xStruct.UniqueAddressHex + "\r\n" + s6xStruct.ShortLabel + "\r\n\r\n" + s6xStruct.Comments;
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xTable))
                        {
                            S6xTable s6xTable = ((S6xTable)cmpNode.Tag).Clone();
                            s6xTable.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xTable.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xTable.Store = true;
                            s6xTable.ColsScalerAddress = string.Empty;
                            s6xTable.RowsScalerAddress = string.Empty;
                            sadBin.S6x.slTables.Add(topNode.Name, s6xTable);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xTable.Label;
                            topNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)].Nodes.Add(topNode);
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xFunction))
                        {
                            S6xFunction s6xFunction = ((S6xFunction)cmpNode.Tag).Clone();
                            s6xFunction.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                            s6xFunction.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                            s6xFunction.Store = true;
                            sadBin.S6x.slFunctions.Add(topNode.Name, s6xFunction);
                            sadBin.S6x.isSaved = false;

                            topNode.Text = s6xFunction.Label;
                            topNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                            topNode.Parent.Nodes.Remove(topNode);

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)].Nodes.Add(topNode);
                        }
                        else if (cmpNode.Tag.GetType() == typeof(S6xScalar))
                        {
                            // No Way Here
                            return;
                        }
                        else
                        {
                            return;
                        }

                        searchTreeViewCount();
                        sadBin.S6x.slScalars.Remove(((S6xScalar)topNode.Tag).UniqueAddress);
                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                        if (niMFHeaderCateg.isValid)
                        {
                            TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                            if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                            tnMFNode = null;
                        }
                        niMFHeaderCateg = null;
                    }
                    elemsTreeViewUpdate = true;
                    break;
            }

            if (elemsTreeViewUpdate)
            {
                S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[topNode.Parent.Name]);
                if (!niMFHeaderCateg.isValid) return;
                TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                tnNode = elemsTreeView.Nodes[topNode.Parent.Name].Nodes[topNode.Name];
                if (tnMFNode == null)
                {
                    tnMFNode = new TreeNode();
                    tnMFNode.Name = topNode.Name;
                    tnMFNode.ContextMenuStrip = elemsContextMenuStrip;
                    niMFHeaderCateg.AddNode(tnMFNode, null, null, null, false);
                    niMFHeaderCateg.HeaderCategoryNode.Text = S6xNav.getHeaderCategLabel(niMFHeaderCateg.HeaderCategory) + " (" + niMFHeaderCateg.ElementsCount.ToString() + ")";
                }
                tnMFNode.Text = topNode.Text;
                tnMFNode.ToolTipText = topNode.ToolTipText;
                tnMFNode.ForeColor = Color.Purple;
                tnMFNode = null;
                niMFHeaderCateg = null;
            }
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compareTreeView.SelectedNode.ExpandAll();
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compareTreeView.SelectedNode.Collapse();
        }

        private void definedFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterDefinedToolStripMenuItem.Checked = !filterDefinedToolStripMenuItem.Checked;
        }

        private void shortLabelFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterShortLabelToolStripMenuItem.Checked = !filterShortLabelToolStripMenuItem.Checked;
        }

        private void filterUniqueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filterUniqueToolStripMenuItem.Checked = !filterUniqueToolStripMenuItem.Checked;
        }

        private void filterToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            compareOutput();
        }
    }
}
