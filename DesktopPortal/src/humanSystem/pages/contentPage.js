import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableStaffPage = Loadable({
    loader: () => import('./staffinfo/staffinfo'),
    loading: () => <LoadableLoading />,
});
const LoadableOnboardPage = Loadable({
    loader: () => import('./staffinfo/onboarding'),
    loading: () => <LoadableLoading />,
});
const LoadableMonthPage = Loadable({
    loader: () => import('./month/month'),
    loading: () => <LoadableLoading />,
});
const LoadableAttendancePage = Loadable({
    loader: () => import('./attendance/attendance'),
    loading: () => <LoadableLoading />,
});
const LoadableStatisticsPage = Loadable({
    loader: () => import('./statistics/statistics'),
    loading: () => <LoadableLoading />,
});
const LoadableBlackPage = Loadable({
    loader: () => import('./black/black'),
    loading: () => <LoadableLoading />,
});
const LoadableStationPage = Loadable({
    loader: () => import('./station/station'),
    loading: () => <LoadableLoading />,
});
const LoadableAchievementPage = Loadable({
    loader: () => import('./achievement/achievement'),
    loading: () => <LoadableLoading />,
});
const LoadableOrganizationPage = Loadable({
    loader: () => import('./organization/organization'),
    loading: () => <LoadableLoading />,
});
const LoadableSetPage = Loadable({
    loader: () => import('./set/set'),
    loading: () => <LoadableLoading />,
});

function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID === "menu_user_mgr") {
        return <LoadableStaffPage />;
    }
    else if (curMenuID === "menu_month") {
        return <LoadableMonthPage />;
    }
    else if (curMenuID === "menu_attendance") {
        return <LoadableAttendancePage />;
    }
    else if (curMenuID === "menu_statistics") {
        return <LoadableStatisticsPage />;
    }
    else if (curMenuID === "menu_black") {
        return <LoadableBlackPage />;
    }
    else if (curMenuID === "menu_station") {
        return <LoadableStationPage />;
    }
    else if (curMenuID === "menu_achievement") {
        return <LoadableAchievementPage />;
    }
    else if (curMenuID === "menu_organization") {
        return <LoadableOrganizationPage />;
    }
    else if (curMenuID === "menu_set") {
        return <LoadableSetPage />;
    }
    else if (curMenuID === "Onboarding") {
        return <LoadableOnboardPage />;
    }
    else {
        return <LoadableStaffPage />;
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