using System;

namespace Inventory.Services.DataExchange.Export.AttributeExporters
{
	public class CustomDataExporter
	{
		public CustomDataExporter(string customDataName, string exportName, Func<object, object> converter)
		{
			if (string.IsNullOrEmpty(customDataName))
			{
				throw new ArgumentException("customDataName");
			}

			if (string.IsNullOrEmpty(exportName))
			{
				throw new ArgumentException("exportName");
			}

			_customDataName = customDataName;
			_exportName = exportName;
			_converter = converter;
		}

		public void ExportAttribute(ExportedEntityWrapper wrapper)
		{
			object value;
			wrapper.CustomData.TryGetValue(_customDataName, out value);

			if (_converter != null)
			{
				value = _converter(value);
			}

			if (value != null)
			{
				wrapper.ExportedAttributes[_exportName] = value;
			}
		}

		readonly Func<object, object> _converter;
		readonly string _customDataName;
		readonly string _exportName;
	}
}