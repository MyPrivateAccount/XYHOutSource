//成交报告综合查询主页面
import React, { Component } from 'react';
import { Layout, Row, Col } from 'antd';
import DRpSearchCondition from './dRpSearchCondition'
import DRpSearchResult from './dRpSearchResult'
import DRpDlg from './dRpDlg'
class DealRpQuery extends Component {
    state={
        SearchCondition:{}
    }
    handleSearch = (cd) => {
        this.rstb.handleSearch(cd)
    }
    handleReset = (cd) => {

    }
    onRSTableRef = (ref) => {
        this.rstb = ref
    }
    render() {
        return (
            <Layout>
                <Row>
                    <Col span={24}>
                        <DRpSearchCondition handleSearch={this.handleSearch} handleReset={this.handleReset}/>
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <DRpSearchResult cd = {this.state.SearchCondition} onRSTableRef={this.onRSTableRef}/>
                    </Col>
                </Row>
                <DRpDlg/>
            </Layout>
        )
    }
}
export default DealRpQuery