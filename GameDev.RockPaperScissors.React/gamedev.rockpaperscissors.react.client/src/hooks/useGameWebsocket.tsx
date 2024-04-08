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
      setIncomingMessage(receivedMessage);
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

  const sendMove = (move:object) => {
    if (ws.current && ws.current.readyState === WebSocket.OPEN) {
      ws.current.send(JSON.stringify(move));
    }
  };

  return { sendMove, isConnected, incomingMessage };
};

export default useGameWebSocket;