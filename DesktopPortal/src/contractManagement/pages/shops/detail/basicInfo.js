import {connect} from 'react-redux';
import {editShopBasic} from '../../../actions/actionCreator';
import React, {Component} from 'react'
import {Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Form, Col, Row, Icon, Tag} from 'antd'
import '../edit/editCommon.less';
import {TAG_COLOR} from '../../../../constants/uiColor'

const {Header, Sider, Content} = Layout;

class BasicInfo extends Component {
    handleEdit = (e) => {
        this.props.editShopBasic();
    }
    getTextByCode(dic, code) {
        let text;
        let isArray = Array.isArray(code);
        dic.map(item => {
            if (isArray) {
                if (code.find((c) => c === item.value)) {
                    text = item.key;
                }
            } else {
                if (item.value === code) {
                    text = item.key;
                }
            }
        });
        return text;
    }
    render() {
        let basicInfo = this.props.basicInfo || {}
        const shopInfo = this.props.shopInfo || {}
        let saleStatus = (basicInfo.saleStatus || '').toString()
        let shopDetailProjectInfo = this.props.shopDetailProjectInfo || []
        let name, address;
        shopDetailProjectInfo.forEach((v, i) => {
            // if (v.name !== null) {
            if (v.id === shopInfo.buildingId) {
                name = v.basicInfo.name
                address = v.basicInfo.address
            }
            // }
        })
        // console.log(shopDetailProjectInfo, name,address, '楼盘信息', basicInfo, this.props.shopInfo)
        return (
            <div className="relative">
                <Layout>
                    <Content className=' basicEdit' style={{padding: '25px 0', marginTop: '25px', backgroundColor: "#ECECEC"}}>
                        <Form layout="horizontal" >
                            <Row type="flex" style={{padding: '1rem 0'}}>
                                <Col span={20} >
                                    <Icon type="tags-o" className='content-icon' /> <span className='content-title'>基本信息</span>
                                </Col>
                                <Col span={4}>
                                    {
                                        [1, 8].includes(this.props.shopInfo.examineStatus) ? null : <Button type="primary" shape="circle" icon="edit" onClick={this.handleEdit} />
                                    }
                                </Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12} >
                                    {/* <Tag color="red-inverse">{this.getTextByCode(this.props.basicData.shopStatus, basicInfo.status)}</Tag> */}
                                    {
                                        this.props.basicData.shopSaleStatus.length === 0 ? null :
                                            <Tag color="orange-inverse">{this.getTextByCode(this.props.basicData.shopSaleStatus, saleStatus)}</Tag>
                                    }
                                    <span className="buildingNo">{basicInfo.buildingNo}-{basicInfo.floorNo}-{basicInfo.number}</span> <span className="buildingName">{name}</span></Col>
                                <Col span={12}></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>内部最低总价：<span className='redSpan'>{basicInfo.totalPrice} 元</span></Col>
                                <Col span={12}>总价：<span className='redSpan'>{basicInfo.guidingPrice} 元/㎡</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>建筑面积：<span className='boldSpan'>{basicInfo.buildingArea} ㎡</span></Col>
                                <Col span={12}>套内面积：<span className='boldSpan'>{basicInfo.houseArea} ㎡</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>朝向：<span>{this.getTextByCode(this.props.basicData.shopTword, basicInfo.toward)}</span></Col>
                                <Col span={12}>可外摆区域：<span className='boldSpan'>{basicInfo.outsideArea} ㎡</span></Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24}>地址：{address}</Col>
                            </Row>
                            <Row className='viewRow' style={{marginTop: '10px'}}>
                                <Col span={24}>
                                    {
                                        basicInfo.isCorner ? <Tag color={TAG_COLOR}>拐角铺</Tag> : null
                                    }
                                    {
                                        basicInfo.hasStreet ? <Tag color={TAG_COLOR}>双边街</Tag> : null
                                    }
                                    {
                                        basicInfo.hasFree ? <Tag color={TAG_COLOR}>赠送({basicInfo.freeArea}㎡)</Tag> : null
                                    }
                                    {
                                        basicInfo.isFaceStreet ? <Tag color={TAG_COLOR}>临街({basicInfo.streetDistance}m)</Tag> : null
                                    }
                                    {
                                        basicInfo.shopCategory ? <Tag color={TAG_COLOR}>{this.getTextByCode(this.props.basicData.shopsTypes, basicInfo.shopCategory)}</Tag> : null
                                    }

                                </Col>
                            </Row>
                        </Form>
                    </Content>
                </Layout>
            </div>

        )
    }
}

function mapStateToProps(state) {
    // console.log(state.shop.shopsInfo, 66)
    return {
        shopInfo: state.shop.shopsInfo,
        basicInfo: state.shop.shopsInfo.basicInfo,
        shopDetailProjectInfo: state.shop.shopDetailProjectInfo,
        basicData: state.basicData,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        editShopBasic: () => dispatch(editShopBasic())
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicInfo);