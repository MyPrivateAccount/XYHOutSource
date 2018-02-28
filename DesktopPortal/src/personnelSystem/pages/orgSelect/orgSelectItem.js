import { connect } from 'react-redux';
import { openAuditDetail } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Menu } from 'antd'

const itemStyle = {
    itemBorder: {

    }
}
//部门选择列表
class OrgSelectItem extends Component {
    state = {
    }

    handleAuditClick = (auditInfo) => {
        console.log("auditInfo:", auditInfo);
        this.props.dispatch(openAuditDetail(auditInfo));
    }

    render() {
        return (
            <Menu mode="inline"
                onClick={this.handleMenuClick} defaultSelectedKeys={["menu_index"]}>
                {menuDefine.map((menu, i) =>

                    <Menu.Item key={menu.menuID}>
                        <Icon type={menu.menuIcon} />
                        <span>{menu.displayName}</span>
                    </Menu.Item>
                )}
            </Menu>
        )
    }

}

function mapStateToProps(state) {
    return {
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(OrgSelectItem);