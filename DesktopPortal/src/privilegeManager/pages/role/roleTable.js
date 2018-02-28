import React, {Component} from 'react';
import {connect} from 'react-redux';
import {Layout, Table, Button, Checkbox, Tree, Tabs, Row, Col, Menu, Icon, Select, Popconfirm, Tooltip, message} from 'antd';
import {
    roleAdd, roleEdit, roleDelete, roleDialogClose,
    rolePrivilegeGet, rolePrivilegeEdit, rolePrivilegeSave, roleAppSave, orgGetPermissionTree,
    roleGetList, roleSlected, getOrgDataByID, privilegeGetList, roleGetAllToolPrivilegeItem
} from '../../actions/actionCreator';
import RoleEditor from './roleEdit';
import {ApplicationTypes} from '../../../constants/baseConfig'

const {Header, Sider, Content} = Layout;
const SubMenu = Menu.SubMenu;
const MenuItemGroup = Menu.ItemGroup;
const TreeNode = Tree.TreeNode;
const Option = Select.Option;


const orgPrivilegeTypes = [{organizationId: '00000000', organizationName: '本组织'},
{organizationId: '11111111', organizationName: '分公司'},
{organizationId: '88888888', organizationName: '全部'}];

class RoleTable extends Component {
    state = {
        privilegeOrgTreeShow: true,//权限树禁用
        privilegeOrgTreeDisabled: true,//权限树禁用
        activePrivilegeItem: '',//当前选中权限
        filialeChecked: false,//分公司checkbox状态
        curOrgChecked: false,//本组织checkbox状态
        allChecked: false,//全选
    }
    componentWillMount() {
        this.props.dispatch(roleGetList());
        this.props.dispatch(roleGetAllToolPrivilegeItem(this.props.appList));
        //setTimeout(() => { }, 100);
        this.props.dispatch(orgGetPermissionTree("RoleCreate"));
        this.props.dispatch(orgGetPermissionTree("PublicRoleOper"));
        this.props.dispatch(orgGetPermissionTree("AuthorizationPermission"));
    }
    componentWillReceiveProps(newProps) {

    }

    handleRoleSlected = (e) => {//角色选择
        console.log("selected角色：", e.selectedKeys, e);
        this.props.dispatch(roleSlected(e.selectedKeys));
        this.props.dispatch(rolePrivilegeGet(e.selectedKeys[0]));
    }

    handleRoleAdd = (e) => {
        console.log("角色新增：");
        this.props.dispatch(roleAdd());
    }
    handleRoleEdit(role) {
        console.log("角色编辑：" + JSON.stringify(role));
        this.props.dispatch(roleEdit(role));
    }

    handlePrivilegeSave = (e) => {
        console.log("权限保存", this.props.checkPrivileges);
        let rolePrivilege = [];//工具-》权限项-》部门
        let {appList} = this.props;
        let {checkedPrivilegeItemNodes} = this.props.checkPrivileges;
        let roleApps = [];
        checkedPrivilegeItemNodes.map((node) => {
            let filterResult = rolePrivilege.filter((p) => p.applicationId == node.applicationId);
            if (filterResult.length == 0) {
                let app = appList.find((a) => a.id == node.applicationId);
                let appPrivilegeItem = {applicationId: app.id, applicationName: app.displayName, permissions: []};
                appPrivilegeItem.permissions.push(node);
                rolePrivilege.push(appPrivilegeItem);
                roleApps.push(app.id);
            } else {
                filterResult[0].permissions.push(node);
            }
        });
        console.log("权限保存JSON：", rolePrivilege, roleApps);
        this.props.dispatch(rolePrivilegeSave({roleId: this.props.activeRole.id, rolePrivilege: rolePrivilege}));
        this.props.dispatch(roleAppSave({roleId: this.props.activeRole.id, entity: roleApps}));
    }
    handleDeleteClick = (role) => {
        console.log("delete role:", role);
        this.props.dispatch(roleDelete(role.id));
    }

    handlePrivilegeItemSelected = (e) => {//权限选择处理（勾选上的权限项，才可以设置组织权限）
        console.log("权限项选中:", e.key);
        this.setState({activePrivilegeItem: e.key});
        let filialeChecked = false, curOrgChecked = false, allChecked = false;
        for (let i in this.props.checkPrivileges.checkedPrivilegeItemNodes) {
            let privilegeItem = this.props.checkPrivileges.checkedPrivilegeItemNodes[i];
            if (privilegeItem.permissionId == e.key) {
                filialeChecked = (privilegeItem.organizations.filter((org) => org.organizationId == '11111111').length > 0);
                curOrgChecked = (privilegeItem.organizations.filter((org) => org.organizationId == '00000000').length > 0);
                allChecked = (privilegeItem.organizations.filter((org) => org.organizationId == '88888888').length > 0);
                break;
            }
        };
        this.setState({privilegeOrgTreeShow: !allChecked, allChecked: allChecked, curOrgChecked: curOrgChecked, filialeChecked: filialeChecked, privilegeOrgTreeDisabled: !this.props.checkPrivileges.checkkeys.includes(e.key)});
    }
    privilegeItemCheckChange = (e) => {//权限项勾选处理
        let {checked, original} = e.target;
        console.log("activePrivilegeItem:", this.state.activePrivilegeItem);
        console.log("checkChange:" + checked, original);
        let checkedKeys = this.props.checkPrivileges.checkkeys.slice();
        let nodeList = this.props.checkPrivileges.checkedPrivilegeItemNodes.slice();
        this.setState({privilegeOrgTreeDisabled: !checked});
        if (checked) {
            checkedKeys.push(original.id);
            nodeList.push({applicationId: original.applicationId, permissionId: original.id, permissionName: original.name, groupName: original.groups, organizations: []});
        } else {
            checkedKeys = checkedKeys.filter((c) => c != original.id);
            for (let i = nodeList.length - 1; i > -1; i--) {
                if (nodeList[i].permissionId == original.id) {
                    nodeList.splice(i, 1);
                    break;
                }
            }
        }
        this.props.dispatch(rolePrivilegeEdit({checkkeys: checkedKeys, checkedPrivilegeItemNodes: nodeList}));
    }
    handleOrgPrivilegeTypeChecked = (e) => {//权限类型勾选处理
        console.log("e", e);
        let {checked, value} = e.target;
        console.log("选择：，value", checked, value);
        if (value == '88888888' && checked) {
            this.setState({privilegeOrgTreeShow: false, filialeChecked: false, curOrgChecked: false, allChecked: true});
        } else {
            this.setState({privilegeOrgTreeShow: true, allChecked: false});
            if (value == '00000000') {
                this.setState({curOrgChecked: checked});
            }
            if (value == '11111111') {
                this.setState({filialeChecked: checked});
            }
        }
        let org = orgPrivilegeTypes.find((o) => o.organizationId == value);
        let checkNodes = this.props.checkPrivileges.checkedPrivilegeItemNodes.slice();
        for (let i in checkNodes) {
            if (checkNodes[i].permissionId == this.state.activePrivilegeItem) {
                if (value == '88888888') {//全部权限
                    checkNodes[i].organizations = checked ? [org] : [];
                } else {
                    if (checked) {
                        checkNodes[i].organizations.push(org);
                    } else {
                        for (let orgIndex = checkNodes[i].organizations.length - 1; orgIndex > -1; orgIndex--) {
                            if (checkNodes[i].organizations[orgIndex].organizationId == value) {
                                checkNodes[i].organizations[orgIndex].splice(orgIndex, 1);
                                break;
                            }
                        }
                    }
                }
                break;
            }
        }
        console.log("checkedPrivilegeItemNodes", checkNodes);
        this.props.dispatch(rolePrivilegeEdit({checkkeys: this.props.checkPrivileges.checkkeys.slice(), checkedPrivilegeItemNodes: checkNodes}));
    }

    //选择权限项的组织结构
    handleOrgOnchecked = (checkedKeys, e) => {
        let {title, eventKey} = e.node.props;
        let organizations = [];
        e.checkedNodes.map((node) => {//直接遍历所有选中的节点
            organizations.push({organizationId: node.key, organizationName: node.props.title});
        });
        let checkedPrivilegeItemNodes = this.props.checkPrivileges.checkedPrivilegeItemNodes.slice();
        for (let i in checkedPrivilegeItemNodes) {
            if (checkedPrivilegeItemNodes[i].permissionId == this.state.activePrivilegeItem) {
                console.log("找到选中的当前权限项，设置部门属性为：", organizations);
                checkedPrivilegeItemNodes[i].organizations = organizations;
                break;
            }
        }
        console.log("org tree checkedkeys", checkedKeys.checked, checkedPrivilegeItemNodes);
        let newCheckPrivileges = Object.assign({}, this.props.checkPrivileges, {checkedPrivilegeItemNodes: checkedPrivilegeItemNodes});
        this.props.dispatch(rolePrivilegeEdit(newCheckPrivileges));
    }


    // onOrgTreeLoadData = (e) => {
    //     let { eventKey } = e.props;
    //     this.props.dispatch(getOrgDataByID({ id: eventKey }));
    //     return new Promise((resolve) => {
    //         setTimeout(() => {
    //             resolve();
    //         }, 500);
    //     });
    // }

    getAppIcon(appInfo) {
        let appIcon = "appstore";
        let appType = appInfo.applicationType;
        let app = ApplicationTypes.find(app => app.key === appType);
        if (app) {
            appIcon = app.icon;
        }
        return appIcon;
    }
    render() {
        let {checkedPrivilegeItemNodes} = this.props.checkPrivileges;
        let checkedOrgkeys = [];
        checkedPrivilegeItemNodes.map((node) => {
            if (node.permissionId == this.state.activePrivilegeItem) {
                node.organizations.map((org) => {
                    checkedOrgkeys.push(org.organizationId);
                });
            }
        });
        //角色默认选中
        let checkRolekeys = [];
        if (this.props.activeRole.id) {
            checkRolekeys.push(this.props.activeRole.id);
        }

        const loop = data => data.map((item) => {
            if (item.children && item.children.length) {
                return <TreeNode key={item.key} title={item.name} original={item.original}>{loop(item.children)}</TreeNode>;
            }
            return <TreeNode key={item.key} title={item.name} original={item.original} isLeaf={item.isLeaf} />;
        });

        return (
            <div className="inner-page">
                <div className="left-panel">
                    <div className="relative">
                        <Layout>
                            <Header>
                                角色列表
                                <Tooltip title="新增角色">
                                    &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleRoleAdd} />
                                </Tooltip>
                            </Header>
                            <Content style={{overflowY: 'auto', padding: '0'}}>
                                <Menu theme={this.state.theme} defaultSelectedKeys={checkRolekeys}
                                    onSelect={this.handleRoleSlected}
                                    mode="inline" >
                                    {
                                        this.props.roleSource.map((role, i) => {
                                            return (
                                                <Menu.Item key={role.id} selectedKeys={[this.props.activeRole.id]}>
                                                    {
                                                        (this.props.activeRole.id == role.id) ?
                                                            <span className="right-tool-bar">
                                                                <Tooltip title="编辑角色">
                                                                    <Button type="primary" className="ml-sm" shape="circle" icon="edit" size="small" onClick={(e) => this.handleRoleEdit(role)} />
                                                                </Tooltip>
                                                                <Tooltip title="删除角色">
                                                                    <Popconfirm title="你确定要删除此角色么?" onConfirm={(e) => this.handleDeleteClick(role)} onCancel={null} okText="是" cancelText="否">
                                                                        <Button type="primary" className="ml-sm" shape="circle" icon="delete" size="small" />
                                                                    </Popconfirm>
                                                                </Tooltip>
                                                            </span> : null
                                                    }
                                                    <span>{role.name}</span>
                                                </Menu.Item>
                                            )
                                        })
                                    }
                                </Menu>
                            </Content>
                        </Layout>
                    </div>
                </div>
                <div className="right-panel">
                    <div className="relative">
                        <Layout>
                            <Content style={{overflowX: 'hidden'}}>
                                <Row gutter={10} style={{height: '100%'}}>
                                    <Col span={12} style={{height: '100%'}}>
                                        <Layout style={{height: '100%'}}>
                                            <Header>工具权限
                                            <Tooltip title="保存工具权限">
                                                    &nbsp;<Button type='primary' shape='circle' icon='save' onClick={this.handlePrivilegeSave} disabled={this.props.activeRole.id == undefined} />
                                                </Tooltip>
                                            </Header>
                                            <Content style={{height: '100%', overflowY: 'auto'}}>
                                                <Menu theme={this.state.theme} onSelect={(e) => this.handlePrivilegeItemSelected(e)} mode="inline" >
                                                    {this.props.privilegeTreeSource.map((appInfo, i) =>
                                                        <SubMenu key={i} title={<span><Icon type={this.getAppIcon(appInfo.original)} /><span>{appInfo.name}</span></span>}>
                                                            {
                                                                appInfo.children.map((group) =>
                                                                    <MenuItemGroup key={group.key} title={group.name}>
                                                                        {
                                                                            group.children.map((item) =>
                                                                                <Menu.Item key={item.key} >
                                                                                    <Checkbox disabled={this.props.activeRole.id == undefined} key={item.key}
                                                                                        checked={this.props.checkPrivileges.checkkeys.find((c) => c == item.key) != undefined}
                                                                                        original={item.original} onChange={(e) => this.privilegeItemCheckChange(e)} /> {item.name}
                                                                                </Menu.Item>
                                                                            )
                                                                        }
                                                                    </MenuItemGroup>
                                                                )
                                                            }
                                                        </SubMenu>
                                                    )}
                                                </Menu>

                                            </Content>
                                        </Layout></Col>
                                    <Col span={12} style={{height: '100%'}}>
                                        <Layout style={{height: '100%'}}>
                                            <Header>组织权限</Header>
                                            <Content style={{height: '100%', overflowY: 'auto'}}>
                                                <Checkbox disabled={this.state.privilegeOrgTreeDisabled} value='88888888' checked={this.state.allChecked} onChange={(e) => this.handleOrgPrivilegeTypeChecked(e)}>全部</Checkbox>
                                                {
                                                    this.state.privilegeOrgTreeShow ? <Checkbox disabled={this.state.privilegeOrgTreeDisabled} checked={this.state.curOrgChecked} value='00000000' onChange={(e) => this.handleOrgPrivilegeTypeChecked(e)}>本组织</Checkbox> : null
                                                }
                                                {
                                                    this.state.privilegeOrgTreeShow ? <Checkbox disabled={this.state.privilegeOrgTreeDisabled} checked={this.state.filialeChecked} value='11111111' onChange={(e) => this.handleOrgPrivilegeTypeChecked(e)}>分公司</Checkbox> : null
                                                }
                                                {
                                                    this.state.privilegeOrgTreeShow ? <Tree checkable={!this.state.privilegeOrgTreeDisabled} defaultCheckedKeys={checkedOrgkeys} checkStrictly={true} onCheck={(checkedKeys, e) => this.handleOrgOnchecked(checkedKeys, e)}
                                                        loadData={this.onOrgTreeLoadData} >
                                                        {loop(this.props.permissionOrgTree.AddRolePermissionTree)}
                                                    </Tree> : null
                                                }

                                            </Content>
                                        </Layout></Col>
                                </Row>
                                <RoleEditor />
                            </Content>
                        </Layout>
                    </div>
                </div>
            </div>
        )
    }
}

function roletableMapStateToProps(state) {
    console.log("roletableMapStateToProps:", state.role);
    return {
        activeRole: state.role.activeRole,
        operInfo: state.role.operInfo,
        roleSource: state.role.roleSource,
        appList: state.app.appList,
        privilegeTreeSource: state.role.privilegeTreeSource,
        treeSource: state.org.treeSource,
        checkPrivileges: state.role.checkPrivileges,
        permissionOrgTree: state.org.permissionOrgTree
    }
}

function roletableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(roletableMapStateToProps, roletableMapDispatchToProps)(RoleTable);