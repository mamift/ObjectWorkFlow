﻿using System;
using NUnit.Framework;
using Rainbow.ObjectFlow.Language;
using Rainbow.ObjectFlow.Policies;

namespace Objectflow.core.tests.Policy
{
    public class WhenUsingIntervalPolicy: Specification
    {
        private IInterval _interval;
        private Rainbow.ObjectFlow.Policies.Policy _policy;

        [Scenario]
        public void Given()
        {
            _interval = new Interval(null);
            _policy = _interval as Rainbow.ObjectFlow.Policies.Policy;
            Assert.IsNotNull(_policy);
        }

        [Observation]
        public void ShouldWaitForSpecifiedTime()
        {
            _interval.Of.Seconds(1);
            
            string before = DateTime.Now.ToLongTimeString();

            _policy.Execute<string>("Red");
            
            Assert.That(DateTime.Now.Subtract(new TimeSpan(0, 0, 1)).ToLongTimeString(), Is.EqualTo(before), "TimeSpan incorrect");
        }
    }
}
