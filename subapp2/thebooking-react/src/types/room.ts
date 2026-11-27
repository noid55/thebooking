
export interface Room {
    roomId?: number
    name: string
    capacity: number
    buildingId: number
    buildingName: string
    description?: string | null
}

export interface Building {
    buildingId: number
    name: string
}
