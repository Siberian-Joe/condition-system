using System.Collections.Generic;
using ConditionSystem.Runtime.Core.Abstractions;
using ConditionSystem.Runtime.Core.Handlers;
using ConditionSystem.Runtime.Core.Metadata;
using ConditionSystem.Runtime.Unity.Definitions.Actions;
using ConditionSystem.Runtime.Unity.Definitions.Aggregations;
using UnityEngine;

namespace ConditionSystem.Runtime.Unity.Definitions.Conditions
{
    [CreateAssetMenu(menuName = "Conditions/Samples/Composite Condition", fileName = "CompositeCondition")]
    [ConditionDefinitionMetadata(typeof(CompositeConditionHandler<>))]
    public sealed class CompositeConditionDefinitionAsset : ConditionDefinitionAsset, ICompositeConditionDefinition,
        IConditionReactionDefinition
    {
        #region Serialized Fields

        [Header("Requirements")] [SerializeField]
        private ConditionEntry[] _entries;

        [SerializeField] private ConditionAggregationDefinitionAsset _aggregation;

        [Header("Completion actions (optional)")] [SerializeField]
        private ConditionActionDefinitionAsset[] _actions;

        #endregion

        #region Properties

        public IReadOnlyList<IConditionEntryDefinition> Entries => _entries;
        public IConditionAggregationDefinition Aggregation => _aggregation;
        public IReadOnlyList<IConditionActionDefinition> Actions => _actions;

        #endregion

        #region Editor Callbacks

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_entries == null)
                return;

            for (var i = 0; i < _entries.Length; i++)
            {
                var entry = _entries[i];
                if (entry == null)
                    continue;

                if (ReferenceEquals(entry.Condition, this) == false)
                    continue;

                Debug.LogError(
                    $"[{nameof(CompositeConditionDefinitionAsset)}] Invalid configuration in asset '{name}': " +
                    $"entry at index {i} references this CompositeConditionDefinition itself. " +
                    "Self-references are not allowed and have been cleared. Please fix the configuration.",
                    this);

                _entries[i] = null;
            }
        }
#endif

        #endregion
    }
}