# EgyptianId

Offline validation and parsing of the 14-digit Egyptian national ID number (الرقم القومي) for .NET.

Give it a number. Get back the birth date, gender and governorate, or the exact reason it was rejected.

```csharp
using EgyptianId;

var result = NationalIdValidator.Validate("29902140100115");

if (result.IsValid)
{
    Console.WriteLine(result.Info!.BirthDate);                       // 1999-02-14
    Console.WriteLine(result.Info.Gender);                           // Male
    Console.WriteLine(result.Info.BirthGovernorate.ArabicName());    // القاهرة
    Console.WriteLine(result.Info.AgeOn(DateOnly.FromDateTime(DateTime.Today)));
}
else
{
    Console.WriteLine(result.Error); // Empty, NonDigit, InvalidLength, InvalidCentury, ...
}
```

Arabic-Indic digits (`٢٩٩٠٢١٤٠١٠٠١١٥`) work too. Spaces and hyphens are ignored.

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

## API

- `NationalIdValidator.Validate(string?, DateOnly? today = null)` returns `NationalIdResult` (`IsValid`, `Error`, `Info`).
- `NationalIdValidator.IsValid(string?)`
- `NationalIdValidator.TryParse(string?, out NationalIdInfo?)`
- `NationalIdInfo`: `Number`, `BirthDate`, `Gender`, `BirthGovernorate`, `AgeOn(DateOnly)`
- `Governorate.ArabicName()`

## Build and test

```bash
dotnet test tests/EgyptianId.Tests/EgyptianId.Tests.csproj
```

## Contributing

Issues and PRs are welcome. If you know of an official source for the check digit, please open an issue with the link.

## License

MIT
