import { connect } from 'react-redux';
import { appAdd, appEdit, orgNodeSave, dialogClose } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Card, Row, Col } from 'antd'

const styles = {
    itemTitle: {
        fontWeight: 'bold'
    }
}



class OrgDetail extends React.Component {
    state = {
        orgInfo: {},
    }
    componentWillReceiveProps(newProps) {
        let { activeTreeNode } = newProps;
        this.setState({ orgInfo: activeTreeNode });
    }


    render() {
        return (
            <Card title="" >
                <Row>
                    <Col span={4} style={styles.itemTitle}>部门名称：</Col>
                    <Col span={8}>{this.state.orgInfo.organizationName}</Col>

                    <Col span={4} style={styles.itemTitle}>部门主管：</Col>
                    <Col span={8}>{this.state.orgInfo.leaderManager}</Col>

                </Row>
                <Row>
                    <Col span={4} style={styles.itemTitle}>部门电话：</Col>
                    <Col span={8}>{this.state.orgInfo.phone}</Col>

                    <Col span={4} style={styles.itemTitle}>传真：</Col>
                    <Col span={8}>{this.state.orgInfo.fax}</Col>

                </Row>
                <Row>
                    <Col span={4} style={styles.itemTitle}>部门地址：</Col>
                    <Col span={20}>{this.state.orgInfo.address}</Col>
                </Row>

            </Card>
        )
    }
}
function orgEditMapStateToProps(state) {
    //console.log('apptableMapStateToProps:' + JSON.stringify(state));
    return {
        activeTreeNode: state.org.activeTreeNode,
        treeSource: state.org.treeSource,
        operInfo: state.org.operInfo
    };
}

function dialogMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(orgEditMapStateToProps, dialogMapDispatchToProps)(OrgDetail);