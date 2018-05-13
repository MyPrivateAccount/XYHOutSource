import { BaseApiUrl, basicDataBaseApiUrl, UploadUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list"//区域数据
    },
    msg: {
        sendMsg: basicDataBaseApiUrl + 'buildingnotice',//发送房源消息
    },
    attach: {
        uploadUrl: `${UploadUrl}/file/upload/`
    },
    userTypeValue: {
        Base: basicDataBaseApiUrl + 'userTypeValue', // put 更新当前用户历史楼盘
    },
    server: {
        uploadImg: basicDataBaseApiUrl + "chargeinfo/uploadmore/",
        addCharge: basicDataBaseApiUrl + "chargeinfo/addcharge/",
    },
}
export default WebApiConfig;