using System;
using System.Windows.Forms;

namespace FinancialTracker
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 🌟 2026 升級：使用迴圈達到「登入 -> 關閉主畫面 -> 重新登入」的無縫切換機制
            while (true)
            {
                frmLogin loginForm = new frmLogin();

                // 顯示登入視窗
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    string username = loginForm.LoggedInUser;

                    // 建立主表單
                    frmMain mainForm = new frmMain(username);

                    // 執行主表單（這裡會卡住直到主表單被關閉）
                    Application.Run(mainForm);

                    // 🌟 當主畫面關閉後，檢查它是被點選了「切換帳號」，還是直接按[X]關閉
                    // 我們透過自訂的 Tag 屬性來傳遞這個訊號
                    if (mainForm.Tag?.ToString() == "SwitchUser")
                    {
                        // 如果是切換帳號，迴圈繼續 (跳回上方重新執行 frmLogin)
                        continue;
                    }
                    else
                    {
                        // 如果是正常關閉程式，跳出迴圈，徹底結束
                        break;
                    }
                }
                else
                {
                    // 使用者在登入畫面直接取消或按 X，結束程式
                    break;
                }
            }

            Application.Exit();
        }
    }
}