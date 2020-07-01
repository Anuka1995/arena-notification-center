import React, { Component } from 'react';
import { Row, Col, FormGroup, Label, Input, CustomInput } from 'reactstrap';
import { FormattedMessage } from "react-intl";
import {AvField, AvGroup, AvInput} from 'availity-reactstrap-validation';

import ToolTipInfo from '../Controls/ToolTipInfo'

class ViewDetails extends Component {
  constructor(props) {
    super(props);
    this.state = {
    };
  }

  render() {

    let isXpath = (this.props.tagType === 0);
    let isEditMode = (this.props.guid != null);

    let numbersOnlyPattern =  this.props.tagDataType=== 1 ? {
      pattern:  { value: '^[0-9,]+$', errorMessage: 'only numbers are allowed' }
    } : null

    return (
      <div>
        <FormGroup row align="left">

          <Col md={10}>
            <Row>
              <Label sm={4}><FormattedMessage id="Text.TagName" defaultMessage="TagName" /><span class="required">*</span></Label>
              <Col sm={8}>
                <AvField name="TagName" id="TagName" placeholder="Phrase Name"
                  value={this.props.tagName}
                  onChange={this.props.onChangeTagName} disabled={isXpath || isEditMode} validate={{
                    required: { value: true, errorMessage: 'Please enter a name' },
                    pattern: { value: '^[A-Za-z0-9_]+$', errorMessage: 'This value is invalid' }
                  }} />
              </Col>
            </Row>
          </Col>
          <span style={{ marginLeft: 'inherit' }} >
            <ToolTipInfo id="phraseNameTooltipId"
              formatMessageId="TooltipNoSpaceForName" defaultMessage="Please-refrain-from-using-spaces-for-Phrase-name"
              placement="top" />
          </span>
        </FormGroup>
        <FormGroup row align="left">
          <Col md={10}>
            <Row>
              <Label sm={4}><FormattedMessage id="TagDescription" defaultMessage="TagDescription" /></Label>
              <Col sm={8}>
                <Input type="#" name="TagDescription" id="TagDescription" placeholder="Description here"
                  value={this.props.tagDesc}
                  onChange={this.props.onChangeTagDesc} disabled={isXpath} />
              </Col>
            </Row>
          </Col>
        </FormGroup>
        <FormGroup row align="left">
          <Col md={10}>
            <Row>
              <Label sm={4}><FormattedMessage id="DataType" defaultMessage="DataType" /></Label>
              {isXpath ?
                this.props.XpathTagdataTypes.map((type, index) => {
                  let idName = "tagDataTypeRadio-" + index;
                  let typeName = type.Name;
                  let imChecked = (this.props.tagDataType === type.Value);
                  return (
                    <div key={index}>
                      <FormGroup check>
                        <Label check sm={12}>
                          <CustomInput type="radio" id={idName} label={typeName} value={typeName} checked={!!imChecked}
                            disabled={isXpath} />
                        </Label>
                      </FormGroup>
                    </div>);
                }) :
                this.props.tagDataTypes.map((type, index) => {
                  let idName = "tagDataTypeRadio-" + index;
                  let typeName = type.Name;
                  let imChecked = (this.props.tagDataType === type.Value);
                  return (
                    <div key={index}>
                      <FormGroup check>
                        <Label check sm={12}>
                          <CustomInput type="radio" id={idName} name="customRadioDataType"
                            label={typeName}
                            value={typeName}
                            checked={!!imChecked}
                            onChange={this.props.onChangeTagDataType} disabled={isXpath || isEditMode} required />
                        </Label>
                      </FormGroup>
                    </div>);
                })
              }
            </Row>
          </Col>
        </FormGroup>
        <AvGroup row align="left">
          <Col md={10}>
            <Row>
              <Label sm={4}><FormattedMessage id="Text.Value" defaultMessage="Value" /><span class="required">*</span></Label>
              <Col sm={8} md={8} lg={8}>
                  <AvField type="textarea" name="Value"
                           id="Value" placeholder=""
                           validate={{
                             required: { value: true, errorMessage: 'This value is required' },
                              ...numbersOnlyPattern
                           }}
                           value={this.props.tagValue}
                           onChange={this.props.onChangeTagValue}
                           disabled={isXpath}
                           style={{ minHeight: 'max-content'}} />
              </Col>
            </Row>
          </Col>
        </AvGroup>
        <FormGroup row align="left">
          <Label sm={4}>
            <CustomInput type="switch" id="isActiveCustomSwitch"
              name="customSwitch"
              label={<FormattedMessage id="ActivePhrase" />}
              onChange={this.props.onChangeIsActive}
              value={this.props.tagIsActive}
              checked={this.props.tagIsActive} disabled={isXpath} />
          </Label>
        </FormGroup>
      </div>
    )
  }
}
export default ViewDetails