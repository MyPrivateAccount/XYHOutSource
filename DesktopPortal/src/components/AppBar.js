import React, {Component} from 'react';
import {Affix, Icon, Badge, Dropdown, Menu, Avatar, Popover, Row, Col, Pagination} from 'antd';
import {connect} from 'react-redux';
import Time from 'react-time-format'
import userManager from '../utils/userManager';
import * as uiSize from '../constants/uiSize';
import {closeWindow, resotreXWindow, minimizeWindow} from '../actions/actionCreators';
import './AppBar.less';
import ModifyPassword from './ModifyPassword';
import {getUnReadCount, setLoadingVisible, getRecieveList, getMsgDetail} from '../actions/actionCreators';
import MessageDetail from './MessageDetail';

class AppBarRight extends Component {
    state = {
        anchorEl: null,
        open: false,
    };

    handleClick = event => {
        this.setState({open: true, anchorEl: event.currentTarget});
    };

    handleSignOut = () => {
        this.setState({open: false});
        return userManager.signoutRedirect().then(() => {
        });

    };

    handleRequestClose = () => {
        this.setState({open: false});
    }

    handleModifyPassword = () => {
        this.setState({open: true});
    }
    dialogVisibleChanged = (v) => {
        this.setState({open: v});
    }

    handleMenuClick = ({item, key, keyPath}) => {
        console.log(key);
        if (key === 'modifyPassword') {
            this.handleModifyPassword();
        } else if (key === 'logout') {
            this.handleSignOut();
        }
    }

    render() {
        const profile = ((this.props.user || {}).profile || {});
        let userName = (profile.nickname || profile.name) || '未登录';
        let img = profile.picture || null;
        return (
            <div>
                <Dropdown overlay={
                    <Menu theme="dark" className="windows-menu" onClick={this.handleMenuClick}>
                        <Menu.Item key="modifyPassword">
                            <a href="javascript:void(0)" >修改密码</a>
                        </Menu.Item>
                        <Menu.Item key="logout">
                            <a href="javascript:void(0)">退出</a>
                        </Menu.Item>
                    </Menu>
                } placement="bottomRight"

                    trigger={["click"]}>
                    <span className="user-panel">
                        <Avatar src={img} className="user-icon">
                            {img ? null : (userName || ' ').substring(0, 1)}
                        </Avatar>
                        <span className="user-name"> {userName}</span>
                    </span>
                </Dropdown>
                <ModifyPassword visible={this.state.open} onVisibleChange={this.dialogVisibleChanged}
                />
            </div>

        );

    }
}

function mapStateToProps(state) {
    return {
        user: state.oidc.user,

    };
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

let UserInfo = connect(mapStateToProps, mapDispatchToProps)(AppBarRight);


class AppBarEx extends Component {
    state = {
        date: Date.now(),
        open: false,
        msgDialogVisible: false,//消息框
        pagination: {current: 1, pageSize: 10, total: 0},
        searchCondition: {pageIndex: 0, pageSize: 10}
    }

    componentDidMount() {
        this.setState({open: false})
        this.tid = setInterval(() => {
            this.setState({date: Date.now()});
        }, 1000);
        this.unReadtid = setInterval(() => {
            this.props.dispatch(getUnReadCount());
        }, 60000);
    }
    componentWillUnmount() {
        if (this.tid) {
            clearInterval(this.tid);
        }
        if (this.unReadtid) {
            clearInterval(this.unReadtid);
        }
    }

    handleVisibleChange = (flag) => {
        this.setState({open: flag});
    }
    handleLoadMsgList = () => {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getRecieveList(this.state.searchCondition));
    }

    handleRestory = (event, value) => {
        this.props.dispatch(resotreXWindow({id: value.id, start: true}));
        this.setState({open: false});
    }

    handleClose = (event, value) => {
        this.props.dispatch(closeWindow(value.id));
        if (this.props.wndList.length === 1) {
            this.setState({open: false});
        }
    }
    handleMessageOpen = (e) => {
        console.log("打开", e, e.target);
        if (e && e.key) {
            this.setState({msgDialogVisible: true});
            this.props.dispatch(setLoadingVisible(true));
            this.props.dispatch(getMsgDetail(e.key));
            setTimeout(() => this.props.dispatch(getUnReadCount()), 500);
        }
    }
    MessageClose = () => {
        this.setState({msgDialogVisible: false});
    }

    componentWillReceiveProps = (newProps) => {
        let paginationInfo = {
            pageSize: newProps.msgList.pageSize,
            current: newProps.msgList.pageIndex + 1,
            total: newProps.msgList.totalCount
        };
        this.setState({pagination: paginationInfo});
    }
    //翻页处理
    handleChangePage = (pageIndex, pageSize) => {
        let condition = {...this.state.searchCondition};
        condition.pageIndex = (pageIndex - 1);
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getRecieveList(condition));
    }
    //最小化所有窗口
    minAllWindow = () => {
        let minBtnArray = document.querySelectorAll("a[title=Minimize]");
        if (minBtnArray && minBtnArray.length > 0) {
            for (let i = 0; i < minBtnArray.length; i++) {
                minBtnArray[i].click();
            }
        }
    }
    render() {
        const {wndList, message} = this.props;
        let wndListMenu = null;
        if (wndList.length > 0) {
            wndListMenu = <div>
                <Dropdown overlay={
                    <Menu theme="dark" className="windows-menu">
                        {wndList.map(wi => (
                            <Menu.Item key={wi.id}>
                                <a href="javascript:void(0)" onClick={(event) => this.handleRestory(event, wi)}>{wi.title}</a>
                                <span className="close-icon" onClick={(event) => this.handleClose(event, wi)}>
                                    <Icon type="close" />
                                </span>
                            </Menu.Item>

                        ))}
                    </Menu>
                } placement="bottomRight"
                    trigger={["click"]}
                    onVisibleChange={this.handleVisibleChange}
                    visible={this.state.open}>
                    <Icon type="windows-o" className="windows-icon" />
                </Dropdown>
            </div>
        }
        //消息相关
        const msgList = this.props.msgList ? (this.props.msgList.extension || []) : [];
        //console.log("消息列表:", msgList);
        let msgListContent = <div style={{margin: '5px', width: '300px'}}>
            <Menu mode="inline" onClick={this.handleMessageOpen}>
                {
                    msgList.map((msg, i) =>
                        <Menu.Item key={msg.id} >
                            <Row style={{padding: '0', marginLeft: '-20px', fontWeight: 'bold'}}>
                                <Col span={16} style={{width: '150px', overflow: 'hidden', textOverflow: 'ellipsis'}}>{!msg.isRead ? <Badge dot>{msg.title}</Badge> : <b>{msg.title}</b>}</Col>
                                <Col span={8}>{(msg.sendTime || "").replace("T", " ")}</Col>
                            </Row>
                        </Menu.Item>)
                }
            </Menu>
            <Pagination {...this.state.pagination} onChange={this.handleChangePage} size='small' style={{margin: '5px'}} />
        </div>;
        const unReadCount = message.unReadCount;
        return (
            <Affix className="appbar">
                <div className="right-bar">
                    {/* <Icon type="link" className="logo" /> */}
                    <img src='../../images/x.png' style={{height: '2rem', cursor: 'pointer'}} onClick={this.minAllWindow} />
                    <div className="title">{/*新耀行*/}</div>

                    {wndListMenu}

                    <Popover placement="bottom" title="系统消息" content={msgListContent} trigger="click">
                        <Badge count={unReadCount} className="message-badge">
                            <Icon type="mail" className="message-icon" style={{cursor: 'pointer'}} onClick={this.handleLoadMsgList} />
                        </Badge>
                    </Popover>

                    {/* <Icon type="android" className="message-icon" style={{cursor: 'pointer'}} onClick={()=>window.open('/download')}/> */}
                    <UserInfo height={uiSize.APPBAR_HEIGHT}
                        iconSize={uiSize.APPBAR_ICON_SIZE}
                        labelStyle={{fontSize: uiSize.APPBAR_USERNAME_FONTSIZE}} />
                    <div>
                        <Time className="time" value={this.state.date} format="HH:mm" />
                        <Time className="date" value={this.state.date} format="YYYY/MM/DD" />
                    </div>

                </div>
                <MessageDetail visible={this.state.msgDialogVisible} onMessageClose={this.MessageClose} />

            </Affix>
        )
    }
}

function appBarMapStateToProps(state) {
    return {
        msgList: state.message.receiveList,
        wndList: state.windows.list,
        message: state.message
    };
}

function appBarMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(appBarMapStateToProps, appBarMapDispatchToProps)(AppBarEx);
