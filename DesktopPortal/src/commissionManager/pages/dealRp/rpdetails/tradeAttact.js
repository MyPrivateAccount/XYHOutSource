//附件组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Modal, Upload, Menu, Icon, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
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
    state = {
        activeMenuID: menuDefine[0].menuID,
        reportFiles:[]
    }
    handleMenuClick = (e) => {
        console.log('click ', e);
        for (let i in menuDefine) {
            if (menuDefine[i].menuID === e.key) {
                this.setState({
                    activeMenuID: menuDefine[i].menuID
                });
                break;
            }
        }
    }
    appendData = (data)=>{
        let reportFiles = this.state.reportFiles
        reportFiles.push(data)
        this.setState({reportFiles})
    }
    getFileList=()=>{
        let list = []
        let menuID = this.state.activeMenuID
        let reportFiles = this.state.reportFiles
        for(let i=0;i<reportFiles.length;i++){
            if(reportFiles[i].type === menuID){
                let fl = {uid:'',name:'',status:'done',url:''}
                fl.uid = reportFiles[i].fileGuid
                fl.name = reportFiles[i].name
                fl.status = 'done'
                fl.url = reportFiles[i].uri
                list.push(fl)
            }
        }
        return list
    }
    render() {
        return (
            <Layout>
                <Sider>
                    <Menu mode="inline"
                        theme="dark"
                        onClick={this.handleMenuClick}
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
                        <div className="clearfix" style={{ margin: 10 }}>
                            <Avatar id={this.state.activeMenuID} fileList={this.getFileList} append={this.appendData}/>
                        </div>
                    </Content>
                </Layout>
            </Layout>
        )
    }
}
export default TradeAttact