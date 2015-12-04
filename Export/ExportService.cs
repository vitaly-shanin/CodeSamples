using System.Collections.Generic;
using System.Linq;

namespace Inventory.Services.DataExchange.Export
{
	public class ExportService
	{
		public ExportedEntities Export(
			IEnumerable<EntityExporter> exporters,
			IEnumerable<IExportCustomizationModule> customizationModules)
		{
			var entities = new ExportedEntities();
			exporters = exporters.ToArray();

			foreach (var exporter in exporters)
			{
				exporter.ExportEntities(entities);
			}

			CustomizeExportedEntities(exporters, customizationModules, entities);

			return entities;
		}

		private void CustomizeExportedEntities(
			IEnumerable<EntityExporter> exporters,
			IEnumerable<IExportCustomizationModule> customizationModules,
			ExportedEntities entities)
		{
			foreach (var customizationModule in customizationModules)
			{
				customizationModule.Customize(entities);
			}

			foreach (var exporter in exporters)
			{
				exporter.ProcessCustomizationResults(entities);
			}
		}
	}
}