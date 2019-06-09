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


function SetCharacters(allNewData){
	
	var features = [];

	allNewData.forEach(currentNewData => {
		newData = currentNewData.Item1;
		newDataStyle = eval("new ol.style.Style({" + currentNewData.Item2 + "});")

		var feature, geometry;
		for (i = 0; i < newData.length; ++i) {
			geometry = new ol.geom.Point(ol.proj.transform(newData[i].split(',').map(Number).reverse(), 'EPSG:4326','EPSG:3857'));
			feature = new ol.Feature(geometry);
			feature.setStyle(newDataStyle);
			features.push(feature);
		}
	});

	var vectorSource = new ol.source.Vector({
		features: features
	});

	charactersLayer.setSource(vectorSource);
}


