namespace SmartAssistant.Services
{
    public record ChatRequest(string sessionId, string message);
    public record ChatResponse(string sessionId, string reply, int inputTokens, int outputTokens);
    public interface IChatAssistant
    {
        Task<ChatResponse> chat(ChatRequest request);
    }
}
