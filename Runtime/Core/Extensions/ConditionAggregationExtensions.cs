using System;
using ConditionSystem.Runtime.Core.Abstractions;
using R3;

namespace ConditionSystem.Runtime.Core.Extensions
{
    public static class ConditionAggregationExtensions
    {
        #region Public API

        public static Observable<bool> Aggregate(
            this IConditionAggregation aggregation,
            Observable<bool>[] states)
        {
            if (aggregation == null)
                throw new ArgumentNullException(nameof(aggregation));

            if (states != null && states.Length != 0)
            {
                return Observable
                    .CombineLatest(states)
                    .Select(aggregation.Aggregate)
                    .DistinctUntilChanged();
            }

            var initial = aggregation.Aggregate(Array.Empty<bool>());
            return Observable.Return(initial);
        }

        #endregion
    }
}