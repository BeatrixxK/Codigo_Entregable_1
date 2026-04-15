const fs = require("fs");
const path = require("path");
const { createClient } = require("redis");

const REDIS_URL =
  "redis://default:SStOKh8Jr8dlzIndL0eVP37Wy8p4o5xj@redis-11674.c16.us-east-1-2.ec2.cloud.redislabs.com:11674";

const ARCHIVOS = [
  { archivo: "menus.json", prefijo: "menu" },
  { archivo: "usuarios.json", prefijo: "usuario" },
  { archivo: "productos.json", prefijo: "producto" },
];

function convertirAHash(obj) {
  const hash = {};

  for (const [clave, valor] of Object.entries(obj)) {
    if (valor === null || valor === undefined) {
      hash[clave] = "";
    } else if (typeof valor === "object") {
      hash[clave] = JSON.stringify(valor);
    } else {
      hash[clave] = String(valor);
    }
  }

  return hash;
}

async function migrarArchivo(client, rutaArchivo, prefijo) {
  if (!fs.existsSync(rutaArchivo)) {
    console.log(`Archivo no encontrado: ${rutaArchivo}`);
    return;
  }

  const contenido = fs.readFileSync(rutaArchivo, "utf8");
  const data = JSON.parse(contenido);

  if (!Array.isArray(data)) {
    console.log(`El archivo ${rutaArchivo} no contiene un arreglo JSON.`);
    return;
  }

  let total = 0;

  for (let i = 0; i < data.length; i++) {
    const item = data[i];

    const id = item.id ?? item.codigo ?? item.uuid ?? i + 1;
    const key = `${prefijo}:${id}`;
    const hash = convertirAHash(item);

    await client.hSet(key, hash);
    total++;
  }

  console.log(`Migrados ${total} registros desde ${path.basename(rutaArchivo)} con prefijo ${prefijo}:`);
}

async function main() {
  const client = createClient({
    url: REDIS_URL,
  });

  client.on("error", (err) => {
    console.error("Error Redis:", err);
  });

  await client.connect();
  console.log("Conectado a Redis.");

  for (const item of ARCHIVOS) {
    const ruta = path.join(__dirname, "..", "DragonNutrex.App", "Data", item.archivo);
    await migrarArchivo(client, ruta, item.prefijo);
  }

  await client.quit();
  console.log("Migración completada.");
}

main().catch((err) => {
  console.error("Error durante la migración:", err);
});