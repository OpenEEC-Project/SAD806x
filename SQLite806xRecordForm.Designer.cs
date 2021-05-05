namespace SAD806x
{
    partial class SQLite806xRecordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLite806xRecordForm));
            this.dbLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.dbSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.fileContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator33 = new System.Windows.Forms.ToolStripSeparator();
            this.fileDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
            this.fileRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogGeneric = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogGeneric = new System.Windows.Forms.SaveFileDialog();
            this.fileContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dbLargeImageList
            // 
            this.dbLargeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dbLargeImageList.ImageStream")));
            this.dbLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dbLargeImageList.Images.SetKeyName(0, "sqLiteTable");
            this.dbLargeImageList.Images.SetKeyName(1, "sqLiteRow");
            this.dbLargeImageList.Images.SetKeyName(2, "sqLiteAdd");
            this.dbLargeImageList.Images.SetKeyName(3, "sqLiteUpdate");
            this.dbLargeImageList.Images.SetKeyName(4, "sqLiteRemove");
            // 
            // dbSmallImageList
            // 
            this.dbSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dbSmallImageList.ImageStream")));
            this.dbSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dbSmallImageList.Images.SetKeyName(0, "sqLiteTable");
            this.dbSmallImageList.Images.SetKeyName(1, "sqLiteRow");
            this.dbSmallImageList.Images.SetKeyName(2, "sqLiteAdd");
            this.dbSmallImageList.Images.SetKeyName(3, "sqLiteUpdate");
            this.dbSmallImageList.Images.SetKeyName(4, "sqLiteRemove");
            // 
            // fileContextMenuStrip
            // 
            this.fileContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileLabelToolStripMenuItem,
            this.toolStripSeparator33,
            this.fileDownloadToolStripMenuItem,
            this.fileUpdateToolStripMenuItem,
            this.toolStripSeparator34,
            this.fileRemoveToolStripMenuItem});
            this.fileContextMenuStrip.Name = "shortCutsContextMenuStrip";
            this.fileContextMenuStrip.Size = new System.Drawing.Size(129, 104);
            // 
            // fileLabelToolStripMenuItem
            // 
            this.fileLabelToolStripMenuItem.Name = "fileLabelToolStripMenuItem";
            this.fileLabelToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.fileLabelToolStripMenuItem.Text = "File Label";
            // 
            // toolStripSeparator33
            // 
            this.toolStripSeparator33.Name = "toolStripSeparator33";
            this.toolStripSeparator33.Size = new System.Drawing.Size(125, 6);
            // 
            // fileDownloadToolStripMenuItem
            // 
            this.fileDownloadToolStripMenuItem.Name = "fileDownloadToolStripMenuItem";
            this.fileDownloadToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.fileDownloadToolStripMenuItem.Text = "Download";
            this.fileDownloadToolStripMenuItem.Click += new System.EventHandler(this.fileDownloadToolStripMenuItem_Click);
            // 
            // fileUpdateToolStripMenuItem
            // 
            this.fileUpdateToolStripMenuItem.Name = "fileUpdateToolStripMenuItem";
            this.fileUpdateToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.fileUpdateToolStripMenuItem.Text = "Update";
            this.fileUpdateToolStripMenuItem.Click += new System.EventHandler(this.fileUpdateToolStripMenuItem_Click);
            // 
            // toolStripSeparator34
            // 
            this.toolStripSeparator34.Name = "toolStripSeparator34";
            this.toolStripSeparator34.Size = new System.Drawing.Size(125, 6);
            // 
            // fileRemoveToolStripMenuItem
            // 
            this.fileRemoveToolStripMenuItem.Name = "fileRemoveToolStripMenuItem";
            this.fileRemoveToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.fileRemoveToolStripMenuItem.Text = "Remove";
            this.fileRemoveToolStripMenuItem.Click += new System.EventHandler(this.fileRemoveToolStripMenuItem_Click);
            // 
            // openFileDialogGeneric
            // 
            this.openFileDialogGeneric.Filter = "All Files|*.*";
            // 
            // saveFileDialogGeneric
            // 
            this.saveFileDialogGeneric.Filter = "All Files|*.*";
            // 
            // SQLite806xRecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 427);
            this.Name = "SQLite806xRecordForm";
            this.Text = "SQLite806xRecordForm";
            this.fileContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList dbLargeImageList;
        private System.Windows.Forms.ImageList dbSmallImageList;
        private System.Windows.Forms.ContextMenuStrip fileContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileLabelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator33;
        private System.Windows.Forms.ToolStripMenuItem fileDownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator34;
        private System.Windows.Forms.ToolStripMenuItem fileRemoveToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogGeneric;
        private System.Windows.Forms.SaveFileDialog saveFileDialogGeneric;

    }
}