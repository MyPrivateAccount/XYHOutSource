//月结页面
import React, { Component } from 'react'
import { Layout, Row, Col, Button, TreeSelect, Input, Modal } from 'antd';
import { connect } from 'react-redux';
import { orgGetPermissionTree, yjGetMonth, yjStart, yjCheckState, yjCancel, yjRollBack, yjQrQuery, yjYjQrCommit, yjSkQuery, yjSkCommit,yjQrEmpQuery, yjSkEmpQuery} from '../../actions/actionCreator'
import LZRYTJTable from './lzryYjTable'
import SFKJQRTable from './sfkjQrTable'

class MonthSum extends Component {
    //内部状态
    state = {
        monthData: { stage: '' },
        btnState: false,//是否禁用
        timer: null,
        yjqrResult:[],
        yjskResult:[],
        oldbranchId:''
    }
    componentDidMount() {
        //获取权限组织
        this.props.dispatch(orgGetPermissionTree("YJ_CW_YJ"));
    }
    componentWillReceiveProps(newProps) {
        if (newProps.operInfo.operType === 'YJ_MONTH_GETUPDATE' ||
            newProps.operInfo.operType === 'YJ_MONTH_CHECK_UPDATE' ||
            newProps.operInfo.operType === 'YJ_MONTH_ROLLBACK_UPDATE') {
            this.setState({ monthData: newProps.result.extension })
            newProps.operInfo.operType = ''
        }
        else if (newProps.operInfo.operType === 'YJ_MONTH_CANCEL_UPDATE') {
            this.setState({ monthData: newProps.result.extension })
            newProps.operInfo.operType = ''
            if (newProps.result.extension.stage === 'STAGE_CANCEL_WAIT') {
                //启动定时器
                this.startCheckTimer()
            }
        }
        else if(newProps.operInfo.operType === 'YJ_MONTH_YJQR_QUERY_UPDATE'){
            if(newProps.yjqrResult){
                this.setState({yjqrResult:newProps.yjqrResult})
            }
            newProps.operInfo.operType=''
        }
        else if(newProps.operInfo.operType === 'YJ_MONTH_SKQR_QUERY_UPDATE'){
            if(newProps.yjskResult){
                this.setState({yjskResult:newProps.yjskResult})
            }
            newProps.operInfo.operType=''
        }
        else if(newProps.operInfo.operType === 'YJ_MONTH_EMP'){
            let emps = newProps.emps
            let yjqr = []
            for(let i=0;i<emps.length;i++){
                let emp = {}
                emp.id = emps[i].id
                emp.isInclude = emps[i].isInclude
                yjqr.push(emp)
            }
            let rq = {}
            rq.emps = yjqr
            rq.branchId = this.state.monthData.branchId
            rq.yyyymm = this.state.monthData.yyyymm
            this.props.dispatch(yjYjQrCommit(rq))
            newProps.operInfo.operType = ''
        }
        else if(newProps.operInfo.operType === 'YJ_MONTH_SKQR_EMP'){
            let emps = newProps.emps
            let yjqr = []
            for(let i=0;i<emps.length;i++){
                let emp = {}
                emp.yyyymm = emps[i].yyyymm
                emp.branchId = emps[i].branchId
                emp.userId= emps[i].userId
                emp.belongId= emps[i].belongId
                emp.position= emps[i].position
                emp.byKjJe= emps[i].byKjJe
                yjqr.push(emp)
            }
            let rq = {}
            rq.emps = yjqr
            rq.branchId = this.state.monthData.branchId
            rq.yyyymm = this.state.monthData.yyyymm
            this.props.dispatch(yjSkCommit(rq))
            newProps.operInfo.operType = ''
        }
    }
    componentWillUnmount() {
        if (this.state.timer) {
            clearInterval(this.state.timer)
        }
    }
    //组织改变
    handleOrgChange = (e) => {
        //查询月结月份
        let searchCondition = {}
        searchCondition.branchId = e
        this.props.dispatch(yjGetMonth(searchCondition))
        this.startCheckTimer()
    }
    //根据月结的状态显示不同的按钮
    showButtonByState = () => {
        let stage = this.state.monthData.stage
        if (stage) {
            if (stage === 'STAGE_UNSTART' ||
                stage === 'STAGE_ROLLBACK' ||
                stage === 'STAGE_CANCEL') {
                //显示开始月结按钮
                return <span><Button disabled={this.state.btnState} type="primiary" onClick={this.handleYjStart}>开始月结</Button></span>
            }
            else if (stage === 'STAGE_FINISH') {
                //显示月结回滚按钮
                return <span><Button disabled={this.state.btnState} type="primiary" onClick={this.handleYjRollBack}>月结回滚</Button></span>
            }
            else if (stage === 'STAGE_ERROR') {
                //显示月结重试按钮
                return <span><Button type="primiary" onClick={this.handleYjStart}>月结重试</Button></span>
            }
            else if (stage === 'STAGE_ROLLBACKING' || stage === 'STAGE_CANCEL_WAIT') {
                return null//不显示任何按钮
            }
            else if (stage === 'STAGE_YJQR') {
                //显示离职人员业绩确认页面
                if(this.state.oldbranchId!==this.state.monthData.branchId){
                    let info = {}
                    info.yyyymm = this.state.monthData.yyyymm
                    info.branchId = this.state.monthData.branchId
                    this.props.dispatch(yjQrQuery(info))//查询业绩
                    this.setState({oldbranchId:info.branchId})
                }
                return null
            }
            else if (stage === 'STAGE_KKQR') {
                //显示显示实发扣减确认页面
                if(this.state.oldbranchId!==this.state.monthData.branchId){
                    let info={}
                    info.yyyymm = this.state.monthData.yyyymm
                    info.branchId = this.state.monthData.branchId
                    this.props.dispatch(yjSkQuery(info))//查询扣减情况
                    this.setState({oldbranchId:info.branchId})
                }
                return null
            }
            else {
                return <span><Button type="primiary" onClick={this.handleYjCancel}>取消月结</Button></span>
            }
        }
    }
    //开始月结
    handleYjStart = () => {
        this.setState({ btnState: true })
        //调用开始月结接口
        let info = {}
        info.yyyymm = this.state.monthData.yyyymm
        info.branchId = this.state.monthData.branchId
        this.props.dispatch(yjStart(info))
        //开启定时器
        this.startCheckTimer()
    }
    //启动状态检查定时器
    startCheckTimer = () => {
        //开启定时器
        if (this.state.timer) {
            clearInterval(this.state.timer)
        }
        let tm = setInterval(this.checkYjState, 20000)
        this.setState({ timer: tm })
    }
    //检查月结状态
    checkYjState = () => {
        //进度查询接口
        let info = {}
        info.yyyymm = this.state.monthData.yyyymm
        info.branchId = this.state.monthData.branchId
        this.props.dispatch(yjCheckState(info))
    }
    //月结回滚
    handleYjRollBack = () => {
        let info = {}
        info.yyyymm = this.state.monthData.yyyymm
        info.branchId = this.state.monthData.branchId
        this.props.dispatch(yjRollBack(info))
        this.setState({ btnState: true })
    }
    //月结取消
    handleYjCancel = () => {
        let info = {}
        info.yyyymm = this.state.monthData.yyyymm
        info.branchId = this.state.monthData.branchId
        this.props.dispatch(yjCancel(info))
        this.setState({ btnState: true })
    }
    //提交确认
    handleOk = () => {
        if(this.state.monthData.stage === 'STAGE_YJQR'){
            this.props.dispatch(yjQrEmpQuery())
        }
        else if(this.state.monthData.stage === 'STAGE_KKQR'){
            this.props.dispatch(yjSkEmpQuery())
        }
        
    }
    //取消
    handleCancel = () => {
        let monthData = { ...this.state.monthData }
        monthData.stage = ''
        this.setState({ monthData })
    }
    getBtnColorByState=(state)=>{
        if(state === this.state.monthData.stage){
            return 'red'
        }
        return 'gray'
    }
    render() {
        return (
            <Layout>
                <Layout.Content>
                    <Row style={{ margin: 10 }}>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button style={{backgroundColor:this.getBtnColorByState('STAGE_UNSTART')}} type="primiary">1</Button>
                            <span>________</span>
                            <Button style={{backgroundColor:this.getBtnColorByState('STAGE_YJQR')}} type="primiary">2</Button>
                            <span>________</span>
                            <Button style={{backgroundColor:this.getBtnColorByState('STAGE_KKQR')}} type="primiary">3</Button>
                            <span>________</span>
                            <Button style={{backgroundColor:this.getBtnColorByState('STAGE_FINISH')}} type="primiary">4</Button>
                        </Col>
                    </Row>
                    <Row style={{ margin: 10, marginLeft: 360 }}>
                        <Col span={2} style={{ textAlign: 'center' }}>
                            <span>月结检查</span>
                        </Col>
                        <Col span={4} style={{ textAlign: 'center' }}>
                            <span>离职人员业绩确认</span>
                        </Col>
                        <Col span={3} style={{ textAlign: 'center' }}>
                            <span>实发扣减确认</span>
                        </Col>
                        <Col span={2} style={{ textAlign: 'center' }}>
                            <span>月结完成</span>
                        </Col>
                    </Row>
                    <Row style={{ marginTop: 80 }}>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <label>
                                <span>分公司</span>
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.YJOrgTree}
                                    placeholder="分公司"
                                    onChange={this.handleOrgChange}>
                                </TreeSelect>
                            </label>
                        </Col>
                    </Row>
                    <Row style={{ marginTop: 10 }}>
                        <Col span={24} style={{ textAlign: 'center', marginLeft: -50 }}>
                            <label>
                                <span>月结年月</span>
                                <Input value={this.state.monthData.yyyymm} style={{ width: 200 }} disabled={true}></Input>
                            </label>
                        </Col>
                    </Row>
                    <Row style={{ marginTop: 10 }}>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            {
                                this.showButtonByState()
                            }
                        </Col>
                    </Row>
                    <Modal width={800} title={this.state.monthData.stage === 'STAGE_YJQR'?'离职人员业绩确认表':'实发扣减表'} maskClosable={false} visible={(this.state.monthData.stage === 'STAGE_YJQR' || this.state.monthData.stage === 'STAGE_KKQR') ? true : false}
                        onOk={this.handleOk} onCancel={this.handleCancel} >
                        {
                            this.state.monthData.stage === 'STAGE_YJQR' ? <LZRYTJTable showSearch={false} dataSource={this.state.yjqrResult.extension}/> : <SFKJQRTable showSearch={false} dataSource={this.state.yjskResult.extension}/>
                        }
                    </Modal>
                </Layout.Content>
            </Layout>
        )
    }
}
function monthSumaryMapStateToProps(state) {
    return {
        permissionOrgTree: state.org.permissionOrgTree,
        result: state.month.result,
        startYjResult: state.month.startYjResult,
        operInfo: state.month.operInfo,
        yjqrResult:state.month.yjqrResult,
        yjskResult:state.month.yjskResult,
        emps:state.month.emps
    }
}

function monthSumaryMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(monthSumaryMapStateToProps, monthSumaryMapDispatchToProps)(MonthSum);