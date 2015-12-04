

module app.common {

	export class inspection {

		constructor(private car: app.common.inspectionCar, private $q: ng.IQService, private dialogs) {
		}

		public convertToDto(): inspectionResultDto {
			var inspectionResultDto: inspectionResultDto = {
				CarId: this.car.Id,
				Packagings: _.map(this.packagings.filter(p => !!p), p => p.Id),
				Completenesses: _.map(this.completenesses, c => c.Id),
				AdditionalInfo: this.additionalInfo,
				TireInspections: this.tireInspections,
				InstalledTireSeason: this.installedTireSeason,
				AdditionalPhotos: this._additionalPhotos,
				Damages: this.getComponentDamagesDto(),
				KeyCount: this.keyCount,
				Mats: this.selectedMat.id || 0,
				CommonData: this.getCommonData(),
				PtsState: this.ptsState,
				HeldImagesSize: 0
			};

			return inspectionResultDto;
		}

		public convertFromDto(
			inspectionInfo: inspectionInfo,
			inspectionResultDto: inspectionResultDto,
			matChoises: idNamePair[]) {
			this._packagings = inspectionResultDto.Packagings.map(
				id => (<packaging[]>this.car.Packagings).filter(p => p.Id === id)[0]);
			this._completenesses = inspectionResultDto.Completenesses.map(
				id => inspectionInfo.Completenesses.filter(c => c.Id === id)[0]);
			this.additionalInfo = inspectionResultDto.AdditionalInfo;
			this._tireInspections = inspectionResultDto.TireInspections;
			this._tireInspections.forEach(ti => {
				if (ti.photoData) {
					ti.inspected = true;
				}
			});
			this._installedTireSeason = inspectionResultDto.InstalledTireSeason;
			this._additionalPhotos = inspectionResultDto.AdditionalPhotos;
			this._componentDamages = this.readComponentDamagesDtos(inspectionInfo, inspectionResultDto.Damages);
			this.keyCount = inspectionResultDto.KeyCount;
			if (inspectionResultDto.Mats !== null && inspectionResultDto.Mats !== undefined) {
				this.selectedMat = matChoises.filter(mc => mc.id === inspectionResultDto.Mats)[0];
			}
			this.ptsState = inspectionResultDto.PtsState;
			this.heldImagesSize = 0;
			this.convertPhotosFromDto();
		}

		public additionalInfo: string;

		public keyCount: number;

		public selectedMat: app.common.idNamePair;

		public ptsState: ptsState;

		public commonData: app.common.commonData[] = [];

		public get tireInspections(): tireInspection[]{
			return this._tireInspections;
		}

		public get additionalPhotos(): additionalPhoto[]{
			return this._additionalPhotos;
		}

		public get completenesses(): completeness[]{
			return this._completenesses;
		}

		public get packagings(): packaging[]{
			return this._packagings;
		}

		public get winterTireUsed() {
			return this.installedTireSeason === 1;
		}

		public get summerTireUsed() {
			return this.installedTireSeason === 0;
		}

		public get installedTireSeason(): number {
			return this._installedTireSeason;
		}

		public get componentDamages(): componentDamage[] {
			return this._componentDamages;
		}

		public updateTireSeason() {
			var seasonTireInspections = this.tireInspections.filter(ti => ti.inspected
				&& ti.season === this.installedTireSeason);

			if (!seasonTireInspections.length) {
				var completedTireInspections = this.tireInspections.filter(ti => ti.inspected);
				if (completedTireInspections.length) {
					this.installedTireSeason = completedTireInspections[0].season;
				} else {
					this._installedTireSeason = null;
				}
			}
		}

		public set installedTireSeason(value: number) {
			var completedInspections = this.tireInspections.filter(ti => ti.inspected);
			// Были произведены осмотры колес - сбросить установленные полностью нельзя.
			if (completedInspections.length && !value) {
				return;
			}

			var seasonInspections = completedInspections.filter(ti => ti.season === value);
			// Попытка установить флаг "установлены" для сезона, осмотр которого не был произведен.
			if (!seasonInspections.length) {
				return;
			}

			this._installedTireSeason = value;
		}

		public heldImagesSize: number = 0;

		// Подготовка BLOB'ов фоток для отсылки на сервер.
		public prepareBlobs(){
			var blobId = 1;
			var blobs = {};

			var prepare = function(images) {
				images.filter(ti => !!ti.image && !!ti.image.blob)
					.forEach(ap => this.prepareBlob(ap, blobId++, blobs));
			};

			prepare(this.additionalPhotos);
			prepare(this.componentDamages);
			prepare(this.tireInspections);
			return blobs;
		}

		private getComponentDamagesDto(): componentDamageDto[] {
			return _.map(this._componentDamages, d => {
				var result = new componentDamageDto();
				result.componentId = d.component.Id;
				result.damageId = d.damage.Id;
				result.photoData = d.photoData;
				return result;
			});
		}

		private readComponentDamagesDtos(
			inspectionInfo: inspectionInfo,
			damages: componentDamageDto[]): componentDamage[] {
			return damages.map(dto => {
				var componentDamage = new app.common.componentDamage();
				componentDamage.component = inspectionInfo.Components.filter(
					c => c.Id === dto.componentId)[0];
				componentDamage.damage = inspectionInfo.DamageTypes.filter(
					d => d.Id === dto.damageId)[0];
				componentDamage.photoData = dto.photoData;
				componentDamage.image = dto.image;
				return componentDamage;
			});
		}

		private getCommonData(): commonValueDto[] {
			return this.commonData.map(cd => {
				var result = new commonValueDto();
				result.DataType = commonDataType[cd.Type];
				if (cd.Value) {
					result.CommonValue = cd.Value.RussianName;
					if (cd.Value.FilterParent) {
						result.FilterParentType = commonDataType[cd.Value.FilterParent.Type];
						result.FilterParent = cd.Value.FilterParent.RussianName;
					}
				} else {
					result.TextValue = cd.TextValue;
				}

				return result;
			});
		}

		private convertPhotosFromDto() {
			this.tireInspections.forEach(ti => this.convertPhotoFromDto(ti));
			this.additionalPhotos.forEach(ap => this.convertPhotoFromDto(ap));
			this.componentDamages.forEach(cd => this.convertPhotoFromDto(cd));
		}

		private convertPhotoFromDto(photoObject: { photoData: string; image: app.common.imageResult }) {
			if (!photoObject.photoData) {
				return;
			}

			var binary = atob(photoObject.photoData.split(',')[1]);
			var array = [];
			for (var i = 0; i < binary.length; i++) {
				array.push(binary.charCodeAt(i));
			}
			var blob = new Blob([new Uint8Array(array)], { type: 'image/png' });
			var result = new imageResult(this.dialogs);
			result.blob = blob;
			result.createUrl();
			photoObject.photoData = null;
			photoObject.image = result;
		}

		private prepareBlob(photoObject: { photoData: string; image: app.common.imageResult }, blobId: number, blobs) {
			photoObject.photoData = blobId.toString();
			blobs[blobId] = photoObject.image.blob;
			URL.revokeObjectURL(photoObject.image.url);
			this.heldImagesSize += photoObject.image.blob.size;
			photoObject.image.blob = null;
			photoObject.image = null;
		}

		private _completenesses = [];
		private _packagings = [];
		private _tireInspections: tireInspection[] = [];
		private _installedTireSeason = undefined;
		private _additionalPhotos: additionalPhoto[] = [];
		private _componentDamages: componentDamage[] = [];
	}

}