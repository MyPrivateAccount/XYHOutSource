import { takeEvery, takeLatest } from 'redux-saga'
import { sagaMiddleware } from '../../'
import dic from './dic'
import area from './area'


export default function(){
    sagaMiddleware.run(dic);
    sagaMiddleware.run(area);
}