import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import {  Col, Button, Card, Label, Row, Input, Form, FormGroup } from 'reactstrap';
import { FormattedMessage } from 'react-intl'

class SchedulesSaveAsModal extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            scheduleName: "",
            saveAsDisabled: true
         }
         this.handleScheduleName = this.handleScheduleName.bind(this)
         this.saveScheduleAs = this.saveScheduleAs.bind(this)
    }

    saveScheduleAs(e){
        e.preventDefault()
        console.log("form submit")
        this.props.onSubmit(this.state.scheduleName)
    }

    handleScheduleName = e => {
        let  btnDisable = false
        if(e.target.value === ""){
            btnDisable = true
        }
        this.setState({ 
            scheduleName: e.target.value,
            saveAsDisabled: btnDisable 
        })
    }
    render() { 
        return ( 
            <Modal {...this.props}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter"
                centered >
                <Form>
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            <FormattedMessage id="SaveAsScheduleConfirmationTitle" defaultMessage="Save As a new Schedule" />
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Col sm="12" >
                            <Card body>
                                <FormGroup>
                                    <Row>
                                        <Col md={4}>
                                            <Label><FormattedMessage id="ScheduleName.SaveAs" defaultMessage="New_Schedule_Name:" /></Label>
                                        </Col>
                                        <Col md={5}>
                                            <Input value={this.state.scheduleName} onChange={this.handleScheduleName} required />
                                        </Col>
                                    </Row>
                                </FormGroup>
                            </Card>
                        </Col>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.props.onHide}><FormattedMessage id="ButtonCancel" defaultMessage="Cancel" /></Button>
                        <Button className="bg-primary btn btn-default" id="SaveAs" onClick={this.saveScheduleAs} disabled={this.state.saveAsDisabled} >
                            <FormattedMessage id="ButtonSave" defaultMessage="Save" />
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}
 
export default SchedulesSaveAsModal;