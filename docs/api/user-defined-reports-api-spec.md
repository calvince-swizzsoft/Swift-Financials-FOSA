# User-Defined Reports API

Secure replacement for the legacy `SSRSReportsController` and
`SSRSReportSettingController`.

## Responsibilities

SwiftFinancials owns:

- categorized report catalogue metadata;
- the governed source `.rdl` file used for recovery/audit;
- active/inactive visibility;
- permission checks; and
- construction of the configured SSRS viewer URL.

SSRS owns publication, data-source credentials, parameter prompting, report
execution, paging, rendering, and PDF/Excel/Word exports. Uploading an RDL to
the SwiftFinancials catalogue deliberately does not claim to deploy it to
SSRS.

## Configuration

```xml
<add key="Ssrs:ViewerBaseUrl" value="https://reports.example.org/Reports/report" />
<add key="Ssrs:MaxRdlBytes" value="5242880" />
```

The viewer URL must be an absolute HTTP(S) URL. Credentials are never stored
in these settings or returned by the API. Configure authentication on SSRS
(preferably Windows/integrated authentication or an approved reverse proxy).

## Database

Apply [`../database/install-user-defined-reports.sql`](../database/install-user-defined-reports.sql).
It creates `swiftFin_UserDefinedReportCategories` and
`swiftFin_UserDefinedReports` idempotently.

## Permissions

- `UserDefinedReportViewing`: browse and launch active reports.
- `UserDefinedReportAdministration`: includes browse plus category and report
  catalogue management.

Both permissions are assignable through the existing role-permission UI.

## Endpoints

```http
GET    /api/reports/user-defined
GET    /api/reports/user-defined/categories
GET    /api/reports/user-defined/{id}/view
POST   /api/reports/user-defined/categories
POST   /api/reports/user-defined                 multipart/form-data
PUT    /api/reports/user-defined/{id}
DELETE /api/reports/user-defined/{id}
GET    /api/reports/user-defined/{id}/rdl
```

Report upload fields are `name`, `description`, `categoryId`, `reportPath`,
and one `.rdl` file. The API limits size, requires the `.rdl` extension,
parses XML with DTD/external resolution disabled, and requires an XML root
named `Report`. Duplicate display names and report paths return HTTP `409`.

The view endpoint constructs a URL exclusively from the configured base and
the stored relative report path. It does not accept an arbitrary URL from the
browser, preventing the catalogue from becoming an open redirect.

