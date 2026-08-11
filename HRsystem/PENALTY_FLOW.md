# 🎭 Penalty & Deduction System — Complete Reference

> **Purpose:** This document explains how the Disciplinary (Penalty) module communicates with the Payroll module to convert violations into salary deductions — told as a story, with technical details for developers.

---

## 📖 Table of Contents

1. [Overview](#overview)
2. [The Characters (Entities)](#the-characters-entities)
3. [The Lifecycle (PenaltyStatus)](#the-lifecycle-penaltystatus)
4. [The Full Story — End to End](#the-full-story--end-to-end)
5. [The Conversion Formula](#the-conversion-formula)
6. [The Safety Limits (DeductionLimitPolicy)](#the-safety-limits-deductionlimitpolicy)
7. [Relationship Map](#relationship-map)
8. [Key Design Principles](#key-design-principles)
9. [SQL Test Scripts](#sql-test-scripts)
10. [Enums Reference](#enums-reference)

---

## Overview

The system follows a strict **"Never deduct directly from attendance"** principle. The flow is:

```
Attendance
    ↓
Violation
    ↓
Manager Approval
    ↓
Penalty (stored in DAYS, not money)
    ↓
Payroll Run (converts days → money using PayrollPolicy)
    ↓
Payslip Deduction
```

**Key rule:** Penalties store **days** (e.g., 0.5 Day). The payroll engine converts those days to **money** using each employee's salary and the company's `PayrollPolicy`. This keeps the system flexible — different employees with different salaries get different deduction amounts for the same violation.

---

## The Characters (Entities)

### 🏛️ PenaltyRule — "The Law Book"

The **rulebook** that HR writes. Each page is one rule:

| Field | Example | Description |
|-------|---------|-------------|
| `Name` | "Late Arrival" | English name |
| `NameAr` | "تأخر صباحي" | Arabic name |
| `Code` | `LATE_ARRIVAL` | Unique system code |
| `Category` | `Attendance` | Attendance, Behavior, Safety, Performance, Other |
| `IsActive` | `true` | Whether the rule is currently enforced |

This is just the **definition** — it doesn't tell anyone what to do yet.

**Examples of rules:**
- Late Arrival
- Early Leave
- Absence
- No Uniform
- Smoking
- Safety Violation
- Unauthorized Leave
- Manual Violations

---

### 📋 PenaltyLevel — "The Punishment Matrix"

Each rule needs to say: *"If someone is late by X minutes, they get Y punishment."*

For **Late Arrival**, we define 4 levels:

| SequenceOrder | FromValue | ToValue | ValueType | DeductionUnit | DeductionValue | IsWarning |
|---------------|-----------|---------|-----------|---------------|----------------|-----------|
| 1 | 1 | 15 | Minutes | WarningOnly | 0 | ✅ |
| 2 | 16 | 30 | Minutes | Day | 0.25 | ❌ |
| 3 | 31 | 60 | Minutes | Day | 0.5 | ❌ |
| 4 | 61 | 9999 | Minutes | Day | 1 | ❌ |

**Critical:** We store **days**, not money. Money depends on each employee's salary — we never hardcode it.

---

### ⚡ PenaltyEscalation — "The Repeat Offender Rules"

What happens if Ahmed is late **5 times in a month**? We don't apply the same level each time — we escalate:

| OccurrenceCount | DeductionUnit | DeductionValue | IsWarning | ActionRequired |
|-----------------|---------------|----------------|-----------|----------------|
| 1 | WarningOnly | 0 | ✅ | — |
| 2 | Day | 0.25 | ❌ | — |
| 3 | Day | 0.5 | ❌ | — |
| 4 | Day | 1 | ❌ | — |
| 5 | WarningOnly | 0 | ✅ | "Send to Investigation" |

The system **counts** how many Approved penalties this employee already has for that rule in the current period, and picks the matching escalation.

---

### 👤 EmployeePenalty — "The Incident Report"

When Ahmed actually gets caught late, someone (HR or Manager) creates an `EmployeePenalty`. This is the **actual incident** — like a police report.

| Field | Example | Description |
|-------|---------|-------------|
| `EmployeeId` | 3 | Which employee |
| `PenaltyRuleId` | 1 | Which rule was broken |
| `PenaltyLevelId` | 3 | Which level was applied (optional) |
| `PenaltyEscalationId` | 2 | Which escalation was applied (optional) |
| `IncidentDate` | 2026-08-10 | When the violation occurred |
| `Status` | `Approved` | Workflow state |
| `DeductionUnit` | `Day` | **Snapshot** of unit at creation |
| `DeductionValue` | 0.5 | **Snapshot** of value at creation |
| `DeductionDays` | 0.5 | Converted days for payroll |
| `DeductionAmount` | 136.36 | Computed monetary value |
| `ApprovedByUserId` | 5 | Who approved |
| `ApprovedDate` | 2026-08-11 | When approved |

> **Why snapshot?** If HR later changes the PenaltyLevel from 0.5 Day to 0.25 Day, we DON'T want past incidents to retroactively change. The snapshot preserves history.

---

### 🏦 PayrollPolicy — "The Conversion Rate Table"

Penalties store **days**, not money. So how do we convert?

`PayrollPolicy` is the **currency converter**:

| Field | Example | Description |
|-------|---------|-------------|
| `WorkingDaysPerMonth` | 22 | Working days in a month |
| `CalendarDaysPerMonth` | 30 | Calendar days in a month |
| `WorkingHoursPerDay` | 8 | Hours per day |
| `DailySalaryCalcMethod` | `WorkingDays` | CalendarDays, WorkingDays, or FixedValue |
| `DailySalaryFixedValue` | null | If method = FixedValue |
| `OvertimeBase` | `BasicSalary` | Base for overtime calc |
| `OvertimeWeekdayMultiplier` | 1.5 | Weekday OT rate |
| `OvertimeWeekendMultiplier` | 2.0 | Weekend OT rate |
| `OvertimeHolidayMultiplier` | 3.0 | Holiday OT rate |
| `LateDeductionMethod` | `PerMinute` | How late deductions are calculated |
| `AbsenceDeductionMethod` | `PerOccurrence` | How absence deductions are calculated |
| `LeaveEncashmentMethod` | `WorkingDays` | How leave encashment is calculated |
| `MaxDeductionPerMonth` | 2000 | Cap on monthly deductions |
| `MaxDeductionPerYear` | 12000 | Cap on yearly deductions |
| `MaxDeductionAmount` | 500 | Cap per single penalty |
| `MaxDeductionDays` | 2 | Cap in days per penalty |
| `MaxDeductionPercentage` | 25 | Cap as % of salary |

---

### 💰 PayrollComponent — "The Salary Components Catalog"

The catalog of everything that appears on a payslip:

| Field | Example | Description |
|-------|---------|-------------|
| `Name` | "Penalty" | English name |
| `NameAr` | "خصم جزائي" | Arabic name |
| `Code` | `PENALTY` | Unique code |
| `Category` | `Deduction` | Salary, Allowance, Bonus, Incentive, Commission, Overtime, Deduction, Other |
| `IsRecurring` | `false` | Auto-copied every month? |
| `IsTaxable` | `false` | Subject to income tax? |
| `IsInsurable` | `false` | Subject to social insurance? |

---

### 📄 PayrollItem — "The Payslip Line"

When payroll runs, every component assigned to an employee becomes a `PayrollItem` (one line on the payslip).

The penalty becomes:

```
PayrollItem {
    PayrollId = 45,
    EmployeeId = 3 (Ahmed),
    PayrollComponentId = 7 (Penalty component),
    Amount = -136.36 (negative = deduction),
    SourceType = Penalty,
    SourceId = 12 (points to the EmployeePenalty record)
}
```

The `SourceType` + `SourceId` are like a **link/breadcrumb** — you can trace any payslip line back to where it came from.

---

### 🛡️ DeductionLimitPolicy — "The Safety Net"

What if someone gets 10 penalties in one month? We need a **cap**:

| Field | Example | What it prevents |
|-------|---------|-----------------|
| `MaxDeductionPerMonth` | 2000 SAR | Can't deduct more than 2000 SAR in one month |
| `MaxDeductionPerYear` | 12000 SAR | Can't deduct more than 12000 SAR in a year |
| `MaxMoneyPerPenalty` | 500 SAR | Single penalty can't exceed 500 SAR |
| `MaxDaysPerPenalty` | 2 days | Penalty can't exceed 2 days of pay |
| `MaxPercentageOfSalary` | 25% | Total deduction capped at 25% of gross salary |

**Example:** Ahmed's total deductions would be 4000 SAR, but `MaxDeductionPerMonth = 2000`. The system **clamps** it to 2000 SAR — the rest rolls over or is waived per policy.

---

## The Lifecycle (PenaltyStatus)

Each `EmployeePenalty` has a workflow:

```
Draft (0) ──→ PendingApproval (1) ──→ Approved (2)
                    │
                    └──→ Rejected (3)
```

**Only Approved penalties can affect payroll.** This creates a **human check** before any money is deducted.

---

## The Full Story — End to End

### 🕗 Morning — Ahmed is late!

```
7:45 AM — Ahmed arrives 45 minutes late
    │
    ▼
1. HR creates an EmployeePenalty
   ├── EmployeeId = 3 (Ahmed)
   ├── PenaltyRuleId = 1 (Late Arrival)
   ├── PenaltyLevelId = 3 (matches 31-60 minutes → 0.5 Day)
   ├── Status = Draft
   │
   ▼
2. HR submits for approval
   ├── Status = PendingApproval
   │
   ▼
3. Manager reviews and clicks "Approve"
   ├── Status = Approved ✓
   ├── ApprovedDate = today
   ├── ApprovedByUserId = manager's ID
   │
   ▼
4. End of month — Payroll Run starts!
   ├── For Ahmed's salary:
   │   ├── System looks at 22 working days
   │   ├── Basic Salary 6000 / 22 = 272.73 daily rate
   │   │
   │   ├── Finds all Approved EmployeePenalties
   │   │   ├── Ahmed has 1 Approved penalty: 0.5 Day
   │   │   │
   │   │   ├── Converts: 0.5 × 272.73 = 136.36 SAR
   │   │   │
   │   │   └── Creates PayrollItem:
   │   │       └── Amount = -136.36 (deduction)
   │   │           SourceType = Penalty
   │   │           SourceId = 12
   │   │
   │   └── Continues to other items...
   │
   ▼
5. Payslip generated for Ahmed:
   ├── Basic Salary: +6000
   ├── Housing: +1500
   ├── Transportation: +500
   ├── **Penalty (0.5 day): -136.36** ← our story!
   ├── Tax: -300
   ├── Insurance: -300
   ├── Net Salary: 7263.64
```

---

## The Conversion Formula

### Daily Salary Rate

```
Daily Salary Rate = Component Amount ÷ DaysInCalculation

Where DaysInCalculation depends on PayrollPolicy.DailySalaryCalcMethod:
  CalendarDays → 30 (fixed) or actual calendar days of month
  WorkingDays  → PayrollPolicy.WorkingDaysPerMonth (e.g., 22)
  FixedValue   → PayrollPolicy.DailySalaryFixedValue
```

### Worked Example

```
Basic Salary: 6000 SAR
WorkingDaysPerMonth: 22
DailySalaryCalcMethod: WorkingDays

Daily Rate = 6000 / 22 = 272.73 SAR/day

Penalty: 0.5 Day
Deduction = 0.5 × 272.73 = 136.36 SAR
```

### Overtime Rate (for comparison)

```
Hourly Rate = OvertimeBase component / (WorkingDaysPerMonth × WorkingHoursPerDay)

Example: Basic Salary 6000, WorkingDays=22, Hours=8
  Hourly Rate = 6000 / (22 × 8) = 6000 / 176 = 34.09 SAR/hour

Overtime Pay = Hourly Rate × Multiplier × Hours Worked
  Weekday: 34.09 × 1.5 × 2 hours = 102.27 SAR
  Weekend: 34.09 × 2.0 × 2 hours = 136.36 SAR
```

---

## Relationship Map

```
PenaltyRule (Law Book)
    │
    ├──< PenaltyLevel (Punishment Matrix)
    │       └──< EmployeePenalty (Incident) ──> HREmployee (Ahmed)
    │                                            │
    ├──< PenaltyEscalation (Repeat Offender)     │
    │       └──< EmployeePenalty (Incident)      │
    │                                            │
    │                                            │
    └──< EmployeePenalty (Incident) ─────────────┘
                 │
                 │  (only if Status = Approved)
                 ▼
          PayrollItem ──> Payroll (Monthly Run)
                 │
                 ├──> PayrollComponent (Penalty = Deduction)
                 │
                 └──> PayrollPolicy (converts days → money)
                          │
                          └──< DeductionLimitPolicy (safety cap)
```

---

## Key Design Principles

1. **No hardcoding** — Penalties store days, then PayrollPolicy converts to money based on each employee's salary.

2. **Snapshot pattern** — EmployeePenalty stores a copy of the punishment at creation time, so policy changes don't retroactively alter old incidents.

3. **Human approval gate** — Only `Approved` penalties flow to payroll. The manager's approval is the human checkpoint.

4. **Traceability** — Every `PayrollItem` has `SourceType` + `SourceId` so you can trace any payslip line back to the original incident.

5. **Safety limits** — `DeductionLimitPolicy` caps how much can be deducted, protecting employees from excessive punishment.

6. **Configuration, not code** — HR can add new rules, new levels, new escalation patterns, and new limits through the admin dashboard — no code changes needed.

---

## SQL Test Scripts

### Test 1 — Create a Penalty Rule with Levels

```sql
-- Rule: Late Arrival
INSERT INTO PenaltyRules (Name, NameAr, Code, Category, IsActive, CreatedAt)
VALUES ('Late Arrival', 'تأخر صباحي', 'LATE_ARRIVAL', 0, 1, GETDATE());

-- Levels: 1-15 min → Warning, 16-30 min → 0.25 Day, 31-60 min → 0.5 Day, >60 min → 1 Day
INSERT INTO PenaltyLevels (PenaltyRuleId, SequenceOrder, FromValue, ToValue, ValueType,
                           DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 1, 1, 15, 0, 4, 0, 1);      -- Warning only

INSERT INTO PenaltyLevels (PenaltyRuleId, SequenceOrder, FromValue, ToValue, ValueType,
                           DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 2, 16, 30, 0, 2, 0.25, 0);  -- 0.25 Day

INSERT INTO PenaltyLevels (PenaltyRuleId, SequenceOrder, FromValue, ToValue, ValueType,
                           DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 3, 31, 60, 0, 2, 0.5, 0);   -- 0.5 Day

INSERT INTO PenaltyLevels (PenaltyRuleId, SequenceOrder, FromValue, ToValue, ValueType,
                           DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 4, 61, 9999, 0, 2, 1, 0);   -- 1 Day
```

### Test 2 — Create Escalation Rules

```sql
INSERT INTO PenaltyEscalations (PenaltyRuleId, OccurrenceCount, DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 1, 4, 0, 1);    -- 1st occurrence: Warning

INSERT INTO PenaltyEscalations (PenaltyRuleId, OccurrenceCount, DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 2, 2, 0.25, 0); -- 2nd: 0.25 Day

INSERT INTO PenaltyEscalations (PenaltyRuleId, OccurrenceCount, DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 3, 2, 0.5, 0);  -- 3rd: 0.5 Day

INSERT INTO PenaltyEscalations (PenaltyRuleId, OccurrenceCount, DeductionUnit, DeductionValue, IsWarning)
VALUES (1, 4, 2, 1, 0);    -- 4th: 1 Day

INSERT INTO PenaltyEscalations (PenaltyRuleId, OccurrenceCount, DeductionUnit, DeductionValue, IsWarning, ActionRequired)
VALUES (1, 5, 4, 0, 1, 'Send to Investigation'); -- 5th: Investigation
```

### Test 3 — Issue a Penalty (Draft)

```sql
INSERT INTO EmployeePenalties (EmployeeId, PenaltyRuleId, PenaltyLevelId, IncidentDate, CreatedDate,
                               Status, DeductionUnit, DeductionValue)
VALUES (1, 1, 2, '2026-08-10', GETDATE(), 0, 2, 0.25);
-- Status 0 = Draft
```

### Test 4 — Approve the Penalty

```sql
UPDATE EmployeePenalties
SET Status = 2, ApprovedDate = GETDATE(), ApprovedByUserId = 1
WHERE Id = 1;
-- Status 2 = Approved → now eligible for payroll deduction
```

### Test 5 — Verify Only Approved Penalties Affect Payroll

```sql
SELECT ep.Id, e.Name AS Employee, pr.NameAr AS Rule,
       ep.DeductionUnit, ep.DeductionValue, ep.Status
FROM EmployeePenalties ep
JOIN HREmployees e ON e.Id = ep.EmployeeId
JOIN PenaltyRules pr ON pr.Id = ep.PenaltyRuleId
WHERE ep.Status = 2;  -- Only Approved
```

### Test 6 — Create a Payroll Policy

```sql
INSERT INTO PayrollPolicies (Name, WorkingDaysPerMonth, CalendarDaysPerMonth, WorkingHoursPerDay,
                             DailySalaryCalcMethod, OvertimeBase, OvertimeWeekdayMultiplier,
                             OvertimeWeekendMultiplier, OvertimeHolidayMultiplier,
                             LateDeductionMethod, AbsenceDeductionMethod, LeaveEncashmentMethod,
                             IsActive, CreatedAt)
VALUES ('Standard Policy', 22, 30, 8, 1, 0, 1.5, 2.0, 3.0, 0, 1, 1, 1, GETDATE());
```

### Test 7 — Create a Deduction Limit Policy

```sql
INSERT INTO DeductionLimitPolicies (Name, PayrollPolicyId, MaxDeductionPerMonth, MaxDeductionPerYear,
                                    MaxMoneyPerPenalty, MaxDaysPerPenalty, MaxPercentageOfSalary,
                                    IsActive, CreatedAt)
VALUES ('Standard Limits', 1, 2000, 12000, 500, 2, 25, 1, GETDATE());
```

---

## Enums Reference

### PenaltyStatus
| Value | Name | Description |
|-------|------|-------------|
| 0 | Draft | Created, not yet submitted |
| 1 | PendingApproval | Submitted, awaiting manager review |
| 2 | Approved | Approved — eligible for payroll |
| 3 | Rejected | Rejected — no payroll impact |

### PenaltyCategory
| Value | Name |
|-------|------|
| 0 | Attendance |
| 1 | Behavior |
| 2 | Safety |
| 3 | Performance |
| 4 | Other |

### DeductionUnit
| Value | Name | Description |
|-------|------|-------------|
| 0 | Money | Fixed monetary amount |
| 1 | Hour | Deduct hours of pay |
| 2 | Day | Deduct days of pay |
| 3 | Percentage | Deduct % of salary |
| 4 | WarningOnly | No deduction, just a warning |

### PenaltyValueType
| Value | Name | Description |
|-------|------|-------------|
| 0 | Minutes | Level range measured in minutes |
| 1 | Occurrences | Level range measured in repeat counts |
| 2 | Days | Level range measured in days |

---

*Document generated for the HRplusBasma HR System — Payroll & Disciplinary Modules.*