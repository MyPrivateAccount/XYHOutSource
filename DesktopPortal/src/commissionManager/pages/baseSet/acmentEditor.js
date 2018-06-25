//业绩分摊比例弹出对话框
import { connect } from 'react-redux';
import { acmentParamSave, acmentParamDlgClose, acmentParamItemAdd } from '../../actions/actionCreator'
import React, { Component } from 'react'
import { Button, Modal, Row, Col, Form, Input, Select, Checkbox, Tooltip,InputNumber} from 'antd'
import AcmentItemEditor from './acmentItemEditor'

const FormItem = Form.Item;
const Option = Select.Option;

class AcmentEditor extends Component {
    state = {
        dialogTitle: '',
        visible: false,
        iedVisible: false,
        isEdit:false,
        paramInfo: { id:'',isfixed: true, branchId: '', code: '', type: 1,percent:'' }
    }
    componentWillMount() {

    }
    componentWillReceiveProps(newProps) {
        let { operType } = newProps.operInfo;
        if(operType === ""){
            return
        }
        if (operType === 'edit') {
            let temp = {...newProps.activeScale}
            temp.isfixed = temp.isfixed==='是'?true:false
            this.setState({ visible: true, dialogTitle: '修改',isEdit:true, paramInfo: temp });
            newProps.operInfo.operType = ""
        } else if (operType === 'add') {
            this.clear()
            this.setState({ visible: true, dialogTitle: '添加',isEdit:false });
            newProps.operInfo.operType = ""
        } else if(operType === 'ACMENT_PARAM_DLGCLOSE'){
            this.setState({ visible: false });
        }
    }
    clear=()=>{
        let paramInfo = {...this.state.paramInfo}
        paramInfo.id = ''
        paramInfo.isfixed = true
        paramInfo.code = ''
        paramInfo.name = ''
        paramInfo.type = '外部佣金'
        paramInfo.percent = ''
        this.setState({paramInfo})
    }
    handleOk = (e) => {

        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log(this.state.paramInfo)
                values.branchId = this.props.orgid;
                values.code = this.state.paramInfo.code;
                values.percent = parseFloat(values.percent)
                if(this.state.isEdit){
                    values.id = this.state.paramInfo.id
                }
                console.log(values);
                this.props.dispatch(acmentParamSave(values))
                this.props.dispatch(acmentParamDlgClose())
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.setState({ iedVisible: false })
        this.props.dispatch(acmentParamDlgClose());
    };
    handleNewItem = (e) => {
        this.setState({ iedVisible: true });
    }
    handleItemValue = (e) => {
        this.setState({ iedVisible: false });
        let paramInfo = {...this.state.paramInfo}
        paramInfo.name = e.itemName
        paramInfo.code = e.itemCode
        this.setState({ paramInfo});
        this.props.form.setFieldsValue({'name':paramInfo.name })
    }
    handleback = (e) => {
        this.setState({ iedVisible: false });
    }
    getPercent=(e)=>{
        if(!this.state.isEdit){
            return ''
        }
        let pp = e
        pp = pp.substr(0,pp.length-1)
        pp = parseFloat(pp)/100
        return pp
    }
    isFixedChange=(e)=>{
        let paramInfo = {...this.state.paramInfo}
        paramInfo.isfixed = e.target.checked
        this.setState({paramInfo},()=>{
            console.log(this.state.paramInfo)
        })
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 9 },
            wrapperCol: { span: 11 },
        };
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <div style={{ display: !this.state.iedVisible ? 'block' : 'none' }}>
                    <Row>
                        <Col span={12}>
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        分摊项
                                </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('name', {

                                    initialValue: this.state.paramInfo.name,
                                })(
                                    <Input style={{ float: 'left', width: 200 }} disabled={this.state.isEdit}></Input>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={12} push={2}>
                        {
                            !this.state.isEdit?(<Tooltip title="新增">
                            <Button type='primary' shape='circle' icon='plus' onClick={this.handleNewItem} />
                        </Tooltip>):null
                        }
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        分摊类型
                                </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('type', {
                                    initialValue: this.state.paramInfo.type==='外部佣金'?"1":"2",
                                })(
                                    <Select initialValue="1" style={{ width: 200 }} disabled={this.state.isEdit}>
                                        <Option value="1">外部佣金</Option>
                                        <Option value="2">内部分摊项</Option>
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            <FormItem
                                {...formItemLayout}
                                label={(<span>默认分摊比例</span>)}>
                                {getFieldDecorator('percent', {
                                    initialValue: this.getPercent(this.state.paramInfo.percent),
                                    rules: [{ required: true, message: '请填写默认分摊比例!' }]
                                })(
                                    <Input step="0.01" type="number"  max="1.00" style={{ float: 'left', width: 200 }}></Input>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12} push={5}>
                            <FormItem>
                                {getFieldDecorator('isfixed', {
                                    initialValue:this.state.paramInfo.isfixed
                                })(
                                    <Checkbox onChange={this.isFixedChange} checked={this.state.paramInfo.isfixed}>固定比例</Checkbox>
                                )}
                            </FormItem></Col>
                    </Row>
                </div>
                <AcmentItemEditor vs={this.state.iedVisible} updateItemValue={this.handleItemValue} back={this.handleback} />
            </Modal>
        )
    }
}

function MapStateToProps(state) {

    return {
        operInfo: state.acm.operInfo,
        activeScale: state.acm.activeScale
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(AcmentEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);