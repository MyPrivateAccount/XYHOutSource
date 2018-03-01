import { connect } from 'react-redux';
import {  getMyCustomerInfo, comfirmAsync, lookAsync, reportAsync, 
    showEditModal,showBatchReportModal,showTemplateModal,getReportCustomerDeal,
    SearchValphone, getUserTypeValue } from '../actions/actionCreator';
import React, { Component } from 'react'
import './index.less'
import { Layout, Table, Button, Checkbox, Row, Col, Tabs, Radio, Input, Menu, Select,Spin,Modal,Pagination,notification,Popconfirm,Popover} from 'antd'
import moment from 'moment'
// import { CopyToClipboard } from 'react-copy-to-clipboard'
import ClipboardButton from 'react-clipboard.js'
import XyhDealDialog from './dialog/xyhDealInfo'
import BatchReport from './dialog/batchReport'
import TemplateDialog from './dialog/template'
import { getReport } from './buildingDish/edit/utils'
import WebApiConfig from '../constants/webapiConfig'
import ApiClient from '../../utils/apiClient'

const { Header, Sider, Content } = Layout;
const TabPane = Tabs.TabPane;
const SubMenu = Menu.SubMenu;
const MenuItemGroup = Menu.ItemGroup;
const Option = Select.Option;

class HouseResourceIndex extends Component {
    state = {
        vaiPhoneInfo: [],
        copied: false,
        value: '', // input搜索框值
        placeholder: '业务员',
        buildingId: '',
        current: 'all',
        visible: false,
        pageIndex: 0,
        index: -1,
        popVisibleIndex: -1,
        myLines: '',
        customerDeal: [],
        columns: [
            { title: '客户姓名', dataIndex: 'customerName', key: 'customerName', width: 110 },
            { title: '客户电话', dataIndex: 'mainPhone', key: 'mainPhone', width: 110 },
            { title: '业务员姓名', dataIndex: 'userTrueName', key: 'userTrueName', width: 110 },
            { title: '业务员组别', dataIndex: 'departmentName', key: 'departmentName', width: 150},
            { title: '业务员电话', dataIndex: 'userPhone', key: 'userPhone', width: 110 },
            { title: '预计带看时间', dataIndex: 'expectedBeltTime', key: 'expectedBeltTime',width: 120},
            { title: '状态', dataIndex: 'transactionsStatus', key: 'transactionsStatus', width: 110},
            { title: '操作', dataIndex: 'opreation', key: 'opreation', width: 120,
                render: (text, record, index) => {
                    let getSpan = (record) => {
                        switch (record.transactionsStatus) {
                            case "等待驻场确认": return '确认报备'
                            case "等待驻场报备": return '已报备'
                            case "等待业务员带看": return '确认带看'
                            case "等待业主成交": return '确认成交'
                            case '业主已成交': return '查看成交信息'
                            default: return ''
                        }
                    }
                    let isDisabled = (record) => {
                        if (moment(record.expectedBeltTime).format('YYYY-MM-DD') !== moment().format('YYYY-MM-DD')) {
                            return true
                        }
                        return false
                    }
                   let span = getSpan(record);
                   let disabled = isDisabled(record);
                   const popTitle = (
                        <p>
                        <p>此组客户尚未进行复制操作，</p>
                        <p>确定后将无法再进行复制发送，</p>
                        <p>请确认是否继续？</p>
                        </p>
                    )
                    let content, customerDeal = this.props.reportCustomerDeal
                    if (customerDeal.length === 0) {
                      content = (
                        <div>暂无成交信息</div>
                      )
                    } else {
                      content = (
                        <div className='customerDealContent'>
                          {
                            customerDeal.sellerType === 1 ? 
                            <p>
                            <p>楼盘名： <span>{customerDeal.buildingName}</span></p>
                            <p>商铺： <span>{customerDeal.shopName}</span></p>
                            <p>销售类型： <span>自售</span></p>
                            <p>组别： <span>{customerDeal.departmentName}</span></p>
                            <p>业务员： <span>{customerDeal.userName}</span></p>
                            <p>业务员电话：<span>{customerDeal.userPhone}</span></p>
                            <p>客户： <span>{customerDeal.customerName}</span></p>
                            <p>客户电话： <span>{customerDeal.customerPhone}</span></p>
                            <p>佣金： <span>{customerDeal.commission}</span></p>
                            <p>成交总价： <span>{customerDeal.totalPrice}</span></p>
                            <p>成交日期： <span>{moment(customerDeal.createTime).format('YYYY-MM-DD')}</span></p>
                            </p>
                            : 
                            <p>
                            <p>楼盘名： <span>{customerDeal.buildingName}</span></p>
                            <p>商铺： <span>{customerDeal.shopName}</span></p>
                            <p>销售类型： <span>第三方</span></p>
                            <p>分销商： <span>{customerDeal.seller}</span></p>
                            <p>客户： <span>{customerDeal.proprietor}</span></p>
                            <p>客户电话： <span>{customerDeal.mobile}</span></p>
                            <p>身份证： <span>{customerDeal.idcard}</span></p>
                            <p>居住地： <span>{customerDeal.address}</span></p>
                            <p>成交日期： <span>{moment(customerDeal.createTime).format('YYYY-MM-DD')}</span></p>
                            </p>
                        }
                        </div>
                      )
                    }
                   return (
                    span === '已报备' ?
                    <div>
                        {span === '已报备' ? 
                        // <CopyToClipboard
                        //     onCopy={() => this.onCopy(record, index)}
                        //     text={this.state.myLines}>
                        //     <Button type="danger" disabled={disabled} size='small' style={{marginRight: '20px'}}>复制</Button>
                        // </CopyToClipboard>

                        <ClipboardButton component="span" 
                        option-text={() => this.onCopy(record, index)} 
                        onSuccess={() => this.copySuccess(record)}>
                                <Button type="danger" disabled={disabled} size='small' style={{marginRight: '20px'}}>复制</Button>
                        </ClipboardButton>
                        : null}
                         <Popconfirm title={popTitle} okText="确定" cancelText="取消"
                                     visible={this.state.popVisibleIndex === index} 
                                     onConfirm={() => this.popConfirm(record, index)} 
                                     onCancel={this.popCancel}>
                                    <Button size='small' type='primary' disabled={disabled}
                                            onClick={() => this.comfirmThisData({id: record.id, transactionsStatus: record.transactionsStatus}, index)}>
                                    {span}</Button>
                        </Popconfirm>
                    </div> 
                    :
                    <div>
                        {  
                            record.transactionsStatus === '业主已成交' ? 
                                <Popover content={content} title="成交信息" trigger="click">
                                    <Button size='small' type='primary' 
                                    onClick={() => this.comfirmThisData(
                                        {id: record.id, 
                                        transactionsStatus: record.transactionsStatus, 
                                        record: record}, index)}>{span}</Button> 
                                </Popover>
                            :
                            record.transactionsStatus === '已失效' ?  null :
                            <Button size='small' type='primary' onClick={() => this.comfirmThisData({id: record.id, transactionsStatus: record.transactionsStatus, record: record}, index)}>{span}</Button>
                        }
                    </div>
                   )
                    
            }},
        ]
    }
    popConfirm = (row, index) => {  // pop 弹框确认
        this.setState({ 
            popVisibleIndex: -1 
        }, () => this.props.report({
            transactionsids: [row.id],
            buildingId: this.state.buildingId,
            type: this.state.current
        }));
    }
    popCancel = () => {
        this.setState({ popVisibleIndex: -1 });
    }

    onCopy = (row, index) => { // 点击复制
        let { buildingInfo } = this.props
        let valPhone = (this.props.buildingInfo.ruleInfo || {}).isCompletenessPhone
        if (this.props.showTemplateInfo.isChecked) {
            if (!(this.state.index === -1 || this.state.index === index)) {
                notification.warning({
                    message: `请先进行“确认报备”操作！`,
                    duration: 3,
                    })
                return
            }
            this.setState({index: index});
            let list = [row]
            let ruleInfo = buildingInfo.ruleInfo || {}
            let reportedTemplate = ruleInfo.reportedTemplate 
            let template = [];
            if (reportedTemplate) {
              template = JSON.parse(reportedTemplate);
            }
            if(valPhone) {
                list[0].mainPhone=list[0].truePhone
            } 
            let line = getReport(template, list[0], this.props.user, this.props.buildInfo || {})
            // console.log(template, list[0], this.props.user, this.props.buildInfo, 111)
            // console.log(line, 7)
            return line
         } else {
             this.props.showTemplateModal()
             this.setState({index: index});
         }
    };
    copySuccess = (row) => {
        notification.success({
            message: '已复制，发送报备信息后，记得回来点 "报备确认" 哦~',
            duration: 3,
        })
    }

    componentWillMount() {
        this.setState({
            buildingId: (this.props.buildingInfo || {}).id,
            customerDeal: this.props.reportCustomerDeal
        })
    }
    componentWillReceiveProps(newProps) {
        if (this.state.buildingId !==  (newProps.buildingInfo || {}).id) {
          this.setState({ buildingId: (newProps.buildingInfo || {}).id}) 
        }
        
    }

    handleClick = (e) => { // 点击menu
        let buildingInfo = this.props.buildingInfo || {}
        let ruleInfo = buildingInfo.ruleInfo || {}
        let valPhone = ruleInfo.isCompletenessPhone
        this.setState({
            current: e.key,
        }, () => {
            if (this.state.current === 'all') {
               this.props.getMyCustomerInfo({
                    buildingId: this.state.buildingId,
                    status: null,
                    pageIndex: this.state.pageIndex,
                    type: 'status',
                    valphone: valPhone
                }) 
            } else {
                if (this.state.current !== '5') {
                    this.props.getMyCustomerInfo({
                        buildingId: this.state.buildingId,
                        status: [parseInt(this.state.current)],
                        pageIndex: this.state.pageIndex,
                        type: 'status',
                        valphone: valPhone
                    }) 
                } else {
                    this.props.getMyCustomerInfo({
                        buildingId: this.state.buildingId,
                        status: [parseInt(this.state.current), 6],
                        pageIndex: this.state.pageIndex,
                        type: 'status'
                    })
                }
                
            }
            
        });
    }

    handlePageChange = (pageIndex, pageSize) => {  // 翻页
        let buildingInfo = this.props.buildingInfo || {}
        let ruleInfo = buildingInfo.ruleInfo || {}
        let valPhone = ruleInfo.isCompletenessPhone
        this.setState({ pageIndex: (pageIndex - 1) }, () => {
            if (this.state.current === 'all') {
                this.props.getMyCustomerInfo({
                     buildingId: this.state.buildingId,
                     status: null,
                     pageIndex: this.state.pageIndex,
                     type: 'status',
                     valphone: valPhone
                 }) 
             } else {
                 if (this.state.current !== '5') {
                     this.props.getMyCustomerInfo({
                         buildingId: this.state.buildingId,
                         status: [parseInt(this.state.current)],
                         pageIndex: this.state.pageIndex,
                         type: 'status',
                         valphone: valPhone
                     }) 
                 } else {
                     this.props.getMyCustomerInfo({
                         buildingId: this.state.buildingId,
                         status: [parseInt(this.state.current), 6],
                         pageIndex: this.state.pageIndex,
                         type: 'status'
                     })
                 }
                 
             }
        });
        
    }

    textChange = (e) => {
        this.setState({ value: e.target.value})
    }

    searchMyCustomer = () => {
        let buildingInfo = this.props.buildingInfo || {}
        let ruleInfo = buildingInfo.ruleInfo || {}
        let valPhone = ruleInfo.isCompletenessPhone
        this.props.getMyCustomerInfo({
            buildingId: this.state.buildingId,
            keyWord: this.state.value,
            type: 'keyWord',
            status: this.state.current,
            pageIndex: this.state.pageIndex,
            valphone: valPhone
        })
    }

    comfirmThisData = (v, index) => { // 点击操作按钮
        // console.log(v, '我点击这条数据确认报备')
        switch (v.transactionsStatus) {
            case "等待驻场确认": 
                this.props.comfirm({ 
                    transactionsids: [v.id], 
                    buildingId: this.state.buildingId,
                    type: this.state.current
                });break;
            case "等待驻场报备":
               if (index === this.state.index) {
                    // 说明用户点的是该行的确认报备
                    this.props.report({
                        transactionsids: [v.id],
                        buildingId: this.state.buildingId,
                        type: this.state.current
                    })
                    this.setState({index: -1})
               } else if (this.state.index === -1) {
                   // 等于-1的情况说明没有点击复制操作，则弹出确认框
                    this.setState({ popVisibleIndex: index })
               }break;
            case "等待业务员带看":
                this.props.look({
                    transactionsids: [v.id], 
                    buildingId: this.state.buildingId,
                    type: this.state.current
                });break;
            case "等待业主成交": 
                this.props.showEditModal(v);
                break;
            case '业主已成交': 
                console.log(3)
                this.props.dispatch(getReportCustomerDeal(v.id));
                break;
            
        }
    }

    comfirm = () => {
        let buildingInfo = this.props.buildingInfo || {}
        let ruleInfo = buildingInfo.ruleInfo || {}
        let valPhone = ruleInfo.isCompletenessPhone
        let submintIds  = this.props.statusCounts.submintIds 
        this.props.comfirm({
            transactionsids: submintIds, 
            type: this.state.current, 
            buildingId: this.state.buildingId,
            valPhone: valPhone
        })
    }

    reportModal = () => { // 点击向开发商报备，弹出模态框，进行复制和确认报备操作
        this.props.showBatchReportModal({
            current: this.state.current,
            buildingId: this.state.buildingId,
        })
    }

    handleOk = (e) => {
        this.props.report({
            transactionsids: this.props.statusCounts.reportIds,
            type: this.state.current, 
            buildingId: this.state.buildingId
        })
        this.setState({ visible: false });
    }

    handleCancel = (e) => {
        this.setState({ visible: false });
    }
    selectBeforeChange = (value) => { // 选择搜索业务员还是客户
        value === '1' ? this.setState({ placeholder: '业务员'}) : this.setState({ placeholder: '客户'})
    }    

    
    render() {
        const {columns, placeholder} = this.state;
        const {isLoading, confirmTrue, reportTrue} = this.props;
        let { infoList, buildingInfo, statusCounts } = this.props;
        let basicInfo, name, areaFullName, icon;
        basicInfo = buildingInfo.basicInfo || {}
        name = basicInfo.name || ''
        areaFullName = basicInfo.areaFullName || ''
        icon = basicInfo.icon
        infoList.forEach((v, i) => {
            v.expectedBeltTime = moment(v.expectedBeltTime).format('YYYY-MM-DD HH:mm')
            switch (v.transactionsStatus) {
                case 0: v.transactionsStatus = '等待驻场确认'; break;
                case 1: v.transactionsStatus = '等待驻场报备'; break;
                case 2: v.transactionsStatus = '等待业务员带看'; break;
                case 3: v.transactionsStatus = '等待业主成交'; break;
                case 4: v.transactionsStatus = '业主已成交'; break;
                case 5: 
                case 6: v.transactionsStatus = '已失效'; break;
            }
        })
        const selectBefore = (
            <Select defaultValue="1" style={{ width: 70 }} onChange={this.selectBeforeChange}>
                <Option value="1">业务员</Option>
                <Option value="2">客户</Option>
            </Select>
        );
        
        return (
            <Spin spinning={isLoading}>
            <div className="relative">
                <Layout>
                    <Content className='content indexPage'>
                        <div> 
                            <Row className='headerContent'>
                                <Col span={12}>
                                    <div className="listLeft">
                                        <div><img src={ icon || '../../../images/default-icon.jpg'}/></div>
                                        <p className="listInfo">
                                            <p>{name}</p>
                                            <p>{areaFullName}</p>
                                        </p>
                                    </div>
                                </Col>
                                <Col span={12}>
                                    <div className='searchBox'>
                                        <Input size="large" placeholder={`请输入${placeholder}姓名或电话号码(11位)`} className="searchInput" 
                                        value={this.state.value} addonBefore={selectBefore}
                                        onChange={this.textChange} onPressEnter={this.searchMyCustomer}/>
                                        <Button type="primary" size="large" onClick={this.searchMyCustomer}>搜索</Button>
                                    </div>
                                </Col>
                            </Row>
                            <Row style={{marginTop: "15px"}}>
                                <Col span={24}>
                                    <Menu onClick={this.handleClick} selectedKeys={[this.state.current]} mode="horizontal">
                                        <Menu.Item key="all">全部</Menu.Item>
                                        <Menu.Item key="0">报备确认</Menu.Item>
                                        <Menu.Item key="1">提交报备</Menu.Item>
                                        <Menu.Item key="2">带看确认</Menu.Item>
                                        <Menu.Item key="3">成交确认</Menu.Item>
                                        <Menu.Item key="4">成交客户</Menu.Item>
                                        <Menu.Item key="5">失效客户</Menu.Item>
                                    </Menu>
                                </Col>
                            </Row>
                            <Row style={{marginTop: "15px"}}>
                            <p>
                                {
                                    statusCounts.submitCount === 0 ? null :
                                    <span>共<span className='redSpan'> {statusCounts.submitCount} </span>条报备信息等待驻场 <span className='buleSpan myPointer' onClick={this.comfirm}> 批量确认</span> 
                                    {
                                        statusCounts.submitCount === 0 || statusCounts.todayReportCount === 0 ? ' ' : ' / '
                                    }
                                    </span>
                                }
                                {
                                    statusCounts.todayReportCount === 0 ? null :
                                    <span> 今日共<span className='redSpan'> {statusCounts.todayReportCount} </span>条报备信息等待驻场 <span className='buleSpan myPointer' onClick={this.reportModal}> 批量向开发商报备</span></span>
                                }
                            </p>
                            </Row>
                            <Row style={{marginTop: "15px"}}>
                                <Table columns={columns} bordered dataSource={infoList} scroll={{ x: 1280 }} rowKey={record => record.id} pagination={false}/>
                                {
                                    infoList.totalCount > 10 ? 
                                    <Pagination showQuickJumper style={{textAlign: 'right', margin: '15px 0'}}
                                    current={this.state.pageIndex + 1}  
                                    total={infoList.totalCount} 
                                    onChange={this.handlePageChange}/> : null
                                }
                            </Row>
                        </div>

                    </Content>
                </Layout>
            </div>
            <TemplateDialog />
            <XyhDealDialog  parentPage='report'/>
            <BatchReport statusCounts={this.props.statusCounts}/>
            </Spin>
        )
    }
}

function apptableMapStateToProps(state) {
    return {
        customerInfo: state.index.customerInfo,
        myBuildingList: state.shop.myBuildingList,
        isLoading: state.shop.indexLoading,
        searchList: state.index.searchList,
        nowInfo: state.index.nowInfo, // 切换后的楼盘信息
        infoList: state.index.infoList,
        buildingInfo: state.index.buildingInfo,
        statusCounts: state.index.statusCounts,
        buildInfo: state.building.buildInfo,
        user: (state.oidc.user || {}).profile || {},
        showTemplateInfo: state.index.showTemplateInfo,
        reportCustomerDeal: state.index.reportCustomerDeal,
        vaiPhoneInfo: state.index.vaiPhoneInfo,
    }
}

function apptableMapDispatchToProps(dispatch) {
    return {
        dispatch,
        getMyCustomerInfo: (...args) => dispatch(getMyCustomerInfo(...args)),
        comfirm: (...args) => dispatch(comfirmAsync(...args)),
        look: (...args) => dispatch(lookAsync(...args)),
        report: (...args) => dispatch(reportAsync(...args)),
        showEditModal: (...args) => dispatch(showEditModal(...args)),
        showBatchReportModal: (...args) => dispatch(showBatchReportModal(...args)),
        showTemplateModal: (...args) => dispatch(showTemplateModal(...args))
    };
}
export default connect(apptableMapStateToProps, apptableMapDispatchToProps)(HouseResourceIndex);