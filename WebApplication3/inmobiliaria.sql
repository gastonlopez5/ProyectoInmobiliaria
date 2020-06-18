-- phpMyAdmin SQL Dump
-- version 4.9.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 18, 2020 at 08:42 PM
-- Server version: 10.4.11-MariaDB
-- PHP Version: 7.2.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Table structure for table `contrato`
--

CREATE TABLE `contrato` (
  `Id` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `Importe` decimal(10,0) NOT NULL,
  `DniGarante` varchar(8) NOT NULL,
  `NombreCompletoGarante` varchar(50) NOT NULL,
  `TelefonoGarante` varchar(10) NOT NULL,
  `EmailGarante` varchar(50) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `InquilinoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `contrato`
--

INSERT INTO `contrato` (`Id`, `FechaInicio`, `FechaFin`, `Importe`, `DniGarante`, `NombreCompletoGarante`, `TelefonoGarante`, `EmailGarante`, `InmuebleId`, `InquilinoId`) VALUES
(14, '2020-04-07', '2020-04-16', '6000', '11223344', 'Lopez Alejandro', '2215985703', 'gastonlopez5@gmail.com', 18, 5),
(24, '2020-04-29', '2020-04-29', '60000', '50110392', 'Thiago Lopez', '1154008019', 'thiago@mail.com', 19, 5),
(25, '2020-05-04', '2020-07-25', '7000', '33766055', 'Cuevas Gabriela', '2664614213', 'gabriela@mail.com', 19, 5),
(29, '2020-05-02', '2022-05-02', '10000', '33766055', 'Cuevas Gabriela', '2664614213', 'gabriela@mail.com', 21, 5),
(30, '2020-05-06', '2020-06-08', '50000', '50110392', 'Thiago Lopez', '1154008019', 'thiago@mail.com', 33, 6),
(31, '2020-03-12', '2020-06-08', '50000', '33766055', 'Thiago Lopez', '1154008019', 'thiago@mail.com', 33, 6),
(32, '2020-05-28', '2020-06-08', '10000', '33766055', 'Thiago Lopez', '1154008019', 'gabriela@mail.com', 33, 6),
(33, '2020-05-08', '2022-05-07', '5000', '33766055', 'Thiago Lopez', '1154008019', 'thiago@mail.com', 20, 5),
(34, '2020-05-07', '2022-05-07', '5000', '50110392', 'Cuevas Gabriela', '1154008019', 'gabriela@mail.com', 34, 6);

-- --------------------------------------------------------

--
-- Table structure for table `empleados`
--

CREATE TABLE `empleados` (
  `Id` int(11) NOT NULL,
  `Dni` varchar(8) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `Telefono` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `empleados`
--

INSERT INTO `empleados` (`Id`, `Dni`, `Nombre`, `Apellido`, `Email`, `Telefono`) VALUES
(5, '33766055', 'Gabriela', 'Cuevas', 'gabrielacuevas2207@gmail.com', '1155400801'),
(7, '98653212', 'Alejandro', 'Lopez', 'alejandrolopezjesus62@gmail.com', '1256786523');

-- --------------------------------------------------------

--
-- Table structure for table `fotoperfil`
--

CREATE TABLE `fotoperfil` (
  `Id` int(11) NOT NULL,
  `Ruta` varchar(100) NOT NULL,
  `PropietarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `fotoperfil`
--

INSERT INTO `fotoperfil` (`Id`, `Ruta`, `PropietarioId`) VALUES
(3, '/FotoPerfil/20/foto.jpg', 19);

-- --------------------------------------------------------

--
-- Table structure for table `galeria`
--

CREATE TABLE `galeria` (
  `Id` int(11) NOT NULL,
  `Ruta` varchar(100) NOT NULL,
  `InmuebleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `galeria`
--

INSERT INTO `galeria` (`Id`, `Ruta`, `InmuebleId`) VALUES
(6, '/Galeria/33/casa2.jpg', 33),
(18, '/Galeria/34/Lighthouse.jpg', 34),
(36, '/Galeria/40/casa2.jpg', 40);

-- --------------------------------------------------------

--
-- Table structure for table `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(50) NOT NULL,
  `Tipo` int(11) NOT NULL,
  `Uso` varchar(50) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Costo` decimal(10,0) NOT NULL,
  `Disponible` tinyint(1) NOT NULL DEFAULT 1,
  `PropietarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `Direccion`, `Tipo`, `Uso`, `Ambientes`, `Costo`, `Disponible`, `PropietarioId`) VALUES
(17, 'Montes de Oca 666', 3, 'Privado', 6, '20000', 0, 16),
(18, 'Chile 2053', 1, 'Residencial', 3, '6000', 0, 16),
(19, 'Cordoba 350', 1, 'Residencial', 2, '60000', 1, 16),
(20, 'Calle prueba', 2, 'Residencial', 2, '5000', 1, 17),
(21, 'Almirante Brown 555', 1, 'Privado', 5, '10000', 1, 17),
(33, 'Juana Manso', 1, 'Residencial', 23, '50000', 1, 19),
(34, 'aaaaaaa', 1, 'Residencial', 3, '5000', 1, 20),
(40, 'aaaa', 3, 'Privado', 5, '5000', 1, 19);

-- --------------------------------------------------------

--
-- Table structure for table `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` varchar(8) NOT NULL,
  `Telefono` varchar(10) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `DireccionTrabajo` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `DireccionTrabajo`) VALUES
(5, 'Lulu', 'Lopez', '44332211', '1154008019', 'lulu@gmail.com', 'Comodoro Pi'),
(6, 'Mariano ', 'Lopez Cuevas', '45689123', '7845123256', 'mariano@mail.com', 'ULP direccion');

-- --------------------------------------------------------

--
-- Table structure for table `pago`
--

CREATE TABLE `pago` (
  `Id` int(11) NOT NULL,
  `NroPago` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Importe` decimal(10,0) NOT NULL,
  `ContratoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `pago`
--

INSERT INTO `pago` (`Id`, `NroPago`, `Fecha`, `Importe`, `ContratoId`) VALUES
(29, 1, '2020-05-02', '7000', 25),
(30, 1, '2020-05-02', '15000', 29),
(31, 1, '2020-05-06', '50000', 31),
(32, 2, '2020-05-06', '50000', 31),
(33, 3, '2020-05-06', '50000', 31),
(34, 2, '2020-05-07', '10000', 29),
(35, 1, '2020-05-07', '5000', 34),
(36, 2, '2020-05-07', '5000', 34);

-- --------------------------------------------------------

--
-- Table structure for table `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Telefono` varchar(10) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `Dni` varchar(8) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `propietarios`
--

INSERT INTO `propietarios` (`Id`, `Nombre`, `Apellido`, `Telefono`, `Email`, `Dni`) VALUES
(16, 'Gloria', 'Diaz', '1245896432', 'gd460418@gmail.com', '78451232'),
(17, 'Luluna', 'Cuevas', '2664614213', 'lulu@mail.com', '92826999'),
(19, 'Gaston', 'Lopez', '2664614213', 'gaston@mail.com', '32826861'),
(20, 'Luis', 'Mercado', '2664614213', 'luis@mail.com', '50110392');

-- --------------------------------------------------------

--
-- Table structure for table `roles`
--

CREATE TABLE `roles` (
  `Id` int(11) NOT NULL,
  `Rol` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `roles`
--

INSERT INTO `roles` (`Id`, `Rol`) VALUES
(1, 'Administrador'),
(2, 'Empleado'),
(3, 'Propietario');

-- --------------------------------------------------------

--
-- Table structure for table `tipoinmueble`
--

CREATE TABLE `tipoinmueble` (
  `Id` int(11) NOT NULL,
  `Tipo` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `tipoinmueble`
--

INSERT INTO `tipoinmueble` (`Id`, `Tipo`) VALUES
(1, 'Departamento'),
(2, 'Casa'),
(3, 'Local'),
(4, 'Deposito');

-- --------------------------------------------------------

--
-- Table structure for table `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `Clave` varchar(100) NOT NULL,
  `RolId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Email`, `Clave`, `RolId`) VALUES
(1, 'gastonlopez5@gmail.com', 'hWr6SMZibgTn2LvF5Ol1QGMe3TtOGOy+XtJlGxacrqQ=', 1),
(14, 'gabrielacuevas2207@gmail.com', '2scuvGC2pOHJ5RJn2qwgfFZI3PQ09D3qaozyjlBmD+k=', 2),
(19, 'alejandrolopezjesus62@gmail.com', 'DlkcU79nZBoNsU1Cv1q0hwnftlUNMcqvV/TYViY2BDk=', 2),
(20, 'gd460418@gmail.com', 'DlkcU79nZBoNsU1Cv1q0hwnftlUNMcqvV/TYViY2BDk=', 3),
(21, 'lulu@mail.com', '2scuvGC2pOHJ5RJn2qwgfFZI3PQ09D3qaozyjlBmD+k=', 3),
(24, 'gaston@mail.com', '2scuvGC2pOHJ5RJn2qwgfFZI3PQ09D3qaozyjlBmD+k=', 3);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InquilinoId` (`InquilinoId`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indexes for table `empleados`
--
ALTER TABLE `empleados`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `fotoperfil`
--
ALTER TABLE `fotoperfil`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`);

--
-- Indexes for table `galeria`
--
ALTER TABLE `galeria`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indexes for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`),
  ADD KEY `Tipo` (`Tipo`);

--
-- Indexes for table `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ContratoId` (`ContratoId`);

--
-- Indexes for table `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `tipoinmueble`
--
ALTER TABLE `tipoinmueble`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `RolId` (`RolId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `contrato`
--
ALTER TABLE `contrato`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

--
-- AUTO_INCREMENT for table `empleados`
--
ALTER TABLE `empleados`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `fotoperfil`
--
ALTER TABLE `fotoperfil`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `galeria`
--
ALTER TABLE `galeria`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=43;

--
-- AUTO_INCREMENT for table `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;

--
-- AUTO_INCREMENT for table `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `pago`
--
ALTER TABLE `pago`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT for table `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `roles`
--
ALTER TABLE `roles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `tipoinmueble`
--
ALTER TABLE `tipoinmueble`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`);

--
-- Constraints for table `fotoperfil`
--
ALTER TABLE `fotoperfil`
  ADD CONSTRAINT `fotoperfil_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`);

--
-- Constraints for table `galeria`
--
ALTER TABLE `galeria`
  ADD CONSTRAINT `galeria_ibfk_1` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`);

--
-- Constraints for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`),
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`Tipo`) REFERENCES `tipoinmueble` (`Id`);

--
-- Constraints for table `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`ContratoId`) REFERENCES `contrato` (`Id`);

--
-- Constraints for table `usuarios`
--
ALTER TABLE `usuarios`
  ADD CONSTRAINT `usuarios_ibfk_1` FOREIGN KEY (`RolId`) REFERENCES `roles` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
