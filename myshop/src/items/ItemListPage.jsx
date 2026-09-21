import { Table } from 'react-bootstrap';

const ItemListPage = () => {
  const items = [
    {
      ItemId: 1,
      Name: "Room 1",
      Description: "Large room: 16 seats. The room is good for: meatings and courses. ",
      ImageUrl: "/images/chickenleg.jpg"
    },
    {
      ItemId: 2,
      Name: "Room 2",
      Description: "Large room: 16 seats. The room is good for: meatings ",
      ImageUrl: "/images/fishandchips.jpg"
    }
  ];

  return (
    <div>
      <h1>Items</h1>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Descriptions</th>
            <th>Images</th>
          </tr>
        </thead>
        <tbody>
          {items.map(item => (
            <tr key={item.ItemId}>
              <td>{item.ItemId}</td>
              <td>{item.Name}</td>
              <td>{item.Description}</td>
              <td><img src={item.ImageUrl} alt={item.Name} width="120" /></td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default ItemListPage;

