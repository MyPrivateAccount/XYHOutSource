import { connect } from 'react-redux';
// import { } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Col, Row, Icon } from 'antd'
import './editCommon.less'

const { Header, Sider, Content } = Layout;

class AttachInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }

    render() {
        let showResult = this.props.showResult;
        let attachList = showResult.showBuildingDetal ? this.props.activeBuilding.attachmentList : this.props.activeShop.attachmentList;
        attachList = attachList || [];
        return (
            <Layout>
                <Content className='content' style={{ backgroundColor: "#ECECEC" }}>
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={23} >
                            <Icon type="tags-o" className='content-icon' /> <span className='content-title'>附加信息</span>
                        </Col>
                        <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({ expandStatus: !this.state.expandStatus })} /></Col>
                    </Row>
                    <Row style={{ marginTop: '25px' }} style={{ display: this.state.expandStatus ? "block" : "none" }}>
                        {/* {attachInfo.pic ?
                            <div className='picViewBox'>
                                <div className='viewPic'>
                                    <img src='https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png' />
                                </div>

                            </div>
                            : null
                        } */}
                        <Col>
                            {
                                attachList.map(attach => <div><Row key={attach.fileGuid}>
                                    <Col span={4}>附件名称：{attach.fileName}</Col>
                                    <Col span={20}><a>查看</a></Col>
                                </Row>
                                    <Row>
                                        <Col span={4}>附件描述：</Col>
                                        <Col span={20}>{attach.summary}</Col>
                                    </Row></div>)
                            }
                        </Col>
                    </Row>
                </Content>
            </Layout>
        )
    }
}

function mapStateToProps(state) {
    return {
        activeBuilding: state.search.activeBuilding,
        activeShop: state.search.activeShop,
        showResult: state.search.showResult
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AttachInfo);