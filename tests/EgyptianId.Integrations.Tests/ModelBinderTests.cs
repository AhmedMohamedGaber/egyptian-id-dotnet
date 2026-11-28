using EgyptianId;
using EgyptianId.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace EgyptianId.Integrations.Tests;

public class ModelBinderTests
{
    private const string Valid = "29902140100115";

    private static DefaultModelBindingContext ContextFor(string? value)
    {
        var query = value is null
            ? new QueryCollection()
            : new QueryCollection(new Dictionary<string, StringValues> { ["id"] = value });

        return new DefaultModelBindingContext
        {
            ActionContext = new ActionContext { HttpContext = new DefaultHttpContext() },
            BindingSource = BindingSource.Query,
            FieldName = "id",
            ModelName = "id",
            IsTopLevelObject = true,
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(NationalIdInfo)),
            ModelState = new ModelStateDictionary(),
            ValueProvider = new QueryStringValueProvider(BindingSource.Query, query, System.Globalization.CultureInfo.InvariantCulture)
        };
    }

    [Fact]
    public async Task Binds_a_valid_value_to_a_decoded_info()
    {
        var context = ContextFor(Valid);

        await new NationalIdInfoModelBinder().BindModelAsync(context);

        Assert.True(context.Result.IsModelSet);
        var info = Assert.IsType<NationalIdInfo>(context.Result.Model);
        Assert.Equal(Valid, info.Number);
        Assert.Equal(new DateOnly(1999, 2, 14), info.BirthDate);
        Assert.Equal(Gender.Male, info.Gender);
        Assert.Equal(Governorate.Cairo, info.BirthGovernorate);
        Assert.Equal(0, context.ModelState.ErrorCount);
    }

    [Fact]
    public async Task Accepts_arabic_indic_digits()
    {
        var context = ContextFor("٢٩٩٠٢١٤٠١٠٠١١٥");

        await new NationalIdInfoModelBinder().BindModelAsync(context);

        Assert.True(context.Result.IsModelSet);
        Assert.Equal(Valid, Assert.IsType<NationalIdInfo>(context.Result.Model).Number);
    }

    [Theory]
    [InlineData("2990214010011", "14 digits")]
    [InlineData("29902149900115", "governorate")]
    [InlineData("hello", "digits only")]
    public async Task An_invalid_value_fails_binding_and_explains_why(string value, string reasonPart)
    {
        var context = ContextFor(value);

        await new NationalIdInfoModelBinder().BindModelAsync(context);

        Assert.False(context.Result.IsModelSet);
        Assert.False(context.ModelState.IsValid);
        var error = Assert.Single(context.ModelState["id"]!.Errors);
        Assert.Contains("not a valid Egyptian national ID number", error.ErrorMessage);
        Assert.Contains(reasonPart, error.ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task An_empty_value_binds_to_null_so_required_keeps_its_meaning(string value)
    {
        var context = ContextFor(value);

        await new NationalIdInfoModelBinder().BindModelAsync(context);

        Assert.True(context.Result.IsModelSet);
        Assert.Null(context.Result.Model);
        Assert.Equal(0, context.ModelState.ErrorCount);
    }

    [Fact]
    public async Task A_missing_value_leaves_the_result_untouched()
    {
        var context = ContextFor(null);

        await new NationalIdInfoModelBinder().BindModelAsync(context);

        Assert.False(context.Result.IsModelSet);
    }

    [Fact]
    public void The_provider_only_claims_national_id_info()
    {
        var provider = new NationalIdInfoModelBinderProvider();
        var metadataProvider = new EmptyModelMetadataProvider();

        var ownContext = new TestProviderContext(metadataProvider.GetMetadataForType(typeof(NationalIdInfo)));
        var otherContext = new TestProviderContext(metadataProvider.GetMetadataForType(typeof(string)));

        Assert.NotNull(provider.GetBinder(ownContext));
        Assert.Null(provider.GetBinder(otherContext));
    }

    [Fact]
    public void AddEgyptianId_registers_the_provider_first()
    {
        var options = new MvcOptions();

        var returned = options.AddEgyptianId();

        Assert.Same(options, returned);
        Assert.IsType<NationalIdInfoModelBinderProvider>(options.ModelBinderProviders[0]);
    }

    private sealed class TestProviderContext : ModelBinderProviderContext
    {
        private readonly EmptyModelMetadataProvider _metadataProvider = new();

        public TestProviderContext(ModelMetadata metadata)
        {
            Metadata = metadata;
        }

        public override BindingInfo BindingInfo { get; } = new();
        public override ModelMetadata Metadata { get; }
        public override IServiceProvider Services => throw new NotSupportedException();
        public override IModelMetadataProvider MetadataProvider => _metadataProvider;

        public override IModelBinder CreateBinder(ModelMetadata metadata)
        {
            throw new NotSupportedException();
        }
    }
}
