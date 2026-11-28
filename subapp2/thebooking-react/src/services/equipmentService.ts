import {Equipment} from '../types/equipment';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5234';

export async function getEquipmentList(): Promise<Equipment[]> {
    const response = await fetch(`${API_URL}/api/equipmentapi/equipmentlist`);
    if (!response.ok) {
        throw new Error(`Fetch failed with status ${response.status}`);
    }
    return response.json();
}

export async function createEquipment(newEquipment: Equipment): Promise<Equipment> {
    const response = await fetch(`${API_URL}/api/equipmentapi/create`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(newEquipment),
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
    return response.json();
}

export async function getEquipment(equipmentId: string): Promise<Equipment> {
    const response = await fetch(`${API_URL}/api/equipmentapi/${equipmentId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch equipment');
    }
    return response.json();
}

export async function updateEquipment(equipment: Equipment): Promise<Equipment> {
    const response = await fetch(`${API_URL}/api/equipmentapi/update/${equipment.equipmentId}`, {
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(equipment),
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
    return response.json();
}

export async function deleteEquipment(equipmentId: number): Promise<void> {
    const response = await fetch(`${API_URL}/api/equipmentapi/delete/${equipmentId}`, {
        method: 'DELETE',
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
}

// Same helper as in buildingService.ts
async function readErrorMessage(response: Response): Promise<string> {
    const text = await response.text();
    try {
        const body = JSON.parse(text);
        if (body?.errors) {
            return Object.values<string[]>(body.errors).flat().join(' ');
        }
    } catch {
        if (text) return text;
    }
    return `Request failed (status ${response.status})`;
}