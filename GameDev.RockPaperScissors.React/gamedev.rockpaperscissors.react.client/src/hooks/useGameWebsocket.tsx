import { useEffect, useState, useRef } from 'react';
import IncomingMessage from '../models/IncomingMessage';

const useGameWebSocket = (url: string) => {
  const [isConnected, setIsConnected] = useState(false);
  const [incomingMessage, setIncomingMessage] = useState<IncomingMessage | null > (null);
  const ws = useRef<WebSocket | null>(null);

  useEffect(() => {
    // Initialize WebSocket connection
    createWebSocket(url);

    // Cleanup on unmount
    return () => {
      ws.current?.close();
    };
  }, [url]);


  const createWebSocket = (url:string) => {
    ws.current = new WebSocket(url);
    const wsCurrent = ws.current;
    wsCurrent.onopen = () => {
      console.log('WebSocket connected');
      setIsConnected(true);
      sendMove("", 'initialConnection');
    };
    wsCurrent.onmessage = (event) => {
      const receivedMessage: IncomingMessage = JSON.parse(event.data);
      setIncomingMessage(receivedMessage);
    };

    wsCurrent.onclose = () => {
      console.log('WebSocket disconnected');
      setIsConnected(false);
    };
  };

  const sendMove = (gameId:string, move:string) => {
    if (ws.current && ws.current.readyState === WebSocket.OPEN) {
      const gameMove = {
        gameId: gameId,
        move: move
      };
      ws.current.send(JSON.stringify(gameMove)); // Send move as a JSON string
      console.log('Sent: ' + JSON.stringify(gameMove));
    }
  };

  return { sendMove, isConnected, incomingMessage };
};

export default useGameWebSocket;