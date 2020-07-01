import React, { Component, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import Tree from 'react-animated-tree'
import * as Icons from './../TreeComponents/icons'
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

class TextTemplateTree extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nodeData: [],
            showIcon: "hidden"
        };
        this.treeNodeClick = this.treeNodeClick.bind(this);
    }

    treeNodeClick(e) {
        console.log(e.title, this.state);
        this.state.showIcon === 'hidden' ?
            this.setState({
                showIcon: 'visible'
            }) : this.setState({ showIcon: 'hidden' })
    }

    render() {
        return !this.props.nodeData.length ? null : (
            <div style={{ textAlign: 'left' }}>
                <Tree key={this.props.id} open={this.props.open} content={this.props.content}
                    onClick={(key) => this.treeNodeClick(key)}>
                    {this.props.nodeData.map((currentNode, index) => (
                        !currentNode.childNodes.length > 0 ?
                            <Tree key={currentNode.id} content={
                                <div style={{ ...styles.leaf }}>
                                    <span className="treeNode" onClick={() => this.props.onLeafNodeClick(currentNode)} >
                                    {(currentNode.type ==='template' && !currentNode.tag.isGenerateSMS )? <span class='no-sms'>{currentNode.title}</span> : <span>{currentNode.title}</span>}</span>
                                </div>
                            } ></Tree> :
                            <TextTemplateTree nodeData={currentNode.childNodes}
                                key={currentNode.id}
                                onLeafNodeClick={this.props.onLeafNodeClick}
                                content={currentNode.title}
                                open={false}></TextTemplateTree>
                    )
                    )}
                </Tree>
            </div>
        );
    }
}
export default TextTemplateTree
