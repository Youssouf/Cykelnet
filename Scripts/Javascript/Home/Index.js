var Index = {
    loadMap: function (routeID) {
        var url = "/Route/Gmap";
        if (routeID != undefined && !isNaN(routeID)) {
            url += "?id=" + routeID;
        }

        $.ajax({
            type: 'GET',
            url: url,
            success: function (data) {
                $('#gmapSection').html(data);
            },
            error: function (data) {
                alert("Fubar!");
            }
        });
    },

    loadEventList: function () {
        $.ajax({
            type: 'GET',
            url: '/Event/EventList',
            success: function (data) {
                $('#sidebar-events').html(data);
            },
            error: function (data) {
                alert("Fubar!");
            }
        });
    },

    showRouteList: function (ListHtml) {
        $('#gmapList').hide('fast');
        // TODO: The HTML is apparently updated before the list is hidden
        //if ($('#gmapList').is(':visible')) {
        //    Index.hideRouteList(function () {
        //        $('#gmapList').html(ListHtml).height($('#gmapSection').height());
        //    });
        //} else {
            $('#gmapList').html(ListHtml).height($('#gmapSection').height());
        //}

        $('#gmapSection').css('left', '420px');
        //$('#gmapSection').animate({ left: '420px' }, { duration: 400, complete: _CN_Map.triggerResize() });
        _CN_Map.triggerResize()
        $('#gmapList').show('fast');
    },

    hideRouteList: function (callback) {
        $('#gmapSection').css('left', '0px');
        //$('#gmapSection').animate({ left: '0px' }, { duration: 0.5, complete: callback });
        $('#gmapList').hide('fast');
        callback();
        _CN_Map.triggerResize();
    },

    searchRoutes: function () {
        var searchString = $('#control_searchRoutes').val();
        if (searchString.replace(/ /g, "") == "")
            return;
        // Search using ajax (Maybe show a loader?)
        $.ajax({
            type: 'POST',
            data: { searchString: searchString },
            dataType: 'text',
            url: '/Route/SearchViaTags',
            success: function (data) {
                console.warn("The search has returned");
                $('#control_searchRoutes').val('').blur();
                Index.showRouteList(data);
            },
            error: function (data) {
                alert("Fubar!");
            }
        });

    },

    getGeolocation: function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                // Found you
                console.warn("Found you...");
                console.warn(position);
                var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                _CN_Map.map.setCenter(latlng);
                _CN_Map.map.setZoom(15);
                _CN_Map.clearRoute();

                var marker = new google.maps.Marker({
                    position: latlng,
                    map: _CN_Map.map,
                    title: "We found you, hehe"
                });
            },
            function (msg) {
                console.warn("Error while tracking");
                console.warn(msg);
            });
        } else {
            // Geolocation is not supported
        }
    },

    showRoute: function (id) {
        if (id == undefined)
            return

        $.ajax({
            type: 'POST',
            url: '/Route/RouteCoords/' + id,
            data: { id: id },
            dataType: 'json',
            success: function (data) {
                _CN_Map.setRouteAndFit(JSON.parse(data));
            },
            error: function (data) {
                alert("Fubar!");
            }
        });
    }
};

$(document).ready(function () {
    Index.loadMap();
    Index.loadEventList();

    // -------------------------------------------------
    // Init event handlers for map controls
    // -------------------------------------------------

    // Make new route
    $('#control_newRoute').click(function (e) {
        e.preventDefault();
        Index.showRouteList($('#new_route_tmpl').html());
        //Index.showRouteList('<button type="button" onclick="Route.saveDialog()">Save route</button>');
        _CN_Map.clearRoute();
        $('#route_form').submit(function (e) {
            e.preventDefault();
            Route.saveDialog();
        });
    });

    // My routes
    $('#control_myRoutes').click(function (e) {
        e.preventDefault();
        // Show list of personal routes
        $.ajax({
            type: 'GET',
            url: '/Route/PersonalRoutes',
            success: function (data) {
                Index.showRouteList(data);
            },
            error: function (data) {
                alert("Fubar!");
            }
        });
    });

    // Favorite routes
    $('#control_favorites').click(function (e) {
        e.preventDefault();
        // Show list of favorite routes
        $.ajax({
            type: 'GET',
            url: '/Route/FavoriteRoutes',
            success: function (data) {
                Index.showRouteList(data);
            },
            error: function (data) {
                alert("Fubar!");
            }
        });
    });

    // All routes
    $('#control_listRoutes').click(function (e) {
        e.preventDefault();
        // Show a list of all routes
        $.ajax({
            type: 'GET',
            url: '/Route/ListRoutes',
            success: function (data) {
                Index.showRouteList(data);
            },
            error: function (data) {
                alert("Fubar!");
            }
        });
    });

    // Search
    $('#controls_searchRoutes_img').click(function (e) {
        Index.searchRoutes();
    });
    $('#control_searchRoutes').keypress(function (e) {
        if (event.which == 13) {
            Index.searchRoutes();
        }
    });

    // Nearest routes query
    $('#control_searchPoint').click(function (e) {
        e.preventDefault();
        alert("Click on the map, to search for the 10 nearest routes.");
        _CN_Map.setNearestRouteOnClick(function (coord) {
            $.ajax({
                type: 'POST',
                data: { Latitude: coord.Latitude, Longitude: coord.Longitude },
                dataType: 'text',
                url: '/Route/SearchViaPoint',
                success: function (data) {
                    Index.showRouteList(data);
                },
                error: function (data) {
                    console.warn(data);
                    alert("Fubar!");
                }
            });
        });
    });
});
