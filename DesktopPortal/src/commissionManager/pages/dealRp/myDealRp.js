//合同列表
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeManager from './rpdetails/tradeManager'
import DealRpTable  from './dealRpTable'
const { Header, Content } = Layout;
const Option = Select.Option;

class MyDealRp extends Component{
    state = {
        isShowManager:false
    }
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
                <DealRpTable/>
                </div>
                <TradeManager vs={this.state.isShowManager}/>
            </Layout>
        )
    }
}
export default MyDealRp