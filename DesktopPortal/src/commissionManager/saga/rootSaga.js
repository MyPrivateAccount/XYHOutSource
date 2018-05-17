import watchAllOrgAsync from './orgSaga';
import watchAllPPftAsync from './ppFtSaga'
import watchAllOrgParamAsync from './orgParamSaga'
import watchAllScaleAsync from './scaleSaga'
import watchAllAcmentAsync from './acmSaga'
//所有的中间件监听
export default function* rootSaga() {
    yield [
        watchAllOrgAsync(),
        watchAllPPftAsync(),
        watchAllOrgParamAsync(),
        watchAllScaleAsync(),
        watchAllAcmentAsync()
    ]
}