import { Link } from 'react-router-dom';
import { Carousel } from 'react-bootstrap';

export default function HomePage() {
  return (
      <div className="text-center my-5">
        <h1 className="display-4">Room booking</h1>

        <p className="lead">
          Find a study room and book it for your group.
        </p>

        <Carousel className="mb-4">
          <Carousel.Item>
            <img
                src="/images/room1.jpg"
                alt="Meeting room"
                className="d-block w-100"
                style={{ height: '350px', objectFit: 'cover' }}
            />
            <Carousel.Caption>
              <h3>Find your perfect room</h3>
            </Carousel.Caption>
          </Carousel.Item>

          <Carousel.Item>
            <img
                src="/images/room2.avif"
                alt="Study room"
                className="d-block w-100"
                style={{ height: '350px', objectFit: 'cover' }}
            />
            <Carousel.Caption>
              <h3>Study together</h3>
            </Carousel.Caption>
          </Carousel.Item>

          <Carousel.Item>
            <img
                src="/images/room3.webp"
                alt="Group meeting room"
                className="d-block w-100"
                style={{ height: '350px', objectFit: 'cover' }}
            />
            <Carousel.Caption>
              <h3>Make room for collaboration</h3>
            </Carousel.Caption>
          </Carousel.Item>

          <Carousel.Item>
            <img
                src="/images/room4.webp"
                alt="Meeting space"
                className="d-block w-100"
                style={{ height: '350px', objectFit: 'cover' }}
            />
            <Carousel.Caption>
              <h3>Book your next meeting</h3>
            </Carousel.Caption>
          </Carousel.Item>
        </Carousel>

        <Link to="/rooms" className="btn btn-primary">Browse rooms</Link>
      </div>
  );
}
