import React, {Component} from 'react';
import {connect} from 'react-redux';
import {orgNodeAdd, orgNodeEdit, getOrgDataByID, orgNodeSelected, orgNodeDelete, setLoadingVisible, empListGet, getAreaList} from '../../actions/actionCreator';
import {Layout, Table, Button, Checkbox, Tree, Tabs, Icon, Popconfirm, Spin, Tooltip} from 'antd';
import OrgEditor from './orgEditor';
import OrgDetail from './orgDetail';
import EmpTable from '../employee/empTable';
import SearchCondition from '../../constants/searchCondition';

const {Header, Sider, Content} = Layout;
const TreeNode = Tree.TreeNode;
const TabPane = Tabs.TabPane;

class OrgTable extends Component {
    state = {
        dialogTitlte: '',
        operInfo: {},
        treeData: [],
        tabDefaultActiveKey: '1',
        defaultSelectedOrgKeys: []
    }
    componentWillMount() {
        this.props.dispatch(getOrgDataByID({id: '0'}));
        this.props.dispatch(getAreaList());
    }
    componentWillReceiveProps(newProps) {
        let {treeSource, result} = newProps;
        console.log("treeSource:", treeSource);
        this.setState({treeData: treeSource});

        // if (result) {
        //     if (result.msg != '') {
        //         result.isOk == true ? message.success(result.msg) : message.error(result.msg);
        //     }
        // }
    }
    handleAddClick = (event) => {
        console.log('handleAddClick');
        this.props.dispatch(orgNodeAdd());
    }
    handleEditClick = (e) => {
        this.props.dispatch(orgNodeEdit());
    }
    handleDeleteClick = (e) => {
        console.log("handledeleteClick", this.props.activeTreeNode.id);
        this.props.dispatch(orgNodeDelete(this.props.activeTreeNode.id));
    }

    handleOrgSelected = (e) => {
        console.log('onSelect', e);
        if (e.length > 0) {
            //let searchCondition = { organizationId: '' };
            //this.props.dispatch(empListGet(e[0]));
            SearchCondition.empListCondition.pageIndex = 0;
            SearchCondition.empListCondition.OrganizationIds = e;
            SearchCondition.empListCondition.keyWords = '';
            this.props.dispatch(setLoadingVisible(true));
            this.props.dispatch(empListGet(SearchCondition.empListCondition));
        }
        this.props.dispatch(orgNodeSelected(e));
    }
    onLoadData = (treeNode) => {
        let {eventKey} = treeNode.props;
        console.log("id:", eventKey);
        this.props.dispatch(getOrgDataByID({id: eventKey}));
        return new Promise((resolve) => {
            setTimeout(() => {
                resolve();
            }, 500);
        });
    }

    handleTabChange = (tabKey) => {
        console.log("tab切换：", tabKey);
        if (tabKey == 'emp') {

        }
    }

    render() {
        let removeBtnDisabled = (this.props.activeTreeNode.id == undefined ? true : false);
        const loop = data => data.map((item) => {
            if (item.children) {
                return <TreeNode title={item.name} key={item.key} dataRef={item}>{loop(item.children)}</TreeNode>;
            }
            return <TreeNode title={item.name} key={item.key} dataRef={item} isLeaf={item.isLeaf} disabled={item.key === '0-0-0'} ></TreeNode>;
        });
        const treeNodes = loop(this.state.treeData);
        return (
            <div className="inner-page" >
                <div className="left-panel">
                    <div className="relative">
                        <Layout>
                            <Header>
                                组织结构
                                    &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                                <Popconfirm title="确认要删除选中部门?" onConfirm={this.handleDeleteClick} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                                    &nbsp;<Button type='primary' shape='circle' icon='delete' disabled={removeBtnDisabled} />
                                </Popconfirm>
                                &nbsp;<Button type='primary' shape='circle' icon='edit' disabled={removeBtnDisabled} onClick={this.handleEditClick} />
                            </Header>
                            <Content style={{overflowY: 'auto'}}>
                                {/**
                                 * 结构树checkable={true} autoExpandParent={false} defaultSelectedKeys 
                                 */}
                                <Tree checkable={false} checkStrictly={true} onSelect={this.handleOrgSelected}
                                    loadData={this.onLoadData} >
                                    {treeNodes}
                                </Tree>
                                <OrgEditor />
                            </Content>
                        </Layout>
                    </div>
                </div>
                <div className="right-panel">
                    <div className="relative">
                        <Layout>
                            <Content style={{overflowY: 'auto', height: '100%'}}>
                                <Spin spinning={this.props.showLoading}>
                                    <Tabs defaultActiveKey="1" defaultActiveKey={this.state.tabDefaultActiveKey} onChange={this.handleTabChange}>
                                        <TabPane tab="员工信息" key="emp">
                                            <EmpTable />
                                        </TabPane>
                                        <TabPane tab="部门信息" key="org">
                                            <OrgDetail />
                                        </TabPane>
                                    </Tabs>
                                </Spin>
                            </Content>
                        </Layout>
                    </div>
                </div>
            </div>

        );
    }
}

function orgtableMapStateToProps(state) {
    console.log('orgtableMapStateToProps:', state.org);
    return {
        activeTreeNode: state.org.activeTreeNode,
        treeSource: state.org.treeSource,
        operInfo: state.org.operInfo,
        showLoading: state.emp.showLoading
    };
}

function orgtableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(orgtableMapStateToProps, orgtableMapDispatchToProps)(OrgTable);