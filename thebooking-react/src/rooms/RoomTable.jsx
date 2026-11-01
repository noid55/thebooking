import { useState } from 'react';
import { Button, Table } from 'react-bootstrap';

const RoomTable = ({ rooms }) => {
  const [showDescriptions, setShowDescriptions] = useState(true);

  const toggleDescriptions = () => {
    setShowDescriptions((prev) => !prev);
  };

  return (
    <div>
      <Button
        variant="secondary"
        className="mb-3"
        onClick={toggleDescriptions}
      >
        {showDescriptions ? 'Hide Descriptions' : 'Show Descriptions'}
      </Button>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Name</th>
            <th>Building</th>
            <th>Capacity</th>
            {showDescriptions && <th>Description</th>}
          </tr>
        </thead>

        <tbody>
          {rooms.map((room) => (
            <tr key={room.roomId}>
              <td>{room.name}</td>
              <td>{room.building}</td>
              <td>{room.capacity}</td>
              {showDescriptions && <td>{room.description}</td>}
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default RoomTable;

