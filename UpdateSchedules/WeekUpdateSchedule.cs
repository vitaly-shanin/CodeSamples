using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class WeekUpdateSchedule
		: UpdateSchedule
	{
		public WeekUpdateSchedule(TimeSpan updateTime, DayOfWeek updateDay)
			: base(updateTime, updateDay)
		{
			if (updateTime.Days > 0)
			{
				throw new ArgumentOutOfRangeException("updateTime");
			}
		}

		public override UpdateScheduleMode ScheduleMode
		{
			get { return UpdateScheduleMode.Week; }
		}

		public override DateTime? GetNextUpdateTime(DateTime lastUpdateTime)
		{
			var mondayOfNextWeek = GetFirstDayInNextWeek(lastUpdateTime.Date);
			var updateDate = GetNearestUpdateDayDate(mondayOfNextWeek);
			return updateDate.Add(UpdateTime);
		}

		private DateTime GetFirstDayInNextWeek(DateTime date)
		{
			DateTime currentDay = date.Date;

			do
			{
				currentDay = currentDay.AddDays(1);
			}
			while (currentDay.DayOfWeek != DayOfWeek.Monday);

			return currentDay;
		}
	}
}
