using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SAD806x
{
    public partial class MainForm : Form
    {
        private string binaryFilePath = string.Empty;
        private string s6xFilePath = string.Empty;
        private string textOutputFilePath = string.Empty;
        private string documentationFilePath = string.Empty;

        private SADBin sadBin = null;
        private SADS6x sadS6x = null;

        private System.Windows.Forms.Timer processUpdateTimer = null;
        private Thread processThread = null;
        private ProcessType processType = ProcessType.None;
        private bool processRunning = false;
        private Cursor processPreviousCursor = null;

        private bool dirtyProperties = false;
        private TreeNode lastElemTreeNode = null;
        private TreeNode nextElemTreeNode = null;
        
        private ArrayList alClipBoardTempUniqueAddresses = null;

        private RecentFiles recentFiles = null;

        private Repository repoRegisters = null;
        private Repository repoTables = null;
        private Repository repoFunctions = null;
        private Repository repoScalars = null;
        private Repository repoStructures = null;

        private Repository repoUnits = null;

        private RepositoryConversion repoConversion = null;

        // Start Options
        private bool autoDisassembly = false;
        private bool autoOutput = false;

        public MainForm(string[] appStartArgs)
        {
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }
            
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            this.DragEnter += new DragEventHandler(MainForm_DragEnter);
            this.DragDrop += new DragEventHandler(MainForm_DragDrop);

            textOutputToolStripTextBox.DoubleClick += new EventHandler(textOutputToolStripTextBox_DoubleClick);
            
            elemsTreeView.BeforeLabelEdit += new NodeLabelEditEventHandler(elemsTreeView_BeforeLabelEdit);
            elemsTreeView.AfterLabelEdit += new NodeLabelEditEventHandler(elemsTreeView_AfterLabelEdit);
            elemsTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(elemsTreeView_NodeMouseClick);
            elemsTreeView.AfterSelect += new TreeViewEventHandler(elemsTreeView_AfterSelect);

            elemBankTextBox.Enter += new EventHandler(elemBankTextBox_Enter);
            elemAddressTextBox.Enter += new EventHandler(elemAddressTextBox_Enter);

            categsContextMenuStrip.Opening += new CancelEventHandler(categsContextMenuStrip_Opening);
            elemsContextMenuStrip.Opening += new CancelEventHandler(elemsContextMenuStrip_Opening);
            fileToolStripMenuItem.DropDownOpening += new EventHandler(fileToolStripMenuItem_DropDownOpening);
            toolsToolStripMenuItem.DropDownOpening += new EventHandler(toolsToolStripMenuItem_DropDownOpening);
            helpToolStripMenuItem.DropDownOpening += new EventHandler(helpToolStripMenuItem_DropDownOpening);

            elemDataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(elemDataGridView_DataBindingComplete);

            tableColsScalerButton.Click += new EventHandler(tableScalerButton_Click);
            tableRowsScalerButton.Click += new EventHandler(tableScalerButton_Click);
            tableColsScalerButton.MouseHover += new EventHandler(tableScalerButton_MouseHover);
            tableRowsScalerButton.MouseHover += new EventHandler(tableScalerButton_MouseHover);
            scalerContextMenuStrip.Opening += new CancelEventHandler(scalerContextMenuStrip_Opening);
            scalerToolStripTextBox.TextChanged += new EventHandler(scalerToolStripTextBox_TextChanged);
            scalerToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(scalerToolStripMenuItem_DropDownItemClicked);

            repoContextMenuStrip.Opening += new CancelEventHandler(repoContextMenuStrip_Opening);
            repoToolStripTextBox.TextChanged += new EventHandler(repoToolStripTextBox_TextChanged);
            repoToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(repoToolStripMenuItem_DropDownItemClicked);

            convertToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(convertToolStripMenuItem_DropDownItemClicked);
            convertInputToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(convertInputToolStripMenuItem_DropDownItemClicked);

            elemContextMenuStrip.Opening += new CancelEventHandler(elemContextMenuStrip_Opening);

            pasteMultToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(pasteMultToolStripMenuItem_DropDownItemClicked);
            pasteMultOnItemToolStripMenuItem.DropDownItemClicked += new ToolStripItemClickedEventHandler(pasteMultToolStripMenuItem_DropDownItemClicked);

            for (int iCMenu = 1; iCMenu <= 16; iCMenu++)
            {
                ToolStripMenuItem tsMI = new ToolStripMenuItem(iCMenu.ToString());
                tsMI.Tag = iCMenu;
                tsMI.ToolTipText = iCMenu.ToString() + " times";
                pasteMultToolStripMenuItem.DropDownItems.Add(tsMI);
                tsMI = new ToolStripMenuItem(iCMenu.ToString());
                tsMI.Tag = iCMenu;
                tsMI.ToolTipText = iCMenu.ToString() + " times";
                pasteMultOnItemToolStripMenuItem.DropDownItems.Add(tsMI);
            }

            processUpdateTimer = new System.Windows.Forms.Timer();
            processUpdateTimer.Enabled = false;
            processUpdateTimer.Interval = 100;
            processUpdateTimer.Tick += new EventHandler(processUpdateTimer_Tick);

            Text = Application.ProductName;

            elemOpsRichTextBox.Font = new Font(FontFamily.GenericMonospace, elemOpsRichTextBox.Font.Size);

            structureTipPictureBox.Tag = Tools.StructureTip();
            signatureTipPictureBox.Tag = Tools.SignatureTip();
            elementSignatureTipPictureBox.Tag = Tools.ElementSignatureTip();

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
            
            attachPropertiesEvents();
            
            initForm();
            initRecentFiles();
            initRepositories();
            initRepositoryConversion();

            // Documentation
            FileInfo fiFI = new FileInfo(Application.ExecutablePath);
            documentationFilePath = fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".pdf";
            fiFI = null;
            documentationToolStripMenuItem.Visible = File.Exists(documentationFilePath);

            // Start Options
            if (appStartArgs == null) return;
            if (appStartArgs.Length == 0) return;

            bool autoFolderPath = false;
            string autoPath = string.Empty;
            foreach (string appStartArg in appStartArgs)
            {
                switch (appStartArg.ToLower())
                {
                    case "-d":   // Binary Disassemble
                        autoDisassembly = true;
                        break;
                    case "-o":   // Binary Disassemble + Output
                        autoDisassembly = true;
                        autoOutput = true;
                        break;
                    case "-f":   // Folder Disassemble + Output
                        autoFolderPath = true;
                        break;
                    default:
                        autoPath = appStartArg;
                        break;
                }
            }

            if (autoPath != string.Empty)
            {
                if (!autoPath.Contains(":")) autoPath = Directory.GetCurrentDirectory() + "\\" + autoPath;

                if (autoFolderPath)
                {
                    if (!Directory.Exists(autoPath)) autoPath = string.Empty;
                }
                else if (!File.Exists(autoPath)) autoPath = string.Empty;
            }
            if (autoPath == string.Empty)
            {
                autoDisassembly = false;
                autoOutput = false;
                autoFolderPath = false;
                return;
            }

            if (autoFolderPath)
            {
                startFolderProcess(autoPath);
            }
            else
            {
                binaryFilePath = autoPath;
                s6xFilePath = string.Empty;

                startLoadProcess();
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] dFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (dFiles == null) return;

            string tmpBinFilePath = string.Empty;
            string tmpS6xFilePath = string.Empty;
            string tmpXdfFilePath = string.Empty;
            string tmpDirFilePath = string.Empty;
            string tmpCmtFilePath = string.Empty;
            foreach (string dFile in dFiles)
            {
                FileInfo fiFI = new FileInfo(dFile);
                switch (fiFI.Extension.ToLower())
                {
                    case ".s6x":
                        tmpS6xFilePath = dFile;
                        break;
                    case ".xdf":
                        tmpXdfFilePath = dFile;
                        break;
                    case ".txt":
                        if (fiFI.Name.ToLower().Contains("_dir")) tmpDirFilePath = dFile;
                        else if (fiFI.Name.ToLower().Contains("_cmt")) tmpCmtFilePath = dFile;
                        break;
                    default:
                        tmpBinFilePath = dFile;
                        break;
                }
            }

            if (tmpBinFilePath != string.Empty) selectBinary(tmpBinFilePath, tmpS6xFilePath);
            else if (sadBin == null) return;

            if (!sadBin.isValid) return;

            if (tmpS6xFilePath != string.Empty) selectS6x(tmpS6xFilePath);
            else if (tmpXdfFilePath != string.Empty) importXdfFile(tmpXdfFilePath);
            else if (tmpDirFilePath != string.Empty) importDirFile(tmpDirFilePath);
            else if (tmpCmtFilePath != string.Empty) importCmtFile(tmpCmtFilePath);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processRunning)
            {
                MessageBox.Show("Please wait until the end of the process.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
            else if (sadS6x != null)
            {
                if (!sadS6x.isSaved)
                {
                    if (MessageBox.Show("S6x file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        e.Cancel = true;
                    }
                }
            }

            if (!e.Cancel) Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processRunning)
            {
                MessageBox.Show("Please wait until the end of the process.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (sadS6x != null)
            {
                if (!sadS6x.isSaved)
                {
                    if (MessageBox.Show("S6x file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }
            Exit();
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
            sadS6x = null;

            Dispose();

            GC.Collect();

            Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (sadBin == null)
            {
                s6xToolStripTextBox.Visible = false;
                saveS6xToolStripMenuItem.Visible = false;
                selectS6xToolStripMenuItem.Enabled = false;
            }
            else
            {
                s6xToolStripTextBox.Visible = true;
                saveS6xToolStripMenuItem.Visible = true;
                saveS6xToolStripMenuItem.Enabled = !sadS6x.isSaved;
                selectS6xToolStripMenuItem.Enabled = true;
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            importSignaturesToolStripMenuItem.Enabled = false;
            importDirFileToolStripMenuItem.Enabled = false;
            exportDirFileToolStripMenuItem.Enabled = false;
            importCmtFileToolStripMenuItem.Enabled = false;
            importXdfFileToolStripMenuItem.Enabled = false;
            exportXdfFileToolStripMenuItem.Enabled = false;
            exportXdfResetToolStripMenuItem.Enabled = false;
            showHexToolStripMenuItem.Enabled = false;
            searchObjectsToolStripMenuItem.Enabled = false;
            searchSignatureToolStripMenuItem.Enabled = false;
            compareBinariesToolStripMenuItem.Enabled = false;
            compareBinariesDifDefToolStripMenuItem.Enabled = false;
            compareS6xToolStripMenuItem.Enabled = false;
            routinesComparisonToolStripMenuItem.Enabled = false;
            calibChartViewToolStripMenuItem.Enabled = false;
            massUpdateToolStripMenuItem.Enabled = false;

            if (sadBin != null)
            {
                if (sadBin.isLoaded && sadBin.isValid)
                {
                    importSignaturesToolStripMenuItem.Enabled = true;
                    importDirFileToolStripMenuItem.Enabled = true;
                    exportDirFileToolStripMenuItem.Enabled = true;
                    importCmtFileToolStripMenuItem.Enabled = true;
                    importXdfFileToolStripMenuItem.Enabled = true;
                    exportXdfFileToolStripMenuItem.Enabled = true;
                    exportXdfResetToolStripMenuItem.Enabled = true;
                    searchObjectsToolStripMenuItem.Enabled = true;
                    searchSignatureToolStripMenuItem.Enabled = true;
                    compareS6xToolStripMenuItem.Enabled = true;
                    massUpdateToolStripMenuItem.Enabled = true;
                    if (sadBin.isDisassembled)
                    {
                        compareBinariesToolStripMenuItem.Enabled = true;
                        compareBinariesDifDefToolStripMenuItem.Enabled = true;
                        routinesComparisonToolStripMenuItem.Enabled = true;
                        calibChartViewToolStripMenuItem.Enabled = true;
                    }
                }

                showHexToolStripMenuItem.Enabled = true;
            }
        }

        private void helpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // Nothing to be managed for now
        }

        private void processUpdateTimer_Tick(object sender, EventArgs e)
        {
            processUpdateTimer.Enabled = false;

            if (!processRunning) return;

            switch (processType)
            {
                case ProcessType.Load:
                    if (sadS6x != null) endLoadProcess();
                    else processUpdateTimer.Enabled = true;
                    break;
                case ProcessType.Disassemble:
                    if (sadBin.ProgressStatus > 100) sadBin.ProgressStatus = 100;
                    if (sadBin.ProgressStatus < -1) sadBin.ProgressStatus = -1;
                    switch (sadBin.ProgressStatus)
                    {
                        case 100:
                        case -1:
                            endDisassembleProcess();
                            break;
                        default:
                            toolStripProgressBarMain.Value = sadBin.ProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadBin.ProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                    }
                    break;
                case ProcessType.Output:
                    if (sadBin.ProgressStatus > 100) sadBin.ProgressStatus = 100;
                    if (sadBin.ProgressStatus < -1) sadBin.ProgressStatus = -1;
                    switch (sadBin.ProgressStatus)
                    {
                        case 100:
                        case -1:
                            endOutputProcess();
                            break;
                        default:
                            toolStripProgressBarMain.Value = sadBin.ProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadBin.ProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                    }
                    break;
            }
        }

        private void attachPropertiesEvents()
        {
            Control.ControlCollection controls = null;

            controls = (Control.ControlCollection)elemMainSplitContainer.Panel1.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)s6xPropertiesTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemScalarPropertiesTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemFunctionPropertiesTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemTablePropertiesTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemStructurePropertiesTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemRoutineTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemOpeTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemRegisterTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemOtherTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemSignatureTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = (Control.ControlCollection)elemElemSignatureTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
        }

        private void attachPropertiesEventsControls(ref Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control.GetType().Name)
                {
                    case "TextBox":
                        ((TextBox)control).ModifiedChanged += new EventHandler(elemProperties_Modified);
                        ((TextBox)control).KeyDown += new KeyEventHandler(elemProperties_TextBox_KeyDown);
                        break;
                    case "CheckBox":
                        ((CheckBox)control).Click += new EventHandler(elemProperties_Modified);
                        break;
                    case "ComboBox":
                        ((ComboBox)control).SelectionChangeCommitted += new EventHandler(elemProperties_Modified);
                        break;
                    case "NumericUpDown":
                        ((NumericUpDown)control).ValueChanged += new EventHandler(elemProperties_Modified);
                        break;
                    case "PictureBox":
                        // For Tips only for now
                        if (control.Tag != null && (control.Name.Contains("tip") || control.Name.Contains("Tip")))
                        {
                            ((PictureBox)control).MouseHover += new EventHandler(TipPictureBox_MouseHover);
                            ((PictureBox)control).Click += new EventHandler(TipPictureBox_Click);
                        }
                        break;
                }
            }
        }

        private void elemProperties_TextBox_KeyDown(object sender, KeyEventArgs e)
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

        private void resetPropertiesModifiedStatus(TabPage tabPage)
        {
            Control.ControlCollection controls = null;

            controls = (Control.ControlCollection)elemMainSplitContainer.Panel1.Controls;
            resetPropertiesModifiedStatusControls(ref controls);
            controls = (Control.ControlCollection)tabPage.Controls;
            resetPropertiesModifiedStatusControls(ref controls);

            dirtyProperties = false;
        }

        private void resetPropertiesModifiedStatusControls(ref Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control.GetType().Name)
                {
                    case "TextBox":
                        ((TextBox)control).Modified = false;
                        break;
                    case "Button":
                        switch (control.Name)
                        {
                            case "functionBackButton":
                            case "scalarBitFlagsButton":
                            case "regBitFlagsButton":
                            case "routineAdvButton":
                            case "signatureAdvButton":
                            case "elementSignatureElemButton":
                            case "tableColsScalerButton":
                            case "tableRowsScalerButton":
                                ((Button)control).Enabled = true;
                                break;
                            default:
                                ((Button)control).Enabled = false;
                                break;
                        }
                        break;
                }
            }
        }

        private void elemProperties_Modified(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(TextBox))
            {
                if (((TextBox)sender).Modified) dirtyProperties = true;
            }
            else
            {
                dirtyProperties = true;
            }

            foreach (TabPage tabPage in elemTabControl.TabPages)
            {
                switch (tabPage.Name)
                {
                    case "s6xPropertiesTabPage":
                        s6xPropertiesUpdateButton.Enabled = true;
                        s6xPropertiesResetButton.Enabled = true;
                        return;
                    case "elemTablePropertiesTabPage":
                        tableUpdateButton.Enabled = true;
                        tableResetButton.Enabled = true;
                        return;
                    case "elemFunctionPropertiesTabPage":
                        functionUpdateButton.Enabled = true;
                        functionResetButton.Enabled = true;
                        functionBackButton.Enabled = false;
                        return;
                    case "elemScalarPropertiesTabPage":
                        scalarUpdateButton.Enabled = true;
                        scalarResetButton.Enabled = true;
                        return;
                    case "elemStructurePropertiesTabPage":
                        structureUpdateButton.Enabled = true;
                        structureResetButton.Enabled = true;
                        return;
                    case "elemRoutineTabPage":
                        routineUpdateButton.Enabled = true;
                        routineResetButton.Enabled = true;
                        return;
                    case "elemOpeTabPage":
                        opeUpdateButton.Enabled = true;
                        opeResetButton.Enabled = true;
                        return;
                    case "elemRegisterTabPage":
                        regUpdateButton.Enabled = true;
                        regResetButton.Enabled = true;
                        return;
                    case "elemOtherTabPage":
                        otherUpdateButton.Enabled = true;
                        otherResetButton.Enabled = true;
                        return;
                    case "elemSignatureTabPage":
                        signatureUpdateButton.Enabled = true;
                        signatureResetButton.Enabled = true;
                        return;
                    case "elemElemSignatureTabPage":
                        elementSignatureUpdateButton.Enabled = true;
                        elementSignatureResetButton.Enabled = true;
                        return;
                }
            }
        }
        
        private void initForm()
        {
            initFormMenu();
            initFormStatus();
            initFormElems();
            
            sadBin = null;
            sadS6x = null;

            alClipBoardTempUniqueAddresses = new ArrayList();

            dirtyProperties = false;
            lastElemTreeNode = null;
            nextElemTreeNode = null;
        }

        private void initFormMenu()
        {
            disassemblyToolStripMenuItem.Enabled = false;
            outputToolStripMenuItem.Enabled = false;
        }

        private void initFormStatus()
        {
            toolStripProgressBarMain.Visible = false;
            analysis1ToolStripStatusLabel.Text = string.Empty;
            analysis2ToolStripStatusLabel.Text = string.Empty;
            analysis3ToolStripStatusLabel.Text = string.Empty;
            analysis4ToolStripStatusLabel.Text = string.Empty;
        }

        private void initFormElems()
        {
            mainPanel.Visible = false;

            initFormElem();
        }

        private void initFormElem()
        {
            elemPanel.Visible = false;

            elemScalarPropertiesTabPage.Text = "Properties";
            elemFunctionPropertiesTabPage.Text = "Properties";
            elemTablePropertiesTabPage.Text = "Properties";
            elemStructurePropertiesTabPage.Text = "Properties";
            elemRoutineTabPage.Text = "Properties";
            elemOpeTabPage.Text = "Properties";
            elemRegisterTabPage.Text = "Properties";
            elemOtherTabPage.Text = "Properties";
            elemSignatureTabPage.Text = "Properties";
            elemElemSignatureTabPage.Text = "Properties";

            showPropertiesTabPage(string.Empty);
            if (elemTabControl.TabPages.Contains(elemOpsTabPage)) elemTabControl.TabPages.Remove(elemOpsTabPage);
            if (elemTabControl.TabPages.Contains(s6xPropertiesTabPage)) elemTabControl.TabPages.Remove(s6xPropertiesTabPage);
        }

        private void initRecentFiles()
        {
            recentToolStripMenuItem.DropDownItems.Clear();

            string rfsFilePath = Application.StartupPath + "\\" + SADDef.recentFilesFileName;
            
            recentFiles = (RecentFiles)ToolsXml.DeserializeFile(rfsFilePath, typeof(RecentFiles));
            if (recentFiles == null) recentFiles = new RecentFiles();
            recentFiles.rfsFilePath = rfsFilePath;

            foreach (RecentFile rfItem in recentFiles.Items)
            {
                ToolStripMenuItem tsItem = new ToolStripMenuItem(rfItem.Label);
                tsItem.Tag = rfItem;
                tsItem.ToolTipText = rfItem.Details;
                tsItem.Click += new EventHandler(recentFileToolStripMenuItem_Click);
                recentToolStripMenuItem.DropDownItems.Add(tsItem);

                if (recentToolStripMenuItem.DropDownItems.Count >= 10) break;
            }
        }

        private void initRepositories()
        {
            repoRegisters = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameRegisters, typeof(Repository));
            if (repoRegisters == null) repoRegisters = new Repository();

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
        }

        private void initRepositoryConversion()
        {
            repoConversion = (RepositoryConversion)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameConversion, typeof(RepositoryConversion));
            if (repoConversion == null) repoConversion = new RepositoryConversion();

            convertToolStripMenuItem.DropDownItems.Clear();
            convertInputToolStripMenuItem.DropDownItems.Clear();

            SortedList slConversions = new SortedList();
            foreach (RepositoryConversionItem repoItem in repoConversion.Items)
            {
                if (!slConversions.ContainsKey(repoItem.Title)) slConversions.Add(repoItem.Title, repoItem);
            }

            foreach (RepositoryConversionItem repoItem in slConversions.Values)
            {
                ToolStripMenuItem tsMI = new ToolStripMenuItem();
                tsMI.Tag = repoItem;
                tsMI.Text = repoItem.Title;
                tsMI.ToolTipText = string.Empty;
                if (repoItem.InternalFormula != null && repoItem.InternalFormula != string.Empty) tsMI.ToolTipText += repoItem.InternalFormula + "\r\n\r\n";
                if (repoItem.Comments != null && repoItem.Comments != string.Empty) tsMI.ToolTipText += repoItem.Comments + "\r\n\r\n";
                if (repoItem.Information != null && repoItem.Information != string.Empty) tsMI.ToolTipText += repoItem.Information + "\r\n\r\n";
                convertToolStripMenuItem.DropDownItems.Add(tsMI);
                ToolStripMenuItem tsMI2 = new ToolStripMenuItem();
                tsMI2.Tag = tsMI.Tag;
                tsMI2.Text = tsMI.Text;
                tsMI2.ToolTipText = tsMI.ToolTipText;
                convertInputToolStripMenuItem.DropDownItems.Add(tsMI2);
            }

            slConversions = null;

        }

        private void selectBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectBinary(string.Empty, string.Empty);
        }

        private void selectBinary(string bFilePath, string sFilePath)
        {
            if (sadS6x != null)
            {
                if (!sadS6x.isSaved)
                {
                    if (MessageBox.Show("S6x file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;

                    return;
                }
            }

            if (bFilePath == string.Empty)
            {
                if (openFileDialogBin.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogBin.FileName)) return;
                binaryFilePath = openFileDialogBin.FileName;
            }
            else
            {
                if (!File.Exists(bFilePath)) return;
                binaryFilePath = bFilePath;
            }
            s6xFilePath = sFilePath;

            startLoadProcess();
        }

        private void selectS6xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectS6x(string.Empty);
        }

        private void selectS6x(string sFilePath)
        {
            if (sadS6x != null)
            {
                if (!sadS6x.isSaved)
                {
                    if (MessageBox.Show("S6x file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }

            if (sFilePath == string.Empty)
            {
                if (openFileDialogS6x.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogS6x.FileName)) return;
                s6xFilePath = openFileDialogS6x.FileName;
            }
            else
            {
                if (!File.Exists(sFilePath)) return;
                s6xFilePath = sFilePath;
            }

            startLoadProcess();
        }

        private void recentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecentFile rfItem = (RecentFile)((ToolStripMenuItem)sender).Tag;
            if (rfItem == null) return;

            string bFilePath = rfItem.binPath + "\\" + rfItem.binFileName;
            string sFilePath = string.Empty;
            if (rfItem.s6xFileName != null || rfItem.s6xFileName != string.Empty) sFilePath = rfItem.s6xPath + "\\" + rfItem.s6xFileName;
            rfItem = null;

            if (!File.Exists(bFilePath)) return;
            if (!File.Exists(sFilePath)) sFilePath = string.Empty;

            selectBinary(bFilePath, sFilePath);
        }

        private void saveS6xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadS6x == null) return;

            if (sadS6x.isSaved) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            if (alClipBoardTempUniqueAddresses.Count > 0)
            {
                MessageBox.Show("S6x Definition contains invalid addresses.\r\nPlease update them before saving.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // S6xBanksProperties Mngt
            SortedList slBanksInfos = null;
            try { slBanksInfos = sadBin.Calibration.Info.slBanksInfos; }
            catch {slBanksInfos = null;}
            sadS6x.ResetBanksProperties(ref slBanksInfos);
            slBanksInfos = null;

            bool newS6xFile = !File.Exists(sadS6x.FilePath);

            if (!sadS6x.Save())
            {
                MessageBox.Show("Saving S6x Definition has failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (sadBin != null) sadBin.S6x = sadS6x;

            ShowElementsTreeRefresh();

            outputToolStripMenuItem.Enabled = false;

            if (sadBin != null)
            {
                disassemblyToolStripMenuItem.Enabled = (sadBin.isLoaded && sadBin.isValid && sadS6x.isSaved);
            }

            if (sadBin != null && newS6xFile && sadS6x.isSaved)
            {
                recentFiles.Update(new RecentFile(sadBin.BinaryFilePath, sadS6x.FilePath));
                recentFiles.Save();
                initRecentFiles();
            }
        }

        private void textOutputSelectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogTextOutput.ShowDialog() != DialogResult.OK) return;

            textOutputSetFilePath(openFileDialogTextOutput.FileName);
        }

        private void textOutputSetFilePath(string filePath)
        {
            textOutputFilePath = filePath;
            textOutputToolStripTextBox.Text = textOutputFilePath.Substring(textOutputFilePath.LastIndexOf('\\') + 1);
        }

        private void disassembleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startDisassembleProcess();
        }

        private void textOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startOutputProcess();
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(documentationFilePath)) return;

            Process.Start(documentationFilePath);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sAbout = string.Empty;
            sAbout = Application.ProductName + " " + Application.ProductVersion + "\t\t\tby Pym\n\r\n\r";
            sAbout += "Purpose:\n\r";
            sAbout += "\t- To disassemble 8061/8065 roms\n\r";
            sAbout += "\t- To do it automatically or semi-automatically\n\r";
            sAbout += "\t- To generate disassembly outputs in multiple formats\n\r";
            sAbout += "\n\rInformation:\n\r";
            sAbout += "\t- Disassembly can generate conflicts on Operations and Elements.";
            sAbout += " They will appear in output and error messages, to be managed manually.";
            sAbout += " For Operations they are essentially related with embedded arguments in calls.";
            sAbout += " For Elements they are essentially related to multiple type usage.\n\r";
            sAbout += "\n\rKnown issues:\n\r";
            sAbout += "\t- To be discovered\n\r";
            sAbout += "\n\rThanks to Andy (tvrfan) for SAD, software used as template for initial output, to Mark Mansur for TunerPro, which permits to continue working generated data.\n\r";
            sAbout += "\t\t\t\t\tPym\n\r";
            sAbout += "\t\t\t\t\t" + Properties.Settings.Default.VersionDate;


            MessageBox.Show(sAbout, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void startLoadProcess()
        {
            processPreviousCursor = Cursor;

            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            initForm();

            toolStripProgressBarMain.Visible = true;

            selectBinaryToolStripMenuItem.Enabled = false;
            selectS6xToolStripMenuItem.Enabled = false;

            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
            analysis4ToolStripStatusLabel.Text = getWaitStatus(string.Empty);

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.Load;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(startLoadAllProcess);
            processThread.Start();
        }

        private void startLoadAllProcess()
        {
            if (binaryFilePath != string.Empty)
            {
                if (s6xFilePath == string.Empty)
                {
                    FileInfo fiFI = new FileInfo(binaryFilePath);
                    s6xFilePath = fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".s6x";
                    fiFI = null;
                }

                sadBin = new SADBin(binaryFilePath, s6xFilePath);
                sadS6x = sadBin.S6x;
            }
        }
        
        private void endLoadProcess()
        {
            processThread = null;
            processRunning = false;
            processType = ProcessType.None;

            selectBinaryToolStripMenuItem.Enabled = true;
            selectS6xToolStripMenuItem.Enabled = true;

            s6xToolStripTextBox.Text = sadS6x.FileName;
            
            analysis1ToolStripStatusLabel.Text = getLoadStatus1();

            analysis4ToolStripStatusLabel.Font = new Font(FontFamily.GenericMonospace, analysis4ToolStripStatusLabel.Font.Size);
            analysis4ToolStripStatusLabel.Text = getLoadStatus4();

            if (sadBin != null)
            {
                if (sadS6x.Properties.Label == null || sadS6x.Properties.Label == string.Empty)
                {
                    sadS6x.Properties.Label = sadS6x.FileName;
                    if (sadBin.Calibration.Info.VidStrategy != null && sadBin.Calibration.Info.VidStrategy != string.Empty) sadS6x.Properties.Label = sadBin.Calibration.Info.VidStrategy;
                }
                if (sadS6x.Properties.XdfBaseOffset == null || sadS6x.Properties.XdfBaseOffset == string.Empty)
                {

                    sadS6x.Properties.XdfBaseOffset = string.Format("{0:x4}", SADDef.EecBankStartAddress);
                    sadS6x.Properties.XdfBaseOffsetSubtract = true;

                    if (sadBin.isValid)
                    {
                        if (sadBin.Calibration.BankAddressBinInt >= SADDef.EecBankStartAddress)
                        {
                            sadS6x.Properties.XdfBaseOffset = string.Format("{0:x1}", sadBin.Calibration.BankAddressBinInt - SADDef.EecBankStartAddress);
                            sadS6x.Properties.XdfBaseOffsetSubtract = false;
                        }
                        else
                        {
                            sadS6x.Properties.XdfBaseOffset = string.Format("{0:x1}", sadBin.Calibration.BankAddressBinInt + SADDef.EecBankStartAddress);
                            sadS6x.Properties.XdfBaseOffsetSubtract = true;
                        }
                    }
                }

                analysis2ToolStripStatusLabel.Font = new Font(FontFamily.GenericMonospace, analysis2ToolStripStatusLabel.Font.Size);
                analysis2ToolStripStatusLabel.Text = getLoadStatus2();
                analysis3ToolStripStatusLabel.Font = new Font(FontFamily.GenericMonospace, analysis3ToolStripStatusLabel.Font.Size);
                analysis3ToolStripStatusLabel.Text = getLoadStatus3();

                //Recent Files Update
                recentFiles.Update(new RecentFile(binaryFilePath, sadS6x.FilePath));
                recentFiles.Save();
                initRecentFiles();
                
                if (sadBin.isValid)
                {
                    disassemblyToolStripMenuItem.Enabled = true;

                    FileInfo fiFI = new FileInfo(binaryFilePath);
                    textOutputSetFilePath(fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".txt");
                    fiFI = null;
                }

                this.Text = getApplicationTitle();

                ShowElementsTreeLoad();
            }

            toolStripProgressBarMain.Value = 100;

            Cursor = processPreviousCursor;

            GC.Collect();

            // AutoStart
            if (autoDisassembly || autoOutput)
            {
                autoDisassembly = false;
                startDisassembleProcess();
            }
        }

        private void startDisassembleProcess()
        {
            if (sadBin == null) return;

            processPreviousCursor = Cursor;

            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            disassemblyToolStripMenuItem.Enabled = false;
            outputToolStripMenuItem.Enabled = false;
            selectBinaryToolStripMenuItem.Enabled = false;
            selectS6xToolStripMenuItem.Enabled = false;

            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
            analysis4ToolStripStatusLabel.Text = getWaitStatus(string.Empty);

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.Disassemble;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(sadBin.processBin);
            processThread.Start();
        }
        
        private void endDisassembleProcess()
        {
            processThread = null;
            processRunning = false;
            processType = ProcessType.None;
            
            analysis4ToolStripStatusLabel.Text = getDisassemblyStatus();

            toolStripProgressBarMain.Value = 100;

            Cursor = processPreviousCursor;

            if (sadBin.ProgressStatus == -1)
            {
                sadBin.ProgressStatus = 100;
                if (sadBin.Errors != null) MessageBox.Show("Process has failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ShowElementsTreeLoadS6x();
            }

            selectBinaryToolStripMenuItem.Enabled = true;
            selectS6xToolStripMenuItem.Enabled = true;

            disassemblyToolStripMenuItem.Enabled = true;
            outputToolStripMenuItem.Enabled = sadBin.isDisassembled;

            GC.Collect();

            // AutoStart
            if (autoOutput)
            {
                autoOutput = false;
                startOutputProcess();
            }
        }

        private void startOutputProcess()
        {
            if (sadBin == null) return;
            
            processPreviousCursor = Cursor;

            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            disassemblyToolStripMenuItem.Enabled = false;
            outputToolStripMenuItem.Enabled = false;
            selectBinaryToolStripMenuItem.Enabled = false;
            selectS6xToolStripMenuItem.Enabled = false;

            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
            analysis4ToolStripStatusLabel.Text = getWaitStatus(string.Empty);

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.Output;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(startOutputTextProcess);
            processThread.Start();
        }

        private void startOutputTextProcess()
        {
            sadBin.ProgressStatus = 0;
            sadBin.ProgressLabel = string.Empty;
            
            // Remap Calibration Objects with Current S6x
            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                if (s6xObject.Store && !s6xObject.Skip)
                {
                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(s6xObject.UniqueAddress))
                    {
                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xObject.UniqueAddress]).isTable)
                        {
                            ((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xObject.UniqueAddress]).TableElem.S6xTable = s6xObject;
                        }
                    }
                    else if (sadBin.Calibration.slExtTables.ContainsKey(s6xObject.UniqueAddress))
                    {
                        ((Table)sadBin.Calibration.slExtTables[s6xObject.UniqueAddress]).S6xTable = s6xObject;
                    }
                }
            }
            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                if (s6xObject.Store && !s6xObject.Skip)
                {
                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(s6xObject.UniqueAddress))
                    {
                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xObject.UniqueAddress]).isFunction)
                        {
                            ((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xObject.UniqueAddress]).FunctionElem.S6xFunction = s6xObject;
                        }
                    }
                    else if (sadBin.Calibration.slExtFunctions.ContainsKey(s6xObject.UniqueAddress))
                    {
                        ((Function)sadBin.Calibration.slExtFunctions[s6xObject.UniqueAddress]).S6xFunction = s6xObject;
                    }
                }
            }
            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                if (s6xObject.Store && !s6xObject.Skip)
                {
                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(s6xObject.UniqueAddress))
                    {
                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xObject.UniqueAddress]).isScalar)
                        {
                            ((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xObject.UniqueAddress]).ScalarElem.S6xScalar = s6xObject;
                        }
                    }
                    else if (sadBin.Calibration.slExtScalars.ContainsKey(s6xObject.UniqueAddress))
                    {
                        ((Scalar)sadBin.Calibration.slExtScalars[s6xObject.UniqueAddress]).S6xScalar = s6xObject;
                    }
                }
            }
            foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
            {
                if (s6xObject.Store && !s6xObject.Skip)
                {
                    if (sadBin.Calibration.slExtStructures.ContainsKey(s6xObject.UniqueAddress))
                    {
                        ((Structure)sadBin.Calibration.slExtStructures[s6xObject.UniqueAddress]).S6xStructure = s6xObject;
                    }
                }
            }
            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.Store && !s6xObject.Skip)
                {
                    if (sadBin.Calibration.slCalls.ContainsKey(s6xObject.UniqueAddress))
                    {
                        ((Call)sadBin.Calibration.slCalls[s6xObject.UniqueAddress]).S6xRoutine = s6xObject;
                    }
                }
            }
            
            // Processing
            SettingsLst toSettings = (SettingsLst)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.settingsTextOuputFileName, typeof(SettingsLst));
            if (toSettings == null) toSettings = new SettingsLst();
            ToolsSettings.Update(ref toSettings, "TEXTOUTPUT");

            bool applySettings = toSettings.Get<bool>("ACTIVE");

            if (applySettings)
            {
                OutputTextV outputText = new OutputTextV(ref sadBin, textOutputFilePath, ref toSettings);
                outputText.processOutputText();
            }
            else
            {
                OutputText outputText = new OutputText(ref sadBin, textOutputFilePath, ref toSettings);
                outputText.processOutputText();
            }

            //sadBin.processOutputText(textOutputFilePath, Application.StartupPath + "\\" + SADDef.settingsTextOuputFileName);
        }
        
        private void endOutputProcess()
        {
            processThread = null;
            processRunning = false;
            processType = ProcessType.None;

            analysis4ToolStripStatusLabel.Text = getOutputStatus();

            toolStripProgressBarMain.Value = 100;

            Cursor = processPreviousCursor;

            if (sadBin.ProgressStatus == -1)
            {
                sadBin.ProgressStatus = 100;
                if (sadBin.Errors != null) MessageBox.Show("Process has failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            disassemblyToolStripMenuItem.Enabled = true;
            outputToolStripMenuItem.Enabled = true;
            selectBinaryToolStripMenuItem.Enabled = true;
            selectS6xToolStripMenuItem.Enabled = true;

            textOutputToolStripTextBox.Visible = true;
        }

        private void startFolderProcess(string folderPath)
        {
            TextWriter txWriter = null;
            string logPath = string.Empty;

            try
            {
                logPath = folderPath + "\\" + Application.ProductName + DateTime.Now.ToString(".yyyyMMdd.HHmmss") + ".txt";
                txWriter = new StreamWriter(logPath, false, Encoding.UTF8);
                txWriter.WriteLine(Application.ProductName + " - Folder Process (*.bin files) on folder : " + folderPath);
                txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Starting.");

                string[] binFiles = Directory.GetFiles(folderPath, "*.bin", SearchOption.TopDirectoryOnly);

                for (int iBinFile = 0; iBinFile < binFiles.Length; iBinFile++)
                {
                    try
                    {
                        txWriter.WriteLine("Processing Binary file : " + binFiles[iBinFile]);
                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Starting.");

                        // Load
                        binaryFilePath = binFiles[iBinFile];
                        s6xFilePath = string.Empty;

                        startLoadAllProcess();

                        GC.Collect();

                        if (sadBin == null)
                        {
                            txWriter.WriteLine("\tLoading Binary file has failed.");
                            continue;
                        }

                        if (!sadBin.isValid)
                        {
                            txWriter.WriteLine("\tBinary file is invalid.");
                            continue;
                        }

                        if (sadS6x.Properties.Label == null || sadS6x.Properties.Label == string.Empty)
                        {
                            sadS6x.Properties.Label = sadS6x.FileName;
                            if (sadBin.Calibration.Info.VidStrategy != string.Empty) sadS6x.Properties.Label = sadBin.Calibration.Info.VidStrategy;
                        }
                        if (sadS6x.Properties.XdfBaseOffset == null || sadS6x.Properties.XdfBaseOffset == string.Empty)
                        {

                            sadS6x.Properties.XdfBaseOffset = string.Format("{0:x4}", SADDef.EecBankStartAddress);
                            sadS6x.Properties.XdfBaseOffsetSubtract = true;

                            if (sadBin.Calibration.BankAddressBinInt >= SADDef.EecBankStartAddress)
                            {
                                sadS6x.Properties.XdfBaseOffset = string.Format("{0:x1}", sadBin.Calibration.BankAddressBinInt - SADDef.EecBankStartAddress);
                                sadS6x.Properties.XdfBaseOffsetSubtract = false;
                            }
                            else
                            {
                                sadS6x.Properties.XdfBaseOffset = string.Format("{0:x1}", sadBin.Calibration.BankAddressBinInt + SADDef.EecBankStartAddress);
                                sadS6x.Properties.XdfBaseOffsetSubtract = true;
                            }
                        }

                        FileInfo fiFI = new FileInfo(binaryFilePath);
                        textOutputSetFilePath(fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".txt");
                        fiFI = null;

                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Loaded.");
                        if (sadBin.Calibration.Info.VidStrategy != string.Empty) txWriter.WriteLine("\t\tStrategy " + sadBin.Calibration.Info.VidStrategy + "(" + sadBin.Calibration.Info.VidStrategyVersion + ")");
                        if (sadBin.Calibration.Info.VidSerial != string.Empty && sadBin.Calibration.Info.VidSerial.Length > 5) txWriter.WriteLine("\t\tPart Number " + sadBin.Calibration.Info.VidSerial.Substring(0, 4) + "-12A650-" + sadBin.Calibration.Info.VidSerial.Substring(4));

                        //Disassembly
                        sadBin.processBin();

                        GC.Collect();

                        if (!sadBin.isDisassembled)
                        {
                            txWriter.WriteLine("\tDisassembly has failed.");
                            continue;
                        }

                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Disassembled.");
                        if (sadBin.Errors != null)
                        {
                            if (sadBin.Errors.Length > 0)
                            {
                                txWriter.WriteLine("\t\tDisassembly Errors(" + sadBin.Errors.Length + ") :");
                                txWriter.WriteLine("\t\t\t" + string.Join("\r\n\t\t\t", sadBin.Errors));
                            }
                        }

                        //Output
                        startOutputTextProcess();

                        GC.Collect();

                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Output done.");
                        if (sadBin.Errors != null)
                        {
                            if (sadBin.Errors.Length > 0)
                            {
                                txWriter.WriteLine("\t\tOutput Errors(" + sadBin.Errors.Length + ") :");
                                txWriter.WriteLine("\t\t\t" + string.Join("\r\n\t\t\t", sadBin.Errors));
                            }
                        }

                        txWriter.Flush();
                    }
                    catch (Exception ex)
                    {
                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Process has failed.");
                        txWriter.WriteLine("\t\t" + ex.Message);
                        txWriter.Flush();
                    }
                }
                txWriter.Flush();

                MessageBox.Show("Folder Process has ended.\r\n\r\nLogs are available in folder with start time.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Folder Process has failed.\r\n\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txWriter.Close();
                txWriter = null;
            }
        }

        private string getApplicationTitle()
        {
            string bseAppTitle = Application.ProductName;
            string addAppTitle = string.Empty;

            if (sadBin == null) return bseAppTitle;

            addAppTitle += sadBin.BinaryFileName;
            if (sadS6x.isValid && sadS6x.isSaved) addAppTitle += " / " + sadS6x.FileName;
            if (sadBin.isValid)
            {
                if (sadBin.Calibration.Info.VidStrategy != string.Empty) addAppTitle += " / " + sadBin.Calibration.Info.VidStrategy + "(" + sadBin.Calibration.Info.VidStrategyVersion + ")";
            }

            return string.Format("{0} ({1})", bseAppTitle, addAppTitle);
        }

        private string getLoadStatus1()
        {
            string status = string.Empty;

            if (sadBin == null)
            {
                status += "No Binary File";
            }
            else if (sadBin.isValid)
            {
                status += sadBin.BinaryFileName + "\r\n";
                if (sadBin.Calibration.Info.is8061) status += "8061";
                else status += "8065";
                if (sadBin.isCorrupted) status += "!";
                if (sadBin.Calibration.Info.isEarly) status += "β";
                if (sadBin.Calibration.Info.isPilot) status += "α";
                status += " Binary - " + sadBin.BinaryFileSize.ToString() + " Bytes";

                if (sadBin.Calibration.Info.VidStrategy != string.Empty) status += "\r\n" + sadBin.Calibration.Info.VidStrategy + "(" + sadBin.Calibration.Info.VidStrategyVersion + ") Strategy";
            }
            else
            {
                status += sadBin.BinaryFileName + "\r\n";
                status += "Unrecognized Binary - " + sadBin.BinaryFileSize.ToString() + " Bytes";
            }

            status += "\r\n\r\n";
            
            if (sadS6x.isValid) status += sadS6x.FileName;
            else status += "No SAD 806x File";

            return status;
        }

        private void analysis1ToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;
            if (!sadBin.isValid) return;

            string status = string.Empty;

            status += sadBin.BinaryFileName + "\r\n\r\n";
            if (sadBin.Calibration.Info.is8061) status += "EEC IV - 8061";
            else status += "EEC V - 8065";
            if (sadBin.isCorrupted) status += " Corrupted";
            if (sadBin.Calibration.Info.isEarly) status += " Early";
            if (sadBin.Calibration.Info.isPilot) status += " Pilot";
            status += " Binary - " + sadBin.BinaryFileSize.ToString() + " Bytes";
            if (sadBin.Calibration.Info.VidStrategy != string.Empty) status += "\r\nStrategy : " + sadBin.Calibration.Info.VidStrategy + "(" + sadBin.Calibration.Info.VidStrategyVersion + ")";
            if (sadBin.Calibration.Info.VidSerial != string.Empty) status += "\r\nPart Number : " + sadBin.Calibration.Info.VidSerial;
            if (sadBin.Calibration.Info.VidVIN != string.Empty) status += "\r\nVIN Code : " + sadBin.Calibration.Info.VidVIN;
            if (!sadBin.Calibration.Info.is8061)
            {
                if (sadBin.Calibration.Info.VidPatsCode != string.Empty) status += "\r\nPATS Code : " + sadBin.Calibration.Info.VidPatsCode;
                if (sadBin.Calibration.Info.VidRevMile > 0) status += "\r\nTyre Revolutions per Mile : " + sadBin.Calibration.Info.VidRevMile.ToString();
                if (sadBin.Calibration.Info.VidRtAxle > 0) status += "\r\nRear End Gear Ratio : " + sadBin.Calibration.Info.VidRtAxle.ToString();
                if (sadBin.Calibration.Info.VidEnabled) status += "\r\nVID Block is Enabled.";
                else status += "\r\nVID Block is Disabled.";
            }
            if (sadBin.Calibration.Info.VidCopyright != string.Empty) status += "\r\n\r\n" + sadBin.Calibration.Info.VidCopyright.Trim();

            MessageBox.Show(status, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string getLoadStatus2()
        {
            string status = string.Empty;

            if (sadBin == null) return status;

            status = "Banks :";
            foreach (string[] bankInfos in sadBin.Calibration.Info.slBanksInfos.Values) status += string.Format("\r\n{0,4}{1,8}{2,3}{3,6}", bankInfos[0], bankInfos[1], "=>", bankInfos[2]);

            return status;
        }

        private string getLoadStatus3()
        {
            string status = string.Empty;
            string col1 = string.Empty;
            string col2 = string.Empty;

            if (sadBin == null) return status;

            if (!sadBin.isValid) return status;

            status = "RBases :";
            int iCount = 0;
            foreach (RBase rBase in sadBin.Calibration.slRbases.Values)
            {
                if (iCount % 2 == 0)
                {
                    col1 = string.Format("{0,2} ({1,4})", rBase.Code, rBase.AddressBank);
                }
                else 
                {
                    col2 = string.Format("{0,2} ({1,4})", rBase.Code, rBase.AddressBank);
                    status += string.Format("\r\n{0,11}, {1,8}", col1, col2);
                    col1 = string.Empty;
                    col2 = string.Empty;
                }
                iCount++;
            }
            if (col1 != string.Empty) status += string.Format("\r\n{0,10}", col1);

            return status;
        }

        private string getLoadStatus4()
        {
            string status = string.Empty;

            if (sadBin == null) return status;

            if (!sadBin.isValid) return status;

            if (sadBin.Calibration.Info.isCheckSumConfirmed)
            {
                if (sadBin.Calibration.Info.isCheckSumValid) status += string.Format("{0,-22}\r\n", "CheckSum is valid");
                else status += string.Format("{0,-22}\r\n", "CheckSum is invalid");
            }
            else
            {
                status += string.Format("{0,-22}\r\n", "CheckSum was not calculated");
            }

            status += string.Format("{0,-18}{1:x4}\r\n", "CheckSum", sadBin.Calibration.Info.CheckSum);
            status += string.Format("{0,-18}{1:x4}\r\n", "SMP Base Address", sadBin.Calibration.Info.SmpBaseAddress);
            status += string.Format("{0,-18}{1:x4}\r\n", "CC Exe Time", sadBin.Calibration.Info.CcExeTime);
            // Not available for Early 8061
            if (!sadBin.Calibration.Info.is8061 || !sadBin.Calibration.Info.isEarly)
            {
                status += string.Format("{0,-18}{1,4}\r\n", "Levels Number", sadBin.Calibration.Info.LevelsNum);
                status += string.Format("{0,-18}{1,4}", "Calibs Number", sadBin.Calibration.Info.CalibsNum);
            }

            return status;
        }

        private void analysis4ToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;

            if (sadBin.isDisassembled)
            {
                if (sadBin.Errors != null)
                {
                    if (sadBin.Errors.Length <= 10)
                    {
                        MessageBox.Show(sadBin.Errors.Length.ToString() + " Error(s)\r\n\r\n" + string.Join("\r\n", sadBin.Errors), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(sadBin.Errors.Length.ToString() + " Error(s)\r\n\r\n" + string.Join("\r\n", sadBin.Errors, 0, 10) + "\r\n...", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (sadBin.isLoaded)
            {
                if (!sadBin.Calibration.Info.isCheckSumValid && sadBin.Calibration.Info.isCheckSumConfirmed)
                {
                    // Only when calculation has been confirmed, otherwise 99% chance to get a bad proposal
                    MessageBox.Show("Correct CheckSum should be " + string.Format("{0:x4}", sadBin.Calibration.Info.correctedChecksum) + ", " + Tools.LsbFirst(string.Format("{0:x4}", sadBin.Calibration.Info.correctedChecksum)) + " in binary.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private string getWaitStatus(string progressLabel)
        {
            string status = string.Empty;

            if (progressLabel != string.Empty) status += progressLabel + "\r\n";
            status += "Please wait...";

            return status;
        }

        private string getDisassemblyStatus()
        {
            string status = string.Empty;

            if (sadBin == null) return status;

            status += sadBin.ProgressLabel + "\r\n";
            status += sadBin.DisassemblyTimeSeconds.ToString() + " seconds.";

            return status;
        }

        private string getOutputStatus()
        {
            string status = string.Empty;

            if (sadBin == null) return status;

            status += sadBin.ProgressLabel;

            return status;
        }

        private bool isDuplicate(TreeNode tnNode)
        {
            if (tnNode.Parent == null) return false;
            if (tnNode.Parent.Parent == null) return false;
            if (tnNode.Parent.Parent.Parent == null) return true;
            return false;
        }
        
        private string getCategName(TreeNode tnNode)
        {
            if (tnNode.Parent == null) return tnNode.Name;
            if (tnNode.Parent.Parent == null) return tnNode.Parent.Name;
            if (tnNode.Parent.Parent.Parent == null) return tnNode.Parent.Parent.Name;
            return string.Empty;
        }

        private void setElementsTreeCategLabel(string categName)
        {
            string categLabel = string.Empty;

            if (!elemsTreeView.Nodes.ContainsKey(categName)) return;

            switch (categName)
            {
                case "RESERVED":
                    categLabel = "Reserved";
                    break;
                case "TABLES":
                    categLabel = "Tables";
                    break;
                case "FUNCTIONS":
                    categLabel = "Functions";
                    break;
                case "SCALARS":
                    categLabel = "Scalars";
                    break;
                case "STRUCTURES":
                    categLabel = "Structures";
                    break;
                case "ROUTINES":
                    categLabel = "Routines";
                    break;
                case "OPERATIONS":
                    categLabel = "Operations";
                    break;
                case "REGISTERS":
                    categLabel = "Registers";
                    break;
                case "OTHER":
                    categLabel = "Other Addresses";
                    break;
                case "SIGNATURES":
                    categLabel = "Routines Signatures";
                    break;
                case "ELEMSSIGNATURES":
                    categLabel = "Elements Signatures";
                    break;
                default:
                    return;
            }

            elemsTreeView.Nodes[categName].Text = categLabel + " (" + elemsTreeView.Nodes[categName].Nodes.Count.ToString() + ")";
        }
        
        // Init Elements Tree to CleanUp and Create Parent Nodes
        private void ShowElementsTreeInit()
        {
            TreeNode tnNode = null;

            elemsTreeView.Nodes.Clear();

            tnNode = new TreeNode();
            tnNode.Name = "PROPERTIES";
            tnNode.Text = "Properties";
            tnNode.ToolTipText = "Properties";
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "RESERVED";
            tnNode.Text = "Reserved";
            tnNode.ToolTipText = "Reserved Elements";
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "TABLES";
            tnNode.Text = "Tables";
            tnNode.ToolTipText = "Tables";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "FUNCTIONS";
            tnNode.Text = "Functions";
            tnNode.ToolTipText = "Functions";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            tnNode.Tag = true;              // For Scalers Refresh
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "SCALARS";
            tnNode.Text = "Scalars";
            tnNode.ToolTipText = "Scalars";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "STRUCTURES";
            tnNode.Text = "Structures";
            tnNode.ToolTipText = "Structures";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "ROUTINES";
            tnNode.Text = "Routines";
            tnNode.ToolTipText = "Routines";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "OPERATIONS";
            tnNode.Text = "Operations";
            tnNode.ToolTipText = "Operations";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "REGISTERS";
            tnNode.Text = "Registers";
            tnNode.ToolTipText = "Registers";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "OTHER";
            tnNode.Text = "Other Addresses";
            tnNode.ToolTipText = "Other Addresses";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "SIGNATURES";
            tnNode.Text = "Routines Signatures";
            tnNode.ToolTipText = "Routines Signatures";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = new TreeNode();
            tnNode.Name = "ELEMSSIGNATURES";
            tnNode.Text = "Elements Signatures";
            tnNode.ToolTipText = "Elements Signatures";
            tnNode.ContextMenuStrip = categsContextMenuStrip;
            elemsTreeView.Nodes.Add(tnNode);

            tnNode = null;
        }

        // Automatic S6xObjects Creation without Store Property and without link to parent object
        private void showElementsTreeRemapS6x()
        {
            if (sadBin == null) return;
            if (sadBin.Calibration == null) return;

            // Automatic S6xObjects Creation without Store Property and without link to parent object
            //      Some Elements are created automatically before Disassembly
            foreach (CalibrationElement calElem in sadBin.Calibration.slCalibrationElements.Values)
            {
                if (calElem.isTable)
                {
                    S6xTable s6xObject = (S6xTable)sadS6x.slTables[calElem.TableElem.UniqueAddress];
                    if (s6xObject != null)
                    {
                        // Skipped and stored Element is moved to Duplicates before being overwritten
                        if (s6xObject.Skip && s6xObject.Store)
                        {
                            S6xTable s6xDup = s6xObject;
                            s6xDup.DuplicateNum = 1;
                            while (sadS6x.slDupTables.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                            sadS6x.slDupTables.Add(s6xDup.DuplicateAddress, s6xDup);
                            s6xObject = null;
                        }
                        else if (s6xObject.Store)
                        {
                            calElem.TableElem.S6xTable = s6xObject;
                        }
                        else
                        {
                            s6xObject = null;
                        }
                        if (s6xObject == null)
                        {
                            sadS6x.slTables[calElem.TableElem.UniqueAddress] = (calElem.TableElem.S6xTable == null) ? new S6xTable(calElem) : calElem.TableElem.S6xTable;
                        }
                    }
                    else
                    {
                        sadS6x.slTables.Add(calElem.TableElem.UniqueAddress, (calElem.TableElem.S6xTable == null) ? new S6xTable(calElem) : calElem.TableElem.S6xTable);
                    }
                }
                else if (calElem.isFunction)
                {
                    S6xFunction s6xObject = (S6xFunction)sadS6x.slFunctions[calElem.FunctionElem.UniqueAddress];
                    if (s6xObject != null)
                    {
                        // Skipped and stored Element is moved to Duplicates before being overwritten
                        if (s6xObject.Skip && s6xObject.Store)
                        {
                            S6xFunction s6xDup = s6xObject;
                            s6xDup.DuplicateNum = 1;
                            while (sadS6x.slDupFunctions.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                            sadS6x.slDupFunctions.Add(s6xDup.DuplicateAddress, s6xDup);
                            s6xObject = null;
                        }
                        else if (s6xObject.Store)
                        {
                            calElem.FunctionElem.S6xFunction = s6xObject;
                        }
                        else
                        {
                            s6xObject = null;
                        }
                        if (s6xObject == null)
                        {
                            sadS6x.slFunctions[calElem.FunctionElem.UniqueAddress] = (calElem.FunctionElem.S6xFunction == null) ? new S6xFunction(calElem) : calElem.FunctionElem.S6xFunction;
                        }
                    }
                    else
                    {
                        sadS6x.slFunctions.Add(calElem.FunctionElem.UniqueAddress, (calElem.FunctionElem.S6xFunction == null) ? new S6xFunction(calElem) : calElem.FunctionElem.S6xFunction);
                    }
                }
                else if (calElem.isScalar)
                {
                    S6xScalar s6xObject = (S6xScalar)sadS6x.slScalars[calElem.ScalarElem.UniqueAddress];
                    if (s6xObject != null)
                    {
                        // Skipped and stored Element is moved to Duplicates before being overwritten
                        if (s6xObject.Skip && s6xObject.Store)
                        {
                            S6xScalar s6xDup = s6xObject;
                            s6xDup.DuplicateNum = 1;
                            while (sadS6x.slDupScalars.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                            sadS6x.slDupScalars.Add(s6xDup.DuplicateAddress, s6xDup);
                            s6xObject = null;
                        }
                        else if (s6xObject.Store)
                        {
                            calElem.ScalarElem.S6xScalar = s6xObject;
                        }
                        else
                        {
                            s6xObject = null;
                        }
                        if (s6xObject == null)
                        {
                            sadS6x.slScalars[calElem.ScalarElem.UniqueAddress] = (calElem.ScalarElem.S6xScalar == null) ? new S6xScalar(calElem) : calElem.ScalarElem.S6xScalar;
                        }
                    }
                    else
                    {
                        sadS6x.slScalars.Add(calElem.ScalarElem.UniqueAddress, (calElem.ScalarElem.S6xScalar == null) ? new S6xScalar(calElem) : calElem.ScalarElem.S6xScalar);
                    }
                }
                else if (calElem.isStructure)
                {
                    if (calElem.StructureElem.ParentStructure == null)  // Included / Duplicated elements are not generated
                    {
                        S6xStructure s6xObject = (S6xStructure)sadS6x.slStructures[calElem.StructureElem.UniqueAddress];
                        if (s6xObject != null)
                        {
                            // Skipped and stored Element is moved to Duplicates before being overwritten
                            if (s6xObject.Skip && s6xObject.Store)
                            {
                                S6xStructure s6xDup = s6xObject;
                                s6xDup.DuplicateNum = 1;
                                while (sadS6x.slDupStructures.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                                sadS6x.slDupStructures.Add(s6xDup.DuplicateAddress, s6xDup);
                                s6xObject = null;
                            }
                            else if (s6xObject.Store)
                            {
                                calElem.StructureElem.S6xStructure = s6xObject;
                            }
                            else
                            {
                                s6xObject = null;
                            }
                            if (s6xObject == null)
                            {
                                sadS6x.slStructures[calElem.StructureElem.UniqueAddress] = (calElem.StructureElem.S6xStructure == null) ? new S6xStructure(calElem) : calElem.StructureElem.S6xStructure;
                            }
                        }
                        else
                        {
                            sadS6x.slStructures.Add(calElem.StructureElem.UniqueAddress, (calElem.StructureElem.S6xStructure == null) ? new S6xStructure(calElem) : calElem.StructureElem.S6xStructure);
                        }
                    }
                }
            }

            foreach (Table extObject in sadBin.Calibration.slExtTables.Values)
            {
                int bankBinAddress = sadBin.getBankBinAddress(extObject.BankNum);
                if (bankBinAddress < 0) continue;
                S6xTable s6xObject = (S6xTable)sadS6x.slTables[extObject.UniqueAddress];
                if (s6xObject != null)
                {
                    // Skipped and stored Element is moved to Duplicates before being overwritten
                    if (s6xObject.Skip && s6xObject.Store)
                    {
                        S6xTable s6xDup = (S6xTable)sadS6x.slTables[extObject.UniqueAddress];
                        s6xDup.DuplicateNum = 1;
                        while (sadS6x.slDupTables.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                        sadS6x.slDupTables.Add(s6xDup.DuplicateAddress, s6xDup);
                        s6xObject = null;
                    }
                    else if (s6xObject.Store)
                    {
                        extObject.S6xTable = s6xObject;
                    }
                    else
                    {
                        s6xObject = null;
                    }
                    if (s6xObject == null)
                    {
                        sadS6x.slTables[extObject.UniqueAddress] = (extObject.S6xTable == null) ? new S6xTable(extObject, bankBinAddress) : extObject.S6xTable;
                    }
                }
                else
                {
                    sadS6x.slTables.Add(extObject.UniqueAddress, (extObject.S6xTable == null) ? new S6xTable(extObject, bankBinAddress) : extObject.S6xTable);
                }
            }

            foreach (Function extObject in sadBin.Calibration.slExtFunctions.Values)
            {
                int bankBinAddress = sadBin.getBankBinAddress(extObject.BankNum);
                if (bankBinAddress < 0) continue;
                S6xFunction s6xObject = (S6xFunction)sadS6x.slFunctions[extObject.UniqueAddress];
                if (s6xObject != null)
                {
                    // Skipped and stored Element is moved to Duplicates before being overwritten
                    if (s6xObject.Skip && s6xObject.Store)
                    {
                        S6xFunction s6xDup = s6xObject;
                        s6xDup.DuplicateNum = 1;
                        while (sadS6x.slDupFunctions.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                        sadS6x.slDupFunctions.Add(s6xDup.DuplicateAddress, s6xDup);
                        s6xObject = null;
                    }
                    else if (s6xObject.Store)
                    {
                        extObject.S6xFunction = s6xObject;
                    }
                    else
                    {
                        s6xObject = null;
                    }
                    if (s6xObject == null)
                    {
                        sadS6x.slFunctions[extObject.UniqueAddress] = (extObject.S6xFunction == null) ? new S6xFunction(extObject, bankBinAddress) : extObject.S6xFunction;
                    }
                }
                else
                {
                    sadS6x.slFunctions.Add(extObject.UniqueAddress, (extObject.S6xFunction == null) ? new S6xFunction(extObject, bankBinAddress) : extObject.S6xFunction);
                }
            }

            foreach (Scalar extObject in sadBin.Calibration.slExtScalars.Values)
            {
                int bankBinAddress = sadBin.getBankBinAddress(extObject.BankNum);
                if (bankBinAddress < 0) continue;
                S6xScalar s6xObject = (S6xScalar)sadS6x.slScalars[extObject.UniqueAddress];
                if (s6xObject != null)
                {
                    // Skipped and stored Element is moved to Duplicates before being overwritten
                    if (s6xObject.Skip && s6xObject.Store)
                    {
                        S6xScalar s6xDup = s6xObject;
                        s6xDup.DuplicateNum = 1;
                        while (sadS6x.slDupScalars.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                        sadS6x.slDupScalars.Add(s6xDup.DuplicateAddress, s6xDup);
                        s6xObject = null;
                    }
                    else if (s6xObject.Store)
                    {
                        extObject.S6xScalar = s6xObject;
                    }
                    else
                    {
                        s6xObject = null;
                    }
                    if (s6xObject == null)
                    {
                        sadS6x.slScalars[extObject.UniqueAddress] = (extObject.S6xScalar == null) ? new S6xScalar(extObject, bankBinAddress) : extObject.S6xScalar;
                    }
                }
                else
                {
                    sadS6x.slScalars.Add(extObject.UniqueAddress, (extObject.S6xScalar == null) ? new S6xScalar(extObject, bankBinAddress) : extObject.S6xScalar);
                }
            }

            foreach (Structure sStruct in sadBin.Calibration.slExtStructures.Values)
            {
                if (sStruct.ParentStructure == null)    // Included / Duplicated elements are not generated
                {
                    S6xStructure s6xObject = (S6xStructure)sadS6x.slStructures[sStruct.UniqueAddress];
                    if (s6xObject != null)
                    {
                        // Skipped and stored Element is moved to Duplicates before being overwritten
                        if (s6xObject.Skip && s6xObject.Store)
                        {
                            S6xStructure s6xDup = s6xObject;
                            s6xDup.DuplicateNum = 1;
                            while (sadS6x.slDupStructures.ContainsKey(s6xDup.DuplicateAddress)) s6xDup.DuplicateNum++;
                            sadS6x.slDupStructures.Add(s6xDup.DuplicateAddress, s6xDup);
                            s6xObject = null;
                        }
                        else if (s6xObject.Store)
                        {
                            sStruct.S6xStructure = s6xObject;
                        }
                        else
                        {
                            s6xObject = null;
                        }
                        if (s6xObject == null)
                        {
                            sadS6x.slStructures[sStruct.UniqueAddress] = (sStruct.S6xStructure == null) ? new S6xStructure(sStruct) : sStruct.S6xStructure;
                        }
                    }
                    else
                    {
                        sadS6x.slStructures.Add(sStruct.UniqueAddress, (sStruct.S6xStructure == null) ? new S6xStructure(sStruct) : sStruct.S6xStructure);
                    }
                }
            }

            foreach (string callAddress in sadBin.Calibration.alMainCallsUniqueAddresses)
            {
                Call cCall = (Call)sadBin.Calibration.slCalls[callAddress];
                Routine rRoutine = null;
                if (cCall.isRoutine) rRoutine = (Routine)sadBin.Calibration.slRoutines[cCall.UniqueAddress];
                S6xRoutine s6xObject = (S6xRoutine)sadS6x.slRoutines[cCall.UniqueAddress];
                if (s6xObject != null)
                {
                    if (s6xObject.Skip || !s6xObject.Store)
                    {
                        sadS6x.slRoutines[cCall.UniqueAddress] = (cCall.S6xRoutine == null) ? new S6xRoutine(cCall, rRoutine) : cCall.S6xRoutine;
                    }
                    else 
                    {
                        cCall.S6xRoutine = s6xObject;
                    }
                }
                else
                {
                    sadS6x.slRoutines.Add(cCall.UniqueAddress, (cCall.S6xRoutine == null) ? new S6xRoutine(cCall, rRoutine) : cCall.S6xRoutine);
                }
                rRoutine = null;
                cCall = null;
            }
        }
        
        private void ShowElementsTreeLoad()
        {
            ShowElementsTreeInit();

            // Reserved Addresses filled in at first load only
            ShowElementsTreeLoadReserved();

            // Load S6xObjects (Automatic S6xObjects Creation Included)
            ShowElementsTreeLoadS6x();
        }

        // Reserved Addresses filled in at first load only
        private void ShowElementsTreeLoadReserved()
        {
            TreeNode tnReserved = null;
            TreeNode tnNode = null;

            if (sadBin == null) return;

            bool bIgnore = false;

            // Reserved Addresses filled in at load only
            tnReserved = elemsTreeView.Nodes["RESERVED"];
            tnReserved.Nodes.Clear();

            if (sadBin.Bank8 != null)
            {
                foreach (ReservedAddress resAdr in sadBin.Bank8.slReserved.Values)
                {
                    bIgnore = false;
                    tnNode = new TreeNode();
                    tnNode.Name = resAdr.UniqueAddress;
                    tnNode.Text = resAdr.Label;
                    switch (resAdr.Type)
                    {
                        case ReservedAddressType.IntVector:
                            foreach (Vector vect in sadBin.Bank8.slIntVectors.Values)
                            {
                                if (vect.UniqueSourceAddress == resAdr.UniqueAddress)
                                {
                                    tnNode.Text = vect.Label;
                                    tnNode.ToolTipText = vect.UniqueAddressHex + "\r\n" + vect.ShortLabel + "\r\n" + vect.Comments;
                                    break;
                                }
                            }
                            break;
                        case ReservedAddressType.CalPointer:
                            foreach (RBase rBase in sadBin.Calibration.slRbases.Values)
                            {
                                if (rBase.AddressBankInt == resAdr.ValueInt - SADDef.EecBankStartAddress)
                                {
                                    tnNode.Text = SADDef.ShortRegisterPrefix + rBase.Code.ToLower();
                                    tnNode.ToolTipText = rBase.AddressBank;
                                    break;
                                }
                            }
                            break;
                        case ReservedAddressType.RomSize:
                            if (resAdr.ValueInt == 0xffff) tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\nNot Defined (ffff)\r\n" + resAdr.Comments;
                            else tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            break;
                        case ReservedAddressType.Ascii:
                        case ReservedAddressType.Hex:
                            tnNode.ToolTipText = resAdr.ValueString;
                            break;
                        case ReservedAddressType.Fill:
                            bIgnore = true;
                            break;
                        case ReservedAddressType.Word:
                        case ReservedAddressType.Byte:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(10) + "\r\n" + resAdr.Comments;
                            break;
                        case ReservedAddressType.CheckSum:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            tnNode.ContextMenuStrip = elemsContextMenuStrip;
                            break;
                        default:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            break;
                    }

                    // VID Block Specificity
                    if (sadBin.Bank8.Num == sadBin.Calibration.Info.VidBankNum)
                    {
                        object[] vidDef = null;
                        
                        if (sadBin.Calibration.Info.is8061) vidDef = SADDef.Info_8061_VID_Block_Addresses;
                        else vidDef = SADDef.Info_8065_VID_Block_Addresses;

                        foreach (object[] vidPart in vidDef) if ((int)vidPart[0] == resAdr.AddressInt) tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    }
                    
                    if (!bIgnore) tnReserved.Nodes.Add(tnNode);
                    tnNode = null;
                }
            }
            if (sadBin.Bank1 != null)
            {
                foreach (ReservedAddress resAdr in sadBin.Bank1.slReserved.Values)
                {
                    bIgnore = false;
                    tnNode = new TreeNode();
                    tnNode.Name = resAdr.UniqueAddress;
                    tnNode.Text = resAdr.Label;
                    switch (resAdr.Type)
                    {
                        case ReservedAddressType.IntVector:
                            foreach (Vector vect in sadBin.Bank1.slIntVectors.Values)
                            {
                                if (vect.UniqueSourceAddress == resAdr.UniqueAddress)
                                {
                                    tnNode.Text = vect.Label;
                                    tnNode.ToolTipText = vect.UniqueAddressHex + "\r\n" + vect.ShortLabel + "\r\n" + vect.Comments;
                                    break;
                                }
                            }
                            break;
                        case ReservedAddressType.Ascii:
                        case ReservedAddressType.Hex:
                            tnNode.ToolTipText = resAdr.ValueString;
                            break;
                        case ReservedAddressType.Fill:
                            bIgnore = true;
                            break;
                        case ReservedAddressType.Word:
                        case ReservedAddressType.Byte:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(10) + "\r\n" + resAdr.Comments;
                            break;
                        case ReservedAddressType.CheckSum:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            tnNode.ContextMenuStrip = elemsContextMenuStrip;
                            break;
                        default:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            break;
                    }

                    // VID Block Specificity
                    if (sadBin.Bank1.Num == sadBin.Calibration.Info.VidBankNum)
                    {
                        object[] vidDef = null;

                        if (sadBin.Calibration.Info.is8061) vidDef = SADDef.Info_8061_VID_Block_Addresses;
                        else vidDef = SADDef.Info_8065_VID_Block_Addresses;

                        foreach (object[] vidPart in vidDef) if ((int)vidPart[0] == resAdr.AddressInt) tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    }

                    if (!bIgnore) tnReserved.Nodes.Add(tnNode);
                    tnNode = null;
                }
            }
            if (sadBin.Bank9 != null)
            {
                foreach (ReservedAddress resAdr in sadBin.Bank9.slReserved.Values)
                {
                    bIgnore = false;
                    tnNode = new TreeNode();
                    tnNode.Name = resAdr.UniqueAddress;
                    tnNode.Text = resAdr.Label;
                    switch (resAdr.Type)
                    {
                        case ReservedAddressType.IntVector:
                            foreach (Vector vect in sadBin.Bank9.slIntVectors.Values)
                            {
                                if (vect.UniqueSourceAddress == resAdr.UniqueAddress)
                                {
                                    tnNode.Text = vect.Label;
                                    tnNode.ToolTipText = vect.UniqueAddressHex + "\r\n" + vect.ShortLabel + "\r\n" + vect.Comments;
                                    break;
                                }
                            }
                            break;
                        case ReservedAddressType.Ascii:
                        case ReservedAddressType.Hex:
                            tnNode.ToolTipText = resAdr.ValueString;
                            break;
                        case ReservedAddressType.Fill:
                            bIgnore = true;
                            break;
                        case ReservedAddressType.Word:
                        case ReservedAddressType.Byte:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(10) + "\r\n" + resAdr.Comments;
                            break;
                        case ReservedAddressType.CheckSum:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            tnNode.ContextMenuStrip = elemsContextMenuStrip;
                            break;
                        default:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            break;
                    }

                    // VID Block Specificity
                    if (sadBin.Bank9.Num == sadBin.Calibration.Info.VidBankNum)
                    {
                        object[] vidDef = null;

                        if (sadBin.Calibration.Info.is8061) vidDef = SADDef.Info_8061_VID_Block_Addresses;
                        else vidDef = SADDef.Info_8065_VID_Block_Addresses;

                        foreach (object[] vidPart in vidDef) if ((int)vidPart[0] == resAdr.AddressInt) tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    }
                    
                    if (!bIgnore) tnReserved.Nodes.Add(tnNode);
                    tnNode = null;
                }
            }
            if (sadBin.Bank0 != null)
            {
                foreach (ReservedAddress resAdr in sadBin.Bank0.slReserved.Values)
                {
                    bIgnore = false;
                    tnNode = new TreeNode();
                    tnNode.Name = resAdr.UniqueAddress;
                    tnNode.Text = resAdr.Label;
                    switch (resAdr.Type)
                    {
                        case ReservedAddressType.IntVector:
                            foreach (Vector vect in sadBin.Bank0.slIntVectors.Values)
                            {
                                if (vect.UniqueSourceAddress == resAdr.UniqueAddress)
                                {
                                    tnNode.Text = vect.Label;
                                    tnNode.ToolTipText = vect.UniqueAddressHex + "\r\n" + vect.ShortLabel + "\r\n" + vect.Comments;
                                    break;
                                }
                            }
                            break;
                        case ReservedAddressType.Ascii:
                        case ReservedAddressType.Hex:
                            tnNode.ToolTipText = resAdr.ValueString;
                            break;
                        case ReservedAddressType.Fill:
                            bIgnore = true;
                            break;
                        case ReservedAddressType.Word:
                        case ReservedAddressType.Byte:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(10) + "\r\n" + resAdr.Comments;
                            break;
                        case ReservedAddressType.CheckSum:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            tnNode.ContextMenuStrip = elemsContextMenuStrip;
                            break;
                        default:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            break;
                    }

                    // VID Block Specificity
                    if (sadBin.Bank0.Num == sadBin.Calibration.Info.VidBankNum)
                    {
                        object[] vidDef = null;

                        if (sadBin.Calibration.Info.is8061) vidDef = SADDef.Info_8061_VID_Block_Addresses;
                        else vidDef = SADDef.Info_8065_VID_Block_Addresses;

                        foreach (object[] vidPart in vidDef) if ((int)vidPart[0] == resAdr.AddressInt) tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    }

                    if (!bIgnore) tnReserved.Nodes.Add(tnNode);
                    tnNode = null;
                }
            }

            setElementsTreeCategLabel(tnReserved.Name);
        }

        private void ShowElementsTreeLoadS6x()
        {
            TreeNode tnNode = null;

            // Automatic S6xObjects Creation without Store Property and without link to parent object
            showElementsTreeRemapS6x();

            elemsTreeView.Nodes["TABLES"].Nodes.Clear();
            foreach (S6xTable s6xTable in sadS6x.slTables.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xTable.UniqueAddress;
                tnNode.Text = s6xTable.Label;
                tnNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["TABLES"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("TABLES");
            // Duplicates
            foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
            {
                if (!elemsTreeView.Nodes["TABLES"].Nodes.ContainsKey(s6xTable.UniqueAddress)) continue;
                if (elemsTreeView.Nodes["TABLES"].Nodes[s6xTable.UniqueAddress].Nodes.ContainsKey(s6xTable.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xTable.DuplicateAddress;
                tnNode.Text = s6xTable.Label;
                tnNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["TABLES"].Nodes[s6xTable.UniqueAddress].Nodes.Add(tnNode);
                tnNode = null;
            }

            elemsTreeView.Nodes["FUNCTIONS"].Nodes.Clear();
            foreach (S6xFunction s6xFunction in sadS6x.slFunctions.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xFunction.UniqueAddress;
                tnNode.Text = s6xFunction.Label;
                tnNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["FUNCTIONS"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("FUNCTIONS");
            // Duplicates
            foreach (S6xFunction s6xFunction in sadS6x.slDupFunctions.Values)
            {
                if (!elemsTreeView.Nodes["FUNCTIONS"].Nodes.ContainsKey(s6xFunction.UniqueAddress)) continue;
                if (elemsTreeView.Nodes["FUNCTIONS"].Nodes[s6xFunction.UniqueAddress].Nodes.ContainsKey(s6xFunction.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xFunction.DuplicateAddress;
                tnNode.Text = s6xFunction.Label;
                tnNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["FUNCTIONS"].Nodes[s6xFunction.UniqueAddress].Nodes.Add(tnNode);
                tnNode = null;
            }
            elemsTreeView.Nodes["FUNCTIONS"].Tag = true;              // For Scalers Refresh

            elemsTreeView.Nodes["SCALARS"].Nodes.Clear();
            foreach (S6xScalar s6xScalar in sadS6x.slScalars.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xScalar.UniqueAddress;
                tnNode.Text = s6xScalar.Label;
                tnNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["SCALARS"].Nodes.Add(tnNode);
                tnNode = null;
            }
            // Duplicates
            foreach (S6xScalar s6xScalar in sadS6x.slDupScalars.Values)
            {
                if (!elemsTreeView.Nodes["SCALARS"].Nodes.ContainsKey(s6xScalar.UniqueAddress)) continue;
                if (elemsTreeView.Nodes["SCALARS"].Nodes[s6xScalar.UniqueAddress].Nodes.ContainsKey(s6xScalar.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xScalar.DuplicateAddress;
                tnNode.Text = s6xScalar.Label;
                tnNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["SCALARS"].Nodes[s6xScalar.UniqueAddress].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("SCALARS");

            elemsTreeView.Nodes["STRUCTURES"].Nodes.Clear();
            foreach (S6xStructure s6xStructure in sadS6x.slStructures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xStructure.UniqueAddress;
                tnNode.Text = s6xStructure.Label;
                tnNode.ToolTipText = s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["STRUCTURES"].Nodes.Add(tnNode);
                tnNode = null;
            }
            // Duplicates
            foreach (S6xStructure s6xStructure in sadS6x.slDupStructures.Values)
            {
                if (!elemsTreeView.Nodes["STRUCTURES"].Nodes.ContainsKey(s6xStructure.UniqueAddress)) continue;
                if (elemsTreeView.Nodes["STRUCTURES"].Nodes[s6xStructure.UniqueAddress].Nodes.ContainsKey(s6xStructure.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xStructure.DuplicateAddress;
                tnNode.Text = s6xStructure.Label;
                tnNode.ToolTipText = s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["STRUCTURES"].Nodes[s6xStructure.UniqueAddress].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("STRUCTURES");

            elemsTreeView.Nodes["ROUTINES"].Nodes.Clear();
            foreach (S6xRoutine s6xRoutine in sadS6x.slRoutines.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xRoutine.UniqueAddress;
                tnNode.Text = s6xRoutine.Label;
                tnNode.ToolTipText = s6xRoutine.UniqueAddressHex + "\r\n" + s6xRoutine.ShortLabel + "\r\n\r\n" + s6xRoutine.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["ROUTINES"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("ROUTINES");

            elemsTreeView.Nodes["OPERATIONS"].Nodes.Clear();
            foreach (S6xOperation s6xOpe in sadS6x.slOperations.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xOpe.UniqueAddress;
                tnNode.Text = s6xOpe.Label;
                tnNode.ToolTipText = s6xOpe.UniqueAddressHex + "\r\n\r\n" + s6xOpe.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["OPERATIONS"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("OPERATIONS");

            elemsTreeView.Nodes["REGISTERS"].Nodes.Clear();
            foreach (S6xRegister s6xReg in sadS6x.slRegisters.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xReg.UniqueAddress;
                tnNode.Text = s6xReg.FullLabel;
                tnNode.ToolTipText = s6xReg.FullComments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["REGISTERS"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("REGISTERS");

            elemsTreeView.Nodes["OTHER"].Nodes.Clear();
            foreach (S6xOtherAddress s6xOth in sadS6x.slOtherAddresses.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xOth.UniqueAddress;
                tnNode.Text = s6xOth.Label;
                tnNode.ToolTipText = s6xOth.UniqueAddressHex + "\r\n\r\n" + s6xOth.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["OTHER"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("OTHER");

            elemsTreeView.Nodes["SIGNATURES"].Nodes.Clear();
            foreach (S6xSignature s6xSig in sadS6x.slSignatures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xSig.UniqueKey;
                tnNode.Text = s6xSig.Label;
                tnNode.ToolTipText = s6xSig.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["SIGNATURES"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("SIGNATURES");

            elemsTreeView.Nodes["ELEMSSIGNATURES"].Nodes.Clear();
            foreach (S6xElementSignature s6xESig in sadS6x.slElementsSignatures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xESig.UniqueKey;
                tnNode.Text = s6xESig.SignatureLabel;
                tnNode.ToolTipText = s6xESig.SignatureComments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                elemsTreeView.Nodes["ELEMSSIGNATURES"].Nodes.Add(tnNode);
                tnNode = null;
            }
            setElementsTreeCategLabel("ELEMSSIGNATURES");

            dirtyProperties = false;
            lastElemTreeNode = null;
            nextElemTreeNode = null;
            elemPanel.Visible = false;
            elemDataGridView.Visible = false;

            mainPanel.Visible = true;
        }

        private void ShowElementsTreeRefresh()
        {
            string[] refreshIgnoredCategs = new string[] {"PROPERTIES", "RESERVED"};

            foreach (TreeNode tnCateg in elemsTreeView.Nodes)
            {
                switch (tnCateg.Name)
                {
                    // No Refresh Required
                    case "PROPERTIES":
                    case "RESERVED":
                        tnCateg.ForeColor = elemsTreeView.ForeColor;
                        break;
                    default:
                        // Refresh Colors only with Parent one
                        foreach (TreeNode tnNode in tnCateg.Nodes)
                        {
                            if (tnNode.ForeColor != tnCateg.ForeColor) tnNode.ForeColor = tnCateg.ForeColor;
                            // Duplicates
                            foreach (TreeNode tnDup in tnNode.Nodes)
                            {
                                if (tnDup.ForeColor != tnCateg.ForeColor) tnDup.ForeColor = tnCateg.ForeColor;
                            }
                        }
                        break;
                }
            }

            dirtyProperties = false;
            lastElemTreeNode = null;
            nextElemTreeNode = null;
            elemsTreeView.SelectedNode = null;
            elemPanel.Visible = false;
            elemDataGridView.Visible = false;

            mainPanel.Visible = true;
        }

        private void categsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            pasteMultToolStripMenuItem.Visible = false;
            
            pasteToolStripMenuItem.Enabled = (Clipboard.ContainsData(SADDef.S6xClipboardFormat) || Clipboard.ContainsData(SADDef.XdfClipboardFormat));
            pasteMultToolStripMenuItem.Enabled = pasteToolStripMenuItem.Enabled;

            categCleanElements.Enabled = false;

            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent != null) return;

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "TABLES":
                    categCleanElements.Enabled = true;
                    break;
                case "FUNCTIONS":
                    categCleanElements.Enabled = true;
                    break;
                case "SCALARS":
                    categCleanElements.Enabled = true;
                    pasteMultToolStripMenuItem.Visible = true;
                    break;
            }
        }

        private void elemsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool bDuplicate = false;

            newOnItemToolStripMenuItem.Enabled = true;

            displayToolStripMenuItem.Enabled = true;
            renameToolStripMenuItem.Enabled = true;
            copyToolStripMenuItem.Enabled = true;

            copySigToolStripMenuItem.Visible = false;
            copyXdfToolStripMenuItem.Visible = false;
            
            showOperationsToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;

            pasteOnItemToolStripMenuItem.Enabled = (Clipboard.ContainsData(SADDef.S6xClipboardFormat) || Clipboard.ContainsData(SADDef.XdfClipboardFormat));
            pasteMultOnItemToolStripMenuItem.Enabled = false;
            pasteOverItemToolStripMenuItem.Enabled = (Clipboard.ContainsData(SADDef.S6xClipboardFormat) || Clipboard.ContainsData(SADDef.XdfClipboardFormat));

            pasteMultOnItemToolStripMenuItem.Visible = false;

            skipOnItemToolStripMenuItem.Enabled = true;

            duplicateToolStripMenuItem.Enabled = false;
            unDuplicateToolStripMenuItem.Enabled = false;
            
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;

            bDuplicate = isDuplicate(elemsTreeView.SelectedNode);

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "RESERVED":
                    // Specific Case for CheckSum and VID Block - It can be copied to Xdf
                    // Nothing else
                    copyXdfToolStripMenuItem.Visible = true;
                    newOnItemToolStripMenuItem.Enabled = false;
                    displayToolStripMenuItem.Enabled = false;
                    renameToolStripMenuItem.Enabled = false;
                    copyToolStripMenuItem.Enabled = false;
                    pasteOnItemToolStripMenuItem.Enabled = false;
                    pasteOverItemToolStripMenuItem.Enabled = false;
                    skipOnItemToolStripMenuItem.Enabled = false;
                    break;
                case "TABLES":
                case "FUNCTIONS":
                case "SCALARS":
                    pasteMultOnItemToolStripMenuItem.Enabled = pasteOnItemToolStripMenuItem.Enabled;
                    copyXdfToolStripMenuItem.Visible = true;
                    showOperationsToolStripMenuItem.Enabled = (sadBin != null && sadBin.isDisassembled);
                    break;
                case "STRUCTURES":
                case "ROUTINES":
                case "OPERATIONS":
                case "OTHER":
                    showOperationsToolStripMenuItem.Enabled = (sadBin != null && sadBin.isDisassembled);
                    break;
            }

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "RESERVED":
                    break;                
                case "TABLES":
                    unDuplicateToolStripMenuItem.Enabled = bDuplicate;
                    if (bDuplicate)
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slDupTables.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    else
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slTables.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    duplicateToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled;
                    break;
                case "FUNCTIONS":
                    unDuplicateToolStripMenuItem.Enabled = bDuplicate;
                    if (bDuplicate)
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slDupFunctions.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    else
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slFunctions.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    duplicateToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled;
                    break;
                case "SCALARS":
                    pasteMultOnItemToolStripMenuItem.Visible = true;
                    unDuplicateToolStripMenuItem.Enabled = bDuplicate;
                    if (bDuplicate)
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slDupScalars.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    else
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slScalars.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    duplicateToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled;
                    break;
                case "STRUCTURES":
                    unDuplicateToolStripMenuItem.Enabled = bDuplicate;
                    if (bDuplicate)
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slDupStructures.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    else
                    {
                        deleteToolStripMenuItem.Enabled = (sadS6x.slStructures.ContainsKey(elemsTreeView.SelectedNode.Name));
                    }
                    duplicateToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled;
                    break;
                case "ROUTINES":
                    deleteToolStripMenuItem.Enabled = (sadS6x.slRoutines.ContainsKey(elemsTreeView.SelectedNode.Name));
                    copySigToolStripMenuItem.Visible = true;
                    copySigToolStripMenuItem.Enabled = (sadBin != null && sadBin.isDisassembled);
                    break;
                default:
                    deleteToolStripMenuItem.Enabled = true;
                    break;
            }
        }

        private void elemContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool additionalConversion = (decimalToolStripMenuItem.Checked == true);
            bool forFunction = false;

            if (nextElemTreeNode != null) if (nextElemTreeNode.Parent != null) forFunction = (getCategName(nextElemTreeNode) == "FUNCTIONS");
            
            convertToolStripSeparator.Visible = additionalConversion;
            convertToolStripMenuItem.Visible = additionalConversion;

            convertInputToolStripMenuItem.Visible = forFunction;
        }

        private void elemsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void elemsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null && e.Node.Name == "PROPERTIES")
            {
                lastElemTreeNode = null;
                nextElemTreeNode = null;
                showProperties();
            }

            if (e.Node.Parent == null) return;
            
            switch (getCategName(e.Node))
            {
                case "RESERVED":
                    lastElemTreeNode = null;
                    nextElemTreeNode = null;
                    return;
                default:
                    break;
            }

            if (isDuplicate(e.Node)) elemsTreeView.Tag = e.Node.Parent.Parent;
            else elemsTreeView.Tag = e.Node.Parent;
            nextElemTreeNode = e.Node;
            showElem();
        }

        private void elemsTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                e.CancelEdit = true;
                return;
            }

            switch (getCategName(e.Node))
            {
                case "RESERVED":
                case "REGISTERS":
                case "OTHER":
                    e.CancelEdit = true;
                    break;
                default:
                    // To prevent removing element when editing it - Deactivation
                    shortCutsElementResetRemoveToolStripMenuItem.Enabled = false;
                    break;
            }
        }

        private void elemsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Parent == null) e.CancelEdit = true;
            if (e.Label == null) e.CancelEdit = true;
            if (e.Label == string.Empty) e.CancelEdit = true;
            if (!e.CancelEdit)
            {
                switch (getCategName(e.Node))
                {
                    case "RESERVED":
                    case "REGISTERS":
                    case "OTHER":
                        e.CancelEdit = true;
                        break;
                    default:
                        // To prevent removing element when editing it - Reactivation
                        shortCutsElementResetRemoveToolStripMenuItem.Enabled = true;
                        break;
                }
            }
            if (e.CancelEdit) return;

            renameElem(e.Node, e.Label);
        }

        private void newOnItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;

            newElement(elemsTreeView.SelectedNode.Parent);
        }

        private void newElemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent != null) return;

            newElement(elemsTreeView.SelectedNode);
        }

        private void newElement(TreeNode categNode)
        {
            if (categNode == null) return;
            
            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            nextElemTreeNode = null;
            elemsTreeView.Tag = categNode;

            hideElemData();

            elemBankTextBox.ReadOnly = false;
            elemAddressTextBox.ReadOnly = false;

            elemLabelTextBox.Text = "New Element";
            switch (categNode.Name)
            {
                case "TABLES":
                case "FUNCTIONS":
                case "SCALARS":
                    elemBankTextBox.Text = "8";
                    elemAddressTextBox.Text = string.Format("{0:x4}", SADDef.EecBankStartAddress);
                    if (sadBin != null)
                    {
                        if (sadBin.Calibration.BankNum != -1)
                        {
                            elemBankTextBox.Text = sadBin.Calibration.BankNum.ToString();
                            elemAddressTextBox.Text = sadBin.Calibration.AddressInternalEnd;
                        }
                    }
                    break;
                case "STRUCTURES":
                case "ROUTINES":
                case "OPERATIONS":
                case "OTHER":
                    elemBankTextBox.Text = "8";
                    elemAddressTextBox.Text = string.Format("{0:x4}", SADDef.EecBankStartAddress);
                    break;
                case "REGISTERS":
                    elemBankTextBox.Text = "R";
                    elemAddressTextBox.Text = string.Empty;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    break;
                case "SIGNATURES":
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = string.Empty;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    break;
                case "ELEMSSIGNATURES":
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = string.Empty;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    break;
            }

            switch (categNode.Name)
            {
                case "TABLES":
                    newElemTableProperties();
                    break;
                case "FUNCTIONS":
                    newElemFunctionProperties();
                    break;
                case "SCALARS":
                    newElemScalarProperties();
                    break;
                case "STRUCTURES":
                    newElemStructureProperties();
                    break;
                case "ROUTINES":
                    newElemRoutineProperties();
                    break;
                case "OPERATIONS":
                    newElemOperationProperties();
                    break;
                case "REGISTERS":
                    newElemRegisterProperties();
                    break;
                case "OTHER":
                    newElemOtherProperties();
                    break;
                case "SIGNATURES":
                    newElemSignatureProperties();
                    break;
                case "ELEMSSIGNATURES":
                    newElemElemSignatureProperties();
                    break;
            }

            showPropertiesTabPage(categNode.Name);
            elemPanel.Visible = true;
        }

        private void skipOnItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skipItem(true);
        }

        private void skipItem(bool skip)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            string uniqueAddress = elemsTreeView.SelectedNode.Name;

            if (isDuplicate(elemsTreeView.SelectedNode))
            {
                switch (getCategName(elemsTreeView.SelectedNode))
                {
                    case "TABLES":
                        if (!sadS6x.slDupTables.ContainsKey(uniqueAddress)) return;
                        ((S6xTable)sadS6x.slDupTables[uniqueAddress]).Skip = skip;
                        ((S6xTable)sadS6x.slDupTables[uniqueAddress]).Store = true;
                        break;
                    case "FUNCTIONS":
                        if (!sadS6x.slDupFunctions.ContainsKey(uniqueAddress)) return;
                        ((S6xFunction)sadS6x.slDupFunctions[uniqueAddress]).Skip = skip;
                        ((S6xFunction)sadS6x.slDupFunctions[uniqueAddress]).Store = true;
                        break;
                    case "SCALARS":
                        if (!sadS6x.slDupScalars.ContainsKey(uniqueAddress)) return;
                        ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).Skip = skip;
                        ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).Store = true;
                        break;
                    case "STRUCTURES":
                        if (!sadS6x.slDupStructures.ContainsKey(uniqueAddress)) return;
                        ((S6xStructure)sadS6x.slDupStructures[uniqueAddress]).Skip = skip;
                        ((S6xStructure)sadS6x.slDupStructures[uniqueAddress]).Store = true;
                        break;
                }
            }
            else
            {
                switch (getCategName(elemsTreeView.SelectedNode))
                {
                    case "TABLES":
                        if (!sadS6x.slTables.ContainsKey(uniqueAddress)) return;
                        ((S6xTable)sadS6x.slTables[uniqueAddress]).Skip = skip;
                        ((S6xTable)sadS6x.slTables[uniqueAddress]).Store = true;
                        break;
                    case "FUNCTIONS":
                        if (!sadS6x.slFunctions.ContainsKey(uniqueAddress)) return;
                        ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Skip = skip;
                        ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Store = true;
                        break;
                    case "SCALARS":
                        if (!sadS6x.slScalars.ContainsKey(uniqueAddress)) return;
                        ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Skip = skip;
                        ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Store = true;
                        break;
                    case "STRUCTURES":
                        if (!sadS6x.slStructures.ContainsKey(uniqueAddress)) return;
                        ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Skip = skip;
                        ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Store = true;
                        break;
                    case "ROUTINES":
                        if (!sadS6x.slRoutines.ContainsKey(uniqueAddress)) return;
                        ((S6xRoutine)sadS6x.slRoutines[uniqueAddress]).Skip = skip;
                        ((S6xRoutine)sadS6x.slRoutines[uniqueAddress]).Store = true;
                        break;
                    case "OPERATIONS":
                        if (!sadS6x.slOperations.ContainsKey(uniqueAddress)) return;
                        ((S6xOperation)sadS6x.slOperations[uniqueAddress]).Skip = skip;
                        break;
                    case "REGISTERS":
                        if (!sadS6x.slRegisters.ContainsKey(uniqueAddress)) return;
                        ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Skip = skip;
                        break;
                    case "OTHER":
                        if (!sadS6x.slOtherAddresses.ContainsKey(uniqueAddress)) return;
                        ((S6xOtherAddress)sadS6x.slOtherAddresses[uniqueAddress]).Skip = skip;
                        break;
                    case "SIGNATURES":
                        if (!sadS6x.slSignatures.ContainsKey(uniqueAddress)) return;
                        ((S6xSignature)sadS6x.slSignatures[uniqueAddress]).Skip = skip;
                        break;
                    case "ELEMSSIGNATURES":
                        if (!sadS6x.slElementsSignatures.ContainsKey(uniqueAddress)) return;
                        ((S6xElementSignature)sadS6x.slElementsSignatures[uniqueAddress]).Skip = skip;
                        break;
                    default:
                        return;
                }
            }

            sadS6x.isSaved = false;

            elemsTreeView.SelectedNode.ForeColor = Color.Purple;

            showElem();
        }

        private void categSkipAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skipAll(true);
        }

        private void categUnSkipAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skipAll(false);
        }

        private void skipAll(bool skip)
        {
            string uniqueAddress = string.Empty;

            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent != null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "TABLES":
                    foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (elemsTreeView.SelectedNode.Nodes[s6xObject.UniqueAddress].Nodes.ContainsKey(s6xObject.DuplicateAddress))
                            {
                                elemsTreeView.SelectedNode.Nodes[uniqueAddress].Nodes[s6xObject.DuplicateAddress].ForeColor = Color.Purple;
                            }
                        }
                        sadS6x.isSaved = false;
                    }
                    break;
                case "FUNCTIONS":
                    foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (elemsTreeView.SelectedNode.Nodes[s6xObject.UniqueAddress].Nodes.ContainsKey(s6xObject.DuplicateAddress))
                            {
                                elemsTreeView.SelectedNode.Nodes[uniqueAddress].Nodes[s6xObject.DuplicateAddress].ForeColor = Color.Purple;
                            }
                        }
                        sadS6x.isSaved = false;
                    }
                    break;
                case "SCALARS":
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (elemsTreeView.SelectedNode.Nodes[s6xObject.UniqueAddress].Nodes.ContainsKey(s6xObject.DuplicateAddress))
                            {
                                elemsTreeView.SelectedNode.Nodes[uniqueAddress].Nodes[s6xObject.DuplicateAddress].ForeColor = Color.Purple;
                            }
                        }
                        sadS6x.isSaved = false;
                    }
                    break;
                case "STRUCTURES":
                    foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xStructure s6xObject in sadS6x.slDupStructures.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (elemsTreeView.SelectedNode.Nodes[s6xObject.UniqueAddress].Nodes.ContainsKey(s6xObject.DuplicateAddress))
                            {
                                elemsTreeView.SelectedNode.Nodes[uniqueAddress].Nodes[s6xObject.DuplicateAddress].ForeColor = Color.Purple;
                            }
                        }
                        sadS6x.isSaved = false;
                    }
                    break;
                case "ROUTINES":
                    foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        s6xObject.Store = true;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case "OPERATIONS":
                    foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case "REGISTERS":
                    foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case "OTHER":
                    foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
                    {
                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case "SIGNATURES":
                    foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
                    {
                        uniqueAddress = s6xObject.UniqueKey;
                        s6xObject.Skip = skip;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case "ELEMSSIGNATURES":
                    foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
                    {
                        uniqueAddress = s6xObject.UniqueKey;
                        s6xObject.Skip = skip;
                        if (elemsTreeView.SelectedNode.Nodes.ContainsKey(uniqueAddress)) elemsTreeView.SelectedNode.Nodes[uniqueAddress].ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
            }

            showElem();
        }

        private void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;
            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "RESERVED":
                    nextElemTreeNode = null;
                    return;
            }

            nextElemTreeNode = elemsTreeView.SelectedNode;
            showElem();
        }
        
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;
            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "RESERVED":
                case "REGISTERS":
                case "OTHER":
                    elemsTreeView.SelectedNode = null;
                    return;
            }

            elemsTreeView.SelectedNode.BeginEdit();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "RESERVED":
                    elemsTreeView.SelectedNode = null;
                    return;
            }

            deleteElem(elemsTreeView.SelectedNode, false, false, true);
        }

        private void showOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation[] ops = null;
            bool forElem = false;
            bool calibElemOpe = false;
            int selectTextStart = -1;
            int selectTextEnd = -1;

            nextElemTreeNode = elemsTreeView.SelectedNode;

            if (nextElemTreeNode == null) return;

            if (nextElemTreeNode.Parent == null) return;
            
            if (sadBin == null) return;

            if (!sadBin.isDisassembled) return;

            switch (getCategName(nextElemTreeNode))
            {
                case "ROUTINES":
                case "OPERATIONS":
                case "OTHER":
                    forElem = false;
                    break;
                default:
                    forElem = true;
                    break;
            }

            if (isDuplicate(nextElemTreeNode))
            {
                ops = sadBin.getElementRelatedOps(nextElemTreeNode.Parent.Name, forElem);
            }
            else
            {
                ops = sadBin.getElementRelatedOps(nextElemTreeNode.Name, forElem);
            }

            // Second Run for OTHER, could be Element too
            if (getCategName(nextElemTreeNode) == "OTHER" && ops.Length == 0)
            {
                forElem = true;
                if (isDuplicate(nextElemTreeNode))
                {
                    ops = sadBin.getElementRelatedOps(nextElemTreeNode.Parent.Name, forElem);
                }
                else
                {
                    ops = sadBin.getElementRelatedOps(nextElemTreeNode.Name, forElem);
                }
            }

            elemOpsRichTextBox.Clear();
            if (!forElem)
            {
                selectTextStart = 0;
                elemOpsRichTextBox.AppendText(nextElemTreeNode.Text + ":\r\n");
                selectTextEnd = elemOpsRichTextBox.Text.Length;
            }
            foreach (Operation ope in ops)
            {
                calibElemOpe = false; 
                if (ope != null)
                {
                    if (forElem)
                    {
                        if (ope.alCalibrationElems != null)
                        {
                            foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                            {
                                if (opeCalElem.UniqueAddress == nextElemTreeNode.Name)
                                {
                                    calibElemOpe = true;
                                    elemOpsRichTextBox.AppendText("\r\n");
                                    selectTextStart = elemOpsRichTextBox.Text.Length - 1;
                                    break;
                                }
                            }
                        }
                        else if (ope.KnownElemAddress != string.Empty)
                        {
                            // No Check on Bank, Could be Apply On Bank or Not for Calibration Bank, standard Address is checked
                            if ((Convert.ToInt32(ope.KnownElemAddress, 16) - SADDef.EecBankStartAddress).ToString() == nextElemTreeNode.Name.Substring(nextElemTreeNode.Name.LastIndexOf(" ") + 1))
                            {
                                calibElemOpe = true;
                                elemOpsRichTextBox.AppendText("\r\n");
                                selectTextStart = elemOpsRichTextBox.Text.Length - 1;
                            }
                        }
                        else if (ope.OtherElemAddress != string.Empty)
                        {
                            // No Check on Bank, Could be Apply On Bank or Not for Calibration Bank, standard Address is checked
                            if ((Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress).ToString() == nextElemTreeNode.Name.Substring(nextElemTreeNode.Name.LastIndexOf(" ") + 1))
                            {
                                calibElemOpe = true;
                                elemOpsRichTextBox.AppendText("\r\n");
                                selectTextStart = elemOpsRichTextBox.Text.Length - 1;
                            }
                        }
                    }
                    elemOpsRichTextBox.AppendText(string.Format("{0,6}: {1,-21}{2,-25}{3,-21}\r\n", ope.UniqueAddressHex, ope.OriginalOp, ope.Instruction, ope.Translation1));
                    if (ope.CallArgsNum > 0)
                    {
                        elemOpsRichTextBox.AppendText(string.Format("{0,6}: {1,-46}{2,-21}\r\n", ope.UniqueCallArgsAddressHex, ope.CallArgs, "#args"));
                    }
                    if (calibElemOpe)
                    {
                        elemOpsRichTextBox.AppendText("\r\n");
                        selectTextEnd = elemOpsRichTextBox.Text.Length;
                    }
                }
            }
            ops = null;

            if (selectTextStart >= 0 && selectTextEnd >= 0)
            {
                try { elemOpsRichTextBox.Select(selectTextStart, selectTextEnd - selectTextStart); }
                catch { }
            }

            if (!elemTabControl.Contains(elemOpsTabPage)) elemTabControl.TabPages.Add(elemOpsTabPage);
            elemTabControl.SelectedTab = elemOpsTabPage;
        }

        private void showProperties()
        {
            TreeNode tnNode = null;

            if (sadS6x == null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            tnNode = elemsTreeView.Nodes["PROPERTIES"];

            elemLabelTextBox.Text = string.Format("{0,20}", tnNode.Text);
            elemLabelTextBox.ForeColor = tnNode.ForeColor;
            elemBankTextBox.Text = string.Empty;
            elemAddressTextBox.Text = string.Empty;
            elemBankTextBox.ReadOnly = true;
            elemAddressTextBox.ReadOnly = true;

            s6xPropertiesLabelTextBox.Text = sadS6x.Properties.Label;

            // Windows 10 1809 (10.0.17763) Issue
            s6xPropertiesCommentsTextBox.Clear();
            s6xPropertiesCommentsTextBox.Multiline = false;
            s6xPropertiesCommentsTextBox.Multiline = true;
            
            s6xPropertiesCommentsTextBox.Text = sadS6x.Properties.Comments;
            s6xPropertiesCommentsTextBox.Text = s6xPropertiesCommentsTextBox.Text.Replace("\n", "\r\n");
            s6xPropertiesNoNumberingCheckBox.Checked = sadS6x.Properties.NoNumbering;
            s6xPropertiesRegListOutputCheckBox.Checked = sadS6x.Properties.RegListOutput;
            s6xPropertiesXdfBaseOffsetTextBox.Text = sadS6x.Properties.XdfBaseOffset;
            s6xPropertiesXdfBaseOffsetCheckBox.Checked = sadS6x.Properties.XdfBaseOffsetSubtract;
            
            s6xPropertiesUpdateButton.Enabled = false;
            s6xPropertiesResetButton.Enabled = false;
            
            resetPropertiesModifiedStatus(s6xPropertiesTabPage);

            showPropertiesTabPage(tnNode.Name);

            lastElemTreeNode = tnNode;

            tnNode = null;

            elemPanel.Visible = true;
        }

        private void showElem()
        {
            if (nextElemTreeNode == null) return;

            if (nextElemTreeNode.Parent == null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }
            
            if (lastElemTreeNode != null)
            {
                if (lastElemTreeNode.Name != nextElemTreeNode.Name) elemOpsRichTextBox.Clear();
            }

            elemLabelTextBox.Text = string.Format("{0,20}", nextElemTreeNode.Text);
            elemLabelTextBox.ForeColor = nextElemTreeNode.ForeColor;
            elemBankTextBox.ReadOnly = false;
            elemAddressTextBox.ReadOnly = false;

            switch (getCategName(nextElemTreeNode))
            {
                case "TABLES":
                case "FUNCTIONS":
                case "SCALARS":
                    if (isDuplicate(nextElemTreeNode))
                    {
                        if (sadS6x.slDupTables.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xTable s6xTable = (S6xTable)sadS6x.slDupTables[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xTable.BankNum.ToString();
                            elemAddressTextBox.Text = s6xTable.Address;
                            s6xTable = null;
                        }
                        else if (sadS6x.slDupFunctions.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xFunction s6xFunction = (S6xFunction)sadS6x.slDupFunctions[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xFunction.BankNum.ToString();
                            elemAddressTextBox.Text = s6xFunction.Address;
                            s6xFunction = null;
                        }
                        else if (sadS6x.slDupScalars.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xScalar s6xScalar = (S6xScalar)sadS6x.slDupScalars[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xScalar.BankNum.ToString();
                            elemAddressTextBox.Text = s6xScalar.Address;
                            s6xScalar = null;
                        }
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xTable s6xTable = (S6xTable)sadS6x.slTables[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xTable.BankNum.ToString();
                            elemAddressTextBox.Text = s6xTable.Address;
                            s6xTable = null;
                        }
                        else if (sadS6x.slFunctions.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xFunction s6xFunction = (S6xFunction)sadS6x.slFunctions[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xFunction.BankNum.ToString();
                            elemAddressTextBox.Text = s6xFunction.Address;
                            s6xFunction = null;
                        }
                        else if (sadS6x.slScalars.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xScalar s6xScalar = (S6xScalar)sadS6x.slScalars[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xScalar.BankNum.ToString();
                            elemAddressTextBox.Text = s6xScalar.Address;
                            s6xScalar = null;
                        }
                    }

                    if (!checkElemBankAddress(false))
                    {
                        elemBankTextBox.ReadOnly = false;
                        elemAddressTextBox.ReadOnly = false;
                        elemBankTextBox.ForeColor = Color.Red;
                        elemAddressTextBox.ForeColor = Color.Red;
                        hideElemData();
                    }
                    else
                    {
                        elemBankTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        showElemData();
                    }
                    showElemProperties();
                    break;
                case "STRUCTURES":
                    if (isDuplicate(nextElemTreeNode))
                    {
                        if (sadS6x.slDupStructures.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xStructure s6xStructure = (S6xStructure)sadS6x.slDupStructures[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xStructure.BankNum.ToString();
                            elemAddressTextBox.Text = s6xStructure.Address;
                            s6xStructure = null;
                        }
                    }
                    else
                    {
                        if (sadS6x.slStructures.ContainsKey(nextElemTreeNode.Name))
                        {
                            S6xStructure s6xStructure = (S6xStructure)sadS6x.slStructures[nextElemTreeNode.Name];
                            elemBankTextBox.Text = s6xStructure.BankNum.ToString();
                            elemAddressTextBox.Text = s6xStructure.Address;
                            s6xStructure = null;
                        }
                    }

                    if (!checkElemBankAddress(false))
                    {
                        elemBankTextBox.ReadOnly = false;
                        elemAddressTextBox.ReadOnly = false;
                        elemBankTextBox.ForeColor = Color.Red;
                        elemAddressTextBox.ForeColor = Color.Red;
                        hideElemData();
                    }
                    else
                    {
                        elemBankTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        showElemData();
                    }
                    showElemProperties();
                    break;
                case "ROUTINES":
                    if (sadS6x.slRoutines.ContainsKey(nextElemTreeNode.Name))
                    {
                        S6xRoutine s6xRoutine = (S6xRoutine)sadS6x.slRoutines[nextElemTreeNode.Name];
                        elemBankTextBox.Text = s6xRoutine.BankNum.ToString();
                        elemAddressTextBox.Text = s6xRoutine.Address;
                        s6xRoutine = null;
                    }

                    if (!checkElemBankAddress(false))
                    {
                        elemBankTextBox.ReadOnly = false;
                        elemAddressTextBox.ReadOnly = false;
                        elemBankTextBox.ForeColor = Color.Red;
                        elemAddressTextBox.ForeColor = Color.Red;
                    }
                    else
                    {
                        elemBankTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemTreeNode.ForeColor;
                    }
                    hideElemData();
                    showElemProperties();
                    break;
                case "OPERATIONS":
                    S6xOperation ope = (S6xOperation)sadS6x.slOperations[nextElemTreeNode.Name];
                    elemBankTextBox.Text = ope.BankNum.ToString();
                    elemAddressTextBox.Text = ope.Address;
                    ope = null;

                    if (!checkElemBankAddress(false))
                    {
                        elemBankTextBox.ReadOnly = false;
                        elemAddressTextBox.ReadOnly = false;
                        elemBankTextBox.ForeColor = Color.Red;
                        elemAddressTextBox.ForeColor = Color.Red;
                    }
                    else
                    {
                        elemBankTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemTreeNode.ForeColor;
                    }
                    hideElemData();
                    showElemProperties();
                    break;
                case "REGISTERS":
                    S6xRegister reg = (S6xRegister)sadS6x.slRegisters[nextElemTreeNode.Name];
                    elemBankTextBox.Text = "R";
                    elemAddressTextBox.Text = reg.Address;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    reg = null;

                    hideElemData();
                    showElemProperties();

                    break;
                case "OTHER":
                    S6xOtherAddress other = (S6xOtherAddress)sadS6x.slOtherAddresses[nextElemTreeNode.Name];
                    elemBankTextBox.Text = other.BankNum.ToString();
                    elemAddressTextBox.Text = other.Address;
                    other = null;

                    if (!checkElemBankAddress(false))
                    {
                        elemBankTextBox.ReadOnly = false;
                        elemAddressTextBox.ReadOnly = false;
                        elemBankTextBox.ForeColor = Color.Red;
                        elemAddressTextBox.ForeColor = Color.Red;
                    }
                    else
                    {
                        elemBankTextBox.ForeColor = nextElemTreeNode.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemTreeNode.ForeColor;
                    }
                    hideElemData();
                    showElemProperties();
                    break;
                case "SIGNATURES":
                    S6xSignature sig = (S6xSignature)sadS6x.slSignatures[nextElemTreeNode.Name];
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = sig.UniqueKey;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    sig = null;

                    hideElemData();
                    showElemProperties();
                    break;
                case "ELEMSSIGNATURES":
                    S6xElementSignature eSig = (S6xElementSignature)sadS6x.slElementsSignatures[nextElemTreeNode.Name];
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = eSig.UniqueKey;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    eSig = null;

                    hideElemData();
                    showElemProperties();
                    break;
            }

            lastElemTreeNode = nextElemTreeNode;

            elemPanel.Visible = true;
        }

        private void hideElemData()
        {
            elemDataGridView.Visible = false;
        }

        // To be able to show scaled data for tables
        private string[] getTableElemDataScale(string scalerUniqueAddress, int stepNumber)
        {
            string[] scaleResult = null;
            S6xFunction s6xScaler = null;

            if (stepNumber <= 0) return new string[] {};
            scaleResult = new string[stepNumber];

            for (int iStep = 0; iStep < stepNumber; iStep++) scaleResult[iStep] = (iStep + 1).ToString();

            if (scalerUniqueAddress != null && scalerUniqueAddress != string.Empty) s6xScaler = (S6xFunction)sadS6x.slFunctions[scalerUniqueAddress];
            if (s6xScaler == null) s6xScaler = (S6xFunction)sadS6x.slDupFunctions[scalerUniqueAddress];
            if (s6xScaler != null) if (s6xScaler.RowsNumber <= 0) s6xScaler = null;
            if (s6xScaler == null) return scaleResult;

            s6xScaler.AddressBinInt = Tools.binAddressCorrected(s6xScaler.BankNum, s6xScaler.AddressInt, ref sadBin, s6xScaler.AddressBinInt);
                            
            int iRowSize = 4;
            if (s6xScaler.ByteInput) iRowSize--;
            if (s6xScaler.ByteOutput) iRowSize--;

            object[] arrRows = new object[s6xScaler.RowsNumber];
            string[] arrBytes = sadBin.getBytesArray(s6xScaler.AddressBinInt, iRowSize * s6xScaler.RowsNumber);
            bool failedInputScale = false;
            bool failedOutputScale = false;
            int iAddress = 0;
            for (int iRow = 0; iRow < arrRows.Length; iRow++)
            {
                object[] arrRow = new object[2];
                int iValue = 0;

                if (s6xScaler.ByteInput)
                {
                    try { iValue = Tools.getByteInt(arrBytes[iAddress], s6xScaler.SignedInput); }
                    catch { iValue = 0; }
                    iAddress++;
                }
                else
                {
                    try { iValue = Tools.getWordInt(arrBytes[iAddress + 1] + arrBytes[iAddress], s6xScaler.SignedInput); }
                    catch { iValue = 0; }
                    iAddress += 2;
                }
                if (failedInputScale) arrRow[0] = iValue;
                else
                {
                    try { arrRow[0] = Tools.ScaleValue(iValue, s6xScaler.InputScaleExpression, true); }
                    catch
                    {
                        failedInputScale = true;
                        arrRow[0] = iValue;
                    }
                }

                if (s6xScaler.ByteOutput)
                {
                    try { iValue = Tools.getByteInt(arrBytes[iAddress], s6xScaler.SignedOutput); }
                    catch { iValue = 0; }
                    iAddress++;
                }
                else
                {
                    try { iValue = Tools.getWordInt(arrBytes[iAddress + 1] + arrBytes[iAddress], s6xScaler.SignedOutput); }
                    catch { iValue = 0; }
                    iAddress += 2;
                }
                if (failedOutputScale) arrRow[1] = iValue;
                else
                {
                    try { arrRow[1] = Tools.ScaleValue(iValue, s6xScaler.OutputScaleExpression, true); }
                    catch
                    {
                        failedOutputScale = true;
                        arrRow[1] = iValue;
                    }
                }
                arrRows[iRow] = arrRow;
            }

            if (failedOutputScale) return scaleResult;
            double[] scaleValues = new double[stepNumber];

            for (int iStep = 0; iStep < stepNumber; iStep++) 
            {
                scaleResult[iStep] = string.Empty;
                scaleValues[iStep] = 0.0;
            }
            double dMaxIndex = 0.0;
            double dMinIndex = double.MaxValue;
            double dMaxValue = 0.0;
            double dMinValue = 0.0;
            for (int iRow = 0; iRow < arrRows.Length; iRow++)
            {
                double dSc  = (double)((object[])arrRows[iRow])[1];
                if (dSc >= dMaxIndex)
                {
                    dMaxIndex = dSc;
                    dMaxValue = (double)((object[])arrRows[iRow])[0];
                }
                if (dSc <= dMinIndex)
                {
                    dMinIndex = dSc;
                    dMinValue = (double)((object[])arrRows[iRow])[0];
                }
                if (dSc == (int)dSc && dSc >= 0 && dSc < stepNumber)
                {
                    scaleValues[(int)dSc] = (double)((object[])arrRows[iRow])[0];
                    scaleResult[(int)dSc] = string.Format("{0:G}", scaleValues[(int)dSc]);
                }
                if (dSc <= 0.0) break;
            }
            if (scaleResult[0] == string.Empty)
            {
                if (dMinIndex > (double)(0) && dMinIndex < (double)(0 + 1))
                {
                    scaleValues[0] = dMinValue;
                    scaleResult[0] = string.Format("{0:G}", scaleValues[0]);
                }
            }
            if (scaleResult[scaleResult.Length - 1] == string.Empty)
            {
                if (dMaxIndex > (double)(scaleResult.Length - 1 - 1) && dMinIndex < (double)(scaleResult.Length - 1 + 1))
                {
                    scaleValues[scaleResult.Length - 1] = dMaxValue;
                    scaleResult[scaleResult.Length - 1] = string.Format("{0:G}", scaleValues[scaleResult.Length - 1]);
                }
                else if (dMaxIndex > 0.0 && dMaxIndex > dMinIndex)
                {
                    for (int iStep = (int)dMaxIndex; iStep < stepNumber; iStep++)
                    {
                        scaleValues[scaleResult.Length - 1] = dMaxValue;
                        scaleResult[iStep] = string.Format("{0:G}", scaleValues[scaleResult.Length - 1]);
                    }
                }
            }
            if (scaleResult[0] == string.Empty || scaleResult[scaleResult.Length - 1] == string.Empty)
            {
                for (int iStep = 0; iStep < stepNumber; iStep++) scaleResult[iStep] = (iStep + 1).ToString();
                return scaleResult;
            }

            int lastStepWithValue = -1;
            for (int iStep = 0; iStep < stepNumber; iStep++)
            {
                if (scaleResult[iStep] == string.Empty) continue;
                if (lastStepWithValue >= 0 && iStep - lastStepWithValue > 1)
                {
                    double stepGap = (scaleValues[iStep] - scaleValues[lastStepWithValue]) / (iStep - lastStepWithValue);
                    for (int iStepUpdate = lastStepWithValue + 1; iStepUpdate < iStep; iStepUpdate++) scaleValues[iStepUpdate] = scaleValues[iStepUpdate - 1] + stepGap;
                }
                lastStepWithValue = iStep;
            }

            // Final Format
            bool pureIntFormat = true;
            bool lowNumberFormat = true;
            for (int iStep = 0; iStep < stepNumber; iStep++)
            {
                if (scaleValues[iStep] != (int)iStep) pureIntFormat = false;
                // Thinking about Transfer function on 5v12
                if (scaleValues[iStep] > 6.0) lowNumberFormat = false;
            }
            string sFormat = "{0:0}";
            if (pureIntFormat || !lowNumberFormat) sFormat = "{0:0}";
            // New Scale Precision field
            if (s6xScaler.InputScalePrecision >= 0 && s6xScaler.InputScalePrecision <= 8) sFormat = "{0:0." + new string('0', s6xScaler.InputScalePrecision) + "}";
            for (int iStep = 0; iStep < stepNumber; iStep++) scaleResult[iStep] = string.Format(sFormat, scaleValues[iStep]);

            return scaleResult;
        }
        
        private void showElemData()
        {
            convertToolStripMenuItem.Tag = null;
            convertInputToolStripMenuItem.Tag = null;
            showElemData(null, null);
        }

        private void showElemData(RepositoryConversionItem rcOutput, RepositoryConversionItem rcInput)
        {
            S6xScalar s6xScalar = null;
            S6xFunction s6xFunction = null;
            S6xTable s6xTable = null;
            S6xStructure s6xStruct = null;
            DataTable dtTable = null;
            Type dataType = null;
            bool dataDecimal = false;
            bool ignoreDefinedConversion = false;
            bool dataReversed = false;
            string sValue = string.Empty;
            int iValue = 0;
            int iAddress = 0;
            string[] arrBytes = null;
            string[] arrCols = null;
            object[] arrRows = null;
            object[] arrRowsHeaders = null;
            object[] arrRow = null;
            bool failedScale = false;
            int iBfTop = -1;

            if (nextElemTreeNode == null) return;

            if (nextElemTreeNode.Parent == null) return;

            if (sadBin == null) return;

            dataDecimal = decimalToolStripMenuItem.Checked;
            dataType = typeof(double);
            dataType = typeof(string);
            if (!dataDecimal) dataType = typeof(string);
            ignoreDefinedConversion = decimalNotConvertedToolStripMenuItem.Checked;
            dataReversed = reverseOrderToolStripMenuItem.Checked;

            elemDataGridView.DataSource = null;

            switch (getCategName(nextElemTreeNode))
            {
                case "SCALARS":
                    if (isDuplicate(nextElemTreeNode)) s6xScalar = (S6xScalar)sadS6x.slDupScalars[nextElemTreeNode.Name];
                    else s6xScalar = (S6xScalar)sadS6x.slScalars[nextElemTreeNode.Name];
                    if (s6xScalar == null) return;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    s6xScalar.AddressBinInt = Tools.binAddressCorrected(s6xScalar.BankNum, s6xScalar.AddressInt, ref sadBin, s6xScalar.AddressBinInt);

                    if (s6xScalar.isBitFlags)
                    {
                        dataType = typeof(string);
                        arrCols = null;
                        if (s6xScalar.BitFlags != null)
                        {
                            if (s6xScalar.BitFlags.Length > 0)
                            {
                                arrCols = new string[s6xScalar.BitFlags.Length + 1];
                                for (int iCol = 1; iCol < arrCols.Length; iCol++) arrCols[iCol] = s6xScalar.BitFlags[arrCols.Length - iCol - 1].ShortLabel;
                            }
                        }
                        // Default BitFlags
                        if (arrCols == null)
                        {
                            iBfTop = 15;
                            if (s6xScalar.Byte) iBfTop = 7;

                            for (int iBf = iBfTop; iBf >= 0; iBf--)
                            {
                                arrCols = new string[iBfTop + 2];
                                for (int iCol = 1; iCol < arrCols.Length; iCol++) arrCols[iCol] = "B" + iBf.ToString();
                            }
                        }
                        // For better output
                        if (arrCols != null) 
                        {
                            for (int iCol = 1; iCol < arrCols.Length; iCol++) arrCols[iCol] = OutputTools.GetSpacesCenteredString(arrCols[iCol], 20);
                        }
                    }
                    else
                    {
                        arrCols = new string[1];
                    }
                    if (s6xScalar.Byte)
                    {
                        arrCols[0] = "Byte";
                        arrBytes = sadBin.getBytesArray(s6xScalar.AddressBinInt, 1);
                        try
                        {
                            sValue = arrBytes[0];
                            iValue = Tools.getByteInt(sValue, s6xScalar.Signed);
                        }
                        catch
                        {
                            sValue = string.Empty;
                            iValue = 0;
                        }
                    }
                    else
                    {
                        arrCols[0] = "Word";
                        arrBytes = sadBin.getBytesArray(s6xScalar.AddressBinInt, 2);
                        try
                        {
                            sValue = Tools.LsbFirst(arrBytes);
                            iValue = Tools.getWordInt(sValue, s6xScalar.Signed);
                        }
                        catch
                        {
                            sValue = string.Empty;
                            iValue = 0;
                        }
                    }
                    // For better output
                    if (arrCols.Length == 1) arrCols[0] = OutputTools.GetSpacesCenteredString(arrCols[0], 100);
                    else arrCols[0] = OutputTools.GetSpacesCenteredString(arrCols[0], 50);

                    arrRow = new object[arrCols.Length];
                    if (s6xScalar.isBitFlags)
                    {
                        BitArray arrBit = new BitArray(new int[] { iValue });
                        for (int iCol = 1; iCol < arrRow.Length; iCol++)
                        {
                            if (arrBit[s6xScalar.BitFlags[arrRow.Length - iCol - 1].Position]) arrRow[iCol] = s6xScalar.BitFlags[arrRow.Length - iCol - 1].SetValue;
                            else arrRow[iCol] = s6xScalar.BitFlags[arrRow.Length - iCol - 1].NotSetValue;
                        }
                    }
                    if (dataDecimal)
                    {
                        if (failedScale) arrRow[0] = iValue;
                        else
                        {
                            try
                            {
                                if (ignoreDefinedConversion)
                                {
                                    if (rcOutput == null) arrRow[0] = iValue;
                                    else arrRow[0] = Tools.ScaleValue(iValue, rcOutput.InternalFormula, 0, true);
                                }
                                else
                                {
                                    if (rcOutput == null) arrRow[0] = Tools.ScaleValue(iValue, s6xScalar.ScaleExpression, s6xScalar.ScalePrecision, true);
                                    else arrRow[0] = Tools.ScaleValue(iValue, rcOutput.InternalFormula.ToUpper().Replace("X", "(" + s6xScalar.ScaleExpression + ")"), s6xScalar.ScalePrecision, true);
                                }
                            }
                            catch
                            {
                                failedScale = true;
                                arrRow[0] = iValue;
                            }
                        }
                    }
                    else
                    {
                        arrRow[0] = sValue.ToUpper();
                    }

                    arrRows = new object[] {arrRow};

                    break;
                case "FUNCTIONS":
                    if (isDuplicate(nextElemTreeNode)) s6xFunction = (S6xFunction)sadS6x.slDupFunctions[nextElemTreeNode.Name];
                    else s6xFunction = (S6xFunction)sadS6x.slFunctions[nextElemTreeNode.Name];
                    if (s6xFunction == null) return;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    s6xFunction.AddressBinInt = Tools.binAddressCorrected(s6xFunction.BankNum, s6xFunction.AddressInt, ref sadBin, s6xFunction.AddressBinInt);

                    arrCols = new string[2];
                    arrCols[0] = "Word Input";
                    iValue = 2;
                    if (s6xFunction.ByteInput)
                    {
                        arrCols[0] = "Byte Input";
                        iValue--;
                    }
                    arrCols[1] = "Word Output";
                    iValue += 2;
                    if (s6xFunction.ByteOutput)
                    {
                        arrCols[1] = "Byte Output";
                        iValue--;
                    }
                    arrCols[0] = OutputTools.GetSpacesCenteredString(arrCols[0], 50);   // For better output
                    arrCols[1] = OutputTools.GetSpacesCenteredString(arrCols[1], 50);   // For better output

                    if (s6xFunction.RowsNumber <= 0)
                    {
                        arrRows = new object[] {};
                    }
                    else
                    {
                        arrRows = new object[s6xFunction.RowsNumber];
                        arrBytes = sadBin.getBytesArray(s6xFunction.AddressBinInt, iValue * arrRows.Length);
                        iAddress = 0;
                        for (int iRow = 0; iRow < arrRows.Length; iRow++)
                        {
                            arrRow = new object[arrCols.Length];
                            for (int iCol = 0; iCol < arrRow.Length; iCol++)
                            {
                                if (iCol % 2 == 0)
                                {
                                    if (s6xFunction.ByteInput)
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress];
                                            iValue = Tools.getByteInt(sValue, s6xFunction.SignedInput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress++;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress + 1] + arrBytes[iAddress];
                                            iValue = Tools.getWordInt(sValue, s6xFunction.SignedInput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress += 2;
                                    }
                                    if (dataDecimal)
                                    {
                                        if (failedScale) arrRow[iCol] = iValue;
                                        else
                                        {
                                            try
                                            {
                                                if (ignoreDefinedConversion)
                                                {
                                                    if (rcInput == null) arrRow[iCol] = iValue;
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcInput.InternalFormula, 0, true);
                                                }
                                                else
                                                {
                                                    if (rcInput == null) arrRow[iCol] = Tools.ScaleValue(iValue, s6xFunction.InputScaleExpression, s6xFunction.InputScalePrecision, true);
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcInput.InternalFormula.ToUpper().Replace("X", "(" + s6xFunction.InputScaleExpression + ")"), s6xFunction.InputScalePrecision, true);
                                                }
                                            }
                                            catch
                                            {
                                                failedScale = true;
                                                arrRow[iCol] = iValue;
                                            }
                                        }
                                    }
                                    else arrRow[iCol] = sValue.ToUpper();
                                }
                                else
                                {
                                    if (s6xFunction.ByteOutput)
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress];
                                            iValue = Tools.getByteInt(sValue, s6xFunction.SignedOutput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress++;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress + 1] + arrBytes[iAddress];
                                            iValue = Tools.getWordInt(sValue, s6xFunction.SignedOutput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress += 2;
                                    }
                                    if (dataDecimal)
                                    {
                                        if (failedScale) arrRow[iCol] = iValue;
                                        else
                                        {
                                            try
                                            {
                                                if (ignoreDefinedConversion)
                                                {
                                                    if (rcOutput == null) arrRow[iCol] = iValue;
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula, 0, true);
                                                }
                                                else
                                                {
                                                    if (rcOutput == null) arrRow[iCol] = Tools.ScaleValue(iValue, s6xFunction.OutputScaleExpression, s6xFunction.OutputScalePrecision, true);
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula.ToUpper().Replace("X", "(" + s6xFunction.OutputScaleExpression + ")"), s6xFunction.OutputScalePrecision, true);
                                                }
                                            }
                                            catch
                                            {
                                                failedScale = true;
                                                arrRow[iCol] = iValue;
                                            }
                                        }
                                    }
                                    else arrRow[iCol] = sValue.ToUpper();
                                }
                            }
                            arrRows[iRow] = arrRow;
                        }
                    }
                    break;
                case "TABLES":
                    if (isDuplicate(nextElemTreeNode)) s6xTable = (S6xTable)sadS6x.slDupTables[nextElemTreeNode.Name];
                    else s6xTable = (S6xTable)sadS6x.slTables[nextElemTreeNode.Name];
                    if (s6xTable == null) return;
                    if (s6xTable.ColsNumber <= 0) return;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    s6xTable.AddressBinInt = Tools.binAddressCorrected(s6xTable.BankNum, s6xTable.AddressInt, ref sadBin, s6xTable.AddressBinInt);

                    //arrCols = new string[s6xTable.ColsNumber];
                    //for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = (iCol + 1).ToString();
                    arrCols = getTableElemDataScale(s6xTable.ColsScalerAddress, s6xTable.ColsNumber);

                    // For better output
                    if (arrCols != null)
                    {
                        for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = OutputTools.GetSpacesCenteredString(arrCols[iCol], 10);
                    }
                    
                    if (s6xTable.RowsNumber <= 0)
                    {
                        arrRows = new object[] {};
                    }
                    else
                    {
                        arrRows = new object[s6xTable.RowsNumber];
                        arrRowsHeaders = getTableElemDataScale(s6xTable.RowsScalerAddress, s6xTable.RowsNumber);
                        if (s6xTable.WordOutput) arrBytes = sadBin.getBytesArray(s6xTable.AddressBinInt, arrCols.Length * arrRows.Length * 2);
                        else arrBytes = sadBin.getBytesArray(s6xTable.AddressBinInt, arrCols.Length * arrRows.Length);
                        iAddress = 0;
                        for (int iRow = 0; iRow < arrRows.Length; iRow++)
                        {
                            arrRow = new object[arrCols.Length];
                            for (int iCol = 0; iCol < arrRow.Length; iCol++)
                            {
                                try
                                {
                                    if (s6xTable.WordOutput)
                                    {
                                        sValue = arrBytes[iAddress + 1] + arrBytes[iAddress];
                                        iValue = Tools.getWordInt(sValue, s6xTable.SignedOutput);
                                    }
                                    else
                                    {
                                        sValue = arrBytes[iAddress];
                                        iValue = Tools.getByteInt(sValue, s6xTable.SignedOutput);
                                    }
                                }
                                catch
                                {
                                    sValue = string.Empty;
                                    iValue = 0;
                                }
                                iAddress++;
                                if (s6xTable.WordOutput) iAddress++;

                                if (dataDecimal)
                                {
                                    if (failedScale) arrRow[iCol] = iValue;
                                    else
                                    {
                                        try
                                        {
                                            if (ignoreDefinedConversion)
                                            {
                                                if (rcOutput == null) arrRow[iCol] = iValue;
                                                else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula, 0, true);
                                            }
                                            else
                                            {
                                                if (rcOutput == null) arrRow[iCol] = Tools.ScaleValue(iValue, s6xTable.CellsScaleExpression, s6xTable.CellsScalePrecision, true);
                                                else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula.ToUpper().Replace("X", "(" + s6xTable.CellsScaleExpression + ")"), s6xTable.CellsScalePrecision, true);
                                            }
                                        }
                                        catch
                                        {
                                            failedScale = true;
                                            arrRow[iCol] = iValue;
                                        }
                                    }
                                }
                                else arrRow[iCol] = sValue.ToUpper();
                            }
                            arrRows[iRow] = arrRow;
                        }
                    }
                    break;
                case "STRUCTURES":
                    if (isDuplicate(nextElemTreeNode)) s6xStruct = (S6xStructure)sadS6x.slDupStructures[nextElemTreeNode.Name];
                    else s6xStruct = (S6xStructure)sadS6x.slStructures[nextElemTreeNode.Name];
                    if (s6xStruct == null) return;
                    if (s6xStruct.Number <= 0) return;

                    s6xStruct.Structure = new Structure(s6xStruct);
                    if (!s6xStruct.Structure.isValid) return;
                    if (s6xStruct.Structure.isEmpty) return;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    s6xStruct.Structure.AddressBinInt = Tools.binAddressCorrected(s6xStruct.Structure.BankNum, s6xStruct.Structure.AddressInt, ref sadBin, s6xStruct.Structure.AddressBinInt);

                    arrBytes = sadBin.getBytesArray(s6xStruct.Structure.AddressBinInt, s6xStruct.Structure.MaxSizeSingle * s6xStruct.Number);
                    s6xStruct.Structure.Read(ref arrBytes, s6xStruct.Number);
                    arrBytes = null;

                    arrCols = new string[s6xStruct.Structure.MaxLineItemsNum];
                    for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = (iCol + 1).ToString();

                    // For better output
                    for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = OutputTools.GetSpacesCenteredString(arrCols[iCol], 10);

                    dataType = typeof(string);
                    
                    if (s6xStruct.Structure.Lines.Count <= 0)
                    {
                        arrRows = new object[] { };
                    }
                    else
                    {
                        arrRows = new object[s6xStruct.Structure.Lines.Count];
                        int iRow = 0;
                        foreach (StructureLine structLine in s6xStruct.Structure.Lines)
                        {
                            arrRow = new object[arrCols.Length];
                            for (int iCol = 0; iCol < structLine.Items.Count; iCol++) arrRow[iCol] = ((StructureItem)structLine.Items[iCol]).Value();
                            arrRows[iRow] = arrRow;
                            iRow++;
                        }
                    }
                    break;
                default:
                    return;
            }

            if (arrCols == null) return;

            if (dataReversed)
            {
                object[] arrReversedRows = new object[arrRows.Length];
                for (int iRow = 0; iRow < arrReversedRows.Length; iRow++) arrReversedRows[arrReversedRows.Length - 1 - iRow] = arrRows[iRow];
                arrRows = arrReversedRows;
                arrReversedRows = null;

                if (arrRowsHeaders != null)
                {
                    arrReversedRows = new object[arrRowsHeaders.Length];
                    for (int iRow = 0; iRow < arrReversedRows.Length; iRow++) arrReversedRows[arrReversedRows.Length - 1 - iRow] = arrRowsHeaders[iRow];
                    arrRowsHeaders = arrReversedRows;
                    arrReversedRows = null;
                }
            }

            dtTable = new DataTable();
            //foreach (string colLabel in arrCols) dtTable.Columns.Add(new DataColumn(colLabel, dataType));
            for (int iCol = 0; iCol < arrCols.Length; iCol++)
            {
                DataColumn dcDC = new DataColumn(iCol.ToString(), dataType);
                dcDC.Caption = arrCols[iCol];
                dtTable.Columns.Add(dcDC);
            }

            foreach (object[] oRow in arrRows) dtTable.Rows.Add(oRow);
            arrRows = null;

            elemDataGridView.Tag = new object[] { arrCols, arrRowsHeaders };

            // For Speed purpose
            elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            elemDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            elemDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            elemDataGridView.DataSource = dtTable;

            // For Speed purpose
            switch (getCategName(nextElemTreeNode))
            {
                case "SCALARS":
                case "FUNCTIONS":
                    elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;
                case "TABLES":
                case "STUCTURES":
                    //elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;
            }

            elemDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            elemDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            dtTable = null;
            arrRowsHeaders = null;

            elemDataGridView.Visible = true;

            s6xScalar = null;
            s6xFunction = null;
            s6xTable = null;
        }

        // For Updating Columns and Rows Headers after Binding
        private void elemDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn dgVC in elemDataGridView.Columns)
            {
                dgVC.HeaderText = ((DataTable)elemDataGridView.DataSource).Columns[dgVC.Index].Caption;
                dgVC.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (elemDataGridView.Tag == null) return;
            object[] arrDefColsRows = (object[])elemDataGridView.Tag;
            if (arrDefColsRows.Length < 2) return;
            object[] arrRowsHeaders = (object[])arrDefColsRows[1];
            if (arrRowsHeaders == null) return;

            foreach (DataGridViewRow dgVR in elemDataGridView.Rows) if (arrRowsHeaders.Length > dgVR.Index) dgVR.HeaderCell.Value = arrRowsHeaders[dgVR.Index];
            arrRowsHeaders = null;
        }

        private void showElemProperties()
        {
            if (nextElemTreeNode == null) return;

            if (nextElemTreeNode.Parent == null) return;

            string categName = getCategName(nextElemTreeNode);
            
            showPropertiesTabPage(categName);
            
            switch (categName)
            {
                case "TABLES":
                    S6xTable s6xTable = null;
                    if (isDuplicate(nextElemTreeNode)) s6xTable = (S6xTable)sadS6x.slDupTables[nextElemTreeNode.Name];
                    else s6xTable = (S6xTable)sadS6x.slTables[nextElemTreeNode.Name];
                    showElemTableProperties(ref s6xTable);
                    s6xTable = null;
                    resetPropertiesModifiedStatus(elemTablePropertiesTabPage);
                    break;
                case "FUNCTIONS":
                    S6xFunction s6xFunction = null;
                    if (isDuplicate(nextElemTreeNode)) s6xFunction = (S6xFunction)sadS6x.slDupFunctions[nextElemTreeNode.Name];
                    else s6xFunction = (S6xFunction)sadS6x.slFunctions[nextElemTreeNode.Name];
                    showElemFunctionProperties(ref s6xFunction);
                    s6xFunction = null;
                    resetPropertiesModifiedStatus(elemFunctionPropertiesTabPage);
                    break;
                case "SCALARS":
                    S6xScalar s6xScalar = null;
                    if (isDuplicate(nextElemTreeNode)) s6xScalar = (S6xScalar)sadS6x.slDupScalars[nextElemTreeNode.Name];
                    else s6xScalar = (S6xScalar)sadS6x.slScalars[nextElemTreeNode.Name];
                    showElemScalarProperties(ref s6xScalar);
                    s6xScalar = null;
                    resetPropertiesModifiedStatus(elemScalarPropertiesTabPage);
                    break;
                case "STRUCTURES":
                    S6xStructure s6xStructure = null;
                    if (isDuplicate(nextElemTreeNode)) s6xStructure = (S6xStructure)sadS6x.slDupStructures[nextElemTreeNode.Name];
                    else s6xStructure = (S6xStructure)sadS6x.slStructures[nextElemTreeNode.Name];
                    showElemStructureProperties(ref s6xStructure);
                    s6xStructure = null;
                    resetPropertiesModifiedStatus(elemStructurePropertiesTabPage);
                    break;
                case "ROUTINES":
                    S6xRoutine s6xRoutine = (S6xRoutine)sadS6x.slRoutines[nextElemTreeNode.Name];
                    showElemRoutineProperties(ref s6xRoutine);
                    s6xRoutine = null;
                    resetPropertiesModifiedStatus(elemRoutineTabPage);
                    break;
                case "OPERATIONS":
                    S6xOperation s6xOpe = (S6xOperation)sadS6x.slOperations[nextElemTreeNode.Name];
                    showElemOperationProperties(ref s6xOpe);
                    s6xOpe = null;
                    resetPropertiesModifiedStatus(elemOpeTabPage);
                    break;
                case "REGISTERS":
                    S6xRegister s6xReg = (S6xRegister)sadS6x.slRegisters[nextElemTreeNode.Name];
                    showElemRegisterProperties(ref s6xReg);
                    s6xReg = null;
                    resetPropertiesModifiedStatus(elemRegisterTabPage);
                    break;
                case "OTHER":
                    S6xOtherAddress s6xOther = (S6xOtherAddress)sadS6x.slOtherAddresses[nextElemTreeNode.Name];
                    showElemOtherProperties(ref s6xOther);
                    s6xOther = null;
                    resetPropertiesModifiedStatus(elemOtherTabPage);
                    break;
                case "SIGNATURES":
                    S6xSignature s6xSig = (S6xSignature)sadS6x.slSignatures[nextElemTreeNode.Name];
                    showElemSignatureProperties(ref s6xSig);
                    s6xSig = null;
                    resetPropertiesModifiedStatus(elemSignatureTabPage);
                    break;
                case "ELEMSSIGNATURES":
                    S6xElementSignature s6xESig = (S6xElementSignature)sadS6x.slElementsSignatures[nextElemTreeNode.Name];
                    showElemElemSignatureProperties(ref s6xESig);
                    s6xESig = null;
                    resetPropertiesModifiedStatus(elemElemSignatureTabPage);
                    break;
            }
        }

        private void showPropertiesTabPage(string categoryName)
        {
            TabPage selectedTabPage = null;
            TabPage infoTabPage = null;
            ArrayList removeTabPages = null;
            
            switch (categoryName)
            {
                case "PROPERTIES":
                    selectedTabPage = s6xPropertiesTabPage;
                    break;
                case "TABLES":
                    selectedTabPage = elemTablePropertiesTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case "FUNCTIONS":
                    selectedTabPage = elemFunctionPropertiesTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case "SCALARS":
                    selectedTabPage = elemScalarPropertiesTabPage;
                    break;
                case "STRUCTURES":
                    selectedTabPage = elemStructurePropertiesTabPage;
                    break;
                case "ROUTINES":
                    selectedTabPage = elemRoutineTabPage;
                    break;
                case "OPERATIONS":
                    selectedTabPage = elemOpeTabPage;
                    break;
                case "REGISTERS":
                    selectedTabPage = elemRegisterTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case "OTHER":
                    selectedTabPage = elemOtherTabPage;
                    break;
                case "SIGNATURES":
                    selectedTabPage = elemSignatureTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case "ELEMSSIGNATURES":
                    selectedTabPage = elemElemSignatureTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
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
            if (infoTabPage != null) elemTabControl.TabPages.Add(infoTabPage);

            elemTabControl.SelectedTab = selectedTabPage;

            selectedTabPage = null;
        }

        private bool checkElemBankAddress(bool showMessage)
        {
            bool bankCheckPassed = true;
            bool addressCheckPassed = true;
            int bankNum = -1;
            string checkMessage = string.Empty;

            switch (elemBankTextBox.Text)
            {
                case "8":
                case "1":
                case "9":
                case "0":
                    bankNum = Convert.ToInt32(elemBankTextBox.Text);
                    break;
                default:
                    bankCheckPassed = false;
                    break;
            }

            if (sadBin == null)
            {
                try
                {
                    int address = Convert.ToInt32(elemAddressTextBox.Text, 16);
                    if (address < SADDef.EecBankStartAddress || address > 0xffff) addressCheckPassed = false;
                }
                catch
                {
                    addressCheckPassed = false;
                }
            }
            else
            {
                try
                {
                    addressCheckPassed = sadBin.isBankAddress(bankNum, Convert.ToInt32(elemAddressTextBox.Text, 16) - SADDef.EecBankStartAddress);
                }
                catch
                {
                    addressCheckPassed = false;
                }
            }

            if (showMessage)
            {
                if (!bankCheckPassed) checkMessage += "Bank is invalid.\r\n";
                if (!addressCheckPassed) checkMessage += "Address is invalid.\r\n";

                if (checkMessage != string.Empty) MessageBox.Show(checkMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bankCheckPassed && addressCheckPassed;
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

        private bool checkColsRowsNumber(string number)
        {
            try
            {
                int num = Convert.ToInt32(number);
                if (num <= 0 || num >= 512) return false;
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

        private TreeNode checkDuplicate(string uniqueAddress, bool showMessages)
        {
            TreeNode tnDuplicate = null;
            bool isReserved = false;
            string sMessage = string.Empty;

            foreach (TreeNode tnCateg in elemsTreeView.Nodes)
            {
                switch (tnCateg.Name)
                {
                    case "PROPERTIES":
                    case "SIGNATURES":
                    case "ELEMSSIGNATURES":
                        break;
                    default:
                        foreach (TreeNode tnNode in tnCateg.Nodes)
                        {
                            if (tnNode.Name == uniqueAddress)
                            {
                                tnDuplicate = tnNode;
                                break;
                            }
                        }
                        break;
                }
                if (tnDuplicate != null) break;
            }

            if (tnDuplicate != null)
            {
                sMessage = "Address is already used.\r\n";
                sMessage += "\t" + tnDuplicate.Text + "\tfound in " + tnDuplicate.Parent.Text + "\r\n";
                switch (tnDuplicate.Parent.Name)
                {
                    case "RESERVED":
                        isReserved = true;
                        break;
                    default:
                        isReserved = sadBin.Calibration.isLoadCreated(tnDuplicate.Name);
                        break;
                }
                if (isReserved)
                {
                    if (showMessages) MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    tnDuplicate = new TreeNode();
                }
                else
                {
                    if (showMessages)
                    {
                        sMessage += "Overwrite existing element ?";
                        if (MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                        {
                            tnDuplicate = new TreeNode();
                        }
                    }
                }
            }

            return tnDuplicate;
        }

        private bool checkElem(string categ)
        {
            switch (categ)
            {
                case "TABLES":
                    return checkElemTable();
                case "FUNCTIONS":
                    return checkElemFunction();
                case "SCALARS":
                    return checkElemScalar();
                case "STRUCTURES":
                    return checkElemStructure();
                case "ROUTINES":
                    return checkElemRoutine();
                case "OPERATIONS":
                    return checkElemOperation();
                case "REGISTERS":
                    return checkElemRegister();
                case "OTHER":
                    return checkElemOther();
                case "SIGNATURES":
                    return checkElemSignature();
                case "ELEMSSIGNATURES":
                    return checkElemElemSignature();
                default:
                    return false;
            }
        }

        private bool checkElemScalar()
        {
            bool checkPassed = true;

            checkPassed = checkElemBankAddress(true);
            
            if (checkPassed) checkScaleExpression(scalarScaleTextBox.Text); //WARNING ONLY

            return checkPassed;
        }

        private bool checkElemFunction()
        {
            bool checkPassed = true;

            checkPassed = checkElemBankAddress(true);
            if (checkPassed)
            {
                checkPassed = checkColsRowsNumber(functionRowsTextBox.Text);
                if (!checkPassed) MessageBox.Show("Invalid Rows number.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (checkPassed)
            {
                checkScaleExpression(functionScaleInputTextBox.Text);   // WARNING ONLY
                checkScaleExpression(functionScaleOutputTextBox.Text);  // WARNING ONLY
            }

            return checkPassed;
        }

        private bool checkElemTable()
        {
            bool checkPassed = true;

            checkPassed = checkElemBankAddress(true);
            if (checkPassed)
            {
                checkPassed = checkColsRowsNumber(tableColsTextBox.Text);
                if (!checkPassed) MessageBox.Show("Invalid Columns number.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (checkPassed)
            {
                checkPassed = checkColsRowsNumber(tableRowsTextBox.Text);
                if (!checkPassed) MessageBox.Show("Invalid Rows number.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (checkPassed) checkScaleExpression(tableScaleTextBox.Text);  // WARNING ONLY

            return checkPassed;
        }

        private bool checkElemStructure()
        {
            bool checkPassed = true;

            checkPassed = checkElemBankAddress(true);
            if (checkPassed)
            {
                checkPassed = checkColsRowsNumber(structureNumTextBox.Text);
                if (!checkPassed) MessageBox.Show("Invalid Number.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (checkPassed)
            {
                Structure sStruct = new Structure();
                sStruct.StructDefString = structureStructTextBox.Text;
                checkPassed = sStruct.isValid;
                sStruct = null;
                if (!checkPassed) MessageBox.Show("Invalid Structure Definition.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return checkPassed;
        }

        private bool checkElemRoutine()
        {
            bool checkPassed = true;

            checkPassed = checkElemBankAddress(true);
            if (checkPassed)
            {
                checkPassed = checkNumber(routineArgsNumTextBox.Text);
                if (!checkPassed) MessageBox.Show("Invalid Number.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return checkPassed;
        }

        private bool checkElemOperation()
        {
            return checkElemBankAddress(true);
        }

        private bool checkElemRegister()
        {
            bool checkPassed = true;

            try
            {
                if (regAddressTextBox.Text.Contains(SADDef.AdditionSeparator))
                {
                    string regPart1 = regAddressTextBox.Text.Substring(0, regAddressTextBox.Text.IndexOf(SADDef.AdditionSeparator));
                    string regPart2 = regAddressTextBox.Text.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);

                    int iRegPart1 = Convert.ToInt32(regPart1, 16);
                    int iRegPart2 = Convert.ToInt32(regPart2, 16);

                    // Register address can be between F000 & FFFF on 8065 for example
                    if (iRegPart1 < 0 || (iRegPart1 >= 0x2000 && iRegPart1 < 0xf000)) checkPassed = false;
                    if (iRegPart2 < 0 || (iRegPart2 >= 0x2000 && iRegPart2 < 0xf000)) checkPassed = false;
                }
                else
                {
                    int regAddress = Convert.ToInt32(regAddressTextBox.Text, 16);
                    // Register address can be between F000 & FFFF on 8065 for example
                    if (regAddress < 0 || (regAddress >= 0x2000 && regAddress < 0xf000))
                    {
                        checkPassed = false;
                        // 8061 Specific case, Registers can be used to translate Calibration Console and Engineering Console memory addresses
                        if (sadBin != null)
                        {
                            if (sadBin.isLoaded && sadBin.isValid)
                            {
                                if (sadBin.Calibration.Info.is8061)
                                {
                                    if (regAddress >= SADDef.CCMemory8061MinAdress && regAddress <= SADDef.ECMemory8061MaxAdress) checkPassed = true;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                checkPassed = false;
            }

            if (!checkPassed) MessageBox.Show("Invalid Register address.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            return checkPassed;
        }

        private bool checkElemOther()
        {
            return checkElemBankAddress(true);
        }

        private bool checkElemSignature()
        {
            return true;
        }

        private bool checkElemElemSignature()
        {
            bool checkPassed = true;

            switch (elementSignatureTypeComboBox.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                    break;
                default:
                    checkPassed = false;
                    break;
            }

            if (!checkPassed) MessageBox.Show("Invalid Element Properties.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            return checkPassed;
        }

        private void newElemScalarProperties()
        {
            scalarByteCheckBox.Checked = true;
            scalarSignedCheckBox.Checked = false;
            scalarScaleTextBox.Text = "X";
            scalarScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;
            scalarUnitsTextBox.Text = string.Empty;
            scalarSkipCheckBox.Checked = false;
            scalarLabelTextBox.Text = elemLabelTextBox.Text;
            scalarSLabelTextBox.Text = SADDef.ShortScalarPrefix + string.Format("{0:d4}", sadS6x.slScalars.Count + 1);
            scalarCommentsTextBox.Text = string.Empty;
            scalarOutputCommentsCheckBox.Checked = false;

            scalarBitFlagsCheckBox.Checked = false;
            scalarBitFlagsButton.Tag = null;
        }

        private void newElemFunctionProperties()
        {
            functionRowsTextBox.Text = "0";
            functionByteCheckBox.Checked = true;
            functionSignedInputCheckBox.Checked = false;
            functionSignedOutputCheckBox.Checked = false;
            functionUnitsInputTextBox.Text = string.Empty;
            functionUnitsOutputTextBox.Text = string.Empty;
            functionScaleInputTextBox.Text = "X";
            functionScaleOutputTextBox.Text = "X";
            functionScalePrecInputNumericUpDown.Value = SADDef.DefaultScalePrecision;
            functionScalePrecOutputNumericUpDown.Value = SADDef.DefaultScalePrecision;
            functionSkipCheckBox.Checked = false;
            functionLabelTextBox.Text = elemLabelTextBox.Text;
            functionSLabelTextBox.Text = SADDef.ShortFunctionPrefix + string.Format("{0:d3}", sadS6x.slFunctions.Count + 1);
            functionCommentsTextBox.Text = string.Empty;
            functionOutputCommentsCheckBox.Checked = false;

            elemInfoRichTextBox.Clear();
        }

        private void newElemTableProperties()
        {
            tableColsTextBox.Text = "0";
            tableRowsTextBox.Text = "0";
            tableWordCheckBox.Checked = false;
            tableSignedCheckBox.Checked = false;
            tableScaleTextBox.Text = "X";
            tableScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;
            tableCellsUnitsTextBox.Text = string.Empty;
            tableColsUnitsTextBox.Text = string.Empty;
            tableRowsUnitsTextBox.Text = string.Empty;
            tableColsScalerButton.Text = string.Empty;
            tableColsScalerButton.Tag = null;
            tableRowsScalerButton.Text = string.Empty;
            tableRowsScalerButton.Tag = null;
            tableSkipCheckBox.Checked = false;
            tableLabelTextBox.Text = elemLabelTextBox.Text;
            tableSLabelTextBox.Text = SADDef.ShortTablePrefix + string.Format("{0:d3}", sadS6x.slTables.Count + 1);
            tableCommentsTextBox.Text = string.Empty;
            tableOutputCommentsCheckBox.Checked = false;

            elemInfoRichTextBox.Clear();
        }

        private void newElemStructureProperties()
        {
            structureSkipCheckBox.Checked = false;
            structureNumTextBox.Text = "1";
            structureStructTextBox.Text = string.Empty;
            structureLabelTextBox.Text = elemLabelTextBox.Text;
            structureSLabelTextBox.Text = SADDef.ShortStructurePrefix + string.Format("{0:d3}", sadS6x.slStructures.Count + 1);
            structureCommentsTextBox.Text = string.Empty;
            structureOutputCommentsCheckBox.Checked = false;
        }

        private void newElemRoutineProperties()
        {
            routineSkipCheckBox.Checked = false;
            routineArgsNumTextBox.Text = "0";
            routineArgsNumOverrideCheckBox.Checked = false;
            routineLabelTextBox.Text = elemLabelTextBox.Text;
            routineSLabelTextBox.Text = SADDef.ShortCallPrefix + string.Format("{0:d4}", sadS6x.slRoutines.Count + 1);
            routineCommentsTextBox.Text = string.Empty;
            routineOutputCommentsCheckBox.Checked = false;

            routineAdvCheckBox.Checked = false;
            routineAdvButton.Tag = null;
        }

        private void newElemOperationProperties()
        {
            opeSkipCheckBox.Checked = false;
            opeLabelTextBox.Text = elemLabelTextBox.Text;
            opeSLabelTextBox.Text = SADDef.ShortOpePrefix + string.Format("{0:d4}", sadS6x.slOperations.Count + 1);
            opeCommentsTextBox.Text = string.Empty;
            opeOutputCommentsCheckBox.Checked = false;
        }

        private void newElemRegisterProperties()
        {
            regAddressTextBox.Text = "00";
            regSkipCheckBox.Checked = false;
            regLabelTextBox.Text = elemLabelTextBox.Text;
            regByteLabelTextBox.Text = string.Empty;
            regWordLabelTextBox.Text = string.Empty;
            regCommentsTextBox.Text = string.Empty;

            elemInfoRichTextBox.Clear();

            regBitFlagsCheckBox.Checked = false;
            regBitFlagsButton.Tag = null;
        }

        private void newElemOtherProperties()
        {
            otherSkipCheckBox.Checked = false;
            otherLabelTextBox.Text = string.Empty;
            otherCommentsTextBox.Text = string.Empty;
            otherOutputCommentsCheckBox.Checked = false;
        }

        private void newElemSignatureProperties()
        {
            signatureSigTextBox.Text = string.Empty;
            signatureSkipCheckBox.Checked = false;
            signatureLabelTextBox.Text = elemLabelTextBox.Text;
            signatureSLabelTextBox.Text = "Sig" + string.Format("{0:d3}", sadS6x.slSignatures.Count + 1);
            signatureCommentsTextBox.Text = string.Empty;
            signatureOutputCommentsCheckBox.Checked = false;

            signatureAdvCheckBox.Checked = false;
            signatureAdvButton.Tag = null;
        }

        private void newElemElemSignatureProperties()
        {
            elementSignatureSigTextBox.Text = string.Empty;
            elementSignatureSkipCheckBox.Checked = false;
            elementSignatureLabelTextBox.Text = elemLabelTextBox.Text;
            elementSignatureCommentsTextBox.Text = string.Empty;

            elementSignatureForcedCheckBox.Checked = false;

            elementSignatureTypeComboBox.SelectedIndex = -1;

            elementSignatureElemButton.Tag = null;
        }

        private void showElemScalarProperties(ref S6xScalar s6xScalar)
        {
            scalarByteCheckBox.Checked = s6xScalar.Byte;
            scalarSignedCheckBox.Checked = s6xScalar.Signed;
            scalarScaleTextBox.Text = s6xScalar.ScaleExpression;
            scalarScalePrecNumericUpDown.Value = s6xScalar.ScalePrecision;
            scalarUnitsTextBox.Text = s6xScalar.Units;
            scalarSkipCheckBox.Checked = s6xScalar.Skip;
            scalarLabelTextBox.Text = s6xScalar.Label;
            scalarSLabelTextBox.Text = s6xScalar.ShortLabel;

            // Windows 10 1809 (10.0.17763) Issue
            scalarCommentsTextBox.Clear();
            scalarCommentsTextBox.Multiline = false;
            scalarCommentsTextBox.Multiline = true;
            
            scalarCommentsTextBox.Text = s6xScalar.Comments;
            scalarCommentsTextBox.Text = scalarCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            scalarOutputCommentsCheckBox.Checked = s6xScalar.OutputComments;

            scalarBitFlagsCheckBox.Checked = s6xScalar.isBitFlags;

            scalarBitFlagsButton.Tag = null;
        }

        private void showElemFunctionProperties(ref S6xFunction s6xFunction)
        {
            // Back Button Mngt
            if (functionBackButton.Tag == null)
            {
                functionBackButton.Visible = false;
            }
            else
            {
                functionResetButton.Tag = functionBackButton.Tag;
                functionBackButton.Tag = null;
                functionBackButton.Visible = true;
            }
            
            functionRowsTextBox.Text = s6xFunction.RowsNumber.ToString();
            functionByteCheckBox.Checked = s6xFunction.ByteInput;
            functionSignedInputCheckBox.Checked = s6xFunction.SignedInput;
            functionSignedOutputCheckBox.Checked = s6xFunction.SignedOutput;
            functionUnitsInputTextBox.Text = s6xFunction.InputUnits;
            functionUnitsOutputTextBox.Text = s6xFunction.OutputUnits;
            functionScaleInputTextBox.Text = s6xFunction.InputScaleExpression;
            functionScaleOutputTextBox.Text = s6xFunction.OutputScaleExpression;
            functionScalePrecInputNumericUpDown.Value = s6xFunction.InputScalePrecision;
            functionScalePrecOutputNumericUpDown.Value = s6xFunction.OutputScalePrecision;
            functionSkipCheckBox.Checked = s6xFunction.Skip;
            functionLabelTextBox.Text = s6xFunction.Label;
            functionSLabelTextBox.Text = s6xFunction.ShortLabel;

            // Windows 10 1809 (10.0.17763) Issue
            functionCommentsTextBox.Clear();
            functionCommentsTextBox.Multiline = false;
            functionCommentsTextBox.Multiline = true;
            
            functionCommentsTextBox.Text = s6xFunction.Comments;
            functionCommentsTextBox.Text = functionCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            functionOutputCommentsCheckBox.Checked = s6xFunction.OutputComments;

            showElemFunctionInfo(ref s6xFunction);
        }

        private void showElemFunctionInfo(ref S6xFunction s6xFunction)
        {
            elemInfoRichTextBox.Clear();
            if (s6xFunction.Information != null && s6xFunction.Information != string.Empty)
            {
                elemInfoRichTextBox.AppendText(s6xFunction.Information);
                return;
            }

            s6xFunction.Information = string.Empty;
            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                if (s6xObject.ColsScalerAddress == s6xFunction.UniqueAddress)
                {
                    s6xFunction.Information += "Set as Columns Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
                if (s6xFunction.DuplicateNum > 0 && s6xObject.ColsScalerAddress == s6xFunction.DuplicateAddress)
                {
                    s6xFunction.Information += "Set as Columns Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
                if (s6xObject.RowsScalerAddress == s6xFunction.UniqueAddress)
                {
                    s6xFunction.Information += "Set as Rows Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
                if (s6xFunction.DuplicateNum > 0 && s6xObject.RowsScalerAddress == s6xFunction.DuplicateAddress)
                {
                    s6xFunction.Information += "Set as Rows Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
            }
            foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
            {
                if (s6xObject.ColsScalerAddress == s6xFunction.UniqueAddress)
                {
                    s6xFunction.Information += "Set as Columns Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
                if (s6xFunction.DuplicateNum > 0 && s6xObject.ColsScalerAddress == s6xFunction.DuplicateAddress)
                {
                    s6xFunction.Information += "Set as Columns Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
                if (s6xObject.RowsScalerAddress == s6xFunction.UniqueAddress)
                {
                    s6xFunction.Information += "Set as Rows Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
                if (s6xFunction.DuplicateNum > 0 && s6xObject.RowsScalerAddress == s6xFunction.DuplicateAddress)
                {
                    s6xFunction.Information += "Set as Rows Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                }
            }

            if (sadBin.isLoaded && sadBin.isDisassembled)
            {
                foreach (TableScaler tScaler in sadBin.Calibration.alTablesScalers)
                {
                    if (tScaler.InputFunctionsUniqueAddresses.Contains(s6xFunction.UniqueAddress))
                    {
                        foreach (string uniqueAddress in tScaler.ColsScaledTablesUniqueAddresses)
                        {
                            if (sadS6x.slTables.ContainsKey(uniqueAddress))
                            {
                                S6xTable s6xObject = (S6xTable)sadS6x.slTables[uniqueAddress];
                                s6xFunction.Information += "Identified as Columns Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                                s6xObject = null;
                            }
                        }
                        foreach (string uniqueAddress in tScaler.RowsScaledTablesUniqueAddresses)
                        {
                            if (sadS6x.slTables.ContainsKey(uniqueAddress))
                            {
                                S6xTable s6xObject = (S6xTable)sadS6x.slTables[uniqueAddress];
                                s6xFunction.Information += "Identified as Rows Scaler on Table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                                s6xObject = null;
                            }
                        }
                    }
                }

                if (s6xFunction.isCalibrationElement)
                {
                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(s6xFunction.UniqueAddress))
                    {
                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xFunction.UniqueAddress]).isFunction)
                        {
                            ArrayList inputRegisters = new ArrayList();
                            ArrayList outputRegisters = new ArrayList();
                            foreach (RoutineCallInfoFunction ciCi in ((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xFunction.UniqueAddress]).FunctionElem.RoutinesCallsInfos)
                            {
                                S6xRegister reg = null;
                                if (ciCi.InputRegister != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.InputRegister)];
                                    if (reg == null && !inputRegisters.Contains(Tools.RegisterInstruction(ciCi.InputRegister))) inputRegisters.Add(Tools.RegisterInstruction(ciCi.InputRegister));
                                    else if (reg != null && !inputRegisters.Contains(reg.Label)) inputRegisters.Add(reg.Label);
                                }
                                if (ciCi.OutputRegister != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegister)];
                                    if (reg == null && !outputRegisters.Contains(Tools.RegisterInstruction(ciCi.OutputRegister))) outputRegisters.Add(Tools.RegisterInstruction(ciCi.OutputRegister));
                                    else if (reg != null && !outputRegisters.Contains(reg.Label)) outputRegisters.Add(reg.Label);
                                }
                                if (ciCi.OutputRegisterByte != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegisterByte)];
                                    if (reg == null && !outputRegisters.Contains(Tools.RegisterInstruction(ciCi.OutputRegisterByte))) outputRegisters.Add(Tools.RegisterInstruction(ciCi.OutputRegisterByte));
                                    else if (reg != null && !outputRegisters.Contains(reg.Label)) outputRegisters.Add(reg.Label);
                                }
                            }
                            if (inputRegisters.Count > 0)
                            {
                                s6xFunction.Information += "Input Registers : " + String.Join(", ", (string[])inputRegisters.ToArray(typeof(string))) + "\r\n";
                            }
                            if (outputRegisters.Count > 0)
                            {
                                s6xFunction.Information += "Output Registers : " + String.Join(", ", (string[])outputRegisters.ToArray(typeof(string))) + "\r\n";
                            }
                            inputRegisters = null;
                            outputRegisters = null;
                        }
                    }
                }
            }

            elemInfoRichTextBox.AppendText(s6xFunction.Information);
        }

        private void showElemTableProperties(ref S6xTable s6xTable)
        {
            tableColsTextBox.Text = s6xTable.ColsNumber.ToString();
            tableRowsTextBox.Text = s6xTable.RowsNumber.ToString();
            tableWordCheckBox.Checked = s6xTable.WordOutput;
            tableSignedCheckBox.Checked = s6xTable.SignedOutput;
            tableScaleTextBox.Text = s6xTable.CellsScaleExpression;
            tableScalePrecNumericUpDown.Value = s6xTable.CellsScalePrecision;
            tableCellsUnitsTextBox.Text = s6xTable.CellsUnits;
            tableColsUnitsTextBox.Text = s6xTable.ColsUnits;
            tableRowsUnitsTextBox.Text = s6xTable.RowsUnits;
            tableColsScalerButton.Text = string.Empty;
            tableColsScalerButton.Tag = null;
            tableRowsScalerButton.Text = string.Empty;
            tableRowsScalerButton.Tag = null;
            S6xFunction s6xScaler = null;
            if (s6xTable.ColsScalerAddress != null)
            {
                s6xScaler = (S6xFunction)sadS6x.slFunctions[s6xTable.ColsScalerAddress];
                if (s6xScaler == null) s6xScaler = (S6xFunction)sadS6x.slDupFunctions[s6xTable.ColsScalerAddress];
                if (s6xScaler != null)
                {
                    tableColsScalerButton.Text = s6xScaler.Label;
                    tableColsScalerButton.Tag = s6xScaler;
                    s6xScaler = null;
                }
            }
            if (s6xTable.RowsScalerAddress != null)
            {
                s6xScaler = (S6xFunction)sadS6x.slFunctions[s6xTable.RowsScalerAddress];
                if (s6xScaler == null) s6xScaler = (S6xFunction)sadS6x.slDupFunctions[s6xTable.RowsScalerAddress];
                if (s6xScaler != null)
                {
                    tableRowsScalerButton.Text = s6xScaler.Label;
                    tableRowsScalerButton.Tag = s6xScaler;
                    s6xScaler = null;
                }
            }
            tableSkipCheckBox.Checked = s6xTable.Skip;
            tableLabelTextBox.Text = s6xTable.Label;
            tableSLabelTextBox.Text = s6xTable.ShortLabel;
            
            // Windows 10 1809 (10.0.17763) Issue
            tableCommentsTextBox.Clear();
            tableCommentsTextBox.Multiline = false;
            tableCommentsTextBox.Multiline = true;

            tableCommentsTextBox.Text = s6xTable.Comments;
            tableCommentsTextBox.Text = tableCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            tableOutputCommentsCheckBox.Checked = s6xTable.OutputComments;

            showElemTableInfo(ref s6xTable);
        }

        private void showElemTableInfo(ref S6xTable s6xTable)
        {
            elemInfoRichTextBox.Clear();
            if (s6xTable.Information != null && s6xTable.Information != string.Empty)
            {
                elemInfoRichTextBox.AppendText(s6xTable.Information);
                return;
            }

            s6xTable.Information = string.Empty;
            if (sadBin.isLoaded && sadBin.isDisassembled)
            {
                foreach (TableScaler tScaler in sadBin.Calibration.alTablesScalers)
                {
                    if (tScaler.ColsScaledTablesUniqueAddresses.Contains(s6xTable.UniqueAddress))
                    {
                        foreach (string uniqueAddress in tScaler.InputFunctionsUniqueAddresses)
                        {
                            if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                            {
                                S6xFunction s6xObject = (S6xFunction)sadS6x.slFunctions[uniqueAddress];
                                s6xTable.Information += "Identified Columns Scaler could be Function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                                s6xObject = null;
                            }
                        }
                    }
                    if (tScaler.RowsScaledTablesUniqueAddresses.Contains(s6xTable.UniqueAddress))
                    {
                        foreach (string uniqueAddress in tScaler.InputFunctionsUniqueAddresses)
                        {
                            if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                            {
                                S6xFunction s6xObject = (S6xFunction)sadS6x.slFunctions[uniqueAddress];
                                s6xTable.Information += "Identified Rows Scaler could be Function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                                s6xObject = null;
                            }
                        }
                    }
                }

                if (s6xTable.isCalibrationElement)
                {
                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(s6xTable.UniqueAddress))
                    {
                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xTable.UniqueAddress]).isTable)
                        {
                            ArrayList colsRegisters = new ArrayList();
                            ArrayList rowsRegisters = new ArrayList();
                            ArrayList outputRegisters = new ArrayList();
                            foreach (RoutineCallInfoTable ciCi in ((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xTable.UniqueAddress]).TableElem.RoutinesCallsInfos)
                            {
                                S6xRegister reg = null;
                                if (ciCi.ColsScalerRegister != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.ColsScalerRegister)];
                                    if (reg == null && !colsRegisters.Contains(Tools.RegisterInstruction(ciCi.ColsScalerRegister))) colsRegisters.Add(Tools.RegisterInstruction(ciCi.ColsScalerRegister));
                                    else if (reg != null && !colsRegisters.Contains(reg.Label)) colsRegisters.Add(reg.Label);
                                }
                                if (ciCi.RowsScalerRegister != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.RowsScalerRegister)];
                                    if (reg == null && !rowsRegisters.Contains(Tools.RegisterInstruction(ciCi.RowsScalerRegister))) rowsRegisters.Add(Tools.RegisterInstruction(ciCi.RowsScalerRegister));
                                    else if (reg != null && !rowsRegisters.Contains(reg.Label)) rowsRegisters.Add(reg.Label);
                                }
                                if (ciCi.OutputRegister != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegister)];
                                    if (reg == null && !outputRegisters.Contains(Tools.RegisterInstruction(ciCi.OutputRegister))) outputRegisters.Add(Tools.RegisterInstruction(ciCi.OutputRegister));
                                    else if (reg != null && !outputRegisters.Contains(reg.Label)) outputRegisters.Add(reg.Label);
                                }
                                if (ciCi.OutputRegisterByte != string.Empty)
                                {
                                    reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegisterByte)];
                                    if (reg == null && !outputRegisters.Contains(Tools.RegisterInstruction(ciCi.OutputRegisterByte))) outputRegisters.Add(Tools.RegisterInstruction(ciCi.OutputRegisterByte));
                                    else if (reg != null && !outputRegisters.Contains(reg.Label)) outputRegisters.Add(reg.Label);
                                }
                            }
                            if (colsRegisters.Count > 0)
                            {
                                s6xTable.Information += "Columns Registers : " + String.Join(", ", (string[])colsRegisters.ToArray(typeof(string))) + "\r\n";
                            }
                            if (rowsRegisters.Count > 0)
                            {
                                s6xTable.Information += "Rows Registers : " + String.Join(", ", (string[])rowsRegisters.ToArray(typeof(string))) + "\r\n";
                            }
                            if (outputRegisters.Count > 0)
                            {
                                s6xTable.Information += "Output Registers : " + String.Join(", ", (string[])outputRegisters.ToArray(typeof(string))) + "\r\n";
                            }
                            colsRegisters = null;
                            rowsRegisters = null;
                            outputRegisters = null;
                        }
                    }
                }
            }

            elemInfoRichTextBox.AppendText(s6xTable.Information);
        }

        private void showElemStructureProperties(ref S6xStructure s6xStructure)
        {
            structureSkipCheckBox.Checked = s6xStructure.Skip;
            structureNumTextBox.Text = s6xStructure.Number.ToString();

            // Windows 10 1809 (10.0.17763) Issue
            structureStructTextBox.Clear();
            structureStructTextBox.Multiline = false;
            structureStructTextBox.Multiline = true;
            
            structureStructTextBox.Text = s6xStructure.StructDef;
            structureStructTextBox.Text = structureStructTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            structureLabelTextBox.Text = s6xStructure.Label;
            structureSLabelTextBox.Text = s6xStructure.ShortLabel;

            // Windows 10 1809 (10.0.17763) Issue
            structureCommentsTextBox.Clear();
            structureCommentsTextBox.Multiline = false;
            structureCommentsTextBox.Multiline = true;
            
            structureCommentsTextBox.Text = s6xStructure.Comments;
            structureCommentsTextBox.Text = structureCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            structureOutputCommentsCheckBox.Checked = s6xStructure.OutputComments;
        }

        private void showElemRoutineProperties(ref S6xRoutine s6xRoutine)
        {
            routineSkipCheckBox.Checked = s6xRoutine.Skip;
            routineArgsNumTextBox.Text = s6xRoutine.ByteArgumentsNum.ToString();
            routineArgsNumOverrideCheckBox.Checked = s6xRoutine.ByteArgumentsNumOverride;
            routineLabelTextBox.Text = s6xRoutine.Label;
            routineSLabelTextBox.Text = s6xRoutine.ShortLabel;

            // Windows 10 1809 (10.0.17763) Issue
            routineCommentsTextBox.Clear();
            routineCommentsTextBox.Multiline = false;
            routineCommentsTextBox.Multiline = true;
            
            routineCommentsTextBox.Text = s6xRoutine.Comments;
            routineCommentsTextBox.Text = routineCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            routineOutputCommentsCheckBox.Checked = s6xRoutine.OutputComments;

            routineAdvCheckBox.Checked = s6xRoutine.isAdvanced;

            routineAdvButton.Tag = null;
        }

        private void showElemOperationProperties(ref S6xOperation ope)
        {
            opeSkipCheckBox.Checked = ope.Skip;
            opeLabelTextBox.Text = ope.Label;
            opeSLabelTextBox.Text = ope.ShortLabel;

            // Windows 10 1809 (10.0.17763) Issue
            opeCommentsTextBox.Clear();
            opeCommentsTextBox.Multiline = false;
            opeCommentsTextBox.Multiline = true;
            
            opeCommentsTextBox.Text = ope.Comments;
            opeCommentsTextBox.Text = opeCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            opeOutputCommentsCheckBox.Checked = ope.OutputComments;
        }

        private void showElemRegisterProperties(ref S6xRegister reg)
        {
            regAddressTextBox.Text = reg.Address;
            regSkipCheckBox.Checked = reg.Skip;
            regLabelTextBox.Text = reg.Label;
            regByteLabelTextBox.Text = reg.ByteLabel;
            regWordLabelTextBox.Text = reg.WordLabel;

            // Windows 10 1809 (10.0.17763) Issue
            regCommentsTextBox.Clear();
            regCommentsTextBox.Multiline = false;
            regCommentsTextBox.Multiline = true;
            
            regCommentsTextBox.Text = reg.Comments;
            regCommentsTextBox.Text = regCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

            elemInfoRichTextBox.Clear();
            if (reg.Information != null) elemInfoRichTextBox.AppendText(reg.Information);

            regBitFlagsCheckBox.Checked = reg.isBitFlags;

            regBitFlagsButton.Tag = null;
        }

        private void showElemOtherProperties(ref S6xOtherAddress other)
        {
            otherSkipCheckBox.Checked = other.Skip;
            otherLabelTextBox.Text = other.Label;

            // Windows 10 1809 (10.0.17763) Issue
            otherCommentsTextBox.Clear();
            otherCommentsTextBox.Multiline = false;
            otherCommentsTextBox.Multiline = true;
            
            otherCommentsTextBox.Text = other.Comments;
            otherCommentsTextBox.Text = otherCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            otherOutputCommentsCheckBox.Checked = other.OutputComments;
        }

        private void showElemSignatureProperties(ref S6xSignature sig)
        {
            // Windows 10 1809 (10.0.17763) Issue
            signatureSigTextBox.Clear();
            signatureSigTextBox.Multiline = false;
            signatureSigTextBox.Multiline = true;

            signatureSigTextBox.Text = sig.Signature;
            signatureSigTextBox.Text = signatureSigTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            signatureSkipCheckBox.Checked = sig.Skip;
            signatureLabelTextBox.Text = sig.Label;
            signatureSLabelTextBox.Text = sig.ShortLabel;

            // Windows 10 1809 (10.0.17763) Issue
            signatureCommentsTextBox.Clear();
            signatureCommentsTextBox.Multiline = false;
            signatureCommentsTextBox.Multiline = true;
            
            signatureCommentsTextBox.Text = sig.Comments;
            signatureCommentsTextBox.Text = signatureCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            signatureOutputCommentsCheckBox.Checked = sig.OutputComments;

            signatureAdvCheckBox.Checked = sig.isAdvanced;

            signatureAdvButton.Tag = null;

            showElemSignatureInfo(ref sig);
        }

        private void showElemSignatureInfo(ref S6xSignature sig)
        {
            elemInfoRichTextBox.Clear();
            if (sig.Information != null && sig.Information != string.Empty)
            {
                elemInfoRichTextBox.AppendText(sig.Information);
                return;
            }
        }

        private void showElemElemSignatureProperties(ref S6xElementSignature eSig)
        {
            // Windows 10 1809 (10.0.17763) Issue
            elementSignatureSigTextBox.Clear();
            elementSignatureSigTextBox.Multiline = false;
            elementSignatureSigTextBox.Multiline = true;
            
            elementSignatureSigTextBox.Text = eSig.Signature;
            elementSignatureSigTextBox.Text = elementSignatureSigTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            elementSignatureSkipCheckBox.Checked = eSig.Skip;
            elementSignatureLabelTextBox.Text = eSig.SignatureLabel;

            // Windows 10 1809 (10.0.17763) Issue
            elementSignatureCommentsTextBox.Clear();
            elementSignatureCommentsTextBox.Multiline = false;
            elementSignatureCommentsTextBox.Multiline = true;
            
            elementSignatureCommentsTextBox.Text = eSig.SignatureComments;
            elementSignatureCommentsTextBox.Text = elementSignatureCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

            elementSignatureForcedCheckBox.Checked = eSig.Forced;

            if (eSig.Scalar != null) elementSignatureTypeComboBox.SelectedIndex = 0;
            else if (eSig.Function != null) elementSignatureTypeComboBox.SelectedIndex = 1;
            else if (eSig.Table != null) elementSignatureTypeComboBox.SelectedIndex = 2;
            else if (eSig.Structure != null) elementSignatureTypeComboBox.SelectedIndex = 3;

            elementSignatureElemButton.Tag = null;

            showElemElemSignatureInfo(ref eSig);
        }

        private void showElemElemSignatureInfo(ref S6xElementSignature eSig)
        {
            elemInfoRichTextBox.Clear();
            if (eSig.Information != null && eSig.Information != string.Empty)
            {
                elemInfoRichTextBox.AppendText(eSig.Information);
                return;
            }
        }

        private void updateProperties()
        {
            bool checkXdfAddressPassed = true;
            try
            {
                int address = Convert.ToInt32(s6xPropertiesXdfBaseOffsetTextBox.Text, 16);
                if (address < 0 || address > 0x3ffff) checkXdfAddressPassed = false;
            }
            catch
            {
                checkXdfAddressPassed = false;
            }
            if (!checkXdfAddressPassed)
            {
                MessageBox.Show("Invalid Xdf base offset.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            resetPropertiesModifiedStatus(s6xPropertiesTabPage);

            sadS6x.Properties.Label = s6xPropertiesLabelTextBox.Text;
            sadS6x.Properties.Comments = s6xPropertiesCommentsTextBox.Text;
            sadS6x.Properties.NoNumbering = s6xPropertiesNoNumberingCheckBox.Checked;
            sadS6x.Properties.RegListOutput = s6xPropertiesRegListOutputCheckBox.Checked;
            sadS6x.Properties.XdfBaseOffset = s6xPropertiesXdfBaseOffsetTextBox.Text;
            sadS6x.Properties.XdfBaseOffsetSubtract = s6xPropertiesXdfBaseOffsetCheckBox.Checked;
            sadS6x.isSaved = false;

            resetPropertiesModifiedStatus(s6xPropertiesTabPage);
            
            elemsTreeView.Nodes["PROPERTIES"].ForeColor = Color.Purple;

            //disassemblyToolStripMenuItem.Enabled = false;
            //outputToolStripMenuItem.Enabled = false;
        }
        
        private void updateElem()
        {
            TreeNode tnCateg = null;
            TreeNode tnNode = null;
            TreeNode tnOverwrite = null;
            int bankNum = -1;
            int addressInt = -1;
            bool bDuplicate = false;
            string uniqueAddressOri = string.Empty;
            string uniqueAddress = string.Empty;
            bool isAddressChange = false;
            bool isClipBoardElem = false;
            string regPart1 = string.Empty;
            string regPart2 = string.Empty;

            if (elemsTreeView.Tag == null) return;

            tnCateg = (TreeNode)elemsTreeView.Tag;

            if (! checkElem(tnCateg.Name)) return;

            uniqueAddress = string.Empty;
            uniqueAddressOri = "X XXXXX";
            if (nextElemTreeNode != null)
            {
                uniqueAddress = nextElemTreeNode.Name;
                uniqueAddressOri = nextElemTreeNode.Name;
                isClipBoardElem = alClipBoardTempUniqueAddresses.Contains(uniqueAddressOri);
                bDuplicate = isDuplicate(nextElemTreeNode);
            }

            switch (tnCateg.Name)
            {
                case "SIGNATURES":
                case "ELEMSSIGNATURES":
                    // Unique Index based / Can not change
                    break;
                case "REGISTERS":
                    if (regAddressTextBox.Text.Contains(SADDef.AdditionSeparator))
                    {
                        regPart1 = regAddressTextBox.Text.Substring(0, regAddressTextBox.Text.IndexOf(SADDef.AdditionSeparator));
                        regPart2 = regAddressTextBox.Text.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);

                        addressInt = Convert.ToInt32(regPart1, 16);
                        uniqueAddress = Tools.RegisterUniqueAddress(addressInt) + SADDef.AdditionSeparator + Convert.ToInt32(regPart2, 16).ToString();
                    }
                    else
                    {
                        addressInt = Convert.ToInt32(regAddressTextBox.Text, 16);
                        uniqueAddress = Tools.RegisterUniqueAddress(addressInt);
                    }

                    if (uniqueAddress != uniqueAddressOri)
                    {
                        tnOverwrite = checkDuplicate(uniqueAddress, true);
                        if (tnOverwrite != null)
                        {
                            if (tnOverwrite.Parent == null) return;

                            tnNode = nextElemTreeNode;      // Backup because erased in deletedElem;
                            deleteElem(tnOverwrite, true, false, true);
                            tnOverwrite = null;
                            nextElemTreeNode = tnNode;
                            tnNode = null;
                        }
                    }
                    break;
                default:
                    // Unique Address based / Can Change
                    bankNum = Convert.ToInt32(elemBankTextBox.Text);
                    addressInt = Convert.ToInt32(elemAddressTextBox.Text, 16) - SADDef.EecBankStartAddress;
                    uniqueAddress = Tools.UniqueAddress(bankNum, addressInt);

                    if (uniqueAddress != uniqueAddressOri && !bDuplicate)
                    {
                        tnOverwrite = checkDuplicate(uniqueAddress, true);
                        if (tnOverwrite != null)
                        {
                            if (tnOverwrite.Parent == null) return;

                            tnNode = nextElemTreeNode;      // Backup because erased in deletedElem;
                            deleteElem(tnOverwrite, true, false, true);
                            tnOverwrite = null;
                            nextElemTreeNode = tnNode;
                            tnNode = null;
                        }
                    }
                    break;
            }

            isAddressChange = (nextElemTreeNode != null && uniqueAddress != uniqueAddressOri && !bDuplicate);

            switch (tnCateg.Name)
            {
                case "TABLES":
                    resetPropertiesModifiedStatus(elemTablePropertiesTabPage);
                    
                    S6xTable s6xTable = new S6xTable();
                    s6xTable.BankNum = bankNum;
                    s6xTable.AddressInt = addressInt;
                    if (bDuplicate) s6xTable.DuplicateNum = ((S6xTable)sadS6x.slDupTables[nextElemTreeNode.Name]).DuplicateNum;
                    s6xTable.AddressBinInt = s6xTable.AddressInt;
                    if (sadBin != null)
                    {
                        s6xTable.AddressBinInt = s6xTable.AddressInt + sadBin.getBankBinAddress(bankNum);
                        s6xTable.isCalibrationElement = sadBin.isCalibrationAddress(s6xTable.AddressBinInt);
                    }
                    s6xTable.Store = true;
                    s6xTable.Skip = tableSkipCheckBox.Checked;
                    s6xTable.Label = tableLabelTextBox.Text;
                    s6xTable.ShortLabel = tableSLabelTextBox.Text;
                    s6xTable.Comments = tableCommentsTextBox.Text;
                    s6xTable.OutputComments = tableOutputCommentsCheckBox.Checked;
                    s6xTable.WordOutput = tableWordCheckBox.Checked;
                    s6xTable.SignedOutput = tableSignedCheckBox.Checked;
                    s6xTable.ColsNumber = Convert.ToInt32(tableColsTextBox.Text);
                    s6xTable.RowsNumber = Convert.ToInt32(tableRowsTextBox.Text);
                    s6xTable.CellsScaleExpression = tableScaleTextBox.Text;
                    s6xTable.CellsScalePrecision = (int)tableScalePrecNumericUpDown.Value;
                    s6xTable.CellsUnits = tableCellsUnitsTextBox.Text;
                    s6xTable.ColsUnits = tableColsUnitsTextBox.Text;
                    s6xTable.RowsUnits = tableRowsUnitsTextBox.Text;

                    if (tableColsScalerButton.Tag == null) s6xTable.ColsScalerAddress = string.Empty;
                    else
                    {
                        if (((S6xFunction)tableColsScalerButton.Tag).DuplicateNum == 0) s6xTable.ColsScalerAddress = ((S6xFunction)tableColsScalerButton.Tag).UniqueAddress;
                        else s6xTable.ColsScalerAddress = ((S6xFunction)tableColsScalerButton.Tag).DuplicateAddress;
                    }

                    if (tableRowsScalerButton.Tag == null) s6xTable.RowsScalerAddress = string.Empty;
                    else
                    {
                        if (((S6xFunction)tableRowsScalerButton.Tag).DuplicateNum == 0) s6xTable.RowsScalerAddress = ((S6xFunction)tableRowsScalerButton.Tag).UniqueAddress;
                        else s6xTable.RowsScalerAddress = ((S6xFunction)tableRowsScalerButton.Tag).DuplicateAddress;
                    }
                    
                    // Xdf Unique Id Matching
                    if (s6xTable.ColsScalerAddress == string.Empty) s6xTable.ColsScalerXdfUniqueId = string.Empty;
                    else if (sadBin.S6x.slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadBin.S6x.slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                    else if (sadBin.S6x.slDupFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadBin.S6x.slDupFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                    else s6xTable.ColsScalerXdfUniqueId = string.Empty;
                    if (s6xTable.RowsScalerAddress == string.Empty) s6xTable.RowsScalerXdfUniqueId = string.Empty;
                    else if (sadBin.S6x.slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadBin.S6x.slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                    else if (sadBin.S6x.slDupFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadBin.S6x.slDupFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                    else s6xTable.RowsScalerXdfUniqueId = string.Empty;

                    if (isAddressChange)
                    {
                        sadS6x.slTables.Remove(uniqueAddressOri); 
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xTable.UniqueAddress;
                        if (sadBin != null) s6xTable.isCalibrationElement = sadBin.isCalibrationAddress(s6xTable.AddressBinInt);
                    }

                    if (bDuplicate)
                    {
                        if (sadS6x.slDupTables.ContainsKey(s6xTable.DuplicateAddress)) sadS6x.slDupTables[s6xTable.DuplicateAddress] = s6xTable;
                        else sadS6x.slDupTables.Add(s6xTable.DuplicateAddress, s6xTable);
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(s6xTable.UniqueAddress)) sadS6x.slTables[s6xTable.UniqueAddress] = s6xTable;
                        else sadS6x.slTables.Add(s6xTable.UniqueAddress, s6xTable);
                    }
                    sadS6x.isSaved = false;

                    if (bDuplicate)
                    {
                        tnNode = nextElemTreeNode;
                    }
                    else
                    {
                        if (tnCateg.Nodes.ContainsKey(s6xTable.UniqueAddress))
                        {
                            tnNode = tnCateg.Nodes[s6xTable.UniqueAddress];
                        }
                        else
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xTable.UniqueAddress;
                        }
                    }
                    tnNode.Text = s6xTable.Label;
                    tnNode.ToolTipText = s6xTable.Comments;

                    s6xTable = null;
                    break;
                case "FUNCTIONS":
                    resetPropertiesModifiedStatus(elemFunctionPropertiesTabPage);

                    S6xFunction s6xFunction = new S6xFunction();
                    s6xFunction.BankNum = bankNum;
                    s6xFunction.AddressInt = addressInt;
                    if (bDuplicate) s6xFunction.DuplicateNum = ((S6xFunction)sadS6x.slDupFunctions[nextElemTreeNode.Name]).DuplicateNum;
                    s6xFunction.AddressBinInt = s6xFunction.AddressInt;
                    if (sadBin != null)
                    {
                        s6xFunction.AddressBinInt = s6xFunction.AddressInt + sadBin.getBankBinAddress(bankNum);
                        s6xFunction.isCalibrationElement = sadBin.isCalibrationAddress(s6xFunction.AddressBinInt);
                    }
                    s6xFunction.Store = true;
                    s6xFunction.Skip = functionSkipCheckBox.Checked;
                    s6xFunction.Label = functionLabelTextBox.Text;
                    s6xFunction.ShortLabel = functionSLabelTextBox.Text;
                    s6xFunction.Comments = functionCommentsTextBox.Text;
                    s6xFunction.OutputComments = functionOutputCommentsCheckBox.Checked;
                    s6xFunction.SignedInput = functionSignedInputCheckBox.Checked;
                    s6xFunction.SignedOutput = functionSignedOutputCheckBox.Checked;
                    s6xFunction.ByteInput = functionByteCheckBox.Checked;
                    s6xFunction.ByteOutput = functionByteCheckBox.Checked;
                    s6xFunction.RowsNumber = Convert.ToInt32(functionRowsTextBox.Text);
                    s6xFunction.InputScaleExpression = functionScaleInputTextBox.Text;
                    s6xFunction.OutputScaleExpression = functionScaleOutputTextBox.Text;
                    s6xFunction.InputScalePrecision = (int)functionScalePrecInputNumericUpDown.Value;
                    s6xFunction.OutputScalePrecision = (int)functionScalePrecOutputNumericUpDown.Value;
                    s6xFunction.InputUnits = functionUnitsInputTextBox.Text;
                    s6xFunction.OutputUnits = functionUnitsOutputTextBox.Text;

                    if (isAddressChange)
                    {
                        sadS6x.slFunctions.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xFunction.UniqueAddress;
                        if (sadBin != null) s6xFunction.isCalibrationElement = sadBin.isCalibrationAddress(s6xFunction.AddressBinInt);

                        // Scalers Remap only for Main Item (No Possible Address Change for Duplicates
                        foreach (S6xTable s6xTab in sadS6x.slTables.Values)
                        {
                            if (s6xTab.ColsScalerAddress == uniqueAddressOri) s6xTab.ColsScalerAddress = uniqueAddress;
                            if (s6xTab.RowsScalerAddress == uniqueAddressOri) s6xTab.RowsScalerAddress = uniqueAddress;
                        }
                        foreach (S6xTable s6xTab in sadS6x.slDupTables.Values)
                        {
                            if (s6xTab.ColsScalerAddress == uniqueAddressOri) s6xTab.ColsScalerAddress = uniqueAddress;
                            if (s6xTab.RowsScalerAddress == uniqueAddressOri) s6xTab.RowsScalerAddress = uniqueAddress;
                        }
                    }

                    if (bDuplicate)
                    {
                        if (sadS6x.slDupFunctions.ContainsKey(s6xFunction.DuplicateAddress)) sadS6x.slDupFunctions[s6xFunction.DuplicateAddress] = s6xFunction;
                        else sadS6x.slDupFunctions.Add(s6xFunction.DuplicateAddress, s6xFunction);
                    }
                    else
                    {
                        if (sadS6x.slFunctions.ContainsKey(s6xFunction.UniqueAddress)) sadS6x.slFunctions[s6xFunction.UniqueAddress] = s6xFunction;
                        else sadS6x.slFunctions.Add(s6xFunction.UniqueAddress, s6xFunction);
                    }
                    sadS6x.isSaved = false;

                    if (bDuplicate)
                    {
                        tnNode = nextElemTreeNode;
                    }
                    else
                    {
                        if (tnCateg.Nodes.ContainsKey(s6xFunction.UniqueAddress))
                        {
                            tnNode = tnCateg.Nodes[s6xFunction.UniqueAddress];
                        }
                        else
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xFunction.UniqueAddress;
                        }
                    }
                    tnNode.Text = s6xFunction.Label;
                    tnNode.ToolTipText = s6xFunction.Comments;

                    // Indicated an Update
                    //      Reset on Table Properties for scalers
                    tnCateg.Tag = true;

                    s6xFunction = null;
                    break;
                case "SCALARS":
                    resetPropertiesModifiedStatus(elemScalarPropertiesTabPage);

                    S6xScalar s6xScalar = null;
                    if (scalarBitFlagsButton.Tag != null)
                    {
                        s6xScalar = (S6xScalar)scalarBitFlagsButton.Tag;
                        scalarBitFlagsButton.Tag = null;
                    }
                    else if (sadS6x.slScalars.ContainsKey(uniqueAddressOri))
                    {
                        s6xScalar = ((S6xScalar)sadS6x.slScalars[uniqueAddressOri]);
                    }
                    else
                    {
                        s6xScalar = new S6xScalar();
                    }
                    s6xScalar.BankNum = bankNum;
                    s6xScalar.AddressInt = addressInt;
                    if (bDuplicate) s6xScalar.DuplicateNum = ((S6xScalar)sadS6x.slDupScalars[nextElemTreeNode.Name]).DuplicateNum;
                    s6xScalar.AddressBinInt = s6xScalar.AddressInt;
                    if (sadBin != null)
                    {
                        s6xScalar.AddressBinInt = s6xScalar.AddressInt + sadBin.getBankBinAddress(bankNum);
                        s6xScalar.isCalibrationElement = sadBin.isCalibrationAddress(s6xScalar.AddressBinInt);
                    }
                    s6xScalar.Store = true;
                    s6xScalar.Skip = scalarSkipCheckBox.Checked;
                    s6xScalar.Label = scalarLabelTextBox.Text;
                    s6xScalar.ShortLabel = scalarSLabelTextBox.Text;
                    s6xScalar.Comments = scalarCommentsTextBox.Text;
                    s6xScalar.OutputComments = scalarOutputCommentsCheckBox.Checked;
                    s6xScalar.Byte = scalarByteCheckBox.Checked;
                    s6xScalar.Signed = scalarSignedCheckBox.Checked;
                    s6xScalar.ScaleExpression = scalarScaleTextBox.Text;
                    s6xScalar.ScalePrecision = (int)scalarScalePrecNumericUpDown.Value;
                    s6xScalar.Units = scalarUnitsTextBox.Text;

                    if (isAddressChange)
                    {
                        sadS6x.slScalars.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xScalar.UniqueAddress;
                        if (sadBin != null) s6xScalar.isCalibrationElement = sadBin.isCalibrationAddress(s6xScalar.AddressBinInt);
                    }

                    if (bDuplicate)
                    {
                        if (sadS6x.slDupScalars.ContainsKey(s6xScalar.DuplicateAddress)) sadS6x.slDupScalars[s6xScalar.DuplicateAddress] = s6xScalar;
                        else sadS6x.slDupScalars.Add(s6xScalar.DuplicateAddress, s6xScalar);
                    }
                    else
                    {
                        if (sadS6x.slScalars.ContainsKey(s6xScalar.UniqueAddress)) sadS6x.slScalars[s6xScalar.UniqueAddress] = s6xScalar;
                        else sadS6x.slScalars.Add(s6xScalar.UniqueAddress, s6xScalar);
                    }
                    sadS6x.isSaved = false;

                    if (bDuplicate)
                    {
                        tnNode = nextElemTreeNode;
                    }
                    else
                    {
                        if (tnCateg.Nodes.ContainsKey(s6xScalar.UniqueAddress))
                        {
                            tnNode = tnCateg.Nodes[s6xScalar.UniqueAddress];
                        }
                        else
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xScalar.UniqueAddress;
                        }
                    }
                    tnNode.Text = s6xScalar.Label;
                    tnNode.ToolTipText = s6xScalar.Comments;

                    s6xScalar = null;
                    break;
                case "STRUCTURES":
                    resetPropertiesModifiedStatus(elemStructurePropertiesTabPage);

                    S6xStructure s6xStructure = new S6xStructure();
                    s6xStructure.BankNum = bankNum;
                    s6xStructure.AddressInt = addressInt;
                    if (bDuplicate) s6xStructure.DuplicateNum = ((S6xStructure)sadS6x.slDupStructures[nextElemTreeNode.Name]).DuplicateNum;
                    s6xStructure.AddressBinInt = s6xStructure.AddressInt;
                    if (sadBin != null)
                    {
                        s6xStructure.AddressBinInt = s6xStructure.AddressInt + sadBin.getBankBinAddress(bankNum);
                        s6xStructure.isCalibrationElement = sadBin.isCalibrationAddress(s6xStructure.AddressBinInt);
                    }
                    s6xStructure.Store = true;
                    s6xStructure.Skip = structureSkipCheckBox.Checked;
                    s6xStructure.Label = structureLabelTextBox.Text;
                    s6xStructure.ShortLabel = structureSLabelTextBox.Text;
                    s6xStructure.Comments = structureCommentsTextBox.Text;
                    s6xStructure.OutputComments = structureOutputCommentsCheckBox.Checked;
                    s6xStructure.Number = Convert.ToInt32(structureNumTextBox.Text);
                    s6xStructure.StructDef = structureStructTextBox.Text;

                    s6xStructure.Structure = new Structure(s6xStructure);

                    if (isAddressChange)
                    {
                        sadS6x.slStructures.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xStructure.UniqueAddress;
                    }

                    if (bDuplicate)
                    {
                        if (sadS6x.slDupStructures.ContainsKey(s6xStructure.DuplicateAddress)) sadS6x.slDupStructures[s6xStructure.DuplicateAddress] = s6xStructure;
                        else sadS6x.slDupStructures.Add(s6xStructure.DuplicateAddress, s6xStructure);
                    }
                    else
                    {
                        if (sadS6x.slStructures.ContainsKey(s6xStructure.UniqueAddress)) sadS6x.slStructures[s6xStructure.UniqueAddress] = s6xStructure;
                        else sadS6x.slStructures.Add(s6xStructure.UniqueAddress, s6xStructure);
                    }
                    sadS6x.isSaved = false;

                    if (bDuplicate)
                    {
                        tnNode = nextElemTreeNode;
                    }
                    else
                    {
                        if (tnCateg.Nodes.ContainsKey(s6xStructure.UniqueAddress))
                        {
                            tnNode = tnCateg.Nodes[s6xStructure.UniqueAddress];
                        }
                        else
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xStructure.UniqueAddress;
                        }
                    }
                    tnNode.Text = s6xStructure.Label;
                    tnNode.ToolTipText = s6xStructure.Comments;

                    s6xStructure = null;
                    break;
                case "ROUTINES":
                    resetPropertiesModifiedStatus(elemRoutineTabPage);

                    S6xRoutine s6xRoutine = null;
                    if (routineAdvButton.Tag != null)
                    {
                        s6xRoutine = (S6xRoutine)routineAdvButton.Tag;
                        routineAdvButton.Tag = null;
                    }
                    else if (sadS6x.slRoutines.ContainsKey(uniqueAddressOri))
                    {
                        s6xRoutine = ((S6xRoutine)sadS6x.slRoutines[uniqueAddressOri]);
                    }
                    else
                    {
                        s6xRoutine = new S6xRoutine();
                    }
                    s6xRoutine.BankNum = bankNum;
                    s6xRoutine.AddressInt = addressInt;
                    s6xRoutine.Store = true;
                    s6xRoutine.Skip = routineSkipCheckBox.Checked;
                    s6xRoutine.ByteArgumentsNum = Convert.ToInt32(routineArgsNumTextBox.Text);
                    s6xRoutine.ByteArgumentsNumOverride = routineArgsNumOverrideCheckBox.Checked;
                    s6xRoutine.Label = routineLabelTextBox.Text;
                    s6xRoutine.ShortLabel = routineSLabelTextBox.Text;
                    s6xRoutine.Comments = routineCommentsTextBox.Text;
                    s6xRoutine.OutputComments = routineOutputCommentsCheckBox.Checked;

                    if (isAddressChange)
                    {
                        sadS6x.slRoutines.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xRoutine.UniqueAddress;
                    }

                    if (sadS6x.slRoutines.ContainsKey(s6xRoutine.UniqueAddress)) sadS6x.slRoutines[s6xRoutine.UniqueAddress] = s6xRoutine;
                    else sadS6x.slRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);
                    sadS6x.isSaved = false;

                    if (tnCateg.Nodes.ContainsKey(s6xRoutine.UniqueAddress))
                    {
                        tnNode = tnCateg.Nodes[s6xRoutine.UniqueAddress];
                    }
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xRoutine.UniqueAddress;
                    }
                    tnNode.Text = s6xRoutine.Label;
                    tnNode.ToolTipText = s6xRoutine.Comments;

                    s6xRoutine = null;
                    break;
                case "OPERATIONS":
                    resetPropertiesModifiedStatus(elemOpeTabPage);

                    S6xOperation s6xOpe = new S6xOperation();
                    s6xOpe.BankNum = bankNum;
                    s6xOpe.AddressInt = addressInt;
                    s6xOpe.Label = opeLabelTextBox.Text;
                    s6xOpe.ShortLabel = opeSLabelTextBox.Text;
                    s6xOpe.Comments = opeCommentsTextBox.Text;
                    s6xOpe.OutputComments = opeOutputCommentsCheckBox.Checked;
                    s6xOpe.Skip = opeSkipCheckBox.Checked;

                    if (isAddressChange)
                    {
                        sadS6x.slOperations.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xOpe.UniqueAddress;
                    }

                    if (sadS6x.slOperations.ContainsKey(s6xOpe.UniqueAddress)) sadS6x.slOperations[s6xOpe.UniqueAddress] = s6xOpe;
                    else sadS6x.slOperations.Add(s6xOpe.UniqueAddress, s6xOpe);
                    sadS6x.isSaved = false;

                    if (tnCateg.Nodes.ContainsKey(s6xOpe.UniqueAddress))
                    {
                        tnNode = tnCateg.Nodes[s6xOpe.UniqueAddress];
                    }
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xOpe.UniqueAddress;
                    }
                    tnNode.Text = s6xOpe.Label;
                    tnNode.ToolTipText = s6xOpe.Comments;

                    s6xOpe = null;
                    break;
                case "REGISTERS":
                    resetPropertiesModifiedStatus(elemRegisterTabPage);

                    S6xRegister s6xReg = null;
                    if (regBitFlagsButton.Tag != null)
                    {
                        s6xReg = (S6xRegister)regBitFlagsButton.Tag;
                        regBitFlagsButton.Tag = null;
                    }
                    else if (sadS6x.slRegisters.ContainsKey(uniqueAddressOri))
                    {
                        s6xReg = ((S6xRegister)sadS6x.slRegisters[uniqueAddressOri]);
                    }
                    else
                    {
                        s6xReg = new S6xRegister();
                    }
                    if (regAddressTextBox.Text.Contains(SADDef.AdditionSeparator))
                    {
                        regPart1 = regAddressTextBox.Text.Substring(0, regAddressTextBox.Text.IndexOf(SADDef.AdditionSeparator));
                        regPart2 = regAddressTextBox.Text.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);

                        s6xReg.AddressInt = Convert.ToInt32(regPart1, 16);
                        s6xReg.AdditionalAddress10 = Convert.ToString(Convert.ToInt32(regPart2, 16), 10);
                    }
                    else
                    {
                        s6xReg.AddressInt = Convert.ToInt32(regAddressTextBox.Text, 16);
                    }
                    s6xReg.Label = regLabelTextBox.Text;
                    s6xReg.ByteLabel = regByteLabelTextBox.Text;
                    s6xReg.WordLabel = regWordLabelTextBox.Text;
                    s6xReg.Comments = regCommentsTextBox.Text;
                    s6xReg.Store = true;
                    s6xReg.Skip = regSkipCheckBox.Checked;

                    if (isAddressChange)
                    {
                        sadS6x.slRegisters.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xReg.UniqueAddress;
                    }

                    if (sadS6x.slRegisters.ContainsKey(s6xReg.UniqueAddress)) sadS6x.slRegisters[s6xReg.UniqueAddress] = s6xReg;
                    else sadS6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                    sadS6x.isSaved = false;

                    if (tnCateg.Nodes.ContainsKey(s6xReg.UniqueAddress))
                    {
                        tnNode = tnCateg.Nodes[s6xReg.UniqueAddress];
                    }
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xReg.UniqueAddress;
                    }
                    tnNode.Text = s6xReg.FullLabel;
                    tnNode.ToolTipText = s6xReg.FullComments;

                    s6xReg = null;
                    break;
                case "OTHER":
                    resetPropertiesModifiedStatus(elemOtherTabPage);

                    S6xOtherAddress s6xOther = new S6xOtherAddress();
                    s6xOther.BankNum = bankNum;
                    s6xOther.AddressInt = addressInt;
                    s6xOther.Label = otherLabelTextBox.Text;
                    s6xOther.Comments = otherCommentsTextBox.Text;
                    s6xOther.OutputComments = otherOutputCommentsCheckBox.Checked;
                    s6xOther.Skip = otherSkipCheckBox.Checked;

                    if (isAddressChange)
                    {
                        sadS6x.slOtherAddresses.Remove(uniqueAddressOri);
                        tnCateg.Nodes[uniqueAddressOri].Name = s6xOther.UniqueAddress;
                    }

                    if (sadS6x.slOtherAddresses.ContainsKey(s6xOther.UniqueAddress)) sadS6x.slOtherAddresses[s6xOther.UniqueAddress] = s6xOther;
                    else sadS6x.slOtherAddresses.Add(s6xOther.UniqueAddress, s6xOther);
                    sadS6x.isSaved = false;

                    if (tnCateg.Nodes.ContainsKey(s6xOther.UniqueAddress))
                    {
                        tnNode = tnCateg.Nodes[s6xOther.UniqueAddress];
                    }
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xOther.UniqueAddress;
                    }
                    tnNode.Text = s6xOther.Label;
                    tnNode.ToolTipText = s6xOther.Comments;

                    s6xOther = null;
                    break;
                case "SIGNATURES":
                    resetPropertiesModifiedStatus(elemSignatureTabPage);

                    S6xSignature s6xSig = null;
                    if (signatureAdvButton.Tag != null)
                    {
                        s6xSig = (S6xSignature)signatureAdvButton.Tag;
                        if (s6xSig.UniqueKey != uniqueAddressOri)
                        {
                            s6xSig.UniqueKey = sadS6x.getNewSignatureUniqueKey();
                        }
                        signatureAdvButton.Tag = null;
                    }
                    else if (sadS6x.slSignatures.ContainsKey(uniqueAddressOri))
                    {
                        s6xSig = (S6xSignature)sadS6x.slSignatures[uniqueAddressOri];
                    }
                    else
                    {
                        s6xSig = new S6xSignature();
                        s6xSig.UniqueKey = sadS6x.getNewSignatureUniqueKey();
                    }

                    s6xSig.Label = signatureLabelTextBox.Text;
                    s6xSig.ShortLabel = signatureSLabelTextBox.Text;
                    s6xSig.Comments = signatureCommentsTextBox.Text;
                    s6xSig.OutputComments = signatureOutputCommentsCheckBox.Checked;
                    s6xSig.Signature = signatureSigTextBox.Text;
                    s6xSig.Skip = signatureSkipCheckBox.Checked;

                    if (sadS6x.slSignatures.ContainsKey(s6xSig.UniqueKey)) sadS6x.slSignatures[s6xSig.UniqueKey] = s6xSig;
                    else sadS6x.slSignatures.Add(s6xSig.UniqueKey, s6xSig);
                    sadS6x.isSaved = false;

                    if (tnCateg.Nodes.ContainsKey(s6xSig.UniqueKey))
                    {
                        tnNode = tnCateg.Nodes[s6xSig.UniqueKey];
                    }
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xSig.UniqueKey;
                    }
                    tnNode.Text = s6xSig.Label;
                    tnNode.ToolTipText = s6xSig.Comments;

                    s6xSig = null;
                    break;
                case "ELEMSSIGNATURES":
                    resetPropertiesModifiedStatus(elemElemSignatureTabPage);

                    S6xElementSignature s6xESig = null;

                    if (elementSignatureElemButton.Tag != null)
                    {
                        s6xESig = (S6xElementSignature)elementSignatureElemButton.Tag;
                        if (s6xESig.UniqueKey != uniqueAddressOri)
                        {
                            s6xESig.UniqueKey = sadS6x.getNewElementSignatureUniqueKey();
                        }
                        elementSignatureElemButton.Tag = null;
                    }
                    else if (sadS6x.slElementsSignatures.ContainsKey(uniqueAddressOri))
                    {
                        s6xESig = (S6xElementSignature)sadS6x.slElementsSignatures[uniqueAddressOri];
                    }
                    else
                    {
                        s6xESig = new S6xElementSignature();
                        s6xESig.UniqueKey = sadS6x.getNewElementSignatureUniqueKey();
                    }

                    s6xESig.SignatureLabel = elementSignatureLabelTextBox.Text;
                    s6xESig.SignatureComments = elementSignatureCommentsTextBox.Text;
                    s6xESig.Signature = elementSignatureSigTextBox.Text;
                    s6xESig.Skip = elementSignatureSkipCheckBox.Checked;
                    s6xESig.Forced = elementSignatureForcedCheckBox.Checked;

                    if (sadS6x.slElementsSignatures.ContainsKey(s6xESig.UniqueKey)) sadS6x.slElementsSignatures[s6xESig.UniqueKey] = s6xESig;
                    else sadS6x.slElementsSignatures.Add(s6xESig.UniqueKey, s6xESig);
                    sadS6x.isSaved = false;

                    if (tnCateg.Nodes.ContainsKey(s6xESig.UniqueKey))
                    {
                        tnNode = tnCateg.Nodes[s6xESig.UniqueKey];
                    }
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xESig.UniqueKey;
                    }
                    tnNode.Text = s6xESig.SignatureLabel;
                    tnNode.ToolTipText = s6xESig.SignatureComments;

                    s6xESig = null;
                    break;
            }

            // To Clean Invalid Address ClipBoard Array
            if (isClipBoardElem && alClipBoardTempUniqueAddresses.Contains(uniqueAddressOri))
            {
                alClipBoardTempUniqueAddresses.Remove(uniqueAddressOri);
                elemAddressTextBox.ForeColor = elemBankTextBox.ForeColor;
            }

            if (tnNode != null)
            {
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Purple;
                if (!bDuplicate)
                {
                    if (!tnCateg.Nodes.ContainsKey(tnNode.Name))
                    {
                        tnCateg.Nodes.Add(tnNode);
                        setElementsTreeCategLabel(tnCateg.Name);
                    }
                }

                lastElemTreeNode = null;
                nextElemTreeNode = tnNode;

                //disassemblyToolStripMenuItem.Enabled = false;
                //outputToolStripMenuItem.Enabled = false;

                showElem();
            }

            tnCateg = null;
            tnNode = null;
        }
        
        private void renameElem(TreeNode tnNode, string label)
        {
            if (tnNode == null) return;

            if (tnNode.Parent == null) return;

            switch (getCategName(tnNode))
            {
                case "TABLES":
                    tableLabelTextBox.Text = label;
                    break;
                case "FUNCTIONS":
                    functionLabelTextBox.Text = label;
                    break;
                case "SCALARS":
                    scalarLabelTextBox.Text = label;
                    break;
                case "STRUCTURES":
                    structureLabelTextBox.Text = label;
                    break;
                case "ROUTINES":
                    routineLabelTextBox.Text = label;
                    break;
                case "OPERATIONS":
                    opeLabelTextBox.Text = label;
                    break;
                case "SIGNATURES":
                    signatureLabelTextBox.Text = label;
                    break;
                case "ELEMSSIGNATURES":
                    elementSignatureLabelTextBox.Text = label;
                    break;
            }

            if (isDuplicate(tnNode)) elemsTreeView.Tag = tnNode.Parent.Parent;
            else elemsTreeView.Tag = tnNode.Parent;
            
            updateElem();

            tnNode = null;
        }

        private void deleteElem(TreeNode tnNode, bool forceDeletion, bool keepNode, bool removeScalersOnTables)
        {
            string categ = string.Empty;
            bool deleteNode = true;
            bool bDuplicate = false;

            if (tnNode == null) return;

            if (tnNode.Parent == null) return;

            bDuplicate = isDuplicate(tnNode);
            categ = getCategName(tnNode);
            switch (categ)
            {
                case "TABLES":
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupTables.ContainsKey(tnNode.Name))
                        {
                            sadS6x.slDupTables.Remove(tnNode.Name);
                            sadS6x.isSaved = false;
                        }
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(tnNode.Name))
                        {
                            if (!forceDeletion && !((S6xTable)sadS6x.slTables[tnNode.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(tnNode.Name))
                                    {
                                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[tnNode.Name]).isTable)
                                        {
                                            deleteNode = false;
                                            sadS6x.slTables[tnNode.Name] = new S6xTable((CalibrationElement)sadBin.Calibration.slCalibrationElements[tnNode.Name]);
                                        }
                                    }
                                    else if (sadBin.Calibration.slExtTables.ContainsKey(tnNode.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slTables[tnNode.Name] = new S6xTable((Table)sadBin.Calibration.slExtTables[tnNode.Name], sadBin.getBankBinAddress(((S6xTable)sadS6x.slTables[tnNode.Name]).BankNum));
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slTables.Remove(tnNode.Name);
                            }
                            else
                            {
                                tnNode.Text = ((S6xTable)sadS6x.slTables[tnNode.Name]).Label;
                                tnNode.ToolTipText = ((S6xTable)sadS6x.slTables[tnNode.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                        }
                    }
                    break;
                case "FUNCTIONS":
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupFunctions.ContainsKey(tnNode.Name))
                        {
                            // Scalers Cleaning on Tables, by using main Item
                            if (removeScalersOnTables)
                            {
                                foreach (S6xTable s6xTable in sadS6x.slTables.Values)
                                {
                                    if (s6xTable.ColsScalerAddress == tnNode.Name || s6xTable.RowsScalerAddress == tnNode.Name)
                                    {
                                        if (s6xTable.ColsScalerAddress == tnNode.Name)
                                        {
                                            s6xTable.ColsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[tnNode.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                                        }
                                        if (s6xTable.RowsScalerAddress == tnNode.Name)
                                        {
                                            s6xTable.RowsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[tnNode.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                                        }
                                    }
                                }
                                foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
                                {
                                    if (s6xTable.ColsScalerAddress == tnNode.Name || s6xTable.RowsScalerAddress == tnNode.Name)
                                    {
                                        if (s6xTable.ColsScalerAddress == tnNode.Name)
                                        {
                                            s6xTable.ColsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[tnNode.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                                        }
                                        if (s6xTable.RowsScalerAddress == tnNode.Name)
                                        {
                                            s6xTable.RowsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[tnNode.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                                        }
                                    }
                                }
                            }
                            
                            sadS6x.slDupFunctions.Remove(tnNode.Name);
                            sadS6x.isSaved = false;
                            // For Scalers Refresh
                            tnNode.Parent.Tag = true;
                        }
                    }
                    else
                    {
                        if (sadS6x.slFunctions.ContainsKey(tnNode.Name))
                        {
                            if (!forceDeletion && !((S6xFunction)sadS6x.slFunctions[tnNode.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(tnNode.Name))
                                    {
                                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[tnNode.Name]).isFunction)
                                        {
                                            deleteNode = false;
                                            sadS6x.slFunctions[tnNode.Name] = new S6xFunction((CalibrationElement)sadBin.Calibration.slCalibrationElements[tnNode.Name]);
                                        }
                                    }
                                    else if (sadBin.Calibration.slExtFunctions.ContainsKey(tnNode.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slFunctions[tnNode.Name] = new S6xFunction((Function)sadBin.Calibration.slExtTables[tnNode.Name], sadBin.getBankBinAddress(((S6xFunction)sadS6x.slFunctions[tnNode.Name]).BankNum));
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slFunctions.Remove(tnNode.Name);
                                // Scalers Cleaning on Tables
                                if (removeScalersOnTables)
                                {
                                    foreach (S6xTable s6xTable in sadS6x.slTables.Values)
                                    {
                                        if (s6xTable.ColsScalerAddress == tnNode.Name || s6xTable.RowsScalerAddress == tnNode.Name)
                                        {
                                            if (s6xTable.ColsScalerAddress == tnNode.Name)
                                            {
                                                s6xTable.ColsScalerAddress = string.Empty;
                                                s6xTable.ColsScalerXdfUniqueId = string.Empty;
                                            }
                                            if (s6xTable.RowsScalerAddress == tnNode.Name)
                                            {
                                                s6xTable.RowsScalerAddress = string.Empty;
                                                s6xTable.RowsScalerXdfUniqueId = string.Empty;
                                            }
                                        }
                                    }
                                    foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
                                    {
                                        if (s6xTable.ColsScalerAddress == tnNode.Name || s6xTable.RowsScalerAddress == tnNode.Name)
                                        {
                                            if (s6xTable.ColsScalerAddress == tnNode.Name)
                                            {
                                                s6xTable.ColsScalerAddress = string.Empty;
                                                s6xTable.ColsScalerXdfUniqueId = string.Empty;
                                            }
                                            if (s6xTable.RowsScalerAddress == tnNode.Name)
                                            {
                                                s6xTable.RowsScalerAddress = string.Empty;
                                                s6xTable.RowsScalerXdfUniqueId = string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tnNode.Text = ((S6xFunction)sadS6x.slFunctions[tnNode.Name]).Label;
                                tnNode.ToolTipText = ((S6xFunction)sadS6x.slFunctions[tnNode.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                            // For Scalers Refresh
                            tnNode.Parent.Tag = true;
                        }
                    }
                    break;
                case "SCALARS":
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupScalars.ContainsKey(tnNode.Name))
                        {
                            sadS6x.slDupScalars.Remove(tnNode.Name);
                            sadS6x.isSaved = false;
                        }
                    }
                    else
                    {
                        if (sadS6x.slScalars.ContainsKey(tnNode.Name))
                        {
                            if (!forceDeletion && !((S6xScalar)sadS6x.slScalars[tnNode.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(tnNode.Name))
                                    {
                                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[tnNode.Name]).isScalar)
                                        {
                                            deleteNode = false;
                                            sadS6x.slScalars[tnNode.Name] = new S6xScalar((CalibrationElement)sadBin.Calibration.slCalibrationElements[tnNode.Name]);
                                        }
                                    }
                                    else if (sadBin.Calibration.slExtScalars.ContainsKey(tnNode.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slScalars[tnNode.Name] = new S6xScalar((Scalar)sadBin.Calibration.slExtScalars[tnNode.Name], sadBin.getBankBinAddress(((S6xScalar)sadS6x.slScalars[tnNode.Name]).BankNum));
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slScalars.Remove(tnNode.Name);
                            }
                            else
                            {
                                tnNode.Text = ((S6xScalar)sadS6x.slScalars[tnNode.Name]).Label;
                                tnNode.ToolTipText = ((S6xScalar)sadS6x.slScalars[tnNode.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                        }
                    }
                    break;
                case "STRUCTURES":
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupStructures.ContainsKey(tnNode.Name))
                        {
                            sadS6x.slDupStructures.Remove(tnNode.Name);
                            sadS6x.isSaved = false;
                        }
                    }
                    else
                    {
                        if (sadS6x.slStructures.ContainsKey(tnNode.Name))
                        {
                            if (!forceDeletion && !((S6xStructure)sadS6x.slStructures[tnNode.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slExtStructures.ContainsKey(tnNode.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slStructures[tnNode.Name] = new S6xStructure((Structure)sadBin.Calibration.slExtStructures[tnNode.Name]);
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slStructures.Remove(tnNode.Name);
                            }
                            else
                            {
                                tnNode.Text = ((S6xStructure)sadS6x.slStructures[tnNode.Name]).Label;
                                tnNode.ToolTipText = ((S6xStructure)sadS6x.slStructures[tnNode.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                        }
                    }
                    break;
                case "ROUTINES":
                    if (sadS6x.slRoutines.ContainsKey(tnNode.Name))
                    {
                        if (!forceDeletion && !((S6xRoutine)sadS6x.slRoutines[tnNode.Name]).Store)
                        {
                            if (sadBin != null)
                            {
                                if (sadBin.Calibration.slCalls.ContainsKey(tnNode.Name))
                                {
                                    deleteNode = false;

                                    Routine rRoutine = null;
                                    if (((Call)sadBin.Calibration.slCalls[tnNode.Name]).isRoutine) rRoutine = (Routine)sadBin.Calibration.slRoutines[tnNode.Name];

                                    sadS6x.slRoutines[tnNode.Name] = new S6xRoutine((Call)sadBin.Calibration.slCalls[tnNode.Name], rRoutine);

                                    rRoutine = null;
                                }
                            }
                        }
                        if (deleteNode)
                        {
                            sadS6x.slRoutines.Remove(tnNode.Name);
                        }
                        else
                        {
                            tnNode.Text = ((S6xRoutine)sadS6x.slRoutines[tnNode.Name]).Label;
                            tnNode.ToolTipText = ((S6xRoutine)sadS6x.slRoutines[tnNode.Name]).Comments;
                        }
                        sadS6x.isSaved = false;
                    }
                    break;
                case "OPERATIONS":
                    if (sadS6x.slOperations.ContainsKey(tnNode.Name)) sadS6x.slOperations.Remove(tnNode.Name);
                    sadS6x.isSaved = false;
                    break;
                case "REGISTERS":
                    if (sadS6x.slRegisters.ContainsKey(tnNode.Name)) sadS6x.slRegisters.Remove(tnNode.Name);
                    sadS6x.isSaved = false;
                    break;
                case "OTHER":
                    if (sadS6x.slOtherAddresses.ContainsKey(tnNode.Name)) sadS6x.slOtherAddresses.Remove(tnNode.Name);
                    sadS6x.isSaved = false;
                    break;
                case "SIGNATURES":
                    if (sadS6x.slSignatures.ContainsKey(tnNode.Name)) sadS6x.slSignatures.Remove(tnNode.Name);
                    sadS6x.isSaved = false;
                    break;
                case "ELEMSSIGNATURES":
                    if (sadS6x.slElementsSignatures.ContainsKey(tnNode.Name))
                    {
                        if (((S6xElementSignature)sadS6x.slElementsSignatures[tnNode.Name]).Forced) deleteNode = false;
                    }
                    if (deleteNode)
                    {
                        sadS6x.slElementsSignatures.Remove(tnNode.Name);
                    }
                    else
                    {
                        tnNode.Text = ((S6xElementSignature)sadS6x.slElementsSignatures[tnNode.Name]).SignatureLabel;
                        tnNode.ToolTipText = ((S6xElementSignature)sadS6x.slElementsSignatures[tnNode.Name]).SignatureComments;
                    }
                    sadS6x.isSaved = false;
                    break;
            }

            if (deleteNode && alClipBoardTempUniqueAddresses.Contains(tnNode.Name)) alClipBoardTempUniqueAddresses.Remove(tnNode.Name);

            if (keepNode) return;

            if (deleteNode)
            {
                nextElemTreeNode = null;
                elemPanel.Visible = false;

                tnNode.Parent.Nodes.Remove(tnNode);
                setElementsTreeCategLabel(categ);
            }
            else
            {
                nextElemTreeNode = tnNode;

                tnNode.ForeColor = tnNode.Parent.ForeColor;

                showElem();
            }

            tnNode = null;
        }

        private void tableColsScalerLabel_Click(object sender, EventArgs e)
        {
            if (tableColsScalerButton.Tag != null)
            {
                TreeNode tnNode = elemsTreeView.Nodes["FUNCTIONS"].Nodes[((S6xFunction)tableColsScalerButton.Tag).UniqueAddress];
                if (tnNode != null)
                {
                    if (((S6xFunction)tableColsScalerButton.Tag).DuplicateNum == 0)
                    {
                        functionBackButton.Tag = nextElemTreeNode;
                        elemsTreeView.SelectedNode = tnNode;
                    }
                    else
                    {
                        tnNode = tnNode.Nodes[((S6xFunction)tableColsScalerButton.Tag).DuplicateAddress];
                        if (tnNode != null)
                        {
                            functionBackButton.Tag = nextElemTreeNode;
                            elemsTreeView.SelectedNode = tnNode;
                        }
                    }
                }
                tnNode = null;
            }
        }

        private void tableRowsScalerLabel_Click(object sender, EventArgs e)
        {
            if (tableRowsScalerButton.Tag != null)
            {
                TreeNode tnNode = elemsTreeView.Nodes["FUNCTIONS"].Nodes[((S6xFunction)tableRowsScalerButton.Tag).UniqueAddress];
                if (tnNode != null)
                {
                    if (((S6xFunction)tableRowsScalerButton.Tag).DuplicateNum == 0)
                    {
                        functionBackButton.Tag = nextElemTreeNode;
                        elemsTreeView.SelectedNode = tnNode;
                    }
                    else
                    {
                        tnNode = tnNode.Nodes[((S6xFunction)tableRowsScalerButton.Tag).DuplicateAddress];
                        if (tnNode != null)
                        {
                            functionBackButton.Tag = nextElemTreeNode;
                            elemsTreeView.SelectedNode = tnNode;
                        }
                    }
                }
                tnNode = null;
            }
        }

        private void functionBackButton_Click(object sender, EventArgs e)
        {
            // Back to Table
            if (functionResetButton.Tag != null)
            {
                elemsTreeView.SelectedNode = (TreeNode)functionResetButton.Tag;
                functionResetButton.Tag = null;
            }
        }

        private void decimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showElemData();
        }

        private void decimalNotConvertedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showElemData();
        }

        private void reverseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showElemData();
        }

        private void convertToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            convertToolStripMenuItem.Tag = e.ClickedItem.Tag;
            showElemData((RepositoryConversionItem)convertToolStripMenuItem.Tag, (RepositoryConversionItem)convertInputToolStripMenuItem.Tag);
        }

        private void convertInputToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            convertInputToolStripMenuItem.Tag = e.ClickedItem.Tag;
            showElemData((RepositoryConversionItem)convertToolStripMenuItem.Tag, (RepositoryConversionItem)convertInputToolStripMenuItem.Tag);
        }

        private void s6xPropertiesUpdateButton_Click(object sender, EventArgs e)
        {
            updateProperties();
        }

        private void scalarUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void functionUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void tableUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void structureUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void routineUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void opeUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void regUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void otherUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void signatureUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void elementSignatureUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void s6xPropertiesResetButton_Click(object sender, EventArgs e)
        {
            showProperties();
        }

        private void scalarResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemScalarPropertiesTabPage);
            }
        }

        private void functionResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemFunctionPropertiesTabPage);
            }
        }

        private void tableResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemTablePropertiesTabPage);
            }
        }

        private void structureResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemStructurePropertiesTabPage);
            }
        }

        private void routineResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemRoutineTabPage);
            }
        }

        private void opeResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemOpeTabPage);
            }
        }

        private void regResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemRegisterTabPage);
            }
        }

        private void otherResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemOtherTabPage);
            }
        }

        private void signatureResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemSignatureTabPage);
            }
        }

        private void elementSignatureResetButton_Click(object sender, EventArgs e)
        {
            if (nextElemTreeNode != null)
            {
                showElemProperties();
            }
            else if (elemsTreeView.Tag != null)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemElemSignatureTabPage);
            }
        }

        private void elemBankTextBox_Enter(object sender, EventArgs e)
        {
            if (!elemBankTextBox.ReadOnly && elemBankTextBox.Text == "Bank") elemBankTextBox.Text = string.Empty;
        }

        private void elemAddressTextBox_Enter(object sender, EventArgs e)
        {
            if (!elemAddressTextBox.ReadOnly && elemAddressTextBox.Text == "Addr") elemAddressTextBox.Text = string.Empty;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnNode = null;
            bool bDuplicate = false;

            tnNode = elemsTreeView.SelectedNode;
            
            if (tnNode == null) return;

            if (tnNode.Parent == null) return;

            bDuplicate = isDuplicate(tnNode);

            switch (getCategName(tnNode))
            {
                case "TABLES":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, bDuplicate ? sadS6x.slDupTables[tnNode.Name] : sadS6x.slTables[tnNode.Name]);
                    break;
                case "FUNCTIONS":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, bDuplicate ? sadS6x.slDupFunctions[tnNode.Name] : sadS6x.slFunctions[tnNode.Name]);
                    break;
                case "SCALARS":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, bDuplicate ? sadS6x.slDupScalars[tnNode.Name] : sadS6x.slScalars[tnNode.Name]);
                    break;
                case "STRUCTURES":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, ((S6xStructure)(bDuplicate ? sadS6x.slDupStructures[tnNode.Name] : sadS6x.slStructures[tnNode.Name])).ClipBoardClone());
                    break;
                case "ROUTINES":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slRoutines[tnNode.Name]);
                    break;
                case "OPERATIONS":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slOperations[tnNode.Name]);
                    break;
                case "REGISTERS":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slRegisters[tnNode.Name]);
                    break;
                case "OTHER":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slOtherAddresses[tnNode.Name]);
                    break;
                case "SIGNATURES":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slSignatures[tnNode.Name]);
                    break;
                case "ELEMSSIGNATURES":
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slElementsSignatures[tnNode.Name]);
                    break;
            }
            tnNode = null;
        }

        private void copySigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnNode = null;
            S6xRoutine s6xRoutine = null;
            S6xSignature s6xSignature = null;
            Operation[] ops = null;

            tnNode = elemsTreeView.SelectedNode;

            if (tnNode == null) return;

            if (tnNode.Parent == null) return;

            switch (tnNode.Parent.Name)
            {
                case "ROUTINES":
                    s6xRoutine = (S6xRoutine)sadS6x.slRoutines[tnNode.Name];
                    break;
                default:
                    return;
            }

            tnNode = null;

            s6xSignature = new S6xSignature();
            s6xSignature.Label = s6xRoutine.Label;
            s6xSignature.ShortLabel = s6xRoutine.ShortLabel;
            s6xSignature.Comments = s6xRoutine.Comments;
            s6xSignature.OutputComments = s6xRoutine.OutputComments;

            if (s6xRoutine.InputArguments != null)
            {
                s6xSignature.InputArguments = new S6xRoutineInputArgument[s6xRoutine.InputArguments.Length];
                for (int i = 0; i < s6xRoutine.InputArguments.Length; i++) s6xSignature.InputArguments[i] = s6xRoutine.InputArguments[i].Clone();
            }
            if (s6xRoutine.InputStructures != null)
            {
                s6xSignature.InputStructures = new S6xRoutineInputStructure[s6xRoutine.InputStructures.Length];
                for (int i = 0; i < s6xRoutine.InputStructures.Length; i++) s6xSignature.InputStructures[i] = s6xRoutine.InputStructures[i].Clone();
            }
            if (s6xRoutine.InputTables != null)
            {
                s6xSignature.InputTables = new S6xRoutineInputTable[s6xRoutine.InputTables.Length];
                for (int i = 0; i < s6xRoutine.InputTables.Length; i++) s6xSignature.InputTables[i] = s6xRoutine.InputTables[i].Clone();
            }
            if (s6xRoutine.InputFunctions != null)
            {
                s6xSignature.InputFunctions = new S6xRoutineInputFunction[s6xRoutine.InputFunctions.Length];
                for (int i = 0; i < s6xRoutine.InputFunctions.Length; i++) s6xSignature.InputFunctions[i] = s6xRoutine.InputFunctions[i].Clone();
            }
            if (s6xRoutine.InputScalars != null)
            {
                s6xSignature.InputScalars = new S6xRoutineInputScalar[s6xRoutine.InputScalars.Length];
                for (int i = 0; i < s6xRoutine.InputScalars.Length; i++) s6xSignature.InputScalars[i] = s6xRoutine.InputScalars[i].Clone();
            }

            s6xSignature.Skip = false;
            s6xSignature.Signature = string.Empty;

            ops = sadBin.getElementRelatedOps(s6xRoutine.UniqueAddress, false);

            s6xRoutine = null;

            foreach (Operation ope in ops)
            {
                if (ope == null) continue;
                s6xSignature.Signature += ope.OriginalOp + "\r\n";
                if (ope.CallArgsNum > 0) s6xSignature.Signature += ope.CallArgs + "\r\n";
            }
            ops = null;

            Clipboard.SetData(SADDef.S6xClipboardFormat, s6xSignature);

            s6xSignature = null;
        }

        private void copyXdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnNode = null;
            object xdfObject = null;

            tnNode = elemsTreeView.SelectedNode;

            if (tnNode == null) return;

            if (tnNode.Parent == null) return;

            switch (getCategName(tnNode))
            {
                case "RESERVED":
                    ReservedAddress resAdr = null;    
                    switch (Convert.ToInt32(tnNode.Name.Substring(0, 1)))
                    {
                        case 8:
                            if (sadBin.Bank8 != null) resAdr = (ReservedAddress)sadBin.Bank8.slReserved[tnNode.Name];
                            break;
                        case 1:
                            if (sadBin.Bank1 != null) resAdr = (ReservedAddress)sadBin.Bank1.slReserved[tnNode.Name];
                            break;
                        case 9:
                            if (sadBin.Bank9 != null) resAdr = (ReservedAddress)sadBin.Bank9.slReserved[tnNode.Name];
                            break;
                        case 0:
                            if (sadBin.Bank0 != null) resAdr = (ReservedAddress)sadBin.Bank0.slReserved[tnNode.Name];
                            break;
                    }
                    if (resAdr != null)
                    {
                        if (resAdr.AddressBinInt >= sadS6x.Properties.XdfBaseOffsetInt)
                        {
                            switch (resAdr.Size)
                            {
                                case 1:
                                case 2:
                                    // Ascii not Managed properly in Xdf    
                                    if (resAdr.Type == ReservedAddressType.Ascii) xdfObject = new XdfTable(resAdr, sadS6x.Properties.XdfBaseOffsetInt);
                                    else xdfObject = new XdfScalar(resAdr, sadS6x.Properties.XdfBaseOffsetInt);
                                    break;
                                default:
                                    xdfObject = new XdfTable(resAdr, sadS6x.Properties.XdfBaseOffsetInt);
                                    break;
                            }
                        }
                        resAdr = null;
                    }
                    break;
                case "TABLES":
                    // Trying to find Scalers Unique Xdf Ids / Useful only after import or synchronized export
                    S6xTable s6xTable = null;
                    if (isDuplicate(tnNode)) s6xTable = (S6xTable)sadS6x.slDupTables[tnNode.Name];
                    else s6xTable = (S6xTable)sadS6x.slTables[tnNode.Name];

                    if (s6xTable.AddressBinInt >= sadS6x.Properties.XdfBaseOffsetInt)
                    {
                        if (s6xTable.ColsScalerAddress != null && s6xTable.ColsScalerAddress != string.Empty)
                        {
                            if (sadS6x.slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                            else if (sadS6x.slDupFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadS6x.slDupFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                            else s6xTable.ColsScalerXdfUniqueId = string.Empty;
                            s6xTable.Store = true;
                        }
                        if (s6xTable.RowsScalerAddress != null && s6xTable.RowsScalerAddress != string.Empty)
                        {
                            if (sadS6x.slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                            else if (sadS6x.slDupFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadS6x.slDupFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                            else s6xTable.RowsScalerXdfUniqueId = string.Empty;
                            s6xTable.Store = true;
                        }
                        sadS6x.slTables[s6xTable.UniqueAddress] = s6xTable;
                        if (sadBin != null)
                        {
                            if (sadBin.Calibration.slCalibrationElements.ContainsKey(s6xTable.UniqueAddress))
                            {
                                ((CalibrationElement)sadBin.Calibration.slCalibrationElements[s6xTable.UniqueAddress]).TableElem.S6xTable = s6xTable;
                            }
                        }
                        xdfObject = new XdfTable(s6xTable, sadS6x.Properties.XdfBaseOffsetInt);
                    }
                    s6xTable = null;
                    break;
                case "FUNCTIONS":
                    S6xFunction s6xFunction = null;
                    if (isDuplicate(tnNode)) s6xFunction = (S6xFunction)sadS6x.slDupFunctions[tnNode.Name];
                    else s6xFunction = (S6xFunction)sadS6x.slFunctions[tnNode.Name];
                    if (s6xFunction.AddressBinInt >= sadS6x.Properties.XdfBaseOffsetInt)
                    {
                        xdfObject = new XdfFunction(s6xFunction, sadS6x.Properties.XdfBaseOffsetInt);
                    }
                    s6xFunction = null;
                    break;
                case "SCALARS":
                    S6xScalar s6xScalar = null;
                    if (isDuplicate(tnNode)) s6xScalar = (S6xScalar)sadS6x.slDupScalars[tnNode.Name];
                    else s6xScalar = (S6xScalar)sadS6x.slScalars[tnNode.Name];
                    if (s6xScalar.AddressBinInt >= sadS6x.Properties.XdfBaseOffsetInt)
                    {
                        if (s6xScalar.isBitFlags)
                        {
                            if (MessageBox.Show("Scalar contains Bit Flags. Bit Flags should be copied one by one.\r\nCopy Scalar to paste Xdf Scalar ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                xdfObject = new XdfScalar(s6xScalar, sadS6x.Properties.XdfBaseOffsetInt);
                            }
                        }
                        else
                        {
                            xdfObject = new XdfScalar(s6xScalar, sadS6x.Properties.XdfBaseOffsetInt);
                        }
                    }
                    s6xScalar = null;
                    break;
            }

            if (xdfObject != null)
            {
                MemoryStream mStr = new MemoryStream();
                ToolsXml.SerializeXdf(ref mStr, xdfObject);
                Clipboard.SetData(SADDef.XdfClipboardFormat, mStr);
                mStr.Close();
                mStr = null;
                xdfObject = null;
            }

            tnNode = null;
        }

        private object getS6xObjectFromXdfData()
        {
            MemoryStream mStream = null;
            object xdfObject = null;
            int bankNum = -1;
            int address = -1;
            int addressBin = -1;
            bool isCalElem = false;

            if (sadBin == null) return null;
            if (sadBin.Calibration == null) return null;
            
            if (!Clipboard.ContainsData(SADDef.XdfClipboardFormat)) return null;

            mStream = (MemoryStream)Clipboard.GetDataObject().GetData(SADDef.XdfClipboardFormat);

            xdfObject = ToolsXml.Deserialize(ref mStream, typeof(XdfScalar));
            if (xdfObject != null)
            {
                addressBin = Tools.binAddressFromXdfAddress(((XdfScalar)xdfObject).getMmedAddress(), sadS6x.Properties.XdfBaseOffsetInt);
                bankNum = sadBin.getBankNum(addressBin);
                if (bankNum < 0) return null;
                address = addressBin - sadBin.getBankBinAddress(bankNum);
                isCalElem = (addressBin >= sadBin.Calibration.AddressBinInt && addressBin <= sadBin.Calibration.AddressBinEndInt);
                return new S6xScalar((XdfScalar) xdfObject, bankNum, address, addressBin, isCalElem);
            }

            mStream.Position = 0;
            xdfObject = ToolsXml.Deserialize(ref mStream, typeof(XdfFunction));
            if (xdfObject != null)
            {
                addressBin = Tools.binAddressFromXdfAddress(((XdfFunction)xdfObject).getMmedAddress(), sadS6x.Properties.XdfBaseOffsetInt);
                bankNum = sadBin.getBankNum(addressBin);
                if (bankNum < 0) return null;
                address = addressBin - sadBin.getBankBinAddress(bankNum);
                isCalElem = (addressBin >= sadBin.Calibration.AddressBinInt && addressBin <= sadBin.Calibration.AddressBinEndInt);
                return new S6xFunction((XdfFunction)xdfObject, bankNum, address, addressBin, isCalElem);
            }

            mStream.Position = 0;
            xdfObject = ToolsXml.Deserialize(ref mStream, typeof(XdfTable));
            if (xdfObject != null)
            {
                addressBin = Tools.binAddressFromXdfAddress(((XdfTable)xdfObject).getMmedAddress(), sadS6x.Properties.XdfBaseOffsetInt);
                bankNum = sadBin.getBankNum(addressBin);
                if (bankNum < 0) return null;
                address = addressBin - sadBin.getBankBinAddress(bankNum);
                isCalElem = (addressBin >= sadBin.Calibration.AddressBinInt && addressBin <= sadBin.Calibration.AddressBinEndInt);
                return new S6xTable((XdfTable)xdfObject, bankNum, address, addressBin, isCalElem, ref sadS6x.slFunctions);
            }

            xdfObject = ToolsXml.Deserialize(ref mStream, typeof(XdfFlag));
            if (xdfObject != null)
            {
                addressBin = Tools.binAddressFromXdfAddress(((XdfFlag)xdfObject).getMmedAddress(), sadS6x.Properties.XdfBaseOffsetInt);
                bankNum = sadBin.getBankNum(addressBin);
                if (bankNum < 0) return null;
                address = addressBin - sadBin.getBankBinAddress(bankNum);
                isCalElem = (addressBin >= sadBin.Calibration.AddressBinInt && addressBin <= sadBin.Calibration.AddressBinEndInt);
                return new S6xScalar((XdfFlag)xdfObject, bankNum, address, addressBin, isCalElem);
            }

            return null;
        }

        private void pasteOverItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent == null) return;

            string[] originalTableScalersAddresses = null;
            object s6xData = null; 
            bool keepScalersOnTables = false;

            if (Clipboard.ContainsData(SADDef.S6xClipboardFormat)) s6xData = Clipboard.GetData(SADDef.S6xClipboardFormat);
            else if (Clipboard.ContainsData(SADDef.XdfClipboardFormat)) s6xData = getS6xObjectFromXdfData();
            if (s6xData == null) return;

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "RESERVED":
                    return;
                case "TABLES":
                    if (isDuplicate(elemsTreeView.SelectedNode))
                    {
                        if (sadS6x.slDupTables.ContainsKey(elemsTreeView.SelectedNode.Name))
                        {
                            originalTableScalersAddresses = new string[] { ((S6xTable)sadS6x.slDupTables[elemsTreeView.SelectedNode.Name]).ColsScalerAddress, ((S6xTable)sadS6x.slDupTables[elemsTreeView.SelectedNode.Name]).RowsScalerAddress };
                        }
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(elemsTreeView.SelectedNode.Name))
                        {
                            originalTableScalersAddresses = new string[] { ((S6xTable)sadS6x.slTables[elemsTreeView.SelectedNode.Name]).ColsScalerAddress, ((S6xTable)sadS6x.slTables[elemsTreeView.SelectedNode.Name]).RowsScalerAddress };
                        }
                    }
                    break;
                case "FUNCTIONS":
                    keepScalersOnTables = (s6xData.GetType().Name == "S6xFunction");
                    break;
            }

            deleteElem(elemsTreeView.SelectedNode, true, true, !keepScalersOnTables);

            pasteElement(elemsTreeView.SelectedNode, originalTableScalersAddresses);

            showElem();
        }

        private void pasteOnItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteElement(null, null);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteElement(null, null);
        }

        private void pasteMultToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Only for Scalars
            pasteElements((e.ClickedItem.Tag == null) ? 0 : (int)e.ClickedItem.Tag);
        }

        // Only for Scalars
        private void pasteElements(int nTimes)
        {
            object s6xData = null;
            string categ = string.Empty;

            if (nTimes < 1) return;
            if (Clipboard.ContainsData(SADDef.S6xClipboardFormat)) s6xData = Clipboard.GetData(SADDef.S6xClipboardFormat);
            if (s6xData == null) return;

            switch (s6xData.GetType().Name)
            {
                case "S6xScalar":
                    // Works only when coming from current session, the original scalar should exist at its address
                    if (!sadS6x.slScalars.ContainsKey(((S6xScalar)s6xData).UniqueAddress)) return;

                    categ = "SCALARS";
                    for (int iTime = 1; iTime <= nTimes; iTime++)
                    {
                        S6xScalar newObject = ((S6xScalar)s6xData).Clone();
                        if (newObject.Byte)
                        {
                            newObject.AddressInt += iTime;
                            newObject.AddressBinInt += iTime;
                        }
                        else
                        {
                            newObject.AddressInt += iTime * 2;
                            newObject.AddressBinInt += iTime * 2;
                        }
                        if (sadS6x.slScalars.ContainsKey(newObject.UniqueAddress)) continue;
                        if (sadS6x.slFunctions.ContainsKey(newObject.UniqueAddress)) continue;
                        if (sadS6x.slTables.ContainsKey(newObject.UniqueAddress)) continue;
                        if (sadS6x.slStructures.ContainsKey(newObject.UniqueAddress)) continue;
                        if (sadS6x.slRoutines.ContainsKey(newObject.UniqueAddress)) continue;
                        if (sadS6x.slOperations.ContainsKey(newObject.UniqueAddress)) continue;

                        sadS6x.slScalars.Add(newObject.UniqueAddress, newObject);
                        sadS6x.isSaved = false;

                        if (elemsTreeView.Nodes[categ].Nodes.ContainsKey(newObject.UniqueAddress)) continue;

                        TreeNode tnNode = new TreeNode();
                        tnNode.Name = newObject.UniqueAddress;
                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        tnNode.ForeColor = Color.Purple;
                        tnNode.Text = newObject.Label;
                        tnNode.ToolTipText = newObject.Comments;
                        elemsTreeView.Nodes[categ].Nodes.Add(tnNode);
                    }
                    setElementsTreeCategLabel(categ);
                    break;
                default:
                    return;
            }
        }

        private void pasteElement(TreeNode overNode, string[] originalTableScalersAddresses)
        {
            object s6xData = null;
            TreeNode tnNode = null;
            string categ = string.Empty;
            string uniqueAddress = string.Empty;
            string dupAddress = string.Empty;
            string label = string.Empty;
            string shortLabel = string.Empty;
            string comments = string.Empty;
            bool isS6xClipBoard = false;
            bool isXdfData = false;
            string sMessage = string.Empty;
            S6xStructure s6xStruct = null;
            string overUniqueAddress = string.Empty;
            int overBankNum = -1;
            int overAddress = -1;
            int overDuplicateNum = -1;
            string overCateg = string.Empty;

            if (Clipboard.ContainsData(SADDef.S6xClipboardFormat))
            {
                s6xData = Clipboard.GetData(SADDef.S6xClipboardFormat);
                isS6xClipBoard = true;
            }
            else if (Clipboard.ContainsData(SADDef.XdfClipboardFormat))
            {
                s6xData = getS6xObjectFromXdfData();
                isS6xClipBoard = false;
                isXdfData = true;
            }

            if (overNode != null)
            {
                if (isDuplicate(overNode))
                {
                    overUniqueAddress = overNode.Parent.Name;
                    try { overDuplicateNum = Convert.ToInt32(overNode.Name.Split(' ')[overNode.Name.Split(' ').Length - 1]); }
                    catch { }
                }
                else
                {
                    overUniqueAddress = overNode.Name;
                }

                try { overBankNum = Convert.ToInt32(overUniqueAddress.Substring(0, 1)); }
                catch { }
                try { overAddress = Convert.ToInt32(overUniqueAddress.Substring(1).Trim()); }
                catch { }
                try { overCateg = getCategName(overNode);}
                catch { }
            }

            if (s6xData == null) return;

            switch (s6xData.GetType().Name)
            {
                case "S6xTable":
                    categ = "TABLES";
                    if (isXdfData && overNode == null)
                    {
                        tnNode = checkDuplicate(((S6xTable)s6xData).UniqueAddress, false);
                        if (tnNode != null)
                        {
                            if (tnNode.Parent == null)
                            // Block Reserved and Calibration Load Elements
                            {
                                tnNode = null;
                                s6xData = null;
                                return;
                            }
                            else if (getCategName(tnNode) != categ)
                            // Type Conflict
                            {
                                sMessage = "Object already exists with another type.\r\n";
                                sMessage += "\t" + tnNode.Text + "\tfound in " + tnNode.Parent.Text + "\r\n";
                                sMessage += "Please remove it before proceeding.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tnNode = null;
                                s6xData = null;
                                return;
                            }
                            tnNode = null;
                        }
                    }
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xTable)s6xData).BankNum = overBankNum;
                        ((S6xTable)s6xData).AddressInt = overAddress;
                        ((S6xTable)s6xData).DuplicateNum = overDuplicateNum;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xTable)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xTable)s6xData).UniqueAddress)) ((S6xTable)s6xData).AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        ((S6xTable)s6xData).AddressBinInt = ((S6xTable)s6xData).AddressInt + sadBin.getBankBinAddress(((S6xTable)s6xData).BankNum);
                        ((S6xTable)s6xData).isCalibrationElement = sadBin.isCalibrationAddress(((S6xTable)s6xData).AddressBinInt);
                    }
                    ((S6xTable)s6xData).Store = true;
                    uniqueAddress = ((S6xTable)s6xData).UniqueAddress;
                    label = ((S6xTable)s6xData).Label;
                    comments = ((S6xTable)s6xData).Comments;

                    if (originalTableScalersAddresses != null)
                    {
                        ((S6xTable)s6xData).ColsScalerAddress = originalTableScalersAddresses[0];
                        ((S6xTable)s6xData).RowsScalerAddress = originalTableScalersAddresses[1];
                    }

                    if (((S6xTable)s6xData).DuplicateNum > 0 && sadS6x.slTables.ContainsKey(uniqueAddress))
                    {
                        dupAddress = ((S6xTable)s6xData).DuplicateAddress;
                        if (sadS6x.slDupTables.ContainsKey(dupAddress)) sadS6x.slDupTables[dupAddress] = s6xData;
                        else sadS6x.slDupTables.Add(dupAddress, s6xData);
                    }
                    else
                    {
                        ((S6xTable)s6xData).DuplicateNum = 0;
                        if (sadS6x.slTables.ContainsKey(uniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            shortLabel = ((S6xTable)sadS6x.slTables[uniqueAddress]).ShortLabel;
                            if (shortLabel != null && shortLabel != string.Empty) ((S6xTable)s6xData).ShortLabel = shortLabel;

                            sadS6x.slTables[uniqueAddress] = s6xData;
                        }
                        else sadS6x.slTables.Add(uniqueAddress, s6xData);
                    }
                    break;
                case "S6xFunction":
                    categ = "FUNCTIONS";
                    if (isXdfData && overNode == null)
                    {
                        tnNode = checkDuplicate(((S6xFunction)s6xData).UniqueAddress, false);
                        if (tnNode != null)
                        {
                            if (tnNode.Parent == null)
                            // Block Reserved and Calibration Load Elements
                            {
                                tnNode = null;
                                s6xData = null;
                                return;
                            }
                            else if (getCategName(tnNode) != categ)
                            // Type Conflict
                            {
                                sMessage = "Object already exists with another type.\r\n";
                                sMessage += "\t" + tnNode.Text + "\tfound in " + tnNode.Parent.Text + "\r\n";
                                sMessage += "Please remove it before proceeding.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tnNode = null;
                                s6xData = null;
                                return;
                            }
                            tnNode = null;
                        }
                    }
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xFunction)s6xData).BankNum = overBankNum;
                        ((S6xFunction)s6xData).AddressInt = overAddress;
                        ((S6xFunction)s6xData).DuplicateNum = overDuplicateNum;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xFunction)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xFunction)s6xData).UniqueAddress)) ((S6xFunction)s6xData).AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        ((S6xFunction)s6xData).AddressBinInt = ((S6xFunction)s6xData).AddressInt + sadBin.getBankBinAddress(((S6xFunction)s6xData).BankNum);
                        ((S6xFunction)s6xData).isCalibrationElement = sadBin.isCalibrationAddress(((S6xFunction)s6xData).AddressBinInt);
                    }
                    ((S6xFunction)s6xData).Store = true;
                    uniqueAddress = ((S6xFunction)s6xData).UniqueAddress;
                    label = ((S6xFunction)s6xData).Label;
                    comments = ((S6xFunction)s6xData).Comments;
                    if (((S6xFunction)s6xData).DuplicateNum > 0 && sadS6x.slFunctions.ContainsKey(uniqueAddress))
                    {
                        dupAddress = ((S6xFunction)s6xData).DuplicateAddress;
                        if (sadS6x.slDupFunctions.ContainsKey(dupAddress)) sadS6x.slDupFunctions[dupAddress] = s6xData;
                        else sadS6x.slDupFunctions.Add(dupAddress, s6xData);
                    }
                    else
                    {
                        ((S6xFunction)s6xData).DuplicateNum = 0;
                        if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            shortLabel = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).ShortLabel;
                            if (shortLabel != null && shortLabel != string.Empty) ((S6xFunction)s6xData).ShortLabel = shortLabel;

                            sadS6x.slFunctions[uniqueAddress] = s6xData;
                        }
                        else sadS6x.slFunctions.Add(uniqueAddress, s6xData);
                    }

                    // For Scalers Refresh
                    elemsTreeView.Nodes["FUNCTIONS"].Tag = true;
                    break;
                case "S6xScalar":
                    categ = "SCALARS";
                    if (isXdfData && overNode == null)
                    {
                        tnNode = checkDuplicate(((S6xScalar)s6xData).UniqueAddress, false);
                        if (tnNode != null)
                        {
                            if (tnNode.Parent == null)
                            // Block Reserved and Calibration Load Elements
                            {
                                tnNode = null;
                                s6xData = null;
                                return;
                            }
                            else if (getCategName(tnNode) != categ)
                            // Type Conflict
                            {
                                sMessage = "Object already exists with another type.\r\n";
                                sMessage += "\t" + tnNode.Text + "\tfound in " + tnNode.Parent.Text + "\r\n";
                                sMessage += "Please remove it before proceeding.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tnNode = null;
                                s6xData = null;
                                return;
                            }
                            tnNode = null;
                        }
                    }
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xScalar)s6xData).BankNum = overBankNum;
                        ((S6xScalar)s6xData).AddressInt = overAddress;
                        ((S6xScalar)s6xData).DuplicateNum = overDuplicateNum;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xScalar)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xScalar)s6xData).UniqueAddress)) ((S6xScalar)s6xData).AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        ((S6xScalar)s6xData).AddressBinInt = ((S6xScalar)s6xData).AddressInt + sadBin.getBankBinAddress(((S6xScalar)s6xData).BankNum);
                        ((S6xScalar)s6xData).isCalibrationElement = sadBin.isCalibrationAddress(((S6xScalar)s6xData).AddressBinInt);
                    }
                    ((S6xScalar)s6xData).Store = true;
                    uniqueAddress = ((S6xScalar)s6xData).UniqueAddress;
                    if (((S6xScalar)s6xData).DuplicateNum > 0 && sadS6x.slScalars.ContainsKey(uniqueAddress))
                    {
                        dupAddress = ((S6xScalar)s6xData).DuplicateAddress;
                        if (sadS6x.slDupScalars.ContainsKey(dupAddress))
                        {
                            if (((S6xScalar)s6xData).isBitFlags)
                            // Flag Paste, no Update on Label and Comments, Flag is added
                            {
                                label = ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).Label;
                                comments = ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).Comments;
                                if (((S6xScalar)s6xData).BitFlags != null)
                                {
                                    foreach (S6xBitFlag bF in ((S6xScalar)s6xData).BitFlags) ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).AddBitFlag(bF);
                                }
                            }
                            else
                            {
                                if (((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).isBitFlags)
                                // Prevent loss of existing BitFlags
                                {
                                    ((S6xScalar)s6xData).BitFlags = ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).BitFlags;
                                }
                                sadS6x.slDupScalars[uniqueAddress] = s6xData;
                                label = ((S6xScalar)s6xData).Label;
                                comments = ((S6xScalar)s6xData).Comments;
                            }
                        }
                        else
                        {
                            label = ((S6xScalar)s6xData).Label;
                            comments = ((S6xScalar)s6xData).Comments;
                            sadS6x.slDupScalars.Add(dupAddress, s6xData);
                        }
                    }
                    else
                    {
                        if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            shortLabel = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).ShortLabel;
                            if (shortLabel != null && shortLabel != string.Empty) ((S6xScalar)s6xData).ShortLabel = shortLabel;

                            if (((S6xScalar)s6xData).isBitFlags)
                            // Flag Paste, no Update on Label and Comments, Flag is added
                            {
                                label = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label;
                                comments = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments;
                                if (((S6xScalar)s6xData).BitFlags != null)
                                {
                                    foreach (S6xBitFlag bF in ((S6xScalar)s6xData).BitFlags) ((S6xScalar)sadS6x.slScalars[uniqueAddress]).AddBitFlag(bF);
                                }
                            }
                            else
                            {
                                if (((S6xScalar)sadS6x.slScalars[uniqueAddress]).isBitFlags)
                                // Prevent loss of existing BitFlags
                                {
                                    ((S6xScalar)s6xData).BitFlags = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).BitFlags;
                                }
                                sadS6x.slScalars[uniqueAddress] = s6xData;
                                label = ((S6xScalar)s6xData).Label;
                                comments = ((S6xScalar)s6xData).Comments;
                            }
                        }
                        else
                        {
                            label = ((S6xScalar)s6xData).Label;
                            comments = ((S6xScalar)s6xData).Comments;
                            sadS6x.slScalars.Add(uniqueAddress, s6xData);
                        }
                    }
                    break;
                case "S6xStructureClipBoard":
                    s6xStruct = new S6xStructure((S6xStructureClipBoard)s6xData);
                    categ = "STRUCTURES";
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        s6xStruct.BankNum = overBankNum;
                        s6xStruct.AddressInt = overAddress;
                        s6xStruct.DuplicateNum = overDuplicateNum;
                    }
                    else if (isS6xClipBoard)
                    {
                        s6xStruct.AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(s6xStruct.UniqueAddress)) s6xStruct.AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        s6xStruct.AddressBinInt = s6xStruct.AddressInt + sadBin.getBankBinAddress(s6xStruct.BankNum);
                        s6xStruct.isCalibrationElement = sadBin.isCalibrationAddress(s6xStruct.AddressBinInt);
                    }
                    s6xStruct.Store = true;
                    uniqueAddress = s6xStruct.UniqueAddress;
                    label = s6xStruct.Label;
                    comments = s6xStruct.Comments;
                    if (s6xStruct.DuplicateNum > 0 && sadS6x.slStructures.ContainsKey(uniqueAddress))
                    {
                        dupAddress = s6xStruct.DuplicateAddress;
                        if (sadS6x.slDupStructures.ContainsKey(dupAddress)) sadS6x.slDupStructures[dupAddress] = s6xStruct;
                        else sadS6x.slDupStructures.Add(dupAddress, s6xStruct);
                    }
                    else
                    {
                        s6xStruct.DuplicateNum = 0;
                        if (sadS6x.slStructures.ContainsKey(uniqueAddress)) sadS6x.slStructures[uniqueAddress] = s6xStruct;
                        else sadS6x.slStructures.Add(uniqueAddress, s6xStruct);
                    }
                    s6xStruct = null;
                    break;
                case "S6xRoutine":
                    categ = "ROUTINES";
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xRoutine)s6xData).BankNum = overBankNum;
                        ((S6xRoutine)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xRoutine)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xRoutine)s6xData).UniqueAddress)) ((S6xRoutine)s6xData).AddressInt++;
                    }
                    ((S6xRoutine)s6xData).Store = true;
                    uniqueAddress = ((S6xRoutine)s6xData).UniqueAddress;
                    label = ((S6xRoutine)s6xData).Label;
                    comments = ((S6xRoutine)s6xData).Comments;
                    if (sadS6x.slRoutines.ContainsKey(uniqueAddress)) sadS6x.slRoutines[uniqueAddress] = s6xData;
                    else sadS6x.slRoutines.Add(uniqueAddress, s6xData);
                    break;
                case "S6xOperation":
                    categ = "OPERATIONS";
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xOperation)s6xData).BankNum = overBankNum;
                        ((S6xOperation)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xOperation)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xOperation)s6xData).UniqueAddress)) ((S6xOperation)s6xData).AddressInt++;
                    }
                    uniqueAddress = ((S6xOperation)s6xData).UniqueAddress;
                    label = ((S6xOperation)s6xData).Label;
                    comments = ((S6xOperation)s6xData).Comments;
                    if (sadS6x.slOperations.ContainsKey(uniqueAddress)) sadS6x.slOperations[uniqueAddress] = s6xData;
                    else sadS6x.slOperations.Add(uniqueAddress, s6xData);
                    break;
                case "S6xRegister":
                    categ = "REGISTERS";
                    if (overAddress != -1)
                    {
                        ((S6xRegister)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xRegister)s6xData).AddressInt = 0xffff;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xRegister)s6xData).UniqueAddress)) ((S6xRegister)s6xData).AddressInt--;
                    }
                    ((S6xRegister)s6xData).Store = true;
                    uniqueAddress = ((S6xRegister)s6xData).UniqueAddress;
                    label = ((S6xRegister)s6xData).Label;
                    comments = ((S6xRegister)s6xData).Comments;
                    if (sadS6x.slRegisters.ContainsKey(uniqueAddress)) sadS6x.slRegisters[uniqueAddress] = s6xData;
                    else sadS6x.slRegisters.Add(uniqueAddress, s6xData);
                    break;
                case "S6xOtherAddress":
                    categ = "OTHER";
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xOtherAddress)s6xData).BankNum = overBankNum;
                        ((S6xOtherAddress)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xOtherAddress)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[categ].Nodes.ContainsKey(((S6xOtherAddress)s6xData).UniqueAddress)) ((S6xOtherAddress)s6xData).AddressInt++;
                    }
                    uniqueAddress = ((S6xOtherAddress)s6xData).UniqueAddress;
                    label = ((S6xOtherAddress)s6xData).UniqueAddressHex;
                    comments = ((S6xOtherAddress)s6xData).Comments;
                    if (sadS6x.slOtherAddresses.ContainsKey(uniqueAddress)) sadS6x.slOtherAddresses[uniqueAddress] = s6xData;
                    else sadS6x.slOtherAddresses.Add(uniqueAddress, s6xData);
                    break;
                case "S6xSignature":
                    categ = "SIGNATURES";
                    if (overUniqueAddress != string.Empty) ((S6xSignature)s6xData).UniqueKey = overUniqueAddress;
                    else ((S6xSignature)s6xData).UniqueKey = sadS6x.getNewSignatureUniqueKey();
                    uniqueAddress = ((S6xSignature)s6xData).UniqueKey;
                    label = ((S6xSignature)s6xData).Label;
                    comments = ((S6xSignature)s6xData).Comments;
                    if (sadS6x.slSignatures.ContainsKey(uniqueAddress)) sadS6x.slSignatures[uniqueAddress] = s6xData;
                    else sadS6x.slSignatures.Add(uniqueAddress, s6xData);
                    break;
                case "S6xElementSignature":
                    categ = "ELEMSSIGNATURES";
                    if (overUniqueAddress != string.Empty) ((S6xElementSignature)s6xData).UniqueKey = overUniqueAddress;
                    else ((S6xElementSignature)s6xData).UniqueKey = sadS6x.getNewElementSignatureUniqueKey();
                    ((S6xElementSignature)s6xData).Forced = false;
                    uniqueAddress = ((S6xElementSignature)s6xData).UniqueKey;
                    label = ((S6xElementSignature)s6xData).SignatureLabel;
                    comments = ((S6xElementSignature)s6xData).SignatureComments;
                    if (sadS6x.slElementsSignatures.ContainsKey(uniqueAddress)) sadS6x.slElementsSignatures[uniqueAddress] = s6xData;
                    else sadS6x.slElementsSignatures.Add(uniqueAddress, s6xData);
                    break;
            }
            if (uniqueAddress != string.Empty)
            {
                if (elemsTreeView.Nodes[categ].Nodes.ContainsKey(uniqueAddress))
                {
                    tnNode = elemsTreeView.Nodes[categ].Nodes[uniqueAddress];
                    if (dupAddress != string.Empty)
                    {
                        if (tnNode.Nodes.ContainsKey(dupAddress)) tnNode = tnNode.Nodes[dupAddress];
                    }
                }
                else
                {
                    if (isS6xClipBoard && overNode == null) alClipBoardTempUniqueAddresses.Add(uniqueAddress);
                    tnNode = new TreeNode();
                    tnNode.Name = uniqueAddress;
                    tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    switch (categ)
                    {
                        case "SIGNATURES":
                        case "ELEMSSIGNATURES":
                            break;
                        default:
                            tnNode.ForeColor = Color.Red;
                            break;
                    }
                }
                tnNode.Text = label;
                tnNode.ToolTipText = comments;

                if (elemsTreeView.Nodes[categ].Nodes.ContainsKey(uniqueAddress))
                {
                    tnNode.ForeColor = Color.Purple;
                }
                else
                {
                    if (overNode != null) tnNode.ForeColor = Color.Purple;
                    elemsTreeView.Nodes[categ].Nodes.Add(tnNode);
                    setElementsTreeCategLabel(categ);
                }

                elemsTreeView.SelectedNode = tnNode;
                tnNode = null;

                if (overNode != null && categ != overCateg)
                {
                    overNode.Parent.Nodes.Remove(overNode);
                    setElementsTreeCategLabel(overCateg);
                }
            }

            sadS6x.isSaved = false;
            s6xData = null;
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnNode = null;
            TreeNode tnDup = null;
            string uniqueAddress = string.Empty;
            string duplicateAddress = string.Empty;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            tnNode = elemsTreeView.SelectedNode;

            if (tnNode == null) return;
            if (tnNode.Parent == null) return;

            tnDup = new TreeNode();
            tnDup.Text = tnNode.Text;
            tnDup.ToolTipText = tnNode.ToolTipText;
            tnDup.ContextMenuStrip = tnNode.ContextMenuStrip;

            if (isDuplicate(tnNode))
            {
                duplicateAddress = tnNode.Name;
                tnNode = tnNode.Parent;
            }
            uniqueAddress = tnNode.Name;
            
            switch (getCategName(tnNode))
            {
                case "TABLES":
                    ((S6xTable)sadS6x.slTables[uniqueAddress]).DuplicateNum = 0;
                    ((S6xTable)sadS6x.slTables[uniqueAddress]).Store = true;
                    S6xTable dupTable = null;
                    if (duplicateAddress == string.Empty) dupTable = ((S6xTable)sadS6x.slTables[uniqueAddress]).Clone();
                    else dupTable = ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Clone();
                    dupTable.DuplicateNum = 1; 
                    while (sadS6x.slDupTables.ContainsKey(dupTable.DuplicateAddress)) dupTable.DuplicateNum++;
                    sadS6x.slDupTables.Add(dupTable.DuplicateAddress, dupTable);
                    tnDup.Name = dupTable.DuplicateAddress;
                    dupTable = null;
                    break;
                case "FUNCTIONS":
                    ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).DuplicateNum = 0;
                    ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Store = true;
                    S6xFunction dupFunction = null;
                    if (duplicateAddress == string.Empty) dupFunction = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Clone();
                    else dupFunction = ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Clone();
                    dupFunction.DuplicateNum = 1;
                    while (sadS6x.slDupFunctions.ContainsKey(dupFunction.DuplicateAddress)) dupFunction.DuplicateNum++;
                    sadS6x.slDupFunctions.Add(dupFunction.DuplicateAddress, dupFunction);
                    tnDup.Name = dupFunction.DuplicateAddress;
                    dupFunction = null;
                    break;
                case "SCALARS":
                    ((S6xScalar)sadS6x.slScalars[uniqueAddress]).DuplicateNum = 0;
                    ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Store = true;
                    S6xScalar dupScalar = null;
                    if (duplicateAddress == string.Empty) dupScalar = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Clone();
                    else dupScalar = ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Clone();
                    dupScalar.DuplicateNum = 1;
                    while (sadS6x.slDupScalars.ContainsKey(dupScalar.DuplicateAddress)) dupScalar.DuplicateNum++;
                    sadS6x.slDupScalars.Add(dupScalar.DuplicateAddress, dupScalar);
                    tnDup.Name = dupScalar.DuplicateAddress;
                    dupScalar = null;
                    break;
                case "STRUCTURES":
                    ((S6xStructure)sadS6x.slStructures[uniqueAddress]).DuplicateNum = 0;
                    ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Store = true;
                    S6xStructure dupStructure = null;
                    if (duplicateAddress == string.Empty) dupStructure = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Clone();
                    else dupStructure = ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Clone();
                    dupStructure.DuplicateNum = 1;
                    while (sadS6x.slDupStructures.ContainsKey(dupStructure.DuplicateAddress)) dupStructure.DuplicateNum++;
                    sadS6x.slDupStructures.Add(dupStructure.DuplicateAddress, dupStructure);
                    tnDup.Name = dupStructure.DuplicateAddress;
                    dupStructure = null;
                    break;
                default:
                    tnDup = null;
                    tnNode = null;
                    return;
            }

            tnNode.Nodes.Add(tnDup);

            sadS6x.isSaved = false;

            elemsTreeView.SelectedNode = tnDup;
            elemsTreeView.SelectedNode.ForeColor = Color.Purple;
            tnNode.ForeColor = Color.Purple;

            tnDup = null;
            tnNode = null;
        }

        private void unDuplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnMain = null;
            TreeNode tnDup = null;
            int dupNum = -1;
            string nText = string.Empty;
            string nToolTipText = string.Empty;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            tnDup = elemsTreeView.SelectedNode;

            if (!isDuplicate(tnDup)) return;

            tnMain = tnDup.Parent;

            switch (getCategName(tnMain))
            {
                case "TABLES":
                    S6xTable mainTable = ((S6xTable)sadS6x.slTables[tnMain.Name]);
                    S6xTable dupTable = ((S6xTable)sadS6x.slDupTables[tnDup.Name]);
                    dupNum = dupTable.DuplicateNum;
                    dupTable.DuplicateNum = mainTable.DuplicateNum;
                    mainTable.DuplicateNum = dupNum;

                    sadS6x.slTables[tnMain.Name] = dupTable;
                    sadS6x.slDupTables[tnDup.Name] = mainTable;

                    dupTable = null;
                    mainTable = null;
                    break;
                case "FUNCTIONS":
                    S6xFunction mainFunction = ((S6xFunction)sadS6x.slFunctions[tnMain.Name]);
                    S6xFunction dupFunction = ((S6xFunction)sadS6x.slDupFunctions[tnDup.Name]);
                    
                    // Scalers Update
                    foreach (S6xTable s6xTable in sadS6x.slTables.Values)
                    {
                        if (s6xTable.ColsScalerAddress == mainFunction.UniqueAddress) s6xTable.ColsScalerAddress = dupFunction.DuplicateAddress;
                        else if (s6xTable.ColsScalerAddress == dupFunction.DuplicateAddress) s6xTable.ColsScalerAddress = mainFunction.UniqueAddress;
                        if (s6xTable.RowsScalerAddress == mainFunction.UniqueAddress) s6xTable.RowsScalerAddress = dupFunction.DuplicateAddress;
                        else if (s6xTable.RowsScalerAddress == dupFunction.DuplicateAddress) s6xTable.RowsScalerAddress = mainFunction.UniqueAddress;
                    }
                    foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
                    {
                        if (s6xTable.ColsScalerAddress == mainFunction.UniqueAddress) s6xTable.ColsScalerAddress = dupFunction.DuplicateAddress;
                        else if (s6xTable.ColsScalerAddress == dupFunction.DuplicateAddress) s6xTable.ColsScalerAddress = mainFunction.UniqueAddress;
                        if (s6xTable.RowsScalerAddress == mainFunction.UniqueAddress) s6xTable.RowsScalerAddress = dupFunction.DuplicateAddress;
                        else if (s6xTable.RowsScalerAddress == dupFunction.DuplicateAddress) s6xTable.RowsScalerAddress = mainFunction.UniqueAddress;
                    }
                    
                    dupNum = dupFunction.DuplicateNum;
                    dupFunction.DuplicateNum = mainFunction.DuplicateNum;
                    mainFunction.DuplicateNum = dupNum;

                    sadS6x.slFunctions[tnMain.Name] = dupFunction;
                    sadS6x.slDupFunctions[tnDup.Name] = mainFunction;

                    dupFunction = null;
                    mainFunction = null;
                    break;
                case "SCALARS":
                    S6xScalar mainScalar = ((S6xScalar)sadS6x.slScalars[tnMain.Name]);
                    S6xScalar dupScalar = ((S6xScalar)sadS6x.slDupScalars[tnDup.Name]);
                    dupNum = dupScalar.DuplicateNum;
                    dupScalar.DuplicateNum = mainScalar.DuplicateNum;
                    mainScalar.DuplicateNum = dupNum;

                    sadS6x.slScalars[tnMain.Name] = dupScalar;
                    sadS6x.slDupScalars[tnDup.Name] = mainScalar;

                    dupScalar = null;
                    mainScalar = null;
                    break;
                case "STRUCTURES":
                    S6xStructure mainStructure = ((S6xStructure)sadS6x.slStructures[tnMain.Name]);
                    S6xStructure dupStructure = ((S6xStructure)sadS6x.slDupStructures[tnDup.Name]);
                    dupNum = dupStructure.DuplicateNum;
                    dupStructure.DuplicateNum = mainStructure.DuplicateNum;
                    mainStructure.DuplicateNum = dupNum;

                    sadS6x.slStructures[tnMain.Name] = dupStructure;
                    sadS6x.slDupStructures[tnDup.Name] = mainStructure;

                    dupStructure = null;
                    mainStructure = null;
                    break;
                default:
                    tnDup = null;
                    tnMain = null;
                    return;
            }

            nText = tnDup.Text;
            nToolTipText = tnDup.ToolTipText;
            tnDup.Text = tnMain.Text;
            tnDup.ToolTipText = tnMain.ToolTipText;
            tnMain.Text = nText;
            tnMain.ToolTipText = nToolTipText;

            sadS6x.isSaved = false;

            elemsTreeView.SelectedNode = tnMain;
            elemsTreeView.SelectedNode.ForeColor = Color.Purple;
            tnDup.ForeColor = Color.Purple;

            tnDup = null;
            tnMain = null;
        }

        private void importSignaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SADS6x sigS6x = null;
            TreeNode tnCateg = null;
            TreeNode tnNode = null;
            
            if (openFileDialogS6x.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(openFileDialogS6x.FileName)) return;

            try
            {
                sigS6x = new SADS6x(openFileDialogS6x.FileName);

                if (!sigS6x.isValid) throw new Exception();

                if (sigS6x.slSignatures != null)
                {
                    tnCateg = elemsTreeView.Nodes["SIGNATURES"];

                    foreach (S6xSignature impSignature in sigS6x.slSignatures.Values)
                    {
                        S6xSignature clone = impSignature.Clone();
                        clone.UniqueKey = string.Empty;
                        foreach (S6xSignature s6xSignature in sadS6x.slSignatures.Values)
                        {
                            if (s6xSignature.ShortLabel == clone.ShortLabel)
                            {
                                clone.UniqueKey = s6xSignature.UniqueKey;
                                break;
                            }
                        }
                        if (clone.UniqueKey == string.Empty)
                        {
                            clone.UniqueKey = sadS6x.getNewSignatureUniqueKey();
                            sadS6x.slSignatures.Add(clone.UniqueKey, clone);

                        }
                        else
                        {
                            sadS6x.slSignatures[clone.UniqueKey] = clone;
                        }
                        sadS6x.isSaved = false;

                        if (tnCateg.Nodes.ContainsKey(clone.UniqueKey))
                        {
                            tnNode = tnCateg.Nodes[clone.UniqueKey];
                        }
                        else
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = clone.UniqueKey;

                            tnCateg.Nodes.Add(tnNode);
                            setElementsTreeCategLabel(tnCateg.Name);
                        }
                        tnNode.Text = clone.Label;
                        tnNode.ToolTipText = clone.Comments;

                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
                if (sigS6x.slElementsSignatures != null)
                {
                    tnCateg = elemsTreeView.Nodes["ELEMSSIGNATURES"];

                    foreach (S6xElementSignature impSignature in sigS6x.slElementsSignatures.Values)
                    {
                        S6xElementSignature clone = impSignature.Clone();
                        clone.UniqueKey = string.Empty;
                        foreach (S6xElementSignature s6xESignature in sadS6x.slElementsSignatures.Values)
                        {
                            if (s6xESignature.SignatureKey == clone.SignatureKey)
                            {
                                clone.UniqueKey = s6xESignature.UniqueKey;
                                break;
                            }
                        }
                        if (clone.UniqueKey == string.Empty)
                        {
                            clone.UniqueKey = sadS6x.getNewElementSignatureUniqueKey();
                            sadS6x.slElementsSignatures.Add(clone.UniqueKey, clone);

                        }
                        else
                        {
                            sadS6x.slElementsSignatures[clone.UniqueKey] = clone;
                        }
                        sadS6x.isSaved = false;

                        if (tnCateg.Nodes.ContainsKey(clone.UniqueKey))
                        {
                            tnNode = tnCateg.Nodes[clone.UniqueKey];
                        }
                        else
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = clone.UniqueKey;

                            tnCateg.Nodes.Add(tnNode);
                            setElementsTreeCategLabel(tnCateg.Name);
                        }
                        tnNode.Text = clone.SignatureLabel;
                        tnNode.ToolTipText = clone.SignatureComments;

                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Selected file is not valid.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sigS6x = null;
                tnCateg = null;
                tnNode = null;
            }
        }

        private void importDirFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importDirFile(string.Empty);
        }

        private void importDirFile(string dFilePath)
        {
            StreamReader sReader = null;
            string sLine = string.Empty;
            string[] arrLine = null;
            int bankColAdder = 0;
            int bankNum = -1;
            int address = -1;
            int addressEnd = -1;
            string uniqueAddress = string.Empty;
            string sValue = string.Empty;
            int bitFlag = -1;
            object[] bitFlags = null;
            ArrayList alNewTreeNodesInfos = new ArrayList();

            SortedList slScalars = new SortedList();
            SortedList slFunctions = new SortedList();
            SortedList slTables = new SortedList();
            SortedList slStructures = new SortedList();
            SortedList slRoutines = new SortedList();
            SortedList slOperations = new SortedList();
            SortedList slLabels = new SortedList();
            SortedList slBitFlags = new SortedList();

            if (dFilePath == string.Empty)
            {
                if (openFileDialogDir.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogDir.FileName)) return;
                dFilePath = openFileDialogDir.FileName;
            }
            else
            {
                if (!File.Exists(dFilePath)) return;
            }

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                sReader = File.OpenText(dFilePath);

                while (!sReader.EndOfStream)
                {
                    string sLabel = string.Empty;
                    string sUpdatedLabel = string.Empty;
                    
                    sLine = sReader.ReadLine();
                    if (sLine != null)
                    {
                        if (sLine != string.Empty)
                        {
                            sLine = sLine.Replace(":", " ").Trim();
                            while (sLine.Contains("  ")) sLine = sLine.Replace("  ", " ");
                            sLine = sLine.Replace(" +", "+").Replace(" -", "-");
                            if (sLine.Contains("\""))
                            {
                                if (sLine.IndexOf('\"') != sLine.LastIndexOf('\"'))
                                {
                                    sLabel = sLine.Substring(sLine.IndexOf('\"'));
                                    if (sLabel.Length > 0) sLabel = sLabel.Substring(1);
                                    sLabel = sLabel.Substring(0, sLabel.LastIndexOf('\"'));
                                    sUpdatedLabel = sLabel;
                                    while (sUpdatedLabel.Contains("\"")) sUpdatedLabel = sUpdatedLabel.Replace("\"", "_");
                                    while (sUpdatedLabel.Contains(" ")) sUpdatedLabel = sUpdatedLabel.Replace(" ", "_");
                                    if (sLabel != sUpdatedLabel) sLine = sLine.Replace("\"" + sLabel + "\"", "\"" + sUpdatedLabel + "\"");
                                }
                            }
                            arrLine = sLine.Split(' ');
                            switch (arrLine[0].ToLower())
                            {
                                case "sym":
                                case "sub":
                                case "subr":
                                    try
                                    {
                                        if (arrLine[2].Length == 1)
                                        {
                                            bankColAdder = 1;
                                            bankNum = Convert.ToInt32(arrLine[2]);
                                            address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                        }
                                        else
                                        {
                                            bankColAdder = 0;
                                            bankNum = 8;
                                            address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                        }
                                        // Register Case
                                        if (address < 0)
                                        {
                                            bankNum = 2;
                                            address += SADDef.EecBankStartAddress;
                                        }
                                        uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                    }
                                    catch
                                    {
                                        bankNum = -1;
                                        address = -1;
                                        uniqueAddress = string.Empty;
                                    }
                                    break;
                                case "code":
                                case "text":
                                case "byte":
                                case "word":
                                case "function":
                                case "func":
                                case "table":
                                case "struct":
                                case "vect":
                                case "vector":
                                    try
                                    {
                                        try
                                        {
                                            bankColAdder = 1;
                                            bankNum = Convert.ToInt32(arrLine[3]);
                                            address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                            addressEnd = Convert.ToInt32(arrLine[2], 16) - SADDef.EecBankStartAddress;
                                            uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                        }
                                        catch
                                        {
                                            if (arrLine[2].Length == 1)
                                            {
                                                bankColAdder = 0;
                                                bankNum = Convert.ToInt32(arrLine[2]);
                                                address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                addressEnd = -1;
                                                uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                            }
                                            else
                                            {
                                                bankColAdder = 0;
                                                bankNum = 8;
                                                address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                addressEnd = Convert.ToInt32(arrLine[2], 16) - SADDef.EecBankStartAddress;
                                                uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                            }
                                        }
                                        if (addressEnd < address)
                                        {
                                            // To be managed like a Label only
                                            arrLine[0] = "sym";
                                            //bankNum = -1;
                                            //address = -1;
                                            //uniqueAddress = string.Empty;
                                        }
                                    }
                                    catch
                                    {
                                        bankNum = -1;
                                        address = -1;
                                        uniqueAddress = string.Empty;
                                    }
                                    break;
                                default:
                                    bankNum = -1;
                                    address = -1;
                                    uniqueAddress = string.Empty;
                                    break;
                            }
                            if (bankNum >= 0 && address >= 0 && uniqueAddress != string.Empty)
                            {
                                try
                                {
                                    /*
                                    word 247e 249d :P 5
                                    SYM 247e "[15a]_[15e]_Init"

                                    function 2e20 2e29 :Y S P 3 :Y S P 2
                                    SYM 2e20 "Func9_continues"

                                    table 34d0 3520 :Y O +9 
                                    SYM 34d0 "Table51"

                                    byte 37a3 37a3 :P 3

                                    SYM 4060 "AD_Chan_Reg_4064_Ref_Somewhere"

                                    code 727e 739b
                                    SYM 727e "Unused_Code_To_739b"

                                    text 9fe0 9fff
                                    SYM 9fe0 "ASCII_Catch_Comments"
                                     
                                    SYM 4060 "AD_Chan_Reg_4064_Ref_Somewhere" :T +4
                                    */
                                    switch (arrLine[0].ToLower())
                                    {
                                        case "sym":
                                            sValue = string.Empty;
                                            bitFlag = -1;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower().StartsWith("t"))
                                                {
                                                    bitFlag = Convert.ToInt32(arrLine[iCol].ToLower().Replace("t", "").Replace("+", ""));
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    sValue = arrLine[iCol].Replace("\"", "");
                                                }
                                            }
                                            if (sValue != string.Empty)
                                            {
                                                if (bitFlag == -1)
                                                {
                                                    if (!slLabels.ContainsKey(uniqueAddress)) slLabels.Add(uniqueAddress, new object[] { bankNum, address, sValue });
                                                }
                                                else
                                                {
                                                    if (slBitFlags.ContainsKey(uniqueAddress))
                                                    {
                                                        bitFlags = new object[((object[])((object[])slBitFlags[uniqueAddress])[2]).Length + 1];
                                                        for (int bF = 0; bF < bitFlags.Length - 1; bF++) bitFlags[bF] = ((object[])((object[])slBitFlags[uniqueAddress])[2])[bF];
                                                        bitFlags[bitFlags.Length - 1] = new object[] { bitFlag, sValue };
                                                        ((object[])slBitFlags[uniqueAddress])[2] = bitFlags;
                                                        bitFlags = null;
                                                    }
                                                    else
                                                    {
                                                        slBitFlags.Add(uniqueAddress, new object[] { bankNum, address, new object[] { new object[] { bitFlag, sValue } } });
                                                    }
                                                }
                                            }
                                            break;
                                        case "code":
                                            S6xOperation ope = new S6xOperation();
                                            ope.BankNum = bankNum;
                                            ope.AddressInt = address;
                                            if (!slOperations.ContainsKey(uniqueAddress)) slOperations.Add(uniqueAddress, ope);
                                            ope = null;
                                            break;
                                        case "text":
                                            S6xStructure tStruct = new S6xStructure();
                                            tStruct.BankNum = bankNum;
                                            tStruct.AddressInt = address;
                                            tStruct.Store = true;
                                            tStruct.Number = 1;
                                            tStruct.StructDef = "Ascii:" + (addressEnd - address + 1).ToString();
                                            if (!slStructures.ContainsKey(uniqueAddress)) slStructures.Add(uniqueAddress, tStruct);
                                            tStruct = null;
                                            break;
                                        case "sub":
                                        case "subr":
                                            S6xRoutine routine = new S6xRoutine();
                                            routine.BankNum = bankNum;
                                            routine.AddressInt = address;
                                            routine.Store = true;
                                            routine.ByteArgumentsNum = 0;
                                            int iNum = 0;
                                            string lastArgType = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower() == "y")
                                                // Byte
                                                {
                                                    lastArgType = "y";
                                                    iNum += 1;
                                                    routine.ByteArgumentsNumOverride = true;
                                                }
                                                else if (arrLine[iCol].ToLower() == "w" || arrLine[iCol].ToLower() == "r")
                                                // Word or Routine Address
                                                {
                                                    lastArgType = "w";
                                                    iNum += 2;
                                                    routine.ByteArgumentsNumOverride = true;
                                                }
                                                else if (arrLine[iCol].ToLower().StartsWith("o"))
                                                {
                                                    switch (lastArgType)
                                                    {
                                                        case "y":
                                                            iNum += Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", "")) - 1;
                                                            routine.ByteArgumentsNumOverride = true;
                                                            break;
                                                        case "w":
                                                            iNum += (2 * Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", ""))) - 2;
                                                            routine.ByteArgumentsNumOverride = true;
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    routine.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    routine.Label = routine.ShortLabel;
                                                }
                                            }
                                            routine.ByteArgumentsNum = iNum;
                                            if (!slRoutines.ContainsKey(uniqueAddress)) slRoutines.Add(uniqueAddress, routine);
                                            routine = null;
                                            break;
                                        case "byte":
                                            for (int iByte = 0; iByte < addressEnd - address + 1; iByte++)
                                            {
                                                S6xScalar scalar = new S6xScalar();
                                                scalar.BankNum = bankNum;
                                                scalar.AddressInt = address + iByte;
                                                scalar.Store = true;
                                                scalar.Byte = true;
                                                scalar.Signed = false;
                                                scalar.ScaleExpression = "X";
                                                for (int iCol = 2+bankColAdder; iCol < arrLine.Length; iCol++)
                                                {
                                                    if (arrLine[iCol].ToLower() == "s") scalar.Signed = true;
                                                    else if (arrLine[iCol].ToLower().StartsWith("v")) scalar.ScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                    {
                                                        scalar.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                        scalar.Label = scalar.ShortLabel;
                                                    }
                                                }
                                                if (!slScalars.ContainsKey(Tools.UniqueAddress(bankNum, address + iByte))) slScalars.Add(Tools.UniqueAddress(bankNum, address + iByte), scalar);
                                                scalar = null;
                                            }
                                            break;
                                        case "word":
                                            for (int iByte = 0; iByte < addressEnd - address + 1; iByte += 2)
                                            {
                                                S6xScalar scalar = new S6xScalar();
                                                scalar.BankNum = bankNum;
                                                scalar.AddressInt = address + iByte;
                                                scalar.Store = true;
                                                scalar.Byte = false;
                                                scalar.Signed = false;
                                                scalar.ScaleExpression = "X";
                                                for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                                {
                                                    if (arrLine[iCol].ToLower() == "s") scalar.Signed = true;
                                                    else if (arrLine[iCol].ToLower().StartsWith("v")) scalar.ScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                    {
                                                        scalar.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                        scalar.Label = scalar.ShortLabel;
                                                    }
                                                }
                                                if (!slScalars.ContainsKey(Tools.UniqueAddress(bankNum, address + iByte))) slScalars.Add(Tools.UniqueAddress(bankNum, address + iByte), scalar);
                                            }
                                            break;
                                        case "function":
                                        case "func":
                                            bool inputPartStarted = false;
                                            bool inputPartEnded = false;
                                            int lineSize = -1;
                                            
                                            S6xFunction func = new S6xFunction();
                                            func.BankNum = bankNum;
                                            func.AddressInt = address;
                                            func.Store = true;
                                            func.RowsNumber = 1;
                                            func.ByteInput = true;
                                            func.ByteOutput = true;
                                            func.SignedInput = false;
                                            func.SignedOutput = false;
                                            func.InputScaleExpression = "X";
                                            func.OutputScaleExpression = "X";
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower() == "w")
                                                {
                                                    if (inputPartStarted) inputPartEnded = true;
                                                    else inputPartStarted = true;
                                                    if (inputPartEnded) func.ByteOutput = false;
                                                    else
                                                    {
                                                        func.ByteInput = false;
                                                        func.ByteOutput = false;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower() == "y")
                                                {
                                                    if (inputPartStarted) inputPartEnded = true;
                                                    else inputPartStarted = true;
                                                    if (inputPartEnded) func.ByteOutput = false;
                                                    else
                                                    {
                                                        func.ByteInput = true;
                                                        func.ByteOutput = true;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower() == "s")
                                                {
                                                    if (inputPartEnded) func.SignedOutput = true;
                                                    else
                                                    {
                                                        func.SignedInput = true;
                                                        func.SignedOutput = true;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower().StartsWith("v"))
                                                {
                                                    if (inputPartEnded) func.OutputScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    else func.InputScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    func.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    func.Label = func.ShortLabel;
                                                }
                                            }

                                            lineSize = 2;
                                            if (!func.ByteInput) lineSize++;
                                            if (!func.ByteOutput) lineSize++;
                                            if ((addressEnd - address + 1) % lineSize == 0) func.RowsNumber = (addressEnd - address + 1) / lineSize;
                                            else func.RowsNumber = (addressEnd - address + 1 - ((addressEnd - address + 1) % lineSize)) / lineSize;

                                            if (func.RowsNumber > 0)
                                            {
                                                if (!slFunctions.ContainsKey(uniqueAddress)) slFunctions.Add(uniqueAddress, func);
                                            }
                                            else if (!slLabels.ContainsKey(uniqueAddress) && func.ShortLabel != null && func.ShortLabel != string.Empty)
                                            {
                                                slLabels.Add(uniqueAddress, new object[] { bankNum, address, func.ShortLabel });
                                            }
                                            break;
                                        case "table":
                                            S6xTable table = new S6xTable();
                                            table.BankNum = bankNum;
                                            table.AddressInt = address;
                                            table.Store = true;
                                            table.ColsNumber = -1;
                                            table.SignedOutput = false;
                                            table.CellsScaleExpression = "X";
                                            table.ColsScalerAddress = string.Empty;
                                            table.RowsScalerAddress = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower().StartsWith("o")) table.ColsNumber = Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", ""));
                                                else if (arrLine[iCol].ToLower() == "s") table.SignedOutput = true;
                                                else if (arrLine[iCol].ToLower().StartsWith("v")) table.CellsScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    table.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    table.Label = table.ShortLabel;
                                                }
                                            }
                                            if (table.ColsNumber > 0)
                                            {
                                                if ((addressEnd - address + 1) % table.ColsNumber == 0) table.RowsNumber = (addressEnd - address + 1) / table.ColsNumber;
                                                else table.RowsNumber = (addressEnd - address + 1 - ((addressEnd - address + 1) % table.ColsNumber)) / table.ColsNumber;
                                            }
                                            if (table.ColsNumber > 0 && table.RowsNumber > 0)
                                            {
                                                if (!slTables.ContainsKey(uniqueAddress)) slTables.Add(uniqueAddress, table);
                                            }
                                            else if (!slLabels.ContainsKey(uniqueAddress) && table.ShortLabel != null && table.ShortLabel != string.Empty)
                                            {
                                                slLabels.Add(uniqueAddress, new object[] { bankNum, address, table.ShortLabel });
                                            }
                                            break;
                                        case "struct":
                                            S6xStructure sStruct = new S6xStructure();
                                            sStruct.BankNum = bankNum;
                                            sStruct.AddressInt = address;
                                            sStruct.Store = true;
                                            sStruct.Number = 1; // To Be Recalculated
                                            sStruct.StructDef = string.Empty;
                                            int iSize = 0;
                                            string lastDataType = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower() == "r")
                                                // Routine Pointer managed as Word
                                                {
                                                    lastDataType = "r";
                                                    iSize += 2;
                                                    if (sStruct.StructDef != string.Empty) sStruct.StructDef += ",";
                                                    sStruct.StructDef += "Word";
                                                }
                                                else if (arrLine[iCol].ToLower() == "y")
                                                // Byte
                                                {
                                                    lastDataType = "y";
                                                    iSize += 1;
                                                    if (sStruct.StructDef != string.Empty) sStruct.StructDef += ",";
                                                    sStruct.StructDef += "Byte";
                                                }
                                                else if (arrLine[iCol].ToLower() == "w")
                                                // Word
                                                {
                                                    lastDataType = "w";
                                                    iSize += 2;
                                                    if (sStruct.StructDef != string.Empty) sStruct.StructDef += ",";
                                                    sStruct.StructDef += "Word";
                                                }
                                                else if (arrLine[iCol].ToLower() == "x")
                                                {
                                                    switch (lastDataType)
                                                    {
                                                        case "y":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Byte")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Byte")).Replace("Byte", "ByteHex");
                                                            break;
                                                        case "w":
                                                        case "r":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Word")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Word")).Replace("Word", "WordHex");
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower() == "s")
                                                {
                                                    switch (lastDataType)
                                                    {
                                                        case "y":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Byte")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Byte")).Replace("Byte", "SByte");
                                                            break;
                                                        case "w":
                                                        case "r":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Word")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Word")).Replace("Word", "SWord");
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower().StartsWith("o"))
                                                {
                                                    sStruct.StructDef += arrLine[iCol].ToLower().Replace("o", "").Replace("+", ":");
                                                    switch (lastDataType)
                                                    {
                                                        case "y":
                                                            iSize += Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", "")) - 1;
                                                            break;
                                                        case "w":
                                                        case "r":
                                                            iSize += (2 * Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", ""))) - 2;
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    sStruct.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    sStruct.Label = sStruct.ShortLabel;
                                                }
                                            }
                                            if (iSize > 0)
                                            {
                                                if ((addressEnd - address + 1) % iSize == 0) sStruct.Number = (addressEnd - address + 1) / iSize;
                                                else sStruct.Number = (addressEnd - address + 1 - ((addressEnd - address + 1) % iSize)) / iSize;
                                            }
                                            if (!slStructures.ContainsKey(uniqueAddress)) slStructures.Add(uniqueAddress, sStruct);
                                            sStruct = null;
                                            break;
                                        case "vect":
                                        case "vector":
                                            S6xStructure vList = new S6xStructure();
                                            vList.BankNum = bankNum;
                                            vList.AddressInt = address;
                                            vList.Number = (addressEnd + 1 - address) / 2;
                                            vList.Store = true;
                                            vList.StructDef = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower().StartsWith("d"))
                                                {
                                                    vList.StructDef = "Vect" + arrLine[iCol].ToLower().Replace("d", "");
                                                    break;
                                                }
                                            }
                                            if (bankColAdder == 0) vList.StructDef = "Vect8";

                                            if (vList.StructDef != string.Empty && vList.Number * 2 == addressEnd + 1 - address)
                                            {
                                                if (!slStructures.ContainsKey(vList.UniqueAddress)) slStructures.Add(vList.UniqueAddress, vList);
                                            }
                                            vList = null;
                                            break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                arrLine = null;
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                int iCount = 1;
                foreach (S6xScalar s6xObject in slScalars.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = SADDef.ShortScalarPrefix + "ScSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                        if (sadBin.Calibration != null)
                        {
                            s6xObject.isCalibrationElement = (s6xObject.AddressBinInt >= sadBin.Calibration.AddressBinInt && s6xObject.AddressBinInt <= sadBin.Calibration.AddressBinEndInt);
                        }
                    }

                    bool bManage = true;
                    TreeNode tnDuplicate = checkDuplicate(s6xObject.UniqueAddress, false);
                    if (tnDuplicate != null)
                    {
                        // Block Reserved and Calibration Load Elements
                        if (tnDuplicate.Parent == null) bManage = false;
                        // Type Conflict
                        else if (tnDuplicate.Parent.Name != "SCALARS") bManage = false;
                        tnDuplicate = null;
                    }
                    if (bManage)
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slScalars[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slScalars.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { "SCALARS", s6xObject.UniqueAddress, s6xObject.Label });
                    }
                }

                iCount = 1; 
                foreach (S6xFunction s6xObject in slFunctions.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = SADDef.ShortFunctionPrefix + "FcSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                        if (sadBin.Calibration != null)
                        {
                            s6xObject.isCalibrationElement = (s6xObject.AddressBinInt >= sadBin.Calibration.AddressBinInt && s6xObject.AddressBinInt <= sadBin.Calibration.AddressBinEndInt);
                        }
                    }

                    bool bManage = true;
                    TreeNode tnDuplicate = checkDuplicate(s6xObject.UniqueAddress, false);
                    if (tnDuplicate != null)
                    {
                        // Block Reserved and Calibration Load Elements
                        if (tnDuplicate.Parent == null) bManage = false;
                        // Type Conflict
                        else if (tnDuplicate.Parent.Name != "FUNCTIONS") bManage = false;
                        tnDuplicate = null;
                    }
                    if (bManage)
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slFunctions[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slFunctions.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { "FUNCTIONS", s6xObject.UniqueAddress, s6xObject.Label });
                    }
                }

                iCount = 1;
                foreach (S6xTable s6xObject in slTables.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = SADDef.ShortTablePrefix + "TbSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                        if (sadBin.Calibration != null)
                        {
                            s6xObject.isCalibrationElement = (s6xObject.AddressBinInt >= sadBin.Calibration.AddressBinInt && s6xObject.AddressBinInt <= sadBin.Calibration.AddressBinEndInt);
                        }
                    }

                    bool bManage = true;
                    TreeNode tnDuplicate = checkDuplicate(s6xObject.UniqueAddress, false);
                    if (tnDuplicate != null)
                    {
                        // Block Reserved and Calibration Load Elements
                        if (tnDuplicate.Parent == null) bManage = false;
                        // Type Conflict
                        else if (tnDuplicate.Parent.Name != "TABLES") bManage = false;
                        tnDuplicate = null;
                    }
                    if (bManage)
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slTables[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slTables.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { "TABLES", s6xObject.UniqueAddress, s6xObject.Label });
                    }
                }

                iCount = 1;
                foreach (S6xStructure s6xObject in slStructures.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress))
                    {
                        if (s6xObject.isVectorsList) s6xObject.ShortLabel = SADDef.ShortStructurePrefix + "VlSadDir" + string.Format("{0:d3}", iCount);
                        else s6xObject.ShortLabel = SADDef.ShortStructurePrefix + "StSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                    }

                    bool bManage = true;
                    TreeNode tnDuplicate = checkDuplicate(s6xObject.UniqueAddress, false);
                    if (tnDuplicate != null)
                    {
                        // Block Reserved and Calibration Load Elements
                        if (tnDuplicate.Parent == null) bManage = false;
                        // Type Conflict
                        else if (tnDuplicate.Parent.Name != "STRUCTURES") bManage = false;
                        tnDuplicate = null;
                    }
                    if (bManage)
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slStructures[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slStructures.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { "STRUCTURES", s6xObject.UniqueAddress, s6xObject.Label });
                    }
                }

                iCount = 1;
                foreach (S6xRoutine s6xObject in slRoutines.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = "RtSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }

                    bool bManage = true;
                    TreeNode tnDuplicate = checkDuplicate(s6xObject.UniqueAddress, false);
                    if (tnDuplicate != null)
                    {
                        // Block Reserved and Calibration Load Elements
                        if (tnDuplicate.Parent == null) bManage = false;
                        // Type Conflict
                        else if (tnDuplicate.Parent.Name != "ROUTINES") bManage = false;
                        tnDuplicate = null;
                    }
                    if (bManage)
                    {
                        sadS6x.isSaved = false;
                        // No direct override, using only Labels and ByteArgumentsNum when provided
                        if (sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress))
                        {
                            S6xRoutine existingRoutine = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];
                            if (s6xObject.ByteArgumentsNumOverride)
                            {
                                existingRoutine.ByteArgumentsNumOverride = s6xObject.ByteArgumentsNumOverride;
                                existingRoutine.ByteArgumentsNum = s6xObject.ByteArgumentsNum;
                            }
                            existingRoutine.ShortLabel = s6xObject.ShortLabel;
                            existingRoutine.Label = s6xObject.Label;
                            existingRoutine = null;
                        }
                        else sadS6x.slRoutines.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { "ROUTINES", s6xObject.UniqueAddress, s6xObject.Label });
                    }
                }

                iCount = 1;
                foreach (S6xOperation s6xObject in slOperations.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = "OpSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }

                    bool bManage = true;
                    TreeNode tnDuplicate = checkDuplicate(s6xObject.UniqueAddress, false);
                    if (tnDuplicate != null)
                    {
                        // Block Reserved and Calibration Load Elements
                        if (tnDuplicate.Parent == null) bManage = false;
                        // Type Conflict
                        else if (tnDuplicate.Parent.Name != "OPERATIONS") bManage = false;
                        tnDuplicate = null;
                    }
                    if (bManage)
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slOperations[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slOperations.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { "OPERATIONS", s6xObject.UniqueAddress, s6xObject.Label });
                    }
                }

                // Remaining Labels created as Other Addresses when Address, Register when Register or Update existing objects
                foreach (object[] oLabel in slLabels.Values)
                {
                    if ((int)oLabel[0] == 2)
                    // Bank Num was set to 2 to identify short addresses (<0x2000) and to manage them as Registers
                    {
                        S6xRegister reg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress((int)oLabel[1])];
                        if (reg == null)
                        {
                            reg = new S6xRegister((int)oLabel[1]);
                            reg.Comments = string.Empty;
                        }
                        reg.Label = oLabel[2].ToString();
                        reg.Store = true;
                        if (sadS6x.slRegisters.ContainsKey(reg.UniqueAddress)) sadS6x.slRegisters[reg.UniqueAddress] = reg;
                        else sadS6x.slRegisters.Add(reg.UniqueAddress, reg);
                        alNewTreeNodesInfos.Add(new string[] { "REGISTERS", reg.UniqueAddress, reg.Label });
                        reg = null;
                    }
                    else
                    {
                        object s6xObject = null;
                        uniqueAddress = Tools.UniqueAddress((int)oLabel[0], (int)oLabel[1]);
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slScalars[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xScalar)s6xObject).Label = oLabel[2].ToString();
                                ((S6xScalar)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xScalar)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { "SCALARS", uniqueAddress, oLabel[2].ToString() });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slFunctions[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xFunction)s6xObject).Label = oLabel[2].ToString();
                                ((S6xFunction)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xFunction)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { "FUNCTIONS", uniqueAddress, oLabel[2].ToString() });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slTables[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xTable)s6xObject).Label = oLabel[2].ToString();
                                ((S6xTable)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xTable)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { "TABLES", uniqueAddress, oLabel[2].ToString() });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slStructures[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xStructure)s6xObject).Label = oLabel[2].ToString();
                                ((S6xStructure)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xStructure)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { "STRUCTURES", uniqueAddress, oLabel[2].ToString() });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slRoutines[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xRoutine)s6xObject).Label = oLabel[2].ToString();
                                ((S6xRoutine)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xRoutine)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { "ROUTINES", uniqueAddress, oLabel[2].ToString() });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slOperations[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xOperation)s6xObject).Label = oLabel[2].ToString();
                                ((S6xOperation)s6xObject).ShortLabel = oLabel[2].ToString();
                                alNewTreeNodesInfos.Add(new string[] { "OPERATIONS", uniqueAddress, oLabel[2].ToString() });
                            }
                        }
                        if (s6xObject == null)
                        {
                            S6xOtherAddress other = new S6xOtherAddress();
                            other.BankNum = (int)oLabel[0];
                            other.AddressInt = (int)oLabel[1];
                            other.Label = oLabel[2].ToString();
                            other.Comments = string.Empty;
                            if (sadS6x.slOtherAddresses.ContainsKey(other.UniqueAddress))
                            {
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).Label = other.Label;
                            }
                            else
                            {
                                sadS6x.slOtherAddresses.Add(other.UniqueAddress, other);
                            }
                            alNewTreeNodesInfos.Add(new string[] { "OTHER", uniqueAddress, oLabel[2].ToString() });
                            other = null;
                        }
                        s6xObject = null;
                    }
                    sadS6x.isSaved = false;
                }

                // Scalars / Registers Bit Flags
                foreach (object[] bFs in slBitFlags.Values)
                {
                    S6xScalar s6xScalar = (S6xScalar)sadS6x.slScalars[Tools.UniqueAddress((int)bFs[0], (int)bFs[1])];
                    if (s6xScalar != null)
                    {
                        bitFlags = (object[])bFs[2];
                        foreach (object[] bF in bitFlags)
                        {
                            S6xBitFlag s6xBitFlag = new S6xBitFlag();
                            s6xBitFlag.Position = (int)bF[0];
                            if ((s6xScalar.Byte && s6xBitFlag.Position > 7) || s6xBitFlag.Position > 15)
                            {
                                s6xBitFlag = null;
                                continue;
                            }
                            s6xBitFlag.ShortLabel = bF[1].ToString();
                            string label = bF[1].ToString();
                            string setValue = "1";
                            string notSetValue = "0";
                            // Not overriding specific existing S6x Information
                            if (s6xScalar.BitFlags != null)
                            {
                                foreach (S6xBitFlag s6xBF in s6xScalar.BitFlags)
                                {
                                    if (s6xBF.Position == s6xBitFlag.Position)
                                    {
                                        label = s6xBF.Label;
                                        setValue = s6xBF.SetValue;
                                        notSetValue = s6xBF.NotSetValue;
                                        break;
                                    }
                                }
                            }
                            s6xBitFlag.Label = label;
                            s6xBitFlag.SetValue = setValue;
                            s6xBitFlag.NotSetValue = notSetValue;
                            s6xScalar.AddBitFlag(s6xBitFlag);
                            s6xBitFlag = null;

                            sadS6x.isSaved = false;
                            alNewTreeNodesInfos.Add(new string[] { "SCALARS", s6xScalar.UniqueAddress, s6xScalar.Label });
                        }
                        bitFlags = null;

                        s6xScalar = null;

                        continue;
                    }

                    if ((int)bFs[0] == 2)
                    // Bank Num was set to 2 to identify short addresses (<0x2000) and to manage them as Registers
                    {
                        S6xRegister s6xReg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress((int)bFs[1])];
                        if (s6xReg == null)
                        // Register Creation based on BitFlag Information
                        {
                            s6xReg = new S6xRegister((int)bFs[1]);
                            sadS6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                            s6xReg.Comments = string.Empty;
                            s6xReg.Label = Tools.RegisterInstruction(s6xReg.Address);
                            s6xReg.Store = true;

                            alNewTreeNodesInfos.Add(new string[] { "REGISTERS", s6xReg.UniqueAddress, s6xReg.Label });
                        }

                        bitFlags = (object[])bFs[2];
                        foreach (object[] bF in bitFlags)
                        {
                            S6xBitFlag s6xBitFlag = new S6xBitFlag();
                            s6xBitFlag.Position = (int)bF[0];
                            if (s6xBitFlag.Position > 15)
                            {
                                s6xBitFlag = null;
                                continue;
                            }
                            s6xBitFlag.ShortLabel = bF[1].ToString();
                            string label = bF[1].ToString();
                            string setValue = "1";
                            string notSetValue = "0";
                            // Not overriding specific existing S6x Information
                            if (s6xReg.BitFlags != null)
                            {
                                foreach (S6xBitFlag s6xBF in s6xReg.BitFlags)
                                {
                                    if (s6xBF.Position == s6xBitFlag.Position)
                                    {
                                        label = s6xBF.Label;
                                        setValue = s6xBF.SetValue;
                                        notSetValue = s6xBF.NotSetValue;
                                        break;
                                    }
                                }
                            }
                            s6xBitFlag.Label = label;
                            s6xBitFlag.SetValue = setValue;
                            s6xBitFlag.NotSetValue = notSetValue;
                            s6xReg.AddBitFlag(s6xBitFlag);
                            s6xBitFlag = null;

                            sadS6x.isSaved = false;
                            alNewTreeNodesInfos.Add(new string[] { "REGISTERS", s6xReg.UniqueAddress, s6xReg.Label });
                        }
                        bitFlags = null;

                        s6xReg = null;

                        continue;
                    }
                }

                // Updates First for Threading Purposes
                foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                {
                    TreeNode tnNode = elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes[newTreeNodeInfos[1]];
                    if (tnNode == null) continue;
                    if (tnNode.Text != newTreeNodeInfos[2]) // For Performance purposes
                    {
                        tnNode.Text = newTreeNodeInfos[2];
                        tnNode.ToolTipText = "SAD Directive Import";
                        tnNode.ForeColor = Color.Purple;
                    }
                    newTreeNodeInfos[0] = null; // To be ignored at creation level
                }
                foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                {
                    if (newTreeNodeInfos[0] == null) continue;
                    if (elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes.ContainsKey(newTreeNodeInfos[1])) continue;
                    TreeNode tnNode = new TreeNode();
                    tnNode.Name = newTreeNodeInfos[1];
                    tnNode.Text = newTreeNodeInfos[2];
                    tnNode.ToolTipText = "SAD Directive Import";
                    tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    tnNode.ForeColor = Color.Red;
                    elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes.Add(tnNode);
                }
                alNewTreeNodesInfos = null;

                setElementsTreeCategLabel("SCALARS");
                setElementsTreeCategLabel("FUNCTIONS");
                elemsTreeView.Nodes["FUNCTIONS"].Tag = true;              // For Scalers Refresh
                setElementsTreeCategLabel("TABLES");
                setElementsTreeCategLabel("STRUCTURES");
                setElementsTreeCategLabel("ROUTINES");
                setElementsTreeCategLabel("OPERATIONS");
                setElementsTreeCategLabel("REGISTERS");
                setElementsTreeCategLabel("OTHER");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Directive file import has failed.\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                arrLine = null;
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                slScalars = null;
                slFunctions = null;
                slTables = null;
                slStructures = null;
                slRoutines = null;
                slOperations = null;
                slLabels = null;
                slBitFlags = null;

                Cursor = processPreviousCursor;

                GC.Collect();
            }
        }

        private void exportDirFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter sWri = null;
            SortedList slElements = new SortedList();
            string elemLine = string.Empty;
            string symLine = string.Empty;
            int endAddress = -1;
            string opt1 = string.Empty;
            string opt2 = string.Empty;
            string scale1 = string.Empty;
            string scale2 = string.Empty;

            if (sadBin == null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            if (saveFileDialogDir.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (File.Exists(saveFileDialogDir.FileName))
            {
                // Dir Original file automatic Backup
                try
                {
                    File.Copy(saveFileDialogDir.FileName, saveFileDialogDir.FileName + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    MessageBox.Show("File backup has failed.\r\nNo other action will be managed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                sWri = new StreamWriter(saveFileDialogDir.FileName, false, new UTF8Encoding(false));

                sWri.WriteLine("----- Commands ------");
                if (sadBin.Calibration.Info.is8061) sWri.WriteLine("opts   :C P N S"); else  sWri.WriteLine("opts   :C P N S H");

                foreach (string[] bankInfos in sadBin.Calibration.Info.slBanksInfos.Values) sWri.WriteLine(string.Format("{0} {1,4} {2,4} {3}", "bank", bankInfos[1], bankInfos[2], bankInfos[0]));
                sWri.WriteLine();

                foreach (RBase rBase in sadBin.Calibration.slRbases.Values) sWri.WriteLine(string.Format("{0} {1} {2,4}", "rbase", rBase.Code, rBase.AddressBank));
                sWri.WriteLine();
                if (sadBin.Calibration.slRconst.Count > 0)
                {
                    foreach (RConst rConst in sadBin.Calibration.slRconst.Values) sWri.WriteLine(string.Format("{0} {1} {2,4}", "rbase", rConst.Code, rConst.Value));
                    sWri.WriteLine();
                }

                slElements = new SortedList();
                
                foreach (S6xScalar s6xElem in sadS6x.slScalars.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    endAddress = SADDef.EecBankStartAddress + s6xElem.AddressInt - 1;
                    scale1 = string.Empty;
                    if (s6xElem.ScaleExpression != null && s6xElem.ScaleExpression != string.Empty)
                    {
                        if (s6xElem.ScaleExpression.Contains("/") && s6xElem.ScaleExpression.ToLower() != "x/1")
                        {
                            try {scale1 = string.Format("{0:0.00}", Convert.ToDouble(s6xElem.ScaleExpression.ToLower().Replace("x", "").Replace("/", "")));}
                            catch {scale1 = string.Empty;}
                        }
                    }
                    if (s6xElem.Byte)
                    {
                        endAddress++;
                        opt1 = ":";
                        if (scale1 != string.Empty) opt1 += "V +" + scale1 + " ";
                        opt1 += "P 3";
                        elemLine = string.Format("{0} {1} {2:x4} {3} {4}", "byte", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        if (s6xElem.isBitFlags)
                        {
                            // Not Managed
                        }
                    }
                    else
                    {
                        endAddress += 2;
                        opt1 = ":";
                        if (scale1 != string.Empty) opt1 += "V +" + scale1 + " ";
                        opt1 += "P 5";
                        elemLine = string.Format("{0} {1} {2:x4} {3} {4}", "word", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        if (s6xElem.isBitFlags)
                        {
                            // Not Managed
                        }
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }
                foreach (S6xFunction s6xElem in sadS6x.slFunctions.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    if (s6xElem.RowsNumber > 0)
                    {
                        endAddress = SADDef.EecBankStartAddress + s6xElem.AddressInt - 1;
                        if (s6xElem.ByteInput) endAddress += s6xElem.RowsNumber; else endAddress += s6xElem.RowsNumber * 2;
                        if (s6xElem.ByteOutput) endAddress += s6xElem.RowsNumber; else  endAddress += s6xElem.RowsNumber * 2;
                        scale1 = string.Empty;
                        if (s6xElem.InputScaleExpression != null && s6xElem.InputScaleExpression != string.Empty)
                        {
                            if (s6xElem.InputScaleExpression.Contains("/") && s6xElem.InputScaleExpression.ToLower() != "x/1")
                            {
                                try { scale1 = string.Format("{0:0.00}", Convert.ToDouble(s6xElem.InputScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                                catch { scale1 = string.Empty; }
                            }
                        }
                        scale2 = string.Empty;
                        if (s6xElem.OutputScaleExpression != null && s6xElem.OutputScaleExpression != string.Empty)
                        {
                            if (s6xElem.OutputScaleExpression.Contains("/") && s6xElem.OutputScaleExpression.ToLower() != "x/1")
                            {
                                try { scale2 = string.Format("{0:0.00}", Convert.ToDouble(s6xElem.OutputScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                                catch { scale2 = string.Empty; }
                            }
                        }

                        if (s6xElem.ByteInput) opt1 = ":Y"; else opt1 = ":W";
                        if (s6xElem.SignedInput) opt1 += " S";
                        if (scale1 != string.Empty) opt1 += " V +" + scale1;
                        if (s6xElem.ByteInput) opt1 += " P 3"; else  opt1 += " P 5";
                        if (s6xElem.ByteOutput) opt2 = ":Y"; else opt2 = ":W";
                        if (s6xElem.SignedOutput) opt2 += " S";
                        if (scale2 != string.Empty) opt2 += " V +" + scale2;
                        if (s6xElem.ByteInput) opt2 += " P 3"; else opt2 += " P 5";
                        elemLine = string.Format("{0} {1} {2:x4} {3} {4} {5}", "function", s6xElem.Address, endAddress, s6xElem.BankNum, opt1, opt2);
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }
                foreach (S6xTable s6xElem in sadS6x.slTables.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    if (s6xElem.ColsNumber > 0)
                    {
                        endAddress = SADDef.EecBankStartAddress + s6xElem.AddressInt - 1;
                        if (s6xElem.WordOutput) endAddress += s6xElem.ColsNumber * 2; else endAddress += s6xElem.ColsNumber * 2;
                        scale1 = string.Empty;
                        if (s6xElem.CellsScaleExpression != null && s6xElem.CellsScaleExpression != string.Empty)
                        {
                            if (s6xElem.CellsScaleExpression.Contains("/") && s6xElem.CellsScaleExpression.ToLower() != "x/1")
                            {
                                try { scale1 = string.Format("{0:0.00}", Convert.ToDouble(s6xElem.CellsScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                                catch { scale1 = string.Empty; }
                            }
                        }
                        opt1 = ":O +" + s6xElem.ColsNumber.ToString();
                        if (s6xElem.WordOutput) opt1 += " W"; else opt1 += " Y";
                        if (s6xElem.SignedOutput) opt1 += " S";
                        if (scale1 != string.Empty) opt1 += " V +" + scale1;
                        if (s6xElem.WordOutput) opt1 += " P 5"; else opt1 += " P 3";
                        elemLine = string.Format("{0} {1} {2:x4} {3} {4}", "table", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }

                foreach (S6xStructure s6xElem in sadS6x.slStructures.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    if (s6xElem.Structure.isValid && !s6xElem.Structure.isEmpty)
                    {
                        endAddress = SADDef.EecBankStartAddress + s6xElem.Structure.AddressEndInt;
                        if (s6xElem.isVectorsList)
                        {
                            opt1 = "D: " + s6xElem.VectorsBankNum.ToString();
                            elemLine = string.Format("{0} {1} {2:x4} {3} {4}", "vect", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        }
                        else
                        {
                            // To Be Reviewed
                            opt1 = ":Y X O +" + s6xElem.Structure.Size.ToString();
                            elemLine = string.Format("{0} {1} {2:x4} {3} {4}", "struct", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        }
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }

                foreach (S6xOperation s6xElem in sadS6x.slOperations.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }

                foreach (S6xRoutine s6xElem in sadS6x.slRoutines.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }

                foreach (S6xOtherAddress s6xElem in sadS6x.slOtherAddresses.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.Label == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2} {3,-30}", "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.Label.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                }

                foreach (S6xRegister s6xElem in sadS6x.slRegisters.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.isDual) continue;   // Not Managed
                    if (s6xElem.Label == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format("{0} {1} {2,-30}", "sym", s6xElem.Address, "\"" + s6xElem.Label.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] {elemLine, symLine});
                    if (s6xElem.isBitFlags)
                    {
                        // Not Managed
                    }
                }

                foreach (string[] lineArray in slElements.Values)
                {
                    foreach (string sLine in lineArray) if (sLine != string.Empty) sWri.WriteLine(sLine);
                    sWri.WriteLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Directive file export has failed.\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                slElements = null;

                try { sWri.Close(); }
                catch { }
                try { sWri.Dispose(); }
                catch { }
                sWri = null;

                Cursor = processPreviousCursor;

                GC.Collect();
            }
        }

        private void importCmtFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importCmtFile(string.Empty);
        }

        private void importCmtFile(string cFilePath)
        {
            StreamReader sReader = null;
            string sLine = string.Empty;
            bool bankModeSet = false;
            bool bankMode = false;
            int bankNum = -1;
            int address = -1;
            string uniqueAddress = string.Empty;
            ArrayList alNewTreeNodesInfos = new ArrayList();

            if (cFilePath == string.Empty)
            {
                if (openFileDialogCmt.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogCmt.FileName)) return;
                cFilePath = openFileDialogCmt.FileName;
            }
            else
            {
                if (!File.Exists(cFilePath)) return;
            }

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            SortedList slComments = new SortedList();

            try
            {
                sReader = File.OpenText(cFilePath);

                while (!sReader.EndOfStream)
                {
                    sLine = sReader.ReadLine();
                    if (sLine != null)
                    {
                        if (sLine != string.Empty)
                        {
                            sLine = sLine.Trim();
                            if (!bankModeSet && sLine.Length > 1)
                            {
                                bankModeSet = true;
                                if (sLine.Substring(1, 1) == " ")
                                {
                                    bankNum = Convert.ToInt32(sLine.Substring(0, 1));
                                    switch (bankNum)
                                    {
                                        case 8:
                                        case 1:
                                        case 9:
                                        case 0:
                                            bankMode = true;
                                            break;
                                    }
                                }
                            }
                            if (bankModeSet && sLine.Length > 0)
                            {
                                if (sLine.StartsWith("1 |") || sLine.StartsWith("1 #") && uniqueAddress != string.Empty)
                                {
                                    //Shortcut Mode, Bank, Address and UniqueAddress will be reused
                                    sLine = sLine.Substring(1).Trim();
                                }
                                else
                                {
                                    try
                                    {
                                        bankNum = 8;
                                        if (bankMode)
                                        {
                                            bankNum = Convert.ToInt32(sLine.Substring(0, 1));
                                            sLine = sLine.Substring(1).Trim();
                                        }
                                        address = Convert.ToInt32(sLine.Substring(0, 4), 16) - SADDef.EecBankStartAddress;
                                        sLine = sLine.Substring(4).Trim();
                                        uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                    }
                                    catch
                                    {
                                        bankNum = -1;
                                        address = -1;
                                        uniqueAddress = string.Empty;
                                    }
                                }
                                if (uniqueAddress != string.Empty)
                                {
                                    sLine = sLine.Replace("||", "\r\n").Replace("|", "").Replace("#", "").Trim();
                                    if (slComments.ContainsKey(uniqueAddress)) slComments[uniqueAddress] = slComments[uniqueAddress].ToString() + "\r\n" + sLine;
                                    else slComments.Add(uniqueAddress, sLine);
                                }
                            }
                        }
                    }
                }

                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                foreach (string uAddr in slComments.Keys)
                {
                    string sComments = slComments[uAddr].ToString();
                    if (sadS6x.slFunctions.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xFunction)sadS6x.slFunctions[uAddr]).Comments = sComments;
                        ((S6xFunction)sadS6x.slFunctions[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { "FUNCTIONS", uAddr, sComments });
                    }
                    else if (sadS6x.slOperations.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).Comments = sComments;
                        alNewTreeNodesInfos.Add(new string[] { "OPERATIONS", uAddr, sComments });
                    }
                    else if (sadS6x.slRoutines.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xRoutine)sadS6x.slRoutines[uAddr]).Comments = sComments;
                        ((S6xRoutine)sadS6x.slRoutines[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { "ROUTINES", uAddr, sComments });
                    }
                    else if (sadS6x.slScalars.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).Comments = sComments;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { "SCALARS", uAddr, sComments });
                    }
                    else if (sadS6x.slStructures.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xStructure)sadS6x.slStructures[uAddr]).Comments = sComments;
                        ((S6xStructure)sadS6x.slStructures[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { "STRUCTURES", uAddr, sComments });
                    }
                    else if (sadS6x.slTables.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xTable)sadS6x.slTables[uAddr]).Comments = sComments;
                        ((S6xTable)sadS6x.slTables[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { "TABLES", uAddr, sComments });
                    }
                    else
                    {
                        try
                        {
                            bankNum = Convert.ToInt32(uAddr.Substring(0, 1));
                            address = Convert.ToInt32(uAddr.Substring(2).Replace(" ", ""));
                        }
                        catch
                        {
                            bankNum = -1;
                            address = -1;
                        }
                        if (bankNum >= 0 && address >= 0)
                        {
                            S6xOtherAddress other = new S6xOtherAddress();
                            other.BankNum = bankNum;
                            other.AddressInt = address;
                            other.Label = string.Empty;
                            other.Comments = slComments[uAddr].ToString();

                            bool bManage = true;
                            TreeNode tnDuplicate = checkDuplicate(other.UniqueAddress, false);
                            if (tnDuplicate != null)
                            {
                                // Block Reserved and Calibration Load Elements
                                if (tnDuplicate.Parent == null) bManage = false;
                                // Type Conflict
                                else if (tnDuplicate.Parent.Name != "OTHER") bManage = false;
                                tnDuplicate = null;
                            }
                            if (bManage)
                            {
                                if (sadS6x.slOtherAddresses.ContainsKey(other.UniqueAddress))
                                {
                                    ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).Comments = other.Comments;
                                }
                                else
                                {
                                    sadS6x.slOtherAddresses.Add(other.UniqueAddress, other);
                                }
                                alNewTreeNodesInfos.Add(new string[] { "OTHER", other.UniqueAddress, other.Comments, other.UniqueAddressHex });

                                other = null;
                                sadS6x.isSaved = false;
                            }
                        }
                    }
                }

                // Updates First for Threading Purposes
                foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                {
                    TreeNode tnNode = elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes[newTreeNodeInfos[1]];
                    if (tnNode == null) continue;
                    if (tnNode.ToolTipText != newTreeNodeInfos[2]) // For Performance purposes
                    {
                        tnNode.ToolTipText = newTreeNodeInfos[2];
                        tnNode.ForeColor = Color.Purple;
                    }
                    newTreeNodeInfos[0] = null; // To be ignored at creation level
                }
                foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                {
                    if (newTreeNodeInfos[0] == null) continue;
                    TreeNode tnNode = new TreeNode();
                    tnNode.Name = newTreeNodeInfos[1];
                    // Specific Case for OTHER
                    if (newTreeNodeInfos[0] == "OTHER") tnNode.Text = newTreeNodeInfos[3];
                    tnNode.ToolTipText = newTreeNodeInfos[2];
                    tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    tnNode.ForeColor = Color.Red;
                    elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes.Add(tnNode);
                }
                alNewTreeNodesInfos = null;

                // Creations only on OTHER (normally)
                setElementsTreeCategLabel("OTHER");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Comments file import has failed.\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                slComments = null;

                Cursor = processPreviousCursor;

                GC.Collect();
            }
        }

        private void importXdfFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXdfFile(string.Empty);
        }

        private void importXdfFile(string xFilePath)
        {
            XdfFile xdfFile = null;
            object[] arrSyncRes = null;
            string[] arrSyncResAddresses = null;
            string duplicatesMessage = string.Empty;
            string conflictsMessage = string.Empty;
            string ignoredMessage = string.Empty;
            ArrayList alNewTreeNodesInfos = new ArrayList();
            ArrayList alNewDupTreeNodesInfos = new ArrayList();
            ArrayList alReservedAddresses = new ArrayList();

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            if (xFilePath == string.Empty)
            {
                if (openFileDialogXdf.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogXdf.FileName)) return;
                xFilePath = openFileDialogXdf.FileName;
            }
            else
            {
                if (!File.Exists(xFilePath)) return;
            }

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            xdfFile = (XdfFile)ToolsXml.DeserializeFile(xFilePath, typeof(XdfFile));
            if (xdfFile == null)
            {
                Cursor = processPreviousCursor;
                MessageBox.Show("Xdf import has failed.\r\nPlease check it is not encrypted.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (TreeNode tnNode in elemsTreeView.Nodes["RESERVED"].Nodes) alReservedAddresses.Add(tnNode.Name);
            
            arrSyncRes = sadS6x.readFromFileObject(ref xdfFile, ref sadBin, ref alReservedAddresses);
            xdfFile = null;

            alReservedAddresses = null;

            // S6x Creations
            //  First Creations
            arrSyncResAddresses = (string[])arrSyncRes[1];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "TABLES", uniqueAddress, ((S6xTable)sadS6x.slTables[uniqueAddress]).Label, ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments });
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "FUNCTIONS", uniqueAddress, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments });
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "SCALARS", uniqueAddress, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments });
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "STRUCTURES", uniqueAddress, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments });
                }
            }
            arrSyncResAddresses = null;
            
            //  Then Duplicates Creations
            arrSyncResAddresses = (string[])arrSyncRes[6];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { "TABLES", duplicateAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).UniqueAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Label, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Comments });
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { "FUNCTIONS", duplicateAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).UniqueAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Label, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Comments });
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { "SCALARS", duplicateAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).UniqueAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Label, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Comments });
                }
            }
            arrSyncResAddresses = null;

            // S6x Updates
            arrSyncResAddresses = (string[])arrSyncRes[0];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "TABLES", uniqueAddress, ((S6xTable)sadS6x.slTables[uniqueAddress]).Label, ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments });
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "FUNCTIONS", uniqueAddress, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments });
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "SCALARS", uniqueAddress, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments });
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { "STRUCTURES", uniqueAddress, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments });
                }
            }
            arrSyncResAddresses = null;

            //  Duplicates Updates
            arrSyncResAddresses = (string[])arrSyncRes[5];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { "TABLES", duplicateAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).UniqueAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Label, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Comments });
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { "FUNCTIONS", duplicateAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).UniqueAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Label, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Comments });
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { "SCALARS", duplicateAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).UniqueAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Label, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Comments });
                }
            }
            arrSyncResAddresses = null;

            // Updates First for Threading Purposes
            foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
            {
                TreeNode tnNode = elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes[newTreeNodeInfos[1]];
                if (tnNode == null) continue;
                if (tnNode.Text != newTreeNodeInfos[2] || tnNode.ToolTipText != newTreeNodeInfos[3]) // For Performance purposes
                {
                    tnNode.Text = newTreeNodeInfos[2];
                    tnNode.ToolTipText = newTreeNodeInfos[3];
                    tnNode.ForeColor = Color.Purple;
                }
                newTreeNodeInfos[0] = null; // To be ignored at creation level
            }
            // Creations
            foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
            {
                if (newTreeNodeInfos[0] == null) continue;
                TreeNode tnNode = new TreeNode();
                tnNode.Name = newTreeNodeInfos[1];
                tnNode.Text = newTreeNodeInfos[2];
                tnNode.ToolTipText = newTreeNodeInfos[3];
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Red;
                elemsTreeView.Nodes[newTreeNodeInfos[0]].Nodes.Add(tnNode);
            }
            alNewTreeNodesInfos = null;
            //  Duplicates Updates
            foreach (string[] newDupTreeNodeInfos in alNewDupTreeNodesInfos)
            {
                TreeNode tnNode = elemsTreeView.Nodes[newDupTreeNodeInfos[0]].Nodes[newDupTreeNodeInfos[2]].Nodes[newDupTreeNodeInfos[1]];
                if (tnNode == null) continue;
                if (tnNode.Text != newDupTreeNodeInfos[3] || tnNode.ToolTipText != newDupTreeNodeInfos[4]) // For Performance purposes
                {
                    tnNode.Text = newDupTreeNodeInfos[3];
                    tnNode.ToolTipText = newDupTreeNodeInfos[4];
                    tnNode.ForeColor = Color.Purple;
                }
                newDupTreeNodeInfos[0] = null; // To be ignored at creation level
            }
            //  Duplicates Creations
            foreach (string[] newDupTreeNodeInfos in alNewDupTreeNodesInfos)
            {
                if (newDupTreeNodeInfos[0] == null) continue;
                TreeNode tnNode = new TreeNode();
                tnNode.Name = newDupTreeNodeInfos[1];
                tnNode.Text = newDupTreeNodeInfos[3];
                tnNode.ToolTipText = newDupTreeNodeInfos[4];
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Red;
                elemsTreeView.Nodes[newDupTreeNodeInfos[0]].Nodes[newDupTreeNodeInfos[2]].Nodes.Add(tnNode);
            }
            alNewDupTreeNodesInfos = null;

            setElementsTreeCategLabel("TABLES");
            setElementsTreeCategLabel("FUNCTIONS");
            setElementsTreeCategLabel("SCALARS");
            setElementsTreeCategLabel("STRUCTURES");

            /*
             * No Interest Now, Duplicates are managed
            duplicatesMessage = string.Empty;
            arrSyncResAddresses = (string[])arrSyncRes[2];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nTable \"" + ((S6xTable)sadS6x.slTables[uniqueAddress]).Label + "\"";
                if (sadS6x.slFunctions.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nFunction \"" + ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label + "\"";
                if (sadS6x.slScalars.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nScalar \"" + ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label + "\"";
                if (sadS6x.slStructures.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nStructure \"" + ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label + "\"";
            }
            arrSyncResAddresses = null;

            if (duplicatesMessage != string.Empty)
            {
                duplicatesMessage = "Following items are duplicated in Xdf file, first copy only is imported :\r\n" + duplicatesMessage;
                MessageBox.Show(duplicatesMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            */

            conflictsMessage = string.Empty;
            arrSyncResAddresses = (string[])arrSyncRes[3];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nTable \"" + ((S6xTable)sadS6x.slTables[uniqueAddress]).Label + "\"";
                if (sadS6x.slFunctions.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nFunction \"" + ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label + "\"";
                if (sadS6x.slScalars.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nScalar \"" + ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label + "\"";
                if (sadS6x.slStructures.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nStructure \"" + ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label + "\"";
            }
            arrSyncResAddresses = null;

            if (conflictsMessage != string.Empty)
            {
                conflictsMessage = "Following items are in conflict, different external type is not imported :\r\n" + conflictsMessage;
                MessageBox.Show(conflictsMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ignoredMessage = string.Empty;
            if (((string[])arrSyncRes[4]).Length > 0)
            {
                ignoredMessage = "Some elements are reserved addresses and can not be updated or are generated at binary load and should be updated manually.\r\n\r\nThey have been ignored.";
                MessageBox.Show(ignoredMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            arrSyncRes = null;

            GC.Collect();

            Cursor = processPreviousCursor;
        }

        // exportXdfResetToolStripMenuItem_Click
        // Permits to reset XdfUniqueId's to restart on a clean base
        private void exportXdfResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadS6x == null) return;
            
            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                if (s6xObject.XdfUniqueId == null || s6xObject.XdfUniqueId == string.Empty)
                {
                    if (s6xObject.ColsScalerXdfUniqueId == null || s6xObject.ColsScalerXdfUniqueId == string.Empty)
                    {
                        if (s6xObject.RowsScalerXdfUniqueId == null || s6xObject.RowsScalerXdfUniqueId == string.Empty) continue;
                    }
                }
                s6xObject.XdfUniqueId = string.Empty;
                TreeNode tnNode = elemsTreeView.Nodes["TABLES"].Nodes[s6xObject.UniqueAddress];
                if (tnNode == null) continue;
                tnNode.ForeColor = Color.Purple;
                // Duplicates Nodes managed here
                if (tnNode.Nodes == null) continue;
                foreach (TreeNode tnDup in tnNode.Nodes) tnDup.ForeColor = Color.Purple;
            }
            foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
            {
                if (s6xObject.XdfUniqueId == null || s6xObject.XdfUniqueId == string.Empty)
                {
                    if (s6xObject.ColsScalerXdfUniqueId == null || s6xObject.ColsScalerXdfUniqueId == string.Empty)
                    {
                        if (s6xObject.RowsScalerXdfUniqueId == null || s6xObject.RowsScalerXdfUniqueId == string.Empty) continue;
                    }
                }
                s6xObject.XdfUniqueId = string.Empty;
                s6xObject.ColsScalerXdfUniqueId = string.Empty;
                s6xObject.RowsScalerXdfUniqueId = string.Empty;
            }

            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                if (s6xObject.XdfUniqueId == null || s6xObject.XdfUniqueId == string.Empty) continue;
                s6xObject.XdfUniqueId = string.Empty;
                TreeNode tnNode = elemsTreeView.Nodes["FUNCTIONS"].Nodes[s6xObject.UniqueAddress];
                if (tnNode == null) continue;
                tnNode.ForeColor = Color.Purple;
                // Duplicates Nodes managed here
                if (tnNode.Nodes == null) continue;
                foreach (TreeNode tnDup in tnNode.Nodes) tnDup.ForeColor = Color.Purple;
            }
            foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
            {
                if (s6xObject.XdfUniqueId == null || s6xObject.XdfUniqueId == string.Empty) continue;
                s6xObject.XdfUniqueId = string.Empty;
            }

            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                if (s6xObject.XdfUniqueId == null || s6xObject.XdfUniqueId == string.Empty) continue;
                s6xObject.XdfUniqueId = string.Empty;
                TreeNode tnNode = elemsTreeView.Nodes["SCALARS"].Nodes[s6xObject.UniqueAddress];
                if (tnNode == null) continue;
                tnNode.ForeColor = Color.Purple;
                // Duplicates Nodes managed here
                if (tnNode.Nodes == null) continue;
                foreach (TreeNode tnDup in tnNode.Nodes) tnDup.ForeColor = Color.Purple;
            }
            foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
            {
                if (s6xObject.XdfUniqueId == null || s6xObject.XdfUniqueId == string.Empty) continue;
                s6xObject.XdfUniqueId = string.Empty;
            }

            sadS6x.isSaved = false;

            Cursor = processPreviousCursor;
        }

        private void exportXdfFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XdfFile xdfFile = null;
            StreamWriter sWri = null;
            object[] arrSyncRes = null;
            string[] arrSyncResAddresses = null;
            string duplicatesMessage = string.Empty;

            if (sadBin == null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }
            
            if (saveFileDialogXdf.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (File.Exists(saveFileDialogXdf.FileName))
            {
                xdfFile = (XdfFile)ToolsXml.DeserializeFile(saveFileDialogXdf.FileName, typeof(XdfFile));
                if (xdfFile == null)
                {
                    MessageBox.Show("Xdf matching has failed.\r\nPlease check destination file is not encrypted.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Xdf Original file autmatic Backup
                try
                {
                    File.Copy(saveFileDialogXdf.FileName, saveFileDialogXdf.FileName + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    xdfFile = null;
                    MessageBox.Show("Xdf backup has failed.\r\nNo other action will be managed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                arrSyncRes = sadS6x.writeToFileObject(ref xdfFile, ref sadBin);
            }
            else
            {
                xdfFile = new XdfFile(ref sadBin);
                sadS6x.writeToFileObject(ref xdfFile, ref sadBin);
                arrSyncRes = new object[] { new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { } };
            }

            // UnicodeEncoding(false, true) works fine, UTF-8 with BOM has issues, like ASCII
            sWri = new StreamWriter(saveFileDialogXdf.FileName, false, new UnicodeEncoding(false, true));
            ToolsXml.Serialize(ref sWri, xdfFile);
            sWri.Close();
            sWri.Dispose();

            sWri = null;
            xdfFile = null;

            // S6x Updates to be applied to Tree View - No Creation
            arrSyncResAddresses = (string[])arrSyncRes[0];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                TreeNode tnNode = null;
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    tnNode = elemsTreeView.Nodes["TABLES"].Nodes[uniqueAddress];
                    tnNode.Text = ((S6xTable)sadS6x.slTables[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    tnNode = elemsTreeView.Nodes["FUNCTIONS"].Nodes[uniqueAddress];
                    tnNode.Text = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    tnNode = elemsTreeView.Nodes["SCALARS"].Nodes[uniqueAddress];
                    tnNode.Text = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    tnNode = elemsTreeView.Nodes["STRUCTURES"].Nodes[uniqueAddress];
                    tnNode.Text = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
            }
            arrSyncResAddresses = null;
            
            // Same thing for duplicates
            arrSyncResAddresses = (string[])arrSyncRes[5];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                TreeNode tnNode = null;
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    S6xTable s6xObject = (S6xTable)sadS6x.slDupTables[duplicateAddress];
                    tnNode = elemsTreeView.Nodes["TABLES"].Nodes[s6xObject.UniqueAddress].Nodes[duplicateAddress];
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    S6xFunction s6xObject = (S6xFunction)sadS6x.slDupFunctions[duplicateAddress];
                    tnNode = elemsTreeView.Nodes["FUNCTIONS"].Nodes[s6xObject.UniqueAddress].Nodes[duplicateAddress];
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    S6xScalar s6xObject = (S6xScalar)sadS6x.slDupScalars[duplicateAddress];
                    tnNode = elemsTreeView.Nodes["SCALARS"].Nodes[s6xObject.UniqueAddress].Nodes[duplicateAddress];
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
            }
            arrSyncResAddresses = null;

            /*
             * No Interest Now, Duplicates are managed

            duplicatesMessage = string.Empty;
            arrSyncResAddresses = (string[])arrSyncRes[2];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nTable \"" + ((S6xTable)sadS6x.slTables[uniqueAddress]).Label + "\"";
                if (sadS6x.slFunctions.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nFunction \"" + ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label + "\"";
                if (sadS6x.slScalars.ContainsKey(uniqueAddress)) duplicatesMessage += "\r\nScalar \"" + ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label + "\"";
            }
            arrSyncResAddresses = null;

            if (duplicatesMessage != string.Empty)
            {
                duplicatesMessage = "Following items are duplicated in Xdf file, first copy only is synchronized :\r\n" + duplicatesMessage;
                MessageBox.Show(duplicatesMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            */

            // Conflicts Mngt not needed

            arrSyncRes = null;

            GC.Collect();

            Cursor = processPreviousCursor;
        }

        private void showHexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortedList slOrderedBanks = new SortedList();
            
            if (sadBin != null)
            {
                if (sadBin.isLoaded && sadBin.isValid)
                {
                    if (sadBin.Bank8 != null) slOrderedBanks.Add(sadBin.Bank8.AddressBinInt, new int[] { 8, sadBin.Bank8.AddressBinInt, sadBin.Bank8.AddressBinEndInt });
                    if (sadBin.Bank1 != null) slOrderedBanks.Add(sadBin.Bank1.AddressBinInt, new int[] { 1, sadBin.Bank1.AddressBinInt, sadBin.Bank1.AddressBinEndInt });
                    if (sadBin.Bank9 != null) slOrderedBanks.Add(sadBin.Bank9.AddressBinInt, new int[] { 9, sadBin.Bank9.AddressBinInt, sadBin.Bank9.AddressBinEndInt });
                    if (sadBin.Bank0 != null) slOrderedBanks.Add(sadBin.Bank0.AddressBinInt, new int[] { 0, sadBin.Bank0.AddressBinInt, sadBin.Bank0.AddressBinEndInt });
                }
            }

            HexForm hexForm = new HexForm(sadBin.getBinBytes, slOrderedBanks);
            hexForm.Show();

            hexForm = null;
            slOrderedBanks = null;
        }

        private void textOutputToolStripTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (!File.Exists(textOutputFilePath)) return;

            Process.Start(textOutputFilePath);
        }

        private void routineAdvButton_Click(object sender, EventArgs e)
        {
            string uniqueAddress = string.Empty;
            S6xRoutine tempRoutine = null;
            S6xRoutine s6xRoutine = null;

            if (routineAdvButton.Tag != null)
            {
                s6xRoutine = (S6xRoutine)routineAdvButton.Tag;
            }
            else if (nextElemTreeNode != null)
            {
                uniqueAddress = nextElemTreeNode.Name;
                s6xRoutine = (S6xRoutine)sadS6x.slRoutines[uniqueAddress];
            }

            if (s6xRoutine == null) tempRoutine = new S6xRoutine();
            else tempRoutine = s6xRoutine.Clone();
            s6xRoutine = null;

            tempRoutine.Label = routineLabelTextBox.Text;

            RoutineForm routineForm = new RoutineForm(ref sadS6x, ref tempRoutine);
            routineForm.ShowDialog();
            routineForm = null;

            tempRoutine.ByteArgumentsNum = 0;
            if (tempRoutine.InputArguments != null)
            {
                foreach (S6xRoutineInputArgument arg in tempRoutine.InputArguments)
                {
                    tempRoutine.ByteArgumentsNum++;
                    if (arg.Word) tempRoutine.ByteArgumentsNum++;
                }
            }
            routineArgsNumTextBox.Text = tempRoutine.ByteArgumentsNum.ToString();
            signatureAdvCheckBox.Checked = tempRoutine.isAdvanced;
            
            // To be reused on Update
            routineAdvButton.Tag = tempRoutine;

            elemProperties_Modified(routineAdvButton, new EventArgs());

            tempRoutine = null;
        }

        private void signatureAvdButton_Click(object sender, EventArgs e)
        {
            string uniqueKey = string.Empty;
            S6xSignature tempSig = null;
            S6xSignature s6xSig = null;

            if (signatureAdvButton.Tag != null)
            {
                s6xSig = (S6xSignature)signatureAdvButton.Tag;
            }
            else if (nextElemTreeNode != null)
            {
                uniqueKey = nextElemTreeNode.Name;
                s6xSig = (S6xSignature)sadS6x.slSignatures[uniqueKey];
            }

            if (s6xSig == null) tempSig = new S6xSignature();
            else tempSig = s6xSig.Clone();
            s6xSig = null;

            tempSig.Label = signatureLabelTextBox.Text;
            tempSig.ShortLabel = signatureSLabelTextBox.Text;
            tempSig.Signature = signatureSigTextBox.Text;

            SigForm sigForm = new SigForm(ref sadS6x, ref tempSig);
            sigForm.ShowDialog();
            sigForm = null;

            signatureSigTextBox.Text = tempSig.Signature;
            signatureAdvCheckBox.Checked = tempSig.isAdvanced;
            
            // To be reused on Update
            signatureAdvButton.Tag = tempSig;

            elemProperties_Modified(signatureAdvButton, new EventArgs());

            tempSig = null;
        }

        private void elementSignatureElemButton_Click(object sender, EventArgs e)
        {
            string uniqueKey = string.Empty;
            S6xElementSignature tempESig = null;
            S6xElementSignature s6xESig = null;

            if (elementSignatureElemButton.Tag != null)
            {
                s6xESig = (S6xElementSignature)elementSignatureElemButton.Tag;
            }
            else if (nextElemTreeNode != null)
            {
                uniqueKey = nextElemTreeNode.Name;
                s6xESig = (S6xElementSignature)sadS6x.slElementsSignatures[uniqueKey];
            }

            if (s6xESig == null)
            {
                tempESig = new S6xElementSignature();
                tempESig.for8061 = true;
                tempESig.Scalar = new S6xRoutineInternalScalar();
                tempESig.Scalar.Label = "New Scalar";
                tempESig.Scalar.ShortLabel = SADDef.ShortScalarPrefix + string.Format("{0:d4}", sadS6x.slScalars.Count + 1);
                tempESig.Scalar.Byte = true;
                tempESig.Scalar.ScaleExpression = "X";
                tempESig.Scalar.ScalePrecision = SADDef.DefaultScalePrecision;
            }
            else
            {
                tempESig = s6xESig.Clone();
            }
            s6xESig = null;

            tempESig.SignatureLabel = elementSignatureLabelTextBox.Text;
            tempESig.Signature = elementSignatureSigTextBox.Text;

            ElemSigForm eSigForm = new ElemSigForm(ref sadS6x, ref tempESig);
            eSigForm.ShowDialog();
            eSigForm = null;

            elementSignatureSigTextBox.Text = tempESig.Signature;
            if (tempESig.Scalar != null) elementSignatureTypeComboBox.SelectedIndex = 0;
            else if (tempESig.Function != null) elementSignatureTypeComboBox.SelectedIndex = 1;
            else if (tempESig.Table != null) elementSignatureTypeComboBox.SelectedIndex = 2;
            else if (tempESig.Structure != null) elementSignatureTypeComboBox.SelectedIndex = 3;

            // To be reused on Update
            elementSignatureElemButton.Tag = tempESig;

            elemProperties_Modified(elementSignatureElemButton, new EventArgs());

            tempESig = null;
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
            else if (nextElemTreeNode != null)
            {
                uniqueAddress = nextElemTreeNode.Name;
                s6xScalar = (S6xScalar)sadS6x.slScalars[uniqueAddress];
            }

            if (s6xScalar == null) tempScalar = new S6xScalar();
            else tempScalar = s6xScalar.Clone();
            s6xScalar = null;

            tempScalar.Label = scalarLabelTextBox.Text;
            tempScalar.Byte = scalarByteCheckBox.Checked;

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref sadS6x, ref tempScalar);
            bitFlagsForm.ShowDialog();
            bitFlagsForm = null;

            scalarBitFlagsCheckBox.Checked = tempScalar.isBitFlags;

            // To be reused on Update
            scalarBitFlagsButton.Tag = tempScalar;

            elemProperties_Modified(scalarBitFlagsButton, new EventArgs());

            tempScalar = null;
        }

        private void regBitFlagsButton_Click(object sender, EventArgs e)
        {
            string uniqueAddress = string.Empty;
            S6xRegister tempReg = null;
            S6xRegister s6xReg = null;

            if (regBitFlagsButton.Tag != null)
            {
                s6xReg = (S6xRegister)regBitFlagsButton.Tag;
            }
            else if (nextElemTreeNode != null)
            {
                uniqueAddress = nextElemTreeNode.Name;
                s6xReg = (S6xRegister)sadS6x.slRegisters[uniqueAddress];
            }

            if (s6xReg == null) tempReg = new S6xRegister();
            else tempReg = s6xReg.Clone();
            s6xReg = null;

            tempReg.Label = regLabelTextBox.Text;

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref sadS6x, ref tempReg);
            bitFlagsForm.ShowDialog();
            bitFlagsForm = null;

            regBitFlagsCheckBox.Checked = tempReg.isBitFlags;

            // To be reused on Update
            regBitFlagsButton.Tag = tempReg;

            elemProperties_Modified(regBitFlagsButton, new EventArgs());

            tempReg = null;
        }

        private void searchObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SearchForm(ref sadS6x, ref elemsTreeView).ShowDialog();
        }

        private void compareBinariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, true, true).ShowDialog();
        }

        private void compareBinariesDifDefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, true, false).ShowDialog();
        }

        private void compareS6xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, false, false).ShowDialog();
        }

        private void searchSignatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SearchSignatureForm(ref sadBin).ShowDialog();
        }

        private void routinesComparisonSkeletonExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;
            if (!sadBin.isDisassembled) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            FileInfo fiFI = new FileInfo(binaryFilePath);
            try { saveFileDialogSkt.FileName = fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".skt"; }
            catch { }
            fiFI = null;

            if (saveFileDialogSkt.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                SortedList slResult = Tools.getRoutinesComparisonSkeleton(ref sadBin);
                Tools.exportRoutinesComparisonSkeleton(ref slResult, saveFileDialogSkt.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Routines skeleton file export has failed.\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = processPreviousCursor;
            }
        }

        private void routinesComparisonSkeletonsCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CompareRoutinesForm(ref sadBin, ref elemsTreeView, ref elemsContextMenuStrip, true).ShowDialog();
        }

        private void routinesComparisonBinariesCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CompareRoutinesForm(ref sadBin, ref elemsTreeView, ref elemsContextMenuStrip, false).ShowDialog();
        }

        private void calibChartViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                new CompareGraphForm(ref sadBin, ref sadS6x, ref elemsTreeView).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void routinesComparisonSkeletonsAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sAbout = string.Empty;
            sAbout = "Routines comparison principle:\n\r\n\r";
            sAbout += "Purpose:\n\r";
            sAbout += "- To match routines between two disassembled binaries\n\r";
            sAbout += "\n\rSkeletons generation Method:\n\r";
            sAbout += "- Skeletons file (.skt) has to be generated after disassembly.\n\r";
            sAbout += "- Routine start and end are here based only on jumps and returns operations.\n\r";
            sAbout += "- Skeleton of routine is generated base on Operations codes.\n\r";
            sAbout += "\n\rSkeletons files comparison Method:\n\r";
            sAbout += "- Two different Skeletons files (.skt) have to be selected.\n\r";
            sAbout += "- Based on routine size (Minimum Operations Count), routine can be ignored.\n\r";
            sAbout += "- Based on routines size difference (Operations Count Gap Maximum Tolerance %), matching can be ignored.\n\r";
            sAbout += "- Based on routines skeletons proximity (Damerau Levenshtein Distance Minimum Tolerance %), matching can be ignored.\n\r";
            sAbout += "- Reports will be outputted, including possible matches with % chance.\n\r";
            sAbout += "- One report for each way.";

            MessageBox.Show(sAbout, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void repoRegistersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("Registers", Application.StartupPath + "\\" + SADDef.repoFileNameRegisters, "REGISTERS", ref sadS6x).ShowDialog();
            
            initRepositories();
        }

        private void repoTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("Tables", Application.StartupPath + "\\" + SADDef.repoFileNameTables, string.Empty, ref sadS6x).ShowDialog();

            initRepositories();
        }

        private void repoFunctionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("Functions", Application.StartupPath + "\\" + SADDef.repoFileNameFunctions, string.Empty, ref sadS6x).ShowDialog();

            initRepositories();
        }

        private void repoScalarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("Scalars", Application.StartupPath + "\\" + SADDef.repoFileNameScalars, string.Empty, ref sadS6x).ShowDialog();

            initRepositories();
        }

        private void repoStructuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("Structures", Application.StartupPath + "\\" + SADDef.repoFileNameStructures, string.Empty, ref sadS6x).ShowDialog();

            initRepositories();
        }

        private void repoUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("Units", Application.StartupPath + "\\" + SADDef.repoFileNameUnits, "UNITS", ref sadS6x).ShowDialog();

            initRepositories();
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
                case "elemRegisterTabPage":
                    searchLabel = regLabelTextBox.Text;
                    break;
                case "elemTablePropertiesTabPage":
                    searchLabel = tableSLabelTextBox.Text;
                    break;
                case "elemFunctionPropertiesTabPage":
                    searchLabel = functionSLabelTextBox.Text;
                    break;
                case "elemScalarPropertiesTabPage":
                    searchLabel = scalarSLabelTextBox.Text;
                    break;
                case "elemStructurePropertiesTabPage":
                    searchLabel = structureSLabelTextBox.Text;
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
                case "elemRegisterTabPage":
                    repoRepository = repoRegisters;
                    break;
                case "elemTablePropertiesTabPage":
                    repoRepository = repoTables;
                    break;
                case "elemFunctionPropertiesTabPage":
                    repoRepository = repoFunctions;
                    break;
                case "elemScalarPropertiesTabPage":
                    repoRepository = repoScalars;
                    break;
                case "elemStructurePropertiesTabPage":
                    repoRepository = repoStructures;
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
                case "elemRegisterTabPage":
                    regLabelTextBox.Text = sSLabel;
                    regCommentsTextBox.Text = sComments;
                    elemProperties_Modified(regLabelTextBox, EventArgs.Empty);
                    break;
                case "elemTablePropertiesTabPage":
                    tableSLabelTextBox.Text = sSLabel;
                    tableLabelTextBox.Text = sLLabel;
                    tableCommentsTextBox.Text = sComments;
                    elemProperties_Modified(tableSLabelTextBox, EventArgs.Empty);
                    break;
                case "elemFunctionPropertiesTabPage":
                    functionSLabelTextBox.Text = sSLabel;
                    functionLabelTextBox.Text = sLLabel;
                    functionCommentsTextBox.Text = sComments;
                    elemProperties_Modified(functionSLabelTextBox, EventArgs.Empty);
                    break;
                case "elemScalarPropertiesTabPage":
                    scalarSLabelTextBox.Text = sSLabel;
                    scalarLabelTextBox.Text = sLLabel;
                    scalarCommentsTextBox.Text = sComments;
                    elemProperties_Modified(scalarSLabelTextBox, EventArgs.Empty);
                    break;
                case "elemStructurePropertiesTabPage":
                    structureSLabelTextBox.Text = sSLabel;
                    structureLabelTextBox.Text = sLLabel;
                    structureCommentsTextBox.Text = sComments;
                    elemProperties_Modified(structureSLabelTextBox, EventArgs.Empty);
                    break;
                case "tableCellsUnitsTextBox":
                    tableCellsUnitsTextBox.Text = sSLabel;
                    elemProperties_Modified(tableCellsUnitsTextBox, EventArgs.Empty);
                    break;
                case "tableColsUnitsTextBox":
                    tableColsUnitsTextBox.Text = sSLabel;
                    elemProperties_Modified(tableColsUnitsTextBox, EventArgs.Empty);
                    break;
                case "tableRowsUnitsTextBox":
                    tableRowsUnitsTextBox.Text = sSLabel;
                    elemProperties_Modified(tableRowsUnitsTextBox, EventArgs.Empty);
                    break;
                case "functionUnitsInputTextBox":
                    functionUnitsInputTextBox.Text = sSLabel;
                    elemProperties_Modified(functionUnitsInputTextBox, EventArgs.Empty);
                    break;
                case "functionUnitsOutputTextBox":
                    functionUnitsOutputTextBox.Text = sSLabel;
                    elemProperties_Modified(functionUnitsOutputTextBox, EventArgs.Empty);
                    break;
                case "scalarUnitsTextBox":
                    scalarUnitsTextBox.Text = sSLabel;
                    elemProperties_Modified(scalarUnitsTextBox, EventArgs.Empty);
                    break;
                case "tableScaleTextBox":
                    tableScaleTextBox.Text = sLLabel;
                    elemProperties_Modified(tableScaleTextBox, EventArgs.Empty);
                    break;
                case "functionScaleInputTextBox":
                    functionScaleInputTextBox.Text = sLLabel;
                    elemProperties_Modified(functionScaleInputTextBox, EventArgs.Empty);
                    break;
                case "functionScaleOutputTextBox":
                    functionScaleOutputTextBox.Text = sLLabel;
                    elemProperties_Modified(functionScaleOutputTextBox, EventArgs.Empty);
                    break;
                case "scalarScaleTextBox":
                    scalarScaleTextBox.Text = sLLabel;
                    elemProperties_Modified(scalarScaleTextBox, EventArgs.Empty);
                    break;
                default:
                    repoContextMenuStrip.Tag = null;
                    return;
            }
        }

        private void repoConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConversionRepoForm(Application.StartupPath + "\\" + SADDef.repoFileNameConversion, ref sadS6x).ShowDialog();

            initRepositoryConversion();
        }

        private void tableScalerButton_Click(object sender, EventArgs e)
        {
            scalerContextMenuStrip.Tag = sender;
            if (((Button)sender).Tag == null) scalerToolStripTextBox.Text = "None";
            else scalerToolStripTextBox.Text = ((S6xFunction)((Button)sender).Tag).ShortLabel;
            scalerContextMenuStrip.Show(Cursor.Position);
        }

        private void tableScalerButton_MouseHover(object sender, EventArgs e)
        {
            if (((Button)sender).Tag == null) return;
            S6xFunction s6xScaler = (S6xFunction)((Button)sender).Tag;
            string toolTip = string.Empty;
            if (s6xScaler.Label != null && s6xScaler.Label != string.Empty) toolTip += s6xScaler.Label + "\r\n\r\n";
            if (s6xScaler.ShortLabel != null && s6xScaler.ShortLabel != string.Empty) toolTip += s6xScaler.ShortLabel + "\r\n\r\n";
            if (s6xScaler.Comments != null && s6xScaler.Comments != string.Empty) toolTip += s6xScaler.Comments;

            mainToolTip.SetToolTip((Button)sender, toolTip);
        }

        private void scalerContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            repoToolStripTextBox_TextChanged(sender, e);
        }

        private void scalerToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            scalerToolStripMenuItem.DropDownItems.Clear();
            scalerToolStripMenuItem.DropDownItems.Add("None");

            if (scalerContextMenuStrip.Tag == null) return;

            string searchLabel = scalerToolStripTextBox.Text.ToUpper();

            SortedList slFiltered = new SortedList();
            foreach (S6xFunction s6xScaler in sadS6x.slFunctions.Values)
            {
                string sLabel = s6xScaler.ShortLabel;
                string label = s6xScaler.Label;

                if (sLabel == null) continue;
                if (label == null) label = string.Empty;

                if (searchLabel == string.Empty || sLabel.ToUpper().Contains(searchLabel) || label.ToUpper().Contains(searchLabel))
                {
                    if (!slFiltered.ContainsKey(s6xScaler.UniqueAddress)) slFiltered.Add(s6xScaler.UniqueAddress, s6xScaler);
                }
            }
            foreach (S6xFunction s6xScaler in sadS6x.slDupFunctions.Values)
            {
                if (slFiltered.ContainsKey(s6xScaler.UniqueAddress))
                {
                    if (!slFiltered.ContainsKey(s6xScaler.DuplicateAddress)) slFiltered.Add(s6xScaler.DuplicateAddress, s6xScaler);
                }
            }

            foreach (S6xFunction s6xScaler in slFiltered.Values)
            {
                ToolStripMenuItem tsMI = new ToolStripMenuItem();
                tsMI.Tag = s6xScaler;
                tsMI.Text = s6xScaler.Label;
                tsMI.ToolTipText = string.Empty;
                if (s6xScaler.ShortLabel != null && s6xScaler.ShortLabel != string.Empty) tsMI.ToolTipText += s6xScaler.ShortLabel + "\r\n\r\n";
                if (s6xScaler.Comments != null && s6xScaler.Comments != string.Empty) tsMI.ToolTipText += s6xScaler.Comments + "\r\n\r\n";
                if (s6xScaler.DuplicateNum == 0)
                {
                    scalerToolStripMenuItem.DropDownItems.Add(tsMI);
                    // Limited to 50
                    if (scalerToolStripMenuItem.DropDownItems.Count >= 50) break;
                }
                else
                {
                    foreach (ToolStripMenuItem tsMainMI in scalerToolStripMenuItem.DropDownItems)
                    {
                        if (tsMainMI.Tag == null) continue;
                        if (((S6xFunction)tsMainMI.Tag).UniqueAddress != s6xScaler.UniqueAddress) continue;
                        if (tsMainMI.DropDownItems.Count == 0)
                        {
                            ToolStripMenuItem tsMainHeader = new ToolStripMenuItem();
                            tsMainHeader.Tag = tsMainMI.Tag;
                            tsMainHeader.Text = tsMainMI.Text;
                            tsMainHeader.ToolTipText = tsMainMI.ToolTipText;
                            tsMainMI.DropDownItems.Add(tsMainHeader);
                            tsMainMI.DropDownItems.Add(new ToolStripSeparator());
                            tsMainMI.DropDownItemClicked += scalerToolStripMenuItem_DropDownItemClicked;
                        }
                        tsMainMI.DropDownItems.Add(tsMI);
                        break;
                    }
                }
            }
            slFiltered = null;
        }

        private void scalerToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (scalerContextMenuStrip.Tag == null) return;
            Button tableScalerButton = (Button)scalerContextMenuStrip.Tag;

            elemProperties_Modified(tableScalerButton, e);

            if (e.ClickedItem.Tag == null)
            {
                tableScalerButton.Text = string.Empty;
                tableScalerButton.Tag = null;
                return;
            }

            tableScalerButton.Text = ((S6xFunction)e.ClickedItem.Tag).Label;
            tableScalerButton.Tag = e.ClickedItem.Tag;

            string sUnits = ((S6xFunction)tableScalerButton.Tag).InputUnits;
            if (sUnits == null || sUnits == string.Empty) return;

            switch (tableScalerButton.Name)
            {
                case "tableColsScalerButton":
                    if (tableColsUnitsTextBox.Text != string.Empty) return;
                    tableColsUnitsTextBox.Text = sUnits;
                    break;
                case "tableRowsScalerButton":
                    if (tableRowsUnitsTextBox.Text != string.Empty) return;
                    tableRowsUnitsTextBox.Text = sUnits;
                    break;
            }
        }

        private void categCleanElements_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.SelectedNode == null) return;
            if (elemsTreeView.SelectedNode.Parent != null) return;

            if (lastElemTreeNode != null && dirtyProperties)
            {
                if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    nextElemTreeNode = lastElemTreeNode;
                    if (isDuplicate(nextElemTreeNode)) elemsTreeView.Tag = nextElemTreeNode.Parent.Parent;
                    else elemsTreeView.Tag = nextElemTreeNode.Parent;
                    return;
                }
            }

            ArrayList alRemoval = null;

            switch (getCategName(elemsTreeView.SelectedNode))
            {
                case "TABLES":
                    alRemoval = new ArrayList();
                    foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                    {
                        if (s6xObject == null) continue;
                        if (!s6xObject.Store) continue;
                        if (s6xObject.Skip) continue;
                        if (s6xObject.isUserDefined) continue;

                        alRemoval.Add(s6xObject.UniqueAddress);
                    }
                    foreach (string uniqueAddress in alRemoval)
                    {
                        TreeNode tnNode = elemsTreeView.SelectedNode.Nodes[uniqueAddress];
                        if (tnNode == null)
                        {
                            sadS6x.slTables.Remove(uniqueAddress);
                            continue;
                        }

                        // Duplicates exist
                        if (tnNode.Nodes.Count > 0) continue;

                        deleteElem(tnNode, true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;

                    // Duplicates
                    alRemoval = new ArrayList();
                    foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                    {
                        if (s6xObject == null) continue;
                        if (!s6xObject.Store) continue;
                        if (s6xObject.Skip) continue;
                        if (s6xObject.isUserDefined) continue;

                        alRemoval.Add(s6xObject.UniqueAddress);
                    }

                    foreach (string uniqueAddress in alRemoval)
                    {
                        TreeNode tnMainNode = elemsTreeView.SelectedNode.Nodes[uniqueAddress];
                        if (tnMainNode == null)
                        {
                            sadS6x.slDupTables.Remove(uniqueAddress);
                            continue;
                        }
                        TreeNode tnNode = tnMainNode.Nodes[uniqueAddress];
                        if (tnNode == null)
                        {
                            sadS6x.slDupTables.Remove(uniqueAddress);
                            continue;
                        }

                        deleteElem(tnNode, true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;
                    break;
                case "FUNCTIONS":
                    alRemoval = new ArrayList();
                    foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                    {
                        if (s6xObject == null) continue;
                        if (!s6xObject.Store) continue;
                        if (s6xObject.Skip) continue;
                        if (s6xObject.isUserDefined) continue;

                        alRemoval.Add(s6xObject.UniqueAddress);
                    }
                    foreach (string uniqueAddress in alRemoval)
                    {
                        TreeNode tnNode = elemsTreeView.SelectedNode.Nodes[uniqueAddress];
                        if (tnNode == null)
                        {
                            sadS6x.slFunctions.Remove(uniqueAddress);
                            continue;
                        }
                        
                        // Duplicates exist
                        if (tnNode.Nodes.Count > 0) continue;

                        deleteElem(tnNode, true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;

                    // Duplicates
                    alRemoval = new ArrayList();
                    foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                    {
                        if (s6xObject == null) continue;
                        if (!s6xObject.Store) continue;
                        if (s6xObject.Skip) continue;
                        if (s6xObject.isUserDefined) continue;

                        alRemoval.Add(s6xObject.UniqueAddress);
                    }

                    foreach (string uniqueAddress in alRemoval)
                    {
                        TreeNode tnMainNode = elemsTreeView.SelectedNode.Nodes[uniqueAddress];
                        if (tnMainNode == null)
                        {
                            sadS6x.slDupFunctions.Remove(uniqueAddress);
                            continue;
                        }
                        TreeNode tnNode = tnMainNode.Nodes[uniqueAddress];
                        if (tnNode == null)
                        {
                            sadS6x.slDupFunctions.Remove(uniqueAddress);
                            continue;
                        }

                        deleteElem(tnNode, true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;
                    break;
                case "SCALARS":
                    alRemoval = new ArrayList();
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        if (s6xObject == null) continue;
                        if (!s6xObject.Store) continue;
                        if (s6xObject.Skip) continue;
                        if (s6xObject.isUserDefined) continue;

                        alRemoval.Add(s6xObject.UniqueAddress);
                    }
                    foreach (string uniqueAddress in alRemoval)
                    {
                        TreeNode tnNode = elemsTreeView.SelectedNode.Nodes[uniqueAddress];
                        if (tnNode == null)
                        {
                            sadS6x.slScalars.Remove(uniqueAddress);
                            continue;
                        }

                        // Duplicates exist
                        if (tnNode.Nodes.Count > 0) continue;

                        deleteElem(tnNode, true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;

                    // Duplicates
                    alRemoval = new ArrayList();
                    foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                    {
                        if (s6xObject == null) continue;
                        if (!s6xObject.Store) continue;
                        if (s6xObject.Skip) continue;
                        if (s6xObject.isUserDefined) continue;

                        alRemoval.Add(s6xObject.UniqueAddress);
                    }
                    foreach (string uniqueAddress in alRemoval)
                    {
                        TreeNode tnMainNode = elemsTreeView.SelectedNode.Nodes[uniqueAddress];
                        if (tnMainNode == null)
                        {
                            sadS6x.slDupScalars.Remove(uniqueAddress);
                            continue;
                        }
                        TreeNode tnNode = tnMainNode.Nodes[uniqueAddress];
                        if (tnNode == null)
                        {
                            sadS6x.slDupScalars.Remove(uniqueAddress);
                            continue;
                        }

                        deleteElem(tnNode, true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;
                    break;
            }
            alRemoval = null;
        }

        private void shortCutsElementResetRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteToolStripMenuItem_Click(sender, e);
        }

        private void settingsTextOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsForm("Text Output Settings", Application.StartupPath + "\\" + SADDef.settingsTextOuputFileName, "TEXTOUTPUT").ShowDialog();
        }

        private void muCBSLLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnCateg = null;

            if ((ToolStripMenuItem)sender == muCBSLLTablesToolStripMenuItem)
            {
                tnCateg = elemsTreeView.Nodes["TABLES"];
                foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        sadS6x.isSaved = false;

                        if (tnCateg == null) continue;
                        TreeNode tnNode = tnCateg.Nodes[s6xObject.UniqueAddress];
                        if (tnNode == null) continue;
                        tnNode.ToolTipText = s6xObject.Comments;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
                foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        sadS6x.isSaved = false;

                        if (tnCateg == null) continue;
                        TreeNode tnNode = tnCateg.Nodes[s6xObject.UniqueAddress];
                        if (tnNode == null) continue;
                        TreeNode tnDupNode = tnNode.Nodes[s6xObject.DuplicateAddress];
                        if (tnDupNode == null) continue;
                        tnDupNode.ToolTipText = s6xObject.Comments;
                        tnDupNode.ForeColor = Color.Purple;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
            }
            else if ((ToolStripMenuItem)sender == muCBSLLFunctionsToolStripMenuItem)
            {
                tnCateg = elemsTreeView.Nodes["FUNCTIONS"];
                foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        sadS6x.isSaved = false;

                        if (tnCateg == null) continue;
                        TreeNode tnNode = tnCateg.Nodes[s6xObject.UniqueAddress];
                        if (tnNode == null) continue;
                        tnNode.ToolTipText = s6xObject.Comments;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
                foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        sadS6x.isSaved = false;

                        if (tnCateg == null) continue;
                        TreeNode tnNode = tnCateg.Nodes[s6xObject.UniqueAddress];
                        if (tnNode == null) continue;
                        TreeNode tnDupNode = tnNode.Nodes[s6xObject.DuplicateAddress];
                        if (tnDupNode == null) continue;
                        tnDupNode.ToolTipText = s6xObject.Comments;
                        tnDupNode.ForeColor = Color.Purple;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
            }
            else if ((ToolStripMenuItem)sender == muCBSLLScalarsToolStripMenuItem)
            {
                tnCateg = elemsTreeView.Nodes["SCALARS"];
                foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        sadS6x.isSaved = false;

                        if (tnCateg == null) continue;
                        TreeNode tnNode = tnCateg.Nodes[s6xObject.UniqueAddress];
                        if (tnNode == null) continue;
                        tnNode.ToolTipText = s6xObject.Comments;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
                foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        sadS6x.isSaved = false;

                        if (tnCateg == null) continue;
                        TreeNode tnNode = tnCateg.Nodes[s6xObject.UniqueAddress];
                        if (tnNode == null) continue;
                        TreeNode tnDupNode = tnNode.Nodes[s6xObject.DuplicateAddress];
                        if (tnDupNode == null) continue;
                        tnDupNode.ToolTipText = s6xObject.Comments;
                        tnDupNode.ForeColor = Color.Purple;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
            }
        }
    }
}
