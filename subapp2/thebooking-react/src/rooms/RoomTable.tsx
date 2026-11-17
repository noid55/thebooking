import { useState } from 'react';
import { Button, Table } from 'react-bootstrap';
import { Room } from '../types/room';

const roomImages = [
  '/images/room1.jpg',
  '/images/room2.avif',
  '/images/room3.webp',
  '/images/room4.webp',
];

interface RoomTableProps {
  rooms: Room[];
}

export default function RoomTable({ rooms }: RoomTableProps) {
  const [showDescriptions, setShowDescriptions] = useState<boolean>(true);
  const [showImages, setShowImages] = useState<boolean>(true);

  const toggleDescriptions = () => {
    setShowDescriptions((prev) => !prev);
  };

  const toggleImages = () => {
    setShowImages((prev) => !prev);
  };

  return (
      <div>
        <div className="d-flex justify-content-end flex-wrap gap-2 mb-3">
          <Button
              variant="outline-secondary"
              size="sm"
              onClick={toggleDescriptions}
          >
            {showDescriptions ? 'Hide Descriptions' : 'Show Descriptions'}
          </Button>

          <Button
              variant="outline-secondary"
              size="sm"
              onClick={toggleImages}
          >
            {showImages ? 'Hide Images' : 'Show Images'}
          </Button>
        </div>

        <Table striped bordered hover responsive>
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
                            width: '120px',
                            height: '80px',
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
}