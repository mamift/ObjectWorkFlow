﻿using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Engine;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace Objectflow.core.tests.CompositeWorkflows
{
    public class WhenDefiningCompositeWorkflows:Specification
    {
        private Workflow<string> _childWorkflow;
        private Workflow<string> _parentWorkflow;
        private TaskList<string> _taskList;

        [Scenario]
        public void Given()
        {
			_taskList = new TaskList<string>();
			_childWorkflow = new Workflow<string>().Do((c) => "red") as Workflow<string>;
			_parentWorkflow = new Workflow<string>(_taskList);
        }

        [Observation]
        public void ShouldCreateTaskListForEachWorkflow()
        {
            Assert.IsFalse(ReferenceEquals(_parentWorkflow.RegisteredOperations, _childWorkflow.RegisteredOperations), "Reference");
        }

        [Observation]
        public void ShouldNotAllowNullWorkflows()
        {
            IWorkflow<string> workflow = null;

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow));
        }

        [Observation]
        public void ShouldNotAllowNullConstraints()
        {
            IWorkflow<string> workflow = Workflow<string>.Definition() as IWorkflow<string>;
            ICheckConstraint constraint = null;

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow, constraint));
        }

        [Observation]
        public void ShouldNotAllowNullworkflowsWithConstraints()
        {
            IWorkflow<string> workflow = null;
            ICheckConstraint constraint = If.IsTrue(true);

            Assert.Throws<ArgumentNullException>(() => Workflow<string>.Definition().Do(workflow, constraint));
        }

        [Observation]
        public void ShouldAddToWorkflowDefinition()
        {
            _parentWorkflow.Do(_childWorkflow);

            Assert.That(_parentWorkflow.RegisteredOperations.Tasks.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Observation]
        public void ShouldAddWorkflowWithconstraints()
        {
            var innerWorkflow = Workflow<string>.Definition().Do(c => "red");
            var workflow = Workflow<string>.Definition().Do(innerWorkflow, If.IsTrue(true)) as Workflow<string>;

            workflow.ShouldNotbeNull();
            workflow.RegisteredOperations.Tasks.Count.ShouldBe(1);
        }

        [Observation]
        public void AddWorkflowWithConstraint()
        {
            var childWorkflow = Workflow<string>.Definition() as IWorkflow<string>;
            _parentWorkflow = Workflow<string>.Definition().Do(childWorkflow, If.IsTrue(true)) as Workflow<string>;

            _parentWorkflow.RegisteredOperations.Tasks.Count.ShouldBe(1);
        }

        [Observation]
        public void ShouldAddWhenDefiningParallelCompositeWorkflow()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition().Do(c => "red");

            var builder = new ParallelSplitBuilder<string>(_taskList);
            builder.AddOperation(innerWorkflow);

            Assert.That(builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }

        [Observation]
        public void ShouldAddWhenDefiningParallelCompositeWorkflowWithConstraint()
        {
            Workflow<string> workflow = Workflow<string>.Definition() as Workflow<string>;
            var innerWorkflow = Workflow<string>.Definition().Do(c => "red");

            var builder = new ParallelSplitBuilder<string>(_taskList);
            builder.AddOperation(innerWorkflow, If.IsTrue(true));

            Assert.That(builder.ParallelOperations.RegisteredOperations.Count, Is.EqualTo(1), "number of operations in workflow");
        }
    }
}