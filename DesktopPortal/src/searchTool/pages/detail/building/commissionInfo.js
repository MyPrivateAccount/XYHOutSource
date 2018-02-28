import { connect } from 'react-redux';
import { commissionEdit,getDynamicInfoList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Icon, Button, Checkbox, Row, Col,Layout,  } from 'antd'
import BasicInfo from './basicInfo';

const { Header, Sider, Content } = Layout;
class CommissionInfo extends Component {
    
    render() {
      
        const {buildInfo} = this.props
        let commissionPlan = buildInfo.commissionPlan || '';
        
        return (
            <Layout>
            <Content className='content' >
            <div style={{ marginTop: '25px', backgroundColor: "#ECECEC" }}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{ padding: '1rem 0' }}>
                        <Col span={20}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>佣金方案</span>
                        </Col>
                        <Col span={4}>
                        </Col>
                    </Row>
                    <Row style={{padding: '20px 4rem'}}>
                        <Col span={24}>{commissionPlan || '暂无'}</Col>
                    </Row>
                </Form>
            </div>
            </Content>
            </Layout>
        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.search.activeBuilding,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(CommissionInfo);