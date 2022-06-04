import { displaySteepness } from './map.js';
import { isWalkabilityDisplayed } from './legend.js';

export let maximumSteepness;

const maximumSteepnessSlider = document.getElementById("maximumSteepness");
const maximumSteepnessOutput = document.getElementById("maximumSteepnessOutput");
const customSteepness = document.getElementById("customSteepness");
const wheelchairSteepness = document.getElementById("wheelchairSteepness");
const poweredSteepness = document.getElementById("poweredSteepness");
const caneSteepness = document.getElementById("caneSteepness");


export function init() {
    maximumSteepness = maximumSteepnessSlider.value;
    maximumSteepnessOutput.innerHTML = maximumSteepnessSlider.value + ' degrees';

    maximumSteepnessSlider.addEventListener("input", () => {
        maximumSteepness = maximumSteepnessSlider.value;
        maximumSteepnessOutput.innerHTML = maximumSteepnessSlider.value + ' degrees';

        if (!isWalkabilityDisplayed()) {
            displaySteepness();
        }
    });

    customSteepness.onclick = steepnessMethodChanged;
    wheelchairSteepness.onclick = steepnessMethodChanged;
    poweredSteepness.onclick = steepnessMethodChanged;
    caneSteepness.onclick = steepnessMethodChanged;

    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    steepnessMethodChanged();
}

function steepnessMethodChanged() {
    if (customSteepness.checked) {
        maximumSteepnessSlider.disabled = false;
    } else {
        maximumSteepnessSlider.disabled = true;
    }
    if (wheelchairSteepness.checked) {
        maximumSteepnessSlider.value = 4.8;
        maximumSteepnessOutput.innerHTML = maximumSteepnessSlider.value + ' degrees';
    } else if (poweredSteepness.checked) {
        maximumSteepnessSlider.value = 7.1;
        maximumSteepnessOutput.innerHTML = maximumSteepnessSlider.value + ' degrees';
    } else if (caneSteepness.checked) {
        maximumSteepnessSlider.value = 8;
        maximumSteepnessOutput.innerHTML = maximumSteepnessSlider.value + ' degrees';
    }
    maximumSteepness = maximumSteepnessSlider.value;

    if (!isWalkabilityDisplayed()) {
        displaySteepness();
    }
}