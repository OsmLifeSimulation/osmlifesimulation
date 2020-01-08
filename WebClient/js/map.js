var map = new ol.Map({
	target: 'map',
	layers: [
		new ol.layer.Tile({
			source: new ol.source.OSM()
		}),
	],
	view: new ol.View({
		center: ol.proj.fromLonLat([44.4766846, 48.8045959]),
		zoom: 9
	})
});

function SetCharacters(allNewData) {
	allNewData.forEach(currentNewData => {

		var layerType = currentNewData.Item1;
		var geojsonObject = currentNewData.Item2;
		
		var sameLayer = map.getLayers().getArray().find((element) => element.get('layerType') == layerType)

		var vectorSource = new ol.source.Vector({
			features: (new ol.format.GeoJSON()).readFeatures(geojsonObject)
		});

		if(sameLayer){
			sameLayer.setSource(vectorSource);
		}
		else{
			var vectorLayer = new ol.layer.Vector({
				style: eval("new ol.style.Style({" + currentNewData.Item3 + "});")
			});
			vectorLayer.set('layerType', layerType);
			vectorLayer.setSource(vectorSource);
			map.addLayer(vectorLayer);
		}
	});
}


