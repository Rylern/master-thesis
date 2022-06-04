import json
from fastapi import APIRouter, HTTPException, Query
from pydantic import BaseModel
from typing import List
from sqlalchemy import text
from ..database.main import engine


class Category(BaseModel):
    name: str
    indicators: List[str]


router = APIRouter(prefix="/category")
    
    
@router.get("/")
async def getCategories():
    allCategories = []

    with engine.connect() as conn:
        resultCategory = conn.execute(text("SELECT * FROM category"))

        for categoryID, categoryName in resultCategory:
            resultIndicator = conn.execute(text(
                "SELECT indicator.name FROM indicator " +
                    "JOIN category_indicator ON indicator.id = category_indicator.indicator_id " +
                    "WHERE category_indicator.category_id = :categoryID"),
                {"categoryID": categoryID})
            response = resultIndicator.all()
            
            indicators = []
            for res in response:
                indicators.append(res[0])
                
            allCategories.append({
                "name": categoryName,
                "indicators": indicators
            })

    return json.dumps({
        "categories": allCategories
    })



@router.post("/")
async def addCategory(category: Category):
    if (category.name != ""):
        with engine.connect() as conn:
            categoryResult = conn.execute(text("INSERT INTO category(name) VALUES (:name) RETURNING id"), {"name": category.name})
            categoryID = categoryResult.all()[0][0]

            for indicator in category.indicators:
                conn.execute(text("INSERT INTO category_indicator(category_id, indicator_id) VALUES ("
                    + ":categoryID,"
                    + "(SELECT id FROM indicator WHERE name=:indicator LIMIT 1))"),
                    {"categoryID": categoryID, "indicator": indicator})
    else:
        raise HTTPException(status_code=400, detail="Category name empty")

    return json.dumps({
        "status": "sucess"
    })

@router.delete("/")
async def deleteCategory(categories: List[str] = Query(None)):
    if categories == None:
        raise HTTPException(status_code=404, detail="No category given")
    else:
        with engine.connect() as conn:
            for category in categories:
                categoryResult = conn.execute(text("SELECT id FROM category WHERE name=:name"), {"name": category}).all()

                if (len(categoryResult) > 0):
                    categoryID = categoryResult[0][0]

                    indicatorsIds = conn.execute(text("DELETE FROM category_indicator WHERE category_id=:categoryID RETURNING indicator_id"), {"categoryID": categoryID}).all()
                    for indicatorId, in indicatorsIds:
                        indicatorResponse = conn.execute(text("SELECT name FROM indicator WHERE id=:indicatorId"), {"indicatorId": indicatorId}).all()
                        for indicatorName, in indicatorResponse:
                            conn.execute(text('DROP TABLE "' + indicatorName + '";'))

                        conn.execute(text("DELETE FROM indicator WHERE id=:indicatorId"), {"indicatorId": indicatorId})

                    conn.execute(text("DELETE FROM category WHERE id=:categoryID"), {"categoryID": categoryID})

                else:
                    raise HTTPException(status_code=404, detail="Category not found")

        return json.dumps({
            "status": "sucess"
        })