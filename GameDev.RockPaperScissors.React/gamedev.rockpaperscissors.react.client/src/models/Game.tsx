import NameId from './NameId'; // Import the base class

class Game extends NameId {
  // You can add additional properties or methods specific to the subclass
  description: string;

  constructor(id: number, name: string, description: string) {
    // Call the base class constructor using super()
    super(id, name);

    // Initialize subclass-specific properties
    this.description = description;
  }
}

export default Game;