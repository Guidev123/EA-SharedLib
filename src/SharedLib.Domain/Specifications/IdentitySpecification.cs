using System.Linq.Expressions;

namespace SharedLib.Domain.Specifications
{
    internal sealed class IdentitySpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> ToExpression() => x => true;
    }
}
