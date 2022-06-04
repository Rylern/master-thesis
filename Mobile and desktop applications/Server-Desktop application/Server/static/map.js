import { maximumSteepness } from './steepness.js';
import { indicators } from './indicators.js';
import { createToast } from './toasts.js';

mapboxgl.accessToken = 'pk.eyJ1IjoicnlsZXJuIiwiYSI6ImNreXR1OW9tZTBpYmsyeG84bDhweXlpOHMifQ.IaYeWhFNJG8TQl3-Rl-zZg';
const HEATMAP_SOURCE_ID = "heatmap_source";
const HEATMAP_LAYER_ID = "heatmap_layer";
const STEEPNESS_SOURCE_ID = "steepness_source";
const STEEPNESS_LAYER_ID = "steepness_layer";
const NAVIGATION_SOURCE_ID = "navigation_source";
const NAVIGATION_LAYER_ID = "navigation_layer";


let map;
let heatmap;
let steepness;


export function init() {
    map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: [6.184376, 62.473821],
        zoom: 11,
        bearing: 0
    });
}

export async function fetchHeatmap() {
    createToast("Walkability", "Fetching indicators...");
    
    const response = await fetch("heatmap/", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(indicators)
    });
    if (response.ok) {
        heatmap = await response.json();
        createToast("Walkability", "Indicators fetched.");
    } else {
        createToast("Walkability", "Server error.");
    }
}

export async function displayHeatmap() {
    hideHeatmap();
    hideSteepness();

    if (heatmap == undefined) {
        await fetchHeatmap();
    }

    map.addSource(HEATMAP_SOURCE_ID, {
        type: 'geojson',
        data: heatmap
    });
    map.addLayer({
        "id": HEATMAP_LAYER_ID,
        "type": "line",
        "source": HEATMAP_SOURCE_ID,
        "paint": {
            "line-opacity": 0.7,
            "line-width": [
                "interpolate",
                ["linear"],
                ["zoom"],
                0, 0,
                22, 5
            ],
            "line-color": [
                "rgb",
                [
                    "interpolate",
                    ["linear"],
                    ["get", "indicator"],
                    0, 254,
                    0.5, 254,
                    1, 145
                ],
                [
                    "interpolate",
                    ["linear"],
                    ["get", "indicator"],
                    0, 119,
                    0.5, 254,
                    1, 254
                ],
                [
                    "interpolate",
                    ["linear"],
                    ["get", "indicator"],
                    0, 77,
                    0.5, 118,
                    1, 114
                ]
            ]
        },
        "layout": {
            "line-cap": "round",
            "line-join": "round"
        }
    }, "building-number-label");
}

function hideHeatmap() {
    if (map.getLayer(HEATMAP_LAYER_ID)) {
        map.removeLayer(HEATMAP_LAYER_ID);
        map.removeSource(HEATMAP_SOURCE_ID);
    }
}

async function fetchSteepness() {
    createToast("Steepness", "Fetching steepness...");

    const response = await fetch("steepness/");
    if (response.ok) {
        steepness = await response.json();
        createToast("Steepness", "Steepness fetched.");
    } else {
        createToast("Steepness", "Server error.");
    }
}

export async function displaySteepness() {
    hideHeatmap();
    hideSteepness();

    if (steepness == undefined) {
        await fetchSteepness();
    }

    const minSteepness = 4.0;
    let maxSteepness = Number(maximumSteepness) < minSteepness ? minSteepness : Number(maximumSteepness);
    maxSteepness += 0.01;
    const midSteepness = (maxSteepness - minSteepness) / 2.0 + minSteepness;

    map.addSource(STEEPNESS_SOURCE_ID, {
        type: 'geojson',
        data: steepness
    });
    map.addLayer({
        "id": STEEPNESS_LAYER_ID,
        "type": "line",
        "source": STEEPNESS_SOURCE_ID,
        "paint": {
            "line-opacity": 0.7,
            "line-width": [
                "interpolate",
                ["linear"],
                ["zoom"],
                0, 0,
                22, 5
            ],
            "line-color": [
                "rgb",
                [
                    "interpolate",
                    ["linear"],
                    ["get", "steepness"],
                    minSteepness, 145,
                    midSteepness, 254,
                    maxSteepness, 254,
                    maxSteepness+0.01, 254
                ],
                [
                    "interpolate",
                    ["linear"],
                    ["get", "steepness"],
                    minSteepness, 254,
                    midSteepness, 254,
                    maxSteepness, 119,
                    maxSteepness+0.01, 0
                ],
                [
                    "interpolate",
                    ["linear"],
                    ["get", "steepness"],
                    minSteepness, 114,
                    midSteepness, 118,
                    maxSteepness, 77,
                    maxSteepness+0.01, 0
                ]
            ]
        },
        "layout": {
            "line-cap": "round",
            "line-join": "round"
        }
    }, "building-number-label");
}

function hideSteepness() {
    if (map.getLayer(STEEPNESS_LAYER_ID)) {
        map.removeLayer(STEEPNESS_LAYER_ID);
        map.removeSource(STEEPNESS_SOURCE_ID);
    }
}

export async function displayNavigation(fromLocation, toLocation)
{
    hideNavigation();

    const url = "navigation/?fromLatitude=" + fromLocation[1] + "&fromLongitude=" + fromLocation[0] + "&toLatitude=" + toLocation[1] + "&toLongitude=" + toLocation[0];
    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(indicators)
    });
    const responseJSON = await response.json();
    
    map.addSource(NAVIGATION_SOURCE_ID, {
        type: 'geojson',
        data: responseJSON
    });
    map.addLayer({
        "id": NAVIGATION_LAYER_ID,
        "type": "line",
        "source": NAVIGATION_SOURCE_ID,
        "paint": {
            "line-opacity": 0.7,
            "line-width": 8,
            "line-color": "#303F9F"
        },
        "layout": {
            "line-cap": "round",
            "line-join": "round"
        }
    }, "building-number-label");
}

export function hideNavigation() {
    if (map.getLayer(NAVIGATION_LAYER_ID)) {
        map.removeLayer(NAVIGATION_LAYER_ID);
        map.removeSource(NAVIGATION_SOURCE_ID);
    }
}