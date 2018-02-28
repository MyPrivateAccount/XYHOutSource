import React, { Component } from 'react';
import Loadable from 'react-loadable';
import { connect } from 'react-redux';
import LoadableLoading from '../../components/LoadableLoading';

const LoadableAppPage = Loadable({//应用管理
    loader: () => import('./appManager/appTable'),
    //loader: () => <LoadableLoading />,
    loading: () => <LoadableLoading />,
});
const LoadableOrgPage = Loadable({//组织管理
    loader: () => import('./organization/orgTable'),
    loading: () => <LoadableLoading />,
});
const LoadableRolePage = Loadable({//角色管理
    loader: () => import('./role/roleTable'),
    //loader: () => import('./appManager/appTable'),
    loading: () => <LoadableLoading />,
});
const LoadablePrivilegePage = Loadable({//权限管理
    loader: () => import('./privilege/privilegeTable'),
    loading: () => <LoadableLoading />,
});
const LoadableEmpPage = Loadable({//用户管理
    loader: () => import('./employee/empManager'),
    loading: () => <LoadableLoading />,
});


function ContentPage(props) {
    const { curMenuID } = props;
    if (curMenuID == "menu_app") {
        console.log("app");
        return <LoadableAppPage />;
    }
    else if (curMenuID == "menu_org") {
        console.log("menu_org");
        return <LoadableOrgPage />;
    }
    else if (curMenuID == "menu_role") {
        console.log("menu_role");
        return <LoadableRolePage />;
    }
    else if (curMenuID === "menu_emp") {
        return <LoadableEmpPage />;
    }
    if (curMenuID == "menu_access") {
        console.log("menu_access");
        return <LoadablePrivilegePage />;
    }
    else {
        console.log("menu_org");
        return <LoadableOrgPage />;
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