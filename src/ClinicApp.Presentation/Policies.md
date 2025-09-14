## roles
- Patinet
- Doctor
- Secretary
- Admin

## requirement
- Doctor can only view his sessions
- Patinet can only view his sessions
- Secretary and Admins can view all sessions
- Only Admins can view rooms
- a doctor times off can only be viewed by admins,
- secretary just can know that there is a time off
- doctor working time can be viewed only by the doctor himself,admin,secretary
- a patient can veiw all doctors (names,major)
- session details (Description) can only be viewed by the doctor
- SessionStatus(Pending,Set,...) can be viewed by all roles
- SessionStateHistory can be viewed by doctor,admins,secretary

## policies
### Session-related

- CanViewOwnSessions
    If user is a doctor → can view sessions where DoctorId == User.DoctorId.
    If user is a patient → can view sessions where PatientId == User.PatientId.

- CanViewAllSessions
    If user is admin or secretary.

- CanViewSessionStatus
    All roles, but doctor/patient limited to their own sessions.

- CanViewSessionDetails
    Only doctor of the session.

- CanViewSessionStateHistory
    Admins, secretary, and doctor,patient of the session.

- CanEditSession
    Probably only admins, doctors, secretary.

- CanCancelSession
    Doctor (for their own),Secretary.

- CanAddSession
    Secretary,Admin(For all doctors),Doctor for their own doctor,

### Room-related

- CanViewRooms
    Only admins.
- CanManageRooms
    only admins.

### Doctor-related

- CanViewDoctorsInfo
    Everyone (even unauthenticated).

- CanViewDoctorWorkingTime
    Doctor himself, admin, secretary.

- CanViewDoctorTimesOff
    Admin, doctor himself.
    (Optional: secretary can only view “time-off blocked” flag, not details → could be a separate policy like CanViewDoctorAvailability)


### Admin-related

- CanManageUsers
    Admins can create, update, or delete doctors/secretaries/patients.

- CanManageSystemSettings (futuristic)
    For SaaS / multi-tenant: admins might have system-level management rights.