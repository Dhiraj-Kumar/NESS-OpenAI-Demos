using System.Configuration;
using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;
using Microsoft.Extensions.AI;
namespace OpenAIDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BasicChat().GetAwaiter().GetResult();
            //StreamChat().GetAwaiter().GetResult();
            //MessageChat().GetAwaiter().GetResult();
            //ChatBot().GetAwaiter().GetResult();
            StructuredOutput().GetAwaiter().GetResult();
        }

        private static async Task BasicChat()
        {
            var apiKey = ConfigurationManager.AppSettings["OPENAI_API_KEY"];
            OpenAIClient client = new OpenAIClient(apiKey);
            ChatClient chatClient = client.GetChatClient("gpt-4o-mini");

            while (true)
            {
                Console.Write("You: ");
                string? message = Console.ReadLine();
                if (message == null || message.ToLower() == "exit")
                    break;
                ChatCompletion response = await chatClient.CompleteChatAsync(message);
                Console.WriteLine($"AI: { response.Content[0].Text}");
            }
        }

        private static async Task StreamChat()
        {
            var apiKey = ConfigurationManager.AppSettings["OPENAI_API_KEY"];
            OpenAIClient client = new OpenAIClient(apiKey);
            ChatClient chatClient = client.GetChatClient("gpt-4o-mini");
            while (true)
            {
                Console.Write("You: ");
                string? message = Console.ReadLine();
                if (message == null || message.ToLower() == "exit")
                    break;
                
                await foreach (StreamingChatCompletionUpdate update in chatClient.CompleteChatStreamingAsync(message))
                {
                    foreach (ChatMessageContentPart part in update.ContentUpdate)
                    {
                        Console.Write(part.Text);
                    }
                }
                Console.WriteLine();
            }

        }

        private static async Task MessageChat()
        {
            var apiKey = ConfigurationManager.AppSettings["OPENAI_API_KEY"];
            OpenAIClient client = new OpenAIClient(apiKey);
            ChatClient chatClient = client.GetChatClient("gpt-4o-mini");

            // System Message - System level information for the LLM
            SystemChatMessage systemChatMessage = new SystemChatMessage("You are a senior .NET architect. Answer only .NET related questions. Be concise and always include code examples");
            UserChatMessage userChatMessage = new UserChatMessage("Who is the prime minister of India");
            ChatCompletion response = await chatClient.CompleteChatAsync(systemChatMessage, userChatMessage);

            Console.WriteLine(response.Content[0].Text);
        }

        private static async Task ChatBot()
        {
            var apiKey = ConfigurationManager.AppSettings["OPENAI_API_KEY"];
            OpenAIClient client = new OpenAIClient(apiKey);
            ChatClient chatClient = client.GetChatClient("gpt-4o-mini");

            // System Message - System level information for the LLM
            
            List<ChatMessage> messages = new List<ChatMessage>()
            {
                new SystemChatMessage("You are a helpful AI assistant who answers query with a bit of humour. Include emojis while responding to questions")
            };
            while (true)
            {
                Console.Write("You: ");
                string? message = Console.ReadLine();
                if (message == null || message.ToLower() == "exit")
                    break;
                messages.Add(new UserChatMessage(message));
                
                ChatCompletion response = await chatClient.CompleteChatAsync(messages);
                messages.Add(new AssistantChatMessage(response.Content[0].Text));
                Console.WriteLine($"AI: {response.Content[0].Text}");
            }
        }

        private static async Task StructuredOutput()
        {
            var apiKey = ConfigurationManager.AppSettings["OPENAI_API_KEY"];
            OpenAIClient client = new OpenAIClient(apiKey);
            ChatClient chatClient = client.GetChatClient("gpt-4o-mini");

            JsonElement schemaElement = AIJsonUtilities.CreateJsonSchema(typeof(CourseInfo));
            BinaryData schema = BinaryData.FromString(schemaElement.GetRawText());


            OpenAI.Chat.ChatResponseFormat format = OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "course_info",
                jsonSchema: schema,
                jsonSchemaIsStrict: false
            );

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage("You are a hellpful AI assistant"),
                new UserChatMessage("""
                    Create a detailed course outline for learning async/await in c# with the following data.
                    - A clear descriptive title
                    - The programming language used
                    - A listt of at least 6 specific topics covered in the course
                    - The estimaed hours to complete the course
                    """)
            };

            ChatCompletion response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions { ResponseFormat=format});

            CourseInfo? course = JsonSerializer.Deserialize<CourseInfo>(response.Content[0].Text);

            Console.WriteLine(response.Content[0].Text);

            Console.WriteLine($"Title: {course!.title}");
            Console.WriteLine($"Language: {course.language}");
            Console.WriteLine($"Topics: {string.Join(", ", course.topics)}");
            Console.WriteLine($"Hours: {course.estimatedHours}");
        }


    }
}
