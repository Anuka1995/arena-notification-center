import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import { Col, Button, FormGroup, Label, Input, CardText, Card ,Form  } from 'reactstrap';
import Loader from 'react-loader-spinner'
import { FormattedMessage } from "react-intl";
import { NotificationManager } from 'react-notifications';

import PhrasesTree from './../TreeComponents/PhrasesTree';
import {DateTimeCommonFormat, FilterAndGetTagsForModal} from '../../Services/TagsService';

import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"
import "./AddTagsModal.css"

export class AddTagsModal extends Component {
    constructor(props){
        super(props);
        this.state = {
            tagTree:[],
            e_tagguid:null,
            hideTagFormText: true,
            hideTagFormInt:true,
            hideTagFormDateTime:true,
            tagName:"",
            tagPath: "",
            tagIsXapth: false,
            departmentId: null,
            searchTerm:'',
            wrappedTagName:'',
            wrapfront : "[$$",
            closeTagModal : false,
            customTag:"",
            isLoadingPhrasesTree: true,
            isLoadingFormat:true,
            CommonDateTimeFormat:[]
        }

        this.handleChangeSearchTerm = e => this.setState({ searchTerm: e.target.value });

        this.showDetailView = (currentNode) => {
            this.clearForms();
            if(currentNode.id){
                var tagGuid = currentNode.id;
                this.setState({
                    e_tagguid: tagGuid,
                    tagPath: currentNode.tag.tagValue,
                    tagIsXapth: currentNode.tag.tagType === 0
                });

                if(currentNode.tag.dataType == 0) {
                    this.setState({hideTagFormText: false, tagName: currentNode.tag.tagName });
                    
                }
                else if(currentNode.tag.dataType == 1) {
                    this.setState({hideTagFormInt: false, tagName: currentNode.tag.tagName});
                }
                else {
                    this.setState({hideTagFormDateTime: false, tagName: currentNode.tag.tagName});
                    this.GetCommonFormat();
                }
            }
        }
    }

    clearForms(){
        this.setState({
            hideTagFormDateTime:true,
            hideTagFormInt:true,
            hideTagFormText:true
        })
    }

    searchTagsOnSubmit = async(e)=> {
        e.preventDefault();
        this.clearForms();
        await this.filterAndGrabTree(this.props.deptId,this.state.searchTerm);
        
    }
    GetCommonFormat = async () => {
        try {
            this.setState({ isLoadingFormat: true });
          var commonFormats = await DateTimeCommonFormat();
          this.setState({ isLoadingFormat: false });
          if (commonFormats != null) {
            this.setState({
              CommonDateTimeFormat: commonFormats
            });
          }
        }
        catch(error) {
            this.setState({ isLoadingFormat: false });
            if(error.statuscode === 401){
                console.log("add tags model error");
                this.props.on401Error();
            } 
            else{
                let errorMsg = (error.cause ?  JSON.stringify(error.cause) : "Phrase Date/Time Formats failed");
                NotificationManager.error(errorMsg, 'Search Phrases');
            }
            
        }
    }
    
    filterAndGrabTree = async(department, text) => {
        try {
            this.setState({ isLoadingPhrasesTree: true });
            var tagTreeNodes = await FilterAndGetTagsForModal(department, text);
            this.setState({ isLoadingPhrasesTree: false });
            if (tagTreeNodes == null) {
                NotificationManager.warning("No data received", "Search Phrases");
            }
            else {
                if(tagTreeNodes.length == 0) {
                    NotificationManager.info("List is Empty", "Search Phrases");
                }
                this.setState({tagTree: []});
                this.setState({tagTree: tagTreeNodes});
            }
        }
        catch(error) {
            this.setState({ isLoadingPhrasesTree: false });
            if(error.statuscode === 401){
                console.log("add tags model error");
                this.props.on401Error();
            } 
            else{
                let errorMsg = (error.cause ?  JSON.stringify(error.cause) : "Phrase search failed");
                NotificationManager.error(errorMsg, 'Search Phrases');
            }
            
        }
    }

    handleTagWithoutFormat = e => {
        var tagname = e.target.value;
        var wrapend= "$$]";
        var concatTagName = this.state.wrapfront.concat(tagname,wrapend);        
        this.setState({  wrappedTagName  : concatTagName });
        this.sendData(concatTagName,this.state.closeTagModal);        
    }

    handleTagWithUpperCase = e => {
        var tagname = e.target.value;
        var wrapend= ":U$$]";
        var concatTagName = this.state.wrapfront.concat(tagname,wrapend);
        this.setState({ wrappedTagName: concatTagName });
        this.sendData(concatTagName,this.state.closeTagModal);
    }

    handleTagWithPrettyPrintName = e => {
        var tagname = e.target.value;
        var wrapend= ":PN$$]";
        var concatTagName = this.state.wrapfront.concat(tagname,wrapend);
        this.setState({ wrappedTagName: concatTagName });
        this.sendData(concatTagName,this.state.closeTagModal);
    }

    handleTagWithPrettyPrintAddress = e => {
        var tagname = e.target.value;
        var wrapend= ":PA$$]";
        var concatTagName = this.state.wrapfront.concat(tagname,wrapend);
        this.setState({ wrappedTagName: concatTagName });
        this.sendData(concatTagName,this.state.closeTagModal);
    }

    handleCustomFormat = e => {
        var format = e.target.value;
        console.log(format);
        var wrapTag = ":TX:";
        var wrapEnd = "$$]"
        var concatTagName = this.state.wrapfront.concat(this.state.tagName,wrapTag,format,wrapEnd);
        this.setState({ wrappedTagName: concatTagName });
        this.sendData(concatTagName,this.state.closeTagModal);
    }

    handleTagWithDateTimeFormatTx = e => {
        var customFormat = e.target.value;
        this.setState({
            customTag:customFormat
        })
    }

    handleTagWithDateTimeCustom = e => {
        var tagName = e.target.value;
        var wrapTag = ":TX:";
        var wrapEnd = "$$]"
        var concatTagName = this.state.wrapfront.concat(tagName,wrapTag,this.state.customTag,wrapEnd);
        this.setState({ wrappedTagName: concatTagName });
        this.sendData(concatTagName,this.state.closeTagModal);
    }

    async componentDidMount() {
        await this.filterAndGrabTree(this.props.deptId, this.state.searchTerm);
    }

    async componentDidUpdate(prevProps) {      
        if ((this.props.deptId !== prevProps.deptId) && (this.props.deptId || this.props.deptId == null)) {
            this.setState({
                departmentId : this.props.deptId,
                wrappedTagName:this.state.wrappedTagName,
            });

          await this.filterAndGrabTree(this.props.deptId, this.state.searchTerm);
        }
    }

    clearInputs() {
        this.setState({
            searchTerm: '',
            tagTree:[],
            customTag:''
        });
        this.clearForms();
        var term = "";
        //this.filterAndGrabTree(this.state.departmentId,term);
    }
    
    sendData = (concatTagName,closeModal) => {
        this.props.onSelectTags(concatTagName);
        this.props.onHide(closeModal);
        this.clearInputs();
    }

    render() {
        let disbleSave = this.state.customTag == 0 ? true : false;
        return (
            <div >
            <Modal
                {...this.props}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter"
                centered 
            >
                <Modal.Header closeButton>

                </Modal.Header>
                <Modal.Body >
                        <Form onSubmit={this.searchTagsOnSubmit} >
                            <FormGroup row>
                                <Label sm={3}><FormattedMessage id="SearchPhrases" defaultMessage="Search-Phrases" /></Label>
                                <Col sm={5}>
                                    <Input type="search" name="search" id="#" placeholder="Search Phrases" 
                                        value={this.state.searchTerm} onChange={this.handleChangeSearchTerm} />
                                </Col>
                                <Col sm={3}>
                                    <Button className="bg-primary offset-1  mb-2" type="submit" >
                                        <FormattedMessage id="ButtonSearch" defaultMessage="Search" />
                                    </Button>
                                </Col>
                            </FormGroup>
                        </Form>
                        <FormGroup row>
                            <Col>
                                <Card body className="overviewTree" style={{maxWidth:'400px',minHeight: '400px', maxHeight : '400px' }}>
                                    
                                    {   this.state.isLoadingPhrasesTree ? 
                                        <div className="d-flex justify-content-center">
                                            <Loader type="ThreeDots"
                                             color="#00BFFF"
                                             height={40}
                                             width={40} />
                                        </div> :
                                        <div style={{ textAlign: 'left' }}>
                                            <PhrasesTree id="phrasesTree" key="1" 
                                                nodeData={this.state.tagTree} 
                                                title="Phrases" 
                                                open
                                                onLeafNodeClick={this.showDetailView} />
                                        </div> 
                                    }
                                </Card>
                            </Col>

                            <Col>
                                <Card body style={{minHeight: '400px', maxHeight : '400px', maxWidth:'400px','overflow-y': 'auto' }}>
                                
                                    <Form hidden={this.state.hideTagFormText}>
                                        <Label className="tagName"><b>{this.state.tagName}</b></Label>
                                        <FormGroup row hidden={!this.state.tagIsXapth}>
                                            <Label sm={4}><FormattedMessage id="AddTags.Xpath" defaultMessage="XPath"/></Label>
                                            <Col sm={12}>
                                                <span className="text" >{this.state.tagPath.substring(1)}</span>
                                            </Col>
                                        </FormGroup>
                                        <FormGroup row>
                                            <Col sm={8}><FormattedMessage id="AddWithoutFormat" defaultMessage="Add-Without-Format" /></Col>
                                            <Button className="bg-primary" value={this.state.tagName} onClick={this.handleTagWithoutFormat}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                        <FormGroup row>
                                            <Col sm={8}><FormattedMessage id="Uppercase" defaultMessage="Uppercase" /></Col>
                                            <Button className="bg-primary " value={this.state.tagName} onClick={this.handleTagWithUpperCase}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                        <FormGroup row>
                                            <Col sm={8}><FormattedMessage id="PrettyprintName" defaultMessage="Pretty-print-Name" /></Col>
                                            <Button className="bg-primary " value={this.state.tagName} onClick={this.handleTagWithPrettyPrintName}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                        <FormGroup row>
                                            <Col sm={8}><FormattedMessage id="PrettyprintAddress" defaultMessage="Pretty-print-Address" /></Col>
                                            <Button className="bg-primary " value={this.state.tagName} onClick={this.handleTagWithPrettyPrintAddress}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                    </Form>
                                    
                                    <Form hidden={this.state.hideTagFormInt}>
                                        <Label  className="tagName"><b>{this.state.tagName}</b></Label>
                                        <FormGroup row>
                                            <Col sm={8}><FormattedMessage id="AddWithoutFormat" defaultMessage="Add-Without-Format" /></Col>
                                            <Button className="bg-primary " value={this.state.tagName} onClick={this.handleTagWithoutFormat}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                    </Form>

                                    <Form hidden={this.state.hideTagFormDateTime}>
                                    <Label  className="tagName"><b>{this.state.tagName}</b></Label>
                                    {this.state.isLoadingFormat ? 
                                        <div className="d-flex justify-content-center">
                                            <Loader type="ThreeDots"
                                             color="#00BFFF"
                                             height={40}
                                             width={40} />
                                        </div> : 
                                        <div>
                                        <FormGroup row>
                                            <Col sm={8}><FormattedMessage id="AddWithoutFormat" defaultMessage="Add-Without-Format" /></Col>
                                            <Button className="bg-primary " value={this.state.tagName} onClick={this.handleTagWithoutFormat}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                        
                                        {this.state.CommonDateTimeFormat.map(({displaySample,format}) => 
                                        <FormGroup row>
                                            <Col sm={8}>{displaySample}</Col>
                                            <Button className="bg-primary " value={format} onClick={this.handleCustomFormat}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>)}
                                       
                                        <Label><FormattedMessage id="CustomFormatTag" defaultMessage="Add-your-custom-format" /></Label>
                                        <FormGroup row>
                                            <Col sm={8}>
                                                <textarea as="textarea" id="textareaId" rows="1" value={this.state.customTag} 
                                                onChange={this.handleTagWithDateTimeFormatTx}
                                                 />
                                            </Col>
                                            <Button className="bg-primary " value={this.state.tagName} onClick={this.handleTagWithDateTimeCustom} disabled={disbleSave}>
                                                <FormattedMessage id="AddPhrase" defaultMessage="AddPhrase" />
                                            </Button>
                                        </FormGroup>
                                        </div>}
                                    </Form>
                                </Card>
                            </Col>
                        </FormGroup>
                </Modal.Body>
            </Modal>
        </div>
        );
    }
}

export default AddTagsModal