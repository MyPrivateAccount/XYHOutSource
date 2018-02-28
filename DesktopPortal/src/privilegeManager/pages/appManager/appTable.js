import { connect } from 'react-redux';
import { appAdd, appEdit, appListGet, setLoadingVisible, appDelete } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Spin, Popconfirm, Tooltip } from 'antd'
import AppEditor from './appEdit';
import { ApplicationTypes } from '../../../constants/baseConfig'

const { Header, Sider, Content } = Layout;

class AppsTable extends Component {
    state = {
        checkList: []
    }
    appTableColumns = [
        { title: '应用编码', dataIndex: 'clientId', key: 'clientId' },
        { title: '应用名称', dataIndex: 'displayName', key: 'displayName' },
        {
            title: '应用类型', dataIndex: 'applicationType', key: 'applicationType', render: (text, recored) => {
                let appType = '';
                let app = ApplicationTypes.find((app) => app.key == recored.applicationType);
                appType = (app != undefined ? app.value : recored.applicationType);
                return (
                    <span>{appType}</span>
                )
            }
        },
        { title: '退出地址', dataIndex: 'postLogoutRedirectUris', key: 'postLogoutRedirectUris' },
        { title: '跳转地址', dataIndex: 'redirectUris', key: 'redirectUris' },
        {
            title: '编辑', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <div>
                    <Tooltip title="应用编辑">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleEditClick(recored)} />
                    </Tooltip>
                    <Tooltip title="删除应用">
                        <Popconfirm title="确认要删除选中应用?" onConfirm={(e) => this.confirmDelete(e, recored.id)} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                            &nbsp;<Button type='primary' size='small' shape='circle' icon='delete' />
                        </Popconfirm>
                    </Tooltip>
                </div>
            )
        }
    ];
    componentWillMount() {
        console.log("加载app列表");
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(appListGet());
    }


    handleAddClick = (event) => {
        this.props.dispatch(appAdd());
    }
    handleRefreshClick = (event) => {
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(appListGet());
    }

    handleEditClick = (appInfo) => {
        console.log("handleEditClick" + JSON.stringify(appInfo));
        this.props.dispatch(appEdit(appInfo));
    }
    confirmDelete = (e, appId) => {
        this.props.dispatch(appDelete(appId));
    }

    cancelDelete = (e) => {
        console.log("不删除", e);

    }
    handleCheckChange = (e, appInfo) => {
        //console.log("checkbox change：" + JSON.stringify(e.target.checked));
        let appId = appInfo.clientId;
        let checked = e.target.checked;
        let checkList = this.state.checkList.slice();
        var hasValue = false;
        for (var i in checkList) {
            if (checkList[i].id == appId) {
                checkList[i].status = checked;
                hasValue = true;
                break;
            }
        }
        if (!hasValue) {
            checkList.push({ id: appId, status: checked });
        }
        this.setState({ checkList: checkList });
    }
    render() {
        //let removeBtnDisabled = this.state.checkList.filter((item) => item.status).length > 0 ? false : true;
        return (
            <div className="relative">
                <Layout>
                    <Header>
                        应用管理
                        <Tooltip title="新增应用">
                            &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                        </Tooltip>
                        <Tooltip title="刷新">
                            &nbsp;<Button type='primary' shape='circle' icon='sync' onClick={this.handleRefreshClick} />
                        </Tooltip>
                    </Header>
                    <Content className='content'>
                        <Spin spinning={this.props.showLoading}>
                            <Table columns={this.appTableColumns} dataSource={this.props.appList} style={{ marginTop: '0.5rem' }} />
                            <AppEditor />
                        </Spin>
                    </Content>
                </Layout>
            </div>
        )
    }
}

function apptableMapStateToProps(state) {
    console.log('应用apptableMapStateToProps:' + JSON.stringify(state.app));
    return {
        activeTreeNode: state.app.activeTreeNode,
        operInfo: state.app.operInfo,
        appList: state.app.appList,
        showLoading: state.emp.showLoading
    }
}

function apptableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(apptableMapStateToProps, apptableMapDispatchToProps)(AppsTable);