import redis
import os
from dotenv import load_dotenv

# Cargamos el archivo .env invisible
load_dotenv()

# Leemos la variable de entorno de forma segura
URI = os.getenv("REDIS_URL")
r = redis.Redis.from_url(URI, decode_responses=True)

# 2. La palabra que estamos buscando
busqueda = "Arroz"

# ... resto de tu código ...