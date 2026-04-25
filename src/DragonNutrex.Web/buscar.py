import redis

# 1. Tu conexión a la nube
URI = 'redis://default:SStOKh8Jr8dlzIndL0eVP37Wy8p4o5xj@redis-11674.c16.us-east-1-2.ec2.cloud.redislabs.com:11674'
r = redis.Redis.from_url(URI, decode_responses=True)

# 2. La palabra que estamos buscando
busqueda = "Arroz"

print(f"🔍 Buscando '{busqueda}' en la base de datos...")

# 3. Escaneamos los productos
llaves = r.keys('Producto:*') + r.keys('producto:*')
encontrado = False

for llave in llaves:
    # Leemos el nombre del producto
    nombre = r.hget(llave, 'Nombre') or r.hget(llave, 'NombreProducto') or ""
    
    # Comparamos ignorando mayúsculas
    if busqueda.lower() in nombre.lower():
        encontrado = True
        print(f"\n✅ ¡Encontrado! La llave es: {llave}")
        print("-" * 30)
        
        # Traemos todos sus datos para que los veas
        datos = r.hgetall(llave)
        for campo, valor in datos.items():
            print(f"   {campo}: {valor}")
        print("-" * 30)

if not encontrado:
    print(f"❌ No se encontró ningún producto que contenga la palabra '{busqueda}'.")