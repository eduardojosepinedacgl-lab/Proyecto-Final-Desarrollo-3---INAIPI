export class WebError extends Error {
  constructor(status, message) { super(message); this.status = status; }
}
export const fail = (status, message) => { throw new WebError(status, message); };
export const uuid = value => typeof value === 'string' && /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(value);
export function integer(value, name = 'Identificador') {
  if (!Number.isSafeInteger(Number(value)) || Number(value) < 1 || Number(value) > 2147483647) fail(400, `${name} inválido.`);
  return Number(value);
}
export function required(value, max, name) {
  if (typeof value !== 'string' || !value.trim() || value.length > max) fail(400, `${name}: requerido, máximo ${max} caracteres.`);
  return value.trim();
}
export function day(value, name = 'Fecha') {
  if (typeof value !== 'string' || !/^\d{4}-\d{2}-\d{2}$/.test(value) || !Number.isFinite(Date.parse(value)) || new Date(value).toISOString().slice(0, 10) !== value) fail(400, `${name} inválida.`);
  return value;
}
export const today = () => new Intl.DateTimeFormat('sv-SE', { timeZone: 'America/Santo_Domingo' }).format(new Date());
export const isAdmin = user => user.NombreRol?.toLowerCase() === 'administrador';
export function scope(user) {
  if (isAdmin(user)) return null;
  return integer(user.CentroId, 'Centro asignado');
}
export function restrict(user, center) {
  const assigned = scope(user);
  if (assigned !== null && assigned !== Number(center)) fail(403, 'Solo puede operar en su centro asignado.');
}
export function enrollment(data, centers) {
  const p = {};
  for (const [key, max, name] of [['Nombres',100,'Nombres'],['Apellidos',100,'Apellidos'],['DocumentoTutor',20,'Documento del tutor'],['NombresTutor',100,'Nombres del tutor'],['ApellidosTutor',100,'Apellidos del tutor'],['TelefonoTutor',20,'Teléfono'],['DireccionTutor',250,'Dirección']]) p[key] = required(data[key], max, name);
  p.FechaNacimiento = day(data.FechaNacimiento, 'Fecha de nacimiento');
  if (p.FechaNacimiento > today() || p.FechaNacimiento < '1900-01-01') fail(400, 'Fecha de nacimiento fuera de rango.');
  p.CentroId = integer(data.CentroId, 'Centro');
  if (!centers.some(c => c.CentroId === p.CentroId)) fail(400, 'Seleccione un centro disponible.');
  p.FotografiaUrl = '';
  if (data.FotografiaUrl) {
    p.FotografiaUrl = required(data.FotografiaUrl,500,'Fotografía');
    try { if (!['http:', 'https:'].includes(new URL(p.FotografiaUrl).protocol)) throw new Error(); } catch { fail(400,'La fotografía debe ser una URL HTTP o HTTPS.'); }
  }
  return p;
}
export function fiscal(data) {
  if (!data) return null;
  const p = {};
  for (const [k,max] of [['Ncf',11],['RncEmisor',11],['Emisor',150],['Receptor',150],['DocumentoReceptor',11]]) p[k] = required(data[k],max,k);
  p.Ncf = p.Ncf.toUpperCase();
  if (!/^B(01|02|14|15)\d{8}$/.test(p.Ncf) || p.Ncf.slice(3) === '00000000') fail(400,'NCF inválido. Use B01, B02, B14 o B15 y ocho dígitos de una secuencia autorizada.');
  if (!/^(\d{9}|\d{11})$/.test(p.RncEmisor) || !/^(\d{9}|\d{11})$/.test(p.DocumentoReceptor)) fail(400,'RNC o cédula: 9 u 11 dígitos, sin guiones.');
  return p;
}
export function validate(kind, data, user, catalog) {
  if (!data || typeof data !== 'object' || Array.isArray(data)) fail(400,'Datos requeridos.');
  const center = integer(data.CentroId,'Centro'); restrict(user,center);
  if (!catalog?.centros.some(c => c.CentroId === center)) fail(400,'Actualice el catálogo y seleccione un centro disponible.');
  if (kind === 'beneficiario.crear') return enrollment(data,catalog.centros);
  const id = integer(data.BeneficiarioId,'Beneficiario');
  const b = catalog.beneficiarios.find(b => b.BeneficiarioId === id);
  if (!b) fail(404,'Beneficiario no disponible. Actualice el catálogo.');
  restrict(user,b.CentroId);
  if (kind === 'beneficiario.actualizar') {
    const p = enrollment({...data,DocumentoTutor:b.DocumentoTutor,NombresTutor:'Sin cambio',ApellidosTutor:'Sin cambio',TelefonoTutor:b.TelefonoTutor,DireccionTutor:b.DireccionTutor},catalog.centros);
    if (Number(data.Version) !== b.VersionRegistro) fail(409,'El registro cambió. Actualice el catálogo y vuelva a editarlo.');
    return {Nombres:p.Nombres,Apellidos:p.Apellidos,FechaNacimiento:p.FechaNacimiento,CentroId:center,FotografiaUrl:p.FotografiaUrl,BeneficiarioId:id,TutorId:b.TutorId,Version:integer(data.Version,'Versión')};
  }
  if (kind === 'atencion.registrar') {
    if (b.CentroId !== center) fail(400,'El beneficiario no pertenece al centro seleccionado.');
    const date = day(data.FechaRegistro,'Fecha de atención');
    if (date > today() || date < '2000-01-01') fail(400,'Fecha de atención fuera de rango.');
    for (const key of ['BaseCentavos','ItbisCentavos']) if (!Number.isSafeInteger(data[key]) || data[key] < 0 || data[key] > 99999999999) fail(400,'Monto inválido. Utilice hasta dos decimales.');
    if (data.ItbisCentavos > data.BaseCentavos) fail(400,'El ITBIS no puede superar la base.');
    return {BeneficiarioId:id,CentroId:center,FechaRegistro:date,DescripcionServicio:required(data.DescripcionServicio,500,'Descripción'),BaseCentavos:data.BaseCentavos,ItbisCentavos:data.ItbisCentavos,Fiscal:fiscal(data.Fiscal),NombreBeneficiario:`${b.NombreBeneficiario} ${b.ApellidoBeneficiario}`,NombreCentro:b.NombreCentro};
  }
  fail(400,'Tipo de operación no disponible.');
}
