import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableIndexPage = Loadable({
    loader: () => import('./mainIndex'),
    loading: () => <LoadableLoading />,
});


function ContentPage(props) {
    const { curMenuID } = props;
    console.log('curMenuID:', curMenuID);
    if (curMenuID == "menu_index") {
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