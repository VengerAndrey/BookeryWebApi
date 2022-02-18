# Bookery WebAPI

### Idea

**Bookery** is a multiplatform app that in general stands for a cloud drive. It uses hierarchical storage, supports basic operations like upload/download/rename/delete, and supports sharing. It is possible to share and hide nodes (folders or files) recursively.

### Module info

This is the obsolete monolith main gateway of the whole app. It serves all client applications through a consistent API. The module is split into domain, data, and API layers. Authorization of clients is required.

Preferably to use [`Bookery`](https://github.com/VengerAndrey/Bookery) microservices.

### Tech stack

- **.NET Core 5.0**, **ASP .NET Core**
- **EntityFramework Core** + MSSQLServer
- Authentication through **JWT**
- *Blob storage* is emulated locally
