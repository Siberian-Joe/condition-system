using System.Collections.Generic;
using System.Linq;
using ConditionSystem.Runtime.Core.Abstractions;

namespace ConditionSystem.Runtime.Core.Aggregations
{
    public sealed class AnyConditionAggregation : IConditionAggregation
    {
        #region IConditionAggregation

        public bool Aggregate(IEnumerable<bool> states) =>
            states?.Any(state => state) == true;

        #endregion
    }
}