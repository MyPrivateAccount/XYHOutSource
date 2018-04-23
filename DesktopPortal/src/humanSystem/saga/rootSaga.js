import watchAllAsync from './searchSaga';
import watchDicAllAsync from './dicSaga';
import watchAllSearchAsync from './searchSaga';

export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchDicAllAsync(),
        watchAllSearchAsync()
    ]
}