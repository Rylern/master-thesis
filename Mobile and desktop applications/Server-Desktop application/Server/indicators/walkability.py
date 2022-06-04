from typing import List
import os
import json
from qgis.PyQt.QtCore import QVariant
from qgis.core import QgsVectorLayer, QgsExpression, QgsField, edit, QgsExpressionContext, QgsExpressionContextUtils
from qgis import processing

from models.indicator import Indicator


baseIndicators = {}
with open(os.path.join("storage", "indicators.json")) as f:
    baseIndicators = json.load(f)

indicatorNames = []
for indicator in baseIndicators["en"]["indicators"]:
    indicatorNames.append(indicator["datasetName"])



def getWeightsAndParameters(indicators: List[Indicator]):
    weightsAndParameters = {}
    for indicatorName in indicatorNames:
        weightsAndParameters[indicatorName] = {
            "weight": 0,
            "parameter": "None"
        }
    for indicator in indicators:
        if indicator.datasetName in indicatorNames:
            weightsAndParameters[indicator.datasetName] = {
                "weight": indicator.weight,
                "parameter": str(indicator.parameter)
            }

    totalWeight = 0
    for key in weightsAndParameters:
        totalWeight += weightsAndParameters[key]["weight"]

    if (totalWeight != 0):
        for key in weightsAndParameters:
            weightsAndParameters[key]["weight"] /= totalWeight
    
    return weightsAndParameters


def getLayer(indicators: List[Indicator]):
    rawLayer = QgsVectorLayer(os.path.join("storage", "indicators.gpkg|layername=indicators"), 'indicators', 'ogr')
    rawLayer.selectAll()
    layer = processing.run("native:saveselectedfeatures", {'INPUT': rawLayer, 'OUTPUT': 'memory:'})['OUTPUT']
    rawLayer.removeSelection()

    layer.dataProvider().addAttributes([QgsField("indicator", QVariant.Double)])
    layer.updateFields()
    index = layer.fields().indexOf('indicator')

    expression = getExpression(indicators)
    context = QgsExpressionContext()
    context.appendScopes(QgsExpressionContextUtils.globalProjectLayerScopes(layer))

    with edit(layer):
        for feature in layer.getFeatures():
            context.setFeature(feature)
            feature[index] = expression.evaluate(context)
            layer.updateFeature(feature)

    return layer

def getExpression(indicators: List[Indicator]):
    weightsAndParameters = getWeightsAndParameters(indicators)

    expression = ""
    for indicator in baseIndicators["en"]["indicators"]:
        if weightsAndParameters[indicator["datasetName"]]["weight"] > 0:
            indicatorExpression = indicator["expression"]
            if ("parameter" in indicatorExpression):
                if (weightsAndParameters[indicator["datasetName"]]["parameter"] == "None"):
                    indicatorExpression = indicatorExpression.replace("parameter", indicator["defaultParameter"])
                elif (weightsAndParameters[indicator["datasetName"]]["parameter"] == "-1.0"):
                    indicatorExpression = indicatorExpression.replace("parameter", "maximum(" + indicator["datasetName"] + ")")
                else:
                    indicatorExpression = indicatorExpression.replace("parameter", weightsAndParameters[indicator["datasetName"]]["parameter"])

            expression += str(weightsAndParameters[indicator["datasetName"]]["weight"]) + " * (" + indicatorExpression + ") +"
    if len(expression) > 0:
        expression = expression[:-1]

    return QgsExpression(expression)

def getWalkabilityIndicators(language: str):
    try:
        return baseIndicators[language]
    except:
        return baseIndicators["en"]

def getSteepnessLayer():
    rawLayer = QgsVectorLayer(os.path.join("storage", "steepness.gpkg|layername=roads"), 'roads', 'ogr')
    rawLayer.selectAll()
    layer = processing.run("native:saveselectedfeatures", {'INPUT': rawLayer, 'OUTPUT': 'memory:'})['OUTPUT']
    rawLayer.removeSelection()
    return layer