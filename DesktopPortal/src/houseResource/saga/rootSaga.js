import { watchGetParList, watchGetAllArea } from './dicSaga';
import { watchBuildingAllAsync } from './buildingSaga';
import { watchGetShop } from './shopSaga';
import { watchIndexAllAsync } from './indexSaga';
import { watchCenterAllAsync } from './xkCenterSagas';
import { watchManagerAllAsync } from './zcManageSaga';
import { watchActiveAllAsync } from './houseActiveSaga';
import { watchMsgAllAsync } from './msgSaga';

export default function* rootSaga() {
    yield [
        watchGetParList(),
        watchGetAllArea(),
        watchBuildingAllAsync(),
        watchGetShop(),
        watchIndexAllAsync(),
        watchCenterAllAsync(),
        watchManagerAllAsync(),
        watchActiveAllAsync(),
        watchMsgAllAsync(),
    ]
}