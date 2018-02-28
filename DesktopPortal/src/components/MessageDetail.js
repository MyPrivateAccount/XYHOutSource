import React, { Component } from 'react'
import { Modal, Row, Col, Pagination, Spin, Menu, Badge } from 'antd';
import { connect } from 'react-redux';
import { setLoadingVisible, getRecieveList } from '../actions/actionCreators';
import moment from 'moment';

class MessageDetail extends Component {
    state = {
        visible: false
    }
    componentDidMount() {

    }
    handleCancel = (e) => {
        console.log(e);
        this.setState({ visible: false });
        if (this.props.onMessageClose) {
            this.props.onMessageClose();
        }
    }
    componentWillReceiveProps = (newProps) => {

    }

    render() {
        let msgDetail = this.props.msgDetail;
        msgDetail.sendTime = moment(msgDetail.sendTime).format("YYYY-MM-DD HH:mm:ss");
        return (
            <Modal title="系统消息" visible={this.props.visible} footer={null}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Spin spinning={this.props.showLoading}>
                    {/*消息详细*/}
                    <div>
                        <Row>
                            <Col style={{ textAlign: 'center', margin: '5px' }}><h2>{msgDetail.title}</h2></Col>
                            <Col style={{ margin: '10px' }}><p>{msgDetail.content}</p></Col>
                            <Col>
                                <Row>
                                    <Col span={18}>发送人：{msgDetail.sendUserTrueName}</Col>
                                    <Col span={6}>{msgDetail.sendTime}</Col>
                                </Row>
                            </Col>
                        </Row>
                    </div>
                </Spin>
            </ Modal >
        )
    }
}


const mapStateToProps = (state, props) => {
    return {
        msgList: state.message.receiveList,
        showLoading: state.message.showLoading,
        msgDetail: state.message.msgDetail
    }
}
const mapDispatchToProps = (dispatch) => {
    return {
        dispatch
    }

}
export default connect(mapStateToProps, mapDispatchToProps)(MessageDetail);
