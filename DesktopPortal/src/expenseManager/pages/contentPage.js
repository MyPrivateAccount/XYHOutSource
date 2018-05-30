import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableIndexPage = Loadable({
    loader: () => import('./charge/mainIndex'),
    loading: () => <LoadableLoading />,
});
const LoadableChargePage = Loadable({
    loader: () => import('./charge/charge'),
    loading: () => <LoadableLoading />,
});
const LoadableRecieptPage = Loadable({
    loader: () => import('./charge/addreciept'),
    loading: () => <LoadableLoading />,
});
const LoadableCostPage = Loadable({
    loader: () => import('./charge/payment'),
    loading: () => <LoadableLoading />,
});
const LoadableLimitPage = Loadable({
    loader: () => import('./chargelimit/chargelimit'),
    loading: () => <LoadableLoading />,
});
const LoadableChargeDetailPage = Loadable({
    loader: () => import('./charge/chargedetail'),
    loading: () => <LoadableLoading />,
});

function ContentPage(props) {
    const { curMenuID, extra } = props;
    //console.log('curMenuID:', curMenuID);
    if (curMenuID === "home") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID === "checkininfo") {
        return <LoadableChargePage />;
    } else if (curMenuID === "addreciept") {
        return <LoadableRecieptPage />;
    } else if (curMenuID === "costcharge") {
        return <LoadableCostPage />;
    } else if (curMenuID === "limitindex") {
        return <LoadableLimitPage />;
    } else if (curMenuID === "chargedetailinfo") {
        return <LoadableChargePage isdetail={true} chargeid={extra} />;
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