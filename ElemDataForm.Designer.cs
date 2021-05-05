namespace SAD806x
{
    partial class ElemDataForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.elemDataGridView = new System.Windows.Forms.DataGridView();
            this.elemContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.decimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decimalNotConvertedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.convertInputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topPanel = new System.Windows.Forms.Panel();
            this.elemLabelTextBox = new System.Windows.Forms.TextBox();
            this.elemBankTextBox = new System.Windows.Forms.TextBox();
            this.elemAddressTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.elemDataGridView)).BeginInit();
            this.elemContextMenuStrip.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // elemDataGridView
            // 
            this.elemDataGridView.AllowUserToAddRows = false;
            this.elemDataGridView.AllowUserToDeleteRows = false;
            this.elemDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.elemDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.elemDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.elemDataGridView.ColumnHeadersHeight = 20;
            this.elemDataGridView.ContextMenuStrip = this.elemContextMenuStrip;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.elemDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.elemDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elemDataGridView.Location = new System.Drawing.Point(0, 24);
            this.elemDataGridView.MultiSelect = false;
            this.elemDataGridView.Name = "elemDataGridView";
            this.elemDataGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.elemDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.elemDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.elemDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.elemDataGridView.ShowCellErrors = false;
            this.elemDataGridView.ShowCellToolTips = false;
            this.elemDataGridView.ShowRowErrors = false;
            this.elemDataGridView.Size = new System.Drawing.Size(884, 297);
            this.elemDataGridView.TabIndex = 4;
            // 
            // elemContextMenuStrip
            // 
            this.elemContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decimalToolStripMenuItem,
            this.decimalNotConvertedToolStripMenuItem,
            this.reverseOrderToolStripMenuItem,
            this.convertToolStripSeparator,
            this.convertInputToolStripMenuItem,
            this.convertToolStripMenuItem});
            this.elemContextMenuStrip.Name = "calElemContextMenuStrip";
            this.elemContextMenuStrip.Size = new System.Drawing.Size(234, 120);
            // 
            // decimalToolStripMenuItem
            // 
            this.decimalToolStripMenuItem.Checked = true;
            this.decimalToolStripMenuItem.CheckOnClick = true;
            this.decimalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.decimalToolStripMenuItem.Name = "decimalToolStripMenuItem";
            this.decimalToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.decimalToolStripMenuItem.Text = "Decimal";
            // 
            // decimalNotConvertedToolStripMenuItem
            // 
            this.decimalNotConvertedToolStripMenuItem.CheckOnClick = true;
            this.decimalNotConvertedToolStripMenuItem.Name = "decimalNotConvertedToolStripMenuItem";
            this.decimalNotConvertedToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.decimalNotConvertedToolStripMenuItem.Text = "Ignore conversion";
            // 
            // reverseOrderToolStripMenuItem
            // 
            this.reverseOrderToolStripMenuItem.CheckOnClick = true;
            this.reverseOrderToolStripMenuItem.Name = "reverseOrderToolStripMenuItem";
            this.reverseOrderToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.reverseOrderToolStripMenuItem.Text = "Reverse Order";
            // 
            // convertToolStripSeparator
            // 
            this.convertToolStripSeparator.Name = "convertToolStripSeparator";
            this.convertToolStripSeparator.Size = new System.Drawing.Size(230, 6);
            // 
            // convertInputToolStripMenuItem
            // 
            this.convertInputToolStripMenuItem.Name = "convertInputToolStripMenuItem";
            this.convertInputToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.convertInputToolStripMenuItem.Text = "Additional Input Conversion";
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.convertToolStripMenuItem.Text = "Additional Output Conversion";
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.elemLabelTextBox);
            this.topPanel.Controls.Add(this.elemBankTextBox);
            this.topPanel.Controls.Add(this.elemAddressTextBox);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(884, 24);
            this.topPanel.TabIndex = 7;
            // 
            // elemLabelTextBox
            // 
            this.elemLabelTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.elemLabelTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elemLabelTextBox.Location = new System.Drawing.Point(0, 0);
            this.elemLabelTextBox.Name = "elemLabelTextBox";
            this.elemLabelTextBox.ReadOnly = true;
            this.elemLabelTextBox.Size = new System.Drawing.Size(806, 20);
            this.elemLabelTextBox.TabIndex = 4;
            this.elemLabelTextBox.Text = "Calibration Element Label";
            // 
            // elemBankTextBox
            // 
            this.elemBankTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.elemBankTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.elemBankTextBox.Location = new System.Drawing.Point(806, 0);
            this.elemBankTextBox.Name = "elemBankTextBox";
            this.elemBankTextBox.ReadOnly = true;
            this.elemBankTextBox.Size = new System.Drawing.Size(30, 20);
            this.elemBankTextBox.TabIndex = 6;
            this.elemBankTextBox.Text = "Bank";
            // 
            // elemAddressTextBox
            // 
            this.elemAddressTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.elemAddressTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.elemAddressTextBox.Location = new System.Drawing.Point(836, 0);
            this.elemAddressTextBox.Name = "elemAddressTextBox";
            this.elemAddressTextBox.ReadOnly = true;
            this.elemAddressTextBox.Size = new System.Drawing.Size(48, 20);
            this.elemAddressTextBox.TabIndex = 7;
            this.elemAddressTextBox.Text = "Addr";
            this.elemAddressTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ElemDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 321);
            this.Controls.Add(this.elemDataGridView);
            this.Controls.Add(this.topPanel);
            this.Name = "ElemDataForm";
            this.Text = "ElemDataForm";
            ((System.ComponentModel.ISupportInitialize)(this.elemDataGridView)).EndInit();
            this.elemContextMenuStrip.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView elemDataGridView;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.TextBox elemLabelTextBox;
        private System.Windows.Forms.TextBox elemBankTextBox;
        private System.Windows.Forms.TextBox elemAddressTextBox;
        private System.Windows.Forms.ContextMenuStrip elemContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem decimalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decimalNotConvertedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reverseOrderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator convertToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem convertInputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
    }
}