using AniRay.Model;
using AniRay.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AniRay.Services.HelperServices.OtherHelpers.CoreData;

namespace AniRay.Services.HelperServices.OtherHelpers
{
    public static class UpsertHelper
    {

        //Basic Checks
        public static ServiceResult<bool> ValidateStringLength(string? value, int minLength, int maxLength, string attributeName, bool nullsAllowed)
        {
            if (!nullsAllowed && (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)))
                return ServiceResult<bool>.Fail($"{attributeName} cannot be null!");

            if (nullsAllowed && value == null)
                return ServiceResult<bool>.Ok(true);

            value = value?.Trim();
            if (value.Length < minLength || value.Length > maxLength)
                return ServiceResult<bool>.Fail($"{attributeName} cannot be less than {minLength} characters or exceed {maxLength} characters!");

            return ServiceResult<bool>.Ok(true);
        }
        public static ServiceResult<bool> ValidateAmount(int? amount, int minValue, int maxValue, string attributeName, bool nullsAllowed)
        {
            if (!nullsAllowed && amount == null)
                return ServiceResult<bool>.Fail($"{attributeName} cannot be null!");

            if (nullsAllowed && amount == null)
                return ServiceResult<bool>.Ok(true);

            if (amount < minValue || amount > maxValue)
                return ServiceResult<bool>.Fail($"{attributeName} amount must be between {minValue} and {maxValue}!");

            return ServiceResult<bool>.Ok(true);
        }
        public static ServiceResult<bool> ValidateDate(DateOnly? dateToCheck, DateOnly minDate, DateOnly maxDate, string attributeName, bool nullsAllowed)
        {
            if (!nullsAllowed && dateToCheck == null)
                return ServiceResult<bool>.Fail($"{attributeName} cannot be null!");

            if (nullsAllowed && dateToCheck == null)
                return ServiceResult<bool>.Ok(true);

            if (dateToCheck < minDate || dateToCheck > maxDate)
                return ServiceResult<bool>.Fail($"Date cannot be before {minDate} or after {maxDate}");

            return ServiceResult<bool>.Ok(true);
        }
        public static ServiceResult<bool> ValidatePrice(decimal? price, string attributeName, bool nullsAllowed)
        {
            if (!nullsAllowed && price == null)
                return ServiceResult<bool>.Fail($"{attributeName} cannot be null!");

            if (nullsAllowed && price == null)
                return ServiceResult<bool>.Ok(true);

            if (price <= 0)
                return ServiceResult<bool>.Fail($"{attributeName} must not be less than 0!");

            return ServiceResult<bool>.Ok(true);
        }

        //Regex Checks
        public static ServiceResult<bool> ValidatePhoneRegex(string? value, int minLength, int maxLength, string attributeName, bool nullsAllowed)
        {
            ServiceResult<bool> result;

            result = ValidateStringLength(value, minLength, maxLength, attributeName, nullsAllowed);
            if (!result.Success) return result;

            value = value.Trim();

            var phoneRegex = new System.Text.RegularExpressions.
                Regex(@"^\+?[0-9\s\-\(\)]{6,20}$");

            if (!phoneRegex.IsMatch(value))
                return ServiceResult<bool>.Fail($"{attributeName} is not a valid phone number pattern!");

            return ServiceResult<bool>.Ok(true);
        }
        public static ServiceResult<bool> ValidatePasswordRegex(string? value1, string? value2, int minLength, int maxLength, string attributeName, bool nullsAllowed)
        {
            ServiceResult<bool> result;

            if (nullsAllowed && value1 == null)
                return ServiceResult<bool>.Ok(true);

            if (value1 != value2)
                return ServiceResult<bool>.Fail($"Passwords do not match");

            result = ValidateStringLength(value1, minLength, maxLength, attributeName, nullsAllowed);
            if (!result.Success) return result;

            var passwordRegex = new System.Text.RegularExpressions.
                Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (!passwordRegex.IsMatch(value1))
                return ServiceResult<bool>.Fail($"{attributeName} must contain at least " +
                    $"8 characters, one uppercase letter, one lowercase letter, one number, and one special character!");

            return ServiceResult<bool>.Ok(true);
        }
        private static ServiceResult<bool> ValidateEmailRegex(string? value, string attributeName)
        {
            value = value.Trim();

            var emailRegex = new System.Text.RegularExpressions.
                Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!emailRegex.IsMatch(value))
                return ServiceResult<bool>.Fail($"{attributeName} is not a valid email address pattern!");

            return ServiceResult<bool>.Ok(true);
        }

        //Foreign Keys Exist Check
        public static async Task<ServiceResult<bool>> ValidateForeignKey<T>(DbContext context, int? foreignKeyId, string fieldName, CancellationToken cancellationToken, bool nullsAllowed)
            where T : class
        {
            if (!nullsAllowed && foreignKeyId == null)
                return ServiceResult<bool>.Fail($"{fieldName} cannot be null!");

            if (nullsAllowed && foreignKeyId == null)
                return ServiceResult<bool>.Ok(true);

            var exists = await context.Set<T>()
                .AnyAsync(e => EF.Property<int>(e, "Id") == foreignKeyId, cancellationToken);

            if (!exists)
                return ServiceResult<bool>.Fail($"{fieldName} does not exist in database.");

            return ServiceResult<bool>.Ok(true);
        }
        public static async Task<ServiceResult<bool>> ValidateUserStatusId(DbContext context, int? foreignKeyId, int userRoleId, int userId, string fieldName, CancellationToken cancellationToken, bool nullsAllowed)
        {
            if (!nullsAllowed && foreignKeyId == null)
                return ServiceResult<bool>.Fail($"{fieldName} cannot be null!");

            if (nullsAllowed && foreignKeyId == null)
                return ServiceResult<bool>.Ok(true);

            var foreignKey = await context.Set<UserStatus>().FirstOrDefaultAsync(u => u.Id == foreignKeyId, cancellationToken);
            if(foreignKey == null)
                return ServiceResult<bool>.Fail($"{fieldName} does not exist in database.");

            if (userRoleId == (int)CoreData.CoreUserRole.User && foreignKey.StatusForUser == false)
                return ServiceResult<bool>.Fail($"Cannot attatch Employe Only Status to a User.");

            if (userRoleId != (int)CoreData.CoreUserRole.User && foreignKey.StatusForEmployee == false)
                return ServiceResult<bool>.Fail($"Cannot attatch User Only Status to an Employee.");

            if(foreignKeyId != (int)CoreData.CoreUserStatus.Active)
            {
                var token = await context.Set<RefreshToken>().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
                if (token == null)
                    return ServiceResult<bool>.Fail("Token not found in database");

                token.Revoked = true;
            }

            return ServiceResult<bool>.Ok(true);
        }

        //String Uniqueness Check (Email, Username etc.)
        public static async Task<ServiceResult<bool>> ValidateStringUniqueness<T>
            (DbContext context, string attributeName, string? value, string fieldName, int? userIdFromToken, CancellationToken cancellationToken, bool nullsAllowed)
            where T : class
        {
            var query = context.Set<T>().AsQueryable();
            query = query.Where(e => EF.Property<string>(e, attributeName) == value);

            if (userIdFromToken != null)
                query = query.Where(e => EF.Property<int>(e, "Id") != userIdFromToken);

            bool exists = await query.AnyAsync(cancellationToken);

            if (exists)
                return ServiceResult<bool>.Fail($"{fieldName} already exists in database.");

            return ServiceResult<bool>.Ok(true);
        }

        //Email Check
        public static async Task<ServiceResult<bool>> ValidateEmailAsync<T>(
            DbContext context, string? email, int minLength, int maxLength, string attributeName,
            int? userIdFromToken, CancellationToken cancellationToken, bool nullsAllowed)
            where T : class
        {
            if(nullsAllowed && email == null)
                return ServiceResult<bool>.Ok(true);

            var result = ValidateStringLength(email, minLength, maxLength, attributeName, nullsAllowed);
            if (!result.Success) return result;

            result = ValidateEmailRegex(email, attributeName);
            if (!result.Success) return result;

            result = await ValidateStringUniqueness<T>(
                context,
                attributeName,
                email,
                attributeName,
                userIdFromToken,
                cancellationToken,
                nullsAllowed);

            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }
        
        //Username Check
        public static async Task<ServiceResult<bool>> ValidateUsernameAsync<T>(
            DbContext context, string? username, int minLength, int maxLength, string attributeName,
            int? userIdFromToken, CancellationToken cancellationToken, bool nullsAllowed)
            where T : class
        {
            var result = ValidateStringLength(username, minLength, maxLength, attributeName, nullsAllowed);
            if (!result.Success) return result;

            result = await ValidateStringUniqueness<T>(
                context,
                attributeName,
                username,
                attributeName,
                userIdFromToken,
                cancellationToken,
                nullsAllowed);

            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }

    }
}