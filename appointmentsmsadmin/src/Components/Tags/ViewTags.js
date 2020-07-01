import React, { Component } from 'react';
import { Container, Row, Col, FormGroup, Label, Input, Button, Card, CustomInput, CardGroup, CardBody } from 'reactstrap';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import Form from 'react-bootstrap/Form';
import { FormattedMessage } from "react-intl";
import { NotificationManager } from 'react-notifications';
import Loader from 'react-loader-spinner'
import { withRouter } from 'react-router';

import PhrasesTree from './../TreeComponents/PhrasesTree';
import SaveTagForm from './SaveTagForm';
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import { FilterAndGetTags, GetTag } from '../../Services/TagsService';

import "react-loader-spinner/dist/loader/css/react-spinner-loader.css";

class ViewTags extends Component {
  constructor(props) {
    super(props);
    this.state = {
      departmentId: null,
      searchTerm: '',
      fullTagTree: [],
      e_tagguid: null,
      hospitalLevelSearch: false,
      showInactive: false,
      isLoadingTree: true,
      selectedNode: ''
    }

    this.handleChangeSearchTerm = e => this.setState({ searchTerm: e.target.value });

    this.onChangeIsHospLevelSearch = e => {
      this.setState({
        hospitalLevelSearch: e.target.checked,
        departmentId: null,
        e_tagguid: null
      })
    };

    this.handleSearchOnGetInactive = e => { this.setState({ showInactive: e.target.checked }) };

    this.handleChangeDepartment = selectedDepartment => {
      this.setState({
        departmentId: selectedDepartment === null ? 0 : selectedDepartment.value,
        e_tagguid: null
      });
    };

    this.handleUpdateSucess = async (e) => {
      //TODO: what to do after a searched item is updated.
      //this.clearAllChanges();
      sessionStorage.removeItem('tagitem');
      await this.filterAndGrabTree(this.state.departmentId, this.state.searchTerm, this.state.showInactive, this.state.hospitalLevelSearch);
    };

    this.handleSaveFormCancel = async (e) => {
      this.clearAllChanges();
      await this.filterAndGrabTree(this.state.departmentId, this.state.searchTerm, this.state.showInactive, this.state.hospitalLevelSearch);
    };

    this.showDetailView = (currentNode) => {
      //set tag guid to props
      var tagGuid = currentNode.id;
      console.log(tagGuid);
      this.setState({
        e_tagguid: tagGuid,
        selectedNode: tagGuid
      });
    }
  }

  async componentDidMount() {
    await this.filterAndGrabTree(this.state.departmentId,
      this.state.searchTerm,
      this.state.showInactive,
      this.state.hospitalLevelSearch);
  }

  componentDidUpdate(prevProps, prevState) {
    if (prevState.departmentId != this.state.departmentId || prevState.hospitalLevelSearch != this.state.hospitalLevelSearch) {
      this.filterAndGrabTree(this.state.departmentId,
        this.state.searchTerm,
        this.state.showInactive,
        this.state.hospitalLevelSearch);
    }
  }

  render() {
    return (
      <div>
        <Breadcrumb>
          <Breadcrumb.Item href="#"><FormattedMessage id="Phrases" defaultMessage="tags" /></Breadcrumb.Item>
          <Breadcrumb.Item active><FormattedMessage id="ViewPhrases" defaultMessage="ViewTags" /></Breadcrumb.Item>
        </Breadcrumb>
        <Form onSubmit={this.searchTagsOnSubmit}>
          <CardGroup>
            <Card>
              <CardBody>
                <DepartmentSelection departmentId={this.state.departmentId} onChangeDepartment={this.handleChangeDepartment} nonEditable={this.state.hospitalLevelSearch} />
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
                  <Label sm={4}><FormattedMessage id="SearchTerm" defaultMessage="SearchTerm" /></Label>
                  <Col sm={8}>
                    <Input type="search" name="#" id="#" placeholder="Search Phrases" value={this.state.searchTerm} onChange={this.handleChangeSearchTerm} />
                  </Col>
                </FormGroup>

                <FormGroup>
                  <CustomInput type="switch" id="GetInactiveSearchCustomCheckBox"
                    name="customCheckBoxInactiveSrch"
                    label={<FormattedMessage id="Phrases.Search.IsActive"></FormattedMessage>}
                    onChange={this.handleSearchOnGetInactive}
                    value={this.state.showInactive}
                    checked={this.state.showInactive} />
                </FormGroup>
                <FormGroup style={{ paddingTop: '10px' }}>
                  <Button className="" color="primary" type="submit" style={{ margin: "auto" }} >
                    <FormattedMessage id="ButtonSearch" defaultMessage="Search" />
                  </Button>
                </FormGroup>
              </CardBody>
            </Card>
          </CardGroup>
          <div className="text-right">
            <FormGroup style={{ paddingTop: '10px' }}>
              <Button className="mr-4 col-md-1"
                style={{ minWidth: 'max-content' }}
                onClick={this.OnSearchClear}>
                <FormattedMessage id="ClearAll" defaultMessage="Clear-All" />
              </Button>
            </FormGroup>
          </div>
        </Form>
        <Row style={{ 'padding': '14px 0 0 0' }}>
          <Col sm={12} md={5} lg={4} >
            <Card body className="overviewTree">

              {this.state.isLoadingTree ?
                <div className="d-flex justify-content-center">
                  <Loader type="ThreeDots"
                    color="#00BFFF"
                    height={40}
                    width={40} />
                </div> :
                <div className="row">
                  <div className="col-md-6" style={{ maxHeight: '450px', maxWidth: '350px' }}>
                    <PhrasesTree id="phrasesTree" key="1"
                      nodeData={this.state.fullTagTree}
                      title="Phrases"
                      open
                      selected={this.state.selectedNode}
                      onLeafNodeClick={this.showDetailView} />
                  </div>
                </div>}

            </Card>
          </Col>
          <Col sm={12} md={7} lg={8} align="right" style={{ 'max-height': '100%' }}>
            <div id="editTagForm">
              <SaveTagForm
                onSaveCancel={this.handleSaveFormCancel}
                tagGuid={this.state.e_tagguid}
                onSaveSuccess={this.handleUpdateSucess} />
            </div>
          </Col>
        </Row>
      </div>
    )
  }

  OnSearchClear = async (e) => {
    this.clearAllChanges();

    this.filterAndGrabTree(null, '', false, false);
  }

  searchTagsOnSubmit = async (e) => {
    e.preventDefault();
    await this.filterAndGrabTree(this.state.departmentId,
      this.state.searchTerm,
      this.state.showInactive,
      this.state.hospitalLevelSearch);

    // clear out the needed stuff
    this.setState({
      //departmentId: null,
      //searchTerm: '',
      //fullTagTree: [],
      e_tagguid: null
    });
  }

  filterAndGrabTree = async (department, text, getInactive, isHospitalLevel) => {
    this.setState({ isLoadingTree: true });
    try {
      var tagTreeNodes = await FilterAndGetTags(department, text, getInactive, isHospitalLevel);
      this.setState({ isLoadingTree: false })
      if (tagTreeNodes == null) {
        NotificationManager.warning("No data received", "Search Phrases");
      }
      else {
        if (tagTreeNodes.length == 0) {
          NotificationManager.info("List is Empty", "Search Phrases");
        }
        this.setState({ fullTagTree: [] });
        this.setState({ fullTagTree: tagTreeNodes });
      }
      this.setState({ isLoadingTree: false })
    }
    catch (error) {
      console.log(error.message);
      this.setState({ isLoadingTree: false });

      if (error.statuscode === 401) {
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: { directLogin: false }
        });
      }
      else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in search phrase!");
        NotificationManager.error(`${errorMsg}`, 'Phrases');
      }
    }
  }

  clearAllChanges() {
    this.setState({
      departmentId: null,
      searchTerm: '',
      fullTagTree: [],
      e_tagguid: null,
      hospitalLevelSearch: false,
      showInactive: false,
      selectedNode: ''
    });

    sessionStorage.removeItem('tagitem');
  }
}

export default withRouter(ViewTags)