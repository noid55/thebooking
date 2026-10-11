import { useEffect, useState } from 'react';
import { Alert, Spinner, Table } from 'react-bootstrap';
import { getRooms } from '../services/roomService';

const RoomListPage = () => {
  const [rooms, setRooms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getRooms()
      .then(setRooms)
      .catch(() => setError('Could not load rooms. Please try again.'))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div>
      <h1>Rooms</h1>

      {loading ? (
        <Spinner animation="border" role="status" />
      ) : error ? (
        <Alert variant="danger">{error}</Alert>
      ) : rooms.length === 0 ? (
        <Alert variant="info">No rooms found.</Alert>
      ) : (
        <Table striped bordered hover>
          <thead>
            <tr>
              <th>Name</th>
              <th>Building</th>
              <th>Capacity</th>
            </tr>
          </thead>
          <tbody>
            {rooms.map((room) => (
              <tr key={room.id}>
                <td>{room.name}</td>
                <td>{room.building}</td>
                <td>{room.capacity}</td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </div>
  );
};

export default RoomListPage;
