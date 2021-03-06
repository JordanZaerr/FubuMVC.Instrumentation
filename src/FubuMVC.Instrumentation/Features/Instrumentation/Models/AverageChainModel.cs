﻿using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Instrumentation.Features.Instrumentation.Models
{
    public class AverageChainModel
    {
        public AverageChainModel()
        {
            BehaviorAverages = Enumerable.Empty<AverageBehaviorModel>();
        }

        public string Constraints { get; set; }
        public BehaviorChain Chain { get; set; }
        public IEnumerable<AverageBehaviorModel> BehaviorAverages { get; set; }
    }
}