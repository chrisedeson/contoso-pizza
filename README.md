# CSE 325 — Week 01

Two .NET 10 (current LTS) projects covering the **Build .NET Applications with C#** Microsoft Learn path:

| Project | Purpose |
| --- | --- |
| `ContosoPizza/` | ASP.NET Core Web API with controllers and CRUD on a `Pizza` model |
| `SalesAnalysis/` | Console app that reads `stores/*/sales.json`, writes `totals.txt`, and a `salesSummary.txt` report |

See [`SUBMISSION.md`](./SUBMISSION.md) for the assignment evidence: the seeded pizza list, four verified API verbs with status codes, and a copy of the sales-summary function.

## Run

```bash
# Web API on http://localhost:5138
cd ContosoPizza && dotnet run --launch-profile http

# Sales aggregation + summary report
cd SalesAnalysis && dotnet run
```
