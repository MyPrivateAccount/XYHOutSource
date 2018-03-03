import {connect} from 'react-redux';
import {openAdjustCustomer, getDicParList, closeCustomerDetail, getCustomerAllPhone} from '../actions/actionCreator';
import React, {Component} from 'react';
import {Button, Row, Col, Icon, Tag, Dropdown, Menu, Modal} from 'antd';
import AdjustCustomer from './dialog/adjustCustomer';
import './search.less'
import moment from 'moment';
import {TAG_COLOR} from '../../constants/uiColor';

const recommendStyle = {
    button: {
        color: TAG_COLOR,
        border: '1px solid ' + TAG_COLOR,
        backgroundColor: '#ffffff',
        marginLeft: '1rem'
    }
}
const customerInfoGroup = [
    {id: 'customerInfo', name: "客户信息"},
    {id: 'businessInfo', name: "业务阶段"},
    {id: 'followInfo', name: "跟进情况"}];

class CustomerDetail extends Component {
    state = {
        showPhoneModal: false
    }
    componentWillMount() {
        this.props.dispatch(getDicParList(["FOLLOWUP_TYPE"]));
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
        let dicInfo = dicSource.find(cl => cl.value === key);
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
        console.log("trans:", trans);
        return (<Menu>
            <Menu.Item>报备项目：{trans.buildingName}-{trans.shopsName}</Menu.Item>
            <Menu.Item>业务员姓名：{trans.userTrueName}</Menu.Item>
            <Menu.Item>业务员组别：{trans.departmentName}</Menu.Item>
            <Menu.Item>业务员电话：{trans.userPhone}</Menu.Item>
            <Menu.Item>预计带看时间：{trans.expectedBeltTime}</Menu.Item>
        </Menu>)
    }
    render() {
        let customerInfo = this.props.activeCustomers.length > 0 ? (this.props.activeCustomers[0] || {}) : {};
        const activeMenu = this.props.activeMenu;
        const customerLevels = this.props.basicData.customerLevels;
        const requirementLevels = this.props.basicData.requirementLevels;
        const followUpTypes = this.props.basicData.followUpTypes;
        const requirementType = this.props.basicData.requirementType;
        const customerSource = this.props.basicData.customerSource;
        if (!customerInfo.customerDealResponse) {
            customerInfo.customerDealResponse = {};
        }
        const isShowPrice = (customerInfo.customerDemandResponse.priceStart !== 0 && customerInfo.customerDemandResponse.priceEnd !== 0);
        const isShowAcreage = (customerInfo.customerDemandResponse.acreageStart !== 0 && customerInfo.customerDemandResponse.acreageEnd !== 0);
        if (customerInfo.customerFollowUpResponse) {
            customerInfo.customerFollowUpResponse = (customerInfo.customerFollowUpResponse || []).filter(followUp => followUp.typeId != 0);
        }
        return (
            <div id="customerInfo">
                {/* <Row style={{textAlign: 'right'}}>
                    <Col>
                        <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                            {
                                customerInfoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                            }
                        </Radio.Group>
                    </Col>
                </Row> */}
                <Row>
                    <Col>
                        <h3><b>客户基本信息</b></h3>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        姓名：{customerInfo.customerName} (生日{customerInfo.birthday ? moment(customerInfo.birthday).format("YYYY-MM-DD") : "未知"})
                    </Col>
                    <Col span={12}>
                        性别：{customerInfo.sex ? "男" : "女"}
                    </Col>
                    {/* <Col span={12}>
                        业态规划：
                    </Col> */}
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
                            isShowPrice ? "价格预算：" + customerInfo.customerDemandResponse.priceStart + "-" + customerInfo.customerDemandResponse.priceEnd + "万元" : null
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
                    {/* <Col span={12}>
                        战略合作：
                    </Col> */}
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
                        客户等级：{this.getDicKey(customerLevels, customerInfo.customerDemandResponse.importance.toString())}
                    </Col>

                    {
                        customerInfo.customerDemandResponse.importance.toString() === '1' ?
                            <Col span={12}> 需求等级：{this.getDicKey(requirementLevels, customerInfo.customerDemandResponse.demandLevel.toString())}</Col>
                            : null
                    }


                </Row>
                <Row>
                    {
                        customerInfo.customerDemandResponse.importance.toString() === '1' ?
                            <Col>
                                客户需求类型：{this.getDicKey(requirementType, customerInfo.customerDemandResponse.requirementType.toString())}
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
                    activeMenu !== "menu_public_pool" && activeMenu !== "menu_invalid" ? <div>
                        <Row>
                            <Col>
                                <h3><b>客户成交信息</b></h3>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={12}>
                                成交房源：{customerInfo.source}
                            </Col>
                            <Col span={12}>
                                成交金额：{customerInfo.customerDealResponse.totalPrice}
                            </Col>
                        </Row>
                        <Row>
                            <Col span={12}>
                                成交佣金：{customerInfo.customerDealResponse.commission}
                            </Col>
                            <Col span={12}>
                                成交时间：{customerInfo.customerDealResponse.createTime}
                            </Col>
                        </Row>
                        <Row>
                            <Col span={12}>
                                是否仍有意向：{customerInfo.isSellIntention ? "是" : "否"}
                            </Col>
                        </Row>
                        <hr /></div> : null
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
                            <Col span={12}>无效类型：{this.getDicKey(this.props.basicData.invalidResions, customerInfo.customerLossResponse.lossTypeId.toString())}</Col>
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
                                    customerInfo.transactionsResponse.map(trans =>
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
                            <h3><b>跟进情况</b></h3>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            {customerInfo.customerFollowUpResponse.map((followUp, i) => {
                                followUp.followUpTime = (followUp.followUpTime === '' || followUp.followUpTime === null) ? null : followUp.followUpTime;
                                return (<div className='followUpInfo' key={followUp.followUpTime + i}>
                                    <Row>
                                        <Col><b>{moment(followUp.followUpTime).format("YYYY-MM-DD HH:mm:ss")}</b></Col>
                                    </Row>
                                    <Row>
                                        <Col>跟进方式：{this.getDicKey(followUpTypes, followUp.typeId.toString())}</Col>
                                    </Row>
                                    <Row>
                                        <Col>客户等级：{this.getDicKey(customerLevels, followUp.importance.toString())}

                                        </Col>
                                    </Row>
                                    {
                                        followUp.importance === 1 ?
                                            <Row>
                                                <Col>
                                                    需求等级：{this.getDicKey(requirementLevels, followUp.demandLevel.toString())}
                                                </Col>
                                            </Row> : null
                                    }
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
                        <Button type="primary" onClick={this.handleAdjustCustomer}>调客</Button>
                        <Button type="primary" onClick={(e) => this.props.dispatch(closeCustomerDetail())}>返回</Button>
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

            </div >
        )
    }

}

function mapStateToProps(state) {
    return {
        basicData: state.basicData,
        activeCustomers: state.search.activeCustomers,
        activeMenu: state.search.activeMenu
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(CustomerDetail);