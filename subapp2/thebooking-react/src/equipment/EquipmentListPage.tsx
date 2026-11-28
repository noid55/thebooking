import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';
import {Alert, Button, Spinner} from 'react-bootstrap';
import {Link} from 'react-router-dom';
import {deleteEquipment, getEquipmentList} from '../services/equipmentService';
import EquipmentTable from './EquipmentTable';
import {Equipment} from '../types/equipment';

export default function EquipmentListPage() {
    const {
        data: equipmentList = [],
        isLoading,
        error,
        refetch,
        isFetching
    } = useQuery<Equipment[], Error>({
        queryKey: ['equipment'],
        queryFn: getEquipmentList,
    });

    const queryClient = useQueryClient();

    const deleteMutation = useMutation<void, Error, number>({
        mutationFn: deleteEquipment,
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['equipment']});
        },
    });

    const handleEquipmentDeleted = (equipmentId: number) => {
        const confirmDelete = window.confirm(`Are you sure you want to delete equipment ${equipmentId}?`);
        if (confirmDelete) {
            deleteMutation.mutate(equipmentId);
        }
    };

    return (
        <div className="py-4">
            <h1 className="mb-4">Equipment</h1>

            <div className="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3">
                <Button variant="outline-primary" size="sm" onClick={() => refetch()} disabled={isFetching}>
                    {isFetching ? 'Loading...' : 'Refresh Equipment'}
                </Button>

                <Link to="/equipmentcreate" className="btn btn-primary btn-sm">
                    Add New Equipment
                </Link>
            </div>

            {deleteMutation.error && (
                <Alert variant="danger" dismissible onClose={() => deleteMutation.reset()}>
                    {deleteMutation.error.message}
                </Alert>
            )}

            {isLoading ? (
                <div className="text-center py-4">
                    <Spinner animation="border" size="sm" role="status"/>
                </div>
            ) : error ? (
                <Alert variant="danger">Could not load equipment. Please try again.</Alert>
            ) : equipmentList.length === 0 ? (
                <Alert variant="info">No equipment found.</Alert>
            ) : (
                <EquipmentTable equipmentList={equipmentList} onEquipmentDeleted={handleEquipmentDeleted}/>
            )}
        </div>
    );
}