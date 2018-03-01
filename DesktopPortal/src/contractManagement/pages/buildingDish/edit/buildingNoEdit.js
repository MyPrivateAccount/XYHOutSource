import { connect } from 'react-redux';
import {
  activeAdd, saveBatchBuildingAsync, viewBatchBuilding, saveMySelectedRows,batchBuildLoadingStart,
  comfirmCreateBuilding, changeCell
} from '../../../actions/actionCreator';
import React, { Component } from 'react'
import {
  Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Cascader, Select, Radio, Table,
  Tag, Tooltip, Modal
} from 'antd'
import moment from 'moment';
import '../components/edit.less'
import EditTimeTable from '../components/editTimeTable'


const FormItem = Form.Item;
const Option = Select.Option;
const CheckboxGroup = Checkbox.Group;
const RadioButton = Radio.Button;
const RadioGroup = Radio.Group;

class BuildingNoEdit extends Component {
  state = {
    loadingState: false,
    visibleModal: false,
    selectedRowKeys: [],
    mySelectedRows: [], // 选择的数据
    selectNum: '1', // 选择规则 1 数字 2 字母 3 自定义
    firstDate: null, // 开盘时间
    lastDate: null, // 交房时间
    defineCode: '', // 自定义中文
    defineStart: '', // 自定义开始楼栋
    defineEnd: '', // 自定义结束楼栋
    numStartValue: '', // 数字开始楼栋
    numEndValue: '', // 数字结束楼栋
    engStartValue: '', // 字母开始楼栋
    engEndValue: '', // 字母结束楼栋
    redText: true, // 验证交房时间必填
    flag: true,
    columns: [{
      title: '编号',
      dataIndex: 'storied'
    }, {
      title: '开盘时间',
      dataIndex: 'openDate',
      render: (text, record, index) => (
        <EditTimeTable
          value={text}
          onChange={this.onCellOpenDateChange(index, 'openDate')}
        />
      ),
    }, {
      title: '交房时间',
      dataIndex: 'deliveryDate',
      render: (text, record, index) => (
        <EditTimeTable
          value={text}
          onChange={this.onCellDeliveryDateChange(index, 'deliveryDate')}
        />
      ),
    }]
  }
  componentWillMount() {
    let selectedRowKeys = [];
    this.props.dataSource.map(buildingNo => {
      selectedRowKeys.push(buildingNo.storied);
    });
    // this.props.dispatch(saveMySelectedRows({ mySelectedRows: this.props.dataSource }))
    this.setState({ selectedRowKeys: selectedRowKeys, mySelectedRows: this.props.dataSource });
  }
  componentWillReceiveProps(newProps) {
    let selectedRowKeys = [];
    newProps.dataSource.map(buildingNo => {
      selectedRowKeys.push(buildingNo.storied);
    });
    // this.props.dispatch(saveMySelectedRows({ mySelectedRows: newProps.dataSource }))
    this.setState({ selectedRowKeys: selectedRowKeys, mySelectedRows: this.props.dataSource });
  }
  onCellOpenDateChange = (index, key) => {
    return (value) => {
      console.log(value, 'opendate')
      this.props.changeCell({ value, index, key })
    };
  }
  onCellDeliveryDateChange = (index, key) => {
    return (value) => {
      this.props.changeCell({ value, index, key })
    };
  }

  handleChange = (e) => { // 选择规则
    switch (e.target.value) {
      case '1': this.setState(Object.assign({}, this.state, { selectNum: '1' })); break;
      case '2': this.setState(Object.assign({}, this.state, { selectNum: '2' })); break;
      default: this.setState(Object.assign({}, this.state, { selectNum: '3' }))
    }

  }
  showModal = () => {
    this.setState({
      visibleModal: true,
      selectNum: '1',
      numStartValue: '',
      numEndValue: '',
      engStartValue: '',
      engEndValue: '',
      defineCode: '',
      defineStart: '',
      defineEnd: '',
      firstDate: null,
      lastDate: null,
      redText: true,
    });
  }
  handleOkModal = (v) => {
    console.log(this.state.redText, '点击')
    let { selectNum, firstDate, lastDate, defineStart, numStartValue, numEndValue,
      engStartValue, engEndValue, defineCode, defineEnd } = this.state
    if (lastDate) {
      this.props.comfirmCreateBuilding({
        selectNum: selectNum,
        firstDate: firstDate,
        lastDate: lastDate,
        numStartValue: numStartValue,
        numEndValue: numEndValue,
        engStartValue: engStartValue,
        engEndValue: engEndValue,
        defineCode: defineCode,
        defineStart: defineStart,
        defineEnd: defineEnd,
      })
      this.setState({
        visibleModal: false,
      });
    }
    else {
      this.setState({
        redText: false,
      });
    }
    //房源动态的处理情况
    if (this.state.mySelectedRows) {
      this.props.dispatch(saveMySelectedRows({ mySelectedRows: this.state.mySelectedRows }))
    }
  }
  handleCancelModal = (e) => {
    this.setState({ visibleModal: false });
  }
  firstTimeChange = (date, dateString) => {
    this.setState({ firstDate: date })
  }
  lastTimeChange = (date, dateString) => {
    this.setState({ lastDate: date }, () => {
      if (this.state.lastDate) { // 有值不显示
        this.setState({ redText: true })
      } else {
        this.setState({ redText: false })
      }
    })
  }
  onChangeDefine = (e) => {
    this.setState({ defineCode: e.target.value })
  }
  clickNum = (value) => {
    if (value === '1') {
      this.setState({
        flag: true,
        defineCode: this.state.defineCode + `[数字]`
      })
    }
  }
  clickEng = (value) => {
    if (value === '2') {
      this.setState({
        flag: false,
        defineCode: this.state.defineCode + `[字母]`
      })
    }
  }
  numStart = (value) => {
    this.setState({ numStartValue: value })
  }
  numEnd = (value) => {
    this.setState({ numEndValue: value })
  }
  engStart = (e) => {
    e.target.value = e.target.value.replace(/[^\a-\z\A-\Z]/g, '')
    this.setState({ engStartValue: e.target.value })
  }
  engEnd = (e) => {
    e.target.value = e.target.value.replace(/[^\a-\z\A-\Z]/g, '')
    this.setState({ engEndValue: e.target.value })
  }
  defineStartChange = (e) => {
    if (this.state.flag) {
      e.target.value = e.target.value.replace(/[^0-9]/g, '')
    } else {
      e.target.value = e.target.value.replace(/[^\a-\z\A-\Z]/g, '')
    }
    this.setState({ defineStart: e.target.value })
  }
  defineEndChange = (e) => {
    if (this.state.flag) {
      e.target.value = e.target.value.replace(/[^0-9]/g, '')
    } else {
      e.target.value = e.target.value.replace(/[^\a-\z\A-\Z]/g, '')
    }
    this.setState({ defineEnd: e.target.value })
  }
  handleCancel = (e) => {
    this.props.viewBatchBuilding();
  }

  handleSave = (e) => {
    e.preventDefault();
    this.props.dispatch(batchBuildLoadingStart())
    let { batchBuildOperType } = this.props.operInfo;
    let buildInfo = this.props.buildInfo;
    console.log(this.state.mySelectedRows, '录入楼盘的楼栋批次数据格式是什么')
    this.props.save({ 
      batchBuildOperType: batchBuildOperType,
      entity: this.state.mySelectedRows,
      id: buildInfo.id,
      ownCity: this.props.user.City 
    })
  }

  onSelectChange = (selectedRowKeys, selectedRows) => { // 勾选列表
    this.props.dispatch(saveMySelectedRows({ mySelectedRows: selectedRows }))
    this.setState({ mySelectedRows: selectedRows, selectedRowKeys }, () => {
      console.log(this.state.mySelectedRows, 'aaaa')
    })
    // this.setState({ mySelectedRows: selectedRows, selectedRowKeys })

  }

  render() {
    const formItemLayout = {
      labelCol: { span: 6 },
      wrapperCol: { span: 14 },
    }
    let { selectNum, firstDate, lastDate, numStartValue,
      numEndValue, engStartValue, defineStart, defineEnd,
      engEndValue, selectedRowKeys } = this.state
    let { mySelectedRows } = this.props
    const rowSelection = {
      selectedRowKeys,
      hideDefaultSelections: true,
      onChange: this.onSelectChange,
      getCheckboxProps: (record) => ({ defaultChecked: false })
    };

    let { batchBuildOperType } = this.props.operInfo;
    return (
      <Form layout="horizontal" style={{ padding: '25px 0', marginTop: "25px", backgroundColor: "#ECECEC" }} className='buildingNoEdit'>
        <Icon type="tags-o" className='content-icon' /> <span className='content-title'>楼栋批次信息</span>
        <Row type="flex" className='beforeContent' style={{ marginTop: "25px", marginBottom: '25px' }} >
          <Col span={24}>
            <Button onClick={this.showModal} type='primary' style={{ marginLeft: '50px' }} disabled={this.props.buildInfo.isDisabled}>批量生成楼栋</Button>
            <Modal title="楼栋批次" visible={this.state.visibleModal}
              onOk={() => this.handleOkModal(selectNum)} onCancel={this.handleCancelModal}>
              <FormItem {...formItemLayout} label="选择楼栋规则">
                <div>
                  <RadioGroup defaultValue={selectNum} onChange={this.handleChange} style={{ marginBottom: '10px' }}>
                    <RadioButton value="1">数字排序</RadioButton>
                    <RadioButton value="2">字母排序</RadioButton>
                    <RadioButton value="3">自定义排序</RadioButton>
                  </RadioGroup>
                  {
                    selectNum !== '3' ?
                      <div style={{ marginBottom: '10px' }} >
                        {selectNum === '1' ?
                          <p><span style={{color: 'red',paddingRight: '5px'}}>*</span>范围：
                          <InputNumber style={{ width: 50 }} min={1} value={numStartValue} onChange={this.numStart} placeholder='开始'/> <span> 至 </span> 
                          <InputNumber style={{ width: 50 }} min={1} value={numEndValue} onChange={this.numEnd}  placeholder='结束'/> </p>
                          :
                          <p><span style={{color: 'red',paddingRight: '5px'}}>*</span>范围：<Input style={{ width: 50 }} value={engStartValue} onChange={this.engStart} placeholder='开始'/> <span> 至 </span> 
                          <Input style={{ width: 50 }} value={engEndValue} onChange={this.engEnd} placeholder='结束'/>  (请统一大小写)</p>
                        }

                      </div>
                      :
                      <div>
                        <p className='defineSpan'>例如：当要输入南区1栋时, 就在输入框中输入南区，再点击一个下面的数字标签。
                                若输入南区A栋时, 就在输入框中输入南区，再点击一个下面的字母标签。</p>
                        <p className='defineInput'>
                          <Input style={{ width: 100 }} onChange={this.onChangeDefine} value={this.state.defineCode} />
                        </p>
                        <p className='tagBtn'>
                          <Button type="dashed" onClick={() => this.clickNum('1')} style={{ marginRight: '5px' }} size='small'>数字</Button>
                          <Button type="dashed" onClick={() => this.clickEng('2')} size='small'>字母</Button>
                        </p>
                        <p><span style={{color: 'red',paddingRight: '5px'}}>*</span>范围：<Input style={{ width: 50 }} onChange={this.defineStartChange} value={defineStart} placeholder='开始'/> <span> 至 </span> <Input style={{ width: 50 }} value={defineEnd} onChange={this.defineEndChange} placeholder='结束'/></p>
                      </div>

                  }
                  <p>
                  <span style={{paddingRight: '5px'}}></span>开盘时间：<DatePicker format='YYYY-MM-DD' onChange={this.firstTimeChange} value={firstDate} /> <br />
                    <span style={{color: 'red',paddingRight: '5px'}}>*</span>交房时间：<DatePicker format='YYYY-MM-DD' onChange={this.lastTimeChange} value={lastDate} /> <br />
                    {this.state.redText ? null : <span style={{ fontWeight: 'bold', color: 'red' }}>必须输入交房时间</span>}

                  </p>

                </div>
              </FormItem>
            </Modal>
          </Col>
        </Row>
        <Row style={{ display: this.props.dataSource.length !== 0 ? 'block' : 'none' }}>
          <Col span={24}>
            <div>
              <Table bordered style={{ width: '50%', marginLeft: '50px' }}
                rowKey={record => record.storied}
                dataSource={this.props.dataSource || null}
                columns={this.state.columns}
                rowSelection={rowSelection} />
            </div>
          </Col>
        </Row>
        {
          this.props.type === 'dynamic' ? null :
            <Row>
              <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                <Button type="primary" htmlType="submit"
                  loading={this.props.loadingState} disabled={this.props.buildInfo.isDisabled}
                  style={{ width: "8rem" }} onClick={this.handleSave}>保存</Button>
                {batchBuildOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
              </Col>
            </Row>
        }
      </Form>
    )
  }
}


function mapStateToProps(state) {
  // console.log(state.building.buildInfo, '基本信息', state.building.dataSource)
  return {
    dataSource: state.building.buildInfo.buildingNoInfos,
    buildInfo: state.building.buildInfo,
    editGroup: state.building.editGroup,
    basicData: state.basicData,
    operInfo: state.building.operInfo,
    optionsNum: state.building.optionsNum,
    optionsEng: state.building.optionsEng,
    mySelectedRows: state.building.mySelectedRows,
    loadingState: state.building.batchBuildloading,
    user: (state.oidc.user || {}).profile || {},
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch,
    activeAdd: (...args) => dispatch(activeAdd(...args)),
    viewBatchBuilding: () => dispatch(viewBatchBuilding()),
    save: (...args) => dispatch(saveBatchBuildingAsync(...args)),
    comfirmCreateBuilding: (...args) => dispatch(comfirmCreateBuilding(...args)),
    changeCell: (...args) => dispatch(changeCell(...args))
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(BuildingNoEdit));