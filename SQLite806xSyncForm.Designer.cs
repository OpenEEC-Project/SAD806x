namespace SAD806x
{
    partial class SQLite806xSyncForm
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
            this.syncTreeView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.confirmButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // syncTreeView
            // 
            this.syncTreeView.CheckBoxes = true;
            this.syncTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syncTreeView.Location = new System.Drawing.Point(0, 13);
            this.syncTreeView.Name = "syncTreeView";
            this.syncTreeView.ShowNodeToolTips = true;
            this.syncTreeView.Size = new System.Drawing.Size(284, 226);
            this.syncTreeView.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unselect objects to remove them";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // confirmButton
            // 
            this.confirmButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.confirmButton.Location = new System.Drawing.Point(0, 239);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(284, 22);
            this.confirmButton.TabIndex = 2;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // SQLite806xSyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.syncTreeView);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.label1);
            this.Name = "SQLite806xSyncForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "806x SQLite Objects Validation";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView syncTreeView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button confirmButton;
    }
}