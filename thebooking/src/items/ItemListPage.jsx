import { Table } from 'react-bootstrap';

const ItemListPage = () => {
  const items = [
    {
      ItemId: 1,
      Name: "Room 1",
      Description: "Large room: 16 seats. The room is good for: meatings and courses. ",
      ImageUrl: "https://placehold.co/400x300?text=Room+1"
    },
    {
      ItemId: 2,
      Name: "Room 2",
      Description: "Large room: 16 seats. The room is good for: meatings ",
      ImageUrl: "https://placehold.co/400x300?text=Room+2"
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
        <tfooter>

        <p>email:Booking.no</p>
        <p>tlf: +47 123 456 78</p>
        </tfooter>

        </tbody>

      </Table>
    </div>
  );
};

export default ItemListPage;

