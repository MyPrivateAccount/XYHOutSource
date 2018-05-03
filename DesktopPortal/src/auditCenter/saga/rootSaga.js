import watchAllAsync from './auditSaga';
import watchHouseActiveAsync from './houseSaga';
import watchContractAsync from './contractSaga';
export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchHouseActiveAsync(),
        watchContractAsync(),
    ]
}