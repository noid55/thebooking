import { Nav, Navbar, NavDropdown } from 'react-bootstrap';

const NavMenu = () => {
  return (
    <Navbar expand="lg">
      <Navbar.Brand href="/">MyShop</Navbar.Brand>
      <Navbar.Toggle aria-controls="basic-navbar-nav" />
      <Navbar.Collapse id="basic-navbar-nav">
        <Nav className="me-auto">
          <Nav.Link href="/">Home</Nav.Link>
          <Nav.Link href="/items">Items</Nav.Link>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default NavMenu;
