import React, { useState } from 'react';
import { Collapse, NavItem, NavLink } from 'reactstrap';
import classNames from 'classnames';
import { Link } from 'react-router-dom';
import './SubMenu.css'

const SubMenu = props => {

  const [collapsed, setCollapsed] = useState(true)
  const toggleNavbar = () => setCollapsed(!collapsed)
  const { title, items, target } = props;

  return (
    <div>
      <NavItem onClick={toggleNavbar} className={classNames({ 'menu-open': !collapsed })} >
        <NavLink className={title == "Overview" ? '' : 'dropdown-toggle'} tag={Link} to={target} style={{ color: 'white', textDecoration: 'none' }} >
          <div className="mr-2" />{title}
        </NavLink>
      </NavItem>
      <Collapse isOpen={title == "Overview" ? false : !collapsed} navbar className={classNames('items-menu', { 'mb-1': !collapsed })}>
        {items.map((item, index) => (
          <NavItem key={index} className="pl-4" style={{ textAlign: 'left' }}>
            <NavLink tag={Link} to={item.target} style={{ color: 'white', textDecoration: 'none' }} activestyle={{ color: 'red', textDecoration: 'none' }} >
              {item.title}
            </NavLink>
          </NavItem>
        ))}
      </Collapse>
    </div>
  );
}

export default SubMenu;
