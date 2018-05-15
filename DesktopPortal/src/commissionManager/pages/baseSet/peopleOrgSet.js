//人数分摊组织设置页面
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import {orgFtParamAdd, orgFtParamUpdate, orgFtParamSave, orgFtDialogClose,orgGetPermissionTree} from '../../actions/actionCreator'
import PeopleOrgFtEditor from './peopleOrgFtEditor'

const { Header, Content } = Layout;
const Option = Select.Option;

class PeopleSet extends Component{
    state = {

    }
    appTableColumns = [
        { title: '组织', dataIndex: 'orgName', key: 'orgName' },
        { title: '分摊比例', dataIndex: 'ftScale', key: 'ftScale' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title='编辑'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleEditClick(recored)} />
                    </Tooltip>
                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleDelClick = (info) =>{

    }
    handleEditClick = (info) =>{

    }
    handleNew = (info)=>{
        console.log(info);
        this.props.dispatch(orgFtParamAdd());
    }
    componentDidMount = ()=>{
        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
    }
    componentWillReceiveProps = (newProps)=>{

    }
    render(){
        return (
            <Layout>
                <div style={{'margin':5}}>
                    组织：
                    <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.AddUserTree}
                                    placeholder="所属组织"
                                    defaultValue={this.props.orgid}>
                    </TreeSelect>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':10}}/>
                </Tooltip>
                <Table columns={this.appTableColumns}></Table>
                <PeopleOrgFtEditor/>
            </Layout>
        )
    }
}
function peoMapStateToProps(state){
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree
    }
}

function peoMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(peoMapStateToProps, peoMapDispatchToProps)(PeopleSet);