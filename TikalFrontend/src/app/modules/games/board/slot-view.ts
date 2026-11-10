import { Group, Mesh } from 'three';
import { LineSegments2 } from 'three/addons/lines/LineSegments2.js';
import { HexCoordinate } from '../models/tile';
import { BoardResources } from './board-resources';
import { hexToWorld } from './hex-layout';

// a place on the board no tile has been set on yet, a flat plate in the shape of a tile
export class SlotView {
  readonly object = new Group();

  constructor(coordinate: HexCoordinate, resources: BoardResources) {
    const body = new Mesh(resources.slotGeometry, resources.slotMaterials);
    const outline = new LineSegments2(resources.slotOutlineGeometry, resources.outlineMaterial);

    this.object.add(body, outline);

    const { x, z } = hexToWorld(coordinate);
    this.object.position.set(x, 0, z);
  }
}
