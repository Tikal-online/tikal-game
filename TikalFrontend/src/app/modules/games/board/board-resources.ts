import {
  Color,
  ColorRepresentation,
  CylinderGeometry,
  DataTexture,
  EdgesGeometry,
  MeshToonMaterial,
  NearestFilter,
  RedFormat,
} from 'three';
import { LineMaterial } from 'three/addons/lines/LineMaterial.js';
import { LineSegmentsGeometry } from 'three/addons/lines/LineSegmentsGeometry.js';
import { HEX_SIZE } from './hex-layout';

export const TILE_HEIGHT = 0.3;

// places without a tile are flat, muted plates
const SLOT_HEIGHT = 0.05;
const SLOT_COLOUR = '#56645a';

const TILE_GAP = 0.06;

const SIDE_SHADE = 0.55;

// cel shading: the light falls into these hard bands instead of a smooth gradient (0 = dark, 255 = fully lit)
const LIGHT_BANDS = [110, 190, 255];

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

  private readonly lightBands = createLightBands();

  private readonly tileMaterialsByColour = new Map<string, MeshToonMaterial[]>();

  readonly slotMaterials = this.tileMaterials(SLOT_COLOUR);

  // [sides, top, bottom] of a tile with the given top colour, the material groups of the tile geometry
  tileMaterials(colour: ColorRepresentation): MeshToonMaterial[] {
    const top = new Color(colour);
    const key = top.getHexString();

    let materials = this.tileMaterialsByColour.get(key);

    if (!materials) {
      const side = top.clone().multiplyScalar(SIDE_SHADE);
      materials = [this.toon(side), this.toon(top), this.toon(side)];
      this.tileMaterialsByColour.set(key, materials);
    }

    return materials;
  }

  dispose(): void {
    this.tileGeometry.dispose();
    this.tileOutlineGeometry.dispose();
    this.slotGeometry.dispose();
    this.slotOutlineGeometry.dispose();
    this.outlineMaterial.dispose();
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

// a one pixel high strip with one texel per band, read without blending so the bands stay hard
function createLightBands(): DataTexture {
  const texture = new DataTexture(new Uint8Array(LIGHT_BANDS), LIGHT_BANDS.length, 1, RedFormat);
  texture.minFilter = NearestFilter;
  texture.magFilter = NearestFilter;
  texture.generateMipmaps = false;
  texture.needsUpdate = true;

  return texture;
}
