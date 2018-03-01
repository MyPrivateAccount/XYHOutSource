import { connect } from 'react-redux';
// import { } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col, Upload, Modal } from 'antd'

const FormItem = Form.Item;

class AttachEdit extends Component {
    state = {
        previewVisible: false,
        previewImage: '',
        fileList: [{
            uid: -1,
            name: 'xxx.png',
            status: 'done',
            url: 'https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png',
        }],
    }
    componentWillMount() {

    }



    render() {
        let fileList = this.state.fileList;
        let previewVisible = this.state.previewVisible;
        let previewImage = this.state.previewImage;
        const uploadButton = (
            <div>
                <Icon type="plus" style={{ fontSize: '36px' }} />
                <div className="ant-upload-text">上传图片</div>
            </div>
        );
        return (
            <Form layout="horizontal" onSubmit={this.handleSubmit} >
            {
                this.props.type === 'dynamic' ? null : 
           <div>
                <Row type="flex" style={{ padding: '1rem 0' }}>
                    <Col span={12}>
                        <h2> <Icon type="tags-o" />附加信息</h2>
                    </Col>
                </Row>
                <Row type="flex" style={{ padding: '1rem 0' }}>
                    <Col span={24}>
                        <h3 style={{ backgroundColor: 'rgba(215, 215, 215, 1)', paddingLeft: '2rem' }}>图片</h3>
                    </Col>
                </Row>
            </div>
            }
                <Row type="flex">
                    <Col span={1}>
                    </Col>
                    <Col span={22}>
                        <div className="clearfix">
                            <Upload
                                action="//jsonplaceholder.typicode.com/posts/"
                                listType="picture-card"
                                fileList={fileList}
                                onPreview={this.handlePreview}
                                onChange={this.handleChange}
                            >
                                {fileList.length >= 3 ? null : uploadButton}
                            </Upload>
                            <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                                <img alt="example" style={{ width: '100%' }} src={previewImage} />
                            </Modal>
                        </div>
                    </Col>
                    <Col span={1}>
                    </Col>
                </Row>
                <Row type="flex" style={{ padding: '1rem 0' }}>
                    <Col span={24}>
                        <h3 style={{ backgroundColor: 'rgba(215, 215, 215, 1)', paddingLeft: '2rem' }}>附件</h3>
                    </Col>
                </Row>
                <Row type="flex">
                    <Col span={12}></Col>
                    <Col span={12}>

                    </Col>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }}>
                        <Button type="primary" htmlType="submit" className='formBtn'>保存</Button>
                        <Button className="login-form-button" className='formBtn'>取消</Button>
                    </Col>
                </Row>
            </Form>
        )
    }
}

function mapStateToProps(state) {
    //console.log('shopsMapStateToProps:' + JSON.stringify(state));
    return {

    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedForm = Form.create()(AttachEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);