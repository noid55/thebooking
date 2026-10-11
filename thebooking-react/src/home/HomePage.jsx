import { Link } from 'react-router-dom';
import { Button } from 'react-bootstrap';

const HomePage = () => {
  return (
    <div className="text-center my-5">
      <h1 className="display-4">Room booking</h1>
      <p className="lead">Find a study room and book it for your group.</p>
      <Button as={Link} to="/rooms" variant="primary">Browse rooms</Button>
    </div>
  );
};

export default HomePage;
