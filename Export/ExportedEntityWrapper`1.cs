using System;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export
{
	public class ExportedEntityWrapper<TEntity> : ExportedEntityWrapper
		where TEntity : InventoryEntityBase
	{
		public ExportedEntityWrapper(TEntity entity)
		{
			Entity = entity;
		}

		public override InventoryEntityBase EntityBase
		{
			get
			{
				return Entity;
			}
			set
			{
				Entity = value as TEntity;
			}
		}

		public override Type EntityType
		{
			get { return typeof(TEntity); }
		}

		public TEntity Entity { get; set; }
	}
}