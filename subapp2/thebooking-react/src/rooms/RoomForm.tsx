
import { useEffect, useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { Alert, Button, Form } from 'react-bootstrap';
import type { Room, Building } from '../types/room';
import { getBuildings } from '../services/roomService';

interface RoomFormProps {
  onRoomChanged: (newRoom: Room) => Promise<unknown> | void;
  roomId?: number;
  initialValues?: Partial<Room>;
  error?: string | null;
  submitting?: boolean;
}

export default function RoomForm({ onRoomChanged, roomId, initialValues, error, submitting }: RoomFormProps) {
  const [name, setName] = useState<string>(initialValues?.name ?? '');
  const [buildingId, setBuildingId] = useState<number>(initialValues?.buildingId ?? 0);
  const [buildings, setBuildings] = useState<Building[]>([]);
  const [buildingError, setBuildingError] = useState<string | null>(null);
  const [capacity, setCapacity] = useState<number>(initialValues?.capacity ?? 1);
  const [description, setDescription] = useState<string>(initialValues?.description ?? '');
  const navigate = useNavigate();

  useEffect(() => {
    getBuildings()
      .then(setBuildings)
      .catch(() => setBuildingError('Could not load buildings.'));
  }, []);

  const onCancel = () => {
    navigate(-1);
  };

  const handleSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();

    const room: Room = {
      roomId,
      name,
      buildingId,
      buildingName: buildings.find(b => b.buildingId === buildingId)?.name ?? '',
      capacity,
      description
    };

    await onRoomChanged(room);
  };

  return (
    <Form onSubmit={handleSubmit}>
      <Form.Group controlId="formRoomName" className="mb-3">
        <Form.Label>Name</Form.Label>
        <Form.Control
          type="text"
          placeholder="Enter room name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          pattern="[0-9a-zA-ZæøåÆØÅ. \-]{2,20}"
          title="The name must be numbers or letters and between 2 to 20 characters."
        />
      </Form.Group>

      <Form.Group controlId="formRoomBuilding" className="mb-3">
        <Form.Label>Building</Form.Label>
        <Form.Select
          value={buildingId}
          onChange={(e) => setBuildingId(Number(e.target.value))}
          required
        >
          <option value={0}>Select a building</option>
          {buildings.map((building) => (
            <option key={building.buildingId} value={building.buildingId}>
              {building.name}
            </option>
          ))}
        </Form.Select>
      </Form.Group>

      {buildingError && <Alert variant="danger">{buildingError}</Alert>}

      <Form.Group controlId="formRoomCapacity" className="mb-3">
        <Form.Label>Capacity</Form.Label>
        <Form.Control
          type="number"
          value={capacity}
          onChange={(e) => setCapacity(Number(e.target.value))}
          required
          min="1"
          max="20"
          step="1"
        />
      </Form.Group>

      <Form.Group controlId="formRoomDescription" className="mb-3">
        <Form.Label>Description</Form.Label>
        <Form.Control
          as="textarea"
          rows={3}
          placeholder="Enter room description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          maxLength={200}
        />
      </Form.Group>

      {error && <Alert variant="danger">{error}</Alert>}

      <Button variant="primary" type="submit" disabled={submitting || buildings.length === 0}>
        {submitting ? 'Saving...' : roomId ? 'Update Room' : 'Create Room'}
      </Button>
      <Button variant="secondary" onClick={onCancel} className="ms-2" disabled={submitting}>
        Cancel
      </Button>
    </Form>
  );
}

