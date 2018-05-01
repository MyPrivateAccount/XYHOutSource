import {connect} from 'react-redux';
import {openAdjustCustomer, getCustomerDetail, closeCustomerDetail, getCustomerAllPhone, showCustomerlossModal, customerlossActive, setLoadingVisible} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Icon, Tag, Dropdown, Menu, Modal, notification, Spin} from 'antd';
import AdjustCustomer from './dialog/adjustCustomer';
import CustomerLoss from './dialog/customerloss';
import './search.less'
import moment from 'moment';
import {TAG_COLOR} from '../../constants/uiColor';
import AddFollow from './dialog/addFollow';
import {getDicPars} from '../../utils/utils';

const recommendStyle = {
    button: {
        color: TAG_COLOR,
        border: '1px solid ' + TAG_COLOR,
        backgroundColor: '#ffffff',
        marginLeft: '1rem'
    },
    prevBtn: {
        float: 'right',
        border: '0',
        color: TAG_COLOR
    }
}

class CustomerDetail extends Component {
    state = {
        showPhoneModal: false,
        showAddFollowDialog: false,
        dicSetting: {//字典配置
            customerSource: [],
            businessTypes: [],
            customerLevels: [],
            requirementLevels: [],
            invalidResions: [],
            requirementType: [],
            followUpTypes: [],
            leaseTypes: []
        }
    }

    componentWillReceiveProps(newProps) {
        //提取字典
        let rootBasicData = this.props.rootBasicData || {};
        let dicSetting = this.state.dicSetting;
        if (this.state.dicSetting.customerSource) {
            dicSetting.customerSource = getDicPars("ZYW_CUSTOMER_SOURCE", rootBasicData.dicList);
            dicSetting.customerLevels = getDicPars("CUSTOMER_LEVEL", rootBasicData.dicList);
            dicSetting.requirementLevels = getDicPars("REQUIREMENT_LEVEL", rootBasicData.dicList);
            dicSetting.invalidResions = getDicPars("ZYW_INVALID_REASON", rootBasicData.dicList);
            dicSetting.requirementType = getDicPars("REQUIREMENT_TYPE", rootBasicData.dicList);
            dicSetting.followUpTypes = getDicPars("FOLLOWUP_TYPE", rootBasicData.dicList);
            dicSetting.leaseTypes = getDicPars("LEASE_TYPE", rootBasicData.dicList);
        }
    }

    componentWillMount() {
        // this.props.dispatch(getDicParList(["FOLLOWUP_TYPE"]));
    }
    handleAnchorChange = (e) => {
        // console.log(e);
        // window.location.href = "#" + e.target.value;//FOLLOWUP_TYPE
    }
    handleAdjustCustomer = (e) => {
        this.props.dispatch(openAdjustCustomer());
    }
    //获取字典值
    getDicKey(dicSource, key) {
        let text = key;
        let dicInfo = dicSource.find(cl => cl.value == key);
        if (dicInfo) {
            text = dicInfo.key;
        }
        return text;
    }
    handleGetAllPhone = (e) => {
        let activeCustomer = this.props.activeCustomers.length > 0 ? this.props.activeCustomers[0] : {};
        if (activeCustomer.id && !activeCustomer.phoneList) {
            this.props.dispatch(getCustomerAllPhone(activeCustomer.id));
        }
        this.setState({showPhoneModal: true})
    }

    //获取报备单
    getReportDetail(trans) {
        return (<Menu>
            <Menu.Item>报备项目：{trans.buildingName}-{trans.shopsName}</Menu.Item>
            <Menu.Item>业务员姓名：{trans.userTrueName}</Menu.Item>
            <Menu.Item>业务员组别：{trans.departmentName}</Menu.Item>
            <Menu.Item>业务员电话：{trans.userPhone}</Menu.Item>
            <Menu.Item>预计带看时间：{trans.expectedBeltTime ? moment(trans.expectedBeltTime).format("YYYY-MM-DD HH:mm:ss") : null}</Menu.Item>
            <Menu.Item>业态：</Menu.Item>
        </Menu>)
    }

    closeAddFollowDialog = () => {
        let showAddFollow = this.state.showAddFollowDialog;
        this.setState({showAddFollowDialog: !showAddFollow});
    }
    //获取客源详情
    getCustomerDetail = (record) => {
        this.props.dispatch(getCustomerDetail(record));
    }

    //上下页切换
    handleChangePrev = (type) => {
        let customerList = (this.props.searchResult || {}).extension || [];
        let customerInfo = this.props.activeCustomers.length > 0 ? (this.props.activeCustomers[0] || {}) : {};
        let curIndex = customerList.findIndex(c => c.id === customerInfo.id);
        if (curIndex === -1) {
            return;
        }
        if (type === "prev") {
            if (curIndex <= 0) {
                notification.info({
                    description: "已经是第一个",
                    duration: 3
                });
                return;
            }
            curIndex--;
        } else if (type === "next") {
            if (curIndex >= (customerList.length - 1)) {
                notification.info({
                    description: "已经是最后一个",
                    duration: 3
                });
                return;
            }
            curIndex++;
        }
        this.props.dispatch(setLoadingVisible(true));
        this.props.dispatch(getCustomerDetail(customerList[curIndex]));
    }

    render() {
        let customerInfo = this.props.activeCustomers.length > 0 ? (this.props.activeCustomers[0] || {}) : {};
        customerInfo.type = 'detail'
        const activeMenu = this.props.activeMenu;
        let {customerLevels, requirementType, followUpTypes, customerSource, requirementLevels, leaseTypes, invalidResions} = this.state.dicSetting;

        let customerDealResponses = customerInfo.customerDealResponses || []
        if (!customerInfo.customerDealResponse) {
            customerInfo.customerDealResponse = {};
        }
        if (!customerInfo.customerLossResponse) {
            customerInfo.customerLossResponse = {};
        }
        const isShowPrice = (customerInfo.customerDemandResponse && customerInfo.customerDemandResponse.priceStart !== 0 && customerInfo.customerDemandResponse.priceEnd !== 0);
        const isShowAcreage = (customerInfo.customerDemandResponse && customerInfo.customerDemandResponse.acreageStart !== 0 && customerInfo.customerDemandResponse.acreageEnd !== 0);
        customerInfo.customerDemandResponse = customerInfo.customerDemandResponse || {};
        if (customerInfo.customerFollowUpResponse) {
            customerInfo.customerFollowUpResponse = (customerInfo.customerFollowUpResponse || []);//.filter(followUp => followUp.typeId != 0);
        }
        const customerDemandResponse = customerInfo.customerDemandResponse || {}
        console.log(customerDemandResponse)
        return (
            <div id="customerInfo">
                <Spin spinning={this.props.showLoading}>
                    <Row>
                        <Col span={12}>
                            <h3><b>客户基本信息</b></h3>
                        </Col>
                        <Col span={12}>
                            <Button style={recommendStyle.prevBtn} onClick={() => this.handleChangePrev('next')}>下一客户</Button>
                            <Button style={recommendStyle.prevBtn} onClick={() => this.handleChangePrev('prev')}>上一客户</Button>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            姓名：{customerInfo.customerName} (生日{customerInfo.birthday ? moment(customerInfo.birthday).format("YYYY-MM-DD") : "未知"})
                    </Col>
                        <Col span={12}>
                            性别：{customerInfo.sex ? "男" : "女"}
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            电话号码：{customerInfo.mainPhone}
                            <Tag size='small' onClick={this.handleGetAllPhone} style={recommendStyle.button} >获取号码详情</Tag>
                        </Col>
                        <Col span={12}>
                            电子邮件：{customerInfo.email}
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            QQ：{customerInfo.qq}
                        </Col>
                        <Col span={12}>
                            微信：{customerInfo.weChat}
                        </Col>
                    </Row>
                    <Row style={{display: (isShowPrice || isShowAcreage) ? "block" : "none"}}>
                        <Col span={12}>
                            {
                                isShowPrice ? "价格预算：" + customerInfo.customerDemandResponse.priceStart + "-" + customerInfo.customerDemandResponse.priceEnd + "元/㎡/月" : null
                            }

                        </Col>
                        {/* <Col span={12}>
                        经营品牌：
                    </Col> */}
                        <Col span={12}>
                            {
                                isShowAcreage ? "面积：" + customerInfo.customerDemandResponse.acreageStart + "-" + customerInfo.customerDemandResponse.acreageEnd + "㎡" : null
                            }
                        </Col>
                        {/* <Col span={12}>
                        经营类型：
                    </Col> */}
                    </Row>
                    <Row>
                        <Col span={12}>
                            意向区域：{customerInfo.customerDemandResponse.areaFullName}
                        </Col>
                        <Col span={12}>
                            意向业态：{customerDemandResponse.businessPlanningName ? customerDemandResponse.businessPlanningName : '无'}
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            客户来源：{this.getDicKey(customerSource, customerInfo.source)}
                        </Col>
                        <Col span={12}>
                            录入时间：{moment(customerInfo.createTime).format("YYYY-MM-DD HH:mm:ss")}
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <h3><b>客户等级信息</b></h3>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            客户等级：{this.getDicKey(customerLevels, customerInfo.customerDemandResponse.importance)}
                        </Col>

                        {
                            customerInfo.customerDemandResponse.importance == '1' ?
                                <Col span={12}> 需求等级：{this.getDicKey(requirementLevels, customerInfo.customerDemandResponse.demandLevel)}</Col>
                                : null
                        }


                    </Row>
                    <Row>
                        {
                            customerInfo.customerDemandResponse.importance === '1' ?
                                <Col>
                                    客户需求类型：{this.getDicKey(requirementType, customerInfo.customerDemandResponse.requirementType)}
                                </Col> : null
                        }
                    </Row>
                    <Row>
                        <Col>
                            <h3><b>客户归属信息</b></h3>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            归属部门：{customerInfo.departmentName}
                        </Col>
                        <Col span={12}>
                            业务员：{customerInfo.userName}
                        </Col>
                    </Row>
                    {
                        //成交信息
                        // activeMenu !== "menu_public_pool" && activeMenu !== "menu_invalid" ? 
                        <div>
                            <Row>
                                <Col>
                                    <h3><b>客户成交信息</b></h3>
                                </Col>
                            </Row>
                            {
                                customerDealResponses.map((v, index) => {
                                    return (
                                        v.sellerType === "1" ?
                                            <div key={index} style={{borderBottom: '1px dashed #dcdcdc', padding: '5px 0'}}>
                                                <Row>
                                                    <Col>
                                                        租赁类型：{(leaseTypes.find(t => t.value === "1") || {}).key}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        成交房源：{v.buildingName} {v.shopName}
                                                    </Col>
                                                    <Col span={12}>
                                                        成交专员：{v.userName || ''}  ({v.departmentName || ''})
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        专员电话：{v.userPhone || ''}
                                                    </Col>
                                                    <Col span={12}>
                                                        客户：{v.customerName || ''}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        客户电话：{v.customerName || ''}
                                                    </Col>
                                                    <Col span={12}>

                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col>
                                                        其他信息
                                                </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        当前经营：{v.currentOperation}
                                                    </Col>
                                                    <Col span={12}>
                                                        开始时间：{v.startDate ? moment(v.startDate).format("YYYY-MM-DD HH:mm:ss") : null}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        结束时间：{v.endDate ? moment(v.endDate).format('YYYY-MM-DD HH:mm') : null}
                                                    </Col>
                                                    <Col span={12}>
                                                        租金：{v.rental}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        支付方式：{v.methodPayment}
                                                    </Col>
                                                    <Col span={12}>
                                                        押金：{v.deposit}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        押金方式：{v.depositType}
                                                    </Col>
                                                    <Col span={12}>
                                                        递增方式：{v.incrementally}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        返租时间：{v.backMonth}
                                                    </Col>
                                                    <Col span={12}>
                                                        返租比例：{v.backRate}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        转让费：{v.transfer}
                                                    </Col>
                                                    <Col span={12}>
                                                        佣金：{v.commission}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        成交日期：{moment(v.createTime).format('YYYY-MM-DD HH:mm')}
                                                    </Col>
                                                </Row>
                                            </div> : <div key={index} style={{borderBottom: '1px dashed #dcdcdc', padding: '5px 0'}}>
                                                <Row>
                                                    <Col>
                                                        租赁类型：{(leaseTypes.find(t => t.value === "2") || {}).key}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        分租商：{v.seller}
                                                    </Col>
                                                    <Col span={12}>
                                                        成交房源：{v.buildingName} {v.shopName}
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col span={12}>
                                                        成交人：{v.salesman}
                                                    </Col>
                                                    <Col span={12}>
                                                        成交日期：{moment(v.createTime).format('YYYY-MM-DD HH:mm')}
                                                    </Col>
                                                </Row>
                                            </div>
                                    )
                                })
                            }
                        </div>
                        // : null
                    }
                    {
                        //无效信息
                        activeMenu === "menu_invalid" ? <div>
                            <Row>
                                <Col>
                                    <h3><b>客户无效信息</b></h3>
                                </Col>
                            </Row>
                            <Row>
                                <Col span={12}>无效类型：{this.getDicKey(invalidResions, customerInfo.customerLossResponse.lossTypeId)}</Col>
                                <Col span={12}>无效原因：{customerInfo.customerLossResponse.lossRemark}</Col>
                            </Row>
                        </div> : null
                    }

                    {
                        //业务阶段
                        activeMenu !== "menu_public_pool" ? <div>
                            <Row>
                                <Col>
                                    <h3><b>业务阶段</b></h3>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    {
                                        (customerInfo.transactionsResponse || []).map(trans =>
                                            <div key={trans.id}>
                                                <Row>
                                                    <Col span={6}>{trans.buildingName}->{trans.shopsName}</Col>  <Col span={6}><Dropdown overlay={this.getReportDetail(trans)}><span style={{cursor: 'pointer', color: '#64bce3'}}>查看报备单</span></Dropdown></Col>
                                                </Row>
                                                <Row>
                                                    <Col>
                                                        {trans.transactionsFollowUpResponse.map(followUp => {
                                                            followUp.markTime = followUp.markTime || followUp.createTime;
                                                            return (<Row key={followUp.id}>
                                                                <Col>
                                                                    {moment(followUp.markTime).format("YYYY-MM-DD HH:mm:ss")}  {followUp.contents}
                                                                </Col>
                                                            </Row>)
                                                        })}
                                                    </Col>
                                                </Row>
                                            </div>)
                                    }
                                </Col>
                            </Row>
                            <hr />
                        </div> : null
                    }
                    {activeMenu !== "menu_public_pool" ? <div>
                        <Row>
                            <Col>
                                <h3><b>跟进情况</b> {customerInfo.customerStatus === 6 ? null : <Button type="primary" onClick={() => this.setState({showAddFollowDialog: true})}>写跟进</Button>}</h3>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                {(customerInfo.customerFollowUpResponse || []).map((followUp, i) => {
                                    followUp.followUpTime = (followUp.followUpTime === '' || followUp.followUpTime === null) ? null : followUp.followUpTime;
                                    return (<div className='followUpInfo' key={followUp.followUpTime + i}>
                                        <Row>
                                            <Col><b>{moment(followUp.followUpTime).format("YYYY-MM-DD HH:mm:ss")}</b></Col>
                                        </Row>
                                        <Row>
                                            {this.getDicKey(followUpTypes, followUp.followMode) != followUp.followMode ? <Col>跟进方式：{this.getDicKey(followUpTypes, followUp.followMode)}</Col> : null}
                                        </Row>
                                        <Row>
                                            {this.getDicKey(customerLevels, followUp.importance) != followUp.importance ? <Col>客户等级：{this.getDicKey(customerLevels, followUp.importance)}</Col> : null}
                                        </Row>
                                        {
                                            followUp.importance === 1 && followUp.demandLevel != this.getDicKey(requirementLevels, followUp.demandLevel) ?
                                                <Row>
                                                    <Col>
                                                        需求等级：{this.getDicKey(requirementLevels, followUp.demandLevel)}
                                                    </Col>
                                                </Row> : null
                                        }
                                        <Row>
                                            <Col>跟进人：{followUp.userTrueName}</Col>
                                        </Row>
                                        <Row>
                                            <Col>备注：{followUp.followUpContents}</Col>
                                        </Row>
                                    </div>)
                                })}
                            </Col>
                        </Row>
                        <hr />
                    </div> : null}

                    <Row>
                        <Col style={{textAlign: "center"}}>
                            {
                                activeMenu === "menu_invalid" ? null :
                                    <Button type="primary" onClick={this.handleAdjustCustomer} disabled={customerInfo.customerStatus === 6}>调客</Button>
                            }
                            {
                                activeMenu === "menu_invalid" ?
                                    <Button type="primary"
                                        onClick={() => this.props.dispatch(customerlossActive({
                                            customerId: customerInfo.id,
                                            isDeleteOldData: false,
                                            type: 'detail'
                                        }))}>激活</Button>
                                    :
                                    <Button type="primary"
                                        onClick={() => this.props.dispatch(showCustomerlossModal(customerInfo))}>拉失效</Button>
                            }
                            <Button type="primary" onClick={(e) => this.props.dispatch(closeCustomerDetail())}>返回</Button>
                        </Col>
                        <Col>
                            <Button style={recommendStyle.prevBtn} onClick={() => this.handleChangePrev('next')}>下一客户</Button>
                            <Button style={recommendStyle.prevBtn} onClick={() => this.handleChangePrev('prev')}>上一客户</Button>
                        </Col>
                    </Row>
                    <AdjustCustomer />
                    <Modal
                        visible={this.state.showPhoneModal}
                        title="客户号码详情" maskClosable
                        onCancel={() => this.setState({showPhoneModal: false})}
                        footer={[<Button key="back" onClick={() => this.setState({showPhoneModal: false})}>关闭</Button>]}>

                        <Row><Col span={8}>电话号码</Col><Col span={8}>创建时间</Col><Col span={8}>失效时间</Col></Row>
                        {customerInfo.phoneList !== undefined ? customerInfo.phoneList.map((phoneInfo, i) => <Row key={"phone" + i}>
                            <Col span={8}>{phoneInfo.phone}{phoneInfo.isMain ? <Icon type="environment" style={{marginLeft: '5px'}} /> : null}</Col>
                            <Col span={8}>{phoneInfo.createTime ? moment(phoneInfo.createTime).format("YYYY-MM-DD HH:mm:ss") : null}</Col>
                            <Col span={8}>{phoneInfo.deleteTime ? moment(phoneInfo.deleteTime).format("YYYY-MM-DD HH:mm:ss") : null}</Col>
                        </Row>) : null}
                    </Modal>
                    <CustomerLoss />
                    <AddFollow visible={this.state.showAddFollowDialog} dicInfo={this.props.basicData} customerInfo={customerInfo} closeDialog={this.closeAddFollowDialog} reloadDetail={this.getCustomerDetail} />
                </Spin>
            </div >
        )
    }

}

function mapStateToProps(state) {
    return {
        rootBasicData: state.rootBasicData,
        activeCustomers: state.search.activeCustomers,
        activeMenu: state.search.activeMenu,
        searchResult: state.search.searchResult,
        showLoading: state.search.showLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(CustomerDetail);