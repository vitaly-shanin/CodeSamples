using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class UpdateNowSchedule
		: UpdateSchedule
	{
		public override bool IsUpdateTime(DateTime lastUpdate)
		{
			if (!_updated)
			{
				_updated = true;
				return true;
			}

			return false;
		}

		public override DateTime? GetNextUpdateTime(DateTime lastUpdateTime)
		{
			return null;
		}

		public override UpdateScheduleMode ScheduleMode
		{
			get { return UpdateScheduleMode.Now; }
		}

		bool _updated;
	}
}
