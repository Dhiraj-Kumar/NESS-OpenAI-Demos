using System.ComponentModel;

namespace SmartAssistant.Controllers
{
    public class SentimentResult
    {
        [Description("Overall sentiment: Positive, Neutral, Negative, Frustrated")]
        public string Sentiment { get; set; } = string.Empty;

        [Description("Confidence score between 0 and 1")]
        public float Confidence { get; set; }

        [Description("Urgency level: Low, Medium, High")]
        public string Urgency { get; set; } = string.Empty;

        [Description("True if the user needs immediate human intervention")]
        public bool RequiresHumanAgent { get; set; }

        [Description("Brief reason for the sentiment classification")]
        public string Reason { get; set; } = string.Empty;
    }
}
