import { FC, useEffect, useState } from 'react';

import { Route, Switch } from 'react-router';
import { BrowserRouter, RouteComponentProps } from 'react-router-dom';

import Home from './components/Pages/Home';

// Layout
import TopMenu from './components/TopMenu';

import { AboutUs } from './components/Pages/AboutUs';
import ContactUs from './components/Pages/ContactUs';
import EventSummary from './components/EventSummary';
import { Faq } from './components/Faq';
import { Footer } from './components/Footer';
import { GettingStarted } from './components/Pages/GettingStarted';
import MyDashboard from './components/Pages/MyDashboard';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { TermsOfService } from './components/Pages/TermsOfService';
import { Board } from './components/Board';
import { VolunteerOpportunities } from './components/VolunteerOpportunities';
import { initializeIcons } from '@uifabric/icons';
import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { apiConfig, getDefaultHeaders, msalClient } from './store/AuthStore';
import { EventDetails, DetailsMatchParams } from './components/Pages/EventDetails';
import { NoMatch } from './components/NoMatch';
import UserData from './components/Models/UserData';
import * as msal from "@azure/msal-browser";
import { Guid } from 'guid-typescript';
import UserProfile from './components/Pages/UserProfile';
import PartnerDashboard, { PartnerDashboardMatchParams } from './components/Partners/PartnerDashboard';
import PartnerRequest from './components/Partners/PartnerRequest';
import SiteAdmin from './components/Admin/SiteAdmin';
import ManageEventDashboard, { ManageEventDashboardMatchParams } from './components/EventManagement/ManageEventDashboard';
import { Shop } from './components/Shop';
import { EventSummaries } from './components/EventSummaries';
import { CancelEvent, CancelEventMatchParams } from './components/EventManagement/CancelEvent';
import EventData from './components/Models/EventData';

import './custom.css';
import Waivers from './components/Waivers/Waivers';
import WaiversReturn from './components/Waivers/WaiversReturn';
import PartnerRequestDetails, { PartnerRequestDetailsMatchParams } from './components/Partners/PartnerRequestDetails';
import { Partnerships } from './components/Partners/Partnerships';

interface AppProps extends RouteComponentProps<ManageEventDashboardMatchParams> {
}

interface PartnerProps extends RouteComponentProps<PartnerDashboardMatchParams> {
}

interface CancelProps extends RouteComponentProps<CancelEventMatchParams> {
}

interface DetailsProps extends RouteComponentProps<DetailsMatchParams> {
}

interface PartnerRequestDetailsProps extends RouteComponentProps<PartnerRequestDetailsMatchParams> {
}

interface WaiversReturnProps extends RouteComponentProps {
}

export const App: FC = () => {
    const [isUserLoaded, setIsUserLoaded] = useState(false);
    const [currentUser, setCurrentUser] = useState<UserData>(new UserData());
    const [myAttendanceList, setMyAttendanceList] = useState<EventData[]>([]);
    const [isUserEventDataLoaded, setIsUserEventDataLoaded] = useState(false);

    useEffect(() => {
        initializeIcons();

        msalClient.addEventCallback((message: msal.EventMessage) => {
            if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
                verifyAccount(message.payload as msal.AuthenticationResult)
            }
            if (message.eventType === msal.EventType.LOGOUT_SUCCESS) {
                clearUser();
            }
        });

        var userStr = sessionStorage.getItem('user');
        if (userStr) {
            var user = JSON.parse(userStr);
            setCurrentUser(user);
            if (user.id === Guid.EMPTY) {
                setIsUserLoaded(false);
            } else {
                setIsUserLoaded(true);
            }
        }
    }, []);

    useEffect(() => {
        handleAttendanceChanged();
    },
        // eslint-disable-next-line
        [isUserLoaded]);

    function ErrorComponent(error: MsalAuthenticationResult) {
        return <p>An Error Occurred: {error}</p>;
    }

    function LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }

    function renderEditEvent(inp: AppProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <ManageEventDashboard {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderPartnerDashboard(inp: PartnerProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <PartnerDashboard {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderEventDetails(inp: DetailsProps) {
        return (
            <EventDetails {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={() => handleAttendanceChanged()} myAttendanceList={myAttendanceList} isUserEventDataLoaded={isUserEventDataLoaded} />
        );
    }

    function renderPartnerRequestDetails(inp: PartnerRequestDetailsProps) {
        return (
            <PartnerRequestDetails {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
        );
    }

    function renderEventSummary(inp: AppProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <EventSummary {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderCancelEvent(inp: CancelProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <CancelEvent {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderWaiversReturn(inp: WaiversReturnProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <WaiversReturn {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} onUserUpdated={handleUserUpdated} />
            </MsalAuthenticationTemplate >);
    }

    function clearUser() {
        setIsUserLoaded(false);
        const user = new UserData();
        setCurrentUser(user)
        sessionStorage.setItem('user', JSON.stringify(user));
    }

    function handleUserUpdated() {
        const account = msalClient.getAllAccounts()[0];

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        setIsUserLoaded(false);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Users/' + currentUser.id, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<UserData>)
                .then(data => {
                    setCurrentUser(data);
                    setIsUserLoaded(true);
                    sessionStorage.setItem('user', JSON.stringify(data));
                });
        });
    }

    function verifyAccount(result: msal.AuthenticationResult) {

        const account = msalClient.getAllAccounts()[0];

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const method = 'POST';
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
            const user = new UserData();

            user.nameIdentifier = result.idTokenClaims["sub"];
            user.sourceSystemUserName = result.account?.username ?? "";

            if (result.account?.idTokenClaims) {
                user.email = result.account?.idTokenClaims["emails"][0] ?? "";
            }

            fetch('/api/Users', {
                method: method,
                headers: headers,
                body: JSON.stringify(user)
            })
                .then(response => response.json() as Promise<UserData> | null)
                .then(data => {
                    if (data) {
                        user.id = data.id;
                        user.userName = data.userName;
                        user.givenName = data.givenName;
                        user.surName = data.surName;
                        user.dateAgreedToPrivacyPolicy = data.dateAgreedToPrivacyPolicy;
                        user.dateAgreedToTermsOfService = data.dateAgreedToTermsOfService;
                        user.dateAgreedToTrashMobWaiver = data.dateAgreedToTrashMobWaiver;
                        user.memberSince = data.memberSince;
                        user.privacyPolicyVersion = data.privacyPolicyVersion;
                        user.termsOfServiceVersion = data.termsOfServiceVersion;
                        user.trashMobWaiverVersion = data.trashMobWaiverVersion;
                        user.isSiteAdmin = data.isSiteAdmin;
                        setCurrentUser(user);
                        setIsUserLoaded(true);
                        sessionStorage.setItem('user', JSON.stringify(user));
                    }
                });
        });
    }

    function handleAttendanceChanged() {
        setMyAttendanceList([]);
        setIsUserEventDataLoaded(false);

        if (!isUserLoaded || !currentUser) {            
            return;
        }

        // If the user is logged in, get the events they are attending
        const accounts = msalClient.getAllAccounts();

        if (accounts !== null && accounts.length > 0) {
            const request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/events/eventsuserisattending/' + currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        setMyAttendanceList(data);
                        setIsUserEventDataLoaded(true);
                    })
            });
        }
    }

    return (
        <MsalProvider instance={msalClient} >
            <div className="d-flex flex-column h-100">
                <BrowserRouter>
                    <TopMenu isUserLoaded={isUserLoaded} currentUser={currentUser} />
                    <div className="container-fluid px-0">
                        <Switch>
                            <Route path="/manageeventdashboard/:eventId?" render={(props: AppProps) => renderEditEvent(props)} />
                            <Route path="/partnerdashboard/:partnerId?" render={(props: PartnerProps) => renderPartnerDashboard(props)} />
                            <Route path="/partnerrequestdetails/:partnerRequestId" render={(props: PartnerRequestDetailsProps) => renderPartnerRequestDetails(props)} />
                            <Route path="/eventsummary/:eventId?" render={(props: AppProps) => renderEventSummary(props)} />
                            <Route path="/eventdetails/:eventId" render={(props: DetailsProps) => renderEventDetails(props)} />
                            <Route path="/cancelevent/:eventId" render={(props: CancelProps) => renderCancelEvent(props)} />
                            <Route path="/waiversreturn" render={(props: WaiversReturnProps) => renderWaiversReturn(props)} />
                            <Route exact path="/mydashboard">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <MyDashboard currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/becomeapartner">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <PartnerRequest currentUser={currentUser} isUserLoaded={isUserLoaded} mode="become" />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/inviteapartner">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <PartnerRequest currentUser={currentUser} isUserLoaded={isUserLoaded} mode="send" />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/siteadmin">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <SiteAdmin currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/userprofile">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <UserProfile currentUser={currentUser} isUserLoaded={isUserLoaded} onUserUpdated={handleUserUpdated} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/waivers">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <Waivers currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/partnerships">
                                <Partnerships />
                            </Route>
                            <Route exact path="/shop">
                                <Shop />
                            </Route>
                            <Route exact path="/aboutus">
                                <AboutUs />
                            </Route>
                            <Route exact path="/board">
                                <Board />
                            </Route>
                            <Route exact path="/contactus">
                                <ContactUs />
                            </Route>
                            <Route exact path="/eventsummaries">
                                <EventSummaries />
                            </Route>
                            <Route exact path="/faq">
                                <Faq />
                            </Route>
                            <Route exact path="/gettingstarted">
                                <GettingStarted />
                            </Route>
                            <Route exact path="/privacypolicy">
                                <PrivacyPolicy />
                            </Route>
                            <Route exact path="/termsofservice">
                                <TermsOfService />
                            </Route>
                            <Route exact path="/volunteeropportunities">
                                <VolunteerOpportunities />
                            </Route>
                            <Route exact path='/'>
                                <Home currentUser={currentUser} isUserLoaded={isUserLoaded} onUserUpdated={handleUserUpdated} onAttendanceChanged={handleAttendanceChanged} myAttendanceList={myAttendanceList} isUserEventDataLoaded={isUserEventDataLoaded} />
                            </Route>
                            <Route>
                                <NoMatch />
                            </Route>
                        </Switch>
                    </div>
                    <Footer />
                </BrowserRouter>
            </div>
        </MsalProvider>
    );
}
