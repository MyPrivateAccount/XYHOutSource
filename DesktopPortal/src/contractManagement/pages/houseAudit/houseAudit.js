import { connect } from 'react-redux';
import {  showTimeDialog } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Layout, Button, Checkbox, Row, Col, Tabs, Radio, Input, Menu, Select,Spin,Modal,Pagination ,Tag,Popover} from 'antd'
import moment from 'moment'
import './houseAudit.less'

const { Header, Sider, Content } = Layout;

class HouseAudit extends Component {
  state = {
    current: '1',
  }
   
  handleClick = (e) => {
    console.log('click ', e);
    this.setState({
      current: e.key,
    });
  }

  render() {
    const isLoading = this.props.isLoading
    return ( 
      <Spin spinning={isLoading}>
      <div className="relative">
      <Layout>
      <Content className='content houseAudit'>
        <div>
            <Menu onClick={this.handleClick} selectedKeys={[this.state.current]} mode="horizontal" style={{marginBottom: '20px'}}>
              <Menu.Item key="1">审核中</Menu.Item>
              <Menu.Item key="2">审核通过</Menu.Item>
              <Menu.Item key="3">审核驳回</Menu.Item>
            </Menu>

            <Row  className='itemStyle'>
              <Col span={24}>
                  <Row className='borderDiv'>
                      <Col span={20}>
                      <div className='listLeft'>
                          <div className='imgDiv'>
                              <img src= 'https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png' />
                          </div>
                          <div className='listText'>
                              <p>保利悦都</p>
                              <p>成都-高新区-金融城</p>
                              <p><Tag color="blue">blue</Tag><Tag color="blue">blue</Tag></p>
                              <p>2017年09月20日</p>
                              <p>驳回原因</p>
                          </div>
                      </div>
                      </Col>
                      <Col span={4} className='priceDiv'>
                          <p style={{color: 'red'}}>新增房源</p>
                          <p className='text'>1.5万元/m²</p>
                          <Button type='primary' size='small'>查看修改</Button>
                      </Col>
                  </Row>
              </Col>
            </Row>
            
        </div>
      </Content>
      </Layout>
      </div>
      </Spin>
    )
  }
}

function mapStateToProps(state) {
  // console.log('首页', state.index.todayReportListId);
  return {
     isLoading: state.houseAudit.isLoading
  }
}

function mapDispatchToProps(dispatch) {
  return {
    showTimeDialog: () => dispatch(showTimeDialog()),
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(HouseAudit);