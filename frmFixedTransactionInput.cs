using System;
using System.Linq;
using System.Windows.Forms;

namespace FinancialTracker
{
    public partial class frmFixedTransactionInput : Form
    {
        // 供外部主畫面讀取的屬性
        public string TransactionType { get; private set; } 
        public string TransactionMemo { get; private set; }
        public int FrequencyDays { get; private set; }
        public decimal Amount { get; private set; }
        public string Category { get; private set; }
        public DateTime StartDate { get; private set; }

        // 元件宣告
        private ComboBox cmbTypeInput; 
        private TextBox txtMemoInput;
        private TextBox txtAmountInput;
        private ComboBox cmbCategoryInput;
        private DateTimePicker dtpStart;
        private NumericUpDown numDays;
        private Label lblPreview;
        private Button btnOk;
        private Button btnCancel;

        public frmFixedTransactionInput()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }


        public void LoadForEdit(Transaction existingItem)
        {
            this.Text = "📅 修改固定記帳群組";
            cmbTypeInput.SelectedItem = existingItem.Type;
            txtMemoInput.Text = existingItem.Memo.Replace("【固定", "").Split('】').Last(); // 清理掉期數標籤
            txtAmountInput.Text = existingItem.Amount.ToString();
            cmbCategoryInput.SelectedItem = existingItem.Category;
            dtpStart.Value = existingItem.Date;

            dtpStart.Enabled = false;

            UpdatePeriodPreview();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "📅 設定常態固定記帳 (收入/支出)";
            this.Size = new System.Drawing.Size(380, 420); 
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 1. 記帳類型 (🌟 新增：固定收入/固定支出切換)
            Label lblType = new Label { Text = "記帳帳目類型：", Left = 20, Top = 20, AutoSize = true };
            cmbTypeInput = new ComboBox { Left = 150, Top = 18, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTypeInput.Items.AddRange(new string[] { "支出", "收入" });
            cmbTypeInput.SelectedIndex = 0; // 預設為支出

            // 2. 款項名稱
            Label lblMemo = new Label { Text = "款項名稱 (備忘)：", Left = 20, Top = 60, AutoSize = true };
            txtMemoInput = new TextBox { Left = 150, Top = 58, Width = 180, Text = "定期定額固定項目" };

            // 3. 金額
            Label lblAmount = new Label { Text = "每次入帳/扣款金額：", Left = 20, Top = 100, AutoSize = true };
            txtAmountInput = new TextBox { Left = 150, Top = 98, Width = 180, Text = "1000" };

            // 4. 類別
            Label lblCategory = new Label { Text = "款項分類：", Left = 20, Top = 140, AutoSize = true };
            cmbCategoryInput = new ComboBox { Left = 150, Top = 138, Width = 180, DropDownStyle = ComboBoxStyle.DropDown };
            LoadCategories();
            cmbCategoryInput.SelectedIndex = 4;

            cmbTypeInput.SelectedIndexChanged += (s, e) => {
                if (cmbTypeInput.SelectedItem.ToString() == "收入")
                {
                    cmbCategoryInput.SelectedIndex = 6; // 「薪資」
                    if (txtMemoInput.Text == "定期定額固定項目") txtMemoInput.Text = "每月固定薪資入帳";
                }
                else
                {
                    cmbCategoryInput.SelectedIndex = 4; // 「其他」
                    if (txtMemoInput.Text == "每月固定薪資入帳") txtMemoInput.Text = "定期定額固定項目";
                }
                UpdatePeriodPreview();
            };

            // 5. 開始時間
            Label lblStartDate = new Label { Text = "首次執行日期：", Left = 20, Top = 180, AutoSize = true };
            dtpStart = new DateTimePicker { Left = 150, Top = 178, Width = 180, Format = DateTimePickerFormat.Short };
            dtpStart.Value = DateTime.Now;

            // 6. 週期天數
            Label lblDays = new Label { Text = "自動執行週期 (天)：", Left = 20, Top = 220, AutoSize = true };
            numDays = new NumericUpDown { Left = 150, Top = 218, Width = 180, Minimum = 1, Maximum = 365, Value = 30 };

            // 7. 未來三週期預覽看板
            GroupBox gbPreview = new GroupBox { Text = "📋 未來三個週期明細預覽", Left = 20, Top = 255, Width = 320, Height = 85 };
            lblPreview = new Label { Left = 15, Top = 22, Width = 290, Height = 55, ForeColor = System.Drawing.Color.DarkSlateGray, Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular) };
            gbPreview.Controls.Add(lblPreview);

            // 綁定連動事件
            dtpStart.ValueChanged += (s, e) => UpdatePeriodPreview();
            numDays.ValueChanged += (s, e) => UpdatePeriodPreview();
            txtAmountInput.TextChanged += (s, e) => UpdatePeriodPreview();

            // 8. 確定與取消按鈕
            btnOk = new Button { Text = "確定", Left = 160, Top = 350, Width = 85, Height = 30 };
            btnCancel = new Button { Text = "取消", Left = 255, Top = 350, Width = 85, Height = 30 };

            btnOk.Click += new EventHandler(btnOk_Click);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            // 載入所有控制項
            this.Controls.Clear();
            this.Controls.AddRange(new Control[] { lblType, cmbTypeInput, lblMemo, txtMemoInput, lblAmount, txtAmountInput, lblCategory, cmbCategoryInput, lblStartDate, dtpStart, lblDays, numDays, gbPreview, btnOk, btnCancel });

            // 初始更新一次預覽
            UpdatePeriodPreview();
        }

        private void UpdatePeriodPreview()
        {
            DateTime start = dtpStart.Value;
            int days = (int)numDays.Value;
            string actionText = cmbTypeInput.SelectedItem?.ToString() == "收入" ? "入帳" : "扣款";

            decimal.TryParse(txtAmountInput.Text, out decimal amt);

            DateTime p1 = start;
            DateTime p2 = start.AddDays(days);
            DateTime p3 = start.AddDays(days * 2);

            lblPreview.Text = $"第 1 週期：{p1:yyyy / MM / dd}  將自動{actionText} ${amt:N0}\n" +
                              $"第 2 週期：{p2:yyyy / MM / dd}  將自動{actionText} ${amt:N0}\n" +
                              $"第 3 週期：{p3:yyyy / MM / dd}  將自動{actionText} ${amt:N0}";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemoInput.Text))
            {
                MessageBox.Show("請輸入款項名稱！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtAmountInput.Text, out decimal amt) || amt <= 0)
            {
                MessageBox.Show("請輸入正確的正整數金額！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedCategory = cmbCategoryInput.Text;
            SaveCustomCategory(selectedCategory);
            if (string.IsNullOrWhiteSpace(selectedCategory))
            {
                selectedCategory = "其他";
            }

            this.TransactionType = cmbTypeInput.SelectedItem?.ToString() ?? "支出";
            this.TransactionMemo = txtMemoInput.Text;
            this.Amount = amt;
            this.Category = selectedCategory; 
            this.FrequencyDays = (int)numDays.Value;
            this.StartDate = dtpStart.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveCustomCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return;

            if (!System.IO.File.Exists("CustomCategories.txt"))
            {
                System.IO.File.WriteAllText(
                    "CustomCategories.txt",
                    category + Environment.NewLine);

                return;
            }

            var categories =
                System.IO.File.ReadAllLines(
                    "CustomCategories.txt");

            if (!categories.Contains(category))
            {
                System.IO.File.AppendAllText(
                    "CustomCategories.txt",
                    category + Environment.NewLine);
            }
        }
        private void LoadCategories()
        {
            cmbCategoryInput.Items.Clear();

            cmbCategoryInput.Items.Add("餐飲");
            cmbCategoryInput.Items.Add("交通");
            cmbCategoryInput.Items.Add("娛樂");
            cmbCategoryInput.Items.Add("購物");
            cmbCategoryInput.Items.Add("醫療");
            cmbCategoryInput.Items.Add("薪資");
            cmbCategoryInput.Items.Add("投資");
            cmbCategoryInput.Items.Add("其他");

            if (System.IO.File.Exists("CustomCategories.txt"))
            {
                foreach (string category in
                         System.IO.File.ReadAllLines(
                         "CustomCategories.txt"))
                {
                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        if (!cmbCategoryInput.Items.Contains(category))
                        {
                            cmbCategoryInput.Items.Add(category);
                        }
                    }
                }
            }
        }
    }
}