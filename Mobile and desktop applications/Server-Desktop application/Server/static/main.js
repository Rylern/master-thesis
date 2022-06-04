import * as Map from './map.js';
import * as Indicators from './indicators.js';
import * as Steepness from './steepness.js';
import * as Navigate from './navigate.js';
import * as Legend from './legend.js';


init();


async function init() {
    Map.init();
    await Indicators.init();
    Steepness.init();
    Navigate.init();
    Legend.init();
};