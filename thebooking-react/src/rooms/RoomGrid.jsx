
import { Card, Col, Row } from 'react-bootstrap';

const RoomGrid = ({ rooms }) => {
  return (
    <Row>
      {rooms.map((room) => (
        <Col md={4} key={room.roomId} className="mb-3">
          <Card>
            <Card.Body>
              <Card.Title>{room.name}</Card.Title>
              <Card.Text>
                <strong>Building:</strong> {room.building}
                <br />
                <strong>Capacity:</strong> {room.capacity}
                <br />
                <strong>Description:</strong> {room.description}
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      ))}
    </Row>
  );
};

export default RoomGrid;

