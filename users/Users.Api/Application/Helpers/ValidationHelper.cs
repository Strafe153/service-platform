using System.Text.RegularExpressions;

namespace Users.Api.Application.Validation;

public static partial class ValidationHelper
{
    public static bool BeValidBirthDate(DateOnly date)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var maxDate = currentDate.AddYears(-18);
        var minDate = currentDate.AddYears(-138);

        var isValidDate = date >= minDate && date <= maxDate;

        return isValidDate;
    }

    [GeneratedRegex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$")]
    public static partial Regex PhoneNumber();
}