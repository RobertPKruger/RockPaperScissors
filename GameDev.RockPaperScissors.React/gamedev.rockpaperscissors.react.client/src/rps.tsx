import './App.css';
import ListGroup from './components/ListGroup';

import { useState, useEffect } from 'react';
import useGameWebSocket from './hooks/useGameWebsocket';


const RPS = () => {
  const [gameList, setGameList] = useState([]);
  const [gameName, setGameName] = useState('');
  const [isCreatingGame, setIsCreatingGame] = useState(false);
  const [isWaitingForOpponent, setIsWaitingForOpponent] = useState(false);
  const [selectedGameId, setSelectedGameId] = useState(null);

  const { sendMove, isConnected, incomingMessage } = useGameWebSocket('wss://localhost:7062/ws');
  const [serverMessages, setServerMessages] = useState([]);

  useEffect(() => {
    if (incomingMessage) {
      // Handle the incoming message
      // For example, add it to an array of messages to display in the UI
      if (incomingMessage.MsgType == "initialConnection") {

      // Add the games to the game list
         for(var i = 0; i < incomingMessage.Games.length; i++) {
            const game = incomingMessage.Games[i];
            const newGame = { id: game.GameId, name: game.Name };
            setGameList(prevList => [...prevList, newGame]);
          }
        
      }else if (incomingMessage.MsgType == "newGame") {

        // Add the new game to the game list
        const newGame = { id: incomingMessage.GameId, name: incomingMessage.Name };
        setGameList(prevList => [...prevList, newGame]);
      }
      else if (incomingMessage.MsgType == "gameJoined") {
        setSelectedGameId(incomingMessage.GameId);
      }
      else {
        setServerMessages(prevMessages => [...prevMessages, incomingMessage]);
      }
    }
  }, [incomingMessage]);

  const createGame = () => {
    setIsCreatingGame(true);
  };


  const handleCreateGame = () => {
    // Send the game name to the server to create a new game
    // Handle the response from the server
    setIsCreatingGame(false);
    sendMove(gameName,'createGame');
    setIsWaitingForOpponent(true); // Display "Waiting for opponent" UI
  };

  const joinGame = (gameId:string) => {
    // Send a message to the server to join the selected game
    // Handle the response from the server
    setSelectedGameId(gameId);
    sendMove(gameId,'joinGame');
  };

  const exitGame = () => {
    // Send a message to the server to exit the current game
    setSelectedGameId(null);
  };

  const handleMove = (gameId, move) => {
    if (move) {
      sendMove(gameId, move);
    }
  };

  const handleGameClick = (game) => {
    joinGame(game.id);
  };

  return (
    <div>
      <h1>Rock Paper Scissors Game</h1>
      {isConnected ? <div>Status: Connected</div> : <div>Status: Disconnected</div>}

      {selectedGameId ? (
        <>
          <div>
            <a href="#" onClick={() => { handleMove(selectedGameId, 'rock'); }}>Rock</a> | <a href="#" onClick={() => { handleMove(selectedGameId, 'paper'); }}>Paper</a> | <a href="#" onClick={() => { handleMove(selectedGameId,'scissors'); }}>Scissors</a>
          </div>
          <div>
            <button onClick={exitGame}>Exit Game</button>
          </div>
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
          <div>
              <h2>Game List</h2>
              <ListGroup items={gameList} onItemClick={handleGameClick} />
          </div>
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