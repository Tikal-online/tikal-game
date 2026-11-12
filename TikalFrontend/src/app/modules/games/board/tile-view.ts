import { Group, Mesh } from 'three';
import { LineSegments2 } from 'three/addons/lines/LineSegments2.js';
import { Tile, TravelCost } from '../models/tile';
import { Hoverable } from './utils/board-hover';
import { BoardResources } from './board-resources';
import { EDGE_OFFSETS, hexToWorld } from './hex-layout';

const TILE_COLOURS: Record<Tile['type'], string> = {
  empty: '#3f7a4f',
  treasure: '#d9a514',
  volcano: '#7a3b2a',
  temple: '#b9b1a0',
};

export class TileView implements Hoverable {
  readonly object = new Group();

  readonly body: Mesh;

  private readonly costBars = new Group();

  private colour: string;

  private highlighted = false;

  constructor(
    tile: Tile,
    private readonly resources: BoardResources,
  ) {
    this.colour = TILE_COLOURS[tile.type];
    this.body = new Mesh(resources.tileGeometry, resources.tileMaterials(this.colour));
    const outline = new LineSegments2(resources.tileOutlineGeometry, resources.outlineMaterial);

    this.object.add(this.body, outline, this.costBars);
    this.update(tile);
  }

  update(tile: Tile): void {
    const { x, z } = hexToWorld(tile.coordinate);

    this.object.position.set(x, 0, z);
    this.colour = TILE_COLOURS[tile.type];
    this.applyMaterials();
    this.showCosts(tile.costs);
  }

  setHighlighted(highlighted: boolean): void {
    this.highlighted = highlighted;
    this.applyMaterials();
  }

  private applyMaterials(): void {
    this.body.material = this.resources.tileMaterials(this.colour, this.highlighted);
  }

  // one bar per cost point at each edge, an edge with a cost of 0 stays empty
  private showCosts(costs: TravelCost): void {
    this.costBars.clear();

    for (const [edge, cost] of Object.entries(costs) as [keyof TravelCost, number][]) {
      const costBars = this.resources.costBars(cost);

      if (!costBars) {
        continue;
      }

      // the bars are made for the edge facing +z, turned towards the neighbour across this edge
      const { x, z } = hexToWorld(EDGE_OFFSETS[edge]);
      const bars = new Mesh(costBars.geometry, costBars.material);
      bars.rotation.y = Math.atan2(x, z);

      this.costBars.add(bars);
    }
  }
}
