import React, { Component } from 'react';
import XWindow from '../components/XWindow'
import { WindowsTheme } from '../components/XWindowTheme';
import { connect } from 'react-redux';
import { openNewWindow, resotreXWindow, appListGet, judgePermission } from '../actions/actionCreators';
import AppBar from '../components/AppBar'
import * as uiSize from '../constants/uiSize';
import DesktopIcon from '../components/DesktopIcon'
import getToolComponent, { getToolDefine } from '../tools'
import ToolMenuPermissionDefine from '../constants/menuPermissionDefine';

const paneStyle = {
  width: '70%',
  height: '80%',
  top: '10%',
  left: '10%'
};


const styles = {
  app: {
    position: 'absolute',
    width: '100%',
    height: '100%',
    background: "url('../../images/background/background.jpg') center center"
  },
  wndContainer: {
    position: 'absolute',
    top: uiSize.APPBAR_HEIGHT,
    bottom: 0,
    left: 0,
    right: 0,
  },
  desktop: {
    position: 'absolute',
    zIndex: 1,
    top: '1rem',
    left: '1rem',
    bottom: '1rem',
    right: '1rem',
    display: 'flex',
    flexDirection: 'column',
    flexWrap: 'wrap',
    justifyContent: 'flex-start',
    alignContent: 'flex-start'
  },
  iframe: {
    border: 'none',
    width: '100%',
    height: '100%',
    overflow: 'hidden',
    position: 'absolute'
  }
}

class App extends Component {
  static defaultProps = {
    color: '#cc7f29',
    theme: 'light'
  };
  defaultStatus = {
    counter: 0
  };

  state = {
    //getToolComponent: null,
    appList: []//已经获取了权限的工具
  }

  constructor(props) {
    super();
    //this.openNewWindow = this.openNewWindow.bind(this);
    // import('../tools').then((a)=>{
    //   this.setState({getToolComponent: a.default});
    // });
  }

  componentWillMount() {
    this.props.dispatch(appListGet());
  }

  componentWillReceiveProps(newProps) {
    if (newProps.appList && this.state.appList.length !== newProps.appList.length) {
      newProps.appList.map(app => {
        this.getToolPermissions(app.clientId);
      });
      this.setState({ appList: newProps.appList });
    }
    console.log("judgePermissions:", newProps.judgePermissions);
  }

  getToolPermissions(tid) {
    let toolPermission = [];
    if (ToolMenuPermissionDefine) {
      let tool = ToolMenuPermissionDefine.find(tool => tool.id === tid);
      if (tool) {
        tool.permissions.map(p => {
          toolPermission.push(p.id);
        });
      }
    }
    if (toolPermission.length > 0) {
      this.props.dispatch(judgePermission(toolPermission));
    }
  }

  createWindow(wi) {
    if (wi.type === 'viewMessage') {

    } else if (wi.type === "tool") {
      let tid = wi.tool;
      // if(!this.state.getToolComponent)
      // return;
      let ToolComponent = getToolComponent(tid);
      return (<XWindow parent={this.wndContainer}  {...WindowsTheme} key={wi.id} {...wi} defaultMaxmize={true} style={paneStyle} dispatch={this.props.dispatch}>
        <ToolComponent windowsId={wi.id} rootPath={tid} dispatch={this.props.dispatch} />
      </XWindow>);
    } else if (wi.type === "web") {
      return (<XWindow parent={this.wndContainer}  {...WindowsTheme} key={wi.id} {...wi} style={paneStyle}>
        <iframe style={styles.iframe} title={wi.title} src={wi.url} />
      </XWindow>);
    }
  }

  render() {
    let { dispatch, wndList, appList } = this.props;
    return (
      <div style={styles.app}>
        <AppBar />
        <div ref={(c) => { this.wndContainer = c; }} style={styles.wndContainer}>
          <div style={styles.desktop}>
            {
              appList.map((app, i) => {
                let define = getToolDefine(app.clientId);
                if (!define)
                  return null;

                return (
                  <DesktopIcon key={i} appInfo={app} icon={define.icon} />
                )
              })
            }
          </div>
          {
            wndList.map(wi => this.createWindow(wi))
          }
        </div>
      </div>
    );
  }
}


function mapStateToProps(state) {
  return {
    wndList: state.windows.list,
    appList: state.app.appList,
    oidc: state.oidc,
    judgePermissions: state.app.judgePermissions
  };
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(App);
