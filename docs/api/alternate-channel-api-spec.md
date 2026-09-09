# Alternate Channels API

Base route: `api/accounts/alternatechannels`. Authentication is required.

## Register and Management listings

`GET /paged?text=&filter=0&pageIndex=0&pageSize=20`

Queries `IAlternateChannelAppService.FindAlternateChannels(text, filter, pageIndex, pageSize, serviceHeader)`.
`text` defaults to empty; `filter` defaults to `0` (primary account number) and must be a defined `AlternateChannelFilter` value.
`pageIndex` is zero-based and non-negative; `pageSize` defaults to 20 and must be between 1 and 100.
Invalid pagination or filter returns HTTP 400 with `{ success: false, message, data: null }`.
Success returns `{ success: true, message: "", data: { PageCollection: [...], ItemsCount: number } }`;
`data` may be null if the application service returns no page.
Both frontend Register and Management use this endpoint. It lists all record statuses; Register performs its existing pending-status filtering in the frontend.

Implemented by `WebApplication1/Areas/Accounts/Controllers/AlternateChannelController.cs`.

## Existing fees endpoints

`AlternateChannelChargeController` separately implements:

- `GET /charge-options`
- `GET /types/{type}/commissions?knownChargeType=...`
- `PUT /types/{type}/commissions`

## Remaining integration gaps

The frontend also declares single-record, account-specific and type/status queries, linking, update, replace, renew, stop, delink, approve and reject routes. These are not implemented by the current controllers. The list endpoint fix does not implement these actions or introduce maker-checker enforcement.

## Verification and deployment

The WebApplication1 Debug build passed after adding the controller and its explicit Compile entry to the project. An authenticated database-backed list request remains to be verified after deployment.
On 2026-09-09 the configured production API returned HTTP 404 for `/paged` while `/charge-options` returned HTTP 401 without a token, confirming the missing listing route on the deployed server.
Deploy the rebuilt backend for the list fix to reach the hosted frontend.
