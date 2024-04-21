using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameDev.RockPaperScissors.React.Server.Websockets
{
    public class Game
    {
        public WebSocket Player1 { get; set; }
        public WebSocket Player2 { get; set; }
        public string Player1Move { get; set; }
        public string Player2Move { get; set; }

        // You can add methods here to check for a winner, reset game, etc.
    }

    public static class RockPaperScissorsWebSockets
    {


        private static List<WebSocket> _connectedClients = new List<WebSocket>();

        public static async Task Echo(HttpContext context, WebSocket currentSocket)
        {
            var clientId = Guid.NewGuid().ToString();  // Unique identifier for each client
            _connectedClients.Add(currentSocket);

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Assuming the message format is simple JSON like { "gameId": "some-id", "move": "rock" }
                var moveData = JsonSerializer.Deserialize<Dictionary<string, string>>(receivedMessage);
                var gameId = moveData["gameId"];
                var move = moveData["move"];

                // Fetch or create game state from dictionary
                Game game;
                if (!_games.TryGetValue(gameId, out game))
                {
                    game = new Game { Player1 = currentSocket };
                    _games[gameId] = game;
                }

                    // Assign second player if not already assigned
                    if (game.Player2 == null && currentSocket != game.Player1)
                    {
                        game.Player2 = currentSocket;
                    }

                    // Update move based on player
                    if (currentSocket == game.Player1)
                        game.Player1Move = move;
                    else if (currentSocket == game.Player2)
                        game.Player2Move = move;

                    // Check if both moves are made
                    if (game.Player1Move != null && game.Player2Move != null)
                    {
                        // Determine result and respond
                        GameMove resultMessage = new GameMove{ Data = DetermineWinner(game.Player1Move, game.Player2Move) };
                        var gameResult = JsonSerializer.Serialize(resultMessage);
                        await SendMessage(game.Player1, gameResult);
                        await SendMessage(game.Player2, gameResult);
                        // Reset moves for next round
                        game.Player1Move = game.Player2Move = null;
                    }
                

                result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            _connectedClients.Remove(currentSocket);
            await currentSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private static async Task SendMessage(WebSocket socket, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        // Implement game logic to determine the winner based on moves
        private static string DetermineWinner(string move1, string move2)
        {
            // Simplified example of determining the winner
            if (move1 == move2) return "Draw!";
            if (move1 == "rock" && move2 == "scissors") return "Player 1 wins!";
            if (move1 == "scissors" && move2 == "paper") return "Player 1 wins!";
            if (move1 == "paper" && move2 == "rock") return "Player 1 wins!";
            return "Player 2 wins!";
        }

        // Dictionary to store game states
        private static ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>();
    }

    public class GameMove
    {
        public string Data { get; set; }
    }

}
