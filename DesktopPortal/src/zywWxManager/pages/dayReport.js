import React, {Component} from 'react'
import {Input, Row, Col, Layout, Button, Icon, Spin, Upload} from 'antd'
import {connect} from 'react-redux'
import {
    getAreaListAsync, setCurrentArea, delAreaAsync,
    addArea, editArea, viewArea
} from '../actions'

const reportStyle = {
    row: {
        margin: '10px',
    },
    input: {
        width: '200px'
    },
    tipInfo: {
        color: '#999',
        fontSize: '12px',
        marginBottom: '5px'
    }
}


class DayReport extends Component {

    componentWillMount() {

    }

    render() {
        const uploadButton = (
            <div>
                <Icon type="plus" style={{fontSize: '20px'}} />
                <div className="ant-upload-text">添加图片</div>
            </div>
        );
        let propsPic = {
            multiple: true,
            listType: "picture-card",
            fileList: [],
            onPreview: (file) => { // 图片预览
                this.setState({
                    previewImage: file.url || file.localUrl,
                    previewVisible: true,
                });
            }
        }
        return (
            <div className="inner-page full">
                <Row>
                    <Col span={24}><h2><Icon type="tags-o" />今日行情</h2></Col>
                </Row>
                <Row style={reportStyle.row}>
                    <Col span={4}>在转商铺：</Col>
                    <Col span={6}><Input style={reportStyle.input} /></Col>
                    <Col span={4}>今日新上（客）：</Col>
                    <Col span={6}><Input style={reportStyle.input} /></Col>
                </Row>
                <Row style={reportStyle.row}>
                    <Col span={4}>今日降价（客）：</Col>
                    <Col span={6}><Input style={reportStyle.input} /></Col>
                    <Col span={4}>今日成交（客：</Col>
                    <Col span={6}><Input style={reportStyle.input} /></Col>
                </Row>
                <Row>
                    <Col span={20} style={{textAlign: 'center'}}><Button type="primary">保存</Button> <Button type="primary">取消</Button></Col>
                </Row>
                <Row>
                    <Col span={24}><h2><Icon type="tags-o" />商铺换手热度</h2></Col>
                </Row>
                <Row style={reportStyle.row}>
                    <Col>
                        <Upload  {...propsPic} beforeUpload={this.handleBeforeUpload} onRemove={this.hanldeRemove}>
                            {uploadButton}
                        </Upload>
                    </Col>
                    <Col style={reportStyle.tipInfo}>支持jpg、png类型的文件，附加大小限制在1M以内。图片尺寸：600px * 450px，分辨率为72px。</Col>
                    <Col span={20} style={{textAlign: 'center'}}><Button type="primary">保存</Button> <Button type="primary">取消</Button></Col>
                </Row>
                {/* <AreaDialog /> */}
            </div>
        )
    }
}

const mapStateToProps = (state, props) => {
    return {
        area: state.area
    }
}

const mapDispatchToProps = (dispatch) => {
    return {
        getCityList: () => dispatch(getAreaListAsync({level: 1})),
        dispatch
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(DayReport);