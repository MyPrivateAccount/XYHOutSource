import watchDicAllAsync from './dicSaga';
import watchAllAsync from './searchSaga';

export default function* rootSaga() {
    yield [
        watchDicAllAsync(),
        watchAllAsync()
    ]
}