import { connect } from 'react-redux';
import { submitShopsInfo, getDicParList } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Row, Col, Icon, Radio, BackTop, Modal, notification } from 'antd'
const { Header, Sider, Content } = Layout;

class Staffinfo extends Comment {
    componentWillMount() {
    }

    render() {
        return (
            <div className="relative">
               <Layout>
                   <Content>
                       <div>
                           <Row type="flex" justify="space-between">
                              <Col span={12} style={{ textAlign: 'left' }}>
                              </Col>
                                <Col span={12} style={{ textAlign: 'right' }}>
                                    <Radio.Group defaultValue='basicInfo' onChange={this.handleAnchorChange} size='large'>
                                        {
                                            shopsInfoGroup.map((info) => <Radio.Button value={info.id} key={info.id}>{info.name}<Icon type="check" /></Radio.Button>)
                                        }
                                    </Radio.Group>
                                </Col>
                           </Row>
                       </div>
                   </Content>
               </Layout>
            </div>
        );
    }
}