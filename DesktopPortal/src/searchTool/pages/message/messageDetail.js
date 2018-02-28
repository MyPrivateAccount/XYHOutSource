import { connect } from 'react-redux';
import { } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Tag, Badge, Popconfirm } from 'antd'
import moment from 'moment';

class MessageDetail extends Component {
    state = {
    }
    componentWillMount() {
        //this.props.dispatch(getMsgList());
    }
    render() {
        let msgDetail = this.props.msgDetail;
        console.log("msgDetail:", msgDetail);
        return (
            <div>
                <h2>{msgDetail.title}</h2>
                <p className='msgContent'>
                    {msgDetail.content}
                </p>
                <div style={{ textAlign: 'right', marginRight: '20px' }}>发布日期：{moment(msgDetail.createTime).format("YYYY-MM-DD HH:mm:ss")}</div>
            </div>
        )
    }

}

function mapStateToProps(state) {
    return {
        msgDetail: state.search.msgDetail
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MessageDetail);