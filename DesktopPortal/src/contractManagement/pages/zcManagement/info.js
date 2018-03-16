import { connect } from 'react-redux';
import { savePerson,getsiteuserlist, changeArr,getExaminesList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Select, Icon, Spin, notification} from 'antd'
import moment from 'moment'


const Option = Select.Option;

class Info extends Component {
  state = {
    currentInfo: [],
    projectId: '',
  }

  initData = (id) => {
      let condition = {
          pageSize: 1,
          contentTypes: 'BuildingsOnSite',
          examinAction: 'BuildingsOnSite',
          contentIds: this.state.projectId
      }
      this.props.dispatch(getExaminesList({ 
          condition: condition
      }))
  }

  componentWillMount() {
   
    this.setState({ 
      currentInfo: this.props.currentInfo, 
      projectId: this.props.currentInfo.id
    }, () => {
      // this.initData(this.state.projectId)
      console.log(this.state.currentInfo, this.state.projectId)
      this.props.dispatch(getsiteuserlist(this.state.projectId))
    })
  }

  componentDidMount() {
     
  }

  componentWillReceiveProps(newProps) {
    if (this.state.projectId !== newProps.currentInfo.id) {
        this.props.dispatch(getsiteuserlist(newProps.currentInfo.id))
        // this.initData(this.state.projectId)
        this.setState({ 
          currentInfo: newProps.currentInfo, 
          projectId: newProps.currentInfo.id
        }, () => {
         console.log(this.state.currentInfo, this.state.projectId, 123)
        })
    }
  }


  clickItem = (v, index) => {
    let checkedArr = this.props.checkedArr.concat([])
    let arr = this.props.userArr.concat([])
    if (!arr[index].isChecked && this.props.checkedArr.length >= 4) {  // 在大于4个并且选中
      notification.warning({
        message: '最多指派四个驻场',
        duration: 3
      })
      return
    }
    arr[index].isChecked = !arr[index].isChecked
    if (arr[index].isChecked) {
      checkedArr.push({
        id: v.id,
        name: v.trueName
      })
    } else {
      let i = checkedArr.findIndex(x => x.id === v.id)
      checkedArr.splice(i,1)
    }
    console.log(checkedArr, '指派驻场checkedArr')
    this.props.dispatch(changeArr({arr:arr, checkedArr:checkedArr }))
  }
  getExamineStatus = (status) => {
    switch (status) {
      case 1: return '审核中';break;
      case 3: return '审核驳回';break;
    }
  }


  render() {
    const { operating } = this.props.editManagerInfo
    return ( 
      <Spin spinning={this.props.infoLoading}>
      <div className="info" style={{marginTop: this.props.type ? 0 : '25px'}}>
          <div className='infoDiv'>
    <p className='title' style={{ width: '140px'}}>
    请选择驻场：
    {
      this.props.examineStatus ? 
      <span style={{fontWeight: 'bold', color: 'red'}}>{this.getExamineStatus(this.props.examineStatus)}
      </span> : null 
    }
    </p>
              <div className='itemUser'>
              {
                this.props.userArr.map((v, index) => {
                  return (
                  <div className={v.isChecked ? 'checkedPerson' : 'checkPerson'}  onClick={() => this.clickItem(v, index)} key={index}>
                    { 
                      v.isChecked ?
                      <Icon type="check" className='icon' size='small'/>
                      : null
                    }
                    <div>{v.trueName}</div>
                  </div>
                  )
                })
              }
              </div>
          </div>
          {
            this.props.type ? null :
            <div className='infoDiv'>
              <Button type='primary' 
              style={{width: '100px'}} 
              onclick={this.save} 
              loading={operating}>保存</Button>
            </div>   
          }            
      </div>
    </Spin>
    )
  }
}

function mapStateToProps(state) {
  return {
    editManagerInfo: state.manager.editManagerInfo,
    infoLoading: state.manager.infoLoading,
    checkedArr: state.manager.checkedArr,
    userArr: state.manager.userArr,
    currentInfo: state.manager.currentInfo,
    examineStatus: state.manager.examineStatus,
    
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch,
    savePerson: (...args) => dispatch(savePerson(...args))
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Info);