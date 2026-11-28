import {useNavigate, useParams} from 'react-router-dom';
import {useQuery, useMutation, useQueryClient} from '@tanstack/react-query';
import BookingForm from './BookingForm';
import {getBooking, updateBooking} from '../services/bookingService';
import {Booking} from '../types/booking';

export default function BookingUpdatePage() {
    const {bookingId} = useParams<{ bookingId: string }>();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const bookingQuery = useQuery<Booking, Error>({
        queryKey: ['bookings', bookingId],
        queryFn: () => getBooking(bookingId!),
        enabled: !!bookingId,
    });

    const mutation = useMutation<Booking, Error, Booking>({
        mutationFn: updateBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['bookings']});
            navigate('/mybookings');
        },
    });

    if (bookingQuery.isLoading) return <p>Loading...</p>;
    if (bookingQuery.isError) return <p>Failed to load booking</p>;
    if (!bookingQuery.data) return <p>No booking found</p>;

    return (
        <div className="py-4">
            <h1 className="mb-4">Update Booking</h1>
            <BookingForm
                bookingId={bookingQuery.data.bookingId}
                initialValues={bookingQuery.data}
                onBookingChanged={(booking) => mutation.mutateAsync(booking)}
                error={mutation.error ? mutation.error.message : null}
                submitting={mutation.isPending}
            />
        </div>
    );
}