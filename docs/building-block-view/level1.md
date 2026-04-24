# Level 1

![system](images/system_level1.svg)

| Subsystem       | Description                                                                                                                                      |
|:----------------|:-------------------------------------------------------------------------------------------------------------------------------------------------|
| Web Frontend    | Hosts the static assets of the web frontend.                                                                                                     |
| BFF             | Sits between the frontend and any other backend components. It implements all needed authentication flows and is the entrypoint into the system. |
| Identity Server | Self hosted identity server to manage local accounts. For users who do not want to create a local account it communicates with 3rd party IDPs.   |
| Tikal Backend   | The main backend of the game. Can only be accessed through the BFF and needs access tokens issued by the identity server.                        |
| OTEL Collector  | Collects telemetry from all parts of the system and exports them to a telemetry aggregator                                                       |
