﻿using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Instrumentation.Sample.Controllers;

namespace FubuMVC.Instrumentation.Sample.Behaviors
{
    public class HelloTextBehavior: BasicBehavior
    {
        private readonly IFubuRequest _request;

        public HelloTextBehavior(IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var model = _request.Get<OtherInputModel>();

            model.HelloText = string.Format("You said: {0}", model.HelloText);

            return DoNext.Continue;
        }
    }
}