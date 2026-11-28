import { useNavigate, useParams } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import BuildingForm from './BuildingForm';
import { getBuilding, updateBuilding } from '../services/buildingService';
import { Building } from '../types/building';

export default function BuildingUpdatePage() {
    const { buildingId } = useParams<{ buildingId: string }>();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const buildingQuery = useQuery<Building, Error>({
        queryKey: ['buildings', buildingId],
        queryFn: () => getBuilding(buildingId!),
        enabled: !!buildingId,
    });

    const mutation = useMutation<Building, Error, Building>({
        mutationFn: updateBuilding,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['buildings'] });
            navigate('/buildings');
        },
    });

    if (buildingQuery.isLoading) return <p>Loading...</p>;
    if (buildingQuery.isError) return <p>Failed to load building</p>;
    if (!buildingQuery.data) return <p>No building found</p>;

    return (
        <div className="py-4">
            <h1 className="mb-4">Update Building</h1>
            <BuildingForm
                buildingId={buildingQuery.data.buildingId}
                initialValues={buildingQuery.data}
                onBuildingChanged={(building) => mutation.mutateAsync(building)}
                error={mutation.error ? mutation.error.message : null}
                submitting={mutation.isPending}
            />
        </div>
    );
}