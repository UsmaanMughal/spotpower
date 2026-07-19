# SpotPower

Spanish day-ahead (OMIE) auction price ingestion, REST API, and Blazor Server frontend.

## Project structure

```
SpotPower.Api             (REST + Quartz)
SpotPower.Core            (Models)
SpotPower.Infrastructure  (EF Core/Data)
SpotPower.Web             (Blazor Server)
```

- **SpotPower.Api** — ASP.NET Core minimal API host and composition root. Runs the Quartz
  scheduler in-process (ingestion and the API deliberately share one host, rather than being
  separate deployables, to avoid two processes writing to the same SQLite file) and exposes
  `GET /api/auction-prices` (JSON / `text/plain` CSV).
- **SpotPower.Core** — the `AuctionPrice` entity. Persistence-ignorant on purpose, so it has
  no EF Core or Quartz dependency.
- **SpotPower.Infrastructure** — `SpotPowerDbContext` (EF Core + SQLite), `OmieClient` /
  `OmieMarginalPriceParser` (downloads and parses OMIE's public `marginalpdbc` file),
  `AuctionImportJob` (the Quartz job, with dedup + backfill), and `SpainClock` (the single
  Europe/Madrid timezone conversion used everywhere).
- **SpotPower.Web** — Blazor Server UI. Talks to `SpotPower.Api` over `HttpClient` only; it
  has no project reference to `SpotPower.Data`/`Infrastructure` and cannot touch the database.

Both `SpotPower.sln` (classic) and `SpotPower.slnx` (new XML format) are provided for IDE
compatibility; either can be used to build the whole solution.
