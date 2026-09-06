using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Application.ErrorHandling
{
    public sealed class ErrorMapping
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
    }

    public static class ApiErrorMapper
    {
        private const int StatusConflict = 409;
        private const int StatusBadRequest = 400;

        private const string GenericDuplicateMessage = "A record with the same unique value already exists.";
        private const string ForeignKeyMessage = "One or more referenced records do not exist. Please check the supplied Tenant/Apartment/Role IDs.";

        private static readonly Regex DuplicateIndexRegex =
            new Regex("Cannot insert duplicate key row in object '(?:\\w+\\.)?(?<table>\\w+)' with unique index 'IX_(?<entity>\\w+)_(?<field>\\w+)'",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DuplicateConstraintRegex =
            new Regex("Cannot insert duplicate key row in object '(?:\\w+\\.)?(?<table>\\w+)' with unique constraint '(?<constraint>\\w+)'",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DuplicateKeyRegex =
            new Regex("duplicate key|Cannot insert duplicate|unique index|unique constraint",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ForeignKeyRegex =
            new Regex("FOREIGN KEY constraint|conflicted with the (INSERT|UPDATE|DELETE) statement",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Dictionary<string, string> DuplicateMessageByField = new Dictionary<string, string>
        {
            ["Apartments_UnitNumber"] = "An apartment with this Unit Number already exists.",
            ["Users_Username"] = "A user with this Username already exists.",
            ["Users_Email"] = "A user with this Email already exists.",
            ["Users_TenantId"] = "A user is already linked to this Tenant.",
            ["Roles_RoleName"] = "A role with this name already exists."
        };

        public static ErrorMapping? TryMap(Exception ex)
        {
            if (ex == null) return null;

            var baseMessage = ex.GetBaseException().Message;

            if (DuplicateKeyRegex.IsMatch(baseMessage))
            {
                return new ErrorMapping
                {
                    StatusCode = StatusConflict,
                    Message = BuildDuplicateMessage(baseMessage)
                };
            }

            if (ForeignKeyRegex.IsMatch(baseMessage))
            {
                return new ErrorMapping
                {
                    StatusCode = StatusBadRequest,
                    Message = ForeignKeyMessage
                };
            }

            return null;
        }

        private static string BuildDuplicateMessage(string message)
        {
            var fieldMatch = DuplicateIndexRegex.Match(message);
            if (fieldMatch.Success)
            {
                var key = $"{fieldMatch.Groups["entity"].Value}_{fieldMatch.Groups["field"].Value}";
                if (TryGetDuplicateMessage(key, out var specificMessage))
                {
                    return specificMessage;
                }
            }

            var constraintMatch = DuplicateConstraintRegex.Match(message);
            if (constraintMatch.Success
                && TryGetDuplicateMessage(constraintMatch.Groups["constraint"].Value, out var constraintMessage))
            {
                return constraintMessage;
            }

            return GenericDuplicateMessage;
        }

        private static bool TryGetDuplicateMessage(string indexOrConstraint, out string message)
        {
            if (indexOrConstraint.StartsWith("IX_", StringComparison.OrdinalIgnoreCase))
            {
                if (DuplicateMessageByField.TryGetValue(indexOrConstraint.Substring(3), out message))
                {
                    return true;
                }
            }

            return DuplicateMessageByField.TryGetValue(indexOrConstraint, out message);
        }
    }
}