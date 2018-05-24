//交易合同管理页面
import React, { Component } from 'react';
import { Modal, Layout, Table, Button, Checkbox, Tree, Tabs, Icon, Popconfirm, Spin, Tooltip } from 'antd';
import TradeContract from './tradeContract'
import TradeEstate from './tradeEstate'
import TradeBnsOwner from './tradeBnsOwer'
import TradeCustomer from './tradeCustomer'
import TradePerDis from './tradePerDis'
import TradeAttact from './tradeAttact'
import TradeTransfer from './tradeTransfer'
import TradeAjust from './tradeAjust'

const { Header, Sider, Content } = Layout;
const TreeNode = Tree.TreeNode;
const TabPane = Tabs.TabPane;

class TradeManager extends Component {
    render() {
        return (
            <div style={{ display: this.props.vs ? 'block' : 'none' }}>
                <Layout>
                    <div>
                    <Tooltip title="返回">
                        <Button type='primary' shape='circle'  icon='arrow-left' style={{ 'margin': 10,float:'left' }} />
                    </Tooltip>
                    <Tooltip title="保存">
                        <Button type='primary' shape='circle' icon='check' style={{ 'margin': 10 ,float:'left'}} />
                    </Tooltip>
                    </div>
                    <Content style={{ overflowY: 'auto', height: '100%' }}>
                        <Tabs defaultActiveKey="jyht">
                            <TabPane tab="交易合同" key="jyht">
                                <TradeContract />
                            </TabPane>
                            <TabPane tab="成交物业" key="cjwy">
                                <TradeEstate/>
                            </TabPane>
                            <TabPane tab="业主信息" key="yzxx">
                                <TradeBnsOwner/>
                            </TabPane>
                            <TabPane tab="客户信息" key="khxx">
                               <TradeCustomer/>
                            </TabPane>
                            <TabPane tab="业绩分配" key="yjfp">
                               <TradePerDis/>
                            </TabPane>
                            <TabPane tab="附件" key="fj">
                              <TradeAttact/>
                            </TabPane>
                            <TabPane tab="按揭过户" key="ajgh">
                              <TradeTransfer/>
                            </TabPane>
                            <TabPane tab="业绩调整" key="yjtz">
                              <TradeAjust/>
                            </TabPane>
                        </Tabs>
                    </Content>
                </Layout>
            </div>
        )
    }
}
export default TradeManager