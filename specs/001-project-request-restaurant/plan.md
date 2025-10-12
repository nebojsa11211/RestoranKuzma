
# Implementation Plan: Restaurant Management & Ordering Suite

**Branch**: `001-project-request-restaurant` | **Date**: 2025-10-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/001-project-request-restaurant/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code, or `AGENTS.md` for all other agents).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary
This feature implements a comprehensive cross-role restaurant management and ordering system supporting Admin, Waiter, Chef, and Guest roles. The system provides real-time order processing, menu management, inventory tracking, reservations, payments, and loyalty programs. It is designed as a single-restaurant solution with real-time SignalR notifications, offline PWA capabilities, and integration with external payment providers and printers. The architecture follows Clean Architecture principles with separate Domain, Application, and Infrastructure layers.

## Technical Context
**Language/Version**: C# 12 / .NET 9 (or .NET 8 LTS)
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, SignalR, Blazor (Server & WebAssembly), ASP.NET Core Identity, Serilog, FluentValidation, MediatR, xUnit
**Storage**: PostgreSQL (primary/cloud via Supabase), SQL Server (local/on-premise), Redis (caching & SignalR backplane)
**Testing**: xUnit (unit & integration), Testcontainers (integration), Playwright (E2E)
**Target Platform**: Linux/Windows Server (API), Web Browsers (Blazor WASM/Server), Docker containers
**Project Type**: Web (multi-frontend + backend API)
**Performance Goals**: <500ms p95 for critical operations, 500+ concurrent users, <2s real-time notification delivery
**Constraints**: Offline PWA capabilities, PCI DSS SAQ A compliance, GDPR support
**Scale/Scope**: Single restaurant instance, 14+ core entities, 127 functional requirements, 4 role-based frontends

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: PASS (with justifications)

The constitution template is placeholder-based and not yet ratified for this project. However, applying general software engineering principles:

**Clean Architecture Compliance**:
- ✅ Domain layer contains only entities, enums, value objects (no dependencies)
- ✅ Application layer has DTOs, services, validation, MediatR handlers
- ✅ Infrastructure separated (EF, Identity, Notifications, Integrations)
- ✅ Clear dependency direction: UI → Application → Domain

**Testing Requirements**:
- ✅ TDD approach: Contract tests written before implementation
- ✅ Unit tests for domain logic and application services
- ✅ Integration tests for controllers and database operations
- ✅ E2E tests for critical user flows

**Complexity Justification**:
- Multiple projects required for separation of concerns (Domain, Application, Infrastructure layers, 4 frontends, API)
- Justified by: Clean Architecture mandate, independent deployment of frontends, role-based access isolation
- Each project has clear, single responsibility

**Observability**:
- ✅ Structured logging via Serilog
- ✅ Health check endpoints
- ✅ Metrics via OpenTelemetry/Prometheus
- ✅ Distributed tracing support

## Project Structure

### Documentation (this feature)
```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
RestaurantSuite.sln

src/
├── RestaurantSuite.Domain/              # Core entities, enums, value objects
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Staff.cs
│   │   ├── MenuCategory.cs
│   │   ├── MenuItem.cs
│   │   ├── MenuItemOption.cs
│   │   ├── MenuItemModifier.cs
│   │   ├── InventoryItem.cs
│   │   ├── Order.cs
│   │   ├── OrderLine.cs
│   │   ├── Table.cs
│   │   ├── Reservation.cs
│   │   ├── Payment.cs
│   │   ├── Printer.cs
│   │   ├── KitchenStation.cs
│   │   ├── LoyaltyAccount.cs
│   │   └── AuditLog.cs
│   ├── Enums/
│   └── ValueObjects/
│
├── RestaurantSuite.Application/         # DTOs, services, commands/queries
│   ├── DTOs/
│   ├── Commands/                        # MediatR command handlers
│   ├── Queries/                         # MediatR query handlers
│   ├── Services/
│   ├── Validators/                      # FluentValidation
│   └── Interfaces/
│
├── RestaurantSuite.Infrastructure.EF/   # EF Core, migrations, repositories
│   ├── DbContext/
│   ├── Configurations/                  # EF entity configurations
│   ├── Migrations/
│   └── Repositories/
│
├── RestaurantSuite.Infrastructure.Identity/  # Auth, JWT, roles
│   ├── Models/
│   ├── Services/
│   └── Configuration/
│
├── RestaurantSuite.Notifications/       # SignalR hubs & workers
│   ├── Hubs/
│   │   ├── OrdersHub.cs
│   │   ├── KitchenHub.cs
│   │   └── NotificationsHub.cs
│   └── Workers/
│
├── RestaurantSuite.Integrations/        # Payment, printer, SMS/email adapters
│   ├── Payments/
│   │   ├── IPaymentAdapter.cs
│   │   ├── StripeAdapter.cs
│   │   └── SquareAdapter.cs
│   ├── Printers/
│   └── Communications/
│
├── RestaurantSuite.Common/              # Shared utilities, constants
│   ├── DTOs/
│   ├── Constants/
│   └── Utilities/
│
├── RestaurantSuite.Api/                 # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── MenuController.cs
│   │   ├── OrdersController.cs
│   │   ├── ReservationsController.cs
│   │   ├── TablesController.cs
│   │   ├── InventoryController.cs
│   │   └── ReportsController.cs
│   ├── Middleware/
│   ├── Filters/
│   └── Program.cs
│
├── RestaurantSuite.Admin/               # Blazor Server (admin dashboard)
│   ├── Pages/
│   │   ├── Dashboard.razor
│   │   ├── MenuEditor.razor
│   │   ├── Inventory.razor
│   │   ├── Staff.razor
│   │   └── Reports.razor
│   ├── Components/
│   └── Services/
│
├── RestaurantSuite.Waiter/              # Blazor WASM PWA
│   ├── wwwroot/
│   │   ├── manifest.json
│   │   └── service-worker.js
│   ├── Pages/
│   │   ├── Tables.razor
│   │   ├── OrderEntry.razor
│   │   └── Payment.razor
│   └── Services/
│
├── RestaurantSuite.Chef/                # Blazor WASM PWA
│   ├── wwwroot/
│   │   ├── manifest.json
│   │   └── service-worker.js
│   ├── Pages/
│   │   └── KitchenDisplay.razor
│   └── Services/
│
└── RestaurantSuite.Guest/               # Blazor WASM PWA
    ├── wwwroot/
    │   ├── manifest.json
    │   └── service-worker.js
    ├── Pages/
    │   ├── Menu.razor
    │   ├── Order.razor
    │   ├── Reservation.razor
    │   └── Loyalty.razor
    └── Services/

tests/
├── RestaurantSuite.Tests.Unit/
│   ├── Domain/
│   ├── Application/
│   └── Services/
│
└── RestaurantSuite.Tests.Integration/
    ├── Controllers/
    ├── Database/
    └── Workflows/

docker/
├── Dockerfile.Api
├── Dockerfile.Admin
├── Dockerfile.Waiter
├── Dockerfile.Chef
├── Dockerfile.Guest
└── docker-compose.yml

.github/
└── workflows/
    └── ci.yml
```

**Structure Decision**: Web application with multiple Blazor frontends and a central API backend. This structure follows Clean Architecture with clear separation between Domain (business logic), Application (use cases), Infrastructure (external concerns), and Presentation (API + 4 Blazor apps). The solution uses a multi-project approach to enable independent scaling, deployment, and development of each role-based frontend while sharing common business logic through the API.

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType claude`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
1. Load `.specify/templates/tasks-template.md` as base
2. Generate tasks from Phase 1 design artifacts:
   - **data-model.md**: Entity creation tasks for 14 core entities + 2 supporting entities
   - **api-contracts.yaml**: Controller implementation tasks for 7 main controllers
   - **signalr-contracts.md**: Hub implementation tasks for 3 hubs
   - **quickstart.md**: Integration test tasks derived from 10 validation steps

3. **Task Categories**:
   - **Foundation** (must run first):
     - Create solution and project structure (14 projects)
     - Configure EF Core DbContext
     - Implement ASP.NET Core Identity + JWT
     - Configure Serilog + OpenTelemetry
     - Set up Redis caching & SignalR backplane

   - **Domain Layer** (can run in parallel after foundation):
     - Create 16 entity classes (User, Staff, Menu*, Inventory*, Order*, Table, Reservation, Payment, Printer, KitchenStation, LoyaltyAccount, AuditLog, InventoryAdjustment, MenuItemTranslation)
     - Create enums (OrderStatus, PaymentStatus, ReservationStatus, etc.)
     - Add validation attributes

   - **Infrastructure Layer**:
     - EF Core entity configurations (fluent API for all entities)
     - Create initial migration
     - Implement repositories (if using repository pattern)
     - Configure database providers (PostgreSQL + SQL Server)
     - Implement JWT token service
     - Configure role-based authorization policies

   - **Application Layer**:
     - Create DTOs for all entities
     - Implement MediatR command handlers (Create, Update, Delete operations)
     - Implement MediatR query handlers (List, GetById operations)
     - Add FluentValidation validators
     - Implement business logic services

   - **SignalR Hubs** (after Application layer):
     - Implement OrdersHub with IOrderClient interface
     - Implement KitchenHub with IKitchenClient interface
     - Implement NotificationsHub with INotificationClient interface
     - Configure Redis backplane

   - **API Controllers** (after Application + Hubs):
     - AuthController (register, login, refresh)
     - MenuController (GET menu, admin CRUD)
     - OrdersController (CRUD, status updates, payment)
     - TablesController (list, status management)
     - ReservationsController (CRUD, filtering)
     - InventoryController (list, adjust stock)
     - ReportsController (sales, stock reports)

   - **Integration Adapters**:
     - IPaymentAdapter interface
     - StripeAdapter implementation
     - SquareAdapter implementation
     - IPrinterAdapter interface
     - NetworkPrinterAdapter (ESC/POS)

   - **Blazor Frontends** (can run in parallel after API):
     - **Admin (Blazor Server)**:
       - Dashboard page with KPIs
       - MenuEditor page (CRUD)
       - Inventory page with alerts
       - Staff management page
       - Reports page (sales, stock)
       - Settings page

     - **Waiter (Blazor WASM PWA)**:
       - Tables page with floor plan
       - OrderEntry page
       - Payment page
       - PWA manifest + service worker

     - **Chef (Blazor WASM PWA)**:
       - KitchenDisplay page
       - SignalR client integration
       - PWA manifest + service worker

     - **Guest (Blazor WASM PWA)**:
       - Menu browsing page
       - Order page
       - Reservation page
       - Loyalty page
       - PWA manifest + service worker

   - **Docker & CI/CD**:
     - Create Dockerfile for API
     - Create Dockerfiles for each Blazor app
     - docker-compose.yml for local dev
     - GitHub Actions CI workflow
     - Database migration bundle

   - **Testing** (TDD - write before implementation):
     - **Contract Tests**: Validate OpenAPI schema for all endpoints
     - **Unit Tests**: Domain entities, validators, services
     - **Integration Tests**:
       - Order placement flow (from quickstart step 3)
       - Order status lifecycle (from quickstart step 4-7)
       - Payment processing (from quickstart step 8)
       - Inventory deduction (from quickstart step 9)
       - Real-time notifications (from quickstart step 4)
     - **E2E Tests** (Playwright):
       - Guest order flow
       - Kitchen processing flow
       - Waiter payment flow

   - **Seed Data**:
     - 4 test users (Admin, Waiter, Chef, Guest)
     - Sample menu (5 items)
     - Sample tables (3 tables)
     - Sample inventory items

**Ordering Strategy**:
- **Sequential Dependencies**:
  1. Foundation → Domain → Infrastructure → Application → API/Hubs → Frontends
  2. Tests written BEFORE implementation (TDD)
  3. Contract tests → Unit tests → Integration tests → E2E tests

- **Parallel Execution Markers** [P]:
  - All entity creation tasks (independent files)
  - All DTO creation tasks
  - All controller implementation (after application layer)
  - All Blazor pages within same frontend
  - All adapter implementations

- **Critical Path**:
  1. Solution setup
  2. Domain entities
  3. EF Core configuration + migration
  4. Identity + JWT
  5. Application services
  6. API controllers
  7. SignalR hubs
  8. Frontend integration
  9. Testing
  10. Docker deployment

**Estimated Task Breakdown**:
- Foundation: 8 tasks
- Domain: 19 tasks (16 entities + 3 enum/validation)
- Infrastructure: 15 tasks
- Application: 24 tasks
- SignalR: 5 tasks
- API: 11 tasks (7 controllers + middleware + swagger)
- Integrations: 6 tasks
- Blazor Admin: 7 tasks
- Blazor Waiter: 5 tasks
- Blazor Chef: 3 tasks
- Blazor Guest: 6 tasks
- Docker/CI: 6 tasks
- Testing: 20 tasks (contract + unit + integration + E2E)
- Seed: 2 tasks

**Total Estimated Tasks**: ~137 tasks

**Estimated Output**: 137+ numbered, dependency-ordered tasks in tasks.md with [P] markers for parallel execution

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning approach described (/plan command)
- [ ] Phase 3: Tasks generated (/tasks command - NOT executed by /plan)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved (via spec clarifications)
- [x] Complexity deviations documented (Clean Architecture justification)

---
*Based on Constitution v2.1.1 - See `/memory/constitution.md`*
