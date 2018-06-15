import {Input } from 'antd';
import React, { Component } from 'react';

class EditableCell extends Component {
  state = {
    value: this.props.value,
    editable: false,
  }
  handleChange = (e) => {
    const value = e.target.value;
    this.setState({ value });
  }
  check = () => {
    this.setState({ editable: false });
    if (this.props.onChange) {
      this.props.onChange(this.state.value);
    }
  }
  edit = () => {
    this.setState({ editable: true });
  }
  render() {
    const { value} = this.state;
    return (
      <div className="editable-cell">
        {
            <Input
              style={{width:80}}
              value={value}
              onChange={this.handleChange}
            />
        }
      </div>
    );
  }
}
export default EditableCell

