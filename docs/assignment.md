# ITPE3200 Web Applications 2026H — Exam Group Project Assignment

**Our chosen task: Task 1 — Study Room Booking Tool.** (Tasks 2 and 3 are omitted below.)
Group size: 3–5 students. Grading: Subapp 1 ≈ 40 %, Subapp 2 ≈ 30 %, Documentation ≈ 30 %.
Documentation template: [ITPE3200 Project Documentation Template.docx](./ITPE3200%20Project%20Documentation%20Template.docx)

## Deliverables
The project is two sub-applications of the same task (they must work independently) plus documentation. Both should be MVPs: proof-of-concept solutions that show the functionality is technically possible.

- **Subapp 1 (MVC):** front-end and back-end both use ASP.NET 10.0 Model-View-Controller.
- **Subapp 2 (API + SPA):** back-end is an ASP.NET 10.0 Web API; front-end is a Single Page Application using a framework (e.g. React).
- **Documentation:** 2000–5000 words (English recommended, Norwegian allowed), following the given template.

The two sub-apps can have similar or different UX and features. Both are graded on the criteria below.

## Task 1: Study Room Booking Tool
Helps students book study rooms and lets other students join freely. The goal is to simplify self-organised learning and prevent room booking conflicts on campus. Inspirations (not requirements):
- **Booking mode:** browse available university rooms; book study rooms with study sessions, including subject, topic and meeting time; search and filter open study groups by course or subject.
- **Participation mode:** join booked study rooms, leave messages, etc.

## Basic requirements (needed to pass)
- [ ] The applications must not crash.
- [ ] A front-end UI with interactive forms, as a web page.
- [ ] CRUD for at least 1 entity.
- [ ] Documentation that explains the project.

## Performance requirements (graded; document in sections 4, 5 and 6)
### User experience (Subapp 1 and 2)
- Basic design, not plain text and bare buttons.
- Dynamic and meaningful UX.
- Friendly navigation.
- A confusing UX, or one that doesn't match normal expectations for the task, lowers the grade.

### Coding style (Subapp 1 and 2)
- Neat, structured, modularised code.
- Understandable code: self-explanatory, or commented in important places (English recommended).
- Bugs or build warnings lower the grade. Verify with `dotnet build` and `npm run build`.

### Functionality (both subapps unless noted)
- [ ] **At least 3 domain entities with CRUD.** Identity/auth entities don't count.
- [ ] **At least one many-to-many (N:M) relationship** between domain entities.
- [ ] **At least one complex server-side domain rule.**
- [ ] **At least 2 distinct types of conditional rendering** (if, ternary `? :`, logical `&&`).
- [ ] **Multi-parameter search and content filtering.**
- [ ] **Repository pattern and Data Access Layer (DAL).**
- [ ] **Async/await database access.**
- [ ] **Client-side API service layer**, keeping HTTP handling separate from client business logic. *(Subapp 2 only)*
- [ ] **ViewModels for passing data to views** *(Subapp 1 only)* / **DTOs** *(Subapp 2 only)*.
- [ ] **Forms with client- and server-side validation, error handling, and server-side logging (ILogger).**
- [ ] **Authentication and authorisation, server- and client-side.** Roles are a bonus; you may treat all users as admins.
- [ ] **At least 12 backend unit tests: 6 positive, 6 negative.** *(Subapp 1 only)*
- Requirements done at low quality or only partly (e.g. error handling and logging covering only some of the code) lower the grade.

**Bonus:** beautiful design, extra-rich UX, extra functionality (be creative).

## Documentation template sections
1. Title & administrative metadata
2. Requirements analysis & traceability matrix
3. System architecture & engineering decision records
4. Database structure and access
5. Feature breakdown & traceability
6. Security, resilience & testing
7. AI usage and verification
8. User experience & usability
9. Conclusion & self-reflection

## Examiner access (important)
- Must use ASP.NET 10 MVC for Subapp 1 and API + SPA for Subapp 2.
- The documentation must explain how to run the app on a different machine.
- Either allow user registration, or put login details in the documentation.
- If the app is hosted in the cloud, put the URL in the documentation.
- If an API key is used, include it with the submission or explain how to obtain one.
- Grading follows OsloMet grading guidelines.
