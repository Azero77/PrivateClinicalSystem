# 🏥 Clinic Appointment System

A backend-driven appointment management system for clinics. Built to explore **Domain-Driven Design (DDD)**, **authentication/authorization**, **clean architecture**, and complex backend workflows like scheduling logic and policy-based access control.

---

## ✨ Features (MVP)

- 👤 **User Roles**: Patient, Doctor, Admin
- 🔐 **Authentication**: JWT or cookie-based login/signup
- 🔒 **Authorization**: Role-based + custom policies
- 📅 **Appointment Booking**:
  - Patients book appointments with doctors
  - Doctors approve/reject appointments
  - Admin can override/modify/delete
- ⏱️ **Schedule Management**:
  - Doctors define their availability
  - Prevent double bookings or invalid time slots
- 📊 **Admin Dashboard**: View/manage all users and appointments

---

## 📦 Domain Model Overview

