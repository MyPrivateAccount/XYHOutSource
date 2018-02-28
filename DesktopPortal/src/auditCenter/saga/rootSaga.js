import watchAllAsync from './auditSaga';
import watchHouseActiveAsync from './houseSaga';

export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchHouseActiveAsync()
    ]
}