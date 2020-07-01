import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import {  Col, Button, CardText, Card } from 'reactstrap';
import { FormattedMessage } from "react-intl";

export class PreviewModal extends Component {
    constructor(props){
        super(props);
        this.state = {
            textMessage:"",
            textAreaMessage:""
        }
    }
    render() {
        return (
            <Modal
                {...this.props}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter"
                centered
            >
               
                <Modal.Body >
                    <div >
                    <Modal.Title id="contained-title-vcenter">
                        <FormattedMessage id="SMSText" defaultMessage="SMS-Text" /> 
                    </Modal.Title>
                    <Col sm="12" >
                        <Card body className="previewModal">
                        <CardText >
                           <p>{this.props.textAreaMessage}</p>
                            
                        </CardText>
                        </Card>
                    </Col>
                    </div>
                    <div>
                    <Modal.Title id="contained-modal-title-vcenter">
                        <FormattedMessage id="SMSPreview" defaultMessage="SMS-Preview" /> 
                    </Modal.Title>
                    <Col sm="12" >
                        <Card body className="previewModal">
                        <CardText >
                           <p>{this.props.textMessage}</p>
                            
                        </CardText>
                        </Card>
                    </Col>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.props.onHide}>Close</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}
export default PreviewModal