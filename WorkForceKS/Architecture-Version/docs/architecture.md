# Architecture Documentation

## Layers

### Models
Përmban entitetet (Employee).

### Data
Menaxhon ruajtjen dhe leximin e të dhënave (CSV).

### Services
Përmban logjikën e biznesit.

### UI
Ndërfaqja me përdoruesin.

---

## Design Decisions

- Përdorim Repository Pattern për fleksibilitet
- Ndarje në shtresa për mirëmbajtje më të lehtë
- CSV si storage i thjeshtë për demonstrim

---

## SOLID Principles

### Single Responsibility
Çdo klasë ka një rol të vetëm.

### Dependency Inversion
Service përdor interface (IRepository), jo implementim konkret.