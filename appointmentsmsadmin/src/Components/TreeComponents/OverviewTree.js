import React, { Component, useState } from 'react';
import { Badge } from 'reactstrap';
import Tree from 'react-animating-tree'

import * as Icons from './../TreeComponents/icons'
import { getOrgNamesfromUnitGids } from './../Controls/DualListBox/DualListBoxUtils'

import 'bootstrap/dist/css/bootstrap.css';
import './OverviewTree.css';

const styles = {
    leaf: {
        cursor: 'pointer',
        display: 'inline'
    },
    icon: {
        width: '1em',
        height: '1em',
        marginLeft: 5,
        cursor: 'cursor',
    }
}

class OverviewTree extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nodeData: []
        };

        this.loadSchedule = this.loadSchedule.bind(this);
    }

    loadSchedule = (node) => {
        this.props.scheduleClicked(node.id);
    }

    loadTemplate = (node) => {
        this.props.templateClicked(node.id);
    }

    getContent(currentNode) {

        if (currentNode.type === "department") {
            return (<span style={{}} >{currentNode.title}</span>);
        } else if (currentNode.type === "schedule") {
            let schStyle = (currentNode.tag.isGlobal ? { color: 'blueviolet' } : {});
            return (<span style={schStyle} onClick={() => this.loadSchedule(currentNode)}>
                {currentNode.title}
                {currentNode.tag.daysCount &&
                    <Badge style={{ marginLeft: '5px' }} id="sch_exinfo" color="secondary">{currentNode.tag.daysCount} day</Badge>
                }
                {(currentNode.tag.excludedIds && this.props.showExcludedOrgUnits) &&
                    <span id="sch_exinfo" style={{ paddingLeft: '5px' }}>
                        {getOrgNamesfromUnitGids(currentNode.tag.excludedIds).map((orgUnit, index) => (<Badge style={{ marginLeft: '5px' }} id={index} color="danger">{orgUnit.title}</Badge>))}
                    </span>
                }
            </span>);
        }
        else if (currentNode.type === "template") {

            let isHighlight = !(currentNode.tag.isSMSGenerate);

            return (
                <span className={isHighlight ? 'no-sms' : null} onClick={() => this.loadTemplate(currentNode)}>
                    {currentNode.title}
                    {(currentNode.tag.targetOpd && this.props.showTargetOrgUnits) &&
                        <Badge style={{ marginLeft: '5px' }} id="t_opdorgunit" color="success">{currentNode.tag.targetOpd}</Badge>
                    }
                    {(currentNode.tag.targetOrgUnit && this.props.showTargetOrgUnits) &&
                        <Badge style={{ marginLeft: '5px' }} id="t_orgunit" color="success">{currentNode.tag.targetOrgUnit}</Badge>
                    }
                    {(currentNode.tag.location && this.props.showTargetOrgUnits) &&
                        <Badge style={{ marginLeft: '5px' }} id="t_location" color="success">{currentNode.tag.location}</Badge>
                    }
                </span>);
        }
        return (<span>{currentNode.title}</span>);
    }

    render() {

        return !this.props.nodeData.length ? null : (
            <div style={{ textAlign: 'left' }}>
                <Tree key={this.props.id} open={this.props.open} content={this.props.content} toggleImmediate={true} >
                    {this.props.nodeData.map((currentNode, index) => (
                        !currentNode.childNodes.length > 0 ?
                            <Tree key={currentNode.id} content={
                                <div style={{ ...styles.leaf }}>
                                    <span className="treeNode" >{this.getContent(currentNode)}</span>
                                    {/* <span className="treeNode" onClick={() => this.props.onLeafNodeClick(currentNode)} >{currentNode.title}</span>
                                    <span className="leafSection" onClick={() => this.treeNodeClick(currentNode)} >&nbsp; <span className="applicableUnits" hidden={orgUnits}>[Section X]</span></span> */}
                                </div>
                            } ></Tree> :
                            <OverviewTree nodeData={currentNode.childNodes}
                                key={currentNode.id}
                                content={
                                    <div style={{ ...styles.leaf }}>
                                        <span className="treeNode" >{this.getContent(currentNode)}</span>
                                    </div>
                                }
                                open={false}
                                showExcludedOrgUnits={this.props.showExcludedOrgUnits}
                                showTargetOrgUnits={this.props.showTargetOrgUnits}
                                showInactiveSchedules={this.props.showInactiveSchedules}
                                scheduleClicked={this.props.scheduleClicked}
                                templateClicked={this.props.templateClicked}></OverviewTree>
                    )
                    )}
                </Tree>
            </div>
        );
    }
}
export default OverviewTree
