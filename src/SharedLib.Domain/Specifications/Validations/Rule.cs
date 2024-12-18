namespace SharedLib.Domain.Specifications.Validations
{
    public class Rule<T>(Specification<T> spec, string errorMessage)
    {
        private readonly Specification<T> _specificationSpec = spec;

        public string ErrorMessage { get; } = errorMessage;

        public bool Validate(T obj) => _specificationSpec.IsSatisfiedBy(obj);
    }
}
