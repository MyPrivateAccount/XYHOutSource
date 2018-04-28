import React, {Component} from 'react'
import {Layout, Button, Spin, Menu, Table, Tooltip, Popconfirm} from 'antd'
import {connect} from 'react-redux'
import FollowUpDialog from './followUpDialog'
import {getFootPrint} from '../actions'


class FootPrint extends Component {
    state = {
        activeTab: 'noProcess',
        searchCondition: {followUpTypes: [], pageIndex: 0, pageSize: 10}
        // pagination: {current: 1, pageSize: 10, total: 0}
    }
    componentWillMount = () => {
        let condition = {...this.state.searchCondition};
        this.props.getFootPrint(condition);
    }

    handleMenuChange = (e) => {
        let condition = {...this.state.searchCondition};
        console.log("key", e.key);
        this.setState({activeTab: e.key});
        if (e.key === "noProcess") {
            condition.pageIndex = 0;
        } else {
            condition.pageIndex = 0;
        }
        this.props.getFootPrint(condition);
    }

    render() {
        let {activeTab} = this.state;
        let {footPrintDataSource, pagination} = this.props.footPrint;
        const baseColumns = [
            {title: '客户电话号码', dataIndex: 'phone', width: '10rem'},
            {title: '意向楼盘', dataIndex: 'buildingName', width: '10rem'},
            {title: '意向商铺', dataIndex: 'shopName', width: '5rem'},
            {title: '商铺地址', dataIndex: 'address', width: '10rem'},
            {title: '意向服务', dataIndex: 'intentionsType'},
        ];
        const footPrintColumns = [
            ...baseColumns,
            {title: '房源详情', dataIndex: 'key', width: '10rem', render: (text, record) => (<Button>房源详情</Button>)},
            {title: '确认处理', dataIndex: 'value', width: '10rem', render: (text, record) => (<Button>确认处理</Button>)}
        ];
        const followUpColumns = [
            ...baseColumns,
            {title: '跟进日期', dataIndex: 'followUpTime', width: '10rem'},
            {title: '跟进记录', dataIndex: 'content', width: '10rem'},
            {title: '跟进人', dataIndex: 'userTrueName', width: '10rem'},
            {title: '详情', dataIndex: 'value', width: '10rem', render: (text, record) => (<Button>查看详情</Button>)},
            {title: '再次跟进', dataIndex: 'value', width: '10rem', render: (text, record) => (<Button>再次跟进</Button>)}
        ];
        return (
            <div className="inner-page">
                <Menu onClick={this.handleMenuChange} selectedKeys={[this.state.activeTab]}
                    mode="horizontal">
                    <Menu.Item key="noProcess">未处理</Menu.Item>
                    <Menu.Item key="hadProcess">已处理</Menu.Item>
                </Menu>
                <Table rowKey={record => record.id} columns={activeTab === 'noProcess' ? footPrintColumns : followUpColumns} dataSource={footPrintDataSource} pagination={pagination} />

                {/* <FollowUpDialog /> */}
            </div>
        )
    }
}

const mapStateToProps = (state, action) => {
    return {
        footPrint: state.footPrint
    }
};

const mapDispatchToProps = (dispatch) => {
    return {
        getFootPrint: (...args) => dispatch(getFootPrint(...args)),
        dispatch
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(FootPrint);
