import React, { Component } from 'react';
import { withRouter } from 'react-router';
import { NotificationManager } from 'react-notifications';
import 'bootstrap/dist/css/bootstrap.css';
import OverviewTree from './../TreeComponents/OverviewTree';
import { Col, Form, FormGroup, Card, CustomInput, Badge } from 'reactstrap';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import { GetOverview } from '../../Services/OverviewDataService';
import { FormattedMessage } from "react-intl";
import Loader from 'react-loader-spinner';
import 'react-notifications/lib/notifications.css';

import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"

class Overview extends Component {

    constructor(props) {
        super(props);

        this.state = {
            treeData: [],
            showExcludedOrgUnits: false,
            showTargetOrgUnits: false,
            hospitalName: '',
             treeDataIsLoading: true,
            showInactiveSchedules:false
        };

        this.handleTargetOrgUnits = e => {
            this.setState({ showTargetOrgUnits: e.target.checked });
        }

        this.handleExcludedOrgUnits = e => {
            this.setState({ showExcludedOrgUnits: e.target.checked })
        };

        this.handleInactiveSchedules = this.handleInactiveSchedules.bind(this);

        this.goToEditSchedule = this.goToEditSchedule.bind(this);

        this.goToEditContentTemplate = this.goToEditContentTemplate.bind(this);
    }

    goToEditSchedule = (sheduleId) => {
        this.props.history.push({ 
            pathname : '/Schedule',
            state : { leafnodeguid : sheduleId }
        });
    }

    goToEditContentTemplate = (templateId) => {
        this.props.history.push({
            pathname: '/TextTemplate',
            state: { leafnodeguid: templateId }
        });
    }

    handleInactiveSchedules = async(e) =>{
        this.setState ({
                  treeData: [],
                  showInactiveSchedules: e.target.checked 
               });
        await this.filterOverviewTree(e.target.checked);
    }

    async componentDidMount() {
       await this.filterOverviewTree(this.state.showInactiveSchedules);
    }

    filterOverviewTree = async(getInactive) => { 
        var usersession = localStorage.getItem('usersession');
        var userSessionObj = JSON.parse(usersession);
        let hospitalName = (userSessionObj ? userSessionObj.Hospital : "Hospital");
        this.setState({ treeDataIsLoading:true});
        try {
            var overview = await GetOverview(getInactive);
            this.setState({ 
                treeData: overview,
                hospitalName: hospitalName,
                treeDataIsLoading: false
             });
            
        } catch(error) {
            if(error.statuscode === 401){
                this.setState({ 
                    treeData: [],
                    treeDataIsLoading: false
                });
                NotificationManager.error("Session expired! Redirecting to login.", 'Overview');
                this.props.history.push({
                    pathname: '/Login',
                    state: {directLogin: false}
                });
            }
             
            let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Overview generation failed!");
            NotificationManager.error(errorMsg, 'Overview');
            this.setState({ treeData: [] });
        }
    }

    render() {
        
        return (
            <Form>
                <Breadcrumb>
                    <Breadcrumb.Item active><FormattedMessage id="SMSsendingplan" /></Breadcrumb.Item>
                </Breadcrumb>
                <Col sm="12" >
                    <Card body >
                    <CustomInput type="switch" id="includeexCustomSwitch" 
                        name="includeexCustomSwitch" 
                        label={<FormattedMessage id="ShowExcludeORgUnits"></FormattedMessage>}
                        checked={this.state.showExcludedOrgUnits}
                        value={this.state.showExcludedOrgUnits}
                        onChange={this.handleExcludedOrgUnits} />

                    <CustomInput type="switch" id="includetargetCustomSwitch" 
                        name="includetargetCustomSwitch" 
                        label={<FormattedMessage id="ShowApplicableORgUnits"></FormattedMessage>}
                        checked={this.state.showTargetOrgUnits} 
                        onChange={this.handleTargetOrgUnits}
                        value={this.state.showTargetOrgUnits} />

                    <CustomInput type="switch" id="includeInactiveCustomSwitch" 
                        name="includeInactiveCustomSwitch" 
                        label={<FormattedMessage id="ShowInactiveSchedules"></FormattedMessage>}
                        checked={this.state.showInactiveSchedules} 
                        onChange={this.handleInactiveSchedules}
                        value={this.state.showInactiveSchedules} />
                    </Card> 
                </Col>
                <br/>
                <Col sm="12" >
                    <Card body>
                        <div align="left">
                            <FormGroup row>
                                <Col sm={12}>
                                    <div>
                                        <div className="col-md-12" style={{ textAlign: 'left', minWidth: '100%'}}>
                                       
                                            { this.state.treeDataIsLoading ? 
                                                <div className="d-flex justify-content-center">
                                                    <Loader type="ThreeDots"
                                                    color="#00BFFF"
                                                    height={40}
                                                    width={40} />
                                                </div> : 
                                                <OverviewTree key="1" 
                                                    nodeData={this.state.treeData} 
                                                    content={this.state.hospitalName} open
                                                    showExcludedOrgUnits={this.state.showExcludedOrgUnits}
                                                    showTargetOrgUnits={this.state.showTargetOrgUnits}
                                                    showInactiveSchedules={this.state.showInactiveSchedules}
                                                    scheduleClicked={this.goToEditSchedule}
                                                    templateClicked={this.goToEditContentTemplate}/>
                                            }
                                        </div>

                                    </div>
                                </Col>
                            </FormGroup>
                        </div>
                    </Card>
                </Col>
            </Form>
        );
    }
}
export default withRouter(Overview)