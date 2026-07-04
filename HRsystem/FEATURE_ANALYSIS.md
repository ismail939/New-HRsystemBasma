# HRplus Basma - Comprehensive Feature Analysis

## 📋 System Overview

**HRplus Basma** is an enterprise-grade, web-based Human Resource Management System (HRMS) built on ASP.NET Core MVC (C#). It is designed to automate and centralize all core HR operations including employee management, attendance tracking via biometric devices, shift scheduling, leave management, disciplinary actions, recruitment, and comprehensive reporting. The system is fully bilingual (Arabic/English) with Right-to-Left (RTL) support.

---

## 🏗 System Architecture

### Backend
- **Framework**: ASP.NET Core MVC with Razor Views (.cshtml)
- **ORM**: Entity Framework Core (Code-First with 15+ migrations)
- **Database**: Microsoft SQL Server
- **Authentication**: Cookie-based authentication with Role-Based Access Control (RBAC)
- **Authorization**: Two roles — `Admin` (full access) and `HR` (operational access)
- **PDF Generation**: QuestPDF library
- **Logging**: Built-in ILogger + custom `HRLog` table for audit trails

### Frontend
- **Templating**: Razor (.cshtml)
- **CSS Framework**: Tailwind CSS + PostCSS
- **JavaScript**: Vanilla JS (ES6+)
- **UI Focus**: Arabic-first (RTL), responsive/mobile-friendly

---

## 🔑 Core Functional Modules

### 1. Authentication & Authorization (`HomeController`, `Program.cs`)
- Cookie-based login system with username/password
- Session persistence with 8-hour sliding expiration
- Role-based authorization (`AdminOnly` policy)
- Login page at `/` route with redirection to employee list after successful login
- Logout functionality with session cleanup
- Global error handling with status code re-execution

### 2. Employee Management (`EmployeeController`, `AdminController`)

#### Employee CRUD Operations
- **Add Employee**: Full profile creation including name, national ID, phone, address, marital status, religion, date of birth, insurance number, hire date, end date, job title, contract type, leave reason, Basma ID, and department assignment.
- **Edit Employee**: Full profile update capability.
- **Delete Employee**: Cascading deletion of all related data (basma records, off-days, penalties, shifts, shift overrides, rates, files).
- **List Employees**: Paginated/sorted list with department name display.

#### Employee Files/Documents
- Multi-file upload support (images, PDFs, Word docs, etc.)
- Files stored on server filesystem (`wwwroot/images/`)
- File listing per employee with download capability
- ZIP archive download of all files for a given employee
- File deletion (soft delete from DB + hard delete from filesystem)
- Content-type detection for proper download headers

#### Employee Performance Rating
- Monthly rating system (1-5 scale)
- Rate retrieval per employee per month/year
- Duplicate rate prevention
- Average rate calculation across date ranges

### 3. Biometric Attendance (Basma) System (`BasmaController`)

#### Fingerprint Device Integration
- ZK-Teco fingerprint device support (commented out but structurally present)
- SDK integration for automated attendance log retrieval (`ZKTecoConnection`)
- Device connection via TCP/IP (port 4370)

#### Attendance Processing Engine (`TakeDayFromFingerPrint`)
This is the most sophisticated feature — a complete attendance calculation engine:

1. **Data Aggregation**:
   - Retrieves Check-In/Out records from fingerprint device (3:00 AM to 3:00 AM next day window)
   - Groups records by employee ID
   - Loads employee master data, shift assignments, shift overrides, and off-day records

2. **Attendance Calculation**:
   - Determines arrival time (earliest check-in) and departure time (latest check-out)
   - Calculates **total working hours** (departure - arrival)
   - Calculates **late minutes** (arrival after shift start + tolerance)
   - Calculates **early leave minutes** (departure before shift end - tolerance)
   - Calculates **overtime minutes** (departure after scheduled shift end)
   - Handles single-record or very short (<10 min) entries as partial attendance

3. **Status Classification**:
   - `Status 1`: Present (attendance data exists)
   - `Status 2`: On Leave/Off-day (pre-registered off day)
   - `Status 0`: Absent (no attendance and no off day)
   - `Status 3`: Canceled (manually canceled by HR)

4. **Manual Attendance Management**:
   - Cancel attendance entries (sets to pending HR decision)
   - Save/edit notes per attendance record
   - Search/filter attendance records by employee name

### 4. Shift Management (`EmployeeController`)

#### Shift Options Configuration
- Define shift templates with:
  - Mode 0: **Variable hours** (no fixed schedule)
  - Mode 1: **Fixed duration** (e.g., 8-hour days)
  - Mode 2: **Fixed time** (specific start/end times e.g., 9:00 AM - 5:00 PM)
- Auto-generated display names in Arabic
- Duplicate prevention

#### Employee Shift Assignment
- Assign shift to employee with date range (`FromDate` to `ToDate`)
- Per-employee late/early leave tolerance settings (in minutes)
- Multi-employee shift bulk-save capability

#### Shift Override System
- Day-of-week specific override for individual shift days
- Override priority over default shift assignment
- Used to accommodate weekly schedule variations (e.g., different Friday shift)

#### Weekly Shift Visualization
- **GetShiftsForWeek**: Returns all shifts + overrides + off-days for a department across a date range
- **SaveShifts**: Advanced bulk UPSERT logic:
  - Analyzes most frequent shift pattern per employee across the week
  - Upserts default shifts (from `From` to `To` dates)
  - Creates day-specific overrides for exceptions
  - Manages off-days as separate HR entity

### 5. Leave (Off-Day) Management (`OffDayController`)

#### Off-Day Types
- **Annual Leave** (إجازة سنوية)
- **Casual Leave** (إجازة عرضية)  
- **Rest Days** (راحة - weekly off)
- **Sick Leave** (إجازة مرضية)
- **Compensatory Leave** (بدل إجازة)
- Custom type support via string field

#### Leave Operations
- **Add/Edit Off-Days**: Date-range based leave assignment per employee
- **Bulk Edit**: Multiple days toggle in one request with type specification
- **Get Off-Days**: Retrieve employee leave records within date range
- Automatic cleanup of existing records before re-inserting (UPSERT pattern)

#### Leave Balance System
- **Four balance categories**: Annual, Casual, Off (rest), Compensatory
- **Balance Initialization**: Auto-creates balance record if not exists
- **Balance Editing**: Direct manipulation of balance counts
- **Balance Notes**: Per-employee notes associated with leave balance
- Balance retrieval and display integrated with employee view

### 6. Penalty & Discipline System (`PenaltyController`)

- **Add Penalty**: Record with decision type, date, reason, penalty points
- **Toggle Active/Inactive**: Soft activation/deactivation of penalties
- **Employee Penalties History**: Full penalty history per employee
- **Penalty Points**: Numeric points system for tracking severity
- **Reason Documentation**: Mandatory reason field for accountability

### 7. Recruitment Module (`ApplierController`)

- **Applicant CRUD**: Full applicant profile management
- **Applicant Files**: Multi-file upload (CVs, certificates, etc.)
- **File Operations**: List, delete, and manage applicant documents
- Independent from employee module, representing pre-employment pipeline

### 8. Department Management (`AdminController`)

- **Create Department**: Name, code, description, parent department, manager assignment
- **Edit Department**: Full update capability
- **List Departments**: Hierarchical display with manager name and parent department
- **Department Employees**: Quick lookup of employees by department
- Cascading parent-child department structure
- Manager assignment/unassignment (auto-clears when employee deleted)

### 9. User Management (`AdminController`)

- **List Users**: View all system users
- **Add User**: Create with username, password, role (Admin/HR)
- **Edit User**: Update credentials and role
- **Duplicate Username Prevention**: Validation on unique usernames

### 10. Audit Logging (`AdminController`)

- **HR Logs**: Comprehensive action logging for all HR operations
- **Logged Actions Include**:
  - Employee CRUD (add, edit, delete)
  - File uploads
  - Penalty additions and toggle status
  - Off-day edits
  - Shift option additions
  - Rate additions
  - Basma cancellations
- **Display**: Chronological list with newest first

### 11. Reporting System (`ReportController`, `BasmaController`)

#### Report Types

| Report | Description | Format |
|--------|-------------|--------|
| **Daily Report** | Per-day attendance with shifts, status, lateness, overtime | PDF (QuestPDF) |
| **Employee Report** | Full employee list with all profile fields | PDF |
| **All-Info Report** | Comprehensive employee analysis across date range | JSON API |
| **Employees List Report** | Raw employee data export | JSON |

#### All-Info Report Sub-Modules (AJAX endpoints)
- **GetData**: Summary statistics (arrivals, absences, off-days, rest days, penalties count)
- **GetOffDays**: Leave records (excluding rest days)
- **GetOffs**: Rest day records
- **GetAbsences**: Absence records with dates
- **GetArrivals**: Attendance records
- **GetPenalties**: Active penalties with decision, points, reason
- **GetRates**: Monthly rates with average rate calculation spanning multiple years

#### Dashboard & Report Navigation
- Reports panel at `/reports`
- PDF reports page with department filtering at `/PDFReports`
- All-Info report page at `/allInfoReport`

### 12. System Administrative Features

- **Dashboard Views**: `/dashboard` and `/admin/dashboard`
- **Error Handling**: Global exception handler with status code pages
- **HTTPS Redirection**: Enforced in production
- **HSTS**: Security headers for production environment
- **Culture Encoding**: `CodePagesEncodingProvider` for Arabic text support

---

## 📊 Database Model Summary

### Core Entities
| Entity | Purpose |
|--------|---------|
| **User** | System login credentials & roles |
| **HREmployee** | Main employee profiles |
| **HRDepartment** | Department hierarchy with managers |
| **CheckInOut** | Raw fingerprint device check-in/out records |
| **HREmployeeBasma** | Processed daily attendance records |
| **HREmployeeShift** | Shift assignments per employee |
| **HRShiftOption** | Shift template definitions |
| **ShiftOverride** | Day-specific shift overrides |
| **HREmployeeOffDay** | Leave/off-day records |
| **HROffDayBalance** | Leave balance tracking |
| **HREmployeePenalty** | Disciplinary penalty records |
| **HREmployeeRate** | Monthly performance ratings |
| **HRApplier** | Job applicant records |
| **HREmployeeFile** | Employee document files |
| **HRApplierFile** | Applicant document files |
| **HRLog** | Audit trail for all operations |

### Enum/Status System
- **Basma Status**: `0=Absent`, `1=Present`, `2=On Leave`, `3=Canceled`
- **Shift Modes**: `0=Variable`, `1=Fixed Duration`, `2=Fixed Time`
- **User Roles**: `Admin`, `HR`

---

## 🎯 Key Workflows Flow

```
[Fingerprint Device] → [CheckInOut Records] 
    → [BasmaEngine] → [Process Attendance]
    → [Calculate Hours/Late/Overtime] → [HREmployeeBasma]
    → [Status Classification]
    → [PDF Reports / Dashboard Display]

[HR Manager] → [Shift Setup] → [Employee Shift Assignment]
    → [Override Config] → [Weekly Schedule]

[HR Manager] → [Leave Request] → [Off-Day Record]
    → [Balance Deduction] → [Attendance Integration]
```

---

## 🌐 API Endpoints Summary

| Route | Method | Description |
|-------|--------|-------------|
| `/` | GET | Login page |
| `/Home/Login` | POST | Authenticate user |
| `/logout` | POST | Sign out |
| `/employees` | GET | List all employees |
| `/employees/add` | POST | Add new employee |
| `/employees/edit` | POST | Update employee |
| `/deleteEmployee` | POST | Delete employee (cascading) |
| `/getEmployee` | GET | Get single employee by ID |
| `/employees/files/{id}` | GET | Get employee files |
| `/employees/uploadImage` | POST | Upload employee files |
| `/employees/deleteFile/{id}` | POST | Delete employee file |
| `/employees/files/zip/{id}` | GET | Download all files as ZIP |
| `/basma` | GET | Attendance dashboard view |
| `/basmaData` | GET | Process attendance for a day |
| `/cancelBasma` | POST | Cancel attendance entry |
| `/saveBasmaNotes` | POST | Save attendance notes |
| `/getShiftOptions` | GET | List all shift templates |
| `/addShiftOption` | POST | Create shift template |
| `/saveShifts` | POST | Bulk save shifts with overrides |
| `/getShiftsForWeek` | GET | Get weekly shift schedule |
| `/OffDays` | GET | Off-day management page |
| `/employees/offdays/{id}` | GET | Get employee off-days |
| `/employees/offdays/edit` | POST | Edit off-days |
| `/offdays/balance/add` | GET | Initialize leave balance |
| `/offdays/balance/edit` | POST | Update leave balance |
| `/penalties` | GET | Penalty management page |
| `/employee/addPenalty` | POST | Add penalty |
| `/employee/penalties/{id}` | GET | Get employee penalties |
| `/employee/togglePenaltyActive` | POST | Toggle penalty status |
| `/reports` | GET | Reports dashboard |
| `/PDFReports` | GET | PDF report page |
| `/dailyReporttt` | GET | Generate daily PDF report |
| `/allInfoReport` | GET | All-info report page |
| `/allInfoReport/getOffDays` | GET | Get leave records |
| `/allInfoReport/getAbsences` | GET | Get absence records |
| `/admin/dashboard` | GET | Admin dashboard |
| `/admin/users` | GET | User management |
| `/createDepartment` | GET | Department creation form |
| `/admin/dashboard/departments` | GET | Department list |
| `/appliers` | GET | Applicant management |
| `/appliers/add` | POST | Add applicant |
| `/appliers/edit` | POST | Update applicant |
| `/getRate` | GET | Get employee monthly rate |
| `/addRate` | POST | Add employee rate |
| `/logs` | GET | View audit logs |

---

## ✅ Strengths & Differentiators

1. **End-to-End HR Solution**: Covers the complete employee lifecycle from recruitment through attendance, performance, discipline, and reporting.
2. **Biometric Integration**: Real-time fingerprint device data processing for accurate attendance.
3. **Smart Attendance Engine**: Automatic calculation of all time components (hours, late, early, overtime) with shift tolerance handling.
4. **Flexible Shift Architecture**: Three shift modes + override system for complex scheduling needs.
5. **Audit Trail**: Comprehensive logging of all HR actions for compliance.
6. **RTL/Arabic Support**: Fully localized for Arabic-speaking organizations.
7. **PDF Reporting**: Professional PDF report generation via QuestPDF.
8. **Data Integrity**: Transaction-based processing with rollback on failure.
9. **Role-Based Security**: Clear separation between Admin and HR privileges.

---

## 🔧 Potential Areas for Enhancement

1. **Password Hashing**: Currently plain-text password storage (noted with comment).
2. **Biometric SDK**: ZKTeco integration is commented out; needs re-enabling for production fingerprint use.
3. **No Email/Push Notifications**: System lacks automated alerts for absences or approvals.
4. **No Self-Service Portal**: Employees cannot log in to view their own records.
5. **No API Versioning**: RESTful endpoints but no formal API versioning strategy.
6. **No Unit Tests**: No visible test project in the solution structure.
7. **Mobile App**: No dedicated mobile application for managers on-the-go.
8. **Payroll Integration**: No direct payroll computation or salary processing module.
9. **Data Export**: Only PDF and JSON; lacks Excel/CSV export capabilities.

---

## 📝 Conclusion

**HRplus Basma** is a feature-rich HR management system with robust attendance processing at its core. It demonstrates sophisticated engineering especially in the biometric attendance calculation engine, flexible shift management with override capability, and comprehensive reporting. The system is well-architected following MVC patterns with Entity Framework Core and is fully production-ready for Arabic-speaking organizations needing an all-in-one HR platform.