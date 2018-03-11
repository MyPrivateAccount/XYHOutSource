import React from 'react'
import Loadable from 'react-loadable';
import LoadableLoading from './components/LoadableLoading';
import { Icon } from 'antd'
import * as Vendor from './vendor'

function createLoadableComponent(loader) {
    return Loadable({
        loader: () => loader,
        loading: () => <LoadableLoading />,
    });
}

//所有插件/工具必须在此文件中注册
const tools = [
    {
        id: 'xtwh', //系统维护插件
        mutipleInstance: false, //是否支持多窗口
        component: () => createLoadableComponent(import('./xtwh')),
        cache: null,
        icon: <Icon type="setting" className="icon" />
    },
    {
        id: 'privilegeManager',//权限中心
        mutipleInstance: false, //是否支持多窗口
        component: () => createLoadableComponent(import('./privilegeManager')),
        cache: null,
        icon: <Icon type="user" className="icon" />
    },
    {
        id: 'houseSource',//驻场
        mutipleInstance: false,
        component: () => createLoadableComponent(import('./houseResource')),
        cache: null,
        icon: <Icon type="home" className="icon" />
    },
    {
        id: 'searchTool',//房源检索
        mutipleInstance: false,
        component: () => createLoadableComponent(import('./searchTool')),
        cache: null,
        icon: <Icon type="search" className="icon" />
    },
    {
        id: 'flowDesign',//流程设计器
        mutipleInstance: false,
        component: () => createLoadableComponent(import('./flowDesign')),
        cache: null,
        icon: <Icon type="share-alt" className="icon" />
    },
    {
        id: 'auditCenter',//审核中心
        mutipleInstance: false,
        component: () => createLoadableComponent(import('./auditCenter')),
        cache: null,
        icon: <Icon type="safety" className="icon" />
    },
    {
        id: 'customerManager',//客源管理
        mutipleInstance: false,
        component: () => createLoadableComponent(import('./customerManager')),
        cache: null,
        icon: <Icon type="solution" className="icon" />
    },
    {
        id: 'recommendTool',//房源推荐工具
        mutipleInstance: false,
        component: () => createLoadableComponent(import('./recommendTool')),
        cache: null,
        icon: <Icon type="like-o" className="icon" />
    },
    {
        id:'personnelSystem',//人事系统
        mutipleInstance:false,
        component: ()=>createLoadableComponent(import('./personnelSystem')),//() => createLoadableComponent(import('./personnelSystem')),
        cache: null,
        icon: <Icon type="user" className="icon"/>
    },
    {
        id:'contractManagement',//合同管理
        mutipleInstance:false,
        component: ()=>createLoadableComponent(import('./contractManagement')),//() => createLoadableComponent(import('./personnelSystem')),
        cache: null,
        icon: <Icon type="book" className="icon"/>
    }
]

export function getToolDefine(id) {
    let tool = tools.find(x => x.id === id);

    if (tool) {
        let toolCopy = { ...tool };
        delete toolCopy.cache;
        return toolCopy;
    }
    return tool;
}

export default function getToolComponent(id) {
    let tool = tools.find(x => x.id === id);
    if (tool) {
        if (tool.cache) {
            return tool.cache;
        }
        tool.cache = tool.component();
        return tool.cache;
    }
    //审核工具引用页面
    tool = auditTools.find(x => x.id === id);
    if (tool) {
        if (tool.cache) {
            return tool.cache;
        }
        tool.cache = tool.component();
        return tool.cache;
    }
    return null;
}

/*************审核相关的工具***************/
const auditTools = [{
    id: 'houseInfo', //楼盘详细页面
    component: () => createLoadableComponent(import('./houseResource/houseAuditViewIndex.js')),
    cache: null
}, {
    id: 'houseActiveInfo', //动态详细页面
    component: () => createLoadableComponent(import('./houseResource/houseActiveAuditView.js')),
    cache: null
}, {
    id: 'customerAuditInfo', //调客审核详细
    component: () => createLoadableComponent(import('./auditCenter/auditDetailView.js')),
    cache: null
}, {
    id: 'BuildingSearchAuditInfo', //楼盘检索缩略信息项
    component: () => createLoadableComponent(import('./searchTool/buildingSearchView.js')),
    cache: null
},
{
    id: 'ContractInfo', //合同检索缩略信息项
    component: () => createLoadableComponent(import('./contractManagement/ContractAuditViewIndex.js')),
    cache: null
},
];
