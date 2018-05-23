//收据组件
import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Select, Row, Col, Form, Input, Tooltip, Button, Modal, Layout,Tabs,DatePicker } from 'antd'

class SJCp extends Component{
    render(){
        return(
            <Layout>
                <Layout.Content>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>收据号码</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>其他收据</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={24}>
                            <label><span style={{ marginRight: '10px' }}>收据备注</span>
                            <Input.TextArea rows={4} style={{width:510}}></Input.TextArea>
                            </label>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
export default SJCp