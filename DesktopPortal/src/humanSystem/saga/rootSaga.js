import watchAllAsync from './searchSaga';
import watchDicAllAsync from './dicSaga';
import watchServerAsync from './serverSaga';

export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchDicAllAsync(),
        watchServerAsync()
    ]
}