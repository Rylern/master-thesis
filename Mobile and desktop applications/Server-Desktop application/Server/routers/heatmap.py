from fastapi import APIRouter, HTTPException
from typing import List
import json, os, datetime
from qgis.core import QgsJsonExporter
from models.indicator import Indicator
from indicators.walkability import getLayer



router = APIRouter(prefix="/heatmap")


@router.post("/")
async def getHeatmap(indicators: List[Indicator]):
    input = getLayer(indicators)
    return json.loads(QgsJsonExporter(input).exportFeatures(input.getFeatures()))

@router.post("/save")
async def saveHeatmap(indicators: List[Indicator]):
    with open(os.path.join("storage", "savedIndicators"), "a") as f:
        f.write(str(datetime.datetime.now()) + ',' + ','.join([str(x) for x in indicators]) + '\n')
    return {
        "status": "sucess"
    }