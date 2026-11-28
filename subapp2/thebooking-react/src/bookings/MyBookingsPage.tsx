import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';
import {Alert, Spinner} from 'react-bootstrap';
import {Link} from 'react-router-dom';
import {deleteBooking, getMyBookings} from '../services/bookingService';
import BookingTable from './BookingTable';
import {Booking} from '../types/booking';

export default function MyBookingsPage() {
    const {data: bookings = [], isLoading, error} = useQuery<Booking[], Error>({
        queryKey: ['bookings'],
        queryFn: getMyBookings,
    });

    const queryClient = useQueryClient();

    const deleteMutation = useMutation<void, Error, number>({
        mutationFn: deleteBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['bookings']});
        },
    });

    const handleBookingDeleted = (bookingId: number) => {
        if (window.confirm(`Are you sure you want to delete booking ${bookingId}?`)) {
            deleteMutation.mutate(bookingId);
        }
    };

    return (
        <div className="py-4">
            <h1 className="mb-4">My bookings</h1>

            <div className="d-flex justify-content-end mb-3">
                <Link to="/bookingcreate" className="btn btn-primary btn-sm">Book a room</Link>
            </div>

            {deleteMutation.error && (
                <Alert variant="danger" dismissible onClose={() => deleteMutation.reset()}>
                    {deleteMutation.error.message}
                </Alert>
            )}

            {isLoading ? (
                <div className="text-center py-4">
                    <Spinner animation="border" size="sm" role="status"/>
                </div>
            ) : error ? (
                <Alert variant="danger">Could not load your bookings. Please try again.</Alert>
            ) : bookings.length === 0 ? (
                <Alert variant="info">You have no bookings yet.</Alert>
            ) : (
                <BookingTable bookings={bookings} onBookingDeleted={handleBookingDeleted}/>
            )}
        </div>
    );
}