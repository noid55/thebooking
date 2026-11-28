import {useNavigate} from 'react-router-dom';
import {useMutation, useQueryClient} from '@tanstack/react-query';
import EquipmentForm from './EquipmentForm';
import {createEquipment} from '../services/equipmentService';
import {Equipment} from '../types/equipment';

export default function EquipmentCreatePage() {
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const mutation = useMutation<Equipment, Error, Equipment>({
        mutationFn: createEquipment,
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['equipment']});
            navigate('/equipment');
        },
    });

    return (
        <div className="py-4">
            <h1 className="mb-4">Add New Equipment</h1>
            <EquipmentForm
                onEquipmentChanged={(equipment) => mutation.mutateAsync(equipment)}
                error={mutation.error ? mutation.error.message : null}
                submitting={mutation.isPending}
            />
        </div>
    );
}