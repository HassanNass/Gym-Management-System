# Gym Management System

A Windows Forms desktop application built with **C# / .NET Framework** and **SQL Server**, developed as a capstone project for a Programming Diploma. The system allows gym staff and administrators to manage members, plans, and users through a clean role-based interface connected to a real database.

---

## Features

### Authentication
- Login form with username and password validation
- Show/hide password toggle
- Role-based access control — the logged-in user's role determines what they can access

### Role Permissions

| Feature | Staff | Admin |
|---|---|---|
| View & Manage Members | ✅ | ✅ |
| View & Manage Plans | ❌ | ✅ |
| View & Manage Users | ❌ | ✅ |

### Dashboard
Displays live statistics pulled directly from the database:
- **Total Members** — all members in the system
- **Active Members** — members currently marked as active
- **Expiring Soon** — active members whose plan expires within the next 30 days (calculated using join date + plan duration)

### Members Form
- Add, update, and delete members
- Fields: First Name, Last Name, Phone, Join Date (DateTimePicker), Plan (ComboBox), Active Status (Checkbox)
- Upload and display a member photo (saved locally in an `Images` folder)
- Search members by last name in real time
- Click any row in the DataGridView to load that member's data into the form
- Access the Member Report from this form

### Plans & Users Forms
- Full CRUD operations for gym plans and system users
- DataGridView display with inline selection

### Member Report
- Generate a printable report using **Microsoft RDLC Report Viewer**
- View all members or filter by a specific Member ID
- Report data is pulled from a dedicated SQL Server view (`MEMBERS_VIEW`)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET Framework 4.7.2) |
| UI Framework | Windows Forms |
| Database | SQL Server Express (LocalDB) |
| DB Connection | ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter`) |
| Reporting | Microsoft RDLC Report Viewer (WinForms) |
| Config | `App.config` with named connection strings |

---

## Project Structure

```
GymManagement_Project/
├── Forms/
│   ├── LoginForm.cs
│   ├── DashboardForm.cs
│   ├── MembersForm.cs
│   ├── PlansForm.cs
│   ├── UsersForm.cs
│   └── ReportForm.cs
├── Models/
│   ├── Member.cs          + MemberRepository
│   ├── Dashboard.cs       + DashboardRepository
│   ├── Plan.cs            + PlanRepository
│   └── User.cs            + UserRepository
├── DBHelper.cs
├── App.config
└── Images/                (member photos stored here at runtime)
```

---

## Database Connection

The app uses a centralized `DBHelper` static class to manage the SQL Server connection:

```csharp
public static SqlConnection GetConnection()
{
    return new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
}
```

Connection string is stored in `App.config` and points to a local SQL Server Express instance:

```
Data Source=localhost\SQLEXPRESS;Initial Catalog=GymSystemDB;Integrated Security=True
```

---

## How to Run

1. Make sure **SQL Server Express** is installed and running on your machine
2. Create a database named `GymSystemDB` and run the SQL scripts to set up the tables and views
3. Open the solution in **Visual Studio**
4. Build the project — NuGet packages will restore automatically
5. Run the application and log in with a valid username and password from the database

---

## Key Implementation Details

- **Repository pattern** — each model has its own repository class that handles all SQL operations, keeping the form code clean
- **Parameterized queries** — all database operations use `AddWithValue` to prevent SQL injection
- **Expiry logic** — expiring members are calculated entirely in SQL using `DATEADD` on join date and plan duration, so no date logic lives in the UI
- **Image handling** — member photos are copied to a local `Images` folder with a `GUID`-based filename to avoid naming conflicts
- **Real-time search** — the members list filters by last name on every keystroke using LINQ on the fetched data

---

## Author

**Hassan Nasrallah**
[github.com/HassanNass](https://github.com/HassanNass)
