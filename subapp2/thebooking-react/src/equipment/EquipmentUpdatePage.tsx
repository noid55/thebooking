import {useNavigate, useParams} from 'react-router-dom';
import {useQuery, useMutation, useQueryClient} from '@tanstack/react-query';
import EquipmentForm from './EquipmentForm';
import {getEquipment, updateEquipment} from '../services/equipmentService';
import {Equipment} from '../types/equipment';

export default function EquipmentUpdatePage() {
    const {equipmentId} = useParams<{ equipmentId: string }>();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const equipmentQuery = useQuery<Equipment, Error>({
        queryKey: ['equipment', equipmentId],
        queryFn: () => getEquipment(equipmentId!),
        enabled: !!equipmentId,
    });

    const mutation = useMutation<Equipment, Error, Equipment>({
        mutationFn: updateEquipment,
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['equipment']});
            navigate('/equipment');
        },
    });

    if (equipmentQuery.isLoading) return <p>Loading...</p>;
    if (equipmentQuery.isError) return <p>Failed to load equipment</p>;
    if (!equipmentQuery.data) return <p>No equipment found</p>;

    return (
        <div className="py-4">
            <h1 className="mb-4">Update Equipment</h1>
            <EquipmentForm
                equipmentId={equipmentQuery.data.equipmentId}
                initialValues={equipmentQuery.data}
                onEquipmentChanged={(equipment) => mutation.mutateAsync(equipment)}
                error={mutation.error ? mutation.error.message : null}
                submitting={mutation.isPending}
            />
        </div>
    );
}