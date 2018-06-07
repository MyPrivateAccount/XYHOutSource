import React, { Component } from 'react';
import { Layout, Row, Col } from 'antd';
import TyxqSearchCondition from './tyxqSearchCondition'
import TyxqSearchResult from './tyxqSearchResult'
class TyxqQuery extends Component {
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
                        <TyxqSearchCondition handleSearch={this.handleSearch} />
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <TyxqSearchResult cd = {this.state.SearchCondition} onRSTableRef={this.onRSTableRef}/>
                    </Col>
                </Row>
            </Layout>
        )
    }
}
export default TyxqQuery