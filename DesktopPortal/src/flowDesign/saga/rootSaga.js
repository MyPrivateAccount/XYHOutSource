import watchAllAsync from './flowChartSaga';


export default function* rootSaga() {
    yield [
        watchAllAsync()
    ]
}