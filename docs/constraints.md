---
icon: lucide/construction
---

# Constraints

This section represents various restrictions that have to be respected within the design of the system and explains
their motivations where necessary.

## Technical Constraints

| Constraint                                                                                                 | Background/Motivation                                                                                                                                                                       |
|:-----------------------------------------------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Backend services are written using [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)          | Since I am mainly a .NET dev and I am familiar with this technology.                                                                                                                        |
| Game is accessible as a web application                                                                    | To make the game more accessible it should be possible to be played without downloading an executable.                                                                                      |
| Authentication uses [Duende Identity solutions](https://duendesoftware.com/)                               | Reimplementing authentication is tedious and unnecessary. Duende integrates well with ASP.NET Core and is free to use for non commercial projects.                                          |
| Deployed in the [Azure Cloud](https://en.wikipedia.org/wiki/Microsoft_Azure)                               | Properly self hosting a complete software stack is its own project which I am not interested in tackling in the scope of this project. I already have limited experience with IaC on Azure. |
| Uses the [OpenTelemetry](https://opentelemetry.io/) framework for any telemetry (logs, traces and metrics) | This will standardise telemetry among all applications and tech stacks and allow integration into basically every telemetry aggregator.                                                     |

## Organizational Constraints

| Constraint                                                              | Background/Motivation                                                                                                                              |
|:------------------------------------------------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------|
| Open Source                                                             | Code is published as open source to be more accessible for other people interested in the project.                                                 |
| Code is hosted as a [Monorepo](https://en.wikipedia.org/wiki/Monorepo). | Since this project will span multiple applications, this will make pull requests more reviewable and deployment workflows and secrets centralized. |
| Code is hosted on [Github](https://github.com/)                         | I am familiar with [Github actions](https://github.com/features/actions) and Github integrates well with Azure.                                    |
