import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import BuildingForm from './BuildingForm';
import { createBuilding } from '../services/buildingService';
import { Building } from '../types/building';

export default function BuildingCreatePage() {
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const mutation = useMutation<Building, Error, Building>({
        mutationFn: createBuilding,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['buildings'] });
            navigate('/buildings');
        },
    });

    return (
        <div className="py-4">
            <h1 className="mb-4">Add New Building</h1>
            <BuildingForm
                onBuildingChanged={(building) => mutation.mutateAsync(building)}
                error={mutation.error ? mutation.error.message : null}
                submitting={mutation.isPending}
            />
        </div>
    );
}