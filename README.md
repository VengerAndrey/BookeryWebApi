# Bookery WebAPI

### Idea

**Bookery** --- is a multiplatform application that in general stands for a cloud drive. It uses a hierarchical storage, supports basic operations like upload/download/rename/delete and supports *sharing*. It is possible to share and hide nodes (folders or files) recursively.

### Module info

This is the main gateway of the whole infrastructure. It serves all client applications through a consistent API. The module is splitted into domain, data and API layers. Authorization of clients is required.

### Tech stack

- **.NET Core 5.0**, **ASP .NET Core**
- **EntityFramework Core** + MSSQLServer
- Authorization through **JWT**
- *Blob storage* is emulated locally
