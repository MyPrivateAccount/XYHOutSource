import {connect} from 'react-redux';
import React, {Component} from 'react';
import {Input, Steps, Icon, Button, Row, Col, notification} from 'antd';
import {TAG_COLOR} from './constants/uiColor';

const styles = {
    gridLayout: {
        width: '400px',
        margin: '0 auto',
        color: TAG_COLOR,
        // border: '1px solid'
    },
    row: {
        margin: '10px'
    }
}

class Download extends Component {
    state = {

    }
    componentWillMount() {

    }
    componentWillReceiveProps(newProps) {
    }

    handleDownload = (type) => {
        if (type === "android") {
            document.location = 'https://www.xinyaohangdc.com/filedownload/yaokongjian.apk';
        } else {
            notification.info({
                message: '提示',
                description: 'apple版本开发中,敬请期待...',
            });
        }
    }

    render() {


        return (
            <Row style={styles.gridLayout}>
                <Col span={24} style={{textAlign: 'center', marginBottom: '20px'}}><h1 style={{display: 'inline'}}>耀空间</h1><div style={{display: 'inline'}}>for mobile</div></Col>
                <Col span={24} style={styles.row}>耀空间APP现已上线，敬请下载！</Col>
                <Col>
                    <Row>
                        <Col span={12}>
                            <Row>
                                <Col span={24} style={styles.row}><Button onClick={() => this.handleDownload('android')}><Icon type="android" />安卓版本</Button></Col>
                                <Col span={24} style={styles.row}><Button onClick={() => this.handleDownload('apple')}><Icon type="apple" />苹果版本</Button></Col>
                            </Row>
                        </Col>
                        <Col span={12}>
                            <img src="../images/download.png" style={{width: '90px'}} alt='二维码' />
                            <div>&nbsp;&nbsp;安卓扫码下载</div>
                        </Col>
                    </Row>
                </Col>
            </Row >
        )
    }
}


export default Download