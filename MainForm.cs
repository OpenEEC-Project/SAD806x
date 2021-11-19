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
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using SQLite806x;

namespace SAD806x
{
    public partial class MainForm : Form
    {
        private bool activatedUniDb806x = true;
        
        private string binaryFilePath = string.Empty;
        private string s6xFilePath = string.Empty;
        private string textOutputFilePath = string.Empty;
        private string documentationFilePath = string.Empty;

        private SADBin sadBin = null;
        private SADS6x sadS6x = null;

        private SADProcessManager sadProcessManager = null;

        private System.Windows.Forms.Timer processUpdateTimer = null;
        private Thread processThread = null;
        private ProcessType processType = ProcessType.None;
        private bool processRunning = false;
        private Cursor processPreviousCursor = null;

        private bool dirtyProperties = false;
        private S6xNavInfo lastElemS6xNavInfo = null;
        private S6xNavInfo nextElemS6xNavInfo = null;
        private S6xNavCategories s6xNavCategories = null;

        private ArrayList alClipBoardTempUniqueAddresses = null;

        private RecentFiles recentFiles = null;

        private Repository repoRegisters = null;
        private Repository repoTables = null;
        private Repository repoFunctions = null;
        private Repository repoScalars = null;
        private Repository repoStructures = null;

        private Repository repoUnits = null;

        private RepositoryConversion repoConversion = null;

        private Repository repoOBDIErrors = null;
        private Repository repoOBDIIErrors = null;

        private SearchForm searchForm = null;
        private CompareForm compareForm = null;
        private CompareRoutinesForm compareRoutinesForm = null;

        private SQLite806xForm sqLite806xForm = null;
        
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
            recentToolStripMenuItem.DropDownOpening += new EventHandler(recentToolStripMenuItem_DropDownOpening);
            toolsToolStripMenuItem.DropDownOpening += new EventHandler(toolsToolStripMenuItem_DropDownOpening);
            helpToolStripMenuItem.DropDownOpening += new EventHandler(helpToolStripMenuItem_DropDownOpening);
            uniDb806xToolStripMenuItem.DropDownOpening += new EventHandler(uniDb806xToolStripMenuItem_DropDownOpening);
            iesOptionsUniDb806xToolStripMenuItem.DropDown.Closing += new ToolStripDropDownClosingEventHandler(iesOptionsUniDb806xToolStripMenuItemDropDown_Closing);

            elemDataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(elemDataGridView_DataBindingComplete);

            sharedIdentificationStatusTrackBar.ValueChanged += new EventHandler(sharedIdentificationStatusTrackBar_ValueChanged);

            sharedCategsDepthMaxToolStripMenuItem.Click += new EventHandler(sharedCategsDepthToolStripMenuItem_Click);
            sharedCategsDepthMedToolStripMenuItem.Click += new EventHandler(sharedCategsDepthToolStripMenuItem_Click);
            sharedCategsDepthMinToolStripMenuItem.Click += new EventHandler(sharedCategsDepthToolStripMenuItem_Click);
            sharedCategsDepthNoneToolStripMenuItem.Click += new EventHandler(sharedCategsDepthToolStripMenuItem_Click);

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

            sqLite806xFileContextMenuStrip.Opening += new CancelEventHandler(sqLite806xFileContextMenuStrip_Opening);

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

            // Double Buffering elemDataGridView for performance on painting
            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type tDGV = elemDataGridView.GetType();
                PropertyInfo piDGV = tDGV.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                if (piDGV != null) piDGV.SetValue(elemDataGridView, true, null);
                piDGV = null;
                tDGV = null;
            }

            elemOpsRichTextBox.Font = new Font(FontFamily.GenericMonospace, elemOpsRichTextBox.Font.Size);

            structureTipPictureBox.Tag = SharedUI.StructureTip();
            signatureTipPictureBox.Tag = SharedUI.SignatureTip();
            elementSignatureTipPictureBox.Tag = SharedUI.ElementSignatureTip();

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

            regScalePrecNumericUpDown.Minimum = SADDef.DefaultScaleMinPrecision;
            regScalePrecNumericUpDown.Maximum = SADDef.DefaultScaleMaxPrecision;
            regScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;

            tfSADVersionToolStripComboBox.Items.AddRange(TfSADTools.TFST_SAD_VERSIONS);
            if (tfSADVersionToolStripComboBox.Items.Count > 0) tfSADVersionToolStripComboBox.SelectedIndex = tfSADVersionToolStripComboBox.Items.Count - 1;

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
            bool autoFolderStatistics = false;
            bool autoFolderStatisticsAddresses = false;
            bool autoFolderStatisticsRegisters = false;
            bool autoFolderStatisticsRegistersAddresses = false;
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
                    case "-st":  // Folder Statistics
                        autoFolderStatistics = true;
                        break;
                    case "-sa":  // Folder Statistics Addresses
                        autoFolderStatisticsAddresses = true;
                        break;
                    case "-sr":  // Folder Statistics Addresses
                        autoFolderStatisticsRegisters = true;
                        break;
                    case "-sra":  // Folder Statistics Registers Addresses
                        autoFolderStatisticsRegistersAddresses = true;
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
                folderStartProcess(autoPath, autoFolderStatistics, autoFolderStatisticsAddresses, autoFolderStatisticsRegisters, autoFolderStatisticsRegistersAddresses);
            }
            else
            {
                binaryFilePath = autoPath;
                s6xFilePath = string.Empty;

                loadStartProcess();
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
            string tmpXlsFilePath = string.Empty;
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
                    case ".xls":
                    case ".xlsx":
                        tmpXlsFilePath = dFile;
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
            else if (tmpXdfFilePath != string.Empty) importXdfFile(tmpXdfFilePath, false);
            else if (tmpXlsFilePath != string.Empty) importXlsFile(tmpXlsFilePath);
            else if (tmpDirFilePath != string.Empty) importDirFile(tmpDirFilePath);
            else if (tmpCmtFilePath != string.Empty) importCmtFile(tmpCmtFilePath);
        }

        private bool confirmDirtyS6x()
        {
            if (sadS6x != null)
            {
                if (!sadS6x.isSaved)
                {
                    if (MessageBox.Show("S6x file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool confirmDirtyProperies()
        {
            if (lastElemS6xNavInfo != null && dirtyProperties)
            {
                if (lastElemS6xNavInfo.isValid)
                {
                    if (MessageBox.Show("Properties have not been validated, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        nextElemS6xNavInfo = lastElemS6xNavInfo;
                        elemsTreeView.Tag = new S6xNavInfo(nextElemS6xNavInfo.HeaderCategoryNode);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool confirmProcessRunning()
        {
            if (processRunning)
            {
                MessageBox.Show("Another process is already in progress. Please wait until its end.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private bool confirmInvalidAddresses()
        {
            if (alClipBoardTempUniqueAddresses != null)
            {
                if (alClipBoardTempUniqueAddresses.Count > 0)
                {
                    MessageBox.Show("S6x Definition contains invalid addresses.\r\nPlease update them before saving.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!confirmProcessRunning()) e.Cancel = true;
            else if (!confirmDirtyS6x()) e.Cancel = true;

            if (!e.Cancel) Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!confirmProcessRunning()) return;
            else if (!confirmDirtyS6x()) return;

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

            try { Application.Exit(); }
            catch { }
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

        private void recentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            initRecentFiles();
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            importSignaturesToolStripMenuItem.Enabled = false;
            tfSADVersionToolStripComboBox.Enabled = false;
            importDirFileToolStripMenuItem.Enabled = false;
            exportDirFileToolStripMenuItem.Enabled = false;
            importCmtFileToolStripMenuItem.Enabled = false;
            exportCmtFileToolStripMenuItem.Enabled = false;
            importXdfFileToolStripMenuItem.Enabled = false;
            importXdfCategsToolStripMenuItem.Enabled = false;
            exportXdfFileToolStripMenuItem.Enabled = false;
            exportXdfResetToolStripMenuItem.Enabled = false;
            importXlsFileToolStripMenuItem.Enabled = false;
            exportXlsFileToolStripMenuItem.Enabled = false;
            showHexToolStripMenuItem.Enabled = false;
            searchObjectsToolStripMenuItem.Enabled = false;
            searchSignatureToolStripMenuItem.Enabled = false;
            compareBinariesToolStripMenuItem.Enabled = false;
            compareBinariesDifDefToolStripMenuItem.Enabled = false;
            compareS6xToolStripMenuItem.Enabled = false;
            routinesComparisonToolStripMenuItem.Enabled = false;
            calibChartViewToolStripMenuItem.Enabled = false;
            massUpdateToolStripMenuItem.Enabled = false;

            // Universal 806x DataBase
            uniDb806xToolStripMenuItem.Enabled = activatedUniDb806x;

            // Settings Imp/Exp
            settingsSAD806xToolStripMenuItem.Enabled = false;   // Not interesting for now
            settingsSADToolStripMenuItem.Enabled = true;
            settingsTunerProToolStripMenuItem.Enabled = true;
            settingsEABEToolStripMenuItem.Enabled = false; // Not interesting for now
            settings806xUniDbToolStripMenuItem.Enabled = false; // Not interesting for now

            if (sadBin != null)
            {
                if (sadBin.isLoaded && sadBin.isValid)
                {
                    importSignaturesToolStripMenuItem.Enabled = true;
                    tfSADVersionToolStripComboBox.Enabled = true;
                    importDirFileToolStripMenuItem.Enabled = true;
                    exportDirFileToolStripMenuItem.Enabled = true;
                    importCmtFileToolStripMenuItem.Enabled = true;
                    exportCmtFileToolStripMenuItem.Enabled = true;
                    importXdfFileToolStripMenuItem.Enabled = true;
                    importXdfCategsToolStripMenuItem.Enabled = true;
                    exportXdfFileToolStripMenuItem.Enabled = true;
                    exportXdfResetToolStripMenuItem.Enabled = true;
                    importXlsFileToolStripMenuItem.Enabled = true;
                    exportXlsFileToolStripMenuItem.Enabled = true;
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

        private void uniDb806xToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            selectUniDb806xToolStripMenuItem.Enabled = true;

            bool validDB = fileUniDb806xToolStripTextBox.Tag != null;
            bool readyBin = false;
            if (sadBin != null) if (sadBin.isLoaded && sadBin.isValid && sadBin.isDisassembled) readyBin = true;

            openUniDb806xToolStripMenuItem.Enabled = validDB;

            importUniDb806xToolStripMenuItem.Enabled = validDB && sadBin != null;
            exportUniDb806xToolStripMenuItem.Enabled = validDB && readyBin;
            syncUniDb806xToolStripMenuItem.Enabled = validDB && readyBin;

            binariesUniDb806xToolStripMenuItem.Enabled = validDB;
            filesUniDb806xToolStripMenuItem.Enabled = validDB;
        }

        private void iesOptionsUniDb806xToolStripMenuItemDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void processUpdateTimer_Tick(object sender, EventArgs e)
        {
            processUpdateTimer.Enabled = false;

            if (!processRunning) return;

            switch (processType)
            {
                case ProcessType.Load:
                    if (sadS6x != null) loadEndProcess();
                    else processUpdateTimer.Enabled = true;
                    break;
                case ProcessType.Disassemble:
                    if (sadBin.ProgressStatus > 100) sadBin.ProgressStatus = 100;
                    if (sadBin.ProgressStatus < -1) sadBin.ProgressStatus = -1;
                    switch (sadBin.ProgressStatus)
                    {
                        case 99:
                            // For Post Process to loop another time before Post process
                            //      It permits to display properly status, before hard processes
                            sadBin.ProgressStatus = 100;
                            toolStripProgressBarMain.Value = sadBin.ProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadBin.ProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                        case 100:
                        case -1:
                            disassembleEndProcess();
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
                        case 99:
                            // For Post Process to loop another time before Post process
                            //      It permits to display properly status, before hard processes
                            sadBin.ProgressStatus = 100;
                            toolStripProgressBarMain.Value = sadBin.ProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadBin.ProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                        case 100:
                        case -1:
                            outputEndProcess();
                            break;
                        default:
                            toolStripProgressBarMain.Value = sadBin.ProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadBin.ProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                    }
                    break;
                case ProcessType.ProcessManager:
                    if (sadProcessManager == null) return;
                    if (sadProcessManager.ProcessProgressStatus > 100) sadProcessManager.ProcessProgressStatus = 100;
                    if (sadProcessManager.ProcessProgressStatus < -1) sadProcessManager.ProcessProgressStatus = -1;
                    switch (sadProcessManager.ProcessProgressStatus)
                    {
                        case 99:
                            // For Post Process to loop another time before Post process
                            //      It permits to display properly status, before hard processes
                            sadProcessManager.ProcessProgressStatus = 100;
                            toolStripProgressBarMain.Value = sadProcessManager.ProcessProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadProcessManager.ProcessProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                        case 100:
                        case -1:
                            sadManagerEndProcess();
                            break;
                        default:
                            toolStripProgressBarMain.Value = sadProcessManager.ProcessProgressStatus;
                            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
                            analysis4ToolStripStatusLabel.Text = getWaitStatus(sadProcessManager.ProcessProgressLabel);
                            processUpdateTimer.Enabled = true;
                            break;
                    }
                    break;
            }
        }

        // Process End Mngt for sadProcessManager
        private void sadManagerEndProcess()
        {
            processThread = null;
            processRunning = false;
            processType = ProcessType.None;

            if (sadProcessManager != null) analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;

            if (sadProcessManager != null)
            {
                if (sadProcessManager.PostProcessAction != string.Empty)
                {
                    sadProcessManager.PostProcessStartTime = DateTime.Now;
                    switch (sadProcessManager.PostProcessAction)
                    {
                        case "importCmtFilePostProcess":
                            importCmtFilePostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "importDirFilePostProcess":
                            importDirFilePostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "importXdfFilePostProcess":
                            importXdfFilePostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "importXlsFilePostProcess":
                            importXlsFilePostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "exportXdfFilePostProcess":
                            exportXdfFilePostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "exportXlsFilePostProcess":
                            exportXlsFilePostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "importUniDb806xPostProcess":
                            importUniDb806xPostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        case "syncUniDb806xPostProcess":
                            syncUniDb806xPostProcess();
                            analysis4ToolStripStatusLabel.Text = sadProcessManager.ProcessProgressLabel;
                            break;
                        default:
                            // Unknown Post Process ???
                            MessageBox.Show("Unknown post process \"" + sadProcessManager.PostProcessAction + "\".", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                    sadProcessManager.PostProcessEndTime = DateTime.Now;
                }
            }
            
            toolStripProgressBarMain.Value = 100;

            Cursor = processPreviousCursor;

            if (sadProcessManager == null) return;

            if (sadProcessManager.ProcessProgressStatus == -1)
            {
                sadProcessManager.ProcessProgressStatus = 100;
                if (!sadProcessManager.ShowProcessErrors) MessageBox.Show(sadProcessManager.ProcessProgressLabel, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (sadProcessManager.ProcessMessages != null)
                {
                    foreach (string sMessage in sadProcessManager.ProcessMessages) MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (sadProcessManager.ShowProcessErrors && sadProcessManager.ProcessErrors != null)
            {
                if (sadProcessManager.ProcessErrors.Count > 0)
                {
                    if (sadProcessManager != null)
                    {
                        if (sadProcessManager.ProcessErrors != null)
                        {
                            if (sadProcessManager.ProcessErrors.Count > 0)
                            {
                                string sErrors = string.Empty;
                                if (sadProcessManager.ProcessProgressLabel != string.Empty) sErrors += sadProcessManager.ProcessProgressLabel + "\r\n\r\n";
                                if (sadProcessManager.ProcessErrors.Count <= sadProcessManager.ShowProcessErrorsLimit) sErrors += string.Join("\r\n", (string[])sadProcessManager.ProcessErrors.ToArray(typeof(string)));
                                else sErrors += string.Join("\r\n", (string[])sadProcessManager.ProcessErrors.ToArray(typeof(string)), 0, sadProcessManager.ShowProcessErrorsLimit) + "\r\n...";
                                MessageBox.Show(sErrors, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
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
            controls = (Control.ControlCollection)sharedDetailsTabPage.Controls;
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
                        ((ComboBox)control).TextChanged += new EventHandler(elemProperties_Modified);
                        break;
                    case "NumericUpDown":
                        ((NumericUpDown)control).ValueChanged += new EventHandler(elemProperties_Modified);
                        break;
                    case "TrackBar":
                        ((TrackBar)control).ValueChanged += new EventHandler(elemProperties_Modified);
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
                // Register Address Calculation
                case Keys.G:
                    if (e.Control) calcRegisterAddress((Control)sender);
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
            controls = (Control.ControlCollection)elemButtonsPanel.Controls;
            resetPropertiesModifiedStatusControls(ref controls);
            controls = (Control.ControlCollection)tabPage.Controls;
            resetPropertiesModifiedStatusControls(ref controls);
            controls = (Control.ControlCollection)sharedDetailsTabPage.Controls;
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
                            case "elemBackButton":
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

            elemValidateButton.Enabled = true;
            elemCancelButton.Enabled = true;
        }
        
        private void initForm()
        {
            initFormMenu();
            initFormStatus();
            initFormElems();

            sadBin = null;
            sadS6x = null;

            alClipBoardTempUniqueAddresses = new ArrayList();

            s6xNavCategories = new S6xNavCategories();

            dirtyProperties = false;
            lastElemS6xNavInfo = null;
            nextElemS6xNavInfo = null;
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
            elemBackButton.Visible = false;

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

            showPropertiesTabPage(S6xNavHeaderCategory.UNDEFINED, false);
            if (elemTabControl.TabPages.Contains(elemOpsTabPage)) elemTabControl.TabPages.Remove(elemOpsTabPage);
            if (elemTabControl.TabPages.Contains(s6xPropertiesTabPage)) elemTabControl.TabPages.Remove(s6xPropertiesTabPage);
        }

        private void initRecentFiles()
        {
            bool cancelInit = true;

            if (recentToolStripMenuItem.Tag == null) cancelInit = false;
            else if (recentToolStripMenuItem.Tag.GetType() != typeof(DateTime)) cancelInit = false;

            string rfsFilePath = Application.StartupPath + "\\" + SADDef.recentFilesFileName;

            if (cancelInit)
            {
                try
                {
                    FileInfo fiFI = new FileInfo(rfsFilePath);
                    if (fiFI.Exists)
                    {
                        if ((DateTime)recentToolStripMenuItem.Tag != fiFI.LastWriteTime) cancelInit = false;
                        recentToolStripMenuItem.Tag = fiFI.LastWriteTime;
                    }
                    else
                    {
                        recentToolStripMenuItem.Tag = null;
                    }
                    fiFI = null;
                }
                catch
                {
                    recentToolStripMenuItem.Tag = null;
                }
            }

            if (cancelInit) return;

            recentToolStripMenuItem.DropDownItems.Clear();
            
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

            repoOBDIErrors = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameOBDIErrors, typeof(Repository));
            if (repoOBDIErrors == null) repoOBDIErrors = new Repository();

            repoOBDIIErrors = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameOBDIIErrors, typeof(Repository));
            if (repoOBDIIErrors == null) repoOBDIIErrors = new Repository();
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
            if (!confirmDirtyS6x()) return;
            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

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

            loadStartProcess();
        }

        private void selectS6xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectS6x(string.Empty);
        }

        private void selectS6x(string sFilePath)
        {
            if (!confirmDirtyS6x()) return;
            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;
            
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

            loadStartProcess();
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

            if (!confirmDirtyProperies()) return;
            if (!confirmInvalidAddresses()) return;

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
            disassembleStartProcess();
        }

        private void textOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputStartProcess();
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(documentationFilePath)) return;

            Process.Start(documentationFilePath);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SharedUI.About(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void loadStartProcess()
        {
            processPreviousCursor = Cursor;

            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            initForm();

            if (compareForm != null) compareForm.Close();
            if (compareRoutinesForm != null) compareRoutinesForm.Close();
            if (searchForm != null) searchForm.Close();
            
            toolStripProgressBarMain.Visible = true;

            selectBinaryToolStripMenuItem.Enabled = false;
            selectS6xToolStripMenuItem.Enabled = false;

            analysis4ToolStripStatusLabel.Font = analysis1ToolStripStatusLabel.Font;
            analysis4ToolStripStatusLabel.Text = getWaitStatus(string.Empty);

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.Load;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(processLoad);
            processThread.Start();
        }

        private void processLoad()
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
        
        private void loadEndProcess()
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
                disassembleStartProcess();
            }
        }

        private void disassembleStartProcess()
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
        
        private void disassembleEndProcess()
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
                outputStartProcess();
            }
        }

        private void outputStartProcess()
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

            processThread = new Thread(processOutputText);
            processThread.Start();
        }

        private void processOutputText()
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
        
        private void outputEndProcess()
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

        // Folder processing
        //      Heavy mode, no threads
        private void folderStartProcess(string folderPath, bool folderStatistics, bool folderStatisticsAddresses, bool folderStatisticsRegisters, bool folderStatisticsRegistersAddresses)
        {
            TextWriter txWriter = null;
            TextWriter txDtWriter = null;
            TextWriter txAdrWriter = null;
            TextWriter txRegWriter = null;
            TextWriter txRegAdrWriter = null;
            string logPath = string.Empty;
            string textDataPath = string.Empty;
            string statsAdrDataPath = string.Empty;
            string statsRegDataPath = string.Empty;
            string statsRegAdrDataPath = string.Empty;
            
            Cursor initialCursor = this.Cursor;

            try
            {
                logPath = folderPath + "\\" + Application.ProductName + DateTime.Now.ToString(".yyyyMMdd.HHmmss") + ".txt";
                textDataPath = folderPath + "\\" + Application.ProductName + DateTime.Now.ToString(".yyyyMMdd.HHmmss") + ".csv";
                statsAdrDataPath = folderPath + "\\" + Application.ProductName + DateTime.Now.ToString(".yyyyMMdd.HHmmss") + ".statsadr.csv";
                statsRegDataPath = folderPath + "\\" + Application.ProductName + DateTime.Now.ToString(".yyyyMMdd.HHmmss") + ".statsreg.csv";
                statsRegAdrDataPath = folderPath + "\\" + Application.ProductName + DateTime.Now.ToString(".yyyyMMdd.HHmmss") + ".statsregadr.csv";
                
                txWriter = new StreamWriter(logPath, false, Encoding.UTF8);
                txWriter.WriteLine(Application.ProductName + " - Folder Process (*.bin files) on folder : " + folderPath);
                txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Starting.");

                List<string> lstTextDataRow = new List<string>() {"Binary Name", "Binary Creation Date", "Binary Modification Date", "Binary Size", "Load/Disassembly/Output Status", "806x Detected Type", "Disassembly Errors", "Disassembly Operations Errors", "Disassembly Elements Errors", "Disassembly Other Errors", "Output Errors", "Strategy", "Strategy Version", "Hardware Part Number", "Copyright"};
                lstTextDataRow.Add("Scalars");
                lstTextDataRow.Add("Tables (Valid)");
                lstTextDataRow.Add("Tables (Invalid)");
                lstTextDataRow.Add("Functions (Valid)");
                lstTextDataRow.Add("Functions (Invalid)");
                lstTextDataRow.Add("Structures (Valid)");
                lstTextDataRow.Add("Structures (Invalid)");
                if (folderStatistics) foreach (StatisticsItems siSI in Enum.GetValues(typeof(StatisticsItems))) lstTextDataRow.Add(Tools.getStatisticsItemLabel(siSI) + " Count");
                string[] textDataRow = lstTextDataRow.ToArray();
                lstTextDataRow = null;

                txDtWriter = new StreamWriter(textDataPath, false, Encoding.UTF8);
                txDtWriter.WriteLine(string.Join(",", textDataRow));

                if (folderStatisticsAddresses)
                {
                    txAdrWriter = new StreamWriter(statsAdrDataPath, false, Encoding.UTF8);
                    txAdrWriter.WriteLine(string.Join(",", new string[] { "Binary Name", "Statistics Type", "Address", "\"Details\"" }));
                    txAdrWriter.Flush();
                }

                if (folderStatisticsRegisters)
                {
                    txRegWriter = new StreamWriter(statsRegDataPath, false, Encoding.UTF8);
                    txRegWriter.WriteLine(string.Join(",", new string[] { "Binary Name", "Register", "Name", "Statistics Type", "Value" }));
                    txRegWriter.Flush();
                }

                if (folderStatisticsRegistersAddresses)
                {
                    txRegAdrWriter = new StreamWriter(statsRegAdrDataPath, false, Encoding.UTF8);
                    txRegAdrWriter.WriteLine(string.Join(",", new string[] { "Binary Name", "Register", "Name", "Statistics Type", "Address", "\"Details\"" }));
                    txRegAdrWriter.Flush();
                }

                string[] binFiles = Directory.GetFiles(folderPath, "*.bin", SearchOption.TopDirectoryOnly);

                for (int iBinFile = 0; iBinFile < binFiles.Length; iBinFile++)
                {
                    // Binary Name, Binary Creation Date, Binary Modification Date, Load/Disassemby/Output Status, Disassembly Errors, Output Errors, Strategy, Strategy Version, Hardware Part Number
                    textDataRow = new string[textDataRow.Length];
                    for (int iTextData = 0; iTextData < textDataRow.Length; iTextData++) textDataRow[iTextData] = string.Empty;

                    try
                    {
                        binaryFilePath = binFiles[iBinFile];
                        if (binaryFilePath == string.Empty) continue;

                        FileInfo fiFI = new FileInfo(binaryFilePath);
                        if (fiFI == null) continue;

                        textDataRow[0] = fiFI.Name;
                        textDataRow[1] = fiFI.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss");
                        textDataRow[2] = fiFI.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss");
                        textDataRow[3] = fiFI.Length.ToString();

                        fiFI = null;

                        textDataRow[4] = "Initialized";

                        txWriter.WriteLine("Processing Binary file : " + binaryFilePath);
                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Starting.");

                        // Load
                        textDataRow[4] = "Loading";

                        s6xFilePath = string.Empty;

                        processLoad();

                        GC.Collect();

                        if (sadBin == null)
                        {
                            txWriter.WriteLine("\tLoading Binary file has failed.");
                            textDataRow[4] = "Load failed";
                            txDtWriter.WriteLine(string.Join(",", textDataRow));
                            txDtWriter.Flush();
                            continue;
                        }

                        if (!sadBin.isValid)
                        {
                            txWriter.WriteLine("\tBinary file is invalid.");
                            textDataRow[4] = "Invalid Binary";
                            txDtWriter.WriteLine(string.Join(",", textDataRow));
                            txDtWriter.Flush();
                            continue;
                        }

                        textDataRow[4] = "Loaded";
                        if (sadBin.is8061) textDataRow[5] = "8061";
                        else textDataRow[5] = "8065";
                        if (sadBin.isPilot) textDataRow[5] += " Pilot";
                        if (sadBin.isEarly) textDataRow[5] += " Early";
                        if (sadBin.is8065SingleBank) textDataRow[5] += " Single Bank";
                        if (sadBin.isCorrupted) textDataRow[5] += " Corrupted";

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

                        fiFI = new FileInfo(binaryFilePath);
                        textOutputSetFilePath(fiFI.Directory.FullName + "\\" + fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".txt");
                        fiFI = null;

                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Loaded.");
                        if (sadBin.Calibration.Info.VidStrategy != string.Empty)
                        {
                            txWriter.WriteLine("\t\tStrategy " + sadBin.Calibration.Info.VidStrategy + "(" + sadBin.Calibration.Info.VidStrategyVersion + ")");
                            textDataRow[11] = sadBin.Calibration.Info.VidStrategy;
                            textDataRow[12] = sadBin.Calibration.Info.VidStrategyVersion;
                            textDataRow[11] = textDataRow[11].Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", " ").Trim();
                            textDataRow[12] = textDataRow[12].Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", " ").Trim();
                        }
                        if (sadBin.Calibration.Info.VidSerial != string.Empty && sadBin.Calibration.Info.VidSerial.Length > 5)
                        {
                            txWriter.WriteLine("\t\tPart Number " + sadBin.Calibration.Info.VidSerial.Substring(0, 4) + "-12A650-" + sadBin.Calibration.Info.VidSerial.Substring(4));
                            textDataRow[13] = sadBin.Calibration.Info.VidSerial.Substring(0, 4) + "-12A650-" + sadBin.Calibration.Info.VidSerial.Substring(4);
                            textDataRow[13] = textDataRow[13].Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", " ").Trim();
                        }
                        if (sadBin.Calibration.Info.VidCopyright != string.Empty)
                        {
                            txWriter.WriteLine("\t\tCopyright " + sadBin.Calibration.Info.VidCopyright);
                            textDataRow[14] = sadBin.Calibration.Info.VidCopyright;
                            textDataRow[14] = textDataRow[14].Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", " ").Trim();
                        }

                        //Disassembly
                        textDataRow[4] = "Disassembling";
                        
                        sadBin.processBin();

                        GC.Collect();

                        if (!sadBin.isDisassembled)
                        {
                            txWriter.WriteLine("\tDisassembly has failed.");
                            textDataRow[4] = "Disassembly Failed";
                            txDtWriter.WriteLine(string.Join(",", textDataRow));
                            txDtWriter.Flush();
                            continue;
                        }

                        textDataRow[4] = "Disassembled";
                        textDataRow[6] = "0";
                        textDataRow[7] = "0";
                        textDataRow[8] = "0";
                        textDataRow[9] = "0";

                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Disassembled.");
                        if (sadBin.Errors != null)
                        {
                            if (sadBin.Errors.Length > 0)
                            {
                                txWriter.WriteLine("\t\tDisassembly Errors(" + sadBin.Errors.Length + ") :");
                                txWriter.WriteLine("\t\t\t" + string.Join("\r\n\t\t\t", sadBin.Errors));
                                textDataRow[6] = sadBin.Errors.Length.ToString();
                                int opsErrors = 0;
                                int elemsErrors = 0;
                                foreach (string sError in sadBin.Errors)
                                {
                                    if (sError == null) continue;
                                    if (sError.StartsWith("Operations Conflict : ")) opsErrors++;
                                    else if (sError.StartsWith("Calibration Elements Conflict : ")) elemsErrors++;
                                }
                                textDataRow[7] = opsErrors.ToString();
                                textDataRow[8] = elemsErrors.ToString();
                                textDataRow[9] = (sadBin.Errors.Length - opsErrors - elemsErrors).ToString();
                            }
                        }

                        //Output
                        textDataRow[4] = "Outputting";

                        processOutputText();

                        GC.Collect();

                        textDataRow[4] = "Outputted";
                        textDataRow[10] = "0";

                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Output done.");
                        if (sadBin.Errors != null)
                        {
                            if (sadBin.Errors.Length > 0)
                            {
                                txWriter.WriteLine("\t\tOutput Errors(" + sadBin.Errors.Length + ") :");
                                txWriter.WriteLine("\t\t\t" + string.Join("\r\n\t\t\t", sadBin.Errors));
                                textDataRow[10] = sadBin.Errors.Length.ToString();
                            }
                        }

                        //Statistics
                        if (folderStatistics || folderStatisticsAddresses || folderStatisticsRegisters || folderStatisticsRegistersAddresses) sadBin.processStatistics();

                        int iScalars = 0;
                        int iTablesValid = 0;
                        int iTablesInvalid = 0;
                        int iFunctionsValid = 0;
                        int iFunctionsInvalid = 0;
                        int iStructuresValid = 0;
                        int iStructuresInvalid = 0;

                        iScalars = sadBin.S6x.slScalars.Count;

                        foreach (S6xTable s6xObject in sadBin.S6x.slTables.Values)
                        {
                            if (s6xObject == null) iTablesInvalid++;
                            else if (s6xObject.ColsNumber > 0 && s6xObject.RowsNumber > 0) iTablesValid++;
                            else iTablesInvalid++;
                        }
                        foreach (S6xFunction s6xObject in sadBin.S6x.slFunctions.Values)
                        {
                            if (s6xObject == null) iFunctionsInvalid++;
                            else if (s6xObject.RowsNumber > 0) iFunctionsValid++;
                            else iFunctionsInvalid++;
                        }
                        foreach (S6xStructure s6xObject in sadBin.S6x.slStructures.Values)
                        {
                            if (s6xObject == null) iStructuresInvalid++;
                            else if (s6xObject.Number > 0) iStructuresValid++;
                            else iStructuresInvalid++;
                        }

                        textDataRow[15] = iScalars.ToString();
                        textDataRow[16] = iTablesValid.ToString();
                        textDataRow[17] = iTablesInvalid.ToString();
                        textDataRow[18] = iFunctionsValid.ToString();
                        textDataRow[19] = iFunctionsInvalid.ToString();
                        textDataRow[20] = iStructuresValid.ToString();
                        textDataRow[21] = iStructuresInvalid.ToString();

                        if (folderStatistics)
                        {
                            int statisticsIndex = textDataRow.Length - Enum.GetValues(typeof(StatisticsItems)).Length;
                            foreach (StatisticsItems siSI in Enum.GetValues(typeof(StatisticsItems)))
                            {
                                textDataRow[statisticsIndex] = "0";
                                if (sadBin.Calibration.Statistics != null)
                                {
                                    if (sadBin.Calibration.Statistics.ContainsKey(siSI)) textDataRow[statisticsIndex] = sadBin.Calibration.Statistics[siSI].ToString();
                                }
                                statisticsIndex++;
                            }
                        }

                        txWriter.Flush();

                        txDtWriter.WriteLine(string.Join(",", textDataRow));
                        txDtWriter.Flush();

                        if (folderStatisticsAddresses)
                        {
                            if (sadBin.Calibration.StatisticsAddresses != null)
                            {
                                foreach (StatisticsItems sItem in sadBin.Calibration.StatisticsAddresses.Keys)
                                {
                                    SortedList slStatisticsAddresses = (SortedList)sadBin.Calibration.StatisticsAddresses[sItem];
                                    if (slStatisticsAddresses == null) continue;
                                    if (slStatisticsAddresses.Count > Tools.getStatisticsItemMaxCountForAddresses(sItem))
                                    {
                                        txAdrWriter.WriteLine(string.Join(",", new string[] { textDataRow[0], Tools.getStatisticsItemLabel(sItem), "X XXXX", "Addresses number is bigger than defined limit (" + Tools.getStatisticsItemMaxCountForAddresses(sItem).ToString() + "). No output will be done for current binary." }));
                                        txAdrWriter.Flush();
                                        continue;
                                    }
                                    foreach (object[] arrSA in slStatisticsAddresses.Values)
                                    {
                                        if (arrSA == null) continue;
                                        if (arrSA.Length != 3) continue;
                                        int iBankNum = (int)arrSA[0];
                                        int iAddress = (int)arrSA[1];
                                        string sDetails = string.Empty;
                                        object oObject = arrSA[2];
                                        if (oObject == null) continue;
                                        if (oObject.GetType() == typeof(Operation))
                                        {
                                            Operation ope = (Operation)oObject;
                                            switch (ope.BankNum)
                                            {
                                                case 8:
                                                    if (sadBin.Bank8 != null) ope = (Operation)sadBin.Bank8.slOPs[ope.UniqueAddress];
                                                    break;
                                                case 1:
                                                    if (sadBin.Bank1 != null) ope = (Operation)sadBin.Bank1.slOPs[ope.UniqueAddress];
                                                    break;
                                                case 9:
                                                    if (sadBin.Bank9 != null) ope = (Operation)sadBin.Bank9.slOPs[ope.UniqueAddress];
                                                    break;
                                                case 0:
                                                    if (sadBin.Bank0 != null) ope = (Operation)sadBin.Bank0.slOPs[ope.UniqueAddress];
                                                    break;
                                            }
                                            if (ope == null) continue;
                                            sDetails = string.Format("{0,6}: {1,-21}{2,-25}{3,-21}", ope.UniqueAddressHex, ope.OriginalOp, ope.Instruction, ope.Translation1);
                                        }
                                        if (sDetails == string.Empty) continue;
                                        txAdrWriter.WriteLine(string.Join(",", new string[] { textDataRow[0], Tools.getStatisticsItemLabel(sItem), Tools.UniqueAddressHex(iBankNum, iAddress), "\"" + sDetails.Replace("\"", "''") + "\"" }));
                                    }
                                    txAdrWriter.Flush();
                                }
                            }
                        }

                        if (folderStatisticsRegisters)
                        {
                            if (sadBin.Calibration.StatisticsRegisters != null)
                            {
                                foreach (StatisticsRegisterItems srItem in sadBin.Calibration.StatisticsRegisters.Keys)
                                {
                                    SortedList slStatisticsRegs = (SortedList)sadBin.Calibration.StatisticsRegisters[srItem];
                                    if (slStatisticsRegs == null) continue;
                                    foreach (object[] arrSR in slStatisticsRegs.Values)
                                    {
                                        if (arrSR == null) continue;
                                        if (arrSR.Length != 2) continue;
                                        Register rReg = (Register)arrSR[0];
                                        SortedList slStatisticsAdr = (SortedList)arrSR[1];
                                        string regInst = Tools.RegisterInstruction(rReg.Address);
                                        string regName = string.Empty;
                                        if (rReg.S6xRegister != null)
                                        {
                                            if (rReg.S6xRegister.Label != regInst) regName = rReg.S6xRegister.Label;
                                        }
                                        txRegWriter.WriteLine(string.Join(",", new string[] { textDataRow[0], regInst, regName, Tools.getStatisticsRegisterItemLabel(srItem), slStatisticsAdr.Count.ToString() }));
                                    }
                                    txRegWriter.Flush();
                                }
                            }
                        }

                        if (folderStatisticsRegistersAddresses)
                        {
                            if (sadBin.Calibration.StatisticsRegisters != null)
                            {
                                foreach (StatisticsRegisterItems srItem in sadBin.Calibration.StatisticsRegisters.Keys)
                                {
                                    // Count has not to be managed
                                    if (srItem == StatisticsRegisterItems.Count) continue;

                                    SortedList slStatisticsRegs = (SortedList)sadBin.Calibration.StatisticsRegisters[srItem];
                                    if (slStatisticsRegs == null) continue;
                                    foreach (object[] arrSR in slStatisticsRegs.Values)
                                    {
                                        if (arrSR == null) continue;
                                        if (arrSR.Length != 2) continue;
                                        Register rReg = (Register)arrSR[0];
                                        SortedList slStatisticsAdr = (SortedList)arrSR[1];
                                        string regInst = Tools.RegisterInstruction(rReg.Address);
                                        string regName = string.Empty;
                                        if (rReg.S6xRegister != null)
                                        {
                                            if (rReg.S6xRegister.Label != regInst) regName = rReg.S6xRegister.Label;
                                        }
                                        if (slStatisticsAdr.Count > Tools.getStatisticsItemMaxCountForRegistersAddresses(srItem))
                                        {
                                            txRegAdrWriter.WriteLine(string.Join(",", new string[] { textDataRow[0], regInst, regName, Tools.getStatisticsRegisterItemLabel(srItem), "X XXXX", "Addresses number is bigger than defined limit (" + Tools.getStatisticsItemMaxCountForRegistersAddresses(srItem).ToString() + "). No output will be done for current register." }));
                                            txRegAdrWriter.Flush();
                                            continue;
                                        }
                                        foreach (object[] arrSA in slStatisticsAdr.Values)
                                        {
                                            if (arrSA == null) continue;
                                            if (arrSA.Length != 3) continue;
                                            int iBankNum = (int)arrSA[0];
                                            int iAddress = (int)arrSA[1];
                                            string sDetails = string.Empty;
                                            object oObject = arrSA[2];
                                            if (oObject == null) continue;
                                            if (oObject.GetType() == typeof(Operation))
                                            {
                                                Operation ope = (Operation)oObject;
                                                switch (ope.BankNum)
                                                {
                                                    case 8:
                                                        if (sadBin.Bank8 != null) ope = (Operation)sadBin.Bank8.slOPs[ope.UniqueAddress];
                                                        break;
                                                    case 1:
                                                        if (sadBin.Bank1 != null) ope = (Operation)sadBin.Bank1.slOPs[ope.UniqueAddress];
                                                        break;
                                                    case 9:
                                                        if (sadBin.Bank9 != null) ope = (Operation)sadBin.Bank9.slOPs[ope.UniqueAddress];
                                                        break;
                                                    case 0:
                                                        if (sadBin.Bank0 != null) ope = (Operation)sadBin.Bank0.slOPs[ope.UniqueAddress];
                                                        break;
                                                }
                                                if (ope == null) continue;
                                                sDetails = string.Format("{0,6}: {1,-21}{2,-25}{3,-21}", ope.UniqueAddressHex, ope.OriginalOp, ope.Instruction, ope.Translation1);
                                            }
                                            if (sDetails == string.Empty) continue;
                                            txRegAdrWriter.WriteLine(string.Join(",", new string[] { textDataRow[0], regInst, regName, Tools.getStatisticsRegisterItemLabel(srItem), Tools.UniqueAddressHex(iBankNum, iAddress), "\"" + sDetails.Replace("\"", "''") + "\"" }));
                                        }
                                    }
                                    txRegAdrWriter.Flush();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        txWriter.WriteLine("\t" + DateTime.Now.ToString("HH:mm:ss") + " - Process has failed.");
                        txWriter.WriteLine("\t\t" + ex.Message);
                        txWriter.Flush();

                        textDataRow[4] += " and Failed";
                        txDtWriter.WriteLine(string.Join(",", textDataRow));
                        txDtWriter.Flush();
                    }
                }

                if (folderStatisticsAddresses)
                {
                    txAdrWriter.Close();
                    txAdrWriter = null;
                }

                if (folderStatisticsRegisters)
                {
                    txRegWriter.Close();
                    txRegWriter = null;
                }

                if (folderStatisticsRegistersAddresses)
                {
                    txRegAdrWriter.Close();
                    txRegAdrWriter = null;
                }

                txDtWriter.Close();
                txDtWriter = null;

                txWriter.Flush();

                txWriter.Close();
                txWriter = null;
                
                MessageBox.Show("Folder Process has ended.\r\n\r\nLogs are available in folder with start time.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    if (folderStatisticsAddresses)
                    {
                        txAdrWriter.Close();
                        txAdrWriter = null;
                    }

                    if (folderStatisticsRegisters)
                    {
                        txRegWriter.Close();
                        txRegWriter = null;
                    }

                    if (folderStatisticsRegistersAddresses)
                    {
                        txRegAdrWriter.Close();
                        txRegAdrWriter = null;
                    }

                    txDtWriter.Close();
                    txDtWriter = null;

                    txWriter.Flush();

                    txWriter.Close();
                    txWriter = null;
                }
                catch { }

                MessageBox.Show("Folder Process has failed.\r\n\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (folderStatisticsAddresses)
                {
                    try
                    {
                        txAdrWriter.Close();
                        txAdrWriter = null;
                    }
                    catch { }
                }

                if (folderStatisticsRegisters)
                {
                    try
                    {
                        txRegWriter.Close();
                        txRegWriter = null;
                    }
                    catch { }
                }

                if (folderStatisticsRegistersAddresses)
                {
                    try
                    {
                        txRegAdrWriter.Close();
                        txRegAdrWriter = null;
                    }
                    catch { }
                }

                try
                {
                    txDtWriter.Close();
                    txDtWriter = null;
                }
                catch { }

                try
                {
                    txWriter.Close();
                    txWriter = null;
                }
                catch { }

                sadBin = null;
                sadS6x = null;
                binaryFilePath = string.Empty;
                s6xFilePath = string.Empty;

                Cursor = initialCursor;
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
                if (sadBin.Calibration.Info.is8065SingleBank) status += "SB";
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
            if (sadBin.Calibration.Info.is8065SingleBank) status += " Single Bank (Dev Only)";
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
                    return;
                }
            }
            else if (sadBin.isLoaded)
            {
                if (!sadBin.Calibration.Info.isCheckSumValid && sadBin.Calibration.Info.isCheckSumConfirmed)
                {
                    // Only when calculation has been confirmed, otherwise 99% chance to get a bad proposal
                    MessageBox.Show("Correct CheckSum should be " + string.Format("{0:x4}", sadBin.Calibration.Info.correctedChecksum) + ", " + Tools.LsbFirst(string.Format("{0:x4}", sadBin.Calibration.Info.correctedChecksum)) + " in binary.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (sadProcessManager != null)
            {
                if (sadProcessManager.ProcessErrors != null)
                {
                    if (sadProcessManager.ProcessErrors.Count > 0)
                    {
                        string sErrors = string.Empty;
                        if (sadProcessManager.ProcessProgressLabel != string.Empty) sErrors += sadProcessManager.ProcessProgressLabel + "\r\n\r\n";
                        if (sadProcessManager.ProcessErrors.Count <= sadProcessManager.ShowProcessErrorsLimit) sErrors += string.Join("\r\n", (string[])sadProcessManager.ProcessErrors.ToArray(typeof(string)));
                        else sErrors += string.Join("\r\n", (string[])sadProcessManager.ProcessErrors.ToArray(typeof(string)), 0, sadProcessManager.ShowProcessErrorsLimit) + "\r\n...";
                        MessageBox.Show(sErrors, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private S6xNavCategoryDepth getS6xNavCategoryDepth()
        {
            S6xNavCategoryDepth categoryDepth = S6xNavCategoryDepth.DISABLED;
            if (sharedCategsDepthMaxToolStripMenuItem.Checked) categoryDepth = S6xNavCategoryDepth.MAXIMUM;
            else if (sharedCategsDepthMedToolStripMenuItem.Checked) categoryDepth = S6xNavCategoryDepth.MEDIUM;
            else if (sharedCategsDepthMinToolStripMenuItem.Checked) categoryDepth = S6xNavCategoryDepth.MINIMUM;

            return categoryDepth;
        }
        
        private void setElementsTreeCategLabel(S6xNavHeaderCategory headerCateg)
        {
            if (!elemsTreeView.Nodes.ContainsKey(S6xNav.getHeaderCategName(headerCateg))) return;

            string categLabel = S6xNav.getHeaderCategLabel(headerCateg);
            if (categLabel == string.Empty) return;

            int iCount = (new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)])).ElementsCount;
            elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Text = categLabel + " (" + iCount.ToString() + ")";
        }
        
        // Init Elements Tree to CleanUp and Create Parent Nodes
        private void ShowElementsTreeInit()
        {
            List<S6xNavHeaderCategory> lHeaderCategs = new List<S6xNavHeaderCategory>();
            lHeaderCategs.Add(S6xNavHeaderCategory.PROPERTIES);
            lHeaderCategs.Add(S6xNavHeaderCategory.RESERVED);
            lHeaderCategs.Add(S6xNavHeaderCategory.TABLES);
            lHeaderCategs.Add(S6xNavHeaderCategory.FUNCTIONS);
            lHeaderCategs.Add(S6xNavHeaderCategory.SCALARS);
            lHeaderCategs.Add(S6xNavHeaderCategory.STRUCTURES);
            lHeaderCategs.Add(S6xNavHeaderCategory.ROUTINES);
            lHeaderCategs.Add(S6xNavHeaderCategory.OPERATIONS);
            lHeaderCategs.Add(S6xNavHeaderCategory.REGISTERS);
            lHeaderCategs.Add(S6xNavHeaderCategory.OTHER);
            lHeaderCategs.Add(S6xNavHeaderCategory.SIGNATURES);
            lHeaderCategs.Add(S6xNavHeaderCategory.ELEMSSIGNATURES);

            elemsTreeView.BeginUpdate();
            elemsTreeView.Nodes.Clear();
            foreach (S6xNavHeaderCategory headerCateg in lHeaderCategs)
            {
                TreeNode tnNode = new TreeNode();
                tnNode.Name = S6xNav.getHeaderCategName(headerCateg);
                tnNode.Text = S6xNav.getHeaderCategLabel(headerCateg);
                tnNode.ToolTipText = S6xNav.getHeaderCategToolTip(headerCateg);
                tnNode.StateImageKey = S6xNav.getS6xNavHeaderCategoryStateImageKey(headerCateg);
                // Context Menu
                switch (headerCateg)
                {
                    case S6xNavHeaderCategory.PROPERTIES:
                    case S6xNavHeaderCategory.RESERVED:
                        break;
                    default:
                        tnNode.ContextMenuStrip = categsContextMenuStrip;
                        break;
                }
                // For Scalers Refresh
                switch (headerCateg)
                {
                    case S6xNavHeaderCategory.FUNCTIONS:
                        tnNode.Tag = true;
                        break;
                }
                elemsTreeView.Nodes.Add(tnNode);
            }
            elemsTreeView.EndUpdate();
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
            if (sadBin == null) return;

            elemsTreeView.BeginUpdate();

            // Reserved Addresses filled in at load only
            S6xNavInfo navInfoReserved = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.RESERVED)]);
            navInfoReserved.DirectNodes.Clear();

            ArrayList alBanks = new ArrayList();
            if (sadBin.Bank8 != null) alBanks.Add(sadBin.Bank8);
            if (sadBin.Bank1 != null) alBanks.Add(sadBin.Bank1);
            if (sadBin.Bank9 != null) alBanks.Add(sadBin.Bank9);
            if (sadBin.Bank0 != null) alBanks.Add(sadBin.Bank0);

            // Categories Mngt
            SortedList slCategoriesTree = new SortedList();
            foreach (SADBank Bank in alBanks) ShowElementsTreeCategsPreparation(ref slCategoriesTree, "Bank " + Bank.Num.ToString(), string.Empty, string.Empty);
            ShowElementsTreeCategsCreation(navInfoReserved, ref slCategoriesTree);
            slCategoriesTree = null;

            foreach (SADBank Bank in alBanks)
            {
                foreach (ReservedAddress resAdr in Bank.slReserved.Values)
                {
                    bool bIgnore = false;
                    bool bNoCategory = false;
                    TreeNode tnNode = new TreeNode();
                    tnNode.Name = resAdr.UniqueAddress;
                    tnNode.Text = resAdr.Label;
                    switch (resAdr.Type)
                    {
                        case ReservedAddressType.IntVector:
                            foreach (Vector vect in Bank.slIntVectors.Values)
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
                                    bNoCategory = true;
                                    // Added to S6xRegisters in the same time, except for Rsi.
                                    if (rBase.Code.ToLower() != "si")
                                    {
                                        S6xRegister s6xReg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress(rBase.Code)];
                                        if (s6xReg == null)
                                        {
                                            try
                                            {
                                                s6xReg = new S6xRegister(rBase.Code);
                                                s6xReg.isRBase = true;
                                                s6xReg.isRConst = false;
                                                s6xReg.ConstValue = Convert.ToString(SADDef.EecBankStartAddress + rBase.AddressBankInt, 16);
                                                s6xReg.AutoConstValue = true;
                                                s6xReg.Label = Tools.RegisterInstruction(rBase.Code);
                                                s6xReg.Comments = "RBase " + s6xReg.Label;
                                                sadS6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                                            }
                                            catch { } // Unmanaged Error
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        case ReservedAddressType.RomSize:
                            if (resAdr.ValueInt == 0xffff) tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\nNot Defined (ffff)\r\n" + resAdr.Comments;
                            else tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            bNoCategory = true;
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
                            bNoCategory = true;
                            break;
                        default:
                            tnNode.ToolTipText = resAdr.UniqueAddressHex + "\r\n" + resAdr.Value(16) + "\r\n" + resAdr.Comments;
                            bNoCategory = true;
                            break;
                    }

                    // VID Block Specificity
                    if (Bank.Num == sadBin.Calibration.Info.VidBankNum)
                    {
                        object[] vidDef = null;

                        if (sadBin.Calibration.Info.is8061) vidDef = SADDef.Info_8061_VID_Block_Addresses;
                        else vidDef = SADDef.Info_8065_VID_Block_Addresses;

                        foreach (object[] vidPart in vidDef)
                        {
                            if ((int)vidPart[0] == resAdr.AddressInt)
                            {
                                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                                bNoCategory = true;
                            }
                        }
                    }

                    if (!bIgnore)
                    {
                        tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(100);
                        //navInfoReserved.DirectNodes.Add(tnNode);
                        ShowElementsTreeNodeAdd(navInfoReserved, tnNode, bNoCategory ? string.Empty : "Bank " + Bank.Num.ToString(), string.Empty, string.Empty);
                    }
                }
            }
            alBanks = null;

            setElementsTreeCategLabel(navInfoReserved.HeaderCategory);

            elemsTreeView.EndUpdate();
        }

        private void ShowElementsTreeLoadS6x()
        {
            S6xNavInfo niHeaderCateg = null;
            SortedList slCategoriesTree = null;
            TreeNode tnNode = null;

            // Automatic S6xObjects Creation without Store Property and without link to parent object
            // 20200909 - Now managed on disassembly
            //showElementsTreeRemapS6x();

            elemsTreeView.BeginUpdate();

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xTable s6xObject in sadS6x.slTables.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xTable s6xTable in sadS6x.slTables.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xTable.UniqueAddress;
                tnNode.Text = s6xTable.Label;
                tnNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xTable.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xTable.Category, s6xTable.Category2, s6xTable.Category3);
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);
            // Duplicates
            foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
            {
                if (!niHeaderCateg.Node.Nodes.ContainsKey(s6xTable.UniqueAddress)) continue;
                if (niHeaderCateg.Node.Nodes[s6xTable.UniqueAddress].Nodes.ContainsKey(s6xTable.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xTable.DuplicateAddress;
                tnNode.Text = s6xTable.Label;
                tnNode.ToolTipText = s6xTable.UniqueAddressHex + "\r\n" + s6xTable.ShortLabel + "\r\n\r\n" + s6xTable.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xTable.IdentificationStatus);
                //niHeaderCateg.DirectNodes[s6xTable.UniqueAddress].Nodes.Add(tnNode);
                TreeNode tnParent = niHeaderCateg.FindElement(s6xTable.UniqueAddress);
                if (tnParent != null) tnParent.Nodes.Add(tnNode);
                tnParent = null;
                tnNode = null;
            }

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xFunction s6xFunction in sadS6x.slFunctions.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xFunction.UniqueAddress;
                tnNode.Text = s6xFunction.Label;
                tnNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xFunction.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xFunction.Category, s6xFunction.Category2, s6xFunction.Category3);
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);
            // Duplicates
            foreach (S6xFunction s6xFunction in sadS6x.slDupFunctions.Values)
            {
                if (!niHeaderCateg.Node.Nodes.ContainsKey(s6xFunction.UniqueAddress)) continue;
                if (niHeaderCateg.Node.Nodes[s6xFunction.UniqueAddress].Nodes.ContainsKey(s6xFunction.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xFunction.DuplicateAddress;
                tnNode.Text = s6xFunction.Label;
                tnNode.ToolTipText = s6xFunction.UniqueAddressHex + "\r\n" + s6xFunction.ShortLabel + "\r\n\r\n" + s6xFunction.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xFunction.IdentificationStatus);
                //niHeaderCateg.DirectNodes[s6xFunction.UniqueAddress].Nodes.Add(tnNode);
                TreeNode tnParent = niHeaderCateg.FindElement(s6xFunction.UniqueAddress);
                if (tnParent != null) tnParent.Nodes.Add(tnNode);
                tnParent = null;
                tnNode = null;
            }
            elemsTreeView.Nodes[niHeaderCateg.HeaderCategoryName].Tag = true;              // For Scalers Refresh

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xScalar s6xScalar in sadS6x.slScalars.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xScalar.UniqueAddress;
                tnNode.Text = s6xScalar.Label;
                tnNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xScalar.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xScalar.Category, s6xScalar.Category2, s6xScalar.Category3);
                tnNode = null;
            }
            // Duplicates
            foreach (S6xScalar s6xScalar in sadS6x.slDupScalars.Values)
            {
                if (!niHeaderCateg.Node.Nodes.ContainsKey(s6xScalar.UniqueAddress)) continue;
                if (niHeaderCateg.Node.Nodes[s6xScalar.UniqueAddress].Nodes.ContainsKey(s6xScalar.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xScalar.DuplicateAddress;
                tnNode.Text = s6xScalar.Label;
                tnNode.ToolTipText = s6xScalar.UniqueAddressHex + "\r\n" + s6xScalar.ShortLabel + "\r\n\r\n" + s6xScalar.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xScalar.IdentificationStatus);
                //niHeaderCateg.DirectNodes[s6xScalar.UniqueAddress].Nodes.Add(tnNode);
                TreeNode tnParent = niHeaderCateg.FindElement(s6xScalar.UniqueAddress);
                if (tnParent != null) tnParent.Nodes.Add(tnNode);
                tnParent = null;
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xStructure s6xObject in sadS6x.slStructures.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xStructure s6xStructure in sadS6x.slStructures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xStructure.UniqueAddress;
                tnNode.Text = s6xStructure.Label;
                tnNode.ToolTipText = s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xStructure.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xStructure.Category, s6xStructure.Category2, s6xStructure.Category3);
                tnNode = null;
            }
            // Duplicates
            foreach (S6xStructure s6xStructure in sadS6x.slDupStructures.Values)
            {
                if (!niHeaderCateg.Node.Nodes.ContainsKey(s6xStructure.UniqueAddress)) continue;
                if (niHeaderCateg.Node.Nodes[s6xStructure.UniqueAddress].Nodes.ContainsKey(s6xStructure.DuplicateAddress)) continue;
                tnNode = new TreeNode();
                tnNode.Name = s6xStructure.DuplicateAddress;
                tnNode.Text = s6xStructure.Label;
                tnNode.ToolTipText = s6xStructure.UniqueAddressHex + "\r\n" + s6xStructure.ShortLabel + "\r\n\r\n" + s6xStructure.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xStructure.IdentificationStatus);
                //niHeaderCateg.DirectNodes[s6xStructure.UniqueAddress].Nodes.Add(tnNode);
                TreeNode tnParent = niHeaderCateg.FindElement(s6xStructure.UniqueAddress);
                if (tnParent != null) tnParent.Nodes.Add(tnNode);
                tnParent = null;
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xRoutine s6xRoutine in sadS6x.slRoutines.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xRoutine.UniqueAddress;
                tnNode.Text = s6xRoutine.Label;
                tnNode.ToolTipText = s6xRoutine.UniqueAddressHex + "\r\n" + s6xRoutine.ShortLabel + "\r\n\r\n" + s6xRoutine.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xRoutine.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xRoutine.Category, s6xRoutine.Category2, s6xRoutine.Category3);
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xOperation s6xObject in sadS6x.slOperations.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xOperation s6xOpe in sadS6x.slOperations.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xOpe.UniqueAddress;
                tnNode.Text = s6xOpe.Label;
                tnNode.ToolTipText = s6xOpe.UniqueAddressHex + "\r\n\r\n" + s6xOpe.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xOpe.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xOpe.Category, s6xOpe.Category2, s6xOpe.Category3);
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xRegister s6xReg in sadS6x.slRegisters.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xReg.UniqueAddress;
                tnNode.Text = s6xReg.FullLabel;
                tnNode.ToolTipText = s6xReg.FullComments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xReg.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xReg.Category, s6xReg.Category2, s6xReg.Category3);
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xObject.Category, s6xObject.Category2, s6xObject.Category3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xOtherAddress s6xOth in sadS6x.slOtherAddresses.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xOth.UniqueAddress;
                tnNode.Text = s6xOth.Label;
                tnNode.ToolTipText = s6xOth.UniqueAddressHex + "\r\n\r\n" + s6xOth.Comments;
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xOth.IdentificationStatus);
                //niHeaderCateg.DirectNodes.Add(tnNode);
                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xOth.Category, s6xOth.Category2, s6xOth.Category3);
                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SIGNATURES)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories Mngt
            slCategoriesTree = new SortedList();
            foreach (S6xSignature s6xSig in sadS6x.slSignatures.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xSig.SignatureCategory, s6xSig.SignatureCategory2, s6xSig.SignatureCategory3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xSignature s6xSig in sadS6x.slSignatures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xSig.UniqueKey;
                tnNode.Text = (s6xSig.SignatureLabel == null || s6xSig.SignatureLabel == string.Empty) ? SADDef.LongSignaturePrefix + s6xSig.Label : s6xSig.SignatureLabel;
                tnNode.ToolTipText = s6xSig.SignatureComments;
                if (s6xSig.Found)
                {
                    if (s6xSig.Ignore) tnNode.ForeColor = Color.Red;
                    else tnNode.ForeColor = Color.DarkGreen;
                }
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xSig.IdentificationStatus);

                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xSig.SignatureCategory, s6xSig.SignatureCategory2, s6xSig.SignatureCategory3);

                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.ELEMSSIGNATURES)]);
            niHeaderCateg.DirectNodes.Clear();
            // Categories mngt
            slCategoriesTree = new SortedList();
            foreach (S6xElementSignature s6xESig in sadS6x.slElementsSignatures.Values) ShowElementsTreeCategsPreparation(ref slCategoriesTree, s6xESig.SignatureCategory, s6xESig.SignatureCategory2, s6xESig.SignatureCategory3);
            ShowElementsTreeCategsCreation(niHeaderCateg, ref slCategoriesTree);
            slCategoriesTree = null;
            foreach (S6xElementSignature s6xESig in sadS6x.slElementsSignatures.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xESig.UniqueKey;
                tnNode.Text = s6xESig.SignatureLabel;
                tnNode.ToolTipText = s6xESig.SignatureComments;
                if (s6xESig.Found)
                {
                    tnNode.ForeColor = Color.DarkGreen;
                }
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xESig.IdentificationStatus);

                ShowElementsTreeNodeAdd(niHeaderCateg, tnNode, s6xESig.SignatureCategory, s6xESig.SignatureCategory2, s6xESig.SignatureCategory3);

                tnNode = null;
            }
            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);

            elemsTreeView.EndUpdate();

            dirtyProperties = false;
            lastElemS6xNavInfo = null;
            nextElemS6xNavInfo = null;
            elemPanel.Visible = false;
            elemDataGridView.Visible = false;

            elemBackButton.Tag = null;
            elemBackButton.Visible = false;
            
            mainPanel.Visible = true;
        }

        private void ShowElementsTreeCategsPreparation(ref SortedList slCategoriesTree, string sCateg1, string sCateg2, string sCateg3)
        {
            if (slCategoriesTree == null) slCategoriesTree = new SortedList();

            int depthLevel = 0;
            if (sharedCategsDepthMaxToolStripMenuItem.Checked) depthLevel = 3;
            else if (sharedCategsDepthMedToolStripMenuItem.Checked) depthLevel = 2;
            else if (sharedCategsDepthMinToolStripMenuItem.Checked) depthLevel = 1;

            if (depthLevel < 1) return;

            if (sCateg1 == null || sCateg1 == string.Empty)
            {
                return;
            }

            S6xNavCategory navCateg1 = new S6xNavCategory(sCateg1);
            object[] arrCategory1Tree = (object[])slCategoriesTree[navCateg1.Key];
            if (arrCategory1Tree == null)
            {
                arrCategory1Tree = new object[] {navCateg1, new SortedList()};
                slCategoriesTree.Add(navCateg1.Key, arrCategory1Tree);
            }

            SortedList slCategory1Tree = (SortedList)arrCategory1Tree[1];
            navCateg1 = null;
            arrCategory1Tree = null;

            if (depthLevel < 2) return;
            
            if (sCateg2 == null || sCateg2 == string.Empty)
            {
                slCategory1Tree = null;
                return;
            }

            S6xNavCategory navCateg2 = new S6xNavCategory(sCateg2);
            object[] arrCategory2Tree = (object[])slCategory1Tree[navCateg2.Key];
            if (arrCategory2Tree == null)
            {
                arrCategory2Tree = new object[] { navCateg2, new SortedList() };
                slCategory1Tree.Add(navCateg2.Key, arrCategory2Tree);
            }

            SortedList slCategory2Tree = (SortedList)arrCategory2Tree[1];
            navCateg2 = null;
            arrCategory2Tree = null;

            if (depthLevel < 3) return;

            if (sCateg3 == null || sCateg3 == string.Empty)
            {
                slCategory2Tree = null;
                return;
            }

            S6xNavCategory navCateg3 = new S6xNavCategory(sCateg3);
            object[] arrCategory3Tree = (object[])slCategory2Tree[navCateg3.Key];
            if (arrCategory3Tree == null)
            {
                arrCategory3Tree = new object[] { navCateg3, new SortedList() };
                slCategory2Tree.Add(navCateg3.Key, arrCategory3Tree);
            }

            navCateg3 = null;
            arrCategory3Tree = null;
        }

        private void ShowElementsTreeCategsCreation(S6xNavInfo niHeaderCateg, ref SortedList slCategoriesTree)
        {
            niHeaderCateg.DirectNodes.Clear();

            // Categories Mngt
            S6xNav.s6xNavCategoriesReset(niHeaderCateg.HeaderCategory, ref s6xNavCategories, ref sadBin, ref sadS6x);

            foreach (object[] arrCategory1Tree in slCategoriesTree.Values)
            {
                S6xNavCategory navCateg1 = (S6xNavCategory)arrCategory1Tree[0];
                SortedList slCategory1Tree = (SortedList)arrCategory1Tree[1];

                TreeNode tnNode = new TreeNode();
                tnNode.Name = navCateg1.Key;
                tnNode.Text = navCateg1.Name;
                tnNode.Tag = navCateg1;
                tnNode.StateImageKey = niHeaderCateg.Node.StateImageKey;
                niHeaderCateg.DirectNodes.Add(tnNode);
                S6xNavInfo niCateg1 = new S6xNavInfo(tnNode);
                tnNode = null;

                foreach (object[] arrCategory2Tree in slCategory1Tree.Values)
                {
                    S6xNavCategory navCateg2 = (S6xNavCategory)arrCategory2Tree[0];
                    SortedList slCategory2Tree = (SortedList)arrCategory2Tree[1];

                    tnNode = new TreeNode();
                    tnNode.Name = navCateg2.Key;
                    tnNode.Text = navCateg2.Name;
                    tnNode.Tag = navCateg2;
                    tnNode.StateImageKey = niCateg1.Node.StateImageKey;
                    niCateg1.DirectNodes.Add(tnNode);
                    S6xNavInfo niCateg2 = new S6xNavInfo(tnNode);
                    tnNode = null;

                    foreach (object[] arrCategory3Tree in slCategory2Tree.Values)
                    {
                        S6xNavCategory navCateg3 = (S6xNavCategory)arrCategory3Tree[0];

                        tnNode = new TreeNode();
                        tnNode.Name = navCateg3.Key;
                        tnNode.Text = navCateg3.Name;
                        tnNode.Tag = navCateg3;
                        tnNode.StateImageKey = niCateg2.Node.StateImageKey;
                        niCateg2.DirectNodes.Add(tnNode);
                        tnNode = null;
                    }
                }
            }
        }

        private void ShowElementsTreeNodeAdd(S6xNavInfo niHeaderCateg, TreeNode tnElement, string sCateg1, string sCateg2, string sCateg3)
        {
            TreeNode tnCateg = null;

            S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();

            if (sCateg1 == null || sCateg1 == string.Empty || categoryDepth == S6xNavCategoryDepth.DISABLED)
            {
                if (!niHeaderCateg.DirectNodes.ContainsKey(tnElement.Name)) niHeaderCateg.DirectNodes.Add(tnElement);
                return;
            }

            S6xNavCategory navCateg = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, sCateg1);
            if (navCateg == null)
            {
                if (!niHeaderCateg.DirectNodes.ContainsKey(tnElement.Name)) niHeaderCateg.DirectNodes.Add(tnElement);
                return;
            }
            if (!niHeaderCateg.DirectNodes.ContainsKey(navCateg.Key))
            {
                tnCateg = new TreeNode();
                tnCateg.Name = navCateg.Key;
                tnCateg.Text = navCateg.Name;
                tnCateg.Tag = navCateg;
                niHeaderCateg.DirectNodes.Add(tnCateg);
                tnCateg = null;
            }
            S6xNavInfo niCateg = new S6xNavInfo(niHeaderCateg.DirectNodes[navCateg.Key]);
            if (!niCateg.isValid)
            {
                if (!niHeaderCateg.DirectNodes.ContainsKey(tnElement.Name)) niHeaderCateg.DirectNodes.Add(tnElement);
                return;
            }

            if (sCateg2 == null || sCateg2 == string.Empty || categoryDepth == S6xNavCategoryDepth.DISABLED || categoryDepth == S6xNavCategoryDepth.MINIMUM)
            {
                if (!niCateg.DirectNodes.ContainsKey(tnElement.Name)) niCateg.DirectNodes.Add(tnElement);
                return;
            }

            S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, sCateg2);
            if (navCateg2 == null)
            {
                if (!niCateg.DirectNodes.ContainsKey(tnElement.Name)) niCateg.DirectNodes.Add(tnElement);
                return;
            }
            if (!niCateg.DirectNodes.ContainsKey(navCateg2.Key))
            {
                tnCateg = new TreeNode();
                tnCateg.Name = navCateg2.Key;
                tnCateg.Text = navCateg2.Name;
                tnCateg.Tag = navCateg2;
                niCateg.DirectNodes.Add(tnCateg);
                tnCateg = null;
            }
            S6xNavInfo niCateg2 = new S6xNavInfo(niCateg.DirectNodes[navCateg2.Key]);
            if (!niCateg2.isValid)
            {
                if (!niCateg.DirectNodes.ContainsKey(tnElement.Name)) niCateg.DirectNodes.Add(tnElement);
                return;
            }

            if (sCateg3 == null || sCateg3 == string.Empty || categoryDepth == S6xNavCategoryDepth.DISABLED || categoryDepth == S6xNavCategoryDepth.MINIMUM || categoryDepth == S6xNavCategoryDepth.MEDIUM)
            {
                if (!niCateg2.DirectNodes.ContainsKey(tnElement.Name)) niCateg2.DirectNodes.Add(tnElement);
                return;
            }

            S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, sCateg3);
            if (navCateg3 == null)
            {
                if (!niCateg2.DirectNodes.ContainsKey(tnElement.Name)) niCateg2.DirectNodes.Add(tnElement);
                return;
            }
            if (!niCateg2.DirectNodes.ContainsKey(navCateg3.Key))
            {
                tnCateg = new TreeNode();
                tnCateg.Name = navCateg3.Key;
                tnCateg.Text = navCateg3.Name;
                tnCateg.Tag = navCateg3;
                niCateg2.DirectNodes.Add(tnCateg);
                tnCateg = null;
            }
            S6xNavInfo niCateg3 = new S6xNavInfo(niCateg2.DirectNodes[navCateg3.Key]);
            if (!niCateg3.isValid)
            {
                return;
            }

            if (!niCateg3.DirectNodes.ContainsKey(tnElement.Name)) niCateg3.DirectNodes.Add(tnElement);
        }

        private void ShowElementsTreeRefresh()
        {
            elemsTreeView.BeginUpdate();
            
            foreach (TreeNode tnHeaderCateg in elemsTreeView.Nodes)
            {
                switch (S6xNav.getHeaderCateg(tnHeaderCateg.Name))
                {
                    // No Refresh Required
                    case S6xNavHeaderCategory.PROPERTIES:
                    case S6xNavHeaderCategory.RESERVED:
                        tnHeaderCateg.ForeColor = elemsTreeView.ForeColor;
                        break;
                    default:
                        // Refresh Colors only with Parent one
                        ShowElementsTreeRefreshCateg(new S6xNavInfo(tnHeaderCateg));
                        break;
                }
            }

            elemsTreeView.EndUpdate();

            dirtyProperties = false;
            lastElemS6xNavInfo = null;
            nextElemS6xNavInfo = null;
            elemsTreeView.SelectedNode = null;
            elemPanel.Visible = false;
            elemDataGridView.Visible = false;

            mainPanel.Visible = true;
        }

        private void ShowElementsTreeRefreshCateg(S6xNavInfo niCateg)
        {
            // Refresh Colors only with Parent one
            foreach (TreeNode tnNode in niCateg.DirectNodes)
            {
                S6xNavInfo navInfo = new S6xNavInfo(tnNode);
                if (navInfo.MainNode == null)
                {
                    ShowElementsTreeRefreshCateg(navInfo);
                }
                else
                {
                    if (navInfo.MainNode.ForeColor != navInfo.HeaderCategoryNode.ForeColor) navInfo.MainNode.ForeColor = navInfo.HeaderCategoryNode.ForeColor;
                    // Duplicates
                    foreach (TreeNode tnDup in navInfo.DuplicateNodes)
                    {
                        if (tnDup.ForeColor != navInfo.HeaderCategoryNode.ForeColor) tnDup.ForeColor = navInfo.HeaderCategoryNode.ForeColor;
                    }
                }
            }
        }

        private void categsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            pasteMultToolStripMenuItem.Visible = false;
            
            pasteToolStripMenuItem.Enabled = (Clipboard.ContainsData(SADDef.S6xClipboardFormat) || Clipboard.ContainsData(SADDef.XdfClipboardFormat));
            pasteMultToolStripMenuItem.Enabled = pasteToolStripMenuItem.Enabled;

            categCleanElements.Enabled = false;
            categCommentsOutputAllToolStripMenuItem.Enabled = false;
            categCommentsOutputNoneToolStripMenuItem.Enabled = false;
            categCommentsInlineAllToolStripMenuItem.Enabled = false;
            categCommentsInlineNoneToolStripMenuItem.Enabled = false;

            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (!navInfo.isHeaderCategory) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    break;
                case S6xNavHeaderCategory.TABLES:
                    categCleanElements.Enabled = true;
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    categCleanElements.Enabled = true;
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    categCleanElements.Enabled = true;
                    pasteMultToolStripMenuItem.Visible = true;
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    categCommentsInlineAllToolStripMenuItem.Enabled = true;
                    categCommentsInlineNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    categCommentsInlineAllToolStripMenuItem.Enabled = true;
                    categCommentsInlineNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    break;
                case S6xNavHeaderCategory.OTHER:
                    categCommentsOutputAllToolStripMenuItem.Enabled = true;
                    categCommentsOutputNoneToolStripMenuItem.Enabled = true;
                    categCommentsInlineAllToolStripMenuItem.Enabled = true;
                    categCommentsInlineNoneToolStripMenuItem.Enabled = true;
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
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

            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            bDuplicate = navInfo.isDuplicate;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
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
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.FUNCTIONS:
                case S6xNavHeaderCategory.SCALARS:
                    pasteMultOnItemToolStripMenuItem.Enabled = pasteOnItemToolStripMenuItem.Enabled;
                    copyXdfToolStripMenuItem.Visible = true;
                    showOperationsToolStripMenuItem.Enabled = (sadBin != null && sadBin.isDisassembled);
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                case S6xNavHeaderCategory.ROUTINES:
                case S6xNavHeaderCategory.OPERATIONS:
                case S6xNavHeaderCategory.OTHER:
                    showOperationsToolStripMenuItem.Enabled = (sadBin != null && sadBin.isDisassembled);
                    break;
            }

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    break;                
                case S6xNavHeaderCategory.TABLES:
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
                case S6xNavHeaderCategory.FUNCTIONS:
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
                case S6xNavHeaderCategory.SCALARS:
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
                case S6xNavHeaderCategory.STRUCTURES:
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
                case S6xNavHeaderCategory.ROUTINES:
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

            if (nextElemS6xNavInfo != null) forFunction = (nextElemS6xNavInfo.HeaderCategory ==  S6xNavHeaderCategory.FUNCTIONS);
            
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
            S6xNavInfo navInfo = new S6xNavInfo(e.Node);
            if (!navInfo.isValid) return;

            if (navInfo.MainNode == null && navInfo.HeaderCategory == S6xNavHeaderCategory.PROPERTIES)
            {
                lastElemS6xNavInfo = null;
                nextElemS6xNavInfo = null;
                showProperties(false);
            }

            if (navInfo.MainNode == null) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    lastElemS6xNavInfo = null;
                    nextElemS6xNavInfo = null;
                    return;
                default:
                    break;
            }

            elemsTreeView.Tag = new S6xNavInfo(navInfo.HeaderCategoryNode);
            nextElemS6xNavInfo = navInfo;
            showElem(false);
        }

        private void elemsTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(e.Node);
            if (!navInfo.isValid) e.CancelEdit = true;
            if (navInfo.MainNode == null) e.CancelEdit = true;

            if (e.CancelEdit) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                case S6xNavHeaderCategory.REGISTERS:
                case S6xNavHeaderCategory.OTHER:
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
            S6xNavInfo navInfo = new S6xNavInfo(e.Node);
            if (!navInfo.isValid) e.CancelEdit = true;
            if (navInfo.MainNode == null) e.CancelEdit = true;
            if (e.Label == null) e.CancelEdit = true;
            if (e.Label == string.Empty) e.CancelEdit = true;
            if (!e.CancelEdit)
            {
                switch (navInfo.HeaderCategory)
                {
                    case S6xNavHeaderCategory.RESERVED:
                    case S6xNavHeaderCategory.REGISTERS:
                    case S6xNavHeaderCategory.OTHER:
                        e.CancelEdit = true;
                        break;
                    default:
                        // To prevent removing element when editing it - Reactivation
                        shortCutsElementResetRemoveToolStripMenuItem.Enabled = true;
                        break;
                }
            }
            if (e.CancelEdit) return;

            renameElem(navInfo, e.Label);
        }

        private void newOnItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo s6xNavInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!s6xNavInfo.isValid) return;
            if (s6xNavInfo.MainNode == null) return;

            newElement(new S6xNavInfo(s6xNavInfo.HeaderCategoryNode));
        }

        private void newElemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo s6xNavInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!s6xNavInfo.isValid) return;
            if (!s6xNavInfo.isHeaderCategory) return;

            newElement(s6xNavInfo);
        }

        private void newElement(S6xNavInfo headerCategInfo)
        {
            if (headerCategInfo == null) return;
            if (!headerCategInfo.isValid) return;
            if (!headerCategInfo.isHeaderCategory) return;

            if (!confirmDirtyProperies()) return;

            nextElemS6xNavInfo = null;
            elemsTreeView.Tag = headerCategInfo;

            hideElemData();

            elemBankTextBox.ReadOnly = false;
            elemAddressTextBox.ReadOnly = false;

            elemLabelTextBox.Text = "New Element";
            switch (headerCategInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.FUNCTIONS:
                case S6xNavHeaderCategory.SCALARS:
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
                case S6xNavHeaderCategory.STRUCTURES:
                case S6xNavHeaderCategory.ROUTINES:
                case S6xNavHeaderCategory.OPERATIONS:
                case S6xNavHeaderCategory.OTHER:
                    elemBankTextBox.Text = "8";
                    elemAddressTextBox.Text = string.Format("{0:x4}", SADDef.EecBankStartAddress);
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    elemBankTextBox.Text = "R";
                    elemAddressTextBox.Text = string.Empty;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = string.Empty;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = string.Empty;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    break;
            }

            switch (headerCategInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    newElemTableProperties();
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    newElemFunctionProperties();
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    newElemScalarProperties();
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    newElemStructureProperties();
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    newElemRoutineProperties();
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    newElemOperationProperties();
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    newElemRegisterProperties();
                    break;
                case S6xNavHeaderCategory.OTHER:
                    newElemOtherProperties();
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    newElemSignatureProperties();
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    newElemElemSignatureProperties();
                    break;
            }

            newSharedDetails(headerCategInfo.HeaderCategory);
            
            showPropertiesTabPage(headerCategInfo.HeaderCategory, false);
            elemPanel.Visible = true;
        }

        private void newSharedDetails(S6xNavHeaderCategory headerCateg)
        {
            sharedDateCreatedDateTimePicker.Value = DateTime.Now;
            sharedDateUpdatedDateTimePicker.Value = DateTime.Now;

            if (headerCateg == S6xNavHeaderCategory.PROPERTIES)
            {
                sharedCategsLabel.Visible = false;
                sharedCategComboBox.Visible = false;
                sharedCateg2ComboBox.Visible = false;
                sharedCateg3ComboBox.Visible = false;
            }
            else
            {
                sharedCategsLabel.Visible = true;
                sharedCategComboBox.Visible = true;
                sharedCateg2ComboBox.Visible = true;
                sharedCateg3ComboBox.Visible = true;

                S6xNav.s6xNavCategoriesLoad(headerCateg, sharedCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
                S6xNav.s6xNavCategoriesLoad(headerCateg, sharedCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
                S6xNav.s6xNavCategoriesLoad(headerCateg, sharedCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

                // 20210511 - PYM - It was not working properly
                /*
                sharedCategComboBox.SelectedText = string.Empty;
                sharedCateg2ComboBox.SelectedText = string.Empty;
                sharedCateg3ComboBox.SelectedText = string.Empty;
                */
                sharedCategComboBox.Text = string.Empty;
                sharedCateg2ComboBox.Text = string.Empty;
                sharedCateg3ComboBox.Text = string.Empty;
            }
            
            sharedIdentificationStatusTrackBar.Value = 0;

            // Windows 10 1809 (10.0.17763) Issue
            sharedIdentificationDetailsTextBox.Clear();
            sharedIdentificationDetailsTextBox.Multiline = false;
            sharedIdentificationDetailsTextBox.Multiline = true;

            sharedIdentificationDetailsTextBox.Text = string.Empty;
        }

        private void skipOnItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skipItem(true);
        }

        private void skipItem(bool skip)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            if (!confirmDirtyProperies()) return;

            string uniqueAddress = navInfo.Node.Name;

            if (navInfo.isDuplicate)
            {
                switch (navInfo.HeaderCategory)
                {
                    case S6xNavHeaderCategory.TABLES:
                        if (!sadS6x.slDupTables.ContainsKey(uniqueAddress)) return;
                        ((S6xTable)sadS6x.slDupTables[uniqueAddress]).Skip = skip;
                        ((S6xTable)sadS6x.slDupTables[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.FUNCTIONS:
                        if (!sadS6x.slDupFunctions.ContainsKey(uniqueAddress)) return;
                        ((S6xFunction)sadS6x.slDupFunctions[uniqueAddress]).Skip = skip;
                        ((S6xFunction)sadS6x.slDupFunctions[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.SCALARS:
                        if (!sadS6x.slDupScalars.ContainsKey(uniqueAddress)) return;
                        ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).Skip = skip;
                        ((S6xScalar)sadS6x.slDupScalars[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.STRUCTURES:
                        if (!sadS6x.slDupStructures.ContainsKey(uniqueAddress)) return;
                        ((S6xStructure)sadS6x.slDupStructures[uniqueAddress]).Skip = skip;
                        ((S6xStructure)sadS6x.slDupStructures[uniqueAddress]).Store = true;
                        break;
                }
            }
            else
            {
                switch (navInfo.HeaderCategory)
                {
                    case S6xNavHeaderCategory.TABLES:
                        if (!sadS6x.slTables.ContainsKey(uniqueAddress)) return;
                        ((S6xTable)sadS6x.slTables[uniqueAddress]).Skip = skip;
                        ((S6xTable)sadS6x.slTables[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.FUNCTIONS:
                        if (!sadS6x.slFunctions.ContainsKey(uniqueAddress)) return;
                        ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Skip = skip;
                        ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.SCALARS:
                        if (!sadS6x.slScalars.ContainsKey(uniqueAddress)) return;
                        ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Skip = skip;
                        ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.STRUCTURES:
                        if (!sadS6x.slStructures.ContainsKey(uniqueAddress)) return;
                        ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Skip = skip;
                        ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.ROUTINES:
                        if (!sadS6x.slRoutines.ContainsKey(uniqueAddress)) return;
                        ((S6xRoutine)sadS6x.slRoutines[uniqueAddress]).Skip = skip;
                        ((S6xRoutine)sadS6x.slRoutines[uniqueAddress]).Store = true;
                        break;
                    case S6xNavHeaderCategory.OPERATIONS:
                        if (!sadS6x.slOperations.ContainsKey(uniqueAddress)) return;
                        ((S6xOperation)sadS6x.slOperations[uniqueAddress]).Skip = skip;
                        break;
                    case S6xNavHeaderCategory.REGISTERS:
                        if (!sadS6x.slRegisters.ContainsKey(uniqueAddress)) return;
                        ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Skip = skip;
                        break;
                    case S6xNavHeaderCategory.OTHER:
                        if (!sadS6x.slOtherAddresses.ContainsKey(uniqueAddress)) return;
                        ((S6xOtherAddress)sadS6x.slOtherAddresses[uniqueAddress]).Skip = skip;
                        break;
                    case S6xNavHeaderCategory.SIGNATURES:
                        if (!sadS6x.slSignatures.ContainsKey(uniqueAddress)) return;
                        ((S6xSignature)sadS6x.slSignatures[uniqueAddress]).Skip = skip;
                        break;
                    case S6xNavHeaderCategory.ELEMSSIGNATURES:
                        if (!sadS6x.slElementsSignatures.ContainsKey(uniqueAddress)) return;
                        ((S6xElementSignature)sadS6x.slElementsSignatures[uniqueAddress]).Skip = skip;
                        break;
                    default:
                        return;
                }
            }

            sadS6x.isSaved = false;

            navInfo.Node.ForeColor = Color.Purple;

            showElem(false);
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
            // 20200308 - PYM - Only managed for stored items now
            //                  To prevent exporting (TunerPro & co) autodetected elements, export before disassembly.
            //                  No managemenet for field DateUpdated - It is not a real definition change
            
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (!navInfo.isHeaderCategory) return;

            if (!confirmDirtyProperies()) return;

            string uniqueAddress = string.Empty;

            elemsTreeView.BeginUpdate();

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xStructure s6xObject in sadS6x.slDupStructures.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
                    {
                        if (s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
                    {
                        if (s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OTHER:
                    foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
                    {
                        if (s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
                    {
                        if (s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueKey;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
                    {
                        if (s6xObject.Skip == skip) continue;

                        uniqueAddress = s6xObject.UniqueKey;
                        s6xObject.Skip = skip;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
            }

            elemsTreeView.EndUpdate();

            showElem(false);
        }

        private void categCommentsOutputAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setOutputCommentsAll(true);
        }

        private void categCommentsOutputNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setOutputCommentsAll(false);
        }

        private void setOutputCommentsAll(bool setOutput)
        {
            // 20200308 - PYM - Only managed for stored items
            //                  No managemenet for field DateUpdated - It is not a real definition change

            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (!navInfo.isHeaderCategory) return;

            if (!confirmDirtyProperies()) return;

            string uniqueAddress = string.Empty;

            elemsTreeView.BeginUpdate();

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xTable s6xObject in sadS6x.slDupTables.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xFunction s6xObject in sadS6x.slDupFunctions.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xStructure s6xObject in sadS6x.slDupStructures.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
                    {
                        if (!s6xObject.Store || s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
                    {
                        if (s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OTHER:
                    foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
                    {
                        if (s6xObject.OutputComments == setOutput) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.OutputComments = setOutput;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
            }

            elemsTreeView.EndUpdate();

            showElem(false);
        }

        private void categCommentsInlineAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setInlineCommentsAll(true);
        }

        private void categCommentsInlineNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setInlineCommentsAll(false);
        }

        private void setInlineCommentsAll(bool setInline)
        {
            // 20200308 - PYM - Only managed for stored items
            //                  No managemenet for field DateUpdated - It is not a real definition change

            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (!navInfo.isHeaderCategory) return;

            if (!confirmDirtyProperies()) return;

            string uniqueAddress = string.Empty;

            elemsTreeView.BeginUpdate();

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.SCALARS:
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.InlineComments == setInline) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.InlineComments = setInline;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    // Duplicates
                    foreach (S6xScalar s6xObject in sadS6x.slDupScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.InlineComments == setInline) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.InlineComments = setInline;
                        TreeNode tnNode = navInfo.FindElementDuplicate(uniqueAddress, s6xObject.DuplicateAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
                    {
                        if (s6xObject.InlineComments == setInline) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.InlineComments = setInline;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OTHER:
                    foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
                    {
                        if (s6xObject.InlineComments == setInline) continue;

                        uniqueAddress = s6xObject.UniqueAddress;
                        s6xObject.InlineComments = setInline;
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode != null) tnNode.ForeColor = Color.Purple;
                        sadS6x.isSaved = false;
                    }
                    break;
            }

            elemsTreeView.EndUpdate();

            showElem(false);
        }

        private void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    nextElemS6xNavInfo = null;
                    return;
            }

            nextElemS6xNavInfo = navInfo;
            showElem(false);
        }
        
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                case S6xNavHeaderCategory.REGISTERS:
                case S6xNavHeaderCategory.OTHER:
                    elemsTreeView.SelectedNode = null;
                    return;
            }

            navInfo.Node.BeginEdit();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    elemsTreeView.SelectedNode = null;
                    return;
            }

            deleteElem(navInfo, false, false, true);
        }

        private void showOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation[] ops = null;
            bool forElem = false;
            bool calibElemOpe = false;
            int selectTextStart = -1;
            int selectTextEnd = -1;

            nextElemS6xNavInfo = new S6xNavInfo(elemsTreeView.SelectedNode);

            if (!nextElemS6xNavInfo.isValid) return;

            if (nextElemS6xNavInfo.MainNode == null) return;
            
            if (sadBin == null) return;

            if (!sadBin.isDisassembled) return;

            switch (nextElemS6xNavInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.ROUTINES:
                case S6xNavHeaderCategory.OPERATIONS:
                case S6xNavHeaderCategory.OTHER:
                    forElem = false;
                    break;
                default:
                    forElem = true;
                    break;
            }

            ops = sadBin.getElementRelatedOps(nextElemS6xNavInfo.MainNode.Name, forElem);

            // Second Run for OTHER, could be Element too
            if (nextElemS6xNavInfo.HeaderCategory ==  S6xNavHeaderCategory.OTHER && ops.Length == 0)
            {
                forElem = true;
                ops = sadBin.getElementRelatedOps(nextElemS6xNavInfo.MainNode.Name, forElem);
            }

            elemOpsRichTextBox.Clear();
            if (!forElem)
            {
                selectTextStart = 0;
                elemOpsRichTextBox.AppendText(nextElemS6xNavInfo.Node.Name + ":\r\n");
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
                                if (opeCalElem.UniqueAddress == nextElemS6xNavInfo.MainNode.Name)
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
                            if ((Convert.ToInt32(ope.KnownElemAddress, 16) - SADDef.EecBankStartAddress).ToString() == nextElemS6xNavInfo.MainNode.Name.Substring(nextElemS6xNavInfo.MainNode.Name.LastIndexOf(" ") + 1))
                            {
                                calibElemOpe = true;
                                elemOpsRichTextBox.AppendText("\r\n");
                                selectTextStart = elemOpsRichTextBox.Text.Length - 1;
                            }
                        }
                        else if (ope.OtherElemAddress != string.Empty)
                        {
                            // No Check on Bank, Could be Apply On Bank or Not for Calibration Bank, standard Address is checked
                            if ((Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress).ToString() == nextElemS6xNavInfo.MainNode.Name.Substring(nextElemS6xNavInfo.MainNode.Name.LastIndexOf(" ") + 1))
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

        private void showSharedDetails(S6xNavHeaderCategory headerCateg, object s6xObject)
        {
            if (s6xObject == null) return;

            string nameOfDateCreated = "DateCreated";
            string nameOfDateUpdated = "DateUpdated";
            string nameOfCategory = "Category";
            string nameOfCategory2 = "Category2";
            string nameOfCategory3 = "Category3";
            string nameOfIdentificationStatus = "IdentificationStatus";
            string nameOfIdentificationDetails = "IdentificationDetails";

            Type s6xType = s6xObject.GetType();

            if (s6xType == typeof(S6xSignature) || s6xType == typeof(S6xElementSignature))
            {
                nameOfCategory = "SignatureCategory";
                nameOfCategory2 = "SignatureCategory2";
                nameOfCategory3 = "SignatureCategory3";
            }

            PropertyInfo piPI = null;
            object oValue = null;

            piPI = s6xType.GetProperty(nameOfDateCreated);
            if (piPI != null) sharedDateCreatedDateTimePicker.Value = Tools.getValidDateTime(piPI.GetValue(s6xObject, null), sadS6x.Properties.DateCreated).ToLocalTime();
            piPI = null;

            piPI = s6xType.GetProperty(nameOfDateUpdated);
            if (piPI != null) sharedDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(piPI.GetValue(s6xObject, null), sadS6x.Properties.DateUpdated).ToLocalTime();
            piPI = null;

            if (s6xType == typeof(S6xProperties))
            {
                sharedCategsLabel.Visible = false;
                sharedCategComboBox.Visible = false;
                sharedCateg2ComboBox.Visible = false;
                sharedCateg3ComboBox.Visible = false;
            }
            else
            {
                sharedCategsLabel.Visible = true;
                sharedCategComboBox.Visible = true;
                sharedCateg2ComboBox.Visible = true;
                sharedCateg3ComboBox.Visible = true;

                S6xNav.s6xNavCategoriesLoad(headerCateg, sharedCategComboBox, S6xNavCategoryLevel.ONE, ref s6xNavCategories);
                S6xNav.s6xNavCategoriesLoad(headerCateg, sharedCateg2ComboBox, S6xNavCategoryLevel.TWO, ref s6xNavCategories);
                S6xNav.s6xNavCategoriesLoad(headerCateg, sharedCateg3ComboBox, S6xNavCategoryLevel.THREE, ref s6xNavCategories);

                piPI = s6xType.GetProperty(nameOfCategory);
                if (piPI != null)
                {
                    oValue = piPI.GetValue(s6xObject, null);
                    if (oValue == null) sharedCategComboBox.Text = string.Empty;
                    else sharedCategComboBox.Text = (string)oValue;
                }
                piPI = null;

                piPI = s6xType.GetProperty(nameOfCategory2);
                if (piPI != null)
                {
                    oValue = piPI.GetValue(s6xObject, null);
                    if (oValue == null) sharedCateg2ComboBox.Text = string.Empty;
                    else sharedCateg2ComboBox.Text = (string)oValue;
                }
                piPI = null;

                piPI = s6xType.GetProperty(nameOfCategory3);
                if (piPI != null)
                {
                    oValue = piPI.GetValue(s6xObject, null);
                    if (oValue == null) sharedCateg3ComboBox.Text = string.Empty;
                    else sharedCateg3ComboBox.Text = (string)oValue;
                }
                piPI = null;
            }

            piPI = s6xType.GetProperty(nameOfIdentificationStatus);
            if (piPI != null)
            {
                oValue = piPI.GetValue(s6xObject, null);
                if ((int)oValue < 0) sharedIdentificationStatusTrackBar.Value = 0;
                else if ((int)oValue > 100) sharedIdentificationStatusTrackBar.Value = 100;
                else sharedIdentificationStatusTrackBar.Value = (int)oValue;
            }
            piPI = null;

            // Windows 10 1809 (10.0.17763) Issue
            sharedIdentificationDetailsTextBox.Clear();
            sharedIdentificationDetailsTextBox.Multiline = false;
            sharedIdentificationDetailsTextBox.Multiline = true;

            piPI = s6xType.GetProperty(nameOfIdentificationDetails);
            if (piPI != null)
            {
                oValue = piPI.GetValue(s6xObject, null);
                if (oValue == null) sharedIdentificationDetailsTextBox.Text = string.Empty;
                else sharedIdentificationDetailsTextBox.Text = (string)oValue;
                sharedIdentificationDetailsTextBox.Text = sharedIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            }
            piPI = null;
        }

        private void showProperties(bool showDetails)
        {
            if (sadS6x == null) return;

            if (!confirmDirtyProperies()) return;

            // Clear elemsTreeView.Tag to indicate, it is properties instead of element
            elemsTreeView.Tag = null;

            hideElemData();

            S6xNavInfo s6xNavInfo = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.PROPERTIES)]);

            // Back Button Mngt
            elemBackPopulate(s6xNavInfo);

            elemLabelTextBox.Text = string.Format("{0,20}", s6xNavInfo.Node.Text);
            elemLabelTextBox.ForeColor = s6xNavInfo.Node.ForeColor;
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
            s6xPropertiesNoNumberingShortFormatCheckBox.Checked = sadS6x.Properties.NoNumberingShortFormat;
            s6xPropertiesRegListOutputCheckBox.Checked = sadS6x.Properties.RegListOutput;
            s6xPropertiesXdfBaseOffsetTextBox.Text = sadS6x.Properties.XdfBaseOffset;
            s6xPropertiesXdfBaseOffsetCheckBox.Checked = sadS6x.Properties.XdfBaseOffsetSubtract;
            // 20210406 - PYM - 0x100 Register Shortcut & SFR Mngt
            s6xPropertiesIgnore8065RegShortcut0x100.Checked = sadS6x.Properties.Ignore8065RegShortcut0x100;
            s6xPropertiesIgnore8065RegShortcut0x100SFR.Checked = sadS6x.Properties.Ignore8065RegShortcut0x100SFR;

            // Windows 10 1809 (10.0.17763) Issue
            s6xPropertiesHeaderTextBox.Clear();
            s6xPropertiesHeaderTextBox.Multiline = false;
            s6xPropertiesHeaderTextBox.Multiline = true;

            s6xPropertiesHeaderTextBox.Text = sadS6x.Properties.Header;
            s6xPropertiesHeaderTextBox.Text = s6xPropertiesHeaderTextBox.Text.Replace("\n", "\r\n");

            s6xPropertiesOutputHeaderCheckBox.Checked = sadS6x.Properties.OutputHeader;

            showSharedDetails(s6xNavInfo.HeaderCategory, sadS6x.Properties);

            resetPropertiesModifiedStatus(s6xPropertiesTabPage);

            showPropertiesTabPage(s6xNavInfo.HeaderCategory, showDetails);

            lastElemS6xNavInfo = s6xNavInfo;

            s6xNavInfo = null;

            elemPanel.Visible = true;
        }

        private void showElem(bool showDetails)
        {
            if (nextElemS6xNavInfo == null) return;
            if (!nextElemS6xNavInfo.isValid) return;

            if (nextElemS6xNavInfo.MainNode == null) return;

            if (!confirmDirtyProperies()) return;
            
            if (lastElemS6xNavInfo != null)
            {
                if (lastElemS6xNavInfo.isValid)
                {
                    if (lastElemS6xNavInfo.MainNode == null) elemOpsRichTextBox.Clear();
                    else if (lastElemS6xNavInfo.MainNode.Name != nextElemS6xNavInfo.MainNode.Name) elemOpsRichTextBox.Clear();
                }
            }

            elemLabelTextBox.Text = string.Format("{0,20}", nextElemS6xNavInfo.MainNode.Text);
            elemLabelTextBox.ForeColor = nextElemS6xNavInfo.MainNode.ForeColor;
            elemBankTextBox.ReadOnly = false;
            elemAddressTextBox.ReadOnly = false;

            // Back Button Mngt
            elemBackPopulate(nextElemS6xNavInfo);

            switch (nextElemS6xNavInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.FUNCTIONS:
                case S6xNavHeaderCategory.SCALARS:
                    if (nextElemS6xNavInfo.isDuplicate)
                    {
                        if (sadS6x.slDupTables.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xTable s6xTable = (S6xTable)sadS6x.slDupTables[nextElemS6xNavInfo.Node.Name];
                            elemBankTextBox.Text = s6xTable.BankNum.ToString();
                            elemAddressTextBox.Text = s6xTable.Address;
                            s6xTable = null;
                        }
                        else if (sadS6x.slDupFunctions.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xFunction s6xFunction = (S6xFunction)sadS6x.slDupFunctions[nextElemS6xNavInfo.Node.Name];
                            elemBankTextBox.Text = s6xFunction.BankNum.ToString();
                            elemAddressTextBox.Text = s6xFunction.Address;
                            s6xFunction = null;
                        }
                        else if (sadS6x.slDupScalars.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xScalar s6xScalar = (S6xScalar)sadS6x.slDupScalars[nextElemS6xNavInfo.Node.Name];
                            elemBankTextBox.Text = s6xScalar.BankNum.ToString();
                            elemAddressTextBox.Text = s6xScalar.Address;
                            s6xScalar = null;
                        }
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xTable s6xTable = (S6xTable)sadS6x.slTables[nextElemS6xNavInfo.Node.Name];
                            elemBankTextBox.Text = s6xTable.BankNum.ToString();
                            elemAddressTextBox.Text = s6xTable.Address;
                            s6xTable = null;
                        }
                        else if (sadS6x.slFunctions.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xFunction s6xFunction = (S6xFunction)sadS6x.slFunctions[nextElemS6xNavInfo.Node.Name];
                            elemBankTextBox.Text = s6xFunction.BankNum.ToString();
                            elemAddressTextBox.Text = s6xFunction.Address;
                            s6xFunction = null;
                        }
                        else if (sadS6x.slScalars.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xScalar s6xScalar = (S6xScalar)sadS6x.slScalars[nextElemS6xNavInfo.Node.Name];
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
                        elemBankTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        showElemData();
                    }
                    showElemProperties(showDetails);
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    if (nextElemS6xNavInfo.isDuplicate)
                    {
                        if (sadS6x.slDupStructures.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xStructure s6xStructure = (S6xStructure)sadS6x.slDupStructures[nextElemS6xNavInfo.Node.Name];
                            elemBankTextBox.Text = s6xStructure.BankNum.ToString();
                            elemAddressTextBox.Text = s6xStructure.Address;
                            s6xStructure = null;
                        }
                    }
                    else
                    {
                        if (sadS6x.slStructures.ContainsKey(nextElemS6xNavInfo.Node.Name))
                        {
                            S6xStructure s6xStructure = (S6xStructure)sadS6x.slStructures[nextElemS6xNavInfo.Node.Name];
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
                        elemBankTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        showElemData();
                    }
                    showElemProperties(showDetails);
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    if (sadS6x.slRoutines.ContainsKey(nextElemS6xNavInfo.Node.Name))
                    {
                        S6xRoutine s6xRoutine = (S6xRoutine)sadS6x.slRoutines[nextElemS6xNavInfo.Node.Name];
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
                        elemBankTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                    }
                    hideElemData();
                    showElemProperties(showDetails);
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    S6xOperation ope = (S6xOperation)sadS6x.slOperations[nextElemS6xNavInfo.Node.Name];
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
                        elemBankTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                    }
                    hideElemData();
                    showElemProperties(showDetails);
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    S6xRegister reg = (S6xRegister)sadS6x.slRegisters[nextElemS6xNavInfo.Node.Name];
                    elemBankTextBox.Text = "R";
                    elemAddressTextBox.Text = reg.Address;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    reg = null;

                    hideElemData();
                    showElemProperties(showDetails);

                    break;
                case S6xNavHeaderCategory.OTHER:
                    S6xOtherAddress other = (S6xOtherAddress)sadS6x.slOtherAddresses[nextElemS6xNavInfo.Node.Name];
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
                        elemBankTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                        elemAddressTextBox.ForeColor = nextElemS6xNavInfo.Node.ForeColor;
                    }
                    hideElemData();
                    showElemProperties(showDetails);
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    S6xSignature sig = (S6xSignature)sadS6x.slSignatures[nextElemS6xNavInfo.Node.Name];
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = sig.UniqueKey;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    sig = null;

                    hideElemData();
                    showElemProperties(showDetails);
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    S6xElementSignature eSig = (S6xElementSignature)sadS6x.slElementsSignatures[nextElemS6xNavInfo.Node.Name];
                    elemBankTextBox.Text = "S";
                    elemAddressTextBox.Text = eSig.UniqueKey;
                    elemBankTextBox.ReadOnly = true;
                    elemAddressTextBox.ReadOnly = true;
                    eSig = null;

                    hideElemData();
                    showElemProperties(showDetails);
                    break;
            }

            lastElemS6xNavInfo = nextElemS6xNavInfo;

            elemPanel.Visible = true;
        }

        private void hideElemData()
        {
            elemDataGridView.Visible = false;
        }

        private void showElemData()
        {
            convertToolStripMenuItem.Tag = null;
            convertInputToolStripMenuItem.Tag = null;
            showElemData(null, null);
        }

        private void showElemData(RepositoryConversionItem rcOutput, RepositoryConversionItem rcInput)
        {
            if (nextElemS6xNavInfo == null) return;
            if (!nextElemS6xNavInfo.isValid) return;

            if (nextElemS6xNavInfo.MainNode == null) return;

            if (sadBin == null) return;

            object s6xObject = null;
            switch (nextElemS6xNavInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.SCALARS:
                    if (nextElemS6xNavInfo.isDuplicate) s6xObject = sadS6x.slDupScalars[nextElemS6xNavInfo.Node.Name];
                    else s6xObject = sadS6x.slScalars[nextElemS6xNavInfo.Node.Name];
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    if (nextElemS6xNavInfo.isDuplicate) s6xObject = sadS6x.slDupFunctions[nextElemS6xNavInfo.Node.Name];
                    else s6xObject = sadS6x.slFunctions[nextElemS6xNavInfo.Node.Name];
                    break;
                case S6xNavHeaderCategory.TABLES:
                    if (nextElemS6xNavInfo.isDuplicate) s6xObject = sadS6x.slDupTables[nextElemS6xNavInfo.Node.Name];
                    else s6xObject = sadS6x.slTables[nextElemS6xNavInfo.Node.Name];
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    if (nextElemS6xNavInfo.isDuplicate) s6xObject = sadS6x.slDupStructures[nextElemS6xNavInfo.Node.Name];
                    else s6xObject = sadS6x.slStructures[nextElemS6xNavInfo.Node.Name];
                    break;
            }

            if (sadBin == null) return;
            if (s6xObject == null) return;

            ToolsElemData.showElemData(ref elemDataGridView, ref sadBin, s6xObject, decimalToolStripMenuItem.Checked, decimalNotConvertedToolStripMenuItem.Checked, reverseOrderToolStripMenuItem.Checked, rcOutput, rcInput);
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

        private void showElemProperties(bool showDetails)
        {
            if (nextElemS6xNavInfo == null) return;
            if (!nextElemS6xNavInfo.isValid) return;

            if (nextElemS6xNavInfo.MainNode == null) return;

            S6xNavHeaderCategory headerCateg = nextElemS6xNavInfo.HeaderCategory;

            showPropertiesTabPage(headerCateg, showDetails);

            switch (headerCateg)
            {
                case S6xNavHeaderCategory.TABLES:
                    S6xTable s6xTable = null;
                    if (nextElemS6xNavInfo.isDuplicate) s6xTable = (S6xTable)sadS6x.slDupTables[nextElemS6xNavInfo.Node.Name];
                    else s6xTable = (S6xTable)sadS6x.slTables[nextElemS6xNavInfo.Node.Name];
                    showElemTableProperties(ref s6xTable);
                    showSharedDetails(headerCateg, s6xTable);
                    s6xTable = null;
                    resetPropertiesModifiedStatus(elemTablePropertiesTabPage);
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    S6xFunction s6xFunction = null;
                    if (nextElemS6xNavInfo.isDuplicate) s6xFunction = (S6xFunction)sadS6x.slDupFunctions[nextElemS6xNavInfo.Node.Name];
                    else s6xFunction = (S6xFunction)sadS6x.slFunctions[nextElemS6xNavInfo.Node.Name];
                    showElemFunctionProperties(ref s6xFunction);
                    showSharedDetails(headerCateg, s6xFunction);
                    s6xFunction = null;
                    resetPropertiesModifiedStatus(elemFunctionPropertiesTabPage);
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    S6xScalar s6xScalar = null;
                    if (nextElemS6xNavInfo.isDuplicate) s6xScalar = (S6xScalar)sadS6x.slDupScalars[nextElemS6xNavInfo.Node.Name];
                    else s6xScalar = (S6xScalar)sadS6x.slScalars[nextElemS6xNavInfo.Node.Name];
                    showElemScalarProperties(ref s6xScalar);
                    showSharedDetails(headerCateg, s6xScalar);
                    s6xScalar = null;
                    resetPropertiesModifiedStatus(elemScalarPropertiesTabPage);
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    S6xStructure s6xStructure = null;
                    if (nextElemS6xNavInfo.isDuplicate) s6xStructure = (S6xStructure)sadS6x.slDupStructures[nextElemS6xNavInfo.Node.Name];
                    else s6xStructure = (S6xStructure)sadS6x.slStructures[nextElemS6xNavInfo.Node.Name];
                    showElemStructureProperties(ref s6xStructure);
                    showSharedDetails(headerCateg, s6xStructure);
                    s6xStructure = null;
                    resetPropertiesModifiedStatus(elemStructurePropertiesTabPage);
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    S6xRoutine s6xRoutine = (S6xRoutine)sadS6x.slRoutines[nextElemS6xNavInfo.Node.Name];
                    showElemRoutineProperties(ref s6xRoutine);
                    showSharedDetails(headerCateg, s6xRoutine);
                    s6xRoutine = null;
                    resetPropertiesModifiedStatus(elemRoutineTabPage);
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    S6xOperation s6xOpe = (S6xOperation)sadS6x.slOperations[nextElemS6xNavInfo.Node.Name];
                    showElemOperationProperties(ref s6xOpe);
                    showSharedDetails(headerCateg, s6xOpe);
                    s6xOpe = null;
                    resetPropertiesModifiedStatus(elemOpeTabPage);
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    S6xRegister s6xReg = (S6xRegister)sadS6x.slRegisters[nextElemS6xNavInfo.Node.Name];
                    showElemRegisterProperties(ref s6xReg);
                    showSharedDetails(headerCateg, s6xReg);
                    s6xReg = null;
                    resetPropertiesModifiedStatus(elemRegisterTabPage);
                    break;
                case S6xNavHeaderCategory.OTHER:
                    S6xOtherAddress s6xOther = (S6xOtherAddress)sadS6x.slOtherAddresses[nextElemS6xNavInfo.Node.Name];
                    showElemOtherProperties(ref s6xOther);
                    showSharedDetails(headerCateg, s6xOther);
                    s6xOther = null;
                    resetPropertiesModifiedStatus(elemOtherTabPage);
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    S6xSignature s6xSig = (S6xSignature)sadS6x.slSignatures[nextElemS6xNavInfo.Node.Name];
                    showElemSignatureProperties(ref s6xSig);
                    showSharedDetails(headerCateg, s6xSig);
                    s6xSig = null;
                    resetPropertiesModifiedStatus(elemSignatureTabPage);
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    S6xElementSignature s6xESig = (S6xElementSignature)sadS6x.slElementsSignatures[nextElemS6xNavInfo.Node.Name];
                    showElemElemSignatureProperties(ref s6xESig);
                    showSharedDetails(headerCateg, s6xESig);
                    s6xESig = null;
                    resetPropertiesModifiedStatus(elemElemSignatureTabPage);
                    break;
            }
        }

        private void showPropertiesTabPage(S6xNavHeaderCategory headerCateg, bool showDetails)
        {
            TabPage selectedTabPage = null;
            TabPage detailsTabPage = sharedDetailsTabPage;
            TabPage infoTabPage = null;
            ArrayList removeTabPages = null;

            switch (headerCateg)
            {
                case S6xNavHeaderCategory.PROPERTIES:
                    selectedTabPage = s6xPropertiesTabPage;
                    break;
                case S6xNavHeaderCategory.TABLES:
                    selectedTabPage = elemTablePropertiesTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    selectedTabPage = elemFunctionPropertiesTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    selectedTabPage = elemScalarPropertiesTabPage;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    selectedTabPage = elemStructurePropertiesTabPage;
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    selectedTabPage = elemRoutineTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    selectedTabPage = elemOpeTabPage;
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    selectedTabPage = elemRegisterTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case S6xNavHeaderCategory.OTHER:
                    selectedTabPage = elemOtherTabPage;
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    selectedTabPage = elemSignatureTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    selectedTabPage = elemElemSignatureTabPage;
                    infoTabPage = elemInfoTabPage;
                    break;
            }

            removeTabPages = new ArrayList();
            foreach (TabPage tabPage in elemTabControl.TabPages)
            {
                if (selectedTabPage != null)
                {
                    if (tabPage == selectedTabPage) continue;
                    if (tabPage == detailsTabPage) continue;
                    if (tabPage == infoTabPage) continue;
                }
                removeTabPages.Add(tabPage);
            }
            foreach (TabPage tabPage in removeTabPages) elemTabControl.TabPages.Remove(tabPage);
            removeTabPages = null;

            if (selectedTabPage != null)
            {
                if (!elemTabControl.TabPages.Contains(selectedTabPage))
                {
                    if (elemTabControl.TabPages.Count == 0) elemTabControl.TabPages.Add(selectedTabPage);
                    else elemTabControl.TabPages.Insert(0, selectedTabPage);
                }
                if (detailsTabPage != null) if (!elemTabControl.TabPages.Contains(detailsTabPage)) elemTabControl.TabPages.Add(detailsTabPage);
                if (infoTabPage != null) if (!elemTabControl.TabPages.Contains(infoTabPage)) elemTabControl.TabPages.Add(infoTabPage);
            }

            if (showDetails) elemTabControl.SelectedTab = detailsTabPage;
            else elemTabControl.SelectedTab = selectedTabPage;

            selectedTabPage = null;
            detailsTabPage = null;
            infoTabPage = null;
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

        private bool checkMinMax(string number)
        {
            if (number == Tools.getValidMinMax(number)) return true;

            MessageBox.Show("Minimum or Maximum value \"" + number + "\" will not be seen as a valid expression.\r\nExpect format is '000000.0000'.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
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

        private S6xNavInfo checkDuplicate(string uniqueAddress, bool showMessages, S6xNavHeaderCategory[] forSpecificCategs, S6xNavHeaderCategory[] exceptSpecificCategs)
        {
            S6xNavInfo niDuplicate = null;
            bool noQuestion = false;
            bool noRemoval = false;
            bool hideDuplicate = false;
            bool fromOtherUpdate = false;
            string sMessage = string.Empty;

            List<S6xNavHeaderCategory> lForCategs = new List<S6xNavHeaderCategory>();
            List<S6xNavHeaderCategory> lExcCategs = new List<S6xNavHeaderCategory>();
            List<S6xNavInfo> lCategs = new List<S6xNavInfo>();

            if (forSpecificCategs != null) lForCategs.AddRange(forSpecificCategs);
            if (exceptSpecificCategs != null) lExcCategs.AddRange(exceptSpecificCategs);

            // Default Ignored Categs
            lExcCategs.AddRange(new S6xNavHeaderCategory[] { S6xNavHeaderCategory.PROPERTIES, S6xNavHeaderCategory.SIGNATURES, S6xNavHeaderCategory.ELEMSSIGNATURES });

            foreach (TreeNode tnCateg in elemsTreeView.Nodes)
            {
                if (lForCategs.Count > 0 && !lForCategs.Contains(S6xNav.getHeaderCateg(tnCateg.Name))) continue;
                if (lExcCategs.Contains(S6xNav.getHeaderCateg(tnCateg.Name))) continue;
                lCategs.Add(new S6xNavInfo(tnCateg));
            }

            fromOtherUpdate = lForCategs.Contains(S6xNavHeaderCategory.OTHER);

            lForCategs = null;
            lExcCategs = null;

            foreach (S6xNavInfo niCateg in lCategs)
            {
                TreeNode tnDuplicate = niCateg.FindElement(uniqueAddress);
                if (tnDuplicate != null)
                {
                    niDuplicate = new S6xNavInfo(tnDuplicate);
                    break;
                }
            }
            if (niDuplicate == null) return null;       // No Duplicate at all

            sMessage = "Address is already used.\r\n";
            sMessage += "\t" + niDuplicate.Node.Text + "\tfound in " + niDuplicate.HeaderCategoryNode.Text + "\r\n";
            switch (niDuplicate.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    noQuestion = true;
                    noRemoval = true;
                    hideDuplicate = false;
                    break;
                case S6xNavHeaderCategory.OTHER:
                    noQuestion = !fromOtherUpdate;
                    noRemoval = !fromOtherUpdate; // Can be overriden, by question
                    hideDuplicate = !fromOtherUpdate; // Can be overriden, by question
                    break;
                default:
                    noQuestion = false;
                    noRemoval = false; // Can be overriden, by question
                    hideDuplicate = false; // Can be overriden, by question
                    if (niDuplicate.HeaderCategory == S6xNavHeaderCategory.REGISTERS)
                    {
                        // Registers created directly at load or on disassembly are protected
                        if (sadS6x.slRegisters.ContainsKey(niDuplicate.Node.Name))
                        {
                            if (((S6xRegister)sadS6x.slRegisters[niDuplicate.Node.Name]).AutoConstValue)
                            {
                                noQuestion = true;
                                noRemoval = true;
                                hideDuplicate = false;
                            }
                        }
                    }
                    else if (sadBin.Calibration.isLoadCreated(niDuplicate.Node.Name))
                    {
                        // Elements created directly at load are protected
                        noQuestion = true;
                        noRemoval = true;
                        hideDuplicate = false;
                    }
                    break;
            }
            if (showMessages)
            {
                if (noQuestion)
                {
                    MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    sMessage += "Overwrite existing element ?";
                    if (MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        noRemoval = false;
                        hideDuplicate = false;
                    }
                    else
                    {
                        noRemoval = true;
                        hideDuplicate = false;
                    }
                }
            }

            if (noRemoval)
            {
                if (hideDuplicate) return null; // No removal and no information on duplicate
                else return new S6xNavInfo(new TreeNode()); // To prevent element removal, often base condition to end the next actions
            }

            return niDuplicate;
        }

        private bool checkElem(S6xNavHeaderCategory headerCateg)
        {
            switch (headerCateg)
            {
                case S6xNavHeaderCategory.TABLES:
                    return checkElemTable();
                case S6xNavHeaderCategory.FUNCTIONS:
                    return checkElemFunction();
                case S6xNavHeaderCategory.SCALARS:
                    return checkElemScalar();
                case S6xNavHeaderCategory.STRUCTURES:
                    return checkElemStructure();
                case S6xNavHeaderCategory.ROUTINES:
                    return checkElemRoutine();
                case S6xNavHeaderCategory.OPERATIONS:
                    return checkElemOperation();
                case S6xNavHeaderCategory.REGISTERS:
                    return checkElemRegister();
                case S6xNavHeaderCategory.OTHER:
                    return checkElemOther();
                case S6xNavHeaderCategory.SIGNATURES:
                    return checkElemSignature();
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    return checkElemElemSignature();
                default:
                    return false;
            }
        }

        private bool checkElemScalar()
        {
            bool checkPassed = true;

            checkPassed = checkElemBankAddress(true);

            if (checkPassed)
            {
                checkScaleExpression(scalarScaleTextBox.Text); //WARNING ONLY
                checkMinMax(scalarMinTextBox.Text);  // WARNING ONLY
                checkMinMax(scalarMaxTextBox.Text);  // WARNING ONLY
            }

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
                checkMinMax(functionMinInputTextBox.Text);  // WARNING ONLY
                checkMinMax(functionMaxInputTextBox.Text);  // WARNING ONLY
                checkMinMax(functionMinOutputTextBox.Text);  // WARNING ONLY
                checkMinMax(functionMaxOutputTextBox.Text);  // WARNING ONLY
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
            if (checkPassed)
            {
                checkScaleExpression(tableScaleTextBox.Text);  // WARNING ONLY
                checkMinMax(tableMinTextBox.Text);  // WARNING ONLY
                checkMinMax(tableMaxTextBox.Text);  // WARNING ONLY
            }

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

            if (!checkPassed)
            {
                MessageBox.Show("Invalid Register address.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return checkPassed;
            }

            try
            {
                if (regConstValueTextBox.Text != string.Empty)
                {
                    int constValue = Convert.ToInt32(regConstValueTextBox.Text, 16);
                    if (constValue < 0 || constValue > 0xffff) checkPassed = false;
                }
            }
            catch
            {
                checkPassed = false;
            }

            if (!checkPassed)
            {
                MessageBox.Show("Invalid Constant value.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return checkPassed;
            }

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
            scalarInlineCommentsCheckBox.Checked = false;

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

            functionMinInputTextBox.Text = string.Empty;
            functionMaxInputTextBox.Text = string.Empty;
            functionMinOutputTextBox.Text = string.Empty;
            functionMaxOutputTextBox.Text = string.Empty;

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
            opeInlineCommentsCheckBox.Checked = false;
        }

        private void newElemRegisterProperties()
        {
            regAddressTextBox.Text = "00";
            regAddressTextBox.ReadOnly = false;
            regSkipCheckBox.Checked = false;

            regRBaseCheckBox.Checked = false;
            regRBaseCheckBox.Enabled = false;
            regRConstCheckBox.Checked = false;
            regRConstCheckBox.Enabled = true;
            regConstValueTextBox.Text = string.Empty;
            regConstValueTextBox.ReadOnly = false;

            regLabelTextBox.Text = elemLabelTextBox.Text;
            regByteLabelTextBox.Text = string.Empty;
            regWordLabelTextBox.Text = string.Empty;
            regScaleTextBox.Text = "X";
            regScalePrecNumericUpDown.Value = SADDef.DefaultScalePrecision;
            regUnitsTextBox.Text = string.Empty;
            foreach (string oItem in regSizeComboBox.Items)
            {
                if (oItem == string.Empty)
                {
                    regSizeComboBox.SelectedItem = oItem;
                    break;
                }
            }
            foreach (string oItem in regSignedComboBox.Items)
            {
                if (oItem == string.Empty)
                {
                    regSignedComboBox.SelectedItem = oItem;
                    break;
                }
            }
            
            regCommentsTextBox.Text = string.Empty;

            elemInfoRichTextBox.Clear();

            regBitFlagsCheckBox.Checked = false;
            regBitFlagsButton.Tag = null;
        }

        private void newElemOtherProperties()
        {
            otherSkipCheckBox.Checked = false;
            otherLabelTextBox.Text = string.Empty;
            otherOutputLabelCheckBox.Checked = false;
            otherCommentsTextBox.Text = string.Empty;
            otherOutputCommentsCheckBox.Checked = false;
            otherInlineCommentsCheckBox.Checked = false;
        }

        private void newElemSignatureProperties()
        {
            signatureSigTextBox.Text = string.Empty;
            signatureSkipCheckBox.Checked = false;
            signatureLabelTextBox.Text = elemLabelTextBox.Text;
            signatureCommentsTextBox.Text = string.Empty;

            signatureForcedCheckBox.Checked = false;

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

            scalarMinTextBox.Text = s6xScalar.Min;
            scalarMaxTextBox.Text = s6xScalar.Max;

            // Windows 10 1809 (10.0.17763) Issue
            scalarCommentsTextBox.Clear();
            scalarCommentsTextBox.Multiline = false;
            scalarCommentsTextBox.Multiline = true;
            
            scalarCommentsTextBox.Text = s6xScalar.Comments;
            scalarCommentsTextBox.Text = scalarCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            scalarOutputCommentsCheckBox.Checked = s6xScalar.OutputComments;
            scalarInlineCommentsCheckBox.Checked = s6xScalar.InlineComments;

            scalarBitFlagsCheckBox.Checked = s6xScalar.isBitFlags;

            scalarBitFlagsButton.Tag = null;
        }

        private void showElemFunctionProperties(ref S6xFunction s6xFunction)
        {
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

            functionMinInputTextBox.Text = s6xFunction.InputMin;
            functionMaxInputTextBox.Text = s6xFunction.InputMax;
            functionMinOutputTextBox.Text = s6xFunction.OutputMin;
            functionMaxOutputTextBox.Text = s6xFunction.OutputMax;

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
                    if (tableColsUnitsTextBox.Text == string.Empty) tableColsUnitsTextBox.Text = s6xScaler.InputUnits;
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
                    if (tableRowsUnitsTextBox.Text == string.Empty) tableRowsUnitsTextBox.Text = s6xScaler.InputUnits;
                    s6xScaler = null;
                }
            }
            tableSkipCheckBox.Checked = s6xTable.Skip;
            tableLabelTextBox.Text = s6xTable.Label;
            tableSLabelTextBox.Text = s6xTable.ShortLabel;

            tableMinTextBox.Text = s6xTable.CellsMin;
            tableMaxTextBox.Text = s6xTable.CellsMax;

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

            showElemRoutineInfo(ref s6xRoutine);
        }

        private void showElemRoutineInfo(ref S6xRoutine s6xRoutine)
        {
            elemInfoRichTextBox.Clear();

            if (s6xRoutine.Information != null && s6xRoutine.Information != string.Empty)
            {
                elemInfoRichTextBox.AppendText(s6xRoutine.Information);
                return;
            }

            s6xRoutine.Information = string.Empty;
            if (sadBin.isLoaded && sadBin.isDisassembled)
            {
                Call cCall = (Call)sadBin.Calibration.slCalls[s6xRoutine.UniqueAddress];
                if (cCall != null)
                {
                    if (cCall.Callers.Count > 1)
                    {
                        if (s6xRoutine.Information != string.Empty) s6xRoutine.Information += "\r\n";
                        s6xRoutine.Information += "Called " + cCall.Callers.Count.ToString() + " times.";
                    }
                }
            }

            elemInfoRichTextBox.AppendText(s6xRoutine.Information);
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
            opeInlineCommentsCheckBox.Checked = ope.InlineComments;
        }

        private void showElemRegisterProperties(ref S6xRegister reg)
        {
            regAddressTextBox.Text = reg.Address;
            regAddressTextBox.ReadOnly = reg.AutoConstValue;

            regSkipCheckBox.Checked = reg.Skip;

            regRBaseCheckBox.Checked = reg.isRBase;
            regRBaseCheckBox.Enabled = false;
            regRConstCheckBox.Checked = reg.isRConst;
            regRConstCheckBox.Enabled = !reg.AutoConstValue;
            regConstValueTextBox.Text = reg.ConstValue;
            regConstValueTextBox.ReadOnly = reg.AutoConstValue;
            
            regLabelTextBox.Text = reg.Label;
            regByteLabelTextBox.Text = reg.ByteLabel;
            regWordLabelTextBox.Text = reg.WordLabel;
            regScaleTextBox.Text = reg.ScaleExpression;
            regScalePrecNumericUpDown.Value = reg.ScalePrecision;
            regUnitsTextBox.Text = reg.Units;
            if (reg.SizeStatus == null) reg.SizeStatus = string.Empty;
            foreach (object oItem in regSizeComboBox.Items)
            {
                if (((string)oItem).ToLower() == reg.SizeStatus.ToLower())
                {
                    regSizeComboBox.SelectedItem = oItem;
                    break;
                }
            }
            if (reg.SignedStatus == null) reg.SignedStatus = string.Empty;
            foreach (object oItem in regSignedComboBox.Items)
            {
                if (((string)oItem).ToLower() == reg.SignedStatus.ToLower())
                {
                    regSignedComboBox.SelectedItem = oItem;
                    break;
                }
            }

            // Windows 10 1809 (10.0.17763) Issue
            regCommentsTextBox.Clear();
            regCommentsTextBox.Multiline = false;
            regCommentsTextBox.Multiline = true;
            
            regCommentsTextBox.Text = reg.Comments;
            regCommentsTextBox.Text = regCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

            regBitFlagsCheckBox.Checked = reg.isBitFlags;

            regBitFlagsButton.Tag = null;

            showElemRegisterInfo(ref reg);
        }

        private void showElemRegisterInfo(ref S6xRegister s6xRegister)
        {
            elemInfoRichTextBox.Clear();
            if (s6xRegister.Information != null && s6xRegister.Information != string.Empty)
            {
                elemInfoRichTextBox.AppendText(s6xRegister.Information);
                return;
            }

            s6xRegister.Information = string.Empty;
            if (sadBin.isLoaded && sadBin.isDisassembled)
            {
                Register rReg = (Register)sadBin.Calibration.slRegisters[s6xRegister.UniqueAddress];
                if (rReg != null)
                {
                    if (rReg.Links != null)
                    {
                        foreach (RegisterRegisterLink lLink in rReg.Links.RegistersLinks.Values)
                        {
                            S6xRegister s6xObject = (S6xRegister)sadS6x.slRegisters[lLink.RegisterUniqueAddress];
                            if (s6xObject == null) continue;
                            foreach (string relatedFunctionsUniqueAddress in lLink.RelatedFunctionsUniqueAddresses)
                            {
                                S6xFunction s6xFunction = (S6xFunction)sadS6x.slFunctions[relatedFunctionsUniqueAddress];
                                if (s6xFunction == null) continue;
                                if (lLink.isSourceScaler) s6xRegister.Information += "Table scaler from register \"" + s6xObject.Label + "\" (" + Tools.RegisterInstruction(s6xObject.Address) + ") through function \"" + s6xFunction.Label + "\" (" + s6xFunction.Address + ")\r\n";
                                else if (lLink.isSourceThroughFunction) s6xRegister.Information += "From register \"" + s6xObject.Label + "\" (" + Tools.RegisterInstruction(s6xObject.Address) + ") through function \"" + s6xFunction.Label + "\" (" + s6xFunction.Address + ")\r\n";
                                if (lLink.isDestinationScaler) s6xRegister.Information += "Source value for table scaler \"" + s6xObject.Label + "\" (" + Tools.RegisterInstruction(s6xObject.Address) + ") through function \"" + s6xFunction.Label + "\" (" + s6xFunction.Address + ")\r\n";
                                else if (lLink.isDestinationThroughFunction) s6xRegister.Information += "Source value for register \"" + s6xObject.Label + "\" (" + Tools.RegisterInstruction(s6xObject.Address) + ") through function \"" + s6xFunction.Label + "\" (" + s6xFunction.Address + ")\r\n";
                            }
                        }

                        foreach (RegisterTableLink lLink in rReg.Links.TablesLinks.Values)
                        {
                            S6xTable s6xObject = (S6xTable)sadS6x.slTables[lLink.TableUniqueAddress];
                            if (s6xObject == null) continue;
                            if (lLink.isColumnsScaler) s6xRegister.Information += "Columns scaler for table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                            if (lLink.isRowsScaler) s6xRegister.Information += "Rows scaler for table \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                            if (lLink.isOutputRegister) s6xRegister.Information += "Output for function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                            if (lLink.isOutputRegisterByte) s6xRegister.Information += "Byte Output for function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                        }

                        foreach (RegisterFunctionLink lLink in rReg.Links.FunctionsLinks.Values)
                        {
                            S6xFunction s6xObject = (S6xFunction)sadS6x.slFunctions[lLink.FunctionUniqueAddress];
                            if (s6xObject == null) continue;
                            if (lLink.isInputRegister) s6xRegister.Information += "Input for function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                            if (lLink.isOutputRegister) s6xRegister.Information += "Output for function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                            if (lLink.isOutputRegisterByte) s6xRegister.Information += "Byte Output for function \"" + s6xObject.Label + "\" (" + s6xObject.Address + ")\r\n";
                        }
                    }
                }
            }

            elemInfoRichTextBox.AppendText(s6xRegister.Information);
        }

        private void showElemOtherProperties(ref S6xOtherAddress other)
        {
            otherSkipCheckBox.Checked = other.Skip;
            otherLabelTextBox.Text = other.Label;
            otherOutputLabelCheckBox.Checked = other.OutputLabel;

            // Windows 10 1809 (10.0.17763) Issue
            otherCommentsTextBox.Clear();
            otherCommentsTextBox.Multiline = false;
            otherCommentsTextBox.Multiline = true;
            
            otherCommentsTextBox.Text = other.Comments;
            otherCommentsTextBox.Text = otherCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");
            otherOutputCommentsCheckBox.Checked = other.OutputComments;
            otherInlineCommentsCheckBox.Checked = other.InlineComments;
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
            signatureLabelTextBox.Text = sig.SignatureLabel;
            if (signatureLabelTextBox.Text == string.Empty) signatureLabelTextBox.Text = SADDef.LongSignaturePrefix + (sig.Label == null ? string.Empty : sig.Label);

            // Windows 10 1809 (10.0.17763) Issue
            signatureCommentsTextBox.Clear();
            signatureCommentsTextBox.Multiline = false;
            signatureCommentsTextBox.Multiline = true;
            
            signatureCommentsTextBox.Text = sig.SignatureComments;
            signatureCommentsTextBox.Text = signatureCommentsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

            signatureAdvCheckBox.Checked = sig.isAdvanced;
            signatureForcedCheckBox.Checked = sig.Forced;

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

        private void updateSharedDetails(S6xNavHeaderCategory headerCateg, object s6xObject)
        {
            if (s6xObject == null) return;

            string nameOfDateCreated = "DateCreated";
            string nameOfDateUpdated = "DateUpdated";
            string nameOfCategory = "Category";
            string nameOfCategory2 = "Category2";
            string nameOfCategory3 = "Category3";
            string nameOfIdentificationStatus = "IdentificationStatus";
            string nameOfIdentificationDetails = "IdentificationDetails";

            // Global Categories header
            S6xNavHeaderCategory replacedHeaderCateg = S6xNavHeaderCategory.UNDEFINED;

            Type s6xType = s6xObject.GetType();

            if (s6xType == typeof(S6xSignature) || s6xType == typeof(S6xElementSignature))
            {
                nameOfCategory = "SignatureCategory";
                nameOfCategory2 = "SignatureCategory2";
                nameOfCategory3 = "SignatureCategory3";
                replacedHeaderCateg = headerCateg;
            }

            PropertyInfo piPI = null;

            piPI = s6xType.GetProperty(nameOfIdentificationStatus);
            if (piPI != null) piPI.SetValue(s6xObject, sharedIdentificationStatusTrackBar.Value, null);
            piPI = null;

            piPI = s6xType.GetProperty(nameOfIdentificationDetails);
            if (piPI != null) piPI.SetValue(s6xObject, sharedIdentificationDetailsTextBox.Text, null);
            piPI = null;

            if (s6xType != typeof(S6xProperties))
            {
                piPI = s6xType.GetProperty(nameOfDateCreated);
                if (piPI != null) piPI.SetValue(s6xObject, sharedDateCreatedDateTimePicker.Value.ToUniversalTime(), null);
                piPI = null;

                piPI = s6xType.GetProperty(nameOfDateUpdated);
                if (piPI != null)
                {
                    sharedDateUpdatedDateTimePicker.Value = DateTime.Now;
                    piPI.SetValue(s6xObject, sharedDateUpdatedDateTimePicker.Value.ToUniversalTime(), null);
                }
                piPI = null;

                piPI = s6xType.GetProperty(nameOfCategory);
                if (piPI != null)
                {
                    piPI.SetValue(s6xObject, sharedCategComboBox.Text, null);
                    S6xNav.s6xNavCategoriesAdd(replacedHeaderCateg, headerCateg, ref sharedCategComboBox, S6xNavCategoryLevel.ONE, sharedCategComboBox.Text, ref s6xNavCategories);
                }
                piPI = null;

                piPI = s6xType.GetProperty(nameOfCategory2);
                if (piPI != null)
                {
                    piPI.SetValue(s6xObject, sharedCateg2ComboBox.Text, null);
                    S6xNav.s6xNavCategoriesAdd(replacedHeaderCateg, headerCateg, ref sharedCateg2ComboBox, S6xNavCategoryLevel.TWO, sharedCateg2ComboBox.Text, ref s6xNavCategories);
                }
                piPI = null;

                piPI = s6xType.GetProperty(nameOfCategory3);
                if (piPI != null)
                {
                    piPI.SetValue(s6xObject, sharedCateg3ComboBox.Text, null);
                    S6xNav.s6xNavCategoriesAdd(replacedHeaderCateg, headerCateg, ref sharedCateg3ComboBox, S6xNavCategoryLevel.THREE, sharedCateg3ComboBox.Text, ref s6xNavCategories);
                }
                piPI = null;
            }

            sadS6x.Properties.DateUpdated = DateTime.UtcNow;
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
            sadS6x.Properties.NoNumberingShortFormat = s6xPropertiesNoNumberingShortFormatCheckBox.Checked;
            sadS6x.Properties.RegListOutput = s6xPropertiesRegListOutputCheckBox.Checked;
            sadS6x.Properties.XdfBaseOffset = s6xPropertiesXdfBaseOffsetTextBox.Text;
            sadS6x.Properties.XdfBaseOffsetSubtract = s6xPropertiesXdfBaseOffsetCheckBox.Checked;
            sadS6x.Properties.Header = s6xPropertiesHeaderTextBox.Text;
            sadS6x.Properties.OutputHeader = s6xPropertiesOutputHeaderCheckBox.Checked;
            // 20210406 - PYM - 0x100 Register Shortcut & SFR Mngt
            sadS6x.Properties.Ignore8065RegShortcut0x100 = s6xPropertiesIgnore8065RegShortcut0x100.Checked;
            sadS6x.Properties.Ignore8065RegShortcut0x100SFR = s6xPropertiesIgnore8065RegShortcut0x100SFR.Checked;

            updateSharedDetails(S6xNavHeaderCategory.PROPERTIES, sadS6x.Properties);

            sadS6x.isSaved = false;

            resetPropertiesModifiedStatus(s6xPropertiesTabPage);
            
            elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.PROPERTIES)].ForeColor = Color.Purple;

            //disassemblyToolStripMenuItem.Enabled = false;
            //outputToolStripMenuItem.Enabled = false;
        }
        
        private void updateElem()
        {
            S6xNavInfo s6xNICateg = null;
            S6xNavInfo niOverwrite = null;
            TreeNode tnNode = null;
            TreeNode tnTmp = null;
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

            s6xNICateg = (S6xNavInfo)elemsTreeView.Tag;

            if (!s6xNICateg.isValid) return;
            if (!s6xNICateg.isHeaderCategory) return;

            if (!checkElem(s6xNICateg.HeaderCategory)) return;

            uniqueAddress = string.Empty;
            uniqueAddressOri = "X XXXXX";
            if (nextElemS6xNavInfo != null)
            {
                if (nextElemS6xNavInfo.isValid)
                {
                    uniqueAddress = nextElemS6xNavInfo.Node.Name;
                    uniqueAddressOri = nextElemS6xNavInfo.Node.Name;
                    isClipBoardElem = alClipBoardTempUniqueAddresses.Contains(uniqueAddressOri);
                    bDuplicate = nextElemS6xNavInfo.isDuplicate;
                }
            }

            switch (s6xNICateg.HeaderCategory)
            {
                case S6xNavHeaderCategory.SIGNATURES:
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    // Unique Index based / Can not change
                    break;
                case S6xNavHeaderCategory.REGISTERS:
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
                        niOverwrite = checkDuplicate(uniqueAddress, true, null, null);
                        if (niOverwrite != null)
                        {
                            if (niOverwrite.Node.Parent == null) return;

                            if (nextElemS6xNavInfo != null) tnNode = nextElemS6xNavInfo.Node;      // Backup because erased in deletedElem;
                            deleteElem(niOverwrite, true, false, true);
                            niOverwrite = null;
                            if (tnNode != null) nextElemS6xNavInfo = new S6xNavInfo(tnNode);
                            tnNode = null;
                        }
                    }
                    break;
                case S6xNavHeaderCategory.OTHER:
                    // Unique Address based / Can Change
                    bankNum = Convert.ToInt32(elemBankTextBox.Text);
                    addressInt = Convert.ToInt32(elemAddressTextBox.Text, 16) - SADDef.EecBankStartAddress;
                    uniqueAddress = Tools.UniqueAddress(bankNum, addressInt);

                    if (uniqueAddress != uniqueAddressOri && !bDuplicate)
                    {
                        niOverwrite = checkDuplicate(uniqueAddress, true, new S6xNavHeaderCategory[] { s6xNICateg.HeaderCategory }, null);
                        if (niOverwrite != null)
                        {
                            if (niOverwrite.Node.Parent == null) return;

                            if (nextElemS6xNavInfo != null) tnNode = nextElemS6xNavInfo.Node;      // Backup because erased in deletedElem;
                            deleteElem(niOverwrite, true, false, true);
                            niOverwrite = null;
                            if (tnNode != null) nextElemS6xNavInfo = new S6xNavInfo(tnNode);
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
                        niOverwrite = checkDuplicate(uniqueAddress, true, null, null);
                        if (niOverwrite != null)
                        {
                            if (niOverwrite.Node.Parent == null) return;

                            if (nextElemS6xNavInfo != null) tnNode = nextElemS6xNavInfo.Node;      // Backup because erased in deletedElem;
                            deleteElem(niOverwrite, true, false, true);
                            niOverwrite = null;
                            if (tnNode != null) nextElemS6xNavInfo = new S6xNavInfo(tnNode);
                            tnNode = null;
                        }
                    }
                    break;
            }

            isAddressChange = (nextElemS6xNavInfo != null && uniqueAddress != uniqueAddressOri && !bDuplicate);

            switch (s6xNICateg.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    resetPropertiesModifiedStatus(elemTablePropertiesTabPage);
                    
                    S6xTable s6xTable = new S6xTable();
                    s6xTable.BankNum = bankNum;
                    s6xTable.AddressInt = addressInt;
                    if (bDuplicate) s6xTable.DuplicateNum = ((S6xTable)sadS6x.slDupTables[nextElemS6xNavInfo.Node.Name]).DuplicateNum;
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
                    s6xTable.CellsMin = tableMinTextBox.Text;
                    s6xTable.CellsMax = tableMaxTextBox.Text;

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

                    updateSharedDetails(S6xNavHeaderCategory.TABLES, s6xTable);

                    if (isAddressChange)
                    {
                        sadS6x.slTables.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xTable.UniqueAddress;
                        tnTmp = null;
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
                        tnNode = nextElemS6xNavInfo.Node;
                    }
                    else
                    {
                        tnNode = s6xNICateg.FindElement(s6xTable.UniqueAddress);
                        if (tnNode == null)
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xTable.UniqueAddress;
                        }
                        tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xTable.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xTable.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xTable.Category3) };
                    }
                    tnNode.Text = s6xTable.Label;
                    tnNode.ToolTipText = s6xTable.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xTable.IdentificationStatus);

                    s6xTable = null;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    resetPropertiesModifiedStatus(elemFunctionPropertiesTabPage);

                    S6xFunction s6xFunction = new S6xFunction();
                    s6xFunction.BankNum = bankNum;
                    s6xFunction.AddressInt = addressInt;
                    if (bDuplicate) s6xFunction.DuplicateNum = ((S6xFunction)sadS6x.slDupFunctions[nextElemS6xNavInfo.Node.Name]).DuplicateNum;
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

                    s6xFunction.InputMin = functionMinInputTextBox.Text;
                    s6xFunction.InputMax = functionMaxInputTextBox.Text;
                    s6xFunction.OutputMin = functionMinOutputTextBox.Text;
                    s6xFunction.OutputMax = functionMaxOutputTextBox.Text;

                    updateSharedDetails(S6xNavHeaderCategory.FUNCTIONS, s6xFunction);

                    if (isAddressChange)
                    {
                        sadS6x.slFunctions.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xFunction.UniqueAddress;
                        tnTmp = null;
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
                        tnNode = nextElemS6xNavInfo.Node;
                    }
                    else
                    {
                        tnNode = s6xNICateg.FindElement(s6xFunction.UniqueAddress);
                        if (tnNode == null)
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xFunction.UniqueAddress;
                        }
                        tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xFunction.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xFunction.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xFunction.Category3) };
                    }
                    tnNode.Text = s6xFunction.Label;
                    tnNode.ToolTipText = s6xFunction.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xFunction.IdentificationStatus);

                    // Indicate an Update
                    //      Reset on Table Properties for scalers
                    s6xNICateg.Node.Tag = true;

                    s6xFunction = null;
                    break;
                case S6xNavHeaderCategory.SCALARS:
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
                    if (bDuplicate) s6xScalar.DuplicateNum = ((S6xScalar)sadS6x.slDupScalars[nextElemS6xNavInfo.Node.Name]).DuplicateNum;
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
                    s6xScalar.InlineComments = scalarInlineCommentsCheckBox.Checked;
                    s6xScalar.Byte = scalarByteCheckBox.Checked;
                    s6xScalar.Signed = scalarSignedCheckBox.Checked;
                    s6xScalar.ScaleExpression = scalarScaleTextBox.Text;
                    s6xScalar.ScalePrecision = (int)scalarScalePrecNumericUpDown.Value;
                    s6xScalar.Units = scalarUnitsTextBox.Text;

                    s6xScalar.Min = scalarMinTextBox.Text;
                    s6xScalar.Max = scalarMaxTextBox.Text;

                    updateSharedDetails(S6xNavHeaderCategory.SCALARS, s6xScalar);

                    if (isAddressChange)
                    {
                        sadS6x.slScalars.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xScalar.UniqueAddress;
                        tnTmp = null;
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
                        tnNode = nextElemS6xNavInfo.Node;
                    }
                    else
                    {
                        tnNode = s6xNICateg.FindElement(s6xScalar.UniqueAddress);
                        if (tnNode == null)
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xScalar.UniqueAddress;
                        }
                        tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xScalar.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xScalar.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xScalar.Category3) };
                    }
                    tnNode.Text = s6xScalar.Label;
                    tnNode.ToolTipText = s6xScalar.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xScalar.IdentificationStatus);

                    s6xScalar = null;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    resetPropertiesModifiedStatus(elemStructurePropertiesTabPage);

                    S6xStructure s6xStructure = new S6xStructure();
                    s6xStructure.BankNum = bankNum;
                    s6xStructure.AddressInt = addressInt;
                    if (bDuplicate) s6xStructure.DuplicateNum = ((S6xStructure)sadS6x.slDupStructures[nextElemS6xNavInfo.Node.Name]).DuplicateNum;
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

                    updateSharedDetails(S6xNavHeaderCategory.STRUCTURES, s6xStructure);

                    if (isAddressChange)
                    {
                        sadS6x.slStructures.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xStructure.UniqueAddress;
                        tnTmp = null;
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
                        tnNode = nextElemS6xNavInfo.Node;
                    }
                    else
                    {
                        tnNode = s6xNICateg.FindElement(s6xStructure.UniqueAddress);
                        if (tnNode == null)
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = s6xStructure.UniqueAddress;
                        }
                        tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xStructure.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xStructure.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xStructure.Category3) };
                    }
                    tnNode.Text = s6xStructure.Label;
                    tnNode.ToolTipText = s6xStructure.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xStructure.IdentificationStatus);

                    s6xStructure = null;
                    break;
                case S6xNavHeaderCategory.ROUTINES:
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

                    updateSharedDetails(S6xNavHeaderCategory.ROUTINES, s6xRoutine);

                    if (isAddressChange)
                    {
                        sadS6x.slRoutines.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xRoutine.UniqueAddress;
                        tnTmp = null;
                    }

                    if (sadS6x.slRoutines.ContainsKey(s6xRoutine.UniqueAddress)) sadS6x.slRoutines[s6xRoutine.UniqueAddress] = s6xRoutine;
                    else sadS6x.slRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);

                    sadS6x.isSaved = false;

                    tnNode = s6xNICateg.FindElement(s6xRoutine.UniqueAddress);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xRoutine.UniqueAddress;
                    }
                    tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xRoutine.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xRoutine.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xRoutine.Category3) };
                    tnNode.Text = s6xRoutine.Label;
                    tnNode.ToolTipText = s6xRoutine.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xRoutine.IdentificationStatus);

                    s6xRoutine = null;
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    resetPropertiesModifiedStatus(elemOpeTabPage);

                    S6xOperation s6xOpe = new S6xOperation();
                    s6xOpe.BankNum = bankNum;
                    s6xOpe.AddressInt = addressInt;
                    s6xOpe.Label = opeLabelTextBox.Text;
                    s6xOpe.ShortLabel = opeSLabelTextBox.Text;
                    s6xOpe.Comments = opeCommentsTextBox.Text;
                    s6xOpe.OutputComments = opeOutputCommentsCheckBox.Checked;
                    s6xOpe.InlineComments = opeInlineCommentsCheckBox.Checked;
                    s6xOpe.Skip = opeSkipCheckBox.Checked;

                    updateSharedDetails(S6xNavHeaderCategory.OPERATIONS, s6xOpe);

                    if (isAddressChange)
                    {
                        sadS6x.slOperations.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xOpe.UniqueAddress;
                        tnTmp = null;
                    }

                    if (sadS6x.slOperations.ContainsKey(s6xOpe.UniqueAddress)) sadS6x.slOperations[s6xOpe.UniqueAddress] = s6xOpe;
                    else sadS6x.slOperations.Add(s6xOpe.UniqueAddress, s6xOpe);

                    sadS6x.isSaved = false;

                    tnNode = s6xNICateg.FindElement(s6xOpe.UniqueAddress);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xOpe.UniqueAddress;
                    }
                    tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xOpe.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xOpe.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xOpe.Category3) };
                    tnNode.Text = s6xOpe.Label;
                    tnNode.ToolTipText = s6xOpe.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xOpe.IdentificationStatus);

                    s6xOpe = null;
                    break;
                case S6xNavHeaderCategory.REGISTERS:
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
                        s6xReg.AdditionalAddress10 = string.Empty;
                    }

                    s6xReg.isRBase = regRBaseCheckBox.Checked;
                    s6xReg.isRConst = regRConstCheckBox.Checked;
                    if (regConstValueTextBox.Text == string.Empty) s6xReg.ConstValue = null;
                    else s6xReg.ConstValue = regConstValueTextBox.Text;

                    s6xReg.Label = regLabelTextBox.Text;
                    s6xReg.ByteLabel = regByteLabelTextBox.Text;
                    s6xReg.WordLabel = regWordLabelTextBox.Text;
                    s6xReg.ScaleExpression = regScaleTextBox.Text;
                    s6xReg.ScalePrecision = (int)regScalePrecNumericUpDown.Value;
                    s6xReg.Units = regUnitsTextBox.Text;
                    s6xReg.SizeStatus = regSizeComboBox.SelectedItem.ToString();
                    s6xReg.SignedStatus = regSignedComboBox.SelectedItem.ToString();
                    s6xReg.Comments = regCommentsTextBox.Text;
                    s6xReg.Store = true;
                    s6xReg.Skip = regSkipCheckBox.Checked;

                    updateSharedDetails(S6xNavHeaderCategory.REGISTERS, s6xReg);

                    if (isAddressChange)
                    {
                        sadS6x.slRegisters.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xReg.UniqueAddress;
                        tnTmp = null;
                    }

                    if (sadS6x.slRegisters.ContainsKey(s6xReg.UniqueAddress)) sadS6x.slRegisters[s6xReg.UniqueAddress] = s6xReg;
                    else sadS6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);

                    sadS6x.isSaved = false;

                    tnNode = s6xNICateg.FindElement(s6xReg.UniqueAddress);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xReg.UniqueAddress;
                    }
                    tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xReg.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xReg.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xReg.Category3) };
                    tnNode.Text = s6xReg.FullLabel;
                    tnNode.ToolTipText = s6xReg.FullComments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xReg.IdentificationStatus);

                    s6xReg = null;
                    break;
                case S6xNavHeaderCategory.OTHER:
                    resetPropertiesModifiedStatus(elemOtherTabPage);

                    S6xOtherAddress s6xOther = new S6xOtherAddress();
                    s6xOther.BankNum = bankNum;
                    s6xOther.AddressInt = addressInt;
                    s6xOther.Label = otherLabelTextBox.Text;
                    s6xOther.OutputLabel = otherOutputLabelCheckBox.Checked;
                    s6xOther.Comments = otherCommentsTextBox.Text;
                    s6xOther.OutputComments = otherOutputCommentsCheckBox.Checked;
                    s6xOther.InlineComments = otherInlineCommentsCheckBox.Checked;
                    s6xOther.Skip = otherSkipCheckBox.Checked;

                    updateSharedDetails(S6xNavHeaderCategory.OTHER, s6xOther);

                    if (isAddressChange)
                    {
                        sadS6x.slOtherAddresses.Remove(uniqueAddressOri);
                        tnTmp = s6xNICateg.FindElement(uniqueAddressOri);
                        if (tnTmp != null) tnTmp.Name = s6xOther.UniqueAddress;
                        tnTmp = null;
                    }

                    if (sadS6x.slOtherAddresses.ContainsKey(s6xOther.UniqueAddress)) sadS6x.slOtherAddresses[s6xOther.UniqueAddress] = s6xOther;
                    else sadS6x.slOtherAddresses.Add(s6xOther.UniqueAddress, s6xOther);

                    sadS6x.isSaved = false;

                    tnNode = s6xNICateg.FindElement(s6xOther.UniqueAddress);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xOther.UniqueAddress;
                    }
                    tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xOther.Category), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xOther.Category2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xOther.Category3) };
                    tnNode.Text = s6xOther.Label;
                    tnNode.ToolTipText = s6xOther.Comments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xOther.IdentificationStatus);

                    s6xOther = null;
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
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

                    s6xSig.SignatureLabel = signatureLabelTextBox.Text;
                    s6xSig.SignatureComments = signatureCommentsTextBox.Text;
                    s6xSig.Signature = signatureSigTextBox.Text;
                    s6xSig.Skip = signatureSkipCheckBox.Checked;
                    s6xSig.Forced = signatureForcedCheckBox.Checked;

                    updateSharedDetails(S6xNavHeaderCategory.SIGNATURES, s6xSig);

                    if (sadS6x.slSignatures.ContainsKey(s6xSig.UniqueKey)) sadS6x.slSignatures[s6xSig.UniqueKey] = s6xSig;
                    else sadS6x.slSignatures.Add(s6xSig.UniqueKey, s6xSig);

                    sadS6x.isSaved = false;

                    tnNode = s6xNICateg.FindElement(uniqueAddressOri);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xSig.UniqueKey;
                    }
                    tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xSig.SignatureCategory), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xSig.SignatureCategory2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xSig.SignatureCategory3) };
                    tnNode.Text = s6xSig.SignatureLabel;
                    tnNode.ToolTipText = s6xSig.SignatureComments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xSig.IdentificationStatus);

                    s6xSig = null;
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
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

                    updateSharedDetails(S6xNavHeaderCategory.ELEMSSIGNATURES, s6xESig);

                    if (sadS6x.slElementsSignatures.ContainsKey(s6xESig.UniqueKey)) sadS6x.slElementsSignatures[s6xESig.UniqueKey] = s6xESig;
                    else sadS6x.slElementsSignatures.Add(s6xESig.UniqueKey, s6xESig);

                    sadS6x.isSaved = false;

                    tnNode = s6xNICateg.FindElement(uniqueAddressOri);
                    if (tnNode == null)
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = s6xESig.UniqueKey;
                    }
                    tnNode.Tag = new S6xNavCategory[] { s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, s6xESig.SignatureCategory), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, s6xESig.SignatureCategory2), s6xNavCategories.getCategory(s6xNICateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, s6xESig.SignatureCategory3) };
                    tnNode.Text = s6xESig.SignatureLabel;
                    tnNode.ToolTipText = s6xESig.SignatureComments;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xESig.IdentificationStatus);

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
                    S6xNavCategory navCateg1 = null;
                    S6xNavCategory navCateg2 = null;
                    S6xNavCategory navCateg3 = null;
                    if (tnNode.Tag != null)
                    {
                        if (tnNode.Tag.GetType() == typeof(S6xNavCategory[]))
                        {
                            S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();
                            if (((S6xNavCategory[])tnNode.Tag).Length >= 1) navCateg1 = ((S6xNavCategory[])tnNode.Tag)[0];
                            if (((S6xNavCategory[])tnNode.Tag).Length >= 2) navCateg2 = ((S6xNavCategory[])tnNode.Tag)[1];
                            if (((S6xNavCategory[])tnNode.Tag).Length >= 3) navCateg3 = ((S6xNavCategory[])tnNode.Tag)[2];
                            if (s6xNICateg.FindElement(tnNode.Name) == null)
                            // Node has to be added
                            {
                                s6xNICateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);
                                setElementsTreeCategLabel(s6xNICateg.HeaderCategory);
                            }
                            else
                            // Node has to be moved
                            {
                                S6xNavInfo niNI = new S6xNavInfo(tnNode);
                                if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                                {
                                    // 20211109 To prevent non necessary reload or history
                                    nextElemS6xNavInfo = null;
                                    elemsTreeView.SelectedNode = null;

                                    tnNode.Parent.Nodes.Remove(tnNode);
                                    s6xNICateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);

                                    lastElemS6xNavInfo = null;
                                    nextElemS6xNavInfo = new S6xNavInfo(tnNode);

                                    // 20211109 To reselect rigth node
                                    elemsTreeView.SelectedNode = tnNode;
                                }
                                niNI = null;
                            }
                        }
                        tnNode.Tag = null;
                    }
                }

                lastElemS6xNavInfo = null;
                nextElemS6xNavInfo = new S6xNavInfo(tnNode);

                //disassemblyToolStripMenuItem.Enabled = false;
                //outputToolStripMenuItem.Enabled = false;

                showElem(elemTabControl.SelectedTab == sharedDetailsTabPage);
            }

            s6xNICateg = null;
            tnNode = null;
        }
        
        private void renameElem(S6xNavInfo navInfo, string label)
        {
            if (navInfo == null) return;
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    tableLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    functionLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    scalarLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    structureLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    routineLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    opeLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    signatureLabelTextBox.Text = label;
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    elementSignatureLabelTextBox.Text = label;
                    break;
            }

            elemsTreeView.Tag = new S6xNavInfo(navInfo.HeaderCategoryNode);
            
            updateElem();
        }

        private void deleteElem(S6xNavInfo navInfo, bool forceDeletion, bool keepNode, bool removeScalersOnTables)
        {
            S6xNavHeaderCategory headerCateg = S6xNavHeaderCategory.UNDEFINED;
            bool deleteNode = true;
            bool bDuplicate = false;

            if (navInfo == null) return;
            if (!navInfo.isValid) return;

            if (navInfo.MainNode == null) return;

            bDuplicate = navInfo.isDuplicate;
            headerCateg = navInfo.HeaderCategory;
            switch (headerCateg)
            {
                case S6xNavHeaderCategory.TABLES:
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupTables.ContainsKey(navInfo.Node.Name))
                        {
                            sadS6x.slDupTables.Remove(navInfo.Node.Name);
                            sadS6x.isSaved = false;
                        }
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(navInfo.Node.Name))
                        {
                            if (!forceDeletion && !((S6xTable)sadS6x.slTables[navInfo.Node.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(navInfo.Node.Name))
                                    {
                                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[navInfo.Node.Name]).isTable)
                                        {
                                            deleteNode = false;
                                            sadS6x.slTables[navInfo.Node.Name] = new S6xTable((CalibrationElement)sadBin.Calibration.slCalibrationElements[navInfo.Node.Name]);
                                        }
                                    }
                                    else if (sadBin.Calibration.slExtTables.ContainsKey(navInfo.Node.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slTables[navInfo.Node.Name] = new S6xTable((Table)sadBin.Calibration.slExtTables[navInfo.Node.Name], sadBin.getBankBinAddress(((S6xTable)sadS6x.slTables[navInfo.Node.Name]).BankNum));
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slTables.Remove(navInfo.Node.Name);
                            }
                            else
                            {
                                navInfo.Node.Text = ((S6xTable)sadS6x.slTables[navInfo.Node.Name]).Label;
                                navInfo.Node.ToolTipText = ((S6xTable)sadS6x.slTables[navInfo.Node.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                        }
                    }
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupFunctions.ContainsKey(navInfo.Node.Name))
                        {
                            // Scalers Cleaning on Tables, by using main Item
                            if (removeScalersOnTables)
                            {
                                foreach (S6xTable s6xTable in sadS6x.slTables.Values)
                                {
                                    if (s6xTable.ColsScalerAddress == navInfo.Node.Name || s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                    {
                                        if (s6xTable.ColsScalerAddress == navInfo.Node.Name)
                                        {
                                            s6xTable.ColsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[navInfo.Node.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                                        }
                                        if (s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                        {
                                            s6xTable.RowsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[navInfo.Node.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                                        }
                                    }
                                }
                                foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
                                {
                                    if (s6xTable.ColsScalerAddress == navInfo.Node.Name || s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                    {
                                        if (s6xTable.ColsScalerAddress == navInfo.Node.Name)
                                        {
                                            s6xTable.ColsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[navInfo.Node.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                                        }
                                        if (s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                        {
                                            s6xTable.RowsScalerAddress = ((S6xFunction)sadS6x.slDupFunctions[navInfo.Node.Name]).UniqueAddress;
                                            if (sadS6x.slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)sadS6x.slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                                        }
                                    }
                                }
                            }

                            sadS6x.slDupFunctions.Remove(navInfo.Node.Name);
                            sadS6x.isSaved = false;
                            // For Scalers Refresh
                            navInfo.HeaderCategoryNode.Tag = true;
                        }
                    }
                    else
                    {
                        if (sadS6x.slFunctions.ContainsKey(navInfo.Node.Name))
                        {
                            if (!forceDeletion && !((S6xFunction)sadS6x.slFunctions[navInfo.Node.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(navInfo.Node.Name))
                                    {
                                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[navInfo.Node.Name]).isFunction)
                                        {
                                            deleteNode = false;
                                            sadS6x.slFunctions[navInfo.Node.Name] = new S6xFunction((CalibrationElement)sadBin.Calibration.slCalibrationElements[navInfo.Node.Name]);
                                        }
                                    }
                                    else if (sadBin.Calibration.slExtFunctions.ContainsKey(navInfo.Node.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slFunctions[navInfo.Node.Name] = new S6xFunction((Function)sadBin.Calibration.slExtTables[navInfo.Node.Name], sadBin.getBankBinAddress(((S6xFunction)sadS6x.slFunctions[navInfo.Node.Name]).BankNum));
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slFunctions.Remove(navInfo.Node.Name);
                                // Scalers Cleaning on Tables
                                if (removeScalersOnTables)
                                {
                                    foreach (S6xTable s6xTable in sadS6x.slTables.Values)
                                    {
                                        if (s6xTable.ColsScalerAddress == navInfo.Node.Name || s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                        {
                                            if (s6xTable.ColsScalerAddress == navInfo.Node.Name)
                                            {
                                                s6xTable.ColsScalerAddress = string.Empty;
                                                s6xTable.ColsScalerXdfUniqueId = string.Empty;
                                            }
                                            if (s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                            {
                                                s6xTable.RowsScalerAddress = string.Empty;
                                                s6xTable.RowsScalerXdfUniqueId = string.Empty;
                                            }
                                        }
                                    }
                                    foreach (S6xTable s6xTable in sadS6x.slDupTables.Values)
                                    {
                                        if (s6xTable.ColsScalerAddress == navInfo.Node.Name || s6xTable.RowsScalerAddress == navInfo.Node.Name)
                                        {
                                            if (s6xTable.ColsScalerAddress == navInfo.Node.Name)
                                            {
                                                s6xTable.ColsScalerAddress = string.Empty;
                                                s6xTable.ColsScalerXdfUniqueId = string.Empty;
                                            }
                                            if (s6xTable.RowsScalerAddress == navInfo.Node.Name)
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
                                navInfo.Node.Text = ((S6xFunction)sadS6x.slFunctions[navInfo.Node.Name]).Label;
                                navInfo.Node.ToolTipText = ((S6xFunction)sadS6x.slFunctions[navInfo.Node.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                            // For Scalers Refresh
                            navInfo.HeaderCategoryNode.Tag = true;
                        }
                    }
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupScalars.ContainsKey(navInfo.Node.Name))
                        {
                            sadS6x.slDupScalars.Remove(navInfo.Node.Name);
                            sadS6x.isSaved = false;
                        }
                    }
                    else
                    {
                        if (sadS6x.slScalars.ContainsKey(navInfo.Node.Name))
                        {
                            if (!forceDeletion && !((S6xScalar)sadS6x.slScalars[navInfo.Node.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slCalibrationElements.ContainsKey(navInfo.Node.Name))
                                    {
                                        if (((CalibrationElement)sadBin.Calibration.slCalibrationElements[navInfo.Node.Name]).isScalar)
                                        {
                                            deleteNode = false;
                                            sadS6x.slScalars[navInfo.Node.Name] = new S6xScalar((CalibrationElement)sadBin.Calibration.slCalibrationElements[navInfo.Node.Name]);
                                        }
                                    }
                                    else if (sadBin.Calibration.slExtScalars.ContainsKey(navInfo.Node.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slScalars[navInfo.Node.Name] = new S6xScalar((Scalar)sadBin.Calibration.slExtScalars[navInfo.Node.Name], sadBin.getBankBinAddress(((S6xScalar)sadS6x.slScalars[navInfo.Node.Name]).BankNum));
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slScalars.Remove(navInfo.Node.Name);
                            }
                            else
                            {
                                navInfo.Node.Text = ((S6xScalar)sadS6x.slScalars[navInfo.Node.Name]).Label;
                                navInfo.Node.ToolTipText = ((S6xScalar)sadS6x.slScalars[navInfo.Node.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                        }
                    }
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    if (bDuplicate)
                    {
                        if (sadS6x.slDupStructures.ContainsKey(navInfo.Node.Name))
                        {
                            sadS6x.slDupStructures.Remove(navInfo.Node.Name);
                            sadS6x.isSaved = false;
                        }
                    }
                    else
                    {
                        if (sadS6x.slStructures.ContainsKey(navInfo.Node.Name))
                        {
                            if (!forceDeletion && !((S6xStructure)sadS6x.slStructures[navInfo.Node.Name]).Store)
                            {
                                if (sadBin != null)
                                {
                                    if (sadBin.Calibration.slExtStructures.ContainsKey(navInfo.Node.Name))
                                    {
                                        deleteNode = false;
                                        sadS6x.slStructures[navInfo.Node.Name] = new S6xStructure((Structure)sadBin.Calibration.slExtStructures[navInfo.Node.Name]);
                                    }
                                }
                            }
                            if (deleteNode)
                            {
                                sadS6x.slStructures.Remove(navInfo.Node.Name);
                            }
                            else
                            {
                                navInfo.Node.Text = ((S6xStructure)sadS6x.slStructures[navInfo.Node.Name]).Label;
                                navInfo.Node.ToolTipText = ((S6xStructure)sadS6x.slStructures[navInfo.Node.Name]).Comments;
                            }
                            sadS6x.isSaved = false;
                        }
                    }
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    if (sadS6x.slRoutines.ContainsKey(navInfo.Node.Name))
                    {
                        if (!forceDeletion && !((S6xRoutine)sadS6x.slRoutines[navInfo.Node.Name]).Store)
                        {
                            if (sadBin != null)
                            {
                                if (sadBin.Calibration.slCalls.ContainsKey(navInfo.Node.Name))
                                {
                                    deleteNode = false;

                                    Routine rRoutine = null;
                                    if (((Call)sadBin.Calibration.slCalls[navInfo.Node.Name]).isRoutine) rRoutine = (Routine)sadBin.Calibration.slRoutines[navInfo.Node.Name];

                                    sadS6x.slRoutines[navInfo.Node.Name] = new S6xRoutine((Call)sadBin.Calibration.slCalls[navInfo.Node.Name], rRoutine);

                                    rRoutine = null;
                                }
                            }
                        }
                        if (deleteNode)
                        {
                            sadS6x.slRoutines.Remove(navInfo.Node.Name);
                        }
                        else
                        {
                            navInfo.Node.Text = ((S6xRoutine)sadS6x.slRoutines[navInfo.Node.Name]).Label;
                            navInfo.Node.ToolTipText = ((S6xRoutine)sadS6x.slRoutines[navInfo.Node.Name]).Comments;
                        }
                        sadS6x.isSaved = false;
                    }
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    if (sadS6x.slOperations.ContainsKey(navInfo.Node.Name)) sadS6x.slOperations.Remove(navInfo.Node.Name);
                    sadS6x.isSaved = false;
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    if (sadS6x.slRegisters.ContainsKey(navInfo.Node.Name))
                    {
                        if (((S6xRegister)sadS6x.slRegisters[navInfo.Node.Name]).AutoConstValue) deleteNode = false;
                    }
                    if (deleteNode)
                    {
                        sadS6x.slRegisters.Remove(navInfo.Node.Name);
                    }
                    else
                    {
                        navInfo.Node.Text = ((S6xRegister)sadS6x.slRegisters[navInfo.Node.Name]).FullLabel;
                        navInfo.Node.ToolTipText = ((S6xRegister)sadS6x.slRegisters[navInfo.Node.Name]).FullComments;
                    }
                    sadS6x.isSaved = false;
                    break;
                case S6xNavHeaderCategory.OTHER:
                    if (sadS6x.slOtherAddresses.ContainsKey(navInfo.Node.Name)) sadS6x.slOtherAddresses.Remove(navInfo.Node.Name);
                    sadS6x.isSaved = false;
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    if (sadS6x.slSignatures.ContainsKey(navInfo.Node.Name))
                    {
                        if (((S6xSignature)sadS6x.slSignatures[navInfo.Node.Name]).Forced) deleteNode = false;
                    }
                    if (deleteNode)
                    {
                        sadS6x.slSignatures.Remove(navInfo.Node.Name);
                    }
                    else
                    {
                        if (((S6xSignature)sadS6x.slSignatures[navInfo.Node.Name]).SignatureLabel == null || ((S6xSignature)sadS6x.slSignatures[navInfo.Node.Name]).SignatureLabel == string.Empty)
                        {
                            navInfo.Node.Text = SADDef.LongSignaturePrefix + ((S6xSignature)sadS6x.slSignatures[navInfo.Node.Name]).Label;
                        }
                        else
                        {
                            navInfo.Node.Text = ((S6xSignature)sadS6x.slSignatures[navInfo.Node.Name]).SignatureLabel;
                        }
                        navInfo.Node.ToolTipText = ((S6xSignature)sadS6x.slSignatures[navInfo.Node.Name]).SignatureComments;
                    }
                    sadS6x.isSaved = false;
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    if (sadS6x.slElementsSignatures.ContainsKey(navInfo.Node.Name))
                    {
                        if (((S6xElementSignature)sadS6x.slElementsSignatures[navInfo.Node.Name]).Forced) deleteNode = false;
                    }
                    if (deleteNode)
                    {
                        sadS6x.slElementsSignatures.Remove(navInfo.Node.Name);
                    }
                    else
                    {
                        navInfo.Node.Text = ((S6xElementSignature)sadS6x.slElementsSignatures[navInfo.Node.Name]).SignatureLabel;
                        navInfo.Node.ToolTipText = ((S6xElementSignature)sadS6x.slElementsSignatures[navInfo.Node.Name]).SignatureComments;
                    }
                    sadS6x.isSaved = false;
                    break;
            }

            if (deleteNode && alClipBoardTempUniqueAddresses.Contains(navInfo.Node.Name)) alClipBoardTempUniqueAddresses.Remove(navInfo.Node.Name);

            if (keepNode) return;

            if (deleteNode)
            {
                nextElemS6xNavInfo = null;
                elemPanel.Visible = false;

                navInfo.Node.Parent.Nodes.Remove(navInfo.Node);
                setElementsTreeCategLabel(headerCateg);
            }
            else
            {
                nextElemS6xNavInfo = navInfo;

                navInfo.Node.ForeColor = navInfo.Node.Parent.ForeColor;

                showElem(false);
            }
        }

        private void tableColsScalerLabel_Click(object sender, EventArgs e)
        {
            if (tableColsScalerButton.Tag != null)
            {
                S6xNavInfo niCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                TreeNode tnNode = niCateg.FindElement(((S6xFunction)tableColsScalerButton.Tag).UniqueAddress);
                if (tnNode != null)
                {
                    if (((S6xFunction)tableColsScalerButton.Tag).DuplicateNum == 0)
                    {
                        //elemBackButton.Tag = nextElemS6xNavInfo;
                        elemsTreeView.SelectedNode = tnNode;
                    }
                    else
                    {
                        tnNode = tnNode.Nodes[((S6xFunction)tableColsScalerButton.Tag).DuplicateAddress];
                        if (tnNode != null)
                        {
                            //elemBackButton.Tag = nextElemS6xNavInfo;
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
                S6xNavInfo niCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                TreeNode tnNode = niCateg.FindElement(((S6xFunction)tableRowsScalerButton.Tag).UniqueAddress);
                if (tnNode != null)
                {
                    if (((S6xFunction)tableRowsScalerButton.Tag).DuplicateNum == 0)
                    {
                        //elemBackButton.Tag = nextElemS6xNavInfo;
                        elemsTreeView.SelectedNode = tnNode;
                    }
                    else
                    {
                        tnNode = tnNode.Nodes[((S6xFunction)tableRowsScalerButton.Tag).DuplicateAddress];
                        if (tnNode != null)
                        {
                            //elemBackButton.Tag = nextElemS6xNavInfo;
                            elemsTreeView.SelectedNode = tnNode;
                        }
                    }
                }
                tnNode = null;
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

        private void elemValidateButton_Click(object sender, EventArgs e)
        {
            if (elemsTreeView.Tag == null) updateProperties();
            else updateElem();
        }

        private void elemCancelButton_Click(object sender, EventArgs e)
        {
            if (elemTabControl.TabPages.Count == 0) return;

            if (elemsTreeView.Tag == null)
            {
                if (elemTabControl.SelectedTab == s6xPropertiesTabPage)
                {
                    dirtyProperties = false;
                    showProperties(false);
                }
                else if (elemTabControl.SelectedTab == sharedDetailsTabPage && elemTabControl.TabPages[0] == s6xPropertiesTabPage)
                {
                    dirtyProperties = false;
                    showProperties(true);
                }
                return;
            }

            if (nextElemS6xNavInfo != null)
            {
                showElemProperties(elemTabControl.SelectedTab == sharedDetailsTabPage);
            }
            else if (elemTabControl.SelectedTab != elemInfoTabPage && elemTabControl.SelectedTab != elemOpsTabPage)
            {
                elemsTreeView.Tag = null;
                elemPanel.Visible = false;
                resetPropertiesModifiedStatus(elemTabControl.SelectedTab);
            }
        }

        private void elemBackButton_Click(object sender, EventArgs e)
        {
            List<S6xNavInfo> backNInfos = (List<S6xNavInfo>)elemBackButton.Tag;
            if (backNInfos == null) return;
            if (backNInfos.Count <= 1) return;
            if (backNInfos.Count == 2)
            {
                TreeNode tnNode = backNInfos[1].Node;
                elemBackButton.Tag = null;
                elemBackUpdate();
                elemsTreeView.SelectedNode = tnNode;
                return;
            }

            elemBackContextMenuStrip.Show(Cursor.Position);
        }

        private void elemBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<S6xNavInfo> backNInfos = (List<S6xNavInfo>)elemBackButton.Tag;
            if (backNInfos == null) return;
            if (backNInfos.Count <= 1) return;

            // Removing current Node
            backNInfos.RemoveAt(0);

            S6xNavInfo navInfo = (S6xNavInfo)((ToolStripMenuItem)sender).Tag;
            if (!backNInfos.Contains(navInfo))
            {
                elemBackUpdate();
                return;
            }

            // Removing Node to be opened
            backNInfos.Remove(navInfo);
            if (backNInfos.Count == 0) backNInfos = null;
            elemBackButton.Tag = backNInfos;

            elemBackUpdate();

            elemsTreeView.SelectedNode = navInfo.Node;
        }

        private void elemBackPopulate(S6xNavInfo nInfo)
        {
            List<S6xNavInfo> backNInfos = (List<S6xNavInfo>)elemBackButton.Tag;
            if (backNInfos == null) backNInfos = new List<S6xNavInfo>();
            if (backNInfos.Count == 0)
            {
                backNInfos.Add(nInfo);
                elemBackButton.Tag = backNInfos;
                elemBackUpdate();
                return;
            }
            if (backNInfos[0] == nInfo) return;

            if (backNInfos.Contains(nInfo)) backNInfos.Remove(nInfo);
            backNInfos.Insert(0, nInfo);
            if (backNInfos.Count > 20) backNInfos.RemoveAt(20);
            elemBackButton.Tag = backNInfos;
            elemBackUpdate();
        }

        private void elemBackUpdate()
        {
            elemBackContextMenuStrip.Items.Clear();

            List<S6xNavInfo> backNInfos = (List<S6xNavInfo>)elemBackButton.Tag;
            if (backNInfos == null)
            {
                elemBackButton.Visible = false;
                return;
            }
            if (backNInfos.Count <= 1)
            {
                elemBackButton.Visible = false;
                return;
            }

            elemBackButton.Visible = true;
            for (int iNInfo = 1; iNInfo < backNInfos.Count; iNInfo++)
            {
                S6xNavInfo nInfo = backNInfos[iNInfo];
                ToolStripMenuItem tsMI = new ToolStripMenuItem();
                tsMI.Tag = nInfo;
                tsMI.Text = nInfo.Node.Text;
                tsMI.ToolTipText = nInfo.Node.ToolTipText;
                tsMI.Click += new EventHandler(elemBackToolStripMenuItem_Click);
                elemBackContextMenuStrip.Items.Add(tsMI);
                // Limited to 20
                if (elemBackContextMenuStrip.Items.Count >= 20) break;
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
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            bool bDuplicate = navInfo.isDuplicate;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, bDuplicate ? sadS6x.slDupTables[navInfo.Node.Name] : sadS6x.slTables[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, bDuplicate ? sadS6x.slDupFunctions[navInfo.Node.Name] : sadS6x.slFunctions[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, bDuplicate ? sadS6x.slDupScalars[navInfo.Node.Name] : sadS6x.slScalars[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, ((S6xStructure)(bDuplicate ? sadS6x.slDupStructures[navInfo.Node.Name] : sadS6x.slStructures[navInfo.Node.Name])).ClipBoardClone());
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slRoutines[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slOperations[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slRegisters[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.OTHER:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slOtherAddresses[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slSignatures[navInfo.Node.Name]);
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    Clipboard.SetData(SADDef.S6xClipboardFormat, sadS6x.slElementsSignatures[navInfo.Node.Name]);
                    break;
            }
            navInfo = null;
        }

        private void copySigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            S6xRoutine s6xRoutine = null;
            S6xSignature s6xSignature = null;
            Operation[] ops = null;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.ROUTINES:
                    s6xRoutine = (S6xRoutine)sadS6x.slRoutines[navInfo.Node.Name];
                    break;
                default:
                    return;
            }

            navInfo = null;

            s6xSignature = new S6xSignature();
            
            s6xSignature.Label = s6xRoutine.Label;
            s6xSignature.ShortLabel = s6xRoutine.ShortLabel;
            s6xSignature.Comments = s6xRoutine.Comments;
            s6xSignature.OutputComments = s6xRoutine.OutputComments;

            s6xSignature.SignatureLabel = SADDef.LongSignaturePrefix + s6xSignature.Label;

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
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            object xdfObject = null;
            XdfHeaderCategory[] defaultElementHeaderCategories = null;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    ReservedAddress resAdr = null;    
                    switch (Convert.ToInt32(navInfo.Node.Name.Substring(0, 1)))
                    {
                        case 8:
                            if (sadBin.Bank8 != null) resAdr = (ReservedAddress)sadBin.Bank8.slReserved[navInfo.Node.Name];
                            break;
                        case 1:
                            if (sadBin.Bank1 != null) resAdr = (ReservedAddress)sadBin.Bank1.slReserved[navInfo.Node.Name];
                            break;
                        case 9:
                            if (sadBin.Bank9 != null) resAdr = (ReservedAddress)sadBin.Bank9.slReserved[navInfo.Node.Name];
                            break;
                        case 0:
                            if (sadBin.Bank0 != null) resAdr = (ReservedAddress)sadBin.Bank0.slReserved[navInfo.Node.Name];
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
                                    if (resAdr.Type == ReservedAddressType.Ascii) xdfObject = new XdfTable(resAdr, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
                                    else xdfObject = new XdfScalar(resAdr, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
                                    break;
                                default:
                                    xdfObject = new XdfTable(resAdr, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
                                    break;
                            }
                        }
                        resAdr = null;
                    }
                    break;
                case S6xNavHeaderCategory.TABLES:
                    // Trying to find Scalers Unique Xdf Ids / Useful only after import or synchronized export
                    S6xTable s6xTable = null;
                    if (navInfo.isDuplicate) s6xTable = (S6xTable)sadS6x.slDupTables[navInfo.Node.Name];
                    else s6xTable = (S6xTable)sadS6x.slTables[navInfo.Node.Name];

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
                        defaultElementHeaderCategories = Tools.XDFDefaultElementHeaderCategories(new string[] { s6xTable.Category, s6xTable.Category2, s6xTable.Category3 });
                        xdfObject = new XdfTable(s6xTable, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
                    }
                    s6xTable = null;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    S6xFunction s6xFunction = null;
                    if (navInfo.isDuplicate) s6xFunction = (S6xFunction)sadS6x.slDupFunctions[navInfo.Node.Name];
                    else s6xFunction = (S6xFunction)sadS6x.slFunctions[navInfo.Node.Name];
                    if (s6xFunction.AddressBinInt >= sadS6x.Properties.XdfBaseOffsetInt)
                    {
                        defaultElementHeaderCategories = Tools.XDFDefaultElementHeaderCategories(new string[] { s6xFunction.Category, s6xFunction.Category2, s6xFunction.Category3 });
                        xdfObject = new XdfFunction(s6xFunction, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
                    }
                    s6xFunction = null;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    S6xScalar s6xScalar = null;
                    if (navInfo.isDuplicate) s6xScalar = (S6xScalar)sadS6x.slDupScalars[navInfo.Node.Name];
                    else s6xScalar = (S6xScalar)sadS6x.slScalars[navInfo.Node.Name];
                    if (s6xScalar.AddressBinInt >= sadS6x.Properties.XdfBaseOffsetInt)
                    {
                        defaultElementHeaderCategories = Tools.XDFDefaultElementHeaderCategories(new string[] { s6xScalar.Category, s6xScalar.Category2, s6xScalar.Category3 });
                        if (s6xScalar.isBitFlags)
                        {
                            if (MessageBox.Show("Scalar contains Bit Flags. Bit Flags should be copied one by one.\r\nCopy Scalar to paste Xdf Scalar ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                xdfObject = new XdfScalar(s6xScalar, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
                            }
                        }
                        else
                        {
                            xdfObject = new XdfScalar(s6xScalar, sadS6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
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

            navInfo = null;
        }

        private object getS6xObjectFromXdfData()
        {
            MemoryStream mStream = null;
            object xdfObject = null;
            int bankNum = -1;
            int address = -1;
            int addressBin = -1;
            bool isCalElem = false;
            XdfHeaderCategory[] defaultElementHeaderCategories = new XdfHeaderCategory[] {};

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
                return new S6xScalar((XdfScalar) xdfObject, bankNum, address, addressBin, isCalElem, defaultElementHeaderCategories);
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
                return new S6xFunction((XdfFunction)xdfObject, bankNum, address, addressBin, isCalElem, defaultElementHeaderCategories);
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
                return new S6xTable((XdfTable)xdfObject, bankNum, address, addressBin, isCalElem, ref sadS6x.slFunctions, defaultElementHeaderCategories);
            }

            xdfObject = ToolsXml.Deserialize(ref mStream, typeof(XdfFlag));
            if (xdfObject != null)
            {
                addressBin = Tools.binAddressFromXdfAddress(((XdfFlag)xdfObject).getMmedAddress(), sadS6x.Properties.XdfBaseOffsetInt);
                bankNum = sadBin.getBankNum(addressBin);
                if (bankNum < 0) return null;
                address = addressBin - sadBin.getBankBinAddress(bankNum);
                isCalElem = (addressBin >= sadBin.Calibration.AddressBinInt && addressBin <= sadBin.Calibration.AddressBinEndInt);
                return new S6xScalar((XdfFlag)xdfObject, bankNum, address, addressBin, isCalElem, defaultElementHeaderCategories);
            }

            return null;
        }

        private void pasteOverItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            string[] originalTableScalersAddresses = null;
            object s6xData = null; 
            bool keepScalersOnTables = false;

            if (Clipboard.ContainsData(SADDef.S6xClipboardFormat)) s6xData = Clipboard.GetData(SADDef.S6xClipboardFormat);
            else if (Clipboard.ContainsData(SADDef.XdfClipboardFormat)) s6xData = getS6xObjectFromXdfData();
            if (s6xData == null) return;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.RESERVED:
                    // Reserved Address - No OverWrite
                    return;
                case S6xNavHeaderCategory.REGISTERS:
                    if (sadS6x.slRegisters.ContainsKey(navInfo.Node.Name))
                    {
                        // Reserved Address, RBase or RConst (AutoDetected) - No OverWrite
                        if (((S6xRegister)sadS6x.slRegisters[navInfo.Node.Name]).AutoConstValue) return;
                    }
                    break;
                case S6xNavHeaderCategory.TABLES:
                    if (navInfo.isDuplicate)
                    {
                        if (sadS6x.slDupTables.ContainsKey(navInfo.Node.Name))
                        {
                            originalTableScalersAddresses = new string[] { ((S6xTable)sadS6x.slDupTables[navInfo.Node.Name]).ColsScalerAddress, ((S6xTable)sadS6x.slDupTables[navInfo.Node.Name]).RowsScalerAddress };
                        }
                    }
                    else
                    {
                        if (sadS6x.slTables.ContainsKey(navInfo.Node.Name))
                        {
                            originalTableScalersAddresses = new string[] { ((S6xTable)sadS6x.slTables[navInfo.Node.Name]).ColsScalerAddress, ((S6xTable)sadS6x.slTables[navInfo.Node.Name]).RowsScalerAddress };
                        }
                    }
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    keepScalersOnTables = (s6xData.GetType() == typeof(S6xFunction));
                    break;
            }

            deleteElem(navInfo, true, true, !keepScalersOnTables);

            pasteElement(navInfo, originalTableScalersAddresses);

            showElem(false);
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
            S6xNavHeaderCategory headerCateg = S6xNavHeaderCategory.UNDEFINED;
            S6xNavCategory navCateg1 = null;
            S6xNavCategory navCateg2 = null;
            S6xNavCategory navCateg3 = null;
            S6xNavInfo niHeaderCateg = null;
            S6xNavInfo niSource = null;
            S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();

            if (nTimes < 1) return;
            if (Clipboard.ContainsData(SADDef.S6xClipboardFormat)) s6xData = Clipboard.GetData(SADDef.S6xClipboardFormat);
            if (s6xData == null) return;

            if (s6xData.GetType() == typeof(S6xScalar)) headerCateg = S6xNavHeaderCategory.SCALARS;

            switch (headerCateg)
            {
                case S6xNavHeaderCategory.SCALARS:
                    // Works only when coming from current session, the original scalar should exist at its address
                    if (!sadS6x.slScalars.ContainsKey(((S6xScalar)s6xData).UniqueAddress)) return;

                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)]);
                    niSource = new S6xNavInfo(niHeaderCateg.FindElement(((S6xScalar)s6xData).UniqueAddress));
                    navCateg1 = niSource.Category;
                    navCateg2 = niSource.Category2;
                    navCateg3 = niSource.Category3;
                    niSource = null;

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

                        newObject.DateCreated = DateTime.UtcNow;
                        newObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.slScalars.Add(newObject.UniqueAddress, newObject);
                        sadS6x.isSaved = false;

                        if (niHeaderCateg.FindElement(newObject.UniqueAddress) != null) continue;

                        TreeNode tnNode = new TreeNode();
                        tnNode.Name = newObject.UniqueAddress;
                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        tnNode.ForeColor = Color.Purple;
                        tnNode.Text = newObject.Label;
                        tnNode.ToolTipText = newObject.Comments;
                        
                        niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);
                    }
                    setElementsTreeCategLabel(headerCateg);
                    break;
                default:
                    return;
            }
        }

        private void pasteElement(S6xNavInfo overNavInfo, string[] originalTableScalersAddresses)
        {
            object s6xData = null;
            S6xNavInfo navInfo = null;
            S6xNavHeaderCategory headerCateg = S6xNavHeaderCategory.UNDEFINED;
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
            S6xNavHeaderCategory overHeaderCateg = S6xNavHeaderCategory.UNDEFINED;
            S6xNavCategory navCateg1 = null;
            S6xNavCategory navCateg2 = null;
            S6xNavCategory navCateg3 = null;

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

            if (overNavInfo != null)
            {
                overHeaderCateg = overNavInfo.HeaderCategory;
                overUniqueAddress = overNavInfo.MainNode.Name;
                if (overNavInfo.isDuplicate)
                {
                    try { overDuplicateNum = Convert.ToInt32(overNavInfo.DuplicateNode.Name.Split(' ')[overNavInfo.DuplicateNode.Name.Split(' ').Length - 1]); }
                    catch { }
                }

                try { overBankNum = Convert.ToInt32(overUniqueAddress.Substring(0, 1)); }
                catch { }
                try { overAddress = Convert.ToInt32(overUniqueAddress.Substring(1).Trim()); }
                catch { }
            }

            if (s6xData == null) return;

            if (s6xData.GetType() == typeof(S6xTable)) headerCateg = S6xNavHeaderCategory.TABLES;
            else if (s6xData.GetType() == typeof(S6xFunction)) headerCateg = S6xNavHeaderCategory.FUNCTIONS;
            else if (s6xData.GetType() == typeof(S6xScalar)) headerCateg = S6xNavHeaderCategory.SCALARS;
            else if (s6xData.GetType() == typeof(S6xStructureClipBoard)) headerCateg = S6xNavHeaderCategory.STRUCTURES;
            else if (s6xData.GetType() == typeof(S6xRoutine)) headerCateg = S6xNavHeaderCategory.ROUTINES;
            else if (s6xData.GetType() == typeof(S6xOperation)) headerCateg = S6xNavHeaderCategory.OPERATIONS;
            else if (s6xData.GetType() == typeof(S6xRegister)) headerCateg = S6xNavHeaderCategory.REGISTERS;
            else if (s6xData.GetType() == typeof(S6xOtherAddress)) headerCateg = S6xNavHeaderCategory.OTHER;
            else if (s6xData.GetType() == typeof(S6xSignature)) headerCateg = S6xNavHeaderCategory.SIGNATURES;
            else if (s6xData.GetType() == typeof(S6xElementSignature)) headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;

            switch (headerCateg)
            {
                case S6xNavHeaderCategory.TABLES:
                    if (isXdfData && overNavInfo == null)
                    {
                        navInfo = checkDuplicate(((S6xTable)s6xData).UniqueAddress, false, null, null);
                        if (navInfo != null)
                        {
                            if (navInfo.Node.Parent == null)
                            // Block Reserved and Calibration Load Elements
                            {
                                navInfo = null;
                                s6xData = null;
                                return;
                            }
                            else if (navInfo.HeaderCategory == S6xNavHeaderCategory.OTHER)
                            // Other Presence
                            {
                                sMessage = "Information is partially defined.\r\n";
                                sMessage += "\t" + navInfo.Node.Text + "\tfound in " + navInfo.HeaderCategoryNode.Text + "\r\n";
                                sMessage += "Just check if it not duplicated.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (navInfo.HeaderCategory != headerCateg)
                            // Type Conflict
                            {
                                sMessage = "Object already exists with another type.\r\n";
                                sMessage += "\t" + navInfo.Node.Text + "\tfound in " + navInfo.HeaderCategoryNode.Text + "\r\n";
                                sMessage += "Please remove it before proceeding.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                navInfo = null;
                                s6xData = null;
                                return;
                            }
                            navInfo = null;
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
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xTable)s6xData).UniqueAddress)) ((S6xTable)s6xData).AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        ((S6xTable)s6xData).AddressBinInt = ((S6xTable)s6xData).AddressInt + sadBin.getBankBinAddress(((S6xTable)s6xData).BankNum);
                        ((S6xTable)s6xData).isCalibrationElement = sadBin.isCalibrationAddress(((S6xTable)s6xData).AddressBinInt);
                    }
                    ((S6xTable)s6xData).Store = true;
                    ((S6xTable)s6xData).DateUpdated = DateTime.UtcNow;
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
                        if (sadS6x.slDupTables.ContainsKey(dupAddress))
                        {
                            ((S6xTable)s6xData).DateCreated = ((S6xTable)sadS6x.slDupTables[dupAddress]).DateCreated;
                            sadS6x.slDupTables[dupAddress] = s6xData;
                        }
                        else
                        {
                            ((S6xTable)s6xData).DateCreated = DateTime.UtcNow;
                            sadS6x.slDupTables.Add(dupAddress, s6xData);
                        }
                    }
                    else
                    {
                        ((S6xTable)s6xData).DuplicateNum = 0;
                        if (sadS6x.slTables.ContainsKey(uniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            shortLabel = ((S6xTable)sadS6x.slTables[uniqueAddress]).ShortLabel;
                            if (shortLabel != null && shortLabel != string.Empty) ((S6xTable)s6xData).ShortLabel = shortLabel;

                            ((S6xTable)s6xData).DateCreated = ((S6xTable)sadS6x.slTables[uniqueAddress]).DateCreated;
                            sadS6x.slTables[uniqueAddress] = s6xData;
                        }
                        else
                        {
                            ((S6xTable)s6xData).DateCreated = DateTime.UtcNow;
                            sadS6x.slTables.Add(uniqueAddress, s6xData);
                        }
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xTable)s6xData).Category != null && ((S6xTable)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xTable)s6xData).Category);
                        if (((S6xTable)s6xData).Category2 != null && ((S6xTable)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xTable)s6xData).Category2);
                        if (((S6xTable)s6xData).Category3 != null && ((S6xTable)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xTable)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    if (isXdfData && overNavInfo == null)
                    {
                        navInfo = checkDuplicate(((S6xFunction)s6xData).UniqueAddress, false, null, null);
                        if (navInfo != null)
                        {
                            if (navInfo.Node.Parent == null)
                            // Block Reserved and Calibration Load Elements
                            {
                                navInfo = null;
                                s6xData = null;
                                return;
                            }
                            else if (navInfo.HeaderCategory == S6xNavHeaderCategory.OTHER)
                            // Other Presence
                            {
                                sMessage = "Information is partially defined.\r\n";
                                sMessage += "\t" + navInfo.Node.Text + "\tfound in " + navInfo.HeaderCategoryNode.Text + "\r\n";
                                sMessage += "Just check if it not duplicated.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (navInfo.HeaderCategory != headerCateg)
                            // Type Conflict
                            {
                                sMessage = "Object already exists with another type.\r\n";
                                sMessage += "\t" + navInfo.Node.Text + "\tfound in " + navInfo.HeaderCategoryNode.Text + "\r\n";
                                sMessage += "Please remove it before proceeding.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                navInfo = null;
                                s6xData = null;
                                return;
                            }
                            navInfo = null;
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
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xFunction)s6xData).UniqueAddress)) ((S6xFunction)s6xData).AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        ((S6xFunction)s6xData).AddressBinInt = ((S6xFunction)s6xData).AddressInt + sadBin.getBankBinAddress(((S6xFunction)s6xData).BankNum);
                        ((S6xFunction)s6xData).isCalibrationElement = sadBin.isCalibrationAddress(((S6xFunction)s6xData).AddressBinInt);
                    }
                    ((S6xFunction)s6xData).Store = true;
                    ((S6xFunction)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xFunction)s6xData).UniqueAddress;
                    label = ((S6xFunction)s6xData).Label;
                    comments = ((S6xFunction)s6xData).Comments;
                    if (((S6xFunction)s6xData).DuplicateNum > 0 && sadS6x.slFunctions.ContainsKey(uniqueAddress))
                    {
                        dupAddress = ((S6xFunction)s6xData).DuplicateAddress;
                        if (sadS6x.slDupFunctions.ContainsKey(dupAddress))
                        {
                            ((S6xFunction)s6xData).DateCreated = ((S6xFunction)sadS6x.slDupFunctions[dupAddress]).DateCreated;
                            sadS6x.slDupFunctions[dupAddress] = s6xData;
                        }
                        else
                        {
                            ((S6xFunction)s6xData).DateCreated = DateTime.UtcNow;
                            sadS6x.slDupFunctions.Add(dupAddress, s6xData);
                        }
                    }
                    else
                    {
                        ((S6xFunction)s6xData).DuplicateNum = 0;
                        if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            shortLabel = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).ShortLabel;
                            if (shortLabel != null && shortLabel != string.Empty) ((S6xFunction)s6xData).ShortLabel = shortLabel;

                            ((S6xFunction)s6xData).DateCreated = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).DateCreated;
                            sadS6x.slFunctions[uniqueAddress] = s6xData;
                        }
                        else
                        {
                            ((S6xFunction)s6xData).DateCreated = DateTime.UtcNow;
                            sadS6x.slFunctions.Add(uniqueAddress, s6xData);
                        }
                    }

                    // For Scalers Refresh
                    elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)].Tag = true;

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xFunction)s6xData).Category != null && ((S6xFunction)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xFunction)s6xData).Category);
                        if (((S6xFunction)s6xData).Category2 != null && ((S6xFunction)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xFunction)s6xData).Category2);
                        if (((S6xFunction)s6xData).Category3 != null && ((S6xFunction)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xFunction)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    if (isXdfData && overNavInfo == null)
                    {
                        navInfo = checkDuplicate(((S6xScalar)s6xData).UniqueAddress, false, null, null);
                        if (navInfo != null)
                        {
                            if (navInfo.Node.Parent == null)
                            // Block Reserved and Calibration Load Elements
                            {
                                navInfo = null;
                                s6xData = null;
                                return;
                            }
                            else if (navInfo.HeaderCategory == S6xNavHeaderCategory.OTHER)
                            // Other Presence
                            {
                                sMessage = "Information is partially defined.\r\n";
                                sMessage += "\t" + navInfo.Node.Text + "\tfound in " + navInfo.HeaderCategoryNode.Text + "\r\n";
                                sMessage += "Just check if it not duplicated.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (navInfo.HeaderCategory != headerCateg)
                            // Type Conflict
                            {
                                sMessage = "Object already exists with another type.\r\n";
                                sMessage += "\t" + navInfo.Node.Text + "\tfound in " + navInfo.HeaderCategoryNode.Text + "\r\n";
                                sMessage += "Please remove it before proceeding.";
                                MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                navInfo = null;
                                s6xData = null;
                                return;
                            }
                            navInfo = null;
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
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xScalar)s6xData).UniqueAddress)) ((S6xScalar)s6xData).AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        ((S6xScalar)s6xData).AddressBinInt = ((S6xScalar)s6xData).AddressInt + sadBin.getBankBinAddress(((S6xScalar)s6xData).BankNum);
                        ((S6xScalar)s6xData).isCalibrationElement = sadBin.isCalibrationAddress(((S6xScalar)s6xData).AddressBinInt);
                    }
                    ((S6xScalar)s6xData).Store = true;
                    ((S6xScalar)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xScalar)s6xData).UniqueAddress;
                    if (((S6xScalar)s6xData).DuplicateNum > 0 && sadS6x.slScalars.ContainsKey(uniqueAddress))
                    {
                        dupAddress = ((S6xScalar)s6xData).DuplicateAddress;
                        if (sadS6x.slDupScalars.ContainsKey(dupAddress))
                        {
                            if (((S6xScalar)s6xData).isBitFlags)
                            // Flag Paste, no Update on Label and Comments, Flag is added
                            {
                                label = ((S6xScalar)sadS6x.slDupScalars[dupAddress]).Label;
                                comments = ((S6xScalar)sadS6x.slDupScalars[dupAddress]).Comments;
                                if (((S6xScalar)s6xData).BitFlags != null)
                                {
                                    foreach (S6xBitFlag bF in ((S6xScalar)s6xData).BitFlags) ((S6xScalar)sadS6x.slDupScalars[dupAddress]).AddBitFlag(bF);
                                }
                            }
                            else
                            {
                                if (((S6xScalar)sadS6x.slDupScalars[dupAddress]).isBitFlags)
                                // Prevent loss of existing BitFlags
                                {
                                    ((S6xScalar)s6xData).BitFlags = ((S6xScalar)sadS6x.slDupScalars[dupAddress]).BitFlags;
                                }
                                ((S6xScalar)s6xData).DateCreated = ((S6xScalar)sadS6x.slDupScalars[dupAddress]).DateCreated;
                                sadS6x.slDupScalars[dupAddress] = s6xData;
                                label = ((S6xScalar)s6xData).Label;
                                comments = ((S6xScalar)s6xData).Comments;
                            }
                        }
                        else
                        {
                            label = ((S6xScalar)s6xData).Label;
                            comments = ((S6xScalar)s6xData).Comments;
                            ((S6xScalar)s6xData).DateCreated = DateTime.UtcNow;
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
                                ((S6xScalar)s6xData).DateCreated = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).DateCreated;
                                sadS6x.slScalars[uniqueAddress] = s6xData;
                                label = ((S6xScalar)s6xData).Label;
                                comments = ((S6xScalar)s6xData).Comments;
                            }
                        }
                        else
                        {
                            label = ((S6xScalar)s6xData).Label;
                            comments = ((S6xScalar)s6xData).Comments;
                            ((S6xScalar)s6xData).DateCreated = DateTime.UtcNow;
                            sadS6x.slScalars.Add(uniqueAddress, s6xData);
                        }
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xScalar)s6xData).Category != null && ((S6xScalar)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xScalar)s6xData).Category);
                        if (((S6xScalar)s6xData).Category2 != null && ((S6xScalar)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xScalar)s6xData).Category2);
                        if (((S6xScalar)s6xData).Category3 != null && ((S6xScalar)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xScalar)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    s6xStruct = new S6xStructure((S6xStructureClipBoard)s6xData);
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        s6xStruct.BankNum = overBankNum;
                        s6xStruct.AddressInt = overAddress;
                        s6xStruct.DuplicateNum = overDuplicateNum;
                    }
                    else if (isS6xClipBoard)
                    {
                        s6xStruct.AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(s6xStruct.UniqueAddress)) s6xStruct.AddressInt++;
                    }
                    if (sadBin != null)
                    {
                        s6xStruct.AddressBinInt = s6xStruct.AddressInt + sadBin.getBankBinAddress(s6xStruct.BankNum);
                        s6xStruct.isCalibrationElement = sadBin.isCalibrationAddress(s6xStruct.AddressBinInt);
                    }
                    s6xStruct.Store = true;
                    s6xStruct.DateUpdated = DateTime.UtcNow;
                    uniqueAddress = s6xStruct.UniqueAddress;
                    label = s6xStruct.Label;
                    comments = s6xStruct.Comments;
                    if (s6xStruct.DuplicateNum > 0 && sadS6x.slStructures.ContainsKey(uniqueAddress))
                    {
                        dupAddress = s6xStruct.DuplicateAddress;
                        if (sadS6x.slDupStructures.ContainsKey(dupAddress))
                        {
                            s6xStruct.DateCreated = ((S6xStructure)sadS6x.slDupStructures[dupAddress]).DateCreated;
                            sadS6x.slDupStructures[dupAddress] = s6xStruct;
                        }
                        else
                        {
                            s6xStruct.DateCreated = DateTime.UtcNow; 
                            sadS6x.slDupStructures.Add(dupAddress, s6xStruct);
                        }
                    }
                    else
                    {
                        s6xStruct.DuplicateNum = 0;
                        if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                        {
                            s6xStruct.DateCreated = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).DateCreated;
                            sadS6x.slStructures[uniqueAddress] = s6xStruct;
                        }
                        else
                        {
                            s6xStruct.DateCreated = DateTime.UtcNow; 
                            sadS6x.slStructures.Add(uniqueAddress, s6xStruct);
                        }
                    }
                    s6xStruct = null;

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xStructureClipBoard)s6xData).Category != null && ((S6xStructureClipBoard)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xStructureClipBoard)s6xData).Category);
                        if (((S6xStructureClipBoard)s6xData).Category2 != null && ((S6xStructureClipBoard)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xStructureClipBoard)s6xData).Category2);
                        if (((S6xStructureClipBoard)s6xData).Category3 != null && ((S6xStructureClipBoard)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xStructureClipBoard)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.ROUTINES:
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xRoutine)s6xData).BankNum = overBankNum;
                        ((S6xRoutine)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xRoutine)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xRoutine)s6xData).UniqueAddress)) ((S6xRoutine)s6xData).AddressInt++;
                    }
                    ((S6xRoutine)s6xData).Store = true;
                    ((S6xRoutine)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xRoutine)s6xData).UniqueAddress;
                    label = ((S6xRoutine)s6xData).Label;
                    comments = ((S6xRoutine)s6xData).Comments;
                    if (sadS6x.slRoutines.ContainsKey(uniqueAddress))
                    {
                        ((S6xRoutine)s6xData).DateCreated = ((S6xRoutine)sadS6x.slRoutines[uniqueAddress]).DateCreated;
                        sadS6x.slRoutines[uniqueAddress] = s6xData;
                    }
                    else
                    {
                        ((S6xRoutine)s6xData).DateCreated = DateTime.UtcNow;
                        sadS6x.slRoutines.Add(uniqueAddress, s6xData);
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xRoutine)s6xData).Category != null && ((S6xRoutine)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xRoutine)s6xData).Category);
                        if (((S6xRoutine)s6xData).Category2 != null && ((S6xRoutine)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xRoutine)s6xData).Category2);
                        if (((S6xRoutine)s6xData).Category3 != null && ((S6xRoutine)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xRoutine)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.OPERATIONS:
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xOperation)s6xData).BankNum = overBankNum;
                        ((S6xOperation)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xOperation)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xOperation)s6xData).UniqueAddress)) ((S6xOperation)s6xData).AddressInt++;
                    }
                    ((S6xOperation)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xOperation)s6xData).UniqueAddress;
                    label = ((S6xOperation)s6xData).Label;
                    comments = ((S6xOperation)s6xData).Comments;
                    if (sadS6x.slOperations.ContainsKey(uniqueAddress))
                    {
                        ((S6xOperation)s6xData).DateCreated = ((S6xOperation)sadS6x.slOperations[uniqueAddress]).DateCreated;
                        sadS6x.slOperations[uniqueAddress] = s6xData;
                    }
                    else
                    {
                        ((S6xOperation)s6xData).DateCreated = DateTime.UtcNow;
                        sadS6x.slOperations.Add(uniqueAddress, s6xData);
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xOperation)s6xData).Category != null && ((S6xOperation)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xOperation)s6xData).Category);
                        if (((S6xOperation)s6xData).Category2 != null && ((S6xOperation)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xOperation)s6xData).Category2);
                        if (((S6xOperation)s6xData).Category3 != null && ((S6xOperation)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xOperation)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.REGISTERS:
                    if (overAddress != -1)
                    {
                        ((S6xRegister)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xRegister)s6xData).AddressInt = 0xffff;
                        ((S6xRegister)s6xData).AutoConstValue = false;
                        ((S6xRegister)s6xData).isRBase = false;
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xRegister)s6xData).UniqueAddress)) ((S6xRegister)s6xData).AddressInt--;
                    }
                    ((S6xRegister)s6xData).Store = true;
                    ((S6xRegister)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xRegister)s6xData).UniqueAddress;
                    label = ((S6xRegister)s6xData).Label;
                    comments = ((S6xRegister)s6xData).Comments;
                    if (sadS6x.slRegisters.ContainsKey(uniqueAddress))
                    {
                        ((S6xRegister)s6xData).DateCreated = ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).DateCreated;
                        sadS6x.slRegisters[uniqueAddress] = s6xData;
                    }
                    else
                    {
                        ((S6xRegister)s6xData).DateCreated = DateTime.UtcNow;
                        sadS6x.slRegisters.Add(uniqueAddress, s6xData);
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xRegister)s6xData).Category != null && ((S6xRegister)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xRegister)s6xData).Category);
                        if (((S6xRegister)s6xData).Category2 != null && ((S6xRegister)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xRegister)s6xData).Category2);
                        if (((S6xRegister)s6xData).Category3 != null && ((S6xRegister)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xRegister)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.OTHER:
                    if (overBankNum != -1 && overAddress != -1)
                    {
                        ((S6xOtherAddress)s6xData).BankNum = overBankNum;
                        ((S6xOtherAddress)s6xData).AddressInt = overAddress;
                    }
                    else if (isS6xClipBoard)
                    {
                        ((S6xOtherAddress)s6xData).AddressInt = 0 - SADDef.EecBankStartAddress;
                        while (elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)].Nodes.ContainsKey(((S6xOtherAddress)s6xData).UniqueAddress)) ((S6xOtherAddress)s6xData).AddressInt++;
                    }
                    ((S6xOtherAddress)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xOtherAddress)s6xData).UniqueAddress;
                    label = ((S6xOtherAddress)s6xData).UniqueAddressHex;
                    comments = ((S6xOtherAddress)s6xData).Comments;
                    if (sadS6x.slOtherAddresses.ContainsKey(uniqueAddress)) sadS6x.slOtherAddresses[uniqueAddress] = s6xData;
                    else sadS6x.slOtherAddresses.Add(uniqueAddress, s6xData);

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xOtherAddress)s6xData).Category != null && ((S6xOtherAddress)s6xData).Category != string.Empty) navCateg1 = new S6xNavCategory(((S6xOtherAddress)s6xData).Category);
                        if (((S6xOtherAddress)s6xData).Category2 != null && ((S6xOtherAddress)s6xData).Category2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xOtherAddress)s6xData).Category2);
                        if (((S6xOtherAddress)s6xData).Category3 != null && ((S6xOtherAddress)s6xData).Category3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xOtherAddress)s6xData).Category3);
                    }
                    break;
                case S6xNavHeaderCategory.SIGNATURES:
                    if (overUniqueAddress != string.Empty) ((S6xSignature)s6xData).UniqueKey = overUniqueAddress;
                    else ((S6xSignature)s6xData).UniqueKey = sadS6x.getNewSignatureUniqueKey();
                    ((S6xSignature)s6xData).Forced = false;
                    ((S6xSignature)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xSignature)s6xData).UniqueKey;
                    label = ((S6xSignature)s6xData).SignatureLabel;
                    comments = ((S6xSignature)s6xData).SignatureComments;
                    if (sadS6x.slSignatures.ContainsKey(uniqueAddress))
                    {
                        ((S6xSignature)s6xData).DateCreated = ((S6xSignature)sadS6x.slSignatures[uniqueAddress]).DateCreated;
                        sadS6x.slSignatures[uniqueAddress] = s6xData;
                    }
                    else
                    {
                        ((S6xSignature)s6xData).DateCreated = DateTime.UtcNow;
                        sadS6x.slSignatures.Add(uniqueAddress, s6xData);
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xSignature)s6xData).SignatureCategory != null && ((S6xSignature)s6xData).SignatureCategory != string.Empty) navCateg1 = new S6xNavCategory(((S6xSignature)s6xData).SignatureCategory);
                        if (((S6xSignature)s6xData).SignatureCategory2 != null && ((S6xSignature)s6xData).SignatureCategory2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xSignature)s6xData).SignatureCategory2);
                        if (((S6xSignature)s6xData).SignatureCategory3 != null && ((S6xSignature)s6xData).SignatureCategory3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xSignature)s6xData).SignatureCategory3);
                    }
                    break;
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    if (overUniqueAddress != string.Empty) ((S6xElementSignature)s6xData).UniqueKey = overUniqueAddress;
                    else ((S6xElementSignature)s6xData).UniqueKey = sadS6x.getNewElementSignatureUniqueKey();
                    ((S6xElementSignature)s6xData).Forced = false;
                    ((S6xElementSignature)s6xData).DateUpdated = DateTime.UtcNow;
                    uniqueAddress = ((S6xElementSignature)s6xData).UniqueKey;
                    label = ((S6xElementSignature)s6xData).SignatureLabel;
                    comments = ((S6xElementSignature)s6xData).SignatureComments;
                    if (sadS6x.slElementsSignatures.ContainsKey(uniqueAddress))
                    {
                        ((S6xElementSignature)s6xData).DateCreated = ((S6xElementSignature)sadS6x.slElementsSignatures[uniqueAddress]).DateCreated;
                        sadS6x.slElementsSignatures[uniqueAddress] = s6xData;
                    }
                    else
                    {
                        ((S6xElementSignature)s6xData).DateCreated = DateTime.UtcNow;
                        sadS6x.slElementsSignatures.Add(uniqueAddress, s6xData);
                    }

                    if (isS6xClipBoard && !isXdfData)
                    {
                        if (((S6xElementSignature)s6xData).SignatureCategory != null && ((S6xElementSignature)s6xData).SignatureCategory != string.Empty) navCateg1 = new S6xNavCategory(((S6xElementSignature)s6xData).SignatureCategory);
                        if (((S6xElementSignature)s6xData).SignatureCategory2 != null && ((S6xElementSignature)s6xData).SignatureCategory2 != string.Empty) navCateg2 = new S6xNavCategory(((S6xElementSignature)s6xData).SignatureCategory2);
                        if (((S6xElementSignature)s6xData).SignatureCategory3 != null && ((S6xElementSignature)s6xData).SignatureCategory3 != string.Empty) navCateg3 = new S6xNavCategory(((S6xElementSignature)s6xData).SignatureCategory3);
                    }
                    break;
            }
            if (uniqueAddress != string.Empty)
            {
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(headerCateg)]);
                TreeNode tnNode = niHeaderCateg.FindElement(uniqueAddress);
                if (tnNode == null)
                {
                    tnNode = new TreeNode();
                    tnNode.Name = uniqueAddress;
                    tnNode.ContextMenuStrip = elemsContextMenuStrip;
                    switch (headerCateg)
                    {
                        case S6xNavHeaderCategory.SIGNATURES:
                        case S6xNavHeaderCategory.ELEMSSIGNATURES:
                            break;
                        default:
                            if (isS6xClipBoard && overNavInfo == null) alClipBoardTempUniqueAddresses.Add(uniqueAddress);
                            tnNode.ForeColor = Color.Red;
                            break;
                    }
                }
                else
                {
                    if (dupAddress != string.Empty)
                    {
                        if (tnNode.Nodes.ContainsKey(dupAddress)) tnNode = tnNode.Nodes[dupAddress];
                    }
                }
                tnNode.Text = label;
                tnNode.ToolTipText = comments;

                if (niHeaderCateg.FindElement(uniqueAddress) == null)
                {
                    if (overNavInfo != null)
                    {
                        tnNode.ForeColor = Color.Purple;
                        navCateg1 = overNavInfo.Category;
                        navCateg2 = overNavInfo.Category2;
                        navCateg3 = overNavInfo.Category3;
                    }
                    niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, dupAddress != string.Empty, getS6xNavCategoryDepth());
                    setElementsTreeCategLabel(headerCateg);
                }
                else
                {
                    tnNode.ForeColor = Color.Purple;
                    // Check if Node has moved
                    S6xNavInfo niNI = new S6xNavInfo(tnNode);
                    if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                    {
                        tnNode.Parent.Nodes.Remove(tnNode);
                        niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, dupAddress != string.Empty, getS6xNavCategoryDepth());
                    }
                    niNI = null;
                }
                // To force refresh of categories in related combo boxes
                sharedCategComboBox.Tag = null;
                sharedCateg2ComboBox.Tag = null;
                sharedCateg3ComboBox.Tag = null;

                elemsTreeView.SelectedNode = tnNode;
                tnNode = null;

                if (overNavInfo != null && headerCateg != overHeaderCateg)
                {
                    overNavInfo.Node.Parent.Nodes.Remove(overNavInfo.Node);
                    setElementsTreeCategLabel(overHeaderCateg);
                }
            }

            sadS6x.isSaved = false;
            s6xData = null;
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (navInfo.MainNode == null) return;

            if (!confirmDirtyProperies()) return;
            
            TreeNode tnDup = null;
            string uniqueAddress = string.Empty;
            string duplicateAddress = string.Empty;

            tnDup = new TreeNode();
            tnDup.Text = navInfo.Node.Text;
            tnDup.ToolTipText = navInfo.Node.ToolTipText;
            tnDup.ContextMenuStrip = navInfo.Node.ContextMenuStrip;

            uniqueAddress = navInfo.MainNode.Name;
            if (navInfo.isDuplicate) duplicateAddress = navInfo.Node.Name;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    ((S6xTable)sadS6x.slTables[uniqueAddress]).DuplicateNum = 0;
                    ((S6xTable)sadS6x.slTables[uniqueAddress]).Store = true;
                    S6xTable dupTable = null;
                    if (duplicateAddress == string.Empty) dupTable = ((S6xTable)sadS6x.slTables[uniqueAddress]).Clone();
                    else dupTable = ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Clone();
                    dupTable.DuplicateNum = 1;
                    dupTable.DateCreated = Tools.getValidDateTime(dupTable.DateCreated, DateTime.UtcNow);
                    dupTable.DateUpdated = DateTime.UtcNow;
                    while (sadS6x.slDupTables.ContainsKey(dupTable.DuplicateAddress)) dupTable.DuplicateNum++;
                    sadS6x.slDupTables.Add(dupTable.DuplicateAddress, dupTable);
                    tnDup.Name = dupTable.DuplicateAddress;
                    dupTable = null;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).DuplicateNum = 0;
                    ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Store = true;
                    S6xFunction dupFunction = null;
                    if (duplicateAddress == string.Empty) dupFunction = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Clone();
                    else dupFunction = ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Clone();
                    dupFunction.DuplicateNum = 1;
                    dupFunction.DateCreated = Tools.getValidDateTime(dupFunction.DateCreated, DateTime.UtcNow);
                    dupFunction.DateUpdated = DateTime.UtcNow;
                    while (sadS6x.slDupFunctions.ContainsKey(dupFunction.DuplicateAddress)) dupFunction.DuplicateNum++;
                    sadS6x.slDupFunctions.Add(dupFunction.DuplicateAddress, dupFunction);
                    tnDup.Name = dupFunction.DuplicateAddress;
                    dupFunction = null;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    ((S6xScalar)sadS6x.slScalars[uniqueAddress]).DuplicateNum = 0;
                    ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Store = true;
                    S6xScalar dupScalar = null;
                    if (duplicateAddress == string.Empty) dupScalar = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Clone();
                    else dupScalar = ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Clone();
                    dupScalar.DuplicateNum = 1;
                    dupScalar.DateCreated = Tools.getValidDateTime(dupScalar.DateCreated, DateTime.UtcNow);
                    dupScalar.DateUpdated = DateTime.UtcNow;
                    while (sadS6x.slDupScalars.ContainsKey(dupScalar.DuplicateAddress)) dupScalar.DuplicateNum++;
                    sadS6x.slDupScalars.Add(dupScalar.DuplicateAddress, dupScalar);
                    tnDup.Name = dupScalar.DuplicateAddress;
                    dupScalar = null;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    ((S6xStructure)sadS6x.slStructures[uniqueAddress]).DuplicateNum = 0;
                    ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Store = true;
                    S6xStructure dupStructure = null;
                    if (duplicateAddress == string.Empty) dupStructure = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Clone();
                    else dupStructure = ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Clone();
                    dupStructure.DuplicateNum = 1;
                    dupStructure.DateCreated = Tools.getValidDateTime(dupStructure.DateCreated, DateTime.UtcNow);
                    dupStructure.DateUpdated = DateTime.UtcNow;
                    while (sadS6x.slDupStructures.ContainsKey(dupStructure.DuplicateAddress)) dupStructure.DuplicateNum++;
                    sadS6x.slDupStructures.Add(dupStructure.DuplicateAddress, dupStructure);
                    tnDup.Name = dupStructure.DuplicateAddress;
                    dupStructure = null;
                    break;
                default:
                    tnDup = null;
                    navInfo = null;
                    return;
            }

            navInfo.MainNode.Nodes.Add(tnDup);

            sadS6x.isSaved = false;

            elemsTreeView.SelectedNode = tnDup;
            elemsTreeView.SelectedNode.ForeColor = Color.Purple;
            navInfo.MainNode.ForeColor = Color.Purple;

            tnDup = null;
            navInfo = null;
        }

        private void unDuplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (!navInfo.isDuplicate) return;

            if (!confirmDirtyProperies()) return;

            int dupNum = -1;
            string nText = string.Empty;
            string nToolTipText = string.Empty;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
                    S6xTable mainTable = ((S6xTable)sadS6x.slTables[navInfo.MainNode.Name]);
                    S6xTable dupTable = ((S6xTable)sadS6x.slDupTables[navInfo.DuplicateNode.Name]);
                    dupNum = dupTable.DuplicateNum;
                    dupTable.DuplicateNum = mainTable.DuplicateNum;
                    dupTable.DateUpdated = DateTime.UtcNow;
                    mainTable.DuplicateNum = dupNum;
                    mainTable.DateUpdated = DateTime.UtcNow;

                    sadS6x.slTables[navInfo.MainNode.Name] = dupTable;
                    sadS6x.slDupTables[navInfo.DuplicateNode.Name] = mainTable;

                    dupTable = null;
                    mainTable = null;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    S6xFunction mainFunction = ((S6xFunction)sadS6x.slFunctions[navInfo.MainNode.Name]);
                    S6xFunction dupFunction = ((S6xFunction)sadS6x.slDupFunctions[navInfo.DuplicateNode.Name]);
                    
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
                    dupFunction.DateUpdated = DateTime.UtcNow;
                    mainFunction.DuplicateNum = dupNum;
                    mainFunction.DateUpdated = DateTime.UtcNow;

                    sadS6x.slFunctions[navInfo.MainNode.Name] = dupFunction;
                    sadS6x.slDupFunctions[navInfo.DuplicateNode.Name] = mainFunction;

                    dupFunction = null;
                    mainFunction = null;
                    break;
                case S6xNavHeaderCategory.SCALARS:
                    S6xScalar mainScalar = ((S6xScalar)sadS6x.slScalars[navInfo.MainNode.Name]);
                    S6xScalar dupScalar = ((S6xScalar)sadS6x.slDupScalars[navInfo.DuplicateNode.Name]);
                    dupNum = dupScalar.DuplicateNum;
                    dupScalar.DuplicateNum = mainScalar.DuplicateNum;
                    dupScalar.DateUpdated = DateTime.UtcNow;
                    mainScalar.DuplicateNum = dupNum;
                    mainScalar.DateUpdated = DateTime.UtcNow;

                    sadS6x.slScalars[navInfo.MainNode.Name] = dupScalar;
                    sadS6x.slDupScalars[navInfo.DuplicateNode.Name] = mainScalar;

                    dupScalar = null;
                    mainScalar = null;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    S6xStructure mainStructure = ((S6xStructure)sadS6x.slStructures[navInfo.MainNode.Name]);
                    S6xStructure dupStructure = ((S6xStructure)sadS6x.slDupStructures[navInfo.DuplicateNode.Name]);
                    dupNum = dupStructure.DuplicateNum;
                    dupStructure.DuplicateNum = mainStructure.DuplicateNum;
                    dupStructure.DateUpdated = DateTime.UtcNow;
                    mainStructure.DuplicateNum = dupNum;
                    mainStructure.DateUpdated = DateTime.UtcNow;

                    sadS6x.slStructures[navInfo.MainNode.Name] = dupStructure;
                    sadS6x.slDupStructures[navInfo.DuplicateNode.Name] = mainStructure;

                    dupStructure = null;
                    mainStructure = null;
                    break;
                default:
                    navInfo = null;
                    return;
            }

            nText = navInfo.DuplicateNode.Text;
            nToolTipText = navInfo.DuplicateNode.ToolTipText;
            navInfo.DuplicateNode.Text = navInfo.MainNode.Text;
            navInfo.DuplicateNode.ToolTipText = navInfo.MainNode.ToolTipText;
            navInfo.MainNode.Text = nText;
            navInfo.MainNode.ToolTipText = nToolTipText;

            sadS6x.isSaved = false;

            elemsTreeView.SelectedNode = navInfo.MainNode;
            elemsTreeView.SelectedNode.ForeColor = Color.Purple;
            navInfo.DuplicateNode.ForeColor = Color.Purple;

            navInfo = null;
}

        private void importSignaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SADS6x sigS6x = null;
            S6xNavInfo niHeaderCateg = null;
            TreeNode tnNode = null;
            S6xNavInfo niNI = null;
            S6xNavCategory navCateg1 = null;
            S6xNavCategory navCateg2 = null;
            S6xNavCategory navCateg3 = null;

            if (openFileDialogS6x.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(openFileDialogS6x.FileName)) return;

            try
            {
                sigS6x = new SADS6x(openFileDialogS6x.FileName);

                if (!sigS6x.isValid) throw new Exception();

                elemsTreeView.BeginUpdate();
                
                if (sigS6x.slSignatures != null)
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SIGNATURES)]);

                    foreach (S6xSignature impSignature in sigS6x.slSignatures.Values)
                    {
                        S6xSignature clone = impSignature.Clone();
                        clone.DateCreated = DateTime.UtcNow;
                        clone.DateUpdated = DateTime.UtcNow;
                        clone.UniqueKey = string.Empty;
                        foreach (S6xSignature s6xSignature in sadS6x.slSignatures.Values)
                        {
                            if (s6xSignature.ShortLabel == clone.ShortLabel)
                            {
                                clone.UniqueKey = s6xSignature.UniqueKey;
                                clone.DateCreated = s6xSignature.DateCreated;
                                clone.DateUpdated = DateTime.UtcNow;
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

                        tnNode = niHeaderCateg.FindElement(clone.UniqueKey);
                        if (tnNode == null)
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = clone.UniqueKey;

                            niHeaderCateg.AddNode(tnNode, new S6xNavCategory(clone.SignatureCategory), new S6xNavCategory(clone.SignatureCategory2), new S6xNavCategory(clone.SignatureCategory3), false, getS6xNavCategoryDepth());
                            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);
                        }
                        else
                        {
                            // Check if Node has moved
                            niNI = new S6xNavInfo(tnNode);
                            navCateg1 = s6xNavCategories.getCategory(niNI.HeaderCategory, S6xNavCategoryLevel.ONE, true, clone.SignatureCategory);
                            navCateg2 = s6xNavCategories.getCategory(niNI.HeaderCategory, S6xNavCategoryLevel.TWO, true, clone.SignatureCategory2);
                            navCateg3 = s6xNavCategories.getCategory(niNI.HeaderCategory, S6xNavCategoryLevel.THREE, true, clone.SignatureCategory3);
                            if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                            {
                                tnNode.Parent.Nodes.Remove(tnNode);
                                niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, getS6xNavCategoryDepth());
                            }
                            niNI = null;
                        }
                        tnNode.Text = clone.Label;
                        tnNode.ToolTipText = clone.Comments;

                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        tnNode.ForeColor = Color.Purple;

                        // To force refresh of categories in related combo boxes
                        sharedCategComboBox.Tag = null;
                        sharedCateg2ComboBox.Tag = null;
                        sharedCateg3ComboBox.Tag = null;
                    }
                }
                if (sigS6x.slElementsSignatures != null)
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.ELEMSSIGNATURES)]);

                    foreach (S6xElementSignature impSignature in sigS6x.slElementsSignatures.Values)
                    {
                        S6xElementSignature clone = impSignature.Clone();
                        clone.UniqueKey = string.Empty;
                        clone.DateCreated = DateTime.UtcNow;
                        clone.DateUpdated = DateTime.UtcNow;
                        foreach (S6xElementSignature s6xESignature in sadS6x.slElementsSignatures.Values)
                        {
                            if (s6xESignature.SignatureKey == clone.SignatureKey)
                            {
                                clone.UniqueKey = s6xESignature.UniqueKey;
                                clone.DateCreated = s6xESignature.DateCreated;
                                clone.DateUpdated = DateTime.UtcNow;
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

                        tnNode = niHeaderCateg.FindElement(clone.UniqueKey);
                        if (tnNode == null)
                        {
                            tnNode = new TreeNode();
                            tnNode.Name = clone.UniqueKey;

                            niHeaderCateg.AddNode(tnNode, new S6xNavCategory(clone.SignatureCategory), new S6xNavCategory(clone.SignatureCategory2), new S6xNavCategory(clone.SignatureCategory3), false, getS6xNavCategoryDepth());
                            setElementsTreeCategLabel(niHeaderCateg.HeaderCategory);
                        }
                        else
                        {
                            // Check if Node has moved
                            niNI = new S6xNavInfo(tnNode);
                            navCateg1 = s6xNavCategories.getCategory(niNI.HeaderCategory, S6xNavCategoryLevel.ONE, true, clone.SignatureCategory);
                            navCateg2 = s6xNavCategories.getCategory(niNI.HeaderCategory, S6xNavCategoryLevel.TWO, true, clone.SignatureCategory2);
                            navCateg3 = s6xNavCategories.getCategory(niNI.HeaderCategory, S6xNavCategoryLevel.THREE, true, clone.SignatureCategory3);
                            if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                            {
                                tnNode.Parent.Nodes.Remove(tnNode);
                                niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, getS6xNavCategoryDepth());
                            }
                            niNI = null;
                        }
                        tnNode.Text = clone.SignatureLabel;
                        tnNode.ToolTipText = clone.SignatureComments;

                        tnNode.ContextMenuStrip = elemsContextMenuStrip;
                        tnNode.ForeColor = Color.Purple;

                        // To force refresh of categories in related combo boxes
                        sharedCategComboBox.Tag = null;
                        sharedCateg2ComboBox.Tag = null;
                        sharedCateg3ComboBox.Tag = null;
                    }
                }

                elemsTreeView.EndUpdate();
            }
            catch
            {
                elemsTreeView.EndUpdate();
                MessageBox.Show("Selected file is not valid.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sigS6x = null;
                niHeaderCateg = null;
                tnNode = null;
            }
        }

        private void importDirFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importDirFile(string.Empty);
        }

        private void importDirFile(string dFilePath)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

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

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { dFilePath, tfSADVersionToolStripComboBox.SelectedItem.ToString() };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(importDirFileStartProcess);
            processThread.Start();
        }

        private void importDirFileStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            string tfSADVersion = string.Empty;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                tfSADVersion = (string)sadProcessManager.Parameters[1];

                if (filePath == null || filePath == string.Empty) throw new Exception();
                if (tfSADVersion == null || tfSADVersion == string.Empty) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            ArrayList alNewTreeNodesInfos = new ArrayList();

            sadProcessManager.SetProcessStarted("Import is starting.");

            try
            {
                TfSADTools.ImportDirFile(filePath, tfSADVersion, ref alNewTreeNodesInfos, ref sadBin, ref sadS6x, ref sadProcessManager);
            }
            catch (Exception ex)
            {
                sadProcessManager.ProcessErrors.Add(ex.Message);
            }

            sadProcessManager.PostProcessAction = "importDirFilePostProcess";
            sadProcessManager.PostProcessParameters = new object[] { alNewTreeNodesInfos };

            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void importDirFilePostProcess()
        {
            if (sadProcessManager == null) return;

            ArrayList alNewTreeNodesInfos = null;

            try
            {
                alNewTreeNodesInfos = (ArrayList)((object[])sadProcessManager.PostProcessParameters)[0];
            }
            catch
            {
                sadProcessManager.ProcessErrors.Add("Invalid post process start.");
                sadProcessManager.SetProcessFailed("Import has failed.");
                return;
            }

            if (alNewTreeNodesInfos != null)
            {
                if (alNewTreeNodesInfos.Count > 0)
                {
                    elemsTreeView.BeginUpdate();

                    try
                    {
                        // alNewTreeNodesInfos - ArrayList containing N string[] arrays
                        // string[] array definition
                        //  0 : Node Categ Name
                        //  1 : Node Name (UniqueAddress)
                        //  2 : Node Text
                        //  3 : Node ToolTipText

                        S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();

                        // Updates First for Threading Purposes
                        foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                        {
                            S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                            if (!niHeaderCateg.isValid) continue;
                            TreeNode tnNode = niHeaderCateg.FindElement(newTreeNodeInfos[1]);
                            if (tnNode == null) continue;
                            if (tnNode.Text != newTreeNodeInfos[2]) // For Performance purposes
                            {
                                tnNode.Text = newTreeNodeInfos[2];
                                tnNode.ToolTipText = newTreeNodeInfos[3];
                                tnNode.ForeColor = Color.Purple;
                            }
                            newTreeNodeInfos[0] = null; // To be ignored at creation level
                        }
                        foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                        {
                            if (newTreeNodeInfos[0] == null) continue;
                            S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                            if (!niHeaderCateg.isValid) continue;
                            if (niHeaderCateg.FindElement(newTreeNodeInfos[1]) != null) continue;
                            TreeNode tnNode = new TreeNode();
                            tnNode.Name = newTreeNodeInfos[1];
                            tnNode.Text = newTreeNodeInfos[2];
                            tnNode.ToolTipText = newTreeNodeInfos[3];
                            tnNode.ContextMenuStrip = elemsContextMenuStrip;
                            tnNode.ForeColor = Color.Red;
                            niHeaderCateg.AddNode(tnNode, null, null, null, false, categoryDepth);
                        }
                    }
                    catch (Exception ex)
                    {
                        sadProcessManager.ProcessErrors.Add("Directive elements creation has failed.\r\n" + ex.Message);
                    }

                    setElementsTreeCategLabel(S6xNavHeaderCategory.SCALARS);
                    setElementsTreeCategLabel(S6xNavHeaderCategory.FUNCTIONS);
                    elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)].Tag = true;              // For Scalers Refresh
                    setElementsTreeCategLabel(S6xNavHeaderCategory.TABLES);
                    setElementsTreeCategLabel(S6xNavHeaderCategory.STRUCTURES);
                    setElementsTreeCategLabel(S6xNavHeaderCategory.ROUTINES);
                    setElementsTreeCategLabel(S6xNavHeaderCategory.OPERATIONS);
                    setElementsTreeCategLabel(S6xNavHeaderCategory.REGISTERS);
                    setElementsTreeCategLabel(S6xNavHeaderCategory.OTHER);

                    elemsTreeView.EndUpdate();
                }
                alNewTreeNodesInfos = null;
            }

            outputToolStripMenuItem.Enabled = false;

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Import is done.");
            else sadProcessManager.SetProcessFinished("Import has finished with errors.");
        }

        private void exportDirFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            if (saveFileDialogDir.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { saveFileDialogDir.FileName, tfSADVersionToolStripComboBox.SelectedItem.ToString() };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(exportDirFileStartProcess);
            processThread.Start();
        }

        private void exportDirFileStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            string tfSADVersion = string.Empty;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                tfSADVersion = (string)sadProcessManager.Parameters[1];

                if (filePath == null || filePath == string.Empty) throw new Exception();
                if (tfSADVersion == null || tfSADVersion == string.Empty) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            try
            {
                sadProcessManager.SetProcessStarted("Export is starting.");
                TfSADTools.ExportDirFile(filePath, tfSADVersion, ref sadBin, ref sadS6x, ref sadProcessManager);
                sadProcessManager.SetProcessFinished("Export is done.");
            }
            catch (Exception ex)
            {
                sadProcessManager.ProcessErrors.Add(ex.Message);
                sadProcessManager.SetProcessFailed("Process has failed.");
            }
        }

        private void importCmtFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importCmtFile(string.Empty);
        }

        private void importCmtFile(string cFilePath)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

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

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { cFilePath, tfSADVersionToolStripComboBox.SelectedItem.ToString() };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(importCmtFileStartProcess);
            processThread.Start();
        }

        private void importCmtFileStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            string tfSADVersion = string.Empty;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                tfSADVersion = (string)sadProcessManager.Parameters[1];

                if (filePath == null || filePath == string.Empty) throw new Exception();
                if (tfSADVersion == null || tfSADVersion == string.Empty) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            ArrayList alNewTreeNodesInfos = new ArrayList();

            sadProcessManager.SetProcessStarted("Import is starting.");

            try
            {
                TfSADTools.ImportCmtFile(filePath, tfSADVersion, ref alNewTreeNodesInfos, ref sadBin, ref sadS6x, ref sadProcessManager);
            }
            catch (Exception ex)
            {
                sadProcessManager.ProcessErrors.Add(ex.Message);
            }

            sadProcessManager.PostProcessAction = "importCmtFilePostProcess";
            sadProcessManager.PostProcessParameters = new object[] { alNewTreeNodesInfos };

            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void importCmtFilePostProcess()
        {
            if (sadProcessManager == null) return;

            ArrayList alNewTreeNodesInfos = null;

            try
            {
                alNewTreeNodesInfos = (ArrayList)((object[])sadProcessManager.PostProcessParameters)[0];
            }
            catch
            {
                sadProcessManager.ProcessErrors.Add("Invalid post process start.");
                sadProcessManager.SetProcessFailed("Import has failed.");
                return;
            }

            if (alNewTreeNodesInfos != null)
            {
                if (alNewTreeNodesInfos.Count != 0)
                {
                    elemsTreeView.BeginUpdate();

                    try
                    {
                        // alNewTreeNodesInfos - ArrayList containing N string[] arrays
                        // string[] array definition
                        //  0 : Node Categ Name
                        //  1 : Node Name (UniqueAddress)
                        //  2 : Node Text
                        //  3 : Node ToolTipText

                        S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();

                        // Updates First for Threading Purposes
                        foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                        {
                            S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                            if (!niHeaderCateg.isValid) continue;
                            TreeNode tnNode = niHeaderCateg.FindElement(newTreeNodeInfos[1]);
                            if (tnNode == null) continue;
                            if (tnNode.ToolTipText != newTreeNodeInfos[3]) // For Performance purposes
                            {
                                tnNode.ToolTipText = newTreeNodeInfos[3];
                                tnNode.ForeColor = Color.Purple;
                            }
                            newTreeNodeInfos[0] = null; // To be ignored at creation level
                        }
                        foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
                        {
                            if (newTreeNodeInfos[0] == null) continue;
                            S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                            if (!niHeaderCateg.isValid) continue;
                            if (niHeaderCateg.FindElement(newTreeNodeInfos[1]) != null) continue;
                            TreeNode tnNode = new TreeNode();
                            tnNode.Name = newTreeNodeInfos[1];
                            // Specific Case for OTHER
                            if (newTreeNodeInfos[0] == S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER)) tnNode.Text = newTreeNodeInfos[2];
                            tnNode.ToolTipText = newTreeNodeInfos[3];
                            tnNode.ContextMenuStrip = elemsContextMenuStrip;
                            tnNode.ForeColor = Color.Red;
                            niHeaderCateg.AddNode(tnNode, null, null, null, false, categoryDepth);
                        }
                    }
                    catch (Exception ex)
                    {
                        sadProcessManager.ProcessErrors.Add("Comments elements update has failed.\r\n" + ex.Message);
                    }

                    // Creations only on OTHER (normally)
                    setElementsTreeCategLabel(S6xNavHeaderCategory.OTHER);

                    elemsTreeView.EndUpdate();
                }
                alNewTreeNodesInfos = null;
            }

            outputToolStripMenuItem.Enabled = false;
            
            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Import is done.");
            else sadProcessManager.SetProcessFinished("Import has finished with errors.");
        }

        private void exportCmtFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            if (saveFileDialogCmt.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;
            
            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { saveFileDialogCmt.FileName, tfSADVersionToolStripComboBox.SelectedItem.ToString() };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(exportCmtFileStartProcess);
            processThread.Start();
        }

        private void exportCmtFileStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            string tfSADVersion = string.Empty;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                tfSADVersion = (string)sadProcessManager.Parameters[1];

                if (filePath == null || filePath == string.Empty) throw new Exception();
                if (tfSADVersion == null || tfSADVersion == string.Empty) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            try
            {
                sadProcessManager.SetProcessStarted("Export is starting.");
                TfSADTools.ExportCmtFile(filePath, tfSADVersion, ref sadBin, ref sadS6x, ref sadProcessManager);
                sadProcessManager.SetProcessFinished("Export is done.");
            }
            catch (Exception ex)
            {
                sadProcessManager.ProcessErrors.Add(ex.Message);
                sadProcessManager.SetProcessFailed("Process has failed.");
            }
        }

        private void importXlsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXlsFile(string.Empty);
        }

        private void importXlsFile(string xlsFilePath)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            if (!confirmXdfXlsOffset()) return;

            if (xlsFilePath == string.Empty)
            {
                if (openFileDialogXls.ShowDialog() != DialogResult.OK) return;
                if (!File.Exists(openFileDialogXls.FileName)) return;
                xlsFilePath = openFileDialogXls.FileName;
            }
            else
            {
                if (!File.Exists(xlsFilePath)) return;
            }

            bool forceXlsAlt = false;
            string statusXls = ToolsXls.CheckXls(xlsFilePath, forceXlsAlt);
            string statusXlsxForced = string.Empty;
            if (statusXls != "OK")
            {
                forceXlsAlt = true;
                statusXlsxForced = ToolsXls.CheckXls(xlsFilePath, forceXlsAlt);
            }
            if (statusXls != "OK" && statusXlsxForced != "OK")
            {
                MessageBox.Show("Impossible to open the selected file.\r\nThe related software is not properly installed or the file is corrupted.\r\nPlease look at the error message.\r\n\r\n" + statusXls, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            ArrayList alReservedAddresses = new ArrayList();
            foreach (TreeNode tnNode in elemsTreeView.Nodes["RESERVED"].Nodes) alReservedAddresses.Add(tnNode.Name);

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { xlsFilePath, forceXlsAlt, alReservedAddresses };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(importXlsFileStartProcess);
            processThread.Start();
        }

        private void importXlsFileStartProcess()
        {
            ArrayList alReservedAddresses = null;

            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            bool forceXlsAlt = false;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                forceXlsAlt = (bool)sadProcessManager.Parameters[1];
                alReservedAddresses = (ArrayList)sadProcessManager.Parameters[2];

                if (filePath == null || filePath == string.Empty) throw new Exception();
                if (alReservedAddresses == null) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            sadProcessManager.SetProcessStarted("Import is starting.");

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nReading file.";
            sadProcessManager.ProcessProgressStatus = 10;

            XlsFile xlsFile = new XlsFile();
            xlsFile.Load(filePath, forceXlsAlt);
            if (!xlsFile.Valid)
            {
                sadProcessManager.SetProcessFailed("Xls import has failed.\r\nPlease check file.");
                return;
            }

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nFile loaded.\r\nDifferences analysis.";
            sadProcessManager.ProcessProgressStatus = 30;

            object[] arrSyncRes = sadS6x.readFromFileObject(ref xlsFile, ref sadBin, ref alReservedAddresses);
            xlsFile = null;

            alReservedAddresses = null;

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nFile Analysed.\r\nGenerating updates.";
            sadProcessManager.ProcessProgressStatus = 70;

            ArrayList alNewTreeNodesInfos = new ArrayList();
            ArrayList alNewDupTreeNodesInfos = new ArrayList();

            string[] arrSyncResAddresses = null;

            // S6x Creations
            //  First Creations
            arrSyncResAddresses = (string[])arrSyncRes[1];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), uniqueAddress, ((S6xTable)sadS6x.slTables[uniqueAddress]).Label, ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category2, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), uniqueAddress, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category2, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), uniqueAddress, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category2, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), uniqueAddress, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category2, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slRegisters.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS), uniqueAddress, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Label, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Comments, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Category, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Category2, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            //  Then Duplicates Creations
            arrSyncResAddresses = (string[])arrSyncRes[6];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), duplicateAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).UniqueAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Label, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Comments, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category2, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), duplicateAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).UniqueAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Label, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Comments, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category2, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), duplicateAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).UniqueAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Label, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Comments, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category2, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupStructures.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), duplicateAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).UniqueAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Label, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Comments, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category2, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            // S6x Updates
            arrSyncResAddresses = (string[])arrSyncRes[0];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), uniqueAddress, ((S6xTable)sadS6x.slTables[uniqueAddress]).Label, ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category2, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), uniqueAddress, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category2, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), uniqueAddress, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category2, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), uniqueAddress, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category2, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slRegisters.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS), uniqueAddress, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Label, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Comments, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Category, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Category2, ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            //  Duplicates Updates
            arrSyncResAddresses = (string[])arrSyncRes[5];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), duplicateAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).UniqueAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Label, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Comments, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category2, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), duplicateAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).UniqueAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Label, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Comments, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category2, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), duplicateAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).UniqueAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Label, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Comments, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category2, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupStructures.ContainsKey(duplicateAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), duplicateAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).UniqueAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Label, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Comments, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category2, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            string conflictsMessage = string.Empty;

            arrSyncResAddresses = (string[])arrSyncRes[3];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nTable \"" + ((S6xTable)sadS6x.slTables[uniqueAddress]).Label + "\"";
                if (sadS6x.slFunctions.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nFunction \"" + ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label + "\"";
                if (sadS6x.slScalars.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nScalar \"" + ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label + "\"";
                if (sadS6x.slStructures.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nStructure \"" + ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label + "\"";
                if (sadS6x.slRegisters.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nRegister \"" + ((S6xRegister)sadS6x.slRegisters[uniqueAddress]).Label + "\"";
            }
            arrSyncResAddresses = null;

            if (conflictsMessage != string.Empty) sadProcessManager.ProcessMessages.Add("Following items are in conflict, different external type is not imported :\r\n" + conflictsMessage);

            if (((string[])arrSyncRes[4]).Length > 0) sadProcessManager.ProcessMessages.Add("Some elements are reserved addresses and can not be updated or are generated at binary load and should be updated manually.\r\n\r\nThey have been ignored.");

            arrSyncRes = null;

            sadProcessManager.PostProcessAction = "importXlsFilePostProcess";
            sadProcessManager.PostProcessParameters = new object[] { alNewTreeNodesInfos, alNewDupTreeNodesInfos };

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nFile Analysed.\r\nApplying updates.";
            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void importXlsFilePostProcess()
        {
            if (sadProcessManager == null) return;

            ArrayList alNewTreeNodesInfos = null;
            ArrayList alNewDupTreeNodesInfos = null;

            try
            {
                alNewTreeNodesInfos = (ArrayList)((object[])sadProcessManager.PostProcessParameters)[0];
                alNewDupTreeNodesInfos = (ArrayList)((object[])sadProcessManager.PostProcessParameters)[1];

                if (alNewTreeNodesInfos == null) throw new Exception();
                if (alNewDupTreeNodesInfos == null) throw new Exception();
            }
            catch
            {
                sadProcessManager.ProcessErrors.Add("Invalid post process start.");
                sadProcessManager.SetProcessFailed("Import has failed.");
                GC.Collect();
                return;
            }

            elemsTreeView.BeginUpdate();

            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.STRUCTURES, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.TABLES, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.FUNCTIONS, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.SCALARS, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.REGISTERS, ref s6xNavCategories, ref sadBin, ref sadS6x);

            // To force refresh of categories in related combo boxes
            sharedCategComboBox.Tag = null;
            sharedCateg2ComboBox.Tag = null;
            sharedCateg3ComboBox.Tag = null;

            S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();

            // Updates First for Threading Purposes
            foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
            {
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                TreeNode tnNode = niHeaderCateg.FindElement(newTreeNodeInfos[1]);
                if (tnNode == null) continue;

                if (tnNode.Text != newTreeNodeInfos[2] || tnNode.ToolTipText != newTreeNodeInfos[3]) // For Performance purposes
                {
                    tnNode.Text = newTreeNodeInfos[2];
                    tnNode.ToolTipText = newTreeNodeInfos[3];
                    tnNode.ForeColor = Color.Purple;
                }
                // Check if Node has moved
                S6xNavInfo niNI = new S6xNavInfo(tnNode);
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newTreeNodeInfos[4]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newTreeNodeInfos[5]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newTreeNodeInfos[6]);
                if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                {
                    tnNode.Parent.Nodes.Remove(tnNode);
                    niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);
                }
                niNI = null;
                newTreeNodeInfos[0] = null; // To be ignored at creation level
            }
            // Creations
            foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
            {
                if (newTreeNodeInfos[0] == null) continue;
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                if (niHeaderCateg.FindElement(newTreeNodeInfos[1]) != null) continue;
                TreeNode tnNode = new TreeNode();
                tnNode.Name = newTreeNodeInfos[1];
                tnNode.Text = newTreeNodeInfos[2];
                tnNode.ToolTipText = newTreeNodeInfos[3];
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Red;
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newTreeNodeInfos[4]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newTreeNodeInfos[5]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newTreeNodeInfos[6]);
                niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);
            }
            alNewTreeNodesInfos = null;
            //  Duplicates Updates
            foreach (string[] newDupTreeNodeInfos in alNewDupTreeNodesInfos)
            {
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newDupTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                TreeNode tnNode = niHeaderCateg.FindElementDuplicate(newDupTreeNodeInfos[2], newDupTreeNodeInfos[1]);
                if (tnNode == null) continue;
                if (tnNode.Text != newDupTreeNodeInfos[3] || tnNode.ToolTipText != newDupTreeNodeInfos[4]) // For Performance purposes
                {
                    tnNode.Text = newDupTreeNodeInfos[3];
                    tnNode.ToolTipText = newDupTreeNodeInfos[4];
                    tnNode.ForeColor = Color.Purple;
                }
                // Check if Node has moved
                S6xNavInfo niNI = new S6xNavInfo(tnNode);
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newDupTreeNodeInfos[5]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newDupTreeNodeInfos[6]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newDupTreeNodeInfos[7]);
                if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                {
                    tnNode.Parent.Nodes.Remove(tnNode);
                    niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, true, categoryDepth);
                }
                niNI = null;
                newDupTreeNodeInfos[0] = null; // To be ignored at creation level
            }
            //  Duplicates Creations
            foreach (string[] newDupTreeNodeInfos in alNewDupTreeNodesInfos)
            {
                if (newDupTreeNodeInfos[0] == null) continue;
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newDupTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                S6xNavInfo niMainElement = new S6xNavInfo(niHeaderCateg.FindElement(newDupTreeNodeInfos[2]));
                if (!niHeaderCateg.isValid) continue;
                if (niMainElement.FindElement(newDupTreeNodeInfos[1]) != null) continue;
                TreeNode tnNode = new TreeNode();
                tnNode.Name = newDupTreeNodeInfos[1];
                tnNode.Text = newDupTreeNodeInfos[3];
                tnNode.ToolTipText = newDupTreeNodeInfos[4];
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Red;
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newDupTreeNodeInfos[5]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newDupTreeNodeInfos[6]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newDupTreeNodeInfos[7]);
                niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, true, categoryDepth);
            }
            alNewDupTreeNodesInfos = null;

            setElementsTreeCategLabel(S6xNavHeaderCategory.TABLES);
            setElementsTreeCategLabel(S6xNavHeaderCategory.FUNCTIONS);
            setElementsTreeCategLabel(S6xNavHeaderCategory.SCALARS);
            setElementsTreeCategLabel(S6xNavHeaderCategory.STRUCTURES);
            setElementsTreeCategLabel(S6xNavHeaderCategory.REGISTERS);

            elemsTreeView.EndUpdate();

            outputToolStripMenuItem.Enabled = false;

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Import is done.");
            else sadProcessManager.SetProcessFinished("Import has finished with errors.");

            GC.Collect();
        }

        private void exportXlsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            if (!confirmXdfXlsOffset()) return;

            if (saveFileDialogXlsx.ShowDialog() != DialogResult.OK) return;

            string xlsFilePath = saveFileDialogXlsx.FileName;
            bool forceXlsAlt = false;
            if (File.Exists(xlsFilePath))
            {
                string statusXls = ToolsXls.CheckXls(xlsFilePath, forceXlsAlt);
                string statusXlsxForced = string.Empty;
                if (statusXls != "OK")
                {
                    forceXlsAlt = true;
                    statusXlsxForced = ToolsXls.CheckXls(xlsFilePath, forceXlsAlt);
                }
                if (statusXls != "OK" && statusXlsxForced != "OK")
                {
                    MessageBox.Show("Impossible to use the selected file.\r\nThe related software is not properly installed or the file is corrupted.\r\nPlease look at the error message.\r\n\r\n" + statusXls, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { xlsFilePath, forceXlsAlt };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(exportXlsFileStartProcess);
            processThread.Start();
        }

        private void exportXlsFileStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            bool forceXlsAlt = false;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                forceXlsAlt = (bool)sadProcessManager.Parameters[1];

                if (filePath == null || filePath == string.Empty) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            XlsFile xlsFile = null;
            object[] arrSyncRes = null;

            sadProcessManager.SetProcessStarted("Export is starting.");

            if (File.Exists(filePath))
            {
                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nReading file.";
                sadProcessManager.ProcessProgressStatus = 10;

                xlsFile = new XlsFile();
                xlsFile.Load(filePath, forceXlsAlt);
                if (!xlsFile.Valid)
                {
                    sadProcessManager.SetProcessFailed("Xls matching has failed.\r\nPlease check destination file.");
                    return;
                }

                // Xlsx only Original file autmatic Backup, for Xls, nothing is overwritten
                if (Path.GetExtension(filePath).ToUpper() == ".XLSX")
                {
                    try
                    {
                        File.Copy(filePath, filePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                    }
                    catch
                    {
                        xlsFile = null;
                        sadProcessManager.SetProcessFailed("Xls backup has failed.\r\nNo other action will be managed.");
                        return;
                    }
                }

                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nFile loaded.\r\nDifferences analysis.";
                sadProcessManager.ProcessProgressStatus = 30;

                arrSyncRes = sadS6x.writeToFileObject(ref xlsFile, ref sadBin);
            }
            else
            {
                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nCreating file.";
                sadProcessManager.ProcessProgressStatus = 10;

                xlsFile = new XlsFile();
                xlsFile.Create(filePath, ref sadBin);

                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nFile created.\r\nOutput preparation.";
                sadProcessManager.ProcessProgressStatus = 30;

                sadS6x.writeToFileObject(ref xlsFile, ref sadBin);
                arrSyncRes = new object[] { new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { } };
            }

            sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nOutput prepared.\r\nWriting file.";
            sadProcessManager.ProcessProgressStatus = 70;

            string saveFilePath = xlsFile.FilePath;
            if (Path.GetExtension(saveFilePath).ToUpper() != ".XLSX") saveFilePath = Path.ChangeExtension(saveFilePath, ".xlsx");

            List<string> lstErrors = new List<string>();
            if (!xlsFile.Save(saveFilePath, ref lstErrors))
            {
                sadProcessManager.SetProcessFailed("Xls file update has failed.");
                xlsFile = null;
                return;
            }
            xlsFile = null;

            if (lstErrors.Count > 0) sadProcessManager.ProcessErrors.AddRange(lstErrors);
            lstErrors = null;

            sadProcessManager.PostProcessAction = "exportXlsFilePostProcess";
            sadProcessManager.PostProcessParameters = new object[] { arrSyncRes };

            sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nOutput done.\r\nApplying updates.";
            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void exportXlsFilePostProcess()
        {
            if (sadProcessManager == null) return;

            object[] arrSyncRes = null;

            try
            {
                arrSyncRes = (object[])((object[])sadProcessManager.PostProcessParameters)[0];

                if (arrSyncRes == null) throw new Exception();
            }
            catch
            {
                sadProcessManager.ProcessErrors.Add("Invalid post process start.");
                sadProcessManager.SetProcessFailed("Export has failed.");
                GC.Collect();
                return;
            }

            elemsTreeView.BeginUpdate();

            string[] arrSyncResAddresses = null;

            // S6x Updates to be applied to Tree View - No Creation
            arrSyncResAddresses = (string[])arrSyncRes[0];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                S6xNavInfo niHeaderCateg = null;
                TreeNode tnNode = null;
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xTable)sadS6x.slTables[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slRegisters.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xStructure)sadS6x.slRegisters[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xStructure)sadS6x.slRegisters[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
            }
            arrSyncResAddresses = null;

            // Same thing for duplicates
            arrSyncResAddresses = (string[])arrSyncRes[5];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                S6xNavInfo niHeaderCateg = null;
                TreeNode tnNode = null;
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                    if (!niHeaderCateg.isValid) continue;
                    S6xTable s6xObject = (S6xTable)sadS6x.slDupTables[duplicateAddress];
                    tnNode = niHeaderCateg.FindElementDuplicate(s6xObject.UniqueAddress, duplicateAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                    if (!niHeaderCateg.isValid) continue;
                    S6xFunction s6xObject = (S6xFunction)sadS6x.slDupFunctions[duplicateAddress];
                    tnNode = niHeaderCateg.FindElementDuplicate(s6xObject.UniqueAddress, duplicateAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                    if (!niHeaderCateg.isValid) continue;
                    S6xScalar s6xObject = (S6xScalar)sadS6x.slDupScalars[duplicateAddress];
                    tnNode = niHeaderCateg.FindElementDuplicate(s6xObject.UniqueAddress, duplicateAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
            }
            arrSyncResAddresses = null;

            elemsTreeView.EndUpdate();

            arrSyncRes = null;

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Export is done.");
            else sadProcessManager.SetProcessFinished("Export has finished with errors.");

            GC.Collect();
        }

        // 20210114 - PYM - XDF/XLS Offset check to prevent issues
        //  When changing binary, essentially on banks order, XDF offset can become obsolete
        //  User is now warned with automatic possibility to use recommanded Offset
        private bool confirmXdfXlsOffset()
        {
            if (sadBin == null) return false;

            int recommendedOffset = SADDef.EecBankStartAddress;
            bool recommandedOffsetSubtract = true;
            if (sadBin.isValid)
            {
                if (sadBin.Calibration.BankAddressBinInt >= SADDef.EecBankStartAddress)
                {
                    recommendedOffset = sadBin.Calibration.BankAddressBinInt - SADDef.EecBankStartAddress;
                    recommandedOffsetSubtract = false;
                }
                else
                {
                    recommendedOffset = sadBin.Calibration.BankAddressBinInt + SADDef.EecBankStartAddress;
                    recommandedOffsetSubtract = true;
                }
            }

            int givenOffset = SADDef.EecBankStartAddress;
            bool givenOffsetSubtract = true;
            if (sadS6x.Properties.XdfBaseOffset != null || sadS6x.Properties.XdfBaseOffset != string.Empty)
            {
                try
                {
                    givenOffset = Convert.ToInt32(sadS6x.Properties.XdfBaseOffset, 16);
                    givenOffsetSubtract = sadS6x.Properties.XdfBaseOffsetSubtract;
                }
                catch { }
            }

            if (givenOffset != recommendedOffset || givenOffsetSubtract != recommandedOffsetSubtract)
            {
                string sMessage = "Defined XDF/XLS Offset does not match recommended one.\r\n";
                sMessage += "Recommanded offset is for starting on Bank 1.\r\n";
                sMessage += "It is :\r\n\t0x" + string.Format("{0:x1}", recommendedOffset) + " " + (recommandedOffsetSubtract ? "subtracted" : "added") + ".\r\n";
                sMessage += "Defined one is :\r\n\t0x" + string.Format("{0:x1}", givenOffset) + " " + (givenOffsetSubtract ? "subtracted" : "added") + ".\r\n";
                sMessage += "\r\nUpdate properties with recommanded offset before processing ?";
                DialogResult dResult = MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dResult == DialogResult.Yes)
                {
                    sadS6x.Properties.XdfBaseOffset = string.Format("{0:x1}", recommendedOffset);
                    sadS6x.Properties.XdfBaseOffsetSubtract = recommandedOffsetSubtract;
                    sadS6x.isSaved = false;

                    s6xPropertiesXdfBaseOffsetTextBox.Text = sadS6x.Properties.XdfBaseOffset;
                    s6xPropertiesXdfBaseOffsetCheckBox.Checked = sadS6x.Properties.XdfBaseOffsetSubtract;
                }
                else if (dResult == DialogResult.No)
                {
                    // No Change
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void importXdfFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXdfFile(string.Empty, false);
        }

        private void importXdfCategsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXdfFile(string.Empty, true);
        }

        private void importXdfFile(string xFilePath, bool categsOnly)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            // 20210114 - PYM - Offset check to prevent issues
            if (!confirmXdfXlsOffset()) return;

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

            ArrayList alReservedAddresses = new ArrayList();
            foreach (TreeNode tnNode in elemsTreeView.Nodes["RESERVED"].Nodes) alReservedAddresses.Add(tnNode.Name);

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { xFilePath, alReservedAddresses, categsOnly };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(importXdfFileStartProcess);
            processThread.Start();
        }

        private void importXdfFileStartProcess()
        {
            ArrayList alReservedAddresses = null;

            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            bool categsOnly = false;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
                alReservedAddresses = (ArrayList)sadProcessManager.Parameters[1];
                categsOnly = (bool)sadProcessManager.Parameters[2];

                if (filePath == null || filePath == string.Empty) throw new Exception();
                if (alReservedAddresses == null) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            sadProcessManager.SetProcessStarted("Import is starting.");

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nReading file.";
            sadProcessManager.ProcessProgressStatus = 10;

            XdfFile xdfFile = (XdfFile)ToolsXml.DeserializeFile(filePath, typeof(XdfFile));
            if (xdfFile == null)
            {
                sadProcessManager.SetProcessFailed("Xdf import has failed.\r\nPlease check it is not encrypted.");
                return;
            }

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nFile loaded.\r\nDifferences analysis.";
            sadProcessManager.ProcessProgressStatus = 30;
            
            object[] arrSyncRes = sadS6x.readFromFileObject(ref xdfFile, ref sadBin, ref alReservedAddresses, categsOnly);
            xdfFile = null;

            alReservedAddresses = null;

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nFile Analysed.\r\nGenerating updates.";
            sadProcessManager.ProcessProgressStatus = 70;

            ArrayList alNewTreeNodesInfos = new ArrayList();
            ArrayList alNewDupTreeNodesInfos = new ArrayList();

            string[] arrSyncResAddresses = null;

            // S6x Creations
            //  First Creations
            arrSyncResAddresses = (string[])arrSyncRes[1];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), uniqueAddress, ((S6xTable)sadS6x.slTables[uniqueAddress]).Label, ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category2, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), uniqueAddress, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category2, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), uniqueAddress, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category2, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), uniqueAddress, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category2, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            //  Then Duplicates Creations
            arrSyncResAddresses = (string[])arrSyncRes[6];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), duplicateAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).UniqueAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Label, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Comments, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category2, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), duplicateAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).UniqueAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Label, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Comments, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category2, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), duplicateAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).UniqueAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Label, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Comments, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category2, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupStructures.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), duplicateAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).UniqueAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Label, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Comments, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category2, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            // S6x Updates
            arrSyncResAddresses = (string[])arrSyncRes[0];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), uniqueAddress, ((S6xTable)sadS6x.slTables[uniqueAddress]).Label, ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category2, ((S6xTable)sadS6x.slTables[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), uniqueAddress, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category2, ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), uniqueAddress, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category2, ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Category3 });
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), uniqueAddress, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Comments, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category2, ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            //  Duplicates Updates
            arrSyncResAddresses = (string[])arrSyncRes[5];
            foreach (string duplicateAddress in arrSyncResAddresses)
            {
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), duplicateAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).UniqueAddress, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Label, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Comments, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category2, ((S6xTable)sadS6x.slDupTables[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), duplicateAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).UniqueAddress, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Label, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Comments, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category2, ((S6xFunction)sadS6x.slDupFunctions[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    alNewDupTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), duplicateAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).UniqueAddress, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Label, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Comments, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category2, ((S6xScalar)sadS6x.slDupScalars[duplicateAddress]).Category3 });
                }
                else if (sadS6x.slDupStructures.ContainsKey(duplicateAddress))
                {
                    alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), duplicateAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).UniqueAddress, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Label, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Comments, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category2, ((S6xStructure)sadS6x.slDupStructures[duplicateAddress]).Category3 });
                }
            }
            arrSyncResAddresses = null;

            string conflictsMessage = string.Empty;

            arrSyncResAddresses = (string[])arrSyncRes[3];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                if (sadS6x.slTables.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nTable \"" + ((S6xTable)sadS6x.slTables[uniqueAddress]).Label + "\"";
                if (sadS6x.slFunctions.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nFunction \"" + ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label + "\"";
                if (sadS6x.slScalars.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nScalar \"" + ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label + "\"";
                if (sadS6x.slStructures.ContainsKey(uniqueAddress)) conflictsMessage += "\r\nStructure \"" + ((S6xStructure)sadS6x.slStructures[uniqueAddress]).Label + "\"";
            }
            arrSyncResAddresses = null;

            if (conflictsMessage != string.Empty) sadProcessManager.ProcessMessages.Add("Following items are in conflict, different external type is not imported :\r\n" + conflictsMessage);

            if (((string[])arrSyncRes[4]).Length > 0) sadProcessManager.ProcessMessages.Add("Some elements are reserved addresses and can not be updated or are generated at binary load and should be updated manually.\r\n\r\nThey have been ignored.");

            arrSyncRes = null;

            sadProcessManager.PostProcessAction = "importXdfFilePostProcess";
            sadProcessManager.PostProcessParameters = new object[] { alNewTreeNodesInfos, alNewDupTreeNodesInfos };

            sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nFile Analysed.\r\nApplying updates.";
            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void importXdfFilePostProcess()
        {
            if (sadProcessManager == null) return;

            ArrayList alNewTreeNodesInfos = null;
            ArrayList alNewDupTreeNodesInfos = null;

            try
            {
                alNewTreeNodesInfos = (ArrayList)((object[])sadProcessManager.PostProcessParameters)[0];
                alNewDupTreeNodesInfos = (ArrayList)((object[])sadProcessManager.PostProcessParameters)[1];

                if (alNewTreeNodesInfos == null) throw new Exception();
                if (alNewDupTreeNodesInfos == null) throw new Exception();
            }
            catch
            {
                sadProcessManager.ProcessErrors.Add("Invalid post process start.");
                sadProcessManager.SetProcessFailed("Import has failed.");
                GC.Collect();
                return;
            }

            elemsTreeView.BeginUpdate();

            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.STRUCTURES, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.TABLES, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.FUNCTIONS, ref s6xNavCategories, ref sadBin, ref sadS6x);
            S6xNav.s6xNavCategoriesReset(S6xNavHeaderCategory.SCALARS, ref s6xNavCategories, ref sadBin, ref sadS6x);

            // To force refresh of categories in related combo boxes
            sharedCategComboBox.Tag = null;
            sharedCateg2ComboBox.Tag = null;
            sharedCateg3ComboBox.Tag = null;

            S6xNavCategoryDepth categoryDepth = getS6xNavCategoryDepth();

            // Updates First for Threading Purposes
            foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
            {
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                TreeNode tnNode = niHeaderCateg.FindElement(newTreeNodeInfos[1]);
                if (tnNode == null) continue;

                if (tnNode.Text != newTreeNodeInfos[2] || tnNode.ToolTipText != newTreeNodeInfos[3]) // For Performance purposes
                {
                    tnNode.Text = newTreeNodeInfos[2];
                    tnNode.ToolTipText = newTreeNodeInfos[3];
                    tnNode.ForeColor = Color.Purple;
                }
                // Check if Node has moved
                S6xNavInfo niNI = new S6xNavInfo(tnNode);
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newTreeNodeInfos[4]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newTreeNodeInfos[5]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newTreeNodeInfos[6]);
                if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                {
                    tnNode.Parent.Nodes.Remove(tnNode);
                    niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);
                }
                niNI = null;
                newTreeNodeInfos[0] = null; // To be ignored at creation level
            }
            // Creations
            foreach (string[] newTreeNodeInfos in alNewTreeNodesInfos)
            {
                if (newTreeNodeInfos[0] == null) continue;
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                if (niHeaderCateg.FindElement(newTreeNodeInfos[1]) != null) continue;
                TreeNode tnNode = new TreeNode();
                tnNode.Name = newTreeNodeInfos[1];
                tnNode.Text = newTreeNodeInfos[2];
                tnNode.ToolTipText = newTreeNodeInfos[3];
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Red;
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newTreeNodeInfos[4]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newTreeNodeInfos[5]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newTreeNodeInfos[6]);
                niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, false, categoryDepth);
            }
            alNewTreeNodesInfos = null;
            //  Duplicates Updates
            foreach (string[] newDupTreeNodeInfos in alNewDupTreeNodesInfos)
            {
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newDupTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                TreeNode tnNode = niHeaderCateg.FindElementDuplicate(newDupTreeNodeInfos[2], newDupTreeNodeInfos[1]);
                if (tnNode == null) continue;
                if (tnNode.Text != newDupTreeNodeInfos[3] || tnNode.ToolTipText != newDupTreeNodeInfos[4]) // For Performance purposes
                {
                    tnNode.Text = newDupTreeNodeInfos[3];
                    tnNode.ToolTipText = newDupTreeNodeInfos[4];
                    tnNode.ForeColor = Color.Purple;
                }
                // Check if Node has moved
                S6xNavInfo niNI = new S6xNavInfo(tnNode);
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newDupTreeNodeInfos[5]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newDupTreeNodeInfos[6]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newDupTreeNodeInfos[7]);
                if (niNI.Category != navCateg1 || niNI.Category2 != navCateg2 || niNI.Category3 != navCateg3)
                {
                    tnNode.Parent.Nodes.Remove(tnNode);
                    niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, true, categoryDepth);
                }
                niNI = null;
                newDupTreeNodeInfos[0] = null; // To be ignored at creation level
            }
            //  Duplicates Creations
            foreach (string[] newDupTreeNodeInfos in alNewDupTreeNodesInfos)
            {
                if (newDupTreeNodeInfos[0] == null) continue;
                S6xNavInfo niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[newDupTreeNodeInfos[0]]);
                if (!niHeaderCateg.isValid) continue;
                S6xNavInfo niMainElement = new S6xNavInfo(niHeaderCateg.FindElement(newDupTreeNodeInfos[2]));
                if (!niHeaderCateg.isValid) continue;
                if (niMainElement.FindElement(newDupTreeNodeInfos[1]) != null) continue;
                TreeNode tnNode = new TreeNode();
                tnNode.Name = newDupTreeNodeInfos[1];
                tnNode.Text = newDupTreeNodeInfos[3];
                tnNode.ToolTipText = newDupTreeNodeInfos[4];
                tnNode.ContextMenuStrip = elemsContextMenuStrip;
                tnNode.ForeColor = Color.Red;
                S6xNavCategory navCateg1 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.ONE, true, newDupTreeNodeInfos[5]);
                S6xNavCategory navCateg2 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.TWO, true, newDupTreeNodeInfos[6]);
                S6xNavCategory navCateg3 = s6xNavCategories.getCategory(niHeaderCateg.HeaderCategory, S6xNavCategoryLevel.THREE, true, newDupTreeNodeInfos[7]);
                niHeaderCateg.AddNode(tnNode, navCateg1, navCateg2, navCateg3, true, categoryDepth);
            }
            alNewDupTreeNodesInfos = null;

            setElementsTreeCategLabel(S6xNavHeaderCategory.TABLES);
            setElementsTreeCategLabel(S6xNavHeaderCategory.FUNCTIONS);
            setElementsTreeCategLabel(S6xNavHeaderCategory.SCALARS);
            setElementsTreeCategLabel(S6xNavHeaderCategory.STRUCTURES);

            elemsTreeView.EndUpdate();

            outputToolStripMenuItem.Enabled = false;

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Import is done.");
            else sadProcessManager.SetProcessFinished("Import has finished with errors.");

            GC.Collect();
        }

        // exportXdfResetToolStripMenuItem_Click
        // Permits to reset XdfUniqueId's to restart on a clean base
        private void exportXdfResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadS6x == null) return;
            
            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            elemsTreeView.BeginUpdate();

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
                TreeNode tnNode = elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)].Nodes[s6xObject.UniqueAddress];
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
                TreeNode tnNode = elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)].Nodes[s6xObject.UniqueAddress];
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
                TreeNode tnNode = elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)].Nodes[s6xObject.UniqueAddress];
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

            elemsTreeView.EndUpdate();

            sadS6x.isSaved = false;

            Cursor = processPreviousCursor;
        }

        private void exportXdfFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            // 20210114 - PYM - Offset check to prevent issues
            if (!confirmXdfXlsOffset()) return;

            if (saveFileDialogXdf.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { saveFileDialogXdf.FileName };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(exportXdfFileStartProcess);
            processThread.Start();
        }

        private void exportXdfFileStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];
             
                if (filePath == null || filePath == string.Empty) throw new Exception();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            XdfFile xdfFile = null;
            StreamWriter sWri = null;
            object[] arrSyncRes = null;

            sadProcessManager.SetProcessStarted("Export is starting.");

            SettingsLst tpSettings = (SettingsLst)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.settingsTunerProImpExpFileName, typeof(SettingsLst));
            if (tpSettings == null) tpSettings = new SettingsLst();
            ToolsSettings.Update(ref tpSettings, "TUNERPROIMPEXP");

            if (File.Exists(filePath))
            {
                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nReading file.";
                sadProcessManager.ProcessProgressStatus = 10;

                xdfFile = (XdfFile)ToolsXml.DeserializeFile(filePath, typeof(XdfFile));
                if (xdfFile == null)
                {
                    sadProcessManager.SetProcessFailed("Xdf matching has failed.\r\nPlease check destination file is not encrypted.");
                    return;
                }

                // Xdf Original file autmatic Backup
                try
                {
                    File.Copy(filePath, filePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    xdfFile = null;
                    sadProcessManager.SetProcessFailed("Xdf backup has failed.\r\nNo other action will be managed.");
                    return;
                }

                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nFile loaded.\r\nDifferences analysis.";
                sadProcessManager.ProcessProgressStatus = 30;

                arrSyncRes = sadS6x.writeToFileObject(ref xdfFile, ref sadBin, ref tpSettings);
            }
            else
            {
                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nCreating file.";
                sadProcessManager.ProcessProgressStatus = 10;

                xdfFile = new XdfFile(ref sadBin);

                sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nFile created.\r\nOutput preparation.";
                sadProcessManager.ProcessProgressStatus = 30;

                sadS6x.writeToFileObject(ref xdfFile, ref sadBin, ref tpSettings);
                arrSyncRes = new object[] { new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { }, new string[] { } };
            }

            sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nOutput prepared.\r\nWriting file.";
            sadProcessManager.ProcessProgressStatus = 70;
            
            // UnicodeEncoding(false, true) works fine, UTF-8 with BOM has issues, like ASCII
            try
            {
                sWri = new StreamWriter(filePath, false, new UnicodeEncoding(false, true));
                ToolsXml.Serialize(ref sWri, xdfFile);
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Xdf file update has failed.");
                return;
            }
            finally
            {
                try { sWri.Close(); }
                catch { }
                try { sWri.Dispose(); }
                catch { }
                sWri = null;
                xdfFile = null;
            }

            sadProcessManager.PostProcessAction = "exportXdfFilePostProcess";
            sadProcessManager.PostProcessParameters = new object[] { arrSyncRes };

            sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nOutput done.\r\nApplying updates.";
            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void exportXdfFilePostProcess()
        {
            if (sadProcessManager == null) return;

            object[] arrSyncRes = null;

            try
            {
                arrSyncRes = (object[])((object[])sadProcessManager.PostProcessParameters)[0];

                if (arrSyncRes == null) throw new Exception();
            }
            catch
            {
                sadProcessManager.ProcessErrors.Add("Invalid post process start.");
                sadProcessManager.SetProcessFailed("Export has failed.");
                GC.Collect();
                return;
            }

            elemsTreeView.BeginUpdate();

            string[] arrSyncResAddresses = null;

            // S6x Updates to be applied to Tree View - No Creation
            arrSyncResAddresses = (string[])arrSyncRes[0];
            foreach (string uniqueAddress in arrSyncResAddresses)
            {
                S6xNavInfo niHeaderCateg = null;
                TreeNode tnNode = null;
                if (sadS6x.slTables.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xTable)sadS6x.slTables[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xTable)sadS6x.slTables[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slFunctions.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xFunction)sadS6x.slFunctions[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slScalars.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Label;
                    tnNode.ToolTipText = ((S6xScalar)sadS6x.slScalars[uniqueAddress]).Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slStructures.ContainsKey(uniqueAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES)]);
                    if (!niHeaderCateg.isValid) continue;
                    tnNode = niHeaderCateg.FindElement(uniqueAddress);
                    if (tnNode == null) continue;
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
                S6xNavInfo niHeaderCateg = null;
                TreeNode tnNode = null;
                if (sadS6x.slDupTables.ContainsKey(duplicateAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                    if (!niHeaderCateg.isValid) continue;
                    S6xTable s6xObject = (S6xTable)sadS6x.slDupTables[duplicateAddress];
                    tnNode = niHeaderCateg.FindElementDuplicate(s6xObject.UniqueAddress, duplicateAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slDupFunctions.ContainsKey(duplicateAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                    if (!niHeaderCateg.isValid) continue;
                    S6xFunction s6xObject = (S6xFunction)sadS6x.slDupFunctions[duplicateAddress];
                    tnNode = niHeaderCateg.FindElementDuplicate(s6xObject.UniqueAddress, duplicateAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
                else if (sadS6x.slDupScalars.ContainsKey(duplicateAddress))
                {
                    niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                    if (!niHeaderCateg.isValid) continue;
                    S6xScalar s6xObject = (S6xScalar)sadS6x.slDupScalars[duplicateAddress];
                    tnNode = niHeaderCateg.FindElementDuplicate(s6xObject.UniqueAddress, duplicateAddress);
                    if (tnNode == null) continue;
                    tnNode.Text = s6xObject.Label;
                    tnNode.ToolTipText = s6xObject.Comments;
                    tnNode.ForeColor = Color.Purple;
                }
            }
            arrSyncResAddresses = null;

            elemsTreeView.EndUpdate();

            arrSyncRes = null;

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Export is done.");
            else sadProcessManager.SetProcessFinished("Export has finished with errors.");

            GC.Collect();
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
            else if (nextElemS6xNavInfo != null)
            {
                uniqueAddress = nextElemS6xNavInfo.Node.Name;
                s6xRoutine = (S6xRoutine)sadS6x.slRoutines[uniqueAddress];
            }

            if (s6xRoutine == null) tempRoutine = new S6xRoutine();
            else tempRoutine = s6xRoutine.Clone();
            s6xRoutine = null;

            tempRoutine.Label = routineLabelTextBox.Text;

            RoutineForm routineForm = new RoutineForm(ref sadS6x, ref tempRoutine, ref elemsTreeViewStateImageList, ref s6xNavCategories);
            bool updatedRoutine = routineForm.ShowDialog() == DialogResult.OK;
            routineForm = null;

            if (updatedRoutine)
            {
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
            }

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
            else if (nextElemS6xNavInfo != null)
            {
                uniqueKey = nextElemS6xNavInfo.Node.Name;
                s6xSig = (S6xSignature)sadS6x.slSignatures[uniqueKey];
            }

            if (s6xSig == null) tempSig = new S6xSignature();
            else tempSig = s6xSig.Clone();
            s6xSig = null;

            tempSig.Label = signatureLabelTextBox.Text;
            tempSig.Signature = signatureSigTextBox.Text;

            SigForm sigForm = new SigForm(ref sadS6x, ref tempSig, ref elemsTreeViewStateImageList, ref s6xNavCategories);
            bool updatedSignature = sigForm.ShowDialog() == DialogResult.OK;
            sigForm = null;

            if (updatedSignature)
            {
                signatureSigTextBox.Text = tempSig.Signature;
                signatureAdvCheckBox.Checked = tempSig.isAdvanced;

                // To be reused on Update
                signatureAdvButton.Tag = tempSig;

                elemProperties_Modified(signatureAdvButton, new EventArgs());
            }

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
            else if (nextElemS6xNavInfo != null)
            {
                uniqueKey = nextElemS6xNavInfo.Node.Name;
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

            ElemSigForm eSigForm = new ElemSigForm(ref sadS6x, ref tempESig, ref elemsTreeViewStateImageList, ref s6xNavCategories);
            bool updatedSignature = eSigForm.ShowDialog() == DialogResult.OK;
            eSigForm = null;

            if (updatedSignature)
            {
                elementSignatureSigTextBox.Text = tempESig.Signature;
                if (tempESig.Scalar != null) elementSignatureTypeComboBox.SelectedIndex = 0;
                else if (tempESig.Function != null) elementSignatureTypeComboBox.SelectedIndex = 1;
                else if (tempESig.Table != null) elementSignatureTypeComboBox.SelectedIndex = 2;
                else if (tempESig.Structure != null) elementSignatureTypeComboBox.SelectedIndex = 3;

                // To be reused on Update
                elementSignatureElemButton.Tag = tempESig;

                elemProperties_Modified(elementSignatureElemButton, new EventArgs());
            }
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
            else if (nextElemS6xNavInfo != null)
            {
                uniqueAddress = nextElemS6xNavInfo.Node.Name;
                s6xScalar = (S6xScalar)sadS6x.slScalars[uniqueAddress];
            }

            if (s6xScalar == null) tempScalar = new S6xScalar();
            else tempScalar = s6xScalar.Clone();
            s6xScalar = null;

            tempScalar.Label = scalarLabelTextBox.Text;
            tempScalar.Byte = scalarByteCheckBox.Checked;

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref sadS6x, ref tempScalar, ref elemsTreeViewStateImageList, ref s6xNavCategories);
            bool updatedBitFlags = bitFlagsForm.ShowDialog() == DialogResult.OK;
            bitFlagsForm = null;

            if (updatedBitFlags)
            {
                scalarBitFlagsCheckBox.Checked = tempScalar.isBitFlags;

                // To be reused on Update
                scalarBitFlagsButton.Tag = tempScalar;

                elemProperties_Modified(scalarBitFlagsButton, new EventArgs());
            }

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
            else if (nextElemS6xNavInfo != null)
            {
                uniqueAddress = nextElemS6xNavInfo.Node.Name;
                s6xReg = (S6xRegister)sadS6x.slRegisters[uniqueAddress];
            }

            if (s6xReg == null) tempReg = new S6xRegister();
            else tempReg = s6xReg.Clone();
            s6xReg = null;

            tempReg.Label = regLabelTextBox.Text;

            BitFlagsForm bitFlagsForm = new BitFlagsForm(ref sadS6x, ref tempReg, ref elemsTreeViewStateImageList, ref s6xNavCategories);
            bool updatedBitFlags = bitFlagsForm.ShowDialog() == DialogResult.OK;
            bitFlagsForm = null;

            if (updatedBitFlags)
            {
                regBitFlagsCheckBox.Checked = tempReg.isBitFlags;

                // To be reused on Update
                regBitFlagsButton.Tag = tempReg;

                elemProperties_Modified(regBitFlagsButton, new EventArgs());
            }

            tempReg = null;
        }

        private void searchObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (searchForm == null)
            {
                searchForm = new SearchForm(ref sadS6x, ref elemsTreeView);
                searchForm.FormClosed += new FormClosedEventHandler(searchForm_FormClosed);
            }
            searchForm.Show();
            searchForm.Focus();

            //new SearchForm(ref sadS6x, ref elemsTreeView).ShowDialog();
        }

        private void searchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            searchForm = null;
        }

        private void compareBinariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compareBinariesToolStripMenuItem.Enabled = false;
            compareBinariesDifDefToolStripMenuItem.Enabled = false;
            compareS6xToolStripMenuItem.Enabled = false;

            if (compareForm == null)
            {
                compareForm = new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, ref elemContextMenuStrip, true, true);
                compareForm.FormClosed += new FormClosedEventHandler(compareForm_FormClosed);
            }
            compareForm.Show();
            compareForm.Focus();

            //new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, true, true).ShowDialog();
        }

        private void compareBinariesDifDefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compareBinariesToolStripMenuItem.Enabled = false;
            compareBinariesDifDefToolStripMenuItem.Enabled = false;
            compareS6xToolStripMenuItem.Enabled = false;

            if (compareForm == null)
            {
                compareForm = new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, ref elemContextMenuStrip, true, false);
                compareForm.FormClosed += new FormClosedEventHandler(compareForm_FormClosed);
            }
            compareForm.Show();
            compareForm.Focus();

            //new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, true, false).ShowDialog();
        }

        private void compareS6xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compareBinariesToolStripMenuItem.Enabled = false;
            compareBinariesDifDefToolStripMenuItem.Enabled = false;
            compareS6xToolStripMenuItem.Enabled = false;

            if (compareForm == null)
            {
                compareForm = new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, ref elemContextMenuStrip, false, false);
                compareForm.FormClosed += new FormClosedEventHandler(compareForm_FormClosed);
            }
            compareForm.Show();
            compareForm.Focus();

            //new CompareForm(ref sadBin, ref sadS6x, ref elemsTreeView, ref elemsContextMenuStrip, false, false).ShowDialog();
        }

        private void compareForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            compareForm = null;
        }

        private void searchSignatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SearchSignatureForm(ref sadBin).ShowDialog();
        }

        private void routinesComparisonSkeletonExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadBin == null) return;
            if (!sadBin.isDisassembled) return;

            if (!confirmDirtyProperies()) return;

            FileInfo fiFI = new FileInfo(binaryFilePath);
            try { saveFileDialogSkt.FileName = fiFI.Name.Substring(0, fiFI.Name.Length - fiFI.Extension.Length) + ".skt"; }
            catch { }
            fiFI = null;

            if (saveFileDialogSkt.ShowDialog() != DialogResult.OK) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                SortedList slResult = ToolsRoutinesComp.getRoutinesComparisonSkeleton(ref sadBin);
                ToolsRoutinesComp.exportRoutinesComparisonSkeleton(ref slResult, saveFileDialogSkt.FileName);
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
            routinesComparisonBinariesCompareToolStripMenuItem.Enabled = false;
            routinesComparisonSkeletonsCompareToolStripMenuItem.Enabled = false;
            
            if (compareRoutinesForm == null)
            {
                compareRoutinesForm = new CompareRoutinesForm(ref sadBin, ref elemsTreeView, ref elemsContextMenuStrip, true);
                compareRoutinesForm.FormClosed += new FormClosedEventHandler(compareRoutinesForm_FormClosed);
            }
            compareRoutinesForm.Show();
            compareRoutinesForm.Focus();

            //new CompareRoutinesForm(ref sadBin, ref elemsTreeView, ref elemsContextMenuStrip, true).ShowDialog();
        }

        private void routinesComparisonBinariesCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            routinesComparisonBinariesCompareToolStripMenuItem.Enabled = false;
            routinesComparisonSkeletonsCompareToolStripMenuItem.Enabled = false;

            if (compareRoutinesForm == null)
            {
                compareRoutinesForm = new CompareRoutinesForm(ref sadBin, ref elemsTreeView, ref elemsContextMenuStrip, false);
                compareRoutinesForm.FormClosed += new FormClosedEventHandler(compareRoutinesForm_FormClosed);
            }
            compareRoutinesForm.Show();
            compareRoutinesForm.Focus();

            //new CompareRoutinesForm(ref sadBin, ref elemsTreeView, ref elemsContextMenuStrip, false).ShowDialog();
        }

        private void compareRoutinesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            compareRoutinesForm = null;

            routinesComparisonBinariesCompareToolStripMenuItem.Enabled = true;
            routinesComparisonSkeletonsCompareToolStripMenuItem.Enabled = true;
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
            MessageBox.Show(SharedUI.AboutRoutinesComparisonSkeletons(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Question);
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

        private void repoOBDIErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("OBDI Error Codes", Application.StartupPath + "\\" + SADDef.repoFileNameOBDIErrors, "OBDIERRORS", ref sadS6x).ShowDialog();

            initRepositories();
        }

        private void repoODBIIErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RepositoryForm("ODBII Error Codes", Application.StartupPath + "\\" + SADDef.repoFileNameOBDIIErrors, "ODBIIERRORS", ref sadS6x).ShowDialog();

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
                case "regUnitsTextBox":
                case "tableScaleTextBox":
                case "functionScaleInputTextBox":
                case "functionScaleOutputTextBox":
                case "scalarScaleTextBox":
                case "regScaleTextBox":
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
                case "regUnitsTextBox":
                case "regScaleTextBox":
                    searchLabel = regUnitsTextBox.Text;
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
                case "regUnitsTextBox":
                    repoRepository = repoUnits;
                    break;
                case "tableScaleTextBox":
                case "functionScaleInputTextBox":
                case "functionScaleOutputTextBox":
                case "scalarScaleTextBox":
                case "regScaleTextBox":
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
                case "regUnitsTextBox":
                    regUnitsTextBox.Text = sSLabel;
                    elemProperties_Modified(regUnitsTextBox, EventArgs.Empty);
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
                case "regScaleTextBox":
                    regScaleTextBox.Text = sLLabel;
                    elemProperties_Modified(regScaleTextBox, EventArgs.Empty);
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

        private void sharedCategsDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiSelected = null;
            if (sharedCategsDepthMaxToolStripMenuItem.Checked) tsmiSelected = sharedCategsDepthMaxToolStripMenuItem;
            else if (sharedCategsDepthMedToolStripMenuItem.Checked) tsmiSelected = sharedCategsDepthMedToolStripMenuItem;
            else if (sharedCategsDepthMinToolStripMenuItem.Checked) tsmiSelected = sharedCategsDepthMinToolStripMenuItem;
            else if (sharedCategsDepthNoneToolStripMenuItem.Checked) tsmiSelected = sharedCategsDepthNoneToolStripMenuItem;

            if ((ToolStripMenuItem)sender == tsmiSelected) return;

            tsmiSelected.Checked = false;
            tsmiSelected = null;

            ((ToolStripMenuItem)sender).Checked = true;

            ShowElementsTreeLoadS6x();
        }

        private void sharedIdentificationStatusTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int iStatus = sharedIdentificationStatusTrackBar.Value;

            sharedIdentificationLabel.Text = string.Format("{0} ({1:d2}%)", sharedIdentificationLabel.Tag, iStatus);
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
            S6xNavInfo navInfo = new S6xNavInfo(elemsTreeView.SelectedNode);
            if (!navInfo.isValid) return;
            if (!navInfo.isHeaderCategory) return;

            if (!confirmDirtyProperies()) return;

            ArrayList alRemoval = null;

            switch (navInfo.HeaderCategory)
            {
                case S6xNavHeaderCategory.TABLES:
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
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode == null)
                        {
                            sadS6x.slTables.Remove(uniqueAddress);
                            continue;
                        }

                        // Duplicates exist
                        if (tnNode.Nodes.Count > 0) continue;

                        deleteElem(new S6xNavInfo(tnNode), true, false, true);
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
                        TreeNode tnMainNode = navInfo.FindElement(uniqueAddress);
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

                        deleteElem(new S6xNavInfo(tnNode), true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
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
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode == null)
                        {
                            sadS6x.slFunctions.Remove(uniqueAddress);
                            continue;
                        }
                        
                        // Duplicates exist
                        if (tnNode.Nodes.Count > 0) continue;

                        deleteElem(new S6xNavInfo(tnNode), true, false, true);
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
                        TreeNode tnMainNode = navInfo.FindElement(uniqueAddress);
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

                        deleteElem(new S6xNavInfo(tnNode), true, false, true);
                    }
                    if (alRemoval.Count > 0) sadS6x.isSaved = false;
                    break;
                case S6xNavHeaderCategory.SCALARS:
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
                        TreeNode tnNode = navInfo.FindElement(uniqueAddress);
                        if (tnNode == null)
                        {
                            sadS6x.slScalars.Remove(uniqueAddress);
                            continue;
                        }

                        // Duplicates exist
                        if (tnNode.Nodes.Count > 0) continue;

                        deleteElem(new S6xNavInfo(tnNode), true, false, true);
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
                        TreeNode tnMainNode = navInfo.FindElement(uniqueAddress);
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

                        deleteElem(new S6xNavInfo(tnNode), true, false, true);
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

        private void settingsSAD806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsForm("SAD806x Import/Export Settings", Application.StartupPath + "\\" + SADDef.settingsSAD806xImpExpFileName, "SAD806XIMPEXP").ShowDialog();
        }

        private void settingsSADToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsForm("SAD Import/Export Settings", Application.StartupPath + "\\" + SADDef.settingsSADImpExpFileName, "SADIMPEXP").ShowDialog();
        }

        private void settingsTunerProToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsForm("TunerPro Import/Export Settings", Application.StartupPath + "\\" + SADDef.settingsTunerProImpExpFileName, "TUNERPROIMPEXP").ShowDialog();
        }

        private void settingsEABEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsForm("EEC Analyser/Binary Editor Import/Export Settings", Application.StartupPath + "\\" + SADDef.settingsTunerProImpExpFileName, "EABEIMPEXP").ShowDialog();
        }

        private void settings806xUniDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsForm("806x universal database Settings", Application.StartupPath + "\\" + SADDef.settings806xUniDbImpExpFileName, "806XUNIDBIMPEXP").ShowDialog();
        }

        private void muCBSLLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S6xNavInfo niHeaderCateg = null;

            elemsTreeView.BeginUpdate();
            
            if ((ToolStripMenuItem)sender == muCBSLLTablesToolStripMenuItem)
            {
                niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES)]);
                foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        s6xObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.isSaved = false;

                        if (!niHeaderCateg.isValid) continue;
                        TreeNode tnNode = niHeaderCateg.FindElement(s6xObject.UniqueAddress);
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
                        s6xObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.isSaved = false;

                        if (!niHeaderCateg.isValid) continue;
                        TreeNode tnNode = niHeaderCateg.FindElement(s6xObject.UniqueAddress);
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
                niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS)]);
                foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        s6xObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.isSaved = false;

                        if (!niHeaderCateg.isValid) continue;
                        TreeNode tnNode = niHeaderCateg.FindElement(s6xObject.UniqueAddress);
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
                        s6xObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.isSaved = false;

                        if (!niHeaderCateg.isValid) continue;
                        TreeNode tnNode = niHeaderCateg.FindElement(s6xObject.UniqueAddress);
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
                niHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS)]);
                foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.Skip || !s6xObject.Store) continue;
                    if (!s6xObject.isUserDefined) continue;
                    string newComments = Tools.CommentsFirstLineShortLabelLabel(s6xObject.Comments, s6xObject.ShortLabel, s6xObject.Label);
                    if (newComments != s6xObject.Comments)
                    {
                        s6xObject.Comments = newComments;
                        s6xObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.isSaved = false;

                        if (!niHeaderCateg.isValid) continue;
                        TreeNode tnNode = niHeaderCateg.FindElement(s6xObject.UniqueAddress);
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
                        s6xObject.DateUpdated = DateTime.UtcNow;
                        sadS6x.isSaved = false;

                        if (!niHeaderCateg.isValid) continue;
                        TreeNode tnNode = niHeaderCateg.FindElement(s6xObject.UniqueAddress);
                        if (tnNode == null) continue;
                        TreeNode tnDupNode = tnNode.Nodes[s6xObject.DuplicateAddress];
                        if (tnDupNode == null) continue;
                        tnDupNode.ToolTipText = s6xObject.Comments;
                        tnDupNode.ForeColor = Color.Purple;
                        tnNode.ForeColor = Color.Purple;
                    }
                }
            }

            elemsTreeView.EndUpdate();
        }

        // 20200616 - PYM
        // calcRegisterAddress
        //  Calculate address when a RConst is used (Rec+12 => 1133 + 12 = 1145)
        //  Started from elemProperties_TextBox_KeyDown, using Ctrl-G
        private void calcRegisterAddress(Control sender)
        {
            if (sadBin == null) return;

            if (sender == null) return;
            if (sender.Parent == null) return;

            switch (sender.Name)
            {
                case "regAddressTextBox":
                    break;
                default:
                    return;
            }

            if (!regAddressTextBox.Text.Contains(SADDef.AdditionSeparator)) return;

            string regPart1 = regAddressTextBox.Text.Substring(0, regAddressTextBox.Text.IndexOf(SADDef.AdditionSeparator)).Trim();
            if (regPart1.Length != 2) return;


            if (!sadBin.S6x.slRegisters.ContainsKey(Tools.RegisterUniqueAddress(regPart1))) return;

            S6xRegister rConst = (S6xRegister)sadBin.S6x.slRegisters[Tools.RegisterUniqueAddress(regPart1)];
            if (!rConst.isRConst) return;

            int rConstValue = 0;
            try { rConstValue = Convert.ToInt32(rConst.ConstValue, 16); }
            catch { return; }

            string regPart2 = regAddressTextBox.Text.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty).Trim();
            int adderValue = 0;
            try
            {
                if (regPart2.Length > 2) adderValue = Convert.ToInt32(regPart2, 16);
                else adderValue = (int)Convert.ToSByte(regPart2, 16);
            }
            catch { return; }

            regAddressTextBox.Text = string.Format("{0:x2}", rConstValue + adderValue);
            regAddressTextBox.Modified = true;
        }

        // Select UniDB 806x (.86x) file
        private void selectUniDb806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!activatedUniDb806x) return;

            if (openFileDialogSQLite.ShowDialog() != DialogResult.OK) return;

            bool bResult = SQLite806xTools.selectUniDb806x(ref fileUniDb806xToolStripTextBox, openFileDialogSQLite.FileName, ref binariesUniDb806xToolStripMenuItem, ref filesUniDb806xToolStripMenuItem, new EventHandler(itemFilesUniDb806xToolStripMenuItem_Click));
        }

        private void openUniDb806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            string dbFilePath = (string)fileUniDb806xToolStripTextBox.Tag;
            if (dbFilePath == string.Empty) return;

            if (sqLite806xForm == null)
            {
                sqLite806xForm = new SQLite806xForm(dbFilePath);
                sqLite806xForm.FormClosed += new FormClosedEventHandler(sqLite806xForm_FormClosed);
            }
            sqLite806xForm.Show();
            sqLite806xForm.Focus();
        }

        private void sqLite806xForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            sqLite806xForm = null;
        }

        private void itemFilesUniDb806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == null) return;
            if (sender.GetType() != typeof(ToolStripMenuItem)) return;

            if (SQLite806xTools.isForStrategyFileAdd((ToolStripMenuItem)sender))
            {
                if (fileUniDb806xToolStripTextBox.Tag == null) return;
                string dbFilePath = (string)fileUniDb806xToolStripTextBox.Tag;
                if (dbFilePath == string.Empty) return;

                SQLite806xTools.strategyFileAdd((ToolStripMenuItem)sender, dbFilePath, ref sadBin, ref binariesUniDb806xToolStripMenuItem, ref filesUniDb806xToolStripMenuItem, new EventHandler(itemFilesUniDb806xToolStripMenuItem_Click));
            }
            else
            {
                // File itself
                sqLite806xFileLabelToolStripMenuItem.Text = ((ToolStripMenuItem)sender).Text;
                sqLite806xFileLabelToolStripMenuItem.ToolTipText = ((ToolStripMenuItem)sender).ToolTipText;
                sqLite806xFileContextMenuStrip.Tag = ((ToolStripMenuItem)sender).Tag;
                sqLite806xFileContextMenuStrip.Show(Cursor.Position);
            }
        }

        private void sqLite806xFileContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (sqLite806xFileContextMenuStrip.Tag == null)
            {
                e.Cancel = true;
                return;
            }
        }

        private void sqLite806xFileDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sqLite806xFileContextMenuStrip.Tag == null) return;

            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            string dbFilePath = (string)fileUniDb806xToolStripTextBox.Tag;
            if (dbFilePath == string.Empty) return;

            if (sqLite806xFileContextMenuStrip.Tag.GetType() == typeof(R_806x_Strategy_Binaries))
            {
                SQLite806xTools.strategyFileDownload<R_806x_Strategy_Binaries>(sqLite806xFileContextMenuStrip.Tag, dbFilePath, ref saveFileDialogBin);
            }
            else if (sqLite806xFileContextMenuStrip.Tag.GetType() == typeof(R_806x_Strategy_Files))
            {
                SQLite806xTools.strategyFileDownload<R_806x_Strategy_Files>(sqLite806xFileContextMenuStrip.Tag, dbFilePath, ref saveFileDialogGeneric);
            }
        }

        private void sqLite806xFileUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sqLite806xFileContextMenuStrip.Tag == null) return;

            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            string dbFilePath = (string)fileUniDb806xToolStripTextBox.Tag;
            if (dbFilePath == string.Empty) return;

            if (sqLite806xFileContextMenuStrip.Tag.GetType() == typeof(R_806x_Strategy_Binaries))
            {
                SQLite806xTools.strategyFileUpdate<R_806x_Strategy_Binaries>(sqLite806xFileContextMenuStrip.Tag, dbFilePath, ref binariesUniDb806xToolStripMenuItem, new EventHandler(itemFilesUniDb806xToolStripMenuItem_Click));
            }
            else if (sqLite806xFileContextMenuStrip.Tag.GetType() == typeof(R_806x_Strategy_Files))
            {
                SQLite806xTools.strategyFileUpdate<R_806x_Strategy_Files>(sqLite806xFileContextMenuStrip.Tag, dbFilePath, ref filesUniDb806xToolStripMenuItem, new EventHandler(itemFilesUniDb806xToolStripMenuItem_Click));
            }
        }

        private void sqLite806xFileRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sqLite806xFileContextMenuStrip.Tag == null) return;

            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            string dbFilePath = (string)fileUniDb806xToolStripTextBox.Tag;
            if (dbFilePath == string.Empty) return;

            if (sqLite806xFileContextMenuStrip.Tag.GetType() == typeof(R_806x_Strategy_Binaries))
            {
                SQLite806xTools.strategyFileRemove<R_806x_Strategy_Binaries>(sqLite806xFileContextMenuStrip.Tag, dbFilePath, ref binariesUniDb806xToolStripMenuItem, new EventHandler(itemFilesUniDb806xToolStripMenuItem_Click));
            }
            else if (sqLite806xFileContextMenuStrip.Tag.GetType() == typeof(R_806x_Strategy_Files))
            {
                SQLite806xTools.strategyFileRemove<R_806x_Strategy_Files>(sqLite806xFileContextMenuStrip.Tag, dbFilePath, ref filesUniDb806xToolStripMenuItem, new EventHandler(itemFilesUniDb806xToolStripMenuItem_Click));
            }
        }

        private void exportUniDb806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!activatedUniDb806x) return;

            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            if ((string)fileUniDb806xToolStripTextBox.Tag == string.Empty) return;

            if (sadBin == null) return;
            if (sadS6x == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            string sMessage = "Export will set database at SAD806x definition level.";
            sMessage += "\r\nPrevious existing elements will be removed.";
            sMessage += "\r\nInformation not present in SAD806x definition will be lost.";
            sMessage += "\r\nA database backup is done before processing.";
            sMessage += "\r\n\r\nContinue ?";
            if (MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                
            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;

            bool[] iesOpt = new bool[10];
            iesOpt[0] = iesOptPropertiesToolStripMenuItem.Checked;
            iesOpt[1] = iesOptTablesToolStripMenuItem.Checked;
            iesOpt[2] = iesOptFunctionsToolStripMenuItem.Checked;
            iesOpt[3] = iesOptScalarsToolStripMenuItem.Checked;
            iesOpt[4] = iesOptStructuresToolStripMenuItem.Checked;
            iesOpt[5] = iesOptRoutinesToolStripMenuItem.Checked;
            iesOpt[6] = iesOptOperationsToolStripMenuItem.Checked;
            iesOpt[7] = iesOptOtherToolStripMenuItem.Checked;
            iesOpt[8] = iesOptRegistersToolStripMenuItem.Checked;
            iesOpt[9] = iesOptSignaturesToolStripMenuItem.Checked;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { fileUniDb806xToolStripTextBox.Tag, iesOpt };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(exportUniDb806xStartProcess);
            processThread.Start();
        }

        private void exportUniDb806xStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;

            bool[] iesOpt = null;
            bool iesOptProperties = false;
            bool iesOptTables = false;
            bool iesOptFunctions = false;
            bool iesOptScalars = false;
            bool iesOptStructures = false;
            bool iesOptRoutines = false;
            bool iesOptOperations = false;
            bool iesOptOther = false;
            bool iesOptRegisters = false;
            bool iesOptSignatures = false;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];

                if (filePath == null || filePath == string.Empty) throw new Exception();

                iesOpt = (bool[])sadProcessManager.Parameters[1];

                iesOptProperties = iesOpt[0];
                iesOptTables = iesOpt[1];
                iesOptFunctions = iesOpt[2];
                iesOptScalars = iesOpt[3];
                iesOptStructures = iesOpt[4];
                iesOptRoutines = iesOpt[5];
                iesOptOperations = iesOpt[6];
                iesOptOther = iesOpt[7];
                iesOptRegisters = iesOpt[8];
                iesOptSignatures = iesOpt[9];
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            if (File.Exists(filePath))
            {
                // Original file autmatic Backup
                try
                {
                    File.Copy(filePath, filePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    sadProcessManager.SetProcessFailed("SQLite file backup has failed.\r\nNo other action will be managed.");
                    return;
                }
            }

            SQLite806xDB db806x = null;

            try
            {
                db806x = new SQLite806xDB(filePath);
            }
            catch
            {
                sadProcessManager.SetProcessFailed("SQLite file opening or schema reading has failed.\r\nNo other action will be managed.");
                return;
            }

            if (!db806x.ValidDB)
            {
                sadProcessManager.SetProcessFailed("SQLite file schema reading is not valid.\r\nNo other action will be managed.");
                return;
            }

            long totalS6xElements = 1;    // To prevent 0 dividing
            long processedS6xElements = 0;
            int processProgressStatus = 0;

            if (iesOptTables) totalS6xElements += sadS6x.slTables.Count;
            if (iesOptFunctions) totalS6xElements += sadS6x.slFunctions.Count;
            if (iesOptScalars) totalS6xElements += sadS6x.slScalars.Count;
            if (iesOptStructures) totalS6xElements += sadS6x.slStructures.Count;
            if (iesOptRoutines) totalS6xElements += sadS6x.slRoutines.Count;
            if (iesOptOperations) totalS6xElements += sadS6x.slOperations.Count;
            if (iesOptOther) totalS6xElements += sadS6x.slOtherAddresses.Count;
            if (iesOptRegisters) totalS6xElements += sadS6x.slRegisters.Count;
            if (iesOptSignatures)
            {
                totalS6xElements += sadS6x.slSignatures.Count;
                totalS6xElements += sadS6x.slElementsSignatures.Count;
            }

            if (iesOptProperties)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Properties...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += 1;
                SQLite806xTools.exportProperties(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptTables)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Tables...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slTables.Count;
                SQLite806xTools.exportTables(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptFunctions)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Functions...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slFunctions.Count;
                SQLite806xTools.exportFunctions(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptScalars)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Scalars...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slScalars.Count;
                SQLite806xTools.exportScalars(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptStructures)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Structures...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slStructures.Count;
                SQLite806xTools.exportStructures(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptRoutines)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Routines...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slRoutines.Count;
                SQLite806xTools.exportRoutines(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptOperations)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Operations...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slOperations.Count;
                SQLite806xTools.exportOperations(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptOther)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Other addresses...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slOtherAddresses.Count;
                SQLite806xTools.exportOthers(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptRegisters)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Registers...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slRegisters.Count;
                SQLite806xTools.exportRegisters(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptSignatures)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Signatures...";
                processProgressStatus = (int)(100.0 * ((double)processedS6xElements / (double)totalS6xElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedS6xElements += sadS6x.slSignatures.Count + sadS6x.slProcessElementsSignatures.Count;
                SQLite806xTools.exportSignatures(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            // SyncStates update - For next sync
            sadS6x.Properties.SyncStateUpdate(db806x.SyncType, db806x.SyncId);

            sadProcessManager.ProcessProgressLabel = "Processing States...";
            SQLite806xTools.exportSyncStates(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);

            GC.Collect();

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Export is done.");
            else sadProcessManager.SetProcessFinished("Export has finished with errors.");
        }

        private void importUniDb806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!activatedUniDb806x) return;

            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            if ((string)fileUniDb806xToolStripTextBox.Tag == string.Empty) return;

            if (sadBin == null) return;
            if (sadS6x == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            string sMessage = "Import will set SAD806x definition at database level.";
            sMessage += "\r\nPrevious existing elements will be removed.";
            sMessage += "\r\nInformation not present in database will be lost.";
            sMessage += "\r\nA definition backup is done before processing.";
            sMessage += "\r\n\r\nContinue ?";
            if (MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;

            bool[] iesOpt = new bool[10];
            iesOpt[0] = iesOptPropertiesToolStripMenuItem.Checked;
            iesOpt[1] = iesOptTablesToolStripMenuItem.Checked;
            iesOpt[2] = iesOptFunctionsToolStripMenuItem.Checked;
            iesOpt[3] = iesOptScalarsToolStripMenuItem.Checked;
            iesOpt[4] = iesOptStructuresToolStripMenuItem.Checked;
            iesOpt[5] = iesOptRoutinesToolStripMenuItem.Checked;
            iesOpt[6] = iesOptOperationsToolStripMenuItem.Checked;
            iesOpt[7] = iesOptOtherToolStripMenuItem.Checked;
            iesOpt[8] = iesOptRegistersToolStripMenuItem.Checked;
            iesOpt[9] = iesOptSignaturesToolStripMenuItem.Checked;
            
            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { fileUniDb806xToolStripTextBox.Tag, iesOpt };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(importUniDb806xStartProcess);
            processThread.Start();
        }

        private void importUniDb806xStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            bool[] iesOpt = null;
            bool iesOptProperties = false;
            bool iesOptTables = false;
            bool iesOptFunctions = false;
            bool iesOptScalars = false;
            bool iesOptStructures = false;
            bool iesOptRoutines = false;
            bool iesOptOperations = false;
            bool iesOptOther = false;
            bool iesOptRegisters = false;
            bool iesOptSignatures = false;
            
            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];

                if (filePath == null || filePath == string.Empty) throw new Exception();

                iesOpt = (bool[])sadProcessManager.Parameters[1];

                iesOptProperties = iesOpt[0];
                iesOptTables = iesOpt[1];
                iesOptFunctions = iesOpt[2];
                iesOptScalars = iesOpt[3];
                iesOptStructures = iesOpt[4];
                iesOptRoutines = iesOpt[5];
                iesOptOperations = iesOpt[6];
                iesOptOther = iesOpt[7];
                iesOptRegisters = iesOpt[8];
                iesOptSignatures = iesOpt[9];
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            if (File.Exists(s6xFilePath))
            {
                // Original file autmatic Backup
                try
                {
                    File.Copy(filePath, filePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    sadProcessManager.SetProcessFailed("SAD806x definition backup has failed.\r\nNo other action will be managed.");
                    return;
                }
            }

            SQLite806xDB db806x = null;

            try
            {
                db806x = new SQLite806xDB(filePath);
            }
            catch
            {
                sadProcessManager.SetProcessFailed("SQLite file opening or schema reading has failed.\r\nNo other action will be managed.");
                return;
            }

            if (!db806x.ValidDB)
            {
                sadProcessManager.SetProcessFailed("SQLite file schema reading is not valid.\r\nNo other action will be managed.");
                return;
            }

            long totalElements = 1;    // To prevent 0 dividing
            long processedElements = 0;
            int processProgressStatus = 0;

            sadProcessManager.ProcessProgressLabel = "Counting records...";
            totalElements = SQLite806xTools.importCount(ref db806x, ref iesOpt, ref sadProcessManager.ProcessErrors);
            processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
            sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
            processedElements += 1;

            if (iesOptProperties)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Properties...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importProperties(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptTables)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Tables...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importTables(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptFunctions)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Functions...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importFunctions(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptScalars)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Scalars...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importScalars(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptStructures)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Structures...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importStructures(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptRoutines)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Routines...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importRoutines(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptOperations)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Operations...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importOperations(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptOther)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Other addresses...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importOthers(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptRegisters)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Registers...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importRegisters(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            if (iesOptSignatures)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Signatures...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.importSignatures(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);
            }

            // SyncStates update - For next sync
            sadS6x.Properties.SyncStateUpdate(db806x.SyncType, db806x.SyncId);

            GC.Collect();

            sadProcessManager.PostProcessAction = "importUniDb806xPostProcess";
            sadProcessManager.PostProcessParameters = new object[] { };

            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void importUniDb806xPostProcess()
        {
            if (sadProcessManager == null) return;

            ShowElementsTreeLoadS6x();

            outputToolStripMenuItem.Enabled = false;
            
            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Import is done.");
            else sadProcessManager.SetProcessFinished("Import has finished with errors.");

            GC.Collect();
        }

        private void syncUniDb806xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!activatedUniDb806x) return;

            if (fileUniDb806xToolStripTextBox.Tag == null) return;
            if ((string)fileUniDb806xToolStripTextBox.Tag == string.Empty) return;

            if (sadBin == null) return;
            if (sadS6x == null) return;

            if (!confirmDirtyProperies()) return;
            if (!confirmProcessRunning()) return;

            string sMessage = "Synchronization will update SAD806x definition and Database in both ways.";
            sMessage += "\r\nOldest elements will be overwritten.";
            sMessage += "\r\nSAD806x only elements will be added.";
            sMessage += "\r\nDatabase only elements will be added.";
            sMessage += "\r\nA definition backup is done before processing.";
            sMessage += "\r\n\r\nContinue ?";
            if (MessageBox.Show(sMessage, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            processPreviousCursor = Cursor;
            Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // To prevent opening an old information
            sadBin.Errors = null;

            bool[] iesOpt = new bool[10];
            iesOpt[0] = iesOptPropertiesToolStripMenuItem.Checked;
            iesOpt[1] = iesOptTablesToolStripMenuItem.Checked;
            iesOpt[2] = iesOptFunctionsToolStripMenuItem.Checked;
            iesOpt[3] = iesOptScalarsToolStripMenuItem.Checked;
            iesOpt[4] = iesOptStructuresToolStripMenuItem.Checked;
            iesOpt[5] = iesOptRoutinesToolStripMenuItem.Checked;
            iesOpt[6] = iesOptOperationsToolStripMenuItem.Checked;
            iesOpt[7] = iesOptOtherToolStripMenuItem.Checked;
            iesOpt[8] = iesOptRegistersToolStripMenuItem.Checked;
            iesOpt[9] = iesOptSignaturesToolStripMenuItem.Checked;

            sadProcessManager = new SADProcessManager();
            sadProcessManager.Parameters = new object[] { fileUniDb806xToolStripTextBox.Tag, iesOpt };

            toolStripProgressBarMain.Value = 0;

            processType = ProcessType.ProcessManager;
            processRunning = true;
            processUpdateTimer.Enabled = true;

            processThread = new Thread(syncUniDb806xStartProcess);
            processThread.Start();
        }

        private void syncUniDb806xStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            bool[] iesOpt = null;
            bool iesOptProperties = false;
            bool iesOptTables = false;
            bool iesOptFunctions = false;
            bool iesOptScalars = false;
            bool iesOptStructures = false;
            bool iesOptRoutines = false;
            bool iesOptOperations = false;
            bool iesOptOther = false;
            bool iesOptRegisters = false;
            bool iesOptSignatures = false;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];

                if (filePath == null || filePath == string.Empty) throw new Exception();

                iesOpt = (bool[])sadProcessManager.Parameters[1];

                iesOptProperties = iesOpt[0];
                iesOptTables = iesOpt[1];
                iesOptFunctions = iesOpt[2];
                iesOptScalars = iesOpt[3];
                iesOptStructures = iesOpt[4];
                iesOptRoutines = iesOpt[5];
                iesOptOperations = iesOpt[6];
                iesOptOther = iesOpt[7];
                iesOptRegisters = iesOpt[8];
                iesOptSignatures = iesOpt[9];
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            if (File.Exists(filePath))
            {
                // Original file autmatic Backup
                try
                {
                    File.Copy(filePath, filePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    sadProcessManager.SetProcessFailed("SAD806x definition backup has failed.\r\nNo other action will be managed.");
                    return;
                }
            }

            SQLite806xDB db806x = null;

            try
            {
                db806x = new SQLite806xDB(filePath);
            }
            catch
            {
                sadProcessManager.SetProcessFailed("SQLite file opening or schema reading has failed.\r\nNo other action will be managed.");
                return;
            }

            if (!db806x.ValidDB)
            {
                sadProcessManager.SetProcessFailed("SQLite file schema reading is not valid.\r\nNo other action will be managed.");
                return;
            }

            DateTime dtLastSync = sadS6x.Properties.SyncStateLastDate(db806x.SyncType, db806x.SyncId);

            long totalElements = 1;    // To prevent 0 dividing

            if (iesOptTables) totalElements += sadS6x.slTables.Count;
            if (iesOptFunctions) totalElements += sadS6x.slFunctions.Count;
            if (iesOptScalars) totalElements += sadS6x.slScalars.Count;
            if (iesOptStructures) totalElements += sadS6x.slStructures.Count;
            if (iesOptRoutines) totalElements += sadS6x.slRoutines.Count;
            if (iesOptOperations) totalElements += sadS6x.slOperations.Count;
            if (iesOptOther) totalElements += sadS6x.slOtherAddresses.Count;
            if (iesOptRegisters) totalElements += sadS6x.slRegisters.Count;
            if (iesOptSignatures)
            {
                totalElements += sadS6x.slSignatures.Count;
                totalElements += sadS6x.slElementsSignatures.Count;
            }
            
            long processedElements = 0;
            int processProgressStatus = 0;

            List<S_SQLiteSyncS6x> unSyncObjects = new List<S_SQLiteSyncS6x>();

            sadProcessManager.ProcessProgressLabel = "Counting records...";
            totalElements += SQLite806xTools.importCount(ref db806x, ref iesOpt, ref sadProcessManager.ProcessErrors);
            processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
            sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
            processedElements += 1;

            if (iesOptProperties)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Properties...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncProperties(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, dtLastSync);
            }

            if (iesOptTables)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Tables...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncTables(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptFunctions)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Functions...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncFunctions(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptScalars)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Scalars...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncScalars(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptStructures)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Structures...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncStructures(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptRoutines)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Routines...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncRoutines(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptOperations)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Operations...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncOperations(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptOther)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Other addresses...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncOthers(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptRegisters)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Registers...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncRegisters(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            if (iesOptSignatures)
            {
                sadProcessManager.ProcessProgressLabel = "Processing Signatures...";
                processProgressStatus = (int)(100.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncSignatures(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors, ref unSyncObjects, dtLastSync);
            }

            // SyncStates update - For next sync
            sadS6x.Properties.SyncStateUpdate(db806x.SyncType, db806x.SyncId);
            sadS6x.isSaved = false;

            sadProcessManager.ProcessProgressLabel = "Processing States...";
            SQLite806xTools.exportSyncStates(ref db806x, ref sadS6x, ref sadProcessManager.ProcessErrors);

            db806x = null;
            
            GC.Collect();

            sadProcessManager.PostProcessAction = "syncUniDb806xPostProcess";
            if (unSyncObjects != null) if (unSyncObjects.Count > 0) sadProcessManager.ProcessProgressLabel = "Asking for choices...";
            // unSyncObjects have been deleted on one side
            //      At this level, they have been recreated temporary
            //      Question is : Should we keep them or remove them from both sides ?
            sadProcessManager.PostProcessParameters = new object[] {filePath, unSyncObjects };

            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void syncUniDb806xPostProcess()
        {
            if (sadProcessManager == null) return;

            if (sadProcessManager.PostProcessParameters == null) return;
            if (((object[])sadProcessManager.PostProcessParameters).Length < 2) return;
            string filePath = (string)((object[])sadProcessManager.PostProcessParameters)[0];
            List<S_SQLiteSyncS6x> unSyncObjects = (List<S_SQLiteSyncS6x>)((object[])sadProcessManager.PostProcessParameters)[1];

            // unSyncObjects have been deleted on one side
            //      At this level, they have been recreated temporary
            //      Question is : Should we keep them or remove them from both sides ?
            if (unSyncObjects != null)
            {
                if (unSyncObjects.Count > 0)
                {
                    List<S_SQLiteSyncS6x> syncObjectsToRemove = new List<S_SQLiteSyncS6x>();

                    SQLite806xDB db806x = null;

                    try { db806x = new SQLite806xDB(filePath); }
                    catch { db806x = null; }

                    if (!db806x.ValidDB) db806x = null;

                    if (db806x != null)
                    {
                        if (new SQLite806xSyncForm(ref db806x, ref unSyncObjects, ref syncObjectsToRemove, ref elemsTreeView).ShowDialog() == DialogResult.OK)
                        {
                            if (syncObjectsToRemove != null)
                            {
                                if (syncObjectsToRemove.Count > 0)
                                {
                                    sadProcessManager = new SADProcessManager();
                                    sadProcessManager.Parameters = new object[] { fileUniDb806xToolStripTextBox.Tag, syncObjectsToRemove };

                                    toolStripProgressBarMain.Value = 0;

                                    processType = ProcessType.ProcessManager;
                                    processRunning = true;
                                    processUpdateTimer.Enabled = true;

                                    processThread = new Thread(syncUniDb806xCleanUpStartProcess);
                                    processThread.Start();

                                    return;
                                }
                            }
                        }
                    }
                }
            }

            ShowElementsTreeLoadS6x();

            outputToolStripMenuItem.Enabled = false;

            if (sadProcessManager.ProcessErrors.Count == 0) sadProcessManager.SetProcessFinished("Synchronization is done.");
            else sadProcessManager.SetProcessFinished("Synchronization has finished with errors.");

            GC.Collect();
        }

        private void syncUniDb806xCleanUpStartProcess()
        {
            if (sadProcessManager == null) sadProcessManager = new SADProcessManager();

            string filePath = string.Empty;
            List<S_SQLiteSyncS6x> syncObjectsToRemove = null;

            try
            {
                if (sadBin == null) throw new Exception();

                filePath = (string)sadProcessManager.Parameters[0];

                if (filePath == null || filePath == string.Empty) throw new Exception();

                syncObjectsToRemove = (List<S_SQLiteSyncS6x>)sadProcessManager.Parameters[1];
                if (syncObjectsToRemove == null) syncObjectsToRemove = new List<S_SQLiteSyncS6x>();
            }
            catch
            {
                sadProcessManager.SetProcessFailed("Invalid process start.");
                return;
            }

            SQLite806xDB db806x = null;

            try
            {
                db806x = new SQLite806xDB(filePath);
            }
            catch
            {
                sadProcessManager.SetProcessFailed("SQLite file opening or schema reading has failed.\r\nNo other action will be managed.");
                return;
            }

            if (!db806x.ValidDB)
            {
                sadProcessManager.SetProcessFailed("SQLite file schema reading is not valid.\r\nNo other action will be managed.");
                return;
            }

            List<object> dbRowsToDelete = new List<object>();   // For removal in one transaction per table
            long totalElements = syncObjectsToRemove.Count;
            long processedElements = 0;
            int processProgressStatus = 0;

            sadProcessManager.ProcessProgressLabel = "Searching/Removing records...";
            foreach (S_SQLiteSyncS6x syncObjectToRemove in syncObjectsToRemove)
            {
                processProgressStatus = (int)(80.0 * ((double)processedElements / (double)totalElements));
                sadProcessManager.ProcessProgressStatus = processProgressStatus >= 99 ? 98 : processProgressStatus;
                processedElements += SQLite806xTools.syncRemovalStep1(syncObjectToRemove, ref db806x, ref sadS6x, ref dbRowsToDelete, ref sadProcessManager.ProcessErrors);
            }

            sadProcessManager.ProcessProgressLabel = "Removing database records...";
            sadProcessManager.ProcessProgressStatus = 80;
            SQLite806xTools.syncRemovalStep2(ref dbRowsToDelete, ref db806x, ref sadProcessManager.ProcessErrors);

            db806x = null;

            GC.Collect();

            sadProcessManager.PostProcessAction = "syncUniDb806xPostProcess";
            sadProcessManager.PostProcessParameters = new object[] { filePath, null };

            sadProcessManager.ProcessProgressStatus = 99;       // To switch to 100 after notification update
        }

        private void iesOptAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iesOptSetAll(true);
        }

        private void iesOptNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iesOptSetAll(false);
        }

        private void iesOptSetAll(bool bChecked)
        {
            iesOptPropertiesToolStripMenuItem.Checked = bChecked;
            iesOptTablesToolStripMenuItem.Checked = bChecked;
            iesOptFunctionsToolStripMenuItem.Checked = bChecked;
            iesOptScalarsToolStripMenuItem.Checked = bChecked;
            iesOptStructuresToolStripMenuItem.Checked = bChecked;
            iesOptRoutinesToolStripMenuItem.Checked = bChecked;
            iesOptOperationsToolStripMenuItem.Checked = bChecked;
            iesOptOtherToolStripMenuItem.Checked = bChecked;
            iesOptRegistersToolStripMenuItem.Checked = bChecked;
            iesOptSignaturesToolStripMenuItem.Checked = bChecked;
        }
    }
}
