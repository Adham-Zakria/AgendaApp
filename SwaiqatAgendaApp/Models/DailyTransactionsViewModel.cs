using DataAccessLayer.Models;

namespace SwaiqatAgendaApp.Models
{
    public class DailyTransactionsViewModel
    {
        public string BranchName { get; set; }
        public DateTime Date { get; set; }
        public decimal OpeningBalance { get; set; }

        public List<Transaction> Revenues { get; set; } = new();
        public List<Transaction> Expenses { get; set; } = new();

        public decimal TotalRevenues => (decimal) Revenues.Sum(t => t.Amount);
        public decimal TotalExpenses => (decimal) Expenses.Sum(t => t.Amount);
        public decimal NetBalance => OpeningBalance + TotalRevenues - TotalExpenses;

        public List<CashierStatusViewModel> Cashiers { get; set; } = new();
    }
}
