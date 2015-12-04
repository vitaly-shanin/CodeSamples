using System.ComponentModel;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public enum UpdateScheduleMode
	{
		[Description("≈жедневно в заданное врем€.")]
		Day,
		[Description("≈женедельно в заданные день недели и врем€.")]
		Week,
		[Description("≈жемес€чно в первый заданный день недели в мес€це в заданное врем€.")]
		Month,
		[Description("Ќе обновл€ть.")]
		None,
		[Description("ќбновить сразу, но только один раз.")]
		Now,
		[Description("ќбновл€ть с заданным интервалом.")]
		Interval
	}
}