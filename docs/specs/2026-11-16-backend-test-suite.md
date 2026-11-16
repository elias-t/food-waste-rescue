# Backend Test Suite

**Date:** 2026-11-16  
**Branch:** `feat/backend-test-suite`

## Goal

Add an xUnit test suite covering Domain entity behaviour, Application layer validators and handlers, and integration tests for the auth and claim flows. The goal is a green, reliable CI baseline that enforces the core business rules and catches regressions.

**Non-goals:**
- Controller / HTTP-level tests (no `WebApplicationFactory` in this pass)
- Background job tests (Hangfire scheduling, email sending)
- Performance / load tests
- Testing infrastructure concerns already covered by EF Core itself (migrations, schema)

---

## Acceptance criteria

### Domain unit tests
- [ ] `FoodListing.Expire()` sets `Status = Expired` and fires `FoodListingExpiredEvent`
- [ ] `FoodListing.Cancel()` sets `Status = Cancelled`
- [ ] `FoodListing.MarkAsClaimed()` sets `Status = Claimed`
- [ ] `FoodListing` constructor fires `FoodListingCreatedEvent`
- [ ] `Claim.Confirm()` sets `Status = Confirmed` and fires `ClaimConfirmedEvent`
- [ ] `Claim.MarkCollected()` sets `Status = Collected` and sets `CollectedAt`
- [ ] `Claim.Cancel()` sets `Status = Cancelled`
- [ ] `BaseEntity` auto-assigns non-empty `Id` and `CreatedAt` on construction

### Validator unit tests
- [ ] `RegisterUserCommandValidator` rejects empty/invalid email, password < 8 chars, empty display name
- [ ] `LoginUserCommandValidator` rejects empty email or password
- [ ] `CreateFoodListingCommandValidator` rejects empty title, past `ExpiresAt`, out-of-range lat/lon
- [ ] `ClaimFoodListingCommandValidator` rejects empty `ListingId`, notes > 500 chars

### Handler integration tests (SQLite in-memory)
- [ ] `RegisterUserCommandHandler` creates a user and returns a JWT; duplicate email returns `Result.Failure`
- [ ] `LoginUserCommandHandler` returns a JWT on valid credentials; wrong password returns `Result.Failure`
- [ ] `CreateFoodListingCommandHandler` persists a listing and returns its `Guid`
- [ ] `ClaimFoodListingCommandHandler` creates a `Claim`, marks listing `Claimed`, enqueues jobs
- [ ] `ClaimFoodListingCommandHandler` returns failure when listing is not `Active`
- [ ] `CancelListingCommandHandler` cancels the listing when caller is the donor
- [ ] `CancelListingCommandHandler` returns failure when caller is not the donor
- [ ] `GetNearbyListingsQueryHandler` returns listings within radius and excludes those outside
- [ ] `GetImpactReportQueryHandler` returns correct aggregation counts
- [ ] `GetUnclaimedExpiringSoonQueryHandler` returns only active, claim-free, expiring-within-3h listings

---

## Design

### New project: `FoodWasteRescue.Tests`

```
backend/tests/FoodWasteRescue.Tests/
  FoodWasteRescue.Tests.csproj
  Domain/
    FoodListingTests.cs
    ClaimTests.cs
    BaseEntityTests.cs
  Application/
    Validators/
      RegisterUserCommandValidatorTests.cs
      LoginUserCommandValidatorTests.cs
      CreateFoodListingCommandValidatorTests.cs
      ClaimFoodListingCommandValidatorTests.cs
    Handlers/
      RegisterUserCommandHandlerTests.cs
      LoginUserCommandHandlerTests.cs
      CreateFoodListingCommandHandlerTests.cs
      ClaimFoodListingCommandHandlerTests.cs
      CancelListingCommandHandlerTests.cs
      GetNearbyListingsQueryHandlerTests.cs
      GetImpactReportQueryHandlerTests.cs
      GetUnclaimedExpiringSoonQueryHandlerTests.cs
  Helpers/
    DbContextFactory.cs      ← SQLite in-memory AppDbContext + Identity setup
    FakeCurrentUser.cs       ← ICurrentUserService stub
    FakeJobClient.cs         ← IBackgroundJobClient stub
```

### Packages (`.Tests` project only)
| Package | Purpose |
|---|---|
| `xunit` | Test framework |
| `xunit.runner.visualstudio` | VS/CI runner |
| `Microsoft.NET.Test.Sdk` | Required runner host |
| `FluentAssertions` | Readable assertion DSL |
| `NSubstitute` | Mocking (IEmailService, IBackgroundJobClient) |
| `Microsoft.EntityFrameworkCore.Sqlite` | In-memory SQLite for handler tests |

### Key design decisions
- **Handler tests use SQLite in-memory** (not EF InMemory provider) so FK constraints, Identity password hashing, and query translations work correctly.
- **Identity services** (`UserManager`, `SignInManager`) are instantiated against the SQLite context — real password hashing, no mocks.
- **IBackgroundJobClient** is stubbed via NSubstitute so `ClaimFoodListingCommandHandler` tests don't need Hangfire SQL.
- **ICurrentUserService** stub (`FakeCurrentUser`) lets tests set an arbitrary user ID and authenticated state.
- **JWT generation** in auth handler tests uses a minimal in-test `JwtSettings` config; output verified as a non-empty string.

### No new migrations, no DB schema changes.

---

## Risks / open questions
- `IdentityService` calls `UserManager`/`SignInManager` directly — these are wired to the SQLite context. If brittle, auth handler tests can mock `IIdentityService` instead.
- `IBackgroundJobClient` is a Hangfire interface; NSubstitute can stub it since it's an interface.
