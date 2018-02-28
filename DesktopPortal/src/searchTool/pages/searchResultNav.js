import { connect } from 'react-redux';
import { getXYHBuildingPrev, getXYHBuildingNext, closeResultDetail, setLoading, backToPrevView } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Icon, notification, Tooltip } from 'antd'


class SearchResultNav extends Component {
    state = {}
    handleNavClick = (oper) => {
        console.log("操作：", oper);
        if (oper === "close") {
            this.props.dispatch(closeResultDetail());
        } else if (oper === "prev") {
            let newBuildingID = "";
            for (let i = 0; i < this.props.searchResult.length; i++) {
                let r = this.props.searchResult[i];
                if (r.id === this.props.activeBuilding.id) {
                    if ((i - 1) > -1) {
                        newBuildingID = this.props.searchResult[(i - 1)].id;
                    } else {
                        notification.warning({
                            message: '已经是第一个了!',
                            duration: 3
                        });
                    }
                    break;
                }
            }
            if (newBuildingID !== "") {
                //console.log("加载的楼盘：", this.props.searchResult[newBuildingID]);
                this.props.dispatch(setLoading(true));
                this.props.dispatch(getXYHBuildingPrev(newBuildingID));
            }
        }
        else if (oper === "next") {
            let newBuildingID = "";
            for (let i = 0; i < this.props.searchResult.length; i++) {
                let r = this.props.searchResult[i];
                if (r.id === this.props.activeBuilding.id) {
                    if ((i + 1) < this.props.searchResult.length) {
                        newBuildingID = this.props.searchResult[(i + 1)].id;
                    } else {
                        notification.warning({
                            message: '已经是最后一个了!',
                            duration: 3
                        });
                    }
                    break;
                }
            }
            if (newBuildingID !== "") {
                //console.log("加载的楼盘：", this.props.searchResult[newBuildingID]);
                this.props.dispatch(setLoading(true));
                this.props.dispatch(getXYHBuildingPrev(newBuildingID));
            }
        } else if (oper === "back") {
            if (this.props.showResult.showShopDetail) {
                this.props.dispatch(backToPrevView({ showShopDetail: false, showBuildingDetal: true }));
            }
        }
    }

    render() {
        const buildInfo = this.props.buildInfo;
        const showResult = this.props.showResult;
        return (
            <div className='searchResultNav itemBorder' >
                <Row>
                    <Col span={4}>
                        <Tooltip title="上一个楼盘">
                            <Button onClick={(e) => this.handleNavClick("prev")}><Icon type="arrow-left" />上一个</Button>
                        </Tooltip>
                    </Col>
                    <Col span={16} style={{ textAlign: 'center' }}>
                        {/* {showResult.showShopDetail ? < Tooltip title={showResult.showShopDetail ? "返回商铺列表" : "返回楼盘"}>
                            <Button type="primary" shape="circle" icon="rollback" onClick={(e) => this.handleNavClick("back")} />&nbsp;
                        </Tooltip> : null}
                        < Tooltip title="关闭">
                            <Button type="primary" shape="circle" icon="close" onClick={(e) => this.handleNavClick("close")} />
                        </Tooltip> */}
                    </Col>
                    <Col span={4} style={{ textAlign: 'right' }}>
                        <Tooltip title="下一个楼盘">
                            <Button onClick={(e) => this.handleNavClick("next")}>下一个<Icon type="arrow-right" /></Button>
                        </Tooltip>
                    </Col>
                </Row>
            </div >
        )
    }

}

function mapStateToProps(state) {
    return {
        searchResult: state.search.searchResult,//查询结果
        activeBuilding: state.search.activeBuilding,//选中的楼盘
        showResult: state.search.showResult,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(SearchResultNav);