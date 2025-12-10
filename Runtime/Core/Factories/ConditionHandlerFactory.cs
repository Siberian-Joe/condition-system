using System;
using System.Collections.Generic;
using ConditionSystem.Runtime.Core.Abstractions;
using ConditionSystem.Runtime.Core.Handlers;
using UnityEngine;

namespace ConditionSystem.Runtime.Core.Factories
{
    public sealed class ConditionHandlerFactory<TContext> : IConditionHandlerFactory<TContext>
        where TContext : IConditionContext
    {
        #region Fields

        private readonly IConditionMetadataProvider _metadataProvider;
        private readonly IConditionInstanceFactory _instanceFactory;
        private readonly HashSet<IConditionDefinition> _constructionStack = new();

        #endregion

        #region Constructors

        public ConditionHandlerFactory(
            IConditionMetadataProvider metadataProvider,
            IConditionInstanceFactory instanceFactory)
        {
            _metadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
            _instanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        #endregion

        #region IConditionHandlerFactory<TContext>

        public IConditionHandler<TContext> Create(IConditionDefinition definition, TContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (definition == null)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Null condition definition received. " +
                    "This condition will be treated as 'always false'.");
                return new BrokenConditionHandler(context);
            }

            if (_constructionStack.Add(definition) == false)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Cyclic reference detected in condition definitions. " +
                    $"Definition '{definition}' will be treated as a broken condition (always false).");
                return new BrokenConditionHandler(context);
            }

            try
            {
                var handlerType = ResolveHandlerTypeSafe(definition);
                return handlerType == null
                    ? new BrokenConditionHandler(context)
                    : CreateHandlerInstanceSafe(handlerType, definition, context);
            }
            finally
            {
                _constructionStack.Remove(definition);
            }
        }

        #endregion

        #region Private Methods

        private Type ResolveHandlerTypeSafe(IConditionDefinition definition)
        {
            try
            {
                var definitionType = definition.GetType();
                var meta = _metadataProvider.GetMetadata(definitionType);
                var handlerType = meta.ImplementationType;

                if (handlerType.IsGenericTypeDefinition)
                    handlerType = handlerType.MakeGenericType(typeof(TContext));

                if (typeof(IConditionHandler<TContext>).IsAssignableFrom(handlerType))
                    return handlerType;

                Debug.LogError(
                    $"[{GetType().Name}] Handler type {handlerType.FullName} must implement " +
                    $"{typeof(IConditionHandler<TContext>).FullName}.");

                return null;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Failed to resolve handler type for definition '{definition}' " +
                    $"(type: {definition.GetType().Name}). This condition will be treated as 'always false'.\n{exception}");
                return null;
            }
        }

        private IConditionHandler<TContext> CreateHandlerInstanceSafe(
            Type handlerType,
            IConditionDefinition definition,
            TContext context)
        {
            try
            {
                return _instanceFactory.CreateHandler(handlerType, definition, context);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Failed to create handler for definition '{definition}' " +
                    $"(type: {definition.GetType().Name}). This condition will be treated as 'always false'.\n{exception}");
                return new BrokenConditionHandler(context);
            }
        }

        #endregion

        #region Nested Types

        private sealed class BrokenConditionHandler : ConditionHandler<TContext>
        {
            #region Constructors

            public BrokenConditionHandler(TContext context) : base(context)
            {
            }

            #endregion
        }

        #endregion
    }
}