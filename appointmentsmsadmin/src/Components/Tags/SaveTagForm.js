import React, { Component } from 'react';
import { withRouter } from 'react-router';
import { Col, FormGroup, Card, Button } from 'reactstrap';
import { AvForm } from 'availity-reactstrap-validation';
import { NotificationManager } from 'react-notifications';
import { FormattedMessage } from "react-intl";
import TagDetails from './TagDetails';
import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import { SaveTag, GetTag } from '../../Services/TagsService';

var _ = require('underscore');

var TagTypes = [{ Name: "Text", Value: 1, Enabled: true }, 
                { Name: "X-Path", Value: 0, Enabled: false }];

var TagDataTypes = [
  { Name: "Text", Value: 0 }, 
  { Name: "Number", Value: 1 }];

var XpathTagdataTypes = [
  { Name: "Text", Value: 0 }, 
  { Name: "Number", Value: 1 },
  { Name: "Date Time", Value: 2 }];

class SaveTagForm extends Component {
  constructor(props)
  {
    super(props);
    
    this.state={
      tagGuid: props.tagGuid,
      departmentId: null,
      tagName: '',
      tagDescription: '',
      tagValue: '',
      isActive: true,
      tagType: null,
      tagDataType: 0,
      dataTypeArray: TagDataTypes,
      tagTypeArray: TagTypes,
      hideForm: (props.tagGuid === null),
      isLoading: false,
      XpathDataTypeArray:XpathTagdataTypes
    }

    this.handleChangeDepartment = selectedDepartment => {
      
      this.setState({ 
        departmentId: selectedDepartment === null ? 0 : selectedDepartment.value
      });
    };

    this.handleChangeTagName = e => this.setState({ tagName: e.target.value });

    this.handleChangeTagDescription = e => this.setState({ tagDescription: e.target.value });

    this.handleChangeTagValue = e => this.setState({ tagValue: e.target.value });

    this.handleChangeIsActive = e => { this.setState({ isActive: e.target.checked }) };

    this.handleChangeTagDataTypeRadio = ({ target }) => {
      let selectedDataType = _.find(this.state.dataTypeArray, function(t) { return (t.Name === target.value); });
      this.setState({tagDataType: selectedDataType.Value});
    };

    this.handleChangeTagTypeRadio = ({ target }) => {
      let selectedTagType = _.find(this.state.tagTypeArray, function(t) { return (t.Name === target.value); });
      this.setState({tagType: selectedTagType.Value});
    };

    this.saveATag = this.saveATag.bind(this);

    this.handleTagDetailLoad = (tagObject) => { 
      this.clearAllChanges();

      if(tagObject){
        this.setState({
          tagGuid: tagObject.tagId,
          tagName: tagObject.tagName,
          tagValue: tagObject.tagValue,
          departmentId: tagObject.departmentId,
          tagDescription: tagObject.description,
          tagType: tagObject.tagType,
          tagDataType: tagObject.dataType,
          isActive: tagObject.isActive,
          hideForm: (tagObject.tagId === null)
        });
      }
     };
  }
  
  componentDidMount() {
    this.setStateFromSavedItem();
  }

  componentDidUpdate(prevProps) {

    // Typical usage (don't forget to compare props):
    if ((this.props.tagGuid !== prevProps.tagGuid) && this.props.tagGuid ) {
      
      this.getTagDetails(this.props.tagGuid).then(tagObject => {
        this.handleTagDetailLoad(tagObject);
      });
    }

    // Handle the tagGuid null --> to hide the form
    if ((this.props.tagGuid !== prevProps.tagGuid) && (this.props.tagGuid === null) ) {
      this.clearAllChanges();      
    }
  }

  getTagDetails = async(tagGuid) => {
    try {
      var tagObject = await GetTag(tagGuid);
      return tagObject;
    }
    catch(error) {
      console.log(error.message);
      
      if(error.statuscode === 401){
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: {directLogin: false}
        });
      } else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in loading phrase!");
        NotificationManager.error(`${errorMsg}`, 'Phrases');
      }
    }
  }

  saveATag = async(e)=> {
    e.preventDefault();
    this.setState({ isLoading: true})

    // hardcode tag type to 1 if creation. Only satic tags allowed to create.
    var tagTypeToSend = ((this.state.tagGuid==null) ? 1 : this.state.tagType);
    var tagContent = {
      tagId: this.state.tagGuid,
      departmentId: this.state.departmentId,
      tagname: this.state.tagName,
      Description: this.state.tagDescription,
      TagValue: this.state.tagValue,
      TagType: tagTypeToSend,
      DataType: this.state.tagDataType,
      IsActive: this.state.isActive
    }

    // Save the tagContent into local storage 
    sessionStorage.setItem('tagitem', JSON.stringify(tagContent));

    try {
      var tagGuid = await SaveTag(tagContent);
      if(tagGuid){
        this.clearAllChanges();
        NotificationManager.success("Save Success!", 'Phrases');
        console.log("saved successfully!");
        this.props.onSaveSuccess();
      }
      this.setState({ isLoading: false })
    }
    catch(error) {
      if(error.statuscode === 401){
        console.log("Ticket is expired. Redirect To Login.");
        this.props.history.push({
          pathname: '/Login',
          state: {directLogin: false}
        });
      } else {
        let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Error in Phrase Creation!")
        NotificationManager.error(errorMsg, 'Phrases');
      }
      this.setState({ isLoading: false})
    }
  }

  clearAllChanges(){
    // clear the state
    this.setState({
      tagGuid: null,
      tagName: '',
      tagValue: '',
      departmentId: null,
      tagDescription: '',
      tagType: null,
      tagDataType: 0,
      isActive: true,
      hideForm: true
    });
    // clear the local storage
    sessionStorage.removeItem('tagitem');
  }

  setStateFromSavedItem() {
    //  check if you can find a tag stored in local storage
    let storedTagItem = JSON.parse(sessionStorage.getItem('tagitem'));
    // set them to the state if found any
    if(storedTagItem != null) {
      this.setState({
        tagGuid: storedTagItem.tagId,
        tagName: storedTagItem.tagname,
        tagValue: storedTagItem.TagValue,
        departmentId: storedTagItem.departmentId,
        tagDescription: storedTagItem.Description,
        tagType: storedTagItem.TagType,
        tagDataType: storedTagItem.DataType,
        isActive: storedTagItem.IsActive,
        hideForm: (storedTagItem.tagId === null)
      });
    }
  }

  render() {
    if(this.state.hideForm){
      return (<div></div>)
    }
    return (
      <div>
        <FormGroup row>
          <Col sm="12">
            <Card body >
              <AvForm onValidSubmit={this.saveATag}>
                <FormGroup row>
                  <Col md={10}>
                      <DepartmentSelection departmentId={this.state.departmentId} 
                        onChangeDepartment={this.handleChangeDepartment} 
                        nonEditable={(this.state.tagType === 0) || (this.state.tagGuid != null)} />
                  </Col>
                </FormGroup>
                    
                <TagDetails tagName={this.state.tagName} onChangeTagName={this.handleChangeTagName} guid={this.state.tagGuid}
                    tagDesc={this.state.tagDescription} onChangeTagDesc={this.handleChangeTagDescription}
                    tagValue={this.state.tagValue} onChangeTagValue={this.handleChangeTagValue}
                    tagIsActive={this.state.isActive} onChangeIsActive={this.handleChangeIsActive}
                    tagType={this.state.tagType} onChangeTagType={this.handleChangeTagTypeRadio} tagTypes={this.state.tagTypeArray} hideTagType={(this.state.tagGuid==null)}
                    tagDataType={this.state.tagDataType} onChangeTagDataType={this.handleChangeTagDataTypeRadio} tagDataTypes={this.state.dataTypeArray} XpathTagdataTypes={this.state.XpathDataTypeArray}
                    OnSaveCancel={this.props.onSaveCancel}/>

                <FormGroup row>
                  <div className="col text-right view-report">
                    <Button className=" mr-4 " onClick={this.props.onSaveCancel} >
                    <FormattedMessage id="ButtonCancel" defaultMessage="Cancel"/>
                    </Button>
                    <Button className="bg-primary mr-4 " type="submit" disabled={(this.state.tagType === 0)}>
                      <FormattedMessage id="ButtonSave" defaultMessage="Save"/>
                    </Button>
                  </div>
                </FormGroup>
              </AvForm>
            </Card>
          </Col>
        </FormGroup>
        {this.state.isLoading ? <div className="overlay"><div className="spinner" /> </div> : null}
      </div>
    )
  }
}

export default withRouter(SaveTagForm)