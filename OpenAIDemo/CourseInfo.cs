using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OpenAIDemo
{
    public class CourseInfo
    {
        [Description("Title of the course")]
        public string title { get; set; }

        [Description("Programming language used")]
        public string language { get; set; }

        [Description("List of topics covered in the course")]
        public List<string> topics { get; set; } = new();

        [Description("Esttimated hours to complete the course")]
        public int estimatedHours { get; set; }

    }
}
