import { connect } from 'react-redux';
import { getAreaList, saveBuildingBasic, viewBuildingBasic,basicLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Icon, Input, InputNumber, Button, Checkbox, Row, Col, Form, DatePicker, Cascader } from 'antd'
import moment from 'moment';

const FormItem = Form.Item;

class BasicEdit extends Component {
    state = {
        loadingState: false,
        areaFullName: '',
    }
    componentWillMount() {
        if (this.props.basicData.areaList.length == 0) {
            this.props.dispatch(getAreaList());
        }
    }
    onAreaChange = (v, selectedOptions) => {
        let text = selectedOptions.map(item => {
            return item.label
        })
        this.setState({areaFullName: text.join('-')})
    }
    handleCancel = (e) => {
        this.props.dispatch(viewBuildingBasic());
    }
    checkMinPrice = (rule, value, callback) => { // 售价不能低于1000
        const form = this.props.form;
        const minPrice = form.getFieldValue('minPrice');
        if ( minPrice < 1000 ) {
          callback('价格不能低于1000')
        } else {
          callback()
        }
    }
    checkMaxPrice = (rule, value, callback) => { // 最高售价不能小于1000并且不能小于最低售价
        const form = this.props.form;
        const maxPrice = form.getFieldValue('maxPrice');
        const minPrice = form.getFieldValue('minPrice');
        if ( maxPrice < 1000 ) {
          callback('价格不能低于1000')
        } else if( maxPrice < minPrice ) {
            callback('最高售价不能小于最低售价')
        } else {
          callback()
        }
      }
    handleSave = (e) => {
        e.preventDefault();
        let { basicOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // this.setState({ loadingState: true });
                this.props.dispatch(basicLoadingStart())
                let newBasicInfo = values;
                newBasicInfo.id = this.props.buildInfo.id;
                if (basicOperType != 'add') {
                    newBasicInfo = Object.assign({}, this.props.buildInfo.buildingBasic, values);
                }
                newBasicInfo.city = values.location[0];
                newBasicInfo.district = values.location[1];
                newBasicInfo.area = values.location[2];
                newBasicInfo.areaFullName = this.state.areaFullName
                let method = (basicOperType === 'add' ? 'POST' : "PUT");
                this.props.dispatch(saveBuildingBasic({ 
                    method: method, 
                    entity: newBasicInfo, 
                    ownCity: this.props.user.City 
                }));
            }
        });
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const basicInfo = this.props.buildInfo.buildingBasic;
        let areaOption = []
        if (this.props.basicData.areaList.length !== 0) {
            if(this.props.user.City) {
                areaOption.push(this.props.basicData.areaList.find(x => x.value === this.props.user.City))
            } else {
                areaOption = this.props.basicData.areaList
            }
        }

        let { basicOperType } = this.props.operInfo;
        return (
            <Form layout="horizontal" style={{padding: '25px 0', marginTop: "25px"}}>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>基本信息 (必填)</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>楼盘名称</span>} >
                            {getFieldDecorator('name', {
                                initialValue: basicInfo.name,
                                rules: [{ required: true, message: '请输入楼盘名称!' }],
                            })(
                                <Input placeholder="楼盘名称" />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>区域</span>} >
                            {getFieldDecorator('location', {
                                initialValue: basicInfo.location,
                                rules: [{ required: true, message: '请输入楼盘名称!' }],
                            })(
                                <Cascader options={areaOption} onChange={this.onAreaChange} changeOnSelect placeholder="请选择片区" style={{ width: '90%' }} />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row type="flex">
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="总户数">
                            {getFieldDecorator('houseHolds', {
                                initialValue: basicInfo.houseHolds,
                                rules: [{ required: true, message: '请输入总户数!' }],
                            })(
                                <InputNumber min={1} max={100000} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <Row>
                            <Col span={12} push={3}>
                                <FormItem {...formItemLayout} label="售价(元/㎡)">
                                    {getFieldDecorator('minPrice', {
                                        initialValue: basicInfo.minPrice,
                                        rules: [{ validator: this.checkMinPrice }],
                                        rules: [{ required: true, message: '请输入售价!' }],
                                    })(
                                        <InputNumber min={1} max={1000000} style={{ width: '80%' }} placeholder='最低售价'/>
                                        )}
                                </FormItem>
                            </Col>
                            <Col span={12} pull={2} className='maxPrice'>
                                <FormItem {...formItemLayout} label="到">
                                    {getFieldDecorator('maxPrice', {
                                        initialValue: basicInfo.maxPrice,
                                        rules: [{ validator: this.checkMaxPrice }],
                                        // rules: [{ required: true, message: '请输入最高售价!' }],
                                    })(
                                        <InputNumber min={1} max={10000000} style={{ width: '80%' }} placeholder='最高售价'/>
                                        )}
                                </FormItem>
                            </Col>
                        </Row>

                    </Col>
                   
                </Row>
                <Row type="flex">
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="开盘时间">
                            {getFieldDecorator('openDate', {
                                initialValue: basicInfo.openDate?moment(basicInfo.openDate, 'YYYY-MM-DD'):null,
                                // rules: [{ required: true, message: '请选择开盘时间!' }],
                            })(
                                <DatePicker format='YYYY-MM-DD' />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="交房时间">
                            {getFieldDecorator('deliveryDate', {
                                initialValue: basicInfo.deliveryDate?moment(basicInfo.deliveryDate, 'YYYY-MM-DD'):null,
                                rules: [{ required: true, message: '请选择交房时间!' }],
                            })(
                                <DatePicker format='YYYY-MM-DD' />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row type="flex">
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="产权年限(年)">
                            {getFieldDecorator('propertyTerm', {
                                initialValue: basicInfo.propertyTerm,
                                rules: [{ required: true, message: '请输入产权年限!' }],
                            })(
                                <InputNumber min={1} max={200} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="土地到期时间">
                            {getFieldDecorator('landExpireDate', {
                                initialValue: basicInfo.landExpireDate?moment(basicInfo.landExpireDate, 'YYYY-MM-DD'):null,
                                rules: [{ required: true, message: '请输入产权年限!' }],
                            })(
                                <DatePicker format='YYYY-MM-DD' />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row type="flex">
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="占地面积(㎡)">
                            {getFieldDecorator('floorSurface', {
                                initialValue: basicInfo.floorSurface,
                                rules: [{ required: true, message: '请输入占地面积!' }],
                            })(
                                <InputNumber min={1} max={10000000} />
                                )}
                        </FormItem>
                    </Col>

                    <Col span={12}>
                        <FormItem {...formItemLayout} label="建筑面积(㎡)">
                            {getFieldDecorator('builtupArea', {
                                initialValue: basicInfo.builtupArea,
                                rules: [{ required: true, message: '请输入建筑面积!' }],
                            })(
                                <InputNumber min={1} max={10000000} />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="容积率">
                            {getFieldDecorator('plotRatio', {
                                initialValue: basicInfo.plotRatio,
                                // rules: [{ required: true, message: '请输入容积率!' }],
                            })(
                                <InputNumber min={0} max={100} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="绿化率(%)">
                            {getFieldDecorator('greeningRate', {
                                initialValue: basicInfo.greeningRate,
                                // rules: [{ required: true, message: '请输入绿化率!' }],
                            })(
                                <InputNumber min={0} max={100} />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="地下停车位(个)">
                            {getFieldDecorator('basementParkingSpace', {
                                initialValue: basicInfo.basementParkingSpace,
                                rules: [{ required: true, message: '请输入停车位!' }],
                            })(
                                <InputNumber min={0} max={100000} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="地面停车位(个)">
                            {getFieldDecorator('parkingSpace', {
                                initialValue: basicInfo.parkingSpace,
                                rules: [{ required: true, message: '请输入停车位!' }],
                            })(
                                <InputNumber min={0} max={100000} />
                                )}
                        </FormItem>
                    </Col>

                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="楼栋总数(栋)">
                            {getFieldDecorator('buildingNum', {
                                initialValue: basicInfo.buildingNum,
                                rules: [{ required: true, message: '请输入楼栋总数!' }],
                            })(
                                <InputNumber min={1} max={100000} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="商铺总数(个)">
                            {getFieldDecorator('shops', {
                                initialValue: basicInfo.shops,
                                rules: [{ required: true, message: '请输入商铺总数!' }],
                            })(
                                <InputNumber min={1} max={100000} />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="物业公司">
                            {getFieldDecorator('pmc', {
                                initialValue: basicInfo.pmc,
                                rules: [{ required: true, message: '请输入物业公司!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="物业费(元/㎡)">
                            {getFieldDecorator('pmf', {
                                initialValue: basicInfo.pmf,
                                rules: [{ required: true, message: '请输入物业费!' }],
                            })(
                                <InputNumber min={1} max={100000} />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="开发商">
                            {getFieldDecorator('developer', {
                                initialValue: basicInfo.developer,
                                rules: [{ required: true, message: '请输入开发商!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="楼盘地址">
                            {getFieldDecorator('address', {
                                initialValue: basicInfo.address,
                                rules: [{ required: true, message: '请输入楼盘地址!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                </Row>

                <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" htmlType="submit" loading={this.props.loadingState} style={{width: "8rem"}} onClick={this.handleSave}>保存</Button>
                        {basicOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
                </Row>
            </Form>
        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.building.buildInfo,
        user: (state.oidc.user || {}).profile || {},
        basicData: state.basicData,
        operInfo: state.building.operInfo,
        loadingState: state.building.basicloading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedForm = Form.create()(BasicEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);