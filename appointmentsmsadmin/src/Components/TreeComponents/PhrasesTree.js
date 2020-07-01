import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import Tree from 'react-animated-tree'

const styles = {
    leaf: {
        cursor: 'pointer',
        display: 'inline',
    },
    icon: {
        width: '1em',
        height: '1em',
        marginLeft: 5,
        cursor: 'cursor',
    },
    sleaf: {
        cursor: 'pointer',
        display: 'inline',
        color: '#0000e6',
        transition: '1s'
    }
}

class PhrasesTree extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nodeData: [],
        };
    }

    render() {
        return !this.props.nodeData.length ? null : (
            <div style={{ textAlign: 'left' }}>
                <Tree key={this.props.id} open={this.props.open} selected={this.props.selected} content={this.props.title}>
                    {this.props.nodeData.map((currentNode) => (
                        !currentNode.childNodes.length > 0 ?
                            <Tree key={currentNode.id} selected={this.props.selected} content={
                                <div style={this.props.selected === currentNode.id ? { ...styles.sleaf } : { ...styles.leaf }}>
                                    <span className="treeNode" onClick={() => this.props.onLeafNodeClick(currentNode)} >{currentNode.title}</span>
                                </div>
                            } ></Tree> :
                            <PhrasesTree title={currentNode.title} id={currentNode.id} nodeData={currentNode.childNodes}
                                key={currentNode.id}
                                content={currentNode.title}
                                open={true}
                                selected={this.props.selected}
                                onLeafNodeClick={this.props.onLeafNodeClick}></PhrasesTree>
                    )
                    )}
                </Tree>
            </div>
        );
    }
}
export default PhrasesTree
