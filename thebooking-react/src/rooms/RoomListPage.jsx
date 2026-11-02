
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
    <div className="py-4">
      {/* Heading and search */}
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
        <h1 className="mb-0">Rooms</h1>

        <Form.Group style={{ width: '300px', maxWidth: '100%' }}>
          <Form.Control
            size="sm"
            type="text"
            placeholder="Search rooms..."
            value={searchQuery}
            onChange={(event) => setSearchQuery(event.target.value)}
          />
        </Form.Group>
      </div>

      {/* Main action buttons */}
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
        <Button
          variant="outline-primary"
          size="sm"
          onClick={() => refetch()}
          disabled={isFetching}
        >
          {isFetching ? 'Loading...' : 'Refresh Rooms'}
        </Button>

        <Button
          variant="secondary"
          size="sm"
          onClick={toggleTableOrGrid}
        >
          {showTable ? 'Show Grid' : 'Show Table'}
        </Button>
      </div>

      {/* Room content */}
      {isLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" role="status" />
        </div>
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

