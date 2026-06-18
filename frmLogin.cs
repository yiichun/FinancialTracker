using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FinancialTracker
{
    public partial class frmLogin : Form
    {
        private TextBox txtUsername;
        private Button btnLogin;
        private Label lblHint;

        public string LoggedInUser { get; private set; }

        public frmLogin()
        {
            this.Width = 380;
            this.Height = 230;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "🔐 智慧理財系統 - 安全登入";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            Label lblTitle = new Label { Text = "歡迎使用個人財務管家", Left = 30, Top = 25, Font = new Font("微軟正黑體", 12, FontStyle.Bold), AutoSize = true };
            Label lblUser = new Label { Text = "請輸入使用者名稱 (或學號)：", Left = 30, Top = 65, Font = new Font("微軟正黑體", 10), AutoSize = true };

            txtUsername = new TextBox { Left = 30, Top = 90, Width = 300, Font = new Font("微軟正黑體", 11) };
            txtUsername.Text = "s1131520";

            lblHint = new Label { Left = 30, Top = 125, ForeColor = Color.Red, Font = new Font("微軟正黑體", 9), AutoSize = true };

            btnLogin = new Button { Text = "登入系統", Left = 240, Top = 135, Width = 90, Height = 35, BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat, Font = new Font("微軟正黑體", 10, FontStyle.Bold) };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblHint);
            this.Controls.Add(btnLogin);

            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string input = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                lblHint.Text = "❌ 請輸入名稱，不可為空白！";
                return;
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (input.IndexOfAny(invalidChars) >= 0)
            {
                lblHint.Text = "❌ 名稱包含非法字元，請重新輸入！";
                return;
            }

            string expectedFileName = $"records_{input}.json";
            string expectedPath = Path.Combine(Application.StartupPath, expectedFileName);

            if (File.Exists(expectedPath))
            {
                LoggedInUser = input;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                var result = MessageBox.Show(
                    $"【系統提示】\n找不到使用者「{input}」的歷史記帳紀錄。\n\n如果您是第一次使用，請點擊【是(Yes)】建立新帳號。\n如果您是老用戶，可能是名字打錯了，請點擊【否(No)】重新輸入。",
                    "🔍 建立新帳號確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    LoggedInUser = input;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    txtUsername.Focus();
                    txtUsername.SelectAll();
                    lblHint.Text = "💡 請更正您的使用者名稱。";
                }
            }
        }
    }
}