namespace Inventory.Services.DataExchange.Export
{
	public abstract class ExportIdGenerator
	{
		protected ExportIdGenerator(string exportedIdName)
		{
			_exportedIdName = exportedIdName;
		}

		public void GenerateEntityId(ExportedEntityWrapper entity)
		{
			var id = GetEntityId(entity);
			entity.ExportedId = id;
			entity.ExportedAttributes[_exportedIdName] = id;
		}

		protected abstract string GetEntityId(ExportedEntityWrapper entity);

		readonly string _exportedIdName;
	}
}