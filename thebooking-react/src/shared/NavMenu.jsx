import { Link, NavLink } from 'react-router-dom';
import { Nav, Navbar } from 'react-bootstrap';

const NavMenu = () => {
  return (
    <Navbar expand="lg">
      <Navbar.Brand as={Link} to="/">TheBooking</Navbar.Brand>
      <Navbar.Toggle aria-controls="basic-navbar-nav" />
      <Navbar.Collapse id="basic-navbar-nav">
        <Nav className="me-auto">
          <Nav.Link as={NavLink} to="/" end>Home</Nav.Link>
          <Nav.Link as={NavLink} to="/rooms">Rooms</Nav.Link>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default NavMenu;
