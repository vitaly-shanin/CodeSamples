using System;
using System.Collections.Generic;
using Inventory.Services.DataExchange.Export.AttributeExporters;

namespace Inventory.Services.DataExchange.Export
{
	public abstract class EntityExporter
	{
		protected EntityExporter()
		{
			AttributeExporters = new List<IAttributeExporter>();
			CustomDataExporters = new List<CustomDataExporter>();
		}

		public string EntityName { get; set; }

		public List<IAttributeExporter> AttributeExporters { get; set; }

		public List<CustomDataExporter> CustomDataExporters { get; set; }

		public ExportIdGenerator ExportedIdGenerator { get; set; }

		public abstract void ExportEntities(ExportedEntities exportedEntities, IEnumerable<Guid> ids = null);

		public void ProcessCustomizationResults(ExportedEntities entities)
		{
			foreach (var entity in entities)
			{
				foreach (var customDataExporter in CustomDataExporters)
				{
					customDataExporter.ExportAttribute(entity);
				}
			}
		}
	}
}