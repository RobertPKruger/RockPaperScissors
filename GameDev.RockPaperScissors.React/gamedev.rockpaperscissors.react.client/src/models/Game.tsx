import NameId from './NameId'; // Import the base class
class Game extends NameId {
  public CurrentMessage: string | undefined;
  public Ready: boolean | undefined = false;
  public IsInProgress: boolean | undefined = false;
}

export default Game;