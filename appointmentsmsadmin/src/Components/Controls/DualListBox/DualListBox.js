import React from 'react';
import { Row, Col, Button, Card, UncontrolledTooltip  } from 'reactstrap';
import Loader from 'react-loader-spinner'
import DynamicDataTree from './DynamicDataTree';

import './DualListBox.css'

var found = false // once you move a node make sure to set this to false again

class DualListBox extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            list: [], 
            selected: null,
            selectedNodesList: [],
            selectedLeft: null,
            selectedRight: null,
            selectedList: [],
            listToReturn: props.selected,
            selectedListUpdated: false,
            treeLoaded: false
         }
        this.handleLeftNodeClick = this.handleLeftNodeClick.bind(this)
        this.handleRightNodeClick = this.handleRightNodeClick.bind(this)

        this.moveToSelected = this.moveToSelected.bind(this)
        this.moveAllToSelected = this.moveAllToSelected.bind(this)
        this.moveToNotSelected = this.moveToNotSelected.bind(this)
        this.moveAllToNotSelected = this.moveAllToNotSelected.bind(this)
        
        this.moveAllToRight = this.moveAllToRight.bind(this);
        this.movedAllToLeft = this.movedAllToLeft.bind(this);
    }
//#region node click events
    handleLeftNodeClick = e => {
        if(e.id !== "root")
        {
            let allselectedNodes = []
            this.findAllNodes(this.state.list, allselectedNodes, e.id)
            console.log("all nodes", allselectedNodes)

            this.setState({ 
                selected: e, 
                selectedLeft: e,
                selectedRight: null,
                selectedNodesList: allselectedNodes
            })
        }
    }

    handleRightNodeClick = e => {
        if(e.id !== "root")
        {
            let allselectedNodes = []
            this.findAllNodes(this.state.selectedList, allselectedNodes, e.id)
            console.log("all nodes", allselectedNodes)

            this.setState({ 
                selected: e,
                selectedRight: e,
                selectedLeft: null,
                selectedNodesList: allselectedNodes
            })
        }
    }
//#endregion

//#region node operations
    CloneObject(object){
        var cloned = JSON.parse(JSON.stringify(object))
        return cloned
    }

    populateReturnList(){
        this.state.listToReturn.length = 0
        this.crawlTheList(this.state.selectedList)
        console.log("listToReturn", this.state.listToReturn)
        this.props.onChanged(this.state.listToReturn)
    }

    crawlTheList(array){
        array.map( n => {
            // check if this node is exist in left.
            // if no add it the list and break from the current loop
            // else recursion
            let node = this.findNode(this.state.list, n.id)
            if(node){
                if(n.childNodes.length > 0){
                    this.crawlTheList(n.childNodes)
                }
                else{
                    // if node is moved to right and still its children in the left side, it should be a duplicate
                    console.log("node still remain in left", node.title)
                }
            }
            else{
                this.state.listToReturn.push(this.CloneObject(n.id))
            }
        })      
    }

    removeNode(array, node){
        var id = node.id
        
        array.forEach(element => {
            if(element.id === id){
                found = true
            }
            if(!found){
                var removedArray = this.removeNode(element.childNodes, node)
                element.childNodes = removedArray
            }
        });       
        array = array.filter( n => n.id !== id)
        return array
    }

    findParent(array, node){
        var id = node.parentNode
        var foundNode = this.findNode(array, id)
        return foundNode;
    }

    findNode(array, id){
        let node;
        for (let index = 0; index < array.length; index++) {
            const element = array[index];
            if(element.id === id){
                node = element
                break
            }
            else{
                node = this.findNode(element.childNodes, id)
                if(node){
                    break
                }
            }
        }
        return node;
    }

    findAllNodes(array, foundNodes, id){
        let node;
        for (let index = 0; index < array.length; index++) {
            const element = array[index];
            if(element.id === id){
                node = element
                foundNodes.push(node)
            }
            else{
                this.findAllNodes(element.childNodes, foundNodes, id)
                
            }
        }
        return foundNodes;
    }

    findPath(array, node){
        let path = []
        path.push(node.id)
        this.pushParent(array, node, path)
        path.reverse()
        return path
    }

    pushParent(array, node, path){       
        let parent = this.findParent(array, node)
        if(parent){
            path.push(parent.id)
            this.pushParent(array, parent, path)
        }
    }
//#endregion

//#region button click events

    moveAllToSelected(array, leftTree){
        let progressListLeft =  leftTree ? leftTree : this.state.list // left tree only for loading props.tree from componentDidMount

        let selectedNodesList = array.length > 0 ? array : this.state.selectedNodesList
        selectedNodesList.map( el => {
            progressListLeft = this.moveToSelected(progressListLeft, el)
        })

        this.setState({
            list: progressListLeft
        })
        return progressListLeft
    }

    moveToSelected(leftArray, e){
        if(this.state.selectedLeft || e.id){
            let alreadyMoved = this.state.selectedList
            let currentSelected = e
            let pathToNode = this.findPath(leftArray, currentSelected)
            pathToNode.pop()

            //Add missing parent nodes to right
            let tempArray = alreadyMoved
            pathToNode.map( n => {
                let rNode = tempArray.find(function(element){
                    return element.id === n
                })

                if(!rNode){
                    //tempArray doesnt have the node so get it from left and add it
                    let lNode = this.findNode(leftArray, n)
                    let cloned = this.CloneObject(lNode)
                    cloned.childNodes = []
                    tempArray.push(cloned)
                }
                else{
                    //already have so no need to add
                }
                 //goto the pushed node and find next node in childNodes
                 tempArray = tempArray.find(function(element){
                     return element.id === n
                 }).childNodes
                 return alreadyMoved
            })

            let parent = this.findParent(alreadyMoved, currentSelected)
            if(parent){
                // selected node also might be there with childnodes
                let node = this.findNode(alreadyMoved, currentSelected.id)
                console.log(node);
                
                if(node){
                    if(parent.id === node.parentNode){
                        node.childNodes.push(...currentSelected.childNodes)
                        console.log(node.childNodes);
                    }
                    else{
                        parent.childNodes.push(currentSelected)
                    }
                }
                else{
                    parent.childNodes.push(currentSelected)
                }
            }
            else{
                let node = this.findNode(alreadyMoved, currentSelected.id)
                if(node){
                    let originalNode = this.findNode(this.state.treeLoaded ? this.state.list : this.props.tree, currentSelected.id)
                    let clone = this.CloneObject(originalNode)
                    node.childNodes.push(...clone.childNodes)
                }
                else{
                    alreadyMoved.push(currentSelected)
                }              
            }

            let removed = this.removeNode(leftArray, currentSelected)
            found = false
            // if parent is empty clean up
            let selectedParent = this.findParent(removed,currentSelected)

            if( selectedParent && selectedParent.childNodes.length === 0){
                removed = this.removeNode(removed, selectedParent)
                found = false
                let root = this.findParent(removed, selectedParent)
                if(root && root.childNodes.length === 0){
                    removed = this.removeNode(removed, root)
                    found = false
                }
            }

            this.setState({
                selected: null,
                list: removed,
                selectedListUpdated: true
            })
            return removed
        }
    }

    moveAllToNotSelected(){
        var progressListRight = this.state.selectedList
        this.state.selectedNodesList.map( el => {
            progressListRight = this.moveToNotSelected(progressListRight, el)
        })
    }

    moveToNotSelected(rightArray, node){
        if(this.state.selectedRight){
            var unmovedList = this.state.list
            var currentSelected = node;

            var pathToNode = this.findPath(rightArray, currentSelected)
            pathToNode.pop()

            //Add missing parent nodes to right
            var tempArray = unmovedList
            pathToNode.map( n => {
                var rNode = tempArray.find(function(element){
                    return element.id === n
                })

                if(!rNode){
                    //tempArray doesnt have the node so get it from left and add it
                    let lNode = this.findNode(rightArray, n)
                    let cloned = this.CloneObject(lNode)
                    cloned.childNodes = []
                    tempArray.push(cloned)                   
                }
                else{
                    //already have so no need to add
                }
                 //goto the pushed node and find next node in childNodes
                 tempArray = tempArray.find(function(element){
                     return element.id === n
                 }).childNodes
                 return unmovedList
            })

            var parent = this.findParent(unmovedList, currentSelected)
            if(parent){
                let node = this.findNode(unmovedList, currentSelected.id)
                if(node){
                    node.childNodes.push(...currentSelected.childNodes)
                }
                else{
                    parent.childNodes.push(currentSelected)
                }
            }
            else{
                let node = this.findNode(unmovedList, currentSelected.id)
                if(node){
                    let clone = this.CloneObject(currentSelected)
                    node.childNodes.push(...clone.childNodes)
                }
                else{
                    unmovedList.push(currentSelected)
                }
            }

            var removed = this.removeNode(rightArray, currentSelected)
            found = false
            // if parent is empty clean up
            var selectedParent = this.findParent(removed,currentSelected)
            if( selectedParent && selectedParent.childNodes.length === 0){
                removed = this.removeNode(removed, selectedParent)
                found = false
                let root = this.findParent(rightArray, selectedParent)
                if(root && root.childNodes.length === 0){
                    removed = this.removeNode(rightArray, root)
                    found = false
                }
            }

            this.setState({
                selected: null,
                selectedList: removed,
                selectedListUpdated: true
            })
            return removed
        }       
    }

    moveAllToRight = e => {
        this.setState({
          selectedList: this.CloneObject(this.props.tree),
          list: [],
          selectedListUpdated: true
        });
      };
    
    movedAllToLeft = e => {
        console.log(this.state.selectedList)
        this.setState({
            list: this.CloneObject(this.props.tree),
            selectedList: [],
            selectedListUpdated: true
        });
    };
//#endregion  

//#region react lifecycle

    componentDidMount(){
     this.setState({ listToReturn: this.CloneObject(this.props.selected) })   
    }
    componentDidUpdate(prevProps, prevState){
        if(prevProps.tree === null && this.props.tree !== null){
            this.setState({
                list: this.CloneObject(this.props.tree),
                treeLoaded: true
            })
        } 
        
        if(this.state.selectedListUpdated){
            this.setState({ selectedListUpdated: false})
            this.populateReturnList()
        }

        if(JSON.stringify(this.props.selected) !== JSON.stringify(this.state.listToReturn) && 
        JSON.stringify(this.props.selected) !== JSON.stringify(prevProps.selected)) 
        {
            this.setState({ listToReturn: this.CloneObject(this.props.selected)})
            let clonedTree = this.CloneObject(this.props.tree)
            let listToReturn =  this.props.selected
            if(listToReturn.length > 0){
                listToReturn.map( id => {
                    let foundNodes= []
                    let nodes = this.findAllNodes(clonedTree, foundNodes, id)
                    if(nodes.length > 0){
                        clonedTree = this.moveAllToSelected(nodes, clonedTree)
                    }
                })
            }
        }
    }
//#endregion

    render() {
        return ( 
            <div className="container">
                <Row className="margin">
                <Col md={5} > 
                    <Card className="fixed-card">
                        { this.state.treeLoaded ?
                            <DynamicDataTree key="1" id="1" nodeData={this.state.list} content="Organizations" open
                                treeNodeClick={this.handleLeftNodeClick} isActive={this.state.selectedLeft} /> :
                            <div className="d-flex justify-content-center">
                                <Loader type="ThreeDots"
                                    color="#00BFFF"
                                    height={40}
                                    width={40} />
                            </div>
                        }                  
                    </Card>
                </Col>
                <Col md={1} className="align-self-center no-padding">
                        <div className="text-center">
                            
                       <Button className="buttonWidth" onClick={this.moveAllToSelected} id="AddSelected"> &gt; </Button>
                       <UncontrolledTooltip placement="right" target="AddSelected" autohide={false} >Add selected unit </UncontrolledTooltip>
                       <Button className="buttonWidth" onClick={this.moveAllToRight} id="AddAll"> &gt;&gt; </Button>
                       <UncontrolledTooltip placement="right" target="AddAll" autohide={false} >Add all units </UncontrolledTooltip>
                       <Button className="buttonWidth" onClick={this.moveAllToNotSelected} id="RemoveSelected"> &lt;</Button>
                       <UncontrolledTooltip placement="right" target="RemoveSelected" autohide={false} >Remove selected unit </UncontrolledTooltip>
                       <Button className="buttonWidth" onClick={this.movedAllToLeft} id="RemoveAll"> &lt;&lt; </Button>
                       <UncontrolledTooltip placement="right" target="RemoveAll" autohide={false} >Remove all units </UncontrolledTooltip>
                        </div>
                        
                </Col>
                <Col md={5}>
                    <Card className="fixed-card">
                    { this.state.treeLoaded ?
                        <DynamicDataTree key="1" id="1" nodeData={this.state.selectedList} content="Exclude Org list" open
                            treeNodeClick={this.handleRightNodeClick} isActive={this.state.selectedRight} /> :
                        <div className="d-flex justify-content-center">
                            <Loader type="ThreeDots"
                                color="#00BFFF"
                                height={40}
                                width={40} />
                        </div>
                    }
                    </Card>                    
                </Col>

                </Row>
            </div>
            
         );
    }
}
 
export default DualListBox;


/* Sample object 
var overview = [{
    "parentNode": null,
    "title": "Department One",
    "id": "25",
    "childNodes": [{
        "parentNode": "25",
        "title": "Section 1",
        "id": "251",
        "childNodes": [{
            "parentNode": "251",
            "title": "Ward 1",
            "id": "2511",
            "childNodes": [],
        },{
            "parentNode": "251",
            "title": "Ward 2",
            "id": "2512",
            "childNodes": [],
        },{
            "parentNode": "251",
            "title": "Ward 3",
            "id": "2513",
            "childNodes": [],
        }]       
    },
    {
        "parentNode": "25",
        "title": "Section 2",
        "id": "252",
        "childNodes": [{
            "parentNode": "252",
            "childNodes": [],
            "title": "Ward 1",
            "id": "2521"
        }],
    },{
        "parentNode": "25",
        "title": "Section 3",
        "id": "253",
        "childNodes": [{
            "parentNode": "253",
            "childNodes": [],
            "title": "Ward 1",
            "id": "2531"
        }],
    }],
}, {
    "parentNode": null,
    "title": "Department Two",
    "id": "26",
    "childNodes": [{
        "parentNode": "26",
        "title": "Section 3",
        "id": "261",
        "childNodes": [{
            "title": "Ward 1",
            "id": "2612",
            "parentNode": "261",
            "childNodes": [],          
        }],
    },
    {
        "parentNode": "26",
        "title": "OPD 2",
        "id": "262",
        "childNodes": [],
    }],
},
{
    "parentNode": null,
    "title": "OPDs",
    "id": "27",
    "childNodes": [
        {
            "parentNode": "27",
            "title": "OPD1",
            "id": "271",
            "childNodes": [],
        }
    ],
}] */