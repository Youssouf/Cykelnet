/** Extend the Maps API with some nice functions */
google.maps.LatLng.prototype.kmTo = function(a){ 
    var e = Math, ra = e.PI/180; 
    var b = this.lat() * ra, c = a.lat() * ra, d = b - c; 
    var g = this.lng() * ra - a.lng() * ra; 
    var f = 2 * e.asin(e.sqrt(e.pow(e.sin(d/2), 2) + e.cos(b) * e.cos(c) * e.pow(e.sin(g/2), 2))); 
    return f * 6378.137; 
}
google.maps.Polyline.prototype.inKm = function(n){ 
    var a = this.getPath(n), len = a.getLength(), dist = 0; 
    for(var i=0; i<len-1; i++){ 
        dist += a.getAt(i).kmTo(a.getAt(i+1)); 
    } 
    return dist; 
} 

var CykelNet_Map = Class.extend({
    init: function (_options) {
        this.Options = $.extend(true, {
            Lat: 0,
            Long: 0,
        }, _options);

        this.NearestRouteAction = { active: false, callback: undefined };

        this.Options.center = new google.maps.LatLng(this.Options.Lat,this.Options.Long);
        
        this.map = new google.maps.Map($('#' + this.Options.MapContainerID).get(0), this.Options);

        var self = this;
        google.maps.event.addDomListener(this.map,
	        'click',
	        function(obj){
                if(self.NearestRouteAction.active){
                    self.NearestRouteAction.active = false;
                    self.NearestRouteAction.callback({Latitude: obj.latLng.lat(), Longitude: obj.latLng.lng()});
                    return;
                }
                self.addCoord(obj.latLng.lat(), obj.latLng.lng());
	        },
	        true
        );

        google.maps.event.addDomListener(this.map,
				'rightclick',
				function(obj){
					self.popCoord();
				},
				true
			);
    },

    getCurrentCenter: function(){
        return {Lat: this.Options.Lat, Long: this.Options.Long};
    },

    addCoord: function(_Lat, _Long){
	    if(!this.route){
		    var pathArray = [new google.maps.LatLng(_Lat, _Long)];
		    var options = {
		        strokeColor: '#0000FF',
		        strokeOpacity: 0.5,
		        map: this.map,
		        path: pathArray
		    };
		    this.route = new google.maps.Polyline(options);
            this.route.setEditable(true);
	    } else {
		    // A new coordinate
		    var _coord	= new google.maps.LatLng(_Lat, _Long);
		    // Push and update the old polyline
		    this.route.getPath().push(_coord);
	    }
    },

    getRoute: function(){
        var coords = [];
        if(!this.route)
        {
            return coords;
        }
        
        this.route.getPath().forEach(function(latlng){
            var obj = {Latitude: latlng.lat(), Longitude: latlng.lng()};
            coords.push(obj);
        });
        return coords;
    },

    popCoord: function(){
        if(this.route){
			this.route.getPath().pop();
		}
    },

    clearRoute: function(){
        if(this.route != undefined)
            this.route.getPath().clear();
    },

    /**
     * @param Object route [{Latitude:'',Longitude:''}]
     */
    setRouteAndFit: function(route){
        this.clearRoute();

        var bounds = new google.maps.LatLngBounds();
        for(var i = 0; i < route.length; i++){
            bounds.extend(new google.maps.LatLng(route[i].latitude, route[i].longitude));
            this.addCoord(route[i].latitude, route[i].longitude);
        }
        this.map.fitBounds(bounds);
        this.map.setCenter(bounds.getCenter());
    },

    triggerResize: function(){
        google.maps.event.trigger(this.map, 'resize');
    },

    setNearestRouteOnClick: function(callback){
        this.NearestRouteAction = { active: true, callback: callback };
        this.clearRoute();
    }
});