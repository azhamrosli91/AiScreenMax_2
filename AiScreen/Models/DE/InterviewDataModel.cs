﻿namespace MaxSystemWebSite.Models.DE
{
    public class InterviewDataModel
    {
        public CandidateModel Candidate { get; set; }
        public string Datetime { get; set; }
        public string Location { get; set; }
        public List<InterviewerModel> Interviewers { get; set; }
    }

    public class CandidateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class InterviewerModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
