using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class IntervalUpdateSchedule
		: UpdateSchedule
	{
		public IntervalUpdateSchedule(TimeSpan updateInterval)
			: base(updateInterval)
		{ }


		public override DateTime? GetNextUpdateTime(DateTime lastUpdateTime)
		{
			return lastUpdateTime.Add(UpdateTime);
		}

		public override UpdateScheduleMode ScheduleMode
		{
			get { return UpdateScheduleMode.Interval;  }
		}
	}
}
