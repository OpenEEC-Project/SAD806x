namespace SAD806x
{
    partial class SearchSignatureForm
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
            this.components = new System.ComponentModel.Container();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.searchButtonsPanel = new System.Windows.Forms.Panel();
            this.searchButton = new System.Windows.Forms.Button();
            this.mainTipPictureBox = new System.Windows.Forms.PictureBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchTreeView = new System.Windows.Forms.TreeView();
            this.mainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.searchPanel.SuspendLayout();
            this.searchButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainTipPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // searchPanel
            // 
            this.searchPanel.AutoSize = true;
            this.searchPanel.Controls.Add(this.searchButtonsPanel);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 139);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(377, 30);
            this.searchPanel.TabIndex = 3;
            // 
            // searchButtonsPanel
            // 
            this.searchButtonsPanel.Controls.Add(this.searchButton);
            this.searchButtonsPanel.Controls.Add(this.mainTipPictureBox);
            this.searchButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchButtonsPanel.Location = new System.Drawing.Point(0, 0);
            this.searchButtonsPanel.MinimumSize = new System.Drawing.Size(30, 30);
            this.searchButtonsPanel.Name = "searchButtonsPanel";
            this.searchButtonsPanel.Size = new System.Drawing.Size(377, 30);
            this.searchButtonsPanel.TabIndex = 5;
            // 
            // searchButton
            // 
            this.searchButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.searchButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchButton.Location = new System.Drawing.Point(0, 0);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(347, 30);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = ">";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // mainTipPictureBox
            // 
            this.mainTipPictureBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.mainTipPictureBox.Image = global::SAD806x.Properties.Resources.question;
            this.mainTipPictureBox.Location = new System.Drawing.Point(347, 0);
            this.mainTipPictureBox.MaximumSize = new System.Drawing.Size(30, 30);
            this.mainTipPictureBox.MinimumSize = new System.Drawing.Size(30, 30);
            this.mainTipPictureBox.Name = "mainTipPictureBox";
            this.mainTipPictureBox.Size = new System.Drawing.Size(30, 30);
            this.mainTipPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mainTipPictureBox.TabIndex = 6;
            this.mainTipPictureBox.TabStop = false;
            // 
            // searchTextBox
            // 
            this.searchTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchTextBox.Location = new System.Drawing.Point(0, 0);
            this.searchTextBox.Multiline = true;
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.searchTextBox.Size = new System.Drawing.Size(377, 139);
            this.searchTextBox.TabIndex = 0;
            // 
            // searchTreeView
            // 
            this.searchTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTreeView.Location = new System.Drawing.Point(0, 169);
            this.searchTreeView.Name = "searchTreeView";
            this.searchTreeView.Size = new System.Drawing.Size(377, 149);
            this.searchTreeView.TabIndex = 4;
            // 
            // mainToolTip
            // 
            this.mainToolTip.AutoPopDelay = 60000;
            this.mainToolTip.InitialDelay = 500;
            this.mainToolTip.ReshowDelay = 100;
            // 
            // SearchSignatureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 318);
            this.Controls.Add(this.searchTreeView);
            this.Controls.Add(this.searchPanel);
            this.Controls.Add(this.searchTextBox);
            this.Name = "SearchSignatureForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search Signature";
            this.searchPanel.ResumeLayout(false);
            this.searchButtonsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainTipPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TreeView searchTreeView;
        private System.Windows.Forms.ToolTip mainToolTip;
        private System.Windows.Forms.Panel searchButtonsPanel;
        private System.Windows.Forms.PictureBox mainTipPictureBox;
    }
}