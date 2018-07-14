//内部分配表格
import { connect } from 'react-redux';
import React, { Component } from 'react';
import ReactDOM from 'react-dom'
import { Select, Table, Button, Tooltip, Input, Form, InputNumber, Spin } from 'antd'
import WebApiConfig from '../../../constants/webApiConfig'
import ApiClient from '../../../../utils/apiClient'
import { dicKeys, permission } from '../../../constants/const'

const FormItem = Form.Item;

class TradeNTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            fetchingUser: false,
            userList: []
        }

        this._lastKeyword=  '';
        this._fetching = false;
        this._timeid = 0;
    }

    _fetchUser = (value)=>{
        if(value ===' '){
            return;
        }
        if(this._timeid){
            clearTimeout(this._timeid)
            this._timeid=0;
        }
        this._timeid = setTimeout(()=>{
            if(this._fetching){
                this._lastKeyword = value;
                return;
            }
            this._lastKeyword= '';
            this.fetchUser(value);
        },200)
        
    }
    fetchUser = async (value) => {
        this._fetching = true;
        let url = `${WebApiConfig.human.orgUser}`;
        if(this._lastKeyword===value){
            this._lastKeyword = '';
        }
        this.setState({userList:[], fetchingUser:true})
        var r = await ApiClient.get(url, true, { permissionId: permission.nyftPepole, keyword: value, pageSize: 0, pageIndex: 0 });
        if (r && r.data && r.data.code === '0') {
            this.setState({ userList: r.data.extension || [] })
        }
        this._fetching = false;
        this.setState({fetchingUser:false})
        if(this._lastKeyword){
            this.fetchUser(this._lastKeyword);
        }
    }
    selectedUser = (row,value) => {
        this.setState({
            data: [],
            fetching: false,
        });
        if (!value || (value && value.length === 0)) {
            this._onRowChanged(row,'uid', null)
          //  this.setState({ userList: [] })
        } else {
            let ru = value[value.length - 1];
            let ui = this.state.userList.find(x=>x.id === ru.key);
        //    this.setState({ userList: [] })
            setTimeout(() => {
                this._onRowChanged(row,'uid', ui)
                
            }, 0);

        }
      var ele =  ReactDOM.findDOMNode(this._tblElement);
      if(ele){
          ele.focus();
      }
      //  document.focus();
    //    this.userSelectElement.blur();
    }

    _getTableColums = () => {
        let sfItems = this.props.items || [];
        let canEdit = this.props.canEdit || false;
        const isTy = this.props.type==='ty'
        let appTableColumns = [
            {
                title: '部门', dataIndex: 'sectionId', key: 'sectionId',
                render: (text, record) => (
                    <FormItem>
                        <Select disabled value={record.sectionId}>
                            <Select.Option value={record.sectionId}>{record.sectionName}</Select.Option>
                        </Select>
                    </FormItem>
                )
            },
            {
                title: '员工', dataIndex: 'uid', key: 'uid',width:'15rem',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['uid'] ? 'error' : ''}>
                        <Select
                            disabled={!canEdit || isTy}
                            mode="multiple"
                            maxTagCount={1}
                            
                            dropdownStyle={{minWidth: 300}}
                            labelInValue
                            value={record.uid||[]}
                            placeholder="输入姓名、员工编号或手机号码"
                            notFoundContent={this.state.fetchingUser ? <Spin size="small" /> : '没有数据'}
                            filterOption={false}
                            onSearch={this._fetchUser}
                            onChange={v=>this.selectedUser(record, v)}
                            style={{ width: '100%' }}
                        >
                            {this.state.userList.map(d => <Select.Option key={d.id}>{`${d.name}\t\t${d.organizationFullName}`}</Select.Option>)}
                        </Select>
                    </FormItem>
                )
            },
            {
                title: '工号', dataIndex: 'workNumber', key: 'workNumber',width:'15rem',
                render: (text, record) => (
                    <FormItem>
                        <Input value={text} disabled />
                    </FormItem>
                )
            },
            {
                title: '身份', dataIndex: 'type', key: 'type',width:'10rem',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['type'] ? 'error' : ''}>
                        <Select value={record.type}  disabled={!canEdit} onChange={v=>this._onRowChanged(record, 'type', v)}>
                            {
                                sfItems.map(tp => <Select.Option key={tp.name} value={tp.code}>{tp.name}</Select.Option>)
                            }
                        </Select>
                    </FormItem>
                )
            },
            {
                title: '比例', dataIndex: 'percent', key: 'percent',width:'10rem',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['percent'] ? 'error' : ''}>
                        <InputNumber min={0}  disabled={!canEdit} value={text} precision={2}  onChange={v=>this._onRowChanged(record, 'percent', v)}/>%
                </FormItem>
                )
            },
            {
                title: '金额', dataIndex: 'money', key: 'money',width:'15rem',
                render: (text, record) => (
                    <FormItem>
                        <InputNumber disabled value={text} />
                    </FormItem>
                )
            },
            {
                title: '单数', dataIndex: 'oddNum', key: 'oddNum',width:'10rem',
                render: (text, record) => (
                    <FormItem hasFeedback validateStatus={record.errors['oddNum'] ? 'error' : ''}>
                        <InputNumber precision={2} disabled={!canEdit} value={record.oddNum} onChange={v=>this._onRowChanged(record, 'oddNum', v)}/>
                    </FormItem>
                )
            },
            {
                title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => {
                    return (canEdit && !isTy) ? <span>

                        <Tooltip title='删除'>
                            &nbsp;<Button type='primary' size='small'  onClick={(e) => this._onDelRow(recored)} >删除</Button>
                        </Tooltip>
                    </span> : null
                }
            }
        ];
        return appTableColumns;
    }
    _onRowChanged = (row, key, value) => {
        if (this.props.onRowChanged) {
            if (value && value.target) {
                value = value.target.value;
            }
            this.props.onRowChanged(row, key, value)
        }
    }

    _onDelRow = (row) => {
        if (this.props.onDelRow) {
            this.props.onDelRow(row);
        }
    }

    render() {
        const { dataSource } = this.props
        const columns = this._getTableColums();
        
        return (
            <Form>
                <Table ref={(ins)=>this._tblElement = ins} bordered size="small" columns={columns} pagination={false} dataSource={dataSource}></Table>
            </Form>
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
export default connect(MapStateToProps, MapDispatchToProps)(TradeNTable);