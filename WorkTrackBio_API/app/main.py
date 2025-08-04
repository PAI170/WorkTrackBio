# app/main.py
# -----------
from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy.orm import Session
from sqlalchemy import text
from app.database import get_db, test_connection
import os
from dotenv import load_dotenv

# Cargar variables de entorno
load_dotenv()

# Crear la aplicación FastAPI
app = FastAPI(
    title=os.getenv("PROJECT_NAME", "WTB API"),
    version="1.0.0",
    description="API para sistema de control de asistencia y proyectos"
)

# Evento que se ejecuta al iniciar la aplicación
@app.on_event("startup")
def startup_event():
    """Se ejecuta una vez cuando la aplicación inicia."""
    print("🚀 Iniciando WTB API...")
    # Probar la conexión a la base de datos al arrancar
    test_connection()

# Endpoint raíz
@app.get("/", tags=["General"])
def read_root():
    """Mensaje de bienvenida de la API."""
    return {
        "message": "¡Hola! WTB API está funcionando correctamente",
        "version": "1.0.0",
        "docs_url": "/docs"
    }

# Endpoint de chequeo de salud
@app.get("/health", tags=["General"])
def health_check():
    """Verifica el estado de la API y la conexión a la base de datos."""
    db_status = "connected" if test_connection() else "disconnected"
    return {
        "status": "OK",
        "database_status": db_status
    }

# Endpoint para probar la conexión y obtener datos
@app.get("/db-test", tags=["Database"])
def test_database_connection(db: Session = Depends(get_db)):
    """
    Endpoint de prueba que se conecta a la base de datos,
    ejecuta una consulta y devuelve el número de tablas.
    """
    try:
        # Usamos text() para indicarle a SQLAlchemy que esto es una consulta SQL literal.
        # Es la forma recomendada y más segura.
        query = text("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'")
        result = db.execute(query)
        
        # fetchone() obtiene la primera (y en este caso, única) fila del resultado.
        table_count = result.scalar_one()

        return {
            "status": "success",
            "message": "Conexión y consulta a la base de datos exitosas.",
            "database_name": db.get_bind().url.database,
            "tables_count": table_count
        }
    except Exception as e:
        # Si algo sale mal, devolvemos un error 500 (Internal Server Error)
        # Es una mejor práctica que devolver un 200 con un status de error.
        raise HTTPException(
            status_code=500,
            detail=f"Error de base de datos: {str(e)}"
        )

