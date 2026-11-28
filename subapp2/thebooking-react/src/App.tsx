import {BrowserRouter as Router, Routes, Route, Navigate} from 'react-router-dom'
import {QueryClient, QueryClientProvider} from '@tanstack/react-query'
import {ReactQueryDevtools} from '@tanstack/react-query-devtools'
import Container from 'react-bootstrap/Container'
import HomePage from './home/HomePage'
import RoomListPage from './rooms/RoomListPage'
import RoomCreatePage from './rooms/RoomCreatePage'
import BuildingListPage from './buildings/BuildingListPage'
import BuildingCreatePage from './buildings/BuildingCreatePage'
import BuildingUpdatePage from './buildings/BuildingUpdatePage'
import EquipmentListPage from './equipment/EquipmentListPage'
import EquipmentCreatePage from './equipment/EquipmentCreatePage'
import EquipmentUpdatePage from './equipment/EquipmentUpdatePage'
import NavMenu from './shared/NavMenu'
import './App.css'

const queryClient = new QueryClient() // creates the single instance


export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <Router>
                <Container>
                    <NavMenu/>
                    <Routes>
                        <Route path="/" element={<HomePage/>}/>
                        <Route path="/rooms" element={<RoomListPage/>}/>
                        <Route path="/roomcreate" element={<RoomCreatePage/>}/>
                        <Route path="/buildings" element={<BuildingListPage/>}/>
                        <Route path="/buildingcreate" element={<BuildingCreatePage/>}/>
                        <Route path="/buildingupdate/:buildingId" element={<BuildingUpdatePage/>}/>
                        <Route path="/equipment" element={<EquipmentListPage/>}/>
                        <Route path="/equipmentcreate" element={<EquipmentCreatePage/>}/>
                        <Route path="/equipmentupdate/:equipmentId" element={<EquipmentUpdatePage/>}/>
                        <Route path="*" element={<Navigate to="/" replace/>}/>
                    </Routes>
                    <footer className="text-center text-muted my-4">
                        <p>Email: Booking.no · Tel: +47 123 456 78</p>
                    </footer>
                </Container>
            </Router>
            <ReactQueryDevtools initialIsOpen={false}/>
        </QueryClientProvider>
    )
}
