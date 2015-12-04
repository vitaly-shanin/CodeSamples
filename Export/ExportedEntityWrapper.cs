using System;
using System.Collections.Generic;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export
{
	public abstract class ExportedEntityWrapper
	{
		public string ExportedEntityName { get; set; }

		public string ExportedId { get; set; }

		public Dictionary<string, object> ExportedAttributes
		{
			get
			{
				return _exportedAttributes ?? (_exportedAttributes = new Dictionary<string, object>());
			}
		}

		public Dictionary<string, object> CustomData
		{
			get
			{
				return _customData ?? (_customData = new Dictionary<string, object>());
			}
		}

		public abstract InventoryEntityBase EntityBase { get; set; }

		public abstract Type EntityType { get; }

		private Dictionary<string, object> _exportedAttributes;
		private Dictionary<string, object> _customData;
	}
}