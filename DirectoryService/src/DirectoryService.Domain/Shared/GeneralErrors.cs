namespace DevQuestions.Domain.Shared;

public static class GeneralErrors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation(null, $"{label} is invalid!");
        }

        public static Error NotFound(Guid? id = null)
        {
            string forId = id == null ? string.Empty : $"for id '{id}'";
            return Error.NotFound(null, $"record not found {forId}");
        }

        public static Error ValueIsRequired(string? name = null)
        {
            string label = name ?? "value";
            return Error.Validation("lenght is invalid", $"{label} lenght is invalid!");
        }
    }
}