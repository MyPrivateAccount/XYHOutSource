import { connect } from 'react-redux';
import { deleteOrgbyId ,upaddOrg, deleteMemOrgbyId, addOrg, updateOrg, getDicParList} from '../../actions/actionCreator';
import React, { Component } from 'react';
import {Table, Input, Form, Select, Button, Row, Col, Tree, Modal} from 'antd';
import { NewGuid } from '../../../utils/appUtils';
import './org.less';

const TreeNode = Tree.TreeNode;
const Option = Select.Option;
const styles = {
    conditionRow: {
        width: '80px',
        display: 'inline-block',
        fontWeight: 'bold',
    },
    bSpan: {
        fontWeight: 'bold',
    },
    otherbtn: {
        padding: '0px, 5px',
    }
}

class Station extends Component {
    constructor(pros) {
        super(pros);

        this.state = {
            department: "",
            expandedKeys: [],
            autoExpandParent: true,
            checkedKeys: [],
            selectedKeys: [],
            tempText: "",
            tempModalItem: {},
            showModal: false,
            confirmLoading: false,
        };
    }

    componentWillMount() {
        this.props.dispatch(getDicParList(["ORGNAZATION_TYPE"]));
    }

    onExpand = (expandedKeys) => {
        this.setState({
            expandedKeys,
            autoExpandParent: false,
          });
    }

    onCheck = (checkedKeys) => {
        this.setState({ checkedKeys });
    }

    onSelect = (selectedKeys, info) => {
        this.setState({ selectedKeys });
    }

    // save(item, e) {
    //     item.name = this.state.tempText;
    //     item.label = this.state.tempText;
    //     item.organizationName = this.state.tempText;
    //     if (item.isnew) {
    //         item.Original = {id:item.key,organizationName:item.name,type:"Group",sort:0,parentId:item.parentId};
    //         this.props.dispatch(addOrg(item));
    //     } else {
    //         item.Original.organizationName = this.state.tempText;
    //         this.props.dispatch(updateOrg(item));
    //     }

    //     item.isnew = false;
    //     e.stopPropagation();
    //     this.setState({selectedKeys: [item.key], tempText: ""});
    // }

    // cancle(item, e) {
    //     if (item.isnew) {
    //         this.props.dispatch(deleteMemOrgbyId(item.key));    
    //     }
    //     e.stopPropagation();
    //     this.setState({selectedKeys: [item.key], tempText: ""});
    // }

    handleOk = () => {
        this.state.tempModalItem.label = this.state.tempModalItem.name;
        this.state.tempModalItem.organizationName = this.state.tempModalItem.name;
        this.state.tempModalItem.isnew = false;
        if (this.state.tempModalItem.isnew) {
            this.state.tempModalItem.Original.organizationName = this.state.tempModalItem.name;
            this.props.dispatch(addOrg(this.state.tempModalItem));
        } else {
            this.state.tempModalItem.Original.organizationName = this.state.tempModalItem.name;
            this.props.dispatch(updateOrg(this.state.tempModalItem));
        }

        this.setState({
            confirmLoading: false,
            showModal: false,
        });
    }

    handleCancel = () => {
        this.setState({
            showModal: false,
            tempModalItem: null,
        });
    }

    edit(item, e) {
        e.stopPropagation();
        this.setState({selectedKeys: [item.key], tempText: "", tempModalItem: {...item, Original:{...item.Original}}, showModal: true,});
    }

    delete(item, e) {
        this.props.dispatch(deleteOrgbyId(item.key));
        e.stopPropagation();
        this.setState({selectedKeys: [item.key], tempText: ""});
    }

    addsub(item, e) {
        let guid = NewGuid();
        this.props.dispatch(upaddOrg({key: guid, value: guid, children:[], name:"", label:"", id:guid, organizationName:"", parentId:item.key, isnew:true}));
        e.stopPropagation();
        this.setState({selectedKeys: [guid], expandedKeys:[item.key, ...this.state.expandedKeys], tempText: ""});
    }

    onchange(item, e) {
        this.state.tempText = e.target.value;
    }

    renderTreeNodes = (data) => {
        let self = this;
        return data.map((item) => {
            if (item.children) {
                const nodetitle = (
                <div>
                    <a>{item.name}&nbsp;&nbsp;</a>
                    {
                        (item.key === self.state.selectedKeys[0])?
                        <span>
                            <a onClick={(e) =>self.edit(item, e)}>编辑 </a> 
                            <a onClick={(e) =>self.delete(item, e)}> 删除</a> 
                            <a onClick={(e) =>self.addsub(item, e)}> 新増</a> 
                        </span>
                        :null
                    }
                </div>);
                return (
                    <TreeNode title={nodetitle} key={item.key} dataRef={item}>
                        {this.renderTreeNodes(item.children)}
                    </TreeNode>
                );
            }
            return <TreeNode {...item} />;
        });
    }

    render() {
        let self = this;
        return (
            <div className="orgBlock">
                <Row>
                    <Col >
                        <div>组织架构:</div>
                    </Col>
                </Row>
                <Modal title="编辑"
                        visible={this.state.showModal}
                        onOk={this.handleOk}
                        confirmLoading={this.state.confirmLoading}
                        onCancel={this.handleCancel}>
                        <Row style={{ margin: '4px' }}>
                            <Col span={3}>组织名：</Col>
                            <Col span={12}><Input onChange={(v)=>{this.state.tempModalItem.name = v.target.value;this.forceUpdate();}} value={this.state.tempModalItem.name}></Input></Col>
                        </Row>
                        <Row style={{ margin: '4px' }}>
                            <Col span={3} >组织类型：</Col>
                            <Col span={12}> 
                                <Select onChange={(v)=>{this.state.tempModalItem.Original.type = v;this.forceUpdate();}} 
                                value={this.state.tempModalItem.Original?this.state.tempModalItem.Original.type:null} style={{width: '100%'}}>
                                    {
                                        (self.props.orgnazitionType && self.props.orgnazitionType.length > 0) ?
                                        self.props.orgnazitionType.map(
                                            function (params) {
                                                return <Option key={params.key} value={params.value}>{params.key}</Option>;
                                            }
                                        ):null
                                    }
                                </Select>
                            </Col>
                        </Row>
                        <Row style={{ margin: '4px' }}>
                            <Col span={3}>主负责人：</Col>
                            <Col span={12}><Input onChange={(v)=>{this.state.tempModalItem.Original.leaderManager = v.target.value;this.forceUpdate();}} 
                            value={this.state.tempModalItem.Original?this.state.tempModalItem.Original.leaderManager:""}></Input></Col>
                        </Row>
                        <Row style={{ margin: '4px' }}>
                            <Col span={3}>负责人：</Col>
                            <Col span={12}><Input onChange={(v)=>{this.state.tempModalItem.Original.manager = v.target.value;this.forceUpdate();}} 
                             value={this.state.tempModalItem.Original?this.state.tempModalItem.Original.manager:""}></Input></Col>
                        </Row>
                </Modal>
                {/* <Table className="contentOrg" rowSelection={rowSelection} rowKey={record => record.key} dataSource={this.props.setDepartmentOrgTree} columns={this.ListColums} /> */}
                <Tree 
                    onExpand={this.onExpand}
                    expandedKeys={this.state.expandedKeys}
                    autoExpandParent={this.state.autoExpandParent}
                    onCheck={this.onCheck}
                    checkedKeys={this.state.checkedKeys}
                    onSelect={this.onSelect}
                    selectedKeys={this.state.selectedKeys}
                    >
                    {this.renderTreeNodes(this.props.setDepartmentOrgTree)}
                </Tree>
            </div>
        );
    }
}

function tableMapStateToProps(state) {
    return {
        orgnazitionType: state.basicData.orgnazitionType,
        departmentTypeLst: state.basicData.departmentTypeLst,
        setDepartmentOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Station);