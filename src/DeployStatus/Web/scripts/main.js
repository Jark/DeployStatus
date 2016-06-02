ko.bindingHandlers.dateString = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor(),
            allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);
        var dateString = valueUnwrapped;
        var parsedDate = moment(valueUnwrapped);
        if (parsedDate.isValid()) {
            var pattern = allBindings.datePattern || 'DD/MM/YYYY';
            dateString = moment(valueUnwrapped).format(pattern);
        }

        $(element).text(dateString);
    }
}

$(document).ready(function () {
	var ViewModel = function() {
		var self = this;
		var hub = $.connection.deployStatusHub;
		var mapping = {
		    key: function (data) {
		        return ko.utils.unwrapObservable(data.Id);
		    },
            'BranchRelatedTrellos': {
                key: function (data) {
                    return ko.utils.unwrapObservable(data.Id);
                }
            },
            'EnvironmentTaggedTrellos': {
                key: function (data) {
                    return ko.utils.unwrapObservable(data.Id);
                }
            },
            'Builds': {
                key: function (data) {
                    return ko.utils.unwrapObservable(data.Id);
                }
            }
		}
		self.Name = ko.observable("Contacting server...");
	    self.LastUpdated = ko.observable("never");
	    self.Environments = ko.mapping.fromJS([], mapping);

	    self.sortFunctions = ko.observableArray([
	        {
	            name: "Deployable",
	            fn: function(a, b) {
	                return a.EnvironmentTaggedTrellos().length < b.EnvironmentTaggedTrellos().length ? -1 : 1;
	            }
	        },
	        {
	            name: "Name",
	            fn: function(a, b) {
	                return a.Name() < b.Name() ? -1 : 1;
	            }
	        }
	    ]);
	    self.selectedSortFunction = ko.observable(self.sortFunctions()[0]);
	    self.sortedEnvironments = ko.computed(function() {
	        var records = ko.observableArray(self.Environments());
	        var sortFunction = self.selectedSortFunction();
            if(sortFunction)
                records.sort(sortFunction);
	        return records();
	    }).extend({ throttle: 5 });

        self.sort = function() {
            this.Environments.sort(self.selectedSortFunction().fn);
        }

	    var updateEnvironments = function (environments) {
	        if (environments.length === 0)
	            return; // keep old state on server restarts

	        ko.mapping.fromJS(environments, self.Environments);
	        var allDropdowns = $('.ui.dropdown');
	        for (var i = 0; i < allDropdowns.length; i++) {
	            var dropDown = $(allDropdowns[i]);
	            if (dropDown.data("dropdown") !== true) {
	                dropDown.data("dropdown", true);
	                dropDown.dropdown();
	            }
	        }
	    };

	    var updateDeploySystemStatus = function (newDeploySystemStatus) {
	        updateEnvironments(newDeploySystemStatus.Environments);
	        self.Name(newDeploySystemStatus.Name);
	        self.LastUpdated(newDeploySystemStatus.LastUpdated);
	    };

	    var startAndInitialise = function () {
	        $.connection.hub.start().done(function () {
	            hub.server.getDeploySystemStatus().done(updateDeploySystemStatus);
	        });
	    }

	    hub.client.deploySystemStatusChanged = updateDeploySystemStatus;
        
        startAndInitialise();

		$.connection.hub.disconnected(function () {
		    setTimeout(function () {
		        startAndInitialise();
		    }, 5); // Restart connection after 5 seconds.
		});
	};

	ko.applyBindings(new ViewModel());
});