import { BaseApiUrl, basicDataBaseApiUrl,UploadUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list",//区域数据
        // OrgList: BaseApiUrl + "Organization/allsons/",//获取子部门  ===>>>> 2018.1.26更改接口
        OrgList: BaseApiUrl + "Organization/sonsandmy/",//获取子部门
        OrgDetail: BaseApiUrl + "Organization/",//获取部门详细
        GetOrgUserList: BaseApiUrl + "user/list",//获取部门下的用户
        permissionOrg: BaseApiUrl + 'Permission/'//有权限的部门,
    },
    search: {
        getContractList: basicDataBaseApiUrl+ 'contractlist/searchcontract',
        



        getAuditList: basicDataBaseApiUrl + 'examines/submitlist',//提交的调客审核列表
        getAuditHistory: basicDataBaseApiUrl + "examines/",//获取审核历史详细
    },
    contract:{
        GetContractInfo: basicDataBaseApiUrl + "contractinfo/"
    },
    contractBasic: {
        Base: basicDataBaseApiUrl + "contractinfo/addsimplecontract",//
        Modify: basicDataBaseApiUrl + "contractinfo/modifysimplecontract",
        Submit: basicDataBaseApiUrl + "contractinfo/checksimplecontract",//
    },
    complement:{
        GetComplement: basicDataBaseApiUrl + "",
        saveComplement: basicDataBaseApiUrl + "contractinfo/autocomplement/",
        //modifyComplemet: basicDataBaseApiUrl + "contractinfo/modifycomplement/",
    },
    attach: {
        GetAttachInfo: basicDataBaseApiUrl + "contractfiles/GetFileListByContractId/",
        uploadUrl: `${UploadUrl}/file/upload/`,
        savePicUrl: basicDataBaseApiUrl + 'contractfiles/contract/uploadmore/',
        deletePicUrl: basicDataBaseApiUrl + "contractfiles/deletecontractfile/"
    },
    extraInfo:{
        search: basicDataBaseApiUrl + "extrainfo/searchcompanya",
        add: basicDataBaseApiUrl + "extrainfo/addcompanyainfo/",
        modify: basicDataBaseApiUrl + "extrainfo/modifycompanyainfo/",
        delete: basicDataBaseApiUrl + "extrainfo/deletecompanyainfo/",
        getall: basicDataBaseApiUrl + "extrainfo/getall",
    }
}
export default WebApiConfig;