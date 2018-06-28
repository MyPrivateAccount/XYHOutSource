//人数分摊组织编辑界面
import {connect} from 'react-redux';
import {orgFtParamAdd, orgFtParamUpdate, orgFtParamSave, orgFtDialogClose,orgGetPermissionTree} from '../../actions/actionCreator'
import React, {Component} from 'react'
import {globalAction} from 'redux-subspace';
import {Select, Button, Checkbox, Modal, Row, Col, Form, Input, InputNumber,message,TreeSelect} from 'antd'
import NumericInput from './numberInput'

const FormItem = Form.Item;
const TreeNode = TreeSelect.TreeNode;

class PeopleOrgFtEditor extends Component{
    state = {
        dialogTitle:'',
        visible:false,
        ppftInfo:{},
        branchId:'',
        isEdit:false
    }
    componentWillMount(){
        
    }
    componentWillReceiveProps(newProps){
        let {operType, result} = newProps.operInfo;
        let {activeTreeNode, treeSource} = newProps;
        //this.setState({ orgInfo: activeTreeNode });

        if (operType == 'edit') {
            this.setState({visible: true, dialogTitle: '修改人数分摊组织',ppftInfo:newProps.activePft,isEdit:true});
            newProps.operInfo.operType = ''
        } else if (operType == 'add') {
            this.setState({visible: true, dialogTitle: '添加人数分摊组织',branchId:newProps.activePft,isEdit:false});
            this.props.dispatch(orgGetPermissionTree("YJ_SZ_KXFTZZ"));
            newProps.operInfo.operType = ''
        }
        else if(operType == 'ORG_FT_DIALOG_CLOSE'){
            newProps.operInfo.operType = ''
            this.setState({visible:false})
        }
    }
    handleOk = (e) => {
        e.preventDefault();
        let {operType} = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                //调用保存接口，进行数据保存,待续
                values.branchId = this.state.branchId
                if(this.state.isEdit){
                    values.branchId = this.state.ppftInfo.branchId
                    values.shareId = this.state.ppftInfo.shareId
                    values.shareName = this.state.ppftInfo.shareName
                }
                this.props.dispatch(orgFtParamSave(values))
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(orgFtDialogClose());
    };
    getPercent = (e) => {
        let pp = e
        pp = pp.substr(0, pp.length - 1)
        pp = parseFloat(pp) / 100
        return pp
    }
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
                            {getFieldDecorator('shareId', {

                                initialValue: !this.state.isEdit?this.state.ppftInfo.shareId:this.state.ppftInfo.shareName,
                                rules: [{ required: true, message: '请选择所属组织!' }]
                            })(
                                this.state.isEdit?<Input style={{ width: 300 }} value={this.state.ppftInfo.shareName} disabled={true}/>:
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.RSFTZZOrgTree}
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
                            {getFieldDecorator('shareRatio', {
                                initialValue: this.state.isEdit?this.getPercent(this.state.ppftInfo.shareRatio):this.state.ppftInfo.shareRatio,
                                rules: [{required: true, message: '请填写分摊比例!' }]
                            })(
                                <NumericInput  style={{float: 'left',width:300}}/>
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
        operInfo:state.ppft.operInfo,
        activePft:state.ppft.activePft
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(PeopleOrgFtEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);