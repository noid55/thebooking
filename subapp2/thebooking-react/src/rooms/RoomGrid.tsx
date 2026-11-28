import { Button, Card, Col, Row } from 'react-bootstrap';
import { Room } from '../types/room';
import { Link } from 'react-router-dom';

const roomImages = [
  '/images/room1.jpg',
  '/images/room2.avif',
  '/images/room3.webp',
  '/images/room4.webp',
];

interface RoomGridProps {
  rooms: Room[];
  onRoomDeleted: (roomId: number) => void;
}

export default function RoomGrid({ rooms, onRoomDeleted }: RoomGridProps) {
  return (
      <Row>
        {rooms.map((room, index) => (
            <Col md={4} key={room.roomId} className="mb-3">
              <Card className="h-100">
                <Card.Img
                    variant="top"
                    src={roomImages[index % roomImages.length]}
                    alt={`Room ${room.name}`}
                    style={{
                      height: '200px',
                      objectFit: 'cover',
                    }}
                />

                <Card.Body className="d-flex flex-column">
                  <Card.Title>{room.name}</Card.Title>

                  <Card.Text>
                    <strong>Building:</strong> {room.building}
                    <br />
                    <strong>Capacity:</strong> {room.capacity}
                    <br />
                    <strong>Description:</strong> {room.description}
                  </Card.Text>
                  <div className="mt-auto d-flex justify-content-center gap-2">
                    <Link to={`/rooms/${room.roomId}`} className="btn btn-primary mt-3">Update</Link>
                    <Button
                        variant="danger"
                        onClick={() => onRoomDeleted(room.roomId!)}>
                      Delete
                    </Button>
                  </div>
                </Card.Body>
              </Card>
            </Col>
        ))}
      </Row>
  );
}