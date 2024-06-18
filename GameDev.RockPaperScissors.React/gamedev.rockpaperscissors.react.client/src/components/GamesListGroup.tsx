// GamesListGroup.tsx
import IGamesListGroupProps from '../interfaces/IGamesListGroupProps'; // Import the IGamesListGroupProps interface

function GamesListGroup({ items, onItemClick }: IGamesListGroupProps) {
  return (
    <ul className="list-group">
      {items.map((item) => (
        <li key={item.Id} className="list-group-item">
          {item.IsInProgress ? (
            <span>{item.Name}</span>
          ) : (
            <a href="#" onClick={() => onItemClick(item)}>{item.Name}</a>
          )}
        </li>
      ))}
    </ul>
  );
}

export default GamesListGroup;