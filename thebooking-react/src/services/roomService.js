// Service layer: components call these functions instead of fetch directly.
// TODO: replace the mock data with fetch() calls to the API.

const MOCK_ROOMS = [
  { id: 1, name: 'Room 1', building: 'P35', capacity: 16 },
  { id: 2, name: 'Room 2', building: 'P48', capacity: 6 },
];

export async function getRooms() {
  return MOCK_ROOMS;
}
