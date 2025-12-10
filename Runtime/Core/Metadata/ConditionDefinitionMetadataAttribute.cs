using System;

namespace ConditionSystem.Runtime.Core.Metadata
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ConditionDefinitionMetadataAttribute : Attribute
    {
        #region Properties

        public Type ImplementationType { get; }

        #endregion

        #region Constructors

        public ConditionDefinitionMetadataAttribute(Type implementationType) =>
            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));

        #endregion
    }
}