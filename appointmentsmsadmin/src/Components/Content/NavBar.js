import React from 'react';
import {
  Navbar, NavbarToggler, Collapse, Nav, UncontrolledDropdown,
  DropdownToggle, DropdownMenu, DropdownItem, NavbarText
} from 'reactstrap';
import { FormattedMessage } from "react-intl";
import { createHashHistory } from "history";
import { Logout } from '../../Services/AuthService';

import './NavBar.css';

class NavBar extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isOpen: true,
      setOpen: true,
      redirect: false,
      username: '',
      role: '',
      hospital: ''
    }
    this.Logout = this.Logout.bind(this);
  }

  componentDidMount() {
    var usersession = localStorage.getItem('usersession');
    var userSessionObj = JSON.parse(usersession);

    this.setState({ 
      username: userSessionObj.UserName,
      role: userSessionObj.Role,
      hospital: userSessionObj.Hospital
    });
  }

  Logout = () => {
    console.log("Logout");
    Logout();
    this.setState({ redirect: true });
    const history = createHashHistory();
    history.go("/login");
  }

  render() {
    return (
      <Navbar color="light" light className="navbar shadow-sm p-2 mb-4 bg-white rounded" expand="md">
        
        <Collapse isOpen={this.state.isOpen} navbar>
          <Nav className="mr-auto offset-4" navbar>
          <NavbarText> 
            <h4>{this.state.hospital}</h4>
            </NavbarText>
          </Nav>
          <UncontrolledDropdown>
            <DropdownToggle nav caret>{this.state.username}</DropdownToggle>
            <DropdownMenu right>
              <DropdownItem><FormattedMessage id="User.Role" defaultMessage="UserRole:"></FormattedMessage>{this.state.role}</DropdownItem>
              <DropdownItem divider />
              <DropdownItem onClick={this.Logout}>
                <FormattedMessage id="Logout" defaultMessage="Logout"></FormattedMessage>
              </DropdownItem>
            </DropdownMenu>
          </UncontrolledDropdown>
        </Collapse>
      </Navbar>
    );
  }
}

export default NavBar
