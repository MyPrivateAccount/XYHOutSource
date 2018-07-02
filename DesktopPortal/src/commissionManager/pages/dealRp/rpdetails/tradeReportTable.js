//选择成交报备列表页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import {Modal, Table, Spin,  Row, Col, Button, Form, DatePicker, Input , notification} from 'antd'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import moment from 'moment'

const FormItem = Form.Item;

class TradeReportTable extends Component {
    appTableColumns = [
        { title: '业务员', dataIndex: 'userName', key: 'userName' },
        { title: '组织', dataIndex: 'departmentName', key: 'departmentName' },
        { title: '成交日期', dataIndex: 'createTime', key: 'createTime' },
        { title: '成交楼盘', dataIndex: 'buildingName', key: 'buildingName' },
        { title: '商铺编号', dataIndex: 'shopName', key: 'shopName' },
        { title: '成交客户', dataIndex: 'customerName', key: 'customerName' },
        { title: '成交佣金', dataIndex: 'commission', key: 'commission' },
        { title: '成交总价', dataIndex: 'totalPrice', key: 'totalPrice' }

    ];
    state = {
        isDataLoading: false,
        pagination: {},
        visible: false,
        list:[],
        selectedItem:null
    }
   
    handleTableChange = (pagination, filters, sorter) => {
        this.setState({
            pagination:{...this.state.pagination,...{pageIndex:pagination.current}}
        },()=>{
            this.search();
        })
    };
    
    search = async ()=>{
        let condition = this.props.form.getFieldsValue();
        condition = {...condition};
        

        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({isDataLoading:true})
        try{
            let url = WebApiConfig.rp.getcjbb;
            let r = await ApiClient.post(url, condition);
            if(r && r.data && r.data.code==='0'){
                this.setState({list: r.data.extension, selectedItem:null});
            }else{
                notification.error({message:'查询报备信息失败'});
            }
        }catch(e){
            notification.error({message:'查询报备信息失败'})
        }
        this.setState({isDataLoading:false})
    }
    handleCancel = (e) => {
        if(this.props.onClose){
            this.props.onClose(false);
        }
    }
    handleOk = async (e) => {
        if(!this.state.selectedItem){
            Modal.warning({
                title: '请先选择成交信息',
                content: '点击表中需要选择的行',
              });
              return;
        }
        if(this.props.selectedCallback){
            this.props.selectedCallback(this.state.selectedItem)
        }
    }
    
    rowClick=(record)=>{
        this.setState({selectedItem: record})
    }

    rowClassName=(record,index)=>{
        if(!this.state.selectedItem){
            return '';
        }
        if(this.state.selectedItem.id === record.id){
            return 'selected'
        }
        return '';
    }

    componentWillReceiveProps(newProps) {
        if(!this.props.visible && newProps.visible){
            let sv = this.props.form.getFieldsValue(['start']).start;
            if(!sv){
                let now = new Date();
                let range = {
                    end:  moment(now),
                    start : moment(now).day(-7)
                }
                this.props.form.setFieldsValue(range);

                setTimeout(() => {
                    this.search();
                }, 0);
            }
        }
    }

    handleDoubleClick = (e) => {
        this.props.onHandleChooseCjbb(e)
        this.setState({ visible: false })
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const {list, pagination} = this.state;

        return (
            <Modal width={900} title={'成交报备列表'} maskClosable={false} visible={this.props.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Spin spinning={this.state.isDataLoading}>
                    <Form>
                        <Row className="form-row">
                            <Col span={12}  style={{display:'flex'}}>
                                <FormItem label="申请日期">
                                    {
                                        getFieldDecorator('start')(
                                            <DatePicker />
                                        )
                                    }

                                </FormItem>
                                -
                    <FormItem>
                                    {
                                        getFieldDecorator('end')(
                                            <DatePicker />
                                        )
                                    }

                                </FormItem>
                            </Col>
                            <Col span={12} style={{display:'flex'}}>
                                <FormItem label="关键字" style={{ flex: 1 }}>
                                    {
                                        getFieldDecorator('keyword')(
                                            <Input placeholder="楼盘名称、客户姓名、电话、业务员姓名、电话" />
                                        )
                                    }

                                </FormItem>
                                <Button style={{ marginLeft: '1rem' }} onClick={this.search}>查询</Button>
                            </Col>
                        </Row>
                    </Form>
                    <Table onRowClick={this.rowClick} rowClassName={this.rowClassName}  onRowDoubleClick={this.handleDoubleClick} columns={this.appTableColumns} dataSource={list} pagination={pagination} onChange={this.handleTableChange} bordered={true}></Table>
                </Spin>
            </Modal>
        )
    }
}
function MapStateToProps(state) {

    return {

    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

const WrappedTradeReportTable = Form.create()(TradeReportTable);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedTradeReportTable);