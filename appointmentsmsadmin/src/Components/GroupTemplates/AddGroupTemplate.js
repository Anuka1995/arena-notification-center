import React, { Component } from 'react';
import { Col, Button, FormGroup, Label, Input, Card, Row, ButtonToolbar } from 'reactstrap';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import Form from 'react-bootstrap/Form';
import { withRouter } from 'react-router';
import { FormattedMessage } from "react-intl";
import { NotificationManager } from 'react-notifications';

import DepartmentSelection from '../OrgUnits/DepartmentSelection';
import ConfirmationModal from '../Modals/ConfirmationModal';
import ToolTipInfo from '../Controls/ToolTipInfo'

import './AddGroupTemplate.css';
import "react-datepicker/dist/react-datepicker.css";

import { SaveGroupTemplate, GetGroupedTemplate } from '../../Services/GroupTemplateService';
import { GetPreviewModal } from '../../Services/TextTemplateService';

import { AddTagsModal } from '../Modals/AddTagsModal';
import { PreviewModal } from '../Modals/PreviewModal';

class AddGroupTemplate extends Component {
    constructor(props) {
        super(props);
        this.state = {
            addTagsModalShow: false,
            departmentList: [],
            currentDepartmentId: 0,
            TextTemplateName: "",
            TextTemplateString: "",
            groupTemplatId: null,
            isSaving: false,
            confimationModalShow: false,
            isEditMode: this.templateIdAttached(),
            handleSelectedTags: '',
            selectedTags: '',
            showPreview: false,
            prevResult: "",
            showMessageForAffeted: false,
            handleOnHide: true,
            keepSessionStorage: false,

        }
        this.insertToTextArea = this.insertToTextArea.bind(this);
        this.handleSelectedTags = this.handleSelectedTags.bind(this);
        this.SaveGroupTemplate = this.SaveGroupTemplate.bind(this);
        this.handleTemplateName = this.handleTemplateName.bind(this);
        this.handleTemplateString = this.handleTemplateString.bind(this);
        this.handleCancelChanges = this.handleCancelChanges.bind(this);
        this.handleConfirmationModal = this.handleConfirmationModal.bind(this);
        this.handleFormSubmit = this.handleFormSubmit.bind(this);
        this.handleOn401Error = this.handleOn401Error.bind(this);
        this.handleCloseModal = this.handleCloseModal.bind(this);
        this.handleOnHide = this.handleOnHide.bind(this);

        this.handleChangeDepartment = selectedDepartment => {
            this.setState({
                currentDepartmentId: selectedDepartment != null ? selectedDepartment.value : 0
            });
        };
    }

    handleSelectedTags(concatTagName) {
        this.setState({
            selectedTags: concatTagName,
            TextTemplateString: concatTagName
        });
        this.insertToTextArea(concatTagName);
    }

    handleOnHide(closeModal) {
        this.setState({
            addTagsModalShow: closeModal
        })
    }

    insertToTextArea(concatTagName) {
        const el = document.getElementById('textarea');
        console.log(concatTagName);
        let textToInsert = concatTagName
        let cursorPosition = el.selectionStart
        let textBeforeCursorPosition = el.value.substring(0, cursorPosition)
        let textAfterCursorPosition = el.value.substring(cursorPosition, el.value.length)
        let valueT = textBeforeCursorPosition + textToInsert + textAfterCursorPosition
        this.setState({
            TextTemplateString: valueT
        });
    }
    
    handleTemplateName(text) {
        this.setState({
            TextTemplateName: text.target.value
        })
    }

    handleTemplateString(text) {
        this.setState({
            TextTemplateString: text.target.value
        })
    }

    handleCancelChanges = e => {
        this.handleCloseModal(e)
        this.clearAllChanges();
        this.props.history.push("/SearchGroupTemplate");
    }

    async handleConfirmationModal(e) {
        e.preventDefault();
        this.handleCloseModal(e)
        await this.SaveGroupTemplate(e)
    }

    handleCloseModal = e => {
        this.setState({ confimationModalShow: false })
        this.setState({ showMessageForAffeted: false })
        if (this.state.isEditMode) {
            this.props.history.push("/SearchGroupTemplate");
        }
    }

    handleOn401Error = () => {
        NotificationManager.error("Session expired! Redirecting to login.", 'Group Templates');
        this.setState({
            keepSessionStorage: true
        })
        this.props.history.push({
            pathname: '/Login',
            state: { directLogin: false }
        });
    }

    handleFormSubmit = e => {

        e.preventDefault()
        if (!this.state.isEditMode) {
            if (this.state.TextTemplateString === "") {
                this.setState({ confimationModalShow: true })
            }
            else {
                this.SaveGroupTemplate(e)
            }
        } else {
            var sessionObject = JSON.parse(sessionStorage.getItem('groupedtextitem'));
            var fieldUpdated = (sessionObject.textTemplateString != this.state.TextTemplateString);

            if (fieldUpdated) {
                this.setState({ showMessageForAffeted: true })
            }
            else { this.SaveGroupTemplate(e) }
        }
    }

    SaveGroupTemplate = async (e) => {
        this.setState({ isSaving: true });

        var groupedTemplateContent = {
            TextTemplateTextId: this.state.groupTemplatId,
            DepartmentId: this.state.currentDepartmentId == 0 ? null : this.state.currentDepartmentId,
            TextTemplateName: this.state.TextTemplateName,
            TextTemplateString: this.state.TextTemplateString
        }

        sessionStorage.setItem('groupedtextitem', JSON.stringify(groupedTemplateContent));
        this.setState({ keepSessionStorage: true })
        try {
            var templateGuid = await SaveGroupTemplate(groupedTemplateContent);
            if (templateGuid) {
                this.setState({ isSaving: false });
                NotificationManager.success("Group Template Saved", 'GroupTextTemplate');
                this.clearAllChanges();
                this.props.history.push("/SearchGroupTemplate");
            }
        }
        catch (error) {
            this.setState({ isSaving: false });

            if (error.statuscode === 401) {
                this.handleOn401Error();
            }
            else {
                sessionStorage.removeItem('groupedtextitem');
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Save/Update Failed");
                NotificationManager.error(errorMsg, 'Group Templates');
            }
        }
    }
    PreviewModalFunction = async (e) => {
        try {

            let previewString = await GetPreviewModal(this.state.TextTemplateString, false);
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
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Preview Failed");
                NotificationManager.error(errorMsg, 'SMS Template');
            }
        }
    }

    async loadGroupTemplateIntoForm() {
        //  check if you can find a template stored in local storage
        let storedTextItem = JSON.parse(sessionStorage.getItem('groupedtextitem'));

        if (storedTextItem != null) { // set them to the state if found any
            console.log("loading from the storage");
            this.setStateFromSavedItem();
        }
        else if (this.templateIdAttached()) {
            await this.getGroupTemplate(this.props.location.state.groupedTemplateId);
        }
    }

    async getGroupTemplate(groupedTemplateID) {
        if (this.templateIdAttached()) {
            try {
                let template = await GetGroupedTemplate(groupedTemplateID);
                this.setState({
                    groupTemplatId: template.textTemplateTextId,
                    currentDepartmentId: template.departmentID,
                    TextTemplateName: template.textTemplateName,
                    TextTemplateString: template.textTemplateString
                });

                sessionStorage.setItem('groupedtextitem', JSON.stringify(template));
            } catch (error) {
                console.log(error.message);
                if (error.statuscode === 401) {
                    this.handleOn401Error();
                }
                else {
                    let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Loading template Failed");
                    NotificationManager.error(errorMsg, 'Grouped Template');
                }
            }
        }
    }

    setStateFromSavedItem() {
        //  check if you can find a template stored in local storage
        let storedTextItem = JSON.parse(sessionStorage.getItem('groupedtextitem'));
        // set them to the state if found any
        if (storedTextItem != null) {
            this.setState({
                groupTemplatId: storedTextItem.TextTemplateTextId,
                TextTemplateName: storedTextItem.TextTemplateName,
                TextTemplateString: storedTextItem.TextTemplateString,
                currentDepartmentId: storedTextItem.DepartmentId
            });
        }
    }

    templateIdAttached() {
        return (this.props.location.state != null && this.props.location.state.groupedTemplateId);
    }

    clearAllChanges() {
        this.setState({
            groupTemplatId: null,
            TextTemplateName: "",
            TextTemplateString: "",
            currentDepartmentId: 0,
            prevResult: "",
            isEditMode:false,
            keepSessionStorage: false
        });
        sessionStorage.removeItem('groupedtextitem');
    }

    //#region Lifcycle Methods
    async componentDidMount() {
        await this.loadGroupTemplateIntoForm();
    }

    componentDidUpdate(prevProps) {
        if (this.props.wrappedTagName !== prevProps.wrappedTagName) {
          this.setState({
            selectedTags:this.props.wrappedTagName
          });
        }
        else if(this.props.location.state !==prevProps.location.state && this.props.location.state == null){
            this.clearAllChanges();
        }
        else if(this.props.location.state !==prevProps.location.state && this.props.location.state == null){
            this.clearAllChanges();
        }
    }

    componentWillUnmount(){
        if(!this.state.keepSessionStorage){
            this.clearAllChanges()
        }
    }
    //#endregion

    render() {
        var disable = (this.state.TextTemplateString && this.state.TextTemplateString.length > 4) ? false : true;
        let closPreview = () => this.setState({ showPreview: false });
        return (
            <div>
                <Breadcrumb>
                    <Breadcrumb.Item><FormattedMessage id="GroupTemplates" defaultMessage="Group-Templates" /></Breadcrumb.Item>
                    <Breadcrumb.Item active>
                    {this.state.isEditMode ?<FormattedMessage id="Edit" defaultMessage="Edit"/>:<FormattedMessage id="Create" defaultMessage="create" />}
                    </Breadcrumb.Item>
                </Breadcrumb>
                <Card body>
                    <Form onSubmit={this.handleFormSubmit} return="false">

                        <FormGroup row>
                            <Col sm={12} >
                                <Col md={8}>
                                    <DepartmentSelection departmentId={this.state.currentDepartmentId}
                                        onChangeDepartment={this.handleChangeDepartment}
                                        nonEditable={this.state.isEditMode} />
                                </Col>
                                <Col md={8}>
                                    <FormGroup row>
                                        <Label md={4} sm={6}><FormattedMessage id="GroupTemplateName" defaultMessage="Group-Template-Name" /><span className="required">*</span></Label>
                                        <Col md={8} sm={6}>
                                            <Input type="#" name="#" id="#" placeholder="Enter Template Name"
                                                value={this.state.TextTemplateName}
                                                disabled={this.state.isEditMode}
                                                onChange={this.handleTemplateName} required />
                                        </Col>
                                    </FormGroup>
                                </Col>
                                <FormGroup align="left">
                                    <Row>
                                        <Col md={8}>
                                            <Row style={{ marginLeft: "0px" }}>
                                                <Col md={4}>
                                                    <Row style={{ marginLeft: "0px" }}>
                                                        <Label><FormattedMessage id="GroupTemplate" defaultMessage="Group-Template" /></Label>
                                                        <ToolTipInfo id="GrpTemplateText"
                                                            formatMessageId="GroupTemplateBody" defaultMessage="No-SMS-willbe-sent"
                                                            placement="bottom" />
                                                    </Row>
                                                </Col>
                                                <Col sm={8}>
                                                    <Form.Control as="textarea" rows="4" id="textarea"
                                                        value={this.state.TextTemplateString}
                                                        onChange={this.handleTemplateString} />
                                                </Col>
                                            </Row> 
                                        </Col>
                                        <Col sm={2} md={2} lg={2} >
                                            <Col>
                                                <Button style={{ minWidth: 'max-content'}} className="bg-primary mb-2" onClick={() => this.setState({ addTagsModalShow: true })}>
                                                    <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                                </Button>
                                            </Col>
                                        </Col>
                                    </Row>
                                </FormGroup>
                                <Col md={8}>
                                    <ButtonToolbar>
                                        <Form.Row>
                                            <Button className="bg-primary offset-1 mb-2" onClick={this.PreviewModalFunction} disabled={disable}>
                                                <FormattedMessage id="Preview" defaultMessage="Preview" />
                                            </Button>
                                            <PreviewModal
                                                textMessage={this.state.prevResult}
                                                show={this.state.showPreview}
                                                onHide={closPreview}
                                                textAreaMessage={this.state.TextTemplateString}
                                            />
                                        </Form.Row>
                                    </ButtonToolbar>
                                </Col>

                                

                                {this.state.confimationModalShow ? <ConfirmationModal textmessage={<FormattedMessage id="GroupTemplate.NoSMS" defaultMessage="The Group Template body is empty. No SMS will be sent for this" />}
                                    show={this.state.confimationModalShow}
                                    title="Group Template info"
                                    onHide={this.handleCloseModal}
                                    onSubmit={this.handleConfirmationModal} /> : null}

                                {this.state.showMessageForAffeted ? <ConfirmationModal textmessage={<FormattedMessage id="AffectToallContentTemplate" defaultMessage="Changes will be effected to all other Content Templates who are using this Group Template" />}
                                    show={this.state.showMessageForAffeted}
                                    title="Group Template info"
                                    onHide={this.handleCloseModal}
                                    onSubmit={this.handleConfirmationModal} /> : null}
                            </Col>
                        </FormGroup>
                        <FormGroup row>
                            <div className="col text-right">
                                <Button className="col-md-1 mr-4 " style={{ minWidth: "min-content"}} variant="primary" type="button" onClick={this.handleCancelChanges} >
                                    <FormattedMessage id="ButtonCancel" defaultMessage="Cancel" />
                                </Button>
                                <Button className="bg-primary col-md-1 mr-4" style={{ minWidth: "min-content"}} type="submit">
                                    <FormattedMessage id="ButtonSave" defaultMessage="Save" />
                                </Button>
                            </div>
                        </FormGroup>
                        {this.state.isSaving ? <div className="overlay"><div className="spinner" /> </div> : null}
                    </Form>
                    {this.state.addTagsModalShow ? <AddTagsModal
                                    deptId={this.state.currentDepartmentId}
                                    show={this.state.addTagsModalShow}
                                    onHide={this.handleOnHide}
                                    onSelectTags={this.handleSelectedTags}
                                    on401Error={this.handleOn401Error}
                                /> : null}
                </Card>
            </div>
        );
    }
}

export default withRouter(AddGroupTemplate);