# Actividad 4 — Pruebas de BibliotecaMVC

## Levantar la app como producción (para ver la página de error)
```
ASPNETCORE_ENVIRONMENT=Production dotnet run --no-launch-profile --urls http://localhost:5188
```
Para modo desarrollo normal: `dotnet run`

## Logging (Actividades 1 y 4)
Los mensajes salen en la terminal donde corre `dotnet run`.

| Prueba | Resultado esperado en consola |
|---|---|
| Entrar a /Libros | `Information`: inicio de consulta y cantidad de registros |
| Entrar a /Categorias | `Information`: inicio de consulta y cantidad de registros |
| Crear un libro válido | `Information`: inicio de creación y libro creado |
| Crear un libro con datos vacíos | `Warning`: creación rechazada por datos no válidos |
| Editar y eliminar un libro | `Information`: inicio y éxito de cada operación |
| Abrir /Libros/Details/9999 | `Warning`: no se encontró el libro 9999 y página 404 amigable |
| Abrir /Categorias/Edit/9999 | `Warning`: no se encontró la categoría 9999 |

## Página de error (Actividad 2)
1. Levantar en Production.
2. Apagar SQL Server: `docker stop sqledge`
3. Entrar a /Libros con sesión iniciada.
4. Resultado: página "Algo salió mal de nuestro lado" sin trazas ni cadenas de conexión, y en consola un `Error` con la excepción.
5. Volver a encender: `docker start sqledge`

## Acceso denegado (Actividad 3)
| Rol | Prueba | Resultado |
|---|---|---|
| Usuario | Abrir /Libros/Create | Página de acceso denegado personalizada |
| Usuario | Abrir /Libros/Edit/1 | Acceso denegado |
| Usuario | Abrir /Roles | Acceso denegado |
| Bibliotecario | Abrir /Libros/Create | Entra normal |
| Bibliotecario | Abrir /Roles | Acceso denegado |
| Admin | Abrir /Roles y /Libros/Create | Entra normal |
| Sin sesión | Abrir /Libros | Redirige a /Cuenta/Login |

## Regresión
- CRUD de Libros funciona igual con Admin y Bibliotecario.
- Usuario solo consulta el catálogo.
- Imágenes de libros siguen subiendo.
