import { connect } from 'react-redux';
import { setSearchLoadingVisible,getDicParList, adduserPage, searchRewardPunishment} from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Spin, Upload, Checkbox, Button, notification, Modal, Row, Col, InputNumber, Table} from 'antd';

import './search.less';

const buttonDef = [
    { buttonID:"import", buttonName:"录入", icon:'', type:'primary', size:'large',},
    { buttonID:"delete", buttonName:"删除", icon:'', type:'primary', size:'large',},
];

const columns = [
    {title: '工号',dataIndex: 'userID',key: 'userID',},
    {title: '姓名',dataIndex: 'name',key: 'name'},
    {title: '有效日期',dataIndex: 'workDate',key: 'workDate'},
    {title: '金额',dataIndex: 'money',key: 'money'},
    {title: '类型',dataIndex: 'typename',key: 'typename',},
    {title: '备注',dataIndex: 'comments',key: 'comments'},
];


const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
        console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
        disabled: record.name === 'Disabled User', // Column configuration not to be checked
        name: record.name,
    }),
}

class MainIndex extends Component {
    state = {
        attendanceList:[],
        selList: [],
    }

    componentWillMount() {//ADMINISTRATIVE_REWARD  ADMINISTRATIVE_PUNISHMENT  ADMINISTRATIVE_DEDUCT
        this.props.dispatch(getDicParList(["ADMINISTRATIVE_REWARD, ADMINISTRATIVE_PUNISHMENT, ADMINISTRATIVE_DEDUCT"]));
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "import") {
            this.props.dispatch(adduserPage({id: "2", menuID: "awpuinput", displayName: '行政惩罚录入', type: 'item'}));
        } else if (e.target.id === "delete") {
            if (this.props.selAttendanceList.length > 0) {
                //this.props.dispatch(deleteAttendenceItem(this.props.selAttendanceList[this.props.selAttendanceList.length-1].id));
            }
            else {
                notification.error({
                    message: '未选择指定奖惩',
                    description: "请选择指定奖惩",
                    duration: 3
                });
            }
        }
    }

    //是否有权限
    hasPermission(buttonInfo) {
        let hasPermission = false;
        if (this.props.judgePermissions && buttonInfo.requirePermission) {
            for (let i = 0; i < buttonInfo.requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(buttonInfo.requirePermission[i])) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            hasPermission = true;
        }
        return hasPermission;
    }

    handleKeyChangeWord = (e) => {
        this.props.searchInfo.keyWord = e.target.value;
    }

    handleSearch = () => {
        this.props.dispatch(searchRewardPunishment(this.props.searchInfo));
    }

    render() {
        let showLoading = this.props.showLoading;
        return (
            <div>
                <Spin spinning={showLoading}>
                    <div className="searchBox">
                        <Row type="flex">
                            <Col span={12}>
                                <Input placeholder={'请输入名称'} onChange = {this.handleKeyChangeWord}/> 
                            </Col>
                            <Col span={8}>
                                <Button type='primary' className='searchButton' onClick={this.handleSearch}>查询</Button>
                            </Col>
                        </Row>
                    </div>
                    {
                        buttonDef.map(
                            (button, i)=>this.hasPermission(button) ?
                            <Button  id= {button.buttonID}
                            onClick={this.handleClickFucButton} 
                            icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button>
                            : null)
                    }
                    <div>
                        <p style={{marginBottom: '10px'}}>目前已为你筛选出<b>{this.props.searchInfo.rewardpunishmenList.extension.length}</b>条考勤信息</p>
                        <div id="searchResult">
                            <Table id= {"table"} rowKey={record => record.key} 
                            columns={columns} 
                            pagination={this.props.searchInfo} 
                            onChange={this.handleChangePage} 
                            dataSource={this.props.searchInfo.rewardpunishmenList.extension} bordered size="middle" 
                            rowSelection={rowSelection} />
                        </div>
                    </div>
                </Spin>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        searchInfo: state.search,
        showLoading: state.search.showLoading,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);