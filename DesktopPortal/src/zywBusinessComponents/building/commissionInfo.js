// import {connect} from 'react-redux';
// import {commissionEdit, getDynamicInfoList} from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Form, Icon, Row, Col, Layout} from 'antd'


const {Header, Sider, Content} = Layout;
class CommissionInfo extends Component {
    state = {
        expandStatus: true
    }
    render() {

        const buildingInfo = this.props.buildingInfo || {};
        let commissionPlan = buildingInfo.commissionPlan || '';

        return (
            <Layout>
                <Content className='content' >
                    <div style={{marginTop: '15px', backgroundColor: "#ECECEC"}}>
                        <Row type="flex" style={{padding: '1rem 0'}}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /><span className='content-title'>佣金方案</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <Row style={{padding: '20px 4rem', display: this.state.expandStatus ? "block" : "none"}}>
                            <Col span={24}>{commissionPlan || '暂无'}</Col>
                        </Row>
                    </div>
                </Content>
            </Layout>
        )
    }
}

export default CommissionInfo;