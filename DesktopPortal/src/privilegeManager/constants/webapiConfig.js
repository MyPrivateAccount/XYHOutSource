import { BaseApiUrl, basicDataBaseApiUrl } from '../../constants/baseConfig';


const WebApiConfig = {
    org: {
        Get: BaseApiUrl + "Organization/sons/",//部门数据获取
        Base: BaseApiUrl + 'Organization',//部门数据新增(post),修改put
        permissionOrg: BaseApiUrl + 'Permission/'//有权限的部门
    },
    user: {
        Base: BaseApiUrl + 'user',//用户基础地址
        List: BaseApiUrl + "user/list",//获取部门用户列表
        Priv: basicDataBaseApiUrl + 'dictionarydefines/position',//用户职级列表
        Delete: BaseApiUrl + 'user/delete',
        UserRoleAdd: BaseApiUrl + 'UserRoles/AddToRoles/',//用户所属角色新增
        UserRoleRemove: BaseApiUrl + 'UserRoles/RemoveFromRoles/',//用户所属角色删除
        ResetPwd: BaseApiUrl + "user/initpassword",//重置密码
    },
    application: {
        Base: BaseApiUrl + 'Application',//应用基础地址
        List: BaseApiUrl + 'Application/list',//获取应用列表
    },
    privilege: {
        Base: BaseApiUrl + 'PermissionItems',
        List: BaseApiUrl + 'PermissionItems/list/',//获取权限列表
        Delete: BaseApiUrl + 'PermissionItems/delete',
    },
    rolePrivilege: {
        Base: BaseApiUrl + 'RolePermissions',//角色权限
        Application: BaseApiUrl + 'RoleApplications/'//角色应用
    },
    role: {
        Base: BaseApiUrl + 'Roleinfo',
        List: BaseApiUrl + 'Roleinfo/list',
    },
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list"//区域数据
    },

}

export default WebApiConfig;