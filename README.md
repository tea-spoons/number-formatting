# Number Formatting
Formats numbers for ingame displays.

## Setup
Decide on a number formatter (`ILongFormatter`) to use, create an instance and store it somewhere for later access.
```csharp
new ExactLongFormatter(',')
new SuffixLongFormatter(new string[] { "k", "m", "b" })
```

## Usage
Use the interface methods `string FormatAmount(long)` and `string FormatCost(long)` for formatting.

*(The following examples use a `SuffixLongFormatter` as declared above.)*

### Summary
*Using `FormatAmount` for owned resources and `FormatCost` for costs ensures that a higher cost will always be formatted to a higher displayed number than the corresponding owned resource amount.*

### FormatAmount
`FormatAmount` rounds downwards, so the result will be less than or equal to the input number. Trailing zeros after the decimal point are removed for cleaner display.
```csharp
100 -> 100
1000 -> 1k
10000 -> 10k
10010 -> 10k // the 10 is cut off
10100 -> 10.1k
```
This behavior allows for displaying numbers that don't over-represent the amount of resources a player has.

### FormatCost
`FormatCost` will make sure that the resulting number will be greater than, if not equal to the input number. Trailing zeros after the decimal point are removed for cleaner display.
```csharp
100 -> 100
1000 -> 1k
10000 -> 10k
10010 -> 10.1k // the next higher number is displayed, given the available digits
```
This method is intended to be used for displaying costs.

### Example
Using both methods together correctly means that the player will never run into a situation where it looks like they have enough resources for a purchase, despite lacking them.

| Value | FormatAmount | FormatCost |
|-------|--------------|------------|
| Owned: **10001** | **10k** | 10.1k |
| Cost: **10002** | 10k | **10.1k** |


If both numbers were formatted with the same method, the player would be presented with the same number twice, but resources would actually not be enough for the purchase.

However, using `FormatAmount` for the owned amount and `FormatCost` for costs, the player will always see a higher cost when the cost is higher.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/number-formatting.git
```

Pin a release by appending a tag, for example `#v0.3.4`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.tea-spoons.large-numbers` 0.7.2

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
