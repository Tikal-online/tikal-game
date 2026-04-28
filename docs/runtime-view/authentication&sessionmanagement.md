# Authentication and Session Management

The BFF creates a user session upon successful authentication. The authentication process flows through several layers.
This section aims to give a general understanding how these layers connect and interact with each other.

## Login

```mermaid
sequenceDiagram
    participant Browser
    participant BFF
    participant IS as Identity Server
    
    Browser->>BFF: GET /bff/login
    BFF->>IS: OIDC Authorization Request
    IS-->>Browser: Login UI
    Browser->>IS: Credentials
    IS-->>BFF: Authorization Code
    BFF->>IS: Token Request
    IS-->>BFF: Access, Refresh and ID Token
    BFF->>BFF: Establish session and store tokens
    BFF-->>Browser: Set-Cookie (session ID)
```

The BFF sets a **HttpOnly, Secure, SameSite** cookie in the browser. This cookie contains the session ID and is sent
automatically with each subsequent request. The cookie is signed and encrypted
using [ASP.NET Core's Data Protection](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-10.0).

The browser never has access to any access or refresh tokens. They are stored on the BFF.

## Calling a protected API

```mermaid
sequenceDiagram
    participant Browser
    participant BFF
    participant API
    
    Browser->>BFF: GET /api/data (with cookie)
    BFF->>BFF: Validate Session
    BFF->>BFF: Get access token
    BFF->>API: GET mapped endpoint (with auth header)
    API->>API: Validate access token
    API-->>BFF: Requested Data
    BFF-->>Browser: Requested Data
```

The BFF retrieves the access token from the session and attaches it to the API call. If the access token is expired or
close to expiring it will refresh it using the refresh token.