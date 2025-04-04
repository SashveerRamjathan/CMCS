# 📊 Contract Monthly Claim System (CMCS)

## 🚀 Overview

The **Contract Monthly Claim System (CMCS)** is a **web-based application** designed to streamline the submission and approval of monthly claims for **Independent Contractor (IC)** lecturers. This system facilitates seamless interactions between **Lecturers**, **HR Personnel**, and **Academic Managers** to ensure an efficient claims process.

Lecturers can submit claims for hours worked, upload supporting documents, and track claim statuses. Academic Managers and HR personnel can review, approve, or reject claims in real-time. The system also includes features such as **role-based access control**, automatic **invoice and report generation**, and **document uploads**.

---

## 🔧 Tech Stack

- **C#** for backend programming
- **.NET Core** and **ASP.NET Core MVC** for the web framework
- **Entity Framework Core** for database access
- **QuestPDF** for generating invoices and reports
- **FluentValidation** for ensuring data integrity
- **SQL Server** for database management
- **HTML, CSS, JavaScript (jQuery)** for frontend enhancements

---

## 🎯 Key Features

### 1. **User Management** 🔑
- Manage different user roles: **HR**, **Lecturers**, and **Academic Managers**.
- Role-based access control ensuring only relevant features are available to each user.

### 2. **Claim Submission** 💼
- **Lecturers** can submit claims via a simple and intuitive form.
- Fields include **hours worked**, **hourly rate**, and **additional notes**.
- **Document upload** feature allows lecturers to submit supporting files (PDF, DOCX, XLSX).

### 3. **Claims Approval** ✅❌
- **Academic Managers** and **HR** can review, approve, or reject claims.
- Each claim has a clear status (e.g., **Pending**, **Approved**, **Rejected**) for easy tracking.
  
### 4. **Claim Tracking** 📊
- Track the progress of claims in real-time with **status updates**.

### 5. **Automatic Invoice & Report Generation** 📑💸
- Generates professional invoices and detailed claims reports in **PDF** format using **QuestPDF**.
- Includes summaries like **total claims**, **average amount**, and **approved claims**.

### 6. **FluentValidation for Data Integrity** 🔒
- Validates all data entries to ensure accuracy and completeness.
- Provides real-time feedback to users with error messages for invalid input.

---

## 🔄 **Features in Detail**

### 🖥️ **Lecturer View**  
Lecturers can:  
- Submit claims anytime with an easy-to-use form.  
- Upload supporting documents.  
- Track the status of claims as they move through the approval process.

### 📋 **Academic Manager & HR View**  
Academic Managers and HR can:  
- View all **pending claims**.  
- **Approve or Reject** claims.  
- **Download supporting documents**.  
- Track **claim statuses** in real-time.

### 🧾 **Report & Invoice Generation**  
Generate and download professional reports and invoices automatically.  
- **PDF format**: High-quality and ready to print.  
- **Automated workflows** for consistent formatting and easier record-keeping.

### 🔒 **Data Validation**  
- Ensures that only valid data is entered using **FluentValidation**.
- Guarantees that claims are valid and complete before being processed.

---

## 📁 **Project Structure**

### Controllers
- **ClaimsController**: Manages claim submission and approval.
- **DashboardsController**: Provides role-specific dashboards.
- **StaffController**: Manages staff details and roles.
- **RegistrationsController**: Manages user registrations and access permissions.
- **ReportController**: Generates claim reports.
- **InvoiceController**: Generates and saves claim invoices.

### Services
- **SeedDatabase**: Seeds initial data (roles, users).
- **ReportInformationService**: Aggregates data for reports.
- **ReportGenerationService**: Generates reports in PDF.
- **InvoiceGenerationService**: Generates invoices in PDF using **QuestPDF**.

### Fluent Validators
- **ClaimValidator**: Ensures valid claims are submitted.
- **UpdateLecturerDetailsValidator**: Ensures lecturer data integrity.
- **UpdateAcademicManagerValidator**: Ensures academic manager data integrity.

### Database Models
- **ApplicationUser**: Represents users (Lecturers, Academic Managers, HR).
- **Claim**: Represents claims submitted by lecturers.
- **Invoice**: Represents generated invoices.
- **Report**: Represents generated reports.

---

## 💡 **What I Learned** 📚

Through building this project, I gained valuable experience in:

* **ASP.NET Core MVC**: Gaining a deeper understanding of how to build dynamic web applications using this powerful framework.
  
* **Entity Framework Core**: Learning how to interact with SQL Server databases, perform CRUD operations, and handle migrations.
  
* **Role-based Access Control (RBAC)**: Implementing user roles with specific access to features and functionality, which was essential for creating a multi-user system.
  
* **QuestPDF**: Using QuestPDF for generating professional invoices and reports. This provided a chance to dive into PDF generation and automated report creation.
  
* **FluentValidation**: Applying data validation techniques to ensure that only correct and complete data is processed, providing real-time feedback to users.
  
* **Frontend Development**: Improving my HTML, CSS, and JavaScript skills while building a user-friendly interface.
  
* **Project Management**: Managing both backend and frontend development, as well as the integration of various components like database management, PDF generation, and validation.

This project provided me with a hands-on opportunity to learn and apply modern web development practices, making me more proficient in the full-stack development process.

