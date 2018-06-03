import { connect } from 'react-redux';
import { setLoadingVisible, adduserPage, deleteSalaryInfo, getSalaryList, getcreateStation } from '../../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Spin, Checkbox, Button, notification } from 'antd';
//import {getDicParList} from '../actions/actionCreator';
import SearchCondition from './searchCondition';
import SearchResult from './searchResult';
import './search.less';

const buttonDef = [
    { buttonID:"addnew", buttonName:"新建", icon:'', type:'primary', size:'large',},
    { buttonID:"modify", buttonName:"修改", icon:'', type:'primary', size:'large',},
    { buttonID:"delete", buttonName:"删除", icon:'', type:'primary', size:'large',},
];


class MainIndex extends Component {
    state = {
    }

    componentWillMount() {
        //this.props.dispatch(getSalaryList(this.props.selAchievementList));
        //this.props.dispatch(setLoadingVisible(false));//测试
    }

    handleClickFucButton = (e) => {
        if (e.target.id === "addnew") {
            this.props.dispatch(adduserPage({id: 11, menuID: 'menu_achievementnew', displayName: '新建薪酬', type:'item'}));
        } else if (e.target.id === "modify") {
            if (this.props.selAchievementList.length > 0) {
                this.props.dispatch(getcreateStation(this.props.selAchievementList[this.props.selAchievementList.length-1].organize));
                this.props.dispatch(adduserPage({id: 12, menuID: 'menu_achievementmodify', displayName: '修改薪酬', type:'item'}));
            }
            else {
                notification.error({
                    message: '未选择指定职位薪酬',
                    description: "请选择指定职位薪酬",
                    duration: 3
                });
            }
        } else if (e.target.id === "delete") {
            if (this.props.selAchievementList.length > 0) {
                this.props.dispatch(deleteSalaryInfo(this.props.selAchievementList[this.props.selAchievementList.length-1]));
            }
            else {
                notification.error({
                    message: '未选择指定职位薪酬',
                    description: "请选择指定职位薪酬",
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
                    <SearchCondition />
                    {
                        buttonDef.map(
                            (button, i)=>this.hasPermission(button) ?
                             <Button key = {i} id= {button.buttonID} style={{marginTop: '10px', marginBottom: '10px', marginRight: '10px', border:0}}
                             onClick={this.handleClickFucButton} 
                             icon={button.icon} size={button.size} type={button.type}>{button.buttonName}</Button> : null
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
        selAchievementList: state.basicData.selAchievementList,
        showLoading: state.basicData.showLoading,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(MainIndex);