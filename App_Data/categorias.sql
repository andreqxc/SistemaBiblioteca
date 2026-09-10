CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL
);

INSERT INTO Categorias (Nombre, Descripcion) VALUES
('Novela', 'Obras de ficción narrativa extensa'),
('Ciencia Ficción', 'Historias basadas en avances científicos o tecnológicos'),
('Historia', 'Libros sobre hechos históricos');
