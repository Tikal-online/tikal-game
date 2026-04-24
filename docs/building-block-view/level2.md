# Level 2

## Tikal Backend

The backend follows the Modular Monolith architecture.

Every module is a closed system independent of other parts of the application and publishes an API of queries
and commands. These queries and commands are the only public part referenced by other components.

If a module is interested in calling a functionality in another module it throws the corresponding query or command in
the layer of indirection which then routes it to the responsible handler.

![backend](images/backend_level2.svg)

| Subsystem            | Description                                                                              |
|:---------------------|:-----------------------------------------------------------------------------------------|
| REST API             | Publishes the functionality of the system to the outside world as a restful API.         |
| Layer of indirection | Responsible for routing queries and commands to the responsible handlers across modules. |
| Accounts Module      | The module responsible for player accounts and related information                       |

## BFF and Web Frontend

The Web Frontend is hosted outside the BFF. The browser accesses the application under the domain of the frontend.
It then calls the BFF which is hosted on a subdomain of the frontend domain.

This has the downside of being more complicated to set up, but allows separate deployment and development of these two
components.

!!! notes

    To learn more about different UI Hosting options using the BFF Security Pattern see the [Official Duende docs](https://docs.duendesoftware.com/bff/architecture/ui-hosting/).

![web&bff](images/web_bff_level2.svg)
