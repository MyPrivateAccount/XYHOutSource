//合同列表
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeManager from './rpdetails/tradeManager'
import DealRpTable  from './dealRpTable'
import SearchCondition from '../../constants/searchCondition'

const { Header, Content } = Layout;
const Option = Select.Option;

class MyDealRp extends Component{
    state = {
        isShowManager:false,
        cd:SearchCondition.rpListCondition
    }
    handleDelClick = (info) =>{

    }
    handleModClick = (info) =>{

    }
    handleNew = (info)=>{
        this.setState({isShowManager:true})
    }
    handleBack = (e)=>{
        this.setState({isShowManager:false})
    }
    componentWillMount=()=>{

    }
    componentDidMount = ()=>{

    }
    componentWillReceiveProps = (newProps)=>{

    }
    onRpTable=(ref)=> {
        this.rptb = ref
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
                <DealRpTable SearchCondition={this.state.cd} onRpTable={this.onRpTable}/>
                </div>
                <TradeManager vs={this.state.isShowManager}  handleback={this.handleBack}/>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(MyDealRp);