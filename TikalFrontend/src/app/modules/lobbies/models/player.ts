import { Colour } from '../../../core/models/colour';

export type Player = {
  userId: string;
  name: string;
  selectedColour: Colour;
  isOwner: boolean;
  isReady: boolean;
  isMe: boolean;
};
