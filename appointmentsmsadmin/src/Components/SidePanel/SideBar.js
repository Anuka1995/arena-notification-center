import React from 'react';
import SubMenu from './SubMenu';
import { Nav } from 'reactstrap';
import classNames from 'classnames';
import './SideBar.css';

class SideBar extends React.Component {
  constructor(props) {
    super(props)
    this.state = {}
  }

  render() {
    return <div className={classNames('sidebar', { 'is-open': this.props.isOpen })}>
      <div className="side-menu">
        <Nav vertical className="list-unstyled" >
          <div className="sidebar-header" >
            <img src="./images/logo-dips1.png" className="align-center" alt="DIPS logo" />
          </div>
          <SubMenu title="Overview" items={[]} target="/" />
          <SubMenu title="Sending Rules" items={submenus[1]} target="#" />
          <SubMenu title="SMS content Templates" items={submenus[2]} target="#" />
          <SubMenu title="Phrases" items={submenus[3]} target="#" />
          <SubMenu title="Group Templates" items={submenus[4]} target="#" />
          <SubMenu title="Configuration" items={submenus[5]} target="#" />
        </Nav>
      </div>
    </div>
  }
}

const submenus = [
  [],
  [
    {
      title: "Create",
      target: "/Schedule"
    },
    {
      title: "Edit/Search",
      target: "/SearchSchedule",
    }
  ],
  [
    {
      title: "Create",
      target: "TextTemplate",
    },
    {
      title: "Edit/Search",
      target: "SearchTextTemplate",
    },
    {
      title: "Filter",
      target: "FilterTextTemplate",
    }
  ],
  [
    {
      title: "Create",
      target: "Phrase",
    },
    {
      title: "View Phrases",
      target: "SearchPhrase",
    }
  ],
  [
    {
      title: "Create",
      target: "GroupTemplate",
    },
    {
      title: "Edit/Search",
      target: "/SearchGroupTemplate",
    }
  ],
  [
    {
      title: "PSS Link",
      target: "/PSSLink"
    },
    {
      title: "Time Format",
      target: "/TimeFormat"
    }
  ]
]


export default SideBar;
