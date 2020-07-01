import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import {  Col, Button, CardText, Card } from 'reactstrap';

export class ConfirmationModal extends Component {
    constructor(props){
        super(props);
        this.state = {
            textMessage:""
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
                <Modal.Header closeButton>
                    <Modal.Title id="contained-modal-title-vcenter">
                        {this.props.title} 
                </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div style={{ width: '100%' }}>
                    <Col sm="12" >
                        <Card body>
                        <CardText >
                           {this.props.textmessage}
                            
                        </CardText>
                        </Card>
                    </Col>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.props.onHide}>Cancel</Button>
                    <Button className="btn btn-default" onClick={this.props.onSubmit}>Continue</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}
export default ConfirmationModal