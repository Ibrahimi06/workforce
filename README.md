# WorkForce KS

![Node.js](https://img.shields.io/badge/backend-Node.js-green)
![Express](https://img.shields.io/badge/framework-Express-black)
![React](https://img.shields.io/badge/frontend-React-blue)
![PostgreSQL](https://img.shields.io/badge/database-PostgreSQL-blue)
![JWT](https://img.shields.io/badge/authentication-JWT-orange)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

## Workforce & Operations Management Platform

**WorkForce KS** is a full-stack workforce management platform designed to help companies manage employees, projects, attendance, payroll, and daily operations from a centralized system.

The platform supports **two user roles: Manager and Worker**, enabling efficient workforce coordination, task assignment, attendance monitoring, and payroll management.

This project was developed as a **university portfolio project** to demonstrate modern **full-stack development practices** using **React, Node.js, Express, and PostgreSQL**.

---

# Core Features

## Authentication & Security

* Secure **JWT authentication**
* User **signup and login**
* **Role-based access control** (Manager / Worker)
* Protected API routes
* Middleware authorization

---

## Employee Management

Managers can:

* Add new employees
* View employee records
* Remove employees
* Manage workforce structure

---

## GPS Attendance System

Workers can:

* Check-in using **GPS location**
* Check-out when work ends
* Track their attendance

Managers can:

* Monitor **live employee locations**
* Review attendance logs
* Track workforce activity

---

## Project & Task Management

Managers can:

* Create projects
* Assign tasks
* Track project progress

Workers can:

* View assigned tasks
* Update task status
* Monitor work responsibilities

---

## Payroll System

The system supports payroll operations including:

* Salary calculation
* Attendance-based payroll data
* Payroll history
* Payroll report generation

---

## Leave Management

Employees can:

* Submit leave requests
* Track request status

Managers can:

* Approve or reject leave requests
* Review leave history

---

## Notifications & Announcements

Internal communication features include:

* Company announcements
* Employee notifications
* Notification read status tracking

---

# System Architecture

```
Frontend (React + Vite)
        │
        │ REST API
        ▼
Backend (Node.js + Express)
        │
        │ SQL Queries
        ▼
Database (PostgreSQL)
```

### Architecture Layers

**Frontend Layer**

* React user interface
* Dashboard views
* Data tables and forms
* API communication

**Backend Layer**

* Express REST API
* Business logic
* Authentication middleware
* Route controllers

**Data Layer**

* PostgreSQL relational database
* Structured workforce data
* Attendance and payroll records

---

# Project Folder Structure

```
workforce/
│
├── backend/
│   ├── .env.example
│   ├── package.json
│   ├── server.js
│
│   ├── db/
│   │   ├── pool.js
│   │   └── schema.sql
│
│   ├── middleware/
│   │   └── auth.js
│
│   └── routes/
│       ├── auth.js
│       ├── attendance.js
│       ├── employees.js
│       ├── projects.js
│       ├── tasks.js
│       ├── payroll.js
│       ├── leaves.js
│       └── notifications.js
│
└── frontend/
    ├── index.html
    ├── vite.config.js
    ├── package.json
    ├── .env
│
    └── src/
        ├── main.jsx
        ├── App.jsx
│
        ├── context/
        │   └── AuthContext.jsx
│
        ├── data/
        │   ├── api.js
        │   └── constants.js
│
        ├── styles/
        │   └── globals.js
│
        ├── components/
        │   └── ui/
        │       └── index.jsx
│
        └── pages/
            ├── AuthPage.jsx
            ├── CheckInOut.jsx
            ├── WorkerDashboard.jsx
            ├── ManagerDashboard.jsx
            ├── Employees.jsx
            ├── Attendance.jsx
            ├── Projects.jsx
            ├── Tasks.jsx
            ├── Payroll.jsx
            ├── Leaves.jsx
            ├── Reports.jsx
            ├── Announcements.jsx
            └── Settings.jsx
```

---

# Installation Guide

## 1 Clone Repository

```
git clone https://github.com/yourusername/workforce-ks.git
cd workforce
```

---

# Backend Setup

Navigate to backend:

```
cd backend
```

Install dependencies:

```
npm install
```

Create environment file:

```
cp .env.example .env
```

Update your PostgreSQL credentials inside `.env`.

---

# Database Setup

Create a PostgreSQL database and run the schema file:

```
schema.sql
```

This script will create all required tables including:

* users
* employees
* attendance
* projects
* tasks
* payroll
* leaves
* notifications

---

Run the backend server:

```
node server.js
```

Backend API will run at:

```
http://localhost:4000/api
```

---

# Frontend Setup

Navigate to frontend:

```
cd frontend
```

Install dependencies:

```
npm install
```

Configure environment variable:

```
VITE_API_URL=http://localhost:4000/api
```

Run development server:

```
npm run dev
```

Frontend will run at:

```
http://localhost:5173
```

---

# Example API Endpoints

### Authentication

| Method | Endpoint     | Description                |
| ------ | ------------ | -------------------------- |
| POST   | /auth/signup | Register new user          |
| POST   | /auth/login  | Login                      |
| GET    | /auth/me     | Current authenticated user |

---

### Employees

| Method | Endpoint       |
| ------ | -------------- |
| GET    | /employees     |
| POST   | /employees     |
| DELETE | /employees/:id |

---

### Attendance

| Method | Endpoint             |
| ------ | -------------------- |
| POST   | /attendance/checkin  |
| POST   | /attendance/checkout |
| GET    | /attendance/live     |

---

### Tasks

| Method | Endpoint          |
| ------ | ----------------- |
| GET    | /tasks            |
| POST   | /tasks            |
| PATCH  | /tasks/:id/status |

---

### Payroll

| Method | Endpoint     |
| ------ | ------------ |
| GET    | /payroll     |
| POST   | /payroll/run |

---

# Future Improvements

Potential enhancements for future versions:

* Mobile application
* Real-time GPS tracking (WebSockets)
* Multi-company SaaS platform
* Analytics dashboard
* Automated payroll exports
* Third-party integrations

---

# Learning Outcomes

This project demonstrates:

* Full-stack web development
* REST API design
* JWT authentication
* PostgreSQL relational database design
* React state management
* Scalable project architecture

---

# Author

**Xhafer Ibrahimi**

University Project – Full-Stack Web Development

---

# License

MIT License
