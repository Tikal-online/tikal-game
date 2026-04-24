---
icon: lucide/rocket
---

# Introduction & Goals

[Tikal](https://en.wikipedia.org/wiki/Tikal_(board_game)) is a
[German-style](https://en.wikipedia.org/wiki/Eurogame) board game released in 1999.

The gameplay is turn-based, with the victor decided by victory points which can be achieved by finding artifacts,
excavating, and maintaining control over temple sites. The theme of the game is that of adventurers exploring parts of a
Central American jungle in which artifacts and temples are discovered.

The main goal of this project is to create an accessible free online multiplayer version of the game.

## Essential Features

Players must be able to

* [x] create and manage their accounts and view statistics related to their gameplay
* [ ] create private and public lobbies with customizable game options and in-game chat
* [ ] start a lobby with enough players to play a game of Tikal

The game must

* [ ] follow the official Tikal rules
* [ ] allow playing in simple and advanced mode

## Quality Goals

The following table describes the key quality objectives of the software architecture. The order of the goals gives you
a rough idea of their importance.

| Quality Goal                                   | Motivation/Description                                                                                                                                               |
|:-----------------------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Accessible documentation (Analysability)       | The main purpose of this project is to act as a learning experience to create a well documented and easily understandable software system of non trivial complexity. |
| Easy developer onboarding (Maintainability)    | Setting up the complete development environment should contain as few manual steps as possible and allow others to run and debug the system in minutes.              |
| Handle network issues gracefully (Reliability) | Because this is an online game it should not only be able to function under perfect conditions, but also handle slow internet speeds, disconnects and reconnects.    |
| Cheap to host (Efficiency)                     | Since this project doesnt aim to make any money the hosting costs should be minimized because I will be paying them myself.                                          |
