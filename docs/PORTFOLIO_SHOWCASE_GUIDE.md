# Portfolio Showcase Guide (TodoAppWasm)

This guide is focused on one objective: make this repository look strong to recruiters while keeping effort realistic.

## 1) What recruiters should immediately understand

When someone opens your repo, they should quickly see:

1. **What it is:** full-stack to-do app (Blazor WASM + ASP.NET Core API).
2. **What you built:** UI, business logic, data access, tests, containerization, CI/CD.
3. **How to verify:** clear local run instructions and a live front-end link.
4. **Engineering maturity:** pipeline, code organization, and known limitations documented.

## 2) Recommended showcase strategy

### Front-end demo (primary)

Deploy the **Blazor WebAssembly app** as a static site on **GitHub Pages** (chosen target).

Why this is best for portfolio:
- Easy for recruiters to open instantly.
- No server warm-up delay.
- No back-end hosting cost required for a first impression.

### Back-end (secondary)

Present back-end in one of two ways:
- **Option A (preferred for now):** keep back-end local/containerized and document API capabilities clearly.
- **Option B (public staging deployment):** deploy API to a public URL on a free/low-cost host (for example Render/Fly/Railway depending current free tier), and wire CORS + HTTPS.

**Clarification:** "public staging deployment" simply means a non-production but publicly reachable API endpoint used for demos/recruiter testing.

If API is not publicly hosted, be explicit in README:
> "Live demo focuses on front-end UX. API is available locally via Docker for technical review."

## 3) CI/CD story you can tell in interviews

Your repo already demonstrates these practices:

- Automated build and tests on push/PR.
- Docker image build and push to GHCR in development workflow.
- Deployment trigger to staging (Render).

In interviews, describe this as:
- "I used GitHub Actions to enforce build/test quality gates."
- "I publish container images to GHCR for reproducible deployment."
- "I separated generic CI and environment-specific development workflow."

## 4) README structure for recruiter impact

Use this order in `README.md`:

1. **Project elevator pitch** (2-3 lines).
2. **Live Demo** (link first, screenshots second).
3. **Architecture diagram or bullet list**.
4. **Tech stack**.
5. **Key features**.
6. **Local setup** (copy/paste commands).
7. **Testing and CI/CD**.
8. **Known limitations + roadmap**.
9. **Author / contact / CV link**.

## 5) Improvements that give best ROI

If you have limited time, prioritize:

1. **Polish UI and UX in Blazor**
   - loading states,
   - empty states,
   - error toasts/messages,
   - confirmation for destructive actions.

2. **Improve reliability signals**
   - add/expand unit tests for app logic,
   - include one integration-style API test if feasible.

3. **Documentation quality**
   - architecture explanation,
   - deployment instructions,
   - "tradeoffs and next steps" section.

4. **Demo quality**
   - one short GIF/video in README,
   - one seeded dataset for predictable demo behavior.

## 6) Proposed task backlog (approve before creating implementation tasks)

The following are suggested tasks. Pick the order before implementation.

### Phase 1 — Must-have (portfolio ready)

- [ ] Add "Live Demo" section + architecture + screenshots to README.
- [ ] Add front-end deployment workflow (GitHub Pages or Azure Static Web Apps).
- [ ] Add app configuration documentation (`ApiBase`, environment behavior).
- [ ] Add "Known Limitations" section with honest back-end hosting note.

### Phase 2 — Quality boost

- [ ] Improve Blazor UX (loading, empty/error states, confirmation modal).
- [ ] Add focused tests around `Application` logic and critical API behavior.
- [ ] Add badges (CI status, .NET version, container image location).

### Phase 3 — Nice-to-have

- [ ] Add architecture diagram image in `/docs/assets`.
- [ ] Add short demo video/GIF.
- [ ] Add a dedicated `CHANGELOG.md` and release tags.

## 7) How to present this on your CV

Use outcome-oriented wording:

- "Built and deployed a full-stack .NET 8 to-do platform using Blazor WebAssembly and ASP.NET Core Web API."
- "Implemented CI/CD with GitHub Actions for automated build, test, Docker image publishing, and staged deployment."
- "Designed layered architecture (UI, HTTP clients, application logic, data access) with reusable DTO/model contracts."

## 8) Suggested immediate decision

Before creating implementation tasks, decide:

1. **Front-end hosting target:** GitHub Pages (selected).
2. **Back-end approach:** local-only documented vs public staging deployment (public URL for demo).
3. **Scope for this final polish cycle:** Phase 1 only, or Phase 1 + selected Phase 2 items.

Once these are chosen, task execution can be done in a clean and recruiter-focused sequence.
