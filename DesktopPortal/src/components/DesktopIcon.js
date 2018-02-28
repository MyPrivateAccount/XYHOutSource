import React, { Component } from 'react';
import { Button, Icon } from 'antd'
import { openNewWindow, resotreXWindow, openOrActiveWindow } from '../actions/actionCreators';
import { connect } from 'react-redux';
import './DesktopIcon.less'


class DesktopIcon extends Component {

    handleDblClick = (event) => {
        let { appInfo } = this.props;
        if (this.props.mutipleInstance) {
            this.props.dispatch(openNewWindow({ title: appInfo.displayName, tool: appInfo.clientId, type: 'tool' }));
        } else {
            this.props.dispatch(openOrActiveWindow({ title: appInfo.displayName, tool: appInfo.clientId, type: 'tool' }));
        }
    };

    render() {
        let { appInfo } = this.props;
        return (
            <Button className="desktop-icon" onDoubleClick={(event) => this.handleDblClick(event)}>
                {this.props.icon}
                <span className="label" >{appInfo.displayName} </span>
            </Button>
        )
    }
}

function appBarMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(null, appBarMapDispatchToProps)(DesktopIcon);