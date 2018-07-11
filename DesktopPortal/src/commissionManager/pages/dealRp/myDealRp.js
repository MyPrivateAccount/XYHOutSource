//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import {notification, Button, Tooltip } from 'antd'
import TradeManager from './rpdetails/tradeManager'
import DealRpTable from './dealRpTable'
import Layer, { LayerRouter } from '../../../components/Layer'
import {Route } from 'react-router'
import uuid from 'uuid'
import {getDicParList} from '../../../actions/actionCreators'
import {dicKeys} from '../../constants/const'
import WebApiConfig from '../../constants/webApiConfig'
import ApiClient from '../../../utils/apiClient'
import {convertReport} from '../../constants/utils'


class MyDealRp extends Component {
    state = {
        list:[],
        loading: false,
        pagination:{pageSize:1, pageIndex: 1, total:0}
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
            bswylx:"一手商铺",
            cjbglx:"商铺",
            jylx: "1",
            xmlx:'1',
            xxjylx:'1',
            cqlx:'1',
            fkfs:'全款',
            sfzjjg:'1',
            htlx:'3',
            cjzj:0,
            ycjyj:0,
            reportWy:{
                wyWylx:"1",
                wyKjlx:"多层",
                wyLl:0,
                wyDts:1,
                wyZxzk:'清水房',
                wyZxnd:new Date().getFullYear(),
                wyJj:"部分",
                wyCx:'东',
                wySfhz:true,
                wyFyfkfs:'全款'
            },
            reportYz:{
                yzCdjzqhtsc:'0~1'
            },
            reportKh:{
                khKhxz:'本地居民',
                khCdjzqhtsc: '0~1'
            },
            distribute:{
                id: uuid.v1()
            }
        }
        this.props.history.push(`${this.props.match.url}/reportInfo`, {entity: newEntity, op: 'add', pagePar: this.state.pagePar})
    }

    componentWillMount = () => {

    }
    componentDidMount = () => {
        this.props.getDicParList([
            dicKeys.jylx
        ]);

        this.search();
    }

    search =async ()=>{
        let condition = {};

        condition.pageSize = this.state.pagination.pageSize;
        condition.pageIndex = this.state.pagination.pageIndex;
        this.setState({loading:true})
        try{
            let url = `${WebApiConfig.rp.myrpGet}`;
            let r = await ApiClient.post(url, condition);
            r = (r||{}).data;
            if(r.code==='0'){
                this.setState({list: r.extension, pagination: {...this.state.pagination, total: r.totalCount}});
            }else{
                notification.error({message:'查询成交报告列表失败'});
            }
        }catch(e){
            notification.error({message:'查询成交报告列表失败'})
        }
        this.setState({loading:false})
    }

    pageChanged = (pagination, filters, sorter)=>{
        this.setState({
            pagination:{...this.state.pagination,...{pageIndex:pagination.current}}
        },()=>{
            this.search();
        })
    }

    viewReport =async (report, action)=>{
        if(!report || !report.id)
            return;
       
        this.setState({loading:true})
        try{
            let url = `${WebApiConfig.rp.rpGet}${report.id}`;
            let r = await ApiClient.get(url);
            r = (r||{}).data;
            if(r.code==='0' ){
                if(r.extension){
                    let es = r.extension.examineStatus
                    let report = convertReport(r.extension);
                    let op =action || ((es===0 || es === 16)? 'edit':'view')
                    this.props.history.push(`${this.props.match.url}/reportInfo`, {entity: report, op: op, pagePar: this.state.pagePar})
                }else{
                    notification.error({message:'成交报告不存在'});
                }
            }else{
                notification.error({message:'获取成交报告详情失败', description: r.message||''});
            }
        }catch(e){
            notification.error({message:'获取成交报告详情失败'})
        }
        this.setState({loading:false})
    }

    reportChanged = (report, action)=>{
        if(action==='DEL'){
            let rl = this.state.list;
            let idx = rl.findIndex(r=>r.id === report.id);
            if(idx>=0){
                rl.splice(idx,1)
                this.setState({list: [...rl]})
            }
        }else{
            let rl = this.state.list;
            let idx = rl.findIndex(r=>r.id === report.id);
            if(idx>=0){
                rl[idx] = {...rl[idx],...report}
                this.setState({list: [...rl]})
            }
        }
    }
  
   
    render() {
        
        return (
            <Layer className="content-page">
                <div style={{ display: !this.state.isShowManager ? 'block' : 'none' }}>
                    <Tooltip title="新增">
                        <Button type='primary'  onClick={this.handleNew} style={{ 'margin': '10' }} >录入成交报告</Button>
                    </Tooltip>
                    <DealRpTable 
                        loading={this.state.loading}
                        viewReport={this.viewReport}
                        dic = {this.props.dic} 
                        dataSource={this.state.list} 
                        pagination={this.state.pagination} 
                        pageChanged={this.pageChanged} 
                        reportChanged = {this.reportChanged}
                        type={'myget'}/>
                </div>
                <LayerRouter>
                    <Route path={`${this.props.match.url}/reportInfo`}  render={(props)=> <TradeManager {...props} reportChanged={this.reportChanged}  />}/>
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
        dispatch,
        getDicParList: (...args) => dispatch(getDicParList(...args))
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(MyDealRp);