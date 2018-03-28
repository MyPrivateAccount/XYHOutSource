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
    getTableColumns = () =>{
        let columns = [
            {
                title: '修改时间',
                //width: 80,
                dataIndex: 'modifyStartTime',
                key: 'modifyStartTime',
                render: (record, text) =>{
                    let newText = text;
                    if (text) {
                        newText = moment(text).format('YYYY-MM-DD');
                    }
                    return (<span>{newText}</span>);
                }
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
                
            }
        ]
        return columns;
    }


    render(){

        let dataSource = this.props.modifyinfo;
        return(
            <Modal 
                title="修改历史"  wrapClassName="vertical-center-modal" confirmLoading={this.props.showLoading} className='contractChoose' footer={null} maskClosable={false} visible={this.props.modifyHistoryVisible} 
                onCancel={this.handleCancel}
            >
                <Table rowKey={record => record.uid} scroll={{ y: 240 }} columns={this.getTableColumns()} pagination={{ pageSize: 5 }}  size="middle" dataSource={dataSource}  />  
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