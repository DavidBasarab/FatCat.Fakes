# Task: generate UTC `DateTime` values

**Status:** to do (owner). It does not block anything in Fog. The Fog follow-up is `C:\Code\Fog\tasks\one_off\fatcat-fakes-utc-dates.md`.

## Problem
`Faker.Create<DateTime>()`, `Faker.Create<DateTime?>()` and `Faker.RandomDateTime()` return `DateTimeKind.Local` values, because `Generators/DateTimeGenerator.cs` starts from `DateTime.Now`.

On a machine in a time zone with daylight saving time, some of those values fall inside the hour that does not exist at spring-forward. In US Eastern that is 2:00–2:59 AM on the second Sunday in March. A value like that serializes with the standard-time offset and reads back from JSON one hour later. Any consumer test that round-trips a large fake object graph through JSON and compares it with the original fails at random.

In Fog this is about 0.8% per `LokrServiceModel` round-trip, so most full test runs have one or two unexplained reds (`LokrApiSpecs.*ReturnResult`, `GetAllLokrsByStatus`, `SearchDashboardLokrsTests` and others). Fog's production dates are all UTC, so only tests are affected.

## Change
1. `DateTimeGenerator.Generate` builds its value from `DateTime.UtcNow`, so the result has `Kind == DateTimeKind.Utc`. UTC has no missing hours, so the round-trip is always exact.
   - `FakeFactory` registers the same generator for `DateTime` and `DateTime?` (`FakeFactory.cs:24-25`), so one change covers both.
   - Inside the generator, don't call `Faker.RandomDateTime()` or `Faker.Create<DateTime>()`. They resolve back to this generator and recurse.
2. Optional, your call: `Generate` adds `randomDays` twice (once on `now`, again on the return). That doubles the range to roughly -3000 to +15000 days (about 2018–2067 from today). Keep it as is unless you want to tighten it, because consumers may rely on dates usually being in the future.

## Tests (FatCat.Fakes.Tests, test first)
- `Create<DateTime>()` returns `Kind == Utc`.
- `Create<DateTime?>()` returns a non-null value with `Kind == Utc`.
- A faked object with `DateTime` and `DateTime?` properties gets UTC values for both.
- `RandomDateTime()` returns `Kind == Utc`.
- Round-trip: a large batch (e.g. 10,000) of fake dates serialized with `System.Text.Json` and read back come back equal. Before the fix, this test fails on a DST-zone machine.

## Release
- Bump `VersionPrefix` in `FatCat.Fakes/FatCat.Fakes.csproj` from `1.0.31` to `1.0.32`, then build and publish as usual.
- Behaviour change: consumers that relied on `Kind == Local` (for example, formatting fake dates as local time and comparing against a hard-coded expectation) may see different output. List this in the release notes.

## Done when
- [ ] Generator returns UTC, and the tests above are green.
- [ ] 1.0.32 is published.
- [ ] Fog follow-up started (`C:\Code\Fog\tasks\one_off\fatcat-fakes-utc-dates.md`).
