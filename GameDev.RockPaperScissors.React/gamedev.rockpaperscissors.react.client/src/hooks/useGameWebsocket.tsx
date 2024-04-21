import { useEffect, useState, useRef } from 'react';

const useGameWebSocket = (url: string) => {
  const [isConnected, setIsConnected] = useState(false);
  const [incomingMessage, setIncomingMessage] = useState(null);
  const ws = useRef<WebSocket | null>(null);

  useEffect(() => {
    // Initialize WebSocket connection
    ws.current = new WebSocket(url);
    const wsCurrent = ws.current;

    wsCurrent.onopen = () => {
      console.log('WebSocket connected');
      setIsConnected(true);
    };

    wsCurrent.onmessage = (event) => {
      const receivedMessage = JSON.parse(event.data);
      setIncomingMessage(receivedMessage.Data);
    };

    wsCurrent.onclose = () => {
      console.log('WebSocket disconnected');
      setIsConnected(false);
    };

    // Cleanup on unmount
    return () => {
      wsCurrent.close();
    };
  }, [url]);

  const sendMove = (move:string) => {
    if (ws.current && ws.current.readyState === WebSocket.OPEN) {
      const gameMove = {
        gameId: "47ec3a5b-2d2f-4a91-966f-ae0e3c1af779",
        move: move
      };
      ws.current.send(JSON.stringify(gameMove)); // Send move as a JSON string
      console.log('Sent: ' + JSON.stringify(gameMove));
    }
  };

  return { sendMove, isConnected, incomingMessage };
};

export default useGameWebSocket;