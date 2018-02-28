import React from 'react';
import { connect } from 'react-redux';
import LoginPage from '../components/loginPage';
import Loadable from 'react-loadable';
import LoadableLoading from '../components/LoadableLoading';

const LoadableMainPage = Loadable({
  loader: () => import('./mainPage'),
  loading: () => <LoadableLoading />,
});

function HomePage(props) {
  const { user } = props;
  // if(module.hot){
  //   return <LoadableMainPage />
  // }
  return !user || user.expired ? <LoginPage/> : <LoadableMainPage />;
}

function mapStateToProps(state) {
  return {
    user: state.oidc.user
  };
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(HomePage);