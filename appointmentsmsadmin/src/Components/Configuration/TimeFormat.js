import React, { Component } from 'react';
import { Button, Input } from 'reactstrap'
import { withRouter } from 'react-router';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import { FormattedMessage } from "react-intl";
import { MDBDataTable } from 'mdbreact';
import { NotificationManager } from 'react-notifications';

import { GetFormats, UpdateFormats, GetPreview } from '../../Services/ConfigurationService'

import './TimeFormat.css'

class TimeFormat extends Component {
    constructor(props) {
        super(props)
        this.state = {
            immutedFormatList: [],
            timeFormats: [],
            isEditMode: false,
            currentEditRow: 0,
            currentRowIsActive: true,
            isSaving: false
        }

        this.handleEditClick = this.handleEditClick.bind(this)
        this.handleTextChange = this.handleTextChange.bind(this)
        this.handleCancelClick = this.handleCancelClick.bind(this)
        this.handleActiveChange = this.handleActiveChange.bind(this)
        this.handleSaveButtonClicked = this.handleSaveButtonClicked.bind(this)
        this.handleUpdatePreview = this.handleUpdatePreview.bind(this)
    }
    //#region handle events
    handleTextChange(e, id){
        console.log(e.target.value);
        console.log(id);
        let list = this.state.timeFormats
        list.map( row => {
            if(row.id === id){
                row.format = e.target.value
            }
            return row
        }) 
        this.setState({
            timeFormats: list
        })
    }

    async handleUpdatePreview(id){
        let list = this.state.timeFormats
        var newFormat = list.find(function(element){
            return element.id === id
        })
        try{
            sessionStorage.setItem('configtimeformats', JSON.stringify(this.state.timeFormats));
            sessionStorage.setItem("configtimeformatstatus", JSON.stringify({ "currentEditRow": this.state.currentEditRow }))
            // get preview for new format from server
            let newPreview = await GetPreview(newFormat.format, newFormat.displaySample)

            list.map( element => {
                if(element.id === id){
                    element.displaySample = newPreview
                }
                return element
            })

            this.setState({
                timeFormats: list
            })
        }
        catch(error) {
            if (error.statuscode === 401) {
                this.handleOn401Error();
            }
            else {
                sessionStorage.removeItem('configtimeformats');
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Preview Update Failed");
                NotificationManager.error(errorMsg, 'Configuration TimeFormat');
            }
        }
    }

    handleEditClick(e, k) {
        this.setState({
            isEditMode: true,
            currentEditRow: k
        })
    }

    handleCancelClick(){       
        this.setState({
            isEditMode: false,
            currentEditRow: 0,
            timeFormats: JSON.parse(JSON.stringify(this.state.immutedFormatList))
        })
    }

    handleActiveChange(e, id){
        console.log(typeof(e.target.value));
        let list = this.state.timeFormats
        list.map( row => {
            if(row.id === id){
                console.log("node found", id, row.active);
                row.active = !row.active
            }
            return row
        })
        this.setState({
            timeFormats: list
        })
    }

    async handleSaveButtonClicked(id){
        this.setState({ isSaving: true})
        sessionStorage.setItem('configtimeformats', JSON.stringify(this.state.timeFormats));
        sessionStorage.setItem("configtimeformatstatus", JSON.stringify({ "currentEditRow": this.state.currentEditRow }))
        let format = this.state.timeFormats.find(function(item){
            return item.id === id
        })
        try{
            await UpdateFormats(format)
            this.setState({
                isEditMode: false,
                currentEditRow: 0,
                isSaving: false
            })
            sessionStorage.removeItem('configtimeformats');
            sessionStorage.removeItem("configtimeformatstatus")
            this.loadData()
        }
        catch(error){
            this.setState({ isSaving: false })
            if (error.statuscode === 401) {
                this.handleOn401Error();
            }
            else {
                sessionStorage.removeItem('configtimeformats');
                sessionStorage.removeItem("configtimeformatstatus")
                let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Save/Update Failed");
                NotificationManager.error(errorMsg, 'Configuration TimeFormat');
            }
        }
    }

    handleOn401Error = () => {
        NotificationManager.error("Session expired! Redirecting to login.", 'Group Templates');
        this.props.history.push({
            pathname: '/Login',
            state: { directLogin: false }
        });
    }

    //#endregion

    //#region lifecycle methods
    async componentDidMount(){
        this.loadData()
    }

    async loadData(){
    try{
        let storedTextItem = JSON.parse(sessionStorage.getItem('configtimeformats'));
        if(storedTextItem){
            let statusVariables = JSON.parse(sessionStorage.getItem('configtimeformatstatus'));
            let currentRow = statusVariables.currentEditRow;
            console.log(currentRow)
            this.setState({
                immutedFormatList: JSON.parse(JSON.stringify(storedTextItem)),
                timeFormats: storedTextItem,
                currentEditRow: currentRow,
                isEditMode: true
            })
        }
        else{
            var formats = await GetFormats()
            this.setState({
                immutedFormatList: JSON.parse(JSON.stringify(formats)),
                timeFormats: JSON.parse(JSON.stringify(formats))
            })
        }
    }
    catch (error) {
        this.setState({ isSaving: false });

        if (error.statuscode === 401) {
            this.handleOn401Error();
        }
        else {
            sessionStorage.removeItem('configtimeformats');
            let errorMsg = (error.cause ? JSON.stringify(error.cause) : "Save/Update Failed");
            NotificationManager.error(errorMsg, 'Configuration TimeFormat');
        }
    }
    }
    //#endregion

    render() {
        var dateFormats = this.state.timeFormats.map( data => {
            let EditableRow = this.state.isEditMode && this.state.currentEditRow === data.id
            return {
                format: <Input className="noBorder" disabled={!EditableRow}
                            value={data.format} 
                            onChange={(e) => this.handleTextChange(e, data.id)}></Input>,
                update: EditableRow ? <div style={{ "textAlign": "center"}}>
                        <Button color="primary" onClick={() => this.handleUpdatePreview(data.id)}>Update Preview</Button>
                    </div> : "",
                preview: <div className="previewAlignments">{data.displaySample}</div>,
                active: <Input type="select" disabled={!EditableRow} value={data.active} onChange={(e) => this.handleActiveChange(e, data.id)}>
                            <option value={true} >Yes</option>
                            <option value={false} >No</option>
                        </Input>,
                edit: EditableRow ? 
                <div>
                    <Button className="leftMargin" color="secondary" onClick={this.handleCancelClick}>Cancel</Button>
                    <Button className="leftMargin" color="primary" onClick={() =>this.handleSaveButtonClicked(data.id)} >Save</Button>
                    
                </div> : 
                <div>
                    <Button className="leftMargin" color="link" onClick={(e) => { this.handleEditClick(e, data.id) }}>Edit</Button>
                </div>
            }
        })

        var data = {
            columns: [{
                label: "Format",
                field: "format",
                attributes: {
                    style: { 
                        "minWidth": "200px",
                        "width": "25%"
                    }
                }
            },
            {
                label: "",
                field: "update",
                attributes: {
                    style: { 
                        "minWidth": "200px",
                        "width": "20%",
                        
                    }
                }
            },
            {
                label: "Preview",
                field: "preview",
                attributes: {
                    style: { 
                        "minWidth": "200px",
                        "width": "25%"
                    }
                }
            },
            {
                label: "Active",
                field: "active",
                attributes: {
                    style: { 
                        "minWidth": "200px",
                        "width": "10%"
                    }
                }
            },
            {
                label: "",
                field: "edit",
                attributes: {
                    style: { 
                        "minWidth": "180px",
                        "width": "20%"
                    }
                }
            }
        ],
        rows: [...dateFormats]
        }

        return (
            <div>
                <Breadcrumb>
                    <Breadcrumb.Item>
                        <FormattedMessage id="Configurations" defaultMessage="Configurations" />
                    </Breadcrumb.Item>
                    <Breadcrumb.Item active >
                        <FormattedMessage id="Configurations.DateTimeFormat" defaultMessage="Date_time_formats" />
                    </Breadcrumb.Item>
                </Breadcrumb>

                <MDBDataTable striped 
                    hover bordered 
                    data={data}
                    searching={false}
                    paging={false}
                    order={['name', 'asc']}
                    paginationLabel={["Previous", "Next"]}
                    infoLabel={["showing", "to", "of","entries"]}
                    noRecordsFoundLabel={ 
                        <p style={{"textAlign":'center' }}>
                            <FormattedMessage id="GroupTemplate.NoRecordsFound" defaultMessage="No matching records found" />
                        </p>}
                    noBottomColumns />
                {this.state.isSaving ? <div className="overlay"><div className="spinner" /> </div> : null}
            </div>
        )
    }
}

export default withRouter(TimeFormat);