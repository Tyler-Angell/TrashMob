import * as React from 'react';
import { Button, Col, Container, Image, Row } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import globes from './assets/gettingStarted/globes.png';

const ReadMore = ({ children }: any) => {
    const text = children;
    const [isReadMore, setIsReadMore] = React.useState(true);
    const toggleReadMore = () => {
        setIsReadMore(!isReadMore);
    };
    return (
        <>
            <p className="text">
                {isReadMore ? text.slice(0, 0) : text}
            </p>
            <p onClick={toggleReadMore} className="p-15 color-primary" role="button">
                {isReadMore ? "See more" : "See less"}
            </p>
        </>
    );
};


export const VolunteerOpportunities: React.FC = () => {

    React.useEffect(() => {
        window.scrollTo(0, 0);
    })

    return (
        <>
            <Container fluid className='bg-grass mb-5'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Recruiting</h1>
                        <p className="font-weight-bold">We’d love to have you join us.</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container>
                <Row>
                    <Col md={8} className="mb-2"><h1 className='m-0'>Open volunteer positions (4)</h1></Col>
                    <Col md={2}></Col>
                    <Col md={2} className="m-0 text-right mb-2"><Link className="btn btn-primary ml-5 py-md-3 banner-button" to="/contactus">
                        Contact us
                    </Link></Col>
                </Row>
                {/* <div className='d-flex justify-content-between align-items-center mt-5'>
                    <h1 className='m-0'>Open volunteer positions (6)</h1>
                    <Button variant="primary" className='text-center p-18 fw-600 px-3 h-49'>
                        Contact us
                    </Button>
                </div> */}
                <Row className='my-5'>
                    <Col className='mb-2' lg={4}>
                        <div className='card-shadow bg-white rounded p-4 h-100'>
                            <h3 className='font-size-xl color-primary fw-500'>
                                Looking to contribute to the growth of TrashMob.eco?
                            </h3>
                            <p className='p-18'>There are many ways to get involved in the growth of TrashMob.eco besides picking litter.</p>
                            <p className='p-18'>On this page are a few ways you can contribute from the comfort of your own home! We encourage you to reach out even if you don't have all the preferred skills.</p>
                            <p className='p-18'>If you are interested in any of these opportunities, contact us at </p>
                            <p className='p-18 color-primary'>info@trashmob.eco.</p>
                        </div>
                    </Col>
                    <Col className='mb-2' lg={8}>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>UX/UI designer</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Design for mobile app and website
                            </p>
                            <p className='p-15'>Preferred skills: Figma, Responsive Web App Design, Mobile App Design</p>
                            <ReadMore>
                                We've got a lot of features in the backlog we'd love to get done, but it all starts with design. If you have UX Design skills and want to help shape the website and mobile app as we continue to
                                grow, send us a note, and we'll put your skills to good use!
                            </ReadMore>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Web developer</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Develop website with React JS and .NETCore
                            </p>
                            <p className='p-15'>Preferred skills: ReactJS, C#, CSS, Azure Maps, AzureAD B2C, Github</p>
                            <ReadMore>
                                The backlog of features we'd like to get done on the website is huge. Some are critical to the way we do help communities get onboarded to TrashMob.eco. Others add fun components that 
                                encourage more and repeated volunteer participation. If you've got the skills to turn UX Designs done in Figma into ReactJS code, or the skills to improve the performance, 
                                security, or maintainability of the backend services, we can always use good developers who are passionate about the environment and willing to learn.
                            </ReadMore>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Mobile developer</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Develop mobile app with .NET MAUI
                            </p>
                            <p className='p-15'>Preferred skills: .NET MAUI, Mobile Deployments to Apple Store and Google Play Store, C#</p>
                            <ReadMore>
                                We're actively working on a TrashMob.eco mobile app, and could use more developers who know their way around building, testing, and deploying mobile app to the 
                                Apple and Google Play stores. We've got lots of cool features we want to add to the app, and need help getting all this done. If you have .NET MAUI skills, or want to
                                learn, this is a great opportunity for developers passionate about the environment to dig in and lend a hand!
                            </ReadMore>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Mobile product manager</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Manage mobile app development
                            </p>
                            <p className='p-15'>Preferred skills: Managing Mobile Deployments to Apple Store and Google Play Store, Mobile App Design</p>
                            <ReadMore>
                                Getting a mobile app out the door involves more than just developers checking in code. Making sure all the boxes are checked on the Apple and Google Play stores is a lot of work.
                                We're looking for someone who can drive the mobile app through the app stores, and work with the designers and devs to plot out a release strategy for more features and
                                more bells and whistles. If this is something you've done before, and you'd love to help a non-profit get it's mobile app launched, send us a note!
                            </ReadMore>
                        </div>
                    </Col>
                </Row>
            </Container>
        </>
    );
}
