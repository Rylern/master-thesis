import { displayNavigation, hideNavigation } from './map.js';


const navigateErrorMessage = document.getElementById("navigateErrorMessage");
const stopNavigation = document.getElementById("stopNavigation");
const navigationSpinner = document.getElementById("navigationSpinner");


export function init() {
    const fromLocationInput = document.getElementById("fromLocation");
    fromLocationInput.addEventListener("input", () => {
        fromLocationInput.classList.remove("is-invalid");
    });

    const toLocationInput = document.getElementById("toLocation");
    toLocationInput.addEventListener("input", () => {
        toLocationInput.classList.remove("is-invalid");
    });

    document.getElementById("fromLocationGetCurrent").onclick = () => {
        getCurrentLocation(fromLocationInput);
    };
    document.getElementById("toLocationGetCurrent").onclick = () => {
        getCurrentLocation(toLocationInput);
    };

    window.addEventListener('click', () => {
        navigateErrorMessage.style.display = "none";
    });

    const navigateModal = bootstrap.Modal.getOrCreateInstance(document.getElementById("navigateModal"));
    document.getElementById("submitNavigate").onclick = async () => {
        const urlFrom = 'https://api.mapbox.com/geocoding/v5/mapbox.places/' + fromLocationInput.value + '.json?limit=1&access_token=' + mapboxgl.accessToken;
        const urlTo = 'https://api.mapbox.com/geocoding/v5/mapbox.places/' + toLocationInput.value + '.json?limit=1&access_token=' + mapboxgl.accessToken;
        const responseFrom = await fetch(urlFrom);
        const responseTo = await fetch(urlTo);
        const responseJSONFrom = await responseFrom.json();
        const responseJSONTo = await responseTo.json();
        const featuresFrom = responseJSONFrom["features"];
        const featuresTo = responseJSONTo["features"];
        if (featuresFrom.length == 0) {
            fromLocationInput.classList.add("is-invalid");
        } else if (featuresTo.length == 0) {
            toLocationInput.classList.add("is-invalid");
        } else {
            const fromLocation = featuresFrom[0]["center"];
            const toLocation = featuresTo[0]["center"];

            navigationSpinner.classList.remove("d-none");
            await displayNavigation(fromLocation, toLocation);
            navigationSpinner.classList.add("d-none");
            
            navigateModal.hide();
            stopNavigation.classList.remove("disabled");
        }
    }

    stopNavigation.onclick = () => {
        hideNavigation();
        stopNavigation.classList.add("disabled");
    };

    navigationSpinner.classList.add("d-none");
}

function getCurrentLocation(input) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(async position => {
            const url = 'https://api.mapbox.com/geocoding/v5/mapbox.places/' + position.coords.longitude + ',' + position.coords.latitude + '.json?limit=1&access_token=' + mapboxgl.accessToken;
            const response = await fetch(url);
            const responseJSON = await response.json();
            const features = responseJSON["features"];
            if (features.length == 0) {
                navigateErrorMessage.innerHTML = "No place found from the current location.";
                navigateErrorMessage.style.display = "block";
            } else {
                input.value = features[0]["place_name"];
            }
        }, error => {
            let errorMessage = "";
            switch(error.code) {
                case error.PERMISSION_DENIED:
                    errorMessage = "User denied the request for Geolocation.";
                    break;
                case error.POSITION_UNAVAILABLE:
                    errorMessage = "Location information is unavailable.";
                    break;
                case error.TIMEOUT:
                    errorMessage = "The request to get user location timed out.";
                    break;
                case error.UNKNOWN_ERROR:
                    errorMessage = "An unknown error occurred.";
                    break;
            }
            navigateErrorMessage.innerHTML = errorMessage;
            navigateErrorMessage.style.display = "block";
        });
    } else {
        navigateErrorMessage.innerHTML = "Geolocation is not supported by this browser.";
        navigateErrorMessage.style.display = "block";
    }
}