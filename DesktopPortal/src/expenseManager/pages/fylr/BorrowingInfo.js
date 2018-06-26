import React, { Component } from 'react';
import { Alert, Select, DatePicker, Modal, Icon, Table, Form, Checkbox, Input, TreeSelect, Row, Col, Button, notification, Spin, InputNumber } from 'antd'
import { connect } from 'react-redux';
import FixedTable from '../../../components/FixedTable'
import { AuthorUrl, basicDataServiceUrl, UploadUrl } from '../../../constants/baseConfig'
import ApiClient from '../../../utils/apiClient'
import { getDicPars, getOrganizationTree } from '../../../utils/utils'
import { getDicParList } from '../../../actions/actionCreators'
import validations from '../../../utils/validations'
import Layer from '../../../components/Layer'
import { borrowingStatus, chargeStatus, permission, billType, recordingStatus } from './const'
import moment from 'moment'
import uuid from 'uuid';
import ConfirmDialog from './ConfirmDialog'
import PaymentDialog from './PaymentDialog'

const FormItem = Form.Item;
const Option = Select.Option;
const confirm = Modal.confirm;

const formFields = ['chargeNo', 'reimburseDepartment', 'reimburseUser', 'expectedPaymentDate', 'memo', 'chargeAmount','reimbursedAmount']


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

class BorrowingInfo extends Component {
    state = {
        nodes: [],
        feeList: [],
        billList: [],
        userList: [],
        fetching: false,
        entity: {},
        op: '',
        saveing: false,
        previewVisible: false,
        previewImage: '',
        loading: false,
        canEditBase: false,
        dlgConfirm: false,
        confirmLoading: false,
        dlgConfirmBill: false,
        confirmBillLoading: false,
        dlgPayment: false,
        paymentLoading: false,
        permission: {},
        pagePar: {}
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
        } else if (initState.entity) {
            let canEditBase = false;
            if (initState.op === 'edit') {
                canEditBase = true;
            }
            this.setState({ canEditBase })
            this.setState({ op: initState.op })
            this.getBorrowingInfo(initState.entity.id);
        }
        this.setState({ pagePar: initState.pagePar || {} })
        console.log(this.props.match);
        //    this.props.getDicParList(['CHARGE_COST_TYPE']);
        this.getNodes();
        this.getPermission();
    }


    getPermission = async () => {
        let url = `${AuthorUrl}/api/Permission/each`
        let r = await ApiClient.post(url, [permission.yjk, permission.yjkgl, permission.yjkqr, permission.yjkfk])
        if (r && r.data && r.data.code === '0') {
            let p = {};
            (r.data.extension || []).forEach(pi => {
                if (pi.permissionItem === permission.yjkqr) {
                    p.qr = pi.isHave;
                } else if (pi.permissionItem === permission.yjkfk) {
                    p.fk = pi.isHave;
                } else if (pi.permissionItem === permission.yjkgl) {
                    p.gl = pi.isHave;
                } else if (pi.permissionItem === permission.yjk) {
                    p.yjk = pi.isHave;
                }
            })
            console.log(p);
            this.setState({ permission: p })
        }

    }

    getBorrowingInfo = async (id) => {
        if (!id) {
            notification.error({ message: '报销单id为空' })
            return;
        }

        this.setState({ loading: true })
        let url = `${basicDataServiceUrl}/api/borrowing/${id}`;
        try {
            let r = await ApiClient.get(url);
            if (r && r.data && r.data.code === '0') {
                if (!r.data.extension) {
                    notification.warn({ message: '警告', description: '单据不存在' });
                } else {
                    //转换
                    let entity = r.data.extension;

                    await this.fetchUser(entity.reimburseUser);

                    this.setState({ entity: entity }, () => {
                        var initValues = {};
                        formFields.forEach(k => {
                            initValues[k] = entity[k]
                        })
                        initValues.reimburseUser = [{ key: entity.reimburseUser }]
                        initValues.expectedPaymentDate = moment(entity.expectedPaymentDate)



                        this.props.form.setFieldsValue(initValues);
                    });
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
            notification.error(`获取报销门店失败:${((r || {}).data || {}).message || ''}`);
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
                    title: `您确定要提交报销单吗?`,
                    content: '提交后不可再进行修改',
                    onOk: async () => {
                        await this._submit(true, borrowingStatus.Submit);
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
        this.props.form.validateFieldsAndScroll();
        var errors = this.props.form.getFieldsError();

        if (validations.checkErrors(errors)) {
            notification.error({ message: '验证失败', description: '表单验证失败，请检查' });
            return;
        }


        let values = this.props.form.getFieldsValue();
        if ((values.chargeAmount * 1) <= 0) {
            notification.error({ message: '验证失败', description: '借款金额小于等于0' });
            return;
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
                let ritem = r.data.extension;
                this.props.form.setFieldsValue({ chargeNo: ritem.chargeNo });
                let newState = {
                    entity: {
                        ...this.state.entity, ...{
                            branchId: ritem.branchId,
                            chargeNo: ritem.chargeNo,
                            createTime: ritem.createTime,
                            submitTime: ritem.submitTime,
                            chargeAmount: ritem.chargeAmount,
                            reimburseUser: ritem.reimburseUser,
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
        await this._confirm(entity, borrowingStatus.Reject);
    }


    confirm = async (entity) => {
        await this._confirm(entity, borrowingStatus.Confirm);
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

    submitBorrowing = async () => {
        await this.submit(true);
    }


    payment = async (entity) => {
        let url = `${basicDataServiceUrl}/api/borrowing/payment`
        let body = {
            id: uuid.v1(),
            chargeId: this.state.entity.id,
            ...entity
        }
        console.log(body)
        this.setState({ paymentLoading: true })
        try {
            let r = await ApiClient.post(url, body);
            if (r && r.data && r.data.code === '0') {
                let rItem = r.data.extension.chargeInfo;
                this.setState({
                    entity: {
                        ...this.state.entity, ...{
                            isPayment: rItem.isPayment,
                            paymentAmount: rItem.paymentAmount,
                            paymentDate: rItem.paymentDate
                        }
                    }
                })
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
        this.setState({ paymentLoading: false })
        this.closeDialog("dlgPayment");
    }

    rowStyle = (record, index) => {
        let groupList = getDicPars('CHARGE_COST_TYPE', this.props.dic);
        let ti = groupList.find(x => (x.value * 1) === record.type)
        if (ti && ti.ext1 === "1") {
            return "limit-row"
        }
        return "";
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const lenValidator = [{ max: 120, message: '参数值长度不得大于120个字符' }]

        let { fetchingUser, userList, permission, entity, pagePar } = this.state;

        let btns = [];
        let sText = '';

        let s = entity.status;

        sText = chargeStatus[s];
        if (entity.isPayment) {
            sText = `${sText}-财务已付款`
        }

        if ((s === borrowingStatus.UnSubmit || s === borrowingStatus.Reject) &&
            !pagePar.noGL && (permission.gl || entity.reimburseUser === this.props.user.sub)) {
            btns.push(<Button type="primary" style={{ marginLeft: '0.5rem' }} onClick={() => this.submitBorrowing()}>提交</Button>)
        }
        if (s === borrowingStatus.Submit && (!pagePar.noQR && permission.qr)) {
            btns.push(<Button type="primary" style={{ marginLeft: '0.5rem' }} onClick={() => this.showDialog("dlgConfirm")}>确认</Button>)
        }
        if (s === borrowingStatus.Confirm && !entity.isPayment && (!pagePar.noFK && permission.fk)) {
            btns.push(<Button type="primary" style={{ marginLeft: '0.5rem' }} onClick={() => this.showDialog("dlgPayment")}>财务确认</Button>)
        }
        let canChangeDepartment = false;
        if (permission.gl) {
            canChangeDepartment = true;
        }

        let showOther=  true;
        if(this.state.op==='add' || this.state.op==='edit'){
            showOther = false;
        }

        const columns = [
            {
                title: '类型',
                dataIndex: 'type',
                key: 'type',
                width: '8rem',
                render: (text, record) => {
                    return billType[record.type];
                }
            },
            {
                title: '单据号',
                dataIndex: 'chargeNo',
                key: 'chargeNo',
                width: '10rem',
                // render: (text, record)=>{

                //     return  record.type===2? <a href="javascript:void();" title="点击查看详情" onClick={()=>this.gotoDetail(record)}>{text}</a>
                //         : <a href="javascript:void();" title="点击查看详情" onClick={()=>this.gotoRepaymentDetail(record)}>{text}</a>
                // }
            },
            {
                title: '部门',
                dataIndex: 'reimburseDepartmentName',
                key: 'reimburseDepartmentName'
            },
            {
                title: '报销人/还款人',
                dataIndex: 'reimburseUserInfo',
                key: 'reimburseUserInfo',
                width: '6rem',
                render: (text, record) => {
                    return (record.reimburseUserInfo || {}).userName
                }
            },
            {
                title: '日期',
                dataIndex: 'createTime',
                key: 'createTime',
                width: '8rem',
                render: (text, record) => {
                    return moment(record.createTime).format('YYYY-MM-DD');
                }
            },
            {
                title: '费用总额',
                dataIndex: 'chargeAmount',
                key: 'chargeAmount',
                className: 'column-money',
                width: '10rem',
                render: (text, record) => {
                    if (record.type === 1) {
                        return '-'
                    }
                    return <span style={{ textAlign: 'right' }}>{record.chargeAmount}</span>
                }
            },
            {
                title: '冲减预借款金额',
                dataIndex: 'reimbursedAmount',
                key: 'reimbursedAmount',
                className: 'column-money',
                width: '10rem',
                render: (text, record) => {
                    return <span style={{ textAlign: 'right' }}>{record.reimbursedAmount}</span>
                }
            },
            {
                title: '状态',
                dataIndex: 'status',
                key: 'status',
                width: '10rem',
                render: (text, record) => {
                    let s = chargeStatus[record.status];
                    if(record.type===1){
                        if(record.isPayment){
                            s = s + " | 已付款"
                        }
                    }
                    if(record.type===3){
                        if(record.recordingStatus===recordingStatus.Confirm){
                            s = s + ` | ${recordingStatus[record.recordingStatus]}`
                        }
                    }

                    return  s;
                }
            }
        ]

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
                <div className="page-title">预借款<div style={styles.statusText}>{sText}</div></div>
                <div className="page-subtitle">{moment(this.state.entity.createTime).format('YYYY年MM月DD日')}</div>

            </div>
            <div>
                {
                    (this.state.entity.status === borrowingStatus.Reject || (this.state.entity.status === borrowingStatus.Submit && this.state.entity.confirmMessage)) ?
                        <Alert message={`驳回意见：${this.state.entity.confirmMessage}`} type="warning" /> : null
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
                            <FormItem hasFeedback label="借款部门">
                                {getFieldDecorator('reimburseDepartment', {
                                    rules: [{ required: true, message: '必须选择报借款部门' }],
                                })(
                                    <TreeSelect
                                        disabled={!this.state.canEditBase || !canChangeDepartment}
                                        style={{ width: 300 }}
                                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                        treeData={this.state.nodes}
                                        placeholder="请选择借款部门"
                                    />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem label="借款人">
                                {getFieldDecorator('reimburseUser', { rules: [{ required: true, message: '必须选择借款人' }] })(
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
                            <FormItem label="借款金额(元)">
                                {getFieldDecorator('chargeAmount', { rules: [{ required: true, message: '必须输入借款金额' }] })(
                                    <InputNumber style={{ textAlign: 'right', width: '100%' }} disabled={!this.state.canEditBase} />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={10}>
                            <FormItem label="预计还款日期">
                                {getFieldDecorator('expectedPaymentDate', { rules: [{ required: true, message: '必须选择预计还款日期' }] })(
                                    <DatePicker disabled={!this.state.canEditBase} />
                                )}
                            </FormItem>
                        </Col>
                        {
                            showOther?<Col span={6}>
                            <FormItem label="已还款(元)">
                                {getFieldDecorator('reimbursedAmount', {})(
                                    <InputNumber style={{ textAlign: 'right', width: '100%' }} disabled />
                                )}
                            </FormItem>
                        </Col>:null
                        }
                        
                    </Row>
                    <Row className="form-row">

                        <Col span={24}>
                            <FormItem label="借款原因">
                                {getFieldDecorator('memo', { rules: [{ required: true, message: '必须输入借款原因' }, ...lenValidator] })(
                                    <Input disabled={!this.state.canEditBase} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                </Form>
            </div>


            <div style={{ marginTop: '1rem', textAlign: 'center' }}>
                {
                    (this.state.canEditBase) ?
                        <div>
                            <Button size="large" loading={this.state.saveing} type="primary" onClick={() => this.submit(false)}>暂存</Button>
                            <Button size="large" style={{ marginLeft: '1rem' }} loading={this.state.saveing} type="primary" onClick={() => this.submit(true)}>提交</Button>
                        </div> : null
                }

            </div>
            {
                (this.state.op === 'add' || this.state.op === 'edit') ? null :
                    <div>
                        <div style={{ marginTop: '1rem' }}>
                            <span>报销/还款明细</span>
                        </div>
                        <div style={{ marginTop: '0.5rem' }}>

                            <Table columns={columns}
                                rowKey={(record) => record.id}
                                rowClassName={this.rowStyle}
                                dataSource={this.state.entity.chargeList}
                                bordered={true} pagination={false} />

                        </div>
                    </div>
            }

            <ConfirmDialog title="预借款确认" loading={this.state.confirmLoading} visible={this.state.dlgConfirm} onCancel={() => this.closeDialog('dlgConfirm')} onReject={this.reject} onSubmit={this.confirm} />
            <PaymentDialog title="付款" loading={this.state.paymentLoading} visible={this.state.dlgPayment} onCancel={() => this.closeDialog('dlgPayment')} onSubmit={this.payment} initData={this.state.entity} />
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

export default connect(mapStateToProps, mapActionToProps)(Form.create()(BorrowingInfo))