using System;
using Inventory.Model;

namespace Inventory.Services.DataExchange.Export
{
	class CompanyDependentExportIdGenerator : ExportIdGenerator
	{
		public CompanyDependentExportIdGenerator(
			CommonExpressions commonExpressions,
			string companyId, string exportedIdName)
			: base(exportedIdName)
		{
			_companyId = companyId;
			_companyBelongingChecker = commonExpressions.EntityWasImportedFromCompany.Compile();
			_realIdExFromCombinedFunc = commonExpressions.GetRealEntityIdExFromCombinedIdEx.Compile();
		}

		protected override string GetEntityId(ExportedEntityWrapper entity)
		{
			var entityIdEx = entity.EntityBase.IdEx;

			bool entityWasImportedWithCompany
				= _companyBelongingChecker(entityIdEx, _companyId);

			if (!entityWasImportedWithCompany)
			{
				// В случае, если экспортируемая сущность не была импортирована
				// вместе с филиалом (создана в АйТи-АИ или перемещена между филиалами),
				// экспортируется внутренний ID сущности в качестве ID для УС заказчика.
				// С точки зрения УС филиала, в которую происходит экспорт, это новая сущность.
				return entity.EntityBase.Id.ToString("N").ToUpperInvariant();
			}

			return _realIdExFromCombinedFunc(entityIdEx);
		}

		readonly Func<string, string> _realIdExFromCombinedFunc;
		readonly Func<string, string, bool> _companyBelongingChecker;
		readonly string _companyId;
	}
}