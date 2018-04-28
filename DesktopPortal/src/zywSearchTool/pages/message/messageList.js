import { connect } from 'react-redux';
import { openMsgDetail, getMsgDetail, getMsgList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Pagination } from 'antd'
import '../searchIndex.less';
import moment from 'moment';

class MessageList extends Component {
    state = {
        condition: { pageIndex: 0, pageSize: 10 },
        pagination: { current: 1, pageSize: 10, total: 0 }
    }
    // componentWillReceiveProps(newProps) {
    //     // let paginationInfo = {
    //     //     pageSize: newProps.msgList.pageSize,
    //     //     current: (newProps.msgList.pageIndex + 1),
    //     //     total: newProps.msgList.totalCount
    //     // }
    //     // this.setState({ pagination: paginationInfo });
    //     // console.log("paginationInfo:========", paginationInfo);
    // }

    handleMsgClick = (msg) => {
        this.props.dispatch(openMsgDetail(msg));
        this.props.dispatch(getMsgDetail(msg.id));
    }
    //翻页处理
    handlePageChange = (pageIndex, pageSize) => {
        let condition = { ...this.state.condition };
        condition.pageIndex = (pageIndex - 1);
        this.setState({ condition: condition });
    }
    render() {
        let msgList = this.props.msgList.extension || [];
        //console.log("msgList:", this.props.msgList);
        let paginationInfo = {
            pageSize: this.props.msgList.pageSize,
            current: this.props.msgList.pageIndex,
            total: this.props.msgList.totalCount
        }
        return (
            <div className="searchResult">
                {
                    msgList.map(msg => {
                        return (
                            <div className='itemBorder' key={msg.id}>
                                <Row>
                                    <Col><b className='title' onClick={(e) => this.handleMsgClick(msg)}>{msg.title}</b></Col>
                                </Row>
                                <Row style={{ marginTop: '1rem' }}>
                                    <Col span={20}>{msg.ext1}</Col>
                                    <Col span={4}>{moment(msg.createTime).format("YYYY-MM-DD HH:mm:ss")}</Col>
                                </Row>
                            </div>
                        )
                    })
                }

                <Pagination  {...paginationInfo} onChange={this.handlePageChange} />

            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        msgList: state.search.msgList
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MessageList);