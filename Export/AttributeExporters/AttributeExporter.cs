using System;
using Inventory.Infrastructure.AttributeExpressions;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export.AttributeExporters
{
	public class AttributeExporter<TEntity, TValue, TResult> : IAttributeExporter<TEntity>
		where TEntity : InventoryEntityBase
	{
		public AttributeExporter(
			string exportName,
			AttributeExpression<TEntity, TValue> expression)
		{
			_exportName = exportName;
			_expression = expression;
		}

		public AttributeExporter(
			string exportName,
			AttributeExpression<TEntity, TValue> expression,
			Func<TValue, TResult> converter)
		{
			_exportName = exportName;
			_expression = expression;
			_converter = converter;
		}

		public void Export(ExportedEntityWrapper<TEntity> exportedEntity)
		{
			var value = _expression.GetValue(exportedEntity.Entity);
			object result = value;

			if (_converter != null)
			{
				result = _converter(value);
			}

			if (result != null)
			{
				exportedEntity.ExportedAttributes[_exportName] = result;
			}
		}

		readonly string _exportName;
		readonly AttributeExpression<TEntity, TValue> _expression;
		readonly Func<TValue, TResult> _converter;
	}
}