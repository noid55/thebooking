export interface Room {
    roomId?: number
    name: string
    capacity: number
    buildingId: number
    buildingName?: string | null
    description?: string | null
}