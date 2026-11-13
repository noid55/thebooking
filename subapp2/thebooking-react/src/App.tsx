import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import Container from 'react-bootstrap/Container'
import HomePage from './home/HomePage'
import RoomListPage from './rooms/RoomListPage'
import RoomCreatePage from './rooms/RoomCreatePage'
import NavMenu from './shared/NavMenu'
import './App.css'

const queryClient = new QueryClient() // creates the single instance

export default function App() {
  return (
      <QueryClientProvider client={queryClient}>
        <Router>
          <Container>
            <NavMenu />
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/rooms" element={<RoomListPage />} />
              <Route path="/roomcreate" element={<RoomCreatePage />} />
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
            <footer className="text-center text-muted my-4">
              <p>Email: Booking.no · Tel: +47 123 456 78</p>
            </footer>
          </Container>
        </Router>
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
  )
}
