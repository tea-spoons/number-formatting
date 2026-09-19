# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.number-formatting` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- Declared the missing dependency on `large-numbers`.
- Fixed rounding in `SuffixLargeIntFormatter`: values just below a suffix boundary printed as `100K` instead of `1M`. Added `FormatCost` tests for it.

## Planned changes

- [x] Bump to 0.3.4 (includes the rounding fix) and publish `v0.3.4` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
<!-- review-items:start -->
- [ ] **P1** Remove the static `StringBuilder` (`[ThreadStatic]`, a pooled builder, or a stack `Span<char>`) and add a concurrent-use test.
- [ ] **P1** Drop the `large-numbers` entry from `package.json`; the `LARGE_NUMBERS` version define already makes it optional (same change as in `static-data`).
- [ ] **P1** Make the separator culture-aware (`FormatAmount(value, culture)`), invariant by default, with tests for `de`, `fr` and `ar`.
- [ ] **P1** Declares `unity: 2022.3`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P2** Model suffix sets on CLDR compact patterns (short and long forms, plural aware) and let a `SuffixProvider` plug them in. Optionally reuse the plural rules of `localizer`.
- [ ] **P2** Add `TryFormat(Span<char>)` overloads.
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Unicode CLDR / ICU compact number formatting](https://unicode-org.github.io/icu-docs/apidoc/released/icu4j/com/ibm/icu/text/CompactDecimalFormat.html) | Unicode / ICU | Compact "short" and "long" formats per locale ("1.2K" in English, "1,2 Mrd." in German), with plural-aware patterns. |

### Findings from reading the code

- **[Threading]** `SuffixLongFormatter` writes into a static shared `StringBuilder` (`SuffixLongFormatter.cs`, line 12). That is fine on the main thread and unsafe from jobs or async code.
- **[i18n]** The decimal separator is a `char` constructor argument and the suffixes come from a `SuffixProvider`. Neither is tied to `CultureInfo` or to locale data, while CLDR defines short and long compact patterns per language.
- **[Allocations]** Every call returns a new `string`; there is no `TryFormat(Span<char>)`.
- **[Coupling]** `large-numbers` is still a declared dependency although the code already detects it through a version define.
<!-- review:end -->

## Notes and ideas

_Add your own here._
