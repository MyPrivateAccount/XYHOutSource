import { connect } from 'react-redux';
import { saveShopSummaryInfo, viewShopSummaryInfo,shopSummaryLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Form, Input, Row, Col, Icon } from 'antd'

const FormItem = Form.Item;

class ShopSummaryEdit extends Component {
    state = {
        loadingState: false
    }
    componentWillMount() {

    }
    componentWillReceiveProps(newProps) {
        this.setState({ loadingState: false });
    }
    handleSave = (e) => {
        let { projectOperType } = this.props.operInfo;
        let shopsInfo = this.props.shopInfo;
        this.props.form.validateFields((err, values) => {
          if (!err) {
            // this.setState({ loadingState: true });
            this.props.dispatch(shopSummaryLoadingStart())
            let info = Object.assign({}, values, { id: shopsInfo.id, buildingId: shopsInfo.buildingId })
            this.props.dispatch(saveShopSummaryInfo({ projectOperType: projectOperType, entity: info }))
          }
        })
    }

    handleCancel = (e) => {
        this.props.dispatch(viewShopSummaryInfo());
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 0 },
            wrapperCol: { span: 24 },
        };
        let { projectOperType } = this.props.operInfo;
        let summary = this.props.shopInfo.summary;
        return (
            <Form layout="horizontal" style={{padding: '25px 0', margin: '20px 0',backgroundColor: "#ECECEC"}}>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>商铺简介</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Col span={1}></Col>
                    <Col span={22}>
                        <FormItem {...formItemLayout}>
                            {getFieldDecorator('summary', {
                                initialValue: summary,
                                rules: [{ required: true, message: '请输入商铺简介' }],
                            })(
                                <Input type="textarea" style={{ height: '100px' }} placeholder="请输入商铺简介!" />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={1}></Col>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button type="primary" htmlType="submit" 
                        disabled={this.props.shopInfo.isDisabled} 
                        loading={this.props.loadingState} style={{width: "8rem"}} onClick={this.handleSave}>保存</Button>
                        {projectOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
                </Row>
            </Form>
        )
    }
}

function mapStateToProps(state) {
    //console.log('BuildingDishDetail MapStateToProps:' + JSON.stringify(state));
    return {
        shopInfo: state.shop.shopsInfo,
        operInfo: state.shop.operInfo,
        loading: state.shop.summaryLoading
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedForm = Form.create()(ShopSummaryEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);