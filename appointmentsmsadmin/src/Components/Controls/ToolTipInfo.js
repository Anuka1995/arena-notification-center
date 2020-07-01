import React, { Component} from 'react';
import { Tooltip } from 'reactstrap'
import { FormattedMessage } from "react-intl";
/* 
you must set a unique id otherwise multiple tooltips will be shown at once
props{
    id: should be unique from other tooltipinfos
    placement: {'auto', 'auto-start', 'auto-end',  'top', 'top-start', 'top-end', 'right', 'right-start',
        'right-end', 'bottom', 'bottom-start', 'bottom-end', 'left', 'left-start', 'left-end',},
    formatMessageId: MessageId of the data.json file,
    defaultMessage: "Fallback message if the translation key is no found"
}
*/
class ToolTipInfo extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            isOpen: false
         }
         this.ToggleToolTip = this.ToggleToolTip.bind(this)
    }

    ToggleToolTip = e => {
        this.setState({
            isOpen: !this.state.isOpen
        })
    }


    render() { 
        return ( 
            <div className="d-inline">
                <Tooltip placement={this.props.placement} isOpen={this.state.isOpen} 
                    target={this.props.id} toggle={this.ToggleToolTip}>
                    <FormattedMessage id={this.props.formatMessageId} defaultMessage={this.props.defaultMessage} />
                </Tooltip>
                <span id={this.props.id} style={{color: "blue", opacity: "0.5", margin: "5px"}}>
                    <sup><i className="fas fa-info-circle"></i></sup>
                </span>
            </div>
         );
    }
}
 
export default ToolTipInfo;
