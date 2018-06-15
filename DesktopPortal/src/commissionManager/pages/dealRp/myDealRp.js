//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeManager from './rpdetails/tradeManager'
import DealRpTable from './dealRpTable'
import SearchCondition from '../../constants/searchCondition'
import { rpClear,syncRp,syncWy,syncYz,syncKh,syncFp } from '../../actions/actionCreator'

const { Header, Content } = Layout;
const Option = Select.Option;

class MyDealRp extends Component {
    state = {
        isShowManager: false,
        cd: SearchCondition.rpListCondition,
        rpId: '',
        editReport: false
    }
    handleDelClick = (info) => {

    }
    handleModClick = (info) => {

    }
    handleNew = (info) => {
        this.setState({ isShowManager: true, rpId: '', editReport: false })
        this.clearRp()
    }
    //清除数据
    clearRp = (e) => {
        this.props.dispatch(syncRp({}))
        this.props.dispatch(syncWy({}))
        this.props.dispatch(syncYz({}))
        this.props.dispatch(syncKh({}))
        this.props.dispatch(syncFp({}))
    }
    handleBack = (e) => {
        this.setState({ isShowManager: false })
        this.rptb.handleMySearch()
    }
    componentWillMount = () => {

    }
    componentDidMount = () => {

    }
    componentWillReceiveProps = (newProps) => {

        if (newProps.operInfo.operType === 'DEALRP_OPEN_RP_DETAIL') {
            this.setState({ isShowManager: true, rpId: newProps.rpOpenParam.id, editReport: true })
            newProps.operInfo.operType = ''
        }
    }
    onRpTable = (ref) => {
        this.rptb = ref
    }
    render() {
        const rowSelection = {
            onChange: (selectedRowKeys, selectedRows) => {
                console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
            },
            getCheckboxProps: record => ({
                disabled: record.name === 'Disabled User', // Column configuration not to be checked
                name: record.name,
            }),
        };
        return (
            <Layout>
                <div style={{ display: !this.state.isShowManager ? 'block' : 'none' }}>
                    <Tooltip title="新增">
                        <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{ 'margin': '10' }} />
                    </Tooltip>
                    <DealRpTable SearchCondition={this.state.cd} onRpTable={this.onRpTable} />
                </div>
                <TradeManager vs={this.state.isShowManager} handleback={this.handleBack} rpId={this.state.rpId} isEdit={this.state.editReport} />
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        operInfo: state.rp.operInfo,
        rpOpenParam: state.rp.rpOpenParam,
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(MyDealRp);