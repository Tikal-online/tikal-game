export type Player = {
  userId: string;
  name: string;
  selectedColour: Colour;
  isOwner: boolean;
  isReady: boolean;
  isMe: boolean;
};

export type Colour = 'red' | 'black' | 'green' | 'blue' | 'yellow';
