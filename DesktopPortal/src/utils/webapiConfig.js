import {BaseApiUrl, basicDataBaseApiUrl} from '../constants/baseConfig';


const WebApiConfig = {
    application: {
        Base: BaseApiUrl + 'Application',//基础地址
        List: BaseApiUrl + 'Application/list',//获取应用列表
    },
    user: {
        Base: BaseApiUrl + 'User',//基础地址
        RestPwd: BaseApiUrl + 'user/resetpassword' //修改密码
    },
    message: {
        unReadCount: basicDataBaseApiUrl + 'messages/unreadcount',//未读消息总数
        receiveList: basicDataBaseApiUrl + 'messages/readlist',//
        getMsgDetail: basicDataBaseApiUrl + 'messages/',//消息详细
    },
    permission: {
        judgePermssion: BaseApiUrl + 'Permission/each',//权限判断
    }, dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list"//区域数据
    }
}

export default WebApiConfig;