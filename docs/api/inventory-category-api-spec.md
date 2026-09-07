# Inventory Categories API

Base route: `api/control/inventory-categories`

All endpoints require bearer authentication and return the standard `{ success, message, data }` envelope.

## Endpoints

- `GET /?text=&pageIndex=0&pageSize=20` — paged, optionally searchable list.
- `GET /all` — unpaged lookup list.
- `GET /{id}` — retrieve one category by GUID.
- `POST /` — create a category.
- `PUT /{id}` — update a category; the body `Id` must match the route GUID.

Create/update body:

```json
{
  "Id": "00000000-0000-0000-0000-000000000000",
  "Description": "Stationery",
  "Remarks": "Pens, rulers and other office stationery",
  "IsLocked": false
}
```

`Description` and `Remarks` are required. `Id` is omitted when creating.
