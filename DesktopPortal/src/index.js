import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import homePage from './container/homePage';
import {AppContainer} from 'react-hot-loader'
import {Provider} from 'react-redux';
import {ConnectedRouter} from 'react-router-redux'
import createHistory from 'history/createBrowserHistory'
import {applyMiddleware, compose} from 'redux'
import {createStore} from 'redux-dynamic-reducer'
import {Route, Switch} from 'react-router'
import CallbackPage from './components/CallbackPage'
//import { OidcProvider } from 'redux-oidc';
import OidcProvider from './components/OidcProvider'
import userManager from './utils/userManager';
import rootReducer from './reducers';
import {routerMiddleware} from 'react-router-redux'
import createSagaMiddleware from 'redux-saga'
import * as uiSize from './constants/uiSize';
// import {helloSaga, watchGetAppListAsync} from './saga/sagas';
import rootSaga from './saga/rootSaga';
import ignoreSubspaceMiddleware from './ignoreSubspaceMiddleware'
import './antd.less';
import './iconfont/iconfont.css'
import Download from './download'

const nextReducer = require('./reducers');
const history = createHistory();

export const sagaMiddleware = createSagaMiddleware();

const globalActions = [
    /.*(@@redux-form.*)/,
    /.*(@@router.*)/
];
export default function configure(initialState) {
    const im = ignoreSubspaceMiddleware(globalActions);
    const rm = routerMiddleware(history);
    const middleware = applyMiddleware(im, rm, sagaMiddleware);

    const store = createStore(rootReducer,
        compose(middleware, window.devToolsExtension ? window.devToolsExtension() : f => f));
    if (module.hot) {
        module.hot.accept('./reducers', () => {
            store.replaceReducer(nextReducer);
        });
    }

    return store;
}



export const store = configure({});
sagaMiddleware.run(rootSaga);


function getRoutePath(p) {
    let root = document.location.pathname;
    if (/\/callback$/i.test(root)) {
        root = root.substring(0, root.length - 9);
    }

    let url = root + '/' + p;
    console.log(url);
    return url;
}
// <MuiThemeProvider theme={muiTheme}>
const render = Component => {
    ReactDOM.render(
        <Provider store={store}>
            <OidcProvider store={store} userManager={userManager}>
                <AppContainer>
                    <ConnectedRouter history={history}>

                        <div>

                            <Route exact path='/' component={Component} />
                            <Route path='/callback' component={CallbackPage} />
                            <Route path='/download' component={Download} />
                        </div>

                    </ConnectedRouter>
                </AppContainer>
            </OidcProvider></Provider>, document.getElementById('root')
    )
}

render(homePage);


if (module.hot) {
    module.hot.accept('./container/homePage', () => {render(homePage)})
}
