import watchAllOrgAsync from './orgSaga';
import watchAllPPftAsync from './ppFtSaga'
import watchAllOrgParamAsync from './orgParamSaga'
import watchAllScaleAsync from './scaleSaga'
import watchAllAcmentAsync from './acmSaga'
import watchDicAllAsync from './dicSaga'
import watchAllRpAsync  from './dealRp/rpSaga'
//所有的中间件监听
export default function* rootSaga() {
    yield [
        watchAllOrgAsync(),
        watchAllPPftAsync(),
        watchAllOrgParamAsync(),
        watchAllScaleAsync(),
        watchAllAcmentAsync(),
        watchDicAllAsync(),
        watchAllRpAsync()
    ]
}