import React, { Component } from 'react';
import { withRouter } from 'react-router';
import Breadcrumb from 'react-bootstrap/Breadcrumb';
import { FormattedMessage } from "react-intl";
import  SaveTagForm  from './SaveTagForm';

class SaveTagPage extends Component {
  constructor(props)
  {
      super(props);

      this.handleCreateSucess = e => {
        this.props.history.push("/SearchPhrase");
      };

      this.handleCreationCancelled = e => {
        sessionStorage.removeItem('tagitem');
        this.props.history.push("/Overview");
      }
  }

  render() {
    return (
      <div>
        <Breadcrumb>
            <Breadcrumb.Item href="#"><FormattedMessage id="Phrases" defaultMessage="tags"/></Breadcrumb.Item>
            <Breadcrumb.Item active><FormattedMessage id="Create" defaultMessage="Add tags"/></Breadcrumb.Item>
        </Breadcrumb>

        <SaveTagForm onSaveSuccess={this.handleCreateSucess}
        onSaveCancel={this.handleCreationCancelled} />
      </div>
    )
  }
}
export default withRouter(SaveTagPage)