using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialTracker
{
    public class Transaction
    {
        public DateTime Date { get; set; }     // 記帳日期
        public string Type { get; set; }        // "收入" 或 "支出"
        public string Category { get; set; }    // "餐飲", "交通", "娛樂", "薪資" 等
        public decimal Amount { get; set; }     // 金額
        public string Memo { get; set; }        // 備註說明

        public string GroupId { get; set; }
    }
}
