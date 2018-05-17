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

function ContentPage(props) {
    const { curMenuID } = props;
    console.log('curMenuID:', curMenuID);
    if (curMenuID === "home") {
        return <LoadableIndexPage />;
    }
    else if (curMenuID === "checkininfo") {
        return <LoadableChargePage />;
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