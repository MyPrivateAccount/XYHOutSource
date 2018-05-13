
import watchGetAlldic from './dicSaga';
import watchServerInterface from './serverSaga'

// export default function* rootSaga(){
//     yield[
//         watchDicAllAsync(),
//     ]
// }

export default function* rootSaga() {
    yield [
        watchGetAlldic(),
        watchServerInterface(),
    ]
}