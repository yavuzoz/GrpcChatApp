using GrpcChatServer.Services;

namespace GrpcChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection(); 

            var app = builder.Build();

            app.MapGrpcService<GrpcChatServer.Services.ChatService>();

            app.MapGrpcReflectionService();

            app.MapGet("/", () => "gRPC Chat Server working!");

            app.Run();
        }
    }
}
