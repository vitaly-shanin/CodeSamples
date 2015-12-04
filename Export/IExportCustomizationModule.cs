namespace Inventory.Services.DataExchange.Export
{
	public interface IExportCustomizationModule
	{
		void Customize(ExportedEntities entities);
	}
}