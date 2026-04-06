# Sprint 2 Report — Xhafer Ibrahimi
Data: 8 Prill 2026

---

## Çka Përfundova

### 1. Feature e Re — Statistika e Pagave ✅
Implementova feature-n e statistikave të pagave e cila kalon nëpër të gjitha shtresat e arkitekturës:

**UI → Service → Repository**

- Shtova `EmployeeStatistics` model i ri në `Models/EmployeeStatistics.cs`
- Shtova metodën `GetStatistics(string? department)` në `EmployeeService`
- Shtova opsionin **7** në menynë e `ConsoleUI` me pamje vizuale
- Useri mund të filtrojë sipas departamentit ose të marrë statistika për të gjithë punonjësit
- Statistikat e shfaqura: numri total, shuma e pagave, mesatarja, maksimumi, minimumi, paguesi më i lartë

**Output demo (departamenti IT):**
```
  ╔══════════════════════════════════════════════╗
  ║  Statistika: IT                              ║
  ╠══════════════════════════════════════════════╣
  ║  Numri i punonjësve : 2                      ║
  ║  Totali i pagave    : €2,700.00              ║
  ║  Paga mesatare      : €1,350.00              ║
  ║  Paga maksimale     : €1,500.00              ║
  ║  Paga minimale      : €1,200.00              ║
  ║  Paguesi më i lartë : Vjosa Gashi            ║
  ╚══════════════════════════════════════════════╝
```

---

### 2. Error Handling ✅
Ristrukturova `FileRepository` dhe `ConsoleUI` për të trajtuar të gjitha rastet e gabimit:

**File errors (Repository layer):**
- `EnsureFileExists()` — nëse `data.csv` mungon, krijohet automatikisht me header
- `GetAll()` — nëse `IOException` ndodh gjatë leximit, kthen listë bosh me mesazh informues
- `Save()` — nëse shkruajtja dështon, hedh `InvalidOperationException` me mesazh të qartë

**Input errors (UI layer):**
- Çdo input numerik (ID, pagë) lexohet me `TryParse` — asnjë `FormatException` nuk arrin tek useri
- Data validohet me `DateTime.TryParse` para çdo thirrjeje te Service
- Mesazhe të qarta: `"Ju lutem shkruani numër valid"`, `"Format i pavlefshëm për datë"`

**ID nuk ekziston (Service layer):**
- `FindById(999)` → kthen `null`, UI tregon `"Punonjësi me ID 999 nuk u gjet."`
- `Delete(999)` → hedh `InvalidOperationException`, UI e kap dhe tregon mesazh
- Programi vazhdon në çdo rast — nuk mbyllet kurrë nga një gabim input

---

### 3. Unit Tests ✅
Krijova projektin `WorkForceKS.Tests` me **20 teste xUnit** duke përdorur `InMemoryRepository` (pa file I/O):

| Kategoria | Testet |
|-----------|--------|
| Add — rast normal | Add_ValidEmployee_ReturnsEmployeeWithId |
| Add — raste kufitare | EmptyName, ZeroSalary, NegativeSalary |
| FindById | Existing, NonExisting, ZeroId, NegativeId |
| List | NoFilter, ByDepartment, ByMinSalary, CaseInsensitive, NonExistingDept |
| Delete | Existing, NonExisting |
| GetStatistics | Count, Total, Max, Min, Filtered, Empty→null, NonExistingDept→null, SingleEmployee |

Të gjitha 20 testet kalojnë (`dotnet test`).

---

## Çka Mbeti
Asgjë nga plani i sprintit nuk mbeti pa u implementuar. Të gjitha kërkesat e detyrave janë plotësuar:
- ✅ Feature funksionale nëpër UI→Service→Repository
- ✅ Error handling në të tri shtresat
- ✅ Projekt test me 20 teste (min kërkesa: 3)

---

## Çka Mësova
Gjatë këtij sprinti mësova rëndësinë e **testimit me objekte fake (InMemoryRepository)** në vend të file-ve reale. Duke ndarë logjikën e biznesit nga I/O-ja e file-it nëpërmjet `IRepository<T>`, testet bëhen të shpejta, të izoluara dhe të ripërsëritshme — nuk varen nga sistemi i file-ve dhe nuk lënë file mbetje pas ekzekutimit. Kjo është parimi i **Dependency Injection** në praktikë.
