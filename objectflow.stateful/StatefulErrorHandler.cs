﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Framework;

namespace Rainbow.ObjectFlow.Stateful
{
    /// <summary>
    /// Fault handler with modifications for stateful workflows. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatefulErrorHandler<T> : DefaultErrorHandler<T>
    {
        /// <summary>
        /// Handles UnallowedTransitionException fatally, making it possible to 
        /// easily break out of a workflow
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Interfaces.ErrorLevel Handle(Exception ex, T data)
        {
			if (base.Handle(ex, data) != Interfaces.ErrorLevel.Fatal)
			{
				if (ex is UnallowedTransitionException)
					return Interfaces.ErrorLevel.Fatal;
				else if (Strict)
					return Interfaces.ErrorLevel.Fatal;
				else
					return base.Handle(ex, data);
			}
			else return Interfaces.ErrorLevel.Fatal;
        }
    }
}
