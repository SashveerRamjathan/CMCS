# Contract Monthly Claim System (CMCS)

## Overview

The Contract Monthly Claim System (CMCS) is a web-based application designed to streamline the submission and approval of monthly claims for Independent Contractor (IC) lecturers. This system facilitates interactions between lecturers, HR personnel, and Academic Managers to ensure a smooth and efficient claims process.

The CMCS allows lecturers to submit claims for hours worked, with the option to attach supporting documents. Academic Managers and HR personnel can review, approve, or reject claims, with real-time status updates to track the progress of each claim.

The system is designed with role-based access control, ensuring that users can only access functionalities relevant to their roles. It includes features such as user management, claims submission, claims approval, document upload, and claim tracking and automatic report and invoice generation.

## Demonstration Video Link

The demonstration video for the Contract Monthly Claim System (CMCS) can be viewed here:

{GOES HERE}

## Features

- **User Management**: Manage different user roles including HR, Lecturers, and Academic Managers.
  
- **Claims Submission**: Lecturers can submit claims for hours worked with a simple and intuitive form.
  
- **Claims Approval**: Academic Managers and HR personnel can review, approve, or reject claims.
  
- **Document Upload**: Lecturers can upload supporting documents for their claims.
  
- **Claim Tracking**: Track the status of claims transparently until they are settled.
  
- **Role-Based Access Control**: Different functionalities are accessible based on user roles.
  
- **Database Seeding**: Automatically seed roles and users into the database on application start-up.
  
- **Automatic Invoice and Report Generation**: Generates professional invoices and detailed reports for claims using QuestPDF, reducing manual effort.

### Automatic Report and Invoice Generation

A new feature integrates the QuestPDF library to generate invoices and reports automatically for claims. This ensures consistency in the formatting and provides a professional output for administrative and record-keeping purposes.

#### Features of the Report and Invoice Generator:

- **Automated Workflow**: Generates invoices at the click of a button, gathering the information it needs from the database itself.
- **PDF Format**: Produces high-quality, print-ready PDF documents.
- **Aggregate Details**: Reports include statistical summaries such as average, maximum, minimum, and total amounts for claims.
- **Claim Summaries**: Lists details of individual approved claims, including hours worked and final amounts in table format in reports.
- **Consistent Design**: Ensures all invoices and reports maintain a unified and professional layout.

The generated documents are stored securely in the database, accessible only by HR for later retrieval, ensuring easy access and compliance with record-keeping standards.

### Fluent Validation for Data Integrity

The system incorporates FluentValidation to ensure the integrity and validity of data submitted by users. This framework improves the reliability of the application by enforcing robust validation rules before processing user input.

#### Features of FluentValidation Integration:

- **Claim Validation**: Validates claim submissions to ensure all required fields are provided and conform to business rules.
- **Lecturer and Academic Manager Information Validation**: Ensures data integrity when updating user profiles.

#### Benefits of Using FluentValidation:

- **Improved User Experience**: Provides immediate feedback to users on invalid input through descriptive error messages.
- **Centralized Rule Management**: Validation rules are defined in dedicated classes, making them easy to maintain and update.
- **Consistency**: Guarantees uniform validation across the application, reducing the risk of processing invalid or incomplete data.
- **Scalability**: Easily adaptable to future validation requirements as the system grows.
  
This integration ensures the data entering the system adheres to predefined standards, promoting smoother workflows and reducing errors in claim processing and user management.

## Project Structure

### Controllers

- **ClaimsController**: Manages the submission and approval of claims.
  
- **DashboardsController**: Provides dashboards tailored for different user roles.
  
- **StaffController**: Manages staff details and roles.
  
- **RegistrationsController**: Manages the approve an reject of IC lecturers and academic managers to allow only authorised users to access the system.
  
- **ReportController**: Manages the generation, viewing, and saving of claim reports.
  
- **InvoiceController**: Manages the generation, viewing, and saving of claim invoices.

### Services

- **SeedDatabase**: Seeds initial data into the database, if the application is run for the first time and the database is empty lecturer, academic manager and HR users are      created allowing the system to be demonstrated.

- **ReportInformationService**: Retrieves and organizes the necessary data for generating reports. This service aggregates claim information, such as total claims submitted, approved claims, and pending claims, and provides statistical summaries to assist in decision-making.
  
- **ReportGenerationService**: Utilizes the data provided by the ReportInformationService to create detailed, professional reports in PDF format. These reports include summaries, analytics, designed for administrative review and record-keeping.
  
- **InvoiceInformationService**: Gathers and structures data required for generating invoices. It ensures the inclusion of essential details like claim amounts, lecturer information, claim descriptions.
  
- **InvoiceGenerationService**: Leverages QuestPDF to produce high-quality invoices in PDF format. It applies a consistent and professional layout, ensuring compliance with organizational standards. This service is triggered by HR for approved claims, automating the invoice generation process for seamless operations.

### Fluent Validators

- **ClaimValidator**: Ensures that claims submitted by lecturers meet all required criteria.

- **UpdateLecturerDetailsValidator**:Ensures the integrity of data when updating lecturer profiles.
  
- **UpdateAcademicManagerValidator**: Ensures the integrity of data when updating academic manager profiles.

### Database Models

- **ApplicationUser**: Extends the IdentityUser class to include additional user information.
  
- **Claim**: Represents a claim submitted by a lecturer.
  
- **Invoice**: Represents an invoice that is generated by the system.
  
- **Report**: Represents a report that is generated by the system.

## Database Initialization

The database is configured to create itself when the application runs for the first time, using Entity Framework Core's `EnsureCreatedAsync` method. It is automatically populated with the necessary roles and users.

### Seeding Roles and Users

The `SeedDatabase` service seeds the following roles:

- **HR**
- **Lecturer**
- **Academic Manager**

And the following users:

- **HR User**
- **Lecturer Users**
- **Academic Manager Users**

### Login and Account Information for Seeded Users

The seeded users have the following login and account credentials:

- **HR User**
  - **Email**: `hr@mail.com`
  - **Password**: `Hr@123`

- **Lecturer Users**
  - **Email**: `lecturer@mail.com` (User 1) [Approved]
  - **Email**: `lecturer2@mail.com` (User 2)
  - **Email**: `lecturer3@mail.com` (User 3)

  - **Password for all users**: `Lecturer@123`

- **Academic Manager Users**
  - **Email**: `academicmanager@mail.com` (User 1) [Approved]
  - **Email**: `academicmanager3@mail.com` (User 1)
  - **Email**: `academicmanager2@mail.com` (User 1)
  
  - **Password for all users**: `AcademicM@123`

## Claim Submission Process

Lecturers can submit claims at any time via a user-friendly interface. The claim submission form includes:

- Claim Name
- Claim Date
- Claim Description
- Hours worked
- Hourly rate (provided upon account registration)
- Total amount (calculated automatically)
- An 'Upload' button for attaching supporting documents (restricted to .pdf, .docx, and .xlsx formats)

## Claims Approval Process

Programme Coordinators and Academic Managers have access to a separate view to verify and approve claims, with clear options to approve or reject each claim.

They can also view the details of each claim, including the lecturer's name, claim date, hours worked, hourly rate, and total amount.

They can also download any supporting documents attached to the claim.

## Claim Tracking

Each claim's status is tracked transparently, updating in real-time as it moves through the approval process. Statuses may include:

- Pending
- Approved
- Rejected

User intuitive progress bars are also provided to indicate the status of each claim, making it easy to identify claims that require attention.

The progress bars are colour-coded to provide visual cues for the claim status:

- **Green**: Approved
- **Red**: Rejected
- **Yellow**: Pending

## Error Handling

Robust error handling mechanisms are implemented to provide meaningful feedback to users in case of errors or exceptions. Error messages are displayed in a user-friendly format, guiding users on how to resolve the issue.

Error handling is implemented at various levels, including:

- **Global Error Handling**: Catches unhandled exceptions and logs the error details.
- **Controller-Level Error Handling**: Provides specific error messages for different controller actions.
- **Model Validation**: Ensures that data submitted by users is valid and meets the required criteria.
- **User-Friendly Error Messages**: Displays error messages in a clear and concise format to guide users on how to proceed.
- **Logging**: Logs error details to help diagnose and troubleshoot issues.


## Technologies Used

- **ASP.NET Core 8**: The web application framework used to build the CMCS.
- **Entity Framework Core**: The object-relational mapping (ORM) framework used to interact with the database.
- **Bootstrap**: The front-end framework used to design the user interface.
- **Fluent Validation**: A library used to implement complex validation logic in a clean and maintainable way.
- **QuestPDF**: A powerful library for generating high-quality PDF documents, used to create professional reports and invoices for claims automatically.
- **Identity Framework**: The authentication and authorization framework used to manage user roles and permissions.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.

## Student Information

- **Name**: Sashveer Lakhan Ramjathan
- **Student Number**: ST10361554
- **Group**: 2
- **Module**: Programming 2B
- **Assessment**: POE Part 3
