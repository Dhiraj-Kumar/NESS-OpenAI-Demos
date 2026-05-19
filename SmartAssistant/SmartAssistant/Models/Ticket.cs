using System.ComponentModel;

namespace SmartAssistant.Models
{
    // Models
    public class SupportTicket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    public class ClassifiedTicket
    {
        [Description("Category: Hardware, Software, Network, Security, Account, Other")]
        public string Category { get; set; } = string.Empty;

        [Description("Priority: Low, Medium, High, Critical")]
        public string Priority { get; set; } = string.Empty;

        [Description("Estimated resolution time in hours")]
        public int EstimatedResolutionHours { get; set; }

        [Description("Short summary of the issue in one sentence")]
        public string Summary { get; set; } = string.Empty;

        [Description("Suggested team to assign: NetworkTeam, HardwareTeam, " +
                     "SoftwareTeam, SecurityTeam, AccountsTeam")]
        public string AssignTo { get; set; } = string.Empty;

        [Description("List of suggested first steps to resolve the issue")]
        public List<string> SuggestedSteps { get; set; } = new();
    }
}
