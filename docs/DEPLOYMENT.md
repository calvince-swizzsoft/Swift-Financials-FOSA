# Deployment

Deploying the SwiftFinancialz backend (`WebApplication1`) and the
[Swizzfinancial-FOSA](https://github.com/calvince-swizzsoft/reactfosa) frontend
together onto a Windows Server, driven over RDP. Both apps are deployed as
plain IIS sites — no containers, no Core runtime. Follow the phases in order;
each assumes the last is done.

Replace `YOUR-SERVER-ADDRESS`, `YOUR-FRONTEND-ADDRESS`, and any port/credential
placeholders below with your actual target values — none of that belongs
hardcoded into either repo.

**Before you start:**
- RDP access to the target Windows Server, with local admin rights
- SQL Server reachable from that server (installed locally, or a connection
  string to an existing instance)
- A way to move files onto the server — RDP clipboard/drive redirection, a
  network share, or a download link
- The address the frontend will use to reach the backend once deployed
  (hostname/IP + port)

## 00 — Windows features on the target

Run on the fresh server before anything else, via **Server Manager → Add
Roles and Features**, or in one pass with PowerShell:

```powershell
Install-WindowsFeature -Name Web-Server, Web-Asp-Net45, Web-Net-Ext45, Web-ISAPI-Ext, Web-ISAPI-Filter -IncludeManagementTools
```

The backend is classic ASP.NET (.NET Framework 4.7.2, not .NET Core), so it
needs the `ASP.NET 4.8` role feature, not "ASP.NET Core". The **URL Rewrite
Module** isn't a Windows Feature — it's a separate download
([iis.net/downloads/microsoft/url-rewrite](https://www.iis.net/downloads/microsoft/url-rewrite)),
needed for SPA client-side routing on the frontend site.

If the server doesn't already have SQL Server, install SQL Server Express
(free, fine for this) or point Phase 03 at an existing instance elsewhere on
your network.

## 01 — Build both apps locally

Do this on your own machine, where the source lives.

**Backend — Release build** (produces a self-contained `bin\` — every
referenced project's DLL lands there too):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\amd64\MSBuild.exe" `
  "WebApplication1\WebApplication1.csproj" `
  /t:Build /p:Configuration=Release /m
```

**Database seeding tool — Release build.** `SwiftFinancials.Utility.exe` is a
separate console app that creates/migrates the three databases and seeds
reference data (navigation menu, enumerations). Run it once against the
target's SQL Server in Phase 03.

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\amd64\MSBuild.exe" `
  "SwiftFinancials.Utility\SwiftFinancials.Utility.csproj" `
  /t:Build /p:Configuration=Release /m
```

**Frontend — production build.** See the frontend repo's own
`docs/DEPLOYMENT.md` — Vite bakes the API URL into the build at compile time,
so `.env.production` has to be set to wherever this backend will actually
live *before* running `npm run build`.

## 02 — Move the build output onto the server

RDP gives you clipboard file copy and drive redirection (your local drives
show up under This PC in the session) — either works. A zipped transfer is
usually faster than many small files.

| From (local) | To (server) |
|---|---|
| `WebApplication1\bin\`, `Content\`, `Scripts\`, `Views\`, `Areas\HelpPage\Views\`, `Areas\HelpPage\HelpPage.css`, `App_Data\`, `Global.asax`, `Web.config`, `favicon.ico` | `C:\inetpub\wwwroot\fosa-api\` (keep the `Areas\HelpPage\...` subpath) |
| `Swizzfinancial-FOSA\dist\*` | `C:\inetpub\wwwroot\fosa-app\` |
| `SwiftFinancials.Utility\bin\Release\*` | anywhere temporary — run it once, then delete |

Everything else in the backend project folder (`Controllers\`, `Areas\*.cs`,
`Models\`, `Helpers\`, the `.csproj` itself) is source already compiled into
`bin\` — it doesn't need to go to the server. The one exception is
`Areas\HelpPage\Views\`: Razor views aren't compiled into the DLL, they're
read from disk at request time. No other Area ships Razor views (the rest are
pure API controllers), so that's the only `Areas\*` subfolder that needs
copying.

## 03 — Point at real databases

**Install SQL Server with SQL authentication enabled.** If SQL Server is
going on the same box as the app, it needs **Mixed Mode (SQL Server and
Windows Authentication)** enabled during setup, with a real `sa` password set
— that's a choice made once during install, not a default. Windows
Authentication–only is the more common wizard default, so don't click past
that screen without checking it.

Update the connection strings in `fosa-api\Web.config`
(`AuthStore`, `BLOBStore`, `SwiftFin_Dev`) and `SwiftFinancials.Utility`'s own
`App.config` (`AuthStore`, `SwiftFin_Dev`) to match — both need editing
together, they aren't shared. If SQL Server runs on the same machine as the
app, `Data Source=(local)` resolves correctly once these files live on the
target server; if it's elsewhere, put the real server name/IP instead.

**Create and seed the databases.** Safe to run more than once — creates all
three databases if they don't exist, applies migrations, seeds the navigation
menu and enumeration reference data, and (see below) seeds a bootstrap login.

```powershell
SwiftFinancials.Utility.exe Production
```

> If it prints `"Each Enum value must have a description attribute set"`,
> that's a real assertion checking every enum in the codebase has a
> `[Description]` — rebuild from Phase 01 against current `main` if you hit
> this (fixed as of this writing).

### The database starts with zero users — this is expected, and self-healing

A brand-new `AuthStore` has no rows in `AspNetUsers`. Every path that could
create one (the Users admin screen, `POST /api/administration/users`) itself
requires an authenticated JWT, so a freshly deployed instance has no way to
log in at all unless something seeds the first account outside that cycle.

`SwiftFinancials.Utility.exe` does this automatically as its last step,
`SeedDefaultAdministrator`, so **every deployment is login-ready the moment
this tool finishes** — nothing extra to remember:

- Ensures an `Administrator` role exists, and syncs it to hold a grant for
  **every** navigation item currently in `NavigationMenu.cs` — on first run
  and on every later re-run, so modules added after go-live are automatically
  visible to it too without a manual re-grant.
- Only while `AspNetUsers` still has zero rows (i.e. only the very first time
  this runs against a given database), creates one `admin` account in that
  role with a random 16-character password, and prints it to the console
  **once**:
  ```
  ==================================================================
   First-run administrator account created:
     Username: admin
     Password: <random, printed once>
   You will be required to change this password on first login.
   This is printed once and is not recoverable - record it now.
  ==================================================================
  ```
  Capture that password from the console before closing the window — it
  isn't stored anywhere in plaintext, and re-running the tool won't reprint
  it once a user exists.
- Signing in with it returns `requiresPasswordChange: true` (no JWT) — the
  same first-login flow every other user goes through, documented in
  [`USER-ONBOARDING-AND-FIRST-LOGIN.md`](USER-ONBOARDING-AND-FIRST-LOGIN.md).
  Change the password, then use this account to create real named
  administrator/employee accounts through the normal Users screen — it's a
  bootstrap identity, not meant for daily use afterward.

If you already deployed before this step existed and are stuck with no way
to log in, just re-run `SwiftFinancials.Utility.exe Production` with a build
that includes it — `AddNavigationItemsAsync` and `SeedEnumerations` are
already safe to re-run (they find-or-create rather than insert blindly), and
`SeedDefaultAdministrator` will detect the empty `AspNetUsers` table and seed
the account then.

## 04 — Backend site in IIS

Open IIS Manager (`inetmgr`) on the server.

1. **Create the app pool.** Right-click **Application Pools → Add
   Application Pool**. Name it (e.g. `fosa-api`), .NET CLR version **v4.0**,
   Managed pipeline mode **Integrated**.
2. **Create the site.** Right-click **Sites → Add Website**. Physical path
   `C:\inetpub\wwwroot\fosa-api`, the app pool above, pick a port.
3. **Turn off debug mode.** In the deployed `Web.config`, set
   `<compilation debug="true" ...>` to `false`. Leaving it on in production
   is slower and leaks stack traces to callers.
4. **Smoke-test it** from the server itself first (rules out a firewall
   issue before chasing a code issue):
   ```powershell
   Invoke-WebRequest http://localhost:PORT/ -UseBasicParsing
   ```
   Then from your own machine with the server's real address — if that hangs
   but localhost worked, it's the Windows Firewall (add an inbound rule for
   the port).

## 05 — Frontend site in IIS

1. **Create a second site.** New app pool (.NET CLR version can be **No
   Managed Code**, it only serves static files), physical path
   `C:\inetpub\wwwroot\fosa-app`, a different port than the backend.
2. **Confirm URL Rewrite is active** — look for a **URL Rewrite** icon in the
   site's features view in IIS Manager. If it's missing, the module from
   Phase 00 didn't install; without it, refreshing on any route other than
   the home page returns a bare IIS 404 instead of the app.
3. **Open this port in the firewall too.** This is a client-side SPA — a
   visitor's browser loads the page from this port, then calls the backend
   API on *its* port directly (no server-side proxy in between). Both ports
   need their own inbound firewall rule; opening only one is a common miss
   (the frontend loads fine on the open port, then every API call silently
   hangs or fails from a different machine, since it needs the *other* port
   open too):
   ```powershell
   New-NetFirewallRule -DisplayName "FOSA Frontend" -Direction Inbound -Protocol TCP -LocalPort PORT -Action Allow
   New-NetFirewallRule -DisplayName "FOSA Backend API" -Direction Inbound -Protocol TCP -LocalPort PORT -Action Allow
   ```

## 06 — Connect the two, then verify

The backend's CORS policy reads the allowed origin from config
(`AllowedCorsOrigins`), not a hardcoded value — it still needs the real
frontend address filled in.

`fosa-api\Web.config` — `<appSettings>`:

```xml
<add key="AllowedCorsOrigins" value="http://YOUR-FRONTEND-ADDRESS:PORT" />
<add key="Frontend:LoginUrl" value="http://YOUR-FRONTEND-ADDRESS:PORT/login" />
```

Multiple origins are comma-separated in `AllowedCorsOrigins`.

If the frontend's `.env.production` guess (Phase 01) didn't match where the
backend actually ended up, update it and rebuild, then re-copy `dist/` over
(Phase 02).

Finally, open the frontend's URL in a browser, log in, and confirm a real API
call succeeds. A CORS misconfiguration shows up in the browser console as a
blocked cross-origin request, not a login failure — check there first if the
page loads but nothing populates.

## 07 — If something breaks

<details>
<summary>Frontend loads, but every API call fails / console shows a CORS error</summary>

The literal browser message is usually: *"has been blocked by CORS policy:
Response to preflight request doesn't pass access control check: No
'Access-Control-Allow-Origin' header is present on the requested resource."*
Almost always `AllowedCorsOrigins` in the backend's `Web.config` doesn't
exactly match the frontend's actual origin — including the port, and http vs
https. A freshly staged `Web.config` still carries the local-dev default
(`http://localhost:5173`) until Phase 06 is done, so this is expected the
first time you load a real deployment, not a sign anything is broken. Set it
to the frontend's real origin, exactly (no trailing slash), then recycle the
backend's app pool (IIS Manager → Application Pools → right-click → Recycle)
so the change takes effect.
</details>

<details>
<summary>Refreshing any page other than the home page shows a plain IIS 404</summary>

The URL Rewrite Module isn't installed, or the site's `web.config` didn't
make it into `dist/`. Confirm `C:\inetpub\wwwroot\fosa-app\web.config` exists
on the server — if not, rebuild the frontend and copy the whole `dist/`
folder again.
</details>

<details>
<summary>Backend returns HTTP 500 for everything</summary>

Temporarily set `debug="true"` back in `Web.config` to see the real stack
trace in the response (revert once diagnosed). Most common cause: a
connection string in `Web.config` still pointing at `(local)` instead of the
real SQL Server.
</details>

<details>
<summary><code>SwiftFinancials.Utility.exe</code> can't reach the database</summary>

Its connection strings live in its own `App.config`, separate from the
backend's `Web.config` — both need editing (Phase 03). If SQL Server is set
to Windows Authentication only, use a connection string without `User
ID`/`Password` and run the exe as an account with DB access instead.
</details>

<details>
<summary>IIS won't start the backend site / "HTTP Error 500.19"</summary>

Usually the app pool's .NET CLR version is wrong (must be **v4.0**, not "No
Managed Code") or the ASP.NET 4.8 feature from Phase 00 didn't register with
IIS. Re-run `%windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -i`
from an elevated prompt to re-register it.
</details>

<details>
<summary>Backend site loads but shows "HTTP Error 403.14 — directory listing disabled"</summary>

Same root cause as 500.19 above, not a missing default document — IIS is
serving `fosa-api` as a plain static folder instead of running it through
ASP.NET at all, so `Global.asax`'s routing never gets a chance to claim `/`.
Check the app pool's **.NET CLR version** first (Application Pools → the
pool bound to this site → Basic Settings) — "No Managed Code" instead of
**v4.0.30319** causes exactly this. If it's already correct, re-run the
`aspnet_regiis.exe -i` command above and recycle the app pool. To confirm
which one it is before changing anything: open the site's **Handler
Mappings** feature in IIS Manager — no `ExtensionlessUrlHandler-Integrated-4.0`
entry means ASP.NET genuinely isn't wired in for this site.
</details>

<details>
<summary>Browsing <code>/Help</code> gives "The view 'Index' ... was not found" (<code>Areas/HelpPage/Views/...</code>)</summary>

Harmless — this is only the optional auto-generated API documentation page
(`/Help`), not the real API. The actual endpoints the frontend calls go
through `System.Web.Http` (Web API), not this MVC/Razor view, so they're
unaffected either way. The view is missing because `Areas\HelpPage\Views\`
(and `Areas\HelpPage\HelpPage.css`) weren't copied — Razor views live on
disk and aren't compiled into `bin\`, unlike every other `Areas\*` folder.
Copy that one subfolder over (Phase 02) and recycle the app pool to fix it,
or ignore it entirely if you don't need the docs page.
</details>

## 08 — Self-hosted runner — optional, enables CI/CD

Once this is running, a push to `main` can build and redeploy automatically —
replacing the manual build-and-copy in Phases 01, 02, 04, and 05 with a
workflow. Register one runner per repo (frontend and backend are separate
GitHub repos, and a runner registers to one repo unless you have org admin).

1. **Register the runner on GitHub first.** In each repo: **Settings →
   Actions → Runners → New self-hosted runner → Windows, x64**. GitHub
   generates a download-and-configure script with a one-time registration
   token already filled in — that token expires quickly, so use GitHub's own
   generated commands directly on the server rather than retyping them from
   anywhere else. The script downloads the runner as a zip, extracts it into
   a folder you choose (e.g. `C:\actions-runner-api`), then runs
   `config.cmd --url https://github.com/OWNER/REPO --token TOKEN` — that
   registers this machine as a runner for that specific repo.
2. **Install it as a Windows Service.** Don't leave it running via
   `run.cmd` in an open window — it dies the moment you disconnect RDP. From
   an elevated prompt, inside the folder `config.cmd` just set up:
   ```powershell
   .\svc install
   .\svc start
   ```
   This registers it as a proper Windows Service, so it starts on boot and
   keeps polling GitHub for jobs whether or not anyone's logged in.
3. **Pick a service account deliberately.** The runner executes workflow
   steps with whatever permissions its Windows Service account has — it
   needs write access to `C:\inetpub\wwwroot\fosa-api` / `fosa-app` and
   rights to recycle the IIS app pools. A dedicated local account scoped to
   just those folders is the tighter setup; running as a full local admin is
   faster — reasonable for a single internal server nobody else touches,
   worth tightening later if that changes.
4. **Confirm it's connected.** Back on the repo's **Settings → Actions →
   Runners** page, the new runner should show a green **Idle** status. If
   it's not there, the service likely isn't running — check with
   `Get-Service actions.runner.*` on the server.
5. **Two repos, two runners.** Repeat registration in the other repo too,
   into a second folder (e.g. `C:\actions-runner-app`) with its own service
   — running two runner services side by side on the same box is normal. The
   alternative is a single **organization-level** runner shared across both
   repos, which needs org admin rights.

> **Before enabling this on either repo:** self-hosted runners execute
> whatever a workflow file says, with that service account's real
> permissions on your server. That's fine for a private repo only trusted
> people can push to. If either repo is ever made public, or starts
> accepting pull requests from outside contributors, go to **Settings →
> Actions → General → Fork pull request workflows** and require approval for
> workflows from forks — otherwise an untrusted PR can run arbitrary code on
> this machine.
