// ListGroup.tsx
import IListGroupProps from '../interfaces/IListGroupProps'; // Import the IListGroupProps interface

function ListGroup({ items, onItemClick }: IListGroupProps) {
  return (
    <ul className="list-group">
      {items.map((item) => (
        <li key={item.Id} className="list-group-item">
          <a href="#" onClick={() => onItemClick(item)}>{item.Name}</a>
        </li>
      ))}
    </ul>
  );
}

export default ListGroup;