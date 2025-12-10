using System;
using ConditionSystem.Runtime.Core.Abstractions;
using UnityEngine;

namespace ConditionSystem.Runtime.Unity.Definitions.Conditions
{
    [Serializable]
    public sealed class ConditionEntry : IConditionEntryDefinition
    {
        #region Serialize Fields

        [SerializeField] private ConditionDefinitionAsset _condition;
        [SerializeField] private ConditionUsage _usage;

        #endregion

        #region Properties

        public IConditionDefinition Condition => _condition;
        public ConditionUsage Usage => _usage;

        #endregion
    }
}