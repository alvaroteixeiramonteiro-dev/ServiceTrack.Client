# ServiceTrack.Client

Windows Forms client for ServiceTrack.API, developed as part of UC00614 — Integrar Sistemas de Informação.

The application manages service requests through HTTP and JSON, following the structure of the API client developed in class.

## Technologies

- C#
- Windows Forms — .NET Framework 4.7.2
- HttpClient
- System.Net.Http.Json

## Features

- Load and display service requests in a DataGridView.
- Select a request to populate the editing fields.
- Create requests.
- Update the selected request.
- Delete requests with confirmation.
- Clear the form.
- Reload data from the API.
- Display validation and communication error messages.

## Project structure

- `principal.cs` — main MDI window and navigation.
- `FormPedidos.cs` — request management interface and events.
- `Models/Pedido.cs` — request data model.
- `Services/ApiService.cs` — HTTP communication with the API.
- `Program.cs` — application entry point.

## Requirements

- Windows
- Visual Studio with the .NET desktop development workload
- .NET Framework 4.7.2 targeting pack
- ServiceTrack.API running with access to its MySQL database

The client does not connect directly to MySQL. Database access is handled by the API.

## Configuration

The API address is configured in `Services/ApiService.cs`:

```csharp
BaseAddress = new Uri("https://localhost:7242/")
```

Update this address if the API uses another port or host.

The HTTPS certificate used by the local API must be trusted on the computer running the client. Certificate validation is not disabled.

## Running the application

1. Start MySQL.
2. Configure and run ServiceTrack.API using its HTTPS profile.
3. Open `ServiceTrack.Client.slnx` in Visual Studio.
4. Restore the NuGet packages.
5. Build and run the client.
6. Open **Pedidos → Gerir pedidos**.

The application loads the requests when the management form opens.

## Using the client

| Action | Behaviour |
|---|---|
| Select a grid row | Copies the request data into the editing fields |
| Limpar | Clears the fields and resets priority and status |
| Guardar | Creates a new request |
| Atualizar | Updates the selected request |
| Eliminar | Requests confirmation before deleting the selected request |
| Recarregar | Clears the form and retrieves the latest data |

**Guardar always creates a new record.** Use **Atualizar** to save changes to an existing request.

The API generates IDs and manages creation, update and resolution dates.

## API operations

| Method | Route | Client operation |
|---|---|---|
| GET | `/api/Pedidos` | Load and reload |
| POST | `/api/Pedidos` | Create |
| PUT | `/api/Pedidos/{id}` | Update |
| DELETE | `/api/Pedidos/{id}` | Delete |

## Input validation

The client checks required fields before creating or updating requests.

Priority options:

- `Baixa`
- `Normal`
- `Alta`

Status options:

- `Aberto`
- `Em curso`
- `Resolvido`

Additional validation is performed by the API. Email validation currently checks presence and length, not address format.

## Manual verification

The following scenarios were tested during development:

- Loading existing records from the API.
- Selecting a row and populating the editing fields.
- Clearing the form.
- Creating and updating requests.
- Deleting a request with confirmation.
- Reloading records created through Swagger.

## Scope

This is an educational portfolio application with a separate API and client solution.

The API uses one MySQL table. The client retains the simple Windows Forms structure used in class, with no additional architectural layers.

Authentication is not implemented. The generated MDI template also contains menu items that are not part of request management.
