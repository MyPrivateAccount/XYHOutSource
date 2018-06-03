//数据接口地址配置

import { BaseApiUrl, basicDataBaseApiUrl,UploadUrl } from '../../constants/baseConfig';


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
    rp:{
        rpAdd:basicDataBaseApiUrl + 'yj/report',//保存成交报告交易合同
        rpWyAdd:basicDataBaseApiUrl+'yj/reportwy',//保存物业信息
        rpYzAdd:basicDataBaseApiUrl+'yj/reportyz',//保存业主信息
        rpKhAdd:basicDataBaseApiUrl+'yj/reportkh',//保存客户信息
        rpGhAdd:basicDataBaseApiUrl+'yj/reportgh',//保存过户信息
        rpFpAdd:basicDataBaseApiUrl+'yj/reportyjfp',//保存业绩分配信息
        //获取接口
        rpGet:basicDataBaseApiUrl+'yj/report/',
        wyGet:basicDataBaseApiUrl+'yj/reportwy/',
        yzGet:basicDataBaseApiUrl+'yj/reportyz/',
        khGet:basicDataBaseApiUrl+'yj/reportkh/',
        ghGet:basicDataBaseApiUrl+'yj/reportgh/',
        fpGet:basicDataBaseApiUrl+'yj/reportyjfp/',
        myrpGet:basicDataBaseApiUrl+'yj/report/myreport',
        searchRp:basicDataBaseApiUrl+'yj/report/search'
    },
    fina:{
        searchPPFt:basicDataBaseApiUrl+'yj/ppft/search'
    },
    attach: {
        uploadUrl: `${UploadUrl}/file/upload/`
    },
    server: {
        uploadImg: basicDataBaseApiUrl + "yj/commissionfiles/",
    },

}

export default WebApiConfig;