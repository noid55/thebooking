import {Badge, Button, Table} from 'react-bootstrap';
import {Link} from 'react-router-dom';
import {Booking} from '../types/booking';

interface BookingTableProps {
    bookings: Booking[];
    onBookingDeleted: (bookingId: number) => void;
}

const formatDate = (s: string) => new Date(s).toLocaleDateString('nb-NO');
const formatTime = (s: string) =>
    new Date(s).toLocaleTimeString('nb-NO', {hour: '2-digit', minute: '2-digit'});

export default function BookingTable({bookings, onBookingDeleted}: BookingTableProps) {
    return (
        <Table striped bordered hover responsive>
            <thead>
            <tr>
                <th>Room</th>
                <th>Course</th>
                <th>Topic</th>
                <th>Date</th>
                <th>Time</th>
                <th>Status</th>
                <th>Actions</th>
            </tr>
            </thead>
            <tbody>
            {bookings.map((booking) => (
                <tr key={booking.bookingId}>
                    <td>{booking.roomName ?? '–'}</td>
                    <td>{booking.course || '–'}</td>
                    <td>{booking.topic}</td>
                    <td>{formatDate(booking.startTime)}</td>
                    <td>{formatTime(booking.startTime)}–{formatTime(booking.endTime)}</td>
                    <td>
                        <Badge bg={booking.status === 'Confirmed' ? 'success' : 'secondary'}>
                            {booking.status}
                        </Badge>{' '}
                        {booking.isOpen ? 'Open' : 'Private'}
                    </td>
                    <td className="text-center">
                        {booking.status === 'Confirmed' && (
                            <Link to={`/bookingupdate/${booking.bookingId}`}>Update</Link>
                        )}
                        <Button onClick={() => onBookingDeleted(booking.bookingId!)}
                                variant="link" className="text-danger">
                            Delete
                        </Button>
                    </td>
                </tr>
            ))}
            </tbody>
        </Table>
    );
}