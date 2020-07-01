import React, { Component } from 'react';
import { Modal, Button } from 'react-bootstrap';
import DualListBox from '../Controls/DualListBox/DualListBox';
import { GetFullOrgTree, GetDepartmentTree } from '../../Services/OrgUnitsService'
import { findNode, findParent } from '../Controls/DualListBox/DualListBoxUtils'

import './ExcludeOrganizationalUnits.css'
import { CustomInput } from 'reactstrap';

export class ExcludeOrganizationalUnits extends Component {
    constructor(props){
        super(props)
        this.state = {
            selected: [], // selected might have duplicated nodes
            unitGids: [], // unitgids is the list without duplicates of selected
            departmentId: props.departmentid,
            completeOrgTree: null,
            departmentTree: null,
            selectedtemperory: []
        }
        this.handleSelectedList = this.handleSelectedList.bind(this)
        this.selectedNodesChanged = this.selectedNodesChanged.bind(this)
    }


    selectedNodesChanged(selected){
        console.log("nodes changed", selected)
        this.setState({ selectedtemperory: selected})
    }

    handleSelectedList(){
        this.setState({ selected: this.state.selectedtemperory }) // save the temp value on update
        let nodes = this.getNodesfromID()
        this.props.onSelectAvoidOrgUnits(nodes, this.state.unitGids);
    }

    getNodesfromID(){
        let currentTreeSource = []
        if(this.props.departmentid && this.props.departmentid !== 0)
        {
            currentTreeSource = JSON.parse(localStorage.getItem("fullorgTree")) //this.state.departmentTree
        }
        else{
            currentTreeSource =  JSON.parse(localStorage.getItem("fullorgTree"))   
        }
        this.state.unitGids.length = 0 // Remove all element to start fresh
        let nodes = this.state.selectedtemperory.map( el => {
            
            let node = findNode(currentTreeSource, el)           
            if(node){
                var subOrg = node.id.substring(0, 6).toLowerCase()
                console.log(subOrg);
                
                if (subOrg === "depsec" || subOrg ==="depwar" || subOrg ==="depopd"){
                    node.childNodes.map( n => {
                        this.state.unitGids.push(n.id)
                    })
                    console.log("sub section", node)
                    let parent = findParent(currentTreeSource, node).title
                    return { title: `${node.title} of ${parent}`, id: node.id }
                }
                else if(node.id.substring(0, 3).toLowerCase() === "hos"){
                    // all the departments or opds have been dragged
                    console.log("all nodes in ", node);
                    
                    node.childNodes.map( n => {
                        this.state.unitGids.push(n.id)
                    })
                    return { title: node.title, id: node.id }
    
                }
                if(!this.state.unitGids.includes(node.id))
                {
                    this.state.unitGids.push(node.id)
                    return { title: node.title, id: node.id }
                }
            }           
        })
        nodes = nodes.filter(function(node){
            if(node === undefined){
                return false
            }
            else{
                return true
            }
        })
        console.log(nodes)
        return nodes
    }

    async componentDidMount(){
        var tree = []
        
        if(this.props.departmentid === null || this.props.departmentid === 0)
        {
            var orgTreeLocal = localStorage.getItem("fullorgTree")
            if(orgTreeLocal){
                tree = JSON.parse(orgTreeLocal)
            }
            else{
                tree = await GetFullOrgTree()
                localStorage.setItem("fullorgTree", JSON.stringify(tree))
            }
            
            this.setState({
                completeOrgTree: tree,
                departmentId: this.props.departmentid
            })
        }
        else{
            tree = await GetDepartmentTree(this.state.departmentId)
            this.setState({
                departmentTree: tree,
                departmentId: this.props.departmentid
            })
        }
        if(this.props.excluded){
            this.setState({ selected: this.props.excluded })
        }
       }

    async componentDidUpdate(prevProps){
        if(prevProps.departmentid !== this.props.departmentid){
            this.setState({ departmentId: this.props.departmentid })
            var tree = []
            if(this.props.departmentid === null || this.props.departmentid === 0)
            {
                if(this.state.completeOrgTree === null){
                    tree = await GetFullOrgTree()
                    this.setState({
                        completeOrgTree: tree
                })
                }
                
            }
            else{
                tree = await GetDepartmentTree(this.props.departmentid)
                this.setState({
                    departmentTree: tree,
                    departmentId: this.props.departmentid
                })
            }
        }

        if(JSON.stringify(prevProps.excluded) !== JSON.stringify(this.props.excluded)){
            
            this.setState({
                selected : this.props.excluded
            })
            this.handleSelectedList()
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
                        Exclude Organizational Units
                </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    { this.state.departmentId === null || this.state.departmentId === 0 ?
                        <DualListBox tree={ this.state.completeOrgTree ? this.state.completeOrgTree :  null} selected={this.state.selected} onChanged={this.selectedNodesChanged} /> :
                        <DualListBox tree={ this.state.departmentTree ? this.state.departmentTree :  null} selected={this.state.selected} onChanged={this.selectedNodesChanged} />}
                    
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={ () => {this.props.onHide(); this.handleSelectedList()}}>Update</Button>
                </Modal.Footer>
            </Modal >
        );
      
    }
}
export default ExcludeOrganizationalUnits