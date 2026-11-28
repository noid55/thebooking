import {Booking} from '../types/booking';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5234';

export async function getMyBookings(): Promise<Booking[]> {
    const response = await fetch(`${API_URL}/api/bookingapi/mybookings`);
    if (!response.ok) {
        throw new Error(`Fetch failed with status ${response.status}`);
    }
    return response.json();
}

export async function getBooking(bookingId: string): Promise<Booking> {
    const response = await fetch(`${API_URL}/api/bookingapi/${bookingId}`);
    if (!response.ok) {
        throw new Error('Failed to fetch booking');
    }
    return response.json();
}

export async function createBooking(newBooking: Booking): Promise<Booking> {
    const response = await fetch(`${API_URL}/api/bookingapi/create`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(newBooking),
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
    return response.json();
}

export async function updateBooking(booking: Booking): Promise<Booking> {
    const response = await fetch(`${API_URL}/api/bookingapi/update/${booking.bookingId}`, {
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(booking),
    });
    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }
    return response.json();
}

export async function deleteBooking(bookingId: number): Promise<void> {
    const response = await fetch(`${API_URL}/api/bookingapi/delete/${bookingId}`, {
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