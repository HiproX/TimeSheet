namespace TimeSheet
{
    partial class FormTable
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.departmentsGroup = new System.Windows.Forms.GroupBox();
            this.departmentsList = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControlPanel = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gridView = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.departmentsGroup.SuspendLayout();
            this.tabControlPanel.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // departmentsGroup
            // 
            this.departmentsGroup.Controls.Add(this.departmentsList);
            this.departmentsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.departmentsGroup.Location = new System.Drawing.Point(0, 0);
            this.departmentsGroup.Name = "departmentsGroup";
            this.departmentsGroup.Size = new System.Drawing.Size(257, 592);
            this.departmentsGroup.TabIndex = 0;
            this.departmentsGroup.TabStop = false;
            this.departmentsGroup.Text = "Департаменты";
            // 
            // departmentsList
            // 
            this.departmentsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.departmentsList.FormattingEnabled = true;
            this.departmentsList.ItemHeight = 15;
            this.departmentsList.Location = new System.Drawing.Point(6, 27);
            this.departmentsList.Name = "departmentsList";
            this.departmentsList.Size = new System.Drawing.Size(245, 544);
            this.departmentsList.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 592);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1287, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabControlPanel
            // 
            this.tabControlPanel.Controls.Add(this.tabPage1);
            this.tabControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPanel.Location = new System.Drawing.Point(0, 0);
            this.tabControlPanel.Name = "tabControlPanel";
            this.tabControlPanel.SelectedIndex = 0;
            this.tabControlPanel.Size = new System.Drawing.Size(1026, 592);
            this.tabControlPanel.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControlPanel.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1018, 564);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.gridView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1012, 558);
            this.panel1.TabIndex = 0;
            // 
            // gridView
            // 
            this.gridView.AllowUserToAddRows = false;
            this.gridView.AllowUserToDeleteRows = false;
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView.Location = new System.Drawing.Point(0, 0);
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.RowTemplate.Height = 25;
            this.gridView.Size = new System.Drawing.Size(1012, 558);
            this.gridView.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.departmentsGroup);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlPanel);
            this.splitContainer1.Size = new System.Drawing.Size(1287, 592);
            this.splitContainer1.SplitterDistance = 257;
            this.splitContainer1.TabIndex = 3;
            // 
            // FormTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1287, 614);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "FormTable";
            this.Text = "Табель";
            this.Load += new System.EventHandler(this.FormTable_Load);
            this.departmentsGroup.ResumeLayout(false);
            this.tabControlPanel.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox departmentsGroup;
        private StatusStrip statusStrip1;
        private TabControl tabControlPanel;
        private ListBox departmentsList;
        private SplitContainer splitContainer1;
        private TabPage tabPage1;
        private Panel panel1;
        public DataGridView gridView;
    }
}