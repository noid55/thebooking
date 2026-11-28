export interface Booking {
    bookingId?: number
    roomId: number
    roomName?: string | null
    course: string
    topic: string
    startTime: string   // e.g. "2026-12-01T10:00"
    endTime: string
    isOpen: boolean
    maxParticipants: number
    status?: string     // "Confirmed" | "Cancelled"
}