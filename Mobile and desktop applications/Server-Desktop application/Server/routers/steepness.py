from fastapi import APIRouter
import json
from qgis.core import QgsJsonExporter
from indicators.walkability import getSteepnessLayer


router = APIRouter(prefix="/steepness")


@router.get("/")
def getSteepness():
    layer = getSteepnessLayer()
    return json.loads(QgsJsonExporter(layer).exportFeatures(layer.getFeatures()))