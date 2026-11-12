import {
  BoxGeometry,
  BufferGeometry,
  Color,
  ColorRepresentation,
  CylinderGeometry,
  DataTexture,
  EdgesGeometry,
  MeshBasicMaterial,
  MeshToonMaterial,
  NearestFilter,
  RedFormat,
  SRGBColorSpace,
} from 'three';
import { LineMaterial } from 'three/addons/lines/LineMaterial.js';
import { LineSegmentsGeometry } from 'three/addons/lines/LineSegmentsGeometry.js';
import { mergeGeometries } from 'three/addons/utils/BufferGeometryUtils.js';
import { HEX_SIZE } from './hex-layout';

const TILE_HEIGHT = 0.3;

// places without a tile are flat, muted plates
const SLOT_HEIGHT = 0.05;
const SLOT_COLOUR = '#56645a';

const TILE_GAP = 0.06;

const SIDE_SHADE = 0.55;

// a hovered tile gets this much lighter (hsl lightness as seen on screen), enough to notice without changing its colour
const HIGHLIGHT_LIGHTEN = 0.07;

// cel shading: the light falls into these hard bands instead of a smooth gradient (0 = dark, 255 = fully lit)
const LIGHT_BANDS = [110, 190, 255];

// travel costs: one ink bar per point along each edge, the first one closest to the edge
const MAX_COST = 3;
const COST_BAR_LENGTH = 0.4;
const COST_BAR_WIDTH = 0.055;
const COST_BAR_HEIGHT = 0.02;
const COST_BAR_GAP = 0.05;
const COST_BAR_INSET = 0.07;

// colour of the bars for a cost of 1, 2 and 3
const COST_BAR_COLOURS = ['#9aa09b', '#e8862a', '#d0402f'];

// ink lines along the edges, in css pixels
const OUTLINE_COLOUR = '#0f1d14';
const OUTLINE_WIDTH = 2;

export class BoardResources {
  // Tiles
  readonly tileGeometry = createHexPrism(TILE_HEIGHT);

  readonly tileOutlineGeometry = createOutlineGeometry(this.tileGeometry);

  // Placeholders
  readonly slotGeometry = createHexPrism(SLOT_HEIGHT);

  readonly slotOutlineGeometry = createOutlineGeometry(this.slotGeometry);

  readonly outlineMaterial = new LineMaterial({ color: OUTLINE_COLOUR, linewidth: OUTLINE_WIDTH });

  // Travel costs
  private readonly costBarMaterials = COST_BAR_COLOURS.map(
    (color) => new MeshBasicMaterial({ color }),
  );

  private readonly costBarGeometries = createCostBarGeometries();

  private readonly lightBands = createLightBands();

  private readonly tileMaterialsByColour = new Map<string, MeshToonMaterial[]>();

  // [sides, top, bottom] of a tile with the given top colour, the material groups of the tile geometry
  tileMaterials(colour: ColorRepresentation, highlighted = false): MeshToonMaterial[] {
    const top = new Color(colour);
    const key = `${top.getHexString()}${highlighted ? '-highlighted' : ''}`;

    let materials = this.tileMaterialsByColour.get(key);

    if (!materials) {
      const side = top.clone().multiplyScalar(SIDE_SHADE);

      if (highlighted) {
        lighten(top);
        lighten(side);
      }

      materials = [this.toon(side), this.toon(top), this.toon(side)];
      this.tileMaterialsByColour.set(key, materials);
    }

    return materials;
  }

  slotMaterials(highlighted = false): MeshToonMaterial[] {
    return this.tileMaterials(SLOT_COLOUR, highlighted);
  }

  // the bars of a cost at the edge facing +z, null for a cost of 0
  costBars(cost: number): { geometry: BufferGeometry; material: MeshBasicMaterial } | null {
    if (cost <= 0) {
      return null;
    }

    const index = Math.min(cost, MAX_COST) - 1;

    return { geometry: this.costBarGeometries[index], material: this.costBarMaterials[index] };
  }

  dispose(): void {
    this.tileGeometry.dispose();
    this.tileOutlineGeometry.dispose();
    this.slotGeometry.dispose();
    this.slotOutlineGeometry.dispose();
    this.outlineMaterial.dispose();
    this.costBarMaterials.forEach((material) => material.dispose());
    this.costBarGeometries.forEach((geometry) => geometry.dispose());
    this.lightBands.dispose();

    for (const materials of this.tileMaterialsByColour.values()) {
      materials.forEach((material) => material.dispose());
    }

    this.tileMaterialsByColour.clear();
  }

  private toon(color: Color): MeshToonMaterial {
    return new MeshToonMaterial({
      color,
      gradientMap: this.lightBands,
      // pushes the faces slightly back, so the ink lines on their edges are not partly hidden by them
      polygonOffset: true,
      polygonOffsetFactor: 1,
      polygonOffsetUnits: 1,
    });
  }
}

// in srgb, so light and dark colours change by the same visible amount
function lighten(colour: Color): void {
  const { h, s, l } = colour.getHSL({ h: 0, s: 0, l: 0 }, SRGBColorSpace);
  colour.setHSL(h, s, Math.min(l + HIGHLIGHT_LIGHTEN, 1), SRGBColorSpace);
}

// flat-topped hexagon of a tile's size, standing on the ground plane
function createHexPrism(height: number): CylinderGeometry {
  const radius = HEX_SIZE - TILE_GAP;

  const geometry = new CylinderGeometry(radius, radius, height, 6);
  geometry.rotateY(Math.PI / 6);
  geometry.translate(0, height / 2, 0);

  return geometry;
}

// ink lines around the top, around the bottom and up the corners
function createOutlineGeometry(geometry: CylinderGeometry): LineSegmentsGeometry {
  const edges = new EdgesGeometry(geometry);

  // plain webgl lines are always one pixel thin, these fat lines keep their width in css pixels
  const lines = new LineSegmentsGeometry().fromEdgesGeometry(edges);
  edges.dispose();

  return lines;
}

// for each cost from 1 to MAX_COST its bars on top of a tile, parallel to the edge facing +z and stacked towards the centre
function createCostBarGeometries(): BufferGeometry[] {
  // distance from the centre of a tile to the middle of its edges
  const apothem = ((HEX_SIZE - TILE_GAP) * Math.sqrt(3)) / 2;

  return Array.from({ length: MAX_COST }, (_, index) => {
    const bars: BufferGeometry[] = [];

    for (let bar = 0; bar <= index; bar++) {
      const geometry = new BoxGeometry(COST_BAR_LENGTH, COST_BAR_HEIGHT, COST_BAR_WIDTH);
      const distance =
        apothem - COST_BAR_INSET - COST_BAR_WIDTH / 2 - bar * (COST_BAR_WIDTH + COST_BAR_GAP);
      geometry.translate(0, TILE_HEIGHT + COST_BAR_HEIGHT / 2, distance);

      bars.push(geometry);
    }

    const merged = mergeGeometries(bars);
    bars.forEach((geometry) => geometry.dispose());

    return merged;
  });
}

// a one pixel high strip with one texel per band, read without blending so the bands stay hard
function createLightBands(): DataTexture {
  const texture = new DataTexture(new Uint8Array(LIGHT_BANDS), LIGHT_BANDS.length, 1, RedFormat);
  texture.minFilter = NearestFilter;
  texture.magFilter = NearestFilter;
  texture.generateMipmaps = false;
  texture.needsUpdate = true;

  return texture;
}
