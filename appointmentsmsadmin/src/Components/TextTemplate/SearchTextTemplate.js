import React, { Component } from 'react';
import { Col, Button, Label, FormGroup, Input, form, ButtonToolbar, CardText, Card, Container, Row, CustomInput } from 'reactstrap';
import { FormattedMessage } from 'react-intl';
import Form from 'react-bootstrap/Form';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import Loader from 'react-loader-spinner'

import { SearchTextTemplateBy } from '../../Services/TextTemplateService'
import { GetDepartmentList, GetOPDList, GetSectionsByDepartment, GetWardsByDepartment, GetWardsBySection, GetOPDListByHospital } from '../../Services/OrgUnitsService';
import TextTemplateTree from '../TreeComponents/TextTemplateTree'
import { GetOverview } from '../../Services/OverviewDataService'
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import { NotificationManager } from 'react-notifications';

import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"
import './AddTextTemplate.css';

class SearchTextTemplate extends Component {
    constructor(props) {
        super(props);
        this.state = {
            handleTemplateName: "",
            TextTemplateTreeOverview: [],
            OPDList: [],
            SectionList: [],
            WardList: [],
            currentDepartmentId: null,
            TextTemplateName: '',
            OPDId: null,
            SectionId: null,
            WardId: null,
            fullSmsTextTree: [],
            showInactive: false,
            smsTextId: null,
            hospitalLevelSearch: false,
            isNonEditable: false,
            isLoadingTemplateTree: true
        }

        this.HandleTemplateName = this.HandleTemplateName.bind(this);
        this.handleChangeDepartment = this.handleChangeDepartment.bind(this);
        this.SelectedOPD = this.SelectedOPD.bind(this);
        this.SelectedSection = this.SelectedSection.bind(this);
        this.SelectedWard = this.SelectedWard.bind(this);

        this.searchSmsTextTemplate = this.searchSmsTextTemplate.bind(this);
        this.handleCheckInactiveTeplates = this.handleCheckInactiveTeplates.bind(this)
        this.showDetailView = this.showDetailView.bind(this)
        this.setFullTree = e => this.setState({ fullSmsTextTree: e.target.value });
        this.onChangeIsHospLevelSearch = this.ChangeIsHospLevelSearch.bind(this);
    }

    componentDidMount() {
        //this.loadTemplateTreeOverview();
        this.loadDepartmentList();
        this.loadOPDListByHospital();
        this.getTreeStruct();
    }

    //handle  hospital level only 
    ChangeIsHospLevelSearch(e) {
        console.log(e.target.checked);
        if (e.target.checked) {
            this.setState({
                isNonEditable: true,
                currentDepartmentId: null,
                OPDId: null,
                SectionId: null,
                WardId: null,
                OPDList: [],
                SectionList: [],
                WardList: []

            })
        } else {
            this.loadOPDListByHospital();
            this.setState({ isNonEditable: false })
        }
        this.setState({ hospitalLevelSearch: e.target.checked })
    }

    //handle template search string
    HandleTemplateName(e) {
        this.setState({ TextTemplateName: e.target.value })
    }

    //handle Department
    handleChangeDepartment = selectedDepartment => {
        var selectionId = selectedDepartment === null ? null : selectedDepartment.value
        this.setState({
            currentDepartmentId: selectionId
        });

        if (selectionId != null) {
            this.setState({
                OPDId: null,
                SectionId: null,
                WardId: null,
                OPDList: [],
                SectionList: [],
                WardList: []
            });
            this.loadOPDList(selectionId);
            this.loadSectionList(selectionId);
            this.loadWardListByDepartment(selectionId);
        } else {
            this.setState({
                OPDId: null,
                SectionId: null,
                WardId: null,
                OPDList: [],
                SectionList: [],
                WardList: []
            });
            this.loadOPDListByHospital();
        }
    };

    //handle active/inactive toggle
    handleCheckInactiveTeplates = e => {
        this.setState({ showInactive: e.target.checked })
    }


    showDetailView = selectedNode => {
        console.log("node is clicked", selectedNode)

        var smsTextGuid = selectedNode.id;
        this.setState({ smsTextId: smsTextGuid });
        this.props.history.push({
            pathname: '/TextTemplate',
            state: {
                leafnodeguid: smsTextGuid,
            }
        }
        );
    }

    //OPD handling   
    SelectedOPD(e) {
        let selectedId = e.target.value;
        this.setState({
            OPDId: (selectedId == 0 ? null : selectedId)
        })
    }

    //Handle Section
    SelectedSection(e) {
        let currentSectionId = e.target.value;
        this.setState({
            SectionId: (currentSectionId == 0 ? null : currentSectionId),
            WardId: null
        })
        if (currentSectionId != null && currentSectionId != 0) {
            this.loadWardListBySection(currentSectionId);
        } else {
            this.loadWardListByDepartment(this.state.currentDepartmentId);
        }

    }

    //Handle Ward
    SelectedWard(e) {
        let currentWardId = e.target.value;
        this.setState({ WardId: (currentWardId == 0 ? null : currentWardId) })
    }

    loadTemplateTreeOverview() {
        GetOverview()
            .then(result => {
                this.setState({ TextTemplateTreeOverview: result });
            })
    }

    loadDepartmentList() {
        GetDepartmentList().then((result) => {
            this.setDepartmentList(result)
        });
    }

    loadOPDList(departmentId) {
        GetOPDList(departmentId).then((result) => {
            this.setOPDList(result)
        });
    }

    loadOPDListByHospital() {
        GetOPDListByHospital().then((result) => {
            this.setOPDList(result)
        });
    }

    loadSectionList(departmentId) {
        GetSectionsByDepartment(departmentId).then((result) => {
            this.setSectionList(result)
        });
    }

    loadWardListByDepartment(departmentId) {
        GetWardsByDepartment(departmentId).then((result) => {
            this.setWardList(result)
        });
    }

    loadWardListBySection(sectionId) {
        GetWardsBySection(sectionId).then((result) => {
            this.setWardList(result)
        });
    }

    setDepartmentList(list) {
        this.setState({ DepartmentList: list })
    }

    setOPDList(list) {
        this.setState({ OPDList: list })
    }

    setSectionList(list) {
        this.setState({ SectionList: list })
    }

    setWardList(list) {
        this.setState({ WardList: list })
    }

    searchSmsTextTemplate = async (e) => {
        e.preventDefault();
        await this.getTreeStruct();
    }

    getTreeStruct = async () => {
        this.setState({isLoadingTemplateTree:true});
        try {
            console.log(this.state.TextTemplateName)
            var smsTextTreeNodes = await SearchTextTemplateBy(this.state.currentDepartmentId, this.state.OPDId, this.state.SectionId, this.state.WardId, this.state.TextTemplateName, !this.state.showInactive, this.state.hospitalLevelSearch);
            this.setState({ 
                fullSmsTextTree: smsTextTreeNodes,
                isLoadingTemplateTree: false
            });
            if(smsTextTreeNodes.length == 0){
                NotificationManager.info('No Search Results  !', 'SMS content template');
            }
        }
        catch (error) {
            console.log(error.message);
            this.setState({ isLoadingTemplateTree: false });
            if (error.statuscode === 401) {
                console.log("Ticket is expired. Redirect To Login.");
                NotificationManager.error("Session expired! Redirecting to login.", 'Schedules');
                this.props.history.push({
                    pathname: '/Login',
                    state: { directLogin: false }
                });
            } else {
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in search!");
                NotificationManager.error(`${errorMsg}`, 'Text Template');
              }
            }
    }
    componentDidUpdate(prevProps, prevState) {
        //Load Tree structure when department is changed
        if (prevState.currentDepartmentId != this.state.currentDepartmentId || prevState.hospitalLevelSearch != this.state.hospitalLevelSearch) {
            this.getTreeStruct();
        }
    }

    render() {

        let OPDOptions = this.state.OPDList.map(v => (<option value={v.opdid}>{v.opdDisplayName}</option>));
        let SectionOptions = this.state.SectionList.map(v => (<option value={v.sectionId}>{v.sectionDisplayName}</option>));
        let WardOptions = this.state.WardList.map(v => (<option value={v.wardId}>{v.wardDisplayName}</option>));

        return (

            <Form onSubmit={this.searchSmsTextTemplate}>
                <Breadcrumb>
                    <Breadcrumb.Item><FormattedMessage id="SMSContentTemplates" defaultMessage="SMS-content-templates" /></Breadcrumb.Item>
                    <Breadcrumb.Item active><FormattedMessage id="SearchBy" defaultMessage="Search By" /></Breadcrumb.Item>
                </Breadcrumb>
                <FormGroup row>
                    <Col sm="6">
                    <Card body style={{borderStyle: 'none'}}>
                        <Card body >
                            <DepartmentSelection departmentId={this.state.currentDepartmentId} onChangeDepartment={this.handleChangeDepartment} nonEditable={this.state.isNonEditable} />

                            <FormGroup row>
                                <Col md={7}>
                                    <CustomInput type="switch" id="isHospitalLevelCustomSwitch"
                                        name="customSwitchIsActiveSrch"
                                        label={<FormattedMessage id="SearchOnHospLevel"></FormattedMessage>}
                                        onChange={this.onChangeIsHospLevelSearch}
                                        value={this.state.hospitalLevelSearch}
                                        checked={this.state.hospitalLevelSearch} />
                                </Col>
                            </FormGroup>
                        </Card ><br />
                        <Card body >
                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="OPD" defaultMessage="OPD" /></Label>
                                <Col md={8}>
                                    <Input type="select" name="OPD" value={this.state.OPDId} placeholder="Search OPD Name.." onChange={this.SelectedOPD} disabled={this.state.isNonEditable} >
                                        <option value="0" >No OPD Selected</option>
                                        {OPDOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="Section" defaultMessage="Section" /></Label>
                                <Col md={8}>
                                    <Input type="select" name="Section" value={this.state.SectionId} placeholder="Search section Name.." onChange={this.SelectedSection} disabled={this.state.isNonEditable} >
                                        <option value="0" >No Section Selected</option>
                                        {SectionOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="Ward" defaultMessage="Ward" /></Label>
                                <Col md={8}>
                                    <Input type="select" name="Ward" value={this.state.WardId} placeholder="Search section Name.." onChange={this.SelectedWard} disabled={this.state.isNonEditable} >
                                        <option value="0" >No Ward Selected</option>
                                        {WardOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4} ><FormattedMessage id="FindbyName/Text" defaultMessage="Find-by-Name/Text" /> </Label>
                                <Col md={8}>                               
                                    <Input type="search" name="TextTemplateName" id="#" placeholder="Search Templates" value={this.state.TextTemplateName} onChange={this.HandleTemplateName} />
                                </Col>
                            </FormGroup>

                            {/* Commmeted until the searching for inactive templates enabled. */}
                            <FormGroup row>                              
                                <Col md={10}>
                                    <CustomInput type="switch" id="isActiveSearchCustomSwitch"
                                        name="customSwitchIsActiveSrch"
                                        label={<FormattedMessage id="TextTemplate.ShowIncativeCheck"></FormattedMessage>}
                                        onChange={this.handleCheckInactiveTeplates}
                                        checked={this.state.showInactive}
                                        value={this.state.showInactive} /></Col>
                            </FormGroup>

                            <FormGroup row>
                                <Col md={8} className="offset-9">
                                    <Button className="btn" color="primary" variant="primary" type="submit" style={{ margin: "auto"}}>
                                        <FormattedMessage id="ButtonSearch" defaultMessage="Search" />
                                    </Button>
                                </Col>
                            </FormGroup>
                        </Card>
                    </Card >
                    </Col>
                    <Col sm="6">
                        <FormGroup row>
                            <Card body className="overviewTree">   
                            { this.state.isLoadingTemplateTree ?
                                 <div className="d-flex justify-content-center">
                                 <Loader type="ThreeDots"
                                     color="#00BFFF"
                                     height={40}
                                     width={40} />
                                 </div> : 
                                 <div className="row" >
                                 <div style={{ textAlign: 'left', maxWidth:'600px', minHeight: '1px', maxHeight : '500px'}}>
                                     <TextTemplateTree id="smsTextTree" key="1"
                                         nodeData={this.state.fullSmsTextTree}
                                         content="sms text template List"
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

        );
    }
}
export default SearchTextTemplate;