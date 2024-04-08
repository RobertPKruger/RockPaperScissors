using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace GameDev.RockPaperScissors.React.Server.Websockets
{
    public static class RockPaperScissorsWebSockets
    {
        private static List<WebSocket> _connectedClients = new List<WebSocket>();

        public static async Task Echo(HttpContext context, WebSocket currentSocket)
        {
            _connectedClients.Add(currentSocket);

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                foreach (var client in _connectedClients)
                {
                    if (client.State == WebSocketState.Open && client != currentSocket)
                    {
                        await client.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }
                result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            _connectedClients.Remove(currentSocket);
            await currentSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

        }
    }

    public class GameMove
    {
        public string Action { get; set; }
        public string Data { get; set; }
    }

    public class MyHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

}
