import React, {Component} from 'react'
import {Form, Icon, Row, Col} from 'antd'

class ShopSummaryInfo extends Component {
    state = {

    }
    render() {
        let summary = (this.props.shopInfo || {}).summary;
        return (
            <div style={{marginTop: '25px', backgroundColor: "#ECECEC"}}>
                <Form layout="horizontal" >
                    <Row type="flex" style={{padding: '1rem 0'}}>
                        <Col span={24}>
                            <Icon type="tags-o" className='content-icon' /><span className='content-title'>商铺简介</span>
                        </Col>
                    </Row>
                    <Row className='viewRow'>
                        <Col span={24}>{summary || '暂无'}</Col>
                    </Row>
                </Form>
            </div>
        )
    }
}

export default ShopSummaryInfo;