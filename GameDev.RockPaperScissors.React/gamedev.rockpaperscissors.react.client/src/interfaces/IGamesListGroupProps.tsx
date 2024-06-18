import NameId  from "../models/Game";

interface IGamesListGroupProps {
  items: NameId[]; // Use the NameId class for items
  onItemClick: (item: NameId) => void; // Use NameId for the onItemClick callback
}

export default IGamesListGroupProps;