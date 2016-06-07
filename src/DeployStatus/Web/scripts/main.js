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

var Filter = function(name, fn) {
    var self = this;
    self.name = name;
    self.fn = fn;
    self.active = ko.observable(false);
    self.displayName = ko.computed(function() {
        return self.active()
            ? "Show " + self.name
            : "Hide " + self.name;
    });
    self.toggle = function() {
        self.active(!self.active());
    };
};

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

	    var nameSort = function(a, b) {
	        return a.Name() < b.Name() ? -1 : 1;
	    };

	    self.sortFunctions = ko.observableArray([
	        {
	            name: "Is Deployable",
	            fn: function(a, b) {
	                var firstLength = a.EnvironmentTaggedTrellos().length;
	                var secondLength = b.EnvironmentTaggedTrellos().length;
	                if (firstLength === secondLength)
	                    return nameSort(a, b);
	                return firstLength < secondLength ? -1 : 1;
	            }
	        },
	        {
	            name: "Name",
	            fn: nameSort
	        }
	    ]);
	    self.sortFn = ko.observable(self.sortFunctions()[0]);

	    self.filters = ko.observableArray([
	        new Filter("Disabled", function (environment) {
	            return environment.IsDisabled() === false;
	        })
	    ]);
	    
	    self.sortedEnvironments = ko.computed(function() {
            var environments = self.Environments();
            var sortFn = self.sortFn();
	        var filters = self.filters();
            var filterFns = filters.filter(function (item){return item.active() === true});
            filterFns.every(function (item) {
                environments = environments.filter(item.fn);
            });
            var sorted = environments.sort(sortFn.fn);
            return sorted;
	    }, self);

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