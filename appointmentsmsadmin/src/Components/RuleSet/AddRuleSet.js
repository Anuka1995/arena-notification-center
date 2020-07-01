import React, { Component } from 'react';
import { Button, Card, Col, CustomInput, FormGroup, Input, InputGroup, InputGroupAddon, InputGroupText, Label } from 'reactstrap';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import Form from 'react-bootstrap/Form';
import { FormattedMessage } from "react-intl";
import { withRouter } from 'react-router';
import DatePicker from "react-datepicker";
import Moment from 'moment';
import TimeField from 'react-simple-timefield';
import { NotificationManager } from 'react-notifications';
import { ExcludeOrganizationalUnits } from '../ExcludeOrganizationalUnitsModal/ExcludeOrganizationalUnits';
import { GetRuleSetById, SaveRuleSet } from '../../Services/RuleSetService';
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import ToolTipInfo from '../Controls/ToolTipInfo';
import { GetFullOrgTree } from '../../Services/OrgUnitsService'
import { getNodeNames, GroupExcludeReshIds } from '../Controls/DualListBox/DualListBoxUtils'
import SchedulesSaveAsModal from '../Modals/SchedulesSaveAsModal';

import 'react-notifications/lib/notifications.css';
import "react-datepicker/dist/react-datepicker.css";

class AddRuleSet extends Component {
  constructor(props) {
    super(props);
    this.state = {
      addModalShow: false,
      showSaveAsModal: false,
      isLoading: false,
      isvalidateAppointmentTime: false,
      departmentId: null,
      ruleSetName: '',
      sendSMSbeforeDays: null,
      minimumTime: null,
      rulesetExpiryDays: null,
      validateAppointmentTimeFrom: '',
      validateAppointmentTimeTo: '',
      rulesValidPeriodFrom: '',
      rulesValidPeriodTo: '',
      isFromSelected: false,
      isToSelected: false,
      timeIntervalFrom: '',
      timeIntervalTo: '',
      IgnoreSMStoAdmittedPatient: false,
      orgUnits: [],
      orgUnitIds: [],
      orgUnitNames: [],
      actualUnitGidList: [], // all the excluded unitgids which we get and post to server
      isActiveRuleSet: true,
      dateErrorValidFrom: '',
      dateErrorValidTo: '',
      rulesetId: null,
      leafnodeguid: null,
      isEditMode: false,
      rulesetExpired: false,
      completeOrgTree: [],
      validationFailInsendDays: false,
    }

    this.handleAvoidOrgUnits = (orgUnit, unitGids) => {
      var unitIds = orgUnit.map(el => el.id)
      var unitNames = orgUnit.map(el => el.title)
      console.log(unitGids)

      this.setState({
        orgUnits: orgUnit,
        orgUnitIds: unitIds,
        actualUnitGidList: unitGids,
        orgUnitNames: unitNames
      });

    }


    this.handleCheck = e => {
      let boolValue = e.target.checked;

      if (!boolValue) {
        this.setState({
          isvalidateAppointmentTime: e.target.checked,
          validateAppointmentTimeFrom: '',
          validateAppointmentTimeTo: ''
        });
      }
      else {
        this.setState({
          isvalidateAppointmentTime: e.target.checked
        });
      }
    }


    this.handleSMSToAdmittedPatient = e => this.setState({ IgnoreSMStoAdmittedPatient: e.target.checked });

    this.handleMinimumTime = e => {
      let hrsCount = parseInt(e.target.value);
      hrsCount = (hrsCount >= 0 ? hrsCount : null);
      this.setState({ minimumTime: hrsCount });
    }

    this.handlerulesetExpireDays = e => {
      let expDays = parseInt(e.target.value);
      expDays = (expDays >= 0 ? expDays : null);
      this.setState({ rulesetExpiryDays: expDays });
      if (this.state.sendSMSbeforeDays != null && expDays != null && expDays > this.state.sendSMSbeforeDays) {
        this.setState({ validationFailInsendDays: true });
      }
      else {
        this.setState({ validationFailInsendDays: false });
      }
    }

    this.handleValidateAppointmentTimeFrom = e => {
      const AppFromTime = this.formatTime(e.target.value)
      this.setState({ validateAppointmentTimeFrom: AppFromTime });
    }

    this.handleValidateAppointmentTimeTo = e => {
      const AppToTime = this.formatTime(e.target.value)
      this.setState({ validateAppointmentTimeTo: AppToTime });
    }
    this.handleTimeIntervalFrom = e => {
      const IntFromTime = this.formatTime(e.target.value)
      this.setState({ timeIntervalFrom: IntFromTime });
    }
    this.handleTimeIntervalTo = e => {
      const IntToTime = this.formatTime(e.target.value)
      this.setState({ timeIntervalTo: IntToTime });
    }

    this.onChangeIsActive = e => this.setState({ isActiveRuleSet: e.target.checked });

    this.handleRuleSetName = e => this.setState({ ruleSetName: e.target.value });

    this.handleSendSMSbeforeDays = (e) => {
      let daysCount = parseInt(e.target.value);
      daysCount = (daysCount >= 0 ? daysCount : null)

      this.setState({
        sendSMSbeforeDays: daysCount
      });
      if (this.state.rulesetExpiryDays != null && daysCount != null && this.state.rulesetExpiryDays > daysCount) {
        this.setState({ validationFailInsendDays: true });
      }
      else {
        this.setState({ validationFailInsendDays: false });
      }

    }

    this.handlerulesValidPeriodFrom = (dateFrom) => {
      this.setState({
        rulesValidPeriodFrom: dateFrom,
        isFromSelected: true
      })
      if (this.state.isToSelected) {
        this.setState({
          isToSelected: false,
          rulesValidPeriodTo: ""
        })
      }
    }


    this.handlerulesValidPeriodTo = (dateTo) => {
      this.setState({
        rulesValidPeriodTo: dateTo,
        isToSelected: true
      })
    }


    this.handleChangeDepartment = selectedDepartment => {
      this.setState({
        departmentId: selectedDepartment === null ? 0 : selectedDepartment.value,
        orgUnits: [],
        orgUnitIds: [],
        orgUnitNames: [],
        actualUnitGidList: []
      });
    };

    this.handleSaveAsButtonClick = this.handleSaveAsButtonClick.bind(this)
    this.handleCloseSaveAsModal = this.handleCloseSaveAsModal.bind(this)
    this.handleSaveAsSchedule = this.handleSaveAsSchedule.bind(this)
    this.handleClearAllExcludeId = this.handleClearAllExcludeId.bind(this)

    this.handleSaveSchedule = this.handleSaveSchedule.bind(this);
    this.addModalClose = this.addModalClose.bind(this)
  }

  addModalClose() {
    this.setState({ addModalShow: false })
  }

  formatTime(days) {
    const newTime = days.replace(/-/g, ':');
    return newTime.substr(0, 5);
  }

  resetDateFormat(date) {
    if (date != null) {
      var setDate = new Date(date);
      console.log(setDate);
      return setDate;
    }
    else
      return '';
  }

  renderRedirect = () => {
    this.clearAllChanges();
    this.props.history.push("/Overview");
  }

  handleSaveAsButtonClick() {
    this.setState({ showSaveAsModal: true })
  }

  handleCloseSaveAsModal() {
    this.setState({ showSaveAsModal: false })
  }

  handleSaveAsSchedule(e) {
    this.setState({ showSaveAsModal: false })
    if(!this.state.validationFailInsendDays){
    this.saveRuleSet(e)
    }
  }

  handleSaveSchedule = async (e) => {
    e.preventDefault();
  if(!this.state.validationFailInsendDays){
     this.saveRuleSet(e)
  }
   
  }
  handleClearAllExcludeId(){
    //clear unitgids
    this.setState({
      orgUnits: [],
      actualUnitGidList: [],
      orgUnitIds: []
    })
  }

  async componentDidMount() {
    await this.loadRuleSetIntoForm();
    let tree = await GetFullOrgTree()
    this.setState({ completeOrgTree: tree })
  }

  async componentDidUpdate(prevProps, prevState) {
    if (prevState.completeOrgTree.length === 0 && this.state.completeOrgTree.length > 0) {
      console.log("completeOrgTree updated", this.state.completeOrgTree)
      let groupedUnitGids = GroupExcludeReshIds(this.state.completeOrgTree, this.state.actualUnitGidList)
      let orgunitstemp = getNodeNames(this.state.completeOrgTree, groupedUnitGids)
      this.setState({ orgUnits: orgunitstemp })
    }
    else if (this.props.location.state !== prevProps.location.state && (this.props.location.state == null)) {
      this.clearAllChanges();
    }
  }

  async loadRuleSetIntoForm() {
    let storedRSItem = JSON.parse(sessionStorage.getItem('rulesetitem'));
    if (storedRSItem != null) { // set them to the state if found any
      this.setStateFromSavedItem();
    }
    else if (this.props.location.state != null) {
      this.setState({
        leafnodeguid: this.props.location.state.leafnodeguid
      });
      await this.HandleUpateRuleSet(this.props.location.state.leafnodeguid);
    }
  }

  async HandleUpateRuleSet(rulesetGuid) {
    if (rulesetGuid != null) {
      this.setState({ isLoading: true })

      try {

      } catch (error) {
        if (error.statuscode === 401) {
          NotificationManager.error("Session expired! Redirecting to login.", 'Schedule');
          this.props.history.push({
            pathname: '/Login',
            state: { directLogin: false }
          });
        } else {
          let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in loading schedule!");
          NotificationManager.error(`${errorMsg}`, 'Schedule');
        }
      }
      let ruleSet = await GetRuleSetById(rulesetGuid);
      let rsValidFrom = (ruleSet.scheduleValidityPeriodFrom === null) ? null : ruleSet.scheduleValidityPeriodFrom.toString().slice(0, 10);
      let rsValidTo = (ruleSet.scheduleValidityPeriodTo === null) ? null : ruleSet.scheduleValidityPeriodTo.toString().slice(0, 10);
      let excludeUnitgids = ruleSet.excludeOrgUnits.map(unit => {
        return unit.unitID
      })
      let toDate = Moment(rsValidTo, "YYYY-MM-DD");

      this.setState({
        isLoading: false,
        rulesetId: rulesetGuid,
        isEditMode: true,
        ruleSetName: ruleSet.rulesetName,
        departmentId: ruleSet.departmentId,
        isvalidateAppointmentTime: ruleSet.isAppointmentTimeValidate,
        sendSMSbeforeDays: ruleSet.sendBeforeInDays,
        minimumTime: (ruleSet.minimumTimeFromMinutes != null ? (ruleSet.minimumTimeFromMinutes / 60) : null),
        rulesetExpiryDays: ruleSet.expireInDays,
        rulesValidPeriodFrom: this.resetDateFormat(rsValidFrom),//Moment(rsValidFrom).toDate(),
        rulesValidPeriodTo: this.resetDateFormat(rsValidTo),//Moment(rsValidTo).toDate(),
        isFromSelected: (rsValidFrom !== null),
        isToSelected: (rsValidTo !== null),
        validateAppointmentTimeFrom: ruleSet.appointmentFrom != null ? ruleSet.appointmentFrom : "",
        validateAppointmentTimeTo: ruleSet.appointmentTo != null ? ruleSet.appointmentTo : "",
        timeIntervalFrom: ruleSet.sendingTimeIntervalFrom != null ? ruleSet.sendingTimeIntervalFrom : "",
        timeIntervalTo: ruleSet.sendingTimeIntervalTo != null ? ruleSet.sendingTimeIntervalTo : "",
        IgnoreSMStoAdmittedPatient: ruleSet.ignoreSMStoAdmittedPatient,
        isActiveRuleSet: ruleSet.isActive,
        rulesetExpired: Moment().isAfter(toDate),
        actualUnitGidList: excludeUnitgids,
        orgUnitIds: excludeUnitgids,
        orgUnits: excludeUnitgids.map(id => { return { title: id } })
      });
    }
  }

  async saveRuleSet(e) {
    this.setState({ isLoading: true })
    let saveAsCopy = false;
    if (e && typeof e === 'string') {
      saveAsCopy = true
    }

    let ruleSetModel = {
      ScheduleValidityPeriodTo: (this.state.rulesValidPeriodTo === '') ? null : this.state.rulesValidPeriodTo,
      ScheduleValidityPeriodFrom: (this.state.rulesValidPeriodFrom === '') ? null : this.state.rulesValidPeriodFrom,
      minimumTimeFromMinutes: (this.state.minimumTime !== '' ? (this.state.minimumTime * 60) : null),
      SendBeforeInDays: this.state.sendSMSbeforeDays === '' ? null : this.state.sendSMSbeforeDays,
      ExpireInDays: this.state.rulesetExpiryDays === '' ? null : this.state.rulesetExpiryDays,
      AppointmentTo: (this.state.isvalidateAppointmentTime) ? this.state.validateAppointmentTimeTo : null,
      AppointmentFrom: (this.state.isvalidateAppointmentTime) ? this.state.validateAppointmentTimeFrom : null,
      isAppointmentTimeValidate: this.state.isvalidateAppointmentTime,
      DepartmentId: (this.state.departmentId === 0) ? null : this.state.departmentId,
      RulesetName: saveAsCopy ? e : this.state.ruleSetName,
      SendingTimeIntervalFrom: (this.state.timeIntervalFrom === '' ? null : this.state.timeIntervalFrom),
      SendingTimeIntervalTo: (this.state.timeIntervalTo === '') ? null : this.state.timeIntervalTo,
      IgnoreSMStoAdmittedPatient: this.state.IgnoreSMStoAdmittedPatient,
      isActive: this.state.isActiveRuleSet,
      RulesetId: saveAsCopy ? null : this.state.rulesetId,
      AvoidOrgUnitIds: this.state.actualUnitGidList.toString(),
      orgUnits: this.state.orgUnits,
      orgUnitIds: this.state.orgUnitIds,
      orgUnitNames: this.state.orgUnitNames
    }

    sessionStorage.setItem('rulesetitem', JSON.stringify(ruleSetModel));

    try {
      var ruleSetGuid = await SaveRuleSet(ruleSetModel);

      if (ruleSetGuid != null && this.state.isEditMode === false) {
        NotificationManager.success("Save Success!", 'Schedules');
        console.log("saved successfully!");
      }
      if (ruleSetGuid != null && this.state.isEditMode === true) {
        NotificationManager.success("Update Success!", 'Schedules');
        console.log("Updated successfully!");
      }
      this.setState({ isLoading: false })
      this.clearAllChanges();
      this.props.history.push("/SearchSchedule");
    }
    catch (error) {
      console.log(error.message);

      if (error.statuscode === 401) {
        NotificationManager.error("Session expired! Redirecting to login.", 'Schedules');
        this.props.history.push({
          pathname: '/Login',
          state: { directLogin: false }
        });
      }
      else {
        sessionStorage.removeItem('rulesetitem');
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Save/Update Failed");
        NotificationManager.error(errorMsg, 'Schedules');
      }
      this.setState({ isLoading: false })
    }
  }

  render() {
    const items = this.state.orgUnits.map(function (item) {
      return <span key={item.id} style={{ padding: '2px' }}><Button variant="info" disabled> {item.title} </Button></span>;
    });
    return (
      <div>
        <Form onSubmit={this.handleSaveSchedule}>
          <Breadcrumb>
            <Breadcrumb.Item><FormattedMessage id="Schedule" defaultMessage="Schedules" /></Breadcrumb.Item>
            <Breadcrumb.Item active >
              {this.state.isEditMode ? <FormattedMessage id="Edit" defaultMessage="Edit-Schedule" /> : <FormattedMessage id="Create" defaultMessage="Create-Schedule" />}
            </Breadcrumb.Item>

          </Breadcrumb>
          <FormGroup row>
            <Col sm="12" >
              <Card body >
                <Col sm={6} className="offset-3">
                  <DepartmentSelection departmentId={this.state.departmentId} onChangeDepartment={this.handleChangeDepartment} nonEditable={this.state.isEditMode} />
                </Col>

                <Col md={6} className="offset-3">
                  <FormGroup row>
                    <Label md={4}><FormattedMessage id="Text.RuleSetName" defaultMessage="Schedule-Name" /><span className="required">*</span></Label>
                    <Col sm={8}>
                      <Input type="text" name="ruleSetName" id="ruleSetName" placeholder="Schedule Name"
                        value={this.state.ruleSetName} onChange={(text) => { this.handleRuleSetName(text) }} disabled={this.state.isEditMode} required />
                    </Col>
                  </FormGroup>
                </Col>
              </Card>
            </Col>
          </FormGroup>
          <FormGroup row>
            <Col sm="12" >
              <Card body >
                <FormGroup row>
                  <Col md={2}>
                    <Label><FormattedMessage id="SendSMSbefore" defaultMessage="SendSMSbefore" /><span className="required">*</span></Label>
                  </Col>

                  <Col md={2} >
                    <Input type="number" className="form-control" min={0} id="sendSMSbeforeDays"
                      name="sendSMSbeforeDays"
                      value={this.state.sendSMSbeforeDays}
                      onChange={(days) => { this.handleSendSMSbeforeDays(days) }} required />
                  </Col>
                  <Col md={2} >
                    <Label><FormattedMessage id="DaysToTheAppointment" defaultMessage="days to the appointment" /></Label>
                  </Col>
                </FormGroup>

                <FormGroup row>
                  <Col md={2}>
                    <Label id="minimum" >
                      <FormattedMessage id="MinimumTime" defaultMessage="Minimum Time" />
                    </Label>
                    <ToolTipInfo id="minimumTimeToolTip"
                      formatMessageId="TooltipMinimumTime" defaultMessage="If-SMS-is-sending-on-the-same-date-what-should-be-the-minimum-time-gap-between-the-SMS-sending-time-and-the-appointment-time"
                      placement="bottom" />
                  </Col>

                  <Col md={2} >
                    <Input type="number" className="form-control"
                      id="minimumTime" max={23} min={0}
                      value={this.state.minimumTime}
                      onChange={(days) => { this.handleMinimumTime(days) }}
                      required={this.state.sendSMSbeforeDays === 0}>
                    </Input>
                  </Col>
                </FormGroup>

                <FormGroup row>
                  <Col md={2}>
                    <Label id="RulesetExpireDays"><FormattedMessage id="RulesetExpireDays" defaultMessage="Retry Expire Days" /></Label>
                    <ToolTipInfo id="expireDaysToolTip"
                      formatMessageId="TooltipRulesetExpireDays" defaultMessage="Number-of-days-to-Retry-upon-SMS-not-sent-on-scheduled-date."
                      placement="bottom" />
                  </Col>

                  <Col sm={2}>
                    <Input min={0} type="number" step="1"
                      value={this.state.rulesetExpiryDays}
                      onChange={(days) => { this.handlerulesetExpireDays(days) }} />
                  </Col>
                </FormGroup>
                {(!this.state.validationFailInsendDays ? "" : <div style={{ color: "red", fontSize: '14px', fontWeight: '650' }}><FormattedMessage id="ErrorMsgInsendDaysRuleSet" defaultMessage="Retry Expire Days should less than Send SMS before !!" /></div>)}

              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12">
              <Card body>
                <FormGroup row style={{ fontWeight: "bold" }}>
                  <Col md={6}>
                    <Label><FormattedMessage id="SendTimeInterval" defaultMessage="SendTimeInterval" /></Label>
                    <ToolTipInfo id="SendTimeIntervalToolTip"
                      formatMessageId="TooltipSendTimeInterval" defaultMessage="SMS-will-only-send-within-this-duration-of-the-sending-Date"
                      placement="right" />
                  </Col>
                </FormGroup>
                <FormGroup row>
                  <Label sm={2}><FormattedMessage id="Select.TimeFrom" defaultMessage="SendTimeIntervalFrom" /></Label>
                  <Col sm={2}>
                    <InputGroup>
                      <TimeField className="form-control"
                        name="timeIntervalFrom" id="timeIntervalFrom" placeholder="time placeholder"
                        value={this.state.timeIntervalFrom}
                        onChange={(days) => { this.handleTimeIntervalFrom(days) }}
                        style={{ width: 100 }}
                        input={<Input label="From" value={this.state.timeIntervalTo} variant="outlined" />} />
                      <InputGroupAddon addonType="append">
                        <InputGroupText>h</InputGroupText>
                      </InputGroupAddon>
                    </InputGroup>
                  </Col>
                  <Label sm={2} style={{ textAlign: "center" }}><FormattedMessage id="Select.TimeTo" defaultMessage="SendTimeIntervalTo" /></Label>
                  <Col sm={2}>
                    <InputGroup>
                      <TimeField className="form-control"
                        name="timeIntervalTo" id="timeIntervalTo" placeholder="time placeholder"
                        value={this.state.timeIntervalTo}
                        onChange={(days) => { this.handleTimeIntervalTo(days) }}
                        style={{ width: 100 }}
                        input={<Input label="To" value={this.state.timeIntervalFrom} variant="outlined" />} />
                      <InputGroupAddon addonType="append">
                        <InputGroupText>h</InputGroupText>
                      </InputGroupAddon>
                    </InputGroup>
                  </Col>
                </FormGroup>
              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12">
              <Card body>
                <Form.Row>
                  <CustomInput type="switch" id="validateTimeCustomSwitch"
                    name="validateTimeCustomSwitch"
                    label={<FormattedMessage id="Validate.Time"/>}
                    onChange={this.handleCheck}
                    value={this.state.isvalidateAppointmentTime}
                    checked={this.state.isvalidateAppointmentTime} />
                  <ToolTipInfo id="ValidateAppointmentTimeTooltip"
                    formatMessageId="tooltipValidateAppointmentTime" defaultMessage="ValidateAppointmentTime"
                    placement="right" />
                </Form.Row>

                <FormGroup row hidden={!this.state.isvalidateAppointmentTime}>
                  <Label sm={2}><FormattedMessage id="Select.TimeFrom" defaultMessage="From" /><span className="required">*</span></Label>

                  <Col sm={2}>
                    <InputGroup>
                      <TimeField className="form-control" disabled={!this.state.isvalidateAppointmentTime}
                        name="validateAppointmentTimeFrom" id="validateAppointmentTimeFrom" placeholder="time placeholder"
                        value={this.state.validateAppointmentTimeFrom}
                        onChange={(days) => { this.handleValidateAppointmentTimeFrom(days) }}
                        required={this.state.isvalidateAppointmentTime}
                        style={{ width: 100 }}
                        input={<Input label="Valid-From" value={this.state.validateAppointmentTimeFrom} variant="outlined" />}
                      />
                      <InputGroupAddon addonType="append">
                        <InputGroupText>h</InputGroupText>
                      </InputGroupAddon>
                    </InputGroup>
                  </Col>

                  <Label sm={2} style={{ textAlign: "center" }}><FormattedMessage id="Select.TimeTo" defaultMessage="To" /><span className="required">*</span></Label>
                  <Col sm={2}>
                    <InputGroup>
                      <TimeField className="form-control" disabled={!this.state.isvalidateAppointmentTime}
                        name="validateAppointmentTimeTo" id="validateAppointmentTimeTo" placeholder="time placeholder"
                        value={this.state.validateAppointmentTimeTo}
                        onChange={(days) => { this.handleValidateAppointmentTimeTo(days) }}
                        required={this.state.isvalidateAppointmentTime}
                        style={{ width: 100 }}
                        input={<Input label="Valid-To" value={this.state.validateAppointmentTimeTo} variant="outlined" />}
                      />
                      <InputGroupAddon addonType="append">
                        <InputGroupText>h</InputGroupText>
                      </InputGroupAddon>
                    </InputGroup>
                  </Col>

                </FormGroup>

              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12" >
              <Card body >
                <FormGroup row>
                  <Col md={2}>
                    <Label id="AvoidSendingTo"><FormattedMessage id="AvoidSendingTo" defaultMessage="Avoid Sending To" /></Label>
                    <ToolTipInfo id="AvoidSendingToTooltip"
                      formatMessageId="TooltipAvoidSendingTo" defaultMessage="Add-organizational-units-here-if-you-want-to-skip-sending-SMS-to-those"
                      placement="bottom" />
                  </Col>

                  <Col md={6}>
                    <div className="form-control" style={{ minHeight: '200px', maxHeight: '400px', overflowY: 'auto' }}>
                      {items}
                    </div>

                  </Col>
                  <Col md={2}>
                    <Button className="bg-primary offset-1  mb-2" onClick={() => this.setState({ addModalShow: true })}>
                      <FormattedMessage id="Add" defaultMessage="Add" />
                    </Button>
                    <Button className="bg-danger offset-1  mb-2" onClick={this.handleClearAllExcludeId}>
                      <FormattedMessage id="Schedule.Exclude.RemoveAll" defaultMessage="Clear_All"/>
                    </Button>
                  </Col>

                  {this.state.addModalShow ? <ExcludeOrganizationalUnits
                    departmentid={this.state.departmentId}
                    excluded={this.state.orgUnitIds}
                    show={this.state.addModalShow}
                    onHide={this.addModalClose}
                    onSelectAvoidOrgUnits={this.handleAvoidOrgUnits} /> : null}

                </FormGroup>
              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12">
              <Card body>
                <Form.Row>
                  <CustomInput type="switch" id="sendingadmittedCustomSwitch"
                    name="sendAdmittedCustomSwitch"
                    label={<FormattedMessage id="NoSMSToAdmittedPatients"/>}
                    onChange={this.handleSMSToAdmittedPatient}
                    value={this.state.IgnoreSMStoAdmittedPatient}
                    checked={this.state.IgnoreSMStoAdmittedPatient} />
                </Form.Row>
              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12" >
              <Card body>
                <FormGroup row style={{ fontWeight: "bold" }}>
                  <Col sm={6}>
                    <Label><FormattedMessage id="RulesetvalidityPeriod" defaultMessage="RulesetvalidityPeriod" /></Label>
                    <ToolTipInfo id="RulesetvalidityPeriodId"
                      formatMessageId="TooltipRulesetvalidityPeriod" defaultMessage="Active-duration-of-the-SMS-sending-rule."
                      placement="right" />
                  </Col>
                </FormGroup>
                <FormGroup>
                  {(this.state.rulesetExpired ? <span style={{ fontWeight: "bold", color: "red" }}><FormattedMessage id="Ruleset.Expired" defaultMessage="Expired" /></span> : "")}
                </FormGroup>

                <FormGroup row >
                  <Label sm={2}><FormattedMessage id="Select.DateFrom" defaultMessage="RulesetvalidityPeriodFrom" /></Label>
                  <Col xl={2}>
                    <div >
                      <DatePicker name="rulesValidPeriodFrom" id="rulesValidPeriodFrom" isClearable={this.state.isFromSelected}
                        todayButton="Today" placeholderText="Click to select a date"
                        dateFormat="yyyy-MM-dd"
                        showYearDropdown showMonthDropdown
                        selected={this.state.rulesValidPeriodFrom} minDate={(new Date())}
                        value={this.state.rulesValidPeriodFrom} onChange={date => { this.handlerulesValidPeriodFrom(date) }}
                        customInput={<Input autocomplete="off" style={{ width: '190px' }} />}
                      />
                      {(this.state.isFromSelected ? "" : <div style={{ color: "red", fontSize: '12px', fontWeight: '650' }}>Date not selected</div>)}
                    </div>
                  </Col>

                  <Label sm={2} style={{ textAlign: "center" }}><FormattedMessage id="Select.DateTo" defaultMessage="RulesetvalidityPeriodTo" /></Label>
                  <Col sm={2}>
                    <div>
                      <DatePicker name="rulesValidPeriodTo" id="rulesValidPeriodTo" isClearable={this.state.isToSelected}
                        todayButton="Today" placeholderText={(this.state.isFromSelected ? "Click to select a date" : "Select 'From' date first")}
                        dateFormat="yyyy-MM-dd" disabled={!this.state.isFromSelected}
                        showYearDropdown showMonthDropdown
                        selected={this.state.rulesValidPeriodTo} minDate={this.state.rulesValidPeriodFrom}
                        onChange={this.handlerulesValidPeriodTo}
                        customInput={<Input autocomplete="off" style={{ width: '190px' }} />}
                      />
                      {(this.state.isToSelected ? "" : <div style={{ color: "red", fontSize: '12px', fontWeight: '650' }}>Date not selected</div>)}
                    </div>
                  </Col>

                </FormGroup>

                <Form.Row>
                  <CustomInput type="switch" id="isActiveCustomSwitch"
                    name="customSwitch"
                    label={<FormattedMessage id="RuleSet.Active"/>}
                    onChange={this.onChangeIsActive}
                    value={this.state.isActiveRuleSet}
                    checked={this.state.isActiveRuleSet} />
                </Form.Row>
              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <div className="col text-right view-report">
              <Button className=" mr-4 col-md-1" variant="primary" onClick={this.renderRedirect}>
                <FormattedMessage id="AddRuleSetBack" defaultMessage="Cancel" />
              </Button>
              <Button className="bg-primary  col-md-1 mr-4 " variant="primary"
                onClick={this.handleSaveAsButtonClick}
                disabled={!this.state.isEditMode}
                hidden={!this.state.isEditMode}>
                <FormattedMessage id="AddRuleSetSaveAs" defaultMessage="SaveAs" />
              </Button>
              <Button className="bg-primary  col-md-1 mr-4 " variant="primary" type="submit" >
                <FormattedMessage id="AddRuleSetSave" defaultMessage="Save" />
              </Button>
            </div>
          </FormGroup>
        </Form>
        {this.state.showSaveAsModal ? <SchedulesSaveAsModal
          show={this.state.showSaveAsModal}
          onHide={this.handleCloseSaveAsModal}
          onSubmit={this.handleSaveAsSchedule} /> : null}

        {this.state.isLoading ? <div className="overlay"><div className="spinner" /> </div> : null}
      </div>
    )
  }

  clearAllChanges() {
    this.setState({
      addModalShow: false,
      isvalidateAppointmentTime: false,
      departmentId: null,
      ruleSetName: '',
      sendSMSbeforeDays: '',
      minimumTime: '',
      rulesetExpiryDays: '',
      validateAppointmentTimeFrom: '',
      validateAppointmentTimeTo: '',
      rulesValidPeriodFrom: '',
      rulesValidPeriodTo: '',
      isFromSelected: false,
      isToSelected: false,
      timeIntervalFrom: '',
      timeIntervalTo: '',
      IgnoreSMStoAdmittedPatient: false,
      orgUnits: [],
      orgUnitIds: [],
      orgUnitNames: [],
      actualUnitGidList: [],
      isActiveRuleSet: true,
      dateErrorValidFrom: '',
      dateErrorValidTo: '',
      rulesetId: null,
      leafnodeguid: null,
      isEditMode: false,
      rulesetExpired: false,
      validationFailInsendDays: false
    });
    sessionStorage.removeItem('rulesetitem');
  }

  setStateFromSavedItem() {
    //  check if you can find a ruleset stored in local storage
    let storedRSItem = JSON.parse(sessionStorage.getItem('rulesetitem'));
    // set them to the state if found any
    if (storedRSItem != null) {
      let orgUnitslist = [];

      if (storedRSItem.AvoidOrgUnitIds != "") {
        orgUnitslist = storedRSItem.AvoidOrgUnitIds.split(",");
      }

      let toDate = Moment(storedRSItem.ScheduleValidityPeriodTo, "YYYY-MM-DD");

      this.setState({
        rulesetId: storedRSItem.RulesetId,
        isEditMode: (storedRSItem.RulesetId !== null),
        departmentId: storedRSItem.DepartmentId,
        ruleSetName: storedRSItem.RulesetName,
        sendSMSbeforeDays: storedRSItem.SendBeforeInDays,
        minimumTime: (storedRSItem.minimumTimeFromMinutes != null ? (storedRSItem.minimumTimeFromMinutes / 60) : null),
        rulesetExpiryDays: storedRSItem.ExpireInDays,
        isvalidateAppointmentTime: storedRSItem.isAppointmentTimeValidate,
        validateAppointmentTimeFrom: storedRSItem.AppointmentFrom,
        validateAppointmentTimeTo: storedRSItem.AppointmentTo,
        rulesValidPeriodFrom: this.resetDateFormat(storedRSItem.ScheduleValidityPeriodFrom),// Moment(storedRSItem.ScheduleValidityPeriodFrom).toDate(),
        rulesValidPeriodTo: this.resetDateFormat(storedRSItem.ScheduleValidityPeriodTo),//Moment(storedRSItem.ScheduleValidityPeriodTo).toDate(),
        timeIntervalFrom: storedRSItem.SendingTimeIntervalFrom,
        isFromSelected: (storedRSItem.ScheduleValidityPeriodFrom !== null),
        isToSelected: (storedRSItem.ScheduleValidityPeriodTo !== null),
        timeIntervalTo: storedRSItem.SendingTimeIntervalTo,
        IgnoreSMStoAdmittedPatient: storedRSItem.IgnoreSMStoAdmittedPatient,
        orgUnitIds: orgUnitslist,
        rulesetExpired: Moment().isAfter(toDate)
      });
    }
  }

}
export default withRouter(AddRuleSet)