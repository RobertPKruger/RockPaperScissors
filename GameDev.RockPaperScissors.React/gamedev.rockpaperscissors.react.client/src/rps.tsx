import './App.css';


import { useState, useEffect } from 'react';
import useGameWebSocket from './hooks/useGameWebsocket';


const RPS = () => {
  const { sendMove, isConnected, incomingMessage } = useGameWebSocket('wss://localhost:7062/ws');
  const [serverMessages, setServerMessages] = useState([]);

  useEffect(() => {
    if (incomingMessage) {
      // Handle the incoming message
      // For example, add it to an array of messages to display in the UI
      setServerMessages(prevMessages => [...prevMessages, incomingMessage]);
    }
  }, [incomingMessage]);

  const handleMove = (move:string) => {
    if (move) {
      sendMove(move);
    }
  };

  return (
    <div>
      <h1>Rock Paper Scissors Game</h1>
      {isConnected ? <div>Status: Connected</div> : <div>Status: Disconnected</div>}

      <a href="#" onClick={() => { handleMove('rock'); }}>Rock</a> | <a href="#" onClick={() => { handleMove('paper'); }}>Paper</a> | <a href="#" onClick={() => { handleMove('scissors'); }}>Scissors</a>

      <div>
        <h2>Server Messages</h2>
        {serverMessages.map((msg, index) => <div key={index}>{JSON.stringify(msg)}</div>)}
      </div>
    </div>
  );
};

export default RPS;