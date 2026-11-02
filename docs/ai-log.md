# AI usage log

Raw material for documentation section 7 (AI usage & verification).
7.1 needs examples per category (idea exploration, understanding code/concepts, UX design, controlled AI coding).
7.2 needs three concrete examples: target task, exact prompt, initial AI output, flaw identified, human fix.

Add an entry each time AI is used. Fill in the "Verification" and "Human fix" parts yourself.

---

## 2026-11-02 · Feature planning for Subapp 1 (MVC)

- **Category (7.1):** Idea exploration
- **Tool:** Claude (claude.ai, Project "Booking service", with read access to the repo)
- **Target task:** Find out which features to add to the MVC app, which is currently bare bones (only Room CRUD + login/register).
- **Exact prompt:**
  > look at my project, I want to polish up the mvc section. Currently it's very bare bones do you have any suggestions for features?
- **Initial AI output (summary):**
  - Compared the code with the assignment's functionality requirements and listed what is missing.
  - Suggested six features mapped to requirements: Booking (study session) CRUD, a Course entity, Room–Facility as the N:M relationship, a no-double-booking domain rule, multi-parameter search, and a "My bookings" page with role-aware UI.
  - Suggested structural work: repository pattern/DAL, ViewModels, an xUnit + NSubstitute test project.
  - Listed six possible issues in the existing code as hints (no code written), e.g. `EnsureCreated()` in the DbContext constructor combined with `Migrate()` in Program.cs, and a double sign-in in `AccountController.Login`.
  - Suggested an order of work, starting with the repository refactor.
  - The plan was saved as a Claude Doc: "MVC Subapp – Feature Plan & Review".
- **Flaw identified:**
  - Open question: the AI suggested Room–Facility as the N:M because Booking ↔ user (joining sessions) involves an Identity entity, which doesn't count. To be decided by the group / checked with the lecturer.
- **Outcome of the open question:** Room–Facility was not used. Room ↔ Equipment became the N:M (join entity `RoomEquipment`), and `Building` was added as the third entity. No Course entity.
- **Verification / human fix (checked against the code 2026-11-03):**
  - Accepted and implemented: repository pattern (`DAL/`, repositories for Room, Building, Equipment, Booking), ViewModels (`BookingFormViewModel`, `RoomFormViewModel`), no-double-booking rule (`BookingService`: `HasOverlap` + `HasUserOverlap`), "My bookings" page (`BookingController.MyBookings`, still uses a hardcoded test user).
  - Changed: Room–Facility → Room–Equipment.
  - Not implemented yet: Course entity, multi-parameter search, unit test project.
  - Code hints: `EnsureCreated()`/`Migrate()` conflict fixed (only migrations now). Double sign-in in `AccountController.Login` still open.
  - _TODO: why the group chose Building/Equipment over Course/Facility._
- **Code generated:** None.

---

## 2026-11-03 · Checking the branch before sharing it with teammates (Claude Code)

- **Category (7.1):** Understanding code/concepts (code review)
- **Tool:** Claude Code (Code section of the Claude desktop app)
- **Target task:** Make sure the branch `testingtestDBSTUFF` works for teammates who may not have run the code before.
- **Exact prompt:**
  > Can you check that my branch works correctly? Look for any errors or faults in the code that might interfere. I want it to work for my teammates that potentially might not have ran any of the code before
- **Output:** Cloned the branch to a temp folder, built and ran it with no database, tested CRUD for all entities, the double-booking checks, business rules and register/login. Reported:
  - Old `Booking.db` crashes startup (`table "Rooms" already exists`): `main` creates the db with `EnsureCreated()` (no migration history), the branch uses migrations. Added a README section on deleting `Booking.db` (commit `c87b45d`).
  - Commit `7ced882` deletes `frontend/`, which also removes it from `main` on merge.
  - Bugs: hardcoded `"seed-user-1"` in `BookingController`, anyone can edit/delete any booking; Building/Equipment have no login requirement; booking without a room gives a generic error; room + equipment creation is not one transaction.
  - Cleanups: Booking→Room relationship configured twice in `BookingDbContext`; `thebooking.csproj` links a non-existent `..\libman.json`; README lacks .NET 10 SDK requirement and uses a machine-specific path.
- **Flaw identified:** Did not report the double sign-in in `AccountController.Login` (login still works, so testing didn't reveal it). Confirmed with the code 2026-11-03.
- **Verification / human fix:** We deleted `Booking.db` and confirmed the app starts and creates a new database without breaking anything. Deleting it can be necessary depending on local setup.
- **Code generated:** None (README text only).
---

## 2026-11-03 · Checking documentation against the code

- **Category (7.1):** Understanding code/concepts
- **Tool:** Claude (claude.ai, Project "Booking service", linked to the repo)
- **Target task:** Find out whether recent changes need documenting, and check the documentation draft against the code.
- **Exact prompts:**
  > I've used the code section of the this desktop app to make some changes. I think it's mostly in the readme but I want to double check that there might be a need to add the changes to the documentation, can you see what was changed in the code session and make a summary [...]

  > Yes please check

  > Can you make the updates to the documentation
- **Output:** Summarised the Code session (README only) via git. Found that the 7.1.1 entry was out of date, and listed gaps that affect later sections: hardcoded user ID in `BookingController.GetCurrentUserId()`, `[Authorize]` only on `RoomController`, no test project, README run path is machine-specific and has no Subapp 2 instructions or test credentials. Updated 7.1.1 in the documentation, this log and the README run path.
- **Verification / human fix:** _TODO: check the updated 7.1.1 text against the code yourselves._
- **Code generated:** None.
