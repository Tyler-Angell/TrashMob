import * as React from 'react'

import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { Col, Container, Image, Row } from 'react-bootstrap';
import { EditEvent } from './EditEvent';
import { ManageEventPartners } from './ManageEventPartners';
import { ManageEventAttendees } from './ManageEventAttendees';
import { Guid } from 'guid-typescript';
import globes from '../assets/gettingStarted/globes.png';

export interface ManageEventDashboardMatchParams {
    eventId?: string;
}

export interface ManageEventDashboardProps extends RouteComponentProps<ManageEventDashboardMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const ManageEventDashboard: React.FC<ManageEventDashboardProps> = (props) => {
    const [eventId, setEventId] = React.useState<string>("");
    const [isEventIdReady, setIsEventIdReady] = React.useState<boolean>();
    const [loadedEventId, setLoadedEventId] = React.useState<string | undefined>(props.match?.params["eventId"]);

    React.useEffect(() => {
        var evId = loadedEventId;
        if (!evId) {
            setEventId(Guid.createEmpty().toString());
            setLoadedEventId(Guid.createEmpty().toString())
        }
        else {
            setEventId(evId);
        }

        setIsEventIdReady(true);
    }, [loadedEventId]);

    function handleEditCancel() {
        props.history.push("/mydashboard");
    }

    function handleEditSave() {
        props.history.push("/mydashboard");
    }


    function renderEventDashboard() {
        return (
            <>
                <EditEvent eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onEditCancel={handleEditCancel} onEditSave={handleEditSave} history={props.history} location={props.location} match={props.match} />
                <ManageEventAttendees eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
                <ManageEventPartners eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </>
        );
    }

    let contents = isEventIdReady
        ? renderEventDashboard()
        : <p><em>Loading...</em></p>;

    return <div>
        <Container fluid className='bg-grass'>
            <Row className="text-center pt-0">
                <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                    <h1 className='font-weight-bold'>Manage Event</h1>
                    <p className="font-weight-bold">We can’t wait to see the results.</p>
                </Col>
                <Col md={5}>
                    <Image src={globes} alt="globes" className="h-100 mt-0" />
                </Col>
            </Row>
        </Container>
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Manage Event</h2>
                        <p>
                            This page allows you to create a new event or edit an existing event. You can set the name, time, and location for the event, and then request services from TrashMob.eco Partners.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {contents}
                    </div>
                </Col>
            </Row>
        </Container>
    </div>;
}

export default withRouter(ManageEventDashboard);
