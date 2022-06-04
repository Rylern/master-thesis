import { displayHeatmap, displaySteepness } from './map.js';


const visualizationChoice = document.getElementById("visualization_choice");
const legendImage = document.getElementById("legend_image");


export function init() {
    displayHeatmapOrSteepness();
    visualizationChoice.addEventListener('change', () => {
        displayHeatmapOrSteepness();
    });
}

function displayHeatmapOrSteepness() {
    if (visualizationChoice.value == "walkability") {
        displayHeatmap();
        legendImage.style.backgroundImage = "linear-gradient(to right, #ff774d, #ffff76, #91ff72)";
    } else {
        displaySteepness();
        legendImage.style.backgroundImage = "linear-gradient(to right, #91ff72, #ffff76, #ff774d)";
    }
}

export function isWalkabilityDisplayed() {
    return visualizationChoice.value == "walkability";
}