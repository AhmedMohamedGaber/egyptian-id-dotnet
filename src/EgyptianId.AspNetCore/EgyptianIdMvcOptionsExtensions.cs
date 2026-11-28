using Microsoft.AspNetCore.Mvc;

namespace EgyptianId.AspNetCore;

/// <summary>Registration helpers for the EgyptianId ASP.NET Core integration.</summary>
public static class EgyptianIdMvcOptionsExtensions
{
    /// <summary>
    /// Registers the <see cref="NationalIdInfo"/> model binder ahead of the built-in binders.
    /// After this, controller actions can take <c>NationalIdInfo</c> parameters directly:
    /// <c>public IActionResult Get(NationalIdInfo id)</c>.
    /// </summary>
    public static MvcOptions AddEgyptianId(this MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.ModelBinderProviders.Insert(0, new NationalIdInfoModelBinderProvider());
        return options;
    }
}
