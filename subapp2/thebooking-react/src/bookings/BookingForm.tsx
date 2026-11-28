import {useState} from 'react';
import type {SyntheticEvent} from 'react';
import {useNavigate} from 'react-router-dom';
import {useQuery} from '@tanstack/react-query';
import {Alert, Button, Form} from 'react-bootstrap';
import {Booking} from '../types/booking';
import {Room} from '../types/room';
import {getRooms} from '../services/roomService';

interface BookingFormProps {
    onBookingChanged: (newBooking: Booking) => Promise<unknown> | void;
    bookingId?: number;
    initialValues?: Partial<Booking>;
    error?: string | null;
    submitting?: boolean;
}

export default function BookingForm({
                                        onBookingChanged,
                                        bookingId,
                                        initialValues,
                                        error = null,
                                        submitting = false,
                                    }: BookingFormProps) {
    const [roomId, setRoomId] = useState<string>(initialValues?.roomId?.toString() ?? '');
    const [course, setCourse] = useState<string>(initialValues?.course ?? '');
    const [topic, setTopic] = useState<string>(initialValues?.topic ?? '');
    const [startTime, setStartTime] = useState<string>(initialValues?.startTime?.slice(0, 16) ?? '');
    const [endTime, setEndTime] = useState<string>(initialValues?.endTime?.slice(0, 16) ?? '');
    const [maxParticipants, setMaxParticipants] = useState<number>(initialValues?.maxParticipants ?? 4);
    const [isOpen, setIsOpen] = useState<boolean>(initialValues?.isOpen ?? true);
    const navigate = useNavigate();

    // Rooms for the dropdown
    const {data: rooms = [], isLoading: roomsLoading} = useQuery<Room[], Error>({
        queryKey: ['rooms'],
        queryFn: getRooms,
    });

    const onCancel = () => {
        navigate(-1);
    };

    const handleSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        const booking: Booking = {
            bookingId,
            roomId: Number(roomId),
            course,
            topic,
            startTime,
            endTime,
            isOpen,
            maxParticipants,
        };
        await onBookingChanged(booking);
    };

    return (
        <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formBookingRoom" className="mb-3">
                <Form.Label>Room</Form.Label>
                <Form.Select value={roomId} onChange={(e) => setRoomId(e.target.value)} required
                             disabled={roomsLoading}>
                    <option value="">{roomsLoading ? 'Loading rooms...' : 'Select a room'}</option>
                    {rooms.map((room) => (
                        <option key={room.roomId} value={room.roomId}>
                            {room.name} ({room.buildingName}, {room.capacity} seats)
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>

            <Form.Group controlId="formBookingCourse" className="mb-3">
                <Form.Label>Course</Form.Label>
                <Form.Control type="text" placeholder="e.g. ITPE3200" value={course}
                              onChange={(e) => setCourse(e.target.value)} maxLength={20}/>
            </Form.Group>

            <Form.Group controlId="formBookingTopic" className="mb-3">
                <Form.Label>Topic</Form.Label>
                <Form.Control type="text" placeholder="What will you work on?" value={topic}
                              onChange={(e) => setTopic(e.target.value)} required minLength={2} maxLength={100}/>
            </Form.Group>

            <Form.Group controlId="formBookingStart" className="mb-3">
                <Form.Label>Start time</Form.Label>
                <Form.Control type="datetime-local" value={startTime}
                              onChange={(e) => setStartTime(e.target.value)} required/>
            </Form.Group>

            <Form.Group controlId="formBookingEnd" className="mb-3">
                <Form.Label>End time</Form.Label>
                <Form.Control type="datetime-local" value={endTime}
                              onChange={(e) => setEndTime(e.target.value)} required/>
                <Form.Text muted>Between 08:00 and 20:00, from 30 minutes to 4 hours.</Form.Text>
            </Form.Group>

            <Form.Group controlId="formBookingMax" className="mb-3">
                <Form.Label>Max participants</Form.Label>
                <Form.Control type="number" min={1} max={20} value={maxParticipants}
                              onChange={(e) => setMaxParticipants(Number(e.target.value))} required/>
            </Form.Group>

            <Form.Group controlId="formBookingOpen" className="mb-3">
                <Form.Check type="checkbox" label="Open for other students to join"
                            checked={isOpen} onChange={(e) => setIsOpen(e.target.checked)}/>
            </Form.Group>

            {error && <Alert variant="danger">{error}</Alert>}

            <Button variant="primary" type="submit" disabled={submitting}>
                {submitting ? 'Saving...' : bookingId ? 'Update Booking' : 'Create Booking'}
            </Button>
            <Button variant="secondary" onClick={onCancel} className="ms-2" disabled={submitting}>
                Cancel
            </Button>
        </Form>
    );
}