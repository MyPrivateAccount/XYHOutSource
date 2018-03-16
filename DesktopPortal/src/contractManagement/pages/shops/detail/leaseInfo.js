import { connect } from 'react-redux';
import { editShopLease } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import moment from 'moment';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Form, Col, Row, Icon } from 'antd';
import '../edit/editCommon.less';

const { Header, Sider, Content } = Layout;
const FormItem = Form.Item;

class LeaseInfo extends Component {
    handleEdit = (e) => {
        this.props.editShopLease();
    }
    getTextByCode(dic, code) {
        let text;
        let isArray = Array.isArray(code);
        dic.map(item => {
            if (isArray) {
                if (code.find((c) => c === item.value)) {
                    text = item.key;
                }
            } else {
                if (item.value === code) {
                    text = item.key;
                }
            }
        });
        return text;
    }
    render() {
        let leaseInfo = this.props.leaseInfo
        let startDate = leaseInfo.startDate ? moment(leaseInfo.startDate).format('YYYY-MM-DD') : "?";
        let endDate = leaseInfo.endDate ? moment(leaseInfo.endDate).format('YYYY-MM-DD') : "?";
        return (
            <div className="relative">
                <Layout>
                    <Content className='' style={{ padding: '25px 0', marginTop: '25px', backgroundColor: "#ECECEC" }}>

                        <Form layout="horizontal" >
                            <Row type="flex" style={{ padding: '1rem 0' }}>
                                <Col span={20} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>租约信息</span>
                                </Col>
                                <Col span={4}>
                                    {
                                        [1, 8].includes(this.props.shopInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    }
                                </Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12} >
                                    当前经营：<span className='boldSpan'>{leaseInfo.currentOperation}</span>
                                </Col>
                                <Col span={12}>起止时间：<span className='boldSpan'>{startDate} ~ {endDate}</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>租金：<span className='redSpan'>{leaseInfo.rental} 元/月</span></Col>
                                <Col span={12}>押金：<span className='redSpan'>{leaseInfo.deposit} 元</span></Col>
                            </Row>
                            {leaseInfo.hasLeaseback ?
                                <Row className='viewRow'>
                                    {leaseInfo.backRate ?
                                        <Col span={12}>返祖比列：<span className='boldSpan'>{leaseInfo.backRate} %</span></Col>
                                        : null
                                    }
                                    <Col span={12}>返祖时间: <span className='boldSpan'>{leaseInfo.backMonth} 月</span></Col>
                                </Row>
                                : null
                            }

                            <Row className='viewRow'>
                                <Col span={12}>递增比率：<span className='boldSpan'>{leaseInfo.upscale} %</span></Col>
                                <Col span={12}>支付方式：<span className='boldSpan'>{this.getTextByCode(this.props.basicData.shopLease, leaseInfo.paymentTime)}</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24}>
                                    {
                                        leaseInfo.backRate !== null ? <span>备注：<span className='boldSpan'>{leaseInfo.backRate}</span></span> : null
                                    }
                                </Col>
                            </Row>
                        </Form>
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    // console.log(state.shop.shopsInfo.leaseInfo, state.basicData, 66)
    return {
        shopInfo: state.shop.shopsInfo,
        leaseInfo: state.shop.shopsInfo.leaseInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        editShopLease: () => dispatch(editShopLease())
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(LeaseInfo);