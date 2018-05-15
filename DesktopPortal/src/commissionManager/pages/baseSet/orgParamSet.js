//组织参数设置
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const { Header, Content } = Layout;
const Option = Select.Option;

class OrgParamSet extends Component{
    state = {

    }
    appTableColumns = [
        { title: '组织', dataIndex: 'orgName', key: 'orgName' },
        { title: '参数名称', dataIndex: 'paramName', key: 'paramName' },
        { title: '参数值', dataIndex: 'paramVal', key: 'paramVal' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

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
    handleNew = (e)=>{
        console.log(e);
        //this.props.dispatch(orgFtParamAdd());
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
export default OrgParamSet