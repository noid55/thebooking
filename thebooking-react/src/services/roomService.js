// Service layer: components call these functions instead of fetch directly.
const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5234';

export async function getRooms() {
  const response = await fetch(`${API_URL}/api/roomapi/roomlist`);
  if (!response.ok) {
    throw new Error(`Fetch failed with status ${response.status}`);
  }
  return response.json();
}
