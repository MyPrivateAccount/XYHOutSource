//业绩分摊项设置页面
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const { Header, Content } = Layout;
const Option = Select.Option;

class AcmentSet extends Component{
    state = {

    }
    appTableColumns = [
        { title: '分摊项名称', dataIndex: 'ftName', key: 'ftName' },
        { title: '分摊类型', dataIndex: 'ftType', key: 'ftType' },
        { title: '默认分摊比例', dataIndex: 'ftScale', key: 'ftScale' },
        { title: '是否固定比例', dataIndex: 'isFixScale', key: 'isFixScale' },
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
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':'10'}}/>
                </Tooltip>
                <Table columns={this.appTableColumns}></Table>
            </Layout>
        )
    }
}
export default AcmentSet