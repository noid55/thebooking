import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { Alert, Button, Form } from 'react-bootstrap';
import { Room } from '../types/room';

interface RoomFormProps {
  onRoomChanged: (newRoom: Room) => Promise<unknown> | void;
  roomId?: number;
  initialValues?: Partial<Room>;
  error?: string | null;
  submitting?: boolean;
}

export default function RoomForm({ onRoomChanged, roomId, initialValues, error, submitting }: RoomFormProps) {
  const [name, setName] = useState<string>(initialValues?.name ?? '');
  const [building, setBuilding] = useState<string>(initialValues?.building ?? '');
  const [capacity, setCapacity] = useState<number>(initialValues?.capacity ?? 1);
  const [description, setDescription] = useState<string>(initialValues?.description ?? '');
  const navigate = useNavigate();

  const onCancel = () => {
    navigate(-1);
  };

  const handleSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    const room: Room = { roomId, name, building, capacity, description };
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
        <Form.Control
          type="text"
          placeholder="Enter building"
          value={building}
          onChange={(e) => setBuilding(e.target.value)}
          required
          pattern="[0-9a-zA-ZæøåÆØÅ. \-]{2,20}"
          title="The building must be numbers or letters and between 2 to 20 characters."
        />
      </Form.Group>

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

      <Button variant="primary" type="submit" disabled={submitting}>
        {submitting ? 'Saving...' : roomId ? 'Update Room' : 'Create Room'}
      </Button>
      <Button variant="secondary" onClick={onCancel} className="ms-2" disabled={submitting}>
        Cancel
      </Button>
    </Form>
  );
}
