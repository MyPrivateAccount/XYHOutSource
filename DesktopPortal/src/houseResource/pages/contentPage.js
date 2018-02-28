import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableIndexPage = Loadable({//首页
    loader: () => import('./index'),
    loading: () => <LoadableLoading />,
});
const LoadableXkCenterPage = Loadable({//销控中心
    loader: () => import('./xkCenter/xkCenter'),
    loading: () => <LoadableLoading />,
});
const LoadableBuildingPage = Loadable({//楼盘录入
    loader: () => import('./buildingDish/edit/buildingDishEdit'),
    loading: () => <LoadableLoading />,
});
const LoadableShopPage = Loadable({//商铺录入
    loader: () => import('./shops/edit/shopsEdit'),
    loading: () => <LoadableLoading />,
});
const LoadableAuditPage = Loadable({//房源审核
    loader: () => import('./houseAudit/houseAudit'),
    loading: () => <LoadableLoading />,
});
const LoadableSendMsgPage = Loadable({//发送消息
    loader: () => import('./message/sendMessage'),
    loading: () => <LoadableLoading />,
});
const LoadableManagerPage = Loadable({//驻场管理
    loader: () => import('./zcManagement/zcManagement'),
    loading: () => <LoadableLoading />,
});
const LoadableActiveProjectPage = Loadable({//房源动态楼盘详情
    loader: () => import('./houseActive/activeProject'),
    loading: () => <LoadableLoading />,
});
const LoadableActiveShopPage = Loadable({//房源动态商铺详情
    loader: () => import('./houseActive/activeShop'),
    loading: () => <LoadableLoading />,
});

function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID === "menu_index") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID === "menu_building_dish") {
        return <LoadableBuildingPage />;
    }
    else if (curMenuID === "menu_shops") {
        return <LoadableShopPage />;
    }
    else if (curMenuID === "menu_xkCenter") {
        return <LoadableXkCenterPage />;
    }
    else if (curMenuID === "menu_houseAudit") {
        return <LoadableAuditPage />;
    }
    else if (curMenuID === "menu_message") {
        return <LoadableSendMsgPage />;
    }
    else if (curMenuID === "menu_manager") {
        return <LoadableManagerPage />;
    }
    else if (curMenuID === "activeProject") {
        return <LoadableActiveProjectPage />;
    }
    else if (curMenuID === "activeShop") {
        return <LoadableActiveShopPage />;
    }
}

function mapStateToProps(state) {
    return {

    };
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default connect(null, mapDispatchToProps)(ContentPage);