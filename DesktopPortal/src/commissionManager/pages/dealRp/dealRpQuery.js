//成交报告综合查询主页面
import React, { Component } from 'react';
import { Layout, Row, Col } from 'antd';
import DRpSearchCondition from './dRpSearchCondition'
import DRpSearchResult from './dRpSearchResult'
import DRpDlg from './dRpDlg'
import ZYComponet from './zYComponet'
import TYComponet from './tYComponet'

class DealRpQuery extends Component {
    state = {
        SearchCondition: {}
    }
    handleSearch = (cd) => {
        this.rstb.handleSearch(cd)
    }
    handleReset = (cd) => {

    }
    onRSTableRef = (ref) => {
        this.rstb = ref
    }
    onOpenDlg = (e) => {
        this.drpDlg.show(e)
    }
    onDlgSelf = (e) => {
        this.drpDlg = e;
    }
    onOpenZy=(e)=>{
        this.zyDlg.show()
    }
    onZYSelf=(e)=>{
        this.zyDlg = e;
    }
    onTYSelf=(e)=>{
        this.tyDlg = e;
    }
    onOpenTy=(e)=>{
        this.tyDlg.show(e)
    }
    render() {
        return (
            <Layout>
                <Row>
                    <Col span={24}>
                        <DRpSearchCondition handleSearch={this.handleSearch} handleReset={this.handleReset} />
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <DRpSearchResult cd={this.state.SearchCondition} onRSTableRef={this.onRSTableRef} onOpenDlg={this.onOpenDlg} onOpenZy={this.onOpenZy} onOpenTy={this.onOpenTy}/>
                    </Col>
                </Row>
                <DRpDlg onDlgSelf={this.onDlgSelf} />
                <ZYComponet onZYSelf = {this.onZYSelf}/>
                <TYComponet onTYSelf = {this.onTYSelf}/>
            </Layout>
        )
    }
}
export default DealRpQuery