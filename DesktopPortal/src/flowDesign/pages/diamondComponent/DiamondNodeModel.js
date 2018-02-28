import * as SRD from "storm-react-diagrams";
import { DiamondPortModel } from "./DiamondPortModel";

export class DiamondNodeModel extends SRD.NodeModel {
	constructor(name, color) {
		super("diamond");
		this.name = name || '';
		this.color = color || 'purple';
		this.addPort(new DiamondPortModel("top"));
		this.addPort(new DiamondPortModel("left"));
		this.addPort(new DiamondPortModel("bottom"));
		this.addPort(new DiamondPortModel("right"));
	}
}
