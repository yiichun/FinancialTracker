using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace FinancialTracker
{
    public partial class frmMain : Form
    {
        // 儲存所有帳目明細的清單
        private List<Transaction> transactions = new List<Transaction>();

        // 檔案儲存路徑
        private string filePath = Path.Combine(Application.StartupPath, "records.json");

        // 每月預算變數
        private decimal monthlyBudget = 10000;
        private bool isBudgetAlertDismissed = false;

        // === 預算頁面動態升級元件 ===
        private ProgressBar pbBudgetProgress;
        private Label lblBudgetTips;

        // === 圖表頁面進階升級元件 ===
        private ComboBox cmbChartType;      // 圖表類型切換 (圓餅圖 / 長條圖)
        private ComboBox cmbChartMonth;     // 月份歷史篩選器
        private string selectedChartMonth = "全部"; // 當前選取的圖表月份
        private ComboBox cmbStartMonth; // 折線圖專用：開始月份
        private ComboBox cmbEndMonth;   // 折線圖專用：結束月份

        // === 匯率自訂功能元件 (完全動態獨立建立) ===
        private TabPage tpExchangeRate;           // 匯率設定分頁
        private DataGridView dgvExchangeRates;     // 匯率編輯表格
        private Button btnSaveExchangeRates;      // 匯率儲存按鈕

        // 🌟 共享的匯率記憶體表格：讓「記帳區」和「匯率區」可以互通
        private DataTable dtExchangeRates;

        // 初始匯率對照表 (以台幣為基準 1)
        private Dictionary<string, decimal> initialRates = new Dictionary<string, decimal>
        {
            { "TWD (新台幣)", 1.0m },
            { "USD (美金)", 32.5m },
            { "JPY (日幣)", 0.21m },
            { "KRW (韓元)", 0.024m },
            { "EUR (歐元)", 35.2m }
        };

        // 系統內建類別清單
        private List<string> categoryList = new List<string> { "餐飲", "交通", "娛樂", "其他" };

        // 排序狀態追蹤 (true = 正序, false = 倒序)
        private bool isDateAscending = false;
        private bool isAmountAscending = true;

        // === 動態 UI 控制項 ===
        private Panel pnlInputArea;
        private DateTimePicker dtpDateInput;
        private ComboBox cmbTypeInput;
        private ComboBox cmbCategoryInput;
        private TextBox txtAmountInput;
        private TextBox txtMemoInput;
        private Button btnSaveInput;
        private Button btnCancelInput;
        private Label lblInputErrorHint;
        private Button btnEdit;

        private TextBox txtSearchMemo;            // 關鍵字搜尋
        private ContextMenuStrip dgvContextMenu;  // 右鍵選單

        // 日期區間篩選控制項
        private DateTimePicker dtpFilterStart;
        private DateTimePicker dtpFilterEnd;
        private CheckBox chkEnableDateFilter;
        private Label lblScoreBadge;              // 財務健康度評分看板

        private int currentEditAction = 0; // 0=無, 1=新增, 2=修改
        private int selectedTransactionIndex = -1;

        // 1. 在類別頂端，多宣告一個全域變數來紀錄目前是誰登入
        private string currentUser = "Guest";

        // 2. 將原本的建構子改成這樣：
        public frmMain(string username)
        {
            InitializeComponent();
            // 讓使用者可以按住 Ctrl 或 Shift 鍵來選取多列
            dgvHistory.MultiSelect = true;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            currentUser = username;

            filePath = Path.Combine(Application.StartupPath, $"records_{currentUser}.json");

            Button btnFixedTransaction = new Button();

            btnFixedTransaction.Size = this.btnAdd.Size;
            btnFixedTransaction.Font = this.btnAdd.Font;
            btnFixedTransaction.BackColor = this.btnAdd.BackColor;
            btnFixedTransaction.ForeColor = this.btnAdd.ForeColor;
            btnFixedTransaction.FlatStyle = this.btnAdd.FlatStyle;
            btnFixedTransaction.FlatAppearance.BorderSize = this.btnAdd.FlatAppearance.BorderSize;

            // 設定文字
            btnFixedTransaction.Text = "📅 固定記帳";

            // 🌟 2. 讓按鈕在同一條上自動並列排下去
            // 找出 pnlActionButtons 面板裡面目前最後一個按鈕的位置，自動往右邊貼齊並列
            int highestLeft = 0;
            foreach (Control ctrl in this.pnlActionButtons.Controls)
            {
                if (ctrl is Button && ctrl.Right > highestLeft)
                {
                    highestLeft = ctrl.Right;
                }
            }
            btnFixedTransaction.Location =new Point(btnSave.Right + 10,btnAdd.Top);

            btnFixedTransaction.Visible = true;

            btnFixedTransaction.BringToFront();
            btnFixedTransaction.Click += new EventHandler(this.btnFixedTransaction_Click);

            this.pnlActionButtons.Controls.Add(btnFixedTransaction);

            pnlChartCanvas.Paint += pnlChartCanvas_Paint;
            InitializeEmbeddedInputPanel();

            Button btnSwitchUser = new Button();
            btnSwitchUser.Text = "🔄 切換帳號";
            btnSwitchUser.Size = new Size(110, 32);

            btnSwitchUser.Top = 40;
            btnSwitchUser.Left = this.Width - 140;
            btnSwitchUser.Anchor = AnchorStyles.Top | AnchorStyles.Right; 

            btnSwitchUser.BackColor = Color.FromArgb(240, 240, 240);
            btnSwitchUser.FlatStyle = FlatStyle.Flat;
            btnSwitchUser.Font = new Font("微軟正黑體", 9, FontStyle.Bold);
            btnSwitchUser.Cursor = Cursors.Hand;

            btnSwitchUser.Click += (s, ev) =>
            {
                var confirmResult = MessageBox.Show(
                    "您確定要登出目前帳號並切換使用者嗎？\n(未儲存的變更將會自動儲存)",
                    "提示",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmResult == DialogResult.Yes)
                {
                    this.Tag = "SwitchUser";
                    this.Close();
                }
            };

            this.Controls.Add(btnSwitchUser);
            btnSwitchUser.BringToFront(); 

            InitializeBudgetAdvancedUI();
            UpdateBudgetDisplay();  
            InitializeChartAdvancedUI();
            RefreshChartMonthItems();
            UpdateChartStats();
            InitializeExchangeRateTab();
            InitializeCurrencyUI();
        }

        private void InitializeChartAdvancedUI()
        {
            if (tpChart == null)
            {
                MessageBox.Show("系統提示：找不到 tpChart 分頁控制項，請檢查 UI 介面名稱！", "提示");
                return;
            }

            tpChart.AutoScroll = true;

            if (cmbChartType == null) cmbChartType = new ComboBox();
            if (!tpChart.Controls.Contains(cmbChartType)) tpChart.Controls.Add(cmbChartType);

            cmbChartType.Location = new Point(25, 20);
            cmbChartType.Size = new Size(180, 25);
            cmbChartType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChartType.Font = new Font("微軟正黑體", 10, FontStyle.Regular);
            cmbChartType.Visible = true; 

            if (cmbChartType.Items.Count == 0)
            {
                cmbChartType.Items.Add("分類支出比例 (圓餅圖)");
                cmbChartType.Items.Add("當月收支總覽 (長條圖)");
                cmbChartType.Items.Add("歷史收支趨勢 (折線圖)");
                cmbChartType.SelectedIndex = 0;
            }

            if (cmbChartMonth == null) cmbChartMonth = new ComboBox();
            if (!tpChart.Controls.Contains(cmbChartMonth)) tpChart.Controls.Add(cmbChartMonth);
            cmbChartMonth.Location = new Point(220, 20);
            cmbChartMonth.Size = new Size(120, 25);
            cmbChartMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChartMonth.Font = new Font("微軟正黑體", 10, FontStyle.Regular);
            cmbChartMonth.Visible = true;

            if (cmbStartMonth == null) cmbStartMonth = new ComboBox();
            if (!tpChart.Controls.Contains(cmbStartMonth)) tpChart.Controls.Add(cmbStartMonth);
            cmbStartMonth.Location = new Point(220, 20);
            cmbStartMonth.Size = new Size(100, 25);
            cmbStartMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStartMonth.Font = new Font("微軟正黑體", 10, FontStyle.Regular);
            cmbStartMonth.Visible = false; 

            if (cmbEndMonth == null) cmbEndMonth = new ComboBox();
            if (!tpChart.Controls.Contains(cmbEndMonth)) tpChart.Controls.Add(cmbEndMonth);
            cmbEndMonth.Location = new Point(330, 20);
            cmbEndMonth.Size = new Size(100, 25);
            cmbEndMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEndMonth.Font = new Font("微軟正黑體", 10, FontStyle.Regular);
            cmbEndMonth.Visible = false;

            if (pnlChartCanvas != null)
            {
                pnlChartCanvas.Dock = DockStyle.None;
                pnlChartCanvas.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                pnlChartCanvas.Location = new Point(25, 90);
                pnlChartCanvas.Size = new Size(530, 260);
                pnlChartCanvas.Visible = true;

                pnlChartCanvas.Paint -= pnlChartCanvas_Paint;
                pnlChartCanvas.Paint += pnlChartCanvas_Paint;
            }

            cmbChartType.SelectedIndexChanged -= CmbChartType_SelectedIndexChanged;
            cmbChartType.SelectedIndexChanged += CmbChartType_SelectedIndexChanged;

            cmbChartMonth.SelectedIndexChanged -= CmbChartMonth_SelectedIndexChanged;
            cmbChartMonth.SelectedIndexChanged += CmbChartMonth_SelectedIndexChanged;

            cmbStartMonth.SelectedIndexChanged -= RangeMonth_Changed;
            cmbStartMonth.SelectedIndexChanged += RangeMonth_Changed;

            cmbEndMonth.SelectedIndexChanged -= RangeMonth_Changed;
            cmbEndMonth.SelectedIndexChanged += RangeMonth_Changed;

            cmbChartType.BringToFront();
            cmbChartMonth.BringToFront();
            cmbStartMonth.BringToFront();
            cmbEndMonth.BringToFront();
        }

        private ComboBox cmbCurrency; 

        private void InitializeCurrencyUI()
        {
            if (btnAdd == null) return;

            cmbCurrency = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("微軟正黑體", 10),
                Size = new Size(125, 25)
            };

            foreach (DataRow row in dtExchangeRates.Rows)
            {
                cmbCurrency.Items.Add(row[0].ToString());
            }
            cmbCurrency.SelectedIndex = 0;

            if (btnAdd.Parent != null)
            {
                btnAdd.Parent.Controls.Add(cmbCurrency);
                cmbCurrency.Location = new Point(btnAdd.Left + btnAdd.Width + 10, btnAdd.Top);
                cmbCurrency.BringToFront();
            }
        }
        private void InitializeExchangeRateTab()
        {
            TabControl mainTabControl = null;
            if (tpChart != null && tpChart.Parent is TabControl)
            {
                mainTabControl = (TabControl)tpChart.Parent;
            }
            else
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is TabControl) { mainTabControl = (TabControl)ctrl; break; }
                }
            }

            if (mainTabControl == null) return;

            tpExchangeRate = new TabPage("💱 匯率設定");
            tpExchangeRate.BackColor = Color.White;

            Label lblTips = new Label
            {
                Text = "💡 提示：請在下方表格直接點擊修改匯率（以新台幣為基準 1），修改後請記得點擊右下角「儲存匯率配置」。",
                Font = new Font("微軟正黑體", 10, FontStyle.Regular),
                ForeColor = Color.DarkSlateGray,
                Location = new Point(25, 20),
                Size = new Size(530, 25)
            };
            tpExchangeRate.Controls.Add(lblTips);

            dgvExchangeRates = new DataGridView
            {
                Location = new Point(25, 55),
                Size = new Size(530, 250),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,

                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("微軟正黑體", 10)
            };

            dgvExchangeRates.CellValidating += DgvExchangeRates_CellValidating;

            tpExchangeRate.Controls.Add(dgvExchangeRates);

            btnSaveExchangeRates = new Button
            {
                Text = "💾 儲存匯率配置",
                Font = new Font("微軟正黑體", 10, FontStyle.Bold),
                Location = new Point(405, 320),
                Size = new Size(150, 35),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveExchangeRates.Click += BtnSaveExchangeRates_Click;
            tpExchangeRate.Controls.Add(btnSaveExchangeRates);

            dtExchangeRates = new DataTable();

            DataColumn colCurrency = new DataColumn("幣別 (Currency)", typeof(string));
            colCurrency.ReadOnly = true;

            DataColumn colRate = new DataColumn("匯率 (對新台幣)", typeof(decimal));

            dtExchangeRates.Columns.Add(colCurrency);
            dtExchangeRates.Columns.Add(colRate);

            foreach (var kvp in initialRates)
            {
                dtExchangeRates.Rows.Add(kvp.Key, kvp.Value);
            }
            dgvExchangeRates.DataSource = dtExchangeRates;

            mainTabControl.TabPages.Add(tpExchangeRate);
        }

        private void DgvExchangeRates_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string input = e.FormattedValue.ToString().Trim();

                if (!decimal.TryParse(input, out decimal rate) || rate <= 0)
                {
                    MessageBox.Show("請輸入有效的匯率數值（必須是大於 0 的正數，且不可包含字母或特殊符號）！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; 
                }
            }
        }

        private void BtnSaveExchangeRates_Click(object sender, EventArgs e)
        {
            dgvExchangeRates.EndEdit(); 
            foreach (DataRow row in dtExchangeRates.Rows)
            {
                string currency = row[0].ToString();
                if (!decimal.TryParse(row[1].ToString(), out decimal rate) || rate <= 0)
                {
                    MessageBox.Show($"請輸入正確的匯率數值（必須大於 0）！錯誤幣別：{currency}", "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (currency.Contains("TWD") && rate != 1.0m)
                {
                    MessageBox.Show("新台幣（TWD）為基準貨幣，匯率必須固定為 1.0！", "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row[1] = 1.0m;
                    return;
                }
            }

            MessageBox.Show("🎉 最新自訂匯率配置已儲存成功！", "系統成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshChartMonthItems()
        {
            if (cmbChartMonth == null) return;

            var months = transactions
                .Select(t => new { t.Date.Year, t.Date.Month })
                .Distinct()
                .OrderByDescending(d => d.Year).ThenByDescending(d => d.Month)
                .Select(d => $"{d.Year}年{d.Month:D2}月")
                .ToList();

            cmbChartMonth.Items.Clear();
            cmbChartMonth.Items.Add("全部");
            if (cmbStartMonth != null) cmbStartMonth.Items.Clear();
            if (cmbEndMonth != null) cmbEndMonth.Items.Clear();

            foreach (var m in months)
            {
                cmbChartMonth.Items.Add(m);
                cmbStartMonth?.Items.Add(m);
                cmbEndMonth?.Items.Add(m);
            }

            if (cmbChartMonth.Items.Count > 0) cmbChartMonth.SelectedIndex = 0;

            if (cmbStartMonth != null && cmbStartMonth.Items.Count > 0)
                cmbStartMonth.SelectedIndex = cmbStartMonth.Items.Count - 1;
            if (cmbEndMonth != null && cmbEndMonth.Items.Count > 0)
                cmbEndMonth.SelectedIndex = 0;
        }

        private void CmbChartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentChartType = cmbChartType?.SelectedItem?.ToString() ?? "";
            bool isLineChart = currentChartType.Contains("折線圖");

            if (cmbChartMonth != null) cmbChartMonth.Visible = !isLineChart;
            if (cmbStartMonth != null) cmbStartMonth.Visible = isLineChart;
            if (cmbEndMonth != null) cmbEndMonth.Visible = isLineChart;

            if (pnlChartCanvas != null)
            {
                pnlChartCanvas.Invalidate(); 
            }
        }

        private void RangeMonth_Changed(object sender, EventArgs e)
        {
            if (pnlChartCanvas != null) pnlChartCanvas.Invalidate();
        }

        private void CmbChartMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null && cb.SelectedItem != null)
            {
                selectedChartMonth = cb.SelectedItem.ToString();
            }

            UpdateChartStats();

            if (pnlChartCanvas != null)
            {
                pnlChartCanvas.Invalidate();
            }
        }
        
        /// <summary>
        /// 根據當前選取的月份，精準計算並更新「最高/最低消費類別」看板
        /// 確保固定記帳項目只要日期符合 selectedChartMonth，就會被納入統計
        /// </summary>
        private void UpdateChartStats()
        {
            var query = transactions.Where(t => t.Type == "支出");
            if (!string.IsNullOrEmpty(selectedChartMonth) && selectedChartMonth != "全部")
            {
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(selectedChartMonth, @"(\d+)年(\d+)月");
                if (match.Success)
                {
                    int filterYear = int.Parse(match.Groups[1].Value);
                    int filterMonth = int.Parse(match.Groups[2].Value);
                    query = query.Where(t => t.Date.Year == filterYear && t.Date.Month == filterMonth);
                }
            }

            var filteredExpenses = query.ToList();

            if (filteredExpenses.Count > 0)
            {
                // 找出金額最大的一筆
                var maxE = filteredExpenses.OrderByDescending(t => t.Amount).First();
                if (lblMaxExpense != null)
                    lblMaxExpense.Text = $"💸 單筆最高支出：{maxE.Category} ${maxE.Amount:N0} {maxE.Memo}";

                // 找出金額最小的一筆
                var minE = filteredExpenses.OrderBy(t => t.Amount).First();
                if (lblMinExpense != null)
                    lblMinExpense.Text = $"🌱 單筆最低支出：{minE.Category} ${minE.Amount:N0}";
            }
            else
            {
                // 如果該月份完全沒有支出紀錄的防呆處理
                if (lblMaxExpense != null)
                    lblMaxExpense.Text = "💸 單筆最高支出：該區間暫無資料";
                if (lblMinExpense != null)
                    lblMinExpense.Text = "🌱 單筆最低支出：該區間暫無資料";
            }
        }
        
        private void InitializeBudgetAdvancedUI()
        {
            TabPage budgetPage = this.tpBudget; 

            if (budgetPage == null)
            {
                Control[] foundPages = this.Controls.Find("tpBudgetSettings", true);
                if (foundPages.Length > 0) budgetPage = foundPages[0] as TabPage;
            }
            if (budgetPage == null)
            {
                Control[] foundPages = this.Controls.Find("tpBudget", true);
                if (foundPages.Length > 0) budgetPage = foundPages[0] as TabPage;
            }

            if (budgetPage == null) return;

            Panel pnlBudgetCard = new Panel();
            pnlBudgetCard.Name = "pnlBudgetCard";
            pnlBudgetCard.Size = new Size(520, 240);
            pnlBudgetCard.Left = lblCurrentBudget.Left;
            pnlBudgetCard.Top = lblCurrentBudget.Top + 80; 
            pnlBudgetCard.BackColor = Color.FromArgb(240, 244, 248); 
            pnlBudgetCard.BorderStyle = BorderStyle.FixedSingle;

            Label lblBarTitle = new Label();
            lblBarTitle.Text = "📊 本月總預算消耗進度：";
            lblBarTitle.Font = new Font("微軟正黑體", 10, FontStyle.Bold);
            lblBarTitle.Left = 20;
            lblBarTitle.Top = 15;
            lblBarTitle.AutoSize = true;

            pbBudgetProgress = new ProgressBar();
            pbBudgetProgress.Size = new Size(480, 25);
            pbBudgetProgress.Left = 20;
            pbBudgetProgress.Top = 45;
            pbBudgetProgress.Maximum = 100;
            pbBudgetProgress.Value = 0;
            pbBudgetProgress.Style = ProgressBarStyle.Blocks; 

            lblBudgetTips = new Label();
            lblBudgetTips.Text = "💡 系統分析中...";
            lblBudgetTips.Font = new Font("微軟正黑體", 10, FontStyle.Bold); 
            lblBudgetTips.Left = 20;
            lblBudgetTips.Top = 85;
            lblBudgetTips.Width = 480;
            lblBudgetTips.Height = 140;

            pnlBudgetCard.Controls.Add(lblBarTitle);
            pnlBudgetCard.Controls.Add(pbBudgetProgress);
            pnlBudgetCard.Controls.Add(lblBudgetTips);

            budgetPage.Controls.Add(pnlBudgetCard);
            pnlBudgetCard.BringToFront(); 
        }

        private void UpdateRelatedTransactions(string groupId, Transaction updatedTemplate)
        {
            var groupItems = transactions.Where(t => t.GroupId == groupId).OrderBy(t => t.Date).ToList();

            for (int i = 0; i < groupItems.Count; i++)
            {
                groupItems[i].Amount = updatedTemplate.Amount;
                groupItems[i].Category = updatedTemplate.Category;
                groupItems[i].Type = updatedTemplate.Type;
                groupItems[i].Memo = $"【固定{updatedTemplate.Type}第 {i + 1} 期】{updatedTemplate.Memo}";
            }

            this.btnSave_Click(this.btnSave, EventArgs.Empty);
            this.RefreshGrid();
        }

        private void btnFixedTransaction_Click(object sender, EventArgs e)
        {
            using (frmFixedTransactionInput inputForm = new frmFixedTransactionInput())
            {
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string newGroupId = Guid.NewGuid().ToString();
                    DateTime now = DateTime.Now;

                    for (int i = 0; i < 3; i++)
                    {
                        DateTime calculatedDate = inputForm.StartDate.AddDays(inputForm.FrequencyDays * i);

                        bool isCurrentMonth = (calculatedDate.Month == now.Month && calculatedDate.Year == now.Year);
                        string monthLabel = isCurrentMonth ? "[本月]" : "[跨月]";
                        string finalMemo = $"{monthLabel}【固定{inputForm.TransactionType}第 {i + 1} 期】{inputForm.TransactionMemo}";

                        Transaction periodItem = new Transaction
                        {
                            Date = calculatedDate,
                            Type = inputForm.TransactionType, 
                            Category = inputForm.Category,
                            Amount = inputForm.Amount,
                            Memo = finalMemo,
                            GroupId = newGroupId
                        };

                        this.transactions.Add(periodItem);
                    }

                    try
                    {
                        string json = JsonConvert.SerializeObject(transactions, Formatting.Indented);
                        File.WriteAllText(filePath, json);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"存檔失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.RefreshGrid();

                    MessageBox.Show($"📅 固定項目設定成功！\n本月項目已自動同步計入統計。",
                                    "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void UpdateBudgetDisplay()
        {
            DateTime now = DateTime.Now;
            decimal currentMonthExpense = transactions
                .Where(t => t.Type == "支出" && t.Date.Year == now.Year && t.Date.Month == now.Month)
                .Sum(t => t.Amount);

            if (pbBudgetProgress != null)
            {
                pbBudgetProgress.Maximum = (int)monthlyBudget;
                pbBudgetProgress.Value = (int)Math.Min(currentMonthExpense, monthlyBudget);
            }

            if (lblCurrentBudget != null)
            {
                lblCurrentBudget.Text = $"目前設定預算：${monthlyBudget:N0}";
            }

            if (lblBudgetTips != null)
            {
                lblBudgetTips.Text = $"📊 本月預算進度：${currentMonthExpense:N0} / ${monthlyBudget:N0}\n";

                if (currentMonthExpense > monthlyBudget)
                {
                    lblBudgetTips.Text += $"⚠️ 警告：本月已超支 ${(currentMonthExpense - monthlyBudget):N0}！請注意節制消費。";
                    lblBudgetTips.ForeColor = Color.Red;
                }
                else
                {
                    decimal remaining = monthlyBudget - currentMonthExpense;
                    double percent = (double)(currentMonthExpense / (monthlyBudget > 0 ? monthlyBudget : 1)) * 100;
                    lblBudgetTips.Text += $"💡 本月預算剩餘：${remaining:N0} (已使用 {percent:F1}%)，表現良好！";
                    lblBudgetTips.ForeColor = Color.DarkGreen;
                }
            }
            if (monthlyBudget > 0) 
            {
                if (currentMonthExpense > monthlyBudget)
                {
                    if (!isBudgetAlertDismissed)
                    {
                        MessageBox.Show(
                            $"⚠️ 警告！您本月的支出 (${currentMonthExpense:N0}) 已經超過設定的預算 (${monthlyBudget:N0})！\n\n請注意控制接下來的花費。",
                            "🚨 預算超支警示",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        isBudgetAlertDismissed = true; 
                    }
                }
                else
                {
                    isBudgetAlertDismissed = false;
                }
            }
        }

        private void btnUpdateBudget_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBudgetInput.Text))
            {
                MessageBox.Show("請輸入預算金額！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (decimal.TryParse(txtBudgetInput.Text, out decimal inputBudget) && inputBudget > 0)
            {
                monthlyBudget = inputBudget;
                isBudgetAlertDismissed = false; 

                UpdateBudgetDisplay();
                SaveData();
                CheckBudget();

                MessageBox.Show($"預算已成功更新為：${monthlyBudget:N0} 元！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBudgetInput.Clear();
            }
            else
            {
                MessageBox.Show("請輸入大於 0 的合法整數數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBudgetInput.SelectAll();
                txtBudgetInput.Focus();
            }
        }

        private void CheckBudget()
        {
            Control[] foundAlert = this.Controls.Find("pnlAlert", true);
            Panel pnlAlert = foundAlert.Length > 0 ? foundAlert[0] as Panel : null;
            Control[] foundMsg = this.Controls.Find("lblAlertMsg", true);
            Label lblAlertMsg = foundMsg.Length > 0 ? foundMsg[0] as Label : null;

            decimal totalExpense = transactions.Where(t => t.Type == "支出").Sum(t => t.Amount);

            if (totalExpense > monthlyBudget)
            {
                if (!isBudgetAlertDismissed)
                {
                    if (pnlAlert != null && lblAlertMsg != null)
                    {
                        lblAlertMsg.Text = $"🚨 嚴重超支警告！目前總支出 (${totalExpense:N0}) 已超出預算設定 (${monthlyBudget:N0})！";
                        pnlAlert.Visible = true;
                        pnlAlert.BringToFront();
                    }
                }
            }
            else
            {
                if (pnlAlert != null)
                {
                    pnlAlert.Visible = false;
                }
                isBudgetAlertDismissed = false;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = $"💰 智慧理財系統 - 當前使用者: {currentUser}";

            if (pnlAlert != null)
            {
                pnlAlert.Height = 35;
                pnlAlert.Width = dgvHistory.Width;
                pnlAlert.Location = new Point(dgvHistory.Left, dgvHistory.Top - 38);
                pnlAlert.BackColor = Color.MistyRose;
                pnlAlert.BorderStyle = BorderStyle.FixedSingle;

                if (lblAlertMsg != null)
                {
                    lblAlertMsg.AutoSize = true;
                    lblAlertMsg.Location = new Point(10, 8);
                    lblAlertMsg.Font = new Font("微軟正黑體", 10, FontStyle.Bold);
                    lblAlertMsg.ForeColor = Color.DarkRed;
                }

                if (btnAlertConfirm != null)
                {
                    btnAlertConfirm.Size = new Size(80, 24);
                    btnAlertConfirm.Location = new Point(pnlAlert.Width - 90, 5);
                    btnAlertConfirm.Font = new Font("微軟正黑體", 9, FontStyle.Regular);
                    btnAlertConfirm.Text = "我知道了";
                    btnAlertConfirm.Click -= btnAlertConfirm_Click;
                    btnAlertConfirm.Click += btnAlertConfirm_Click;
                }
            }

            LoadData(); 
            RefreshGrid();
            InitializeBudgetAdvancedUI();
            UpdateBudgetDisplay();
            InitializeChartAdvancedUI();
            RefreshChartMonthItems();
            UpdateChartStats();
            UpdateUI();

            txtAmountInput.KeyPress += (s, ev) => {
                if (!char.IsControl(ev.KeyChar) && !char.IsDigit(ev.KeyChar))
                {
                    ev.Handled = true;
                }
            };

            this.KeyPreview = true;
            this.KeyDown += FrmMain_KeyDown;

            dgvHistory.ColumnHeaderMouseClick += dgvHistory_ColumnHeaderMouseClick;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                Random rand = new Random();
                string[] testCategories = { "餐飲", "交通", "娛樂" };
                Transaction testTx = new Transaction
                {
                    Date = DateTime.Now.AddDays(-rand.Next(0, 30)),
                    Type = "支出",
                    Category = testCategories[rand.Next(testCategories.Length)],
                    Amount = rand.Next(100, 1500),
                    Memo = "🤖 Demo 快捷鍵測試數據"
                };
                transactions.Add(testTx);
                UpdateUI();
            }
        }

        #region 💾 檔案讀寫與歷史格式防呆相容轉換

        private void LoadData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath).Trim();

                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        if (jsonString.StartsWith("["))
                        {
                            transactions = JsonConvert.DeserializeObject<List<Transaction>>(jsonString) ?? new List<Transaction>();
                        }
                        else if (jsonString.StartsWith("{"))
                        {
                            var savedData = JsonConvert.DeserializeObject<SaveStructure>(jsonString);
                            if (savedData != null)
                            {
                                transactions = savedData.Transactions ?? new List<Transaction>();
                                if (savedData.Categories != null && savedData.Categories.Count > 0)
                                {
                                    categoryList = savedData.Categories;
                                }
                                if (savedData.MonthlyBudget > 0)
                                {
                                    monthlyBudget = savedData.MonthlyBudget;
                                }
                            }
                        }
                    }
                }
                SyncCategoryComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入檔案失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateBudgetDisplay();
        }

        private void SaveData()
        {
            try
            {
                var saveData = new SaveStructure
                {
                    Transactions = transactions,
                    Categories = categoryList,
                    MonthlyBudget = monthlyBudget
                };
                string jsonString = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"儲存檔案失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class SaveStructure
        {
            public List<Transaction> Transactions { get; set; }
            public List<string> Categories { get; set; }
            public decimal MonthlyBudget { get; set; }
        }

        #endregion

        #region 🔄 UI 更新與財務健康診斷

        private void UpdateUI()
        {
            decimal income = transactions.Where(t => t.Type == "收入").Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.Type == "支出").Sum(t => t.Amount);
            decimal balance = income - expense;

            lblTotalIncome.Text = $"總收入: ${income:N0}";
            lblTotalExpense.Text = $"總支出: ${expense:N0}";
            lblBalance.Text = $"目前結餘: ${balance:N0}";

            if (expense > monthlyBudget)
            {
                lblTotalExpense.ForeColor = Color.DarkRed;
                if (!isBudgetAlertDismissed && pnlAlert != null)
                {
                    lblAlertMsg.Text = $"⚠️ 預算警示：本月支出 (${expense:N0}) 已超過設定上限 (${monthlyBudget:N0})！";
                    pnlAlert.Visible = true;
                }
            }
            else
            {
                lblTotalExpense.ForeColor = Color.Red;
                if (pnlAlert != null) pnlAlert.Visible = false;
                isBudgetAlertDismissed = false;
            }

            var totalExpensesList = transactions.Where(t => t.Type == "支出").ToList();
            if (totalExpensesList.Count > 0)
            {
                var maxE = totalExpensesList.OrderByDescending(t => t.Amount).First();
                lblMaxExpense.Text = $"💸 單筆最高支出：{maxE.Category} ${maxE.Amount:N0} {maxE.Memo}";

                var minE = totalExpensesList.OrderBy(t => t.Amount).First();
                lblMinExpense.Text = $"🌱 單筆最低支出：{minE.Category} ${minE.Amount:N0}";
            }
            else
            {
                lblMaxExpense.Text = "💸 單筆最高支出：暫無資料";
                lblMinExpense.Text = "🌱 單筆最低支出：暫無資料";
            }

            if (lblScoreBadge != null)
            {
                if (income == 0 && expense == 0)
                {
                    lblScoreBadge.Text = "財務診斷：尚未開張 (請先開始記帳)";
                    lblScoreBadge.ForeColor = Color.Gray;
                }
                else
                {
                    decimal burnRate = monthlyBudget > 0 ? (expense / monthlyBudget) : 0;
                    decimal savingsRate = income > 0 ? (balance / income) : 0;

                    if (balance < 0) { lblScoreBadge.Text = "財務診斷：等級 E (入不敷出！赤字警告大危機)"; lblScoreBadge.ForeColor = Color.Red; }
                    else if (burnRate > 1.0m) { lblScoreBadge.Text = "財務診斷：等級 D (破產邊緣！超出預算控制)"; lblScoreBadge.ForeColor = Color.OrangeRed; }
                    else if (savingsRate >= 0.5m) { lblScoreBadge.Text = "財務診斷：等級 A+ (儲蓄神人！結餘超過收入 50%)"; lblScoreBadge.ForeColor = Color.Green; }
                    else if (savingsRate >= 0.3m) { lblScoreBadge.Text = "財務診斷：等級 A (穩健小資！財務非常健康)"; lblScoreBadge.ForeColor = Color.DarkGreen; }
                    else { lblScoreBadge.Text = "財務診斷：等級 B (月光潛在戶！建議減少娛樂開銷)"; lblScoreBadge.ForeColor = Color.Blue; }
                }
            }

            RefreshGrid();
            pnlChartCanvas.Invalidate();

            InitializeFinancialDiagnosisTab();
        }

        private void RefreshGrid()
        {
            string selectedCategory = cmbFilterCategory.SelectedItem?.ToString() ?? "全部";
            string keyword = (txtSearchMemo != null && txtSearchMemo.Text != "🔍 輸入關鍵字搜尋Memo...") ? txtSearchMemo.Text.Trim() : "";

            var query = transactions.AsQueryable();

            if (selectedCategory != "全部")
            {
                query = query.Where(t => t.Category == selectedCategory);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Memo != null && t.Memo.Contains(keyword));
            }
            if (chkEnableDateFilter != null && chkEnableDateFilter.Checked)
            {
                DateTime start = dtpFilterStart.Value.Date;
                DateTime end = dtpFilterEnd.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.Date >= start && t.Date <= end);
            }

            var filteredList = query.ToList();

            dgvHistory.DataSource = null;
            dgvHistory.DataSource = filteredList;

            foreach (DataGridViewRow row in dgvHistory.Rows)
            {
                if (row.Cells["Type"]?.Value == null) continue;
                string typeValue = row.Cells["Type"].Value.ToString();
                if (typeValue == "支出")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 235);
                }
                else if (typeValue == "收入")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 245, 230);
                }
            }

            DateTime now = DateTime.Now;

            decimal currentMonthIncome = transactions
                .Where(t => t.Type == "收入" && t.Date.Month == now.Month && t.Date.Year == now.Year)
                .Sum(t => t.Amount);

            decimal currentMonthExpense = transactions
                .Where(t => t.Type == "支出" && t.Date.Month == now.Month && t.Date.Year == now.Year)
                .Sum(t => t.Amount);

            if (lblTotalIncome != null) lblTotalIncome.Text = $"收入：${currentMonthIncome:N0}";
            if (lblTotalExpense != null) lblTotalExpense.Text = $"支出：${currentMonthExpense:N0}";
            if (lblBalance != null) lblBalance.Text = $"餘額：${(currentMonthIncome - currentMonthExpense):N0}";

            UpdateBudgetDisplay();
            RefreshChartMonthItems();
            UpdateChartStats();

            if (pnlChartCanvas != null)
            {
                pnlChartCanvas.Invalidate();
            }
        }

        private void dgvHistory_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string columnName = dgvHistory.Columns[e.ColumnIndex].Name;

            if (columnName == "Date")
            {
                isDateAscending = !isDateAscending;
                transactions = isDateAscending ?
                    transactions.OrderBy(t => t.Date).ToList() :
                    transactions.OrderByDescending(t => t.Date).ToList();
            }
            else if (columnName == "Amount")
            {
                isAmountAscending = !isAmountAscending;
                transactions = isAmountAscending ?
                    transactions.OrderBy(t => t.Amount).ToList() :
                    transactions.OrderByDescending(t => t.Amount).ToList();
            }
            else
            {
                return;
            }

            RefreshGrid();
        }

        #endregion

        #region ⚡ 控制項與自訂類別擴充點擊事件

        private void SyncCategoryComboBoxes()
        {
            if (cmbCategoryInput == null) return;

            if (File.Exists("CustomCategories.txt"))
            {
                foreach (string category in File.ReadAllLines("CustomCategories.txt"))
                {
                    if (!string.IsNullOrWhiteSpace(category) && !categoryList.Contains(category))
                    {
                        categoryList.Add(category);
                    }
                }
            }

            cmbCategoryInput.Items.Clear();
            cmbCategoryInput.Items.AddRange(categoryList.ToArray());
            cmbCategoryInput.Items.Add("[➕ 新增自訂類別...]");

            if (cmbFilterCategory != null)
            {
                cmbFilterCategory.DropDownStyle = ComboBoxStyle.DropDown;
                cmbFilterCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbFilterCategory.AutoCompleteSource = AutoCompleteSource.ListItems;

                string currentSelected = cmbFilterCategory.SelectedItem?.ToString() ?? "全部";
                cmbFilterCategory.Items.Clear();
                cmbFilterCategory.Items.Add("全部");
                cmbFilterCategory.Items.AddRange(categoryList.ToArray());

                if (cmbFilterCategory.Items.Contains(currentSelected))
                    cmbFilterCategory.SelectedItem = currentSelected;
                else
                    cmbFilterCategory.SelectedIndex = 0;
            }
        }

        private void CmbCategoryInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategoryInput.SelectedItem?.ToString() == "[➕ 新增自訂類別...]")
            {
                Form prompt = new Form()
                {
                    Width = 350,
                    Height = 160,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "新增自訂分類",
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                Label textLabel = new Label() { Left = 20, Top = 15, Text = "請輸入想要新增的自訂消費類別名稱：", AutoSize = true, Font = new Font("微軟正黑體", 10) };
                TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 290, Font = new Font("微軟正黑體", 10) };
                Button confirmation = new Button() { Text = "確定", Left = 110, Width = 90, Top = 85, DialogResult = DialogResult.OK, BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
                Button cancellation = new Button() { Text = "取消", Left = 220, Width = 90, Top = 85, DialogResult = DialogResult.Cancel, BackColor = Color.LightPink, FlatStyle = FlatStyle.Flat };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancellation);
                prompt.AcceptButton = confirmation;

                string newCategory = "";
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    newCategory = textBox.Text.Trim();
                }

                if (!string.IsNullOrEmpty(newCategory) && newCategory != "[➕ 新增自訂類別...]" && newCategory != "全部")
                {
                    if (!categoryList.Contains(newCategory))
                    {
                        categoryList.Add(newCategory);
                        SaveData();
                        SyncCategoryComboBoxes();
                        cmbCategoryInput.SelectedItem = newCategory;
                    }
                    else
                    {
                        MessageBox.Show("此消費分類已經存在系統中囉！", "提示");
                        cmbCategoryInput.SelectedIndex = 0;
                    }
                }
                else
                {
                    cmbCategoryInput.SelectedIndex = 0;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmTransactionEdit editForm = new frmTransactionEdit();

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                Transaction inputTx = editForm.NewTransaction;
                if (inputTx == null) return;

                decimal amt = inputTx.Amount;
                string memo = inputTx.Memo;

                decimal finalAmountInTWD = amt;
                string selectedCurrency = "TWD (新台幣)";

                if (cmbCurrency != null && cmbCurrency.SelectedItem != null)
                {
                    selectedCurrency = cmbCurrency.SelectedItem.ToString();

                    if (dtExchangeRates != null)
                    {
                        DataRow[] foundRows = dtExchangeRates.Select($"[幣別 (Currency)] = '{selectedCurrency}'");
                        if (foundRows.Length > 0 && dtExchangeRates.Columns.Contains("匯率 (對新台幣)"))
                        {
                            decimal currentRate = Convert.ToDecimal(foundRows[0]["匯率 (對新台幣)"]);
                            finalAmountInTWD = amt * currentRate;

                            if (currentRate != 1.0m)
                            {
                                memo = $"[{selectedCurrency} {amt}] " + memo;
                            }
                        }
                    }
                }

                Transaction newTx = new Transaction
                {
                    Date = inputTx.Date,
                    Type = inputTx.Type,
                    Category = inputTx.Category,
                    Amount = finalAmountInTWD,
                    Memo = memo
                };

                transactions.Add(newTx);
                MessageBox.Show($"成功記帳！套用目前自訂匯率換算為新台幣：${finalAmountInTWD:N0}", "提示");

                SyncCategoryComboBoxes();

                SaveData();
                RefreshGrid();
                UpdateChartStats();
                UpdateBudgetDisplay();

                if (cmbCurrency != null) cmbCurrency.SelectedIndex = 0;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvHistory.CurrentRow == null || dgvHistory.CurrentRow.Index < 0)
            {
                MessageBox.Show("請先在表格中選擇要修改的帳目項目！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedTx = (Transaction)dgvHistory.CurrentRow.DataBoundItem;
            selectedTransactionIndex = transactions.IndexOf(selectedTx);

            if (selectedTransactionIndex < 0 || selectedTransactionIndex >= transactions.Count) return;

            Transaction target = transactions[selectedTransactionIndex];
            currentEditAction = 2;
            lblInputErrorHint.Text = "";

            dtpDateInput.Value = target.Date;
            cmbTypeInput.SelectedItem = target.Type;

            if (!cmbCategoryInput.Items.Contains(target.Category))
            {
                categoryList.Add(target.Category);
                SyncCategoryComboBoxes();
            }
            cmbCategoryInput.SelectedItem = target.Category;

            txtAmountInput.Text = target.Amount.ToString("F0");
            txtMemoInput.Text = target.Memo;

            pnlInputArea.Visible = true;
            txtAmountInput.Focus();
            cmbCategoryInput.Enabled = (target.Type != "收入");
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count == 0) return;

            var confirm = MessageBox.Show($"確定要刪除選中的 {dgvHistory.SelectedRows.Count} 筆資料嗎？",
                                          "刪除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            List<Transaction> itemsToDelete = new List<Transaction>();
            foreach (DataGridViewRow row in dgvHistory.SelectedRows)
            {
                if (row.DataBoundItem is Transaction item)
                {
                    itemsToDelete.Add(item);
                }
            }

            foreach (var item in itemsToDelete)
            {
                if (!string.IsNullOrEmpty(item.GroupId))
                {
                    transactions.RemoveAll(t => t.GroupId == item.GroupId);
                }
                else
                {
                    transactions.Remove(item);
                }
            }

            if (this.btnSave != null) this.btnSave_Click(this.btnSave, EventArgs.Empty);
            this.RefreshGrid();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            MessageBox.Show("所有記帳資料已安全儲存至本機檔案！", "儲存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void btnAlertConfirm_Click(object sender, EventArgs e)
        {
            if (pnlAlert != null) pnlAlert.Visible = false;
            isBudgetAlertDismissed = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveData();
            base.OnFormClosing(e);
        }

        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            var firstCheck = MessageBox.Show("確定要【清除所有記帳資料】嗎？此動作無法復原！", "危險操作警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (firstCheck == DialogResult.Yes)
            {
                var secondCheck = MessageBox.Show("真的確定？所有儲存的 JSON 檔案紀錄都會被抹除喔！", "最後確認", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (secondCheck == DialogResult.Yes)
                {
                    transactions.Clear();
                    SaveData();
                    isBudgetAlertDismissed = false;
                    UpdateUI();
                    MessageBox.Show("所有資料已成功清空！", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void dgvHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= transactions.Count) return;

            Transaction selected = transactions[e.RowIndex];

            if (!string.IsNullOrEmpty(selected.GroupId))
            {
                using (frmFixedTransactionInput editForm = new frmFixedTransactionInput())
                {
                    editForm.LoadForEdit(selected);

                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        UpdateRelatedTransactions(selected.GroupId, new Transaction
                        {
                            Type = editForm.TransactionType,
                            Amount = editForm.Amount,
                            Memo = editForm.TransactionMemo,
                            Category = editForm.Category
                        });
                    }
                }
            }
            else
            {
                using (frmTransactionEdit editForm = new frmTransactionEdit())
                {
                    editForm.LoadTransaction(selected);

                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        transactions[e.RowIndex] = editForm.NewTransaction;
                        this.btnSave_Click(this.btnSave, EventArgs.Empty);
                        this.RefreshGrid();
                    }
                }
            }
        }

        private void BtnBackupSystem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("要建立資料備份檔嗎？\n[是]: 建立新備份\n[否]: 從舊備份檔還原資料", "💾 資料備份與還原系統", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            string backupPath = Path.Combine(Application.StartupPath, "records_backup.json");

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (transactions.Count == 0) { MessageBox.Show("目前沒資料可以備份！", "提示"); return; }
                    var saveData = new SaveStructure { Transactions = transactions, Categories = categoryList };
                    string jsonString = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                    File.WriteAllText(backupPath, jsonString);
                    MessageBox.Show("資料備份完畢！(儲存於 records_backup.json)", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"備份失敗: {ex.Message}"); }
            }
            else if (result == DialogResult.No)
            {
                try
                {
                    if (!File.Exists(backupPath)) { MessageBox.Show("找不到任何歷史備份檔案！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    string jsonString = File.ReadAllText(backupPath);
                    var savedData = JsonConvert.DeserializeObject<SaveStructure>(jsonString);
                    if (savedData != null)
                    {
                        transactions = savedData.Transactions ?? new List<Transaction>();
                        categoryList = savedData.Categories ?? categoryList;
                    }
                    SaveData();
                    SyncCategoryComboBoxes();
                    UpdateUI();
                    MessageBox.Show("已成功從備份檔還原所有資料與自訂類別！", "還原成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"還原失敗: {ex.Message}"); }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV 檔案 (*.csv)|*.csv"; 
            saveFileDialog.Title = "請選擇匯出位置";
            saveFileDialog.FileName = $"帳目紀錄_{DateTime.Now:yyyyMMdd}.csv"; 

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;

                    using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("日期,類別,收支類型,金額,備註");

                        foreach (var tx in transactions)
                        {
                            string line = string.Format("{0:yyyy-MM-dd},{1},{2},{3},\"{4}\"",
                                tx.Date, tx.Category, tx.Type, tx.Amount, tx.Memo.Replace("\"", "\"\""));
                            sw.WriteLine(line);
                        }
                    }

                    MessageBox.Show($"匯出成功！檔案已儲存至：\n{filePath}", "系統訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"匯出失敗，錯誤訊息：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region 🎨 GDI+ 動態分類圓餅圖繪製
        private void pnlChartCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvasG = e.Graphics;
            canvasG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            string currentChartType = cmbChartType?.SelectedItem?.ToString() ?? "分類支出比例 (圓餅圖)";

            bool filterByMonth = !string.IsNullOrEmpty(selectedChartMonth) && selectedChartMonth != "全部";
            int filterYear = 0;
            int filterMonth = 0;

            if (filterByMonth)
            {
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(selectedChartMonth, @"(\d+)年(\d+)月");
                if (match.Success)
                {
                    filterYear = int.Parse(match.Groups[1].Value);
                    filterMonth = int.Parse(match.Groups[2].Value);
                }
                else
                {
                    filterByMonth = false;
                }
            }

            // =========================================================================
            // 📊 類型一：分類支出比例 (圓餅圖)
            // =========================================================================
            if (currentChartType.Contains("圓餅圖"))
            {
                var query = transactions.Where(t => t.Type == "支出");

                if (filterByMonth)
                {
                    query = query.Where(t => t.Date.Year == filterYear && t.Date.Month == filterMonth);
                }

                var categoryTotals = query
                    .GroupBy(t => t.Category)
                    .Select(group => new { Category = group.Key, Total = group.Sum(t => t.Amount) })
                    .ToList();

                decimal totalExpense = categoryTotals.Sum(c => c.Total);

                if (totalExpense == 0)
                {
                    using (Font font = new Font("微軟正黑體", 13, FontStyle.Bold))
                        canvasG.DrawString($"📊 區間【{selectedChartMonth}】暫無支出數據", font, Brushes.Gray, 50, 130);
                    return;
                }

                Color[] colors = new Color[] { Color.LightCoral, Color.LightSkyBlue, Color.LightGreen, Color.Khaki, Color.LightSalmon, Color.MediumPurple };
                float startAngle = 0f;
                int colorIndex = 0;
                int legendX = 300;
                int legendY = 40;

                using (Font textFont = new Font("微軟正黑體", 10, FontStyle.Regular))
                {
                    foreach (var item in categoryTotals)
                    {
                        float sweepAngle = (float)(item.Total / totalExpense) * 360f;
                        using (Brush brush = new SolidBrush(colors[colorIndex % colors.Length]))
                        {
                            canvasG.FillPie(brush, 40, 40, 200, 200, startAngle, sweepAngle);
                        }
                        canvasG.DrawPie(Pens.White, 40, 40, 200, 200, startAngle, sweepAngle);

                        using (Brush brush = new SolidBrush(colors[colorIndex % colors.Length]))
                            canvasG.FillRectangle(brush, legendX, legendY, 15, 15);
                        canvasG.DrawRectangle(Pens.Gray, legendX, legendY, 15, 15);

                        double percentage = (double)(item.Total / totalExpense) * 100;
                        canvasG.DrawString($"{item.Category}: ${item.Total:N0} ({percentage:F1}%)", textFont, Brushes.Black, legendX + 25, legendY - 2);

                        startAngle += sweepAngle;
                        colorIndex++;
                        legendY += 25;
                    }

                    using (Font totalFont = new Font("微軟正黑體", 11, FontStyle.Bold))
                        canvasG.DrawString($"總計 ({selectedChartMonth}) 支出金額: ${totalExpense:N0}", totalFont, Brushes.DarkRed, 40, 260);
                }
            }

            // =========================================================================
            // ⚖️ 類型二：當月收支總覽 (長條圖)
            // =========================================================================
            else if (currentChartType.Contains("長條圖"))
            {
                var barQuery = transactions.AsQueryable();

                if (filterByMonth)
                {
                    barQuery = barQuery.Where(t => t.Date.Year == filterYear && t.Date.Month == filterMonth);
                }

                decimal totalIncome = barQuery.Where(t => t.Type == "收入").Sum(t => t.Amount);
                decimal totalExpense = barQuery.Where(t => t.Type == "支出").Sum(t => t.Amount);

                canvasG.DrawLine(Pens.Black, 60, 220, 360, 220); 
                canvasG.DrawLine(Pens.Black, 60, 40, 60, 220); 

                decimal maxAmount = Math.Max(totalIncome, totalExpense);
                if (maxAmount == 0) maxAmount = 1000;

                int incomeBarHeight = (int)((totalIncome / maxAmount) * 140);
                int expenseBarHeight = (int)((totalExpense / maxAmount) * 140);

                using (Font font = new Font("微軟正黑體", 10, FontStyle.Regular))
                using (Font boldFont = new Font("微軟正黑體", 11, FontStyle.Bold))
                {
                    canvasG.FillRectangle(Brushes.LightGreen, 100, 220 - incomeBarHeight, 50, incomeBarHeight);
                    canvasG.DrawRectangle(Pens.ForestGreen, 100, 220 - incomeBarHeight, 50, incomeBarHeight);
                    canvasG.DrawString("總收入", font, Brushes.Black, 105, 225);
                    canvasG.DrawString($"${totalIncome:N0}", boldFont, Brushes.DarkGreen, 95, 195 - incomeBarHeight);

                    canvasG.FillRectangle(Brushes.LightCoral, 220, 220 - expenseBarHeight, 50, expenseBarHeight);
                    canvasG.DrawRectangle(Pens.Firebrick, 220, 220 - expenseBarHeight, 50, expenseBarHeight);
                    canvasG.DrawString("總支出", font, Brushes.Black, 225, 225);
                    canvasG.DrawString($"${totalExpense:N0}", boldFont, Brushes.DarkRed, 215, 195 - expenseBarHeight);

                    decimal balance = totalIncome - totalExpense;
                    string balanceText = balance >= 0 ? $"🎉 區間盈餘：${balance:N0}" : $"🚨 區間透支：${Math.Abs(balance):N0}";
                    Brush balanceBrush = balance >= 0 ? Brushes.DarkGreen : Brushes.Red;
                    canvasG.DrawString($"{balanceText} ({selectedChartMonth})", boldFont, balanceBrush, 80, 260);
                }
            }

            // =========================================================================
            // 📈 類型三：歷史收支趨勢 (折線圖)
            // =========================================================================
            else if (currentChartType.Contains("折線圖"))
            {
                int startVal = 0, endVal = 999999;
                if (cmbStartMonth != null && cmbStartMonth.SelectedItem != null)
                {
                    var m1 = System.Text.RegularExpressions.Regex.Match(cmbStartMonth.SelectedItem.ToString(), @"(\d+)年(\d+)月");
                    if (m1.Success) startVal = int.Parse(m1.Groups[1].Value) * 100 + int.Parse(m1.Groups[2].Value);
                }
                if (cmbEndMonth != null && cmbEndMonth.SelectedItem != null)
                {
                    var m2 = System.Text.RegularExpressions.Regex.Match(cmbEndMonth.SelectedItem.ToString(), @"(\d+)年(\d+)月");
                    if (m2.Success) endVal = int.Parse(m2.Groups[1].Value) * 100 + int.Parse(m2.Groups[2].Value);
                }

                if (startVal > endVal) { int temp = startVal; startVal = endVal; endVal = temp; }

                var trendData = transactions
                    .Select(t => new { t.Date.Year, t.Date.Month, t.Type, t.Amount, SortVal = t.Date.Year * 100 + t.Date.Month })
                    .Where(t => t.SortVal >= startVal && t.SortVal <= endVal)
                    .GroupBy(t => new { t.Year, t.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new
                    {
                        Label = $"{g.Key.Year}/{g.Key.Month}",
                        Income = g.Where(x => x.Type == "收入").Sum(x => x.Amount),
                        Expense = g.Where(x => x.Type == "支出").Sum(x => x.Amount)
                    })
                    .ToList();

                if (trendData.Count == 0)
                {
                    canvasG.DrawString("📈 該區間內暫無歷史記帳數據", new Font("微軟正黑體", 13), Brushes.Gray, 50, 130);
                    return;
                }

                int margin = 65;
                int spacingX = 75;
                int requiredWidth = margin * 2 + (trendData.Count > 1 ? (trendData.Count - 1) * spacingX : 0) + 60;

                if (requiredWidth > 530)
                    pnlChartCanvas.Width = requiredWidth;
                else
                    pnlChartCanvas.Width = 530;

                int axisBottom = 230; 
                int axisTop = 40; 

                canvasG.DrawLine(Pens.LightGray, margin, axisBottom, pnlChartCanvas.Width - 20, axisBottom);
                canvasG.DrawLine(Pens.LightGray, margin, axisTop, margin, axisBottom);

                decimal maxVal = trendData.Max(d => Math.Max(d.Income, d.Expense));
                if (maxVal == 0) maxVal = 1000;

                List<Point> incomePoints = new List<Point>();
                List<Point> expensePoints = new List<Point>();

                using (Font font = new Font("微軟正黑體", 9))
                {
                    int maxHeight = 170;

                    for (int i = 0; i < trendData.Count; i++)
                    {
                        int x = margin + (i * spacingX);
                        int yIncome = axisBottom - (int)((trendData[i].Income / maxVal) * maxHeight);
                        int yExpense = axisBottom - (int)((trendData[i].Expense / maxVal) * maxHeight);

                        incomePoints.Add(new Point(x, yIncome));
                        expensePoints.Add(new Point(x, yExpense));

                        canvasG.DrawString(trendData[i].Label, font, Brushes.DimGray, x - 18, axisBottom + 8);
                    }

                    if (incomePoints.Count > 1) canvasG.DrawLines(new Pen(Color.FromArgb(46, 139, 87), 3), incomePoints.ToArray()); // 海洋綠
                    if (expensePoints.Count > 1) canvasG.DrawLines(new Pen(Color.FromArgb(220, 20, 60), 3), expensePoints.ToArray());   // 猩紅

                    for (int i = 0; i < trendData.Count; i++)
                    {
                        canvasG.FillEllipse(Brushes.DarkGreen, incomePoints[i].X - 4, incomePoints[i].Y - 4, 8, 8);
                        canvasG.DrawString($"${trendData[i].Income:0}", font, Brushes.Green, incomePoints[i].X - 15, incomePoints[i].Y - 18);

                        canvasG.FillEllipse(Brushes.Crimson, expensePoints[i].X - 4, expensePoints[i].Y - 4, 8, 8);
                        canvasG.DrawString($"${trendData[i].Expense:0}", font, Brushes.Firebrick, expensePoints[i].X - 15, expensePoints[i].Y + 6);
                    }

                    int legendX = margin + 20;
                    int legendY = 15;

                    canvasG.FillRectangle(new SolidBrush(Color.FromArgb(235, 255, 255, 255)), legendX - 5, legendY - 5, 95, 45);
                    canvasG.DrawRectangle(Pens.Gainsboro, legendX - 5, legendY - 5, 95, 45);

                    canvasG.FillRectangle(new SolidBrush(Color.FromArgb(46, 139, 87)), legendX, legendY + 6, 14, 8);
                    canvasG.DrawString("收入趨勢", font, Brushes.Black, legendX + 22, legendY + 3);

                    canvasG.FillRectangle(new SolidBrush(Color.FromArgb(220, 20, 60)), legendX, legendY + 24, 14, 8);
                    canvasG.DrawString("支出趨勢", font, Brushes.Black, legendX + 22, legendY + 21);
                }
            }
        }
        #endregion

        #region 🛠️ 動態建立內嵌面板與調整排版寬度位置

        private void InitializeEmbeddedInputPanel()
        {
            pnlInputArea = new Panel();
            pnlInputArea.Size = new Size(260, 360);
            pnlInputArea.Location = new Point(730, 140);
            pnlInputArea.BackColor = Color.FromArgb(245, 245, 245);
            pnlInputArea.BorderStyle = BorderStyle.FixedSingle;
            pnlInputArea.Visible = false;

            Label lblTitle = new Label { Text = "📝 帳目資料編輯", Font = new Font("微軟正黑體", 11, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
            pnlInputArea.Controls.Add(lblTitle);

            Label lblDate = new Label { Text = "日期：", Location = new Point(10, 45), AutoSize = true, Font = new Font("微軟正黑體", 9) };
            dtpDateInput = new DateTimePicker { Location = new Point(70, 40), Size = new Size(170, 25), Format = DateTimePickerFormat.Short };
            pnlInputArea.Controls.Add(lblDate);
            pnlInputArea.Controls.Add(dtpDateInput);

            Label lblType = new Label { Text = "類型：", Location = new Point(10, 85), AutoSize = true, Font = new Font("微軟正黑體", 9) };
            cmbTypeInput = new ComboBox { Location = new Point(70, 80), Size = new Size(170, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTypeInput.Items.AddRange(new string[] { "支出", "收入" });
            cmbTypeInput.SelectedIndexChanged += CmbTypeInput_SelectedIndexChanged;
            pnlInputArea.Controls.Add(lblType);
            pnlInputArea.Controls.Add(cmbTypeInput);

            Label lblCategory = new Label { Text = "分類：", Location = new Point(10, 125), AutoSize = true, Font = new Font("微軟正黑體", 9) };
            cmbCategoryInput = new ComboBox { Location = new Point(70, 120), Size = new Size(170, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategoryInput.SelectedIndexChanged += CmbCategoryInput_SelectedIndexChanged;
            pnlInputArea.Controls.Add(lblCategory);
            pnlInputArea.Controls.Add(cmbCategoryInput);

            Label lblAmount = new Label { Text = "金額：", Location = new Point(10, 165), AutoSize = true, Font = new Font("微軟正黑體", 9) };
            txtAmountInput = new TextBox { Location = new Point(70, 160), Size = new Size(170, 25) };
            pnlInputArea.Controls.Add(lblAmount);
            pnlInputArea.Controls.Add(txtAmountInput);

            Label lblMemo = new Label { Text = "備忘：", Location = new Point(10, 205), AutoSize = true, Font = new Font("微軟正黑體", 9) };
            txtMemoInput = new TextBox { Location = new Point(70, 200), Size = new Size(170, 50), Multiline = true };
            pnlInputArea.Controls.Add(lblMemo);
            pnlInputArea.Controls.Add(txtMemoInput);

            lblInputErrorHint = new Label { Location = new Point(10, 260), ForeColor = Color.Red, Font = new Font("微軟正黑體", 9, FontStyle.Bold), AutoSize = true, Text = "" };
            pnlInputArea.Controls.Add(lblInputErrorHint);

            btnSaveInput = new Button { Text = "💾 儲存", Location = new Point(30, 300), Size = new Size(90, 35), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            btnSaveInput.Click += BtnSaveInput_Click;
            pnlInputArea.Controls.Add(btnSaveInput);

            btnCancelInput = new Button { Text = "❌ 取消", Location = new Point(140, 300), Size = new Size(90, 35), BackColor = Color.LightPink, FlatStyle = FlatStyle.Flat };
            btnCancelInput.Click += BtnCancelInput_Click;
            pnlInputArea.Controls.Add(btnCancelInput);

            this.tpSummary.Controls.Add(pnlInputArea);
            pnlInputArea.BringToFront();

            btnEdit = new Button { Text = "✏️ 修改項目", Size = new Size(110, 40), Font = new Font("微軟正黑體", 10, FontStyle.Bold), BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat, Location = new Point(507, 6) };
            btnEdit.Click += btnEdit_Click;
            if (pnlActionButtons != null) pnlActionButtons.Controls.Add(btnEdit);

            Button btnExport = new Button { Text = "📊 匯出資料", Size = new Size(110, 40), Font = new Font("微軟正黑體", 10, FontStyle.Bold), BackColor = Color.LightGoldenrodYellow, FlatStyle = FlatStyle.Flat, Location = new Point(625, 6) };
            btnExport.Click += BtnExport_Click;
            if (pnlActionButtons != null) pnlActionButtons.Controls.Add(btnExport);

            Button btnBackup = new Button { Text = "📂 備份還原", Size = new Size(110, 40), Font = new Font("微軟正黑體", 10, FontStyle.Bold), BackColor = Color.LightCyan, FlatStyle = FlatStyle.Flat, Location = new Point(740, 6) };
            btnBackup.Click += BtnBackupSystem_Click;
            if (pnlActionButtons != null) pnlActionButtons.Controls.Add(btnBackup);

            txtSearchMemo = new TextBox { Size = new Size(170, 35), Location = new Point(cmbFilterCategory.Right + 10, cmbFilterCategory.Top), Font = new Font("微軟正黑體", 9), Text = "🔍 輸入關鍵字搜尋Memo...", ForeColor = Color.Gray };
            txtSearchMemo.Enter += (s, ev) => { if (txtSearchMemo.Text == "🔍 輸入關鍵字搜尋Memo...") { txtSearchMemo.Text = ""; txtSearchMemo.ForeColor = Color.Black; } };
            txtSearchMemo.Leave += (s, ev) => { if (string.IsNullOrWhiteSpace(txtSearchMemo.Text)) { txtSearchMemo.Text = "🔍 輸入關鍵字搜尋Memo..."; txtSearchMemo.ForeColor = Color.Gray; } };
            txtSearchMemo.TextChanged += (s, ev) => { RefreshGrid(); };
            if (cmbFilterCategory.Parent != null) cmbFilterCategory.Parent.Controls.Add(txtSearchMemo);

            chkEnableDateFilter = new CheckBox { Text = "篩選日期", Location = new Point(txtSearchMemo.Right + 45, txtSearchMemo.Top + 2), AutoSize = true, Font = new Font("微軟正黑體", 9, FontStyle.Bold) };
            chkEnableDateFilter.CheckedChanged += (s, ev) => { RefreshGrid(); };

            dtpFilterStart = new DateTimePicker { Format = DateTimePickerFormat.Short, Size = new Size(135, 25), Location = new Point(chkEnableDateFilter.Right + 1, txtSearchMemo.Top) };
            dtpFilterEnd = new DateTimePicker { Format = DateTimePickerFormat.Short, Size = new Size(135, 25), Location = new Point(dtpFilterStart.Right + 30, txtSearchMemo.Top) };

            Label lblTo = new Label { Text = "至", Location = new Point(dtpFilterStart.Right + 5, txtSearchMemo.Top + 4), AutoSize = true, Font = new Font("微軟正黑體", 9) };

            if (cmbFilterCategory.Parent != null)
            {
                cmbFilterCategory.Parent.Controls.Add(chkEnableDateFilter);
                cmbFilterCategory.Parent.Controls.Add(dtpFilterStart);
                cmbFilterCategory.Parent.Controls.Add(lblTo);
                cmbFilterCategory.Parent.Controls.Add(dtpFilterEnd);
            }

            dtpFilterStart.ValueChanged += (s, ev) => { if (chkEnableDateFilter.Checked) RefreshGrid(); };
            dtpFilterEnd.ValueChanged += (s, ev) => { if (chkEnableDateFilter.Checked) RefreshGrid(); };

            if (this.tabMain != null)
            {
                this.tabMain.SelectedIndexChanged += (s, ev) => {
                    if (tabMain.SelectedTab != null && tabMain.SelectedTab.Name == "tpDiagnosis")
                    {
                        UpdateDiagnosisReport();
                    }
                };
            }
        }

        private void BtnSaveInput_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAmountInput.Text))
            {
                lblInputErrorHint.Text = "❌ 錯誤：金額不可為空白！";
                return;
            }

            if (!decimal.TryParse(txtAmountInput.Text, out decimal amount) || amount <= 0)
            {
                lblInputErrorHint.Text = "❌ 錯誤：請輸入大於 0 的正確金額數字！";
                return;
            }

            lblInputErrorHint.Text = "";

            if (currentEditAction == 1)
            {
                Transaction newTx = new Transaction
                {
                    Date = dtpDateInput.Value,
                    Type = cmbTypeInput.SelectedItem?.ToString() ?? "支出",
                    Category = cmbCategoryInput.SelectedItem?.ToString() ?? "其他",
                    Amount = amount,
                    Memo = txtMemoInput.Text
                };
                transactions.Add(newTx);
            }
            else if (currentEditAction == 2)
            {
                if (selectedTransactionIndex >= 0 && selectedTransactionIndex < transactions.Count)
                {
                    transactions[selectedTransactionIndex].Date = dtpDateInput.Value;
                    transactions[selectedTransactionIndex].Type = cmbTypeInput.SelectedItem?.ToString() ?? "支出";
                    transactions[selectedTransactionIndex].Category = cmbCategoryInput.SelectedItem?.ToString() ?? "其他";
                    transactions[selectedTransactionIndex].Amount = amount;
                    transactions[selectedTransactionIndex].Memo = txtMemoInput.Text;
                }
            }

            pnlInputArea.Visible = false;
            currentEditAction = 0;
            SaveData();
            UpdateUI();
        }

        private void BtnCancelInput_Click(object sender, EventArgs e)
        {
            pnlInputArea.Visible = false;
            currentEditAction = 0;
        }

        private void CmbTypeInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTypeInput.SelectedItem?.ToString() == "收入")
            {
                cmbCategoryInput.SelectedItem = "其他";
                cmbCategoryInput.Enabled = false;
            }
            else
            {
                cmbCategoryInput.Enabled = true;
            }
        }

        #endregion

        #region 🏥 財務診斷核心報表產生系統

        private Label lblDiagStatus;        // 診斷總結標題
        private RichTextBox rtbDiagDetails; // 詳細診斷建議內容
        private Label lblDiagScore;         // 財務健康分數卡片
        private bool isDiagControlsInitialized = false;

        /// <summary>
        /// 初始化「財務診斷」分頁內部的動態控制項
        /// </summary>
        private void InitializeFinancialDiagnosisTab()
        {
            if (this.tabMain == null || this.tpDiagnosis == null) return;
            if (isDiagControlsInitialized) return; 

            tpDiagnosis.BackColor = Color.FromArgb(250, 250, 250);
            tpDiagnosis.Padding = new Padding(20);

            Label lblTitle = new Label();
            lblTitle.Text = "📊 個人本月財務健康診斷報告";
            lblTitle.Font = new Font("微軟正黑體", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            tpDiagnosis.Controls.Add(lblTitle);

            lblDiagScore = new Label();
            lblDiagScore.Size = new Size(180, 80);
            lblDiagScore.Location = new Point(550, 20);
            lblDiagScore.BorderStyle = BorderStyle.FixedSingle;
            lblDiagScore.BackColor = Color.LightGoldenrodYellow;
            lblDiagScore.Font = new Font("微軟正黑體", 16, FontStyle.Bold);
            lblDiagScore.TextAlign = ContentAlignment.MiddleCenter;
            tpDiagnosis.Controls.Add(lblDiagScore);

            lblDiagStatus = new Label();
            lblDiagStatus.Font = new Font("微軟正黑體", 12, FontStyle.Bold);
            lblDiagStatus.Location = new Point(25, 65);
            lblDiagStatus.AutoSize = true;
            tpDiagnosis.Controls.Add(lblDiagStatus);

            rtbDiagDetails = new RichTextBox();
            rtbDiagDetails.Size = new Size(710, 360);
            rtbDiagDetails.Location = new Point(20, 110);
            rtbDiagDetails.Font = new Font("微軟正黑體", 11, FontStyle.Regular);
            rtbDiagDetails.ReadOnly = true;
            rtbDiagDetails.BackColor = Color.White;
            rtbDiagDetails.BorderStyle = BorderStyle.FixedSingle;
            tpDiagnosis.Controls.Add(rtbDiagDetails);

            isDiagControlsInitialized = true;
        }

        private void UpdateDiagnosisReport()
        {
            if (transactions == null || rtbDiagDetails == null) return;

            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            decimal income = transactions.Where(t => t.Type == "收入" && t.Date.Year == currentYear && t.Date.Month == currentMonth).Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.Type == "支出" && t.Date.Year == currentYear && t.Date.Month == currentMonth).Sum(t => t.Amount);
            var expensesList = transactions.Where(t => t.Type == "支出" && t.Date.Year == currentYear && t.Date.Month == currentMonth).ToList();

            int score = 100;
            string statusText = "";
            Color statusColor = Color.Green;

            rtbDiagDetails.Clear();

            if (transactions.Count == 0)
            {
                lblDiagScore.Text = "評分：-- 分";
                lblDiagScore.BackColor = Color.LightGray;
                lblDiagStatus.Text = "🔍 系統提示：目前尚無記帳資料，請先至主頁新增帳目。";
                lblDiagStatus.ForeColor = Color.Gray;
                return;
            }

            rtbDiagDetails.Left = 30;
            rtbDiagDetails.Width = 500;
            rtbDiagDetails.Height = 600;

            string rightPanelName = "pnlDiagInstruction";
            Control[] foundControls = this.tpDiagnosis.Controls.Find(rightPanelName, true);
            RichTextBox rtbInstruction;

            if (foundControls.Length == 0)
            {
                rtbInstruction = new RichTextBox();
                rtbInstruction.Name = rightPanelName;
                rtbInstruction.Left = 550;
                rtbInstruction.Top = rtbDiagDetails.Top;
                rtbInstruction.Width = 360;
                rtbInstruction.Height = rtbDiagDetails.Height;
                rtbInstruction.ReadOnly = true;
                rtbInstruction.BackColor = Color.FromArgb(245, 248, 250);
                rtbInstruction.Font = new Font("微軟正黑體", 10);
                rtbInstruction.BorderStyle = BorderStyle.FixedSingle;

                System.Text.StringBuilder isb = new System.Text.StringBuilder();
                isb.AppendLine("📋 【 財務診斷系統 - 指標評估說明書 】");
                isb.AppendLine("========================================");
                isb.AppendLine("本系統採用三維度核心演算法，初始總分為 100 分：\n");
                isb.AppendLine("🎯 指標一：本月預算控管度 (權重最高)");
                isb.AppendLine("   ▪ 評估方式：檢查「總支出」是否超出「月預算」。");
                isb.AppendLine("   ▪ 扣分標準：");
                isb.AppendLine("     - 總支出 超過 預算：【扣 40 分】(嚴重赤字)");
                isb.AppendLine("     - 總支出 超過 預算的 80%：【扣 15 分】(黃燈警報)");
                isb.AppendLine("     - 控制在 80% 以內：【不扣分】(安全綠燈)\n");
                isb.AppendLine("💰 指標二：收支儲蓄結構率 (理財型態)");
                isb.AppendLine("   ▪ 評估方式：依據「50-30-20 理財法則」計算儲蓄率。");
                isb.AppendLine("   ▪ 扣分標準：");
                isb.AppendLine("     - 儲蓄率 < 0% (入不敷出)：【扣 20 分】");
                isb.AppendLine("     - 儲蓄率 < 20% (儲蓄過低)：【扣 10 分】");
                isb.AppendLine("     - 儲蓄率 ≥ 20%：【不扣分】(體質優良)\n");
                isb.AppendLine("💸 指標三：大額消費風險評估 (衝動防禦)");
                isb.AppendLine("   ▪ 評估方式：監測單筆支出是否過大，影響現金流。");
                isb.AppendLine("   ▪ 扣分標準：");
                isb.AppendLine("     - 單筆最高支出 超過 預算的 25%：【扣 15 分】");
                isb.AppendLine("     - 其餘平衡分配：【不扣分】(金流平穩)\n");
                isb.AppendLine("========================================");
                isb.AppendLine("👑 財務狀態判定標準：");
                isb.AppendLine("   🟢 卓越 (Excellent) ： 85 ~ 100 分");
                isb.AppendLine("   🟡 穩定 (Stable)    ： 70 ~ 84 分");
                isb.AppendLine("   🔴 危險 (High Risk) ： 70 分以下");

                rtbInstruction.Text = isb.ToString();
                this.tpDiagnosis.Controls.Add(rtbInstruction);
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("==========================================");
            sb.AppendLine($"  📅 診斷時間：{DateTime.Now:yyyy-MM-dd HH:mm:ss}   |   專屬識別碼：{currentUser}");
            sb.AppendLine("==========================================\n");

            sb.AppendLine("【 🎯 指標一：本月預算控管度 】");
            if (expense > monthlyBudget)
            {
                decimal overBudget = expense - monthlyBudget;
                score -= 40;
                sb.AppendLine($"❌ 嚴重警示：本月總支出 (${expense:N0}) 已超過設定預算 (${monthlyBudget:N0})！");
                sb.AppendLine($"👉 赤字高達 ${overBudget:N0}。請立即停止非必要性的消費！\n");
            }
            else if (expense > monthlyBudget * 0.8m)
            {
                score -= 15;
                sb.AppendLine($"⚠️ 黃燈警報：支出 (${expense:N0}) 已達預算的 80% 警戒線。");
                sb.AppendLine("👉 本月剩餘可用額度有限，建議開啟省錢模式，避免月底超支。\n");
            }
            else
            {
                sb.AppendLine($"🟢 安全：目前支出 (${expense:N0}) 控制在預算 (${monthlyBudget:N0}) 的安全範圍內。");
                sb.AppendLine("👉 表現優異！請繼續保持良好的消費自律。\n");
            }

            sb.AppendLine("【 💰 指標二：收支儲蓄結構率 】");
            if (income > 0)
            {
                decimal savingsRate = ((income - expense) / income) * 100;
                sb.AppendLine($"📊 目前您的個人本月儲蓄率為：{savingsRate:F1}%");

                if (savingsRate < 0)
                {
                    score -= 20;
                    sb.AppendLine("❌ 警示：您目前處於「入不敷出」的負儲蓄狀態！正在消耗過去的存款。");
                    sb.AppendLine("👉 理財點評：必須開源節流，首要任務是降低「支出佔比」。\n");
                }
                else if (savingsRate < 20)
                {
                    score -= 10;
                    sb.AppendLine("⚠️ 提示：儲蓄率低於 20% 黃金標準。");
                    sb.AppendLine("👉 理財點評：建議參考「50-30-20理財法」，每個月強迫自己先存下 20% 的收入。\n");
                }
                else
                {
                    sb.AppendLine("🟢 優良：儲蓄率高於 20%！符合健康的資產累積速度。");
                    sb.AppendLine("👉 理財點評：您的財務體質非常健康，存下來的金錢可以規劃部分進行穩健投資。\n");
                }
            }
            else
            {
                sb.AppendLine("⚪ 提示：本月尚無輸入「收入」資料，無法計算精準儲蓄率。");
                sb.AppendLine("👉 建議平時賺取的薪資或零用錢也要記帳，評估才更精準喔！\n");
            }

            sb.AppendLine("【 💸 指標三：大額消費風險評估 】");
            if (expensesList.Count > 0)
            {
                decimal maxExpenseAmount = expensesList.Max(t => t.Amount);
                var maxExpenseItem = expensesList.First(t => t.Amount == maxExpenseAmount);

                if (maxExpenseAmount > monthlyBudget * 0.25m)
                {
                    score -= 15;
                    sb.AppendLine($"⚠️ 發現大額怪獸：單筆最高支出為【{maxExpenseItem.Category}】金額 ${maxExpenseAmount:N0} (備忘：{maxExpenseItem.Memo})");
                    sb.AppendLine("👉 診斷：此筆消費佔了預算不小的比例。請反思該筆交易是「需要」還是「想要」？");
                    sb.AppendLine("👉 建議：未來超過千元的大型非緊急支出，強迫自己實施「24小時冷靜期」再購買。\n");
                }
                else
                {
                    sb.AppendLine("🟢 良好：本月沒有發現極端、不合理的大額單筆衝動消費。");
                    sb.AppendLine("👉 消費分配相當平均，金流掌控平穩。\n");
                }
            }
            else
            {
                sb.AppendLine("🟢 良好：尚無任何支出紀錄。\n");
            }

            if (score >= 85)
            {
                statusText = "👑 財務狀態：卓越 (Excellent)";
                statusColor = Color.DarkGreen;
                lblDiagScore.BackColor = Color.LightGreen;
            }
            else if (score >= 70)
            {
                statusText = "⚖️ 財務狀態：穩定 (Stable)";
                statusColor = Color.DarkOrange;
                lblDiagScore.BackColor = Color.Khaki;
            }
            else
            {
                statusText = "🚨 財務狀態：危險 (High Risk)";
                statusColor = Color.Red;
                lblDiagScore.BackColor = Color.LightPink;
            }

            lblDiagStatus.Text = statusText;
            lblDiagStatus.ForeColor = statusColor;
            lblDiagScore.Text = $"健康評分\n{score} 分";
            rtbDiagDetails.Text = sb.ToString();
        }
        #endregion
    }
}