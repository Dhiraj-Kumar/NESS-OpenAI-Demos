using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Services;

namespace SmartAssistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IChatAssistant chatAssistant;

        public AssistantController(IChatAssistant chatAssistant)
        {
            this.chatAssistant = chatAssistant;
        }

        [Authorize]
        [HttpPost("ask")]
        public async Task<ActionResult<ChatResponse>> Ask([FromBody] ChatRequest request)
        {
            return Ok(await chatAssistant.chat(request));
        }
    }
}
