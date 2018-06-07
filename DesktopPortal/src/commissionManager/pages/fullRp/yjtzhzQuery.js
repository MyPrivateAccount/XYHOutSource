import React, { Component } from 'react';
import { Layout, Row, Col } from 'antd';
import YjtzhzSearchCondition from './yjtzhzSearchCondition'
import YjtzhzSearchResult from './yjtzhzSearchResult'
class YjtzhzQuery extends Component {
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
                        <YjtzhzSearchCondition handleSearch={this.handleSearch} />
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <YjtzhzSearchResult cd = {this.state.SearchCondition} onRSTableRef={this.onRSTableRef}/>
                    </Col>
                </Row>
            </Layout>
        )
    }
}
export default YjtzhzQuery