namespace SAD806x
{
    partial class HexForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.hexDataGridView = new System.Windows.Forms.DataGridView();
            this.hexContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.hexDataGridView)).BeginInit();
            this.hexContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // hexDataGridView
            // 
            this.hexDataGridView.AllowUserToAddRows = false;
            this.hexDataGridView.AllowUserToDeleteRows = false;
            this.hexDataGridView.AllowUserToResizeColumns = false;
            this.hexDataGridView.AllowUserToResizeRows = false;
            this.hexDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.hexDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.hexDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.hexDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.hexDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.hexDataGridView.ContextMenuStrip = this.hexContextMenuStrip;
            this.hexDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexDataGridView.Location = new System.Drawing.Point(0, 0);
            this.hexDataGridView.Name = "hexDataGridView";
            this.hexDataGridView.ReadOnly = true;
            this.hexDataGridView.RowHeadersVisible = false;
            this.hexDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.hexDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.hexDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.hexDataGridView.ShowCellErrors = false;
            this.hexDataGridView.ShowCellToolTips = false;
            this.hexDataGridView.ShowEditingIcon = false;
            this.hexDataGridView.ShowRowErrors = false;
            this.hexDataGridView.Size = new System.Drawing.Size(684, 502);
            this.hexDataGridView.TabIndex = 0;
            // 
            // hexContextMenuStrip
            // 
            this.hexContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.hexContextMenuStrip.Name = "hexContextMenuStrip";
            this.hexContextMenuStrip.Size = new System.Drawing.Size(145, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // HexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 502);
            this.Controls.Add(this.hexDataGridView);
            this.Name = "HexForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hex Editor";
            this.Load += new System.EventHandler(this.HexForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.hexDataGridView)).EndInit();
            this.hexContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView hexDataGridView;
        private System.Windows.Forms.ContextMenuStrip hexContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}