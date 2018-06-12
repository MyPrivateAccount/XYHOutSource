//人数分摊组织编辑界面
import {connect} from 'react-redux';
import {orgFtParamAdd, orgFtParamUpdate, orgFtParamSave, orgFtDialogClose} from '../../actions/actionCreator'
import React, {Component} from 'react'
import {globalAction} from 'redux-subspace';
import {Select, Button, Checkbox, Modal, Row, Col, Form, Input, InputNumber,message,TreeSelect} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;
const TreeNode = TreeSelect.TreeNode;

class PeopleOrgFtEditor extends Component{
    state = {
        dialogTitle:'',
        visible:false,
        ppftInfo:{}
    }
    componentWillMount(){

    }
    componentWillReceiveProps(newProps){
        let {operType, result} = newProps.operInfo;
        let {activeTreeNode, treeSource} = newProps;
        //this.setState({ orgInfo: activeTreeNode });

        if (operType == 'edit') {
            this.setState({visible: true, dialogTitle: '修改人数分摊组织',ppftInfo:newProps.activePft});
        } else if (operType == 'add') {
            this.setState({visible: true, dialogTitle: '添加人数分摊组织'});
        } else {
            this.props.form.resetFields();
            this.setState({visible: false});
        }
    }
    handleOk = (e) => {
        e.preventDefault();
        let {operType} = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                //调用保存接口，进行数据保存,待续
                this.props.dispatch(orgFtParamSave(values))
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(orgFtDialogClose());
    };
    render(){
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 14},
        };
        const loop = data => data.map((item) => {
            if (item.children) {
                return <TreeNode title={item.name} key={item.key} value={item.key}>{loop(item.children)}</TreeNode>;
            }
            return <TreeNode title={item.name} key={item.key} value={item.key} isLeaf={item.isLeaf} />;
        });
        const treeNodes = loop(this.props.treeSource);
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    组织
                                </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('branchId', {

                                initialValue: this.state.ppftInfo.organizationId,
                                rules: [{ required: true, message: '请选择所属组织!' }]
                            })(
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.AddUserTree}
                                    placeholder="请选择所属组织">
                                    {treeNodes}
                                </TreeSelect>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>分摊比例</span>)}>
                            {getFieldDecorator('ftbl', {
                                initialValue: this.state.ppftInfo.proportions,
                                rules: [{required: true, message: '请填写分摊比例!' }]
                            })(
                                <Input style={{float: 'left',width:300}}/>
                                )}
                        </FormItem></Col>
                </Row>
            </Modal>
        )
    }
}

function MapStateToProps(state) {

    return {
        treeSource: state.org.treeSource,
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        operInfo:state.ppft.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(PeopleOrgFtEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);