import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import Container from 'react-bootstrap/Container'
import HomePage from './home/HomePage'
import RoomListPage from './rooms/RoomListPage'
import NavMenu from './shared/NavMenu'
import './App.css'

function App() {
  return (
    <Router>
      <Container>
        <NavMenu />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/rooms" element={<RoomListPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
        <footer className="text-center text-muted my-4">
          <p>Email: Booking.no · Tel: +47 123 456 78</p>
        </footer>
      </Container>
    </Router>
  )
}

export default App
