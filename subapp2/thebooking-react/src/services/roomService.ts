import { Room } from '../types/room';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5234';

export async function getRooms(): Promise<Room[]> {
  const response = await fetch(`${API_URL}/api/roomapi/roomlist`);
  if (!response.ok) {
    throw new Error(`Fetch failed with status ${response.status}`);
  }
  return response.json();
}

export async function createRoom(newRoom: Room): Promise<Room> {
  const response = await fetch(`${API_URL}/api/roomapi/create`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(newRoom),
  });
  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }
  return response.json();
}

// ASP.NET returns validation errors as { errors: { Field: ["message"] } }
async function readErrorMessage(response: Response): Promise<string> {
  try {
    const body = await response.json();
    if (body?.errors) {
      return Object.values<string[]>(body.errors).flat().join(' ');
    }
    if (typeof body === 'string') return body;
  } catch {
    // not JSON: use the generic message
  }
  return `Failed to create room (status ${response.status})`;
}
