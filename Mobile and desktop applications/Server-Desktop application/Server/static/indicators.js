import { displayHeatmap, fetchHeatmap } from './map.js';
import { isWalkabilityDisplayed } from './legend.js';

export let indicators;

let allIndicators;
let description;
const editWeightsContent = document.getElementById("editWeightsContent");
const selectIndicatorsContent = document.getElementById("selectIndicatorsContent");


export async function init() {
    await getAllIndicators();
    initUI();
}

async function getAllIndicators() {
    const language = (window.navigator.userLanguage || window.navigator.language).split("-")[0];
    const response = await fetch("indicators/?language=" + language);
    const responseJSON = await response.json();

    description = responseJSON["description"];
    allIndicators = responseJSON["indicators"];
    indicators = JSON.parse(JSON.stringify(allIndicators));
    indicators.forEach(indicator => {
        indicator["weight"] = 3;
    });
}

function initUI() {
    const selectIndicators = document.getElementById("selectIndicators");
    selectIndicators.onclick = setUpSelectIndicatorsWindow;

    const selectIndicatorsNextButton = document.getElementById("selectIndicatorsNext");
    selectIndicatorsNextButton.onclick = async () => {
        indicators = [];

        allIndicators.forEach(indicator => {
            const checkbox = document.getElementById(indicator["datasetName"] + 'checkbox');
            if (checkbox.checked) {
                const newIndicator = JSON.parse(JSON.stringify(indicator));
                newIndicator["weight"] = 3;
                if (indicator["parameterLabel"]) {
                    const parameterCheckbox = document.getElementById(indicator["datasetName"] + 'parameterCheckbox');
                    if (parameterCheckbox.checked) {
                        const parameter = document.getElementById(indicator["datasetName"] + 'parameter');
                        newIndicator["parameter"] = parseFloat(parameter.value);
                    } else {
                        newIndicator["parameter"] = -1;
                    }
                }
                indicators.push(newIndicator);
            }
        });
    };

    const editWeightsModal = document.getElementById('editWeightsModal');
    editWeightsModal.addEventListener('show.bs.modal', () => {
        let content = '<div class="spinner-border" role="status">';
        content += '<span class="visually-hidden">Loading...</span>';
        content += '</div>';                 
        editWeightsContent.innerHTML = content;
    });
    editWeightsModal.addEventListener('shown.bs.modal', setUpEditWeightsWindow);

    const editWeightsSubmit = document.getElementById('editWeightsSubmit');
    editWeightsSubmit.onclick = async () => {
        indicators.forEach(indicator => {
            indicator["weight"] = document.getElementById(indicator["datasetName"] + 'weight').value;
        });

        await fetchHeatmap();
        if (isWalkabilityDisplayed()) {
            displayHeatmap();
        }
    };

    const saveIndicatorsModal = bootstrap.Modal.getOrCreateInstance(document.getElementById("saveIndicatorsModal"));
    const saveIndicatorsName = document.getElementById('saveIndicatorsName');
    const saveIndicatorsSave = document.getElementById('saveIndicatorsSave');
    saveIndicatorsSave.addEventListener('click', () => {
        if (saveIndicatorsName.value != "") {
            window.localStorage.setItem(saveIndicatorsName.value, JSON.stringify(indicators));
            saveIndicatorsName.value = "";
            saveIndicatorsModal.hide();
        }
    });

    const loadIndicatorsModal = document.getElementById("loadIndicatorsModal");
    const loadIndicatorsModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById("loadIndicatorsModal"));
    const loadIndicatorsSaveList = document.getElementById('loadIndicatorsSaveList');
    const loadIndicatorsLoad = document.getElementById('loadIndicatorsLoad');
    loadIndicatorsModal.addEventListener('show.bs.modal', () => {
        let content = '';
        Object.keys(window.localStorage).forEach(key => {
            if (!key.includes("mapbox")) {
                content += '<option>' + key + '</option>';  
            }
        });    
        loadIndicatorsSaveList.innerHTML = content;
    });
    loadIndicatorsLoad.addEventListener('click', async () => {
        if (loadIndicatorsSaveList.value != "") {
            indicators = JSON.parse(window.localStorage.getItem(loadIndicatorsSaveList.value));
            loadIndicatorsModalInstance.hide();

            await fetchHeatmap();
            if (isWalkabilityDisplayed()) {
                displayHeatmap();
            }
        }
    });
}

function setUpSelectIndicatorsWindow() {
    if (allIndicators != undefined) {
        let content = "<p>" + description + "</p>";

        const groups = [...new Set(allIndicators.map(indicator => {
            return indicator["group"];
        }))];

        groups.forEach(group => {
            content += '<h3>' + group + '</h3>';

            allIndicators.forEach(indicator => {
                if (indicator["group"] == group) {
                    let checked = '';
                    for (let i=0; i<indicators.length; i++) {
                        if (indicators[i]["datasetName"] == indicator["datasetName"]) {
                            checked = 'checked';
                        }
                    }
                    
                    content += '<div class="my-2">';
                    content += '    <div class="form-check">';
                    content += '        <input class="form-check-input" type="checkbox" value="" id="' + indicator["datasetName"] + 'checkbox" ' + checked + '>';
                    content += '        <label class="form-check-label fw-bold" for="' + indicator["datasetName"] + 'checkbox">' + indicator["name"] + '</label>';
                    content += '        <p>' + indicator["description"] + '</p>';
                    if (indicator["parameterLabel"]) {
                        let parameterValue = indicator["defaultParameter"];
                        let parameterChecked = "checked";
                        for (let i=0; i<indicators.length; i++) {
                            if (indicators[i]["datasetName"] == indicator["datasetName"] && indicators[i]["parameter"]) {
                                if (indicators[i]["parameter"] == -1) {
                                    parameterChecked = "";
                                } else {
                                    parameterValue = indicators[i]["parameter"];
                                }
                            }
                        }
                        content += '        <div class="m-0 d-flex justify-content-around">';
                        content += '            <label class="form-label" for="' + indicator["datasetName"] + 'parameterCheckbox">' + indicator["parameterLabel"] + '</label>';
                        content += '            <input class="form-check-input" type="checkbox" id="' + indicator["datasetName"] + 'parameterCheckbox" ' + parameterChecked + '>';
                        content += '        </div>';
                        content += '        <div class="m-0 d-flex justify-content-center">';
                        content += '            <input class="form-control text-center w-50" type="number" min="0" value="' + parameterValue + '" step="0.1" id="' + indicator["datasetName"] + 'parameter">';
                        content += '        </div>';
                    }
                    content += '    </div>';
                    content += '</div>';
                }
            });

            content += '<hr/>';
        });

        selectIndicatorsContent.innerHTML = content;

        allIndicators.forEach(indicator => {
            const parameterCheckbox = document.getElementById(indicator["datasetName"] + "parameterCheckbox");
            if (parameterCheckbox) {
                const parameter = document.getElementById(indicator["datasetName"] + "parameter");
                if (parameterCheckbox.checked) {
                    parameter.style.display = "block";
                } else {
                    parameter.style.display = "none";
                }

                parameterCheckbox.addEventListener('input', () => {
                    if (parameterCheckbox.checked) {
                        parameter.style.display = "block";
                    } else {
                        parameter.style.display = "none";
                    }
                });
            }
        });
    } else {
        setTimeout(setUpSelectIndicatorsWindow, 300);
    }
}

function setUpEditWeightsWindow() {
    if (indicators == undefined) {
        setTimeout(setUpEditWeightsWindow, 300);
    } else {
        let content = '<form>';
        content += '<div class="m-3 row">';
        content += '    <div class="col"></div>';
        content += '    <div class="col d-flex justify-content-between">';
        content += '        <p class="fw-bold">Low</p>';
        content += '        <p class="fw-bold">Medium</p>';
        content += '        <p class="fw-bold">High</p>';
        content += '    </div>';    
        content += '</div>';    
        indicators.forEach(indicator => {
            content += '<div class="m-3 row">';
            content += '    <label class="form-label col" for="' + indicator["datasetName"] + 'weight">' + indicator["name"].toString() + '</label>';
            content += '    <input class="form-range col" type="range" min="1" max="3" step="1" value="' + indicator["weight"] + '" id="' + indicator["datasetName"] + 'weight">';
            content += '</div>';                 
        });
        content += '</form>';
        editWeightsContent.innerHTML = content;
    }
}