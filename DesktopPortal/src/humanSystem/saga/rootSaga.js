import watchAllAsync from './searchSaga';
import watchDicAllAsync from './dicSaga';

export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchDicAllAsync()
    ]
}