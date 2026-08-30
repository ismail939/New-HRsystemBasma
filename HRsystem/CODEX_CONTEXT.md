# HRplusBasma HR System — Full Technical Context (for Codex / ChatGPT)

> **How to use:** Paste this entire document as context into Codex, then ask your question or give your task. It contains everything needed to understand what exists, how the modules relate, and what remains to be built.

---

## 1. Project Overview & Tech Stack

**Purpose:** An enterprise-grade, bilingual (Arabic-first, RTL) web-based HRMS covering the full employee lifecycle: recruitment, employee records, biometric attendance, shifts, leave/off-days, disciplinary penalties, payroll, notifications, and reporting.

**Solution name:** `HRsystem` — a single ASP.NET Core MVC project (`.csproj` based).

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC, Razor views (`.cshtml`), .NET 6+ |
| ORM | Entity Framework Core (**Code-First**), ~24 migrations in `Migrations/` |
| Database | Microsoft SQL Server |
| Auth | Cookie authentication, 8h sliding expiration, RBAC — roles `Admin` and `HR`; policy `AdminOnly` (= role `Admin`) |
| PDF | QuestPDF (Community license) |
| CSS | Tailwind CSS + PostCSS |
| JS | Vanilla ES6+ (feature folders under `wwwroot/js/`) |
| Logging/Audit | Built-in `ILogger` + custom `HRLog` table |
| Biometric | ZK-Teco fingerprint device integration (currently **commented out** in `Program.cs`) |

**Startup (`Program.cs`):** registers DbContext, cookie auth, `NotificationService` (`INotificationService`), `OffDayBalanceAutoService` (`IOffDayBalanceAutoService`), calls `DbSeeder.SeedAsync(context)` at startup, configures error handling, RTL-supporting encoding registration, and default MVC routing.

---

## 2. High-Level Architecture

```
Controllers/  (14 controllers)  ← HTTP/REST, route attributes [Route("/...")]
   ↓
Services/  (NotificationService, OffDayBalanceAutoService)
   ↓
Data/AppDbContext.cs  ← EF Core DbContext, all Fluent API relationship config
   ↓
Models/  ← EF entities (POCOs with data annotations) + Models/Enums/ (enums stored as int)
   ↓
Migrations/  ← code-first history, applied via `dotnet ef database update`
   ↑
Views/  ← Razor, RTL. ViewModels/ for typed views. Query/table data often via ViewBag.
```

**Conventions (follow these when adding code):**
- Controllers: `[Authorize(Roles = "Admin,HR")]` at class level; actions decorated with `[HttpGet]`/`[HttpPost]` + `[Route("/module/action")]`.
- Models: `[Key]`, `[Required]`, `[ForeignKey(nameof(...))]` data annotations; navigation props marked `[JsonIgnore]` when they'd cause JSON cycles.
- Enums: stored as `int` — each enum entity property has `.HasConversion<int>()` in `AppDbContext.OnModelCreating`.
- Relationships: configured explicitly in `OnModelCreating` with `DeleteBehavior.Restrict` (or `SetNull` for users who approved things).
- Audit: important user actions append an `HRLog` row (`Action` = description string).
- UI: Arabic labels as the default; `.cshtml` under `Views/<Controller>/`; JS under `wwwroot/js/<Feature>/`.

---

## 3. Complete Entity Map (all DbSets in `AppDbContext`)

### 3.1 HR Core
| Entity (table) | Role | Key relationships |
|---|---|---|
| `HREmployee` | Employee master data (name, national ID, DOB, hire date, job, contract, department, BasmaId, insurance number, phone…) | FK → `HRDepartment`; 1→N `EmployeePayrollComponents`; unique index on `NationalId` |
| `HRDepartment` | Departments | 1→N employees, 1→N `DepartmentPayrollPolicy` |
| `HRLog` | Audit log of HR actions | — |
| `HREmployeeFile` | Employee documents (on-disk under `wwwroot/images/`) | FK → employee |
| `HREmployeeRate` | Monthly performance rating (1–5), prevented duplicates per month/year | FK → employee |
| `HRApplier` / `HRApplierFile` | Recruitment applicants + their CVs | FK → applicant |
| `User` | Login accounts (Admin/HR) | FK from penalties (approver), component-history (changed-by) |
| `Notification` | In-app notifications, read/unread, indexed | FK → `User`, nullable FK → related `Request` |
| `Request` | Leave/approval requests with workflow (RequestType, LeaveType, Status) | FKs → employee, created-by user, responded-by user |

### 3.2 Attendance / Shifts / Leave
| Entity | Role | Notes |
|---|---|---|
| `HREmployeeBasma` | **Daily attendance snapshot per employee per day** — arrival/departure, total hours, late minutes, early-leave minutes, overtime minutes, status (1 present / 0 absent / 2 on leave / 3 absent+leave), `OffDayType` (sick/annual/unpaid/official) | Unique index `(EmployeeId, DayDate)`; **this is what payroll reads** |
| `CheckInOut` | Raw device punch records | index on check-in/out |
| `HREmployeeShift` | Employee ↔ shift assignment with date ranges | fk employee, shift option |
| `HRShiftOption` | Shift template definitions (fixed-time / variable / fixed-duration modes, tolerances) | — |
| `ShiftOverride` | One-off shift override for a specific date | — |
| `HREmployeeOffDay` | Leave/off-day records per employee | — |
| `HROffDayBalance` | Leave balances per type (annual, casual, rest, comp) | managed partly by `OffDayBalanceAutoService` |

**Attendance engine:** `BasmaController.TakeDayFromFingerPrint` aggregates device punches (3 AM → 3 AM window), groups by employee, applies shift/override/off-day logic, and writes one `HREmployeeBasma` daily record per employee with computed hours/late/overtime/status.

### 3.3 Disciplinary (Penalty) Module — **fully wired to payroll**
| Entity | Role |
|---|---|
| `PenaltyRule` | "Law book": rule definition (Name, NameAr, Code unique, Category: Attendance/Behavior/Safety/Performance/Other, IsActive) |
| `PenaltyLevel` | Punishment matrix for a rule: `SequenceOrder`, `FromValue`, `ToValue`, `ValueType` (Minutes/Occurrences/Days), `DeductionUnit`, `DeductionValue`, `IsWarning` — a late of 16–30 min → 0.25 day, etc. **Penalties store DAYS, never money.** |
| `PenaltyEscalation` | Repeat-offender rules: `OccurrenceCount` → harder deduction or warning/"Send to Investigation" |
| `EmployeePenalty` | The actual issued penalty, status workflow `Draft → PendingApproval → Approved / Rejected`, stores `DeductionUnit` (Money/Hour/Day/Percentage/WarningOnly) + `DeductionValue`, `ApprovedByUserId`. **Has nullable `PayrollItemId` back-link** |
| `DeductionLimitPolicy` | Safety caps: max deduction per month/year, max money/day/percentage per penalty; optional FK→`PayrollPolicy`; `MaxPercentageOfSalary` |

### 3.4 Payroll Module — the newest, enterprise design
| Entity | Role | Critical details |
|---|---|---|
| `Payroll` | Monthly payroll **header** (Month, Year, Status: `Draft→Reviewed→Approved→Locked`, GeneratedBy/ReviewedBy/ApprovedBy + timestamps, Notes) | FK→optional `PayrollPolicyId`; 1→N `PayrollItems` |
| `PayrollComponent` | **Master definition** of a salary component (Basic Salary, Transport, Housing, Overtime, Commission, Incentive, Absence, Late, Penalty…): `Code` unique, `Name`/`NameAr`, `Category`, `IsRecurring`, `CalculationMethod`, `DefaultAmount`/`DefaultPercentage`/`FormulaExpression`, `IsTaxable`, `IsInsurable`, `IsActive`, `SortOrder` | `Category` is the **decision switch** (see §5) |
| `EmployeePayrollComponent` | **Per-employee assignment** with concrete `Amount`, validity `StartDate`/`EndDate`, `IsActive`. This holds the employee's actual salary numbers | FK→employee, FK→component; unique index `(EmployeeId, PayrollComponentId, StartDate)` |
| `PayrollComponentHistory` | Salary-change audit: OldAmount→NewAmount, EffectiveDate, Reason, ChangedByUser | FK→employee, component, user |
| `PayrollPolicy` | Company calc rules: WorkingDaysPerMonth (22), CalendarDaysPerMonth (30), WorkingHoursPerDay (8), `DailySalaryCalcMethod`, `OvertimeBase`, overtime multipliers (1.5/2/3), `LateDeductionMethod`, `AbsenceDeductionMethod`, `LeaveEncashmentMethod`, max-deduction limits | `DailySalaryCalcMethod` enum: CalendarDays/WorkingDays/FixedValue |
| `EmployeePayrollPolicy` / `DepartmentPayrollPolicy` | Assign a policy to an employee or department with date ranges | FK→employee/department, FK→PayrollPolicy |
| `OvertimePolicy` | Overtime rules: `HourlyRateMethod` (BasicSalary/GrossSalary/FixedRate), `FixedHourlyRate`, weekday/weekend/holiday multipliers, min/max minutes, `RequiresApproval` | **Modeled, NOT yet wired into payroll** |
| `EmployeeOvertimePolicy` | Employee ↔ overtime policy assignment | — |
| `OvertimeEntry` | A specific overtime approval: date, minutes, rate, multiplier, amount, `OvertimeType`, IsApproved; **nullable `PayrollItemId` back-link** | **Modeled, NOT yet wired into payroll** |
| `CommissionPlan` | Commission schemes: `Type` (Percentage/FixedAmount/TargetBased/Tiered/ProfitBased), `Value` | |
| `CommissionTier` | Tiers for tiered plans (cascade-deleted with plan) | |
| `EmployeeCommissionPlan` | Employee ↔ commission plan assignment | |
| `CommissionTransaction` | An earned commission: date, Amount, BaseAmount; **nullable `PayrollItemId` back-link** | **Modeled, NOT yet wired into payroll** |
| `PayrollDetail` | **Per-employee monthly summary (1 row/employee/payroll)**: BasicSalary, TotalEarnings, TotalDeductions, GrossSalary, NetSalary, TaxableAmount, InsurableAmount, TaxAmount, InsuranceAmount, attendance stats (PresentDays, AbsentDays, LateMinutes, OvertimeHours, Paid/UnpaidLeaves, OfficialHolidays), DailySalaryRate, Notes | FK→Payroll, FK→employee; 1→N `PayrollEarnings`, 1→N `PayrollDeductions` |
| `PayrollEarning` | **Payslip line snapshot** (Name string, Amount, IsTaxable, IsInsurable) | FK→`PayrollDetailId` only — **no FK to PayrollComponent** |
| `PayrollDeduction` | **Payslip line snapshot** (same shape as earning) | FK→`PayrollDetailId` only — **no FK to PayrollComponent** |
| `PayrollItem` | **Source-of-truth ledger row**: FK→`PayrollComponentId` (+ Amount, Quantity, Rate, IsManual, IsActive), `SourceType` (`Recurring/Overtime/Commission/Penalty/Manual`), `SourceId`, Notes | Referenced by `EmployeePenalty.PayrollItemId`, `OvertimeEntry.PayrollItemId`, `CommissionTransaction.PayrollItemId` for **dedup** |
| `DeductionLimitPolicy` | (see §3.3) | |
---

## 4. Module-by-Module — Where Things Live

### 4.1 Authentication
- `HomeController` (`/`, login POST), cookie sessions; logout; global error handling. Users seeded/created via Admin user management.

### 4.2 Employees & Departments
- `EmployeeController`: CRUD `/employees`; files upload/download/ZIP; monthly rate `/addRate`,`/getRate`.
- `AdminController`: admin dashboard `/admin/dashboard`, users, departments, logs `/logs`, create-department.

### 4.3 Attendance (Basma)
- `BasmaController`: device punch import + `TakeDayFromFingerPrint` engine → writes `HREmployeeBasma` daily rows; shift management (`/saveShifts`, `/getShiftsForWeek`); off-days (`/OffDays`); balance (`/offdays/balance/add|edit`).

### 4.4 Requests & Notifications
- `RequestController` + `Request` model (leave/approvals); `NotificationController` + `NotificationService` (in-app); `LeaveRequestController`.

### 4.5 Recruitment
- `ApplierController`: applicant CRUD + file uploads (`HRApplier`/`HRApplierFile`).

### 4.6 Penalties (Disciplinary)
- `PenaltyController`: penalty rules management + `EmployeePenalty` CRUD + approval workflow (`/employee/togglePenaltyActive` etc.); `EditModal.js` in `wwwroot/js/Penalty/`.
- **Approved penalties auto-convert to salary deductions during payroll generation** (details in §6). Full reference: `PENALTY_FLOW.md`.
### 4.7 Payroll
- `PayrollController` routes:
  - `GET /payroll` — landing page
  - `GET /payroll/place-salaries` + `GET /payroll/place-salaries/get-employee-detail` — old employee-salary view
  - `GET/POST /payroll/salary-components…` — CRUD for `PayrollComponent` masters (`add`, `edit`, `toggle`, `get`, `active`)
  - `GET/POST /payroll/salaries…` — per-employee salary/component assignment management (`get-employee`, `add`, `edit`, `delete`, `toggle`) → writes `EmployeePayrollComponent`
  - `GET /payroll/list` — monthly payroll list
  - `GET /payroll/generate` + `POST /payroll/generate/run` — **the payroll engine**
  - `GET /payroll/details/{id}` — payroll detail view (totals, status workflow buttons, notes)
  - `POST /payroll/update-status` — Draft→Reviewed→Approved→Locked
  - `POST /payroll/update-notes`
  - `GET /payroll/salary-history` + `GET /payroll/salary-history/get` — component change history
- Views: `GeneratePayroll.cshtml`, `Index.cshtml`, `PayrollDetails.cshtml`, `PayrollList.cshtml`, `PlaceSalaries.cshtml`, `Salaries.cshtml`, `SalaryComponents.cshtml`, `SalaryHistory.cshtml`

### 4.8 Reports
- `ReportController`: reports dashboard `/reports`, PDF reports page `/PDFReports`, daily report `/dailyReporttt`, all-info report `/allInfoReport` (+ `/getOffDays`, `/getAbsences`). JS at `wwwroot/js/AllInfoReport/`, `wwwroot/js/PDFReport/`.

### 4.9 Employee Dashboard
- `EmployeeDashboardController`: employee-facing summary (salary, attendance, next pay date) using `EmployeeDashboardViewModel`.

---

## 5. The Payroll Generation Pipeline (`POST /payroll/generate/run`) — full walkthrough

`RunPayrollGeneration(month, year)` is **the heart of the system**. Here's exactly what happens:

1. **Uniqueness check** — if a `Payroll` already exists for that month/year → reject.

2. **Create the payroll header** (`Payroll` row, Status=Draft, GeneratedBy=user).

3. **Compute calendar context** — `startDate`/`endDate`, `workingDays` = number of calendar days in the month, `workingHoursPerDay = 8m`.

4. **Find the `PENALTY` component** (`PayrollComponent.Code == "PENALTY"`) — used to post approved penalties as deductions.

5. **Per employee loop:**
   a. Load active `EmployeePayrollComponent`s for the month.
   b. **Split by `Category`** (this is where Category is the decision switch):
      - `basicComponent` = the one whose `PayrollComponent.Category == Salary` → `basicSalary = its Amount` (0 if none).
      - `earningComponents` = all with `Category != Deduction`
      - `deductionComponents` = all with `Category == Deduction`
      - `totalEarnings` = Σ earning amounts; `totalDeductions` = Σ deduction amounts.
   c. **Read attendance** from `HREmployeeBasma` in the month range → PresentDays (status 1), AbsentDays (status 0 or 3), LateMinutes (Σ), OvertimeHours (Σ minutes / 60), PaidLeaves (annual/sick), UnpaidLeaves, OfficialHolidays.
   d. **Daily salary rate** = `basicSalary / workingDays` (used for penalty money math + stored on the detail row).
   e. **Approved-penalty conversion** (the Disciplinary→Payroll bridge):
      - Query `EmployeePenalty` where `Status == Approved`, **`PayrollItemId == null`** (never yet processed), incident date in month.
      - Skip `WarningOnly`. Convert per `DeductionUnit`:
        - `Money` → amount = `DeductionValue`
        - `Hour` → hourly = dailyRate/8; amount = hours × hourly; days = hours/8
        - `Day` → amount = days × dailyRate
        - `Percentage` → amount = % × basicSalary
      - Add to `totalDeductions`.
   f. **Tax/insurance base** — `taxableAmount`/`insurableAmount` = Σ amounts of **earning** components where `PayrollComponent.IsTaxable/IsInsurable` (deductions excluded).
   g. `grossSalary = totalEarnings`; `netSalary = grossSalary − totalDeductions`.
   h. **Write `PayrollDetail`** (aggregate row) — **`TaxAmount = 0`, `InsuranceAmount = 0`** (stubbed — see Gaps).
   i. **Write line rows per component** — for every assigned component: create a `PayrollItem` (`PayrollComponentId`, Amount, SourceType=Recurring) **and** either a `PayrollEarning` (Category ≠ Deduction) or `PayrollDeduction` (Category == Deduction) snapshot.
   j. **Write penalty rows** — for each penalty entry: add a `PayrollDeduction` snapshot; then `SaveChanges` to materialize PayrollItem IDs and **back-link** `penalty.PayrollItemId = item.Id`, `penalty.DeductionAmount`, `penalty.DeductionDays`. → **This is what prevents double-deduction next month** (`p.PayrollItemId == null`).

6. Write an `HRLog`, save, return `{ success, payrollId }`.

### The three-tier data model (important for understanding)
For each component processed, the system writes **three representations**:

| Row | Purpose | FK to component? |
|---|---|---|
| `PayrollItem` | **Ledger/audit trail** — traces which component + origin (SourceType/SourceId), enables dedup via back-links | ✅ Yes |
| `PayrollEarning`/`PayrollDeduction` | **Payslip display lines** (just snapshot Name + Amount + flags) | ❌ No (only PayrollDetailId) |
| `PayrollDetail` | **Per-employee bottom line** (totals, tax/insurance, attendance) | ❌ No |

### Payroll status workflow
`Draft → Reviewed → Approved → Locked`. `UpdatePayrollStatus` re-checks `Locked` (no edits allowed), stamps Reviewer/Approver name+date. UI: `PayrollDetails.cshtml` shows the workflow buttons and full pay-slip grid (Basic, Earnings, Deductions, Gross, Net, Taxable, Insurable, Tax, Insurance, attendance columns) with totals row.
---

## 6. Cross-Module Relationships (the "how it all fits" story)

```
                 ZK-Teco device punches
                        │
                        ▼
               CheckInOut (raw)
                        │ TakeDayFromFingerPrint engine (shifts + overrides + offdays)
                        ▼
             HREmployeeBasma (daily attendance rows: hours, late, overtime, status)
                        │
                        │  Payroll RunPayrollGeneration reads attendance stats
                        ▼
   Approved EmployeePenalties ──► converted (days × daily rate) ──┐
   (Status=Approved & PayrollItemId==null)                        │
        ▲                                                         ▼
   PenaltyLevel / PenaltyEscalation / PenaltyRule         PayrollDetail (per employee)
   (based on minutes/days since incident, repeat count)         ▲
        │                                                       │ Σ
        └── PenaltyRule.Code "PENALTY" component ←── PayrollComponent master
                                                       Category=Salary → base salary & daily rate
                                                       Category=Deduction → deduction bucket
                                                       IsTaxable/IsInsurable → tax/insurance bases
   PayrollItem ← composed of component assignments + penalties (SourceType/SourceId)
        └── back-link EmployeePenalty.PayrollItemId → blocks re-processing
```

**Key facts to keep straight:**
- `PayrollComponent.Category == Deduction` is the **single fork** that decides earning vs deduction everywhere (totals, line tables, tax/insurance base).
- `PayrollEarning`/`PayrollDeduction` are **denormalized snapshots for the payslip only** — no component FK.
- `PayrollItem` is the **audit + dedup backbone** — it carries `PayrollComponentId` and the origin (`SourceType`, `SourceId`), and origin tables back-link their `PayrollItemId` when processed.
- Penalties **store days**, payroll converts to money via daily rate — same violation can yield different money for different salaries.

---

## 7. Current Implementation Status

**✅ Complete & wired:**
- Auth (Admin/HR), user management, audit logs
- Employee CRUD + files + monthly ratings
- Departments
- Attendance engine (daily `HREmployeeBasma`), shifts, shift overrides
- Off-days/leave balances (auto-service), requests & notifications
- Recruitment (applicants + files)
- Disciplinary module end-to-end (rules, levels, escalations, penalty workflow)
- **Payroll**: components CRUD, per-employee salary assignment, monthly generation (earnings + deductions + attendance), status workflow Draft→Locked, penalty→deduction integration, salary history
- Reporting: PDF reports, all-info report, admin dashboard

**⚠️ Stubbed / not yet implemented (the gaps):**

1. **Tax — no tables for tax rates/brackets.** Only per-component `IsTaxable` flags + `PayrollDetail.TaxableAmount`. `TaxAmount` is hardcoded `0` with the comment "يتم حساب الضريبة لاحقاً حسب الشرائح" (tax computed later per brackets). There is **no `TaxBracket` table, no `TaxPolicy`, no income-tax engine anywhere**.
2. **Insurance — no table/field for rates.** Same story: `IsInsurable` flags + `InsurableAmount` exist; `InsuranceAmount` hardcoded `0` ("يتم حساب التأمينات لاحقاً حسب النسبة"). No social-insurance rate field on `PayrollPolicy` or anywhere else.
3. **Overtime module modeled but NOT connected to payroll generation.** `OvertimePolicy`, `EmployeeOvertimePolicy`, `OvertimeEntry` exist (with `PayrollItemId` back-link designed), but `RunPayrollGeneration` never reads `OvertimeEntries`. There is no Overtime controller/UI either.
4. **Commission module modeled but NOT connected to payroll generation.** `CommissionPlan`, `CommissionTier`, `EmployeeCommissionPlan`, `CommissionTransaction` exist (with `PayrollItemId` back-link designed), but generation never reads `CommissionTransactions`. No Commission controller/UI.
5. **Attendance-based deductions not computed.** `PayrollPolicy` has `LateDeductionMethod` / `AbsenceDeductionMethod` and limits, but absence/late hours never turn into money rows in generation — only component snapshots + penalties do.
6. **Data redundancy / design smell:** the same component is written 3× (`PayrollItem` + `PayrollEarning/PayrollDeduction` + `PayrollDetail` totals). Totals and payslip lines could be derived from `PayrollItems`; consolidation is a candidate refactor.
7. **Minor:** passwords stored plain-text; ZK-Teco SDK commented out (needs re-enabling for production); no unit-test project; `PayrollItemSourceType.Overtime/Commission/Manual` enum values exist but only `Recurring` + `Penalty` are produced today.
---

## 8. Recommended "Next Steps" (priority-ordered task list)

1. **Tax brackets module** — add `TaxBracket` entity (e.g., `PayrollPolicyId` optional, From, To, Rate), DbSet + migration, admin CRUD UI, progressive calculation engine; wire into `RunPayrollGeneration` so `TaxAmount` is real and flows into `TotalDeductions` → `NetSalary`.
2. **Social insurance rates** — add rate field(s) to `PayrollPolicy` (employee share; optional employer share), compute `InsuranceAmount` in generation, include in deductions.
3. **Wire Overtime into payroll** — build Overtime UI (policy CRUD + entry approval), then in generation: collect approved `OvertimeEntry` rows for the month with `PayrollItemId == null`, compute amounts, create `PayrollItem` (SourceType=Overtime) + `PayrollEarning` snapshot + add to totals, back-link `PayrollItemId`.
4. **Wire Commission into payroll** — UI for plans/tiers/transactions, then same pattern with SourceType=Commission.
5. **Attendance-based late/absence deductions** — implement via `LateDeductionMethod`/`AbsenceDeductionMethod`, respecting `DeductionLimitPolicy` caps and `MaxDeductionPercentage`.
6. **(Optional refactor)** consolidate to `PayrollItem` as the single source of truth, derive `PayrollDetail` totals + payslip lines from it, and drop/keep `PayrollEarning`/`PayrollDeduction` as display-only views.
7. **Polish** — password hashing, unit tests, CSV/Excel export, re-enable ZK-Teco.

---

## 9. Key Files to Reference While Working

- `Program.cs` — composition root, services, seeding
- `Data/AppDbContext.cs` — all relationships, indexes, enum conversions, DbSets
- `Data/DbSeeder.cs` — seed components (incl. `PENALTY`) and penalty rules
- `Controllers/PayrollController.cs` — generation engine + all payroll endpoints
- `Controllers/PenaltyController.cs` — disciplinary workflow
- `Controllers/BasmaController.cs` — attendance engine
- `Models/Enums/PayrollEnums.cs` — Category, CalculationMethod, SourceType, PayrollStatus, etc.
- `Models/Enums/DisciplinaryEnums.cs` — PenaltyStatus, DeductionUnit, etc.
- `PENALTY_FLOW.md` — detailed penalty→payroll reference
- `Migrations/20260810131553_PayrollModuleEnterpriseDesign.cs` — the big payroll schema migration
- `Migrations/20260726074131_AddTaxAndInsuranceFlags.cs` — where TaxableAmount/InsurableAmount/TaxAmount/InsuranceAmount were added (note: amount columns only, no rate tables)

---

*Document prepared from code inspection of the `HRsystem` repository (main branch).*