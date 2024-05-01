// ListGroup.tsx
import NameId from '../models/NameId'; // Import the NameId class

interface ListGroupProps {
  items: NameId[]; // Use the NameId class for items
  onItemClick: (item: NameId) => void; // Use NameId for the onItemClick callback
}

function ListGroup({ items, onItemClick }: ListGroupProps) {
  return (
    <ul className="list-group">
      {items.map((item) => (
        <li key={item.id} className="list-group-item">
          <a href="#" onClick={() => onItemClick(item)}>{item.name}</a>
        </li>
      ))}
    </ul>
  );
}

export default ListGroup;