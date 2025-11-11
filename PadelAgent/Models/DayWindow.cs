namespace PadelAgent.Models;

public sealed record DayWindow(DateOnly Date, TimeOnly From, TimeOnly To);