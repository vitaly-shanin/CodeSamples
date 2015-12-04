using System;
using System.Linq;
using Inventory.Model;
using Inventory.Repositories;
using System.Linq.Expressions;
using System.Collections.Generic;
using Inventory.Services.DataExchange.Export.AttributeExporters;
using Inventory.Infrastructure.AttributeExpressions;

namespace Inventory.Services.DataExchange.Export
{
	public class EntityExporter<TEntity> : EntityExporter
		where TEntity : InventoryEntityBase
	{
		public Dictionary<IReferencedIdExporter<TEntity>, EntityExporter> Includes { get; set; }

		public EntityExporter(RepositoriesCollection repositories)
		{
			_repositories = repositories;
			Includes = new Dictionary<IReferencedIdExporter<TEntity>, EntityExporter>();
		}

		public Expression<Func<TEntity, bool>> Condition { get; set; }

		public override void ExportEntities(ExportedEntities exportedEntities, IEnumerable<Guid> ids = null)
		{
			var condition = Condition;

			if (ids != null)
			{
				condition = Condition.And(i => ids.Contains(i.Id));
			}

			var entities = 
				_repositories.GetRepository<TEntity>()
				.Find(condition)
				.Select(e => new ExportedEntityWrapper<TEntity>(e))
				.ToList();

			foreach (var entity in entities)
			{
				ExportSimpleAttributes(entity);
				exportedEntities.Add(entity);
			}

			ExportIncludes(exportedEntities);
		}

		private void ExportIncludes(ExportedEntities exportedEntities)
		{
			var entities = exportedEntities.GetEntitiesOfType<TEntity>().ToList();

			foreach (var include in Includes)
			{
				var includeIds = GetIncludeIds(exportedEntities, include.Key);

				if (includeIds.Any())
				{
					var exporter = include.Value;
					exporter.ExportEntities(exportedEntities, includeIds);
				}

				foreach (var entity in entities)
				{
					include.Key.Export(entity, exportedEntities);
				}
			}
		}

		private Guid[] GetIncludeIds(ExportedEntities exportedEntities, IReferencedIdExporter idExporter)
		{
			var ids = exportedEntities
				.GetEntitiesOfType<TEntity>()
				.Select(idExporter.GetValue)
				.Where(id => id.HasValue)
				.Select(id => id.Value)
				.Where(id => !exportedEntities.EntityWithIdExported(id))
				.Distinct()
				.ToArray();

			return ids;
		}

		private void ExportSimpleAttributes(ExportedEntityWrapper<TEntity> entity)
		{
			entity.ExportedEntityName = EntityName;

			if (ExportedIdGenerator != null)
			{
				ExportedIdGenerator.GenerateEntityId(entity);
			}

			foreach (var attributeExporter in AttributeExporters.OfType<IAttributeExporter<TEntity>>())
			{
				attributeExporter.Export(entity);
			}
		}

		readonly RepositoriesCollection _repositories;
	}
}