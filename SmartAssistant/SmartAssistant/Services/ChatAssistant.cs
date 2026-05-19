
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using OpenAI.Chat;
using System.Runtime.CompilerServices;
using System.Text;

namespace SmartAssistant.Services
{
    public class ChatAssistant : IChatAssistant
    {
        private readonly ChatClient client;
        private readonly IMemoryCache memoryCache;
        private readonly string systemPrompt = "You are a helpful IT helpdesk assistant. You are giving concise responses";

        public ChatAssistant(ChatClient client, IMemoryCache cache)
        {
            this.client = client;
            memoryCache = cache;
        }
        public async Task<ChatResponse> chat(ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.sessionId))
                throw new Exception("Session Id is required");

            List<ChatMessage> history = memoryCache.GetOrCreate(
                request.sessionId,
                entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                    return new List<ChatMessage>
                    {
                        new SystemChatMessage(systemPrompt)
                    };
                }
            );

            history.Add(new UserChatMessage(request.message));
            StringBuilder reply = new();
            ChatCompletion response = await client.CompleteChatAsync(history);
            reply.Append(response.Content[0].Text);

            history.Add(new AssistantChatMessage(reply.ToString()));

            memoryCache.Set(request.sessionId, history, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return new ChatResponse(
                sessionId: request.sessionId,
                reply: response.Content[0].Text, 
                inputTokens: response.Usage.InputTokenCount, 
                outputTokens: response.Usage.OutputTokenCount
            );
        }

        
    }
}
