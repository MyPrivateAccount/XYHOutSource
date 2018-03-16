import React, { Component } from 'react'
import { Modal, Button, Form, Checkbox, Input,InputNumber,Select,notification } from 'antd';
import {connect} from 'react-redux';
import { customerDealAsync, hideEditModal, getDicParList,getReportCustomerDeal,
    getMakeDealCustomerInfo, getShopsSaleStatus } from '../../actions/actionCreator';
import moment from 'moment';
import './dealInfo.less';
import WebApiConfig from '../../constants/webapiConfig'
import ApiClient from '../../../utils/apiClient'


const FormItem = Form.Item;
const Option = Select.Option;


class XyhDealDialog extends Component {
    state = {
        inputDisabled: false,
        selectDisabled: false,
        seller: 0,
        selectCustomer: [],
        buildingId: '',
        id: '', // 标识id
        shopName: '',
        shopList: [],
        customerDeal: {},
        examineStatus: 0,
        mycustomer: []

    }
    componentWillMount() {
        this.props.getDicParList();
        let { entity } = this.props.editDealInfo || {}
        this.setState({
            buildingId: entity.buildingId,
            id: entity.id,
        }, ()=>{
            if (this.props.parentPage !== 'report') { // 此页面是销控页面的成交信息
                console.log(entity.buildingId, entity.id, 'dididi')
                this.initCustomerData(entity.buildingId, entity.id)

            } else { // 此页面是驻场首页的成交信息
                console.log(entity.buildingId, entity.id, 'dididi')
                this.initShopData(entity.buildingId, entity.id)
            }
        })
        
    }
    initCustomerData = async (buildingId, shopId) => { // 获取成交客户列表
        console.log(buildingId, shopId, 123)
        let url =  `${WebApiConfig.customerTransactions.Search}`;
        let condition = {
            pageIndex: 0,
            pageSize: 99999,
            status: [3], 
            buildingId: buildingId, 
        };
        try{
            let data = await ApiClient.post(url, condition);
            console.log(data, '获取成交客户列表')
            let res = data.data || {}
            if (res.code === '0') {
                if (res.extension) {
                    this.setState({
                        mycustomer: res.extension
                    }, () => {
                        console.log(this.state.mycustomer, 444)
                        this.getCustomerDeal({id: shopId, type: 'xk'}) // 获取成交信息
                    })
                }
            } else {
                notification.error({
                    message: '获取商铺失败',
                    duration: 3
                });
            }
        } catch(e) {

        }
    }

    initShopData = async (buildingId, flowId) => { // 获取商铺列表
        console.log(buildingId, flowId, 123)
        let url =  `${WebApiConfig.xk.Base}`;
        let condition = {
            pageIndex: 0,
            pageSize: 100000000,
            saleStatus: ['2'], 
            buildingIds: [buildingId], 
        };
        try{
            let data = await ApiClient.post(url, condition);
            console.log(data, '商铺列表数据')
            let res = data.data || {}
            if (res.code === '0') {
                if (res.extension) {
                    this.setState({
                        shopList: res.extension
                    }, () => {
                        this.getCustomerDeal({id: flowId, type: 'report'}) // 获取成交信息
                    })
                }
            } else {
                notification.error({
                    message: '获取商铺失败',
                    duration: 3
                });
            }
        } catch(e) {

        }
    }

    getCustomerDeal = async (v) => { // 获取成交信息
        let url;
        if (v.type === 'report') { // 报备
            url = `${WebApiConfig.customerDeal.GetReportCustomerDeal + v.id}`;
        } else { // 销控中心
            url = `${WebApiConfig.customerDeal.GetCustomerDeal + v.id}`;
        }
        console.log(url, '查询成交信息url')
        try{
            let data = await ApiClient.get(url);
            let res = data.data || {}
            if (res.code === '0') {
                if (v.type !== 'report') {
                    res.extension = res.extension[0] || {}
                    console.log(res.extension, '成交信息111')
                }
                console.log(res.extension, '成交信息')
                if (res.extension) { // 有审核信息
                    let examineStatus = res.extension.examineStatus // 0, 1, 8, 16
                    if ([1, 16].includes(examineStatus)) { // 审核中和审核驳回
                        this.setState({
                          customerDeal: res.extension,
                          examineStatus: examineStatus,
                          seller: res.extension.sellerType.toString()
                        })
                    }
                } else {
                    this.setState({
                        customerDeal: {},
                        examineStatus: 0,
                        seller: 0
                    })
                }
            } else {
                notification.error({
                    message: '获取成交信息失败',
                    duration: 3
                });
            }
        } catch(e) {

        }
    }

    componentWillReceiveProps(newProps) {
        let { entity } = newProps.editDealInfo || {}
        console.log(entity, '``````````')
        this.setState({buildingId: entity.buildingId})
        if (this.props.parentPage !== 'report') {
            if (this.state.id !== entity.id) {
                console.log(this.state.id, entity.id, '该条数据的商铺id')
                this.initCustomerData(entity.buildingId, entity.id)
                this.setState({id: entity.id})
            }
        } else {
            if (this.state.id !== entity.id) { // 说明点击的是不同的客户报备信息
                console.log( entity.id, '该条数据的报备id')
                this.initShopData(entity.buildingId, entity.id)
                this.setState({id: entity.id})
            }
        }
    }

    handleOk = (e) => {
        let { entity } = this.props.editDealInfo
        const {selectCustomer} = this.state
        
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                if (this.props.parentPage === 'report') {
                    let body = {
                        sellerType: 1, // 自售
                        salesman: entity.userId,  // 业务员
                        commission: values.commission, // 佣金
                        totalPrice: values.totalPrice, // 成交总价
                        projectId: entity.buildingId, // 楼盘id
                        shopId: values.shopId, // 商铺id
                        // mark: values.mark, // 备注
                        flowId: entity.id, //报备流程id
                        customer: entity.customerId, // 客户id
                        shopName: this.state.shopName, // 商铺名称
                        buildingName: entity.buildingName // 楼盘名称
                    }
                    this.props.save({body: body, page: 'report'})
                } else {
                    // 销控...
                    let body;
                    switch (this.state.seller) {
                        case '1': // 自售
                        body = {
                            sellerType: 1, // 自售
                            salesman: selectCustomer.userId,  // 业务员  无
                            commission: values.commission, // 佣金
                            totalPrice: values.totalPrice, // 成交总价
                            projectId: entity.buildingId, // 楼盘id
                            shopId: entity.id, // 商铺id
                            // mark: values.mark, // 备注
                            flowId: selectCustomer.id, //报备流程id
                            customer: selectCustomer.customerId, // 客户id
                            shopName: entity.buildingNo-entity.floorNo-entity.number, // 商铺名称
                            buildingName: entity.buildingName // 楼盘名称
                        };
                        this.props.save({body: body, page: 'xk'});
                        break;
                        case '2': // 第三方
                        body = {
                            sellerType: 2, // 第三方
                            proprietor : values.proprietor,
                            mobile: values.mobile, 
                            projectId: entity.buildingId, // 楼盘id
                            shopId: entity.id, // 商铺id
                            // mark: values.mark, // 备注
                            address : values.address , 
                            seller: values.company,
                            idcard: values.idcard,
                            shopName: `${entity.buildingNo}-${entity.floorNo}-${entity.number}`, // 商铺名称
                            buildingName: entity.buildingName // 楼盘名称
                        }; 
                        this.props.save({body: body, page: 'xk'});
                        break;
                        default: // 未知
                        body = {
                            sellerType: 10, // 第三方
                            proprietor : values.proprietor,
                            mobile: values.mobile, 
                            projectId: entity.buildingId, // 楼盘id
                            shopId: entity.id, // 商铺id
                            // mark: values.mark, // 备注
                            address : values.address , 
                        };
                        this.props.save({body: body, page: 'xk'});
                       
                    }
                }
            }
        });
        this.props.form.resetFields();
    }
    handleCancel = (e) => {
        this.props.cancel();
        this.props.form.resetFields();
    }
    handleSelectChange = (v, option) => {
        console.log(v, '????', option)
        this.setState({shopName: option.props.children})
    }
    handleSelectType = (v) => {
        console.log(v)
        this.setState({seller: v})
    }
    handleCustomer =  (v) => {
        console.log(v, '选择客户')
        // let customer = this.props.customerInfo  || []
        let customer = this.state.mycustomer  || []
        let selectCustomer = customer.find(item => {
            return item.id === v
        })
        this.setState({
            selectCustomer: selectCustomer
        })

    }
    render() {
        console.log(this.state.customerDeal, this.state.examineStatus, '审核数据', this.props.basicData.xkSellerType)
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: {
              xs: { span: 24 },
              sm: { span: 6 },
            },
            wrapperCol: {
              xs: { span: 24 },
              sm: { span: 14 },
            }
        };
        let { title, show, entity, operating } = this.props.editDealInfo;
        const { selectDisabled, inputDisabled,customerDeal,examineStatus } = this.state
        if  (examineStatus === 1) {
            title = <span>{title}  <span style={{color: 'red',marginLeft: '10px'}}>审核中</span></span>
        } else if(examineStatus === 16){
            title = <span>{title}  <span style={{color: 'red',marginLeft: '10px'}}>审核驳回</span></span>
        }
        const parentPage = this.props.parentPage
        let nowDate = moment().format('YYYY-MM-DD')
        // let customer = (this.props.customerInfo  || []).slice()// 客户
        let customer = (this.state.mycustomer  || []).slice()// 客户
        let myseller = (this.props.customerInfo || []).slice() // 成交人
        return (
            <Modal title={title} 
                visible={show}
                confirmLoading={operating}
                footer={[
                    <Button  size="large" onClick={this.handleCancel}>取消</Button>,
                    <Button  type="primary" size="large" onClick={this.handleOk} disabled={examineStatus === 1 ? true : false}>确定</Button>,
                ]}
                // onOk={this.handleOk}
                onCancel={this.handleCancel}>
                <Form ref={(e) => this.form = e}>
                {
                    parentPage === 'report' ?
                    <div>
                        <FormItem  {...formItemLayout} label='分行（组）名称'>
                            {getFieldDecorator('departmentName', {
                                initialValue: entity.departmentName,
                            })(
                                <Input disabled />
                            )}
                        </FormItem>
                        <FormItem  {...formItemLayout} label="成交人">
                            {getFieldDecorator('salesman', {
                                initialValue: entity.userTrueName,
                            })(
                                <Input disabled />
                            )}
                        </FormItem>
                        <FormItem  {...formItemLayout} label="成交客户">
                            {getFieldDecorator('customer', {
                                initialValue: entity.customerName,
                            })(
                                <Input disabled />
                            )}
                        </FormItem>
                        <FormItem  {...formItemLayout} label='商铺'>
                            {getFieldDecorator('shopId', {
                                initialValue: [1, 16].includes(examineStatus) ? customerDeal.shopId : null,
                                rules: [{ required: true, message: '请选择成交商铺' }],
                            })(
                                <Select allowClear={true} onSelect={this.handleSelectChange} disabled={[1].includes(examineStatus) ? true : false}>
                                {
                                    this.state.shopList.map( v => <Option key={v.id}>{`${v.buildingNo}-${v.floorNo}-${v.number}`}</Option> )
                                }
                                </Select>
                            )}
                        </FormItem>
                    </div>
                    :
                    <div>
                            <FormItem  {...formItemLayout} label='销售方' className='dealInfo'>
                                {getFieldDecorator('type', {
                                    initialValue: [1, 16].includes(examineStatus) ? customerDeal.sellerType.toString() : null,
                                    rules: [{ required: true, message: '请输入成交方式' }],
                                })(
                                    <Select onChange={this.handleSelectType} allowClear={true}>
                                    {
                                        this.props.basicData.xkSellerType.map( v => <Option key={v.value}>{v.key}</Option> )
                                    }
                                    </Select>
                                )}
                            </FormItem>
                            {
                                this.state.seller === '1' ? 
                                <p>
                                    <FormItem  {...formItemLayout} label="成交客户">
                                        {getFieldDecorator('customerName', {
                                            initialValue:[1, 16].includes(examineStatus) ? customerDeal.flowId : null,
                                            rules: [{ required: true, message: '请选择成交客户' }],
                                        })(
                                            <Select  allowClear={true} onChange={this.handleCustomer}>
                                            {
                                                customer.map( v => <Option key={v.id}>{`${v.customerName} (${v.buildingName})`}</Option> )
                                            }
                                            </Select>
                                        )}
                                    </FormItem>
                                </p> :
                                <p>
                                        
                                            {
                                                this.state.seller === '2' ? 
                                                <FormItem  {...formItemLayout} label='分销商' className='dealInfo'>
                                                {getFieldDecorator('company', {
                                                    rules: [{ required: true, message: '请选择分销商' }],
                                                })(
                                                    <Select  allowClear={true}>
                                                    {
                                                        this.props.basicData.xkSeller.map( v => <Option key={v.value}>{v.key}</Option> )
                                                    }
                                                    </Select>
                                                )}
                                                </FormItem> : null
                                            }
                                           
                                                {    ['2', '10'].includes(this.state.seller) ? 
                                                     <div className='notNeed'>
                                                        <p style={{textAlign:'center', color: '#B9B7B7', marginBottom: '10px'}}>成交人信息</p>
                                                        <FormItem  {...formItemLayout} label='姓名' className='dealInfo'>
                                                            {getFieldDecorator('proprietor', {
                                                            
                                                            })(
                                                                <Input  />
                                                            )}
                                                        </FormItem>
                                                        <FormItem  {...formItemLayout} label='电话' className='dealInfo'>
                                                            {getFieldDecorator('mobile', {
                                                            
                                                            })(
                                                                <Input  />
                                                            )}
                                                        </FormItem>
                                                        <FormItem  {...formItemLayout} label='身份证' className='dealInfo'>
                                                            {getFieldDecorator('idcard', {
                                                            
                                                            })(
                                                                <Input  />
                                                            )}
                                                        </FormItem>
                                                        <FormItem  {...formItemLayout} label='地址' className='dealInfo'>
                                                            {getFieldDecorator('address', {
                                                            
                                                            })(
                                                                <Input  />
                                                            )}
                                                        </FormItem> 
                                                    </div> : null
                                            }
                                       
                                </p>
                               
                            }
             
                    </div>
                }
                {
                    this.state.seller === '1' || parentPage === 'report'? 
                    <p>
                    <FormItem  {...formItemLayout} label="成交总价(元)">
                        {getFieldDecorator('totalPrice', {
                            initialValue: [1, 16].includes(examineStatus) ? customerDeal.totalPrice : null,
                            rules: [{ required: true, message: '请输入成交总价' }],
                        })(
                            <InputNumber min={0} disabled={[1].includes(examineStatus) ? true : false}/>
                        )}
                    </FormItem>   
                    <FormItem  {...formItemLayout} label="佣金(元)">
                        {getFieldDecorator('commission', {
                            initialValue: [1, 16].includes(examineStatus) ? customerDeal.commission : null,
                            rules: [{ required: true, message: '请输入佣金' }],
                        })(
                            <InputNumber min={0} disabled={[1].includes(examineStatus) ? true : false}/>
                        )}
                    </FormItem> 
                    </p> : null
                }
                 {/* <FormItem  {...formItemLayout} label="备注">
                        {getFieldDecorator('mark', {
                        })(
                            <Input type="textarea" rows={4} placeholder='如：减拥情况说明'/>
                        )}
                 </FormItem>  */}
                </Form>
                <div style={{marginTop:'25px', textAlign: 'right'}}>成交日期： {nowDate}</div>
            </Modal>
        )
    }
}

const mapStateToProps = (state, props)=>{
    // console.log(state.basicData, 'jahahhah')
    return{
        editDealInfo: state.index.editDealInfo,
        basicData: state.basicData,
        customerInfo: state.center.customerInfo,
        kanBanList: state.center.kanBanList,
    }
}
const mapDispatchToProps = (dispatch)=>{
    return {
        dispatch,
        save: (...args) => dispatch(customerDealAsync(...args)),
        cancel: ()=> dispatch(hideEditModal()),
        getDicParList: () => dispatch(getDicParList(['XK_SELLER','XK_SELLER_TYPE','SHOP_SALE_STATUS'])),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create({
    mapPropsToFields(props) {
        if (props.form) {
            props.form.resetFields();
        }
    }
})(XyhDealDialog));