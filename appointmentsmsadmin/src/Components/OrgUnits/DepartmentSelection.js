import React, { Component } from 'react';
import { FormattedMessage } from "react-intl";
import { Col, FormGroup, Label, Input } from 'reactstrap';
import { GetDepartmentList } from '../../Services/OrgUnitsService';
import { NotificationManager } from 'react-notifications';
import 'react-notifications/lib/notifications.css';
import { Redirect } from 'react-router';
import Select from 'react-select'
import { withRouter } from 'react-router';

import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"

class DepartmentSelection extends Component {
  constructor(props) {
    super(props);
    this.state = {
      DepartmentList: [],
      isLoading: true,
      department:null
    }
  }

  async componentDidMount() {
    try {    
      var departmentsFromApi = await GetDepartmentList();
      this.setState({
        isLoading: false,
        DepartmentList: departmentsFromApi
      });
    }
    catch (error) {
      console.log(error.message);
      this.setState({ isLoading: false })
      NotificationManager.error('Get Departments Failed !', 'Department Selection');

      if (error.statuscode === 401) {
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: { directLogin: false }
        });
      }
    }
  }

  render() {

    if (this.state.redirect) {
      return (
        <Redirect to={'/Login'} />
      )
    }

    var departmentListOption = this.state.DepartmentList.map(({ departmentId, departmentName }) => {
      return {
        value: departmentId,
        label: departmentName
      }
    });

    var selected = null
    if (this.props.departmentId >= 0) {
      selected = departmentListOption.filter(department =>
        department.value == this.props.departmentId
      )
    }
    
    if (this.props.isReload) {
      if(this.props.depOptions!=null){
        departmentListOption = this.props.depOptions;
      }          
    }

    let isDisabled = (this.props.nonEditable);
    return (

      <FormGroup row align="left">
        <Label md={4}><FormattedMessage id="Select.Department" defaultMessage="Department" /></Label>
        <Col md={8}>
          <Select name="departmentId" id="departmentId"
            value={selected}
            isDisabled={isDisabled}
            onChange={this.props.onChangeDepartment}
            isClearable={true}
            isLoading={this.state.isLoading}
            options={departmentListOption}
            placeholder={<FormattedMessage id="SearchDepartment"></FormattedMessage>}>
        </Select>
        </Col>
      </FormGroup>

    )
  }
}
export default withRouter(DepartmentSelection)