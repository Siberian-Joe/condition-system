using ConditionSystem.Runtime.Core.Aggregations;
using ConditionSystem.Runtime.Core.Metadata;
using UnityEngine;

namespace ConditionSystem.Runtime.Unity.Definitions.Aggregations
{
    [CreateAssetMenu(menuName = "Conditions/Aggregations/Any")]
    [ConditionDefinitionMetadata(typeof(AnyConditionAggregation))]
    public sealed class AnyConditionAggregationDefinitionAsset : ConditionAggregationDefinitionAsset
    {
    }
}