﻿using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Agent.Worker.CodeCoverage
{
    internal class JaCoCoSummaryReader : AgentService, ICodeCoverageSummaryReader
    {
        public Type ExtensionType => typeof(ICodeCoverageSummaryReader);
        public string Name => "JaCoCo";

        public IEnumerable<CodeCoverageStatistics> GetCodeCoverageSummary(IExecutionContext context, string summaryXmlLocation)
        {
            var doc = CodeCoverageUtilities.ReadSummaryFile(context, summaryXmlLocation);
            return ReadDataFromNodes(doc, summaryXmlLocation);
        }

        private IEnumerable<CodeCoverageStatistics> ReadDataFromNodes(XmlDocument doc, string summaryXmlLocation)
        {
            var listCoverageStats = new List<CodeCoverageStatistics>();

            if (doc == null)
            {
                return null;
            }

            //read data from report nodes
            XmlNode reportNode = doc.SelectSingleNode("report");
            if (reportNode != null)
            {
                XmlNodeList counterNodeList = doc.SelectNodes("/report/counter");
                if (counterNodeList != null)
                {

                    foreach (XmlNode counterNode in counterNodeList)
                    {
                        var coverageStatistics = new CodeCoverageStatistics();

                        if (counterNode.Attributes != null)
                        {
                            if (counterNode.Attributes["type"] != null)
                            {
                                coverageStatistics.Label = counterNode.Attributes["type"].Value;
                                coverageStatistics.Position = CodeCoverageUtilities.GetPriorityOrder(coverageStatistics.Label);
                            }

                            if (counterNode.Attributes[c_covered] != null)
                            {
                                float covered;
                                if (!float.TryParse(counterNode.Attributes[c_covered].Value, out covered))
                                {
                                    throw new InvalidDataException(StringUtil.Loc("InvalidValueInXml", c_covered, summaryXmlLocation));
                                }
                                coverageStatistics.Covered = (int)covered;
                                if (counterNode.Attributes[c_missed] != null)
                                {
                                    float missed;
                                    if (!float.TryParse(counterNode.Attributes[c_missed].Value, out missed))
                                    {
                                        throw new InvalidDataException(StringUtil.Loc("InvalidValueInXml", c_missed, summaryXmlLocation));
                                    }
                                    coverageStatistics.Total = (int)missed + (int)covered;
                                }
                            }
                        }

                        listCoverageStats.Add(coverageStatistics);
                    }
                }
            }
            return listCoverageStats.AsEnumerable();
        }



        private const string c_covered = "covered";
        private const string c_missed = "missed";
    }
}