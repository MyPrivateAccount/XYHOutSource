import { connect } from 'react-redux';
import { deletePicAsync, setDescription, setTitle } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Input,Spin,Upload, Icon, Modal,notification } from 'antd'
import moment from 'moment'
import { NewGuid } from '../../../utils/appUtils';



class TextPic extends Component {
  state = {
    detailValue: '',
    defaultValue: '',
    defaultTitleValue: '',
    titleValue: '',
    id: ''
  }
  

  componentWillMount() {
    // console.log(this.props.dynamicStatusArr, '/idididi')
    const dynamicStatusArr = this.props.dynamicStatusArr || []
    this.setState({
      id: dynamicStatusArr.id || ''
    })
  }
  componentWillReceiveProps(newProps) {
    const { dynamicStatusArr, recordList } = newProps
    // console.log(dynamicStatusArr, dynamicStatusArr.id, this.state.id , '9999999999999999999')
    if (dynamicStatusArr.id !== this.state.id) {
      if (recordList.length > 0) { // 说明是有审核的
        if ([1, 16].includes(dynamicStatusArr.examineStatus)) { // 0, 1, 8, 16
          this.setState({
            detailValue: dynamicStatusArr.content,
            titleValue: dynamicStatusArr.title
          })
        } 
      } 
      this.setState({id: dynamicStatusArr.id},() => {
        console.log(this.state.id, '>>>>>>>')
      })
    } 
  }
 
  onChangeTitleValue = (e) => {
    this.setState({titleValue: e.target.value}, () => {
       this.props.dispatch(setTitle(this.state.titleValue))
    })
  }

  onChangeValue = (e) => {
    this.setState({detailValue: e.target.value}, () => {
      this.props.dispatch(setDescription(this.state.detailValue))
    })
  }



  render() {

    const text= this.props.text // 审核状态
   
    return ( 
      <div className='textPic'>
        <div style={{marginBottom: '10px'}}>填写以下信息，及时通知业务员房源动态信息变更</div>
        <Input placeholder='请输入标题（必填）...' 
        style={{width: 200, marginBottom: '10px'}}
        onChange={this.onChangeTitleValue}
        defaultValue={this.state.defaultTitleValue}
        value={this.state.titleValue}
        disabled = {text === '审核中' ? true : false}/>
        <div className='text'>
          <Input type="textarea" rows={8} 
          placeholder='请输入详细描述（必填）...' 
          defaultValue={this.state.defaultValue}
          value={this.state.detailValue}
          disabled = {text === '审核中' ? true : false}
          onChange={this.onChangeValue}/>
        </div>
      </div>
    )
  }
}

function mapStateToProps(state) {
  return {
    descriptionValue: state.active.descriptionValue,
    dynamicStatusArr: state.active.dynamicStatusArr,
    buildInfo: state.building.buildInfo,
    recordList: state.active.recordList,
    projectId: state.active.projectId,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(TextPic);