# create_structure.py
# Script para crear la estructura de carpetas del proyecto FastAPI

import os

def create_directory_structure():
    """Crea la estructura completa de carpetas para el proyecto FastAPI"""
    
    # Estructura de carpetas
    folders = [
        "app",
        "app/models",
        "app/schemas", 
        "app/routers",
        "app/services",
        "app/utils"
    ]
    
    # Crear carpetas
    for folder in folders:
        os.makedirs(folder, exist_ok=True)
        print(f"✅ Carpeta creada: {folder}")
    
    # Crear archivos __init__.py (necesarios en Python)
    init_files = [
        "app/__init__.py",
        "app/models/__init__.py", 
        "app/schemas/__init__.py",
        "app/routers/__init__.py",
        "app/services/__init__.py",
        "app/utils/__init__.py"
    ]
    
    # Crear archivos principales vacíos
    main_files = [
        "app/main.py",
        "app/database.py",
        "app/models/user.py",
        "app/models/employee.py", 
        "app/models/project.py",
        "app/models/assistance.py",
        "app/schemas/user.py",
        "app/schemas/employee.py",
        "app/schemas/project.py", 
        "app/schemas/assistance.py",
        "app/routers/auth.py",
        "app/routers/employees.py",
        "app/routers/projects.py",
        "app/routers/assistance.py",
        "app/services/auth_service.py",
        "app/services/employee_service.py",
        "app/utils/security.py",
        "app/utils/dependencies.py",
        ".env",
        "requirements.txt",
        "README.md"
    ]
    
    # Crear archivos __init__.py
    for init_file in init_files:
        with open(init_file, 'w') as f:
            f.write('# __init__.py\n')
        print(f"✅ Archivo creado: {init_file}")
    
    # Crear archivos principales
    for main_file in main_files:
        if not os.path.exists(main_file):
            with open(main_file, 'w') as f:
                if main_file.endswith('.py'):
                    f.write(f'# {os.path.basename(main_file)}\n')
                elif main_file == '.env':
                    f.write('# Variables de entorno\n')
                elif main_file == 'README.md':
                    f.write('# WTB API\n\nAPI para sistema de control de asistencia\n')
                else:
                    f.write('')
            print(f"✅ Archivo creado: {main_file}")
    
    print("\n🎉 ¡Estructura de proyecto creada exitosamente!")
    print("\n📁 Estructura final:")
    print("""
wtb_api/
├── app/
│   ├── __init__.py
│   ├── main.py
│   ├── database.py
│   ├── models/
│   │   ├── __init__.py
│   │   ├── user.py
│   │   ├── employee.py
│   │   ├── project.py
│   │   └── assistance.py
│   ├── schemas/
│   │   ├── __init__.py
│   │   ├── user.py
│   │   ├── employee.py
│   │   ├── project.py
│   │   └── assistance.py
│   ├── routers/
│   │   ├── __init__.py
│   │   ├── auth.py
│   │   ├── employees.py
│   │   ├── projects.py
│   │   └── assistance.py
│   ├── services/
│   │   ├── __init__.py
│   │   ├── auth_service.py
│   │   └── employee_service.py
│   └── utils/
│       ├── __init__.py
│       ├── security.py
│       └── dependencies.py
├── .env
├── requirements.txt
└── README.md
    """)

if __name__ == "__main__":
    create_directory_structure()