import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import RoomForm from './RoomForm';
import { createRoom } from '../services/roomService';
import { Room } from '../types/room';

export default function RoomCreatePage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const mutation = useMutation<Room, Error, Room>({
    mutationFn: createRoom,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['rooms'] });
      navigate('/rooms');
    },
  });

  return (
    <div className="py-4">
      <h1 className="mb-4">Add New Room</h1>
      <RoomForm
        onRoomChanged={(room) => mutation.mutateAsync(room)}
        error={mutation.error ? mutation.error.message : null}
        submitting={mutation.isPending}
      />
    </div>
  );
}
