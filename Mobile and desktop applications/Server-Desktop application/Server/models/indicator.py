from pydantic import BaseModel
from typing import Optional

class Indicator(BaseModel):
    datasetName: str
    weight: float
    parameter: Optional[float]