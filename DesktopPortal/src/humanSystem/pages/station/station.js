import { connect } from 'react-redux';
import { createStation, getOrgList, adduserPage, setStation, deleteStation, getcreateStation, setLoadingVisible } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Form, InputNumber, Cascader, Button, Row, Col, Spin} from 'antd'
import './station.less';

const styles = {
    conditionRow: {
        width: '80px',
        display: 'inline-block',
        fontWeight: 'bold',
    },
    bSpan: {
        fontWeight: 'bold',
    },
    otherbtn: {
        padding: '0px, 5px',
    }
}

const EditableCell = ({ editable, value, onChange }) => (
  <div>
    {editable
      ? <Input style={{ margin: '-5px 0' }} value={value} onChange={e => onChange(e.target.value)} />
      : value
    }
  </div>
);

const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
      console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
      disabled: record.name === 'Disabled User', // Column configuration not to be checked
      name: record.name,
    }),
};


class Station extends Component {
    constructor(pros) {
        super(pros);

        this.state = {department: ""};
        this.ListColums = [
            {
                title: '职位名称',
                dataIndex: 'stationname',
                key: 'stationname',
                width: '25%',
                render: (text, record) => this.renderColumns(text, record, 'stationname'),},
            {
                title: '操作',
                dataIndex: 'operation',
                render: (text, record) => {
                const { editable } = record;
                return (
                  <div className="editable-row-operations">
                    {
                      editable ?
                        <span>
                          <a onClick={() => this.save(record.key)}>保存</a>
                          <a onClick={()=>this.cancel(record.key)}>取消</a>
                        </span>
                        : <span> <a onClick={() => this.edit(record.key)}>编辑</a> <a onClick={() => this.delete(record.key, record.stationname)}>删除</a> </span>
                    }
                  </div>
                );
              },
            },
        ]

        this.cacheData = this.props.stationList.map(item => ({ ...item }));
    }
    
    renderColumns(text, record, column) {
        return (
            <EditableCell
            editable={record.editable}
            value={text}
            onChange={value => this.handleChange(value, record.key, column)}
            />
        );
    }

    handleChange(value, key, column) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            target[column] = value;
            this.forceUpdate();
        }
    }

    edit(key) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            target.editable = true;
            this.forceUpdate();
        }
    }

    delete(key, name) {
        this.props.stationList.splice(this.props.stationList.findIndex(item => key === item.key), 1);
        this.forceUpdate();
        this.props.dispatch(deleteStation({positionName: name, parentID:this.state.department}));
    }

    save(key) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            delete target.editable;
            this.forceUpdate();
            this.cacheData = newData.map(item => ({ ...item }));
            this.props.dispatch(setStation({positionName: target.stationname, parentID:this.state.department}));
        }
    }

    cancel(key) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            if (target.isnew) {
                this.props.stationList.splice(this.props.stationList.findIndex(item => key === item.key), 1);
                this.forceUpdate();
                return;
            }
            Object.assign(target, this.cacheData.filter(item => key === item.key)[0]);
            delete target.editable;
            this.forceUpdate();
        }
    }

    componentWillMount() {
        this.props.dispatch(setLoadingVisible(false));
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                //this.props.dispatch(postBlackLst(values));
            }
        });
    }

    handleDepartmentChange = (e) => {
        if (!e) {
            this.props.dispatch(setLoadingVisible(true));
            this.props.dispatch(getcreateStation(this.state.department));
        }
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e[e.length-1];
        
    }

    handleSearchBoxToggle1 = (e) => {
        //暂时写个测试
        let nkey = 1;
        if (this.props.stationList.length > 0) {
            nkey = +this.props.stationList[this.props.stationList.length-1].key+2;
        }
        
        this.props.stationList.push({key: nkey+'', stationname: "test", editable: true, isnew: true});
        this.forceUpdate();
        //this.props.dispatch(adduserPage({id: 11, menuID: 'menu_blackaddnew', disname: '新建黑名单', type:'item'}));
    }

    render() {
        return (
            <div style={{display: "block"}}>
                <Row className='searchBox'>
                    <Col span={12}>
                        <label style={styles.conditionRow}>选择分公司 ：</label>
                        <Cascader options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门"/>
                    </Col>
                </Row>
                <Row className="btnBlock">
                    <Col span={6}>
                        <Button className="searchButton" type="primary" onClick={this.handleSearchBoxToggle1}>新建</Button>
                    </Col>
                </Row>
                <Spin spinning={this.props.showLoading} delay={200} tip="查询中...">
                    <Table rowSelection={rowSelection} rowKey={record => record.key} dataSource={this.props.stationList} columns={this.ListColums} onChange={this.handleTableChange} bordered />
                    {/* dataSource={} */}
                </Spin>
            </div>
        );
    }
}

function tableMapStateToProps(state) {
    return {
        showLoading: state.search.showLoading,
        stationList: state.search.stationList,
        setDepartmentOrgTree: state.basicData.searchOrgTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Station);