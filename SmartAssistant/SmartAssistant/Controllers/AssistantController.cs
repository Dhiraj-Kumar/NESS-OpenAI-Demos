using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using SmartAssistant.Services;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace SmartAssistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IChatAssistant chatAssistant;
        private readonly ChatClient client;

        public AssistantController(IChatAssistant chatAssistant, ChatClient client)
        {
            this.chatAssistant = chatAssistant;
            this.client = client;
        }

        [Authorize]
        [HttpPost("ask")]
        public async Task<ActionResult<SmartAssistant.Services.ChatResponse>> Ask([FromBody] ChatRequest request)
        {
            return Ok(await chatAssistant.chat(request));
        }

        [HttpPost("stream")]
        public async Task StreamResponse([FromBody] ChatRequest request)
        {
            Response.ContentType = "text/even-stream";

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage("You are a helpful AI assistant."),
                new UserChatMessage(request.message)
            };

            await foreach (StreamingChatCompletionUpdate update in client.CompleteChatStreamingAsync(messages))
            {
                foreach (ChatMessageContentPart part in update.ContentUpdate)
                {
                    await Response.WriteAsync($"data: {part.Text}\n\n");
                    await Response.Body.FlushAsync();
                }
            }

            await Response.WriteAsync("DONE");
        }
    }
}
