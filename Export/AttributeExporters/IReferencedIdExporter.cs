using System;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export.AttributeExporters
{
	public interface IReferencedIdExporter
	{
		Guid? GetValue(ExportedEntityWrapper entityWrapper);
	}

	public interface IReferencedIdExporter<TEntity> : IReferencedIdExporter
		where TEntity : InventoryEntityBase
	{
		void Export(ExportedEntityWrapper<TEntity> entityWrapper, ExportedEntities entities);
	}
}