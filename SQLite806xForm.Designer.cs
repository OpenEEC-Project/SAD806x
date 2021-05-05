namespace SAD806x
{
    partial class SQLite806xForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLite806xForm));
            this.dbSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.dbLargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.dbStateImageList = new System.Windows.Forms.ImageList(this.components);
            this.dbListViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableItemsToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbListView = new System.Windows.Forms.ListView();
            this.labelColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.typeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.dbListViewContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dbSmallImageList
            // 
            this.dbSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dbSmallImageList.ImageStream")));
            this.dbSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dbSmallImageList.Images.SetKeyName(0, "sqLiteTable");
            this.dbSmallImageList.Images.SetKeyName(1, "sqLiteRow");
            // 
            // dbLargeImageList
            // 
            this.dbLargeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dbLargeImageList.ImageStream")));
            this.dbLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dbLargeImageList.Images.SetKeyName(0, "sqLiteTable");
            this.dbLargeImageList.Images.SetKeyName(1, "sqLiteRow");
            // 
            // dbStateImageList
            // 
            this.dbStateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("dbStateImageList.ImageStream")));
            this.dbStateImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.dbStateImageList.Images.SetKeyName(0, "sqLiteTable");
            this.dbStateImageList.Images.SetKeyName(1, "sqLiteRow");
            // 
            // dbListViewContextMenuStrip
            // 
            this.dbListViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.tableItemsToolStripSeparator,
            this.viewToolStripMenuItem});
            this.dbListViewContextMenuStrip.Name = "dbTreeViewContextMenuStrip";
            this.dbListViewContextMenuStrip.Size = new System.Drawing.Size(118, 76);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // tableItemsToolStripSeparator
            // 
            this.tableItemsToolStripSeparator.Name = "tableItemsToolStripSeparator";
            this.tableItemsToolStripSeparator.Size = new System.Drawing.Size(114, 6);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // dbListView
            // 
            this.dbListView.AutoArrange = false;
            this.dbListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.labelColumnHeader,
            this.typeColumnHeader,
            this.descriptionColumnHeader});
            this.dbListView.ContextMenuStrip = this.dbListViewContextMenuStrip;
            this.dbListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbListView.LargeImageList = this.dbLargeImageList;
            this.dbListView.Location = new System.Drawing.Point(0, 0);
            this.dbListView.Name = "dbListView";
            this.dbListView.ShowItemToolTips = true;
            this.dbListView.Size = new System.Drawing.Size(1136, 777);
            this.dbListView.SmallImageList = this.dbSmallImageList;
            this.dbListView.StateImageList = this.dbStateImageList;
            this.dbListView.TabIndex = 0;
            this.dbListView.UseCompatibleStateImageBehavior = false;
            this.dbListView.View = System.Windows.Forms.View.Tile;
            // 
            // labelColumnHeader
            // 
            this.labelColumnHeader.Text = "Item";
            // 
            // typeColumnHeader
            // 
            this.typeColumnHeader.Text = "Type";
            // 
            // descriptionColumnHeader
            // 
            this.descriptionColumnHeader.Text = "Description";
            // 
            // SQLite806xForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 777);
            this.Controls.Add(this.dbListView);
            this.Name = "SQLite806xForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "806x SQLite Database";
            this.dbListViewContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList dbSmallImageList;
        private System.Windows.Forms.ImageList dbLargeImageList;
        private System.Windows.Forms.ImageList dbStateImageList;
        private System.Windows.Forms.ContextMenuStrip dbListViewContextMenuStrip;
        private System.Windows.Forms.ListView dbListView;
        private System.Windows.Forms.ColumnHeader labelColumnHeader;
        private System.Windows.Forms.ColumnHeader typeColumnHeader;
        private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator tableItemsToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    }
}