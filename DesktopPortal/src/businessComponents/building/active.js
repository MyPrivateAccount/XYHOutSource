import {connect} from 'react-redux';
import {basicDataBaseApiUrl} from '../../constants/baseConfig';
import ApiClient from '../../utils/apiClient'
import React, {Component} from 'react';
import moment from 'moment';
import {Spin, Popconfirm, Layout, Pagination, Icon} from 'antd';
import './ative.less';
const {Header, Sider, Content} = Layout;
const style = {

}
class Active extends Component {
  state = {
    pageIndex: 0,
    showLoading: false,
    activeList: []//房源动态列表
  }

  componentWillMount() {
    this.loadActiveList();
    console.log(this.props.user, 9999)
  }
  //加载动态详情
  loadActiveList() {
    let buildingId = (this.props.buildingInfo || {}).id;
    let url = basicDataBaseApiUrl + 'updaterecord/list';
    let body = {
      updateTypes: [1, 2],
      examineStatus: [8],
      pageIndex: this.state.pageIndex,
      pageSize: 10,
      contentIds: [buildingId],
      cityCode: this.props.user.City,
      isCurrent: true
    }
    this.setState({showLoading: true});
    ApiClient.post(url, body).then(res => {
      console.log(url,body,'获取房源动态列表res', res);
      if (res && res.data.code == '0') {
        let activeList = res.data.extension || [];
        activeList.totalCount = res.data.totalCount;
        this.setState({activeList: activeList});
      }
      this.setState({showLoading: false});
    }).catch((e) => {
      console.log('获取房源动态列表失败');
      this.setState({showLoading: false});
    })
  }

  getTime = (val) => {
    let a = moment(`${moment().format('YYYY-MM-DD')} 23:59:59`).diff(moment(val), 'days')
    switch (a) {
      case 0: return moment(val).format('HH:mm')
      case 1: return '一天前'
      default: return moment(val).format('YYYY-MM-DD')
    }
  }

  // 翻页
  handlePageChange = (pageIndex, pageSize) => {  // 翻页
    this.setState({pageIndex: (pageIndex - 1)}, () => this.loadActiveList());
  }

  render() {
    const dynamicData = this.state.activeList;
    return (
      <Layout>
        <Content className='content' >
          <Spin spinning={this.state.showLoading}>
            {
              dynamicData.length !== 0 ?
                <div className='activePageList'>
                  {
                    dynamicData.map((data, index) => {
                      return (
                        <div className='activePageList_item' key={index}>
                          <p className='activePageList_item_title'>{data.title}</p>
                          <div className='activePageList_item_content'>
                            <p style={{color: '#999', fontSize: '1rem'}}>
                              {data.buildingName}
                              <span>{data.areaFullName}</span>
                            </p>
                            <p style={{color: '#999', fontSize: '1rem'}}>
                              {data.userName}
                              <span>{this.getTime(data.createTime || data.submitTime)}</span>
                            </p>
                          </div>
                        </div>
                      )
                    })
                  }
                  {
                    dynamicData.length === 0 ? null :
                      <Pagination showQuickJumper style={{textAlign: 'right', margin: '15px 0'}}
                        current={this.state.pageIndex + 1}
                        total={dynamicData.totalCount}
                        onChange={this.handlePageChange} />
                  }
                </div> :
                <div style={{textAlign: 'center', fontSize: '1rem', padding: '10px 0', borderBottom: '1px solid #e0e0e0'}}>
                  <Icon type="frown-o" />
                  <span style={{marginLeft: '15px'}}>暂无数据</span>
                </div>
            }
          </Spin>
        </Content>
      </Layout>
    )
  }

}

function mapStateToProps(state) {
  return {
    dynamicData: state.search.dynamicData,
    user: (state.oidc.user || {}).profile || {},
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(Active);