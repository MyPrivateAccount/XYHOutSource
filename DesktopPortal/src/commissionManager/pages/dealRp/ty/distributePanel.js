import React, { Component } from 'react';
import { Table, Row } from 'antd';
import { getDicPars } from '../../../../utils/utils'
import { dicKeys } from '../../../constants/const'
import '../rp.less'


class DistributePanel extends Component {

    _wyColumns = () => {
        const { wyItems, dic } = this.props;
        let sfdxList = getDicPars(dicKeys.sfdx, dic)
        return [
            {
                title: '款项类型',
                dataIndex: 'moneyType',
                key: 'moneyType',
                align: 'center',
                render: (text, record) => {
                    let item = (wyItems || []).find(x => x.code === text);
                    return (item || {}).name || ''
                }
            },
            {
                title: '收付对象',
                dataIndex: 'object',
                key: 'object',
                align: 'center',
                render: (text, record) => {
                    let item = (sfdxList || []).find(x => x.value === text);
                    return (item || {}).key || '';
                }
            },
            {
                title: '备注',
                dataIndex: 'remark',
                key: 'remark'
            },
            {
                title: '金额',
                dataIndex: 'money',
                key: 'money',
                width: '16rem',
                className: 'column-money'
            },
        ];
    }

    _nyColumns = () => {
        const { nyItems } = this.props;

        return [
            {
                title: '部门',
                dataIndex: 'sectionName',
                key: 'sectionName'
            },
            {
                title: '员工',
                dataIndex: 'username',
                key: 'username',
                align: 'center'
            },
            {
                title: '工号',
                dataIndex: 'workNumber',
                key: 'workNumber',
                align: 'center'
            },
            {
                title: '金额',
                dataIndex: 'money',
                key: 'money',
                width: '16rem',
                className: 'column-money'
            },
            {
                title: '比例',
                dataIndex: 'percent',
                key: 'percent',
                width: '10rem',
                align: 'center'
            },
            {
                title: '单数',
                dataIndex: 'oddNum',
                key: 'oddNum',
                width: '10rem',
                align: 'center'
            },
            {
                title: '身份',
                dataIndex: 'type',
                key: 'type',
                width: '12rem',
                align: 'center',
                render: (text, record) => {
                    let item = (nyItems || []).find(x => x.code === text);
                    return (item || {}).name || ''
                }
            },
        ];
    }

    render() {
        const d = this.props.distribute || {};
        const ol = d.reportOutsides || [];
        const il = d.reportInsides || [];

        const { hideYj, hideNy, hideWy, onlyTable } = this.props;

        const oColumns = this._wyColumns();
        const iColumns = this._nyColumns();
        return (<div>
            {
                hideYj ? null :
                    <Row>
                        <span className="rp-yj">业主佣金：</span><span className="rp-yj-je"> {d.ownerMoney || 0} </span>
                        <span className="rp-yj">客户佣金：</span><span className="rp-yj-je"> {d.customMoney || 0} </span>
                    </Row>
            }
            {
                hideWy ? null :
                    <div>
                        {onlyTable ? null : <div className="rp-yj-tbl-title">对外合作关系</div>}
                        <Table pagination={false} bordered size="small" columns={oColumns} dataSource={ol} />
                        {onlyTable ? null : <Row>
                            <span className="rp-yj">净佣金：</span><span className="rp-yj-je"> {d.jyj} </span>
                        </Row>}
                    </div>
            }

            {
                hideNy ? null : <div><div className="rp-yj-tbl-title">内部分配</div>
                    <Table pagination={false} bordered size="small" columns={iColumns} dataSource={il} />
                </div>
            }

        </div>)
    }
}


export default DistributePanel;