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

const LoadContractDetailPage = Loadable({
    loader: () => import('./contractDetail'),
    loading: () => null//<LoadableLoading />
});

const LoadComplementPage = Loadable({
    loader: () => import('./complement'),
    loading: () => null//<LoadableLoading />
});
const LoadCompanyAPage = Loadable({
    loader: () => import('./companyA'),
    loading: () => null//<LoadableLoading />
});
function ContentPage(props) {
    const { curMenuID } = props;
    console.log('curMenuID:', curMenuID);
    if (curMenuID == "menu_index") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID == "menu_partA") {
        //return <LoadableAdjustPage />;
        return <LoadCompanyAPage />;
    }
    else if (curMenuID === "menu_record") {
        return <LoadContractRecordPage />; 
    }
    else if (curMenuID === "menu_attachMent") {
        return <LoadAttatchMentPage />;
    }
    else if (curMenuID === "menu_contractDetail") {
        return <LoadContractDetailPage />; 
    }
    else if (curMenuID === "menu_complement") {
        return <LoadComplementPage />;
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