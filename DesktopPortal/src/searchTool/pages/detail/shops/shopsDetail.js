import { connect } from 'react-redux';
// import { } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Icon, Radio } from 'antd'
import AttachInfo from './attachInfo';
import BasicInfo from './basicInfo';
import LeaseInfo from './leaseInfo';
import SupportInfo from './supportInfo';
import './editCommon.less';
import '../building/buildingDish.less';

const { Header, Sider, Content } = Layout;

class ShopsDetail extends Component {
    state = {

    }
    componentWillMount() {

    }
    render() {
        return (
            <div className="relative">
                <Layout>
                    <Content className='content buildingDish'>
                        <div>
                            <Row>
                                {/**
                                 * 基本信息
                                 */}
                                <Col span={24}><BasicInfo /></Col>
                            </Row>
                            <Row>
                                {/**
                                 * 租约信息
                                 */}
                                <Col span={24}><LeaseInfo /></Col>
                            </Row>
                            <Row>
                                {/**
                                 * 配套设置
                                 */}
                                <Col span={24}><SupportInfo /></Col>
                            </Row>
                            <Row>
                                {/**
                                 * 附加信息
                                 */}
                                <Col span={24}><AttachInfo /></Col>
                            </Row>
                        </div>
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    console.log('shopsMapStateToProps:' + JSON.stringify(state));
    return {

    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ShopsDetail);