//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, notification, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeManager from './rpdetails/tradeManager'
import DealRpTable from './dealRpTable'
import SearchCondition from '../../constants/searchCondition'
import { rpClear,syncRp,syncWy,syncYz,syncKh,syncFp } from '../../actions/actionCreator'
import Layer, { LayerRouter } from '../../../components/Layer'
import {Route } from 'react-router'
import uuid from 'uuid'

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
        if(!this.props.user.Filiale){
            notification.error({message:'您没有归属于任何分公司，无法录入成交报告'})
            return;
        }

        var newEntity = {
            id: uuid.v1(),
            gsmc: this.props.user.Filiale,
            gsmcName: this.props.user.FilialeName,
            bswylx:"1",
            cjbglx:"1",
            jylx: "1",
           
        }
        this.props.history.push(`${this.props.match.url}/reportInfo`, {entity: newEntity, op: 'add', pagePar: this.state.pagePar})

      //  this.setState({ isShowManager: true, rpId: '', editReport: false })
      //  this.clearRp()
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

        // if (newProps.operInfo.operType === 'DEALRP_OPEN_RP_DETAIL') {
        //     this.setState({ isShowManager: true, rpId: newProps.rpOpenParam.id, editReport: true })
        //     newProps.operInfo.operType = ''
        // }
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
            <Layer className="content-page">
                <div style={{ display: !this.state.isShowManager ? 'block' : 'none' }}>
                    <Tooltip title="新增">
                        <Button type='primary'  onClick={this.handleNew} style={{ 'margin': '10' }} >录入成交报告</Button>
                    </Tooltip>
                    <DealRpTable SearchCondition={this.state.cd} onRpTable={this.onRpTable} type={'myget'}/>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/reportInfo`}  render={(props)=> <TradeManager {...props} vs={this.state.isShowManager} handleback={this.handleBack} rpId={this.state.rpId} isEdit={this.state.editReport} />}/>
                </LayerRouter>
               
            </Layer>
        )
    }
}
function MapStateToProps(state) {

    return {
        // operInfo: state.rp.operInfo,
        // rpOpenParam: state.rp.rpOpenParam,
        dic: state.basicData.dicList,
        user: state.oidc.user.profile||{}
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(MyDealRp);