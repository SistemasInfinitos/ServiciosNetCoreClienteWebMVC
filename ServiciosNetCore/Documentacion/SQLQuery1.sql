/*Nomenclaturas de programación: camelCase*/
/*modelado simplificado de un requerimiento que exige solución escalable, segura y tolerante a fallos */

--CREATE DATABASE DesarrolladorSemiSenior 
--USE DesarrolladorSemiSenior

/* se modifica tabla  por correccion del modelo con el fin de garantizar el requerimiento principal del cliente */
--DROP TABLE Usuarios
CREATE TABLE Usuarios
(
	id	int not null primary key identity(1,1),
	nombreUsuario	nvarchar(50)not null,
	passwordHash	nvarchar (max)not null,
	estado bit not null default 0,
	fechaCreacion datetime not null default GETDATE(),
	fechaActualizacion datetime null,
	personaId int  null
)  
--alter table Usuarios personaId int  null-- se pone null para controlar dependencia circular
--DROP TABLE Productos
CREATE TABLE Productos
(
	id	int not null primary key identity(1,1),
	descripcion	varchar(50)not null,
	valorUnitario decimal(18,4) not null,
	iva decimal(18,0) not null,
	estado bit not null default 0,
	fechaCreacion datetime not null default GETDATE(),
	fechaActualizacion datetime null,
)  
--alter table Productos add iva decimal(18,0) not null default 0  // el iva no puede quedar a criterio del vendedor--
/* se crea tabla adicional por correccion del modelo con el fin de garantizar el requerimiento principal del cliente */
--DROP TABLE EncabezadoPedidos
CREATE TABLE EncabezadoPedidos
(
	id	int not null primary key identity(1,1),
	usuarioId	Int not null,
	clientePersonaId	Int not null,
	valorNeto	decimal(18,4)not null default 0,
	valorIva	decimal(18,4)not null default 0,
	valorTotal	decimal(18,4)not null default 0,
	estado bit not null default 0,
	fechaCreacion datetime not null default GETDATE(),
	fechaActualizacion datetime null,
) 

/* se crea tabla adicional por correccion del modelo con el fin de garantizar el requerimiento principal del cliente */
--DROP TABLE DetallePedidos
CREATE TABLE DetallePedidos
(
	id	int not null primary key identity(1,1),
	encabezadoPedidosId	Int not null,
	productoId	Int not null,
	cantidad	decimal(18,4)not null,
	porcentajeIva	decimal(18,4)not null,
	valorUnitario decimal(18,4)not null,
	estado bit not null default 0,
	fechaCreacion datetime not null default GETDATE(),
	fechaActualizacion datetime null,
) 

/* se crea tabla adicional por correccion del modelo con el fin de garantizar el requerimiento principal del cliente */
--DROP TABLE Personas
CREATE TABLE Personas
(
	id	int not null primary key identity(1,1),
	nombres	nvarchar(50)not null, /* primer nombre segundo nombre se simplifican por tiempos de la prueba */
	apellidos	nvarchar(50)not null, /* priemr apellido segundo apellido se simplifican por tiempos de la prueba */
	estado bit not null default 0,
	fechaCreacion datetime not null default GETDATE(),
	fechaActualizacion datetime null,
) 

ALTER TABLE Usuarios  WITH noCHECK ADD  CONSTRAINT PersonasUsuariosId FOREIGN KEY(personaId)REFERENCES Personas (id)--un usuario debe ser una persona
--ALTER TABLE Usuarios drop PersonasUsuariosId
ALTER TABLE Usuarios ADD  CONSTRAINT [UQ_Usuarios] UNIQUE NONCLUSTERED(nombreUsuario) /*garantiza que no se duplique elusuario*/
ALTER TABLE EncabezadoPedidos  WITH CHECK ADD  CONSTRAINT EncabezadoPedidosUsuarioId FOREIGN KEY(usuarioId)REFERENCES Usuarios (id)
ALTER TABLE EncabezadoPedidos  WITH CHECK ADD  CONSTRAINT EncabezadoPedidosPersonaClienteId FOREIGN KEY(clientePersonaId)REFERENCES Personas (id)
ALTER TABLE DetallePedidos  WITH CHECK ADD  CONSTRAINT DetallePedidosProductosId FOREIGN KEY(productoId)REFERENCES Productos (id)
ALTER TABLE DetallePedidos  WITH CHECK ADD  CONSTRAINT DetallePedidosEncabezadoPedidos FOREIGN KEY(encabezadoPedidosId)REFERENCES EncabezadoPedidos (id)
/*se omite la uditoria de usuario por temas de tiempo*/
/*se crea sp con el fin de demostrar conocimientos y otras alternativas*/
create procedure SpDeleteDetallePedido
@id int 
as
begin 
--declare @id int =1--prueba 
if EXISTS(select COUNT(id)cantidad from DetallePedidos where id=@id)
	begin
	delete DetallePedidos where id=@id 
	select respuesta=@@ROWCOUNT; 
	end
else 
	select respuesta=@@ROWCOUNT
end

Create view viewEncabezadoPedidos as
select p.id,p.usuarioId,p.clientePersonaId,p.valorNeto,p.valorIva
,p. valorTotal,p.estado
,CONVERT(varchar,FORMAT(p.fechaCreacion, 'yyyy/MM/dd HH:mm','en-US'))fechaCreacion
,ISNULL(CONVERT(varchar,FORMAT(p.fechaActualizacion, 'yyyy/MM/dd HH:mm','en-US')),'') fechaActualizacion
,u.nombreUsuario
,concat(pp.nombres,' ',pp.apellidos)cliente
 from EncabezadoPedidos p
join Personas pp on pp.id=p.clientePersonaId
join Usuarios u on u.id=p.usuarioId
