import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import RoomForm from './RoomForm';
import { Room } from '../types/room';
import { fetchRoom, updateRoom } from '../services/roomService';

export default function RoomUpdatePage() {
    const { roomId } = useParams<{ roomId: string }>();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const roomQuery = useQuery({
        queryKey: ['rooms', roomId],
        queryFn: () => fetchRoom(roomId!),
        enabled: !!roomId,
    });

    const mutation = useMutation({
        mutationFn: updateRoom,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['rooms'] });
            navigate(`/rooms`);
        },
    });

    if (roomQuery.isLoading) return <p>Loading...</p>;
    if (roomQuery.isError) return <p>Failed to load room</p>;
    if (!roomQuery.data) return <p>Room not found</p>;

   return (
        <div>
            <h2>Update Room</h2>
            <RoomForm
                onRoomChanged={(room: Room) => mutation.mutate(room)}
                roomId={roomQuery.data.roomId}
                initialValues={roomQuery.data}
                error={mutation.error?.message}
                submitting={mutation.isPending}
            />
        </div>
    );
}
