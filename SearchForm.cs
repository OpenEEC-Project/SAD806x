using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class SearchForm : Form
    {
        private SADS6x sadS6x = null;
        private TreeView elemsTreeView = null;

        private System.Windows.Forms.Timer mainUpdateTimer = null;
        
        public SearchForm(ref SADS6x mainSadS6x, ref TreeView mainElemsTreeView)
        {
            sadS6x = mainSadS6x;
            elemsTreeView = mainElemsTreeView;
            
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(SearchForm_FormClosing);

            searchTreeView.StateImageList = elemsTreeView.StateImageList;
            
            searchTextBox.KeyPress += new KeyPressEventHandler(searchTextBox_KeyPress);
            searchTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(searchTreeView_NodeMouseClick);
            searchTreeView.AfterSelect += new TreeViewEventHandler(searchTreeView_AfterSelect);

            mainUpdateTimer = new System.Windows.Forms.Timer();
            mainUpdateTimer.Enabled = false;
            mainUpdateTimer.Interval = 100;
            mainUpdateTimer.Tick += new EventHandler(mainUpdateTimer_Tick);

        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainUpdateTimer.Enabled = false;
        }

        private void mainUpdateTimer_Tick(object sender, EventArgs e)
        {
            mainUpdateTimer.Enabled = false;

            try
            {
                bool refreshCount = false;

                foreach (TreeNode tnCateg in searchTreeView.Nodes)
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
                        if (tnMainNode.StateImageKey != tnNode.StateImageKey) tnNode.StateImageKey = tnMainNode.StateImageKey;
                    }

                    foreach (TreeNode tnNode in lsRemoval) tnCateg.Nodes.Remove(tnNode);
                    refreshCount |= lsRemoval.Count > 0;
                }

                if (refreshCount) searchTreeViewCount();
                mainUpdateTimer.Enabled = true;
            }
            catch { }
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)13:
                    searchButton_Click(sender, e);
                    break;
            }
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
            Search(null);
        }

        private void routinesWithArgumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search("ARGSROUTINES");
        }

        private void routinesAdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search("ADVROUTINES");
        }

        private void Search(string Mode)
        {
            if (sadS6x == null) return;
            if (elemsTreeView == null) return;

            List<string[]> results = null;
            
            switch (Mode)
            {
                case "ARGSROUTINES":
                    results = SearchArgsRoutines();
                    break;
                case "ADVROUTINES":
                    results = SearchAdvancedRoutines();
                    break;
                default:
                    if (searchTextBox.Text == string.Empty) return;
                    results = SearchText(searchTextBox.Text.ToLower());
                    break;
            }

            mainUpdateTimer.Enabled = false;

            searchTreeView.BeginUpdate();
            
            searchTreeViewInit();

            foreach (string[] result in results)
            {
                S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[result[0]]);
                if (!niMFHeaderCateg.isValid) continue;

                TreeNode parentNode = searchTreeView.Nodes[niMFHeaderCateg.HeaderCategoryName];
                if (parentNode == null) continue;

                TreeNode tnMFNode = niMFHeaderCateg.FindElement(result[1]);
                if (tnMFNode == null) continue;
                if (parentNode.Nodes.ContainsKey(result[1])) continue;

                TreeNode tnNode = new TreeNode();
                tnNode.Name = tnMFNode.Name;
                tnNode.Text = tnMFNode.Text;
                tnNode.ToolTipText = tnMFNode.ToolTipText;
                tnNode.StateImageKey = tnMFNode.StateImageKey;
                parentNode.Nodes.Add(tnNode);
            }

            searchTreeViewCount();

            searchTreeView.EndUpdate();

            mainUpdateTimer.Enabled = true;
        }

        private List<string[]> SearchText(string searchText)
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.InputUnits, s6xObject.OutputUnits, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), s6xObject.UniqueAddress });
            }
            foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS), s6xObject.UniqueAddress });
            }
            foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER), s6xObject.UniqueAddress });
            }
            foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ByteLabel, s6xObject.WordLabel, s6xObject.ConstValue, s6xObject.Units, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                // BitFlags AddOn
                if (!isResult && s6xObject.BitFlagsNum > 0)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
                    {
                        searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments, s6xBF.Category, s6xBF.Category2, s6xBF.Category3, s6xBF.IdentificationDetails };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS), s6xObject.UniqueAddress });
            }
            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (!isResult && s6xObject.InputArguments != null)
                {
                    // Nothing Interesting to search for
                }
                if (!isResult && s6xObject.InputStructures != null)
                {
                    // Nothing Interesting to search for
                }
                if (!isResult && s6xObject.InputTables != null)
                {
                    foreach (S6xRoutineInputTable s6xSubObject in s6xObject.InputTables)
                    {
                        searchValues = new string[] { s6xSubObject.ForcedCellsUnits, s6xSubObject.ForcedColsUnits, s6xSubObject.ForcedRowsUnits };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InputFunctions != null)
                {
                    foreach (S6xRoutineInputFunction s6xSubObject in s6xObject.InputFunctions)
                    {
                        searchValues = new string[] { s6xSubObject.ForcedInputUnits, s6xSubObject.ForcedOutputUnits };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InputScalars != null)
                {
                    foreach (S6xRoutineInputScalar s6xSubObject in s6xObject.InputScalars)
                    {
                        searchValues = new string[] { s6xSubObject.ForcedUnits };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), s6xObject.UniqueAddress });
            }
            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Units, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                // BitFlags AddOn
                if (!isResult && s6xObject.BitFlagsNum > 0)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
                    {
                        searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments, s6xBF.Category, s6xBF.Category2, s6xBF.Category3, s6xBF.IdentificationDetails };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), s6xObject.UniqueAddress });
            }
            foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), s6xObject.UniqueAddress });
            }
            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.CellsUnits, s6xObject.ColsUnits, s6xObject.RowsUnits, s6xObject.Comments, s6xObject.Information, s6xObject.Category, s6xObject.Category2, s6xObject.Category3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), s6xObject.UniqueAddress });
            }
            foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.SignatureLabel, s6xObject.SignatureCategory, s6xObject.SignatureComments, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information, s6xObject.SignatureCategory, s6xObject.SignatureCategory2, s6xObject.SignatureCategory3, s6xObject.IdentificationDetails, s6xObject.RoutineCategory, s6xObject.RoutineCategory2, s6xObject.RoutineCategory3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (!isResult && s6xObject.InputArguments != null)
                {
                    // Nothing Interesting to search for
                }
                if (!isResult && s6xObject.InputStructures != null)
                {
                    // Nothing Interesting to search for
                }
                if (!isResult && s6xObject.InputTables != null)
                {
                    foreach (S6xRoutineInputTable s6xSubObject in s6xObject.InputTables)
                    {
                        searchValues = new string[] { s6xSubObject.ForcedCellsUnits, s6xSubObject.ForcedColsUnits, s6xSubObject.ForcedRowsUnits };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InputFunctions != null)
                {
                    foreach (S6xRoutineInputFunction s6xSubObject in s6xObject.InputFunctions)
                    {
                        searchValues = new string[] { s6xSubObject.ForcedInputUnits, s6xSubObject.ForcedOutputUnits };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InputScalars != null)
                {
                    foreach (S6xRoutineInputScalar s6xSubObject in s6xObject.InputScalars)
                    {
                        searchValues = new string[] { s6xSubObject.ForcedUnits };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InternalStructures != null)
                {
                    foreach (S6xRoutineInternalStructure s6xSubObject in s6xObject.InternalStructures)
                    {
                        searchValues = new string[] { s6xSubObject.Label, s6xSubObject.ShortLabel, s6xSubObject.Comments, s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3, s6xSubObject.IdentificationDetails };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InternalTables != null)
                {
                    foreach (S6xRoutineInternalTable s6xSubObject in s6xObject.InternalTables)
                    {
                        searchValues = new string[] { s6xSubObject.Label, s6xSubObject.ShortLabel, s6xSubObject.CellsUnits, s6xSubObject.ColsUnits, s6xSubObject.RowsUnits, s6xSubObject.Comments, s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3, s6xSubObject.IdentificationDetails };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InternalFunctions != null)
                {
                    foreach (S6xRoutineInternalFunction s6xSubObject in s6xObject.InternalFunctions)
                    {
                        searchValues = new string[] { s6xSubObject.Label, s6xSubObject.ShortLabel, s6xSubObject.InputUnits, s6xSubObject.OutputUnits, s6xSubObject.Comments, s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3, s6xSubObject.IdentificationDetails };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (!isResult && s6xObject.InternalScalars != null)
                {
                    foreach (S6xRoutineInternalScalar s6xSubObject in s6xObject.InternalScalars)
                    {
                        searchValues = new string[] { s6xSubObject.Label, s6xSubObject.ShortLabel, s6xSubObject.Units, s6xSubObject.Comments, s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3, s6xSubObject.IdentificationDetails };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;

                        // BitFlags AddOn
                        if (s6xSubObject.BitFlagsNum > 0)
                        {
                            foreach (S6xBitFlag s6xBF in s6xSubObject.BitFlags)
                            {
                                searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments, s6xBF.Category, s6xBF.Category2, s6xBF.Category3, s6xBF.IdentificationDetails };
                                foreach (string searchValue in searchValues)
                                {
                                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                                    if (cSearchValue.Contains(searchText))
                                    {
                                        isResult = true;
                                        break;
                                    }
                                }
                                if (isResult) break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SIGNATURES), s6xObject.UniqueKey });
            }
            foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.SignatureLabel, s6xObject.SignatureKey, s6xObject.SignatureCategory, s6xObject.SignatureComments, s6xObject.Information, s6xObject.SignatureCategory, s6xObject.SignatureCategory2, s6xObject.SignatureCategory3, s6xObject.IdentificationDetails };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (!isResult && s6xObject.Scalar != null)
                {
                    searchValues = new string[] { s6xObject.Scalar.Label, s6xObject.Scalar.ShortLabel, s6xObject.Scalar.Units, s6xObject.Scalar.Comments, s6xObject.Scalar.Category, s6xObject.Scalar.Category2, s6xObject.Scalar.Category3, s6xObject.Scalar.IdentificationDetails };
                    foreach (string searchValue in searchValues)
                    {
                        string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                        if (cSearchValue.Contains(searchText))
                        {
                            isResult = true;
                            break;
                        }
                    }
                    // BitFlags AddOn
                    if (!isResult && s6xObject.Scalar.BitFlagsNum > 0)
                    {
                        foreach (S6xBitFlag s6xBF in s6xObject.Scalar.BitFlags)
                        {
                            searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments };
                            foreach (string searchValue in searchValues)
                            {
                                string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                                if (cSearchValue.Contains(searchText))
                                {
                                    isResult = true;
                                    break;
                                }
                            }
                            if (isResult) break;
                        }
                    }
                }
                if (!isResult && s6xObject.Function != null)
                {
                    searchValues = new string[] { s6xObject.Function.Label, s6xObject.Function.ShortLabel, s6xObject.Function.InputUnits, s6xObject.Function.OutputUnits, s6xObject.Function.Comments, s6xObject.Function.Category, s6xObject.Function.Category2, s6xObject.Function.Category3, s6xObject.Function.IdentificationDetails };
                    foreach (string searchValue in searchValues)
                    {
                        string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                        if (cSearchValue.Contains(searchText))
                        {
                            isResult = true;
                            break;
                        }
                    }
                }
                if (!isResult && s6xObject.Table != null)
                {
                    searchValues = new string[] { s6xObject.Table.Label, s6xObject.Table.ShortLabel, s6xObject.Table.CellsUnits, s6xObject.Table.ColsUnits, s6xObject.Table.RowsUnits, s6xObject.Table.Comments, s6xObject.Table.Category, s6xObject.Table.Category2, s6xObject.Table.Category3, s6xObject.Table.IdentificationDetails };
                    foreach (string searchValue in searchValues)
                    {
                        string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                        if (cSearchValue.Contains(searchText))
                        {
                            isResult = true;
                            break;
                        }
                    }
                }
                if (!isResult && s6xObject.Structure != null)
                {
                    searchValues = new string[] { s6xObject.Structure.Label, s6xObject.Structure.ShortLabel, s6xObject.Structure.Comments, s6xObject.Structure.Category, s6xObject.Structure.Category2, s6xObject.Structure.Category3, s6xObject.Structure.IdentificationDetails };
                    foreach (string searchValue in searchValues)
                    {
                        string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                        if (cSearchValue.Contains(searchText))
                        {
                            isResult = true;
                            break;
                        }
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ELEMSSIGNATURES), s6xObject.UniqueKey });
            }

            return results;
        }

        private List<string[]> SearchArgsRoutines()
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.ByteArgumentsNum > 0) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), s6xObject.UniqueAddress });
            }

            return results;
        }

        private List<string[]> SearchAdvancedRoutines()
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.isAdvanced) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), s6xObject.UniqueAddress });
            }

            return results;
        }

        private void searchTreeViewInit()
        {
            searchTreeView.Nodes.Clear();

            foreach (TreeNode tnMainParent in elemsTreeView.Nodes)
            {
                switch (S6xNav.getHeaderCateg(tnMainParent.Name))
                {
                    case S6xNavHeaderCategory.PROPERTIES:
                    case S6xNavHeaderCategory.RESERVED:
                        continue;
                    case S6xNavHeaderCategory.TABLES:
                    case S6xNavHeaderCategory.FUNCTIONS:
                    case S6xNavHeaderCategory.SCALARS:
                    case S6xNavHeaderCategory.STRUCTURES:
                    case S6xNavHeaderCategory.ROUTINES:
                    case S6xNavHeaderCategory.OPERATIONS:
                    case S6xNavHeaderCategory.REGISTERS:
                    case S6xNavHeaderCategory.OTHER:
                    case S6xNavHeaderCategory.SIGNATURES:
                    case S6xNavHeaderCategory.ELEMSSIGNATURES:
                        TreeNode tnParent = new TreeNode();
                        tnParent.Name = tnMainParent.Name;
                        tnParent.Text = S6xNav.getHeaderCategLabel(S6xNav.getHeaderCateg(tnMainParent.Name));
                        tnParent.ToolTipText = tnMainParent.ToolTipText;
                        tnParent.StateImageKey = tnMainParent.StateImageKey;
                        searchTreeView.Nodes.Add(tnParent);
                        break;
                    default:
                        continue;
                }
            }
        }

        private void searchTreeViewCount()
        {
            foreach (TreeNode tnParent in searchTreeView.Nodes)
            {
                switch (S6xNav.getHeaderCateg(tnParent.Name))
                {
                    case S6xNavHeaderCategory.TABLES:
                    case S6xNavHeaderCategory.FUNCTIONS:
                    case S6xNavHeaderCategory.SCALARS:
                    case S6xNavHeaderCategory.STRUCTURES:
                    case S6xNavHeaderCategory.ROUTINES:
                    case S6xNavHeaderCategory.OPERATIONS:
                    case S6xNavHeaderCategory.REGISTERS:
                    case S6xNavHeaderCategory.OTHER:
                    case S6xNavHeaderCategory.SIGNATURES:
                    case S6xNavHeaderCategory.ELEMSSIGNATURES:
                        break;
                    default:
                        continue;
                }
                tnParent.Text = S6xNav.getHeaderCategLabel(tnParent.Name) + " (" + tnParent.Nodes.Count.ToString() + ")";
            }
        }
    }
}
