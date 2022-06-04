from fastapi import APIRouter, Request, templating


templates = templating.Jinja2Templates(directory="templates")
router = APIRouter()



@router.get("/")
async def read_item(request: Request):
    return templates.TemplateResponse("homepage.html", {"request": request})
