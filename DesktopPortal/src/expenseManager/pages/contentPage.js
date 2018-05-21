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

function ContentPage(props) {
    const { curMenuID } = props;
    console.log('curMenuID:', curMenuID);
    if (curMenuID === "home") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID === "checkininfo") {
        return <LoadableChargePage />;
    } else if (curMenuID === "addreciept") {
        return <LoadableRecieptPage />;
    } else if (curMenuID === "costcharge") {
        return <LoadableCostPage />;
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