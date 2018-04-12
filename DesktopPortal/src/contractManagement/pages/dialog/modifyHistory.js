import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import {closeModifyHistory} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table,Button} from 'antd';
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


class ModifyHistory extends Component{
    lastSearchInfo = {};
    componentWillMount(){

    }

    componentWillReceiveProps(newProps) {

    }
    handleCancel = () => {

        //this.props.dispatch(closeAdjustCustomer());
        this.props.dispatch(closeModifyHistory());
    }
    getModifyType = (type) => {
        switch (type) {
          case 1: return '创建合同';
          case 2: return '修改合同';
          default:
            return type;
        }
      }
    getExamineValueByType = (type) =>{
        switch(type){
            case 0: return "未提交";
            case 1: return "审核中";
            case 8: return "审核通过";
            case 16: return "驳回";
        default:
            return type;
        }

    }
    getTableColumns = () =>{
        let columns = [
            {
                title: '修改时间',
                width: 175,
                dataIndex: 'modifyStartTime',
                key: 'modifyStartTime',
                render: (record, text) =>{
                    
                    let newText = record;
                    if (record) {
                        newText = moment(record).format('YYYY-MM-DD hh:mm:ss');
                    }
                    return (<span>{newText}</span>);
                },
  
                sorter: (a, b)=>{
                    return (moment(a.modifyStartTime).isSameOrBefore(moment(b.modifyStartTime)) ? -1: 1);
                },
                //fixed:"left",
            },
            {
                title: '修改人',
                width: 250,
                dataIndex: 'modifyPepole',
                key: 'modifyPepole',
                //fixed:"left",
            },
            {
                title:'操作类型',
                width: 175,
                dataIndex: 'type',
                key: "type",
                render:(record, text) =>{
                    let newText = record;
                    if (record) {
                        newText = this.getModifyType(record);
                    }
                    return (<span>{newText}</span>);
                },
                //fixed:"left",
            },
            {
                title: '审核状态',
                width: 100,
                dataIndex: 'examineStatus',
                key: 'examineStatus',
                render: (record, text) =>{
                    let newText = record;
                    newText = this.getExamineValueByType(record);
                    
                    return (<span>{newText}</span>);
                },
                //fixed:"left",
            },
        ]
        return columns;
    }
   

    render(){
        console.log("this.props.modifyInfo:", this.props.modifyInfo);
        let dataSource = (this.props.modifyInfo || []).sort((a, b) => {return (moment(a.modifyStartTime).isSameOrBefore(moment(b.modifyStartTime)) ? -1: 1) }) ? this.props.modifyInfo: [];
        //console.log('dataSource:', dataSource);
        return(
            <Modal 
                title="修改历史"  wrapClassName="vertical-center-modal"  width='700' confirmLoading={this.props.showLoading} className='contractChoose' footer={null} maskClosable={false} visible={this.props.modifyHistoryVisible} 
                onCancel={this.handleCancel}
            >
                <Table rowKey={record => record.uid} scroll={{ y: 240 }} columns={this.getTableColumns()} pagination={{ pageSize: 5 }}  size="small" dataSource={dataSource} />  
            </Modal>
        )
    }
}

function mapStateToProps(state){
    
    return {
        modifyInfo: state.contractData.contractInfo.modifyInfo,
        modifyHistoryVisible: state.contractData.modifyHistoryVisible
    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch,
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ModifyHistory);