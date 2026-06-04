# HRplus Basma - Enterprise HR Management System

A comprehensive, web-based Human Resource Management System designed for organizations to manage employee attendance, leave, shifts, and HR operations with integrated biometric support.

---

## 📋 Table of Contents

- [Tech Stack](#tech-stack)
- [Features](#core-hr-system-features)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [License](#license)

---

## 🛠️ Tech Stack

### **Backend**
- **Framework**: ASP.NET Core MVC (latest)
- **ORM**: Entity Framework Core with code-first migrations
- **Database**: Microsoft SQL Server
- **Authentication**: Cookie-based authentication with role-based access control (RBAC)
- **API**: RESTful JSON APIs for dynamic operations
- **Logging**: Built-in ILogger for audit trails

### **Frontend**
- **Markup**: HTML5 with Razor templating (.cshtml)
- **Styling**: Tailwind CSS framework
- **Scripting**: JavaScript (ES6+)
- **RTL Support**: Full Right-to-Left (Arabic) language support
- **Responsive Design**: Mobile-first approach

### **Biometric Integration**
- **Device Support**: ZK-Teco fingerprint device integration
- **SDK**: ZK-Teco SDK for automated attendance capture

### **Reporting & Export**
- **PDF Generation**: QuestPDF library for report generation
- **Report Types**: Multiple report formats and templates

### **Development Tools**
- **CSS Processing**: Tailwind CSS with PostCSS
- **Package Manager**: npm for JavaScript dependencies
- **Project File**: .csproj with NuGet package management

---

## 🎯 Core HR System Features

### **1. Employee Management**
- Complete employee profile management with personal information (name, ID, DOB, contact, address, insurance number)
- Department assignment with hierarchical structure
- Employee status tracking (hire date, end date, contract type)
- Multi-document file upload support

### **2. Attendance & Biometric Integration**
- Automated attendance tracking via ZK-Teco fingerprint devices
- Real-time arrival/departure time recording
- Automatic calculation of working hours, late minutes, early leave, and overtime
- Attendance status management (Present, Absent, On Leave, Combinations)
- Configurable tardiness tolerance settings per shift

### **3. Shift Management**
- Flexible shift configuration (morning, night, variable hours)
- Multiple shift modes: Fixed time, Variable hours, or Fixed duration
- Per-employee shift assignment with date ranges
- Shift override capability for specific dates
- Individual late/early leave tolerance settings

### **4. Leave Management**
- Off-day type categorization (Annual, Casual, Rest days, Leave compensation)
- Leave balance tracking and display per employee
- Date-range based leave request processing
- Historical leave records and management

### **5. Penalty & Discipline System**
- Structured penalty recording with decision types and penalty points
- Reason documentation for disciplinary actions
- Active/inactive penalty status tracking
- Historical penalty records

### **6. Recruitment Module**
- Job applicant database and tracking
- Applicant status management
- Document upload support (CVs, certifications, etc.)

### **7. Comprehensive Reporting**
- Advanced analytics dashboard
- Multi-format reports (Employee lists, Attendance summaries, Department-specific)
- PDF report generation
- Customizable date-range filtering
- Statistical calculations (arrivals, absences, leaves, penalties)

### **8. Administration & Security**
- Role-based access control (Admin, HR)
- User management
- Department and manager hierarchy management
- Complete audit logging of all HR operations

---

## 📦 Installation

### Prerequisites
- .NET 6.0 or higher
- SQL Server 2019 or higher
- Node.js 14+ (for Tailwind CSS build)
- Visual Studio 2022 or Visual Studio Code

### Setup Steps

1. **Clone the repository**
```bash
git clone <repository-url>
cd HRsystem
```

2. **Install .NET dependencies**
```bash
dotnet restore
```

3. **Install npm dependencies**
```bash
npm install
```

4. **Configure database connection**
- Update `appsettings.json` with your SQL Server connection string
- Example:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=HRSystemDB;Trusted_Connection=true;"
}
```

5. **Apply database migrations**
```bash
dotnet ef database update
```

6. **Build Tailwind CSS**
```bash
npm run build:css
```

7. **Run the application**
```bash
dotnet run
```

The application will be available at `https://localhost:5001`

---

## 🚀 Getting Started

### Default Roles
- **Admin**: Full system access, user management, system configuration
- **HR**: HR operations, employee management, attendance, leaves, reports


### Initial Setup
1. Create an admin user through the user management interface
2. Create departments and assign managers
3. Configure shift options and settings
4. Register employees and assign to departments
5. Configure biometric device (if using ZK-Teco integration)

---

## 📁 Project Structure

```
HRsystem/
├── Controllers/          # MVC controllers for all features
├── Models/              # Entity models for database
├── Views/               # Razor templates (.cshtml files)
├── Data/                # Database context and connections
├── Migrations/          # EF Core database migrations
├── ViewModels/          # View-specific data models
├── Reports/             # Report generation classes
├── wwwroot/             # Static files (CSS, JS, images)
├── Helpers/             # Utility functions
├── Program.cs           # Application startup configuration
└── appsettings.json     # Configuration settings
```

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 👥 Support

For issues, questions, or contributions, please contact the development team or submit an issue in the repository.

---

**HRplus Basma** - Professional HR Management for the Modern Organization
