import { Box3, Vector3 } from 'three';

/**
 * Distance from `target` along `direction` (pointing from the target towards the camera) at which every point
 * is in view of a perspective camera looking at the target.
 * `margin` > 1 leaves space around the points, e.g. 1.1 keeps them within 1 / 1.1 of the view.
 */
export function fitDistance(
  points: readonly Vector3[],
  target: Vector3,
  direction: Vector3,
  verticalFovRadians: number,
  aspect: number,
  margin: number,
): number {
  // directions of the camera's view: into the scene, to the right and up on the screen
  const forward = direction.clone().normalize().negate();
  const right = new Vector3().crossVectors(forward, new Vector3(0, 1, 0)).normalize();
  const up = new Vector3().crossVectors(right, forward);

  // how far the view spreads per unit of depth, vertically and horizontally
  const tanVertical = Math.tan(verticalFovRadians / 2) / margin;
  const tanHorizontal = tanVertical * aspect;

  // a point fits once its depth in front of the camera covers its offset from the view's centre,
  // its depth is the camera's distance plus how far the point lies along the view
  let distance = 0;

  for (const point of points) {
    const relative = point.clone().sub(target);
    const depthOffset = relative.dot(forward);

    distance = Math.max(
      distance,
      Math.abs(relative.dot(right)) / tanHorizontal - depthOffset,
      Math.abs(relative.dot(up)) / tanVertical - depthOffset,
    );
  }

  return distance;
}

export function boxCorners({ min, max }: Box3): Vector3[] {
  return [min.x, max.x].flatMap((x) =>
    [min.y, max.y].flatMap((y) => [min.z, max.z].map((z) => new Vector3(x, y, z))),
  );
}
