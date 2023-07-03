using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface IJobMilestone
    {
        List<JobMilestoneModel> GetJobsMilestones();
        List<JobMilestoneModel> GetJobMilestones(string jobId);
        List<JobMilestoneModel> GetJobsMilestonesAfterDate(DateTime date);
        string CreateJobMilestone(JobMilestoneModel jm);
        string EditJobMilestone(JobMilestoneModel jm);
        string DeleteJobMilestone(JobMilestoneModel jm);
        string DeleteAllJobMilestones(JobMilestoneModel jm);
    }
}
