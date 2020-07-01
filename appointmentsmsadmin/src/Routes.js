import React, { useEffect } from 'react';
import { BrowserRouter, useHistory } from 'react-router-dom';
import { Switch, Route, Redirect } from 'react-router-dom';

import Login from './Components/Login/Login';
import Overview from './Components/Overview/Overview';
import AddRuleSet from './Components/RuleSet/AddRuleSet';
import ViewRuleSet from './Components/RuleSet/ViewRuleSet';
import AddTextTemplate from './Components/TextTemplate/AddTextTemplate';
import ViewTags from './Components/Tags/ViewTags';
import SaveTagPage from './Components/Tags/SaveTagPage';
import AddGroupTemplate from './Components/GroupTemplates/AddGroupTemplate';
import SearchGroupTemplate from './Components/GroupTemplates/SearchGroupTemplate';
import Layout from './Layout';
import SearchTextTemplate from './Components/TextTemplate/SearchTextTemplate';
import FilterTextTemplate from './Components/TextTemplate/FilterTextTemplate';
import PSSLink from './Components/Configuration/PSSLink'
import TimeFormat from './Components/Configuration/TimeFormat';
import { PokeSession } from './Services/AuthService';


function TryPoke() {
    // Do a POKE and update the ticket
    PokeSession().then(session => {
        localStorage.setItem("token", session.token);
        localStorage.setItem("loggedInData", session.validTo);
    }).catch(err => {
        console.log("ERROR in Poke request: " + err);
        localStorage.removeItem('token');
        localStorage.removeItem('loggedInData');
    });
}

function CheckLogOnStatus() {
    let token = localStorage.getItem('token');
    let validTime = localStorage.getItem('loggedInData');

    if (token && validTime) {
        TryPoke();
        return true;
    } else {
        return false;
    }
}

const PrivateRoute = ({ component: Component, ...rest }) => {

    const history = useHistory();

    useEffect(() => {
        const timer = setTimeout(() => {
            history.push('/Login', { directLogin: false });
        }, (1000 * 60 * 15));
        return () => clearTimeout(timer);
      }, []);

    return (
        <Route {...rest} render={(props) => (
            (CheckLogOnStatus())
                ? (<Layout><Component {...props} /></Layout>)
                : (<Redirect to={{
                    pathname: '/login',
                    state: { from: props.location }
                }} />)
        )} />);
}

const Routes = () => {
    console.log('Application running in ' + process.env.NODE_ENV + ' mode');
    let basePath = "";

    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
        basePath = "/"
    } else {
        basePath = "/DIPS-SMS/AppointmentSMSClient"
    }

    return (
        <BrowserRouter basename={basePath} >
            <Switch>
                <Route exact path="/Login"><Login /></Route>
                <PrivateRoute path='/Overview' component={Overview} />
                <PrivateRoute path='/Schedule' component={AddRuleSet} />
                <PrivateRoute path='/TextTemplate' component={AddTextTemplate} />
                <PrivateRoute path='/SearchSchedule' component={ViewRuleSet} />
                <PrivateRoute path='/SearchPhrase' component={ViewTags} />
                <PrivateRoute path='/Phrase' component={SaveTagPage} />
                <PrivateRoute path='/GroupTemplate' component={AddGroupTemplate} />
                <PrivateRoute path='/SearchGroupTemplate' component={SearchGroupTemplate} />
                <PrivateRoute path='/SearchTextTemplate' component={SearchTextTemplate} />
                <PrivateRoute path='/FilterTextTemplate' component={FilterTextTemplate} />
                <PrivateRoute path='/PSSLink' component={PSSLink} />
                <PrivateRoute path='/TimeFormat' component={TimeFormat} />
                <PrivateRoute path='/' component={Overview} />
            </Switch>
        </BrowserRouter>
    );
}

export default Routes;