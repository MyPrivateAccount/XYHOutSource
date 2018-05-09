
import watchDicAllAsync from './dicSaga';


// export default function* rootSaga(){
//     yield[
//         watchDicAllAsync(),
//     ]
// }

export default function* rootSaga() {
    yield [
   
        watchDicAllAsync(),
     
    ]
}