from fastapi import APIRouter, HTTPException
from typing import List
import json
from qgis.core import QgsJsonExporter
from indicators.qgis import processing
from models.indicator import Indicator
from indicators.walkability import getLayer


router = APIRouter(prefix="/navigation")



@router.post("/")
async def getNavigation(indicators: List[Indicator], fromLatitude: float, fromLongitude: float, toLatitude: float, toLongitude: float):
    try:
        input = getLayer(indicators)
    except:
        raise HTTPException(status_code=500, detail="Server error")

    startPoint = str(fromLongitude) + "," + str(fromLatitude) + " [EPSG:4326]"
    endPoint = str(toLongitude) + "," + str(toLatitude) + " [EPSG:4326]"

    params = {
        'INPUT': input,
        'STRATEGY': 1,
        'START_POINT': startPoint,
        'END_POINT': endPoint,
        'SPEED_FIELD': 'indicator',
        'OUTPUT': 'TEMPORARY_OUTPUT'
    }
    try:
        output = processing.run("native:shortestpathpointtopoint", params)['OUTPUT']
        return json.loads(QgsJsonExporter(output).exportFeatures(output.getFeatures()))
    except:
        raise HTTPException(status_code=404, detail="Route not found")