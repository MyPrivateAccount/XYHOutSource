//月结页面
import React, { Component } from 'react'
import { Layout, Row, Col, Button, TreeSelect,Input } from 'antd';
import { connect } from 'react-redux';
import { orgGetPermissionTree,yjGetMonth } from '../../actions/actionCreator'

class MonthSum extends Component {
    //内部状态
    state={
        monthData:{}
    }
    componentDidMount() {
        //获取权限组织
        this.props.dispatch(orgGetPermissionTree("YJ_CW_YJ"));
    }
    componentWillReceiveProps(newProps) {
        if(newProps.operInfo.operType === 'YJ_MONTH_GETUPDATE'){
            this.setState({monthData:newProps.result.extension})
            newProps.operInfo.operType = ''
        }
    }
    //组织改变
    handleOrgChange=(e)=>{
        //查询月结月份
        let searchCondition={}
        searchCondition.branchId = e
        this.props.dispatch(yjGetMonth(searchCondition))
    }
    //根据月结的状态显示不同的按钮
    showButtonByState=()=>{
        let stage = this.state.monthData.stage
        if(stage){
            if(stage === 'STAGE_UNSTART'||
               stage === 'STAGE_ROLLBACK'||
               stage === 'STAGE_CANCEL'){
                   //显示开始月结按钮
                   return <span><Button type="primiary">开始月结</Button></span>
            }
        }
    }
    render() {
        return (
            <Layout>
                <Layout.Content>
                    <Row style={{ margin: 10 }}>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type="primiary">1</Button>
                            <span>________</span>
                            <Button type="primiary">2</Button>
                            <span>________</span>
                            <Button type="primiary">3</Button>
                            <span>________</span>
                            <Button type="primiary">4</Button>
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
                </Layout.Content>
            </Layout>
        )
    }
}
function monthSumaryMapStateToProps(state) {
    return {
        permissionOrgTree: state.org.permissionOrgTree,
        result:state.month.result,
        operInfo:state.month.operInfo
    }
}

function monthSumaryMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(monthSumaryMapStateToProps, monthSumaryMapDispatchToProps)(MonthSum);