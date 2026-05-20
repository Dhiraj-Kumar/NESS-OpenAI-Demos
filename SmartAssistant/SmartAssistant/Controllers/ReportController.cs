using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Chat;
using SmartAssistant.Models;

namespace SmartAssistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ChatClient client;

        public ReportController(ChatClient client)
        {
            this.client = client;
        }

        [HttpPost("report")]
        public async Task<ActionResult> GenerateWeeklyReport([FromBody] WeeklyReportRequest data)
        {
            List<ChatMessage> messages = new()
            {
                new SystemChatMessage("""
                    You are an IT helpdesk reporting assistant.
                    Generate professional, concise weekly reports for IT managers.
                    Use clear sections: Summary, Key Highlights, Issues Breakdown of top issues, Recommendations.
                    Keep a professional tone.
                """),
                new UserChatMessage($"""
                    Generate a weekly IT helpdesk report for the following data:
                    - Total Tickets Received: {data.totalTickets}
                    - Resolved: {data.resolved}
                    - Pending: {data.pending}
                    - Critical Issues: {data.critical}
                    - Average Resolution Time: {data.averageResolutionHours}
                    - Top Issues: {data.topIssues}
                """)
            };

            ChatCompletion response = await client.CompleteChatAsync(messages);
            return Ok(response.Content[0].Text);
        }
    }
}
