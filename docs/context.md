---
icon: lucide/map
---

# Context

This section describes the environment of the Tikal project. Who is using it, and with what other systems it interacts
with.

## Business Context

![business context](images/business_context.svg)

### Player

Tikal is played between 2-4 people, who perform actions in turns. The players communicate via an authoritative
game server with each other. For this purpose they need to communicate with the system, e.g. about their own and their
opponents moves.

### Administrator

There will be the need to restrict misbehaving players and manage active lobbies and games. This will be the tasks of
the administrator. For this purpose they need to communicate with the system.

### 3rd Party Identity Providers

Players should not be forced to create a separate account for this game. They should be able to use existing accounts
for common identity providers. For this purpose the system needs to communicate with these 3rd party providers.

### Telemetry Aggregator

The system will send all telemetry to a telemetry aggregator which provides dashboards and user interfaces to query the
data.

## Technical Context

![technical context](images/technical_context.svg)

### 3rd Party Identity Providers

The system will communicate with 3rd party identity providers using [OIDC](https://en.wikipedia.org/wiki/OpenID) to
manage the identity of users who do not wish to create an account specifically for Tikal.

### Telemetry Aggregator

The system will send out all telemetry (logs, traces and metrics) using OTLP/gRPC according to
the [OTLP Specification](https://opentelemetry.io/docs/specs/otlp/#otlpgrpc).