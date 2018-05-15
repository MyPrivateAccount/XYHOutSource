import watchAllOrgAsync from './orgSaga';
//所有的中间件监听
export default function* rootSaga() {
    yield [
        watchAllOrgAsync()
    ]
}