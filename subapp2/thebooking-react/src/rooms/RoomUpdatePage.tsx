import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import RoomForm from './RoomForm';
import { Room } from '../types/room';
const API_URL = import.meta.env.VITE_API_URL;

async function fetchRoom(roomId: string): Promise<Room> {
    const response = await fetch(`${API_URL}/rooms/${roomId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch room data');
    }
    return response.json();
}

async function updateRoom(room: Room): Promise<Room> {
    const response = await fetch(`${API_URL}/rooms/${room.roomId}`, {
        method: 'PUT',
        headers: {'Content-Type': 'application/json',},
        body: JSON.stringify(room),
    });
    if (!response.ok) {
        throw new Error(`Failed to update room (status ${response.status})`);
    }
    return response.json();
}

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
