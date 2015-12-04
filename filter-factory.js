(function (ng) {

	factory.$inject = ['$window'];

	function factory($window) {
		return function (key, defaultFilter) {
			
			var self = this;
			var prefix = 'filter-';

			self.setDefaultFilter = function (value) {
				defaultFilter = ng.extend(value || {}, { visible: true });
			};

			self.setDefaultFilter(defaultFilter);

			self.visible = function () {
				return self.applied.visible;
			};

			self.hidden = function () {
				return !self.visible();
			};

			self.toggle = function () {
				if (self.visible()) {
					self.hide();
					return;
				}

				self.show();
			};

			self.hide = function () {
				setVisible(false);
			};

			self.show = function () {
				setVisible(true);
			};

			self.applied = getAppliedFilter();
			self.tmp = getTmpFilter();

			self.apply = function () {
				var filter = self.tmp;
				self.applied = ng.copy(filter);
				saveToStorage(filter);
			};

			self.clear = function (key) {
				self.tmp = ng.copy(defaultFilter);
			};

			function getFilterFromStorage() {
				var serialized = $window.localStorage.getItem(prefix + key);

				if (serialized) {
					return ng.fromJson(serialized);
				}
			};

			function saveToStorage(filter) {
				var copy = ng.copy(filter);
				var serialized = ng.toJson(copy);
				$window.localStorage.setItem(prefix + key, serialized);
			};

			function setVisible(visible) {
				self.applied.visible = self.tmp.visible = visible;
				saveToStorage(self.applied);
			};

			function getTmpFilter() {
				if (self.tmp) {
					return self.tmp;
				}

				return ng.copy(self.applied);
			};

			function getAppliedFilter() {
				if (self.applied) {
					return self.applied;
				}

				var newFilter = getFilterFromStorage() || ng.copy(defaultFilter);
				self.applied = newFilter;
				return newFilter;
			};
		};
	};

	angular.module('sr').factory('filter-factory', factory);

})(window.angular);