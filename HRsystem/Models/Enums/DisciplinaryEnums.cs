namespace HRsystem.Models.Enums
{
    public enum PenaltyStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Approved = 2,
        Rejected = 3
    }

    public enum PenaltyCategory
    {
        Attendance = 0,
        Behavior = 1,
        Safety = 2,
        Performance = 3,
        Other = 4
    }

    public enum DeductionUnit
    {
        Money = 0,
        Hour = 1,
        Day = 2,
        Percentage = 3,
        WarningOnly = 4
    }

    public enum PenaltyValueType
    {
        Minutes = 0,
        Occurrences = 1,
        Days = 2
    }
}