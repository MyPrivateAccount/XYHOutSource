import * as SRD from "storm-react-diagrams";
import { DiamondNodeModel } from "./DiamondNodeModel";
import { DiamondPortModel } from "./DiamondPortModel";

const tt = SRD.AbstractInstanceFactory(DiamondNodeModel);
const pt = SRD.AbstractInstanceFactory(DiamondPortModel);
export class DiamondNodeFactory extends tt {
	constructor() {
		super("DiamondNodeModel");
	}

	getInstance() {
		return new DiamondNodeModel();
	}
}

export class DiamondPortFactory extends pt {
	constructor() {
		super("DiamondPortModel");
	}

	getInstance() {
		return new DiamondPortModel();
	}
}
