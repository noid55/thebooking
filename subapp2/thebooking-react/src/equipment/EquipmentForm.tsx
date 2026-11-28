import {useState} from 'react';
import type {SyntheticEvent} from 'react';
import {useNavigate} from 'react-router-dom';
import {Alert, Button, Form} from 'react-bootstrap';
import {Equipment} from '../types/equipment';

interface EquipmentFormProps {
    onEquipmentChanged: (newEquipment: Equipment) => Promise<unknown> | void;
    equipmentId?: number;
    initialValues?: Partial<Equipment>;
    error?: string | null;
    submitting?: boolean;
}

export default function EquipmentForm({
                                          onEquipmentChanged,
                                          equipmentId,
                                          initialValues,
                                          error = null,
                                          submitting = false,
                                      }: EquipmentFormProps) {
    const [name, setName] = useState<string>(initialValues?.name ?? '');
    const navigate = useNavigate();

    const onCancel = () => {
        navigate(-1);
    };

    const handleSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        const equipment: Equipment = {equipmentId, name};
        await onEquipmentChanged(equipment);
    };

    return (
        <Form onSubmit={handleSubmit}>
            <Form.Group controlId="formEquipmentName" className="mb-3">
                <Form.Label>Name</Form.Label>
                <Form.Control
                    type="text"
                    placeholder="Enter equipment name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                    minLength={2}
                    maxLength={50}
                />
            </Form.Group>

            {error && <Alert variant="danger">{error}</Alert>}

            <Button variant="primary" type="submit" disabled={submitting}>
                {submitting ? 'Saving...' : equipmentId ? 'Update Equipment' : 'Create Equipment'}
            </Button>
            <Button variant="secondary" onClick={onCancel} className="ms-2" disabled={submitting}>
                Cancel
            </Button>
        </Form>
    );
}