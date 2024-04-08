import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import RPS from './rps';
import Home from './home';
import Weather from './Weather';
import './App.css';

function App() {
    

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/rps" element={<RPS />} />
        <Route path="/weather" element={<Weather />} />
        {/* Add more routes as needed */}
      </Routes>
    </Router>
  );

}

export default App;