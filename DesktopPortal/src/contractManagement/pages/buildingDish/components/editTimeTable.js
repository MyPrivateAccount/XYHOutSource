import { Table, Input, Icon, Button, Popconfirm, InputNumber, DatePicker } from 'antd';
import { connect } from 'react-redux';
import { } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import moment from 'moment';


class EditTable extends React.Component {
  state = {
    value: this.props.value,
  }
  componentWillReceiveProps(newprops) {
    this.setState({
      value: newprops.value
    })
  }
  handleChange = (dates, dateStrings) => {
    const value = dates;
    this.setState({ value }, () => {
      if (this.props.onChange) {
        this.props.onChange(this.state.value);
      }
    });
  }
  render() {
    let { value } = this.state;
    // console.log(value, '111111')
    return (
      <div className="editable-cell">
        <div className="editable-cell-input-wrapper">
          <DatePicker format='YYYY-MM-DD' allowClear={false}
            value={value !== null ? moment(value, 'YYYY-MM-DD') : null}
            onChange={this.handleChange} />
        </div>
      </div>
    );
  }
}

export default EditTable;