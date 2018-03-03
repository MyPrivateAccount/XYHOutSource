import React, { Component } from 'react'
import { Modal, Button, Card, notification,Popconfirm, } from 'antd';
import {connect} from 'react-redux';
import { hideBatchReportModal, reportAsync,showTemplateModal,SearchValphone  } from '../../actions/actionCreator';
import moment from 'moment';
import './batchReport.less';
// import { CopyToClipboard } from 'react-copy-to-clipboard'
import ClipboardButton from 'react-clipboard.js'
import { getReport } from '../buildingDish/edit/utils'
import TemplateDialog from '../dialog/template'
import WebApiConfig from '../../constants/webapiConfig'
import ApiClient from '../../../utils/apiClient'

class BatchReportDialog extends Component {
    state = {
      copied: false, // 复制与否
      list: [], // 20条一组
      totalCount: 0, // 总报备条数
      status: 2, // 2表示报备成功  0 表示未复制   1 是表示已复制  （测试） 
      myLines: '',
      visible: false, // popVisibel
      condition: false, // 根据是否复制
      index: -1,
      vaiPhoneInfo: [],
    }
    componentWillReceiveProps(newProps) {
      const { transactionsResponses } = newProps.statusCounts
      this.group(transactionsResponses)
    }
    handleOk = (e) => {
        
    }
    handleCancel = (e) => {
        this.props.cancel();
    }
   group = (list) => {
     let buildingInfo = this.props.buildingInfo || {}
     let maxCustomer = (buildingInfo.ruleInfo || {}).maxCustomer
        //每20条为一组
        let gl = [];
        let curIdx = 0;
        let cur = {};
        for (let i = 0; i < list.length; i++) {
            if (curIdx === 0) {
                cur = {
                    seq: gl.length, // seq 表示第几组
                    list: [],
                    status: 0       // 2表示报备成功  0 表示未复制   1 是表示已复制  （测试） 
                }
                gl.push(cur);
            }
            cur.list.push(list[i]);
            curIdx++;
            if (curIdx >= maxCustomer) {
                curIdx = 0;
            }
        }
        this.setState({ list: gl, totalCount: list.length })
    }

    copyReport = (row) => { // 点击复制
      let { buildingInfo } = this.props
      let valPhone = (this.props.buildingInfo.ruleInfo || {}).isCompletenessPhone
      if (this.props.showTemplateInfo.isChecked) {
          //如果除此之外的组中存在已复制，未确认的组，不允许继续复制，必须先进行报备确认操作。
          let ogs = this.state.list.filter(x => x.seq !== row.seq && x.status === 1);
          if (ogs.length > 0) {
            notification.warning({
              message: `请先进行第${ogs.map(x => x.seq + 1).join(',')}组的“确认报备”操作！`,
              duration: 3,
            })
            return '';
          }
          let list =  row.list
          let lines = [];
          let ruleInfo = buildingInfo.ruleInfo || {}
          let reportedTemplate = ruleInfo.reportedTemplate 
          let template = [];
          if (reportedTemplate) {
            template = JSON.parse(reportedTemplate);
          }
          let split = '----------'
          for (let i = 0; i < list.length; i++) {
              if (lines.length > 0) {
                  lines.push(split)
              }
              let line = getReport(template, list[i], this.props.user, this.props.buildInfo || {})
              lines.push(line);
          }
          console.log(list, lines, 7)
          return lines.join('\r\n')
        } else {
          this.props.showTemplateModal()
        }
    }

    copySuccess = (row) => {
      notification.success({
          message: '已复制，发送报备信息后，记得回来点 "报备确认" 哦~',
          duration: 3,
      })
    }

  _changeGroupStatus = (row, status) => {  // 点击复制后更改该组的状态
    let newList = [...this.state.list];
    let old = newList.find(x => x.seq === row.seq);
    if (old) {
        let newItem = { ...row }
        newItem.status = status;
        let idx = newList.indexOf(old);
        newList.splice(idx, 1, newItem)
        this.setState({ list: newList })
    }
  }
  reported = (row, index) => {  // 点击报备确认再进行提示
    // console.log(row, '点击报备确认')
    // console.log(this.state.condition, 'condition');
    if (row.status === 0) { 
      this.setState({ visible: true, index:index });
    }
    else {
      this._reported(row);
    }
  }
  _reported = (row) => { // 点击确认报备后进行调用接口
    let { current, buildingId } = this.props.batchReportInfo
    let ids = row.list.map(x => x.id);
    // console.log(ids, 'id组')
    this.props.reportAsync({
      transactionsids: ids,
      type: current, 
      buildingId: buildingId
    })  
  }
  popConfirm = (row) => {
    this.setState({ index: -1 }, () => this._reported(row));
  }
  popCancel = () => {
    this.setState({ index: -1 });
  }

    render() {
        const { show } = this.props.batchReportInfo;
        const {todayReportCount, transactionsResponses} = this.props.statusCounts
        const myTitle = (
           <p>共 <span style={{color: 'red', fontWeight:'blod'}}>{todayReportCount}</span> 条报备信息等待驻场提交报备 （20条一组）</p>
        )
        const popTitle = (
          <p>
            <p>此组客户尚未进行复制操作</p>
            <p>确定后将无法再进行复制发送</p>
            <p>请确认是否继续？</p>
          </p>
        )
        return (
            <Modal title={myTitle}
                className='batchReportModal'
                visible={show}
                footer={null}
                onCancel={this.handleCancel}>
                <p style={{ marginBottom: '10px'}}>
                  <span style={{ fontWeight: 600 }}> 操作流程：</span>1. 点击复制 --> 2. 粘贴到微信或短信 --> 3. 发送 --> 4. 回到此页，点击确认报备 --> 5. 继续报备下一批次
                </p>
                {
                  this.state.list.map((v, index) => {
                    return (
                    <div key={index}>
                        {
                           v.status === 0 || v.status === 1 ? 
                           <Card style={{ width: 150 }} className='cardItem'>
                              {/* <CopyToClipboard onCopy={() => this.copyReport(v)} text={this.state.myLines}>
                                <p className='itemDiv'>
                                    <i className='iconfont icon-fuzhi'></i>
                                    <span>复制</span>
                                </p>
                              </CopyToClipboard> */}
                               <ClipboardButton component="span" 
                                option-text={() => this.copyReport(v)} 
                                onSuccess={() => this.copySuccess(v)}>
                                      <p className='itemDiv'>
                                          <i className='iconfont icon-fuzhi'></i>
                                          <span>复制</span>
                                      </p>
                                </ClipboardButton>
                                <Popconfirm title={popTitle} okText="确定" cancelText="取消"
                                            visible={this.state.index === index} 
                                            onConfirm={() => this.popConfirm(v)} 
                                            onCancel={this.popCancel}>
                                    <p className='itemDiv' onClick={() => this.reported(v, index)}> 
                                      <i className='iconfont icon-querenbaobei'></i>
                                      <span>确认报备</span>
                                    </p>
                                </Popconfirm>
                          </Card> :
                          <Card style={{ width: 150 }} className='cardItem'>
                              <p className='itemDiv'> 
                                <i className='iconfont icon-chenggong'></i>
                                <span>已报备</span>
                              </p>
                          </Card>
                        }
                    </div>
                      
                  )})
                }
                 <TemplateDialog />
            </Modal>
        )
    }
}

const mapStateToProps = (state, props)=>{
    return{
      batchReportInfo: state.index.batchReportInfo,
      buildInfo: state.building.buildInfo,
      user: (state.oidc.user || {}).profile || {},
      showTemplateInfo: state.index.showTemplateInfo,
      nowInfo: state.index.nowInfo, // 切换后的楼盘信息
      buildingInfo: state.index.buildingInfo,
    }
}
const mapDispatchToProps = (dispatch)=>{
    return {
        cancel: () => dispatch(hideBatchReportModal()),
        reportAsync: (...args) => dispatch(reportAsync(...args)),
        showTemplateModal: (...args) => dispatch(showTemplateModal(...args))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(BatchReportDialog);