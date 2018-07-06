import {BaseApiUrl, basicDataBaseApiUrl, UploadUrl} from '../../constants/baseConfig';

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
    server: {
        PostHumanPicture: basicDataBaseApiUrl + "humanfile/humaninfo/uploadmore/",
        // PostHumaninfo: basicDataBaseApiUrl + "humaninfo/addhuman",
        PostHumaninfo: basicDataBaseApiUrl + 'humaninfo',
        GetWorkNumber: basicDataBaseApiUrl + "humaninfo/jobnumber",
        LastMonth: basicDataBaseApiUrl + "month/lastmonth",
        RecoverMonth: basicDataBaseApiUrl + "month/backmonth",
        CreateMonth: basicDataBaseApiUrl + "month/createmonth",
        monthFormData: basicDataBaseApiUrl + "month/monthformdata",
        SetBlack: basicDataBaseApiUrl + "humaninfoblack",
        DeleteBlack: basicDataBaseApiUrl + "humaninfoblack/",
        SetStation: basicDataBaseApiUrl + "humanstation/setstation",
        DeleteStation: basicDataBaseApiUrl + "humanstation/deletestation",
        setSalary: basicDataBaseApiUrl + "positionsalary",
        editSalary: basicDataBaseApiUrl + 'positionsalary/',
        deleteSalary: basicDataBaseApiUrl + "humansalary/deletestation",
        getHumanImage: basicDataBaseApiUrl + "humanfile/getfileinfo",
        setSocialInsure: basicDataBaseApiUrl + "humaninfo/becomehuman",
        leavePositon: basicDataBaseApiUrl + "humanleave",
        postAttendenceSettingList: basicDataBaseApiUrl + 'humanattendance/setattendancesetting',
        importAttendenceList: basicDataBaseApiUrl + 'humanattendance/importattendancelst',
        deleteAttendenceList: basicDataBaseApiUrl + 'humanattendance/deleteattendanceitem',
        addRPInfo: basicDataBaseApiUrl + 'rewardpunishment/addrewardpunishment',
        deleteRPInfo: basicDataBaseApiUrl + 'rewardpunishment/deleterewardpunishment',
        savePartTimeJob: basicDataBaseApiUrl + '',//兼职保存
        adjustHuman: basicDataBaseApiUrl + 'humanadjustment',//异动调薪
        getPartTimeJobList: basicDataBaseApiUrl + 'humanpartposition/',//获取兼职列表
        removePartTimeJob: basicDataBaseApiUrl + 'humanpartposition/',//删除兼职信息
    },
    search: {
        searchHumanList: basicDataBaseApiUrl + 'humaninfo/search',
        getAuditList: basicDataBaseApiUrl + 'examines/submitlist',//提交的调客审核列表
        getRepeatJudgeInfo: basicDataBaseApiUrl + 'customerInfo/customerheavy/',//获取重客判断信息
        getCustomerByUserID: basicDataBaseApiUrl + 'customerInfo/salesmancustomer/',//根据用户ID获取客户列表
        getAuditHistory: basicDataBaseApiUrl + "examines/",//获取审核历史详细
        getAllMonthList: basicDataBaseApiUrl + 'month/monthlist',
        getBlackList: basicDataBaseApiUrl + 'humaninfoblack/list',
        getStationList: basicDataBaseApiUrl + 'humanstation/stationlist',
        getSalaryList: basicDataBaseApiUrl + 'positionsalary/list/',//根据职位获取薪酬信息
        getSalaryItem: basicDataBaseApiUrl + 'humansalary/salaryitem',
        getAttendenceSettingList: basicDataBaseApiUrl + 'humanattendance/attendancesetting',
        getAttendenceList: basicDataBaseApiUrl + 'humanattendance/searchattendancelst',
        getRPInfoList: basicDataBaseApiUrl + 'rewardpunishment/searchrewardpunishment',
        getHumanlistByorg: basicDataBaseApiUrl + 'humaninfo/hulistbyorg',
        getHumanDetail: basicDataBaseApiUrl + 'humaninfo/'
    },
    attach: {
        uploadUrl: `${UploadUrl}/file/upload/`,
    },
    auth: {
        deleteOrg: BaseApiUrl + 'Organization/',
        addupdateOrg: BaseApiUrl + 'Organization/',
    },
}
export default WebApiConfig;