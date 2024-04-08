import { Link } from 'react-router-dom';
import './App.css';

function home() {
  return (
    <div>
    <div>Choose an app:</div>
    <nav>
      <Link to="/">Home</Link><br />
        <Link to="/rps">Rock, Paper, Scissors</Link><br />
        <Link to="/weather">weather</Link>
      {/* Add more links as needed */}
      </nav>
    </div>
  );
}

export default home;