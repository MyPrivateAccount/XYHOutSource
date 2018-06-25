import React, { Component, Children } from 'react'
import './Layer.less'
import Spinner from './Spinner'

export class LayerRouter extends Component {
    render() {
        return (
            <div>
                {this.props.children}
            </div>
        )
    }
}

class Layer extends Component {

    constructor(props){
        super(props);

        this.scrollPage = this.scrollPage.bind(this)
        this.bindContentPanel=this.bindContentPanel.bind(this);
    }

    componentWillUnmount(){
        if(this.pageElement){
            this.pageElement.removeEventListener('scroll', this.scrollPage)
        }
    }

    filterChildren() {
        let router = '';
        let otherChildren = [];
        Children.map(this.props.children, (element) => {
            if(element){
            if (element.type === LayerRouter) {
                router = element;
            } else {
                otherChildren = [...otherChildren, element];
            }
        }
        });

        return [router, ...otherChildren];
    }

    scrollPage = ()=>{
        console.log(arguments);
    }

    bindContentPanel =(e)=>{

        this.contentPanel = e;
        if(!this.contentPanel){
            return;
        }
        this.contentPanel.addEventListener('touchmove', function(e) {
            e.stopPropagation();
        });
    }

    render() {
        const [router, ...children] = this.filterChildren();

        return (
            <div className={`xyh-layer ${this.props.className||''}`} >
                <div className="rel"  ref={this.bindContentPanel}>
                {
                    this.props.fixedPanel
                }
                    <div className="panel">
                      
                        {children}
                    
                    </div>
                    {router}
                    {this.props.showLoading? (<Spinner></Spinner>):null}
                </div>
            </div>
        )
    }
}

export default Layer;
