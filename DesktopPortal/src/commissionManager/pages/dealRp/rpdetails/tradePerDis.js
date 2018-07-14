//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { DatePicker, Form, Layout, Button, Row, Col, Input, Spin, InputNumber } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'
import SfkTable from '../sfk/sfkTable'
import uuid from 'uuid'
import reportValidation from '../../../constants/reportValidation'
import validations from '../../../../utils/validations'

const FormItem = Form.Item;
class TradePerDis extends Component {

    constructor(props) {
        super(props);
        this.state = {
            isDataLoading: false,
            outsideList: [],
            insideList: [],
            totalyj: 0,
            yjKhyjdqr: '',
            yjYzyjdqr: '',
            yjFtItems: [],//业绩分摊项
            humanList: [],//员工列表
        }

    }

    componentWillMount = () => {

    }
    componentDidMount = () => {
        this.initEntity(this.props);
    }
    componentWillReceiveProps = (nextProps) => {
        if (this.props.entity !== nextProps.entity && nextProps.entity) {

            this.initEntity(nextProps)
        }
    }

    initEntity = (nextProps) => {
        var entity = nextProps.entity;
        if (!entity) {
            return;
        }

        let mv = {};
        Object.keys(entity).map(key => {
            mv[key] = entity[key];
        })

        let il = entity.reportInsides || [];
        il.forEach(item => {
            item.uid = [{ key: item.uid, label: item.username }];
        })

        this.setState({ outsideList: entity.reportOutsides || [], insideList: il })
        this.props.form.setFieldsValue(mv);
    }


    //添加内佣项目
    handleAddNbFp = () => {
        let item = {
            id: uuid.v1(),
            type: null,
            remark: '',
            uid: null,
            money: 0,
            odd_num: null,
            errors: {

            }
        }

        item.errors = validations.validate(item, reportValidation.nrItem)

        let nyList = [...this.state.insideList, item]
        this.setState({ insideList: nyList })
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

        let jyj = this.props.form.getFieldValue("yjJyj") || 0;
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
            ny = Math.round(ny * 100) / 100;
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
    //删除内佣项
    onNyDelRow = (row) => {
        let ol = this.state.insideList;
        let idx = ol.findIndex(x => x.id === row.id);
        if (idx < 0) {
            return;
        }
        ol.splice(idx, 1);
        this.setState({ insideList: [...ol] }, () => {

        })
    }

    //计算业绩浄佣金
    _calcYjJyj = () => {
        let zyj = (this.props.form.getFieldValue("yjZcjyj") || '0') * 1;
        //扣减外佣
        let wyJe = 0;
        this.state.outsideList.forEach(item => {
            wyJe = wyJe + (item.money * 1)
            wyJe = Math.round(wyJe * 100) / 100;
        })
        let jyj = zyj - wyJe

        this.props.form.setFieldsValue({ yjJyj: jyj })


        let il = this.state.insideList;
        let nyJe = 0;
        let lastRow = null;
        // let nyItems = this.props.nyItems;
        il.forEach(item => {
            // let x = nyItems.find(x=>x.code === item.type);       
            item.money = Math.round((jyj * (item.percent || 0))) / 100;
            nyJe = nyJe + item.money;
            nyJe = Math.round(nyJe * 100) / 100;
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
            var yj = this.props.form.getFieldsValue(["yjYzys", "yjKhys"]);
            var zyj = (yj.yjYzys || 0) * 1 + (yj.yjKhys || 0) * 1;
            this.props.form.setFieldsValue({ yjZcjyj: zyj })

            //更新外部分摊项
            let ol = this.state.outsideList;
            let wyItems = this.props.wyItems;
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

    getValues = () => {
        let vals = this.props.form.getFieldsValue();
        vals.reportInsides = this.state.insideList.map(x => {
            let item = { ...x }
            item.uid = x.uid[0].key;
            item.username = x.uid[0].label;
            return item;
        })
        vals.reportOutsides = this.state.outsideList.map(x => ({ ...x }))
        return vals;
    }

    render() {
        const { getFieldDecorator, getFieldValue } = this.props.form;
        const { report, entity } = this.props;

        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 },
        };

        let yjTotal = report.ycjyj;
        let zyj = (getFieldValue("yjZcjyj") || '0') * 1;
        const canEdit = this.props.canEdit;
        let factgetList = report.factGetList||[];


        return (
            <Layout>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>业主应收</span>)}>
                                {
                                    getFieldDecorator('yjYzys')(
                                        <InputNumber disabled={!canEdit} style={{ width: 200 }} onChange={this.calcZyj}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>业主佣金到期日</span>)}>
                                {
                                    getFieldDecorator('yjYzyjdqr', {
                                        rules: [{ required: false, message: '请选择成交日期!' }]
                                    })(
                                        <DatePicker disabled={true} style={{ width: 200 }} ></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>客户应收</span>)}>
                                {
                                    getFieldDecorator('yjKhys')(
                                        <InputNumber disabled={!canEdit} style={{ width: 200 }} onChange={this.calcZyj}></InputNumber>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout2} label={(<span>客户佣金到期日</span>)}>
                                {
                                    getFieldDecorator('yjKhyjdqr', {
                                        rules: [{ required: false, message: '请选择成交日期!' }]
                                    })(
                                        <DatePicker disabled={true} style={{ width: 200 }} ></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row className="form-row">
                        <Col span={24}>
                            <FormItem {...formItemLayout} style={{ width: '25rem' }} hasFeedback validateStatus={yjTotal === zyj ? '' : 'error'} label={(<span>总成交佣金</span>)}>
                                {
                                    getFieldDecorator('yjZcjyj')(
                                        <Input disabled={true}></Input>
                                    )
                                }

                            </FormItem>
                        </Col>
                    </Row>

                    {
                        canEdit ? null :
                            <div>
                                <div className="divider"></div>
                                <Row>
                                    <span className="rp-yj">业主佣金：</span><span className="rp-yj-je"> {entity.yjYzys || 0} </span>
                                    <span className="rp-yj">已收取：</span><span className="rp-yj-je"> {entity.yjYsyy || 0} </span>
                                    <span className="rp-yj">余额：</span><span className="rp-yj-je"> {entity.yjYyyk || 0} </span>

                                </Row>
                                <Row>
                                    <span className="rp-yj">客户佣金：</span><span className="rp-yj-je"> {entity.yjKhys || 0} </span>
                                    <span className="rp-yj">已收取：</span><span className="rp-yj-je"> {entity.yjYsky || 0} </span>
                                    <span className="rp-yj">余额：</span><span className="rp-yj-je"> {entity.yjKyyk || 0} </span>

                                </Row>
                                <div className="rp-yj-tbl-title">收付款明细</div>
                                <SfkTable view={this.props.viewFactGet} list={factgetList} wyItems = {this.props.wyItems} />
                                <div className="divider"></div>
                            </div>
                    }

                    <div className="rp-yj-tbl-title">外佣
                    {canEdit ? <Button style={{ marginLeft: '2rem' }} onClick={this.handleAddWy}>新增外佣</Button> : null}</div>

                    <Row>
                        <TradeWyTable canEdit={canEdit} onRowChanged={this.onWyRowChanged} onDelRow={this.onWyDelRow} dic={this.props.dic} items={this.props.wyItems} dataSource={this.state.outsideList} />
                    </Row>
                    <Row className="form-row" >
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>净佣金</span>)}>
                                {
                                    getFieldDecorator('yjJyj', {
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 200 }} disabled={true}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <div className="rp-yj-tbl-title">内部分配
                    {canEdit ? <Button style={{ marginLeft: '2rem' }} onClick={this.handleAddNbFp}>新增内部分配</Button> : null}</div>

                    <Row>
                        <TradeNTable canEdit={canEdit}
                            onRowChanged={this.onNyRowChanged}
                            onDelRow={this.onNyDelRow}
                            dic={this.props.dic}
                            items={this.props.nyItems}
                            dataSource={this.state.insideList} />
                    </Row>
                    {/* <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row> */}
                </Spin>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        // basicData: state.base,
        // operInfo: state.rp.operInfo,
        // ext: state.rp.ext,
        // acmOperInfo: state.acm.operInfo,
        // syncData: state.rp.syncData,
        // syncFpOp: state.rp.syncFpOp,
        // syncFpData: state.rp.syncFpData,
        // scaleSearchResult:state.acm.scaleSearchResult,
        // humanList:state.org.humanList,
        // humanOperInfo:state.org.operInfo

    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradePerDis);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
