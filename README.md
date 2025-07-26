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

## Invariants

✅ Clinic Appointment System — Invariants
1. 🔐 Role-Based Access is Enforced
Each user action is bound to their role (Patient, Doctor, Admin).

Patients can only view/book their own appointments.

Doctors can only access their schedule and assigned appointments.

Admins have global visibility and control.

2. 📅 No Double Booking
A doctor or a time slot cannot be booked for more than one appointment at the same time.

Booking engine must atomically lock time slots.

Prevents race conditions in concurrent booking attempts.

3. ⏳ Appointments Can Only Be Scheduled Within Doctor's Available Hours
Bookings must respect predefined working hours, breaks, and holidays.

No override unless done explicitly by an Admin.

Includes location-specific availability if applicable.

4. 🧾 Every Appointment Has a Clear Lifecycle
An appointment must always be in one of the defined states:

Pending → Approved/Rejected/Rescheduled → Completed/Cancelled

State transitions are logged and time-stamped for traceability.

5. 📬 Patients Must Be Informed About Appointment Status
Any change to appointment status must trigger a notification (email/SMS/system).

Ensures transparency and reduces no-shows.

Follows event-driven pattern (e.g., on approval → send confirmation).

6. 📊 Data Privacy and Separation Are Guaranteed
A user can never access or infer data from another role unless explicitly authorized.

Enforced through database-level row filters or query constraints.

All PII is securely stored and access-logged.

7. 📈 All Actions Are Auditable
Any change to core entities (appointments, schedules, users) must be traceable.

Who did what, when, and to which record.

Needed for medical/legal compliance and troubleshooting.

8. 🚫 Booking Logic is Context-Aware
System must enforce rules like:

No booking in the past

Minimum lead time (e.g., can’t book within 1 hour)

Session length must match doctor's allowed durations

9. 👥 Each Doctor Has an Independent Schedule
Doctor calendars are isolated and customizable.

One doctor’s schedule change must not affect others unless it's a clinic-wide policy.

10. 🔄 System Is Resilient to Partial Failures
Critical actions (like booking or status updates) are either:

Fully completed or

Roll back safely (atomic transactions)

