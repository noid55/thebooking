import {Button, Table} from 'react-bootstrap';
import {Link} from 'react-router-dom';
import {Equipment} from '../types/equipment';

interface EquipmentTableProps {
    equipmentList: Equipment[];
    onEquipmentDeleted: (equipmentId: number) => void;
}

export default function EquipmentTable({equipmentList, onEquipmentDeleted}: EquipmentTableProps) {
    return (
        <Table striped bordered hover responsive>
            <thead>
            <tr>
                <th>Name</th>
                <th>Actions</th>
            </tr>
            </thead>
            <tbody>
            {equipmentList.map((equipment) => (
                <tr key={equipment.equipmentId}>
                    <td>{equipment.name}</td>
                    <td className="text-center">
                        <Link to={`/equipmentupdate/${equipment.equipmentId}`}>Update</Link>
                        <Button
                            onClick={() => onEquipmentDeleted(equipment.equipmentId!)}
                            variant="link"
                            className="text-danger"
                        >
                            Delete
                        </Button>
                    </td>
                </tr>
            ))}
            </tbody>
        </Table>
    );
}