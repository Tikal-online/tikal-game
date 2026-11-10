import { Group, Mesh } from 'three';
import { LineSegments2 } from 'three/addons/lines/LineSegments2.js';
import { HexCoordinate } from '../models/tile';
import { Hoverable } from './board-hover';
import { BoardResources } from './board-resources';
import { hexToWorld } from './hex-layout';

// a place on the board no tile has been set on yet, a flat plate in the shape of a tile
export class SlotView implements Hoverable {
  readonly object = new Group();

  readonly body: Mesh;

  constructor(
    coordinate: HexCoordinate,
    private readonly resources: BoardResources,
  ) {
    this.body = new Mesh(resources.slotGeometry, resources.slotMaterials());
    const outline = new LineSegments2(resources.slotOutlineGeometry, resources.outlineMaterial);

    this.object.add(this.body, outline);

    const { x, z } = hexToWorld(coordinate);
    this.object.position.set(x, 0, z);
  }

  setHighlighted(highlighted: boolean): void {
    this.body.material = this.resources.slotMaterials(highlighted);
  }
}
