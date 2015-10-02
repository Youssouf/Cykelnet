var Route = {
    saveDialog: function () {
        if (_CN_Map.getRoute().length == 0) {
            alert("You must draw a route before you can save it");
            return;
        }

        /*var _diaHtml = $('#route_save_tmpl').html();
        this.Dialog = $(_diaHtml).dialog({
            autoOpen: false,
            closeOnEscape: true,
            draggable: false,
            modal: true,
            resizable: false,
            title: "Save route"
        });

        this.Dialog.dialog('open');
        $('#route_form').submit(function (e) {
            e.preventDefault();*/
            var _form = $('#route_form'),
                _rName = _form.find('#route_name').val(),
                _rCity = _form.find('#route_city').val(),
                _rDescription = _form.find('#route_description').val(),
                _rTags = JSON.stringify(Route.getTags())

            Route.saveRoute(_rName, _rCity, _rDescription, _rTags);

        //});
    },

    closeDialog: function () {
        this.Dialog.dialog('close');
    },

    saveRoute: function (routeName, routeCity, routeDescription, routeTags) {
        $.ajax({
            type: 'POST',
            url: '/Route/Create',
            data: {
                jsonRoute: JSON.stringify(_CN_Map.getRoute()),
                routeName: routeName,
                routeCity: routeCity,
                routeDescription: routeDescription,
                routeTags: routeTags
            },
            dataType: 'text',
            success: function (data) {
                alert("The route was saved");
                Route.Dialog.dialog('close');
            },
            error: function (data) {
                alert("There was an error saving the route");
            }
        });
    },

    getTags: function () {
        var firstRun = true,
            result = [];
        $('input[name="route_tags[]"]').each(function (index, element) {
            result.push($(element).val());
        });

        return result;
    },

    addTag: function () {
        var _newTag = $('#tag_add').val(),
            isNew = true;
        $('#tag_add').val(''); // Reset input

        if (_newTag == "")
            return false;

        if (jQuery.inArray(_newTag, Route.getTags()) > -1)
            return false; // The tag already existed

        var _inpElem = "<input type='text' name='route_tags[]' value='" + _newTag + "' />",
            _liElem = "<li>" + _newTag + "</li>";

        $('#route_tags').append(_inpElem);
        $('#route_tag_list').append(_liElem);
    }
};

var _CN_Map,
    _MapOptions = {
        Lat: 57.028811,
        Long: 9.917771,
        MapContainerID: "MapContainer",
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        kilometerLabel: "# KM"
    };

_CN_Map = new CykelNet_Map(_MapOptions);
    
// Calculate the bounds and fit the map to the route
// Also adds the route to the CykelNet_Map object
if(_route != undefined){
    var bounds = new google.maps.LatLngBounds();
    for(var i=0;i<_route.length;i++){
        bounds.extend(new google.maps.LatLng(_route[i].latitude, _route[i].longitude));
        _CN_Map.addCoord(_route[i].latitude, _route[i].longitude);
    }
    _CN_Map.map.fitBounds(bounds);
}
