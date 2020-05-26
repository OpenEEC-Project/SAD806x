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
    public partial class RepositoryForm : Form
    {
        private string repoFilePath = string.Empty;
        private string specificType = string.Empty;
        private Repository repoRepository = null;
        private RepositoryItem selectedRepoItem = null;
        private BindingSource bindingSource = null;
        private SADS6x sadS6x = null;

        public RepositoryForm(string title, string filePath, string specType, ref SADS6x s6x)
        {
            InitializeComponent();

            this.Text = title;
            repoFilePath = filePath;
            specificType = specType;
            sadS6x = s6x;

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            repoRepository = (Repository)ToolsXml.DeserializeFile(repoFilePath, typeof(Repository));
            if (repoRepository == null) repoRepository = new Repository();
            repoRepository.isSaved = true;

            switch (specificType)
            {
                case "REGISTERS":
                case "UNITS":
                    labelLabel.Visible = false;
                    labelTextBox.Visible = false;
                    sLabelLabel.Text = labelLabel.Text;
                    break;
                case "OBDIERRORS":
                    sLabelLabel.Text = SADDef.repoLabelOBDIErrorsShortLabel;
                    sLabelLabel.MouseMove += new MouseEventHandler(ShortLabelLabel_MouseMove);
                    break;
                case "OBDIIERRORS":
                    sLabelLabel.Text = SADDef.repoLabelOBDIIErrorsShortLabel;
                    sLabelLabel.MouseMove += new MouseEventHandler(ShortLabelLabel_MouseMove);
                    break;
            }

            bindingSource = new BindingSource();
            bindingSource.DataSource = repoRepository.Items.FindAll(searchRepositoryItem);

            repoListBox.DisplayMember = "ShortLabel";
            repoListBox.DataSource = bindingSource;

            this.FormClosing += new FormClosingEventHandler(RepositoryForm_FormClosing);

            searchTextBox.KeyPress += new KeyPressEventHandler(searchTextBox_KeyPress);
            repoListBox.MouseMove += new MouseEventHandler(repoListBox_MouseMove);
            repoListBox.MouseDown += new MouseEventHandler(repoListBox_MouseDown);
            repoListBox.MouseClick += new MouseEventHandler(repoListBox_MouseClick);
            repoListBox.SelectedIndexChanged += new EventHandler(repoListBox_SelectedIndexChanged);

            sLabelTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            labelTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            commentsTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            informationTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);

            sLabelTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            labelTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            commentsTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            informationTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            
            sLabelTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);
            labelTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);
            commentsTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);
            informationTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);

            repoListBox.SelectedItem = null;
            repoListBox_SelectedIndexChanged(repoListBox, new EventArgs());

            listLoadS6xToolStripSeparator.Visible = (sadS6x != null);
            listLoadS6xToolStripMenuItem.Visible = (sadS6x != null);
        }

        private void RepositoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!repoRepository.isSaved)
            {
                if (MessageBox.Show("Repository file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                Dispose();

                GC.Collect();
            }
        }

        private bool searchRepositoryItem(RepositoryItem rItem)
        {
            string search = searchTextBox.Text.ToUpper();

            if (search == string.Empty) return true;

            string[] searchedValues = new string[] { rItem.ShortLabel, rItem.Label, rItem.Comments, rItem.Information };

            foreach (string searchedValue in searchedValues)
            {
                if (searchedValue == null || searchedValue == string.Empty) continue;
                if (searchedValue.ToUpper().Contains(search)) return true;
            }

            return false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!repoRepository.Save(repoFilePath)) throw new Exception();

                sLabelTextBox.Modified = false;
                labelTextBox.Modified = false;
                commentsTextBox.Modified = false;
                informationTextBox.Modified = false;
            }
            catch
            {
                MessageBox.Show("Saving repository has failed.\r\n\r\nPlease check related file access.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 13) return;

            RepositoryItem prevSelectedRepoItem = selectedRepoItem;
            
            bindingSource.DataSource = repoRepository.Items.FindAll(searchRepositoryItem);

            if (prevSelectedRepoItem != null)
            {
                if (repoListBox.Items.Contains(prevSelectedRepoItem))
                {
                    repoListBox.SelectedItem = prevSelectedRepoItem;
                }
                else
                {
                    repoListBox.SelectedItem = null;
                    selectedRepoItem = null;
                }
                prevSelectedRepoItem = null;
            }
        }

        private void listAddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RepositoryItem newRepoItem = new RepositoryItem("NEW", "New");
            repoRepository.Items.Add(newRepoItem);
            repoRepository.isSaved = false;

            bindingSource.ResetBindings(false);

            repoListBox.SelectedItem = newRepoItem;
            repoListBox_SelectedIndexChanged(sender, e);
        }

        private void listRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (repoListBox.SelectedItem == null) return;
            repoRepository.Items.Remove((RepositoryItem)repoListBox.SelectedItem);
            repoRepository.isSaved = false;

            repoListBox.SelectedItem = null;
            repoListBox_SelectedIndexChanged(sender, e);

            bindingSource.ResetBindings(false);
        }

        private void listLoadS6xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sadS6x == null) return;
            List<RepositoryItem> s6xRepoItems = new List<RepositoryItem>();
            List<string> newLabels = null;
            string newLabel = string.Empty;

            switch (this.Text.ToUpper())
            {
                case "REGISTERS":
                    foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        RepositoryItem rItem = new RepositoryItem(s6xObject.Label, s6xObject.Label);
                        rItem.Comments = s6xObject.Comments;
                        s6xRepoItems.Add(rItem);
                    }
                    break;
                case "TABLES":
                    foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        RepositoryItem rItem = new RepositoryItem(s6xObject.ShortLabel, s6xObject.Label);
                        rItem.Comments = s6xObject.Comments;
                        s6xRepoItems.Add(rItem);
                    }
                    break;
                case "FUNCTIONS":
                    foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        RepositoryItem rItem = new RepositoryItem(s6xObject.ShortLabel, s6xObject.Label);
                        rItem.Comments = s6xObject.Comments;
                        s6xRepoItems.Add(rItem);
                    }
                    break;
                case "SCALARS":
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        RepositoryItem rItem = new RepositoryItem(s6xObject.ShortLabel, s6xObject.Label);
                        rItem.Comments = s6xObject.Comments;
                        s6xRepoItems.Add(rItem);
                    }
                    break;
                case "STRUCTURES":
                    foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        RepositoryItem rItem = new RepositoryItem(s6xObject.ShortLabel, s6xObject.Label);
                        rItem.Comments = s6xObject.Comments;
                        s6xRepoItems.Add(rItem);
                    }
                    break;
                case "UNITS":
                    newLabels = new List<string>();
                    foreach (S6xTable s6xObject in sadS6x.slTables.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        newLabel = s6xObject.CellsUnits;
                        newLabel = (newLabel == null) ? string.Empty : newLabel = newLabel.Trim();
                        if (newLabel != string.Empty) if (!newLabels.Contains(newLabel)) newLabels.Add(newLabel);
                        newLabel = s6xObject.ColsUnits;
                        newLabel = (newLabel == null) ? string.Empty : newLabel = newLabel.Trim();
                        if (newLabel != string.Empty) if (!newLabels.Contains(newLabel)) newLabels.Add(newLabel);
                        newLabel = s6xObject.RowsUnits;
                        newLabel = (newLabel == null) ? string.Empty : newLabel = newLabel.Trim();
                        if (newLabel != string.Empty) if (!newLabels.Contains(newLabel)) newLabels.Add(newLabel);
                    }
                    foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        newLabel = s6xObject.InputUnits;
                        newLabel = (newLabel == null) ? string.Empty : newLabel = newLabel.Trim();
                        if (newLabel != string.Empty) if (!newLabels.Contains(newLabel)) newLabels.Add(newLabel);
                        newLabel = s6xObject.OutputUnits;
                        newLabel = (newLabel == null) ? string.Empty : newLabel = newLabel.Trim();
                        if (newLabel != string.Empty) if (!newLabels.Contains(newLabel)) newLabels.Add(newLabel);
                    }
                    foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
                    {
                        if (!s6xObject.Store || s6xObject.Skip) continue;
                        newLabel = s6xObject.Units;
                        newLabel = (newLabel == null) ? string.Empty : newLabel = newLabel.Trim();
                        if (newLabel != string.Empty) if (!newLabels.Contains(newLabel)) newLabels.Add(newLabel);
                    }
                    foreach (string nLabel in newLabels) s6xRepoItems.Add(new RepositoryItem(nLabel, null));
                    break;
                default:
                    return;
            }

            foreach (RepositoryItem s6xRepoItem in s6xRepoItems)
            {
                bool bExists = false;
                foreach (RepositoryItem currentRepoItem in repoRepository.Items)
                {
                    if (s6xRepoItem.ShortLabel != currentRepoItem.ShortLabel) continue;
                    bExists = true;
                    currentRepoItem.Label = s6xRepoItem.Label;
                    currentRepoItem.Comments = s6xRepoItem.Comments;
                }
                if (bExists) continue;
                repoRepository.Items.Add(s6xRepoItem);
            }

            repoRepository.isSaved = false;
            bindingSource.ResetBindings(false);
        }

        private void ShortLabelLabel_MouseMove(object sender, MouseEventArgs e)
        {
            string newTip = string.Empty;
            
            switch (specificType)
            {
                case "OBDIERRORS":
                    newTip = SADDef.repoToolTipOBDIErrorsShortLabel;
                    break;
                case "OBDIIERRORS":
                    newTip = SADDef.repoToolTipOBDIIErrorsShortLabel;
                    break;
            }

            if (newTip == string.Empty) return;

            if (repoToolTip.GetToolTip(sLabelLabel) != newTip) repoToolTip.SetToolTip(sLabelLabel, newTip);
        }

        private void repoListBox_MouseMove(object sender, MouseEventArgs e)
        {
            int itemIndex = repoListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0) return;

            string newTip = ((RepositoryItem)repoListBox.Items[itemIndex]).Label;
            if (repoToolTip.GetToolTip(repoListBox) != newTip) repoToolTip.SetToolTip(repoListBox, newTip);
        }

        private void repoListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int itemIndex = repoListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0)
            {
                selectedRepoItem = null;
                return;
            }

            repoListBox.SelectedIndex = itemIndex;
        }

        private void repoListBox_MouseClick(object sender, MouseEventArgs e)
        {
            int itemIndex = repoListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0)
            {
                selectedRepoItem = null;
                return;
            }

            repoListBox.SelectedIndex = itemIndex;
        }

        private void repoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedRepoItem = (RepositoryItem)repoListBox.SelectedItem;

            sLabelTextBox.DataBindings.Clear();
            labelTextBox.DataBindings.Clear();
            commentsTextBox.DataBindings.Clear();
            informationTextBox.DataBindings.Clear();

            detailsPanel.Visible = (selectedRepoItem != null);

            if (selectedRepoItem == null) return;

            sLabelTextBox.DataBindings.Add("Text", this.selectedRepoItem, "ShortLabel", false, DataSourceUpdateMode.OnPropertyChanged);
            labelTextBox.DataBindings.Add("Text", this.selectedRepoItem, "Label", false, DataSourceUpdateMode.OnPropertyChanged);
            commentsTextBox.DataBindings.Add("Text", this.selectedRepoItem, "Comments", false, DataSourceUpdateMode.OnPropertyChanged);
            informationTextBox.DataBindings.Add("Text", this.selectedRepoItem, "Information", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void RepoItemTextBox_ModifiedChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Modified)
            {
                repoRepository.isSaved = false;

                bindingSource.ResetBindings(false);
            }
        }

        private void RepoItemTextBox_Leave(object sender, EventArgs e)
        {
            RepoItemTextBox_ModifiedChanged(sender, e);
        }

        private void RepoItemTextBox_KeyDown(object sender, KeyEventArgs e)
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
            }
        }
    }
}
