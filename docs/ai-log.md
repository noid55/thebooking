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
- **Verification / human fix:** _TODO: which suggestions did the group check against the code and assignment, and what did you accept, change or reject?_
- **Code generated:** None.
