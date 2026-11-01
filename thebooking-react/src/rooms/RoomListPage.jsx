import { useQuery } from '@tanstack/react-query';
import { Alert, Button, Spinner, Table } from 'react-bootstrap';
import { getRooms } from '../services/roomService';

const RoomListPage = () => {
  const { data: rooms = [], isLoading, error, refetch, isFetching } = useQuery({
    queryKey: ['rooms'],
    queryFn: getRooms,
  });

  return (
    <div>
      <h1>Rooms</h1>
      <Button
        variant="primary"
        className="mb-3"
        onClick={() => refetch()}
        disabled={isFetching}
      >
        {isFetching ? 'Loading...' : 'Refresh Rooms'}
      </Button>

      {isLoading ? (
        <Spinner animation="border" role="status" />
      ) : error ? (
        <Alert variant="danger">Could not load rooms. Please try again.</Alert>
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
              <tr key={room.roomId}>
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
