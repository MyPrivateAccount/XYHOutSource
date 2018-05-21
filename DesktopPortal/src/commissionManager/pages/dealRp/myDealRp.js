//合同列表
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeManager from './tradeManager'
const { Header, Content } = Layout;
const Option = Select.Option;

class MyDealRp extends Component{
    state = {
        isShowManager:false
    }
    appTableColumns = [
        { title: '审批通过日期', dataIndex: 'passDate', key: 'passDate' },
        { title: '成交编号', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '上业绩日期', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '类型', dataIndex: 'dealType', key: 'dealType' },
        { title: '物业名称', dataIndex: 'wyName', key: 'wyName' },
        { title: '物业地址', dataIndex: 'wyAddress', key: 'wyAddress' },
        { title: '成交总价', dataIndex: 'totalPrice', key: 'totalPrice' },
        { title: '总佣金', dataIndex: 'totalCms', key: 'totalCms' },
        { title: '佣金比例', dataIndex: 'cmsScale', key: 'cmsScale' },
        { title: '所属部门', dataIndex: 'dep', key: 'dep' },
        { title: '录入人', dataIndex: 'inputer', key: 'inputer' },
        { title: '成交人', dataIndex: 'clinch', key: 'clinch' },
        { title: '进行的申请', dataIndex: 'application', key: 'application' },
        { title: '审批状态', dataIndex: 'checkState', key: 'checkState' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="作废">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                    <Tooltip title='修改'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleDelClick = (info) =>{

    }
    handleModClick = (info) =>{

    }
    handleNew = (info)=>{
        this.setState({isShowManager:true})
    }
    componentDidMount = ()=>{

    }
    componentWillReceiveProps = (newProps)=>{

    }
    render(){
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
            console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
            },
            getCheckboxProps: record => ({
            disabled: record.name === 'Disabled User', // Column configuration not to be checked
            name: record.name,
            }),
        };
        return (
            <Layout>
                <div style={{display:!this.state.isShowManager?'block':'none'}}>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':'10'}}/>
                </Tooltip>
                <Table rowSelection={rowSelection} columns={this.appTableColumns}></Table>
                </div>
                <TradeManager vs={this.state.isShowManager}/>
            </Layout>
        )
    }
}
export default MyDealRp