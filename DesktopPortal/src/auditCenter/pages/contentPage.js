import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableMyAuditPage = Loadable({//我审批的
    loader: () => import('./myAuditListPage'),
    loading: () => <LoadableLoading />,
});
const LoadableMySubmitPage = Loadable({//我审批的
    loader: () => import('./mySubmitListPage'),
    loading: () => <LoadableLoading />,
});
const LoadableCopyToMePage = Loadable({//抄送我的
    loader: () => import('./copyToMeListPage'),
    loading: () => <LoadableLoading />,
});


function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID === "menu_myAudit") {
        return <LoadableMyAuditPage />;
    }
    else if (curMenuID === "menu_mySubmit") {
        return <LoadableMySubmitPage />
    }
    else if (curMenuID === "menu_copyToMe") {
        return <LoadableCopyToMePage />
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