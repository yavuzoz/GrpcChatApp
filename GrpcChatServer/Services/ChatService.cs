using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcChatServer.Protos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GrpcChatServer.Services
{
    public class ChatService : Protos.ChatService.ChatServiceBase
    {
        private readonly List<IServerStreamWriter<ChatMessage>> _clients = new List<IServerStreamWriter<ChatMessage>>();
        private readonly object _lock = new object();
        private readonly string logFilePath = "chat_log.txt";

        public override async Task SendMessage(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            lock (_lock)
            {
                _clients.Add(responseStream);
            }

            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    LogMessage(message);
                    await BroadcastMessage(message);
                }
            }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(responseStream);
                }
            }
        }

        public override async Task<Empty> UserStatusUpdate(UserStatus request, ServerCallContext context)
        {
            string statusMessage = request.IsConnected
                ? $"{request.Username} joined the chat."
                : $"{request.Username} left the chat.";

            ChatMessage statusChatMessage = new ChatMessage
            {
                Username = "System",
                Text = statusMessage,
                MessageType = MessageType.Info
            };

            Console.WriteLine($"DEBUG: UserStatusUpdate called for {request.Username} - IsConnected: {request.IsConnected}");

            await BroadcastMessage(statusChatMessage);
            LogMessage(statusChatMessage);

            return new Empty();
        }

        private void LogMessage(ChatMessage message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{message.MessageType}] {message.Username}: {message.Text}";
            Console.WriteLine(logEntry);
            Console.WriteLine("RECEIVED MESSAGE: " + logEntry);
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }

        private async Task BroadcastMessage(ChatMessage message)
        {
            lock (_lock)
            {
                foreach (var client in _clients)
                {
                    client.WriteAsync(message);
                }
            }
        }
    }
}
