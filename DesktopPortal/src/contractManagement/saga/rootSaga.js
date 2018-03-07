import watchAllAsync from './searchSaga';
import watchDicAllAsync from './dicSaga';
import { watchContractAllAsync } from './contractSaga';

export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchDicAllAsync(),
        watchContractAllAsync(),
    ]
}