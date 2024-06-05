import './App.css';
import ListGroup from './components/ListGroup';
import NameId from './models/NameId';
import IncomingMessage from './models/IncomingMessage';

import { useState, useEffect } from 'react';
import useGameWebSocket from './hooks/useGameWebsocket';

import GameState from './models/GameState';
import Game from './models/Game';


const RPS = () => {
  const [gamesList, setGameList] = useState<Game[]>([]);
  const [currentGame, setCurrentGame] = useState<Game>();
  const [gameName, setGameName] = useState('');
  const [currentPlayer, setCurrentPlayer] = useState('');
  const [playerMove, setPlayerMove] = useState('');
  const [isCreatingGame, setIsCreatingGame] = useState(false);
  const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(false);

  const { sendMove, isConnected, incomingMessage }:
    { sendMove: any, isConnected: boolean, incomingMessage: IncomingMessage | null } = useGameWebSocket('wss://localhost:7062/ws');
  const [serverMessages, setServerMessages] = useState<IncomingMessage[]>([]);

  useEffect(() => {

    switch (incomingMessage?.MsgType) {
      case GameState.InitialConnection:
        for (let i = 0; i < incomingMessage.Games.length; i++) {
          const game = incomingMessage.Games[i];
          const newGame = new Game(game.Id, game.Name);
          setGameList(gamesList => [...gamesList, newGame]);
        }
        break;
      case GameState.NewGame: {
        const newGame = new Game(incomingMessage.Id, incomingMessage.Name);
        setGameList(gamesList => [...gamesList, newGame]);
        setCurrentGame(newGame);
        setServerMessages([]);
        break;
      }
      case GameState.GameJoined: {
        const selectedGame = gamesList.find(game => game.Id === incomingMessage.Id);
        if (selectedGame) {
          selectedGame.Ready = true;
          setCurrentGame(selectedGame);
          setIsWaitingForOpponent(false);
        }
        break;
      }
      case GameState.ExitGame:
        setGameList(gamesList => gamesList.filter(game => game.Id !== currentGame?.Id));
        setCurrentGame(undefined);
        setIsWaitingForOpponent(false);
        setServerMessages(prevMessages => [...prevMessages, incomingMessage]);
        break;
      case GameState.GameFinished:
        setIsWaitingForOpponent(false);
        setServerMessages(prevMessages => [...prevMessages, incomingMessage]);
        setGameList(gamesList => gamesList.filter(game => game.Id !== currentGame?.Id));
        setCurrentGame(undefined);
        break;
      default:
        if (incomingMessage)
          setServerMessages(prevMessages => [...prevMessages, incomingMessage]);
    }
  }, [incomingMessage]);

  const createGame = () => {
    setIsCreatingGame(true);
    setCurrentPlayer("Player1");
  };


  const handleCreateGame = () => {
    if(!gameName) {
      return;
    }

    // Send the game name to the server to create a new game
    // Handle the response from the server
    setIsCreatingGame(false);
    sendMove(gameName, GameState.CreateGame);
    setIsWaitingForOpponent(true); // Display "Waiting for opponent" UI
  };

  const joinGame = (gameId: string) => {
    // Send a message to the server to join the selected game
    // Handle the response from the server
    sendMove(gameId, GameState.JoinGame);
    setCurrentPlayer("Player2");
  };

  const exitGame = () => {
    // Send a message to the server to exit the current game 
    sendMove(currentGame?.Id, GameState.ExitGame);
    setCurrentGame(undefined);

  };

  const handleMove = (gameId: string, move: string) => {
    if (move) {
      sendMove(gameId, move);
      setPlayerMove(move);
    }
  };

  const handleGameClick = (game: NameId) => {
    joinGame(game.Id);
  };

  return (
    <div>
      <h1>Rock Paper Scissors Game</h1>
      <h2>{currentPlayer}</h2>
      <h2>{playerMove}</h2>
      {isConnected ? <div>Status: Connected</div> : <div>Status: Disconnected</div>}

      {currentGame?.Ready ? (
        <>
          {playerMove === '' &&
            (<div>
              <div>
                <a href="#" onClick={() => { handleMove(currentGame?.Id ?? '', 'rock'); }}>Rock</a> | <a href="#" onClick={() => { handleMove(currentGame?.Id ?? '', 'paper'); }}>Paper</a> | <a href="#" onClick={() => { handleMove(currentGame?.Id ?? '', 'scissors'); }}>Scissors</a>
              </div>
              <div>
                <button onClick={exitGame}>Exit Game</button>
              </div>
            </div>)
          }
        </>
      ) : (
        <>
          {!isCreatingGame && !isWaitingForOpponent && (
            <div>
              <button onClick={createGame}>Create New Game</button>
            </div>
          )}
          {isCreatingGame && (
            <div>
              <input type="text" value={gameName} onChange={(e) => setGameName(e.target.value)} />
              <button onClick={handleCreateGame}>Create Game</button>
            </div>
          )}
          {isWaitingForOpponent && (
            <div>
              <p>Waiting for opponent...</p>
            </div>
          )}
          {!isWaitingForOpponent && (
            <div>
              <h2>Game List</h2>
              <ListGroup items={gamesList} onItemClick={handleGameClick} />
            </div>
          )}
        </>
      )}

      <div>
        <h2>Server Messages</h2>
        {serverMessages.map((msg, index) => <div key={index}>{JSON.stringify(msg)}</div>)}
      </div>
    </div>
  );
};

export default RPS;