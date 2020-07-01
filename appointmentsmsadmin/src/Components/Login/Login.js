import React, { Component } from 'react';
import { Button, Label, Input, Form } from 'reactstrap';
import { FormattedMessage } from "react-intl";
import { withRouter } from 'react-router';
import { NotificationContainer, NotificationManager } from 'react-notifications';

import { GetUserRoles } from '../../Services/AuthService';
import UserRole from './UserRole';

import 'react-notifications/lib/notifications.css';
import './Login.css';

class Login extends Component {

    constructor(props) {
        super(props);
        this.state = {
            userName: '',
            password: '',
            userRoles: [],
            redirectToRoles: false,
            directLogin: true
        };
        this.handleUserName = this.handleUserName.bind(this);
        this.handlePassword = this.handlePassword.bind(this);
        this.getUserRoles = this.getUserRoles.bind(this);
    }

    handleUserName(text) {
        this.setState({ userName: text.target.value })
    }
    handlePassword(text) {
        this.setState({ password: text.target.value })
    }

    getUserRoles = async(e)=>{
        e.preventDefault();

        var logonParams = {
            username: this.state.userName,
            password: this.state.password
        }

        try {
            var userInfo = await GetUserRoles(logonParams);

            this.setState({ userRoles: userInfo.userRoles });
            this.setState({
                redirectToRoles: true
            });
        } catch(error) {
            let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in user-roles request!");
            NotificationManager.error(`${errorMsg}`, 'Login');
        }
    }

    componentDidMount(){
        if(this.props.location != null && 
            this.props.location.state != null && 
            this.props.location.state.directLogin === false) {
                this.setState({ directLogin: false });
        }
    }

    render() {
        if (this.state.redirectToRoles) {
            return (
                <UserRole 
                    username={this.state.userName} 
                    password={this.state.password} 
                    userroles={this.state.userRoles}
                    directLogin={this.state.directLogin}  />
            )
        }
        return (
            <div className="login-page">
                <div className="form">
                <Form className="login-form" onSubmit={this.getUserRoles}>
                <h4><span className="font-weight-bold" ><FormattedMessage id="LoginHeader" defaultMessage="DIPS-Appointment-SMS"/></span></h4>
                    <hr className="my-2" />
                    <Label>User Name</Label>
                    <Input type="text" name="userName" placeholder="UserName" onChange={(text) => { this.handleUserName(text) }} required></Input>
                    
                    <Label>Password</Label>
                    <Input type="password" name="password" placeholder="Password" onChange={(password) => { this.handlePassword(password) }} required/>
                    <Button type='submit'>Get User Roles</Button>
                </Form>
                </div>
                <NotificationContainer />
            </div>
        )
    }
}
export default withRouter(Login)