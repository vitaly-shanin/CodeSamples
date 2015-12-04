using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export
{
	/// <summary>
	/// Хранит коллекцию сущностей, учавствующих в экспорте.
	/// </summary>
	public class ExportedEntities : IEnumerable<ExportedEntityWrapper>
	{
		public IEnumerator<ExportedEntityWrapper> GetEnumerator()
		{
			var seed = Enumerable.Empty<ExportedEntityWrapper>();
			var aggregation = _entities.Values.Aggregate(seed, (v1, v2) => v1.Union(v2));
			return aggregation.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(ExportedEntityWrapper entityWrapper)
		{
			var type = entityWrapper.EntityType;
			if (!_entities.ContainsKey(type))
			{
				_entities.Add(type, new List<ExportedEntityWrapper>());
			}

			_entities[type].Add(entityWrapper);

			if (entityWrapper.EntityBase != null)
			{
				_exportedEntitiesIds.Add(entityWrapper.EntityBase.Id);
			}
		}

		public IEnumerable<ExportedEntityWrapper> GetEntitiesOfType(Type type)
		{
			List<ExportedEntityWrapper> entities;
			_entities.TryGetValue(type, out entities);
			if (entities != null)
			{
				return entities;
			}

			return Enumerable.Empty<ExportedEntityWrapper>();
		}

		public IEnumerable<ExportedEntityWrapper<T>> GetEntitiesOfType<T>()
			where T : InventoryEntityBase
		{
			var entities = GetEntitiesOfType(typeof(T));
			return entities.Cast<ExportedEntityWrapper<T>>();
		}

		public bool EntityWithIdExported(Guid id)
		{
			return _exportedEntitiesIds.Contains(id);
		}

		/// <summary>
		/// Конвертирует экспортированные сущности в стандартный дата сет, используя
		/// экспортируемые имена сущностей <see cref="ExportedEntityWrapper.ExportedEntityName"/>
		/// в качестве имен таблиц, имена экспортированных атрибутов в качестве имен столбцов.
		/// </summary>
		public DataSet ConvertToDataSet()
		{
			var groupedEntities = this.GroupBy(ew => ew.ExportedEntityName);

			var dataSet = new DataSet("InventDataSet");

			foreach (var entry in groupedEntities)
			{
				var tableName = entry.Key;
				var table = new DataTable(tableName);
				FillDataTable(table, entry);
				dataSet.Tables.Add(table);
			}

			return dataSet;
		}

		private void FillDataTable(DataTable table, IEnumerable<ExportedEntityWrapper> entities)
		{
			foreach (var entity in entities)
			{
				var entityRow = table.NewRow();

				foreach (var attribute in entity.ExportedAttributes)
				{
					var columnName = attribute.Key;
					var value = attribute.Value;

					if (!table.Columns.Contains(columnName))
					{
						table.Columns.Add(columnName, value.GetType());
					}

					entityRow[columnName] = value;
				}

				table.Rows.Add(entityRow);
			}
		}

		readonly Dictionary<Type, List<ExportedEntityWrapper>> _entities
			= new Dictionary<Type, List<ExportedEntityWrapper>>();

		readonly HashSet<Guid> _exportedEntitiesIds = new HashSet<Guid>();
	}
}