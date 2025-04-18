namespace Tea.Domain.Common
{
    public class DailyRevenueInMonthResponse
    {
        public int DayOfMonth { get; set; }      
        public DateTime DateValue { get; set; }
        public required string DayName { get; set; }      
        public decimal DailyRevenue { get; set; } 
        public int OrderCount { get; set; }
    }
}
