
import React, { Component } from 'react';
import { Col, FormGroup, Label, Input, Button, Form,Card ,CustomInput} from 'reactstrap';
import { FormattedMessage } from 'react-intl';
import { withRouter } from 'react-router';
import Loader from 'react-loader-spinner'

import { SearchRuleSetBy } from '../../Services/RuleSetService';
import RuleSetTree from './../TreeComponents/RuleSetTree';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import { NotificationManager } from 'react-notifications';

import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"

class ViewRuleSet extends Component {

    constructor(props) {
        super(props);
        this.state = {
            deps: [],
            addModalShow: false,
            checked: false,
            departmentid: null,
            ruleSetNameorGuid:'',
            getInactiveRule:false,
            ruleSetguid: null,
            fullRuleSetTree: [],
            getHospitalLevelRules:false,
            isLoadingRuleSetTree: true
        }
        
        this.handleChangeDepartment = selectedDepartment => {
            this.setState({
              departmentid: selectedDepartment === null ? 0 : selectedDepartment.value
            });
          };
        this.handleRuleSetNameorGuid = e =>  this.setState({ ruleSetNameorGuid : e.target.value });
        this.ShowInvalidRules = e => this.setState({getInactiveRule:!this.state.getInactiveRule})
        this.searchByRuleSetByDepartment = this.searchByRuleSetByDepartment.bind(this);
        this.ShowInvalidRules = this.ShowInvalidRules.bind(this);
        this.setFullTree = e => this.setState({fullRuleSetTree:e.target.value});
        this.onChangeHospitalLevelSearch = e => {
            this.setState({getHospitalLevelRules :  e.target.checked,departmentid:null});
        }

        this.showDetailView = (currentNode) => {
            //set ruleSet guid to props
            console.log(currentNode.id);
            var ruleSetGuid = currentNode.id;
            this.setState({ruleSetguid: ruleSetGuid});          
            this.props.history.push({
                pathname : '/Schedule',
                state :{
                leafnodeguid : ruleSetGuid,
                }
                } 
              );
          }
    }

    async componentDidMount() {
        await this.getRuleSetTree(this.state.departmentid, this.state.ruleSetNameorGuid,this.state.getInactiveRule,this.state.getHospitalLevelRules);
    }

    componentDidUpdate(prevProps, prevState) {
      //Load Tree structure when department is changed
      if (prevState.departmentid != this.state.departmentid || prevState.getHospitalLevelRules != this.state.getHospitalLevelRules) {
        this.getRuleSetTree(this.state.departmentid, this.state.ruleSetNameorGuid,this.state.getInactiveRule,this.state.getHospitalLevelRules);
      }
  }

    render() {

        let addModalClose = () => this.setState({ addModalShow: false })

        return (
            <Form onSubmit={this.searchByRuleSetByDepartment}>
                <Breadcrumb>
                    <Breadcrumb.Item><FormattedMessage id="Schedule" defaultMessage="Schedules" /></Breadcrumb.Item>
                    <Breadcrumb.Item active><FormattedMessage id="FilteredBy" defaultMessage="Filtered-By" /></Breadcrumb.Item>
                </Breadcrumb>
            <FormGroup row>
              <Col>
                <Card body style={{borderStyle: 'none'}}>
                  <Card body>
                    <FormGroup row>
                      <Col sm="12">
                        <DepartmentSelection departmentId={this.state.departmentid} onChangeDepartment={this.handleChangeDepartment} nonEditable={this.state.getHospitalLevelRules} /> 
                      </Col>
                    </FormGroup>
                    <FormGroup row>
                      <Col sm="7">
                      <CustomInput type="switch" id="isActiveCustomSwitch" 
                          name="customSwitch" 
                          label={<FormattedMessage id="SearchOnHospLevel"></FormattedMessage>}
                          onChange={this.onChangeHospitalLevelSearch}
                          value={this.state.getHospitalLevelRules}
                          checked={this.state.getHospitalLevelRules}/>
                      </Col>  
                    </FormGroup>
                  </Card><br />
                  <Card body>
                    <FormGroup row>
                      <Label md={4}><FormattedMessage id="Text.RuleSetNameOrGuid" defaultMessage="Schedule Name/Guid"/></Label>
                      <Col sm={8}>
                        <Input type="text" name="ruleSetName" id="ruleSetNameorGuid" value={this.state.ruleSetNameorGuid} placeholder="Schedule Name"  onChange={text => { this.handleRuleSetNameorGuid(text) }} />
                      </Col>
                    </FormGroup>
                    <FormGroup row>
                      <Col sm={12} >
                        <CustomInput type="switch" id="columnFilter" 
                          name="columnFilter" 
                          label={<FormattedMessage id="ShowInactiveSchedules" defaultMessage="Show Inactive Schedules" />}
                          onChange={this.ShowInvalidRules}
                          value={this.state.getInactiveRule}
                          checked={this.state.getInactiveRule}/>
                      </Col>
                      <Col   className="offset-9">
                              <Button className="btn" color="primary"><FormattedMessage id="ButtonSearch" defaultMessage="Search" /></Button>
                      </Col>
                    </FormGroup>
                    </Card>
                </Card> 
             </Col>
              <Col >
                  <FormGroup row>
                    <Card body className="overviewTree">    
                    { this.state.isLoadingRuleSetTree ? 
                    <div className="d-flex justify-content-center">
                    <Loader type="ThreeDots"
                        color="#00BFFF"
                        height={40}
                        width={40} />
                    </div> :  
                    <div className="row" >
                    <div  style={{ textAlign: 'left', maxWidth:'600px',minHeight: '1px', maxHeight : '500px' }}>
                      <RuleSetTree id="RuleSetTree" key="1" 
                      nodeData={this.state.fullRuleSetTree} 
                      content="Schedule List"
                      open
                      onLeafNodeClick={this.showDetailView} />
                    </div>
                  </div>
                  }
                      
                    </Card>
                  </FormGroup>
              </Col>
            </FormGroup>
            </Form>
        )
    }

    searchByRuleSetByDepartment = async(e)=> {
        e.preventDefault();
        await this.getRuleSetTree(this.state.departmentid, this.state.ruleSetNameorGuid,this.state.getInactiveRule,this.state.getHospitalLevelRules);
      }
    
      getRuleSetTree = async(departmentId, text,isActive,gethospitalLvele) => {
        this.setState({ isLoadingRuleSetTree: true });
        try {
           var  departmentIdForSend = (departmentId==0)? null: departmentId;
          var ruleSetTreeNodes = await SearchRuleSetBy(departmentIdForSend,text,isActive,gethospitalLvele);
          this.setState({fullRuleSetTree: ruleSetTreeNodes});
          if(ruleSetTreeNodes.length==0){
            NotificationManager.info('No Search Results  !', 'Schedules');
          }
          this.setState({ isLoadingRuleSetTree: false })
        }
        catch(error) {
          console.log(error.message);
          this.setState({ isLoadingRuleSetTree: false })
          if (error.statuscode === 401) {
            NotificationManager.error("Session expired! Redirecting to login.", 'Schedules');
            this.props.history.push({
                pathname: '/Login',
                state: { directLogin: false }
            });
        } else {
            let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Search Failed");
            NotificationManager.error(errorMsg, 'Schedules');
        }
          }
      }
}
export default withRouter(ViewRuleSet)