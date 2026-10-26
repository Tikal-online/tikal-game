import { Colour } from '../../../core/models/colour';

export type Player = {
  userId: string;
  name: string;
  colour: Colour;
  points: number;
};
