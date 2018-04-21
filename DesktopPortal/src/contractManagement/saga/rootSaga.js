import watchAllAsync from './searchSaga';
import watchDicAllAsync from './dicSaga';
import { watchContractAllAsync } from './contractSaga';
import {watchCompanyAsync} from './companyASaga'
export default function* rootSaga() {
    yield [
        watchAllAsync(),
        watchDicAllAsync(),
        watchContractAllAsync(),
        watchCompanyAsync(),
    ]
}