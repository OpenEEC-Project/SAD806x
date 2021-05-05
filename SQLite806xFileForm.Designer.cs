namespace SAD806x
{
    partial class SQLite806xFileForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialogBin = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialogGeneric = new System.Windows.Forms.OpenFileDialog();
            this.uploadButton = new System.Windows.Forms.Button();
            this.topPanel = new System.Windows.Forms.Panel();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.selectButton = new System.Windows.Forms.Button();
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.centerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.commentsPanel = new System.Windows.Forms.Panel();
            this.commentsTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.descriptionSortNumberPanel = new System.Windows.Forms.Panel();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sortNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.topPanel.SuspendLayout();
            this.centerSplitContainer.Panel1.SuspendLayout();
            this.centerSplitContainer.Panel2.SuspendLayout();
            this.centerSplitContainer.SuspendLayout();
            this.commentsPanel.SuspendLayout();
            this.descriptionSortNumberPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sortNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialogBin
            // 
            this.openFileDialogBin.Filter = "Binary File|*.bin|All Files|*.*";
            // 
            // openFileDialogGeneric
            // 
            this.openFileDialogGeneric.Filter = "All Files|*.*";
            // 
            // uploadButton
            // 
            this.uploadButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.uploadButton.Enabled = false;
            this.uploadButton.Location = new System.Drawing.Point(0, 0);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(75, 21);
            this.uploadButton.TabIndex = 0;
            this.uploadButton.Text = "Upload";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.filePathTextBox);
            this.topPanel.Controls.Add(this.selectButton);
            this.topPanel.Controls.Add(this.uploadButton);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(488, 21);
            this.topPanel.TabIndex = 1;
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePathTextBox.Location = new System.Drawing.Point(75, 0);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.ReadOnly = true;
            this.filePathTextBox.Size = new System.Drawing.Size(338, 20);
            this.filePathTextBox.TabIndex = 1;
            // 
            // selectButton
            // 
            this.selectButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.selectButton.Location = new System.Drawing.Point(413, 0);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 21);
            this.selectButton.TabIndex = 2;
            this.selectButton.Text = "Select";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // fileTreeView
            // 
            this.fileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTreeView.Location = new System.Drawing.Point(0, 0);
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.ShowNodeToolTips = true;
            this.fileTreeView.Size = new System.Drawing.Size(488, 160);
            this.fileTreeView.TabIndex = 3;
            // 
            // centerSplitContainer
            // 
            this.centerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.centerSplitContainer.Location = new System.Drawing.Point(0, 21);
            this.centerSplitContainer.Name = "centerSplitContainer";
            this.centerSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // centerSplitContainer.Panel1
            // 
            this.centerSplitContainer.Panel1.Controls.Add(this.fileTreeView);
            // 
            // centerSplitContainer.Panel2
            // 
            this.centerSplitContainer.Panel2.Controls.Add(this.commentsPanel);
            this.centerSplitContainer.Panel2.Controls.Add(this.descriptionSortNumberPanel);
            this.centerSplitContainer.Size = new System.Drawing.Size(488, 255);
            this.centerSplitContainer.SplitterDistance = 160;
            this.centerSplitContainer.TabIndex = 4;
            // 
            // commentsPanel
            // 
            this.commentsPanel.Controls.Add(this.commentsTextBox);
            this.commentsPanel.Controls.Add(this.label3);
            this.commentsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commentsPanel.Location = new System.Drawing.Point(0, 21);
            this.commentsPanel.Name = "commentsPanel";
            this.commentsPanel.Size = new System.Drawing.Size(488, 70);
            this.commentsPanel.TabIndex = 3;
            // 
            // commentsTextBox
            // 
            this.commentsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commentsTextBox.Location = new System.Drawing.Point(0, 20);
            this.commentsTextBox.Multiline = true;
            this.commentsTextBox.Name = "commentsTextBox";
            this.commentsTextBox.Size = new System.Drawing.Size(488, 50);
            this.commentsTextBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(488, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Comments";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // descriptionSortNumberPanel
            // 
            this.descriptionSortNumberPanel.Controls.Add(this.descriptionTextBox);
            this.descriptionSortNumberPanel.Controls.Add(this.label2);
            this.descriptionSortNumberPanel.Controls.Add(this.label1);
            this.descriptionSortNumberPanel.Controls.Add(this.sortNumberNumericUpDown);
            this.descriptionSortNumberPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.descriptionSortNumberPanel.Location = new System.Drawing.Point(0, 0);
            this.descriptionSortNumberPanel.Name = "descriptionSortNumberPanel";
            this.descriptionSortNumberPanel.Size = new System.Drawing.Size(488, 21);
            this.descriptionSortNumberPanel.TabIndex = 1;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionTextBox.Location = new System.Drawing.Point(60, 0);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(325, 20);
            this.descriptionTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "Description";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(385, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sort number";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sortNumberNumericUpDown
            // 
            this.sortNumberNumericUpDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.sortNumberNumericUpDown.Location = new System.Drawing.Point(451, 0);
            this.sortNumberNumericUpDown.Name = "sortNumberNumericUpDown";
            this.sortNumberNumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.sortNumberNumericUpDown.TabIndex = 5;
            this.sortNumberNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SQLite806xFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 276);
            this.Controls.Add(this.centerSplitContainer);
            this.Controls.Add(this.topPanel);
            this.Name = "SQLite806xFileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File upload";
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.centerSplitContainer.Panel1.ResumeLayout(false);
            this.centerSplitContainer.Panel2.ResumeLayout(false);
            this.centerSplitContainer.ResumeLayout(false);
            this.commentsPanel.ResumeLayout(false);
            this.commentsPanel.PerformLayout();
            this.descriptionSortNumberPanel.ResumeLayout(false);
            this.descriptionSortNumberPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sortNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogBin;
        private System.Windows.Forms.OpenFileDialog openFileDialogGeneric;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.TreeView fileTreeView;
        private System.Windows.Forms.SplitContainer centerSplitContainer;
        private System.Windows.Forms.Panel descriptionSortNumberPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown sortNumberNumericUpDown;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel commentsPanel;
        private System.Windows.Forms.TextBox commentsTextBox;
        private System.Windows.Forms.Label label3;
    }
}