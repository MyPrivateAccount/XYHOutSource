import React, { Component } from 'react';
import { Layout, Row, Col } from 'antd';
import FyxqSearchCondition from './fyxqSearchCondition'
import FyxqSearchResult from './fyxqSearchResult'
class FyxqQuery extends Component {
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
                        <FyxqSearchCondition handleSearch={this.handleSearch} />
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <FyxqSearchResult cd = {this.state.SearchCondition} onRSTableRef={this.onRSTableRef}/>
                    </Col>
                </Row>
            </Layout>
        )
    }
}
export default FyxqQuery