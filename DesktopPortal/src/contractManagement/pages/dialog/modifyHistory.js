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
    getTableColumns = () =>{
        let columns = [
            {
                title: '修改时间',
                //width: 80,
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
            },
            {
                title: '修改人',
                //width: 80,
                dataIndex: 'modifyPepole',
                key: 'modifyPepole',
                
            },
            {
                title:'操作类型',
                dataIndex: 'type',
                key: "type",
                render:(record, text) =>{
                    let newText = record;
                    if (record) {
                        newText = this.getModifyType(record);
                    }
                    return (<span>{newText}</span>);
                }
            }
        ]
        return columns;
    }
   

    render(){
    
        let dataSource = this.props.modifyinfo.sort((a, b) => {return (moment(a.modifyStartTime).isSameOrBefore(moment(b.modifyStartTime)) ? -1: 1) }) ? this.props.modifyinfo: [];
        console.log('dataSource:', dataSource);
        return(
            <Modal 
                title="修改历史"  wrapClassName="vertical-center-modal" confirmLoading={this.props.showLoading} className='contractChoose' footer={null} maskClosable={false} visible={this.props.modifyHistoryVisible} 
                onCancel={this.handleCancel}
            >
                <Table rowKey={record => record.uid} scroll={{ y: 240 }} columns={this.getTableColumns()} pagination={{ pageSize: 5 }}  size="middle" dataSource={dataSource}   />  
            </Modal>
        )
    }
}

function mapStateToProps(state){
    return {
        modifyinfo: state.contractData.contractInfo.modifyinfo,
        modifyHistoryVisible: state.contractData.modifyHistoryVisible
    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch,
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ModifyHistory);