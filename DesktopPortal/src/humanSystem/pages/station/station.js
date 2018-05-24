import { connect } from 'react-redux';
import { createStation, getOrgList, adduserPage } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Form, Popconfirm, InputNumber, Cascader, Button, Row, Col, Spin} from 'antd'
import './station.less';

const styles = {
    conditionRow: {
        width: '80px',
        display: 'inline-block',
        fontWeight: 'bold',
    },
    bSpan: {
        fontWeight: 'bold',
    },
    otherbtn: {
        padding: '0px, 5px',
    }
}

  const FormItem = Form.Item;
  const EditableContext = React.createContext();
  
  const EditableRow = ({ form, index, ...props }) => (
    <EditableContext.Provider value={form}>
      <tr {...props} />
    </EditableContext.Provider>
  );
  
  const EditableFormRow = Form.create()(EditableRow);
  
  class EditableCell extends React.Component {
    getInput = () => {
      if (this.props.inputType === 'number') {
        return <InputNumber size="small" />;
      }
      return <Input size="small" />;
    };

    render() {
      const {
        editing,
        dataIndex,
        title,
        inputType,
        record,
        index,
        ...restProps
      } = this.props;

      return (
        <EditableContext.Consumer>
          {(form) => {
            const { getFieldDecorator } = form;
            return (
              <td {...restProps}>
                {editing ? (
                  <FormItem>
                    {getFieldDecorator(dataIndex, {
                      rules: [
                        {
                          required: true,
                          message: `Please Input ${title}!`,
                        },
                      ],
                      initialValue: record[dataIndex],
                    })(this.getInput())}
                  </FormItem>
                ) : (
                  restProps.children
                )}
              </td>
            );
          }}
        </EditableContext.Consumer>
      );
    }
  }
  

const ListColums = [
    { title: '职位名称', dataIndex: 'stationname', key: 'stationname' },
    {
        title: 'operation',
        dataIndex: 'operation',
        render: (text, record) => {
            const editable = this.isEditing(record);
            return (
                <div className="editable-row-operations">
                    {editable ? (
                    <span>
                        <EditableContext.Consumer>
                        {form => (
                            <a
                            href="javascript:;"
                            onClick={() => this.save(form, record.key)}
                            >
                            Save
                            </a>
                        )}
                        </EditableContext.Consumer>
                        <Popconfirm
                        title="Sure to cancel?"
                        onConfirm={() => this.cancel(record.key)}
                        >
                        <a>Cancel</a>
                        </Popconfirm>
                    </span>
                    ) : (
                    <a onClick={() => this.edit(record.key)}>Edit</a>
                    )}
                </div>
            );
        },
    },
]
const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
      console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
      disabled: record.name === 'Disabled User', // Column configuration not to be checked
      name: record.name,
    }),
};


class Station extends Component {
    state = {
        department: ''
    }

    componentWillMount() {
        
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                //this.props.dispatch(postBlackLst(values));
            }
        });
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e;
    }

    handleSearchBoxToggle1 = (e) => {
        //this.props.dispatch(adduserPage({id: 11, menuID: 'menu_blackaddnew', disname: '新建黑名单', type:'item'}));
    }

    handleSearchBoxToggle2 = (e) => {
        //this.props.dispatch(adduserPage({id: 11, menuID: 'menu_blackaddnew', disname: '新建黑名单', type:'item'}));
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        return (
            <div style={{display: "block"}}>
                <Row className='searchBox'>
                    <Col span={12}>
                        <label style={styles.conditionRow}>选择分公司 ：</label>
                        <Cascader options={this.props.setDepartmentOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                    </Col>
                </Row>
                <Row className="btnBlock">
                    <Col span={6}>
                        <Button className="searchButton" type="primary" onClick={this.handleSearchBoxToggle1}>新建</Button>
                        <Button className="searchButton" type="primary" onClick={this.handleSearchBoxToggle2}>保存</Button>
                    </Col>
                </Row>
                <Table rowSelection={rowSelection} rowKey={record => record.key} columns={ListColums} onChange={this.handleTableChange} />
                {/* dataSource={} */}
            </div>
        );
    }
}

function tableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.basicData.searchOrgTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Station));