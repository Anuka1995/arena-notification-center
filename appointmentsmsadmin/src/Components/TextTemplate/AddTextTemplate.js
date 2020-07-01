import React, { Component } from 'react';
import { Row, Col, Button, FormGroup, Label, ButtonToolbar, Input, Card, CustomInput } from 'reactstrap';
import { NotificationManager } from 'react-notifications';
import { withRouter } from 'react-router';
import Select from 'react-select'
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import Form from 'react-bootstrap/Form';
import { FormattedMessage } from "react-intl";
import DatePicker from "react-datepicker";
import Moment from 'moment';

import { SaveTextTemplate, GetSMSTextTemplateBy } from '../../Services/TextTemplateService'
import { GetRuleSetList, GetAvailableOPDList, GetAvailableSectionList, GetAvailableWardList, GetAvailableLocationList, GetAvailableDepartmentList, CheckIsDepartmentExluded, GetRuleSet } from '../../Services/RuleSetService'
import { GetGroupedTemplates } from '../../Services/GroupTemplateService'
import { GetCareLevelDetails, GetContactTypes } from '../../Services/DipsContactTypeService'
import { GetPreviewModal } from '../../Services/TextTemplateService';
import TextTemplateSaveAsModal from '../Modals/TextTemplateSaveAsModal';
import { AddTagsModal } from '../Modals/AddTagsModal';
import { PreviewModal } from '../Modals/PreviewModal';
import { ConfirmationModal } from '../Modals/ConfirmationModal'
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import ToolTipInfo from '../Controls/ToolTipInfo';

import 'react-notifications/lib/notifications.css';
import './AddTextTemplate.css';

class AddTextTemplate extends Component {
  constructor(props) {
    super(props);    
    this.state = {
      smsTextId: null,
      leafnodeguid: null,
      SelectExistingGroups: false,
      currentGroupTemplateId: null,
      groupTemplates: [],
      textValue: "",
      textBeingEdited: "",
      schedules: [],
      availableDepartments: [],
      locations: [],
      opds: [],
      sections: [],
      wards: [],
      careLevels: [],
      contactTypes: [],
      displaySelectedCareLevels: [],      
      displaySelectedContactTypes: [],
      prevResult: "",
      templateName: "",
      currentDepartmentId: null,
      currentLocationId: null,
      currentSchedule: null,
      currentSectionId: null,
      currentWardId: null,
      currentCareLevels: [],
      currentContactTypes: [],
      pssLinkIncluded: false,
      isVideoAppoinment: false,
      departmentEnable: false,
      handleSelectedTags: '',
      selectedTags: '',
      handleOnHide: true,
      isEditMode: false,
      isLoading: false,
      addTagsModalShow: false,
      showPreview: false,
      confimationModalShow: false,      
      loadingGroupTemplates: false,
      showSaveAsModal: false,
      hasValidatePeriod: false,
      isFromSelected: false,
      isToSelected: false,
      ValidFrom: "",
      ValidTo: "",
      templateIsActive: true,
      errors: [] 
    }

    this.handleTemplateName = e => this.setState({ templateName: e.target.value });
    this.handlePssLink = e => this.setState({ pssLinkIncluded: e.target.checked });
    this.handleIsVideoCheck = e => this.setState({ isVideoAppoinment: e.target.checked });
    this.handleChangeOPD = e => {
      var opdId = parseInt(e.target.value);
      this.setState({
        currentOPDid: (opdId == 0 ? null : opdId)
      });
    }
    this.handleChangeWard = e => {
      var wardId = parseInt(e.target.value);
      this.setState({
        currentWardId: wardId == 0 ? null : wardId
      });
    }
    this.handleChangeLocation = e => {
      var locId = parseInt(e.target.value);
      this.setState({
        currentLocationId: locId,
      });
    }
    this.insertText = this.insertText.bind(this);
    this.handleSelectedTags = this.handleSelectedTags.bind(this);
    this.handleScheduleChange = this.handleScheduleChange.bind(this);
    this.handleChangeDepartment = this.handleChangeDepartment.bind(this);
    this.handleChangeSection = this.handleChangeSection.bind(this);
    this.handleCareLevels = this.handleCareLevels.bind(this);
    this.handleContactTypes = this.handleContactTypes.bind(this);
    this.handleGroupTemplates = this.handleGroupTemplates.bind(this);
    this.handleSelectExistingGroup = this.handleSelectExistingGroup.bind(this);
    this.saveTextTemplate = this.saveTextTemplate.bind(this);
    this.handleOnHide = this.handleOnHide.bind(this);
    this.handleConfirmationModal = this.handleConfirmationModal.bind(this)
    this.handleFormSubmit = this.handleFormSubmit.bind(this)
    this.handleTextArea = this.handleTextArea.bind(this);
    this.handleCloseSaveAsModal = this.handleCloseSaveAsModal.bind(this)
    this.handleSaveAsTextTemplate = this.handleSaveAsTextTemplate.bind(this)
    this.handleSaveAsTextTemplateClicked = this.handleSaveAsTextTemplateClicked.bind(this);
    this.onSaveCancel = this.onSaveCancel.bind(this);
    this.handleOn401Error = this.handleOn401Error.bind(this);
    this.handleSetValidatePeriod = this.handleSetValidatePeriod.bind(this);
    this.handleValidFrom = this.handleValidFrom.bind(this);
  }

  async componentDidMount() {
    this.setState({ isLoading: true })
    var textTemplate = await this.loadSmsTextDetail();
    this.InitSchedules(textTemplate ? textTemplate.rulesetId : null);
  }

  async loadSmsTextDetail() {
    await this.InitCareLevels();
    await this.InitContactTypes();

    let storedSMSTextItem = JSON.parse(sessionStorage.getItem('texttemplateitem'));

    if (storedSMSTextItem != null) {
      console.log("storedSMSTextItem");
      this.setStateFromSavedItem();
    } else if (this.props.location.state != null) {
      console.log("Process GET");
      this.setState({ 
        leafnodeguid: this.props.location.state.leafnodeguid,
        isEditMode: this.props.location.state.leafnodeguid !== null
       });
      var smsTemplate = await this.HandleUpdateSMSText(this.props.location.state.leafnodeguid);
      this.setState({ isLoading: false })
      return smsTemplate
    }
    this.setState({ isLoading: false })
  }

  async HandleUpdateSMSText(leafnodeguid) {
    if (leafnodeguid != null) {
      var smsTextTemplate = await GetSMSTextTemplateBy(leafnodeguid);
      
      let departmentId = smsTextTemplate.departmentId;
      let ruleSetId = smsTextTemplate.rulesetId;
      let sectionId = smsTextTemplate.sectionId;

      this.LoadOrgUnitsFor(ruleSetId, departmentId, sectionId);
      this.UpdateGroupedTemplates(departmentId, null, false, departmentId < 1);

      let selectedOfficialLevelOfCare = this.getOLevelOfCareDisplay(smsTextTemplate.officialLevelOfCare);
      let selectedContactTypes = this.getContactTypeDisplay(smsTextTemplate.contactType);

      this.setState({
        smsTextId: smsTextTemplate.textTemplateId,
        templateName: smsTextTemplate.textTemplateName,
        pssLinkIncluded: smsTextTemplate.isPSSLinkInclude,
        isVideoAppoinment: smsTextTemplate.isVideoAppoinment,
        currentLocationId: smsTextTemplate.locationId,
        currentSectionId: smsTextTemplate.sectionId,
        currentWardId: smsTextTemplate.wardId,
        currentOPDid: smsTextTemplate.opdId,
        currentCareLevels: smsTextTemplate.officialLevelOfCare,
        currentContactTypes: smsTextTemplate.contactType,
        textValue: smsTextTemplate.smsTextTemplate,
        SelectExistingGroups: smsTextTemplate.groupTemplateId ? true : false,
        currentGroupTemplateId: smsTextTemplate.groupTemplateId,
        displaySelectedCareLevels: selectedOfficialLevelOfCare,
        displaySelectedContactTypes: selectedContactTypes,
        hasValidatePeriod: smsTextTemplate.validFrom === null ? false : true,
        ValidFrom: (smsTextTemplate.validFrom === null) ? "" : Moment(smsTextTemplate.validFrom).toDate(),
        ValidTo: (smsTextTemplate.validTo === null) ? "" : Moment(smsTextTemplate.validTo).toDate(),
        isFromSelected: (smsTextTemplate.validFrom === null ? false : true),
        isToSelected: (smsTextTemplate.validTo === null ? false : true),
        templateIsActive: smsTextTemplate.isActive
        //have to be done
      });
      return smsTextTemplate
    }
  }

  handleSelectedTags(concatTagName) {
    this.setState({
      selectedTags: concatTagName,
      textValue: concatTagName
    });

    this.insertText(concatTagName);

  }
  handleOnHide(closeModal) {
    this.setState({
      addTagsModalShow: closeModal
    })
  }
  insertText(concatTagName) {
    const el = document.getElementById('textareaId');
    let textToInsert = " " + concatTagName
    let cursorPosition = el.selectionStart
    console.log(el.selectionStart);
    let textBeforeCursorPosition = el.value.substring(0, cursorPosition)
    let textAfterCursorPosition = el.value.substring(cursorPosition, el.value.length)
    let valueT = textBeforeCursorPosition + textToInsert + textAfterCursorPosition
    console.log(valueT);
    this.setState({
      textValue: valueT
    });
  }
  //#region handle events

  handleTextArea = e => {
    this.setState({
      textValue: e.target.value
    });
  }
  handleScheduleChange = async (e) => {
    var rulseset = this.state.schedules.find(c => c.rulesetId == e.target.value);
    
    if (rulseset != null) {
      this.GetAvailableOPDListAsync(rulseset.departmentId, rulseset.rulesetId);
      let enableDepartment = (rulseset.departmentId == null ? true : false);

      if (rulseset.departmentId != null) {

        let isExcluded = this.CheckDepartmentIdExcluded(rulseset.departmentId, rulseset.rulesetId);
        isExcluded.then(
          result => {
            if (!result) {
              this.GetAvailableLocationListAsync(rulseset.departmentId, rulseset.rulesetId);
              this.GetAvailableSectionListAsync(rulseset.departmentId, rulseset.rulesetId);
              this.GetAvailableWardListAsync(rulseset.departmentId, null, rulseset.rulesetId);
              this.UpdateGroupedTemplates(rulseset.departmentId, null, false, false);
              this.setState({
                currentDepartmentId: rulseset.departmentId,
              });
            } else {
              this.setState({
                currentDepartmentId: null,
                sections: [],
                wards: [],
                locations: [],
                opds: []
              });
            }
          }
        )
      }
      else {
        this.GetAvailableDepartmentListAsync(rulseset.rulesetId);
        this.UpdateGroupedTemplates(rulseset.departmentId, null, false, true);
        //if schedule is in hospital level clear the sublevels of departments
        this.setState({
          currentDepartmentId: rulseset.departmentId,
          sections: [],
          wards: [],
          locations: []
        })
      }

      this.setState({
        currentSchedule: rulseset.rulesetId,
        departmentEnable: enableDepartment,
        currentGroupTemplateId: null,
        textValue: "",
        SelectExistingGroups: false,
      });
    }
    else if (rulseset == undefined) {
      this.setState({
        currentDepartmentId: null,
        currentSchedule: null,
        opds: [],
        departmentEnable: false,
        sections: [],
        wards: [],
        locations: [],
        groupTemplates: [],
        SelectExistingGroups: false,
        currentGroupTemplateId: null,
        textValue: ""
      });
    }
  }

  handleChangeDepartment = async (e) => {
    let departmentId = e === null ? null : e.value

    this.setState({
      currentDepartmentId: departmentId
    })

    console.log(this.state.currentSchedule);
    if (e != null) {

      this.UpdateGroupedTemplates(departmentId, null, false, false);
      this.setState({
        currentGroupTemplateId: null,
        textValue: ""
      })

      this.GetAvailableOPDListAsync(departmentId, this.state.currentSchedule);
      this.GetAvailableSectionListAsync(departmentId, this.state.currentSchedule);
      this.GetAvailableLocationListAsync(departmentId, this.state.currentSchedule);
      this.GetAvailableWardListAsync(departmentId, null, this.state.currentSchedule);
    }
    else {
      // clear out other org unit drop downs EXCEPT opd
      console.log("null department id selected !");
      this.GetAvailableOPDListAsync(departmentId, this.state.currentSchedule);
      this.UpdateGroupedTemplates(departmentId, null, false, true);

      this.setState({
        currentGroupTemplateId: null,
        locations: [],
        sections: [],
        wards: [],
        textValue: ""
      });
    }
  };

  async CheckDepartmentIdExcluded(departmentId, scheduleId) {
    let isExclueded = await CheckIsDepartmentExluded(departmentId, scheduleId);
    return Boolean(isExclueded);
  }

  async GetAvailableDepartmentListAsync(scheduleId) {
    let deps = await GetAvailableDepartmentList(scheduleId);
    this.setState({ availableDepartments: deps })
  }

  async GetAvailableOPDListAsync(departmentId, scheduleId) {
    let opds = await GetAvailableOPDList(departmentId, scheduleId)
    this.setState({ opds: opds })
  }

  async GetAvailableSectionListAsync(departmentId, scheduleId) {
    let sections = await GetAvailableSectionList(departmentId, scheduleId)
    
    this.setState({ sections: sections })
  }

  async GetAvailableLocationListAsync(departmentId, scheduleId) {
    let opds = await GetAvailableLocationList(departmentId, scheduleId)
    this.setState({ locations: opds })
  }
  async GetAvailableWardListAsync(departmentId, sectionId, scheduleId) {
    let opds = await GetAvailableWardList(departmentId, sectionId, scheduleId)
    this.setState({ wards: opds })
  }

  handleChangeSection = async (e) => {
    var selectionId = parseInt(e.target.value);
    let wardsList = [];

    this.setState({
      currentSectionId: selectionId,
      wards: wardsList,
      currentWardId: null
    });

    if (selectionId === 0 || selectionId == null) {
      wardsList = await GetAvailableWardList(this.state.currentDepartmentId, null, this.state.currentSchedule);
    }
    else {
      wardsList = await GetAvailableWardList(this.state.currentDepartmentId, selectionId, this.state.currentSchedule);
    }

    this.setState({
      wards: wardsList
    });
  }

  handleCareLevels = selectedCareLevels => {
    var selected = [];
    if (selectedCareLevels != null) {
      selected = selectedCareLevels.map(c => c.value);
    }
    this.setState({
      displaySelectedCareLevels: selectedCareLevels,
      currentCareLevels: selected
    });
  }

  handleContactTypes = selectedContactTypes => {    
    var selected = [];
    if (selectedContactTypes != null) {
      selected = selectedContactTypes.map(c => c.value);
    }
    this.setState({
      displaySelectedContactTypes: selectedContactTypes,
      currentContactTypes: selected
    });
  }

  handleGroupTemplates = selectedGroupTemplate => {
    var selectionId = selectedGroupTemplate == null ? null : selectedGroupTemplate.value;
    var text = selectedGroupTemplate == null ? "" : selectedGroupTemplate.text;

    if (selectionId != null) {
      let errorItems = this.state.errors;
      errorItems['grouptemplate'] = '';
      this.setState({
        errors: errorItems
      });
    }
    this.setState({
      currentGroupTemplateId: selectionId,
      textValue: text,
    });
  }

  handleSetValidatePeriod = e => {
    this.setState({ hasValidatePeriod: !this.state.hasValidatePeriod })
  }

  handleValidFrom = (dateFrom) => {

    this.setState({
        ValidFrom: dateFrom,
        isFromSelected: true
    })
    if (this.state.isToSelected) {
        this.setState({
            isToSelected: false,
            ValidTo: ""
        })
    }
  }

  handleValidTo = (dateTo) => {
    this.setState({
        ValidTo: dateTo,
        isToSelected: true
    })
  }

  formatDate(date) {
    if(date === "" || date === null){
      const newDate = Moment(null);
      return newDate;
    }
    else{
      const newDate = Moment(date, 'YYYY-MM-DD').format().slice(0, 10);
      return newDate;
    }
  }

  handleSelectExistingGroup = async (e) => {
    // save the pre editing tempalte to a state if it was set to not to select from existng
    let tempTemplate = "";
    if (!this.state.SelectExistingGroups && this.state.textValue != null) {
      tempTemplate = this.state.textValue;
    }


    // get the bool value from checkbox
    let selectFromExisting = e.target.checked;
    if (selectFromExisting) {  // if select exisitng set the text value to ""        
      this.setState({
        textValue: "",
        currentGroupTemplateId: null,
        textBeingEdited: tempTemplate,
        SelectExistingGroups: selectFromExisting // set checked value to SelectExistingGroups
      });
    }
    else {  // else set the text value to presaved text   
      let errorItems = this.state.errors;
      errorItems['grouptemplate'] = '';
      let beignEdited = this.state.textBeingEdited;
      this.setState({
        errors: errorItems,
        textValue: beignEdited,
        currentGroupTemplateId: null,
        SelectExistingGroups: selectFromExisting
      });
    }
  }

  handleFormSubmit = e => {
    e.preventDefault()
    if (this.CheckIsGroupTemplateNotSelected()) {
      console.log('Group text template shoud be selected if check mark is checked.');
     
    } else {
      if (this.checkIsEmptySMSTemplate()) {
        this.setState({
          confimationModalShow: true
        })
      }
      else {
        this.saveTextTemplate(e)
      }
    }
  }

  handleConfirmationModal = e => {
    e.preventDefault();
    this.handleCloseModal(e);
    this.saveTextTemplate(e);
  }

  handleCloseModal = e => {
    this.setState({ confimationModalShow: false })
  }

  handleSaveAsTextTemplateClicked = e => {
    this.setState({ showSaveAsModal: true })
  }

  onSaveCancel = (e) => {
    this.clearInputs();
    this.props.history.push("/Overview");
  }

  handleSaveAsTextTemplate(e) {
    this.setState({ showSaveAsModal: false })
    this.saveTextTemplate({ SaveAs: true, TemplateName: e })
  }

  handleCloseSaveAsModal = e => {
    this.setState({ showSaveAsModal: false })
  }

  handleTemplateIsActive = e => {
    this.setState({ templateIsActive: !this.state.templateIsActive })

}
  //#endregion

  //#region getMethods
  UpdateGroupedTemplates(departmentId, searchText, showInactive, isHospitalLevel) {
    this.setState({ loadingGroupTemplates: true })

    GetGroupedTemplates(departmentId, searchText, showInactive, isHospitalLevel)
      .then(result => {
        if (result != null) {
          this.setState({ groupTemplates: result })
        }
        this.setState({ loadingGroupTemplates: false })
      })
  }

  InitSchedules = async (id) => {
    try {
      var rulesets = []
      if(id){
        var ruleset = await GetRuleSet(id)
        rulesets = [ruleset]
      }
      else{
        rulesets = await GetRuleSetList();
      }
      
      rulesets.sort((a, b) => (a.rulesetName.toLowerCase() > b.rulesetName.toLowerCase()) ? 1 : -1)
      if (rulesets != null) {
        this.setState({
          schedules: rulesets
        });
      }
    }
    catch (error) {
      console.log(error.message);
      if (error.statuscode === 401) {
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: { directLogin: false }
        });
      } else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in loading schedules!");
        NotificationManager.error(`${errorMsg}`, 'Text Template');
      }
    }
  }

  InitCareLevels = async () => {
    try {
      let careLevels = await GetCareLevelDetails();
      this.setState({
        careLevels: careLevels
      });
    }
    catch (error) {
      console.log(error.message);
      if (error.statuscode === 401) {
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: { directLogin: false }
        });
      } else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in loading dips official care levels!");
        NotificationManager.error(`${errorMsg}`, 'Text Template');
      }
    }
  }

  InitContactTypes = async () => {
    try {
      let contacttypes = await GetContactTypes();
      this.setState({
        contactTypes: contacttypes
      });
    }
    catch (error) {
      console.log(error.message);
      if (error.statuscode === 401) {
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: { directLogin: false }
        });
      } else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in loading dips contact types!");
        NotificationManager.error(`${errorMsg}`, 'Text Template');
      }
    }
  }

  setStateFromSavedItem() {
    //  check if you can find a template stored in local storage
    let storedTextTmplItem = JSON.parse(sessionStorage.getItem('texttemplateitem'));

    // set them to the state if found any
    if (storedTextTmplItem != null) {
      this.LoadOrgUnitsFor(storedTextTmplItem.RulesetId,
        storedTextTmplItem.DepartmentId,
        storedTextTmplItem.SectionId);

      this.UpdateGroupedTemplatesReload(storedTextTmplItem.DepartmentId, null, false, storedTextTmplItem.DepartmentId < 1, 
        storedTextTmplItem.GroupTemplateId,
        storedTextTmplItem.SelectExistingGroups,
        storedTextTmplItem.SMSTextTemplate);

      let selectedOfficialLevelOfCare = this.getOLevelOfCareDisplay(storedTextTmplItem.OfficialLevelOfCare);
      let selectedContactTypes = this.getContactTypeDisplay(storedTextTmplItem.ContactType);

      this.setState({     
        smsTextId: storedTextTmplItem.TextTemplateId,
        templateName: storedTextTmplItem.TextTemplateName,
        pssLinkIncluded: storedTextTmplItem.isPSSLinkInclude,
        isVideoAppoinment: storedTextTmplItem.IsVideoAppoinment,
        currentLocationId: storedTextTmplItem.LocationId,
        currentSectionId: storedTextTmplItem.SectionId,
        currentWardId: storedTextTmplItem.WardId,
        currentOPDid: storedTextTmplItem.OPDId,
        currentCareLevels: storedTextTmplItem.OfficialLevelOfCare,
        currentContactTypes: storedTextTmplItem.ContactType,
        displaySelectedCareLevels: selectedOfficialLevelOfCare,
        displaySelectedContactTypes: selectedContactTypes,
        hasValidatePeriod: (storedTextTmplItem.ValidFrom != null && storedTextTmplItem.ValidTo != null),
        isFromSelected: (storedTextTmplItem.ValidFrom === null ? false : true),
        isToSelected: (storedTextTmplItem.ValidTo === null ? false : true),
        ValidFrom: Moment(storedTextTmplItem.ValidFrom).toDate(),
        ValidTo: Moment(storedTextTmplItem.ValidTo).toDate(),
        templateIsActive: storedTextTmplItem.IsActive
      });
    }
  }
  //#endregion

  saveTextTemplate = async (e) => {
    this.setState({ isLoading: true })

    var savebutton = document.getElementById("saveButton")
    var saveCopy = e.SaveAs
    savebutton.disabled = true;

    var textTemplate = {
      TextTemplateId: this.state.isEditMode && !saveCopy ? this.state.smsTextId : null,
      DepartmentId: this.state.currentDepartmentId === 0 ? null : this.state.currentDepartmentId,
      TextTemplateName: saveCopy ? e.TemplateName : this.state.templateName,
      isPSSLinkInclude: this.state.pssLinkIncluded,
      LocationId: this.state.currentLocationId < 1 ? null : this.state.currentLocationId,
      SectionId: this.state.currentSectionId === 0 ? null : this.state.currentSectionId,
      WardId: this.state.currentWardId == 0 ? null : this.state.currentWardId,
      OPDId: this.state.currentOPDid == 0 ? null : this.state.currentOPDid,
      SMSTextTemplate: this.state.SelectExistingGroups ? "" : this.state.textValue,
      RulesetId: this.state.currentSchedule,
      OfficialLevelOfCare: this.state.currentCareLevels,
      ContactType: this.state.currentContactTypes,
      GroupTemplateId: this.state.SelectExistingGroups ? this.state.currentGroupTemplateId : null,
      SelectExistingGroups: this.state.SelectExistingGroups,
      IsVideoAppoinment: this.state.isVideoAppoinment,
      ValidFrom: this.state.hasValidatePeriod ? this.formatDate(this.state.ValidFrom) : null,
      ValidTo: this.state.hasValidatePeriod ? this.formatDate(this.state.ValidTo) : null,
      isFromSelected: (this.state.ValidFrom === null ? false : true),
      isToSelected: (this.state.ValidTo === null ? false : true),
      IsActive: this.state.templateIsActive
    }

    sessionStorage.setItem('texttemplateitem', JSON.stringify(textTemplate));

    try {
      var templateGuid = await SaveTextTemplate(textTemplate);

      if (templateGuid) {
        NotificationManager.success("TextTemplate Saved", 'SMSTextTemplate');
        savebutton.disabled = false;
        this.clearInputs();
        this.props.history.push("/SearchTextTemplate");
      }
      
      this.setState({ isLoading: false});
    }
    catch (error) {
      savebutton.disabled = false;

      if (error.statuscode === 401) {
        this.handleOn401Error();
      }
      else {
        sessionStorage.removeItem('texttemplateitem');
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Save/Update Failed");
        NotificationManager.error(errorMsg, 'Text Template');
      }
      this.setState({ isLoading: false })
    }
  }

  PreviewModalFunction = async (e) => {
    try {

      let previewString = await GetPreviewModal(this.state.textValue, this.state.pssLinkIncluded);
      this.setState({
        prevResult: previewString,
        showPreview: true
      });
    }
    catch (error) {
      console.log(error.message);
      if (error.statuscode === 401) {
        this.handleOn401Error();
      } else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in loading preview!");
        NotificationManager.error(`${errorMsg}`, 'Text Template');
      }
    }
  }

  AddTagsModalFunction = async (e) => {
    this.setState({
      addTagsModalShow: true
    });
  }

  handleOn401Error = () => {
    NotificationManager.error("Session expired! Redirecting to login.", 'Group Templates');
    this.props.history.push({
      pathname: '/Login',
      state: { directLogin: false }
    });
  }

  CheckIsGroupTemplateNotSelected() {
    let errors = {};
    if (this.state.SelectExistingGroups) {
      if (this.state.currentGroupTemplateId === null) {
        errors['grouptemplate'] = "*Please select a group template.";
        this.setState({
          errors: errors
        });
        return true;
      }
    }
    return false;
  }

  checkIsEmptySMSTemplate() {

    //if selected to add text grom GT
    if(this.state.SelectExistingGroups){
      // check the attached group tempalte content
      if(this.state.currentGroupTemplateId === null)
          return true;

      const found = this.state.groupTemplates.find(element => element.textTemplateTextId === this.state.currentGroupTemplateId);
      if((this.state.currentGroupTemplateId != null) && found &&  (found.textTemplateString === null || found.textTemplateString === "")) {
        return true;
      }

      return false;
      
    } else {  // else if selected to add text
      if (this.state.textValue === null || this.state.textValue === "") {
        return true;
      }
      return false;     
    }
  }

  clearInputs() {
    this.setState({
      leafnodeguid: null,
      currentSchedule: null,
      templateName: "",
      currentDepartmentId: null,
      currentLocationId: null,
      currentOPDid: null,
      currentSectionId: null,
      currentWardId: null,
      currentCareLevels: [],
      currentContactTypes: [],
      opds: [],
      sections: [],
      wards: [],
      locations: [],
      groupTemplates: [],
      displaySelectedCareLevels: null,
      displaySelectedContactTypes: null,
      SelectExistingGroups: false,
      currentGroupTemplateId: null,
      pssLinkIncluded: false,
      prevResult: "",
      textValue: "",
      isEditMode: false,
      schedules: [],
      isVideoAppoinment: false,
      hasValidatePeriod: false,
      isFromSelected: false,
      isToSelected: false,
      ValidFrom: "",
      ValidTo: "",
      templateIsActive: true
    });

    sessionStorage.removeItem('texttemplateitem');
  }

  componentDidUpdate(prevProps) {
    if (this.props.wrappedTagName !== prevProps.wrappedTagName) {
      this.setState({
        selectedTags: this.props.wrappedTagName
      });
    }
    else if(this.props.location.state !==prevProps.location.state && this.props.location.state == null){
      this.clearInputs();
      this.InitSchedules();
    }
  }


//#region Supportive Functions for Handlers on Data Load for Edit
  LoadOrgUnitsFor(rulesetid, departmentid, sectionid) {
    this.setState({
      currentSchedule: rulesetid,
      currentDepartmentId: departmentid
    });
    this.GetAvailableOPDListAsync(departmentid, rulesetid);

    if (departmentid != null) {
      this.GetAvailableLocationListAsync(departmentid, rulesetid);
      this.GetAvailableSectionListAsync(departmentid, rulesetid);
      this.GetAvailableWardListAsync(departmentid, sectionid, rulesetid);
    } else {
      this.GetAvailableDepartmentListAsync(rulesetid);
      this.setState({
        departmentEnable: true,
        sections: [],
        locations: [],
        wards: []
      });
    }
  }

  UpdateGroupedTemplatesReload(departmentId, searchText, showInactive, isHospitalLevel, groupId, isExisting, textTmplValue) {
    this.setState({ loadingGroupTemplates: true })

    GetGroupedTemplates(departmentId, searchText, showInactive, isHospitalLevel)
      .then(result => {
        if (result != null) {
          let textTempalteValue = "";
          if(isExisting) {
            textTempalteValue = "textTemplateTextId";
            var found = result.find(function(element) {
              return (element.textTemplateTextId === groupId);
            });
            textTempalteValue = found.textTemplateString;
          }
          this.setState({ 
            groupTemplates: result,
            textValue: (isExisting ? textTempalteValue : textTmplValue),
            currentGroupTemplateId: groupId,
            SelectExistingGroups: isExisting,
          })
        }
        this.setState({ loadingGroupTemplates: false })
      })
  }

  getOLevelOfCareDisplay(careItems) {
    var selectedOfficialLevelOfCare = null
    if (careItems) {
        selectedOfficialLevelOfCare = this.state.careLevels.filter(c =>
          careItems.includes(c.officialLevelOfCareId)
        );
        selectedOfficialLevelOfCare = selectedOfficialLevelOfCare.map(c => {
          return {
            value: c.officialLevelOfCareId,
            label: c.officialLevelOfCareName
          }
        });
      }
    return selectedOfficialLevelOfCare;
  }

  getContactTypeDisplay(contactTypeItems) {
    var selectedContactTypes = null
      if (contactTypeItems) {
        selectedContactTypes = this.state.contactTypes.filter(c =>
          contactTypeItems.includes(c.contactTypeId)
        )

        selectedContactTypes = selectedContactTypes.map(c => {
          return {
            value: c.contactTypeId,
            label: c.contactTypeName
          }
        })
      }
      return selectedContactTypes;
  }
//#endregion

  render() {
    //#region render code
    var isValidFromEmpty = this.state.ValidFrom === null ? true : false;
    var disable = (this.state.textValue && this.state.textValue.length > 4) ? false : true;
    var scheduleOptions = this.state.schedules.map(s => (<option key={s.rulesetId} value={s.rulesetId}>{s.rulesetName}</option>))
    var opdOptions = this.state.opds.map(o => (<option key={o.opdid} value={o.opdid}>{o.opdDisplayName}</option>))
    var sectionOptions = this.state.sections.map(s => (<option value={s.sectionId}>{s.sectionDisplayName}</option>))
    var wardOptions = this.state.wards.map(w => (<option value={w.wardId}>{w.wardDisplayName}</option>))
    var locationOptions = this.state.locations.map(l => (<option key={l.locationId} value={l.locationId}>{l.locationDisplayName}</option>))
    
    var sectionOptions = this.state.sections.map(s => (<option key={s.sectionId} value={s.sectionId}>{s.sectionDisplayName}</option>))

    var departmentListOption = this.state.availableDepartments.map(({ departmentId, departmentName }) => {
      return {
        value: departmentId,
        label: departmentName
      }
    });
    
    var groupTemplateOptions = this.state.groupTemplates.map(({ textTemplateTextId, textTemplateString, textTemplateName }) => {
      return {
        value: textTemplateTextId,
        label: textTemplateName,
        text: textTemplateString
      }
    })
    
    var groupTemplateSelected = null;
    if (this.state.currentGroupTemplateId != null) {
      groupTemplateSelected = groupTemplateOptions.find(department =>
          department.value == this.state.currentGroupTemplateId
      )
    }
    var careLevelOptions = this.state.careLevels.map(({ officialLevelOfCareId, officialLevelOfCareName }) => {
      return {
        value: officialLevelOfCareId,
        label: officialLevelOfCareName
      }
    });

    var contactTypeOptions = this.state.contactTypes.map(({ contactTypeId, contactTypeName }) => {
      return {
        value: contactTypeId,
        label: contactTypeName
      }
    });    

    let closPreview = () => this.setState({ showPreview: false })    
    //#endregion
    return (
      <div style={{ margin: '10px' }}>
        <Form id="submitForm" onSubmit={e => this.handleFormSubmit(e)}>
          <Breadcrumb>
            <Breadcrumb.Item><FormattedMessage id="SMSContentTemplates" defaultMessage="SMS-content-templates" /></Breadcrumb.Item>
            {this.state.isEditMode ? <Breadcrumb.Item active><FormattedMessage id="Edit" defaultMessage="Edit" /></Breadcrumb.Item>
              : <Breadcrumb.Item active><FormattedMessage id="Create" defaultMessage="Create" /></Breadcrumb.Item>}
          </Breadcrumb>
          <FormGroup row>
            <Col sm="12" >
              <Card body >
                <div className="offset-3">
                  <FormGroup row>
                    <Label sm={3}><FormattedMessage id="Schedule" defaultMessage="Schedule" /><span className="required">*</span></Label>
                    <Col sm={5}>
                      <Input type="select" name="#" id="ruleSetId" value={this.state.currentSchedule} onChange={this.handleScheduleChange} disabled={this.state.isEditMode} required>
                        <option value="" id="0" >Select Schedule...</option>
                        {scheduleOptions}
                      </Input>
                    </Col>
                  </FormGroup>
                  <FormGroup row>
                    <Label sm={3}><FormattedMessage id="Name" defaultMessage="Name" /><span className="required">*</span></Label>
                    <Col sm={5}>
                      <Input type="#" name="#" id="#" placeholder="Enter Template Name" required onChange={this.handleTemplateName} value={this.state.templateName} disabled={this.state.isEditMode} />
                    </Col>
                  </FormGroup>
                </div>
              </Card>
            </Col>
          </FormGroup>
          <FormGroup row>
            <Col md={12} lg={7} >
              <Card body style={{ height: "100%" }} align="left">
                <Row>
                  <Col md={12} sm={12} lg={6}>
                    <FormGroup>
                      <DepartmentSelection departmentId={this.state.currentDepartmentId} 
                          onChangeDepartment={this.handleChangeDepartment} 
                          nonEditable={(this.state.departmentEnable === false)} 
                          depOptions={departmentListOption} isReload={true} />               
                    </FormGroup>
                  </Col>
                  <Col md={12} sm={12} lg={6}>
                    <FormGroup>
                      <Row>
                        <Label md={4} lg={3}><FormattedMessage id="Location" defaultMessage="Location" /></Label>
                        <Col sm={8} lg={8}>
                          <Input type="select" name="select" id="locationId"
                            value={this.state.currentLocationId}
                            onChange={this.handleChangeLocation}>
                            <option value="0">Select Location...</option>
                            {locationOptions}
                          </Input>
                        </Col>
                      </Row>
                    </FormGroup>
                  </Col>
                </Row>
                <FormGroup>
                  <Row>
                  <Label md={4} lg={2}><FormattedMessage id="OPD" defaultMessage="OPD" /></Label>
                  <Col md={8} lg={4}>
                    <Input type="select" name="select"
                      value={this.state.currentOPDid}
                      onChange={this.handleChangeOPD} >
                      <option value="0">Select OPD here...</option>
                      {opdOptions}
                    </Input>
                  </Col>
                  </Row>
                </FormGroup>
                <FormGroup row>
                  <Label md={4} lg={2}><FormattedMessage id="Section" defaultMessage="Section" /></Label>
                  <Col md={8} lg={4}>
                    <Input type="select" name="select" id="sectionsId"
                      value={this.state.currentSectionId}
                      onChange={this.handleChangeSection}>
                      <option value="0">Select Section Name...</option>
                      {sectionOptions}
                    </Input>
                  </Col>
                </FormGroup>
                <FormGroup row>
                  <Label md={4} lg={2}><FormattedMessage id="Ward" defaultMessage="Ward" /></Label>
                  <Col md={8} lg={4}>
                    <Input type="select" name="select" id="wardId"
                      value={this.state.currentWardId}
                      onChange={this.handleChangeWard}>
                      <option value="0">Select ward Name...</option>
                      {wardOptions}
                    </Input>
                  </Col>
                </FormGroup>

              </Card>
            </Col>
            <Col md={12} lg={5} align="right">
              <Card body style={{ height: "100%" }} align="left">
                <FormGroup row>
                  <Label sm={6}><FormattedMessage id="OfficialLevelOfCare" defaultMessage="Official-Level-Of-Care" /></Label>
                  <Col sm={6}>
                    <Select isMulti={true} options={careLevelOptions} id="careLevels" key="careLevels"
                      value={this.state.displaySelectedCareLevels}
                      onChange={this.handleCareLevels} />
                  </Col>
                </FormGroup>
                <FormGroup row>
                  <Label sm={6}><FormattedMessage id="ContactType" defaultMessage="Contact-Type" /></Label>
                  <Col sm={6}>
                    <Select isMulti={true} id="contactTypes" key="contatType"
                      value={this.state.displaySelectedContactTypes}
                      onChange={this.handleContactTypes}
                      options={contactTypeOptions} />
                  </Col>
                </FormGroup>
              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12">
              <Card body>
                <Form.Row>
                  <CustomInput type="switch" id="isVideoSwitch"
                    name="isVideoSwitch"
                    label={<FormattedMessage id="TextTemplate.IsVideo"></FormattedMessage>}
                    onChange={this.handleIsVideoCheck}
                    value={this.state.isVideoAppoinment}
                    checked={this.state.isVideoAppoinment} />
                </Form.Row>
              </Card>
            </Col>
            
          </FormGroup>

          <FormGroup row>
            <Col sm="12" >
              <Card body >
                <FormGroup as='Col' controlid="#">
                  <Form.Row>
                    <Form.Check type="checkbox" label={<FormattedMessage id="SelectGroupTemplate" defaultMessage="SelectFromExistingGroups" />}
                      onChange={this.handleSelectExistingGroup}
                      checked={this.state.SelectExistingGroups}
                      value={this.state.SelectExistingGroups} />
                  </Form.Row>
                  <FormGroup row>
                    <Col sm={4} className="offset-3" hidden={!this.state.SelectExistingGroups}>    
                    <Select type="select" name="select" id="groupTemplateId"
                        placeholder={<FormattedMessage id="SelectGroupTemplatePlaceHolder" defaultMessage="Select_Group_Template" />}
                        isLoading={this.state.loadingGroupTemplates}
                        isClearable={true}
                        onChange={this.handleGroupTemplates}
                        value={groupTemplateSelected}
                        options={groupTemplateOptions} />                      
                      <div className="errorMsg">{this.state.errors.grouptemplate}</div>                       
                    </Col>
                  </FormGroup>
                </FormGroup>
                <FormGroup row>
                  <Label sm={3}><FormattedMessage id="TextTemplate" defaultMessage="Text-Template" /></Label>
                  <Col sm={6}>
                    <Form.Control as="textarea" id="textareaId" rows="4" value={this.state.textValue} onChange={this.handleTextArea}
                      disabled={this.state.SelectExistingGroups} />
                  </Col>
                  <Col sm={4} lg={3}>
                    <Button className="bg-primary offset-1  mb-2" style={{ minWidth: "max-content"}} onClick={this.AddTagsModalFunction} disabled={this.state.SelectExistingGroups}>
                      <FormattedMessage id="AddPhrase" defaultMessage="Add-Phrase" />
                    </Button></Col>
                </FormGroup>
                <FormGroup as='Col' controlid="#">
                  <Form.Row>
                    <Form.Check type="checkbox" label="Include Patient Self Service Link"
                      checked={this.state.pssLinkIncluded}
                      value={this.state.pssLinkIncluded} onChange={this.handlePssLink} />
                  </Form.Row>
                </FormGroup>
                <ButtonToolbar>
                  <Form.Row>
                    <Button className="bg-primary offset-1 mb-2" onClick={this.PreviewModalFunction} disabled={disable}>
                      <FormattedMessage id="Preview" defaultMessage="Preview" />
                    </Button>
                    <PreviewModal
                      textMessage={this.state.prevResult}
                      show={this.state.showPreview}
                      onHide={closPreview}
                     textAreaMessage={this.state.textValue}
                    />
                  </Form.Row>
                </ButtonToolbar>

              </Card>
            </Col>
          </FormGroup>

          <FormGroup row>
            <Col sm="12">
              <Card body>
                <Form.Row>
                  <CustomInput type="switch" id="isActiveCustomSwitch"
                      name="customSwitch"
                      label={<FormattedMessage id="GroupTemplate.ValidityPeriod"></FormattedMessage>}
                      onChange={this.handleSetValidatePeriod}
                      value={this.state.hasValidatePeriod}
                      checked={this.state.hasValidatePeriod}
                  />
                  <ToolTipInfo id="GroupTemplateValidityTooltipId"
                      formatMessageId="GroupTemplateValidPeriod" defaultMessage="Restrict-Template-to-be-active-for-specific-time-period"
                      placement="right" />
                </Form.Row>

                <Col md={11}>
                  {this.state.hasValidatePeriod ? <FormGroup row className="offset-3">
                      <FormGroup>
                          <Label><FormattedMessage id="Select.From" defaultMessage="GroupTemplateValidityPeriodFrom" /></Label>
                          <div >
                              <DatePicker name="date" id="#" isClearable={this.state.isFromSelected} required
                                  todayButton="Today" placeholderText="Click to select a date"
                                  dateFormat="yyyy-MM-dd"
                                  showYearDropdown showMonthDropdown
                                  selected={this.state.ValidFrom} minDate={(new Date())}
                                  value={this.state.ValidFrom} onChange={date => { this.handleValidFrom(date) }}
                                  customInput={<Input />}
                              />
                              {(this.state.isFromSelected ? "" : <div style={{ color: "red", fontSize: '12px', fontWeight: '650' }}>Date not selected</div>)}
                          </div>
                      </FormGroup>
                      <FormGroup className="offset-1">
                          <Label><FormattedMessage id="Select.To" defaultMessage="RulesetvalidityPeriodTo" /></Label>
                          <div>
                              <DatePicker name="date" id="#" isClearable={this.state.isToSelected} 
                                  todayButton="Today" placeholderText={(this.state.isFromSelected ? "Click to select a date" : "Select 'From' date first")}
                                  dateFormat="yyyy-MM-dd" disabled={isValidFromEmpty}
                                  showYearDropdown showMonthDropdown
                                  selected={this.state.ValidTo} minDate={this.state.ValidFrom}
                                  value={this.state.ValidTo} onChange={date => { this.handleValidTo(date) }}
                                  customInput={<Input />}
                              />
                              {(this.state.isToSelected ? "" : <div style={{ color: "red", fontSize: '12px', fontWeight: '650' }}>Date not selected</div>)}
                          </div>
                      </FormGroup>
                  </FormGroup> : null
                  }
                </Col>
                <Form.Row>
                      <CustomInput type="switch" id="templateActiveswitch"
                          name="templateAcitiveswitch"
                          label={<FormattedMessage id="GroupTemplate.templateIsActive" defaultMessage="Template is active"></FormattedMessage>}
                          onChange={this.handleTemplateIsActive}
                          value={this.state.templateIsActive}
                          checked={this.state.templateIsActive}
                      />
                </Form.Row>
              </Card>
            </Col>
          </FormGroup>

          <Form.Row>
            <div className="col text-right view-report">
              <Button className="col-md-2 col-lg-1 mr-4" style={{ minWidth: "fit-content"}} onClick={this.onSaveCancel} >
                <FormattedMessage id="ButtonCancel" defaultMessage="Cancel"/>
              </Button>
              <Button className="bg-primary col-md-2 col-lg-1 mr-4 " variant="primary" id="saveAsButton" value="SaveAs"
                onClick={this.handleSaveAsTextTemplateClicked}
                hidden={!this.state.isEditMode}>
                <FormattedMessage id="ButtonSaveAs" defaultMessage="ButtonSaveAs" />
              </Button>
              <Button className="bg-primary col-md-2 col-lg-1 mr-4" variant="primary" type="submit" id="saveButton" name="saveButton" >
                <FormattedMessage id="ButtonSave" defaultMessage="ButtonSave" />
              </Button>
            </div>
          </Form.Row>
        </Form>

        {this.state.addTagsModalShow ? <AddTagsModal
          deptId={this.state.currentDepartmentId}
          show={this.state.addTagsModalShow}
          onHide={this.handleOnHide}
          onSelectTags={this.handleSelectedTags}
          on401Error={this.handleOn401Error} /> : null}

        {this.state.showSaveAsModal ? <TextTemplateSaveAsModal show={this.state.showSaveAsModal}
          onHide={this.handleCloseSaveAsModal}
          onSubmit={(e) => this.handleSaveAsTextTemplate(e)} /> : null}


        {this.state.confimationModalShow ? <ConfirmationModal
          textmessage={<FormattedMessage id="TextTemplate.NoSMS" defaultMessage="Txt Template body is empty. No SMS will be sent for this" />}
          show={this.state.confimationModalShow}
          title="SMS Content Template info"
          onHide={this.handleCloseModal}
          onSubmit={this.handleConfirmationModal} /> : null}

        {this.state.isLoading ? <div className="overlay"><div className="spinner" /> </div> : null}
      </div>
    );
  }
}

export default withRouter(AddTextTemplate);