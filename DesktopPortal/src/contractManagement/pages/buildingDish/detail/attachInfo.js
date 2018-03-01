import { connect } from 'react-redux';
// import { } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip } from 'antd'

const { Header, Sider, Content } = Layout;

class AttachInfo extends Component {
    state = {

    }
    componentWillMount() {

    }



    render() {
        return (
            <div className="relative">
                <Layout>
                    {/* <Header>
                        应用管理
                        <Tooltip title="新增应用">
                            &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                        </Tooltip>
                    </Header> */}
                    <Content className='content'>

                    楼盘附加信息
                    </Content>
                </Layout>
            </div>
        )
    }
}

function mapStateToProps(state) {
    //console.log('shopsMapStateToProps:' + JSON.stringify(state));
    return {

    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AttachInfo);