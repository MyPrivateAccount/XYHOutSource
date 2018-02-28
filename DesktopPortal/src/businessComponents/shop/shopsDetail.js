import React, {Component} from 'react'
import {Layout, Row, Col} from 'antd'
import AttachInfo from './attachInfo';
import BasicInfo from './basicInfo';
import LeaseInfo from './leaseInfo';
import SupportInfo from './supportInfo';
import './editCommon.less';
import '../building/buildingDish.less';

const {Header, Sider, Content} = Layout;

class ShopsDetail extends Component {
    state = {

    }
    componentWillMount() {

    }
    render() {
        const shopInfo = (this.props.shopInfo || {});
        return (
            <div className="relative">
                <Layout>
                    <Content className='content buildingDish'>
                        <div>
                            <Row>
                                {/**
                                 * 基本信息
                                 */}
                                <Col span={24}><BasicInfo shopInfo={shopInfo} /></Col>
                            </Row>
                            <Row>
                                {/**
                                 * 租约信息
                                 */}
                                <Col span={24}><LeaseInfo shopInfo={shopInfo} /></Col>
                            </Row>
                            <Row>
                                {/**
                                 * 配套设置
                                 */}
                                <Col span={24}><SupportInfo shopInfo={shopInfo} /></Col>
                            </Row>
                            <Row>
                                {/**
                                 * 附加信息
                                 */}
                                {/* <Col span={24}><AttachInfo shopInfo={shopInfo} /></Col> */}
                            </Row>
                        </div>
                    </Content>
                </Layout>
            </div>
        )
    }
}

export default ShopsDetail;