import { rotateCosts, TravelCost } from './tile';

const COSTS: TravelCost = {
  north: 1,
  northEast: 2,
  southEast: 3,
  south: 0,
  southWest: 0,
  northWest: 0,
};

describe('rotateCosts', () => {
  test('given one clockwise step when rotateCosts then moves every cost to the next edge clockwise', () => {
    // when
    const rotated = rotateCosts(COSTS, 1);

    // then
    expect(rotated).toEqual({
      north: 0,
      northEast: 1,
      southEast: 2,
      south: 3,
      southWest: 0,
      northWest: 0,
    });
  });

  test('given one counter-clockwise step when rotateCosts then moves every cost to the next edge counter-clockwise', () => {
    // when
    const rotated = rotateCosts(COSTS, -1);

    // then
    expect(rotated).toEqual({
      north: 2,
      northEast: 3,
      southEast: 0,
      south: 0,
      southWest: 0,
      northWest: 1,
    });
  });

  test.for([0, 6, -6, 12])(
    'given %i steps when rotateCosts then returns the same costs',
    (steps: number) => {
      // when
      const rotated = rotateCosts(COSTS, steps);

      // then
      expect(rotated).toEqual(COSTS);
    },
  );

  test.for([
    [2, -4],
    [5, -1],
    [7, 1],
  ])('given %i steps when rotateCosts then equals %i steps', ([steps, equivalent]) => {
    // when & then
    expect(rotateCosts(COSTS, steps)).toEqual(rotateCosts(COSTS, equivalent));
  });
});
