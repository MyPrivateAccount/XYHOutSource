import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableIndexPage = Loadable({
    loader: () => import('./mainIndex'),
    loading: () => <LoadableLoading />,
});

const LoadAttatchMentPage = Loadable({
    loader: () => import('./attachMent'),
    loading: () => null//<LoadableLoading />
});

const LoadContractRecordPage = Loadable({
    loader: () => import('./contractRecord'),
    loading: () => <LoadableLoading />
});


const LoadableSelectOrgPage = Loadable({
    loader: () => import('./orgSelect/orgSelect'),
    loading: () => <LoadableLoading />,
});


function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID == "menu_index") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID == "menu_renew") {
        //return <LoadableAdjustPage />;
        return <LoadableIndexPage />;
    }
    else if (curMenuID === "menu_record") {
        return <LoadContractRecordPage />; 
    }
    else if (curMenuID === "menu_attachMent") {
        return <LoadAttatchMentPage />;
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