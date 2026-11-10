import { Group, Mesh } from 'three';
import { LineSegments2 } from 'three/addons/lines/LineSegments2.js';
import { Tile } from '../models/tile';
import { BoardResources } from './board-resources';
import { hexToWorld } from './hex-layout';

// jungle greens of the mockup, varied so neighbouring jungle tiles stay apart
const JUNGLE_COLOURS = ['#3f7a4f', '#4b8657', '#467f53', '#3a7149'];

// the other tile types in the colours of the mockup's legend
const TILE_COLOURS: Record<Exclude<Tile['type'], 'empty'>, string> = {
  treasure: '#d9a514',
  volcano: '#7a3b2a',
  temple: '#b9b1a0',
};

export class TileView {
  readonly object = new Group();

  private readonly body: Mesh;

  constructor(
    tile: Tile,
    private readonly resources: BoardResources,
  ) {
    this.body = new Mesh(resources.tileGeometry, resources.tileMaterials(tileColour(tile)));
    const outline = new LineSegments2(resources.tileOutlineGeometry, resources.outlineMaterial);

    this.object.add(this.body, outline);
    this.update(tile);
  }

  update(tile: Tile): void {
    const { x, z } = hexToWorld(tile.coordinate);

    this.object.position.set(x, 0, z);
    this.body.material = this.resources.tileMaterials(tileColour(tile));
  }
}

function tileColour(tile: Tile): string {
  if (tile.type !== 'empty') {
    return TILE_COLOURS[tile.type];
  }

  // empty tiles are the jungle, the pattern only depends on the position so it stays the same on every load
  const { q, r } = tile.coordinate;
  const row = r + Math.floor(q / 2);
  const index = (q * 3 + row * 2) % JUNGLE_COLOURS.length;

  return JUNGLE_COLOURS[(index + JUNGLE_COLOURS.length) % JUNGLE_COLOURS.length];
}
