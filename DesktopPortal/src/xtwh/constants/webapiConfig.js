import {basicDataBaseApiUrl} from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        Base: basicDataBaseApiUrl + 'dic',//获取数据字典
        GroupList: basicDataBaseApiUrl + 'dictionarygroups/list',//获取字典组 post
        GroupListSave: basicDataBaseApiUrl + 'dictionarygroups',//添加、修改字典组 post
        GroupListDelete: basicDataBaseApiUrl + 'dictionarygroups/',// 删除字典组
        ParList: basicDataBaseApiUrl + 'dictionarydefines/',// 获取字典类型列表 get
        ParListSave: basicDataBaseApiUrl + 'dictionarydefines',//添加、修改字典类型列表
        ParListDelete: basicDataBaseApiUrl + 'dictionarydefines/delete',// 删除字典项批量删除
        StartDicValue: basicDataBaseApiUrl + 'dictionarydefines/reuse',// 启用字典项定义put
    },
    area: {
        Base: basicDataBaseApiUrl + 'areadefines',//获取区域
        List: basicDataBaseApiUrl + 'areadefines/list',//1区域列表
        ChildList: basicDataBaseApiUrl + 'areadefines/',// 2,3级区域列表
        Delete: basicDataBaseApiUrl + 'areadefines/delete',//批量删除
    },
    tradePlanning: {
        list: basicDataBaseApiUrl + '',//
    }
}

export default WebApiConfig;