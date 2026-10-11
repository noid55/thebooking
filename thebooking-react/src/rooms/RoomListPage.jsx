import { useEffect, useState } from 'react';
import { Table } from 'react-bootstrap';
import { getRooms } from '../services/roomService';

const RoomListPage = () => {
  const [rooms, setRooms] = useState([]);

  useEffect(() => {
    getRooms().then(setRooms);
  }, []);

  return (
    <div>
      <h1>Rooms</h1>
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
    </div>
  );
};

export default RoomListPage;
