import * as React from "react";
import { DiamondNodeModel } from "./DiamondNodeModel";
import { PortWidget } from "storm-react-diagrams";

/**
 * @author Dylan Vorster
 */
export class DiamonNodeWidget extends React.Component {

	static defaultProps = {
		size: 100,
		node: null
	}

	constructor(props) {
		super(props); console.log("props:", props);
		this.state = {};
	}

	render() {
		return (
			<div
				className={"diamond-node"}
				style={{
					position: "relative",
					width: this.props.size,
					height: this.props.size
				}}
			>
				<svg
					width={this.props.size}
					height={this.props.size}
					dangerouslySetInnerHTML={{
						__html:
						`
					<g id="Layer_1">
					</g>
					<g id="Layer_2">
						<polygon fill="${this.props.node.color}" stroke="#000000" stroke-width="3" stroke-miterlimit="10" points="10,` +
						this.props.size / 2 +
						` ` +
						this.props.size / 2 +
						`,20 ` +
						(this.props.size - 10) +
						`,` +
						this.props.size / 2 +
						` ` +
						this.props.size / 2 +
						`,` +
						(this.props.size - 10 - 10) +
						` "/>
						<text x='50%' y='53%' fill='#fff' style='font-size:12px;text-anchor: middle;'>${this.props.node.name}</text>
					</g>
				`
					}}
				/>
				<div
					style={{
						position: "absolute",
						zIndex: 10,
						top: this.props.size / 2 - 8,
						left: -8 + 10
					}}
				>
					<PortWidget name="left" node={this.props.node} />
				</div>
				<div
					style={{
						position: "absolute",
						zIndex: 10,
						left: this.props.size / 2 - 8,
						top: -8 + 17
					}}
				>
					<PortWidget name="top" node={this.props.node} />
				</div>
				<div
					style={{
						position: "absolute",
						zIndex: 10,
						left: this.props.size - 8 - 10,
						top: this.props.size / 2 - 8
					}}
				>
					<PortWidget name="right" node={this.props.node} />
				</div>
				<div
					style={{
						position: "absolute",
						zIndex: 10,
						left: this.props.size / 2 - 8,
						top: this.props.size - 8 - 15
					}}
				>
					<PortWidget name="bottom" node={this.props.node} />
				</div>
			</div>
		);
	}
}

export var DiamonNodeWidgetFactory = React.createFactory(DiamonNodeWidget);
