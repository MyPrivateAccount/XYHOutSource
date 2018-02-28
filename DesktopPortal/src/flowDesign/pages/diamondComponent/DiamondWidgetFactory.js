import * as SRD from "storm-react-diagrams";
import { DiamonNodeWidgetFactory } from "./DiamondNodeWidget";

export class DiamondWidgetFactory extends SRD.NodeWidgetFactory {
	constructor() {
		super("diamond");
	}
	generateReactWidget(diagramEngine, node) {
		return DiamonNodeWidgetFactory({ node: node });
	}
}