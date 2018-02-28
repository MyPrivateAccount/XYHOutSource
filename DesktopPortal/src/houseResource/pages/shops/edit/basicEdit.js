// author (chensijing)
// 用于填写商铺基本信息的表单

import { connect } from 'react-redux';
import React, { Component } from 'react';
import './editCommon.less';
import { saveShopBasicAsync, getBuildingsListAsync, viewShopBasic, shopBasicLoadingStart} from '../../../actions/actionCreator';
import {
  Layout, Table, Button, Checkbox, Radio,
  Popconfirm, Tooltip, Form, Input,
  Icon, Cascader, Select, InputNumber,
  Row, Col,notification
} from 'antd';

const { Header, Sider, Content } = Layout;
const FormItem = Form.Item;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const RadioButton = Radio.Button;

class BasicEdit extends Component {
  state = {
    disabled: false, // 是否赠送
    disabledStree: false, // 是否临街 
    loading: false,
    floorNos: [],// 楼层
    buildNos: [],
    hasFree: false,
    isFaceStreet: false,
    buildingId: '',
  }


  buildingNoCompare = (property) => {
    return function(a, b) {
      var value1 = parseInt(a[property])
      var value2 = parseInt(b[property])
      return value1 - value2
    }
  }

  componentWillMount() {
    this.setState({
      buildingId: (this.props.shopsInfo || {}).buildingId
    }, () => {
      console.log(this.state.buildingId, '刚进来')
    })
    //楼层
    let fls = [];
    for(let i = -10;i<=50;i++){
        if(i===0)
        {    
            continue;
        }
        if(i<0){
            fls.push({label: `负${Math.abs(i)}层`, value: i})
        }else{
            fls.push({label: `${i}层`, value: i})
        }
    }
    this.setState({floorNos: fls})

    // 楼栋
    this.initDataSource(this.props.dataSource)

    const basicInfo = this.props.basicInfo;
    if (Object.keys(basicInfo).length !== 0) {
      this.setState({
        hasFree: basicInfo.hasFree,
        isFaceStreet: basicInfo.isFaceStreet,
      })
    }
  }

  initDataSource = (data) => {
    if (data.length === 0) {
      notification.warning({
        message: '此楼盘下面尚未创建楼栋，请先在楼盘中添加楼栋列表，再创建商铺',
        duration: 3
      })
    } 
    else {
      let buildNos = []
      data.map(v => {
         buildNos.push(v)
      })
      this.setState({
        buildNos : buildNos.sort(this.buildingNoCompare('storied'))
      })
    }
  }
  componentWillReceiveProps(newProps) {
    if(newProps.shopsInfo.buildingId !== this.state.buildingId) {
      this.initDataSource(newProps.dataSource)
      this.setState({
        buildingId: (newProps.shopsInfo || {}).buildingId
      }, () => {
        console.log(this.state.buildingId, '嗯呐')
      })
    }
  }

  onChange = (e) => { // 是否赠送
    this.setState({ hasFree: e.target.value })
  }
  onChangeStreet = (e) => { // 是否临街 
    this.setState({ isFaceStreet: e.target.value })
  }
  handleCancel = (e) => {
    this.props.viewShopBasic();
  }
  handleSave = (e) => {
    e.preventDefault();
    let { basicOperType } = this.props.operInfo;
    let shopsInfo = this.props.shopsInfo;
    this.props.form.validateFields((err, values) => {
      if (!err) {
        // this.setState({ loading: true });
        this.props.dispatch(shopBasicLoadingStart())
        let info = Object.assign({}, values, { id: shopsInfo.id, buildingId: shopsInfo.buildingId })
        this.props.save({ basicOperType: basicOperType, entity: info })
      }
    })
  }

  render() {
    const { getFieldDecorator } = this.props.form;
    const formItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 6 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 14 },
      },
    };
    const basicInfo = this.props.basicInfo;
    if (JSON.stringify(basicInfo) !== '{}') {
      basicInfo.saleStatus = (basicInfo.saleStatus).toString()
    }
    const basicOperType = this.props.operInfo.basicOperType;
    const shopsInfo = this.props.shopsInfo;
    return (
      <div className="relative">
        <Layout>
          <Content className='basicEdit' style={{ padding: '25px 0', margin: '20px 0', backgroundColor: "#ECECEC" }}>
            <Icon type="tags-o" className='content-icon' /> <span className='content-title'>基本信息</span>
            <Form ref={(e) => this.form = e} onSubmit={this.handleSubmit}>
              <Row type="flex" justify="space-between" style={{ paddingTop: "25px" }}>
                <Col span={12}>

                  <FormItem    {...formItemLayout} label="所属楼盘" >
                    {getFieldDecorator('buildingId', {
                      initialValue: shopsInfo.buildName,
                      rules: [{ required: true, message: '请输入所属楼盘' },],
                    })(
                      <Input className='someInput' disabled={true} />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="所属楼栋" >
                    {getFieldDecorator('buildingNo', {
                      initialValue: basicInfo.buildingNo,
                      rules: [{ required: true, message: '请输入所属楼栋' },],
                    })(
                      <Select size="large" className='someInput'>
                        {
                          this.state.buildNos.map((item) => 
                          <Option key={item.storied}>{item.storied}</Option>)
                        }
                      </Select>
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="所属楼层" >
                    {getFieldDecorator('floorNo', {
                      initialValue: basicInfo.floorNo,
                      rules: [{ required: true, message: '请输入所属楼层' },],
                    })(
                      <Select size="large" className='someInput'>
                      {
                        this.state.floorNos.map((item) => 
                        <Option key={item.value}>{item.label}</Option>)
                      }
                      </Select>
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="铺面编号" >
                    {getFieldDecorator('number', {
                      initialValue: basicInfo.number,
                      rules: [{ required: true, message: '请输入铺面编号' },],
                    })(
                      <InputNumber min={0} className='someInput' placeholder='不用输入楼栋及楼层'/>
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="商铺类别" >
                    {getFieldDecorator('shopCategory', {
                      initialValue: basicInfo.shopCategory,
                      rules: [{ required: true, message: '请选择商铺类别' },],
                    })(
                      <Select size="large" className='someInput'>
                        {
                          this.props.basicData.shopsTypes.map((item) =>
                            <Option key={item.value}>{item.key}</Option>)
                        }
                      </Select>
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="销售状态">
                    {getFieldDecorator('saleStatus', {
                      initialValue: basicInfo.saleStatus,
                      rules: [{ required: true, message: '请输入销售状态' },],
                    })(
                      <Select size="large" className='someInput'>
                        {
                          this.props.basicData.shopSaleStatus.map((item) =>
                            <Option key={item.value}>{item.key}</Option>)
                        }
                      </Select>
                      )}
                  </FormItem>
                 
                  <FormItem    {...formItemLayout} label="内部最低总价(元)">
                    {getFieldDecorator('guidingPrice', {
                      initialValue: basicInfo.guidingPrice,
                      rules: [{ required: true, message: '请输入内部最低总价' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="标准总价(元)">
                    {getFieldDecorator('totalPrice', {
                      initialValue: basicInfo.totalPrice,
                      rules: [{ required: true, message: '请输入楼盘总价' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="建筑面积(㎡)">
                    {getFieldDecorator('buildingArea', {
                      initialValue: basicInfo.buildingArea,
                      rules: [{ required: true, message: '请输入建筑面积' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="套内面积(㎡)">
                    {getFieldDecorator('houseArea', {
                      initialValue: basicInfo.houseArea,
                      rules: [{ required: true, message: '请输入套内面积' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  



                </Col>

                <Col span={12}>
                  <FormItem    {...formItemLayout} label="铺内层数(层)">
                    {getFieldDecorator('floors', {
                      initialValue: basicInfo.floors,
                      rules: [{ required: true, message: '请输入层数' },],
                    })(
                      <Input className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="朝向">
                    {getFieldDecorator('toward', {
                      initialValue: basicInfo.toward,
                      rules: [{ required: true, message: '请输入朝向' },],
                    })(
                      <Select size="large" className='someInput'>
                        {
                          this.props.basicData.shopTword.map((item) =>
                            <Option key={item.value}>{item.key}</Option>)
                        }
                      </Select>

                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="面宽(m)">
                    {getFieldDecorator('width', {
                      initialValue: basicInfo.width,
                      rules: [{ required: true, message: '请输入面宽' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="进深(m)">
                    {getFieldDecorator('depth', {
                      initialValue: basicInfo.depth,
                      rules: [{ required: true, message: '请输入进深' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="层高(m)">
                    {getFieldDecorator('height', {
                      initialValue: basicInfo.height,
                      rules: [{ required: true, message: '请输入层高' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="可外摆区域(㎡)">
                    {getFieldDecorator('outsideArea', {
                      initialValue: basicInfo.outsideArea,
                      rules: [{ required: true, message: '请输入可外摆区域' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="赠送">
                    {getFieldDecorator('hasFree', {
                      initialValue: basicInfo.hasFree,
                      // rules: [{ required: true, message: '请勾选是否赠送' },],
                    })(
                      <RadioGroup onChange={this.onChange} >
                        <Radio value={true}>是</Radio>
                        <Radio value={false}>否</Radio>
                      </RadioGroup>
                      )}
                  </FormItem>
                  {
                    this.state.hasFree ? 
                    <FormItem    {...formItemLayout} label="赠送面积(㎡)">
                    {getFieldDecorator('freeArea', {
                      initialValue: basicInfo.freeArea,
                      // rules: [{ validator: this.checkHasFree },],
                      rules: [{ required: true, message: '请输入赠送面积' },],
                    })(
                      <InputNumber min={0} className='someInput'  />
                      )}
                  </FormItem> : null
                  }
                  
                  <FormItem    {...formItemLayout} label="临街">
                    {getFieldDecorator('isFaceStreet', {
                      initialValue: basicInfo.isFaceStreet,
                      // rules: [{ required: true, message: '请勾选是否临街' },],
                    })(
                      <RadioGroup onChange={this.onChangeStreet} >
                        <Radio value={true}>是</Radio>
                        <Radio value={false}>否</Radio>
                      </RadioGroup>
                      )}
                  </FormItem>
                  {
                    this.state.isFaceStreet ? 
                    <FormItem    {...formItemLayout} label="临街距离">
                    {getFieldDecorator('streetDistance', {
                      initialValue: basicInfo.streetDistance,
                      // rules: [{ validator: this.checkIsFaceStreet },],
                      rules: [{ required: true, message: '请输入临街距离' },],
                    })(
                      <InputNumber min={0} className='someInput' />
                      )}
                  </FormItem> : null
                  }
                 <FormItem    {...formItemLayout} label="拐角铺">
                    {getFieldDecorator('isCorner', {
                      initialValue: basicInfo.isCorner,
                      // rules: [{ required: true, message: '请勾选是否是拐角铺' },],
                    })(
                      <RadioGroup onChange={this.onChange} >
                        <Radio value={true}>是</Radio>
                        <Radio value={false}>否</Radio>
                      </RadioGroup>
                      )}
                  </FormItem>
                  <FormItem    {...formItemLayout} label="双边街">
                    {getFieldDecorator('hasStreet', {
                      initialValue: basicInfo.hasStreet,
                      // rules: [{ required: true, message: '请勾选是否是双边街' },],
                    })(
                      <RadioGroup onChange={this.onChange} >
                        <Radio value={true}>是</Radio>
                        <Radio value={false}>否</Radio>
                      </RadioGroup>
                      )}
                  </FormItem>


                 
                 
                 


                  
                </Col>
              </Row>
            </Form>
            <Row type="flex" justify="center" className='BtnTop'>
              <Button type="primary" size='default' 
              style={{ width: "8rem" }} onClick={this.handleSave} 
              loading={this.props.loading}>保存</Button>
              {basicOperType !== 'add' ? <Button size='default' className='oprationBtn' onClick={this.handleCancel}>取消</Button> : null}
            </Row>

          </Content>
        </Layout>
      </div>
    )
  }
}

function mapStateToProps(state) {
  return {
    dataSource: state.shop.buildingNoInfos,
    basicInfo: state.shop.shopsInfo.basicInfo,
    buildingList: state.shop.buildingList,
    operInfo: state.shop.operInfo,
    shopsInfo: state.shop.shopsInfo,
    basicData: state.basicData,
    loading: state.shop.basicloading
  }
}

const mapDispatchToProps = (dispatch) => {
  return {
    dispatch,
    save: (...args) => dispatch(saveShopBasicAsync(...args)),
    getBuildingsList: () => dispatch(getBuildingsListAsync()),
    viewShopBasic: () => dispatch(viewShopBasic())
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(BasicEdit));