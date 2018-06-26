import React, { Component } from 'react';
import { Alert, Select, Upload, Modal, Icon, Table, Form, Checkbox, Input, TreeSelect, Row, Col, Button, notification, Spin, InputNumber } from 'antd'
import { connect } from 'react-redux';
import FixedTable from '../../../components/FixedTable'
import { AuthorUrl, basicDataServiceUrl, UploadUrl } from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import { getDicPars, getOrganizationTree } from '../../../utils/utils'
import { getDicParList } from '../../../actions/actionCreators'
import validations from '../../../utils/validations'
import Layer from '../../../components/Layer'
import { repaymentStatus, permission, borrowingStatus, recordingStatus, chargeStatus } from './const'
import moment from 'moment'
import uuid from 'uuid';
import ConfirmDialog from './ConfirmDialog'
import PaymentDialog from './PaymentDialog'

const FormItem = Form.Item;
const Option = Select.Option;
const confirm = Modal.confirm;

const formFields = ['chargeNo', 'reimburseDepartment', 'reimburseUser', 'memo', 'chargeAmount', 'reimbursedAmount']


const styles = {
    statusText: {
        position: 'absolute',
        right: '1rem',
        top: '1rem',
        fontSize: '1.2rem'
    },
    topBtnBar: {
        position: 'absolute',
        top: '0.5rem',
        left: '0.5rem',
        zIndex: 100
    },
    topBtnBarBackground: {
        backgroundColor: '#80808080',
        padding: '0.5rem',
        paddingLeft: '1rem',
        paddingRight: '1rem',
        borderRadius: '0.5rem'
    },
    limitTextStyle: {
        marginLeft: '1rem'
    }
}

class Repayment extends Component {
    state = {
        nodes: [],
        feeList: [],
        billList: [],
        userList: [],
        fetching: false,
        entity: {},
        op: '',
        saveing: false,
        loading: false,
        canEditBase: false,
        dlgConfirm: false,
        confirmLoading: false,
        dlgPayment: false,
        paymentLoading: false,
        permission: {},
        pagePar: {},
        reimburseInfo: null
    }

    componentDidMount = () => {
        let initState = (this.props.location || {}).state || {};
        if (initState.op === 'add') {
            this.setState({ entity: initState.entity, op: initState.op, canEditBase: true, canEditBill: true, canEditFee: true })
            this.props.form.setFieldsValue({
                reimburseDepartment: initState.entity.reimburseDepartment,
                reimburseUser: [{ key: initState.entity.reimburseUser, label: initState.entity.reimburseUserName }]
            })
            this.fetchUser(initState.entity.reimburseUser)
            if(initState.entity.chargeId){
                this.getReimburseInfo(initState.entity.chargeId);
            }
        } else if (initState.entity) {
            let canEditBase = false;
            if (initState.op === 'edit') {
                canEditBase =  true;
            } 
            this.setState({ canEditBase})
            this.setState({ op: initState.op })
            this.getRepaymentInfo(initState.entity.id);
        }
        this.setState({ pagePar: initState.pagePar ||{}})
        this.getNodes();
        this.getPermission();
        
    }


    getPermission = async () => {
        let url = `${AuthorUrl}/api/Permission/each`
        let r = await ApiClient.post(url, [permission.yjk, permission.yjkgl,  permission.yjkqr, permission.yjkfk])
        if (r && r.data && r.data.code === '0') {
            let p = {};
            (r.data.extension || []).forEach(pi => {
                if (pi.permissionItem === permission.yjkqr) {
                    p.qr = pi.isHave;
                } else if (pi.permissionItem === permission.yjkfk) {
                    p.fk = pi.isHave;
                } else if (pi.permissionItem === permission.yjkgl) {
                    p.gl = pi.isHave;
                }else if (pi.permissionItem === permission.yjk) {
                    p.yjk = pi.isHave;
                }
            })
            console.log(p);
            this.setState({ permission: p })
        }
    }

    getRepaymentInfo = async (id) => {
        if (!id) {
            notification.error({ message: '还款单据id为空' })
            return;
        }

        this.setState({ loading: true })
        let url = `${basicDataServiceUrl}/api/borrowing/${id}`;
        try {
            let r = await ApiClient.get(url);
            if (r && r.data && r.data.code === '0') {
                if (!r.data.extension) {
                    notification.warn({ message: '警告', description: '还款单据不存在' });
                } else {
                    //转换
                    let entity = r.data.extension;
                    
                    await this.fetchUser(entity.reimburseUser);
                    
                    this.setState({entity: entity }, () => {
                        var initValues = {};
                        formFields.forEach(k => {
                            initValues[k] = entity[k]
                        })
                        initValues.reimburseUser = [{ key: entity.reimburseUser,label:entity.reimburseUserInfo.userName||'' }]
                        this.props.form.setFieldsValue(initValues);
                    });

                    if(entity.chargeId){
                        this.getReimburseInfo(entity.chargeId);
                    }
                }
            }
        } catch (e) {
            notification.error({ message: '异常', description: e.message })
        }
        this.setState({ loading: false })
    }

    getReimburseInfo = async (id)=>{
        this.setState({ loading: true })
        let url = `${basicDataServiceUrl}/api/borrowing/${id}`;
        try {
            let r = await ApiClient.get(url);
            if (r && r.data && r.data.code === '0') {
                if (!r.data.extension) {
                    notification.warn({ message: '警告', description: '借款单据不存在' });
                } else {
                    //转换
                    let entity = r.data.extension;
                    this.setState({reimburseInfo: entity})
                }
            }
        } catch (e) {
            notification.error({ message: '异常', description: e.message })
        }
        this.setState({ loading: false })
    }

   

    getNodes = async () => {
        let url = `${AuthorUrl}/api/Permission/${permission.yjkgl}`;
        let r = await ApiClient.get(url, true);
        if (r && r.data && r.data.code === '0') {
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({ nodes: nodes });
        } else {
            notification.error(`获取还款部门失败:${((r || {}).data || {}).message || ''}`);
        }
    }

    fetchUser = async (value) => {
        let url = `${basicDataServiceUrl}/api/humaninfo/simpleSearch`;
        var r = await ApiClient.get(url, true, { permissionId: permission.yjkgl, keyword: value, pageSize: 0, pageIndex: 0 });
        if (r && r.data && r.data.code === '0') {
            this.setState({ userList: r.data.extension })
        }
    }
    selectedUser = (value) => {
        this.setState({
            data: [],
            fetching: false,
        });
        if (!value || (value && value.length === 0)) {
            this.props.form.setFieldsValue({ userId: [] })
        } else {
            let ru = value[value.length - 1];
            setTimeout(() => {
                this.props.form.setFieldsValue({ reimburseUser: [ru] })
            }, 0);
         
            
        }
        this.setState({ userList: [] })
    }

    

    submit = async (b) => {
        if (this.state.op === 'add' || this.state.op === 'edit') {
            //新增
            if (b) {
                confirm({
                    title: `您确定要提交预借款还款吗?`,
                    content: '提交后不可再进行修改',
                    onOk: async () => {
                        await this._submit(true, repaymentStatus.Submit);
                    },
                    onCancel() {

                    },
                });

            } else {
                await this._submit(false);
            }
        } 
    }

    _submit = async (b, status) => {
        //检查
        this.props.form.validateFieldsAndScroll();
        var errors = this.props.form.getFieldsError();

        if (validations.checkErrors(errors)) {
            notification.error({ message: '验证失败', description: '表单验证失败，请检查' });
            return;
        }


        let values = this.props.form.getFieldsValue();
        let isBackup = values.isBackup;
        if ((values.chargeAmount * 1) === 0) {
            notification.error({ message: '验证失败', description: '费用总额为0' });
            return;
        }
        if(this.state.entity.chargeId){
            if((values.chargeAmount * 1)<(values.reimbursedAmount*1)){
                notification.error({ message: '验证失败', description: '抵扣借款金额大于报销总额' });
                return;
            }
        }


        var entity = { ...this.state.entity, ...values };

        entity.reimburseUser = entity.reimburseUser[0].key;

       
        if (b) {
            entity.status = status;
        }

        console.log(entity);
        this.setState({ saveing: true })
        try {
            let url = `${basicDataServiceUrl}/api/borrowing`;
            let r = await ApiClient.post(url, entity);
            if (r && r.data && r.data.code === '0') {
                console.log(r.data.extension);
                let ritem = r.data.extension;
                this.props.form.setFieldsValue({ chargeNo: ritem.chargeNo });
                let newState = {
                    entity: {
                        ...this.state.entity, ...{
                            branchId: ritem.branchId,
                            chargeNo: ritem.chargeNo,
                            createTime: ritem.createTime,
                            submitTime: ritem.submitTime,
                            status: ritem.status
                        }
                    }
                };

                if (entity.status === borrowingStatus.Submit) {
                    newState.canEditBase = false;
                }
                this.setState(newState);
                if (this.props.changeCallback) {
                    this.props.changeCallback(this.state.entity);
                }
                notification.success({ message: "保存成功！" });
            } else {
                if (r.data.code === "410") {
                    Modal.error({
                        title: '费用超限',
                        content: r.data.message || '',
                    });
                    this.setState({ saveing: false })
                    return;
                }
                notification.error({ message: '保存失败', description: `保存失败：${((r || {}).data || {}).message || ''}` })
            }
        } catch (e) {

        }
        this.setState({ saveing: false })
    }


    closeDialog = (key) => {
        let d = {};
        d[key] = false;
        this.setState(d);
    }
    showDialog = (key) => {
        let d = {};
        d[key] = true;
        this.setState(d);
    }

    reject = async (entity) => {
        await this._confirm(entity, repaymentStatus.Reject);
    }


    confirm = async (entity) => {
        await this._confirm(entity, repaymentStatus.Confirm);
    }

    _confirm = async (entity, status) => {
        let url = `${basicDataServiceUrl}/api/borrowing/confirm`
        let body = {
            id: this.state.entity.id,
            status: status,
            message: entity.message
        }
        console.log(body)
        this.setState({ confirmLoading: true })
        try {
            let r = await ApiClient.post(url, body);
            if (r && r.data && r.data.code === '0') {
                this.setState({ entity: { ...this.state.entity, ...{ status: status, confirmMessage: entity.message } } })
                if (this.props.changeCallback) {
                    this.props.changeCallback(this.state.entity);
                }
                notification.success({ message: '操作成功' })
            } else {
                notification.error({ message: `操作失败:${((r || {}).data || {}).message || ''}` })
            }
        } catch (e) {
            notification.error({ message: `发生异常：${e.message || ''}` })
        }
        this.setState({ confirmLoading: false })
        this.closeDialog("dlgConfirm");
    }

    submitCharge = async () => {
        await this.submit(true);
    }

    
    recordingConfirm = async () => {
        confirm({
            title: `财务确认`,
            content: '确认预借款还款是否已入账',
            onOk: async () => {
                await this._recordingConfirm(this.state.entity, recordingStatus.Confirm)
            },
            onCancel() {

            },
        });
    }

    _recordingConfirm = async (entity, status) => {
        let url = `${basicDataServiceUrl}/api/borrowing/recordingconfirm`
        let body = {
            id: this.state.entity.id,
            status: status
        }
       
        try {
            let r = await ApiClient.post(url, body);
            if (r && r.data && r.data.code === '0') {
                this.setState({ entity: { ...this.state.entity, ...{ recordingStatus: status } } })
                if (this.props.changeCallback) {
                    this.props.changeCallback(this.state.entity);
                }
                notification.success({ message: '操作成功' })
            } else {
                notification.error({ message: `操作失败:${((r || {}).data || {}).message || ''}` })
            }
        } catch (e) {
            notification.error({ message: `发生异常：${e.message || ''}` })
        }
       
       
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const lenValidator = [{ max: 120, message: '参数值长度不得大于120个字符' }]
        let {fetchingUser, userList, permission, entity,pagePar} = this.state;

        let btns = [];
        let sText = '';
        let s = entity.status;

        sText = chargeStatus[s];
        if( entity.recordingStatus=== recordingStatus.Confirm){
            sText = `${sText}-${recordingStatus[entity.recordingStatus]}`
        }

        if( (s === repaymentStatus.UnSubmit || s === repaymentStatus.Reject) &&
        !pagePar.noGL && ( permission.gl || entity.reimburseUser === this.props.user.sub )){
            btns.push(<Button type="primary" style={{marginLeft: '0.5rem'}} onClick={()=>this.submitBorrowing()}>提交</Button>)
        } 
        if(s === repaymentStatus.Submit && (!pagePar.noQR && permission.qr) ){
            btns.push(<Button type="primary" style={{marginLeft: '0.5rem'}} onClick={()=>this.showDialog("dlgConfirm")}>确认</Button>)
        }
        if(s === repaymentStatus.Confirm && entity.recordingStatus!==recordingStatus.Confirm && (!pagePar.noFK && permission.fk) ){
            btns.push(<Button type="primary" style={{marginLeft: '0.5rem'}} onClick={()=>this.recordingConfirm()}>财务确认</Button>)
        }

        let canChangeDepartment = false;
        if(permission.gl){
            canChangeDepartment = true;
        }
        
        let ri = this.state.reimburseInfo;
        let riText = ''
        if(ri){
            riText = `对应预借款单据${ri.chargeNo}, 总借款金额：${ri.chargeAmount}元，已报销/还款：${ri.reimbursedAmount||0}元，可还款：${ri.chargeAmount-(ri.reimbursedAmount||0)}元`
        }

       
        return <Layer showLoading={this.state.loading} className="content-page"
            fixedPanel={
                <div style={styles.topBtnBar}>
                    {
                        btns.length > 0 ? <div style={styles.topBtnBarBackground}>
                            {btns}
                        </div> : null
                    }

                </div>
            }>
            <div>
                <div className="page-title">预借款还款<div style={styles.statusText}>{sText}</div></div>
                <div className="page-subtitle">{moment(this.state.entity.createTime).format('YYYY年MM月DD日')}</div>

            </div>
            <div>
                {
                    (this.state.entity.status === repaymentStatus.Reject || (this.state.entity.status === repaymentStatus.Submit && this.state.entity.confirmMessage)) ?
                        <Alert message={`驳回意见：${this.state.entity.confirmMessage}`} type="warning" /> : null
                }
                
                {
                    riText ?
                        <Alert message={riText} type="warning" /> : null
                }
                
                <Form ref={(e) => this.form = e}>
                    <Row className="form-row">
                        <Col span={6}>
                            <FormItem label="单据号">
                                {getFieldDecorator('chargeNo', {})(
                                    <Input readOnly disabled />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={10}>
                            <FormItem hasFeedback label="还款部门">
                                {getFieldDecorator('reimburseDepartment', {
                                    rules: [{ required: true, message: '必须选择还款部门' }],
                                })(
                                    <TreeSelect
                                        disabled={!this.state.canEditBase || !canChangeDepartment}
                                        style={{ width: 300 }}
                                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                        treeData={this.state.nodes}
                                        placeholder="请选择还款部门"
                                    />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem label="还款人">
                                {getFieldDecorator('reimburseUser', { rules: [{ required: true, message: '必须选择还款人' }] })(
                                    <Select
                                        disabled={!this.state.canEditBase || !canChangeDepartment}
                                        mode="multiple"
                                        maxTagCount={1}
                                        labelInValue
                                        placeholder="输入姓名、员工编号或手机号码"
                                        notFoundContent={fetchingUser ? <Spin size="small" /> : null}
                                        filterOption={false}
                                        onSearch={this.fetchUser}
                                        onChange={this.selectedUser}
                                        style={{ width: '100%' }}
                                    >
                                        {userList.map(d => <Option key={d.id}>{`${d.name}(${d.organizationFullName})`}</Option>)}
                                    </Select>
                                )}
                            </FormItem>
                        </Col>

                    </Row>
                   


                    <Row className="form-row">
                       
                            <Col span={6}>
                                <FormItem label="本次还款借款">
                                    {getFieldDecorator('reimbursedAmount', {rules:[{required: true, message:'请输入本次还款金额'}]})(
                                        <InputNumber disabled={!this.state.canEditBase} style={{ textAlign: 'right', width:'100%' }}  />
                                    )}
                                </FormItem>
                            </Col>
                            <Col span={18}>
                            <FormItem label="说明">
                                {getFieldDecorator('memo', { rules: [...lenValidator] })(
                                    <Input disabled={!this.state.canEditBase} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>

                </Form>
            </div>
           
            <div style={{ marginTop: '1rem', textAlign: 'center' }}>
                {
                    (this.state.canEditBase ) ?
                        <div>
                            <Button size="large" loading={this.state.saveing} type="primary" onClick={() => this.submit(false)}>暂存</Button>
                            <Button size="large" style={{ marginLeft: '1rem' }} loading={this.state.saveing} type="primary" onClick={() => this.submit(true)}>提交</Button>
                        </div> : null
                }

            </div>

           
            <ConfirmDialog title="还款确认" loading={this.state.confirmLoading} visible={this.state.dlgConfirm} onCancel={() => this.closeDialog('dlgConfirm')} onReject={this.reject} onSubmit={this.confirm} />
            <PaymentDialog title="财务确认" loading={this.state.paymentLoading} visible={this.state.dlgPayment} onCancel={() => this.closeDialog('dlgPayment')} onSubmit={this.payment} initData={this.state.entity} />
        </Layer>
    }
}

const mapStateToProps = (state, props) => ({
    dic: state.basicData.dicList,
    user: state.oidc.user.profile
})
const mapActionToProps = (dispatch) => ({
    getDicParList: (...args) => dispatch(getDicParList(...args))
})

export default connect(mapStateToProps, mapActionToProps)(Form.create()(Repayment))