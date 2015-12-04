using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class NoUpdateSchedule
		: UpdateSchedule
	{
		public override DateTime? GetNextUpdateTime(DateTime lastUpdateTime)
		{
			return null;
		}

		public override bool IsUpdateTime(DateTime lastUpdate)
		{
			return false;
		}

		public override UpdateScheduleMode ScheduleMode
		{
			get { return UpdateScheduleMode.None; }
		}
	}

}
