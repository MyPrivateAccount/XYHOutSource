import {connect} from 'react-redux';
import {createStation, getOrgList, getDicParList, setStation, deleteStation, getcreateStation, setSearchLoadingVisible} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Table, Input, Form, Select, Cascader, Button, Row, Col, Spin, TreeSelect, Popconfirm} from 'antd'
import './station.less';
import Layer, {LayerRouter} from '../../../components/Layer'
import {Route} from 'react-router'
import AddStation from './addstation'
import {getDicPars} from '../../../utils/utils';

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

const EditableCell = ({editable, value, onChange}) => (
    <div>
        {editable
            ? <Input style={{margin: '-5px 0'}} value={value} onChange={e => onChange(e.target.value)} />
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
        this.cacheData = this.props.stationList.map(item => ({...item}));
    }

    gotoSubPage = (path, params) => {
        this.props.history.push(`${this.props.match.url}/${path}`, {...params})
    }

    selectChange(key, v) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            target.positionType = v;
        }
    }

    handleChange(value, key, column) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            target[column] = value;
            this.forceUpdate();
        }
    }

    edit(recordInfo) {
        this.gotoSubPage("addStation", recordInfo);
    }

    delete = (record) => {
        this.props.dispatch(deleteStation({entity: record}));
    }

    save(key) {
        const newData = [...this.props.stationList];
        const target = newData.filter(item => key === item.key)[0];
        if (target) {
            delete target.editable;
            this.forceUpdate();
            this.cacheData = newData.map(item => ({...item}));
            this.props.dispatch(setStation({id: target.id, positionName: target.stationname, positionType: target.positionType, parentID: this.state.department}));
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
        // this.props.dispatch(getDicParList(["POSITION_TYPE"]));
        // this.props.dispatch(setSearchLoadingVisible(false));
    }

    componentDidMount() {
        let dicPositions = getDicPars("POSITION_TYPE", this.props.rootBasicData);
        this.setState({dicPositions: dicPositions}, () => {
            let ListColums = [
                {
                    title: '职位名称',
                    dataIndex: 'positionName',
                    key: 'positionName'
                },
                {
                    title: '职位类型',
                    dataIndex: 'positionType',
                    key: 'positionType',
                    render: (text, record) => {
                        let positionObj = this.state.dicPositions.find(item => item.value === record.positionType);
                        return (
                            <div>
                                {positionObj ? positionObj.key : ''}
                            </div>
                        );
                    }
                },
                {
                    title: '操作',
                    dataIndex: 'operation',
                    render: (text, record) => {
                        const {editable} = record;
                        return (
                            <div className="editable-row-operations">
                                <Button type="primary" size='small' style={{marginRight: '10px'}} onClick={() => this.edit(record)}>编辑</Button>
                                <Popconfirm title="确认删除该记录?" onConfirm={() => this.delete(record)} okText="确认" cancelText="取消">
                                    <Button type="primary" size='small' >删除</Button>
                                </Popconfirm >
                            </div>
                        );
                    },
                },
            ]
            this.setState({ListColums: ListColums});
        });

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
    handleChooseDepartmentChange = (e) => {
        console.log("部门change:", e);
        this.setState({department: e});
    }

    handleSearchBoxToggle1 = (e) => {
        //暂时写个测试
        let nkey = 1;
        if (this.props.stationList.length > 0) {
            nkey = +this.props.stationList[this.props.stationList.length - 1].key + 2;
        }

        this.props.stationList.push({key: nkey + '', stationname: "test", positionType: "", editable: true, isnew: true});
        this.forceUpdate();
        //this.props.dispatch(adduserPage({id: 11, menuID: 'menu_blackaddnew', disname: '新建黑名单', type:'item'}));
        this.gotoSubPage("addStation", {})
    }
    search = () => {
        this.props.dispatch(setSearchLoadingVisible(true));
        this.props.dispatch(getcreateStation(this.state.department));
    }

    render() {
        let ListColums = this.state.ListColums || [];
        return (
            <Layer>
                <Row>
                    <Col style={{marginTop: '10px'}}>
                        <label style={styles.conditionRow}>选择分公司 ：</label>
                        <TreeSelect style={{width: '300px', marginRight: '10px'}}
                            allowClear
                            treeData={this.props.setContractOrgTree}
                            onChange={this.handleChooseDepartmentChange}
                            placeholder="请选择所属分公司"
                        />
                        <Button type="primary" onClick={this.search}>查询</Button>
                    </Col>
                </Row>
                <Row className="btnBlock">
                    <Col style={{marginBottom: '15px', marginTop: '15px'}}>
                        <Button type="primary" onClick={this.handleSearchBoxToggle1}>新建</Button>
                    </Col>

                </Row>
                <Spin spinning={this.props.showLoading} delay={200} tip="查询中...">
                    <Table rowSelection={rowSelection} rowKey={record => record.key} dataSource={this.props.stationList} columns={ListColums} onChange={this.handleTableChange} bordered />
                </Spin>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/addStation`} render={(props) => <AddStation  {...props} />} />
                </LayerRouter>
            </Layer>
        );
    }
}

function tableMapStateToProps(state) {
    return {
        stationTypeList: state.basicData.stationTypeList,
        showLoading: state.search.showLoading,
        stationList: state.search.stationList,
        setContractOrgTree: state.basicData.searchOrgTree,
        rootBasicData: (state.rootBasicData || {}).dicList,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Station);