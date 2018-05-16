import watchAllOrgAsync from './orgSaga';
import watchAllPPftAsync from './ppFtSaga'
//所有的中间件监听
export default function* rootSaga() {
    yield [
        watchAllOrgAsync(),
        watchAllPPftAsync()
    ]
}