using System;
using ConditionSystem.Runtime.Core.Metadata;

namespace ConditionSystem.Runtime.Core.Abstractions
{
    public interface IConditionMetadataProvider
    {
        ConditionDefinitionMetadataAttribute GetMetadata(Type definitionType);
    }
}