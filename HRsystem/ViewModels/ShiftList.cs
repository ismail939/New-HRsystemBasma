namespace HRsystem.ViewModels;

public class ShiftList
{
    public List<ShiftItem> Shifts { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}