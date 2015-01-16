using System.Collections.Generic;

namespace Deplagiator.Validation.Structures
{
    public class ValidationResult
    {
        public List<KeyValuePair<string, string>> ValidationMessages { get; private set; }

        public bool Succeded { get { return ValidationMessages.Count == 0; } }

        public ValidationResult()
        {
            ValidationMessages = new List<KeyValuePair<string, string>>();
        }

        public void AddError(string property, string message)
        {
            ValidationMessages.Add(new KeyValuePair<string, string>(property, message));
        }

        public static bool operator true(ValidationResult result)
        {
            return result.Succeded;
        }

        public static bool operator false(ValidationResult result)
        {
            return result.Succeded;
        }
    }
}
