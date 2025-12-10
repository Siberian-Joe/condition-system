using System;
using System.Collections.Generic;
using System.Reflection;
using ConditionSystem.Runtime.Core.Abstractions;

namespace ConditionSystem.Runtime.Core.Metadata
{
    public sealed class ConditionMetadataProvider : IConditionMetadataProvider
    {
        #region Fiedls

        private readonly Dictionary<Type, ConditionDefinitionMetadataAttribute> _cache = new();

        #endregion

        #region IConditionMetadataProvider

        public ConditionDefinitionMetadataAttribute GetMetadata(Type definitionType)
        {
            if (definitionType == null)
                throw new ArgumentNullException(nameof(definitionType));

            if (_cache.TryGetValue(definitionType, out var cached))
                return cached;

            var attribute = definitionType.GetCustomAttribute<ConditionDefinitionMetadataAttribute>();

            _cache[definitionType] = attribute ?? throw new InvalidOperationException(
                $"Type {definitionType.FullName} has no [{nameof(ConditionDefinitionMetadataAttribute)}].");
            return attribute;
        }

        #endregion
    }
}