import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import {  Col, Button, Card, Label, Row, Input } from 'reactstrap';
import { FormattedMessage } from 'react-intl'

class TextTemplateSaveAsModal extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            templateName: ""
         }
         this.handleTextTemplateName = this.handleTextTemplateName.bind(this)
    }

    handleTextTemplateName = e => {
        this.setState({ templateName: e.target.value})
    }
    render() { 
        return ( 
            <Modal {...this.props}
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered >
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                    <FormattedMessage id="SaveAsConfirmationTitle" defaultMessage="Save As Content Template" />
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Col sm="12" >
                    <Card body>
                    {/* <FormattedMessage id="SaveAsConfirmationBody" defaultMessage="Are you sure to save a copy of this Template?" /> */}
                    <Row>
                    <Col md={4}>
                    <Label>New Template Name:</Label>
                    </Col>
                    <Col md={5}>
                        <Input value={this.state.templateName} onChange={this.handleTextTemplateName} />
                    </Col>
                    </Row>
                </Card>
                </Col>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={this.props.onHide}><FormattedMessage id="ButtonCancel" defaultMessage="Cancel" /></Button>
                <Button className="btn btn-default" id="SaveAs"
                    onClick={(e) => this.props.onSubmit(this.state.templateName)}>
                        <FormattedMessage id="ButtonSave" defaultMessage="Save" />
                    </Button>
            </Modal.Footer>
        </Modal>
         );
    }
}
 
export default TextTemplateSaveAsModal;