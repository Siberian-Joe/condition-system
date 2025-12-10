using System.Collections.Generic;
using System.Linq;
using ConditionSystem.Runtime.Core.Abstractions;

namespace ConditionSystem.Runtime.Core.Aggregations
{
    public sealed class AllConditionAggregation : IConditionAggregation
    {
        #region IConditionAggregation

        public bool Aggregate(IEnumerable<bool> states) =>
            states != null && states
                .DefaultIfEmpty(false)
                .All(state => state);

        #endregion
    }
}