import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableIndexPage = Loadable({
    loader: () => import('./searchIndex'),
    loading: () => <LoadableLoading />,
});
const LoadableAdjustPage = Loadable({
    loader: () => import('./adjustCustomerAudit'),
    loading: () => <LoadableLoading />,
});
const LoadableAnalysisPage = Loadable({
    loader: () => import('./analysis'),
    loading: () => <LoadableLoading />,
});
const LoadableSelectOrgPage = Loadable({
    loader: () => import('./orgSelect/orgSelect'),
    loading: () => <LoadableLoading />,
});
const LoadableRepeatCustomerPage = Loadable({
    loader: () => import('./repeatCustomer'),
    loading: () => <LoadableLoading />,
});

function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID == "menu_index") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID == "menu_audit") {
        return <LoadableAdjustPage />;
    }
    else if (curMenuID === "menu_org_select") {
        return <LoadableSelectOrgPage />;
    }
    else if (curMenuID === "menu_analysis") {
        return <LoadableAnalysisPage />;
    }
    else if (curMenuID === "menu_repeatCustomer") {
        return <LoadableRepeatCustomerPage />;
    }
    else {
        return <LoadableIndexPage />;
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