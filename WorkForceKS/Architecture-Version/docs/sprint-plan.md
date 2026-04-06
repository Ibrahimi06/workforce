# Sprint 2 Plan — Xhafer Ibrahimi
Data: 1 Prill 2026

## Gjendja Aktuale

### Çka funksionon tani
- CRUD i plotë për punonjës (Shto, Listo, Kërko, Ndrysho, Fshi)
- Arkitektura me shtresa: UI → Service → Repository
- `FileRepository` — lexim/shkrim i të dhënave nga file CSV
- `PostgresRepository` — implementim alternativ me Npgsql
- `EmployeeService` — logjikë biznesi me validim bazë
- `ConsoleUI` — ndërfaqe konsole me menu interaktive
- `IRepository<T>` — interface generike për Repository Pattern
- `Employee` model me të gjitha fushat e nevojshme

### Çka nuk funksionon
- `FileRepository` nuk trajton rastin kur file mungon (hedh IOException)
- Input i gabuar për pagë (p.sh. "abc") shkakton `FormatException` në `ShowFiltered()`
- Input i gabuar për ID (p.sh. "xyz") shkakton `FormatException` në `FindById()`
- Nuk ka asnjë statistikë (total, mesatare, max, min)
- Nuk ka teste automatike

### A kompajlohet dhe ekzekutohet programi
Po — projekti kompajlohet dhe ekzekutohet pa gabime me CSV mode.

---

## Plani i Sprintit

### Feature e Re — Statistika e Pagave
**Çka bën:** Useri zgjedh opsionin 7 nga menyja, opsionalisht filtron sipas departamentit, dhe programi tregon:
- Numrin total të punonjësve
- Shumën totale të pagave
- Pagën mesatare
- Pagën maksimale dhe minimale
- Emrin e punonjësit me pagën më të lartë

**Si e përdor useri:** Menyu → `7` → shkruan departamentin ose lë bosh → shfaqen statistikat në boks vizual.

**Shtresat:**
- `UI` → merr input nga useri, shfaq rezultatin
- `Service.GetStatistics(department?)` → llogarit të gjitha vlerat
- `Repository.GetAll()` → kthon listën e të dhënave

### Error Handling — Çka do të shtosh

**Pjesë që mund të crashojnë tani:**
1. `File.ReadAllLines()` — nëse file CSV mungon ose është i pa-aksesshëm
2. Input i gabuar për pagë (`"abc"`) te `ShowFiltered()` dhe `AddEmployee()`
3. Input i gabuar për ID (`"xyz"`) te `FindById()` dhe `DeleteEmployee()`

**Si do t'i trajtoj:**
1. `FileRepository.EnsureFileExists()` krijon file automatikisht nëse mungon; `GetAll()` kthen listë bosh me mesazh nëse ka `IOException`
2. Të gjitha input-et numerike lexohen me `TryParse` dhe validohen para se t'i dërgohen Service — nuk kalon asnjë `FormatException` deri te useri
3. ID validohet me `TryParse` + kontroll `> 0` para thirrjes `FindById()`

### Teste — Çka do të testoj

**Metodat:** `Add`, `FindById`, `List`, `Delete`, `GetStatistics`

**Rastet kufitare:**
- `Add` me emër bosh → `ArgumentException`
- `Add` me pagë negative/zero → `ArgumentException`
- `FindById` me ID = 0 ose negative → `ArgumentException`
- `FindById` me ID që nuk ekziston → `null`
- `GetStatistics` me repository bosh → `null`
- `GetStatistics` me departament që nuk ekziston → `null`
- `GetStatistics` me një punonjës → average = max = min = salary e tij

---

## Afati
- Deadline: Martë, 8 Prill 2026, ora 08:30
