namespace SmartAssistant.Models
{
    public class WeeklyReportRequest
    {
        public int totalTickets { get; set; }
        public int resolved { get; set; }
        public int pending { get; set; }
        public int critical { get; set; }
        public List<string> topIssues { get; set; } = new();
        public int averageResolutionHours { get; set; }
    }
}
