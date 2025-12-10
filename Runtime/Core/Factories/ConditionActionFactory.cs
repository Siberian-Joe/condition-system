using System;
using ConditionSystem.Runtime.Core.Abstractions;
using UnityEngine;

namespace ConditionSystem.Runtime.Core.Factories
{
    public sealed class ConditionActionFactory<TContext> : IConditionActionFactory<TContext>
        where TContext : IConditionContext
    {
        #region Fields

        private readonly IConditionMetadataProvider _metadataProvider;
        private readonly IConditionInstanceFactory _instanceFactory;

        #endregion

        #region Constructors

        public ConditionActionFactory(
            IConditionMetadataProvider metadataProvider,
            IConditionInstanceFactory instanceFactory)
        {
            _metadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
            _instanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        #endregion

        #region IConditionActionFactory<TContext>

        public IConditionAction Create(IConditionActionDefinition definition, TContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (definition == null)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Null action definition received. This action will be ignored.");
                return new BrokenConditionAction();
            }

            var actionType = ResolveActionTypeSafe(definition);
            return actionType == null
                ? new BrokenConditionAction()
                : CreateActionInstanceSafe(actionType, definition, context);
        }

        #endregion

        #region Private Methods

        private Type ResolveActionTypeSafe(IConditionActionDefinition definition)
        {
            try
            {
                var definitionType = definition.GetType();
                var meta = _metadataProvider.GetMetadata(definitionType);
                var actionType = meta.ImplementationType;

                if (actionType.IsGenericTypeDefinition)
                    actionType = actionType.MakeGenericType(typeof(TContext));

                if (typeof(IConditionAction).IsAssignableFrom(actionType))
                    return actionType;

                Debug.LogError(
                    $"[{GetType().Name}] Action type {actionType.FullName} must implement {nameof(IConditionAction)}. " +
                    "This action will be ignored.");

                return null;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Failed to resolve action type for definition '{definition}' " +
                    $"(type: {definition.GetType().Name}). This action will be ignored.\n{exception}");
                return null;
            }
        }

        private IConditionAction CreateActionInstanceSafe(
            Type actionType,
            IConditionActionDefinition definition,
            TContext context)
        {
            try
            {
                return _instanceFactory.CreateAction(actionType, definition, context);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[{GetType().Name}] Failed to instantiate action {actionType.FullName}. " +
                    $"This action will be ignored.\n{exception}");
                return new BrokenConditionAction();
            }
        }

        #endregion

        #region Nested Types

        private sealed class BrokenConditionAction : IConditionAction
        {
            #region IConditionAction

            public void Execute()
            {
            }

            #endregion
        }

        #endregion
    }
}