import geopandas as gpd
from shapely.geometry import Polygon, Point
from pyproj import CRS
from sqlalchemy import inspect
from ..database.main import engine


def computeCategories(coordinates, indicators, weights, tileSize):
    tiles = getTiles(coordinates, tileSize)
    tiles.crs = CRS.from_epsg(4326)
    tiles = tiles.to_crs('epsg:3857')
    tiles['centroid'] = tiles.centroid
    tiles = tiles.to_crs('epsg:4326')
    tiles = tiles.set_geometry('centroid')
    tiles = tiles.to_crs('epsg:4326')

    tiles = retrieveIndicatorsFromDB(indicators, tiles)

    tiles = setGlobalIndicator(indicators, tiles, weights)

    for indicator in indicators:
        del tiles[indicator]

    tiles = tiles.set_geometry('geometry')
    del tiles['centroid']
    tiles["valueAvailable"] = tiles["globalIndicator"] == tiles["globalIndicator"]

    return tiles.to_json()


def getTiles(coordinates, tileSize):
    tiles = []

    startingLongitude = coordinates[0][0]
    startingLatitude = coordinates[0][1]
    endingLongitude = coordinates[1][0]
    endingLatitude = coordinates[2][1]

    latitudeRange = endingLatitude - startingLatitude
    longitudeRange = endingLongitude - startingLongitude
    latitudeStep = tileSize / 2
    longitudeStep = tileSize
    latitudeNumber = int(latitudeRange / latitudeStep)+1
    longitudeNumber = int(longitudeRange / longitudeStep)+1
    
    for i in range(latitudeNumber):
        for j in range(longitudeNumber):
            points = [
                Point(startingLongitude + j*longitudeStep - longitudeStep/2.1, startingLatitude + i*latitudeStep - latitudeStep/2.1),
                Point(startingLongitude + j*longitudeStep + longitudeStep/2.1, startingLatitude + i*latitudeStep - latitudeStep/2.1),
                Point(startingLongitude + j*longitudeStep + longitudeStep/2.1, startingLatitude + i*latitudeStep + latitudeStep/2.1),
                Point(startingLongitude + j*longitudeStep - longitudeStep/2.1, startingLatitude + i*latitudeStep + latitudeStep/2.1)
            ]
            tiles.append(Polygon(points))
    
    
    return gpd.GeoDataFrame(tiles, columns=['geometry'])


def retrieveIndicatorsFromDB(indicators, tiles):
    for indicator in indicators:
        if inspect(engine).has_table(indicator):
            sql = getSQLRequest(indicator, tiles)
            tilesFromDB = gpd.read_postgis(sql, engine, geom_col="geometry")
            tilesFromDB.crs = CRS.from_epsg(4326)
            
            indicatorTiles = gpd.sjoin(tiles, tilesFromDB, 'left', 'within')
            indicatorTiles = indicatorTiles[~indicatorTiles.index.duplicated()]
            tiles[indicator] = indicatorTiles[indicator]
            tiles[indicator] = tiles[indicator] / tiles[indicator].max()

    return tiles


def getSQLRequest(indicator, tiles):
    request = 'SELECT * FROM "' + indicator + '" WHERE '
    for _, val in tiles['centroid'].iteritems():
        request += "ST_Contains (geometry, ST_GeometryFromText('" + str(val) + "', 4326)) OR "
    request = request[:-4]
    request += ";"
    return request


def setGlobalIndicator(indicators, tiles, weights):
    totalWeight = 0
    for weight in weights:
        totalWeight += weight

    globalIndicators = []
    for _, row in tiles.iterrows():
        globalIndicator = 0
        for i in range(len(indicators)):
            globalIndicator += row[indicators[i]] * weights[i] / totalWeight
        globalIndicators.append(globalIndicator)
            
    tiles["globalIndicator"] = globalIndicators
    return tiles