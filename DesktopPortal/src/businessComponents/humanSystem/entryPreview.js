import React, { Component } from 'react'
import { Layout, Icon, Row, Col, Form } from 'antd'
import SocialSecurity from './socialSecurity'
class EntryPreview extends Component {
    state = {

    }
    componentDidMount() {

    }

    render() {

        return (
            <div>
                <SocialSecurity readOnly />
            </div>
        )
    }

}
export default EntryPreview;
