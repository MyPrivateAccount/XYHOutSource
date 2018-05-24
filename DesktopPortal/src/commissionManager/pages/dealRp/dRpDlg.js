//弹出的收付款框模版
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Row, Col, Form, Input, Tooltip, Button, Modal, Layout,Tabs } from 'antd'
import FKCp from './fKComponet'
import SKCp from './sKComponet'
import SJCp from './sJComponet'
import Avatar from './rpdetails/tradeUpload'

const TabPane = Tabs.TabPane;
class DRpDlg extends Component {
    state = {
        vs:true
    }
    handleOk = (e) => {
        this.setState({vs:false})
    };
    handleCancel = (e) => {
        this.setState({vs:false})
    };
    render() {
        return (
            <Modal title={'收款'} width={800} maskClosable={false} visible={this.state.vs}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Layout>
                    <Row style={{margin:5}}>
                        <Col span={24} style={{marginLeft:10}}>
                            <label>
                                <span style={{ marginRight: '10px' }}>总佣金</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交编号</span>
                                <Input style={{ width: 200 }}></Input>
                            </label>
                        </Col>
                        <Col span={12}>
                            <label>
                                <span style={{ marginRight: '10px' }}>物业名称</span>
                                <Input style={{ width: 200 }}></Input>
                            </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={24}>
                            <Tabs defaultActiveKey="dsdfk">
                                <TabPane tab="代收代付款" key="dsdfk">
                                    <SKCp/>
                                </TabPane>
                                <TabPane tab="收据" key="sj">
                                    <SJCp/>
                                </TabPane>
                                <TabPane tab="附件" key="fj">
                                    <Avatar/>
                                </TabPane>
                            </Tabs>
                        </Col>
                    </Row>
                </Layout>
            </Modal>
        )
    }
}
export default DRpDlg