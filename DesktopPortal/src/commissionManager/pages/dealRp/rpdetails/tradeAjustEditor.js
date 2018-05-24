//业绩调整组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import {Modal,Layout } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'

class TradAjustEditor extends Component {

    render() {
        return (
                <Modal width={600} title={'调佣明细'} maskClosable={false} visible={false}>
                   <TradeWyTable/>
                   <TradeNTable/>
                </Modal>
        )
    }
}
export default TradAjustEditor