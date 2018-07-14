//调佣对话框
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { notification, Row, Col, Form, InputNumber, TreeSelect, Select, Spin, Input, Button, Modal } from 'antd'
import uuid from 'uuid'
import { getDicParList } from '../../../../actions/actionCreators'
import { dicKeys, examineStatusMap } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import SkTable from './skTable'


import '../rp.less'

const FormItem = Form.Item;
const Option = Select.Option;
const confirm = Modal.confirm;

class JyPanel extends Component {

    constructor(props) {
        super(props);
        this.state = {
            report: {},
            list: [],
            saving: false
        }
    }

    componentWillMount = () => {
        this.initEntity(this.props)
    }

    componentWillReceiveProps = (nextProps) => {
        if (this.props.distribute !== nextProps.distribute && nextProps.distribute) {
            this.initEntity(nextProps);
        }
    }

    initEntity = (props) => {
        this.setState({ report: props.report, list: props.list || [] }, () => {

        })

    }

    submit = () => {
        if (!this.skTable) {
            return;
        }

        let ids = this.skTable.getSelectedIds();
        if (!ids || ids.length === 0) {
            notification.error({ message: '请勾选需要结佣的收款记录' })
            return;
        }

        confirm({
            title: `您确定要进行结佣确认么?`,
            content: '',
            onOk: async () => {

                await this._submit(ids);
            },
            onCancel() {

            },
        });
    }

    _submit = async (values) => {
        if (!values) {
            return;
        }

        this.setState({ saving: true })
        try {
            let url = `${WebApiConfig.rp.rpJy}${this.state.report.id}`
            let r = await ApiClient.post(url, values, null, 'PUT');
            r = (r || {}).data;
            if (r.code === '0') {
                let nl = r.extension || [];
                let list = this.state.list;
                nl.forEach(item => {
                    let old = list.find(x => x.id === item.id)
                    if (old) {
                        old.jrr = item.jrr;
                        old.jrrq = item.jrrq;
                    }
                })

                this.setState({ list: [...list], saved: true })
                notification.success({ message: '结佣确认成功!' })

            } else {
                notification.error({ message: '确认失败', description: r.message || '' })
            }
        } catch (e) {
            notification.error({ message: '确认失败：' + (e.message || '') })
        }

        this.setState({ saving: false })
    }

    render() {
        let { report, list } = this.state;
        let { canEdit } = this.props;
        if (this.state.saved) {
            canEdit = false;
        }
        let yjfp = report.reportYjfp || {};

        return (
            <div style={{paddingTop:'2rem'}}>
                <Row>
                    <span className="rp-yj">业主佣金：</span><span className="rp-yj-je"> {yjfp.yjYzys || 0} </span>
                    <span className="rp-yj">已收取：</span><span className="rp-yj-je"> {yjfp.yjYsyy || 0} </span>
                    <span className="rp-yj">余额：</span><span className="rp-yj-je"> {yjfp.yjYyyk || 0} </span>

                </Row>
                <Row>
                    <span className="rp-yj">客户佣金：</span><span className="rp-yj-je"> {yjfp.yjKhys || 0} </span>
                    <span className="rp-yj">已收取：</span><span className="rp-yj-je"> {yjfp.yjYsky || 0} </span>
                    <span className="rp-yj">余额：</span><span className="rp-yj-je"> {yjfp.yjKyyk || 0} </span>

                </Row>
                <Row>
                    <span className="rp-yj">成交编号：</span><span className="rp-yj-je"> {report.cjbgbh} </span>
                    <span className="rp-yj">物业名称：</span><span className="rp-yj-je"> {(report.reportWy || {}).wyMc} </span>
                </Row>
                <div className="divider"></div>
                <div className="rp-yj-tbl-title" style={{ marginBottom: '2rem' }}>请勾选此次结佣收款项</div>
                <Row>
                    <SkTable
                        ref={(ins) => this.skTable = ins}
                        list={list}
                        canSelect={true}
                        canEdit={canEdit}
                    />
                </Row>

                {
                    canEdit ? <Row className="rp-yj-btn-bar">
                        <Button loading={this.state.saving} type="primary" onClick={this.submit} size="large">结佣确认</Button>
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
const WrappedRegistrationForm = Form.create()(JyPanel);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);