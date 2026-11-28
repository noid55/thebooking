import { Button, Table } from 'react-bootstrap';
import { Building } from '../types/building';
import { Link } from 'react-router-dom';

interface BuildingTableProps {
    buildings: Building[];
    onBuildingDeleted: (buildingId: number) => void;
}

export default function BuildingTable({ buildings, onBuildingDeleted }: BuildingTableProps) {
    return (
        <Table striped bordered hover responsive>
            <thead>
            <tr>
                <th>Name</th>
                <th>Address</th>
                <th>Actions</th>
            </tr>
            </thead>
            <tbody>
            {buildings.map((building) => (
                <tr key={building.buildingId}>
                    <td>{building.name}</td>
                    <td>{building.address ?? '–'}</td>
                    <td className="text-center">
                        <Link to={`/buildingupdate/${building.buildingId}`}>Update</Link>
                        <Button
                            onClick={() => onBuildingDeleted(building.buildingId!)}
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