﻿using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Instrumentation.Features.Instrumentation.Models;
using FubuMVC.Instrumentation.Runtime;

namespace FubuMVC.Instrumentation.Chains
{
    public class AverageChainVisualizerBuilder : IAverageChainVisualizerBuilder
    {
        private readonly BehaviorGraph _graph;
        private readonly IInstrumentationReportCache _cache;

        public AverageChainVisualizerBuilder(BehaviorGraph graph, IInstrumentationReportCache cache)
        {
            _graph = graph;
            _cache = cache;
        }

        public AverageChainModel VisualizerFor(Guid uniqueId)
        {
            var chain = _graph
                .Behaviors
                .SingleOrDefault(c => c.UniqueId == uniqueId);

            if (chain == null) return null;

            return new AverageChainModel
            {
                Chain = chain,
                BehaviorAverages = BuildBehaviorAverages(chain)
            };
        }

        private IEnumerable<AverageBehaviorModel> BuildBehaviorAverages(BehaviorChain chain)
        {
            var report = _cache.GetReport(chain.UniqueId);
            var averages = chain
                .Where(node =>
                    node.BehaviorType.AssemblyQualifiedName != null
                    && !node.BehaviorType.AssemblyQualifiedName.StartsWith("FubuMVC.Instrumentation")
                    && !node.BehaviorType.AssemblyQualifiedName.StartsWith("FubuMVC.Diagnostics"))
                .Select(node =>
                {
                    var behavior = new AverageBehaviorModel
                    {
                        Id = node.UniqueId,
                        DisplayType = node.GetType().PrettyPrint(),
                        BehaviorType = node.ToString()
                    };

                    if (report != null && report.Reports.Any())
                    {
                        report.Reports.Each(log =>
                        {
                            var startStep = log.FindStep<BehaviorStart>(s => s.Correlation.Node.UniqueId == node.UniqueId);
                            var endStep = log.FindStep<BehaviorFinish>(s => s.Correlation.Node.UniqueId == node.UniqueId);
                            if (startStep != null && endStep != null)
                            {
                                var span = endStep.Log.Time - startStep.Log.Time;
                                behavior.HitCount++;
                                behavior.TotalExecutionTime += span.TotalMilliseconds;
                            }

                        });
                    }

                    return behavior;
                });

            return averages;
        }
    }
}