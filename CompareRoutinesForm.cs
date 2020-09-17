using System;
using System.Collections;
using System.Collections.Generic;
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
        private SortedList slPossibleMatchingMixedElements = null;
        private SortedList slCounterMatchingRegisters = null;
        private SortedList slCounterMatchingMixedElements = null;

        private System.Windows.Forms.Timer mainUpdateTimer = null;
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

            mainUpdateTimer = new System.Windows.Forms.Timer();
            mainUpdateTimer.Enabled = false;
            mainUpdateTimer.Interval = 500;
            mainUpdateTimer.Tick += new EventHandler(mainUpdateTimer_Tick);

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
            mainUpdateTimer.Enabled = false;

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

        private void mainUpdateTimer_Tick(object sender, EventArgs e)
        {
            mainUpdateTimer.Enabled = false;

            try
            {
                bool refreshCount = false;

                foreach (TreeNode tnCateg in compareTreeView.Nodes)
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
                        if (tnNode.Tag == null) continue;
                        // New Elements have to stay in place
                        if (tnNode.Tag.GetType() == typeof(string)) continue;

                        TreeNode tnMainNode = niMFHeaderCateg.FindElement(tnNode.Name);
                        if (tnMainNode == null)
                        {
                            if (tnNode.Tag.GetType() == typeof(Register)) continue;

                            lsRemoval.Add(tnNode);
                            continue;
                        }
                        if (tnMainNode.Text != tnNode.Text) tnNode.Text = tnMainNode.Text;
                        // Not ToolTipText management, it is specific here
                        //if (tnMainNode.ToolTipText != tnNode.ToolTipText) tnNode.ToolTipText = tnMainNode.ToolTipText;
                    }

                    foreach (TreeNode tnNode in lsRemoval) tnCateg.Nodes.Remove(tnNode);
                    refreshCount |= lsRemoval.Count > 0;
                }

                if (refreshCount) compareTreeViewCount();
                mainUpdateTimer.Enabled = true;
            }
            catch { }
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
            if (e.Node.Parent.Parent != null)
            {
                if (e.Node.Parent.Parent.Parent == null) return;
                else if (e.Node.Parent.Parent.Parent.Name != S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES)) return;
            }
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
            mainUpdateTimer.Enabled = false;
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

            mainUpdateTimer.Enabled = true;

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

            SADBin cmpSadBin = null;
            ToolsRoutinesComp.compareRoutinesComparisonSkeletons(ref currentSkeleton, ref fileSkeleton, ref sadBin, ref cmpSadBin, minOpsNumber, gapTolerance, levTolerance);
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

            ToolsRoutinesComp.compareRoutinesComparisonSkeletons(ref currentSkeleton, ref fileSkeleton, ref sadBin, ref cmpSadBin, minOpsNumber, gapTolerance, levTolerance);
            progressGlobalStatus = 100;
        }

        // Compare Analysis
        //      To move Routine level elements to top structures
        private void compareAnalysis()
        {
            // No Analysis for File Skeleton mode
            if (skeletonMode) return;

            // To Fill in slPossibleMatchingMixedElements, slPossibleMatchingRegisters and their counter equivalent based on Current Skeleton and Compare one
            compareAnalysisSkeleton(ref currentSkeleton, true);
            compareAnalysisSkeleton(ref fileSkeleton, false);

            // Now time to do some calculations, which will be reused after that
            // UniqueMatching, UniqueCounterMatching and CounterMatchingCount

            // Elements
            foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slPossibleMatchingMixedElements.Values)
            {
                rsaMDO.UniqueMatching = rsaMDO.slMatchings.Count == 1;
                rsaMDO.UniqueCounterMatching = true;
                foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                {
                    if (slCounterMatchingMixedElements.ContainsKey(rsaMGO.MatchingKey))
                    {
                        rsaMGO.CounterMatchingCount = ((RoutineSkeletonAnalysisMatchedObject)slCounterMatchingMixedElements[rsaMGO.MatchingKey]).slMatchings.Count;
                    }
                    if (!rsaMGO.UniqueCounterMatching) rsaMDO.UniqueCounterMatching = false;
                    // Continuing to calculate on each Matching Object
                }
            }

            // Counter equivalence
            foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slCounterMatchingMixedElements.Values)
            {
                rsaMDO.UniqueMatching = rsaMDO.slMatchings.Count == 1;
                rsaMDO.UniqueCounterMatching = true;
                foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                {
                    if (slPossibleMatchingMixedElements.ContainsKey(rsaMGO.MatchingKey))
                    {
                        rsaMGO.CounterMatchingCount = ((RoutineSkeletonAnalysisMatchedObject)slPossibleMatchingMixedElements[rsaMGO.MatchingKey]).slMatchings.Count;
                    }
                    if (!rsaMGO.UniqueCounterMatching) rsaMDO.UniqueCounterMatching = false;
                    // Continuing to calculate on each Matching Object
                }
            }

            // Registers
            foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slPossibleMatchingRegisters.Values)
            {
                rsaMDO.UniqueMatching = rsaMDO.slMatchings.Count == 1;  // Works when BitFlags are present too, because their registers are in slMatchings
                rsaMDO.UniqueCounterMatching = true;
                foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                {
                    if (slCounterMatchingRegisters.ContainsKey(rsaMGO.MatchingKey))
                    {
                        rsaMGO.CounterMatchingCount = ((RoutineSkeletonAnalysisMatchedObject)slCounterMatchingRegisters[rsaMGO.MatchingKey]).slMatchings.Count;
                    }
                    if (!rsaMGO.UniqueCounterMatching) rsaMDO.UniqueCounterMatching = false;    // Works when BitFlags are present too, because their registers are in slMatchings
                    // Continuing to calculate on each Matching Object
                }

                if (rsaMDO.slSubMatched == null) continue;

                // Now BitFlags level
                rsaMDO.SubMatchingCompatibility = true;
                foreach (RoutineSkeletonAnalysisMatchedObject rsaBFMDO in rsaMDO.slSubMatched.Values)
                {
                    rsaBFMDO.UniqueMatching = rsaBFMDO.slMatchings.Count == 1;
                    rsaBFMDO.UniqueCounterMatching = true;
                    foreach (RoutineSkeletonAnalysisMatchingObject rsaBFMGO in rsaBFMDO.slMatchings.Values)
                    {
                        if (rsaBFMGO.MatchingValue != rsaBFMDO.MatchedValue) rsaMDO.SubMatchingCompatibility = false;
                        if (rsaBFMGO.ParentMatchingKey == string.Empty) continue;

                        RoutineSkeletonAnalysisMatchedObject rsaCounterMDO = (RoutineSkeletonAnalysisMatchedObject)slCounterMatchingRegisters[rsaBFMGO.ParentMatchingKey];
                        if (rsaCounterMDO.slSubMatched == null) continue;
                        if (!rsaCounterMDO.slSubMatched.ContainsKey(rsaBFMGO.MatchingKey)) continue;
                        rsaBFMGO.CounterMatchingCount = ((RoutineSkeletonAnalysisMatchedObject)rsaCounterMDO.slSubMatched[rsaBFMGO.MatchingKey]).slMatchings.Count;
                        if (!rsaBFMGO.UniqueCounterMatching) rsaBFMDO.UniqueCounterMatching = false;
                        // Continuing to calculate on each Matching Object
                    }
                }
            }

            // Counter equivalence
            foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slCounterMatchingRegisters.Values)
            {
                rsaMDO.UniqueMatching = rsaMDO.slMatchings.Count == 1;  // Works when BitFlags are present too, because their registers are in slMatchings
                rsaMDO.UniqueCounterMatching = true;
                foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                {
                    if (slCounterMatchingRegisters.ContainsKey(rsaMGO.MatchingKey))
                    {
                        rsaMGO.CounterMatchingCount = ((RoutineSkeletonAnalysisMatchedObject)slPossibleMatchingRegisters[rsaMGO.MatchingKey]).slMatchings.Count;
                    }
                    if (!rsaMGO.UniqueCounterMatching) rsaMDO.UniqueCounterMatching = false;    // Works when BitFlags are present too, because their registers are in slMatchings
                    // Continuing to calculate on each Matching Object
                }

                if (rsaMDO.slSubMatched == null) continue;

                // Now BitFlags level
                foreach (RoutineSkeletonAnalysisMatchedObject rsaBFMDO in rsaMDO.slSubMatched.Values)
                {
                    rsaBFMDO.UniqueMatching = rsaBFMDO.slMatchings.Count == 1;
                    rsaBFMDO.UniqueCounterMatching = true;
                    foreach (RoutineSkeletonAnalysisMatchingObject rsaBFMGO in rsaBFMDO.slMatchings.Values)
                    {
                        if (rsaBFMGO.MatchingValue != rsaBFMDO.MatchedValue) rsaMDO.SubMatchingCompatibility = false;
                        if (rsaBFMGO.ParentMatchingKey == string.Empty) continue;

                        RoutineSkeletonAnalysisMatchedObject rsaCounterMDO = (RoutineSkeletonAnalysisMatchedObject)slPossibleMatchingRegisters[rsaBFMGO.ParentMatchingKey];
                        if (rsaCounterMDO.slSubMatched == null) continue;
                        if (!rsaCounterMDO.slSubMatched.ContainsKey(rsaBFMGO.MatchingKey)) continue;
                        rsaBFMGO.CounterMatchingCount = ((RoutineSkeletonAnalysisMatchedObject)rsaCounterMDO.slSubMatched[rsaBFMGO.MatchingKey]).slMatchings.Count;
                        if (!rsaBFMGO.UniqueCounterMatching) rsaBFMDO.UniqueCounterMatching = false;
                        // Continuing to calculate on each Matching Object
                    }
                }
            }

        }

        // Compare Analysis by Skeleton
        //      To move Routine level elements to top structures
        private void compareAnalysisSkeleton(ref SortedList binSkeleton, bool currentSkeleton)
        {
            if (skeletonMode) return;

            if (binSkeleton == null) return;
            if (binSkeleton.Count == 0) return;

            SortedList slMatchingMixedElements = null;
            SortedList slMatchingRegisters = null;

            if (currentSkeleton)
            {
                slPossibleMatchingMixedElements = new SortedList();
                slPossibleMatchingRegisters = new SortedList();

                slMatchingMixedElements = slPossibleMatchingMixedElements;
                slMatchingRegisters = slPossibleMatchingRegisters;
            }
            else
            {
                slCounterMatchingMixedElements = new SortedList();
                slCounterMatchingRegisters = new SortedList();

                slMatchingMixedElements = slCounterMatchingMixedElements;
                slMatchingRegisters = slCounterMatchingRegisters;
            }

            foreach (RoutineSkeleton rsSkt in binSkeleton.Values)
            {
                if (rsSkt.alMatches == null) continue;
                if (rsSkt.alMatches.Count == 0) continue;

                if (rsSkt.slPossibleMatchingMixedElements != null)
                {
                    foreach (object[] matchingMixedElem in rsSkt.slPossibleMatchingMixedElements.Values)
                    {
                        string mdoKey = (string)matchingMixedElem[0];
                        string mdgKey = (string)matchingMixedElem[1];
                        int iOccur = (int)matchingMixedElem[2];
                        RoutineSkeletonAnalysisMatchedObject rsaMDO = (RoutineSkeletonAnalysisMatchedObject)slMatchingMixedElements[mdoKey];
                        if (rsaMDO == null)
                        {
                            rsaMDO = new RoutineSkeletonAnalysisMatchedObject(mdoKey);
                            slMatchingMixedElements.Add(rsaMDO.MatchedKey, rsaMDO);
                        }
                        rsaMDO.AddMatching(mdgKey, iOccur);
                    }
                }
                if (rsSkt.slPossibleMatchingRegisters != null)
                {
                    foreach (object[] matchingReg in rsSkt.slPossibleMatchingRegisters.Values)
                    {
                        string mdoKey = Tools.RegisterUniqueAddress((string)matchingReg[0]);
                        string mdoValue = (string)matchingReg[0];
                        string mdgKey = Tools.RegisterUniqueAddress((string)matchingReg[1]);
                        string mdgValue = (string)matchingReg[1];
                        int iOccur = (int)matchingReg[2];
                        SortedList slMatchingRegBifFlags = null;
                        if (matchingReg[3] != null) slMatchingRegBifFlags = (SortedList)matchingReg[3];

                        RoutineSkeletonAnalysisMatchedObject rsaMDO = (RoutineSkeletonAnalysisMatchedObject)slMatchingRegisters[mdoKey];
                        if (rsaMDO == null)
                        {
                            rsaMDO = new RoutineSkeletonAnalysisMatchedObject(mdoKey);
                            rsaMDO.MatchedValue = mdoValue;
                            slMatchingRegisters.Add(rsaMDO.MatchedKey, rsaMDO);
                        }
                        RoutineSkeletonAnalysisMatchingObject rsaMGO = rsaMDO.AddMatching(mdgKey, iOccur);
                        rsaMGO.MatchingValue = mdgValue;

                        if (slMatchingRegBifFlags == null) continue;

                        foreach (object[] arrBitFlags in slMatchingRegBifFlags.Values)
                        {
                            string mdoBFKey = arrBitFlags[0].ToString();
                            string mdoBFValue = mdoBFKey;
                            string mdgBFKey = mdgKey + "." + arrBitFlags[1].ToString();
                            string mdgBFValue = arrBitFlags[1].ToString();
                            int iBFOccur = (int)arrBitFlags[2];

                            if (rsaMDO.slSubMatched == null) rsaMDO.slSubMatched = new SortedList();
                            RoutineSkeletonAnalysisMatchedObject rsaBFMDO = (RoutineSkeletonAnalysisMatchedObject)rsaMDO.slSubMatched[mdoBFKey];
                            if (rsaBFMDO == null)
                            {
                                rsaBFMDO = rsaMDO.AddSubMatched(mdoBFKey);
                                rsaBFMDO.MatchedValue = mdoBFValue;
                            }
                            RoutineSkeletonAnalysisMatchingObject rsaBFMGO = rsaBFMDO.AddMatching(mdgBFKey, iBFOccur);
                            rsaBFMGO.MatchingValue = mdgBFValue;
                            rsaBFMGO.ParentMatchingKey = mdgKey;
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
                compareTreeViewCount();
                return;
            }
            
            if (currentSkeleton.Count == 0)
            {
                compareTreeViewCount();
                return;
            }

            compareTreeView.BeginUpdate();
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

                int counterMatchingTotal = 0;

                // Elements Output Inside Routine
                if (!skeletonMode)
                {
                    if (rsSkt.alMixedElements != null)
                    {
                        foreach (object oElement in rsSkt.alMixedElements)
                        {
                            if (oElement == null) continue;

                            string rCategName = string.Empty;
                            TreeNode tnRCateg = null;
                            TreeNode tnRElem = null;

                            if (oElement.GetType() == typeof(Structure))
                            {
                                S6xStructure s6xStructure = (S6xStructure)sadBin.S6x.slStructures[((Structure)oElement).UniqueAddress];
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

                                if (tnRCateg.Nodes.ContainsKey(s6xStructure.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = s6xStructure.UniqueAddress;
                                tnRElem.Text = s6xStructure.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                            if (oElement.GetType() == typeof(Table))
                            {
                                S6xTable s6xTable = (S6xTable)sadBin.S6x.slTables[((Table)oElement).UniqueAddress];
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

                                if (tnRCateg.Nodes.ContainsKey(s6xTable.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = s6xTable.UniqueAddress;
                                tnRElem.Text = s6xTable.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                            if (oElement.GetType() == typeof(Function))
                            {
                                S6xFunction s6xFunction = (S6xFunction)sadBin.S6x.slFunctions[((Function)oElement).UniqueAddress];
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

                                if (tnRCateg.Nodes.ContainsKey(s6xFunction.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = s6xFunction.UniqueAddress;
                                tnRElem.Text = s6xFunction.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                            if (oElement.GetType() == typeof(Scalar))
                            {
                                S6xScalar s6xScalar = (S6xScalar)sadBin.S6x.slScalars[((Scalar)oElement).UniqueAddress];
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

                                if (tnRCateg.Nodes.ContainsKey(s6xScalar.UniqueAddress)) continue;

                                tnRElem = new TreeNode();
                                tnRElem.Name = s6xScalar.UniqueAddress;
                                tnRElem.Text = s6xScalar.Label;
                                tnRElem.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                tnRCateg.Nodes.Add(tnRElem);
                                continue;
                            }
                        }
                    }
                }
                
                // Matches Output
                foreach (object[] arrRes in rsSkt.alMatches)
                {
                    RoutineSkeleton rsMatchingSkt = (RoutineSkeleton)arrRes[0];

                    // Matching routines unicity
                    int counterMatching = 0;
                    if (fileSkeleton.ContainsKey(rsMatchingSkt.UniqueAddress))
                    {
                        if (((RoutineSkeleton)fileSkeleton[rsMatchingSkt.UniqueAddress]).alMatches != null) counterMatching = ((RoutineSkeleton)fileSkeleton[rsMatchingSkt.UniqueAddress]).alMatches.Count;
                        counterMatchingTotal += counterMatching;
                    }

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

                    if (counterMatching > 1) tnMatchingNode.ToolTipText += "\r\nOther matching routine(s) : " + (counterMatching - 1).ToString();

                    // Elements inside Match Output
                    if (!skeletonMode)
                    {
                        if (rsMatchingSkt.alMixedElements != null)
                        {
                            foreach (object oElement in rsMatchingSkt.alMixedElements)
                            {
                                if (oElement == null) continue;
                                
                                string rCategName = string.Empty;
                                TreeNode tnRCateg = null;
                                TreeNode tnRElem = null;

                                if (oElement.GetType() == typeof(Structure))
                                {
                                    S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[((Structure)oElement).UniqueAddress];
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

                                    if (tnRCateg.Nodes.ContainsKey(s6xStructure.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = s6xStructure.UniqueAddress;
                                    tnRElem.Text = s6xStructure.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                                if (oElement.GetType() == typeof(Table))
                                {
                                    S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[((Table)oElement).UniqueAddress];
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

                                    if (tnRCateg.Nodes.ContainsKey(s6xTable.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = s6xTable.UniqueAddress;
                                    tnRElem.Text = s6xTable.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                                if (oElement.GetType() == typeof(Function))
                                {
                                    S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[((Function)oElement).UniqueAddress];
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

                                    if (tnRCateg.Nodes.ContainsKey(s6xFunction.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = s6xFunction.UniqueAddress;
                                    tnRElem.Text = s6xFunction.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                                if (oElement.GetType() == typeof(Scalar))
                                {
                                    S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[((Scalar)oElement).UniqueAddress];
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

                                    if (tnRCateg.Nodes.ContainsKey(s6xScalar.UniqueAddress)) continue;

                                    tnRElem = new TreeNode();
                                    tnRElem.Name = s6xScalar.UniqueAddress;
                                    tnRElem.Text = s6xScalar.Label;
                                    tnRElem.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                    tnRCateg.Nodes.Add(tnRElem);
                                    continue;
                                }
                            }
                        }
                    }
                }

                if (counterMatchingTotal > rsSkt.alMatches.Count) tnNode.ToolTipText += "\r\nMatching routine(s) show(s) no unicity.";

                if (showNode) parentNode.Nodes.Add(tnNode);
            }

            if (!skeletonMode)
            {
                // Mixed Elements
                if (slPossibleMatchingMixedElements != null)
                {
                    foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slPossibleMatchingMixedElements.Values)
                    {
                        if (filterUniqueToolStripMenuItem.Checked)
                        {
                            if (!rsaMDO.UniqueMatching) continue;
                            if (!rsaMDO.UniqueCounterMatching) continue;
                        }

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
                            tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slStructures[rsaMDO.MatchedKey];
                                toolTipHeader = "Number : " + ((S6xStructure)s6xObject).Number;
                                curShortLabel = ((S6xStructure)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slTables[rsaMDO.MatchedKey];
                                toolTipHeader = "Colums : " + ((S6xTable)s6xObject).ColsNumber + "\r\n" + "Rows : " + ((S6xTable)s6xObject).RowsNumber + "\r\n";
                                toolTipHeader += ((S6xTable)s6xObject).SignedOutput ? "Signed" : "Unsigned" + " " + (((S6xTable)s6xObject).WordOutput ? "Word" : "Byte") + " Table";
                                curShortLabel = ((S6xTable)s6xObject).ShortLabel;
                            }
                        }
                        if (tnCurNode == null)
                        {
                            niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                            if (!niMFHeaderCateg.isValid) continue; // Bad Header
                            tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slFunctions[rsaMDO.MatchedKey];
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
                            tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                            niMFHeaderCateg = null;
                            if (tnCurNode != null)
                            {
                                s6xObject = sadBin.S6x.slScalars[rsaMDO.MatchedKey];
                                toolTipHeader = ((S6xScalar)s6xObject).Signed ? "Signed" : "Unsigned" + " " + (((S6xScalar)s6xObject).Byte ? "Byte" : "Word") + " Scalar";
                                curShortLabel = ((S6xScalar)s6xObject).ShortLabel;
                            }
                        }

                        TreeNode elemParentNode = null;
                        // Element identified
                        if (tnCurNode != null)
                        {
                            niMFElement = new S6xNavInfo(tnCurNode);
                            if (!niMFElement.isValid) continue; // Bad Element

                            elemParentNode = compareTreeView.Nodes[niMFElement.HeaderCategoryName];
                        }
                        else
                        //Included Element to be created, but Header Categ to be identified
                        {
                            foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                            {
                                if (cmpSadBin.S6x.slStructures.ContainsKey(rsaMGO.MatchingKey)) elemParentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)];
                                else if (cmpSadBin.S6x.slTables.ContainsKey(rsaMGO.MatchingKey)) elemParentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)];
                                else if (cmpSadBin.S6x.slFunctions.ContainsKey(rsaMGO.MatchingKey)) elemParentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)];
                                else if (cmpSadBin.S6x.slScalars.ContainsKey(rsaMGO.MatchingKey)) elemParentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)];
                                if (elemParentNode != null) break;
                            }
                        }

                        if (elemParentNode == null) continue;

                        TreeNode tnElemNode = null;
                        if (elemParentNode.Nodes.ContainsKey(rsaMDO.MatchedKey))
                        {
                            tnElemNode = elemParentNode.Nodes[rsaMDO.MatchedKey];
                        }
                        else
                        {
                            tnElemNode = new TreeNode();
                            if (tnCurNode == null)
                            {
                                tnElemNode.Name = rsaMDO.MatchedKey;
                                tnElemNode.Text = Tools.UniqueAddressHex(rsaMDO.MatchedKey);
                                tnElemNode.ToolTipText = "New element to be created at " + tnElemNode.Text;
                                s6xObject = rsaMDO.MatchedKey;
                            }
                            else
                            {
                                tnElemNode.Name = tnCurNode.Name;
                                tnElemNode.Text = tnCurNode.Text;
                                tnElemNode.ToolTipText = tnCurNode.ToolTipText;
                            }
                            if (toolTipHeader != string.Empty) tnElemNode.ToolTipText = toolTipHeader + "\r\n" + tnElemNode.ToolTipText;
                            tnElemNode.ContextMenuStrip = resultElemContextMenuStrip;
                            tnElemNode.Tag = s6xObject;
                        }
                        if (!rsaMDO.UniqueCounterMatching) tnElemNode.ToolTipText += "\r\nMatching element(s) show(s) no unicity.";

                        foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                        {
                            TreeNode tnMatchingNode = new TreeNode();
                            tnMatchingNode.Name = rsaMGO.MatchingKey;
                            tnMatchingNode.Text = Tools.UniqueAddressHex(rsaMGO.MatchingKey);
                            tnMatchingNode.ToolTipText = "Occurrences : " + rsaMGO.Occurences;
                            if (!rsaMGO.UniqueCounterMatching) tnMatchingNode.ToolTipText += "\r\nOther matching element(s) : " + (rsaMGO.CounterMatchingCount - 1).ToString();
                            tnMatchingNode.ContextMenuStrip = resultElemContextMenuStrip;

                            S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[rsaMGO.MatchingKey];
                            if (s6xStructure != null)
                            {
                                if (s6xStructure.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xStructure.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xStructure.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xStructure.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + "Number : " + s6xStructure.Number;
                                tnMatchingNode.ToolTipText += "\r\n" + s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                                tnMatchingNode.Tag = s6xStructure;
                                tnElemNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[rsaMGO.MatchingKey];
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
                                tnElemNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[rsaMGO.MatchingKey];
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
                                tnElemNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }
                            S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[rsaMGO.MatchingKey];
                            if (s6xScalar != null)
                            {
                                if (s6xScalar.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xScalar.Store) continue;
                                if (filterShortLabelToolStripMenuItem.Checked && s6xScalar.ShortLabel.ToUpper() == curShortLabel.ToUpper()) continue;

                                tnMatchingNode.Text = s6xScalar.Label;
                                tnMatchingNode.ToolTipText += "\r\n" + (s6xScalar.Signed ? "Signed" : "Unsigned") + " " + (s6xScalar.Byte ? "Byte" : "Word") + " Scalar";
                                tnMatchingNode.ToolTipText += "\r\n" + s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                                tnMatchingNode.Tag = s6xScalar;
                                tnElemNode.Nodes.Add(tnMatchingNode);
                                showNode = true;
                                continue;
                            }

                        }

                        if (showNode)
                        {
                            if (!elemParentNode.Nodes.Contains(tnElemNode)) elemParentNode.Nodes.Add(tnElemNode);
                        }
                    }
                }

                // Registers
                if (slPossibleMatchingRegisters != null)
                {
                    foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slPossibleMatchingRegisters.Values)
                    {
                        if (filterUniqueToolStripMenuItem.Checked)
                        {
                            if (!rsaMDO.UniqueMatching) continue;
                            if (!rsaMDO.UniqueCounterMatching) continue;
                        }

                        TreeNode parentNode = compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS)];
                        if (parentNode == null) break; ;

                        if (parentNode.Nodes.ContainsKey(Tools.RegisterUniqueAddress(rsaMDO.MatchedKey))) continue;

                        TreeNode tnNewNode = new TreeNode();
                        tnNewNode.Name = rsaMDO.MatchedKey;
                        Register rReg = (Register)sadBin.Calibration.slRegisters[tnNewNode.Name];
                        if (rReg == null) continue;

                        tnNewNode.Tag = rReg;

                        S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[parentNode.Name]);
                        if (!niMFHeaderCateg.isValid) continue;
                        TreeNode tnMFNode = niMFHeaderCateg.FindElement(tnNewNode.Name);
                        if (tnMFNode == null)
                        {
                            tnNewNode.Text = Tools.RegisterInstruction(rsaMDO.MatchedValue);
                        }
                        else
                        {
                            tnNewNode.Text = tnMFNode.Text;
                            if (rReg.S6xRegister != null)
                            {
                                tnNewNode.Text = rReg.S6xRegister.FullLabel;
                                if (rReg.S6xRegister.MultipleMeanings)
                                {
                                    tnNewNode.ToolTipText = "Byte : " + rReg.S6xRegister.ByteLabel + "\r\n";
                                    tnNewNode.ToolTipText += "Word : " + rReg.S6xRegister.WordLabel + "\r\n";
                                }
                                tnNewNode.ToolTipText += rReg.S6xRegister.FullComments;
                            }
                        }
                        if (!rsaMDO.UniqueCounterMatching) tnNewNode.ToolTipText += "\r\nMatching register(s) show(s) no unicity.";
                        // Context Menu is available with or without BitFlags
                        tnNewNode.ContextMenuStrip = resultElemContextMenuStrip;

                        if (rsaMDO.slSubMatched == null)
                        {
                            // No Bit Flags, classical Matching will be used
                            foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                            {
                                S6xRegister s6xCmpReg = (S6xRegister)cmpSadBin.S6x.slRegisters[rsaMGO.MatchingKey];
                                if (s6xCmpReg == null) continue;
                                if (s6xCmpReg.Skip) continue;
                                if (filterDefinedToolStripMenuItem.Checked && !s6xCmpReg.Store) continue;

                                if (filterShortLabelToolStripMenuItem.Checked && rReg.S6xRegister != null)
                                {
                                    if (rReg.S6xRegister.Label.ToLower() == s6xCmpReg.Label.ToLower()) continue;
                                }

                                TreeNode tnMatchingNode = new TreeNode();
                                tnMatchingNode.Name = rsaMGO.MatchingKey;
                                tnMatchingNode.Text = s6xCmpReg.FullLabel;
                                tnMatchingNode.ToolTipText = "Occurrences : " + rsaMGO.Occurences;
                                if (!rsaMGO.UniqueCounterMatching) tnMatchingNode.ToolTipText += "\r\nOther matching register(s) : " + (rsaMGO.CounterMatchingCount - 1).ToString();
                                if (s6xCmpReg.MultipleMeanings)
                                {
                                    tnMatchingNode.ToolTipText = "\r\n" + "Byte : " + s6xCmpReg.ByteLabel + "\r\n";
                                    tnMatchingNode.ToolTipText += "Word : " + s6xCmpReg.WordLabel + "\r\n";
                                }
                                tnMatchingNode.ToolTipText += "\r\n" + s6xCmpReg.FullComments;
                                tnMatchingNode.Tag = s6xCmpReg;
                                tnMatchingNode.ContextMenuStrip = resultElemContextMenuStrip;
                                tnNewNode.Nodes.Add(tnMatchingNode);

                                if (s6xCmpReg.isBitFlags)
                                {
                                    foreach (S6xBitFlag s6xBf in s6xCmpReg.BitFlags)
                                    {
                                        TreeNode tnMachingRegBFNode = new TreeNode();
                                        tnMachingRegBFNode.Name = s6xCmpReg.UniqueAddress + "." + s6xBf.Position.ToString();
                                        tnMachingRegBFNode.Text = string.Format("B{0,-5}{1}", s6xBf.Position.ToString(), s6xBf.ShortLabel);
                                        tnMachingRegBFNode.ToolTipText = tnMachingRegBFNode.Text + "\r\n" + s6xBf.Label + "\r\n" + s6xBf.Comments;
                                        tnMachingRegBFNode.Tag = new object[] { s6xCmpReg, s6xBf };
                                        // No Menu to Import it, because nothing to be compared
                                        //tnMachingRegBFNode.ContextMenuStrip = resultElemContextMenuStrip;
                                        tnMatchingNode.Nodes.Add(tnMachingRegBFNode);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Bit Flags, specific Matching will be used
                            foreach (RoutineSkeletonAnalysisMatchedObject rsaBFMDO in rsaMDO.slSubMatched.Values)
                            {
                                S6xBitFlag s6xBF = null;

                                TreeNode tnBFNode = new TreeNode();
                                tnBFNode.Name = rsaBFMDO.MatchedKey;
                                tnBFNode.Text = "B" + rsaBFMDO.MatchedValue;
                                tnBFNode.ToolTipText = tnBFNode.Text;
                                tnBFNode.ContextMenuStrip = resultElemContextMenuStrip;
                                if (rReg.S6xRegister != null) s6xBF = rReg.S6xRegister.GetBitFlag(Convert.ToInt32(rsaBFMDO.MatchedValue));
                                if (s6xBF != null)
                                {
                                    tnBFNode.Text = string.Format("B{0,-5}{1}", rsaBFMDO.MatchedValue, s6xBF.ShortLabel);
                                    tnBFNode.ToolTipText = tnBFNode.Text + "\r\n" + s6xBF.Label + "\r\n" + s6xBF.Comments;
                                }
                                tnBFNode.Tag = s6xBF;

                                foreach (RoutineSkeletonAnalysisMatchingObject rsaBFMGO in rsaBFMDO.slMatchings.Values)
                                {
                                    S6xRegister s6xCmpReg = (S6xRegister)cmpSadBin.S6x.slRegisters[rsaBFMGO.ParentMatchingKey];
                                    if (s6xCmpReg == null) continue;
                                    if (s6xCmpReg.Skip) continue;
                                    if (filterDefinedToolStripMenuItem.Checked && !s6xCmpReg.Store) continue;

                                    S6xBitFlag s6xCmpBF = s6xCmpReg.GetBitFlag(Convert.ToInt32(rsaBFMGO.MatchingValue));
                                    if (s6xCmpBF == null) continue;

                                    if (filterShortLabelToolStripMenuItem.Checked && s6xBF != null)
                                    {
                                        if (s6xBF.Label.ToLower() == s6xCmpBF.Label.ToLower()) continue;
                                    }

                                    TreeNode tnMatchingBFNode = new TreeNode();
                                    tnMatchingBFNode.Name = rsaBFMGO.MatchingKey;
                                    tnMatchingBFNode.Text = string.Format("B{0,-5}", rsaBFMGO.MatchingValue);
                                    tnMatchingBFNode.ToolTipText = "Occurrences : " + rsaBFMGO.Occurences;
                                    tnMatchingBFNode.ContextMenuStrip = resultElemContextMenuStrip;
                                    tnMatchingBFNode.Tag = s6xCmpBF;
                                    if (s6xCmpReg.isBitFlags)
                                    {
                                        foreach (S6xBitFlag s6xCmpBf in s6xCmpReg.BitFlags)
                                        {
                                            if (s6xCmpBf.Position.ToString() == rsaBFMGO.MatchingValue)
                                            {
                                                tnMatchingBFNode.Text = string.Format("B{0,-5}{1}", rsaBFMGO.MatchingValue, s6xCmpBf.ShortLabel);
                                                tnMatchingBFNode.ToolTipText = "Occurrences : " + rsaBFMGO.Occurences;
                                                tnMatchingBFNode.ToolTipText += "\r\n" + tnMatchingBFNode.Text + "\r\n" + s6xCmpBf.Label + "\r\n" + s6xCmpBf.Comments;
                                                break;
                                            }
                                        }
                                    }
                                    tnBFNode.Nodes.Add(tnMatchingBFNode);

                                    TreeNode tnMatchingBFRegNode = new TreeNode();
                                    tnMatchingBFRegNode.Name = s6xCmpReg.UniqueAddress;
                                    tnMatchingBFRegNode.Text = s6xCmpReg.FullLabel;
                                    if (s6xCmpReg.MultipleMeanings)
                                    {
                                        tnMatchingBFRegNode.ToolTipText = "\r\n" + "Byte : " + s6xCmpReg.ByteLabel + "\r\n";
                                        tnMatchingBFRegNode.ToolTipText += "Word : " + s6xCmpReg.WordLabel + "\r\n";
                                    }
                                    tnMatchingBFRegNode.ToolTipText += "\r\n" + s6xCmpReg.FullComments;
                                    tnMatchingBFRegNode.Tag = s6xCmpReg;
                                    tnMatchingBFNode.Nodes.Add(tnMatchingBFRegNode);

                                    if (s6xCmpReg.isBitFlags)
                                    {
                                        foreach (S6xBitFlag s6xBf in s6xCmpReg.BitFlags)
                                        {
                                            TreeNode tnMatchingBFRegBFNode = new TreeNode();
                                            tnMatchingBFRegBFNode.Name = s6xCmpReg.UniqueAddress + "." + s6xBf.Position.ToString();
                                            tnMatchingBFRegBFNode.Text = string.Format("B{0,-5}{1}", s6xBf.Position.ToString(), s6xBf.ShortLabel);
                                            tnMatchingBFRegBFNode.ToolTipText = tnMatchingBFRegBFNode.Text + "\r\n" + s6xBf.Label + "\r\n" + s6xBf.Comments;
                                            tnMatchingBFRegBFNode.Tag = new object[] { s6xCmpReg, s6xBf };
                                            tnMatchingBFRegNode.Nodes.Add(tnMatchingBFRegBFNode);
                                        }
                                    }
                                }

                                if (tnBFNode.Nodes.Count > 0) tnNewNode.Nodes.Add(tnBFNode);
                            }
                        }

                        if (tnNewNode.Nodes.Count > 0)
                        {
                            parentNode.Nodes.Add(tnNewNode);
                        }
                    }
                }
            }

            compareTreeViewCount();

            compareTreeView.EndUpdate();
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

                // Mixed Elements
                foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slPossibleMatchingMixedElements.Values)
                {
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
                        tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slStructuresReporting;
                            curType = "STRUCTURE";
                            curUniqueAddressHex = ((S6xStructure)sadBin.S6x.slStructures[rsaMDO.MatchedKey]).UniqueAddressHex;
                            curFullLabel = ((S6xStructure)sadBin.S6x.slStructures[rsaMDO.MatchedKey]).ShortLabel + " - " + ((S6xStructure)sadBin.S6x.slStructures[rsaMDO.MatchedKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slTablesReporting;
                            curType = "TABLE";
                            curUniqueAddressHex = ((S6xTable)sadBin.S6x.slTables[rsaMDO.MatchedKey]).UniqueAddressHex;
                            curFullLabel = ((S6xTable)sadBin.S6x.slTables[rsaMDO.MatchedKey]).ShortLabel + " - " + ((S6xTable)sadBin.S6x.slTables[rsaMDO.MatchedKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slFunctionsReporting;
                            curType = "FUNCTION";
                            curUniqueAddressHex = ((S6xFunction)sadBin.S6x.slFunctions[rsaMDO.MatchedKey]).UniqueAddressHex;
                            curFullLabel = ((S6xFunction)sadBin.S6x.slFunctions[rsaMDO.MatchedKey]).ShortLabel + " - " + ((S6xFunction)sadBin.S6x.slFunctions[rsaMDO.MatchedKey]).Label;
                        }
                    }
                    if (tnCurNode == null)
                    {
                        niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                        if (!niMFHeaderCateg.isValid) continue; // Bad Header
                        tnCurNode = niMFHeaderCateg.FindElement(rsaMDO.MatchedKey);
                        niMFHeaderCateg = null;
                        if (tnCurNode != null)
                        {
                            curSL = slScalarsReporting;
                            curType = "SCALAR";
                            curUniqueAddressHex = ((S6xScalar)sadBin.S6x.slScalars[rsaMDO.MatchedKey]).UniqueAddressHex;
                            curFullLabel = ((S6xScalar)sadBin.S6x.slScalars[rsaMDO.MatchedKey]).ShortLabel + " - " + ((S6xScalar)sadBin.S6x.slScalars[rsaMDO.MatchedKey]).Label;
                        }
                    }

                    if (tnCurNode == null)
                    {
                        //Included Element to be created, but Header Categ to be identified
                        {
                            foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                            {
                                if (cmpSadBin.S6x.slStructures.ContainsKey(rsaMGO.MatchingKey))
                                {
                                    curSL = slStructuresReporting;
                                    curType = "STRUCTURE";
                                    curUniqueAddressHex = Tools.UniqueAddressHex(rsaMDO.MatchedKey);
                                    curFullLabel = "New structure to be created at " + curUniqueAddressHex;
                                }
                                else if (cmpSadBin.S6x.slTables.ContainsKey(rsaMGO.MatchingKey))
                                {
                                    curSL = slTablesReporting;
                                    curType = "TABLE";
                                    curUniqueAddressHex = Tools.UniqueAddressHex(rsaMDO.MatchedKey);
                                    curFullLabel = "New table to be created at " + curUniqueAddressHex;
                                }
                                else if (cmpSadBin.S6x.slFunctions.ContainsKey(rsaMGO.MatchingKey))
                                {
                                    curSL = slFunctionsReporting;
                                    curType = "FUNCTION";
                                    curUniqueAddressHex = Tools.UniqueAddressHex(rsaMDO.MatchedKey);
                                    curFullLabel = "New function to be created at " + curUniqueAddressHex;
                                }
                                else if (cmpSadBin.S6x.slScalars.ContainsKey(rsaMGO.MatchingKey))
                                {
                                    curSL = slScalarsReporting;
                                    curType = "SCALAR";
                                    curUniqueAddressHex = Tools.UniqueAddressHex(rsaMDO.MatchedKey);
                                    curFullLabel = "New scalar to be created at " + curUniqueAddressHex;
                                }
                                if (curSL != null) break;
                            }
                        }
                    }
                    if (curSL == null) continue;    // Not Identified
                    
                    foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                    {
                        string cmpType = string.Empty;
                        string cmpUniqueAddressHex = string.Empty;
                        string cmpFullLabel = string.Empty;

                        S6xStructure s6xStructure = (S6xStructure)cmpSadBin.S6x.slStructures[rsaMGO.MatchingKey];
                        if (s6xStructure != null)
                        {
                            if (s6xStructure.Skip) continue;

                            cmpType = "STRUCTURE";
                            cmpUniqueAddressHex = ((S6xStructure)cmpSadBin.S6x.slStructures[rsaMGO.MatchingKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xStructure)cmpSadBin.S6x.slStructures[rsaMGO.MatchingKey]).ShortLabel + " - " + ((S6xStructure)cmpSadBin.S6x.slStructures[rsaMGO.MatchingKey]).Label;

                            slMatches.Add(rsaMGO.MatchingKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, rsaMGO.Occurences.ToString() });
                            continue;
                        }
                        S6xTable s6xTable = (S6xTable)cmpSadBin.S6x.slTables[rsaMGO.MatchingKey];
                        if (s6xTable != null)
                        {
                            if (s6xTable.Skip) continue;

                            cmpType = "TABLE";
                            cmpUniqueAddressHex = ((S6xTable)cmpSadBin.S6x.slTables[rsaMGO.MatchingKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xTable)cmpSadBin.S6x.slTables[rsaMGO.MatchingKey]).ShortLabel + " - " + ((S6xTable)cmpSadBin.S6x.slTables[rsaMGO.MatchingKey]).Label;

                            slMatches.Add(rsaMGO.MatchingKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, rsaMGO.Occurences.ToString() });
                            continue;
                        }
                        S6xFunction s6xFunction = (S6xFunction)cmpSadBin.S6x.slFunctions[rsaMGO.MatchingKey];
                        if (s6xFunction != null)
                        {
                            if (s6xFunction.Skip) continue;

                            cmpType = "FUNCTION";
                            cmpUniqueAddressHex = ((S6xFunction)cmpSadBin.S6x.slFunctions[rsaMGO.MatchingKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xFunction)cmpSadBin.S6x.slFunctions[rsaMGO.MatchingKey]).ShortLabel + " - " + ((S6xFunction)cmpSadBin.S6x.slFunctions[rsaMGO.MatchingKey]).Label;

                            slMatches.Add(rsaMGO.MatchingKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, rsaMGO.Occurences.ToString() });
                            continue;
                        }
                        S6xScalar s6xScalar = (S6xScalar)cmpSadBin.S6x.slScalars[rsaMGO.MatchingKey];
                        if (s6xScalar != null)
                        {
                            if (s6xScalar.Skip) continue;

                            cmpType = "SCALAR";
                            cmpUniqueAddressHex = ((S6xScalar)cmpSadBin.S6x.slScalars[rsaMGO.MatchingKey]).UniqueAddressHex;
                            cmpFullLabel = ((S6xScalar)cmpSadBin.S6x.slScalars[rsaMGO.MatchingKey]).ShortLabel + " - " + ((S6xScalar)cmpSadBin.S6x.slScalars[rsaMGO.MatchingKey]).Label;

                            slMatches.Add(rsaMGO.MatchingKey, new string[] { cmpType, cmpUniqueAddressHex, cmpFullLabel, rsaMGO.Occurences.ToString() });
                            continue;
                        }
                    }

                    if (slMatches.Count == 0) continue;

                    curSL.Add(rsaMDO.MatchedKey, new object[] { curType, curUniqueAddressHex, curFullLabel, slMatches });
                }

                // Registers
                foreach (RoutineSkeletonAnalysisMatchedObject rsaMDO in slPossibleMatchingRegisters.Values)
                {
                    string curFullLabel = string.Empty;
                    SortedList slMatches = new SortedList();

                    if (!sadBin.S6x.slRegisters.ContainsKey(rsaMDO.MatchedKey)) curFullLabel = Tools.RegisterInstruction(rsaMDO.MatchedValue);
                    else curFullLabel = ((S6xRegister)sadBin.S6x.slRegisters[rsaMDO.MatchedKey]).FullLabel;

                    foreach (RoutineSkeletonAnalysisMatchingObject rsaMGO in rsaMDO.slMatchings.Values)
                    {
                        string cmpFullLabel = string.Empty;

                        S6xRegister s6xCmpReg = (S6xRegister)cmpSadBin.S6x.slRegisters[rsaMGO.MatchingKey];
                        if (s6xCmpReg == null) continue; // No Interest for reporting
                        if (s6xCmpReg.Skip || !s6xCmpReg.Store) continue;

                        cmpFullLabel = s6xCmpReg.FullLabel;

                        slMatches.Add(rsaMGO.MatchingKey, new string[] { cmpFullLabel, rsaMGO.Occurences.ToString() });
                    }

                    if (slMatches.Count == 0) continue;

                    slRegistersReporting.Add(rsaMDO.MatchedKey, new object[] { curFullLabel, slMatches });
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

        private void compareTreeViewCount()
        {
            foreach (TreeNode tnParent in compareTreeView.Nodes)
            {
                string categLabel = S6xNav.getHeaderCategLabel(tnParent.Name);
                if (categLabel == string.Empty) return;
                tnParent.Text = categLabel + " (" + tnParent.Nodes.Count.ToString() + ")";
            }
        }

        private void elemsTreeViewUpdate(ref object[] elemsTreeViewUpdates)
        {
            if (elemsTreeViewUpdates == null) return;

            elemsTreeView.BeginUpdate();
            ArrayList alHeaderCategsForCount = new ArrayList();
            TreeNode singleAddedNode = null;
            foreach (object[] elemsTreeViewUpdate in elemsTreeViewUpdates)
            {
                string sAction = (string)elemsTreeViewUpdate[0];
                S6xNavInfo niETHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName((S6xNavHeaderCategory)elemsTreeViewUpdate[1])]);
                if (!niETHeaderCateg.isValid) continue;
                TreeNode tnNode = (TreeNode)elemsTreeViewUpdate[2];
                TreeNode tnETNode = null;

                switch (sAction.ToUpper())
                {
                    case "ADD":
                        tnETNode = niETHeaderCateg.FindElement(tnNode.Name);
                        if (tnETNode != null) continue;

                        tnETNode = new TreeNode();
                        tnETNode.Name = tnNode.Name;
                        tnETNode.Text = tnNode.Text;
                        tnETNode.ToolTipText = tnNode.ToolTipText;
                        tnETNode.ForeColor = Color.Purple;
                        tnETNode.ContextMenuStrip = elemsContextMenuStrip;
                        niETHeaderCateg.AddNode(tnETNode, null, null, null, false);
                        if (!alHeaderCategsForCount.Contains(niETHeaderCateg)) alHeaderCategsForCount.Add(niETHeaderCateg);

                        // singleAddedNode available when alone in Updates or preceded by a deletion
                        if (elemsTreeViewUpdates.Length == 1) singleAddedNode = tnETNode;
                        else if (elemsTreeViewUpdates.Length == 2) if (((string)((object[])elemsTreeViewUpdates[0])[0]).ToUpper() == "DEL") singleAddedNode = tnETNode;
                        break;
                    case "UPD":
                        tnETNode = niETHeaderCateg.FindElement(tnNode.Name);
                        if (tnETNode == null) continue;
                        tnETNode.Text = tnNode.Text;
                        tnETNode.ToolTipText = tnNode.ToolTipText;
                        tnETNode.ForeColor = Color.Purple;
                        break;
                    case "DEL":
                        tnETNode = niETHeaderCateg.FindElement(tnNode.Name);
                        if (tnETNode != null) tnETNode.Parent.Nodes.RemoveByKey(tnETNode.Name);
                        tnETNode = null;
                        if (!alHeaderCategsForCount.Contains(niETHeaderCateg)) alHeaderCategsForCount.Add(niETHeaderCateg);
                        break;
                }
            }

            foreach (S6xNavInfo niETHeaderCateg in alHeaderCategsForCount)
            {
                niETHeaderCateg.HeaderCategoryNode.Text = S6xNav.getHeaderCategLabel(niETHeaderCateg.HeaderCategory) + " (" + niETHeaderCateg.ElementsCount.ToString() + ")";
            }
            elemsTreeView.EndUpdate();

            if (singleAddedNode != null)
            {
                try { elemsTreeView.SelectedNode = singleAddedNode; }
                catch { }
            }
        }
        
        private void importElementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode categNode = compareTreeView.SelectedNode;
            if (categNode == null) return;

            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[categNode.Name]);
            if (!niMFHeaderCateg.isValid) return;

            ArrayList alNodeNames = new ArrayList();
            foreach (TreeNode tnNode in categNode.Nodes) alNodeNames.Add(tnNode.Name);

            object[] elemsTreeViewUpdates = null;
            compareTreeView.BeginUpdate();
            foreach (string nodeName in alNodeNames)
            {
                TreeNode topNode = categNode.Nodes[nodeName];
                TreeNode cmpNode = null;
                TreeNode bfNode = null;
                importElement(ref topNode, ref cmpNode, ref bfNode, false, ref elemsTreeViewUpdates);
            }
            alNodeNames = null;
            compareTreeView.EndUpdate();

            elemsTreeViewUpdate(ref elemsTreeViewUpdates);
        }

        private void importElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnNode = compareTreeView.SelectedNode;

            if (tnNode.Parent == null) return;

            TreeNode topNode = null;
            TreeNode cmpNode = null;
            TreeNode bfNode = null;
            object[] elemsTreeViewUpdates = null;

            if (tnNode.Parent.Parent == null)
            {
                // Sub Level 1 Mode
                topNode = tnNode;
            }
            else if (tnNode.Parent.Parent.Parent == null)
            {
                // Sub Level 2 Mode
                if (tnNode.Parent.Parent.Name == S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS) && tnNode.Name.Length == 1)
                {
                    // tnNode is Register BitFlag
                    topNode = tnNode.Parent;
                    bfNode = tnNode;
                }
                else
                {
                    // tnNode is matching node
                    topNode = tnNode.Parent;
                    cmpNode = tnNode;
                }
            }
            else if (tnNode.Parent.Parent.Parent.Name == S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS))
            {
                // tnNode is matching Register BitFlag
                topNode = tnNode.Parent.Parent;
                bfNode = tnNode.Parent;
                cmpNode = tnNode;
            }
            else
            {
                // Not Managed
                return;
            }
            tnNode = null;

            importElement(ref topNode, ref cmpNode, ref bfNode, true, ref elemsTreeViewUpdates);

            elemsTreeViewUpdate(ref elemsTreeViewUpdates);
        }

        private void importElement(ref TreeNode topNode, ref TreeNode cmpNode, ref TreeNode bfNode, bool singleMode, ref object[] elemsTreeViewUpdates)
        {
            if (topNode == null) return;
            
            RoutineSkeletonAnalysisMatchedObject rsaMDO = null;
            RoutineSkeletonAnalysisMatchedObject rsaBFMDO = null;

            bool uniqueMatching = true;
            bool uniqueCounterMatching = true;
            bool bitFlagCompatibility = true;

            switch (S6xNav.getHeaderCateg(topNode.Parent.Name))
            {
                case S6xNavHeaderCategory.ROUTINES:
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    rsaMDO = (RoutineSkeletonAnalysisMatchedObject)slPossibleMatchingRegisters[topNode.Name];
                    if (bfNode != null && rsaMDO.slSubMatched != null) rsaBFMDO = (RoutineSkeletonAnalysisMatchedObject)rsaMDO.slSubMatched[bfNode.Name];
                    break;
                default:    // Elements
                    rsaMDO = (RoutineSkeletonAnalysisMatchedObject)slPossibleMatchingMixedElements[topNode.Name];
                    break;
            }

            if (cmpNode == null)
            {
                // Unicity Mngt 
                switch (S6xNav.getHeaderCateg(topNode.Parent.Name))
                {
                    case S6xNavHeaderCategory.ROUTINES:
                        // Routine Contains details on its Elements
                        foreach (TreeNode subNode in topNode.Nodes)
                        {
                            if (subNode.Tag != null) // Not an Element
                            {
                                if (cmpNode != null)
                                {
                                    uniqueMatching = false;
                                    cmpNode = null;
                                    break;
                                }
                                cmpNode = subNode;
                            }
                        }
                        if (cmpNode != null)
                        {
                            if (fileSkeleton.ContainsKey(cmpNode.Name)) uniqueCounterMatching = ((SortedList)(((object[])fileSkeleton[cmpNode.Name])[1])).Count > 1;
                        }
                        break;
                    default:    // Elements
                        if (rsaBFMDO != null)
                        {
                            uniqueMatching = rsaBFMDO.UniqueMatching;
                            uniqueCounterMatching = rsaBFMDO.UniqueCounterMatching;
                            bitFlagCompatibility = rsaBFMDO.SubMatchingCompatibility;
                            // cmpNode only defined when no Sub Matched Elements are available (Only to stay compliant with Registers)
                            if (rsaBFMDO.slSubMatched == null) cmpNode = bfNode.Nodes[0];
                        }
                        else if (rsaMDO != null)
                        {
                            uniqueMatching = rsaMDO.UniqueMatching;
                            uniqueCounterMatching = rsaMDO.UniqueCounterMatching;
                            bitFlagCompatibility = rsaMDO.SubMatchingCompatibility;
                            // cmpNode only defined when no Sub Matched Elements are available (Only to stay compliant with Registers)
                            if (rsaMDO.slSubMatched == null) cmpNode = topNode.Nodes[0];
                            else if (uniqueMatching && bitFlagCompatibility)
                            {
                                if (topNode.Nodes[0].Nodes.Count == 1)
                                {
                                    if (topNode.Nodes[0].Nodes[0].Nodes.Count == 1) cmpNode = topNode.Nodes[0].Nodes[0].Nodes[0];
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (S6xNav.getHeaderCateg(topNode.Parent.Name))
                {
                    case S6xNavHeaderCategory.ROUTINES:
                        if (fileSkeleton.ContainsKey(cmpNode.Name)) uniqueCounterMatching = ((SortedList)(((object[])fileSkeleton[cmpNode.Name])[1])).Count > 1;
                        break;
                    default:    // Elements
                        if (rsaBFMDO != null)
                        {
                            uniqueMatching = rsaBFMDO.UniqueMatching;
                            uniqueCounterMatching = rsaBFMDO.UniqueCounterMatching;
                            bitFlagCompatibility = rsaBFMDO.SubMatchingCompatibility;
                        }
                        else if (rsaMDO != null)
                        {
                            uniqueMatching = rsaMDO.UniqueMatching;
                            uniqueCounterMatching = rsaMDO.UniqueCounterMatching;
                            bitFlagCompatibility = rsaMDO.SubMatchingCompatibility;
                        }
                        break;
                }
            }

            // Unicity Mngt 
            if (!uniqueMatching)
            {
                if (!singleMode) return;
                MessageBox.Show("Multiple matchings are available. Please select the right one.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!uniqueCounterMatching)
            {
                if (!singleMode) return;
                if (MessageBox.Show("Counter matching is not unique. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            }

            if (topNode.Parent == null) return;
            if (cmpNode == null) return;
            if (cmpNode.Tag == null) return;

            bool elemsTreeViewUpdate = false;
            bool elementCreation = false;
            bool differentTypes = false;

            ArrayList alElemsTreeViewUpdates = new ArrayList();
            if (elemsTreeViewUpdates != null) alElemsTreeViewUpdates.AddRange(elemsTreeViewUpdates);

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
                    Register rReg = (Register)sadBin.Calibration.slRegisters[topNode.Name];
                    if (rReg == null) return;
                    // AutoConstValue Registers (RBase, RConst) are protected
                    if (rReg.RBase != null || rReg.RConst != null)
                    {
                        if (!singleMode) return;
                        MessageBox.Show("RBase or RConst detected registers are protected. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Whole Register (With all BitFlags if available and compliant)
                    if (bfNode == null)
                    {
                        if (!rsaMDO.SubMatchingCompatibility)
                        {
                            if (!singleMode) return;
                            if (MessageBox.Show("BitFlags are not aligned on positions. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }

                        rReg.S6xRegister = ((S6xRegister)cmpNode.Tag).Clone();
                        rReg.S6xRegister.AddressInt = rReg.AddressInt;
                        rReg.S6xRegister.AdditionalAddress10 = rReg.AdditionalAddress10;
                        rReg.S6xRegister.Store = true;
                        if (sadBin.S6x.slRegisters.ContainsKey(rReg.S6xRegister.UniqueAddress)) sadBin.S6x.slRegisters[rReg.S6xRegister.UniqueAddress] = rReg.S6xRegister;
                        else sadBin.S6x.slRegisters.Add(rReg.S6xRegister.UniqueAddress, rReg.S6xRegister);
                        sadBin.S6x.isSaved = false;

                        topNode.Text = rReg.S6xRegister.FullLabel;
                        topNode.ToolTipText = rReg.S6xRegister.FullComments;
                        elemsTreeViewUpdate = true;
                        break;  // To prevent processing single BitFlag after this
                    }

                    // No details on BitFlag to update ot create
                    if (rsaBFMDO == null) return;

                    // Single BitFlag // bfNode is defined
                    if (rReg.S6xRegister == null)
                    {
                        // Default S6xRegister created
                        rReg.S6xRegister = new S6xRegister(rReg.Address);
                        rReg.S6xRegister.Label = rReg.Instruction;
                        rReg.S6xRegister.ScaleExpression = "X";
                        rReg.S6xRegister.Store = true;
                        if (!sadBin.S6x.slRegisters.ContainsKey(rReg.S6xRegister.UniqueAddress)) sadBin.S6x.slRegisters.Add(rReg.S6xRegister.UniqueAddress, rReg.S6xRegister);
                        else sadBin.S6x.slRegisters[rReg.S6xRegister.UniqueAddress] = rReg.S6xRegister;

                        topNode.Text = rReg.S6xRegister.FullLabel;
                        topNode.ToolTipText = rReg.S6xRegister.FullComments;
                    }

                    S6xBitFlag s6xBF = rReg.S6xRegister.GetBitFlag(Convert.ToInt32(rsaBFMDO.MatchedValue));
                    if (s6xBF != null) rReg.S6xRegister.RemoveBitFlag(s6xBF.Position);
                    s6xBF = ((S6xBitFlag)cmpNode.Tag).Clone();
                    s6xBF.Position = Convert.ToInt32(rsaBFMDO.MatchedValue);
                    rReg.S6xRegister.AddBitFlag(s6xBF);

                    sadBin.S6x.isSaved = false;

                    bfNode.Text = string.Format("B{0,-5}{1}", s6xBF.Position, s6xBF.ShortLabel);
                    bfNode.ToolTipText = topNode.Text + "\r\n" + s6xBF.Label + "\r\n" + s6xBF.Comments;
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    if (topNode.Tag == null) return;
                    if (topNode.Tag.GetType() == typeof(string))
                    {
                        if (!singleMode) return;
                        if (MessageBox.Show("Element has to be created. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        elementCreation = true;
                    }
                    else if (typeof(S6xStructure) != topNode.Tag.GetType())
                    {
                        if (!singleMode) return;
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    
                    if (typeof(S6xStructure) != cmpNode.Tag.GetType())
                    {
                        if (!elementCreation)
                        {
                            if (!singleMode) return;
                            if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xStructure s6xStruct = ((S6xStructure)cmpNode.Tag).Clone();
                        if (!elementCreation)
                        {
                            // To prevent minor differences issues
                            if (s6xStruct.Number != ((S6xStructure)topNode.Tag).Number)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Rows number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
                        }

                        s6xStruct.BankNum = Convert.ToInt32(topNode.Name.Substring(0, 1));
                        s6xStruct.AddressInt = Convert.ToInt32(topNode.Name.Substring(1).Trim());
                        s6xStruct.Store = true;
                        if (sadBin.S6x.slStructures.ContainsKey(topNode.Name)) sadBin.S6x.slStructures[topNode.Name] = s6xStruct;
                        else sadBin.S6x.slStructures.Add(topNode.Name, s6xStruct);
                        sadBin.S6x.isSaved = false;

                        topNode.Text = s6xStruct.Label;
                        topNode.ToolTipText = s6xStruct.UniqueAddressHex + "\r\n" + s6xStruct.ShortLabel + "\r\n\r\n" + s6xStruct.Comments;

                        if (elementCreation) topNode.Tag = s6xStruct;
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

                            if (elementCreation) topNode.Tag = s6xTable;

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

                            if (elementCreation) topNode.Tag = s6xFunction;

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

                            if (elementCreation) topNode.Tag = s6xScalar;

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes.Add(topNode);
                        }
                        else
                        {
                            return;
                        }

                        compareTreeViewCount();
                        if (!elementCreation)
                        {
                            sadBin.S6x.slStructures.Remove(((S6xStructure)topNode.Tag).UniqueAddress);
                            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                            if (niMFHeaderCateg.isValid)
                            {
                                TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                                //if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                                if (tnMFNode != null) alElemsTreeViewUpdates.Add(new object[] {"DEL", S6xNavHeaderCategory.STRUCTURES, tnMFNode});
                                tnMFNode = null;
                            }
                            niMFHeaderCateg = null;
                        }
                    }
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.TABLES:
                    if (topNode.Tag == null) return;
                    if (topNode.Tag.GetType() == typeof(string))
                    {
                        if (!singleMode) return;
                        if (MessageBox.Show("Element has to be created. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        elementCreation = true;
                    }
                    else if (typeof(S6xTable) != topNode.Tag.GetType())
                    {
                        if (!singleMode) return;
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (typeof(S6xTable) != cmpNode.Tag.GetType())
                    {
                        if (!elementCreation)
                        {
                            if (!singleMode) return;
                            if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xTable s6xTable = ((S6xTable)cmpNode.Tag).Clone();
                        if (!elementCreation)
                        {
                            // To prevent minor differences issues
                            if (s6xTable.ColsNumber != ((S6xTable)topNode.Tag).ColsNumber)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Columns number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
                            if (s6xTable.RowsNumber != ((S6xTable)topNode.Tag).RowsNumber)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Rows number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
                            if (s6xTable.WordOutput != ((S6xTable)topNode.Tag).WordOutput)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Table types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
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

                        if (elementCreation) topNode.Tag = s6xTable;
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
                            topNode.Parent.Nodes.Remove(topNode);

                            if (elementCreation) topNode.Tag = s6xStruct;

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)].Nodes.Add(topNode);
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

                            if (elementCreation) topNode.Tag = s6xFunction;

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

                            if (elementCreation) topNode.Tag = s6xScalar;

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes.Add(topNode);
                        }
                        else
                        {
                            return;
                        }

                        compareTreeViewCount();
                        if (!elementCreation)
                        {
                            sadBin.S6x.slTables.Remove(((S6xTable)topNode.Tag).UniqueAddress);
                            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                            if (niMFHeaderCateg.isValid)
                            {
                                TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                                //if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                                if (tnMFNode != null) alElemsTreeViewUpdates.Add(new object[] {"DEL", S6xNavHeaderCategory.TABLES, tnMFNode});
                                tnMFNode = null;
                            }
                            niMFHeaderCateg = null;
                        }
                    }
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    if (topNode.Tag == null) return;
                    if (topNode.Tag.GetType() == typeof(string))
                    {
                        if (!singleMode) return;
                        if (MessageBox.Show("Element has to be created. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        elementCreation = true;
                    }
                    else if (typeof(S6xFunction) != topNode.Tag.GetType())
                    {
                        if (!singleMode) return;
                        if (!elementCreation) MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (typeof(S6xFunction) != cmpNode.Tag.GetType())
                    {
                        if (!singleMode) return;
                        if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xFunction s6xFunction = ((S6xFunction)cmpNode.Tag).Clone();
                        if (!elementCreation)
                        {
                            // To prevent minor differences issues
                            if (s6xFunction.RowsNumber != ((S6xFunction)topNode.Tag).RowsNumber)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Rows number are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
                            if (s6xFunction.ByteInput != ((S6xFunction)topNode.Tag).ByteInput)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Input types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
                            if (s6xFunction.ByteOutput != ((S6xFunction)topNode.Tag).ByteOutput)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Output types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
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

                        if (elementCreation) topNode.Tag = s6xFunction;
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
                            topNode.Parent.Nodes.Remove(topNode);

                            if (elementCreation) topNode.Tag = s6xStruct;

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)].Nodes.Add(topNode);
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

                            if (elementCreation) topNode.Tag = s6xTable;

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

                            if (elementCreation) topNode.Tag = s6xScalar;

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes.Add(topNode);
                        }
                        else
                        {
                            return;
                        }

                        compareTreeViewCount();
                        if (!elementCreation)
                        {
                            sadBin.S6x.slFunctions.Remove(((S6xFunction)topNode.Tag).UniqueAddress);
                            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                            if (niMFHeaderCateg.isValid)
                            {
                                TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                                //if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                                if (tnMFNode != null) alElemsTreeViewUpdates.Add(new object[] {"DEL", S6xNavHeaderCategory.FUNCTIONS, tnMFNode});
                                tnMFNode = null;
                            }
                            niMFHeaderCateg = null;
                        }
                    }
                    elemsTreeViewUpdate = true;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    if (topNode.Tag == null) return;
                    if (topNode.Tag.GetType() == typeof(string))
                    {
                        if (!singleMode) return;
                        if (MessageBox.Show("Element has to be created. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        elementCreation = true;
                    }
                    else if (typeof(S6xScalar) != topNode.Tag.GetType())
                    {
                        if (!singleMode) return;
                        MessageBox.Show("One element is invalid. Nothing can be done.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (typeof(S6xScalar) != cmpNode.Tag.GetType())
                    {
                        if (!elementCreation)
                        {
                            if (!singleMode) return;
                            if (MessageBox.Show("Elements have not the same type. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                        }
                        differentTypes = true;
                    }

                    if (!differentTypes)
                    {
                        S6xScalar s6xScalar = ((S6xScalar)cmpNode.Tag).Clone();
                        // To prevent minor differences issues
                        if (!elementCreation)
                        {
                            if (s6xScalar.Byte != ((S6xScalar)topNode.Tag).Byte)
                            {
                                if (!singleMode) return;
                                if (MessageBox.Show("Types are different. Continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                            }
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

                        if (elementCreation) topNode.Tag = s6xScalar;
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
                            topNode.Parent.Nodes.Remove(topNode);

                            if (elementCreation) topNode.Tag = s6xStruct;

                            compareTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)].Nodes.Add(topNode);
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

                            if (elementCreation) topNode.Tag = s6xTable;

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

                            if (elementCreation) topNode.Tag = s6xFunction;

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

                        compareTreeViewCount();
                        if (!elementCreation)
                        {
                            sadBin.S6x.slScalars.Remove(((S6xScalar)topNode.Tag).UniqueAddress);
                            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                            if (niMFHeaderCateg.isValid)
                            {
                                TreeNode tnMFNode = niMFHeaderCateg.FindElement(topNode.Name);
                                //if (tnMFNode != null) tnMFNode.Parent.Nodes.RemoveByKey(topNode.Name);
                                if (tnMFNode != null) alElemsTreeViewUpdates.Add(new object[] {"DEL", S6xNavHeaderCategory.SCALARS, tnMFNode});
                                tnMFNode = null;
                            }
                            niMFHeaderCateg = null;
                        }
                    }
                    elemsTreeViewUpdate = true;
                    break;
            }

            if (elemsTreeViewUpdate)
            {
                S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[topNode.Parent.Name]);
                if (!niMFHeaderCateg.isValid) return;
                TreeNode tnETNode = niMFHeaderCateg.FindElement(topNode.Name);

                TreeNode tnMFNode = new TreeNode();
                tnMFNode.Name = topNode.Name;
                tnMFNode.Text = topNode.Text;
                tnMFNode.ToolTipText = topNode.ToolTipText;

                if (tnETNode == null) alElemsTreeViewUpdates.Add(new object[] {"ADD", niMFHeaderCateg.HeaderCategory, tnMFNode});
                else alElemsTreeViewUpdates.Add(new object[] {"UPD", niMFHeaderCateg.HeaderCategory, tnMFNode});

                tnMFNode = null;
                niMFHeaderCateg = null;

                if (singleMode && tnETNode != null)
                {
                    try { elemsTreeView.SelectedNode = tnETNode; }
                    catch { }
                }
                tnETNode = null;
            }

            elemsTreeViewUpdates = alElemsTreeViewUpdates.ToArray();
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
