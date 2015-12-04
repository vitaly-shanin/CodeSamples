using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class MonthUpdateSchedule
		: UpdateSchedule
	{
		public MonthUpdateSchedule(TimeSpan updateTime, DayOfWeek updateDay)
			: base(updateTime, updateDay)
		{
			if (updateTime.Days > 0)
			{
				throw new ArgumentOutOfRangeException("updateTime");
			}
		}

		public override DateTime? GetNextUpdateTime(DateTime lastUpdateTime)
		{
			var nextUpdateDate = GetNearestUpdateDayDate(GetFirstDayInNextMonth(lastUpdateTime));
			return nextUpdateDate.Add(UpdateTime);
		}

		public override UpdateScheduleMode ScheduleMode
		{
			get { return UpdateScheduleMode.Month; }
		}

		private DateTime GetFirstDayInNextMonth(DateTime lastUpdate)
		{
			return new DateTime(lastUpdate.Year, lastUpdate.Month + 1, 1);
		}
	}
}
