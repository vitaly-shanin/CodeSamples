using System.ComponentModel;

namespace UpdateDataScheduleService.UpdateSchedules
{
	public enum UpdateScheduleMode
	{
		[Description("��������� � �������� �����.")]
		Day,
		[Description("����������� � �������� ���� ������ � �����.")]
		Week,
		[Description("���������� � ������ �������� ���� ������ � ������ � �������� �����.")]
		Month,
		[Description("�� ���������.")]
		None,
		[Description("�������� �����, �� ������ ���� ���.")]
		Now,
		[Description("��������� � �������� ����������.")]
		Interval
	}
}