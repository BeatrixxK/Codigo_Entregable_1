local nombres = {
  'Andrea','Roberto','Carlos','Valeria','Daniel','Sofia',
  'Javier','Lucia','Fernando','Camila','Mateo','Isabella',
  'Diego','Mariana','Alejandro','Gabriela','Sebastian','Paula',
  'Ricardo','Daniela','Esteban','Natalia','Hector','Elena',
  'Martin','Veronica','Adrian','Carolina','Luis','Patricia'
}

local pesos = {62,75,80,58,90,55,78,60,85,57,73,59,88,61,82,56,79,63,84,54,77,65,86,53,81,64,89,52,76,66}
local alturas = {162,175,180,160,185,158,178,163,182,159,176,161,184,164,179,157,177,165,183,156,174,166,181,155,173,167,186,154,172,168}
local actividades = {'Ligera','Moderada','Alta'}
local objetivos = {'Bajar peso','Mantener peso','Subir masa muscular'}
local dietas = {'Vegana','Keto','Mediterranea','Vegetariana','Balanceada'}

local viejas = redis.call('KEYS', 'usuario:*')
for _, key in ipairs(viejas) do
  redis.call('DEL', key)
end

local emails = redis.call('KEYS', 'usuario:email:*')
for _, key in ipairs(emails) do
  redis.call('DEL', key)
end

for i, nombre in ipairs(nombres) do
  local id = redis.sha1hex(nombre .. tostring(i) .. tostring(redis.call('TIME')[1]))
  local key = 'usuario:' .. id
  local email = string.lower(nombre) .. '@' .. string.lower(nombre) .. '.com'

  redis.call('HSET', key,
    'Id', id,
    'Nombre', nombre,
    'Email', email,
    'Peso', pesos[i],
    'Altura', alturas[i],
    'Actividad', actividades[((i - 1) % #actividades) + 1],
    'Objetivo', objetivos[((i - 1) % #objetivos) + 1],
    'TipoDieta', dietas[((i - 1) % #dietas) + 1],
    'Password', 'Upi.2025'
  )

  redis.call('SET', 'usuario:email:' .. email, id)
end

return '30 usuarios creados'
