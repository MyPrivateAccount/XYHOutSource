//提成比例设置
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const { Header, Content } = Layout;
const Option = Select.Option;

class InComeScaleSet extends Component{
    state = {

    }
    appTableColumns = [
        { title: '组织', dataIndex: 'orgName', key: 'orgName' },
        { title: '职位类别', dataIndex: 'jobType', key: 'jobType' },
        { title: '起始业绩', dataIndex: 'stIncome', key: 'stIncome' },
        { title: '结束业绩', dataIndex: 'endIncome', key: 'endIncome' },
        { title: '提成比例', dataIndex: 'incomeScale', key: 'incomeScale' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='修改'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title="删除">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleDelClick(recored)} />
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

    }
    componentDidMount = ()=>{

    }
    componentWillReceiveProps = (newProps)=>{

    }
    render(){
        return (
            <Layout>
                <div style={{'margin':5}}>
                    组织：
                    <Select defaultValue="lucy" style={{ width: 120 }}>
                        <Option value="jack">Jack</Option>
                        <Option value="lucy">Lucy</Option>
                        <Option value="disabled" disabled>Disabled</Option>
                        <Option value="Yiminghe">yiminghe</Option>
                    </Select>
                    <span style={{'margin-left':10}}>职位类别：</span>
                    <Select defaultValue="lucy" style={{ width: 120 }}>
                        <Option value="jack">Jack</Option>
                        <Option value="lucy">Lucy</Option>
                        <Option value="disabled" disabled>Disabled</Option>
                        <Option value="Yiminghe">yiminghe</Option>
                    </Select>
                    <Button style={{'width':80,'margin-left':10}}>查询</Button>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':'10'}}/>
                </Tooltip>
                <Table columns={this.appTableColumns}></Table>
            </Layout>
        )
    }
}
export default InComeScaleSet