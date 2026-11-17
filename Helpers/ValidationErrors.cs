using Microsoft.AspNetCore.Mvc.ModelBinding;
using Orders.Api.Models;

namespace Orders.Api.Helpers;

public static class ValidationErrors
{
    public static IReadOnlyList<FieldError> FromModelState(ModelStateDictionary modelState)
    {
        return modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .Select(entry => new FieldError(
                NormalizeField(entry.Key),
                entry.Value!.Errors.Select(ResolveMessage).ToArray()))
            .ToArray();
    }

    private static string ResolveMessage(ModelError error)
    {
        if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
        {
            return error.ErrorMessage;
        }

        return "The supplied value has an invalid format.";
    }

    private static string NormalizeField(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        var field = key.StartsWith("$.", StringComparison.Ordinal) ? key[2..] : key;
        var separator = field.LastIndexOf('.');
        return separator >= 0 ? field[(separator + 1)..] : field;
    }
}
