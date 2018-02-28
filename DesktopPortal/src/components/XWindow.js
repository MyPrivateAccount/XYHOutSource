import React from "react"
import PropTypes from 'prop-types';
import {connect} from 'react-redux';
import {activeWindow, closeWindow, restoreWindow, maxmizeWindow, minimizeWindow, resotreXWindow} from '../actions/actionCreators';


export const disableSelect = {
  userSelect: 'none',
  WebkitUserSelect: 'none',
  msUserSelect: 'none',
  MozUserSelect: 'none',
  OUserSelect: 'none'
}

export const defaultTheme = {
  title: {
    userSelect: 'none',
    WebkitUserSelect: 'none',
    msUserSelect: 'none',
    MozUserSelect: 'none',
    OUserSelect: 'none',
    overflow: 'hidden',
    width: '100%',
    height: 25,
  },
  frame: {
    position: 'absolute',
    margin: 0,
    padding: 0,
    overflow: 'hidden',
  },
  transition: 'all 0.25s ease-in-out'
}

const styles = {
  innerFrame: {
    position: 'absolute',
    left: 1,
    top: 1,
    right: 1,
    bottom: 1,
    borderColor: 'transparent',
    borderStyle: 'solid',
    borderWidth: 5,
    backgroundColor: 'white'
  }
}

function prefixedTransition(transition) {
  return transition ? {
    transition: transition,
    WebkitTransition: transition,
    msTransition: transition,
    MozTransition: transition,
    OTransition: transition,
  } : {}
}

class XWindow extends React.Component {
  constructor(props) {
    super(props)
    const {
      transition,
      theme
    } = this.props
    this.cursorX = 0
    this.cursorY = 0
    this.clicked = null
    this.allowTransition = false
    this.frameRect = {}
    this.state = {
      cursor: 'auto',
      transition: prefixedTransition(transition ? transition : theme.transition),

    };

    this.mouseMoveListener = this._onMove.bind(this)
    this.mouseUpListener = this._onUp.bind(this)
  }
  componentDidMount() {
    const {
      initialWidth,
      initialHeight,
      initialTop,
      initialLeft,
      attachedTo,
    } = this.props
    //初始区域
    const boundingBox = this.getFrameRect()
    this.frameRect.width = initialWidth || boundingBox.width
    this.frameRect.height = initialHeight || boundingBox.height
    this.frameRect.top = initialTop || this.refs.frame.offsetTop
    this.frameRect.left = initialLeft || this.refs.frame.offsetLeft

    this.prevState = {
      ...this.frameRect
    }

    attachedTo.addEventListener('mousemove', this.mouseMoveListener)
    attachedTo.addEventListener('mouseup', this.mouseUpListener)

    if (this.props.defaultMaxmize) {
      setTimeout(() => {
        this.maximize();
      }, 0);

    }
  }
  componentWillReceiveProps(nextProps) {
    if (nextProps.transition !== this.props.transition) {
      this.setState({transition: prefixedTransition(nextProps.transition)})
    }
    if (nextProps.isRestoring) {
      setTimeout(() => {
        if (this.props.isMaximized) {
          const {boundary} = this.props;
          let h = this.props.attachedTo.innerHeight;
          if (boundary && typeof boundary.top === 'number') {
            h = h - boundary.top;
          }
          h -= 40;
          this.props.dispatch(maxmizeWindow(this.props.id))
          this.transform({top: 0, left: 0, width: this.props.attachedTo.innerWidth, height: h}, true, false)

        } else {
          this.props.dispatch(restoreWindow(this.props.id))
          this.transform(this.prevState, true, false)
        }
      })
      this.props.dispatch(resotreXWindow({id: this.props.id, start: false}))

    }
  }
  componentWillUnmount() {
    this.props.attachedTo.removeEventListener('mousemove', this.mouseMoveListener)
    this.props.attachedTo.removeEventListener('mouseup', this.mouseUpListener)
  }
  transform(state, allowTransition = true, updateHistory = true, ui = true) {
    const boundingBox = this.getFrameRect()

    let top = this.refs.frame.offsetTop
    let left = this.refs.frame.offsetLeft
    let width = boundingBox.width
    let height = boundingBox.height

    if (updateHistory) {
      this.prevState = {
        top: top,
        left: left,
        width: width,
        height: height,
      }
      console.log('save:' + this.prevState);
    }

    if (!state) return;

    this.frameRect.top = typeof state.top === 'number' ? state.top :
      state.bottom ? (state.bottom - (state.height || height)) : top
    this.frameRect.left = typeof state.left === 'number' ? state.left :
      state.right ? (state.right - (state.width || width)) : left
    this.frameRect.width = typeof state.width === 'number' ? state.width :
      (typeof state.right === 'number' && typeof state.left === 'number') ? state.right - state.left :
        typeof state.right === 'number' ? state.right - this.frameRect.left : width
    this.frameRect.height = typeof state.height === 'number' ? state.height :
      (typeof state.bottom === 'number' && typeof state.top === 'number') ? state.top - state.bottom :
        typeof state.bottom === 'number' ? state.bottom - this.frameRect.top : height
    this.allowTransition = allowTransition
    if (ui) {
      if (this.props.onTransform) {
        setTimeout(this.props.onTransform.bind(this, this.frameRect, this.prevState))
      }
      this.forceUpdate()
    }
  }
  restore(allowTransition = true) {
    console.log(this.prevState);
    this.props.dispatch(restoreWindow(this.props.id))
    this.transform(this.prevState, allowTransition, false)

  }
  minimize(allowTransition = true) {
    this.transform({width: 0, height: 0}, allowTransition, !this.props.isMaximized)
    this.props.dispatch(minimizeWindow(this.props.id))
  }
  maximize(allowTransition = true) {
    let h = this.props.attachedTo.innerHeight;
    if (this.props.parent) {
      h = h - this.props.parent.offsetTop;
    }

    this.props.dispatch(maxmizeWindow(this.props.id))
    this.transform({top: 0, left: 0, width: this.props.attachedTo.innerWidth, height: h}, allowTransition)

  }
  activeWindow(key) {
    this.props.dispatch(activeWindow(key));
  }
  closeWindow(key) {

    this.props.dispatch(closeWindow(key ? key : this.props.id));
  }

  render() {
    const {
      style,
      contentStyle,
      titleStyle,
      xtheme,
      minWidth,
      minHeight,
      animate,
      cursorRemap,
      children,
      onMove,
      onResize
    } = this.props
    let boundary = this.props.boundary;

    const pervFrameRect = {...this.frameRect}
    let hits = this.hitEdges

    if (this.clicked) {

      const boundingBox = this.clicked.boundingBox

      if (hits.top || hits.bottom || hits.left || hits.right) {
        if (hits.right) this.frameRect.width = Math.max(this.cursorX - boundingBox.left, minWidth)
        if (hits.bottom) this.frameRect.height = Math.max(this.cursorY - boundingBox.top, minHeight)

        if (hits.left) {
          let currentWidth = boundingBox.right - this.cursorX
          if (currentWidth > minWidth) {
            this.frameRect.width = currentWidth
            this.frameRect.left = this.clicked.frameLeft + this.cursorX - this.clicked.x
          }
        }

        if (hits.top) {
          let currentHeight = boundingBox.bottom - this.cursorY
          if (currentHeight > minHeight) {
            this.frameRect.height = currentHeight
            this.frameRect.top = this.clicked.frameTop + this.cursorY - this.clicked.y
          }
        }
      }
      else if (this.state.cursor === 'move') {
        this.frameRect.top = this.clicked.frameTop + this.cursorY - this.clicked.y
        this.frameRect.left = this.clicked.frameLeft + this.cursorX - this.clicked.x
      }
    } else {

    }

    //边界限制
    if (!boundary) {
      if (this.props.parent) {
        boundary = {
          left: 0,
          right: this.props.parent.offsetWidth,
          top: 0,
          bottom: this.props.parent.offsetHeight
        }
      }
    }
    if (boundary) {
      let {
        top,
        left,
        width,
        height
      } = this.frameRect
      if (hits && this.clicked) {
        if (hits.right) {
          if (typeof boundary.right === 'number' && left + width > boundary.right) {
            this.frameRect.width = boundary.right - this.frameRect.left;
          }
        }
        if (hits.left) {
          if (typeof boundary.left === 'number' && left < boundary.left) {
            this.frameRect.left = boundary.left;
            this.frameRect.width = this.frameRect.right - this.frameRect.left;
          }
        }
        if (hits.top) {
          if (typeof boundary.top === 'number' && top < boundary.top) {
            let diff = this.frameRect.top - boundary.top;
            this.frameRect.top = boundary.top;
            this.frameRect.height = this.frameRect.height + diff;
          }
        }
        if (hits.bottom) {
          if (typeof boundary.bottom === 'number' && top + height > boundary.bottom) {
            this.frameRect.height = boundary.bottom - top;
          }
        }
      }

      if (typeof boundary.top === 'number' && top < boundary.top) {
        this.frameRect.top = boundary.top
      }

    }

    let cursor = this.state.cursor

    if (cursorRemap) {
      let res = cursorRemap.call(this, cursor)

      if (res && typeof res === 'string') cursor = res
    }

    const wndState = {
      title: this.props.title,
      isMaximized: this.props.isMaximized,
      isWindowFocused: this.props.isWindowFocused,
      onCloseClick: () => this.closeWindow(this.props.id),
      onRestoreDownClick: () => this.restore(),
      onMaximizeClick: () => this.maximize(),
      onMinimizeClick: () => this.minimize()
    }

    let titleBar = (
      <div ref="title"
        style={{
          ...xtheme.title,
          ...titleStyle,
          cursor
        }}>
        {typeof this.props.titleBar !== 'string' ?
          React.cloneElement(this.props.titleBar, {...wndState}) : this.props.titleBar}
      </div>)

    const childrenWithProps = React.Children.map(children, function (child) {
      return typeof child === 'string' ? child : React.cloneElement(child)
    })

    let frameTransition = (animate && this.allowTransition) ? this.state.transition : {}

    if (onMove && (pervFrameRect.top !== this.frameRect.top ||
      pervFrameRect.left !== this.frameRect.left)) {
      setTimeout(onMove.bind(this, this.frameRect, pervFrameRect))
    }

    if (onResize && (pervFrameRect.width !== this.frameRect.width ||
      pervFrameRect.height !== this.frameRect.height)) {
      setTimeout(onResize.bind(this, this.frameRect, pervFrameRect))
    }
    let bd = {};
    let bg = {};
    if (this.props.theme && this.props.theme.palette) {

      bd.borderColor = this.props.theme.palette.primary[500]
      bg.backgroundColor = bd.borderColor;
    }
    return (
      <div ref="frame"
        onMouseDownCapture={this._onDown.bind(this)}
        onMouseMoveCapture={(e) => {
          if (this.clicked !== null) {
            e.preventDefault()
          }
        }}
        style={{
          ...xtheme.frame,
          ...frameTransition,
          cursor: cursor,
          ...style,
          ...this.frameRect,
          zIndex: this.props.zIndex,
          ...bg,
          visibility: this.props.isMinimized ? 'hidden' : 'visible',
          ...(this.clicked ? disableSelect : {})
        }}>
        <div style={{...styles.innerFrame}}>
          {titleBar}
          <div ref='content'
            style={{cursor: 'default', position: 'absolute', width: '100%', top: (xtheme.title.height + 5), bottom: 0, overflow: 'auto', ...contentStyle}}>
            <div style={{position: 'relative', width: '100%', height: '100%', borderTop: '1px solid #fff'}}>
              {childrenWithProps}
            </div>
          </div>
        </div>
      </div>
    )
  }
  //获取顶层div区域
  getFrameRect() {
    return this.refs.frame.getBoundingClientRect()
  }
  getDOMFrame() {
    return this.refs.frame
  }
  getTitleRect() {
    return this.refs.title.getBoundingClientRect()
  }

  //设置鼠标状态
  _cursorStatus(e) {
    const boundingBox = this.getFrameRect()
    this.cursorX = e.clientX
    this.cursorY = e.clientY

    //拖动在render中处理
    if (this.clicked || this.props.isMaximized) return

    //边界宽度，默认4像素
    let hitRange = this.props.edgeDetectionRange
    let hitTop = this.cursorY <= boundingBox.top + hitRange
    let hitBottom = this.cursorY >= boundingBox.bottom - hitRange
    let hitLeft = this.cursorX <= boundingBox.left + hitRange
    let hitRight = this.cursorX >= boundingBox.right - hitRange

    let cursor = 'auto'

    if (hitTop || hitBottom || hitLeft || hitRight) {
      if (hitRight && hitBottom || hitLeft && hitTop) {
        cursor = 'nwse-resize'
      } else if (hitRight && hitTop || hitBottom && hitLeft) {
        cursor = 'nesw-resize'
      } else if (hitRight || hitLeft) {
        cursor = 'ew-resize'
      } else if (hitBottom || hitTop) {
        cursor = 'ns-resize'
      }
      e.stopPropagation()
    }
    else {
      //在标题栏
      const titleBounding = this.getTitleRect()

      if (this.cursorX > titleBounding.left && this.cursorX < titleBounding.right &&
        this.cursorY > titleBounding.top && this.cursorY < titleBounding.bottom) {
        cursor = 'move'
        console.log(cursor);
      }
    }

    this.hitEdges = {
      top: hitTop,
      bottom: hitBottom,
      left: hitLeft,
      right: hitRight
    }

    if (cursor !== this.state.cursor) {
      this.setState({cursor: cursor})
    }

  }
  _onDown(e) {
    this.allowTransition = false
    if (this.props.isMaximized)
      return;
    this._cursorStatus(e)
    const boundingBox = this.getFrameRect()
    this.clicked = {
      x: e.clientX, y: e.clientY, boundingBox: boundingBox,
      frameTop: this.refs.frame.offsetTop, frameLeft: this.refs.frame.offsetLeft
    }
    this.activeWindow(this.props.id);
  }
  _onUp(e) {

    this.clicked = null
    this._cursorStatus(e)

  }
  _onMove(e) {
    this._cursorStatus(e)
    if (this.clicked !== null) {
      this.forceUpdate()
    }
  }
}

function mapStateToProps(state) {
  return {

  };
}

function mapDispatchToProps(dispatch) {
  return {
    dispatch
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(XWindow);

XWindow.propTypes = {
  titleBar: PropTypes.oneOfType([
    PropTypes.object,
    PropTypes.string,
  ]),
  style: PropTypes.object,
  contentClassName: PropTypes.object,
  contentStyle: PropTypes.object,
  titleStyle: PropTypes.object,
  xtheme: PropTypes.object,
  minWidth: PropTypes.number,
  minHeight: PropTypes.number,
  edgeDetectionRange: PropTypes.number,
  initialWidth: PropTypes.number,
  initialHeight: PropTypes.number,
  initialTop: PropTypes.number,
  initialLeft: PropTypes.number,
  transition: PropTypes.string,
  animate: PropTypes.bool,
  onMove: PropTypes.func,
  onResize: PropTypes.func,
  onTransform: PropTypes.func,
  cursorRemap: PropTypes.func,
  boundary: PropTypes.object,
  attachedTo: PropTypes.object,
  parent: PropTypes.object,
}

XWindow.defaultProps = {
  minWidth: 20,
  minHeight: 20,
  edgeDetectionRange: 4,
  theme: defaultTheme,
  initialWidth: null,
  initialHeight: null,
  initialTop: null,
  initialLeft: null,
  animate: true,
  attachedTo: window,
  parent: null
}