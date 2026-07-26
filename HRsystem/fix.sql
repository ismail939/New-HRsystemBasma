BEGIN TRANSACTION;
GO

CREATE TABLE [EmployeeSalaries] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [SalaryComponentId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_EmployeeSalaries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeSalaries_HREmployees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [HREmployees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeSalaries_SalaryComponents_SalaryComponentId] FOREIGN KEY ([SalaryComponentId]) REFERENCES [SalaryComponents] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Payrolls] (
    [Id] int NOT NULL IDENTITY,
    [Month] int NOT NULL,
    [Year] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [GeneratedDate] datetime2 NOT NULL,
    [GeneratedBy] nvarchar(100) NULL,
    [ReviewedDate] datetime2 NULL,
    [ReviewedBy] nvarchar(100) NULL,
    [ApprovedDate] datetime2 NULL,
    [ApprovedBy] nvarchar(100) NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_Payrolls] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SalaryHistories] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [SalaryComponentId] int NULL,
    [PreviousValue] decimal(18,2) NULL,
    [NewValue] decimal(18,2) NOT NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [Reason] nvarchar(500) NULL,
    [ChangedBy] nvarchar(100) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_SalaryHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SalaryHistories_HREmployees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [HREmployees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SalaryHistories_SalaryComponents_SalaryComponentId] FOREIGN KEY ([SalaryComponentId]) REFERENCES [SalaryComponents] ([Id])
);
GO

CREATE TABLE [PayrollDetails] (
    [Id] int NOT NULL IDENTITY,
    [PayrollId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    [BasicSalary] decimal(18,2) NOT NULL,
    [TotalEarnings] decimal(18,2) NOT NULL,
    [TotalDeductions] decimal(18,2) NOT NULL,
    [GrossSalary] decimal(18,2) NOT NULL,
    [NetSalary] decimal(18,2) NOT NULL,
    [PresentDays] int NOT NULL,
    [AbsentDays] int NOT NULL,
    [LateMinutes] float NOT NULL,
    [OvertimeHours] float NOT NULL,
    [PaidLeaves] int NOT NULL,
    [UnpaidLeaves] int NOT NULL,
    [OfficialHolidays] int NOT NULL,
    [DailySalaryRate] decimal(18,2) NOT NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_PayrollDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PayrollDetails_HREmployees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [HREmployees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PayrollDetails_Payrolls_PayrollId] FOREIGN KEY ([PayrollId]) REFERENCES [Payrolls] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PayrollDeductions] (
    [Id] int NOT NULL IDENTITY,
    [PayrollDetailId] int NOT NULL,
    [SalaryComponentId] int NULL,
    [Name] nvarchar(100) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_PayrollDeductions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PayrollDeductions_PayrollDetails_PayrollDetailId] FOREIGN KEY ([PayrollDetailId]) REFERENCES [PayrollDetails] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PayrollDeductions_SalaryComponents_SalaryComponentId] FOREIGN KEY ([SalaryComponentId]) REFERENCES [SalaryComponents] ([Id])
);
GO

CREATE TABLE [PayrollEarnings] (
    [Id] int NOT NULL IDENTITY,
    [PayrollDetailId] int NOT NULL,
    [SalaryComponentId] int NULL,
    [Name] nvarchar(100) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Notes] nvarchar(500) NULL,
    CONSTRAINT [PK_PayrollEarnings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PayrollEarnings_PayrollDetails_PayrollDetailId] FOREIGN KEY ([PayrollDetailId]) REFERENCES [PayrollDetails] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PayrollEarnings_SalaryComponents_SalaryComponentId] FOREIGN KEY ([SalaryComponentId]) REFERENCES [SalaryComponents] ([Id])
);
GO

CREATE INDEX [IX_EmployeeSalaries_EmployeeId] ON [EmployeeSalaries] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeSalaries_SalaryComponentId] ON [EmployeeSalaries] ([SalaryComponentId]);
GO

CREATE INDEX [IX_PayrollDeductions_PayrollDetailId] ON [PayrollDeductions] ([PayrollDetailId]);
GO

CREATE INDEX [IX_PayrollDeductions_SalaryComponentId] ON [PayrollDeductions] ([SalaryComponentId]);
GO

CREATE INDEX [IX_PayrollDetails_EmployeeId] ON [PayrollDetails] ([EmployeeId]);
GO

CREATE INDEX [IX_PayrollDetails_PayrollId] ON [PayrollDetails] ([PayrollId]);
GO

CREATE INDEX [IX_PayrollEarnings_PayrollDetailId] ON [PayrollEarnings] ([PayrollDetailId]);
GO

CREATE INDEX [IX_PayrollEarnings_SalaryComponentId] ON [PayrollEarnings] ([SalaryComponentId]);
GO

CREATE INDEX [IX_SalaryHistories_EmployeeId] ON [SalaryHistories] ([EmployeeId]);
GO

CREATE INDEX [IX_SalaryHistories_SalaryComponentId] ON [SalaryHistories] ([SalaryComponentId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260725102334_fixPayrollDetailFloatToDouble', N'8.0.8');
GO

COMMIT;
GO

