//收款组件
import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Row, Col, Form, Input, Layout } from 'antd'
import validations from '../../../../utils/validations'

const FormItem = Form.Item;

class SJCp extends Component {

    state={
        entity:{},
        zhList: []
    }


    componentDidMount = ()=>{
        this.initEntity(this.props)
    }

    componentWillReceiveProps =(nextProps)=>{
        if(this.props.entity !== nextProps.entity){
            this.initEntity(nextProps)
        }
    }

    initEntity=(props)=>{
        let entity = props.entity ||{};
        this.setState({entity: entity},()=>{
            let fe = {
                sjhm: entity.sjhm,
                qtsj: entity.qtsj,
                sjbz: entity.sjbz
            }
            this.props.form.setFieldsValue(fe);
        })
    }

    getValues = ()=>{
       let r = validations.validateForm(this.props.form)
       return r;
    }

    
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        let type = this.props.type;
        let sjhmMustInput = type ==='sk';
        let canEdit = this.props.canEdit;

        return (
            <Layout>
                <Layout.Content>
                    <Row style={{ margin: 5 }}>
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>收据号码</span>)}>
                                {
                                    getFieldDecorator('sjhm', {
                                        rules: [{ required: sjhmMustInput, message: '必须输入收据号码' }],
                                    })(
                                        <Input disabled={!canEdit} style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>其它收据</span>)}>
                                {
                                    getFieldDecorator('qtsj', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input disabled={!canEdit}  style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('sjbz', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input.TextArea disabled={!canEdit}  rows={4} style={{ width: 510 }}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        ext: state.rp.ext,
        operInfo: state.rp.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(SJCp);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);