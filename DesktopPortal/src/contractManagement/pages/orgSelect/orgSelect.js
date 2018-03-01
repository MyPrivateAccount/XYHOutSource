import { connect } from 'react-redux';
import { closeOrgSelect, changeActiveOrg } from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Button, Row, Col, Menu, Checkbox } from 'antd';

const { SubMenu } = Menu;
const itemStyle = {
    itemBorder: {
        position: 'relative',
        zIndex: '100',
        background: '#fff',
        width: '100%'
    },
    autoHeight: {
        height: '100%'
    }
}

class OrgSelect extends Component {
    state = {
        checkedOrgs: []
    }

    handleOrgChecked = (orgInfo, checked) => {
        // console.log("orgInfo checked:", orgInfo, checked);
        if (checked) {
            this.setState({ checkedOrgs: [orgInfo.id] });
            this.props.dispatch(changeActiveOrg(orgInfo));
        }
        this.props.dispatch(closeOrgSelect());
    }

    getChildOrg(orgInfo) {
        if (orgInfo) {
            if (orgInfo.children.length === 0) {
                return (<Menu.Item key={orgInfo.id}>
                    <Checkbox onChange={(e) => this.handleOrgChecked(orgInfo, e.target.checked)}>{orgInfo.organizationName}</Checkbox>
                </Menu.Item>)
            } else {
                return (<SubMenu key={orgInfo.id} title={<span><Checkbox onChange={(e) => this.handleOrgChecked(orgInfo, e.target.checked)}></Checkbox> {orgInfo.organizationName}</span>}>
                    {
                        orgInfo.children.map(org => this.getChildOrg(org))
                    }
                </SubMenu >)
            }
        }
    }

    render() {
        const orgList = this.props.orgInfo.orgList;
        const levelsCount = this.props.orgInfo.levelCount;
        let span = Math.round(24 / levelsCount);
        // console.log("部门选择:", orgList, span);
        return (
            <div style={itemStyle.itemBorder}>
                <Row style={itemStyle.autoHeight}>
                    <Col span={span} style={itemStyle.autoHeight}>
                        <Menu selectedKeys={this.state.checkedOrgs} style={itemStyle.autoHeight}>
                            <Menu.Item key="0"><Checkbox onChange={(e) => this.handleOrgChecked({ id: '0', organizationName: '不限' }, e.target.checked)}></Checkbox>不限</Menu.Item>
                            {orgList.map(org =>
                                this.getChildOrg(org)
                            )}
                        </Menu>
                    </Col>
                </Row>
            </div>
        )
    }

}

function mapStateToProps(state) {
    return {
        orgInfo: state.basicData.orgInfo
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(OrgSelect);