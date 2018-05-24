//附件组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import {Modal, Upload, Menu, Icon, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import Avatar from './tradeUpload'
const { Header, Sider, Content } = Layout;

const menuDefine = [
    { menuID: "menu_htyjfyj", displayName: "合同原件复印件", menuIcon: '' },
    { menuID: "menu_fwfqrs", displayName: "服务费确认书", menuIcon: '' },
    { menuID: "menu_jybg", displayName: "减佣报告", menuIcon: '' },
    { menuID: "menu_cjqkmxb", displayName: "成交情况明细表", menuIcon: '', requirePermission: ['PermissionItemCreate'] },
    { menuID: "menu_mmcjgjqkb", displayName: "买卖成交跟进情况表", menuIcon: '', requirePermission: ['ApplicationCreate'] },
    { menuID: "menu_yzkhsfz", displayName: "业主客户身份证", menuIcon: '', requirePermission: ['ApplicationCreate'] },
    { menuID: "menu_fczgmht", displayName: "房产证或购买合同", menuIcon: '', requirePermission: ['ApplicationCreate'] },
    { menuID: "menu_qksm", displayName: "情况说明", menuIcon: '', requirePermission: ['ApplicationCreate'] },
    { menuID: "menu_gfajjyxz", displayName: "购房按揭交易须知", menuIcon: '', requirePermission: ['ApplicationCreate'] },
];

class TradeAttact extends Component {
    render() {
        return (
            <Layout>
                <Sider>
                    <Menu mode="inline"
                        theme="dark"
                        defaultSelectedKeys={["menu_htyjfyj"]}>
                        {menuDefine.map((menu, i) =>
                            <Menu.Item key={menu.menuID}>
                                <Icon type={menu.menuIcon} />
                                <span>{menu.displayName}</span>
                            </Menu.Item>
                        )}
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                        <Input style={{ width: 300 }}></Input>
                        <Button type="primary">选择图片</Button>
                    </Header>
                    <Content>
                        <div className="clearfix" style={{margin:10}}>
                         <Avatar/>
                        </div>
                    </Content>
                </Layout>
            </Layout>
        )
    }
}
export default TradeAttact