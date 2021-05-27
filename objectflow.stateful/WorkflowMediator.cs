﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Stateful;

namespace Rainbow.ObjectFlow.Stateful
{
	/// <summary>
	/// Describes a workflow process or sub-process that an object can go through. Actual
	/// processes will descend from this class.
	/// </summary>
	public abstract class WorkflowMediator<T> : Rainbow.ObjectFlow.Stateful.IWorkflowMediator<T>
		where T : class, IStatefulObject
	{
		/// <summary>
		/// A reference to the workflow. Call <see cref="InitializeWorkflowIfNecessary"/> if this is null
		/// </summary>
		protected IStatefulWorkflow<T> _workflow;

		/// <summary>
		/// This is where you define your workflow
		/// </summary>
		/// <remarks>We'll pass this a DP to record the steps that must happen and proxy
		/// the steps to an actual WorkFlow object. This way we have a full ordering built
		/// up in memory so we can easily jump steps and start part-way through."/></remarks>
		protected abstract IStatefulWorkflow<T> Define();

		/// <summary>
		/// Pre-process hook so that you can put global checks on all entry points for this 
		/// workflow. If validation fails, this will silently exit the workflow
		/// </summary>
		/// <param name="object"></param>
		/// <returns><c>false</c> if validation fails</returns>
		protected virtual bool Validate(T @object)
		{
			return true;
		}

		/// <summary>
		/// For resuming a workflow that has already begun or starting a workflow on a new
		/// object.
		/// </summary>
		/// <param name="initializer"></param>
		/// <returns></returns>
		public virtual T Start(T initializer)
		{
			return Start(initializer, null);
		}

		/// <summary>
		/// For resuming a workflow that has already begun or starting a workflow on a new
		/// object.
		/// </summary>
		/// <param name="initializer"></param>
		/// <param name="parameters">Optional parameters for this workflow segment</param>
		/// <returns></returns>
		public virtual T Start(T initializer, IDictionary<string, object> parameters)
		{
			InitializeWorkflowIfNecessary();
			var beginning = initializer.GetStateId(_workflow.WorkflowId);

			T ret = initializer;
			if (Validate(initializer))
			{
				if (parameters != null && parameters.Count == 0)
					ret = _workflow.Start(initializer);
				else
					ret = _workflow.StartWithParams(initializer, parameters);
			}

			object ending = (ret != null)? ret.GetStateId(_workflow.WorkflowId) : null;
			OnFinished(ret, beginning, ending);

			return ret;
		}

		/// <summary>
		/// For resuming a workflow that has already begun or starting a workflow on a new
		/// object.
		/// </summary>
		/// <param name="initializer"></param>
		/// <param name="parameters">Optional parameters for this workflow segment</param>
		/// <returns></returns>
		public T Start(T initializer, object parameters)
		{
			return Start(initializer, parameters.ToDictionary());
		}

		/// <summary>
		/// Called when a segment finishes executing. This is where you should implement
		/// persistence.
		/// </summary>
		/// <param name="subject">The object exiting the workflow segment</param>
		/// <param name="from">beginning workflow state</param>
		/// <param name="to">ending workflow state</param>
		protected virtual void OnFinished(T subject, object from, object to)
		{
		}

		/// <summary>
		/// Lazy initializes the workflow. Always call this before you need to reference <see cref="_workflow"/>
		/// </summary>
		protected virtual void InitializeWorkflowIfNecessary()
		{
			if (_workflow == null)
				_workflow = Define();
		}

		/// <summary>
		/// Calculate whether or not the current role can make the transition from one state
		/// to the next. This looks up in the transition table
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public virtual bool CanDoTransition(object from, object to)
		{
			return true;
		}

		/// <summary>
		/// Allows other applications to query the workflow for transitions that are allowed
		/// and won't be denied. This makes it possible to consolidate all workflow logic
		/// and keep UI separate. 
		/// </summary>
		/// <param name="object">The stateful object that wants to know what it can do.</param>
		/// <returns></returns>
		public virtual IEnumerable<ITransition> GetPossibleTransitions(T @object)
		{
			InitializeWorkflowIfNecessary();
			var empty = new ITransition[0];
			if (@object == null)
				return empty;
			else
				return GetPossibleTransitionsForState(@object.GetStateId(_workflow.WorkflowId));
		}

		private IEnumerable<ITransition> GetPossibleTransitionsForState(object fromState)
		{
			InitializeWorkflowIfNecessary();
			var empty = new ITransition[0];
			var enumerable = _workflow.PossibleTransitions;
			if (enumerable == null)
				return empty;
			else return enumerable.Where(x =>
				object.Equals(x.From, fromState));
		}

		#region IStateObserver<T> Members

		/// <summary>
		/// If <see cref="IsInWorkflow"/> is false, this indicates that the entity has been
		/// in a workflow at least once. This returns null if this information can't be determined.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool? HasBeenInWorkflow(T entity)
		{
			return _workflow.HasBeenInWorkflow(entity);
		}

		/// <summary>
		/// True if the entity is currently passing through the workflow
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool IsInWorkflow(T entity)
		{
			return _workflow.IsInWorkflow(entity);
		}

		#endregion
	}
}
