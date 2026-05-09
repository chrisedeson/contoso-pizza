# CSE 325 — Week 01 Assignment Notes

**Student:** Christopher Edeson
**SDK:** .NET 10.0.107 (current LTS) on Ubuntu 24.04 / WSL2
**Source page:** https://byui-cse.github.io/cse325-ww-course/week01/assignment-01.html

---

## Part 1 — Build .NET Applications with C#

The Microsoft Learn modules listed in the assignment were completed locally by
building the two artifacts in this repo:

- `ContosoPizza/` — Web API with ASP.NET Core controllers (CRUD on a `Pizza` model).
- `SalesAnalysis/` — console app reading `stores/*/sales.json` files and writing aggregate output.

### 1A. Pizza list — original content + additional record

The base tutorial seeds two pizzas. I added a third: **Hawaiian BBQ** (`Id = 3`).
The full seed list lives in `ContosoPizza/Services/PizzaService.cs`:

```csharp
Pizzas = new List<Pizza>
{
    new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
    new Pizza { Id = 2, Name = "Veggie",          IsGlutenFree = true  },
    new Pizza { Id = 3, Name = "Hawaiian BBQ",    IsGlutenFree = false }   // added record
};
```

Initial `GET /Pizza` response confirms all three records are present:

```json
[
  { "id": 1, "name": "Classic Italian", "isGlutenFree": false },
  { "id": 2, "name": "Veggie",          "isGlutenFree": true  },
  { "id": 3, "name": "Hawaiian BBQ",    "isGlutenFree": false }
]
```

### 1B. Verified API behavior — request / response / status code

The API was started with `dotnet run --launch-profile http` (listening on
`http://localhost:5138`) and exercised with `curl`. Each verb below was run
against a fresh in-memory state.

#### GET (collection) — `200 OK`

Request:
```
GET http://localhost:5138/Pizza
```
Response: **Status 200**
```json
[
  { "id": 1, "name": "Classic Italian", "isGlutenFree": false },
  { "id": 2, "name": "Veggie",          "isGlutenFree": true  },
  { "id": 3, "name": "Hawaiian BBQ",    "isGlutenFree": false }
]
```

#### GET (single item) — `200 OK`

Request:
```
GET http://localhost:5138/Pizza/2
```
Response: **Status 200**
```json
{ "id": 2, "name": "Veggie", "isGlutenFree": true }
```

#### POST — `201 Created`

Request:
```
POST http://localhost:5138/Pizza
Content-Type: application/json

{ "name": "Margherita", "isGlutenFree": false }
```
Response: **Status 201**, `Location: http://localhost:5138/Pizza/4`
```json
{ "id": 4, "name": "Margherita", "isGlutenFree": false }
```

#### PUT — `204 No Content`

Request:
```
PUT http://localhost:5138/Pizza/3
Content-Type: application/json

{ "id": 3, "name": "Hawaiian", "isGlutenFree": false }
```
Response: **Status 204** (empty body — pizza renamed from "Hawaiian BBQ" to "Hawaiian").

#### DELETE — `204 No Content`

Request:
```
DELETE http://localhost:5138/Pizza/1
```
Response: **Status 204** (empty body — `Classic Italian` removed).

#### Final state confirmation — `200 OK`

```
GET http://localhost:5138/Pizza
```
Response: **Status 200**
```json
[
  { "id": 2, "name": "Veggie",     "isGlutenFree": true  },
  { "id": 3, "name": "Hawaiian",   "isGlutenFree": false },
  { "id": 4, "name": "Margherita", "isGlutenFree": false }
]
```

The full state transition is consistent: PUT renamed id 3, DELETE removed id 1,
POST added id 4 — all four verbs returned the documented status codes.

---

## Part 2 — Sales summary function

Added to `SalesAnalysis/Program.cs`. The function walks every `*.json` sales file
under `stores/`, totals their `Total` values, and writes
`salesTotalDir/salesSummary.txt` with the running total plus a per-file breakdown.

Sample output (from running `dotnet run` against the seeded `stores/` data):

```
Sales Summary
----------------------------
 Total Sales: $54,238.20

 Details:
  204/sales.json: $7,321.65
  201/sales.json: $13,452.95
  202/sales.json: $9,874.50
  203/sales.json: $23,589.10
```

### Working function — text copy

```csharp
// Additional function required by Part 2 of the assignment.
// Writes a Sales Summary report file containing the total sales across all
// store files plus a per-file currency breakdown.
static void WriteSalesSummaryReport(IEnumerable<string> salesFiles, string outputDir, CultureInfo culture)
{
    var report = new StringBuilder();
    double total = 0;
    var perFile = new List<(string FileName, double Amount)>();

    foreach (string file in salesFiles)
    {
        string json = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData>(json);
        double amount = data?.Total ?? 0;
        total += amount;

        string storeId = Directory.GetParent(file)?.Name ?? "unknown";
        perFile.Add(($"{storeId}/{Path.GetFileName(file)}", amount));
    }

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {total.ToString("C", culture)}");
    report.AppendLine();
    report.AppendLine(" Details:");
    foreach (var (fileName, amount) in perFile)
    {
        report.AppendLine($"  {fileName}: {amount.ToString("C", culture)}");
    }

    File.WriteAllText(Path.Combine(outputDir, "salesSummary.txt"), report.ToString());
}
```

The function is invoked from top-level statements as:

```csharp
WriteSalesSummaryReport(salesFiles, salesTotalDir, culture);
```

`StringBuilder` is used to assemble the report and `ToString("C", culture)` (with
`new CultureInfo("en-US")`) formats every monetary value as US dollars regardless
of the host locale.

---

## How to run locally

```bash
# Web API
cd ContosoPizza
dotnet run --launch-profile http
# then in another shell: curl http://localhost:5138/Pizza

# Files / sales summary
cd SalesAnalysis
dotnet run
# inspect: salesTotalDir/totals.txt and salesTotalDir/salesSummary.txt
```
