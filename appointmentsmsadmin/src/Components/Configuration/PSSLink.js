import React, { Component } from 'react';
import { withRouter } from 'react-router';
import { Col, Button, FormGroup, Label, Input, Card, Row } from 'reactstrap';
import Form from 'react-bootstrap/Form';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import { FormattedMessage } from "react-intl";
import { NotificationManager } from 'react-notifications';

import ToolTipInfo from '../Controls/ToolTipInfo';
import ConfirmationModal from '../Modals/ConfirmationModal';

import { SaveHospitalURL, GetHospitalURL } from '../../Services/PSSLinkService';

class PSSLink extends Component {
    constructor(props) {
        super(props)
        this.state = {
            hospitalURL: '',              //'http://localhost:3000/PSSLink',
            id: "",
            name: "",
            active: true,
            confimationModalShow: false,  //to show confirmation modal
            loadSaving: false,            //show loader
            isEditMode: false,            //enable edit mode
            keepSessionStorage: false
        }

        this.handleHospitalURL = this.handleHospitalURL.bind(this);
        // this.handleConfirmationModal = this.handleConfirmationModal(this);
        this.handleCloseModal = this.handleCloseModal.bind(this);
        this.setEditableMode = this.setEditableMode.bind(this);
        this.handleCancelChanges = this.handleCancelChanges.bind(this);
        this.handleOn401Error = this.handleOn401Error.bind(this);
        //  this.handleFormSubmit = this.handleFormSubmit(this);
        // this.SavePSSLink = this.SavePSSLink.bind(this);
    }


    handleHospitalURL = (url) => {
        this.setState({
            hospitalURL: url.target.value
        });
    }

    ///////Confirmation Modal before saving///////
    handleConfirmationModal = async (e) => {
        console.log('handle confirmation works..');
        e.preventDefault();
        this.handleCloseModal();
        await this.SavePSSLink(e);

    }
    handleCloseModal() {
        this.setState({ confimationModalShow: false });
    }

    ///////Enables Edit Mode///////
    setEditableMode = () => {
        this.setState({
            isEditMode: true
        })
    }

    clearAllChanges() {
        this.setState({
            confimationModalShow: false,
            loadSaving: false,
            isEditMode: false,
            keepSessionStorage: false
        })
        sessionStorage.removeItem('PSSUrlContent');
    }

    ///////Cancel Changes///////
    handleCancelChanges() {
        if (this.state.isEditMode) {
            var confirm = window.confirm("This will cancel all the changes?");
            if (confirm) window.location.reload(true);
        } else {
            this.props.history.push("/Overview");
        }
        //this.props.history.push("/PSSLink");
    }

    handleOn401Error = () => {
        NotificationManager.error("Session expired! Redirecting to login.", 'PSS Link');
        this.setState({
            keepSessionStorage: true
        })
        this.props.history.push({
            pathname: '/Login',
            state: { directLogin: false }
        });
    }

    handleFormSubmit = async (e) => {
        e.preventDefault();
        console.log("handleFormSubmit...");
        if (this.state.hospitalURL != null) {
            this.setState({
                confimationModalShow: true
            })
            //this.SavePSSLink(e);
        }
        else alert("Not Submitted");
    }

    SavePSSLink = async (e) => {
        this.setState({ isSaving: true });
        let PSSLinkContent = {
            Link: this.state.hospitalURL,
            Id: this.state.id,
            Name: this.state.name,
            active: this.state.active
        }

        sessionStorage.setItem('PSSUrlContent', JSON.stringify(PSSLinkContent));
        this.setState({ keepSessionStorage: true })
        try {
            let savedUrlStatus = await SaveHospitalURL(PSSLinkContent);
            if (savedUrlStatus) {
                this.setState({ isSaving: false })
                NotificationManager.success("URL successfully updated", 'PSSUrlUpdate')
                this.clearAllChanges();
                this.props.history.push("/PSSLink");
            }
        }
        catch (error) {
            this.setState({ isSaving: false });
            if (error.statuscode === 401) {
                this.handleOn401Error();
            }
            else {
                sessionStorage.removeItem('PSSUrlContent');
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Save/Update Failed");
                NotificationManager.error(errorMsg, 'UrlUpdateFail');
            }
        }

    }

    async loadPSSLink() {
        let storedURL = JSON.parse(sessionStorage.getItem('PSSUrlContent'));
        if (storedURL != null) {
            console.log("Loading from local storage");
            this.setStateFromStorage();
        }
        else {
            await this.getPSSLink();
        }
    }
    setStateFromStorage() {
        let storedURL = JSON.parse(sessionStorage.getItem('PSSUrlContent'));
        if (storedURL != null) {
            this.setState({
                hospitalURL: storedURL.Link,
                id: storedURL.Id,
                name: storedURL.Name,
                active: storedURL.active
            })
        }
    }

    async getPSSLink() {
        let getUrl = await GetHospitalURL();
        this.setState({
            hospitalURL: getUrl.link,
            id: getUrl.id,
            name: getUrl.name,
            active: getUrl.active
        })
    }

    async componentDidMount() {
        await this.loadPSSLink();
    }

    componentWillMount() {
        this.clearAllChanges();
    }


    render() {
        return (
            <div>
                <Breadcrumb>
                    <Breadcrumb.Item><FormattedMessage id="Configuration" defaultMessage="Configuration" /></Breadcrumb.Item>
                    <Breadcrumb.Item active>
                        <FormattedMessage id="PSSLink" defaultMessage="PSS Hospital URL" />
                    </Breadcrumb.Item>
                </Breadcrumb>
                <Card body>
                    <Form onSubmit={this.handleFormSubmit} return="false">
                        <FormGroup row>
                            <Col sm={12} >
                                {/* <Col md={8}>
                                    <FormGroup row>
                                        <Label md={4} sm={6}><FormattedMessage id="DownloadURL" defaultMessage="Download-URL" /><span className="required">*</span></Label>
                                        <Col md={8} sm={6}>
                                            <Input type="#" name="#" id="#" placeholder="Download URL"
                                                value={this.state.hospitalURL}
                                                onChange={this.handleHospitalURL} />
                                        </Col>
                                    </FormGroup>
                                </Col> */}
                                <FormGroup align="left">
                                    <Row>
                                        <Col md={8}>
                                            <Row style={{ marginLeft: "0px" }}>
                                                <Col md={4}>
                                                    <Row style={{ marginLeft: "0px" }}>
                                                        <Label><FormattedMessage id="DownloadURL" defaultMessage="Download-URL" /></Label>
                                                        <ToolTipInfo id="PSSLinkText"
                                                            formatMessageId="PSSLinkBody" defaultMessage="Download hospital URL"
                                                            placement="bottom" />
                                                    </Row>
                                                </Col>
                                                <Col sm={8}>
                                                    <Form.Control as="textarea" rows="4" id="textarea" disabled={!this.state.isEditMode}
                                                        value={this.state.hospitalURL}
                                                        onChange={this.handleHospitalURL} />
                                                </Col>
                                            </Row>
                                        </Col>
                                    </Row>
                                </FormGroup>
                                {this.state.confimationModalShow ?
                                    <ConfirmationModal textmessage={<FormattedMessage id="PSSConfirmation" defaultMessage={"Confirm to save the edited changes?"} />}
                                        show={this.state.confimationModalShow}
                                        title="PSS save confirmation"
                                        onHide={this.handleCloseModal}
                                        onSubmit={this.handleConfirmationModal}
                                    /> : null}
                            </Col>
                        </FormGroup>
                        <FormGroup row>
                            <div className="col text-right">
                                <Button className="col-md-1 mr-4 " style={{ minWidth: "min-content" }} variant="primary" type="button" onClick={this.handleCancelChanges} >
                                    <FormattedMessage id="ButtonCancel" defaultMessage="Cancel" />
                                </Button>
                                {this.state.isEditMode ?
                                    < Button className="bg-primary col-md-1 mr-4" style={{ minWidth: "min-content" }} type="submit">
                                        <FormattedMessage id="ButtonSave" defaultMessage="Save" />
                                    </Button> : null}
                                {this.state.isEditMode ? null : <Button className="bg-primary col-md-1 mr-4" style={{ minWidth: "min-content" }} type="button" onClick={this.setEditableMode}>
                                    <FormattedMessage id="ButtonEdit" defaultMessage="Edit" />
                                </Button>}
                            </div>
                        </FormGroup>
                        {this.state.loadSaving ? <div className="overlay"><div className="spinner" /> </div> : null}
                    </Form>
                </Card>
            </div>
        )
    }
}

export default withRouter(PSSLink);