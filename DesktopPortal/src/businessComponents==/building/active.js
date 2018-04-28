import { connect } from 'react-redux';
// import { getActiveList } from '../../../actions/actionCreator';
import React, { Component } from 'react';
import moment from 'moment';
import { Button, Row, Col, Tag, Badge, Popconfirm,Layout,Pagination,Icon } from 'antd';
import './ative.less';
const {Header, Sider, Content} = Layout;
const style = {
  
}
class Active extends Component {
    state = {
      pageIndex: 0,
    }

    componentWillMount() {
      // this.props.dispatch(getActiveList({ 
      //   pageIndex: this.state.pageIndex, 
      //   buildingId: this.props.id  
      // }))
    }

    getTime = (val) => {
      let a = moment(`${moment().format('YYYY-MM-DD')} 23:59:59`).diff(moment(val), 'days')
      switch(a) {
        case 0 : return moment(val).format('HH:mm')
        case 1 : return '一天前'
        default : return moment(val).format('YYYY-MM-DD')
      }
    }

    // 翻页
    handlePageChange = (pageIndex, pageSize) => {  // 翻页
      // this.setState({ pageIndex: (pageIndex - 1) }, () => {
      //     this.props.dispatch(getActiveList({
      //         buildingId: this.props.id,
      //         pageIndex: this.state.pageIndex,
      //     }))
      // })
    }

    render() {
      const dynamicData = this.props.dynamicData || []
        return (
          <Layout>
          <Content className='content' >
          {
            dynamicData.length !== 0 ? 
          <div className='activePageList'>
          {
            dynamicData.map((data, index) => {
              return (
                <div className='activePageList_item' key={index}>
                  <p className='activePageList_item_title'>{data.title}</p>
                  <div className='activePageList_item_content'>
                    <p style={{color:'#999', fontSize: '1rem'}}>
                      {data.buildingName}
                      <span>{data.areaFullName}</span>
                    </p>
                    <p style={{color:'#999', fontSize: '1rem'}}>
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
              onChange={this.handlePageChange}/>
          }
          </div> :  
          <div style={{textAlign:'center',fontSize: '1rem',padding: '10px 0',borderBottom:'1px solid #e0e0e0'}}>
            <Icon type="frown-o" /> 
            <span style={{marginLeft: '15px'}}>暂无数据</span>
          </div> 
          }
          </Content>
          </Layout>
        )
    }

}

function mapStateToProps(state) {
    return {
      dynamicData: state.search.dynamicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(Active);