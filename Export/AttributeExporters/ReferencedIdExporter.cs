using System;
using System.Linq;
using Inventory.Infrastructure.AttributeExpressions;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export.AttributeExporters
{
	public class ReferencedIdExporter<TEntity, TReferencedEntity> : IReferencedIdExporter<TEntity>
		where TEntity : InventoryEntityBase
		where TReferencedEntity : InventoryEntityBase
	{
		public ReferencedIdExporter(string exportedName, AttributeExpression<TEntity, Guid?> reference)
		{
			_exportedName = exportedName;
			_reference = reference;
		}

		public Guid? GetValue(ExportedEntityWrapper exportedEntity)
		{
			var referencedId = _reference.GetValue(exportedEntity.EntityBase as TEntity);
			return referencedId;
		}

		public void Export(ExportedEntityWrapper<TEntity> entityWrapper, ExportedEntities entities)
		{
			var referencedId = GetValue(entityWrapper);

			if (referencedId.HasValue)
			{
				var corespondingExportedEntity = entities
					.GetEntitiesOfType<TReferencedEntity>()
					.FirstOrDefault(e => e.Entity.Id == referencedId);

				if (corespondingExportedEntity != null)
				{
					var exportedId = corespondingExportedEntity.ExportedId;
					entityWrapper.ExportedAttributes[_exportedName] = exportedId;
				}
			}
		}

		readonly string _exportedName;
		readonly AttributeExpression<TEntity, Guid?> _reference;
	}
}