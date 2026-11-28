using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace EgyptianId.AspNetCore;

/// <summary>
/// Supplies <see cref="NationalIdInfoModelBinder"/> for action parameters and
/// model properties of type <see cref="NationalIdInfo"/>.
/// </summary>
public sealed class NationalIdInfoModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.Metadata.ModelType == typeof(NationalIdInfo)
            ? new BinderTypeModelBinder(typeof(NationalIdInfoModelBinder))
            : null;
    }
}
