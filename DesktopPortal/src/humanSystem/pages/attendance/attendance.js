import { connect } from 'react-redux';
import { setSearchLoadingVisible, adduserPage, getAttendenceSettingList, postSetAttendenceSettingList, importAttendenceList,searchAttendenceList ,deleteAttendenceItem} from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Spin, Upload, Checkbox, Button, notification, Modal, Row, Col, InputNumber} from 'antd';
import { exceltoattendenceobj } from '../../constants/import';
//import {getDicParList} from '../actions/actionCreator';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';
import './search.less';

const buttonDef = [
    { buttonID:"import", buttonName:"导入", icon:'', isupload: true, type:'primary', size:'large',},
    { buttonID:"setting", buttonName:"配置", icon:'', type:'primary', size:'large',},
    // { buttonID:"modify", buttonName:"修改", icon:'', type:'primary', size:'large',},
    { buttonID:"delete", buttonName:"删除", icon:'', type:'primary', size:'large',},
];

const typeDic = [//对应服务器列表
    "",
    "调休",
    "事假",
    "病假",
    "年假",
    "婚假",
    "丧假",
    "迟到",
    "旷工",
]

class MainIndex extends Component {
    state = {
        attendanceList:[]//用来当临时表
    }

    componentWillMount() {
        this.props.dispatch(getAttendenceSettingList());//
    }

    handleOk = () => {
       this.props.dispatch(postSetAttendenceSettingList(this.state.attendanceList));
    }

    handleCancel = () => {
        this.setState({
            showModal: false,
            attendanceList: [],
        });
    }

    importExcel= (file) => {
        let self = this;
        let reader = new FileReader();
        reader.onload  = function (e) {
            this.props.dispatch(importAttendenceList(exceltoattendenceobj(e.target.result)));
        }
        reader.readAsArrayBuffer(file);
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "setting") {
            this.setState({attendanceList: this.props.attendanceSettingList.slice(), showModal: true,});
        } else if (e.target.id === "delete") {
            if (this.props.selAttendanceList.length > 0) {
                this.props.dispatch(deleteAttendenceItem(this.props.selAttendanceList[this.props.selAttendanceList.length-1].id));
            }
            else {
                notification.error({
                    message: '未选择指定考勤',
                    description: "请选择指定考勤",
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

    render() {
        let showLoading = this.props.showLoading;
        return (
            <div >
                <Spin spinning={showLoading}>
                    <Modal title="考勤金额设定"
                            visible={this.state.showModal}
                            onOk={this.handleOk}
                            onCancel={this.handleCancel}>
                            {
                                this.state.attendanceList.map(
                                    function(dv, i) {
                                        return (
                                            <Row style={{ margin: '6px' }}>
                                                <Col span={3}>typeDic[v.type]</Col>
                                                <Col span={12}>
                                                    <InputNumber 
                                                        onChange={(v)=>{dv.times = v.target.value;this.forceUpdate();}} 
                                                        value={dv.times}>
                                                    </InputNumber>
                                                </Col>
                                                <Col span={12}>
                                                    <InputNumber 
                                                        onChange={(v)=>{dv.money = v.target.value;this.forceUpdate();}} 
                                                        value={dv.money}>
                                                    </InputNumber>
                                                </Col>
                                            </Row>
                                        );
                                    }
                                )
                            }
                    </Modal>
                    <SearchCondition />
                    {
                        buttonDef.map(
                            (button, i)=>this.hasPermission(button) ?
                            button.isupload?
                            <div key = {i} style={{display:"inline-block", marginTop:"10px",marginBottom: '10px', marginRight: '10px', border:0}}>
                            <Upload showUploadList={false} beforeUpload={this.importExcel} style={{border:0}} >
                            <Button  id= {button.buttonID} style={{ border:0}}
                            onClick={this.handleClickFucButton} 
                            icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button></Upload>
                            </div>
                            :
                            <div key = {i} style={{display:"inline-block",position:"relative", marginTop:"10px",marginBottom: '10px', marginRight: '10px', border:0}}>
                            <Button  id= {button.buttonID}
                            onClick={this.handleClickFucButton} 
                            icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button></div>
                            : null
                        )
                    }
                    
                    <SearchResult />
                </Spin>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        selAttendanceList: state.basicData.selAttendanceList,
        attendanceList: state.search.attendanceList,
        attendanceSettingList: state.search.attendanceSettingList,
        showLoading: state.search.showLoading,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);