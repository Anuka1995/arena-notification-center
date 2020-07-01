import React, { Component } from 'react';
import { Col, FormGroup, Label, Input, Card, Table, Button, CustomInput, CardGroup, CardBody } from 'reactstrap';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import Form from 'react-bootstrap/Form';
import { FormattedMessage } from "react-intl";
import { withRouter } from 'react-router';
import { NotificationManager } from 'react-notifications';
import Loader from 'react-loader-spinner'
import { MDBDataTable } from 'mdbreact';

import { GetGroupedTemplates } from '../../Services/GroupTemplateService';
import DepartmentSelection from '../OrgUnits/DepartmentSelection';

import './SearchGroupTemplate.css'

class SearchGroupTemplate extends Component {
    constructor(props) {
        super(props);
        this.state = {
            departmentList: [],
            groupedTemplates: [],
            currentDepartmentId: 0,
            searchText: "",
            isLoading: true,
            hospitalLevelSearch: false
        }
        this.handleSearchText = this.handleSearchText.bind(this);
        this.SearchButtonClicked = this.SearchButtonClicked.bind(this);

        this.handleChangeDepartment = selectedDepartment => {
            this.setState({
                currentDepartmentId: selectedDepartment === null ? 0 : selectedDepartment.value
            });
        };
        this.onChangeIsHospLevelSearch = e => {
            this.setState({
                hospitalLevelSearch: e.target.checked,
                currentDepartmentId: null
            })
        };
    }

    async componentDidMount() {
        this.loadGroupTemplates();
    }

    componentDidUpdate(prevProps, prevState) {
        //Though we are not using prevProps, we have to set the argument. because prevState is always the second arg
        if (prevState.currentDepartmentId != this.state.currentDepartmentId || prevState.hospitalLevelSearch != this.state.hospitalLevelSearch) {
            this.SearchGroupTemplates()
        }
    }

    handleSearchText(e) {
        this.setState({
            searchText: e.target.value
        })
    }

    loadGroupTemplates = async () => {
        try {
            let grpTemplates = await GetGroupedTemplates(null, null, false, false);

            this.setState({
                isLoading: false,
                groupedTemplates: grpTemplates
            });
        }
        catch (error) {
            console.log(error.message);
            this.setState({ isLoading: false })

            if (error.statuscode === 401) {
                NotificationManager.error("Session expired! Redirecting to login.", 'Schedules');
                this.props.history.push({
                    pathname: '/Login',
                    state: { directLogin: false }
                });
            }
            else {
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Search Failed");
                NotificationManager.error(errorMsg, 'Grouped Text Template');
            }
        }
    }

    handleEditClick = (e, templateId) => {
        e.preventDefault();
        this.props.history.push({
            pathname: '/GroupTemplate',
            state: { groupedTemplateId: templateId }
        });
    };

    SearchButtonClicked = async (e) => {
        e.preventDefault()
        this.SearchGroupTemplates()
    }

    SearchGroupTemplates = async () => {
        this.setState({ isLoading: true });
        // TODO add showIncative to filter active/inative Grouptemplates
        
        console.log("show hospital level", this.state.hospitalLevelSearch);

        try {
            let grpTemplates = await GetGroupedTemplates(
                 this.state.currentDepartmentId,
                 this.state.searchText,
                 this.state.hospitalLevelSearch);

            this.setState({ isLoading: false });

            if (grpTemplates == null) {
                NotificationManager.warning("No data received", "Group Text Template");
            }
            else {
                if (grpTemplates.length == 0) {
                    NotificationManager.info("List is Empty", "Group Text Template");
                }
                this.setState({ groupedTemplates: grpTemplates });
            }
        }
        catch (error) {
            console.log(error.message);
            this.setState({ isLoading: false })

            if (error.statuscode === 401) {
                NotificationManager.error("Session expired! Redirecting to login.", 'Grouped Text Template');
                this.props.history.push({
                    pathname: '/Login',
                    state: { directLogin: false }
                });
            } else {
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Search Failed");
                NotificationManager.error(errorMsg, 'Grouped Text Template');
            }
        }
    }

    OnSearchClear = async (e) => {
        this.setState({
            currentDepartmentId: 0,
            searchText: "",
            hospitalLevelSearch: false,
            groupedTemplates: [],
            isLoading: true
        });
        this.loadGroupTemplates();
    }

    render() {
        let smsTemplates = this.state.groupedTemplates
            .map(v => (
                {
                    name: v.textTemplateName,
                    departmentShortName: v.departmentShortName,
                    templateString: v.textTemplateString,
                    edit: <a href="#" onClick={(e) => { this.handleEditClick(e, v.textTemplateTextId) }}>Edit</a>
                }
            ));

        var data = {
           columns: [
              {
                label: <FormattedMessage id="GroupTemplateName" defaultMessage="Group-Template-Name" />,
                field: 'name',
                sort: 'asc',
                width: 150,
                attributes: {
                    style: { 
                        "text-align": "center",
                        "border-right": "1px rgb(220,220,220) solid"
                    }
                }
              },
              {
                label: <FormattedMessage id="Department.ShortName" defaultMessage="DepartmentShortName" />,
                field: 'departmentShortName',
                sort: 'asc',
                width: 200,
                attributes: {
                    style: { 
                        "text-align": "center",
                        "border-right": "1px rgb(220,220,220) solid"
                    }
                }
              },
              {
                label: <FormattedMessage id="String" defaultMessage="Text" />,
                field: 'templateString',
                sort: 'asc',
                width: 400,
                attributes: {
                    style: { 
                        "text-align": "center",
                        "border-right": "1px rgb(220,220,220) solid"
                    }
                }
              },
              {
                label: "",
                field: 'edit',
                sort: 'asc',
                width: 150,
              }
            ],
            rows: [...smsTemplates]
          };
          
        return (
            <div>
                <Breadcrumb>
                    <Breadcrumb.Item>Group Templates</Breadcrumb.Item>
                    <Breadcrumb.Item active>Search</Breadcrumb.Item>
                </Breadcrumb>
                <Form onSubmit={this.SearchButtonClicked}>
                    <CardGroup>
                        <Card>
                            <CardBody>
                                <DepartmentSelection departmentId={this.state.currentDepartmentId} onChangeDepartment={this.handleChangeDepartment}
                                    nonEditable={this.state.hospitalLevelSearch} />
                                <FormGroup>
                                    <CustomInput type="switch" id="isHospLevelCustomSwitch"
                                        name="customSwitchHL"
                                        label={<FormattedMessage id="SearchOnHospLevel"></FormattedMessage>}
                                        onChange={this.onChangeIsHospLevelSearch}
                                        value={this.state.hospitalLevelSearch}
                                        checked={this.state.hospitalLevelSearch} />
                                </FormGroup>
                        </CardBody>
                    </Card>
                    <Card>
                        <CardBody>
                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="SearchTerm" defaultMessage="Search" /></Label>
                                <Col sm={8}>
                                    <Input type="search" name="#" id="#" placeholder="search here" value={this.state.searchText}
                                        onChange={this.handleSearchText} />
                                </Col>
                            </FormGroup>
                            <FormGroup style={{paddingTop: '10px'}}>
                                <Button className="" color="primary" type="submit" style={{ margin: "auto" }}  >
                                    <FormattedMessage id="ButtonSearch" defaultMessage="Search" />
                                </Button>
                            </FormGroup>
                        </CardBody>
                    </Card>
                    </CardGroup>
                    <div className="text-right">
                        <FormGroup style={{ paddingTop: '10px' }}>
                            <Button className="mr-4 col-md-1" onClick={this.OnSearchClear}>
                                <FormattedMessage id="ClearAll" defaultMessage="Clear-All" />
                            </Button>
                        </FormGroup>
                    </div>
                </Form>
                
                   <MDBDataTable striped 
                        hover bordered 
                        data={data} 
                        searchLabel="Shallow Search" 
                        searching={false}
                        order={['name', 'asc']}
                        paginationLabel={["Previous", "Next"]}
                        infoLabel={["showing", "to", "of","entries"]}
                        noRecordsFoundLabel={ 
                            this.state.isLoading ? 
                            <div className="d-flex justify-content-center">
                                <Loader type="ThreeDots"
                                    color="#00BFFF"
                                    height={40}
                                    width={40} />
                            </div> : 
                            <p style={{"text-align":'center' }}>
                                <FormattedMessage id="GroupTemplate.NoRecordsFound" defaultMessage="No matching records found" />
                            </p>}
                        noBottomColumns />  
            </div>         
        );
    }
}

export default withRouter(SearchGroupTemplate);