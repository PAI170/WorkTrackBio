# app/database.py
# ----------------
"""
Configuración de la base de datos para FastAPI con SQLAlchemy y SQL Server
"""

import os
from dotenv import load_dotenv
from sqlalchemy import create_engine, text
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base

# Cargar variables de entorno desde el archivo .env
load_dotenv()

# Obtener la URL de la base de datos desde las variables de entorno
DATABASE_URL = os.getenv("DATABASE_URL")

# --- Verificación ---
# Es una buena práctica asegurarse de que la variable se cargó correctamente
if not DATABASE_URL:
    raise ValueError("No se encontró la variable de entorno DATABASE_URL. Asegúrate de que el archivo .env existe y está configurado.")

# Crear el engine de SQLAlchemy
# echo=True muestra las queries SQL en consola (útil para debugging)
engine = create_engine(
    DATABASE_URL,
    echo=True,  # Cambiar a False en producción para no llenar los logs
    pool_pre_ping=True,  # Verifica que la conexión esté viva antes de usarla
    pool_recycle=3600,   # Recicla la conexión cada hora para evitar timeouts
)

# Crear una clase SessionLocal configurada
# Cada instancia de SessionLocal será una nueva sesión de base de datos
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# Crear una clase Base
# Nuestros modelos ORM heredarán de esta clase
Base = declarative_base()

# Dependency para inyectar la sesión de la base de datos en los endpoints
def get_db():
    """
    Dependency de FastAPI que crea y proporciona una sesión de base de datos
    por cada petición y se asegura de cerrarla al final.
    """
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

# Función para probar la conexión al iniciar la app
def test_connection():
    """
    Prueba si la conexión a la base de datos es exitosa ejecutando
    una consulta simple.
    """
    try:
        # Usamos 'with' para asegurarnos de que la conexión se cierre
        with engine.connect() as connection:
            # Es mejor usar text() para que SQLAlchemy trate la query de forma segura
            connection.execute(text("SELECT 1"))
        print("✅ Conexión a la base de datos exitosa.")
        return True
    except Exception as e:
        print(f"❌ Error al conectar con la base de datos: {e}")
        # Imprimimos la URL que se está usando para facilitar el debug (sin la contraseña si la tuviera)
        print(f"URL de conexión utilizada: {DATABASE_URL}")
        return False
