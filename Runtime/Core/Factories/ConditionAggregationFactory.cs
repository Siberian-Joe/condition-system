using System;
using System.Collections.Generic;
using ConditionSystem.Runtime.Core.Abstractions;
using UnityEngine;

namespace ConditionSystem.Runtime.Core.Factories
{
    public sealed class ConditionAggregationFactory : IConditionAggregationFactory
    {
        #region Fields

        private readonly IConditionMetadataProvider _metadataProvider;
        private readonly IConditionInstanceFactory _instanceFactory;

        #endregion

        #region Constructors

        public ConditionAggregationFactory(
            IConditionMetadataProvider metadataProvider,
            IConditionInstanceFactory instanceFactory)
        {
            _metadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
            _instanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        #endregion

        #region IConditionAggregationFactory

        public IConditionAggregation Create(IConditionAggregationDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Null aggregation definition received. " +
                    "This aggregation will be treated as 'always false'.");
                return new BrokenConditionAggregation();
            }

            var aggregationType = ResolveAggregationTypeSafe(definition);
            return aggregationType == null
                ? new BrokenConditionAggregation()
                : CreateAggregationInstanceSafe(aggregationType, definition);
        }

        #endregion

        #region Private Methods

        private Type ResolveAggregationTypeSafe(IConditionAggregationDefinition definition)
        {
            try
            {
                var definitionType = definition.GetType();
                var meta = _metadataProvider.GetMetadata(definitionType);
                var aggregationType = meta.ImplementationType;

                if (typeof(IConditionAggregation).IsAssignableFrom(aggregationType))
                    return aggregationType;

                Debug.LogError(
                    $"[{GetType().Name}] Aggregation type {aggregationType.FullName} " +
                    $"must implement {nameof(IConditionAggregation)}. " +
                    "This aggregation will be treated as 'always false'.");

                return null;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Failed to resolve metadata for aggregation " +
                    $"definition '{definition}'. This aggregation will be treated as 'always false'.\n{exception}");
                return null;
            }
        }

        private IConditionAggregation CreateAggregationInstanceSafe(
            Type aggregationType,
            IConditionAggregationDefinition definition)
        {
            try
            {
                return _instanceFactory.CreateAggregation(aggregationType, definition);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Failed to instantiate aggregation " +
                    $"{aggregationType.FullName}. This aggregation will be treated as 'always false'.\n{exception}");
                return new BrokenConditionAggregation();
            }
        }

        #endregion

        #region Nested Types

        private sealed class BrokenConditionAggregation : IConditionAggregation
        {
            #region IConditionAggregation

            public bool Aggregate(IEnumerable<bool> states) => false;

            #endregion
        }

        #endregion
    }
}