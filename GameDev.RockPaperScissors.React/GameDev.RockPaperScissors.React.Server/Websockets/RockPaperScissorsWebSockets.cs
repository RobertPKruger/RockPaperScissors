using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameDev.RockPaperScissors.React.Server.Websockets
{
    public static class GameState
    {
        public static string CreateGame = "createGame";
        public static string JoinGame = "joinGame";
        public static string InitialConnection = "initialConnection";
        public static string NewGame = "newGame";
        public static string GameJoined = "gameJoined";
        public static string ExitGame = "exitGame";
        public static string GameFinished = "gameFinished";
    }

    public class  MessageType
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string MsgType { get; set; } = ""; //createGame, joinGame, initialConnection

        public string Data { get; set;}
    }

    public class InitialConnection : MessageType
    {
        public string ServerNotes { get; set; }

        public List<Game> Games { get; set; }
    }

    public class Game : MessageType
    {
        public WebSocket Player1 { get; set; }
        public WebSocket Player2 { get; set; }
        public string Player1Move { get; set; }
        public string Player2Move { get; set; }
    }

    public static class RockPaperScissorsWebSockets
    {
        private static List<WebSocket> _connectedClients = new List<WebSocket>();

        public static async Task Echo(HttpContext context, WebSocket currentSocket)
        {
            _connectedClients.Add(currentSocket);

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            try
            {
                result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in initial receive: {ex.Message}");
                await currentSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error during initial receive", CancellationToken.None);
                return;
            }

            while (!result.CloseStatus.HasValue)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Assuming the message format is simple JSON like { "gameId": "some-id", "move": "rock" }
                var moveData = JsonSerializer.Deserialize<Dictionary<string, string>>(receivedMessage);

                var gameId = moveData["gameId"];
                var move = moveData["move"];

                Game game = null;

                if (move == GameState.ExitGame)
                {
                    if (_games.TryGetValue(gameId, out game))
                    {
                        ResolveGame(game, true);
                    }
                    result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    continue;

                }

                if (move == GameState.InitialConnection)
                {

                    var games = _games.Values.ToList();

                    var initialConnection = new InitialConnection { MsgType = GameState.InitialConnection, ServerNotes = "Welcome to the server", Games = games };

                    await SendMessage(currentSocket, JsonSerializer.Serialize(initialConnection));
                    result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    continue;
                };

                if (move == GameState.CreateGame)
                {

                    var name = gameId;
                    gameId = $"{gameId}-{Guid.NewGuid()}";

                    game = new Game { Id = gameId, Name = name, MsgType = GameState.NewGame, Player1 = currentSocket };
                    _games[gameId] = game;

                    foreach (var client in _connectedClients)
                    {
                        if (client.State == WebSocketState.Open)
                        {
                            await SendMessage(client, JsonSerializer.Serialize(game));
                        }
                    }

                    result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    continue;
                };

                if (move == GameState.JoinGame)
                {

                    if (!_games.TryGetValue(gameId, out game))
                    {
                        game = new Game { Player1 = currentSocket };
                        _games[gameId] = game;
                    }

                    game.Player2 = currentSocket;

                    var joinMessage = new MessageType { Id = gameId, MsgType = GameState.GameJoined };

                    if (game.Player1 != null)
                    {
                        await SendMessage(game.Player1, JsonSerializer.Serialize(joinMessage));
                    }

                    if (game.Player2 != null)
                    {
                        await SendMessage(game.Player2, JsonSerializer.Serialize(joinMessage));
                    }

                    result = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    continue;
                };

                // Fetch or create game state from dictionary
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
                else
                    game.Player2Move = move;

                // Check if both moves are made
                if (game.Player1Move != null && game.Player2Move != null)
                {
                    var resultMessage = ResolveGame(game);

                    var gameResult = JsonSerializer.Serialize(resultMessage);
                    await SendMessage(game.Player1, gameResult);
                    await SendMessage(game.Player2, gameResult);

                    _connectedClients.Remove(game.Player1);
                    _connectedClients.Remove(game.Player2);

                    game.Player1 = null;
                    game.Player2 = null;
                    _games.TryRemove(gameId, out game);
                }

                if (currentSocket != null) { 
                    //player made move and is waiting for the other player to make a move
                    await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

            }
        }

        private static MessageType ResolveGame(Game game, bool canceled = false)
        {
            // Determine result and respond
            MessageType resultMessage = new MessageType();

            if (canceled) {
                resultMessage.Data = "canceled";
                resultMessage.MsgType = GameState.ExitGame;
            } else { 
                resultMessage.Data = DetermineWinner(game.Player1Move, game.Player2Move);
                resultMessage.MsgType = GameState.GameFinished;
            }

            return resultMessage;
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
            if (move1 == "rock" && move2 == "scissors" || move1 == "scissors" && move2 == "paper" || move1 == "paper" && move2 == "rock") return $"Player 1 wins with {move1} over {move2}!";
            return $"Player 2 wins with {move2} over {move1}!";
        }

        // Dictionary to store game states
        private static ConcurrentDictionary<string, Game> _games = new ConcurrentDictionary<string, Game>();
    }

}
