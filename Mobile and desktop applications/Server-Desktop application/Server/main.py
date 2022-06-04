from fastapi import FastAPI, staticfiles
from routers import heatmap, navigation, indicators, steepness, homepage
from indicators import qgis


app = FastAPI()
app.mount("/static", staticfiles.StaticFiles(directory="static"), name="static")

app.include_router(heatmap.router)
app.include_router(navigation.router)
app.include_router(indicators.router)
app.include_router(steepness.router)
app.include_router(homepage.router)


@app.on_event("shutdown")
def app_shutdown():
    qgis.onShutdown()