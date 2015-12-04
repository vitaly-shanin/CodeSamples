/// <reference path="../../typings/lodash/lodash.d.ts" />
/// <reference path="../services/breezeclient.ts" />

module app.inspectionList {

	export class inspectionListService {

		public static $inject = ['breeze-client', 'inspection-result-sender', 'web-client', '$q', 'dialogs'];
		constructor(
			private breezeClient: services.breezeClient,
			private resultSender: app.inspection.resultSender,
			private webClient: services.webClient,
			private $q: ng.IQService,
			private dialogs) {
		}

		public forceNextInspectionInfoReload: boolean = false;

		public getInspectionInfo(): ng.IPromise<common.inspectionInfo> {
			var defer = this.$q.defer<common.inspectionInfo>();

			if (!this.inspectionInfo || !this.inspectionInfo.hasUnsentCars && this.forceNextInspectionInfoReload) {
				if (this.forceNextInspectionInfoReload) {
					this.cleanInspections();
				}

				this.forceNextInspectionInfoReload = false;
				this.breezeClient.getInspectionInfo(
					info => this.onInspectionInfoObtained(info).then(() => defer.resolve(this.inspectionInfo)),
					() => { defer.resolve(this.inspectionInfo); });
			} else {
				defer.resolve(this.inspectionInfo);
			}

			return defer.promise;
		}

		public cleanInspections() {
			if (this.inspectionInfo) {
				this.inspectionInfo.InspectionResultDtos.forEach(dto => {
					var images = _.union(dto.AdditionalPhotos.map(p => p.image),
						dto.Damages.map(d => d.image),
						dto.TireInspections.map(ti => ti.image))
						.filter(i => i != null);
					images.forEach(i => i.cleanUp());
				});

				this.inspectionInfo.InspectionResultDtos = null;
			}

			this.cars.forEach(car => {
				if (car && car.inspection) {
					car.inspection.prepareBlobs();
					car.inspection = null;
				}
			});
		}


		public inspectionInfo: app.common.inspectionInfo;

		public carSelectionFilterOptions: ngGrid.IFilterOptions = {
			filterText: '',
			useExternalFilter: false,
			columns: 'Vin;Tti;modelFull;'
		}

		public get cars() : app.common.inspectionCar[] {
			if (this.inspectionInfo != undefined) {
				return this.inspectionInfo.Cars;
			}
			return null;
		}

		public get hasUnsentCars(): boolean {
			if (this.inspectionInfo) {
				return this.inspectionInfo.hasUnsentCars;
			}
			return null;
		}

		private onInspectionInfoObtained(info: app.common.inspectionInfo): ng.IPromise<{}> {
			this.inspectionInfo = info;
			this.resultSender.process(info);
			return this.$q.all(this.inspectionInfo.InspectionResultDtos.map(dto => this.getReviewImages(dto)));
		}
		
		private getReviewImages(inspectionResultDto: common.inspectionResultDto): ng.IPromise<{}> {
			var promises =
				inspectionResultDto.TireInspections.map(ti => this.getReviewImage(ti)).concat(
					inspectionResultDto.AdditionalPhotos.map(ap => this.getReviewImage(ap))).concat(
					inspectionResultDto.Damages.map(d => this.getReviewImage(d)));
			return this.$q.all(promises);
		}

		private getReviewImage(photoObject: { photoData: string; image: common.imageResult }): ng.IPromise<void> {
			return this.webClient.getPhoto(photoObject.photoData)
				.then(d => {
					var imageResult = new common.imageResult(this.dialogs);
					var contentType = d.headers("content-type");
					imageResult.blob = new Blob([d.data], {
						type: contentType
					});
					imageResult.createUrl();
					photoObject.image = imageResult;
					photoObject.photoData = null;
				});
		}

	}

}