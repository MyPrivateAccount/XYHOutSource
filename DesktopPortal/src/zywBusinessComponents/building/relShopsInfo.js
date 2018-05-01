import React, {Component} from 'react'
import {Layout, Form, Icon, Row, Col} from 'antd'
import {getDicPars} from '../../utils/utils'

const {Header, Sider, Content} = Layout;
class RelShopsInfo extends Component {
    state = {
        expandStatus: true
    }
    componentWillMount() {

    }
    getTextByCode(dic, code) {
        let text = code;
        let isArray = Array.isArray(code);
        if (!dic || !code) {
            return "";
        }
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
        let basicData = this.props.basicData || [];
        let relShopInfo = (this.props.buildingInfo || {}).shopInfo || {};
        let shopCategory = getDicPars('SHOP_CATEGORY', basicData);
        let saleModel = getDicPars('ZYW_SALE_MODE', basicData);

        return (
            <Layout>
                <Content className='content' >
                    <div style={{marginTop: '15px', backgroundColor: "#ECECEC"}}>
                        <Row type="flex" style={{padding: '1rem 0'}}>
                            <Col span={23}>
                                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>商铺整体概况</span>
                            </Col>
                            <Col span={1}><Icon type={this.state.expandStatus ? "up" : "down"} onClick={(e) => this.setState({expandStatus: !this.state.expandStatus})} /></Col>
                        </Row>
                        <div style={{display: this.state.expandStatus ? "block" : "none"}}>
                            <Row className='viewRow'>
                                <Col span={12}>商铺销售模式：{this.getTextByCode(saleModel, relShopInfo.saleMode)}</Col>
                                <Col span={12}>商铺类别：{this.getTextByCode(shopCategory, relShopInfo.shopCategory)}</Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={12}>周边三公里居住人数：{relShopInfo.populations}</Col>
                            </Row>
                            <Row className='viewRow'>
                                <Col span={24}>优惠政策：{relShopInfo.preferentialPolicies}</Col>
                            </Row>
                        </div>
                    </div>
                </Content>
            </Layout>
        )
    }
}
export default RelShopsInfo;