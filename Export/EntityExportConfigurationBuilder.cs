using System;
using System.Linq.Expressions;
using Inventory.Infrastructure.AttributeExpressions;
using Inventory.Model;
using Inventory.Repositories;
using Inventory.Services.DataExchange.Export.AttributeExporters;

namespace Inventory.Services.DataExchange.Export
{
	/// <summary>
	/// Облегчает построение конфигурации экспорта сущности при определении
	/// общей конфигурации экспорта.
	/// </summary>
	public class EntityExportConfigurationBuilder<TEntity>
		where TEntity : InventoryEntityBase
	{
		public EntityExportConfigurationBuilder(
			CommonExpressions commonExpressions,
			RepositoriesCollection repositories)
		{
			_commonExpressions = commonExpressions;
			_repositories = repositories;

			Exporter = new EntityExporter<TEntity>(repositories);
		}

		public EntityExportConfigurationBuilder<TEntity> OnCondition(
			Expression<Func<TEntity, bool>> predicate)
		{
			Exporter.Condition = predicate;
			return this;
		}

		public EntityExportConfigurationBuilder<TEntity> EntityName(string name)
		{
			Exporter.EntityName = name;
			return this;
		}

		public EntityExportConfigurationBuilder<TEntity> Attribute<TValue>(string name,
			Expression<Func<TEntity, TValue>> attribute)
		{
			return Attribute<TValue, TValue>(name, attribute, null);
		}

		public EntityExportConfigurationBuilder<TEntity> Attribute<TValue, TConvertedValue>(string name,
			Expression<Func<TEntity, TValue>> attribute, Func<TValue, TConvertedValue> converter)
		{
			var attributeExpression = new AttributeExpression<TEntity, TValue>(attribute);
			var attributeExporter = new AttributeExporter<TEntity, TValue, TConvertedValue>(
				name, attributeExpression, converter);
			Exporter.AttributeExporters.Add(attributeExporter);

			return this;
		}

		public EntityExportConfigurationBuilder<TEntity> CustomData(string name)
		{
			return CustomData(name, name);
		}

		public EntityExportConfigurationBuilder<TEntity> CustomData(
			string customDataName, string exportedName)
		{
			var customDataExporter = new CustomDataExporter(customDataName, exportedName, null);
			Exporter.CustomDataExporters.Add(customDataExporter);
			return this;
		}

		public EntityExportConfigurationBuilder<TEntity> CustomData(
			string customDataName, string exportedName,
			Func<object, object> converter)
		{
			var customDataExporter = new CustomDataExporter(customDataName, exportedName, converter);
			Exporter.CustomDataExporters.Add(customDataExporter);
			return this;
		}

		public EntityExportConfigurationBuilder<TEntity> Include<TReferencedEntity>(
			string exportedIdName,
			Action<EntityExportConfigurationBuilder<TReferencedEntity>> buildingFunc,
			Expression<Func<TEntity, Guid?>> reference)
			where TReferencedEntity : InventoryEntityBase
		{
			var referenceAttributeExpression = new AttributeExpression<TEntity, Guid?>(reference);
			var referencedIdExporter = new ReferencedIdExporter<TEntity, TReferencedEntity>(
				exportedIdName, referenceAttributeExpression);

			EntityExporter<TReferencedEntity> referencedEntityExporter;

			if (typeof(TEntity) != typeof(TReferencedEntity))
			{
				var builder = new EntityExportConfigurationBuilder<TReferencedEntity>(
					_commonExpressions, _repositories);
				buildingFunc(builder);
				referencedEntityExporter= builder.Exporter;
			}
			else
			{
				// При ссылке сущности на себя используем уже существующий экспортер во избежание рекурсии.
				referencedEntityExporter = Exporter as EntityExporter<TReferencedEntity>;
			}

			Exporter.Includes.Add(referencedIdExporter, referencedEntityExporter);
			return this;
		}

		public EntityExportConfigurationBuilder<TEntity> CompanyDependentId(string exportedName, string companyId)
		{
			var generator = new CompanyDependentExportIdGenerator(
				_commonExpressions, companyId, exportedName);
			Exporter.ExportedIdGenerator = generator;
			return this;
		}

		public EntityExporter<TEntity> Exporter { get; private set; }
		readonly RepositoriesCollection _repositories;
		readonly CommonExpressions _commonExpressions;
	}
}