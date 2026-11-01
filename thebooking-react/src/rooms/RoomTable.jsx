import { useState } from 'react';
import { Button, Table } from 'react-bootstrap';

const roomImages = [
  '/images/room1.jpg',
  '/images/room2.avif',
  '/images/room3.webp',
  '/images/room4.webp',
];

const RoomTable = ({ rooms }) => {
  const [showDescriptions, setShowDescriptions] = useState(true);
  const [showImages, setShowImages] = useState(true);

  const toggleDescriptions = () => {
    setShowDescriptions((prev) => !prev);
  };

  const toggleImages = () => {
    setShowImages((prev) => !prev);
  };

  return (
    <div>
      <Button
        variant="secondary"
        className="mb-3 me-2"
        onClick={toggleDescriptions}
      >
        {showDescriptions ? 'Hide Descriptions' : 'Show Descriptions'}
      </Button>

      <Button
        variant="secondary"
        className="mb-3"
        onClick={toggleImages}
      >
        {showImages ? 'Hide Images' : 'Show Images'}
      </Button>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Name</th>
            <th>Building</th>
            <th>Capacity</th>
            {showDescriptions && <th>Description</th>}
            {showImages && <th>Image</th>}
          </tr>
        </thead>

        <tbody>
          {rooms.map((room, index) => (
            <tr key={room.roomId}>
              <td>{room.name}</td>
              <td>{room.building}</td>
              <td>{room.capacity}</td>

              {showDescriptions && (
                <td>{room.description}</td>
              )}

              {showImages && (
                <td>
                  <img
                    src={roomImages[index % roomImages.length]}
                    alt={`Room ${room.name}`}
                    style={{
                      width: '150px',
                      height: '100px',
                      objectFit: 'cover',
                    }}
                  />
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default RoomTable;
