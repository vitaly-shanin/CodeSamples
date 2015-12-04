using System;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public abstract class UpdateSchedule
	{
		protected UpdateSchedule()
			: this (TimeSpan.Zero, null)
		{ }

		protected UpdateSchedule(TimeSpan time)
			: this(time, null)
		{ }

		protected UpdateSchedule(TimeSpan time, DayOfWeek? day)
		{
			UpdateTime = time;
			UpdateDay = day;
		}

		/// <param name="lastUpdate"></param>
		/// <param name="now">Для юнит-тестирования.</param>
		public bool IsUpdateTime(DateTime lastUpdate, DateTime now)
		{
			var nextUpdateTime = GetNextUpdateTime(lastUpdate);
			if (now >= nextUpdateTime)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Узнать, наступило ли время обмена данными.
		/// </summary>
		/// <param name="lastUpdate">Время последнего предыдущего успешного обмена данными.</param>
		public virtual bool IsUpdateTime(DateTime lastUpdate)
		{
			return IsUpdateTime(lastUpdate, DateTime.Now);
		}

		/// <summary>
		/// Узнать, когда наступит следующее время обмена данными.
		/// Возвращает null, если для данного объекта невозможно
		/// предварительно узнать время следующего обмена.
		/// </summary>
		/// <remarks>Не зависит от текущего времени - возвращает только ожидаемое время следующего обмена
		/// как функцию от времени последнего обмена.</remarks>
		/// <param name="lastUpdateTime">Время последнего предыдущего успешного обмена данными.</param>
		public abstract DateTime? GetNextUpdateTime(DateTime lastUpdateTime);

		/// <summary>
		/// Возвращает ближайщий к date следующий день,
		/// день недели которого совпадает с UpdateDay.
		/// </summary>
		protected DateTime GetNearestUpdateDayDate(DateTime date)
		{
			if (!UpdateDay.HasValue)
			{
				throw new InvalidOperationException();
			}

			var result = date.Date;
			while (result.DayOfWeek != UpdateDay)
			{
				result = result.AddDays(1);
			}

			return result;
		}

		public abstract UpdateScheduleMode ScheduleMode
		{
			get;
		}

		public TimeSpan UpdateTime
		{
			get;
			private set;
		}

		public DayOfWeek? UpdateDay
		{
			get;
			private set;
		}
	}
}
