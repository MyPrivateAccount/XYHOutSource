import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

export const LoadableAcmentPage = Loadable({//业绩分摊项设置
    loader: () => import('./baseSet/acmentSet'),
    loading: () => <LoadableLoading />,
});
export const LoadablePeopleSetPage = Loadable({//人数分摊组织设置
    loader: () => import('./baseSet/peopleOrgSet'),
    loading: () => <LoadableLoading />,
});
export const LoadableInComeScaleSetPage = Loadable({//分成比例设置
    loader: () => import('./baseSet/incomeScaleSet'),
    loading: () => <LoadableLoading />,
});
export const LoadableOrgParamSetPage = Loadable({//组织参数设置
    loader: () => import('./baseSet/orgParamSet'),
    loading: () => <LoadableLoading />,
});
export const LoadableDealRpPage = Loadable({//我录入的成交报告
    loader:()=>import('./dealRp/myDealRp'),
    loading:()=><LoadableLoading/>
});
export const LoadableDealRpQueryPage = Loadable({//我录入的成交报告
    loader:()=>import('./dealRp/dealRpQuery'),
    loading:()=><LoadableLoading/>
});
export const LoadableYjfpQueryPage = Loadable({//业绩分摊查看
    loader:()=>import('./dealRp/yjfpQuery'),
    loading:()=><LoadableLoading/>
});
export const LoadableMonthPage = Loadable({//月结
    loader:()=>import('./fina/monthSumary'),
    loading:()=><LoadableLoading/>
});
export const LoadablePPFTPage = Loadable({//人员分摊
    loader:()=>import('./fina/ppFtTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableYFTCPage = Loadable({//应发提成
    loader:()=>import('./fina/yfTcTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableSFTCPage = Loadable({//实发提成
    loader:()=>import('./fina/sfTcTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableTCCBPage = Loadable({//提成成本
    loader:()=>import('./fina/tcCbTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableYFTCCJPage = Loadable({//应发提成冲减
    loader:()=>import('./fina/yftcCjTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableLZRYYJPage = Loadable({//离职人员业绩
    loader:()=>import('./fina/lzryYjTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableSFKJQRJPage = Loadable({//实发扣减确认
    loader:()=>import('./fina/sfkjQrTable'),
    loading:()=><LoadableLoading/>
});
export const LoadableFYXQBPage = Loadable({//分佣详情表
    loader:()=>import('./fullRp/fyxqQuery'),
    loading:()=><LoadableLoading/>
});
export const LoadableYJTZHZPage = Loadable({//调整佣
    loader:()=>import('./fullRp/yjtzhzQuery'),
    loading:()=><LoadableLoading/>
});
export const LoadableTYXQPage = Loadable({//调整佣
    loader:()=>import('./fullRp/tyxqQuery'),
    loading:()=><LoadableLoading/>
});

// function ContentPage(props) {
//     const { curMenuID } = props;
//     const {user} = props;

//     if (curMenuID === "menu_yjftxsz") {
//         console.log("menu_yjftxsz");
//         return <LoadableAcmentPage user={user}/>;
//     }
//     else if(curMenuID === "menu_rsftzzsz"){
//         console.log("menu_rsftzzsz");
//         return <LoadablePeopleSetPage user={user}/>;
//     }
//     else if(curMenuID === "menu_tcblsz"){
//         console.log("menu_tcblsz");
//         return <LoadableInComeScaleSetPage user={user}/>;
//     }
//     else if(curMenuID === "menu_zzcssz"){
//         console.log("menu_zzcssz");
//         return <LoadableOrgParamSetPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_myrp'){
//         console.log("menu_myrp");
//         return <LoadableDealRpPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_query'){
//         console.log("menu_query");
//         return <LoadableDealRpQueryPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_sumbymonth'){
//         console.log("menu_sumbymonth");
//         return <LoadableMonthPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_ps'){
//         console.log("menu_ps");
//         return <LoadablePPFTPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_yftcb'){
//         console.log("menu_yftcb");
//         return <LoadableYFTCPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_sftcb'){
//         console.log("menu_sftcb");
//         return <LoadableSFTCPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_tccbftb'){
//         console.log("menu_tccbftb");
//         return <LoadableTCCBPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_yftccjb'){
//         console.log("menu_yftccjb");
//         return <LoadableYFTCCJPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_lzryyjqrb'){
//         console.log("menu_lzryyjqrb");
//         return <LoadableLZRYYJPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_sfkjqrb'){
//         console.log("menu_sfkjqrb");
//         return <LoadableSFKJQRJPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_fyxcb'){
//         return <LoadableFYXQBPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_yjtzmxb'){
//         return <LoadableYJTZHZPage user={user}/>;
//     }
//     else if(curMenuID === 'menu_tymxb'){
//         return <LoadableTYXQPage user={user}/>;
//     }
//     return null;
// }


// function mapStateToProps(state) {
//     return {

//     };
// }

// function mapDispatchToProps(dispatch) {
//     return {
//         dispatch
//     };
// }

// export default connect(null, mapDispatchToProps)(ContentPage);