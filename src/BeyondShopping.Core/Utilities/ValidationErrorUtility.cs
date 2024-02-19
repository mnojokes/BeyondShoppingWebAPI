using FluentValidation.Results;
using System.Text;

namespace BeyondShopping.Core.Utilities;

public static class ValidationErrorUtility
{
    public static string GetAllValidationErrorMessages(ValidationResult validationResult)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < validationResult.Errors.Count; ++i)
        {
            sb.Append(validationResult.Errors[i].ErrorMessage);
            if (i + 1 < validationResult.Errors.Count)
            {
                sb.Append(' ');
            }
        }

        return sb.ToString();
    }
}
