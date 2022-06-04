from fastapi import APIRouter
from indicators.walkability import getWalkabilityIndicators
import copy


router = APIRouter(prefix="/indicators")


@router.get("/")
async def getIndicators(language: str, advancedMode: bool = True):
    indicators = getWalkabilityIndicators(language)
    indicatorsFiltered = copy.deepcopy(indicators)
    if not(advancedMode):
        for indicator in indicators["indicators"]:
            if (indicator["advancedMode"]):
                indicatorsFiltered["indicators"].remove(indicator)
    return indicatorsFiltered