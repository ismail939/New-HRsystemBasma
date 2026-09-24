# Payroll Integration Test Scenarios

These scenarios describe the complete payroll workflow. Each scenario should use a separate test database or an isolated database transaction.

## Scenario 1: Normal payroll with basic salary only

**Given**

- An active employee has a basic salary of 10,000.
- There are no allowances, deductions, penalties, overtime, tax, or insurance.
- No payroll exists for the selected month.

**When** payroll is generated for the month.

**Then**

- One payroll record is created.
- One payroll detail record is created for the employee.
- Gross salary is 10,000.
- Total deductions are 0.
- Net salary is 10,000.

## Scenario 2: Payroll with allowances

**Given**

- An employee has a basic salary of 10,000.
- The employee has a transport allowance of 1,000 and a housing allowance of 2,000.
- Both allowances are active earnings.

**When** payroll is generated.

**Then**

- Gross salary is 13,000.
- The payroll contains three earning items.
- The allowances appear in payroll earnings.
- Net salary includes the allowances.

## Scenario 3: Payroll with recurring deductions

**Given**

- An employee earns 10,000.
- The employee has an active recurring deduction of 500.

**When** payroll is generated.

**Then**

- Gross salary is 10,000.
- Total deductions include 500.
- Net salary is 9,500.
- The deduction is saved as a payroll deduction and payroll item.

## Scenario 4: Payroll with progressive tax

**Given**

- Tax brackets are 0–10,000 at 10%, 10,000–20,000 at 20%, and above 20,000 at 30%.
- The employee has taxable earnings of 30,000.

**When** payroll is generated.

**Then**

- Tax is calculated progressively: 1,000 + 2,000 + 3,000.
- Tax amount is 6,000.
- Tax is included in total deductions.
- The saved payroll detail contains the correct taxable amount and tax amount.

## Scenario 5: Payroll with insurance limits

**Given**

- The employee has insurable earnings of 20,000.
- The insurance policy has a minimum of 5,000, maximum of 10,000, and employee rate of 10%.

**When** payroll is generated.

**Then**

- The insured base is limited to 10,000.
- Employee insurance deduction is 1,000.
- Insurance is included in total deductions and saved in payroll details.

## Scenario 6: Payroll with approved overtime

**Given**

- Basic salary is 10,000.
- Working days are 20 and working hours per day are 8.
- The employee has 10 approved weekday overtime hours.
- The overtime multiplier is 1.5.

**When** payroll is generated.

**Then**

- Hourly rate is 62.50.
- Overtime pay is 937.50.
- Overtime is included in gross salary.
- An overtime payroll item is created.
- The overtime entry is linked to the payroll item.

## Scenario 7: Unapproved overtime is excluded

**Given**

- The employee has 10 overtime hours.
- The overtime entry is not approved.

**When** payroll is generated.

**Then**

- The unapproved overtime is not included in gross salary.
- No overtime payroll item is created for that entry.
- The overtime entry remains unprocessed.

## Scenario 8: Weekend and holiday overtime use different rates

**Given**

- The employee has approved overtime on a weekday, weekend, and holiday.
- Each overtime type has a different configured multiplier.

**When** payroll is generated.

**Then**

- Weekday hours use the weekday multiplier.
- Weekend hours use the weekend multiplier.
- Holiday hours use the holiday multiplier.
- The total overtime pay equals the sum of the three calculations.

## Scenario 9: Approved money penalty

**Given**

- The employee has an approved penalty of 500 using the money unit.
- The penalty belongs to the payroll month.

**When** payroll is generated.

**Then**

- 500 is added to total deductions.
- A penalty payroll item and deduction row are created.
- The penalty is linked to the created payroll item.
- The penalty amount is saved as 500.

## Scenario 10: Approved day and percentage penalties

**Given**

- The employee has an approved one-day penalty.
- The employee has an approved 10% penalty.
- The daily rate and basic salary are known.

**When** payroll is generated.

**Then**

- The day penalty equals one daily salary rate.
- The percentage penalty equals 10% of basic salary.
- Both penalties are included in total deductions.
- Both penalty records are linked to payroll items.

## Scenario 11: Warning-only penalty is ignored financially

**Given**

- The employee has an approved warning-only penalty.

**When** payroll is generated.

**Then**

- No money is added to total deductions.
- No financial penalty payroll item is created.
- The warning remains recorded as a disciplinary event.

## Scenario 12: Penalties from another month are excluded

**Given**

- The employee has an approved penalty dated outside the payroll month.

**When** payroll is generated.

**Then**

- The penalty is not included in the current payroll.
- No payroll item is created for it.
- The penalty remains available for the correct payroll period.

## Scenario 13: Leave and attendance affect payroll statistics

**Given**

- The employee has present, absent, paid-leave, unpaid-leave, and official-holiday attendance records.

**When** payroll is generated.

**Then**

- Present days are counted correctly.
- Absent days are counted correctly.
- Paid leaves are counted correctly.
- Unpaid leaves are counted correctly.
- Official holidays are counted correctly.
- Late minutes and overtime hours are summed correctly.

## Scenario 14: Duplicate payroll generation is rejected

**Given**

- A payroll already exists for the selected month and year.

**When** payroll generation is requested again for the same month and year.

**Then**

- A failure response is returned.
- No second payroll header is created.
- No duplicate payroll details or payroll items are created.

## Scenario 15: Employee with incomplete payroll data

**Given**

- An active employee has no basic salary or has incomplete payroll configuration.

**When** payroll is generated.

**Then**

- The application follows the defined missing-data policy.
- It either creates a zero-value payroll detail or reports a clear validation error.
- No invalid negative values are saved.
- The failure or handling decision is recorded in logs when appropriate.

## Suggested integration-test structure

```text
HRsystem.Tests/
└── Integration/
    ├── PayrollGenerationTests.cs
    ├── PayrollTaxTests.cs
    ├── PayrollOvertimeTests.cs
    ├── PayrollPenaltyTests.cs
    └── PayrollAttendanceTests.cs
```

Each test should arrange database data, execute the application service or controller workflow, and then assert both the returned result and the records saved in the database.
