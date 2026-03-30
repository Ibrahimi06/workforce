# WorkForce KS — Implementation Documentation

**Studenti:** Xhafer Ibrahimi  
**Kursi:** Arkitekturë Softuerike  
**Data:** Mars 2026  
**Gjuha:** C# / .NET 8  
**Databaza:** PostgreSQL (Npgsql driver)

---

## 1. Struktura e Projektit

```
WorkForceKS/
├── Models/
│   └── Employee.cs           ← Modeli kryesor (6 atribute)
├── Data/
│   ├── IRepository.cs        ← Interface gjenerik CRUD
│   └── PostgresRepository.cs ← Implementim me PostgreSQL
├── Services/
│   └── EmployeeService.cs    ← Logjika e biznesit + validimi
├── UI/
│   └── ConsoleUI.cs          ← Ndërfaqja e konsolës
├── scripts/
│   └── setup.sql             ← Script SQL për krijimin e tabelës + seed
├── docs/
│   └── implementation.md     ← Ky dokument
├── Program.cs                ← Composition Root
├── WorkForceKS.csproj        ← Konfigurimi i projektit (.NET 8 + Npgsql)
├── appsettings.json          ← Connection string lokal
└── .gitignore
```

---

## 2. Modeli: Employee.cs

Modeli **Employee** ka 6 atribute:

| Atributi   | Tipi      | Përshkrim                          |
|------------|-----------|------------------------------------|
| Id         | int       | Primary key (auto-increment nga DB)|
| Name       | string    | Emri dhe mbiemri i punonjësit      |
| Position   | string    | Pozita e punës                     |
| Department | string    | Departamenti                       |
| Salary     | decimal   | Paga mujore në euro                |
| HiredAt    | DateTime  | Data e fillimit të punës           |

---

## 3. Repository: PostgresRepository.cs

Implementon `IRepository<Employee>` me **raw SQL** dhe **Npgsql**:

| Metoda         | SQL                                      | Përshkrim                            |
|----------------|------------------------------------------|--------------------------------------|
| `GetAll()`     | `SELECT ... ORDER BY id`                | Kthen të gjithë punonjësit           |
| `GetById(id)`  | `SELECT ... WHERE id = @id`             | Ktheja një punonjës ose null         |
| `Add(e)`       | `INSERT ... RETURNING id`               | Shton dhe cakton ID-n automatikisht  |
| `Update(e)`    | `UPDATE ... WHERE id = @id`             | Përditëson të gjitha fushat          |
| `Delete(id)`   | `DELETE FROM employees WHERE id = @id`  | Fshin me parametër të sigurt         |

**`SeedIfEmpty()`** — kontrollon nëse tabela është bosh dhe fut 6 rekorde fillestare vetëm herën e parë.

**`EnsureTableExists()`** — ekzekuton `CREATE TABLE IF NOT EXISTS` automatikisht kur aplikacioni niset — nuk kërkon ekzekutim manual të SQL.

---

## 4. Service: EmployeeService.cs

Merr `IRepository<Employee>` si **dependency injection** në konstruktor.

### 4.1 List() — Listim me filtrim
```csharp
var it = _service.List(department: "IT", minSalary: 1200);
```
Filtron në memorie pas marrjes nga DB. Dy parametra opsionalë:
- `department` → kërkon me `Contains` (case-insensitive)
- `minSalary`  → filtron punonjësit me pagë ≥ vlerës

### 4.2 FindById() — Kërko sipas ID
```csharp
var emp = _service.FindById(3);
```
Hedh `ArgumentException` nëse ID ≤ 0.

### 4.3 Add() — Shto me validim
```csharp
_service.Add("Artan Berisha", "Menaxher", "HR", 1800, DateTime.Today);
```
**Validime:**
- Emri nuk mund të jetë bosh
- Pozita nuk mund të jetë bosh
- Departamenti nuk mund të jetë bosh
- Paga duhet të jetë > 0

### 4.4 Update() — Përditëso
Kontrollon nëse punonjësi ekziston, validon inputin, dhe thërret `_repo.Update()`.

### 4.5 Delete() — Fshi
Kontrollon nëse punonjësi ekziston para fshirjes. Kërkon konfirmim nga UI.

---

## 5. UI: ConsoleUI.cs

Meny me 6 opsione. Rrjedha e plotë:

```
UI → Service → PostgresRepository → PostgreSQL
```

```
  ┌─────────────────────────────────┐
  │         WORKFORCE KS            │
  ├─────────────────────────────────┤
  │  1. Listo të gjithë punonjësit  │
  │  2. Listo me filtrim            │
  │  3. Kërko sipas ID              │
  │  4. Shto punonjës               │
  │  5. Ndrysho punonjës            │
  │  6. Fshi punonjës               │
  │  0. Dil                         │
  └─────────────────────────────────┘
```

---

## 6. Output — Simulim i Ekzekutimit

### Opsioni 1 — Lista e gjithë punonjësve

```
  ID    Emri                   Pozita               Departamenti    Paga       Punësuar
  ────────────────────────────────────────────────────────────────────────────────────────
  1     Artan Berisha          Menaxher             HR              €1,800.00  2020-03-01
  2     Vjosa Gashi            Zhvillues             IT              €1,500.00  2021-06-15
  3     Blerim Krasniqi        Analista              Financa         €1,350.00  2019-01-10
  4     Drita Morina           Dizajner UI/UX        IT              €1,200.00  2022-09-01
  5     Fatos Hyseni           Kontabilist           Financa         €1,100.00  2018-04-20
  6     Lirie Osmani           Asistent Admin        Administrim     €  950.00  2023-02-01
  ────────────────────────────────────────────────────────────────────────────────────────
  Gjithsej: 6 punonjës.
```

### Opsioni 2 — Filtrim (Departamenti: IT, Paga min: 1200)

```
  Departamenti []: IT
  Paga minimale []: 1200

  ID    Emri                   Pozita               Departamenti    Paga       Punësuar
  ────────────────────────────────────────────────────────────────────────────────────────
  2     Vjosa Gashi            Zhvillues             IT              €1,500.00  2021-06-15
  4     Drita Morina           Dizajner UI/UX        IT              €1,200.00  2022-09-01
  ────────────────────────────────────────────────────────────────────────────────────────
  Gjithsej: 2 punonjës.
```

### Opsioni 4 — Shto punonjës

```
  ── Shto Punonjës ──
  Emri dhe mbiemri: Fisnik Rama
  Pozita: Inxhinier DevOps
  Departamenti: IT
  Paga (€): 1650
  Data e punësimit (YYYY-MM-DD): 2024-01-10

  ✓ Punonjësi u shtua me ID: 7
```

### Opsioni 5 — Ndrysho punonjës

```
  ID e punonjësit për ndryshim: 6
  Punonjësi aktual: [6] Lirie Osmani | Asistent Admin | Administrim | €950.00 | Punësuar: 2023-02-01

  Emri [Lirie Osmani]:
  Pozita [Asistent Admin]: Koordinatore HR
  Departamenti [Administrim]: HR
  Paga (€) [950]: 1050
  Data e punësimit [2023-02-01]:

  ✓ Punonjësi me ID 6 u përditësua.
```

### Opsioni 6 — Fshi punonjës

```
  ID e punonjësit për fshirje: 7

  ⚠  Jeni i sigurt se doni të fshini 'Fisnik Rama'? (po/jo): po

  ✓ Punonjësi me ID 7 u fshi.
```

### Validim — Shtim me gabime

```
  Emri dhe mbiemri:           ← (bosh, Enter)
  Pozita:                     ← (bosh)
  Departamenti: IT
  Paga (€): -500

  ✗ Gabim: Emri nuk mund të jetë bosh.
            → Pozita nuk mund të jetë bosh.
            → Paga duhet të jetë më e madhe se 0.
```

---

## 7. Setup dhe Ekzekutim

### Kërkesat

- .NET 8 SDK
- PostgreSQL 14+

### Hapat

```bash
# 1. Klono projektin
git clone https://github.com/Ibrahimi06/workforce
cd workforce/WorkForceKS

# 2. Krijo bazën e të dhënave
psql -U postgres -c "CREATE DATABASE workforceks;"

# 3. (Opsional) Ekzekuto scriptin SQL
psql -U postgres -d workforceks -f scripts/setup.sql

# 4. Cakto connection string
export DATABASE_URL="Host=localhost;Database=workforceks;Username=postgres;Password=yourpassword"

# 5. Instalo dependencies dhe ekzekuto
dotnet restore
dotnet run
```

> Aplikacioni automatikisht krijon tabelën dhe fut të dhënat fillestare nëse tabela është bosh — pa pasur nevojë për hapa manualë SQL.

---

## 8. Arkitektura — Shtresat

```
┌─────────────────────────────────────────────────────────┐
│                      Program.cs                         │
│              (Composition Root / DI Manual)             │
└────────────────────────────┬────────────────────────────┘
                             │ krijon dhe lidh
          ┌──────────────────┼──────────────────┐
          ▼                  ▼                  ▼
   ┌─────────────┐  ┌──────────────────┐  ┌──────────┐
   │  ConsoleUI  │  │ EmployeeService  │  │ Postgres │
   │   (UI/)     │──▶  (Services/)    │──▶ Repository│
   └─────────────┘  └──────────────────┘  │ (Data/)  │
                                          └────┬─────┘
                                               │
                                          ┌────▼─────┐
                                          │PostgreSQL│
                                          │  Server  │
                                          └──────────┘
```

**Parimi i Dependency Injection:** Çdo shtresë merr varësinë e saj nga niveli i sipërm — `ConsoleUI` nuk di për databazën, `EmployeeService` nuk di për PostgreSQL.

---

## 9. Çfarë Funksionon

| Ushtrimi | Funksionaliteti | Statusi |
|----------|----------------|---------|
| **Ushtrimi 1** | `Employee` model me 6 atribute | ✅ |
| **Ushtrimi 1** | `IRepository<T>` interface gjenerik | ✅ |
| **Ushtrimi 1** | `PostgresRepository` me GetAll/GetById/Add | ✅ |
| **Ushtrimi 1** | Seed me 6 rekorde fillestare | ✅ |
| **Ushtrimi 2** | `EmployeeService.List()` me filtrim | ✅ |
| **Ushtrimi 2** | `EmployeeService.Add()` me validim | ✅ |
| **Ushtrimi 2** | `EmployeeService.FindById()` | ✅ |
| **Ushtrimi 2** | Dependency Injection përmes konstruktorit | ✅ |
| **Ushtrimi 3** | Menu interaktive me 6 opsione | ✅ |
| **Ushtrimi 3** | Rrjedha UI → Service → Repository → DB | ✅ |
| **Ushtrimi 3** | Read + Create funksionojnë end-to-end | ✅ |
| **Bonus** | Update i plotë (Repository + Service + UI) | ✅ |
| **Bonus** | Delete me konfirmim (Repository + Service + UI) | ✅ |
