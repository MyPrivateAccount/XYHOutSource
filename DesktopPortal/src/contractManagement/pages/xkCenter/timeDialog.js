import { connect } from 'react-redux';
import {  cancelTimeDialog, saveTimeAsync } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Modal, DatePicker ,Form  } from 'antd'
import moment from 'moment'
import echarts from 'echarts'

const FormItem = Form.Item;

class TimeDialog extends Component {
  state = {
    
  }
  handleOk = (e) => {
    this.props.form.validateFields((err, values) => {
      if (!err) {
        this.props.save({
          time: moment(values.time).format('YYYY-MM-DD'), 
          id: [this.props.editTimeInfo.shopId],
          saleStatus: this.props.editTimeInfo.saleStatus,
          current: this.props.editTimeInfo.current,
          buildingId: this.props.editTimeInfo.buildingId,
        })
      }
    })
  }
  handleCancel = (e) => {
      this.props.cancelTimeDialog();
  }
 
  render() {
    const { getFieldDecorator } = this.props.form;
    const { show, operating } = this.props.editTimeInfo
    return ( 
      <Modal  title='小定日期至' 
       visible={show}
       confirmLoading={operating}
       onOk={this.handleOk}
       onCancel={this.handleCancel}>
        <p>注意：小定录入后，商铺将被锁定，必须手动解锁</p>
        <Form>
            <FormItem> 
              {getFieldDecorator('time', {
                rules: [{ required: true, message: '请输入小定时间' }],
              })(
                <DatePicker style={{margin: '10px 0'}}/>
              )}
            </FormItem>
        </Form>
      </Modal>
    )
  }
}

function mapStateToProps(state) {
  // console.log('首页', state.index.todayReportListId);
  return {
    editTimeInfo: state.center.editTimeInfo
  }
}

function mapDispatchToProps(dispatch) {
  return {
    cancelTimeDialog: () => dispatch(cancelTimeDialog()),
    save: (...args) => dispatch(saveTimeAsync(...args))
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(TimeDialog));