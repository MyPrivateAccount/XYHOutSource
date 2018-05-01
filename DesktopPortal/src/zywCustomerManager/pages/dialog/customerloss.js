import {connect} from 'react-redux';
import {hideCustomerlossModal, customerloss} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Modal, Radio, Input} from 'antd';
import {getDicPars} from '../../../utils/utils';

const RadioGroup = Radio.Group;

class CustomerLoss extends Component {
  state = {
    value: '1',
    invalidReason: '',
    invalidResions: []
  }
  componentWillMount() {

  }
  componentWillReceiveProps(newProps) {
    console.log("字典:", this.props.rootBasicData);
    if (this.state.invalidResions.length === 0) {
      let invalidResions = getDicPars("ZYW_INVALID_REASON", this.props.rootBasicData.dicList);
      this.setState({invalidResions: invalidResions || []});
    }
  }

  handleOk = () => {
    let currentCustomer = this.props.currentCustomer
    let searchInfo = this.props.searchInfo
    let obj = {
      customerId: currentCustomer.id,
      lossTypeId: parseInt(this.state.value),
      lossRemark: this.state.invalidReason
    }
    this.props.dispatch(customerloss({obj: obj, searchInfo: searchInfo, type: this.props.type}))
  }

  handleCancel = () => {
    this.props.dispatch(hideCustomerlossModal());
  }

  onChange = (e) => {
    this.setState({value: e.target.value});
  }
  getText = (e) => {
    this.setState({invalidReason: e.target.value});
  }

  render() {
    // console.log(this.props.type, '666')
    let {isLoading, isShow} = this.props
    return (
      <Modal title="无效原因"
        confirmLoading={isLoading}
        maskClosable={false}
        visible={isShow}
        onOk={this.handleOk}
        onCancel={this.handleCancel}>
        <RadioGroup onChange={this.onChange} value={this.state.value}>
          {
            this.state.invalidResions.map((item, index) =>
              <Radio key={index} value={item.value}>{item.key}</Radio>
            )
          }
        </RadioGroup>
        <Input type="textarea" rows={6} placeholder='请输入无效的原因' style={{marginTop: '10px'}} onChange={this.getText} />

      </Modal>
    )
  }
}

function mapStateToProps(state) {
  return {
    // basicData: state.orgData,
    isShow: state.search.isShow,
    isLoading: state.search.isLoading,
    currentCustomer: state.search.currentCustomer,
    type: state.search.type,
    rootBasicData: state.rootBasicData,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(CustomerLoss);