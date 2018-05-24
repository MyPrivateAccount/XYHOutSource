//查询条件组件
import React, { Component } from 'react'
import { Layout, Row, Col,Button ,Select,DatePicker} from 'antd';

class SearchCondition extends Component{
    render(){
        return (
            <Layout>
                <Layout.Content>
                    <Row>
                        <Col span={24}>
                          <label style={{margin:10}}>
                              <span>分公司</span>
                              <Select style={{width:100}}></Select>
                          </label>
                          <label style={{margin:10}}>
                              <span>月结月份</span>
                              <DatePicker style={{width:100}}></DatePicker>
                          </label>
                          <Button type="primiary">查询</Button>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
export default SearchCondition