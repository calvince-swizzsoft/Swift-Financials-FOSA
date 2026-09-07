# Unit of Measure API

Base route: `api/control/unitsofmeasurement`

All endpoints require bearer authentication and return the standard `{ success, message, data }` envelope.

- `GET /?text=&pageIndex=0&pageSize=20` — paged, optionally searchable list.
- `GET /all` — unpaged lookup list.
- `GET /{id}` — retrieve one unit by GUID.
- `POST /` — create a unit.
- `PUT /{id}` — update a unit; body and route IDs must match.

```json
{
  "Id": "00000000-0000-0000-0000-000000000000",
  "Name": "Dozen",
  "Contains": 12,
  "BaseUnitId": "00000000-0000-0000-0000-000000000000"
}
```

`Name` is required. `Contains` and `BaseUnitId` are supplied together for a composite unit, or both left null for a base unit. A unit cannot refer to itself.
