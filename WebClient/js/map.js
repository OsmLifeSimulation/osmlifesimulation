var charactersLayer = new ol.layer.Vector({

	});

var map = new ol.WebGLMap({
	target: 'map',
	layers: [
	  new ol.layer.Tile({
		source: new ol.source.OSM()
	  }),
	  charactersLayer
	],
	view: new ol.View({
	  center: ol.proj.fromLonLat([44.4766846, 48.8045959]),
	  zoom: 9
	})
});

  
  var CharacterStyle = new ol.style.Style({
		image: new ol.style.Circle({
		opacity: 1.0,
		scale: 1.0,
		radius: 3,
		fill: new ol.style.Fill({
		  color: 'rgba(155, 21, 21, 0.8)'
		}),
		stroke: new ol.style.Stroke({
		  color: 'rgba(53, 5, 5, 1)',
		  width: 1
		}),

	  })
  });


function SetCharacters(newData){
	
	var features = new Array(newData.length);
	var feature, geometry;
	
	for (i = 0; i < newData.length; ++i) {
		geometry = new ol.geom.Point(ol.proj.transform(newData[i], 'EPSG:4326','EPSG:3857'));
		feature = new ol.Feature(geometry);
		feature.setStyle(CharacterStyle);
		features[i] = feature;
	}

	var vectorSource = new ol.source.Vector({
		features: features
	});

	charactersLayer.setSource(vectorSource);
}


