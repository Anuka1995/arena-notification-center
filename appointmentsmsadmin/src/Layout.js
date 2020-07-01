import React, { useState } from 'react';
import { Container } from 'reactstrap';
import classNames from 'classnames';
import { NotificationContainer } from 'react-notifications';

import SideBar from './Components/SidePanel/SideBar';
import NavBar from './Components/Content/NavBar';

import 'react-notifications/lib/notifications.css';
import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"

const Layout = props => {

    const [isOpen] = useState(true);

    return (
        <div className="App">
            <div className="App wrapper">
                <SideBar isOpen={isOpen} />
                <Container className={classNames('content', { 'is-open': isOpen })} 
                style={{ maxHeight: '100vh', maxWidth: 'initial', overflowY: 'auto'}}>
                    <NavBar toggle={props.toggle} />
                    {props.children}

                </Container>
                <NotificationContainer />
            </div>
        </div>
    );
}

export default Layout;