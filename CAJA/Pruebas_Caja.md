Plan de Pruebas: Módulo de Caja (Contingencia y Sincronización)
1. Preparación del Entorno (Setup)

    Restauración de Paquetes: Al abrir el proyecto recién clonado, hacer clic derecho en la solución dentro de Visual Studio y seleccionar "Restore NuGet Packages" para que descargue Newtonsoft.Json, log4net y Microsoft.ReportingServices.

    Creación de Base de Datos: Abrir el archivo 01_CrearEsquemaLocal.sql ubicado en la carpeta DatabaseLocal, conectarse a la instancia (localdb)\MSSQLLocalDB y ejecutar el script completo para generar la base de datos INAIPI_CajaLocal y sus tablas.

    Estado Inicial (Offline): Verificar que en el archivo Services/ConexionSNDClient.cs, el método EnviarTransaccion esté retornando false para iniciar simulando una caída de red.

2. Validación de Contingencia y Reportes

    Captura de Cobro: Ejecutar la aplicación (F5), navegar al menú Transacciones > Cobro de Servicio y registrar un cobro de prueba llenando los tres campos requeridos.

    Notificación Visual: Al hacer clic en "Procesar Cobro", el sistema debe mostrar un mensaje advirtiendo que no hay red y que la transacción se guardó en contingencia (Outbox).

    Emisión del Ticket: Inmediatamente después de cerrar el mensaje de advertencia, debe abrirse automáticamente la ventana encapsulada con el comprobante de caja renderizado en PDF/vista previa con los datos exactos digitados.

3. Validación de Persistencia Local

    Consulta del Historial: Navegar al menú Transacciones > Historial de Transacciones.

    Estado Outbox: La grilla debe mostrar la transacción recién creada. La columna Sincronizado debe estar desmarcada, confirmando que el registro está atrapado en la base de datos local esperando red.

4. Validación del Sincronizador Background

    Simulación de Conexión: Con la aplicación detenida, ir al archivo Services/ConexionSNDClient.cs y cambiar el retorno de false a true.

    Sondeo Automático: Correr la aplicación y abrir la ventana del Historial de Transacciones. Observar la barra de estado inferior.

    Sincronización: Al cabo de máximo 10 segundos, la barra de estado debe cambiar a "ONLINE" color verde, y si se cierra y se vuelve a abrir el historial (o si se le agregó un botón de refrescar), la casilla de Sincronizado ahora debe aparecer marcada, confirmando que el subproceso vació la cola local.

5. Validación de Auditoría (Log4Net)

    Forzar Excepción: Insertar un throw new Exception("Prueba de auditoría"); temporal en el botón de cobro y ejecutar un intento de guardado.

    Escritura en Disco: Navegar a la ruta física del proyecto bin\Debug\Logs (esta carpeta se crea localmente al compilar, por lo que tu compañero tendrá que generar su propio error para verla). Abrir el archivo CajaOffline_Log.txt y verificar que el error haya quedado registrado con fecha, hilo y detalle técnico.
