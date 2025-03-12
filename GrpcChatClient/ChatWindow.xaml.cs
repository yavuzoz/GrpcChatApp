using Grpc.Net.Client;
using GrpcChatClient.Protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Grpc.Core;

namespace GrpcChatClient
{
    public partial class ChatWindow : Window
    {
        private static List<ChatWindow> _openChatWindows = new List<ChatWindow>();
        private readonly string _username;
        private readonly ChatService.ChatServiceClient _client;
        private readonly AsyncDuplexStreamingCall<ChatMessage, ChatMessage> _stream;
        private readonly SolidColorBrush _windowBackground;

        public ChatWindow(string username)
        {
            InitializeComponent();
            _username = username;

            var channel = GrpcChannel.ForAddress("http://localhost:5082");
            _client = new ChatService.ChatServiceClient(channel);
            _stream = _client.SendMessage();

            _openChatWindows.Add(this);
            txtUserInfo.Text = $"User: {_username}";

            _windowBackground = new SolidColorBrush(GetRandomColor());
            this.Background = _windowBackground;

            Task.Run(ReceiveMessages);

            NotifyUserStatus(true);
        }

        private async Task ReceiveMessages()
        {
            await foreach (var message in _stream.ResponseStream.ReadAllAsync())
            {
                Dispatcher.Invoke(() => UpdateAllChatWindows(message));
            }
        }

        private void UpdateAllChatWindows(ChatMessage message)
        {
            foreach (var chatWindow in _openChatWindows)
            {
                chatWindow.lstMessages.Items.Add($"{message.Username}: {message.Text}");
            }
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            await _stream.RequestStream.WriteAsync(new ChatMessage
            {
                Username = _username,
                Text = txtMessage.Text,
                MessageType = MessageType.Info
            });

            txtMessage.Clear();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _openChatWindows.Remove(this);
            NotifyUserStatus(false);
        }

        private async void NotifyUserStatus(bool isConnected)
        {
            await _client.UserStatusUpdateAsync(new UserStatus
            {
                Username = _username,
                IsConnected = isConnected
            });
        }

        private Color GetRandomColor()
        {
            Random rand = new Random();
            return Color.FromRgb((byte)rand.Next(100, 256), (byte)rand.Next(100, 256), (byte)rand.Next(100, 256));
        }
    }
}
