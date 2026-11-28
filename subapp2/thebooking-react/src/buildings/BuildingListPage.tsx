import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Alert, Button, Spinner } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { deleteBuilding, getBuildings } from '../services/buildingService';
import BuildingTable from './BuildingTable';
import { Building } from '../types/building';

export default function BuildingListPage() {
    const {
        data: buildings = [],
        isLoading,
        error,
        refetch,
        isFetching
    } = useQuery<Building[], Error>({
        queryKey: ['buildings'],
        queryFn: getBuildings,
    });

    const queryClient = useQueryClient();

    const deleteMutation = useMutation<void, Error, number>({
        mutationFn: deleteBuilding,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['buildings'] });
        },
    });

    // Page-level logic: decides what happens when a building is deleted
    const handleBuildingDeleted = (buildingId: number) => {
        const confirmDelete = window.confirm(`Are you sure you want to delete building ${buildingId}?`);
        if (confirmDelete) {
            deleteMutation.mutate(buildingId);
        }
    };
    
    return (
        <div className="py-4">
            <h1 className="mb-4">Buildings</h1>

            <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
                <Button
                    variant="outline-primary"
                    size="sm"
                    onClick={() => refetch()}
                    disabled={isFetching}
                >
                    {isFetching ? 'Loading...' : 'Refresh Buildings'}
                </Button>

                <Link to="/buildingcreate" className="btn btn-primary btn-sm">
                    Add New Building
                </Link>
            </div>

            {deleteMutation.error && (
                <Alert variant="danger" dismissible onClose={() => deleteMutation.reset()}>
                    {deleteMutation.error.message}
                </Alert>
            )}

            {isLoading ? (
                <div className="text-center py-4">
                    <Spinner animation="border" size="sm" role="status" />
                </div>
            ) : error ? (
                <Alert variant="danger">Could not load buildings. Please try again.</Alert>
            ) : buildings.length === 0 ? (
                <Alert variant="info">No buildings found.</Alert>
            ) : (
                <BuildingTable buildings={buildings} onBuildingDeleted={handleBuildingDeleted} />
            )}
        </div>
    );
}