import React, {Component} from 'react'
import {Row, Col, Layout, Tooltip, Button, Menu, Spin, Popconfirm} from 'antd'
import {connect} from 'react-redux'
import {} from '../actions'
import AreaDialog from './AreaDialog'

const {Header, Sider, Content} = Layout;

class TradePlannnings extends Component {

    componentWillMount() {
        // this.props.getCityList();

    }

    changeCity = ({item, key, keyPath}) => {
        if ((this.props.area.city.current || {}).code === key)
            return;
        this.props.setCurrentCity(key);
        this.props.getDistrictList(key);
    }
    changeDistrict = ({item, key, keyPath}) => {
        if ((this.props.area.district.current || {}).code === key)
            return;
        this.props.setCurrentDistrict(key);
        this.props.getTradingAreaList(key);
    }
    changeTradingArea = ({item, key, keyPath}) => {
        if ((this.props.area.tradingArea.current || {}).code === key)
            return;
        this.props.setCurrentTradingArea(key);
    }

    render() {
        let {city, district, tradingArea} = this.props.area;

        return (
            <div className="inner-page full">
                <Row className="h-100">
                    <Col className="h-100" span={8}>
                        <Layout className="full r-05">
                            <Header>
                                城市
                                <Tooltip placement="topLeft" title="添加城市" arrowPointAtCenter>
                                    <Button className="ml-1" type='primary' shape='circle' icon='plus' size="small" onClick={() => this.props.addCity()} />
                                </Tooltip>
                            </Header>
                            <Content className="overflow">
                                <Spin className="h-100" spinning={city.loadingList}>
                                    <Menu onClick={this.changeCity} selectedKeys={[(city.current || {}).code]}>
                                        {
                                            city.list.filter((item) => {
                                                return item.deleted === undefined || false
                                            }).map((item) => {
                                                return (
                                                    <Menu.Item key={item.code}>
                                                        <span>{item.name} ({item.code}, {item.order})</span>
                                                        {(city.current || {}).code === item.code ? (
                                                            <span className="right-tool-bar">
                                                                <Tooltip placement="topLeft" title="修改城市" arrowPointAtCenter>
                                                                    <Button disabled={item.operating} type="primary" shape="circle" icon="edit" size="small" onClick={(e) => this.props.editCity(item.code)} />
                                                                </Tooltip>
                                                                <Popconfirm title="你确定要删除此城市么?" onConfirm={(e) => this.props.delCity(item.code)} onCancel={null} okText="是" cancelText="否">
                                                                    <Button disabled={item.operating} loading={item.operating} type="primary" className="ml-sm" shape="circle" icon="delete" size="small" />
                                                                </Popconfirm>
                                                            </span>) : null
                                                        }
                                                    </Menu.Item>
                                                )
                                            })
                                        }

                                    </Menu>
                                </Spin>
                            </Content>
                        </Layout>
                    </Col>
                    <Col className="h-100" span={8}>
                        <Layout className="full l-05 r-05">
                            <Header>
                                {
                                    city.current != null ? city.current.name : null
                                }
                                {
                                    city.current != null ? (
                                        <Tooltip placement="topLeft" title="添加区域" arrowPointAtCenter>
                                            <Button className="ml-1" type='primary' shape='circle' icon='plus' size="small" onClick={() => this.props.addDistrict(city.current.code)} />
                                        </Tooltip>) : null
                                }
                            </Header>
                            <Content className="overflow">
                                <Spin spinning={district.loadingList}>
                                    <Menu onClick={this.changeDistrict} selectedKeys={[(district.current || {}).code]}>
                                        {
                                            district.list.filter((item) => {
                                                return item.deleted === undefined || false
                                            }).map((item) => {
                                                return (
                                                    <Menu.Item key={item.code}>
                                                        <span>{item.name} ({item.code}, {item.order})</span>
                                                        {(district.current || {}).code === item.code ? (
                                                            <span className="right-tool-bar">
                                                                <Tooltip placement="topLeft" title="修改区域" arrowPointAtCenter>
                                                                    <Button disabled={item.operating} type="primary" shape="circle" icon="edit" size="small" onClick={(e) => this.props.editDistrict(item.code)} />
                                                                </Tooltip>
                                                                <Popconfirm title="你确定要删除此区域么?" onConfirm={() => this.props.delDistrict(item.code)} onCancel={null} okText="是" cancelText="否">
                                                                    <Button disabled={item.operating} loading={item.operating} type="primary" className="ml-sm" shape="circle" icon="delete" size="small" />
                                                                </Popconfirm>
                                                            </span>) : null
                                                        }
                                                    </Menu.Item>
                                                )
                                            })
                                        }

                                    </Menu>
                                </Spin>
                            </Content>
                        </Layout>
                    </Col>
                    <Col className="h-100" span={8}>
                        <Layout className="full l-05">
                            <Header>
                                {
                                    district.current != null ? district.current.name : null
                                }
                                {
                                    district.current != null ? (
                                        <Tooltip placement="topLeft" title="添加片区" arrowPointAtCenter>
                                            <Button className="ml-1" type='primary' shape='circle' icon='plus' size="small" onClick={() => this.props.addTradingArea(district.current.code)} />
                                        </Tooltip>) : null
                                }
                            </Header>
                            <Content className="overflow">
                                <Spin spinning={tradingArea.loadingList}>
                                    <Menu onClick={this.changeTradingArea} selectedKeys={[(tradingArea.current || {}).code]}>
                                        {
                                            tradingArea.list.filter((item) => {
                                                return item.deleted === undefined || false
                                            }).map((item) => {
                                                return (
                                                    <Menu.Item key={item.code}>
                                                        <span>{item.name} ({item.code}, {item.order})</span>
                                                        {(tradingArea.current || {}).code === item.code ? (
                                                            <span className="right-tool-bar">
                                                                <Tooltip placement="topLeft" title="修改片区" arrowPointAtCenter>
                                                                    <Button disabled={item.operating} type="primary" shape="circle" icon="edit" size="small" onClick={() => this.props.editTradingArea(item.code)} />
                                                                </Tooltip>
                                                                <Popconfirm title="你确定要删除此片区么?" onConfirm={() => this.props.delTradingArea(item.code)} onCancel={null} okText="是" cancelText="否">
                                                                    <Button disabled={item.operating} loading={item.operating} type="primary" className="ml-sm" shape="circle" icon="delete" size="small" />
                                                                </Popconfirm>
                                                            </span>) : null
                                                        }
                                                    </Menu.Item>
                                                )
                                            })
                                        }

                                    </Menu>
                                </Spin>
                            </Content>
                        </Layout>
                    </Col>
                </Row>
                <AreaDialog />
            </div>
        )
    }
}

const mapStateToProps = (state, props) => {
    return {
        area: state.area
    }
}

const mapDispatchToProps = (dispatch) => {
    return {
        dispatch
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(TradePlannnings);