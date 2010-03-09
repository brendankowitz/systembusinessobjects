using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.BusinessObjects.Infrastructure
{
    /// <summary>
    /// A specification is a domain-level concept, albeit one carefully constructed to facilitate
    /// efficient data access.
    /// </summary>
    /// <example>
    /// <code>
    /// var overdueBooksSpec = new OverdueBooksSpecification();
    /// var overdue = bookRepository.Find(overdueBookSpec);
    /// </code>
    /// </example>
    public abstract class Specification<T>
    {
        Func<T, bool> _compiledPredicate;

        Func<T, bool> CompiledPredicate
        {
            get
            {
                _compiledPredicate = _compiledPredicate ?? Predicate.Compile();
                return _compiledPredicate;
            }
        }

        /// <summary>
        /// Returns true if the item is matched by this specification.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>The result of testing.</returns>
        public bool IsSatisfiedBy(T item)
        {
            return CompiledPredicate(item);
        }

        /// <summary>
        /// Return the elements that match the specification.
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        public IEnumerable<T> SatisfyingElementsFrom(IEnumerable<T> candidates)
        {
            return candidates.Where(CompiledPredicate);
        }

        /// <summary>
        /// Describes the selection criteria for the specification.
        /// </summary>
        public abstract Expression<Func<T, bool>> Predicate { get; }
    }
}
