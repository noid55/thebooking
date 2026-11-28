import {Building} from '../types/building';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5234';

export async function getBuildings(): Promise<Building[]> {
    const response = await fetch(`${API_URL}/api/buildingapi/buildinglist`);
    if (!response.ok) {
        throw new Error(`Fetch failed with status ${response.status}`);
    }
    return response.json();
}


export async function createBuilding(newBuilding: Building): Promise<Building> {
    const response = await fetch(`${API_URL}/api/buildingapi/create`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(newBuilding),
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
    return response.json();
}

export async function getBuilding(buildingId: string): Promise<Building> {
    const response = await fetch(`${API_URL}/api/buildingapi/${buildingId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch building');
    }
    return response.json();
}

export async function updateBuilding(building: Building): Promise<Building> {
    const response = await fetch(`${API_URL}/api/buildingapi/update/${building.buildingId}`, {
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(building),
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
    return response.json();
}

export async function deleteBuilding(buildingId: number): Promise<void> {
    const response = await fetch(`${API_URL}/api/buildingapi/delete/${buildingId}`, {
        method: 'DELETE',
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
}

// Reads the error message from the API. Validation errors come as JSON
// ({ errors: { Field: ["message"] } }), while e.g. Conflict("...") and
// NotFound("...") come as plain text, so we read the body as text first.
async function readErrorMessage(response: Response): Promise<string> {
    const text = await response.text();
    try {
        const body = JSON.parse(text);
        if (body?.errors) {
            return Object.values<string[]>(body.errors).flat().join(' ');
        }
    } catch {
        // not JSON: the plain-text message is the error
        if (text) return text;
    }
    return `Request failed (status ${response.status})`;
}