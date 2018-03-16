import { connect } from 'react-redux';
import { closeModal, openModal,savePerson, getBuildingsite } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Button, Row, Col,Spin,Modal } from 'antd'
import moment from 'moment'
import './zcManagment.less'
import Info from './info'

const { Header, Sider, Content } = Layout;

class ZcManagement extends Component {
  state = {
    thisData: {},
    pageIndex: 0,
  }
  

  componentWillMount() {
    this.props.dispatch(getBuildingsite())
  }
  componentDidMount() {
     
  }
  componentWillReceiveProps(newProps) {
    
  }
  handleEdit = (item) => {
    this.props.openModal(item)
    
  }
  handleOk = (e) => {
    const { currentInfo, checkedArr }  = this.props
    let obj = {
      residentUser1: (checkedArr[0] || {}).id || null,
      residentUser2: (checkedArr[1] || {}).id || null,
      residentUser3: (checkedArr[2] || {}).id || null,
      residentUser4: (checkedArr[3] || {}).id || null,
      residentUserName1: (checkedArr[0] || {}).name || null,
      residentUserName2: (checkedArr[1] || {}).name || null,
      residentUserName3: (checkedArr[2] || {}).name || null,
      residentUserName4: (checkedArr[3] || {}).name || null,
      id: currentInfo.id,
      name: (currentInfo.basicInfo || {}).name || '',
    }
    console.log(obj, '指派驻场obj')
    this.props.savePerson(obj)
  }
  handleCancel = () => {
    this.props.closeModal();
  }

  render() {
    const { show, operating } = this.props.editManagerInfo
    // console.log(this.props.dataSource, '哈哈哈列表出来了')
   
    return ( 
      <Spin spinning={this.props.isLoading}>
      <div className="relative">
          <Layout>
              <Content className='content zcManagementPage'>
                {/* <Info /> */}
                {
                  (this.props.dataSource || []).map((item, index) => {
                    return (
                      <Row  className='itemStyle' key={index}>
                        <Col span={24}>
                            <Row className='borderDiv'>
                                <Col span={20}>
                                <div className='listLeft'>
                                    <div className='imgDiv'>
                                        <img src= { item.icon || '../../../images/default-icon.jpg' } alt='房子' />
                                    </div>
                                    <div className='listText'>
                                        <p style={{fontWeight:'bold'}}>{item.basicInfo.name}</p>
                                        <p className='itemPerson'>
                                          <span>{item.residentUser1Info.userName || '暂无'}</span>
                                          <span>{item.residentUser2Info.userName || '暂无'}</span>
                                          <span>{item.residentUser3Info.userName || '暂无'}</span>
                                          <span>{item.residentUser4Info.userName || '暂无'}</span>
                                        </p>
                                    </div>
                                </div>
                                </Col>
                                <Col span={4} className='priceDiv'>
                                    <Button type="primary" shape="circle" icon="edit" onClick={() => this.handleEdit(item)} />
                                </Col>
                            </Row>
                        </Col>
                      </Row>
                    )
                  })
                }
                {/* {
                    this.props.dataSource.length === 0 ? null :
                    <Pagination showQuickJumper style={{textAlign: 'right', margin: '15px 0'}}
                      current={this.state.pageIndex + 1}  
                      total={this.props.dataSource.totalCount} 
                      onChange={this.handlePageChange}/>
                } */}
                <Modal  title='修改驻场' 
                        visible={show}
                        confirmLoading={operating}
                        footer={[
                          <Button  size="large" onClick={this.handleCancel}>取消</Button>,
                          <Button  type="primary" size="large" onClick={this.handleOk} disabled={this.props.examineStatus === 1 ? true : false}>确定</Button>,
                        ]}
                        // onOk={this.handleOk}
                        onCancel={this.handleCancel}
                        >
                        <Info type='modal' />
                </Modal>
              </Content>
          </Layout>
      </div>
    </Spin>
    )
  }
}

function mapStateToProps(state) {
  return {
    editManagerInfo: state.manager.editManagerInfo,
    dataSource: state.manager.dataSource,
    isLoading: state.manager.isLoading,
    checkedArr: state.manager.checkedArr,
    userArr: state.manager.userArr,
    currentInfo: state.manager.currentInfo,
    examineStatus: state.manager.examineStatus,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch,
    closeModal: () => dispatch(closeModal()),
    openModal: (...args) => dispatch(openModal(...args)),
    savePerson: (...args) => dispatch(savePerson(...args))
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(ZcManagement);