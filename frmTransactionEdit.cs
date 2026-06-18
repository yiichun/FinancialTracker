using System;
using System.Linq;
using System.Windows.Forms;

namespace FinancialTracker
{
    public partial class frmTransactionEdit : Form
    {
        public Transaction NewTransaction { get; private set; }

        public frmTransactionEdit()
        {
            InitializeComponent();

            LoadCategories();

            cmbCategory.DropDownStyle =
                ComboBoxStyle.DropDown;

            cmbCategory.SelectedIndexChanged +=
                cmbCategory_SelectedIndexChanged;
        }

        private void frmTransactionEdit_Load(object sender, EventArgs e)
        {
            if (cmbType.Items.Count > 0) cmbType.SelectedIndex = 0;
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;

            this.AcceptButton = this.btnConfirm;
            this.CancelButton = this.btnCancel;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("請輸入正確的正整數金額！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NewTransaction = new Transaction
            {
                Date = dtpDate.Value,
                Type = cmbType.SelectedItem?.ToString() ?? "支出",
                Category = cmbCategory.SelectedItem?.ToString() ?? "其他",
                Amount = amount,
                Memo = txtMemo.Text
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void LoadTransaction(Transaction tx)
        {
            dtpDate.Value = tx.Date;

            cmbType.SelectedItem = tx.Type;

            if (!cmbCategory.Items.Contains(tx.Category))
            {
                cmbCategory.Items.Add(tx.Category);
            }

            cmbCategory.SelectedItem = tx.Category;

            txtAmount.Text = tx.Amount.ToString();

            txtMemo.Text = tx.Memo;
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();

            cmbCategory.Items.Add("餐飲");
            cmbCategory.Items.Add("交通");
            cmbCategory.Items.Add("娛樂");
            cmbCategory.Items.Add("購物");
            cmbCategory.Items.Add("醫療");
            cmbCategory.Items.Add("薪資");
            cmbCategory.Items.Add("投資");
            cmbCategory.Items.Add("其他");

            if (System.IO.File.Exists("CustomCategories.txt"))
            {
                foreach (string category in
                         System.IO.File.ReadAllLines("CustomCategories.txt"))
                {
                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        if (!cmbCategory.Items.Contains(category))
                        {
                            cmbCategory.Items.Add(category);
                        }
                    }
                }
            }

            cmbCategory.Items.Add("[➕ 新增自訂類別...]");
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
                System.IO.File.ReadAllLines("CustomCategories.txt");

            if (!categories.Contains(category))
            {
                System.IO.File.AppendAllText(
                    "CustomCategories.txt",
                    category + Environment.NewLine);
            }
        }

        private void cmbCategory_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            if (cmbCategory.SelectedItem?.ToString()
                == "[➕ 新增自訂類別...]")
            {
                string newCategory =
                    Microsoft.VisualBasic.Interaction.InputBox(
                        "請輸入新的類別名稱",
                        "新增類別",
                        "");

                if (!string.IsNullOrWhiteSpace(newCategory))
                {
                    SaveCustomCategory(newCategory);

                    LoadCategories();

                    cmbCategory.SelectedItem = newCategory;
                }
            }
        }
    }
}