//调佣对话框
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { notification, Row, Col, Form, InputNumber, TreeSelect, Select, Spin, Input, Button, Modal } from 'antd'
import DistributePanel from './distributePanel'
import uuid from 'uuid'
import { getDicParList } from '../../../../actions/actionCreators'
import { dicKeys, examineStatusMap } from '../../../constants/const'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import TradeWyTable from '../rpdetails/tradeWyTable'
import TradeNTable from '../rpdetails/tradeNTable'
import reportValidation from '../../../constants/reportValidation'
import validations from '../../../../utils/validations'

import '../rp.less'

const FormItem = Form.Item;
const Option = Select.Option;
const confirm = Modal.confirm;

class TyPanel extends Component {

    constructor(props) {
        super(props);
        this.state = {
            wyItems: [],
            nyItems: [],
            canEdit: false,
            distribute: {},
            outsideList: [],
            insideList: [],
            saving: false
        }
    }

    componentWillMount = () => {
        this.initEntity(this.props)
        this.props.getDicParList([
            dicKeys.sfdx
        ])
        this._getFtItems();
    }

    componentWillReceiveProps = (nextProps) => {
        if (this.props.distribute !== nextProps.distribute && nextProps.distribute) {
            this.initEntity(nextProps);
        }
    }

    initEntity = (props) => {
        let d = props.distribute || {};
        let il = d.reportInsides || [];
        il.forEach(item => {
            item.uid = [{ key: item.uid, label: item.username }];
            item.errors = {};
        })

        let ol = d.reportOutsides || [];
        ol.forEach(item => {
            item.errors = {};
        })

        this.setState({ distribute: d, outsideList: d.reportOutsides || [], insideList: il }, () => {
            this.props.form.setFieldsValue({
                ownerMoney: d.ownerMoney,
                customMoney: d.customMoney,
                yjZcjyj: (d.customMoney || 0) + (d.ownerMoney || 0),
                jyj: d.jyj || 0,
                updateReason: d.updateReason
            })
        })

    }

    _getFtItems = async () => {

        let url = `${WebApiConfig.baseset.acmentlistget}${this.props.user.Filiale}`
        let r = await ApiClient.get(url);
        r = (r || {}).data || {};
        if (r.code === '0') {
            let list = r.extension || [];
            let wyItems = [], nyItems = [];
            list.forEach(item => {
                if (item.type === 1) {
                    wyItems.push(item)
                } else if (item.type === 2) {
                    nyItems.push(item)
                }
            })
            this.setState({ wyItems: wyItems, nyItems: nyItems })
        } else {
            notification.error({ message: '获取分摊项列表失败' })
        }
    }

    //计算业绩浄佣金
    _calcYjJyj = () => {
        let zyj = (this.props.form.getFieldValue("yjZcjyj") || '0') * 1;
        //扣减外佣
        let wyJe = 0;
        this.state.outsideList.forEach(item => {
            wyJe = wyJe + (item.money * 1)
            wyJe = Math.round(wyJe*100)/100;
        })
        let jyj = zyj - wyJe

        this.props.form.setFieldsValue({ jyj: jyj })


        let il = this.state.insideList;
        let nyJe = 0;
        let lastRow = null;
        // let nyItems = this.props.nyItems;
        il.forEach(item => {
            // let x = nyItems.find(x=>x.code === item.type);       
            item.money = Math.round((jyj * (item.percent || 0))) / 100;
            nyJe = nyJe + item.money;
            nyJe = Math.round(nyJe*100)/100;
            lastRow = item;
        })
        let diff = Math.abs(nyJe - jyj);
        if (diff <= 0.2 && diff > 0) {
            lastRow.money = lastRow.money + (jyj - nyJe);
        }

        this.setState({ insideList: [...il] }, () => {

        })
    }

    calcZyj = () => {
        setTimeout(() => {
            var yj = this.props.form.getFieldsValue(["ownerMoney", "customMoney"]);
            var zyj = (yj.ownerMoney || 0) * 1 + (yj.customMoney || 0) * 1;
            this.props.form.setFieldsValue({ yjZcjyj: zyj })

            //更新外部分摊项
            let ol = this.state.outsideList;
            let wyItems = this.state.wyItems;
            ol.forEach(item => {
                let x = wyItems.find(x => x.code === item.moneyType);
                if (x && x.isfixed) {
                    item.money = Math.round((zyj * (x.percent || 0)) * 100) / 100;
                }
            })

            this.setState({ outsideList: [...ol] }, () => {
                this._calcYjJyj()
            })


        }, 0)

    }

    //添加外佣项
    handleAddWy = () => {
        let item = {
            id: uuid.v1(),
            moneyType: null,
            remark: '',
            object: '',
            money: 0,
            isNew: true,
            errors: {

            }
        }

        item.errors = validations.validate(item, reportValidation.wyItem)

        let wyList = [...this.state.outsideList, item]
        this.setState({ outsideList: wyList })
    }

    //外佣列表变更
    onWyRowChanged = (row, key, value) => {
        let ol = this.state.outsideList;
        let idx = ol.findIndex(x => x.id === row.id);
        if (idx < 0) {
            return;
        }

        let newRow = { ...ol[idx] }
        newRow[key] = value;

        if (key === 'moneyType') {
            let zyj = (this.props.form.getFieldValue("yjZcjyj") || '0') * 1;

            let item = this.props.wyItems.find(x => x.code === value);
            if (item && item.percent) {
                newRow.money = Math.round((zyj * (item.percent || 0)) * 100) / 100;
            }
        }

        ol[idx] = newRow;

        newRow.errors = validations.validate(newRow, reportValidation.wyItem)


        this.setState({ outsideList: [...ol] }, () => {
            this._calcYjJyj()
        })

    }

    onWyDelRow = (row) => {
        let ol = this.state.outsideList;
        let idx = ol.findIndex(x => x.id === row.id);
        if (idx < 0) {
            return;
        }
        ol.splice(idx, 1);
        this.setState({ outsideList: [...ol] }, () => {
            this._calcYjJyj()
        })
    }

    //内佣列表变更
    onNyRowChanged = (row, key, value) => {
        let ol = this.state.insideList;
        let idx = ol.findIndex(x => x.id === row.id);
        if (idx < 0) {
            return;
        }

        let newRow = { ...ol[idx] }
        newRow[key] = value;
        if (key === 'uid') {
            if (value) {
                newRow.uid = [{ key: value.id, label: value.name }];
                newRow.sectionId = value.departmentId;
                newRow.sectionName = value.organizationFullName;
                newRow.workNumber = value.userID;
            } else {
                newRow.uid = [];
                newRow.sectionId = '';
                newRow.sectionName = '';
                newRow.workNumber = '';
            }

        }

        let jyj = this.props.form.getFieldValue("jyj") || 0;
        if (key === 'type') {

            let nyItem = this.props.nyItems.find(x => x.code === value);
            if (nyItem) {
                newRow.percent = Math.round((nyItem.percent || 0) * 10000) / 100;
                newRow.money = Math.round((newRow.percent) * jyj) / 100;
            }
        }
        if (key === 'percent') {
            newRow.money = Math.round((newRow.percent) * jyj) / 100;
        }

        let ny = 0;
        ol.forEach(item => {
            ny = item.money * 1 + ny;
            ny = Math.round(ny*100)/100;
        })

        //尾差
        let diff = Math.abs(ny - jyj)
        if (diff <= 0.2 && diff > 0) {
            newRow.money = newRow.money + (jyj - ny);
        }



        ol[idx] = newRow;

        newRow.errors = validations.validate(newRow, reportValidation.nrItem)




        this.setState({ insideList: [...ol] }, () => {

        })

    }

    //验证
    validateVales = () => {
        let r = { code: '0', entity: null }

        let distribute = this.props.form.getFieldsValue();


        

        let ol = this.state.outsideList || [];
        for (let i = 0; i < ol.length; i++) {
            let item = ol[i]
            let e = validations.validate(item, reportValidation.wyItem);
            if (validations.checkErrors(e)) {
                r.code = "500"
                r.errors = e;
                return r;
            }
        }


        let il = this.state.insideList || [];
        for (let i = 0; i < il.length; i++) {
            let item = il[i]
            let e = validations.validate(item, reportValidation.nrItem);
            if (validations.checkErrors(e)) {
                r.code = "500"
                r.errors = e;
                return r;
            }
        }

        ol = ol.map(item => {
            let ni = { ...item }
            return ni;
        })
        il = il.map(item => {
            let ni = { ...item }
            ni.uid = item.uid[0].key;
            ni.username = item.uid[0].label;
            return ni;
        })

        distribute.reportInsides = il;
        distribute.reportOutsides = ol;
        distribute.preId = this.state.distribute.preId;
        distribute.id = this.state.distribute.id;
        distribute.reportId = this.state.distribute.reportId;

        let errors = validations.validate(distribute, reportValidation.distribute);

        if (validations.checkErrors(errors)) {
            r.code = "500"
            r.errors = errors;
            return r;
        }


        r.entity = distribute;

        console.log(r);

        return r;

    }

    submit = () => {

        let r = this.validateVales();
        if (r.code !== '0') {
            let msg = [];
            for (let k in r.errors) {
                msg.push(<li>{r.errors[k]}</li>)
            }
            notification.error({ message: '验证失败，无法保存', description: <ol>{msg}</ol> })
            return;
        }

        let values = r.entity;

        confirm({
            title: `您确定要提交此调佣申请么?`,
            content: '提交后不可再进行修改',
            onOk: async () => {

                await this._submit(values);
            },
            onCancel() {

            },
        });
    }

    _submit = async (values) => {
        if(!values){
            return;
        }

        this.setState({ saving: true })
        let url = `${WebApiConfig.rp.rpDisSubmit}`
        let r = await ApiClient.post(url, values, null, 'PUT');
        r = (r || {}).data;
        if (r.code === '0') {
            let newDis = r.extension;
            let d = this.state.distribute;
             d.id = newDis.id;
             d.status = newDis.status;
             d.seq = newDis.seq;
             d.updateNum = newDis.updateNum;
          
            this.setState({ distribute: { ...d } })
            notification.success({ message: '调佣已提交审核' })
            
        } else {
            notification.error({ message: '提交失败', description: r.message || '' })
        }

        this.setState({ saving: false })
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };

        let canEdit = false;
        let s = this.state.distribute.status;
        if (this.props.opType === 'add' || this.props.opType === 'edit') {
            
            if (s === 0 || s === 16) {
                canEdit = true;
            }
        }

        let statusText = examineStatusMap[s]||''

        let realCanEdit = canEdit && !this.state.saving;

        return (
            <div>
                <DistributePanel
                    nyItems={this.state.nyItems}
                    wyItems={this.state.wyItems}
                    dic={this.props.dic}
                    distribute={this.state.distribute.preDistribute}
                />
                <div className="rp-yj-tbl-title" style={{backgroundColor:'#efa8a8', marginBottom:'0.5rem'}}>本次调整后业绩分配<span style={{marginLeft:'1rem'}}>[状态：{statusText}]</span></div>
                <div>
                    <Row className="form-row">
                        <Col span={24} style={{ display: 'flex' }}>
                            <FormItem label={(<span>业主佣金</span>)}>
                                {
                                    getFieldDecorator('ownerMoney')(
                                        <InputNumber disabled={!realCanEdit} style={{ width: 200 }} onChange={this.calcZyj}></InputNumber>
                                    )
                                }
                            </FormItem>

                            <FormItem label={(<span>客户佣金</span>)}>
                                {
                                    getFieldDecorator('customMoney')(
                                        <InputNumber disabled={!realCanEdit} style={{ width: 200 }} onChange={this.calcZyj}></InputNumber>
                                    )
                                }
                            </FormItem>

                            <FormItem label={(<span>总佣金</span>)}>
                                {
                                    getFieldDecorator('yjZcjyj')(
                                        <InputNumber disabled style={{ width: 200 }} ></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={24}>
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('updateReason', {
                                        rules: [{ required: true, message: '请填写备注' }]
                                    })(
                                        <Input.TextArea rows={4} disabled={!realCanEdit}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <div className="rp-yj-tbl-title">外佣
                    {realCanEdit ? <Button style={{marginLeft:'1rem'}} onClick={this.handleAddWy}>新增外佣</Button> : null}
                    </div>
                    <Row>
                        <TradeWyTable
                            canEdit={realCanEdit}
                            onRowChanged={this.onWyRowChanged}
                            onDelRow={this.onWyDelRow}
                            dic={this.props.dic}
                            items={this.state.wyItems}
                            type="ty"
                            dataSource={this.state.outsideList} />
                    </Row>
                    <Row className="form-row" >
                        <Col span={12}>
                            <FormItem label={(<span>净佣金</span>)}>
                                {
                                    getFieldDecorator('jyj')(
                                        <Input style={{ width: 200 }} disabled={true}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <div className="rp-yj-tbl-title">内部分配</div>

                    <Row>
                        <TradeNTable
                            canEdit={realCanEdit}
                            onRowChanged={this.onNyRowChanged}
                            onDelRow={this.onNyDelRow}
                            dic={this.props.dic}
                            items={this.state.nyItems}
                            type="ty"
                            dataSource={this.state.insideList} />
                    </Row>

                    {
                        canEdit ? <Row className="rp-yj-btn-bar">
                            <Button loading={this.state.saving} type="primary" onClick={this.submit} size="large">提交申请</Button>
                        </Row> : null
                    }


                </div>

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
const WrappedRegistrationForm = Form.create()(TyPanel);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);