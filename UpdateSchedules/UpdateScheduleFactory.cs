using System;
using UpdateDataScheduleService.Properties;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public class UpdateScheduleFactory
	{
		public UpdateSchedule GetImportScheduleFromProperties()
		{
			var updateModeString = Settings.Default.ImportMode;
			UpdateScheduleMode updateMode;
			if (!Enum.TryParse(updateModeString, out updateMode))
			{
				Logger.AppendInfo(string.Format(Resources.InvalidExportUpdateSchedule, updateModeString));
				return new NoUpdateSchedule();
			}

			var importTime = Settings.Default.ImportTime;
			var importDay = Settings.Default.ImportDay;
			return GetSchedule(updateMode, importTime, importDay);
		}

		public UpdateSchedule GetExportScheduleFromProperties()
		{
			var updateModeString = Settings.Default.ExportMode;
			UpdateScheduleMode updateMode;
			if (!Enum.TryParse(updateModeString, out updateMode))
			{
				Logger.AppendInfo(string.Format(Resources.InvalidExportUpdateSchedule, updateModeString));
				return new NoUpdateSchedule();
			}

			var exportTime = Settings.Default.ExportTime;
			var exportDay = Settings.Default.ExportDay;
			return GetSchedule(updateMode, exportTime, exportDay);
		}

		private UpdateSchedule GetSchedule(UpdateScheduleMode updateMode, TimeSpan updateTime,
			DayOfWeek updateDay)
		{
			switch (updateMode)
			{
				case UpdateScheduleMode.Now:
					return new UpdateNowSchedule();
				case UpdateScheduleMode.Day:
					return new DayUpdateSchedule(updateTime);
				case UpdateScheduleMode.Week:
					return new WeekUpdateSchedule(updateTime, updateDay);
				case UpdateScheduleMode.Month:
					return new MonthUpdateSchedule(updateTime, updateDay);
				case UpdateScheduleMode.Interval:
					return new IntervalUpdateSchedule(updateTime);
				default:
					return new NoUpdateSchedule();
			}
		}
	}

}
