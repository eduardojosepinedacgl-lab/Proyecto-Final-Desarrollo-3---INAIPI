# INAIPI WEB v2

Aplicación Node.js sin dependencias npm externas, con dos portales responsivos en español. Basada en el repositorio `Proyecto-Final-Desarrollo-3---INAIPI`, rama `main`, commit `9555c17`.

Para instalar, lee `INSTRUCCIONES-WEB-v2.md` en la raíz de esta entrega. Se necesita Node.js 22.19+; se recomienda Node.js 24 LTS. El CORE real es ASP.NET Web API de .NET Framework, ejecutado en Windows con Visual Studio/IIS Express y SQL Server.

## Arranque

```powershell
# Dentro de WEB/INAIPI.WEB:
npm run setup
# Ejecutar el SQL 08, recompilar y arrancar el CORE.
npm start
```

No requiere `npm install`. No uses Live Server. `npm run demo` abre una demostración independiente en el puerto 3001; `npm test` ejecuta las pruebas automáticas con un CORE simulado.

| Área | URL local | Identidad |
| --- | --- | --- |
| Personal | http://127.0.0.1:3000 | Usuarios existentes del CORE |
| Familias / clientes | http://127.0.0.1:3000/familias | Cuenta propia creada con correo y contraseña |

## Flujos disponibles

**Personal:** autenticar con el CORE, consultar el catálogo de centros/beneficiarios, crear beneficiarios, editar con versión optimista, registrar atenciones realizadas e importes, asociar una referencia fiscal, revisar/rechazar solicitudes de clientes, ver pendientes y errores, reintentar y exportar reportes.

**Familias:** crear cuenta e iniciar sesión, solicitar una inscripción, seguir su estado y descargar una constancia imprimible desde el momento de recepción. Cuando la inscripción está confirmada en CORE, aparece en «Mis niños y niñas» y permite solicitar atención. El personal registra la atención una vez realizada. La fecha preferida no reserva una cita.

Las familias únicamente ven las solicitudes de su propia cuenta. No se permite reclamar registros antiguos por conocer el documento de un tutor. Las cuentas familiares y solicitudes se almacenan en la WEB; las inscripciones y atenciones aprobadas se guardan en CORE bajo el usuario revisor.

El catálogo público solo expone información de centros. Se alimenta del catálogo obtenido por el personal; el administrador debe actualizarlo al instalar. No publica beneficiarios, tutores ni solicitudes.

## Persistencia e idempotencia

`data/inaipi-web.sqlite` guarda sesiones, catálogo por usuario, cuentas familiares, solicitudes, operaciones y eventos. Usa SQLite con WAL, transacciones `BEGIN IMMEDIATE`, `synchronous=FULL` y claves únicas. Las contraseñas familiares se guardan con scrypt y sal aleatoria; los identificadores de sesión se guardan hasheados.

Cada operación recibe un UUID antes de enviarse. La API WEB la guarda antes de responder. Un reintento con el mismo UUID y datos devuelve el mismo registro; cambiar los datos con ese UUID produce conflicto. La revisión de una solicitud y la inserción de su operación se realizan en la misma transacción SQLite.

El trabajador toma operaciones con una concesión temporal, envía una a la vez y conserva el resultado. Reintenta errores de red, 404 de instalación, 408, 429 y errores 5xx con espera creciente, hasta cinco minutos. Un 4xx de validación/permisos/conflicto requiere revisión. Después de reiniciar, los trabajos en curso se recuperan al vencer la concesión.

El nuevo procedimiento `dbo.ppWebProcesarOperacion` guarda el cambio de negocio y `tblWebOperaciones` en una única transacción SQL. Un bloqueo de aplicación por UUID impide dobles ejecuciones concurrentes. Si el CORE guarda pero se pierde la respuesta, el siguiente envío obtiene el mismo ID confirmado. Los cambios de beneficiario conservan `Version`; un conflicto no sobrescribe cambios ajenos.

| Estado | Significado |
| --- | --- |
| Pendiente de revisión | Solicitud familiar recibida, sin aprobación del centro |
| Rechazada | El personal rechazó la solicitud y dejó una respuesta |
| Pendiente de envío | Operación guardada localmente, todavía sin confirmación CORE |
| Enviando | El trabajador está esperando al CORE |
| Confirmada | El CORE devolvió un recibo válido con el mismo UUID |
| Requiere revisión | El CORE rechazó datos, permisos o versión; no se oculta el error |

El funcionamiento sin CORE requiere una sesión de personal vigente y un catálogo previamente obtenido. El primer inicio de sesión del personal requiere CORE disponible. Las familias pueden autenticar y consultar sus solicitudes mientras el servidor WEB está encendido, aunque CORE esté caído. No se implementa un service worker ni guardado cuando el navegador está desconectado de la WEB. Un fallo de envío conserva el formulario en memoria para repetir el mismo UUID; no cierres ni recargues esa pestaña hasta recibir confirmación.

## Integración con el repositorio

La Integración actual tiene un canal `/api/caja/sincronizar` con cola SQL para lotes de atenciones de CAJA. No incluye inscripciones, versiones de beneficiarios ni referencias fiscales. Esta WEB usa una extensión **directa WEB → CORE** para sus tres procesos, con su propia cola persistente. No altera la cola ni el contrato de CAJA/Integración.

La extensión añade:

| Archivo / elemento | Propósito |
| --- | --- |
| `Controllers/WebSyncController.cs` | API privada de catálogo y operaciones WEB |
| `08_Web_Persistencia.sql` | Tablas de recibos y referencias fiscales; procedimiento transaccional |
| `WebApiKey` en `Web.config` | Clave compartida únicamente entre servidores |
| Una entrada `Compile` en el `.csproj` | Incluye el controlador en el proyecto clásico |

`npm run setup` crea `.env` si falta, conserva una clave hexadecimal válida de 64 caracteres o genera una, la coloca en `Web.config` y agrega la entrada `Compile` una sola vez. No modifica `CnCoreDb`; antes de la primera modificación guarda `Web.config.antes-web-v2`.

| Endpoint CORE | Contrato |
| --- | --- |
| `POST api/seguridad/login` | Contrato original: `Username`, `PasswordHash` SHA-256 UTF-8 hexadecimal |
| `GET api/web/catalogo?usuarioId=...` | `{centros:[], beneficiarios:[]}`, filtrado por usuario activo y centro |
| `POST api/web/operaciones` | `{TransaccionGuid, Tipo, UsuarioId, Datos}` |

Los endpoints `api/web` requieren `X-Web-Key`. El catálogo y procedimiento vuelven a comprobar usuario/rol activos y centro. Administrador puede gestionar todos los centros; los demás usuarios necesitan un centro asignado. La clave no se envía al navegador.

## Configuración

| Variable | Valor inicial / propósito |
| --- | --- |
| `CORE_API_URL` | `https://localhost:44333/`; raíz real del servicio, no la URL de WEB |
| `PORT` | `3000` |
| `HOST` | `127.0.0.1`; acceso local predeterminado |
| `WEB_API_KEY` | Generada por `setup`; debe coincidir con `WebApiKey` en CORE |
| `DATA_DIR` | `./data`, relativa a la carpeta WEB; también acepta ruta absoluta |
| `SYNC_INTERVAL_MS` | `5000` |
| `CORE_TIMEOUT_MS` | `8000` |
| `SESSION_HOURS` | `12`, duración fija de sesión |
| `COOKIE_SECURE` | `false` para HTTP local; `true` cuando WEB se sirve por HTTPS |
| `NODE_EXTRA_CA_CERTS` | Opcional: certificado CA en PEM si no está en el almacén del sistema |

`npm start` usa el almacén de certificados del sistema mediante `--use-system-ca`. Debes confiar previamente en el certificado de desarrollo de IIS Express. Nunca se desactiva la validación TLS.

## Reportes y referencias fiscales

- Constancia individual de cada inscripción, edición y atención: datos, estado actual, responsable, recibo CORE e historial completo.
- Constancia de cada solicitud familiar: datos propios, estado y respuesta del centro, más importes/referencia fiscal cuando se registre la atención.
- Resumen imprimible con totales de operaciones **confirmadas**. El navegador permite guardar PDF.
- CSV de operaciones y referencias fiscales con filtro de fecha de recepción WEB (hora RD), estado y tipo.
- CSV de todas las solicitudes y CSV de los últimos 2,000 eventos de auditoría; no aplican los filtros del resumen.

Los reportes de operaciones/atenciones abarcan lo registrado desde esta instalación WEB, no el historial completo de CAJA u otras aplicaciones. El catálogo de beneficiarios sí viene del CORE.

Los importes se conservan como centavos enteros en WEB y como `decimal(18,2)` en CORE. La combinación RNC emisor + NCF es única en ambas capas. Se acepta el formato B01/B02/B14/B15 con ocho dígitos de secuencia; la validación de formato no demuestra autorización de la DGII. El NCF debe estar previamente autorizado y asignado por el responsable fiscal. No hay emisión electrónica e-CF, consulta DGII ni cobro/pasarela de pagos. Referencia oficial de estructura: https://dgii.gov.do/cicloContribuyente/facturacion/comprobantesFiscales/Paginas/tiposComprobantes.aspx

Los textos se escapan en HTML y las celdas CSV neutralizan prefijos que podrían interpretarse como fórmulas.

## Respaldo y operación

La cola y las cuentas no viven en el navegador. Conserva `data` al actualizar. Para una copia consistente sencilla: detén WEB con Ctrl+C, copia **toda** la carpeta `data` y `.env` a un respaldo protegido, y vuelve a arrancar. Respalda también `INAIPI_Core` con SQL Server. No copies solo el archivo SQLite con el proceso encendido: WAL puede contener datos recientes.

Después de recuperar un respaldo, mantén la misma base CORE y UUID existentes; los recibos evitan repetir operaciones ya confirmadas. Un respaldo antiguo puede no incluir operaciones posteriores: la idempotencia no recupera datos que nunca estuvieron en ese respaldo.

Los archivos de datos y configuración no son rutas públicas. Hay cookies HttpOnly/SameSite, CSRF, comprobación de origen, límites de cuerpo e intentos de autenticación, y CSP. La versión se entrega para ejecución local. Los endpoints originales del CORE conservan su seguridad previa y no se endurecen aquí: mantén CORE restringido al equipo/red de confianza. Una publicación para clientes reales necesita HTTPS, acceso de red configurado, gestión de usuarios y respaldos. No incluye recuperación de contraseña por correo, verificación de correo ni envío de notificaciones.

## Archivos

- `server.js`: servidor HTTP, sesiones, autorización, APIs y exportación.
- `lib/`: validación, SQLite, trabajador CORE y reportes.
- `public/`: interfaz del personal y portal familiar, estilos e impresión.
- `scripts/setup.mjs`: preparación de configuración y proyecto CORE.
- `scripts/demo.mjs`: entorno de demostración aislado.
- `test/`: CORE simulado y pruebas automáticas.
- `PRUEBAS.md`: alcance verificado y comprobaciones Windows pendientes.
