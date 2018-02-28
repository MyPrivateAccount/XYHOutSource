import { BaseApiUrl, basicDataBaseApiUrl, FlowChartApiUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list"//区域数据
    },
    flowChart: {
        Invoke: FlowChartApiUrl + "cmd/invoke?appToken=app:nwf",//流程列表 
    }
}
export default WebApiConfig;