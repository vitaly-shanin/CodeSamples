using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class DayUpdateSchedule
		: UpdateSchedule
	{
		public DayUpdateSchedule(TimeSpan updateStartTime)
			: base(updateStartTime)
		{
			if (updateStartTime.Days > 0)
			{
				throw new ArgumentOutOfRangeException("updateStartTime");
			}
		}

		public override DateTime? GetNextUpdateTime(DateTime lastUpdateTime)
		{
			var updateDate = lastUpdateTime.Date.AddDays(1);
			return updateDate.Add(UpdateTime);
		}

		public override UpdateScheduleMode ScheduleMode
		{
			get { return UpdateScheduleMode.Day; }
		}
	}
}
