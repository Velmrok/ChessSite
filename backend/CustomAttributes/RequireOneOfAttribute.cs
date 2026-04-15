namespace backend.CustomAttributes;

using System.ComponentModel.DataAnnotations;

public class RequireOneOfAttribute : ValidationAttribute
{
    private readonly string[] _properties;

    public RequireOneOfAttribute(params string[] properties)
    {
        _properties = properties;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var hasValue = _properties.Any(prop =>
        {
            var propInfo = context.ObjectType.GetProperty(prop);
            var propValue = propInfo?.GetValue(context.ObjectInstance);
            return propValue is string str && !string.IsNullOrWhiteSpace(str);
        });

        if (!hasValue)
        {
            return new ValidationResult($"One of the following fields is required: {string.Join(", ", _properties)}");
        }

        return ValidationResult.Success;
    }
}