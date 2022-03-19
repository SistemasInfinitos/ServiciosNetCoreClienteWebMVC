/*Nomenclaturas de programaci�n: camelCase*/
/*modelado simplificado de un requerimiento que exige soluci�n escalable, segura y tolerante a fallos */

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
	personaId int not null
)  

--DROP TABLE Productos
CREATE TABLE Productos
(
	id	int not null primary key identity(1,1),
	descripcion	varchar(50)not null,
	valorUnitario decimal(18,4) not null,
	estado bit not null default 0,
	fechaCreacion datetime not null default GETDATE(),
	fechaActualizacion datetime null,
)  

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
	cantidad	decimal(18,4),
	porcentajeIva	decimal(18,4),
	valorUnitario decimal(18,4),
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

--ALTER TABLE Usuarios  WITH CHECK ADD  CONSTRAINT PersonasUsuariosId FOREIGN KEY(personaId)REFERENCES Personas (id)-- se omite por tiempos un usuario debe ser una persona
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