import { connect } from 'react-redux';
import { deleteOrgbyId ,upaddOrg, deleteMemOrgbyId, addOrg, updateOrg} from '../../actions/actionCreator';
import React, { Component } from 'react';
import {Table, Input, Form, Select, Button, Row, Col, Tree} from 'antd';
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
        };
    }

    componentWillMount() {
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

    edit(item, e) {
        item.editable = true;
        e.stopPropagation();
       this.setState({selectedKeys: [item.key], tempText: ""});
    }

    save(item, e) {
        item.editable = false;

        item.name = this.state.tempText;
        item.label = this.state.tempText;
        item.organizationName = this.state.tempText;
        if (item.isnew) {
            item.Original = {id:item.key,organizationName:item.name,type:"Group",sort:0,parentId:item.parentId};
            this.props.dispatch(addOrg(item));
        } else {
            item.Original.organizationName = this.state.tempText;
            this.props.dispatch(updateOrg(item));
        }

        item.isnew = false;
        e.stopPropagation();
        this.setState({selectedKeys: [item.key], tempText: ""});
    }

    cancle(item, e) {
        if (item.isnew) {
            this.props.dispatch(deleteMemOrgbyId(item.key));    
        }
        item.editable = false;
        e.stopPropagation();
        this.setState({selectedKeys: [item.key], tempText: ""});
    }

    delete(item, e) {
        this.props.dispatch(deleteOrgbyId(item.key));
        e.stopPropagation();
        this.setState({selectedKeys: [item.key], tempText: ""});
    }

    addsub(item, e) {
        let guid = NewGuid();
        this.props.dispatch(upaddOrg({key: guid, value: guid, children:[], name:"", label:"", id:guid, organizationName:"", parentId:item.key,editable:true,isnew:true}));
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
                    {item.editable
                        ?
                        <Input style={{ maxHeight: '18px' ,maxWidth:'56px'}} defaultValue={item.name} onChange={(e) => self.onchange(item, e)} />
                        :<a>{item.name}&nbsp;&nbsp;</a>}
                    {
                        (item.key === self.state.selectedKeys[0]||item.editable)?
                        item.editable?
                        <span>
                            <a onClick={(e) =>self.save(item, e)}>保存 </a>
                            <a onClick={(e) =>self.cancle(item, e)}> 取消</a>
                        </span>
                        : 
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
        return (
            <div className="orgBlock">
                <Row>
                    <Col >
                        <div>组织架构:</div>
                    </Col>
                </Row>
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
        setDepartmentOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Station);