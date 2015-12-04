using Inventory.Model;

namespace Inventory.Services.DataExchange.Export.AttributeExporters
{
	public interface IAttributeExporter
	{ }

	public interface IAttributeExporter<TEntity> : IAttributeExporter
		where TEntity : InventoryEntityBase
	{
		void Export(ExportedEntityWrapper<TEntity> exportedEntity);
	}
}