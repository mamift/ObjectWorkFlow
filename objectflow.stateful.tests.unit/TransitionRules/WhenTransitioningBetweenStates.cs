﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.ObjectFlow.Stateful.tests.TransitionRules
{
	[TestFixture]
	class WhenTransitioningBetweenStates
	{
		private DefaultTransitionRule<TestObject> sut;
		private TestObject obj;

		[SetUp]
		public void Given()
		{
			sut = new DefaultTransitionRule<TestObject>("workflow");
			obj = new TestObject() { State = "begin" };
		}

		[Observation]
		public void StartsInWorkflow()
		{
			Assert.True(sut.IsInWorkflow(obj));
		}

		[Observation]
		public void CanTransition()
		{
			sut.Transition(obj, "end");
			Assert.True(sut.IsInWorkflow(obj));
			Assert.That(sut.HasBeenInWorkflow(obj) == true);
		}
	}
}
