//查询条件组件
import React, { Component } from 'react'
import { Layout, Row, Col, Button, TreeSelect, DatePicker } from 'antd';
import { connect } from 'react-redux';
import { orgGetPermissionTree } from '../../actions/actionCreator'
class SearchCondition extends Component {

    handleChangeTime = (e, field) => {
        if (field === 'yjMonth') {
            this.props.searchCondition.yjMonth = e
        }
    }
    handleSelect = (e, field) => {
        this.props.searchCondition.organizationId = e
    }
    handleSearch = (e) => {
        this.props.searchCondition.pageSize = 10
        this.props.searchCondition.pageIndex = 1
        this.props.handleSearch(this.props.searchCondition)
    }
    componentDidMount() {
        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
    }
    render() {
        return (
            <Layout>
                <Layout.Content>
                    <Row>
                        <Col span={24}>
                            <label style={{ margin: 10 }}>
                                <span>分公司</span>
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.AddUserTree}
                                    placeholder="分公司"
                                    defaultValue={this.props.orgid}
                                    onChange={(e) => this.handleSelect(e, 'organizationId')}>
                                </TreeSelect>
                            </label>
                            <label style={{ margin: 10 }}>
                                <span>月结月份</span>
                                <DatePicker style={{ width: 100 }} onChange={(e, dateString) => this.handleChangeTime(dateString, 'yjMonth')} />
                            </label>
                            <Button type="primiary" onClick={this.handleSearch}>查询</Button>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        searchCondition:state.fina.searchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(SearchCondition);