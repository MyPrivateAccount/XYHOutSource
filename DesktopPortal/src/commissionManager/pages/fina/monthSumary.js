//月结页面
import React, { Component } from 'react'
import { Layout, Row, Col,Button ,Select,DatePicker} from 'antd';

class MonthSum extends Component{
    render(){
        return (
            <Layout>
                <Layout.Content>
                    <Row style={{margin:10}}>
                        <Col span={24} style={{textAlign:'center'}}>
                          <Button type="primiary">1</Button>
                          <span>________</span>
                          <Button type="primiary">2</Button>
                          <span>________</span>
                          <Button type="primiary">3</Button>
                          <span>________</span>
                          <Button type="primiary">4</Button>
                        </Col>
                    </Row>
                    <Row style={{margin:10,marginLeft:360}}>
                        <Col span={2} style={{textAlign:'center'}}>
                          <span>月结检查</span>
                        </Col>
                        <Col span={4} style={{textAlign:'center'}}>
                          <span>离职人员业绩确认</span>
                        </Col>
                        <Col span={3} style={{textAlign:'center'}}>
                          <span>实发扣减确认</span>
                        </Col>
                        <Col span={2} style={{textAlign:'center'}}>
                          <span>月结完成</span>
                        </Col>
                    </Row>
                    <Row style={{marginTop:80}}>
                        <Col span={24} style={{textAlign:'center'}}>
                          <label>
                              <span>分公司</span>
                              <Select style={{width:200}}></Select>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{marginTop:10}}>
                        <Col span={24} style={{textAlign:'center',marginLeft:-50}}>
                          <label>
                              <span>月结年月</span>
                              <DatePicker style={{width:100}}></DatePicker>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{marginTop:10}}>
                        <Col span={24} style={{textAlign:'center'}}>
                          <label>
                              <Button type="primiary">月结</Button>
                          </label>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
export default MonthSum