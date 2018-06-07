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
const LoadableBecomeStaffdPage = Loadable({
    loader: () => import('./staffinfo/becomeStaff'),
    loading: () => <LoadableLoading />,
});
const LoadableLeftdPage = Loadable({
    loader: () => import('./staffinfo/leave'),
    loading: () => <LoadableLoading />,
});
const LoadableChangedPage = Loadable({
    loader: () => import('./staffinfo/change'),
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
const LoadableAddBlackPage = Loadable({
    loader: () => import('./black/addblack'),
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
const LoadableAddAchievementPage = Loadable({
    loader: () => import('./achievement/addachievement'),
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
    else if (curMenuID === "menu_blackaddnew") {
        return <LoadableAddBlackPage ismodify="0" />;
    }
    else if (curMenuID === "menu_blackmodify") {
        return <LoadableAddBlackPage ismodify="1" />;
    }
    else if (curMenuID === "menu_station") {
        return <LoadableStationPage />;
    }
    else if (curMenuID === "menu_achievement") {
        return <LoadableAchievementPage />;
    }
    else if(curMenuID === "menu_achievementnew") {
        return <LoadableAddAchievementPage ismodify="0" />;
    }
    else if(curMenuID === "menu_achievementmodify") {
        return <LoadableAddAchievementPage ismodify="1" />;
    }
    else if (curMenuID === "menu_organization") {
        return <LoadableOrganizationPage />;
    }
    else if (curMenuID === "menu_set") {
        return <LoadableSetPage />;
    }
    else if (curMenuID === "Onboarding") {
        return <LoadableOnboardPage ismodify="0" />;
    }
    else if (curMenuID === "OnboardingShow") {
        return <LoadableOnboardPage ismodify="1" />;
    }
    else if (curMenuID === "BecomeStaff") {
        return <LoadableBecomeStaffdPage />;
    }
    else if (curMenuID === "changestation") {
        return <LoadableChangedPage />;
    }
    else if (curMenuID === "leftstation") {
        return <LoadableLeftdPage />;
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