import json
from fastapi import APIRouter, Query
from sqlalchemy import text
from typing import List
from fastapi import File, UploadFile, HTTPException
import geopandas as gpd
from ..database.main import engine
from ..indicators.indicatorComputation import computeCategories

    
router = APIRouter(prefix="/indicator")
    
    
@router.get("/")
async def getIndicators():
    allIndicators = []

    with engine.connect() as conn:
        resultIndicator = conn.execute(text("SELECT name FROM indicator"))
        for name, in resultIndicator:
            allIndicators.append(name)

    return json.dumps({
        "indicators": allIndicators
    })



@router.get("/compute")
async def getCategoryValues(startingLongitude: float, startingLatitude: float, endingLongitude: float, endingLatitude: float, indicators: List[str] = Query(None), weights: List[float] = Query(None), tileSize: float = 0.005):
    coordinates = [(startingLongitude, startingLatitude), (endingLongitude, startingLatitude), (endingLongitude, endingLatitude), (startingLongitude, endingLatitude)]
    return computeCategories(coordinates, indicators, weights, tileSize)


@router.post("/")
async def createIndicator(file: UploadFile = File(...)):
    try:
        data = gpd.read_file(file.file)

        for column in data.columns:
            if (column != 'geometry'):
                indicators = data.copy()
                for otherColumn in indicators.columns:
                    if (otherColumn != 'geometry' and otherColumn != column):
                        del indicators[otherColumn]
                indicators.to_postgis(column, engine, if_exists='append')
                with engine.connect() as conn:
                    conn.execute(text("INSERT INTO indicator(name) VALUES (:name)"), {"name": column})

        return json.dumps({
            "status": "success"
        })
    except:
        raise HTTPException(status_code=400, detail="Bad file format")