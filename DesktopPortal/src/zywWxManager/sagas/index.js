import {takeEvery, takeLatest} from 'redux-saga'
import {sagaMiddleware} from '../../'
import footPrint from './footPrint'
import hotShop from './hotShop'
import dayReport from './dayReport'


export default function () {
    sagaMiddleware.run(footPrint);
    sagaMiddleware.run(hotShop);
    sagaMiddleware.run(dayReport);
}