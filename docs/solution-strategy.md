---
icon: lucide/lightbulb
---

# Solution Strategy

This section contains a highly compacted architecture overview. A contrast of the most important goals and approaches.

## Introduction to the Strategy

The following table contrasts the key [quality goals](Introduction&goals.md#quality-goals) with matching architecture
approaches.

| Quality Goal                                   | Matching approaches in the solution                                                                                                                                                                                                   |
|:-----------------------------------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Accessible documentation (Analysability)       | Architecture documentation following the [Arc42](https://docs.arc42.org) template.                                                                                                                                                    |
| Accessible documentation (Analysability)       | Automatic deployment of the newest version of the documentation to [Github Pages](https://docs.github.com/en/pages).                                                                                                                  |
| Accessible documentation (Analysability)       | Documentation lives right next to the code in form of [Markdown](https://en.wikipedia.org/wiki/Markdown) files. No Documentation should be written on external platforms.                                                             |
| Easy developer onboarding (Maintainability)    | The system must provide a [devcontainer](https://containers.dev/) with all needed software dependencies and necessary IDE extensions pre installed.                                                                                   |
| Easy developer onboarding (Maintainability)    | The system must provide an [Aspire](https://aspire.dev/get-started/what-is-aspire/) project for the orchestration of the local development environment. The built-in Dashboard will also act as an OTEL collector during development. |
| Easy developer onboarding (Maintainability)    | All individual parts of the system as well as dependencies must be available as a Docker container, to be able to easily spin up/down the whole system with one command.                                                              |
| Handle network issues gracefully (Reliability) | Communication between the client and the server will use [Websockets](https://en.wikipedia.org/wiki/WebSocket) to instantly detect a player disconnecting.                                                                            |
| Handle network issues gracefully (Reliability) | The system must keep track of game state and maintain play sessions for a certain amount of time after a player disconnects. If the player reconnects in time the game should be restored from the saved state and continue.          |
| Cheap to host (Efficiency)                     | The system should be deployed on minimal resources with all auto scaling disabled. The potential performance degradation under load is accepted.                                                                                      |
| Cheap to host (Efficiency)                     | The system should be built with minimal resource consumption in mind and there should be tests tracking the performance of the system and the corresponding delta of changes.                                                         |

## Top Level System Decomposition

The system can be roughly split into the following parts:

- A web frontend through which users will interact with the system
- The main backend which publishes all needed functionality to play the game
- An identity server which will manage local user accounts and interact with 3rd party identity providers
- A BFF host whose main purpose is to ensure that no sensitive access tokens are accessible by the web frontend
- An OTEL Collector which will centralize OTEL configuration and can be pointed at any OTEL aggregator

!!! notes

    For high level structural details about the system refer to the [Level 1 Building Block View](building-block-view/level1.md#overview).

The backend acts as an authoritative game server which tracks the game state and ensures the validity of any submitted
moves. Player clients will not communicate directly with each other. All communication is facilitated by the backend.

The BFF host will sit between the web frontend and the backend / identity server. It will implement all needed auth
flows and be responsible for providing the access token used to authenticate requests from the frontend to the backend.

??? info "Click to learn more about BFF"

    The BFF host is supposed to implement the [BFF security framework](https://docs.duendesoftware.com/bff/) from Duende. 
    It acts as a security proxy between the frontend and the backend and is responsible for all OAuth and OIDC protocol 
    interactions. See [here](https://docs.duendesoftware.com/bff/) if you want to learn more about it.

## Technology Decisions

| System Part               | Chosen Technology | Motivation/Reason                                                      |
|:--------------------------|:------------------|:-----------------------------------------------------------------------|
| Backend Services          | ASP.NET Core      | see [Constraints](constraints.md#technical-constraints)                |
| Databases                 | Postgres          | [Just use Postgres](https://mccue.dev/pages/8-16-24-just-use-postgres) |
| Web Frontend              | Angular           | I am interested in learning Angular in the scope of this project.      |
| Identity Management       | Duende Solutions  | see [Constraints](constraints.md#technical-constraints)                |
| Deployment Infrastructure | Azure Cloud       | see [Constraints](constraints.md#technical-constraints)                |
