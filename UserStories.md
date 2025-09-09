1. Authentication & Authorization
1.1 User Registration(Done)
Story:
As a patient, I want to sign up using my email and password, so that I can access the clinic system.

Acceptance Criteria:

Must validate email format and password strength.

On success, store user with role = Patient.

Show confirmation that registration succeeded.

1.2 Login(Done)
Story:
As any user, I want to log in with my credentials, so that I can securely access my account.

Acceptance Criteria:

On success, issue JWT or set secure cookie.

Failed login attempts return clear error.

Enforce lockout after X failed attempts.

1.3 Role-based Access(Till Api Done we will write permissions)
Story:
As an admin, I want to restrict actions based on role, so that users can only perform allowed operations.

Acceptance Criteria:

Patients can only manage their own Sessions.

Doctors can only access their own schedules.

Admins can perform all actions.

2. Session Booking(done)
2.1 Book Session
Story:
As a sercretary, I want to select a doctor, date, and time from their available schedule, so that I can book a Session without calling the doctor.

Acceptance Criteria:

Only show available time slots (no double bookings).

Validate that slot is within doctor's working hours.

After it , a session is made for a doctor , patient,room

Prevent booking in the past or too close to now (min lead time).

On success, Session status = Pending.

2.2 Approve/Reject Session
Story:
As a doctor, I want to approve or reject pending Sessions, so that I can control my schedule.

Acceptance Criteria:

Changing status to Approved/Rejected sends notification to patient.

Status changes are logged with timestamp.

Reject requires a reason.

2.3 Admin Override
Story:
As an admin, I want to modify, delete, or approve/reject any Session, so that I can resolve conflicts and enforce clinic policies.

Acceptance Criteria:

Admin actions are always logged.

Overrides bypass normal availability checks only when explicitly confirmed.

3. Schedule Management
3.1 Define Availability
Story:
As a doctor, I want to define my working hours, breaks, and holidays, so that patients can only book valid slots.

Acceptance Criteria:

Supports recurring weekly schedules.

Allows marking exceptions (holidays, sick days).

Past dates cannot be edited.

3.2 Prevent Double Booking
Story:
As the system, I want to ensure no overlapping bookings exist, so that doctors aren’t double booked.

Acceptance Criteria:

Uses atomic transaction to lock slot during booking.

Concurrent booking attempts fail gracefully.

4. Notifications
4.1 Session Confirmation
Story:
As a patient, I want to receive a confirmation when my Session is approved, so that I can plan accordingly.

Acceptance Criteria:

Email/SMS is sent on approval.

Includes date, time, doctor’s name, and location/telehealth link.

4.2 Status Updates
Story:
As a patient, I want to be notified of any changes to my Session, so that I am always informed.

Acceptance Criteria:

Notifications for reschedule, cancellation, or rejection.

Sent immediately after status change.

5. Admin Dashboard
5.1 Manage Users
Story:
As an admin, I want to view and manage all registered users, so that I can maintain the system.

Acceptance Criteria:

Search/filter by name, email, role.

Enable/disable user accounts.

Log all changes.

5.2 View All Sessions
Story:
As an admin, I want to view all Sessions across all doctors, so that I have full visibility.

Acceptance Criteria:

Filter by status, date range, doctor, or patient.

Export to CSV for reporting.

6. Compliance & Audit
6.1 Audit Logging
Story:
As the system, I want to log every critical change with who, when, and what was changed, so that we can meet compliance requirements.

Acceptance Criteria:

Logs stored securely and tamper-proof.

Include Session changes, user role changes, and schedule changes.

6.2 Privacy Enforcement
Story:
As a patient, I want to be sure my personal data is only accessible to authorized roles, so that my privacy is protected.

Acceptance Criteria:

Row-level security prevents cross-user data leakage.

Attempting unauthorized access returns a 403.
