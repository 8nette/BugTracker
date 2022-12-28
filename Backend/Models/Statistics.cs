using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Statistics
    {
        public OverallCompletion completion { get; set; }

        public OverallPriority priority { get; set; }

        public IEnumerable<CurrentOpen> currentOpens { get; set; }

        public IEnumerable<OpenAge> openAges { get; set; }

        public IEnumerable<AverageDefectAge> averageDefects { get; set; }

        public IEnumerable<TaskOpenBugs> taskOpenBugs { get; set; }

        public IEnumerable<SubmittedOpenClosed> submittedOpenCloseds { get; set; }

        public IEnumerable<DeveloperProductivity> developerProductivities { get; set; }

        public IEnumerable<AreaBugs> areaBugs { get; set; }
    }

    public class CurrentOpen
    {
        public int numberOfOpenBugs { get; set; }

        public DateTime day { get; set; }
    }

    public class OpenAge
    {
        public int numberOfNotClosedBugs { get; set; }

        public int ageInWeeks { get; set; }
    }

    public class TaskOpenBugs
    {
        public int numberOfOpenBugs { get; set; }

        public int week { get; set; }

        public int year { get; set; }
    }

    public class OverallCompletion
    {
        public int numberOfOpenBugs { get; set; }

        public int numberOfNotReproduceBugs { get; set; }

        public int numberOfNotFixBugs { get; set; }

        public int numberOfTestBugs { get; set; }

        public int numberOfResolvedBugs { get; set; }
    }

    public class OverallPriority
    {
        public int numberOfCriticalBugs { get; set; }

        public int numberOfHighBugs { get; set; }

        public int numberOfNormalBugs { get; set; }

        public int numberOfLowBugs { get; set; }
    }

    public class SubmittedOpenClosed
    {
        public int numberOfOpenBugs { get; set; }

        public int numberOfClosedBugs { get; set; }

        public int numberOfSubmittedBugs { get; set; }

        public int week { get; set; }

        public int year { get; set; }
    }

    public class DeveloperProductivity
    {
        public string developerUsername { get; set; }

        public int numberOfOpenBugs { get; set; }

        public int numberOfNotFixBugs { get; set; }

        public int numberOfNotReproduceBugs { get; set; }

        public int numberOfResolvedBugs { get; set; }

        public int numberOfTestBugs { get; set; }
    }

    public class AreaBugs
    {
        public string area { get; set; }

        public int generatedBugs { get; set; }

        public int openBugs { get; set; }
    }

    public class AverageDefectAge
    {
        public int averageDefectAge { get; set; }

        public int week { get; set; }

        public int year { get; set; }
    }
}
