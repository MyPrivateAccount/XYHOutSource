import { connect } from 'react-redux';
// import { } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Icon, Radio } from 'antd'
import AttachInfo from './attachInfo';
import BasicInfo from './basicInfo';
import LeaseInfo from './leaseInfo';
import SupportInfo from './supportInfo';
import { shopsInfoGroup } from '../../../constants/commonDefine';

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
                    <Content className='content'>

                        <div>
                            <Row>
                                <Col span={12}>
                                    {this.props.shopInfo.examineStatus === 8 ? null :
                                        <Button type="primary" size='large' style={{ width: "10rem" }}>提交</Button>
                                    }
                                </Col>
                                <Col span={12}>
                                    <Radio.Group defaultValue='basicInfo' onChange={this.handleSizeChange}>
                                        {
                                            shopsInfoGroup.map((info) => <Radio.Button key={info.id} value={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                        }
                                    </Radio.Group></Col>
                            </Row>
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
    // console.log('shopsMapStateToProps:' + JSON.stringify(state));
    return {
        shopInfo: state.shop.shopsInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ShopsDetail);