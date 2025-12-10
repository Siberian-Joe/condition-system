using ConditionSystem.Runtime.Core.Aggregations;
using ConditionSystem.Runtime.Core.Metadata;
using UnityEngine;

namespace ConditionSystem.Runtime.Unity.Definitions.Aggregations
{
    [CreateAssetMenu(menuName = "Conditions/Aggregations/All")]
    [ConditionDefinitionMetadata(typeof(AllConditionAggregation))]
    public sealed class AllConditionAggregationDefinitionAsset : ConditionAggregationDefinitionAsset
    {
    }
}