import * as SRD from "storm-react-diagrams";
import * as _ from "lodash";

export class DiamondPortModel extends SRD.PortModel {
	//position: string | "top" | "bottom" | "left" | "right";

	constructor(pos) {//: string = "top"
		super(pos || "top");
		this.position = pos;
	}

	serialize() {
		return _.merge(super.serialize(), {
			position: this.position
		});
	}

	deSerialize(data) {
		super.deSerialize(data);
		this.position = data.position;
	}
}
