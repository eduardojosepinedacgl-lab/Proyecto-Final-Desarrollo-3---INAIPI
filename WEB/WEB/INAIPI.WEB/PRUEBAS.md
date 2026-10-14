# Verificación de INAIPI WEB v2

Fecha: 14 de octubre de 2026. Base del repositorio: `9555c17`. Datos de pruebas ficticios.

## Pruebas automáticas reproducibles

Desde `WEB/INAIPI.WEB`:

```powershell
npm test
```

Resultado de esta entrega: **8 pruebas aprobadas**, con Node.js 24.19.0 y un CORE HTTP simulado. No requieren SQL Server ni escriben en la base real. Cada prueba utiliza una carpeta temporal independiente.

| Prueba | Verificación |
| --- | --- |
| Cola persistente | Guardado sin CORE, reinicio de WEB, recuperación de sesión/operación y confirmación posterior |
| Respuesta perdida | CORE simulado guarda y corta la respuesta; un reintento recupera el recibo sin duplicar el beneficiario |
| Concurrencia | Versión antigua produce revisión; la nueva edición confirma y su repetición exacta funciona después de refrescar el catálogo |
| Seguridad | Sesión, CSRF, origen, aislamiento por centro, reportes y bloqueo de rutas de datos/configuración |
| Importes y reportes | Centavos enteros, NCF único, filtros, escape HTML y neutralización de fórmulas CSV |
| Portal familiar | Aislamiento entre clientes, inscripción, aprobación, caída de CORE, reinicio, atención, rechazo y constancias propias |
| Trabajador | Dos conexiones SQLite no reclaman el mismo trabajo; una concesión vencida se recupera y un resultado viejo no sobrescribe el nuevo |
| Instalación | `setup` conserva la conexión a SQL, mantiene la misma clave y agrega el controlador una sola vez |

## Navegador

Se verificó con Chromium en escritorio (1440 px) y móvil (390 px):

1. Inicio de sesión del personal y publicación de centros.
2. Registro de una cuenta familiar y envío de inscripción.
3. Revisión y aprobación del personal.
4. Consulta del niño/niña confirmado en el portal familiar.
5. Solicitud de atención y registro de atención realizada con importes y NCF.
6. CORE simulado desconectado: operación pendiente visible y persistida.
7. Reconexión: confirmación y constancia con referencia fiscal.
8. Alta y edición de beneficiario desde el personal.
9. Reportes, navegación móvil y cierre de sesión.

Resultado: recorrido completado, sin errores JavaScript no controlados y sin desbordamiento horizontal de la página en móvil. Las tablas extensas usan su propio desplazamiento.

## Extensión CORE

- `WebSyncController.cs` compiló junto con el repositorio, el acceso a datos y los DTO utilizados, contra referencias de .NET Framework 4.7.2 y las dependencias existentes.
- El compilador emitió una advertencia `CS1701` por la identidad de referencia de `System.Net.Http` de Web API. Debe verificarse la resolución de ensamblados en Visual Studio/IIS con el proyecto completo.
- `08_Web_Persistencia.sql` se analizó con Microsoft SQL Server ScriptDom (parser T-SQL 160): **0 errores de sintaxis**.

Esto no equivale a ejecutar el procedimiento en SQL Server ni a una prueba real de IIS Express. Este entorno no dispone de tu instancia SQL, Visual Studio, certificado de Windows ni usuarios/base locales.

## Prueba final en Windows

Después de instalar la extensión conforme a `INSTRUCCIONES-WEB-v2.md`:

1. Recompila CORE en Visual Studio, arráncalo y ejecuta WEB con `npm start`.
2. Inicia sesión con el usuario real del CORE y actualiza el catálogo.
3. Usa un registro de prueba para crear una inscripción y verifica su confirmación tanto en WEB como en `tblBeneficiarios` y `tblWebOperaciones`.
4. Edita el beneficiario y comprueba el incremento de `Version`.
5. Crea una cuenta familiar, envía y aprueba una inscripción. Comprueba que otra cuenta no puede verla.
6. Detén CORE, registra una atención de prueba sin referencia fiscal real y confirma que WEB la conserva pendiente.
7. Detén y arranca WEB sin borrar `data`; la operación debe continuar pendiente.
8. Arranca CORE y comprueba una única atención y un único recibo por UUID.
9. Abre su constancia, guarda PDF y exporta CSV.

Para probar referencias fiscales utiliza una base aislada y datos de prueba; no consumas ni reutilices una secuencia fiscal real por una prueba técnica. Esta entrega registra referencias existentes; no solicita autorización fiscal ni realiza cobros.
