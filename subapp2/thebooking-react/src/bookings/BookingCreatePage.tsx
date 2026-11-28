import {useNavigate} from 'react-router-dom';
import {useMutation, useQueryClient} from '@tanstack/react-query';
import BookingForm from './BookingForm';
import {createBooking} from '../services/bookingService';
import {Booking} from '../types/booking';

export default function BookingCreatePage() {
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const mutation = useMutation<Booking, Error, Booking>({
        mutationFn: createBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['bookings']});
            navigate('/mybookings');
        },
    });

    return (
        <div className="py-4">
            <h1 className="mb-4">Book a room</h1>
            <BookingForm
                onBookingChanged={(booking) => mutation.mutateAsync(booking)}
                error={mutation.error ? mutation.error.message : null}
                submitting={mutation.isPending}
            />
        </div>
    );
}