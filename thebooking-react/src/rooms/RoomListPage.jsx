import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Alert, Button, Form, Spinner } from 'react-bootstrap';
import { getRooms } from '../services/roomService';
import RoomTable from './RoomTable';
import RoomGrid from './RoomGrid';

const RoomListPage = () => {
  const [showTable, setShowTable] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');

  const {
    data: rooms = [],
    isLoading,
    error,
    refetch,
    isFetching
  } = useQuery({
    queryKey: ['rooms'],
    queryFn: getRooms,
  });

  const toggleTableOrGrid = () => {
    setShowTable((prev) => !prev);
  };

  const filteredRooms = rooms.filter((room) =>
    room.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    room.building.toLowerCase().includes(searchQuery.toLowerCase()) ||
    room.description.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div>
      <h1>Rooms</h1>

      <Form.Group className="mb-3">
        <Form.Label>Search rooms</Form.Label>
        <Form.Control
          type="text"
          placeholder="Search by name, building or description..."
          value={searchQuery}
          onChange={(event) => setSearchQuery(event.target.value)}
        />
      </Form.Group>

      <Button
        variant="primary"
        className="mb-3 me-2"
        onClick={() => refetch()}
        disabled={isFetching}
      >
        {isFetching ? 'Loading...' : 'Refresh Rooms'}
      </Button>

      <Button
        variant="secondary"
        className="mb-3"
        onClick={toggleTableOrGrid}
      >
        {showTable ? 'Show Grid' : 'Show Table'}
      </Button>

      {isLoading ? (
        <Spinner animation="border" role="status" />
      ) : error ? (
        <Alert variant="danger">
          Could not load rooms. Please try again.
        </Alert>
      ) : filteredRooms.length === 0 ? (
        <Alert variant="info">
          No rooms found.
        </Alert>
      ) : showTable ? (
        <RoomTable rooms={filteredRooms} />
      ) : (
        <RoomGrid rooms={filteredRooms} />
      )}
    </div>
  );
};

export default RoomListPage;

