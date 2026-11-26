# EgyptianId

[![NuGet](https://img.shields.io/nuget/v/EgyptianId.svg)](https://www.nuget.org/packages/EgyptianId)
[![Downloads](https://img.shields.io/nuget/dt/EgyptianId.svg)](https://www.nuget.org/packages/EgyptianId)
[![CI](https://github.com/AhmedMohamedGaber/egyptian-id-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/AhmedMohamedGaber/egyptian-id-dotnet/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

Offline validation and parsing of the 14-digit Egyptian national ID number (الرقم القومي) for .NET.

Give it a number. Get back the birth date, gender and governorate, or the exact reason it was rejected. No network calls, no dependencies.

## Install

```bash
dotnet add package EgyptianId
```

Package page: https://www.nuget.org/packages/EgyptianId. Targets .NET 6 and .NET 8.

## Quick start

```csharp
using EgyptianId;

var result = NationalIdValidator.Validate("29902140100115");

if (result.IsValid)
{
    var info = result.Info!;
    Console.WriteLine(info.BirthDate);                      // 1999-02-14
    Console.WriteLine(info.Gender);                         // Male
    Console.WriteLine(info.BirthGovernorate.ArabicName());  // القاهرة
    Console.WriteLine(info.BirthGovernorate.EnglishName()); // Cairo
    Console.WriteLine(info.Age);                            // completed years today
    Console.WriteLine(info.IsAdult());                      // true
    Console.WriteLine(info.Masked);                         // **********0115
    Console.WriteLine(info.Formatted);                      // 2 990214 01 0011 5
}
else
{
    Console.WriteLine(result.Error); // Empty, NonDigit, InvalidLength, InvalidCentury, ...
}
```

## Features

- **Arabic and Persian digits.** `٢٩٩٠٢١٤٠١٠٠١١٥` works as is. Spaces, hyphens, non-breaking spaces and the invisible direction marks that come with copied Arabic text are ignored.
- **Exact error reasons.** `Empty`, `NonDigit`, `InvalidLength`, `InvalidCentury`, `InvalidBirthDate`, `FutureBirthDate`, `InvalidGovernorate`.
- **Decoded data.** Birth date, gender, governorate (Arabic and English names), age on any date, `IsAdult`.
- **Safe to log.** `Masked` hides all but the last four digits.
- **Validation attribute.** `[EgyptianNationalId]` for ASP.NET, Blazor and any DataAnnotations model.
- **Test data generator.** `NationalIdGenerator` makes valid random numbers, repeatable with a seed.
- **Output helpers.** `NationalIdFormatter.ToArabicIndicDigits`, `Mask`, `Format`.
- **Tested.** Unit tests cover every error, leap days, age edges, Arabic input and the generator. CI runs on every push.

## ASP.NET and DataAnnotations

```csharp
public class RegisterRequest
{
    [Required]
    [EgyptianNationalId]
    public string? NationalId { get; set; }
}
```

Null and empty values pass the attribute, so add `[Required]` when the field is mandatory.

## Generate test numbers

```csharp
var number = NationalIdGenerator.Generate();
var seeded = NationalIdGenerator.Generate(new Random(42), Gender.Female, Governorate.Giza, new DateOnly(2001, 9, 5));
```

These numbers are random. They are not issued to anyone.

## What it checks

| Digits | Meaning | Rule |
| --- | --- | --- |
| 1 | Century | `2` = 1900s, `3` = 2000s |
| 2-7 | Birth date (YYMMDD) | Must be a real calendar date, not in the future |
| 8-9 | Governorate | Must be an assigned code (01-04, 11-19, 21-29, 31-35, 88) |
| 10-13 | Serial | Digit 13 is odd for male, even for female |
| 14 | Check digit | **Not checked** |

## Limits

- The check digit has no officially published algorithm, so this library does not validate it. A number that passes here has the right structure. It is not proof the number was issued to anyone.
- Do not use this as identity verification. Use it to catch typos early in forms and imports.
- Treat national ID numbers as personal data. Mask them in logs.

## API

- `NationalIdValidator.Validate(string?, DateOnly? today = null)` returns `NationalIdResult` (`IsValid`, `Error`, `Info`).
- `NationalIdValidator.IsValid`, `TryParse`, `Parse` (throws `FormatException`), `Normalize`.
- `NationalIdInfo`: `Number`, `BirthDate`, `Gender`, `BirthGovernorate`, `Age`, `AgeOn(DateOnly)`, `IsAdult(DateOnly?)`, `Masked`, `Formatted`.
- `Governorate.ArabicName()`, `Governorate.EnglishName()`.
- `NationalIdFormatter.Mask`, `Format`, `ToArabicIndicDigits`.
- `NationalIdGenerator.Generate`.
- `EgyptianNationalIdAttribute`.

## Build and test

```bash
dotnet test tests/EgyptianId.Tests/EgyptianId.Tests.csproj
```

## Contributing

Issues and PRs are welcome. If you know of an official source for the check digit, please open an issue with the link.

## License

MIT
