import {connect} from 'react-redux';
//import {closeAdjustCustomer, getUserByOrg, adjustCustomer, setLoadingVisible} from '../../actions/actionCreator';
import {closeModifyHistory} from '../../actions/actionCreator';
import React, {Component} from 'react';
import {Row, Col, Modal, Select, TreeSelect, Form, Table,Button} from 'antd';


const Option = Select.Option;
const FormItem = Form.Item;

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
                // width: 80,
                dataIndex: 'modifyTime',
                key: 'modifyTime',

            },
            {
                title: '修改人',
                // width: 80,
                dataIndex: 'modifyUser',
                key: 'modifyUser',
                
            },
            {
                title:'操作类型',
                dataIndex: 'modifyType',
                key: "modifyType"
            }
        ]
        return columns;
    }


    render(){
        let dataSource = this.modifyRecord;
        return(
            <Modal 
                title="修改历史" style={{ top: 20 }} confirmLoading={this.props.showLoading} className='contractChoose' maskClosable={false} visible={this.props.modifyHistoryVisible} 
                onCancel={this.handleCancel}
            >
                <Table rowKey={record => record.uid} scroll={{ y: 240 }} columns={this.getTableColumns()} pagination={{ pageSize: 5 }} bordered dataSource={dataSource}  />  
            </Modal>
        )
    }
}

function mapStateToProps(state){
    return {
        modifyRecord: state.contractData.contractInfo.modifyInfo,
        modifyHistoryVisible: state.contractData.modifyHistoryVisible
    };
}

function mapDispatchToProps(dispatch){
    return {
        dispatch,
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ModifyHistory);