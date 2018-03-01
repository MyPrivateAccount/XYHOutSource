import React, { Component } from 'react'
import { Modal, Button, Card, notification,Popconfirm, Checkbox } from 'antd';
import {connect} from 'react-redux';
import { hideTemplateModal, changeChecked  } from '../../actions/actionCreator';
import moment from 'moment';
import './batchReport.less';
import { CopyToClipboard } from 'react-copy-to-clipboard'
import { getReport } from '../buildingDish/edit/utils'


const expCustomer = {
    "id": "4fa9fe37-2899-4174-a1c9-ad8e00188b5a",
    "customerId": "fba92d39-2aeb-4d19-88cd-99143e6fa598",
    "customerName": "张三",
    "userId": "5cfe1b85-360b-4014-94bc-3e37cece7339",
    "departmentId": "0",
    "departmentName": "西区一区一组",
    "mainPhone": "150****3636",
    "userPhone": '13122220000',
    "userTrueName": "业务员A",
    "transactionsStatus": 2,
    "reportTime": "2017-11-30T17:59:32.191637",
    "buildingId": "b5a0baae-9691-4a20-9723-c07f9d2b7631",
    "shopsId": "77b11f39-d70f-47d1-9edc-24ab794dbc34",
    "buildingName": "万科金色悦城",
    "shopsName": "3",
    "expectedBeltTime": "2017-11-28T17:48:53.770883"
}

class TemplateDialog  extends Component {
    state = {
    }
    componentWillMount() {
    }
    handleOk = (e) => {
      this.props.cancel();
    }
    handleCancel = (e) => {
      this.props.cancel();
    }
    checkedChange = (e) => { 
    let isChecked = e.target.checked
    this.props.changeChecked(isChecked);
}

    render() {
        let { buildingInfo } = this.props;
        let ruleInfo, reportedTemplate;
        ruleInfo = buildingInfo.ruleInfo || {}
        reportedTemplate = ruleInfo.reportedTemplate 
        let template = [];
        if (reportedTemplate) {
          template = JSON.parse(reportedTemplate);
        }
        let p = this.props.buildInfo || {};
        if (!p.buildingBasic || !p.buildingBasic.name) {
            p.buildingBasic = { name: '楼盘名称' }
        }
        let tempateStr = getReport(template, expCustomer, this.props.user, p)

        const { show } = this.props.showTemplateInfo;
        const popTitle = (
          <p>
            <p>此组客户尚未进行复制操作</p>
            <p>确定后将无法再进行复制发送</p>
            <p>请确认是否继续？</p>
          </p>
        )
        return (
          <Modal title="报备模板预览" visible={show} onCancel={this.handleCancel}
              footer={[
                  <Button key="submit" type="primary" onClick={this.handleOk} >
                  确定
                  </Button>
              ]}>
              <div className='myModal'>
                  <pre>
                      {tempateStr ? tempateStr : '该楼盘还没有报备模板'}
                  </pre>
              </div>
              <Checkbox onChange={this.checkedChange}>下次不再提示</Checkbox>
          </Modal>
        )
    }
}

const mapStateToProps = (state, props)=>{
    return{
      showTemplateInfo: state.index.showTemplateInfo,
      buildInfo: state.building.buildInfo,
      user: (state.oidc.user || {}).profile || {},
    //   nowInfo: state.index.nowInfo, // 切换后的楼盘信息
      buildingInfo: state.index.buildingInfo,
    }
}
const mapDispatchToProps = (dispatch)=>{
    return {
        cancel: () => dispatch(hideTemplateModal()),
        changeChecked: (...args) => dispatch(changeChecked(...args)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(TemplateDialog);