import {useState} from 'react';
import type {SyntheticEvent} from 'react';
import {useNavigate} from 'react-router-dom';
import {Alert, Button, Form} from 'react-bootstrap';
import {Building} from '../types/building';

interface BuildingFormProps {
    onBuildingChanged: (newBuilding: Building) => Promise<unknown> | void;
    buildingId?: number;
    initialValues?: Partial<Building>;
    error?: string | null;
    submitting?: boolean;
}

export default function BuildingForm({
                                         onBuildingChanged,
                                         buildingId,
                                         initialValues,
                                         error = null,
                                         submitting = false,
                                     }: BuildingFormProps) {
    const [name, setName] = useState<string>(initialValues?.name ?? '');
    const [address, setAddress] = useState<string>(initialValues?.address ?? '');
    const navigate = useNavigate();

    const onCancel = () => {
        navigate(-1); // back one step in the history
    };

    const handleSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        // An empty address is sent as null, so the table shows '–'
        const building: Building = {buildingId, name, address: address || null};
        await onBuildingChanged(building);
    };

    return (
        <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formBuildingName" className="mb-3">
                <Form.Label>Name</Form.Label>
                <Form.Control
                    type="text"
                    placeholder="Enter building name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                    minLength={2}
                    maxLength={50}
                />
            </Form.Group>

            <Form.Group controlId="formBuildingAddress" className="mb-3">
                <Form.Label>Address</Form.Label>
                <Form.Control
                    type="text"
                    placeholder="Enter address"
                    value={address}
                    onChange={(e) => setAddress(e.target.value)}
                    maxLength={100}
                />
            </Form.Group>

            {error && <Alert variant="danger">{error}</Alert>}

            <Button variant="primary" type="submit" disabled={submitting}>
                {submitting ? 'Saving...' : buildingId ? 'Update Building' : 'Create Building'}
            </Button>
            <Button variant="secondary" onClick={onCancel} className="ms-2" disabled={submitting}>
                Cancel
            </Button>
        </Form>
    );
}