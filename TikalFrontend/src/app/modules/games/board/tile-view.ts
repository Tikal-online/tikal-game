import { Group, Mesh } from 'three';
import { LineSegments2 } from 'three/addons/lines/LineSegments2.js';
import { Tile } from '../models/tile';
import { Hoverable } from './utils/board-hover';
import { BoardResources } from './board-resources';
import { hexToWorld } from './hex-layout';

const TILE_COLOURS: Record<Tile['type'], string> = {
  empty: '#3f7a4f',
  treasure: '#d9a514',
  volcano: '#7a3b2a',
  temple: '#b9b1a0',
};

export class TileView implements Hoverable {
  readonly object = new Group();

  readonly body: Mesh;

  private colour: string;

  private highlighted = false;

  constructor(
    tile: Tile,
    private readonly resources: BoardResources,
  ) {
    this.colour = TILE_COLOURS[tile.type];
    this.body = new Mesh(resources.tileGeometry, resources.tileMaterials(this.colour));
    const outline = new LineSegments2(resources.tileOutlineGeometry, resources.outlineMaterial);

    this.object.add(this.body, outline);
    this.update(tile);
  }

  update(tile: Tile): void {
    const { x, z } = hexToWorld(tile.coordinate);

    this.object.position.set(x, 0, z);
    this.colour = TILE_COLOURS[tile.type];
    this.applyMaterials();
  }

  setHighlighted(highlighted: boolean): void {
    this.highlighted = highlighted;
    this.applyMaterials();
  }

  private applyMaterials(): void {
    this.body.material = this.resources.tileMaterials(this.colour, this.highlighted);
  }
}
