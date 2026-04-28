# Backend Internals

## Example Request Flow

The structure of the backend was described in the
corresponding [Level 2 Building Block View](../building-block-view/level2.md#tikal-backend).

This example shows how a simple read request is expected to travel through the layers of the backend application.

```mermaid
sequenceDiagram
    actor User
    participant API as Rest API
    participant Layer as Layer of Indirection
    participant Handler
    participant Infrastructure
    
    User->>API: GET /api/data
    API->>API: Validate access token
    API->>API: Build Query
    API->>Layer: Send Query
    Layer->>Handler: Route Query to responsible Handler
    Handler->>Handler: Perform any logic
    Handler->>Infrastructure: Query any data
    Infrastructure-->>Handler: data
    Handler-->>Layer: Query Result
    Layer-->>API: Query Result
    API->>API: Build Response
    API-->>User: Response
```