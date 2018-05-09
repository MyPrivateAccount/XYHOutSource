import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import {closeFollowHistoryDialog} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table,Button, Timeline, Icon} from 'antd';
import moment from 'moment';
import "./dealInfo.less";
const Option = Select.Option;
const FormItem = Form.Item;

const modelStyle = {
    verticalCenterModal: {
        textAlign: 'center',
        whiteSpace: 'nowrap',
      },
}


class FollowHistory extends Component{

    state = {
        visible: false,
        followHistory:[],
    }
   
    componentWillMount(){

    }

    componentWillReceiveProps(newProps) {

    }
    handleCancel = () => {

        //this.props.dispatch(closeAdjustCustomer());
        this.props.dispatch(closeFollowHistoryDialog());
    }

    dateTimeRender = (text) => {
        
        let newText = text;
        if (text) {
            newText = moment(text).format('YYYY-MM-DD');
        }
        
        return (<span>申请日期{newText}</span>);
    }
    projectNameRender = (text) =>{
        return (<span>项目名称:{text}</span>);
    }
    numRender = (text) =>{
        return (<span>合同编号:{text}</span>);
    }
    render(){
        let contractInfo = (contractInfo || {});
        return(
            <Modal 
                title="续签记录"  wrapClassName="vertical-center-modal"    className='contractChoose' footer={null} maskClosable={false} visible={this.props.followHistoryVisible} 
                onCancel={this.handleCancel}
            >
            <Timeline>
                {
                    (this.props.followHistory || []).map((item, index) =>
                        <Timeline.Item color={index == this.props.followHistory.length -1 ? 'red' : 'green'} key={item.id}>
                            {item.name}
                            <p>{this.numRender(item.num)}</p>
                            <p>{this.dateTimeRender(item.createTime)}</p>
                            <p>{this.projectNameRender(item.projectName)}</p>
                        </Timeline.Item>
                    )
                }
   
            </Timeline>    
            </Modal>
        )
    }
}

function mapStateToProps(state){
    
    return {
        contractInfo: state.contractData.contractInfo,
        followHistory: state.contractData.contractInfo.followHistory,
        followHistoryVisible: state.contractData.followHistoryVisible
    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch,
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(FollowHistory);