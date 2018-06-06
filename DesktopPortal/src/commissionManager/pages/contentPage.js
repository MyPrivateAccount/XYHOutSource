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
const LoadableMonthPage = Loadable({//月结
    loader:()=>import('./fina/monthSumary'),
    loading:()=><LoadableLoading/>
});
const LoadablePPFTPage = Loadable({//人员分摊
    loader:()=>import('./fina/ppFtTable'),
    loading:()=><LoadableLoading/>
});
const LoadableYFTCPage = Loadable({//应发提成
    loader:()=>import('./fina/yfTcTable'),
    loading:()=><LoadableLoading/>
});
const LoadableSFTCPage = Loadable({//实发提成
    loader:()=>import('./fina/sfTcTable'),
    loading:()=><LoadableLoading/>
});
const LoadableTCCBPage = Loadable({//提成成本
    loader:()=>import('./fina/tcCbTable'),
    loading:()=><LoadableLoading/>
});
const LoadableYFTCCJPage = Loadable({//应发提成冲减
    loader:()=>import('./fina/yftcCjTable'),
    loading:()=><LoadableLoading/>
});
const LoadableLZRYYJPage = Loadable({//离职人员业绩
    loader:()=>import('./fina/lzryYjTable'),
    loading:()=><LoadableLoading/>
});
const LoadableSFKJQRJPage = Loadable({//实发扣减确认
    loader:()=>import('./fina/sfkjQrTable'),
    loading:()=><LoadableLoading/>
});
const LoadableFYXQBPage = Loadable({//分佣详情表
    loader:()=>import('./fullRp/fyxqQuery'),
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
    else if(curMenuID === 'menu_sumbymonth'){
        console.log("menu_sumbymonth");
        return <LoadableMonthPage/>;
    }
    else if(curMenuID === 'menu_ps'){
        console.log("menu_ps");
        return <LoadablePPFTPage/>;
    }
    else if(curMenuID === 'menu_yftcb'){
        console.log("menu_yftcb");
        return <LoadableYFTCPage/>;
    }
    else if(curMenuID === 'menu_sftcb'){
        console.log("menu_sftcb");
        return <LoadableSFTCPage/>;
    }
    else if(curMenuID === 'menu_tccbftb'){
        console.log("menu_tccbftb");
        return <LoadableTCCBPage/>;
    }
    else if(curMenuID === 'menu_yftccjb'){
        console.log("menu_yftccjb");
        return <LoadableYFTCCJPage/>;
    }
    else if(curMenuID === 'menu_lzryyjqrb'){
        console.log("menu_lzryyjqrb");
        return <LoadableLZRYYJPage/>;
    }
    else if(curMenuID === 'menu_sfkjqrb'){
        console.log("menu_sfkjqrb");
        return <LoadableSFKJQRJPage/>;
    }
    else if(curMenuID === 'menu_fyxcb'){
        return <LoadableFYXQBPage/>;
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