<div dir="rtl">

# EgyptianId

[English](README.md) · **العربية**

[![NuGet](https://img.shields.io/nuget/v/EgyptianId.svg)](https://www.nuget.org/packages/EgyptianId)
[![Downloads](https://img.shields.io/nuget/dt/EgyptianId.svg)](https://www.nuget.org/packages/EgyptianId)
[![CI](https://github.com/AhmedMohamedGaber/egyptian-id-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/AhmedMohamedGaber/egyptian-id-dotnet/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

مكتبة .NET للتحقق من الرقم القومي المصري المكوّن من 14 رقمًا واستخراج بياناته، دون أي اتصال بالإنترنت.

أعطها الرقم، وتحصل على تاريخ الميلاد والنوع والمحافظة، أو السبب الدقيق للرفض. بدون اتصالات شبكية وبدون أي اعتماديات.

## الحزم

| الحزمة | NuGet | ماذا تضيف |
| --- | --- | --- |
| [EgyptianId](https://www.nuget.org/packages/EgyptianId) | [![NuGet](https://img.shields.io/nuget/v/EgyptianId.svg)](https://www.nuget.org/packages/EgyptianId) | الأساس: التحقق واستخراج البيانات والعمر والمحافظات والتنسيق ومحوّل JSON ومولّد أرقام للاختبار. بدون أي اعتماديات. |
| [EgyptianId.AspNetCore](https://www.nuget.org/packages/EgyptianId.AspNetCore) | [![NuGet](https://img.shields.io/nuget/v/EgyptianId.AspNetCore.svg)](https://www.nuget.org/packages/EgyptianId.AspNetCore) | ربط النماذج في ASP.NET Core: اربط قيمة الطلب مباشرة بكائن `NationalIdInfo` بعد فكّ ترميزه. |
| [EgyptianId.FluentValidation](https://www.nuget.org/packages/EgyptianId.FluentValidation) | [![NuGet](https://img.shields.io/nuget/v/EgyptianId.FluentValidation.svg)](https://www.nuget.org/packages/EgyptianId.FluentValidation) | قاعدة `IsValidEgyptianNationalId()` مع رسالة دقيقة ورمز خطأ برمجي لكل سبب رفض. |

الحزم الثلاث تدعم .NET 6 و .NET 8.

## التثبيت

```bash
dotnet add package EgyptianId                  # الأساس: التحقق واستخراج البيانات
dotnet add package EgyptianId.AspNetCore       # اختياري: ربط النماذج في ASP.NET Core
dotnet add package EgyptianId.FluentValidation # اختياري: قاعدة FluentValidation
```

حزمتا التكامل تعتمدان على الأساسية، فأضف فقط ما تستخدمه.

## البداية السريعة

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
    Console.WriteLine(info.Age);                            // العمر بالسنوات الكاملة
    Console.WriteLine(info.IsAdult());                      // true
    Console.WriteLine(info.Masked);                         // **********0115
    Console.WriteLine(info.Formatted);                      // 2 990214 01 0011 5
}
else
{
    Console.WriteLine(result.Error); // Empty, NonDigit, InvalidLength, InvalidCentury, ...
}
```

## المميزات

- **الأرقام العربية والفارسية.** `٢٩٩٠٢١٤٠١٠٠١١٥` تعمل كما هي. المسافات والشرطات والمسافات غير المنقطعة وعلامات الاتجاه غير المرئية التي تأتي مع النص العربي المنسوخ تُتجاهل تلقائيًا.
- **أسباب رفض دقيقة.** `Empty` و `NonDigit` و `InvalidLength` و `InvalidCentury` و `InvalidBirthDate` و `FutureBirthDate` و `InvalidGovernorate`.
- **بيانات مستخرجة.** تاريخ الميلاد والنوع والمحافظة (بالاسمين العربي والإنجليزي) والعمر في أي تاريخ و `IsAdult`.
- **آمن للسجلات.** `Masked` يخفي كل الأرقام عدا الأربعة الأخيرة.
- **سمة تحقق.** `[EgyptianNationalId]` لـ ASP.NET و Blazor وأي نموذج DataAnnotations.
- **ربط نماذج ASP.NET Core.** الكنترولرز و Minimal APIs تستقبل `NationalIdInfo` مباشرة.
- **قاعدة FluentValidation.** سطر واحد يعطي رسائل دقيقة ورموز أخطاء يمكن ترجمتها.
- **System.Text.Json مدمج.** `NationalIdInfo` يُسلسل ويُقرأ كالرقم نصًّا بدون أي إعداد.
- **مولّد أرقام اختبار.** `NationalIdGenerator` ينتج أرقامًا صالحة عشوائية، وقابلة للتكرار ببذرة ثابتة.
- **أدوات إخراج.** `NationalIdFormatter.ToArabicIndicDigits` و `Mask` و `Format`.
- **مختبَرة بالكامل.** اختبارات تغطي كل أسباب الرفض والسنوات الكبيسة وحالات العمر الحدّية والإدخال العربي والمولّد وكل التكاملات. CI يعمل مع كل push.

## ASP.NET و DataAnnotations

```csharp
public class RegisterRequest
{
    [Required]
    [EgyptianNationalId]
    public string? NationalId { get; set; }
}
```

القيم الفارغة تجتاز السمة، لذا أضف `[Required]` عندما يكون الحقل إلزاميًا.

## ربط النماذج في ASP.NET Core

الحزمة `EgyptianId.AspNetCore`.

```csharp
builder.Services.AddControllers(options => options.AddEgyptianId());
```

بعدها يستقبل الإكشن القيمة مفكوكة مباشرة. الرقم غير الصالح يدخل `ModelState` مع سبب دقيق، و`[ApiController]` يحوّل ذلك تلقائيًا إلى استجابة 400:

```csharp
[HttpGet("/citizens/{id}")]
public IActionResult Get(NationalIdInfo id)
{
    return Ok(new { id.BirthDate, id.BirthGovernorate, age = id.Age });
}
```

أما Minimal APIs فلا تحتاج أي إعداد: `NationalIdInfo` يطبّق الشكل المطلوب من `TryParse` الذي تربط به Minimal APIs قيم المسارات والاستعلامات.

```csharp
app.MapGet("/citizens/{id}", (NationalIdInfo id) => Results.Ok(id.Age));
```

## FluentValidation

الحزمة `EgyptianId.FluentValidation`.

```csharp
using FluentValidation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.NationalId).NotEmpty().IsValidEgyptianNationalId();
    }
}
```

كل رسالة فشل تذكر السبب الدقيق (مثل «يجب أن يتكون من 14 رقمًا بالضبط» أو «رمز محافظة غير معروف») ويحمل `ErrorCode` اسم قيمة `NationalIdError`، فيمكن لواجهة عربية أن تربط كل رمز بصياغتها الخاصة. مرّر `allowNullOrEmpty: true` للحقول الاختيارية.

## JSON

بدون أي إعداد. `NationalIdInfo` يحمل المحوّل الخاص به، فيسلسله `System.Text.Json` كنص من 14 رقمًا ويقرؤه مع التحقق:

```csharp
var json = JsonSerializer.Serialize(info);                    // "29902140100115"
var back = JsonSerializer.Deserialize<NationalIdInfo>(json);  // يرمي JsonException عند رقم غير صالح
```

## توليد أرقام للاختبار

```csharp
var number = NationalIdGenerator.Generate();
var seeded = NationalIdGenerator.Generate(new Random(42), Gender.Female, Governorate.Giza, new DateOnly(2001, 9, 5));
```

هذه الأرقام عشوائية وغير صادرة لأي شخص.

## ماذا تتحقق المكتبة

| الأرقام | المعنى | القاعدة |
| --- | --- | --- |
| 1 | القرن | `2` = مواليد القرن العشرين، `3` = مواليد القرن الحادي والعشرين |
| 2-7 | تاريخ الميلاد (YYMMDD) | تاريخ تقويمي حقيقي وليس في المستقبل |
| 8-9 | المحافظة | رمز معتمد (01-04، 11-19، 21-29، 31-35، 88) |
| 10-13 | الرقم التسلسلي | الرقم 13 فردي للذكر وزوجي للأنثى |
| 14 | رقم التحقق | **لا يُتحقق منه** |

## الحدود

- لا توجد خوارزمية رسمية منشورة لرقم التحقق، لذا لا تتحقق المكتبة منه. الرقم الذي يجتاز هنا بنيته سليمة، وهذا ليس إثباتًا أنه صادر لشخص فعلي.
- لا تستخدم المكتبة كتحقق من الهوية. استخدمها لالتقاط أخطاء الكتابة مبكرًا في النماذج والاستيراد.
- عامل أرقام الهوية كبيانات شخصية، واحجبها في السجلات.

## الواجهة البرمجية

- `NationalIdValidator.Validate(string?, DateOnly? today = null)` ترجع `NationalIdResult` (فيه `IsValid` و `Error` و `Info`).
- `NationalIdValidator.IsValid` و `TryParse` و `Parse` (يرمي `FormatException`) و `Normalize`.
- `NationalIdInfo`: `Number` و `BirthDate` و `Gender` و `BirthGovernorate` و `Age` و `AgeOn(DateOnly)` و `IsAdult(DateOnly?)` و `Masked` و `Formatted`، إضافة إلى `NationalIdInfo.TryParse` لربط Minimal APIs.
- `Governorate.ArabicName()` و `Governorate.EnglishName()`.
- `NationalIdFormatter.Mask` و `Format` و `ToArabicIndicDigits`.
- `NationalIdGenerator.Generate`.
- `EgyptianNationalIdAttribute`.
- `NationalIdInfoJsonConverter` (مسجَّل مسبقًا على `NationalIdInfo`).
- `EgyptianId.AspNetCore`: `NationalIdInfoModelBinder` و `NationalIdInfoModelBinderProvider` و `MvcOptions.AddEgyptianId()`.
- `EgyptianId.FluentValidation`: `IsValidEgyptianNationalId(allowNullOrEmpty: false)`.

## البناء والاختبار

```bash
dotnet test EgyptianId.sln
```

## المساهمة

المشكلات وطلبات الدمج مرحّب بها. إذا عرفت مصدرًا رسميًا لخوارزمية رقم التحقق، افتح مشكلة وأرفق الرابط.

## الرخصة

MIT

</div>
