//成交报告综合查询主页面
import React, { Component } from 'react';
import { Layout, Row, Col } from 'antd';
import DRpSearchCondition from './dRpSearchCondition'
import DRpSearchResult from './dRpSearchResult'
import DRpDlg from './dRpDlg'
class DealRpQuery extends Component {
    render() {
        return (
            <Layout>
                <Row>
                    <Col span={24}>
                        <DRpSearchCondition/>
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <DRpSearchResult />
                    </Col>
                </Row>
                <DRpDlg/>
            </Layout>
        )
    }
}
export default DealRpQuery