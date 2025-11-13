namespace SwaiqatAgendaApp.Models
{
    public class CashierStatusViewModel
    {
        public string CashierName { get; set; }
        public string CashierCode { get; set; }
        public decimal? ShortageOrSurplus { get; set; } // موجب = زيادة / سالب = عجز
    }
}
