import React, { Component } from 'react';
import { Button, Label, Form, FormGroup, Col, Input } from 'reactstrap';
import { FormattedMessage } from "react-intl";
import { withRouter } from 'react-router';
import { GetTicket } from '../../Services/AuthService';
import { NotificationContainer, NotificationManager } from 'react-notifications';
import { GetFullOrgTree } from '../../Services/OrgUnitsService'

import './Login.css';
import 'react-notifications/lib/notifications.css';

var _ = require('underscore');

class UserRole extends Component {
    constructor(props) {
        super(props);
        this.state = {
            username: this.props.username,
            password: this.props.password,
            roles: [],
            userrole: '0',
            isRoleSelected:false
        };

        this.getTicket = this.getTicket.bind(this);

        this.onChangeUserRole = e => {
            var selectionId = parseInt(e.target.value);
            if(selectionId!=0){
            this.setState({ 
                userrole: e.target.value,
                isRoleSelected:true
             });}
          };
    }

    backRedirect = () => {
       this.props.history.push("/");
     }
    
    componentDidMount(){
        this.setState({ roles: [{ userRoleId: "0", roleName: '(Select User Role)' }].concat(this.props.userroles) }); 
    }

    getTicket = async(e) => {
        e.preventDefault();

        try { 
            var session = await GetTicket(this.state.username, this.state.password, this.state.userrole);

            var selectedrole = this.state.userrole;
            var roleslist = this.state.roles;
            var selectedRoleObj = _.find(roleslist, function(r){ 
                return r.userRoleId == selectedrole; 
            });

            var usersession = {
                UserName: this.state.username,
                Token: session.token,
                Hospital: session.hospitalName,
                Role: selectedRoleObj.roleName,
                HospitalID: session.hospitalId
            };

            sessionStorage.setItem('userData', usersession);
            localStorage.setItem("token", usersession.Token);
            localStorage.setItem('usersession', JSON.stringify(usersession));
            localStorage.setItem("loggedInData", session.validTo);

            if(this.props.directLogin){
                this.props.history.push("/Overview");
                this.storeOrgTreeLocally()
            }
            else{
                this.props.history.goBack();
            }
        }
        catch(error) {
            let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in login request!");
            NotificationManager.error(`${errorMsg}`, 'Login');
        }
    }

    async storeOrgTreeLocally(){
        var tree = await GetFullOrgTree()
        localStorage.setItem("fullorgTree", JSON.stringify(tree))
    }

    render() {

        let userRolesList = this.state.roles.map(v => (<option value={v.userRoleId}>{v.roleName}</option>));

        return (
            <div className="login-page">
                <div className="form">
                <Form className="login-form" onSubmit={this.getTicket}>
                <h4><span className="font-weight-bold" ><FormattedMessage id="LoginHeader" defaultMessage="DIPS-Appoinment-SMS"/></span></h4>
                    <hr className="my-2" />
                    <div className="paddingdiv">
                    <Label><FormattedMessage id="UserRoles" defaultMessage="Select User Role"/></Label>
                    <Input type="select" name="userRoleId" id="userRoleId"
                            value={this.state.userrole}
                            onChange={this.onChangeUserRole}
                            placeholder={<FormattedMessage id="selectUserRole"></FormattedMessage>}>
                            {userRolesList}
                    </Input>
                    </div>
                    
                    <div className="paddingdiv">
                    <Button onClick={this.backRedirect}> Back</Button>
                    </div>
                    <div className="paddingdiv">
                    <Button type='submit' disabled={!this.state.isRoleSelected}>Login</Button>
                    </div>
                    
                </Form>
                </div>
                <NotificationContainer />
            </div>
        )
    }
}
export default withRouter(UserRole)