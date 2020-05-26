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
    public partial class ConversionRepoForm : Form
    {
        private string repoFilePath = string.Empty;
        private RepositoryConversion repoRepository = null;
        private RepositoryConversionItem selectedRepoItem = null;
        private BindingSource bindingSource = null;
        private SADS6x sadS6x = null;

        public ConversionRepoForm(string filePath, ref SADS6x s6x)
        {
            InitializeComponent();

            repoFilePath = filePath;
            sadS6x = s6x;

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            repoRepository = (RepositoryConversion)ToolsXml.DeserializeFile(repoFilePath, typeof(RepositoryConversion));
            if (repoRepository == null) repoRepository = new RepositoryConversion();
            repoRepository.isSaved = true;

            bindingSource = new BindingSource();
            bindingSource.DataSource = repoRepository.Items.FindAll(searchRepositoryConversionItem);

            repoListBox.DisplayMember = "Title";
            repoListBox.DataSource = bindingSource;

            this.FormClosing += new FormClosingEventHandler(RepositoryForm_FormClosing);

            searchTextBox.KeyPress += new KeyPressEventHandler(searchTextBox_KeyPress);
            repoListBox.MouseMove += new MouseEventHandler(repoListBox_MouseMove);
            repoListBox.MouseDown += new MouseEventHandler(repoListBox_MouseDown);
            repoListBox.MouseClick += new MouseEventHandler(repoListBox_MouseClick);
            repoListBox.SelectedIndexChanged += new EventHandler(repoListBox_SelectedIndexChanged);
            
            titleTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            iFormulaTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            commentsTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            informationTextBox.ModifiedChanged += new EventHandler(RepoItemTextBox_ModifiedChanged);
            
            titleTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            iFormulaTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            commentsTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            informationTextBox.KeyDown += new KeyEventHandler(RepoItemTextBox_KeyDown);
            
            titleTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);
            iFormulaTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);
            commentsTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);
            informationTextBox.Leave += new EventHandler(RepoItemTextBox_Leave);

            iFormulaTextBox.TextChanged += new EventHandler(iFormulaTextBox_TextChanged);

            repoListBox.SelectedItem = null;
            repoListBox_SelectedIndexChanged(repoListBox, new EventArgs());
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

        private bool searchRepositoryConversionItem(RepositoryConversionItem rItem)
        {
            string search = searchTextBox.Text.ToUpper();

            if (search == string.Empty) return true;

            string[] searchedValues = new string[] { rItem.Title, rItem.Comments, rItem.Information, rItem.InternalFormula };
            
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

                titleTextBox.Modified = false;
                iFormulaTextBox.Modified = false;
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

            RepositoryConversionItem prevSelectedRepoItem = selectedRepoItem;

            bindingSource.DataSource = repoRepository.Items.FindAll(searchRepositoryConversionItem);

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
            RepositoryConversionItem newRepoItem = new RepositoryConversionItem("New");
            repoRepository.Items.Add(newRepoItem);
            repoRepository.isSaved = false;

            bindingSource.ResetBindings(false);

            repoListBox.SelectedItem = newRepoItem;
            repoListBox_SelectedIndexChanged(sender, e);
        }

        private void listRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (repoListBox.SelectedItem == null) return;
            repoRepository.Items.Remove((RepositoryConversionItem)repoListBox.SelectedItem);
            repoRepository.isSaved = false;

            repoListBox.SelectedItem = null;
            repoListBox_SelectedIndexChanged(sender, e);

            bindingSource.ResetBindings(false);
        }

        private void repoListBox_MouseMove(object sender, MouseEventArgs e)
        {
            int itemIndex = repoListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0) return;

            string newTip = ((RepositoryConversionItem)repoListBox.Items[itemIndex]).InternalFormula;
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
            selectedRepoItem = (RepositoryConversionItem)repoListBox.SelectedItem;

            titleTextBox.DataBindings.Clear();
            iFormulaTextBox.DataBindings.Clear();
            commentsTextBox.DataBindings.Clear();
            informationTextBox.DataBindings.Clear();

            detailsPanel.Visible = (selectedRepoItem != null);

            if (selectedRepoItem == null) return;

            titleTextBox.DataBindings.Add("Text", this.selectedRepoItem, "Title", false, DataSourceUpdateMode.OnPropertyChanged);
            iFormulaTextBox.DataBindings.Add("Text", this.selectedRepoItem, "InternalFormula", false, DataSourceUpdateMode.OnPropertyChanged);
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

        private void iFormulaTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Tools.ScaleExpressionCheck(iFormulaTextBox.Text)) iFormulaLabel.ForeColor = titleLabel.ForeColor;
            else iFormulaLabel.ForeColor = Color.Red;
        }
    }
}
