import {watchGetAppList} from './appSaga';
import {watchResetPassword} from './resetPwdSaga';
import {watchMsgAsync} from './sagas';
import {watchDicAllAsync} from './dicSaga';
export default function* rootSaga() {
    yield [
        watchGetAppList(),
        watchResetPassword(),
        watchMsgAsync(),
        watchDicAllAsync()
    ]
}