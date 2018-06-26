import { connect } from 'react-redux';
import { setSearchLoadingVisible, adduserPage,} from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Spin, Upload, Checkbox, Button, notification, Modal, Row, Col, InputNumber} from 'antd';
import { exceltoattendenceobj } from '../../constants/import';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';
import './search.less';

const buttonDef = [
    { buttonID:"import", buttonName:"录入", icon:'', type:'primary', size:'large',},
    // { buttonID:"modify", buttonName:"修改", icon:'', type:'primary', size:'large',},
    { buttonID:"delete", buttonName:"删除", icon:'', type:'primary', size:'large',},
];

class MainIndex extends Component {
    state = {
        attendanceList:[]
    }

    componentWillMount() {//ADMINISTRATIVE_REWARD  ADMINISTRATIVE_PUNISHMENT  ADMINISTRATIVE_DEDUCT
        
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

    render() {
        let showLoading = this.props.showLoading;
        return (
            <div>
                <Spin spinning={showLoading}>
                    <SearchCondition />
                    {
                        buttonDef.map(
                            (button, i)=>this.hasPermission(button) ?
                            <Button  id= {button.buttonID}
                            onClick={this.handleClickFucButton} 
                            icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button>
                            : null)
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