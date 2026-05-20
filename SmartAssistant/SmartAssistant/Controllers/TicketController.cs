using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using SmartAssistant.Models;
using System.Text.Json;
using Microsoft.Extensions.AI;
using ChatMessage = OpenAI.Chat.ChatMessage;
using System.Diagnostics;
using Serilog;

namespace SmartAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ChatClient _chatClient;

        public TicketController(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        [HttpPost("classify")]
        public async Task<ActionResult<ClassifiedTicket>> Classify([FromBody] SupportTicket ticket)
        {
            JsonElement schemaElement = AIJsonUtilities.CreateJsonSchema(typeof(ClassifiedTicket));
            BinaryData schema = BinaryData.FromString(schemaElement.GetRawText());

            OpenAI.Chat.ChatResponseFormat format = OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "classified_ticket",
                jsonSchema: schema,
                jsonSchemaIsStrict: false
            );

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage("""
                    You are an IT helpdesk classifier. Analyze support tickets and classify them.
                    Always populate every field. Be specific with suggested steps.
                    """),
                new UserChatMessage($"""
                    Classify this support ticket:
                    Title: {ticket.Title}
                    Description: {ticket.Description}
                    Submitted By: {ticket.SubmittedBy}
                    """)
            };

            var stopwatch = Stopwatch.StartNew();

            Log.Information("AI call started");

            ChatCompletion response = await _chatClient.CompleteChatAsync(
                messages,
                new ChatCompletionOptions { ResponseFormat = format }
            );

            ClassifiedTicket? classified = JsonSerializer.Deserialize<ClassifiedTicket>(
                response.Content[0].Text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            stopwatch.Stop();

            Log.Information("AI Call Completed. Operation={Operation}, Model={Model}, InputTokens={InputTokens}, OutputTokens={OutputTokens}, TimeTaken={TimeTaken}", "Classify", "gpt-4o-mini", response.Usage.InputTokenCount, response.Usage.OutputTokenCount, stopwatch.Elapsed.TotalMilliseconds);

            return Ok(classified);
        }

        public record SentimentRequest(string Message);

        [HttpPost("analyze")]
        public async Task<ActionResult<SentimentResult>> Analyze([FromBody] SentimentRequest request)
        {
            JsonElement schemaElement = AIJsonUtilities.CreateJsonSchema(typeof(SentimentResult));
            BinaryData schema = BinaryData.FromString(schemaElement.GetRawText());

            OpenAI.Chat.ChatResponseFormat format = OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "sentiment_result",
                jsonSchema: schema,
                jsonSchemaIsStrict: false
            );

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage("""
                    You are a sentiment analysis engine for an IT helpdesk.
                    Analyze the customer message and return a structured sentiment report.
                    Set RequiresHumanAgent to true if the user is highly frustrated,
                    threatening to escalate, or the issue is critical.
                    """),
                new UserChatMessage(request.Message)
            };

            ChatCompletion response = await _chatClient.CompleteChatAsync(
                messages,
                new ChatCompletionOptions { ResponseFormat = format }
            );

            SentimentResult? result = JsonSerializer.Deserialize<SentimentResult>(
                response.Content[0].Text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return Ok(result);
        }

    }
}
