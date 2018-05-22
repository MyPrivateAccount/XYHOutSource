import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableAcmentPage = Loadable({//业绩分摊项设置
    loader: () => import('./baseSet/acmentSet'),
    loading: () => <LoadableLoading />,
});
const LoadablePeopleSetPage = Loadable({//人数分摊组织设置
    loader: () => import('./baseSet/peopleOrgSet'),
    loading: () => <LoadableLoading />,
});
const LoadableInComeScaleSetPage = Loadable({//分成比例设置
    loader: () => import('./baseSet/incomeScaleSet'),
    loading: () => <LoadableLoading />,
});
const LoadableOrgParamSetPage = Loadable({//组织参数设置
    loader: () => import('./baseSet/orgParamSet'),
    loading: () => <LoadableLoading />,
});
const LoadableDealRpPage = Loadable({//我录入的成交报告
    loader:()=>import('./dealRp/myDealRp'),
    loading:()=><LoadableLoading/>
});
const LoadableDealRpQueryPage = Loadable({//我录入的成交报告
    loader:()=>import('./dealRp/dealRpQuery'),
    loading:()=><LoadableLoading/>
});

function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID === "menu_yjftxsz") {
        console.log("menu_yjftxsz");
        return <LoadableAcmentPage/>;
    }
    else if(curMenuID === "menu_rsftzzsz"){
        console.log("menu_rsftzzsz");
        return <LoadablePeopleSetPage/>;
    }
    else if(curMenuID === "menu_tcblsz"){
        console.log("menu_tcblsz");
        return <LoadableInComeScaleSetPage/>;
    }
    else if(curMenuID === "menu_zzcssz"){
        console.log("menu_zzcssz");
        return <LoadableOrgParamSetPage/>;
    }
    else if(curMenuID === 'menu_myrp'){
        console.log("menu_myrp");
        return <LoadableDealRpPage/>;
    }
    else if(curMenuID === 'menu_query'){
        console.log("menu_query");
        return <LoadableDealRpQueryPage/>;
    }
    return null;
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