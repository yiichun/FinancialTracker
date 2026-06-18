namespace FinancialTracker
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tpSummary = new System.Windows.Forms.TabPage();
            this.pnlAlert = new System.Windows.Forms.Panel();
            this.btnAlertConfirm = new System.Windows.Forms.Button();
            this.lblAlertMsg = new System.Windows.Forms.Label();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbFilterCategory = new System.Windows.Forms.ComboBox();
            this.pnlActionButtons = new System.Windows.Forms.Panel();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlDashboard = new System.Windows.Forms.Panel();
            this.lblMinExpense = new System.Windows.Forms.Label();
            this.lblMaxExpense = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.lblTotalExpense = new System.Windows.Forms.Label();
            this.lblTotalIncome = new System.Windows.Forms.Label();
            this.tpBudget = new System.Windows.Forms.TabPage();
            this.pnlChartContainer = new System.Windows.Forms.Panel();
            this.lblCurrentBudget = new System.Windows.Forms.Label();
            this.btnUpdateBudget = new System.Windows.Forms.Button();
            this.txtBudgetInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tpChart = new System.Windows.Forms.TabPage();
            this.pnlChartCanvas = new System.Windows.Forms.Panel();
            this.tpDiagnosis = new System.Windows.Forms.TabPage();
            this.tabMain.SuspendLayout();
            this.tpSummary.SuspendLayout();
            this.pnlAlert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.panel1.SuspendLayout();
            this.pnlActionButtons.SuspendLayout();
            this.pnlDashboard.SuspendLayout();
            this.tpBudget.SuspendLayout();
            this.pnlChartContainer.SuspendLayout();
            this.tpChart.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tpSummary);
            this.tabMain.Controls.Add(this.tpBudget);
            this.tabMain.Controls.Add(this.tpChart);
            this.tabMain.Controls.Add(this.tpDiagnosis);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Margin = new System.Windows.Forms.Padding(7);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(2884, 1753);
            this.tabMain.TabIndex = 0;
            // 
            // tpSummary
            // 
            this.tpSummary.Controls.Add(this.pnlAlert);
            this.tpSummary.Controls.Add(this.dgvHistory);
            this.tpSummary.Controls.Add(this.panel1);
            this.tpSummary.Controls.Add(this.pnlActionButtons);
            this.tpSummary.Controls.Add(this.pnlDashboard);
            this.tpSummary.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tpSummary.Location = new System.Drawing.Point(10, 61);
            this.tpSummary.Margin = new System.Windows.Forms.Padding(7);
            this.tpSummary.Name = "tpSummary";
            this.tpSummary.Padding = new System.Windows.Forms.Padding(7);
            this.tpSummary.Size = new System.Drawing.Size(2864, 1682);
            this.tpSummary.TabIndex = 0;
            this.tpSummary.Text = "主頁總覽";
            this.tpSummary.UseVisualStyleBackColor = true;
            // 
            // pnlAlert
            // 
            this.pnlAlert.BackColor = System.Drawing.Color.MistyRose;
            this.pnlAlert.Controls.Add(this.btnAlertConfirm);
            this.pnlAlert.Controls.Add(this.lblAlertMsg);
            this.pnlAlert.Location = new System.Drawing.Point(973, 425);
            this.pnlAlert.Margin = new System.Windows.Forms.Padding(7);
            this.pnlAlert.Name = "pnlAlert";
            this.pnlAlert.Size = new System.Drawing.Size(607, 644);
            this.pnlAlert.TabIndex = 3;
            this.pnlAlert.Visible = false;
            // 
            // btnAlertConfirm
            // 
            this.btnAlertConfirm.Location = new System.Drawing.Point(161, 475);
            this.btnAlertConfirm.Margin = new System.Windows.Forms.Padding(7);
            this.btnAlertConfirm.Name = "btnAlertConfirm";
            this.btnAlertConfirm.Size = new System.Drawing.Size(245, 83);
            this.btnAlertConfirm.TabIndex = 1;
            this.btnAlertConfirm.Text = "我知道了";
            this.btnAlertConfirm.UseVisualStyleBackColor = true;
            // 
            // lblAlertMsg
            // 
            this.lblAlertMsg.AutoSize = true;
            this.lblAlertMsg.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAlertMsg.Location = new System.Drawing.Point(184, 79);
            this.lblAlertMsg.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblAlertMsg.Name = "lblAlertMsg";
            this.lblAlertMsg.Size = new System.Drawing.Size(187, 68);
            this.lblAlertMsg.TabIndex = 0;
            this.lblAlertMsg.Text = "label2";
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.BackgroundColor = System.Drawing.SystemColors.Menu;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.Location = new System.Drawing.Point(7, 497);
            this.dgvHistory.Margin = new System.Windows.Forms.Padding(7);
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersWidth = 92;
            this.dgvHistory.RowTemplate.Height = 24;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(2850, 1178);
            this.dgvHistory.TabIndex = 2;
            this.dgvHistory.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistory_CellDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbFilterCategory);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(7, 367);
            this.panel1.Margin = new System.Windows.Forms.Padding(7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2850, 130);
            this.panel1.TabIndex = 5;
            // 
            // cmbFilterCategory
            // 
            this.cmbFilterCategory.FormattingEnabled = true;
            this.cmbFilterCategory.Items.AddRange(new object[] {
            "全部",
            "餐飲",
            "交通",
            "娛樂",
            "薪資",
            "其他"});
            this.cmbFilterCategory.Location = new System.Drawing.Point(7, 14);
            this.cmbFilterCategory.Margin = new System.Windows.Forms.Padding(7);
            this.cmbFilterCategory.Name = "cmbFilterCategory";
            this.cmbFilterCategory.Size = new System.Drawing.Size(524, 45);
            this.cmbFilterCategory.TabIndex = 4;
            this.cmbFilterCategory.SelectedIndexChanged += new System.EventHandler(this.cmbFilterCategory_SelectedIndexChanged);
            // 
            // pnlActionButtons
            // 
            this.pnlActionButtons.Controls.Add(this.btnClearAll);
            this.pnlActionButtons.Controls.Add(this.btnSave);
            this.pnlActionButtons.Controls.Add(this.btnDelete);
            this.pnlActionButtons.Controls.Add(this.btnAdd);
            this.pnlActionButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActionButtons.Location = new System.Drawing.Point(7, 232);
            this.pnlActionButtons.Margin = new System.Windows.Forms.Padding(7);
            this.pnlActionButtons.Name = "pnlActionButtons";
            this.pnlActionButtons.Size = new System.Drawing.Size(2850, 135);
            this.pnlActionButtons.TabIndex = 1;
            // 
            // btnClearAll
            // 
            this.btnClearAll.BackColor = System.Drawing.Color.Thistle;
            this.btnClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearAll.Location = new System.Drawing.Point(890, 14);
            this.btnClearAll.Margin = new System.Windows.Forms.Padding(7);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(282, 90);
            this.btnClearAll.TabIndex = 3;
            this.btnClearAll.Text = "⚠️ 清除所有資料";
            this.btnClearAll.UseVisualStyleBackColor = false;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.PaleTurquoise;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(1994, 14);
            this.btnSave.Margin = new System.Windows.Forms.Padding(7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(257, 90);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "💾 儲存檔案";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.LightPink;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(619, 14);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(7);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(257, 90);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "❌ 刪除選取";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.LightGreen;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(12, 14);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(7);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(257, 90);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "➕ 新增帳目";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // pnlDashboard
            // 
            this.pnlDashboard.Controls.Add(this.lblMinExpense);
            this.pnlDashboard.Controls.Add(this.lblMaxExpense);
            this.pnlDashboard.Controls.Add(this.lblBalance);
            this.pnlDashboard.Controls.Add(this.lblTotalExpense);
            this.pnlDashboard.Controls.Add(this.lblTotalIncome);
            this.pnlDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDashboard.Location = new System.Drawing.Point(7, 7);
            this.pnlDashboard.Margin = new System.Windows.Forms.Padding(7);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(2850, 225);
            this.pnlDashboard.TabIndex = 0;
            // 
            // lblMinExpense
            // 
            this.lblMinExpense.AutoSize = true;
            this.lblMinExpense.BackColor = System.Drawing.Color.Transparent;
            this.lblMinExpense.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblMinExpense.Location = new System.Drawing.Point(1479, 130);
            this.lblMinExpense.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblMinExpense.Name = "lblMinExpense";
            this.lblMinExpense.Size = new System.Drawing.Size(117, 43);
            this.lblMinExpense.TabIndex = 4;
            this.lblMinExpense.Text = "label2";
            // 
            // lblMaxExpense
            // 
            this.lblMaxExpense.AutoSize = true;
            this.lblMaxExpense.BackColor = System.Drawing.Color.Transparent;
            this.lblMaxExpense.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblMaxExpense.Location = new System.Drawing.Point(1479, 50);
            this.lblMaxExpense.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblMaxExpense.Name = "lblMaxExpense";
            this.lblMaxExpense.Size = new System.Drawing.Size(117, 43);
            this.lblMaxExpense.TabIndex = 3;
            this.lblMaxExpense.Text = "label2";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lblBalance.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBalance.Location = new System.Drawing.Point(838, 86);
            this.lblBalance.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(224, 46);
            this.lblBalance.TabIndex = 2;
            this.lblBalance.Text = "目前結餘: $0";
            // 
            // lblTotalExpense
            // 
            this.lblTotalExpense.AutoSize = true;
            this.lblTotalExpense.BackColor = System.Drawing.Color.LightPink;
            this.lblTotalExpense.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalExpense.Location = new System.Drawing.Point(439, 86);
            this.lblTotalExpense.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblTotalExpense.Name = "lblTotalExpense";
            this.lblTotalExpense.Size = new System.Drawing.Size(188, 46);
            this.lblTotalExpense.TabIndex = 1;
            this.lblTotalExpense.Text = "總支出: $0";
            // 
            // lblTotalIncome
            // 
            this.lblTotalIncome.AutoSize = true;
            this.lblTotalIncome.BackColor = System.Drawing.Color.LightGreen;
            this.lblTotalIncome.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTotalIncome.Location = new System.Drawing.Point(40, 86);
            this.lblTotalIncome.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblTotalIncome.Name = "lblTotalIncome";
            this.lblTotalIncome.Size = new System.Drawing.Size(188, 46);
            this.lblTotalIncome.TabIndex = 0;
            this.lblTotalIncome.Text = "總收入: $0";
            // 
            // tpBudget
            // 
            this.tpBudget.Controls.Add(this.pnlChartContainer);
            this.tpBudget.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tpBudget.Location = new System.Drawing.Point(10, 61);
            this.tpBudget.Margin = new System.Windows.Forms.Padding(7);
            this.tpBudget.Name = "tpBudget";
            this.tpBudget.Padding = new System.Windows.Forms.Padding(7);
            this.tpBudget.Size = new System.Drawing.Size(2864, 1682);
            this.tpBudget.TabIndex = 1;
            this.tpBudget.Text = "預算設定";
            this.tpBudget.UseVisualStyleBackColor = true;
            // 
            // pnlChartContainer
            // 
            this.pnlChartContainer.Controls.Add(this.lblCurrentBudget);
            this.pnlChartContainer.Controls.Add(this.btnUpdateBudget);
            this.pnlChartContainer.Controls.Add(this.txtBudgetInput);
            this.pnlChartContainer.Controls.Add(this.label1);
            this.pnlChartContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChartContainer.Location = new System.Drawing.Point(7, 7);
            this.pnlChartContainer.Margin = new System.Windows.Forms.Padding(7);
            this.pnlChartContainer.Name = "pnlChartContainer";
            this.pnlChartContainer.Size = new System.Drawing.Size(2850, 1668);
            this.pnlChartContainer.TabIndex = 0;
            // 
            // lblCurrentBudget
            // 
            this.lblCurrentBudget.AutoSize = true;
            this.lblCurrentBudget.Location = new System.Drawing.Point(65, 380);
            this.lblCurrentBudget.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblCurrentBudget.Name = "lblCurrentBudget";
            this.lblCurrentBudget.Size = new System.Drawing.Size(343, 38);
            this.lblCurrentBudget.TabIndex = 3;
            this.lblCurrentBudget.Text = "目前設定預算：$10,000";
            // 
            // btnUpdateBudget
            // 
            this.btnUpdateBudget.BackColor = System.Drawing.Color.LightBlue;
            this.btnUpdateBudget.Location = new System.Drawing.Point(54, 205);
            this.btnUpdateBudget.Margin = new System.Windows.Forms.Padding(7);
            this.btnUpdateBudget.Name = "btnUpdateBudget";
            this.btnUpdateBudget.Size = new System.Drawing.Size(427, 97);
            this.btnUpdateBudget.TabIndex = 2;
            this.btnUpdateBudget.Text = "更新並套用預算";
            this.btnUpdateBudget.UseVisualStyleBackColor = false;
            this.btnUpdateBudget.Click += new System.EventHandler(this.btnUpdateBudget_Click);
            // 
            // txtBudgetInput
            // 
            this.txtBudgetInput.Location = new System.Drawing.Point(429, 36);
            this.txtBudgetInput.Margin = new System.Windows.Forms.Padding(7);
            this.txtBudgetInput.Name = "txtBudgetInput";
            this.txtBudgetInput.Size = new System.Drawing.Size(235, 46);
            this.txtBudgetInput.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(44, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 43);
            this.label1.TabIndex = 0;
            this.label1.Text = "請設定每月預算上限：";
            // 
            // tpChart
            // 
            this.tpChart.Controls.Add(this.pnlChartCanvas);
            this.tpChart.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tpChart.Location = new System.Drawing.Point(10, 61);
            this.tpChart.Margin = new System.Windows.Forms.Padding(7);
            this.tpChart.Name = "tpChart";
            this.tpChart.Size = new System.Drawing.Size(2864, 1682);
            this.tpChart.TabIndex = 2;
            this.tpChart.Text = "統計圖表";
            this.tpChart.UseVisualStyleBackColor = true;
            // 
            // pnlChartCanvas
            // 
            this.pnlChartCanvas.BackColor = System.Drawing.Color.White;
            this.pnlChartCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChartCanvas.Location = new System.Drawing.Point(0, 0);
            this.pnlChartCanvas.Margin = new System.Windows.Forms.Padding(7);
            this.pnlChartCanvas.Name = "pnlChartCanvas";
            this.pnlChartCanvas.Size = new System.Drawing.Size(2864, 1682);
            this.pnlChartCanvas.TabIndex = 0;
            // 
            // tpDiagnosis
            // 
            this.tpDiagnosis.Location = new System.Drawing.Point(10, 61);
            this.tpDiagnosis.Name = "tpDiagnosis";
            this.tpDiagnosis.Size = new System.Drawing.Size(2864, 1682);
            this.tpDiagnosis.TabIndex = 3;
            this.tpDiagnosis.Text = "🏥 財務診斷";
            this.tpDiagnosis.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2884, 1753);
            this.Controls.Add(this.tabMain);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMain";
            this.Text = "記帳APP";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabMain.ResumeLayout(false);
            this.tpSummary.ResumeLayout(false);
            this.pnlAlert.ResumeLayout(false);
            this.pnlAlert.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.panel1.ResumeLayout(false);
            this.pnlActionButtons.ResumeLayout(false);
            this.pnlDashboard.ResumeLayout(false);
            this.pnlDashboard.PerformLayout();
            this.tpBudget.ResumeLayout(false);
            this.pnlChartContainer.ResumeLayout(false);
            this.pnlChartContainer.PerformLayout();
            this.tpChart.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tpSummary;
        private System.Windows.Forms.TabPage tpBudget;
        private System.Windows.Forms.Panel pnlDashboard;
        private System.Windows.Forms.Label lblTotalExpense;
        private System.Windows.Forms.Label lblTotalIncome;
        private System.Windows.Forms.TabPage tpChart;
        private System.Windows.Forms.Panel pnlActionButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel pnlChartContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCurrentBudget;
        private System.Windows.Forms.Button btnUpdateBudget;
        private System.Windows.Forms.TextBox txtBudgetInput;
        private System.Windows.Forms.Panel pnlChartCanvas;
        private System.Windows.Forms.Panel pnlAlert;
        private System.Windows.Forms.Button btnAlertConfirm;
        private System.Windows.Forms.Label lblAlertMsg;
        private System.Windows.Forms.ComboBox cmbFilterCategory;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Label lblMinExpense;
        private System.Windows.Forms.Label lblMaxExpense;
        private System.Windows.Forms.TabPage tpDiagnosis;
    }
}

