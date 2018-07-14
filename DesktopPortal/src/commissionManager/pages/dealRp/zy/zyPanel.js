//调佣对话框
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { notification, Row, Col, Form, InputNumber, TreeSelect, Select, Spin, Input, Button, Modal } from 'antd'
import { getDicParList } from '../../../../actions/actionCreators'
import { dicKeys, permission, examineStatusMap } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { getOrganizationTree } from '../../../../utils/utils'
import validations from '../../../../utils/validations'


import '../rp.less'

const FormItem = Form.Item;
const Option = Select.Option;
const confirm = Modal.confirm;

class ZyPanel extends Component {

    constructor(props) {
        super(props);
        this.state = {
            report: {},
            saving: false,
            nodes: [],
            userList: []
        }
    }

    componentWillMount = () => {
        this.initEntity(this.props)
        this.getNodes();
    }

    componentWillReceiveProps = (nextProps) => {
        if (this.props.report !== nextProps.report && nextProps.report) {
            this.initEntity(nextProps);
        }
    }

    initEntity = (props) => {
        let r = props.report || {};
        this.setState({ report: r }, () => {
            this.props.form.setFieldsValue({
                applySectionId: r.applySectionId,
                uid: r.uid
            })
            if (r.applySectionId) {
                this.getUserList(r.applySectionId);
            }

        })

    }

    getNodes = async () => {
        let url = `${WebApiConfig.org.permissionOrg}${permission.op_zy}`;
        let r = await ApiClient.get(url, true);
        if (r && r.data && r.data.code === '0') {
            var nodes = getOrganizationTree(r.data.extension);
            this.setState({ nodes: nodes });
        } else {
            notification.error(`获取组织失败:${((r || {}).data || {}).message || ''}`);
        }
        this.getNoded = true;
    }

    orgChanged = (id) => {
        this.setState({ userList: [] })
        this.props.form.setFieldsValue({
            uid: null
        })
        this.getUserList(id);
    }

    getUserList = async (id) => {
        let url = `${WebApiConfig.human.orgUser}`;
        let r = await ApiClient.get(url, true, { permissionId: permission.op_zf, branchId: id, pageSize: 0, pageIndex: 0 });
        r = (r || {}).data || {};

        let ul = [];
        if (r.code === '0') {
            ul = r.extension || [];
        } else {
            notification.error({ message: `获取成交用户列表失败:${((r || {}).data || {}).message || ''}` });
        }

        this.setState({ userList: ul })
    }

    submit = () => {
        let r = validations.validateForm(this.props.form);
        if (r.hasError) {
            notification.error({ message: '输入有误，请检查是否填写了所有必填项' })
            return;
        }
        let values =  r.values;
        values.id = this.state.report.id;

        let u = this.state.userList.find(x=>x.id === values.uid);
        if(!u){
            notification.error({ message: '无归属人信息' })
            return;
        }
        values.uTrueName=u.name;
        values.uUserName=u.userId;


        confirm({
            title: `进确定要将成交报告转移至新的归属人么?`,
            content: '请确认',
            onOk: async () => {

                await this._submit(r.values);
            },
            onCancel() {

            },
        });
    }

    _submit = async (values)=>{
        if(!values){
            return;
        }

        this.setState({ saving: true })
        let url = `${WebApiConfig.rp.rpZy}`
        let r = await ApiClient.post(url, values, null, 'PUT');
        r = (r || {}).data;
        if (r.code === '0') {
            let nr = r.extension;
            let d = this.state.report;
            d.applySectionId = nr.applySectionId;
            d.uid = nr.uid;
            d.uTrueName = nr.uTrueName;
            d.uUserName = nr.uUserName;
          
            this.setState({ report: { ...d } })
            notification.success({ message: '成交报告已成功转移' })
            
        } else {
            notification.error({ message: '转移失败', description: r.message || '' })
        }

        this.setState({ saving: false })
    }


    render() {
        const { getFieldDecorator } = this.props.form;


        let report = this.state.report || {};
        let wy = report.reportWy || {}
        let canEdit = this.props.canEdit;

        return (
            <div>
                <Row style={{ marginTop: '2rem' }}>
                    <span className="rp-yj">成交编号：</span><span className="rp-yj-je"> {report.cjbgbh} </span>
                    <span className="rp-yj">物业名称：</span><span className="rp-yj-je"> {wy.wyMc || ""} </span>
                </Row>
                <div className="divider"></div>
                <Row>
                    <span className="rp-yj">当前部门：</span><span className="rp-yj" style={{ width: 'auto' }}>{report.applySectionName}</span>
                </Row>
                <Row>
                    <span className="rp-yj">当前归属人：</span><span className="rp-yj" style={{ width: 'auto' }}>{report.uTrueName}</span>
                </Row>
                <div className="divider"></div>
                <div className="rp-yj-tbl-title" style={{marginBottom: '2rem'}}>新的归属</div>
                <Row className="form-row">
                    <Col>
                        <FormItem label="所属部门">
                            {getFieldDecorator('applySectionId', {
                                rules: [{ required: true, message: '请选择所属部门' }]
                            })(
                                <TreeSelect
                                    style={{width: '15rem'}}
                                    dropdownStyle={{ maxHeight: 400, minWidth: 400, overflow: 'auto' }}
                                    disabled={!canEdit}
                                    onChange={this.orgChanged}
                                   
                                    treeData={this.state.nodes}
                                    placeholder="请选择所属部门"
                                />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row className="form-row">
                    <Col>
                        <FormItem label="归属人">
                            {getFieldDecorator('uid', {
                                rules: [{ required: true, message: '请选择归属人' }]
                            })(
                                <Select disabled={!canEdit} style={{width: '15rem'}}>
                                    {
                                        this.state.userList.map(u => <Option key={u.id} value={u.id}>{u.name}</Option>)
                                    }
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                {
                    canEdit ? <Row className="rp-yj-btn-bar">
                        <Button loading={this.state.saving} type="primary" onClick={this.submit} size="large">转移</Button>
                    </Row> : null
                }
            </div>
        )
    }
}
function MapStateToProps(state) {

    return {
        dic: state.basicData.dicList,
        user: state.oidc.user.profile || {}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}
const WrappedRegistrationForm = Form.create()(ZyPanel);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);