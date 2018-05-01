import {basicDataBaseApiUrl} from '../../constants/baseConfig';

const WebApiConfig = {
    footPrint: {
        getList: basicDataBaseApiUrl + 'yhp/customerfootprint/list',
        getDetail: basicDataBaseApiUrl + 'businessplanning/',
        process: basicDataBaseApiUrl + 'businessplanning'
    },
    hotShops: {
        searchBuilding: basicDataBaseApiUrl + 'buildings/list',
    }
}

export default WebApiConfig;