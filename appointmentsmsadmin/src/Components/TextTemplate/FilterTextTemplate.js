import React, { Component } from 'react';
import { Col, Button, Label, FormGroup, Input, form, ButtonToolbar, CardText, Card, Container, Row, CustomInput } from 'reactstrap';
import { FormattedMessage } from 'react-intl';
import Form from 'react-bootstrap/Form';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import { NotificationManager } from 'react-notifications';

import { FilterTextTemplatBy } from '../../Services/TextTemplateService';
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import {GetRuleSetList,GetRuleSetById,GetAvailableDepartmentList,GetAvailableOPDList,CheckIsDepartmentExluded,GetAvailableWardList,GetAvailableSectionList,GetAvailableLocationList} from '../../Services/RuleSetService';



class FilterTextTemplate extends Component {
    constructor(props) {
        super(props);
        this.state = {
            currentDepartmentId: null,
            isNonEditable: false,
            schedules: [],
            SectionList: [],
            OPDList: [],
            SectionList: [],
            WardList: [],
            locationList:[],
            OPDId: null,
            WardId: null,
            ruleSetId:null,
            sectionId:null,
            locationId:null,
            contactTypes:null,
            officialLevelofcare:null,
            textTemplateName:null,
            textTemplateId:null,
            locationId:null,
            availableDepartments: [],

        }
        this.handleChangeDepartment = this.handleChangeDepartment.bind(this);
        this.SelectedOPD = this.SelectedOPD.bind(this);
        this.handleWard = this.handleWard.bind(this);
        this.handleRuleSet = this.handleRuleSet.bind(this);
        this.handleSection = this.handleSection.bind(this);
        this.FilterSmsTextTemplate =this.FilterSmsTextTemplate.bind(this);
        this.GetDepByRuleSet = this.GetDepByRuleSet.bind(this);
        this.clearChanges = this.clearChanges.bind(this);
        this.handleLocation = this.handleLocation.bind(this);
    }
    //handle Department
    handleChangeDepartment = selectedDepartment => {
        
        var depId = selectedDepartment === null ? null : selectedDepartment.value
        this.setState({
            currentDepartmentId: depId
        });
        if(selectedDepartment != null){
            this.GetAvailableOPDListAsync(depId, this.state.ruleSetId);
            this.GetAvailableSectionListAsync(depId, this.state.ruleSetId);
            this.GetAvailableWardListAsync(depId, null, this.state.ruleSetId);
        }
    }
    //OPD handling   
    SelectedOPD(e) {
        let selectedId = e.target.value;
        this.setState({
            OPDId: (selectedId == 0 ? null : selectedId)
        });

        if(this.state.WardId !== null){
            this.setState({  WardId: 0 });
        }
    }
    
    
    handleRuleSet(e) {
        let currentRuleSetId = e.target.value;       
        this.setState({ 
            ruleSetId:(currentRuleSetId == 0 ? null : currentRuleSetId) 
        });

        if(currentRuleSetId != 0){
            this.GetDepByRuleSet(currentRuleSetId);
        }
        else{
            //No rule set selected. 
            console.log("NO rule Set selected");
            this.setState({ 
                OPDList: [],
                SectionList: [],
                WardList: [],
                currentDepartmentId:null,
                isNonEditable:false
            });
        }
    }
    //Hnalde Section
    handleSection = async (e) => {
        let currentSectionId = e.target.value;
        this.setState({ 
            sectionId: (currentSectionId == 0 ? null : currentSectionId) ,
            WardId: null
        });
        if (currentSectionId != 0) {
            var wardList = await GetAvailableWardList(this.state.currentDepartmentId, currentSectionId, this.state.ruleSetId);
            this.setState({ 
                OPDId: null,
                OPDList: [],
                WardList :wardList,
            });
        } else {
            this.GetAvailableWardListAsync(this.state.currentDepartmentId, null, this.state.ruleSetId);
            this.GetAvailableOPDListAsync(this.state.currentDepartmentId, this.state.ruleSetId);
        }
    }
    //Directing to Edit page when click on resulted template
    handleClickOnResultTemplate = (e, templateId) => {
        e.preventDefault();
        console.log(templateId);
        this.props.history.push({
            pathname: '/TextTemplate',
            state: {
                leafnodeguid: templateId,
            }
        });
    };
    
    //Handle Ward
    handleWard(e) {
        let currentWardId = e.target.value;
        this.setState({ 
            WardId: (currentWardId == 0 ? null : currentWardId) 
        });

        if(this.state.OPDId !== null){
            console.log("opd is selected");
            this.setState({ OPDId: 0 });
            
        }
    }

    //Hanlde location
    handleLocation(e) {
        let currentlocatioId = e.target.value;
        this.setState({ 
            locationId: (currentlocatioId == 0 ? null : currentlocatioId) 
        });

        if(this.state.OPDId !== null){
            console.log("opd is selected");
            this.setState({ OPDId: 0 });
            
        }
    }
    async GetAvailableDepartmentListAsync(scheduleId) {
        let deps = await GetAvailableDepartmentList(scheduleId);
        this.setState({ availableDepartments: deps })
    }
     async GetAvailableWardListAsync(departmentId, sectionId, scheduleId) {
        let wards = await GetAvailableWardList(departmentId, sectionId, scheduleId)
        this.setState({ WardList: wards })
      }
    async GetAvailableOPDListAsync(departmentId, GetOPDListscheduleId) {
        console.log("get Hospital level OPDs");
        let opdsList = await GetAvailableOPDList(departmentId, GetOPDListscheduleId)
        this.setState({ OPDList: opdsList })
    }
    async GetAvailableSectionListAsync(departmentId, scheduleId) {
        let sections = await GetAvailableSectionList(departmentId, scheduleId)
        
        this.setState({ SectionList: sections })
      }
    async GetAvailableLocationListAsync(departmentId, scheduleId) {
        let locationList = await GetAvailableLocationList(departmentId, scheduleId)
        this.setState({locationList: locationList })
    }

    async CheckDepartmentIdExcluded(departmentId, scheduleId) {
        let isExclueded = await CheckIsDepartmentExluded(departmentId, scheduleId);
        return Boolean(isExclueded);
      }

    GetDepByRuleSet(currentRuleSetId){
        GetRuleSetById(currentRuleSetId).then((ruleSet) => {
       if(ruleSet.departmentId != null){
        let isExcluded = this.CheckDepartmentIdExcluded(ruleSet.departmentId, currentRuleSetId);
        isExcluded.then(
            result => {
              if (!result) {
                this.GetAvailableSectionListAsync(ruleSet.departmentId, currentRuleSetId);
                this.GetAvailableWardListAsync(ruleSet.departmentId, null, currentRuleSetId);
                this.GetAvailableOPDListAsync(ruleSet.departmentId, this.state.ruleSetId);
                this.GetAvailableLocationListAsync(ruleSet.departmentId, currentRuleSetId);
                this.setState({
                  currentDepartmentId: ruleSet.departmentId,  isNonEditable:true
                });
              } else {
                this.setState({
                  currentDepartmentId: null,
                  sections: [],
                  wards: [],
                  opds: []
                });
              }
            }
        )
       }
        else{
            console.log("Hospital level Rule Set selected");
            this.setState({ 
                isNonEditable:false,
                currentDepartmentId:null,
                SectionList: [],
                WardList: [],
                OPDId: null,
                WardId: null,
                sectionId:null,
              }); 
              //Load Deps By hospital
              this.GetAvailableDepartmentListAsync(currentRuleSetId);
              this.GetAvailableOPDListAsync(null,currentRuleSetId);
        }
     });
   }  
   clearChanges = async(e) =>{
    this.setState({ 
        ruleSetId:'',
        currentDepartmentId:'',
        OPDId: '',
        WardId: '',
        sectionId:'',
        isNonEditable:false,
        textTemplateName:''
      });
   }
    FilterSmsTextTemplate = async(e)=> {
        this.setState({textTemplateName:''});
        this.setState({textTemplateId:''});
        
        try{
            console.log("FilterSmsTextTemplate called");
            var textString = await FilterTextTemplatBy(this.state.ruleSetId,this.state.currentDepartmentId,this.state.sectionId,this.state.OPDId,this.state.WardId,this.state.locationId,this.state.contactTypes,this.state.officialLevelofcare);
            
            if(textString === ''){
                NotificationManager.info("No results Found!!!", 'Filter SmsText Template');
            }
            else{
                this.setState({textTemplateName:textString.textTemplateName});
                this.setState({textTemplateId:textString.textTemplateId});

            }
          

        }catch(error){

            if(error.statuscode === 401){
                NotificationManager.error("Session expired! Redirecting to login.", 'Schedules');
                this.props.history.push({
                  pathname: '/Login',
                  state: {directLogin: false}
                });
              }
              else{
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Filter Text Template Failed");
                NotificationManager.error(errorMsg, 'Text Templates Filter');
              }
        }

    }

    InitSchedules = async () => {
        try {
          var rulesets = await GetRuleSetList();
          if (rulesets != null) {
            this.setState({
              schedules: rulesets
            });
          }
          
        }
        catch (error) {
          console.log(error.message);
          NotificationManager.error(error.message, 'Schedules');
        }
      }

      async componentDidMount(){
        this.setState({ isLoading: true })
        this.InitSchedules();
    }
    
    render(){
        var scheduleOptions = this.state.schedules.map(s => (<option key={s.rulesetId} value={s.rulesetId}>{s.rulesetName}</option>))
        let OPDOptions = this.state.OPDList.map(v => (<option value={v.opdid}>{v.opdDisplayName}</option>));
        let SectionOptions = this.state.SectionList.map(v => (<option value={v.sectionId}>{v.sectionDisplayName}</option>));
        let locationOPtions = this.state.locationList.map(v =>(<option value={v.locationId}>{v.locationDisplayName}</option>));
        let WardOptions = this.state.WardList.map(v => (<option value={v.wardId}>{v.wardDisplayName}</option>));
        var departmentListOption = this.state.availableDepartments.map(({ departmentId, departmentName }) => {
            return {
              value: departmentId,
              label: departmentName
            }
          });
        return(
            <Form onSubmit={this.FilterSmsTextTemplate}>
                <Breadcrumb>
                    <Breadcrumb.Item><FormattedMessage id="SMSContentTemplates" defaultMessage="SMS-content-templates" /></Breadcrumb.Item>
                    <Breadcrumb.Item active><FormattedMessage id="FilteredBy" defaultMessage="Filtered-By" /></Breadcrumb.Item>
                </Breadcrumb>
                <FormGroup row>
                    <Col sm="6" >
                        <Card body >
                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="Text.RuleSetName" defaultMessage="Schedule-Name" /><span className="required">*</span></Label>
                                <Col md={8}>
                                    <Input type="select" name="Schedule" placeholder="Select Schedule Name.."  onChange={text => { this.handleRuleSet(text)}} value={this.state.ruleSetId}>
                                        <option value="0" >No Schedule Selected</option>
                                        {scheduleOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <DepartmentSelection departmentId={this.state.currentDepartmentId} nonEditable={this.state.isNonEditable} onChangeDepartment={text=> {this.handleChangeDepartment(text)}} depOptions={departmentListOption}/>

                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="Section" defaultMessage="Section" /></Label>
                                <Col md={8}>
                                    <Input type="select" name="Section" placeholder="Search section Name.." onChange={text=> {this.handleSection(text)}} value={this.state.sectionId}>
                                        <option value="0" >No Section Selected</option>
                                        {SectionOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="OPD" defaultMessage="Outdoor patient department(OPD)" /></Label>
                                <Col md={8}>
                                    <Input type="select" name="OPD" placeholder="Search OPD Name.." value={this.state.OPDId} onChange={this.SelectedOPD}>
                                        <option value="0" >No OPD Selected</option>
                                        {OPDOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="Ward" defaultMessage="Ward" /></Label>
                                <Col md={8}>
                                    <Input type="select" name="Ward" placeholder="Search section Name.." value={this.state.WardId} onChange={this.handleWard}>
                                        <option value="0" >No Ward Selected</option>
                                       {WardOptions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4}><FormattedMessage id="Location" defaultMessage="Location" /></Label>
                                <Col md={8}>                               
                                    <Input type="select" name="Location" id="#" placeholder="Search Location" value={this.state.locationId} onChange={this.handleLocation}>
                                    <option value="0" >No Location Selected</option>
                                    {locationOPtions}
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4} ><FormattedMessage id="OfficialLevelOfCare" defaultMessage="OfficialLevelOfCare" /></Label>
                                <Col md={8}>                               
                                    <Input type="select" name="OfficialLevelOfCare" id="#" placeholder="Search Official level of care" >
                                    <option value="0" >Not Selected</option>

                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                                <Label md={4} ><FormattedMessage id="ContactType" defaultMessage="ContactType" /></Label>
                                <Col md={8}>                               
                                    <Input type="select" name="ContactType" id="#" placeholder="Contact Type" >
                                    <option value="0" >No Contact Type Selected</option>
                                    </Input>
                                </Col>
                            </FormGroup>

                            <FormGroup row>
                           <div className="col text-right view-report" >
                                <Button className="mr-4 col-md-2-sm-3"  onClick={this.clearChanges}  >
                                <FormattedMessage id="ClearAll" defaultMessage="Clear-All" /></Button>
                                <Button className="btn" color="primary" variant="primary" onClick={this.FilterSmsTextTemplate}>
                                 <FormattedMessage id="ButtonSearch" defaultMessage="Search" /></Button>
                            </div>
                                
                            </FormGroup>
                        </Card >
                    </Col>
                    <Col sm="6" >
                        {<div className="col-md-6" >
                            <div style={{ textAlign: 'left' }}>
                           {(this.state.textTemplateName != '' &&  this.state.textTemplateName != null) ?<span style={{paddingRight:'10px'}}> <FormattedMessage id="ContentTemplate" defaultMessage="ContentTemplate"/></span>  :""}
                            <a href="#" onClick={(e) => { this.handleClickOnResultTemplate(e, this.state.textTemplateId)} }>{this.state.textTemplateName}</a>
                            </div>
                        </div> }
                    </Col>
                </FormGroup>
            </Form>
        );
    }
   
}


export default FilterTextTemplate;