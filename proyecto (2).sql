-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 22-11-2025 a las 22:29:07
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `proyecto`
--

DELIMITER $$
--
-- Procedimientos
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `GuardarPersonaje` (IN `Nombre` VARCHAR(50), IN `Contrasena` VARCHAR(50), IN `ID_Clase` INT, IN `ID_Alineamiento` INT, IN `ID_Subraza` INT, IN `ID_Transfondo` INT, IN `HP` INT, IN `Fuerza` INT, IN `Sabiduria` INT, IN `Inteligencia` INT, IN `Constitucion` INT, IN `Destreza` INT, IN `Carisma` INT, IN `Iniciativa` INT)   BEGIN
    INSERT INTO Jugador (
        Nombre, Contrasena, ID_Clase, ID_Alineamiento, ID_Subraza, ID_Transfondo, ID_Nivel,
        HP, EXP, Fuerza, Sabiduria, Inteligencia, Constitucion, Destreza, Carisma, Iniciativa, Oro
    )
    VALUES (
        Nombre, Contrasena, ID_Clase, ID_Alineamiento, ID_Subraza, ID_Transfondo, 1,
        HP, 0, Fuerza, Sabiduria, Inteligencia, Constitucion, Destreza, Carisma, Iniciativa, 0
    );
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `ValidarCredenciales` (IN `nombreJugador` VARCHAR(100), IN `contrasenaJugador` VARCHAR(100))   BEGIN
    SELECT COUNT(*) AS Coincidencias
    FROM jugador
    WHERE Nombre = nombreJugador AND Contrasena = contrasenaJugador;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `alineamiento`
--

CREATE TABLE `alineamiento` (
  `ID_Alineamiento` int(11) NOT NULL,
  `Nombre` varchar(20) NOT NULL,
  `Descripcion` varchar(120) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `alineamiento`
--

INSERT INTO `alineamiento` (`ID_Alineamiento`, `Nombre`, `Descripcion`) VALUES
(1, 'Legal Bueno', 'Vinculado a juramentos antiguos, su justicia es eco de un reino caido'),
(2, 'Neutral Bueno', 'Camina entre ruinas, guiado por compasion sin ley ni corona'),
(3, 'Caotico Bueno', 'Rompe cadenas por almas perdidas, aunque el mundo lo llame traidor'),
(4, 'Legal Neutral', 'Sirve ordenes olvidadas, indiferente al dolor o la redencion'),
(5, 'Neutral Puro', 'Equilibrio encarnado, como piedra que observa sin juicio'),
(6, 'Caotico Neutral', 'Errante sin causa, su voluntad danza con el viento y la ruina'),
(7, 'Legal Malvado', 'Obedece leyes crueles, donde el orden es instrumento de sometimiento'),
(8, 'Neutral Malvado', 'Busca poder en cenizas, sin lealtad ni piedad, como sombra que devora'),
(9, 'Caotico Malvado', 'Arde en caos y destruccion, como heraldo de tormentas sin nombre');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `atributosgen`
--

CREATE TABLE `atributosgen` (
  `ID_Atributo` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `atributosgen`
--

INSERT INTO `atributosgen` (`ID_Atributo`, `Nombre`, `Descripcion`) VALUES
(1, 'Fuerza', 'Poder bruto que rompe sellos, puertas y voluntades'),
(2, 'Destreza', 'Agilidad precisa, como filo que danza entre ruinas'),
(3, 'Constitucion', 'Resistencia forjada en dolor, cicatrices y hierro'),
(4, 'Inteligencia', 'Memoria de eras perdidas, dominio de lo arcano'),
(5, 'Sabiduria', 'Percepcion del abismo, juicio entre sombras y ecos'),
(6, 'Carisma', 'Presencia que invoca pactos, doblega almas y destinos');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `clase`
--

CREATE TABLE `clase` (
  `ID_Clase` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `DadosGolpe` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `clase`
--

INSERT INTO `clase` (`ID_Clase`, `Nombre`, `Descripcion`, `DadosGolpe`) VALUES
(1, 'Guerrero', 'Combatiente versatil experto en armas y armaduras', 10),
(2, 'Mago', 'Usuario de magia arcana con gran poder ofensivo', 6),
(3, 'Paladin', 'Se lo que se conoce como una fuerza', 8),
(4, 'Clerigo', 'Sanador y protector con magia divina', 8);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `clase_habilidades`
--

CREATE TABLE `clase_habilidades` (
  `ID_ClHa` int(11) NOT NULL,
  `Competente` tinyint(1) NOT NULL,
  `Cantidad` int(11) NOT NULL,
  `ID_Clase` int(11) NOT NULL,
  `ID_Habilidad` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `clase_habilidades`
--

INSERT INTO `clase_habilidades` (`ID_ClHa`, `Competente`, `Cantidad`, `ID_Clase`, `ID_Habilidad`) VALUES
(1, 1, 1, 1, 1),
(2, 1, 1, 1, 2),
(3, 1, 1, 1, 3),
(4, 1, 1, 1, 4),
(5, 1, 1, 1, 5),
(6, 1, 1, 1, 6),
(7, 1, 1, 1, 7),
(8, 0, 1, 1, 8),
(9, 0, 1, 1, 9),
(10, 0, 1, 1, 10),
(11, 1, 1, 3, 8),
(12, 1, 1, 3, 9),
(13, 1, 1, 3, 10),
(14, 1, 1, 3, 11),
(15, 1, 1, 3, 12),
(16, 1, 1, 3, 16),
(17, 1, 1, 3, 17),
(18, 1, 1, 3, 18),
(19, 1, 5, 3, 19),
(20, 1, 1, 3, 20),
(21, 1, 1, 1, 21),
(22, 1, 1, 1, 22),
(23, 1, 1, 1, 23),
(24, 1, 1, 1, 24),
(25, 1, 1, 3, 25);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `conjuros`
--

CREATE TABLE `conjuros` (
  `ID_Conjuro` int(11) NOT NULL,
  `ID_Clase` int(11) NOT NULL,
  `ID_Hechizo` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `conjuros`
--

INSERT INTO `conjuros` (`ID_Conjuro`, `ID_Clase`, `ID_Hechizo`) VALUES
(1, 2, 1),
(2, 2, 2),
(3, 2, 3),
(4, 2, 4),
(5, 2, 5),
(6, 2, 6),
(7, 2, 7),
(8, 2, 8),
(9, 2, 9),
(10, 2, 10),
(11, 2, 11),
(12, 2, 12),
(13, 2, 13),
(14, 2, 14),
(15, 2, 15),
(16, 2, 16),
(17, 2, 17),
(18, 2, 18),
(19, 2, 19),
(20, 2, 20),
(21, 2, 21),
(22, 2, 22),
(23, 2, 23),
(24, 2, 24),
(25, 4, 25),
(26, 4, 26),
(27, 4, 27),
(28, 4, 28),
(29, 4, 29),
(30, 4, 30),
(31, 4, 31),
(32, 4, 32),
(33, 4, 33),
(34, 4, 34),
(35, 4, 35),
(36, 4, 36),
(37, 4, 37),
(38, 4, 38),
(39, 4, 39),
(40, 4, 40),
(41, 4, 41),
(42, 4, 42),
(43, 4, 43),
(44, 4, 44),
(45, 4, 45),
(46, 4, 46),
(47, 4, 47),
(48, 4, 48);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `dialogonpc`
--

CREATE TABLE `dialogonpc` (
  `ID_DialogoNPC` int(11) NOT NULL,
  `Prioridad` int(11) NOT NULL,
  `Texto` text NOT NULL,
  `ID_NPC` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `enemigo`
--

CREATE TABLE `enemigo` (
  `ID_Enemigo` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `HP` int(11) NOT NULL,
  `Fuerza` int(11) NOT NULL,
  `Sabiduria` int(11) NOT NULL,
  `Inteligencia` int(11) NOT NULL,
  `Constitucion` int(11) NOT NULL,
  `Destreza` int(11) NOT NULL,
  `Carisma` int(11) NOT NULL,
  `Iniciativa` int(11) NOT NULL,
  `Nivel` int(11) NOT NULL,
  `ID_Transfondo` int(11) NOT NULL,
  `ID_Clase` int(11) NOT NULL,
  `ID_Subraza` int(11) NOT NULL,
  `ID_Objeto` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `enemigo`
--

INSERT INTO `enemigo` (`ID_Enemigo`, `Nombre`, `HP`, `Fuerza`, `Sabiduria`, `Inteligencia`, `Constitucion`, `Destreza`, `Carisma`, `Iniciativa`, `Nivel`, `ID_Transfondo`, `ID_Clase`, `ID_Subraza`, `ID_Objeto`) VALUES
(1, 'Zombi', 22, 13, 6, 3, 16, 6, 5, -2, 1, 5, 4, 8, 51),
(2, 'Varnoth', 39, 13, 11, 14, 10, 16, 10, 3, 2, 1, 1, 2, 51),
(3, 'Serpiente de Fuego', 22, 12, 10, 7, 11, 14, 10, 2, 1, 6, 2, 7, 51),
(4, 'Tarak', 27, 10, 14, 12, 10, 16, 8, 3, 1, 2, 3, 1, 51),
(5, 'Oso Lechuza', 59, 20, 13, 3, 17, 12, 7, 1, 3, 7, 1, 9, 51),
(6, 'Pulpo Siervo Espora', 52, 17, 6, 2, 13, 13, 1, 1, 3, 6, 4, 5, 51),
(7, 'Miconido Adulto', 22, 10, 10, 10, 12, 10, 13, 0, 1, 4, 4, 7, 51),
(8, 'Sinensa', 60, 12, 15, 16, 14, 10, 10, 0, 2, 4, 4, 7, 51),
(9, 'Kobold Alado', 7, 7, 7, 8, 9, 16, 8, 3, 1, 5, 3, 3, 51),
(10, 'Manitas Kobold', 10, 7, 7, 15, 10, 14, 9, 2, 1, 5, 2, 3, 51),
(11, 'Brote Miconido', 7, 8, 11, 8, 10, 10, 5, 0, 1, 4, 4, 7, 51),
(12, 'Hongo Violeta', 18, 3, 3, 1, 10, 1, 1, -5, 1, 4, 4, 7, 51),
(13, 'Kobold', 5, 7, 7, 8, 9, 15, 8, 2, 1, 5, 3, 3, 51),
(14, 'Estirge', 2, 4, 8, 2, 11, 16, 6, 3, 1, 7, 1, 9, 51),
(15, 'Gul', 22, 13, 10, 7, 10, 15, 6, 2, 1, 5, 4, 8, 51),
(16, 'Cria de Dragon Azul', 52, 17, 11, 12, 15, 10, 15, 0, 3, 8, 1, 6, 51),
(17, 'Cria de Dragon de Bronce', 32, 17, 11, 12, 15, 10, 15, 0, 2, 8, 1, 6, 51),
(18, 'Runara (Dragona de Bronce Adulta)', 212, 25, 15, 16, 23, 10, 19, 0, 13, 8, 2, 6, 51),
(19, 'Arpia', 38, 12, 10, 7, 12, 13, 13, 1, 1, 5, 3, 9, 51),
(20, 'Draco de Humo', 22, 6, 10, 6, 12, 14, 11, 2, 1, 6, 2, 7, 51);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `habilidades`
--

CREATE TABLE `habilidades` (
  `ID_Habilidades` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `habilidades`
--

INSERT INTO `habilidades` (`ID_Habilidades`, `Nombre`) VALUES
(1, 'Ataque adicional'),
(2, 'Defensa con escudo'),
(3, 'Golpe preciso'),
(4, 'Golpe poderoso'),
(5, 'Intercepcion'),
(6, 'Riposte'),
(7, 'Ataque giratorio'),
(8, 'Carga divina'),
(9, 'Juramento de venganza'),
(10, 'Proteccion divina'),
(11, 'Golpe cegador'),
(12, 'Golpe aturdidor'),
(13, 'Golpe desarmador'),
(14, 'Golpe doble'),
(15, 'Golpe de represalia'),
(16, 'Aura de coraje'),
(17, 'Aura de proteccion'),
(18, 'Canalizar divinidad'),
(19, 'Imposicion de manos'),
(20, 'Golpe divino'),
(21, 'Golpe brutal'),
(22, 'Golpe de distraccion'),
(23, 'Golpe de derribo'),
(24, 'Golpe de zona'),
(25, 'Golpe de juicio');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `hechizo`
--

CREATE TABLE `hechizo` (
  `ID_Hechizo` int(11) NOT NULL,
  `Nombre` varchar(100) DEFAULT NULL,
  `Descripcion` varchar(100) DEFAULT NULL,
  `DMG` int(11) NOT NULL,
  `ID_Nivel` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `hechizo`
--

INSERT INTO `hechizo` (`ID_Hechizo`, `Nombre`, `Descripcion`, `DMG`, `ID_Nivel`) VALUES
(1, 'Magic Missile', 'Lanza proyectiles magicos automaticos que impactan sin fallo.', 10, 1),
(2, 'Shield', 'Genera un escudo casi inpenetrable.', 0, 1),
(3, 'Mage Armor', 'Otorga un escudo de mana tan denso que es capaz de romper armas poco efectivas.', 0, 1),
(4, 'Sleep', 'Duerme criaturas con HP bajo en area.', 0, 1),
(5, 'Mirror Image', 'Crea duplicados ilusorios que desvian ataques.', 0, 2),
(6, 'Misty Step', 'Teletransportacion corta como accion adicional.', 0, 2),
(7, 'Scorching Ray', 'Tres rayos de fuego dirigidos a uno o varios objetivos.', 18, 2),
(8, 'Fireball', 'Explosion de fuego en area que inflige gran dano.', 28, 3),
(9, 'Counterspell', 'Interrumpe la activacion de un hechizo enemigo.', 0, 3),
(10, 'Lightning Bolt', 'Rayo electrico en linea que inflige dano masivo.', 28, 3),
(11, 'Greater Invisibility', 'Invisibilidad que no se rompe al atacar o lanzar hechizos.', 0, 4),
(12, 'Polymorph', 'Transforma a una criatura en otra forma fisica.', 0, 4),
(13, 'Wall of Force', 'Crea una barrera invisible e impenetrable.', 0, 5),
(14, 'Cone of Cold', 'Cono de hielo que inflige dano en area.', 36, 5),
(15, 'Chain Lightning', 'Rayo que salta entre multiples objetivos.', 40, 6),
(16, 'Disintegrate', 'Inflige dano masivo y desintegra si reduce a 0 HP.', 75, 6),
(17, 'Plane Shift', 'Transporta a otra dimension o plano.', 0, 7),
(18, 'Finger of Death', 'Dano necrotico masivo y crea zombi si mata.', 70, 7),
(19, 'Power Word Stun', 'Aturde sin tirada de salvacion si tiene menos de 150 HP.', 0, 8),
(20, 'Incendiary Cloud', 'Nube de fuego movil que inflige dano continuo.', 40, 8),
(21, 'Wish', 'Hechizo mas poderoso, puede replicar cualquier otro.', 0, 9),
(22, 'Meteor Swarm', 'Explosion masiva de fuego y dano contundente.', 80, 9),
(23, 'Detect Magic', 'Detecta presencia de magia en objetos o criaturas.', 0, 1),
(24, 'Thunderwave', 'Onda de choque que empuja y dana en area.', 12, 1),
(25, 'Cure Wounds', 'Cura puntos de golpe tocando al objetivo.', 0, 1),
(26, 'Bless', 'Otorga bonificacion a ataques y salvaciones.', 0, 1),
(27, 'Guiding Bolt', 'Rayo de energia radiante que inflige dano y otorga ventaja.', 14, 1),
(28, 'Inflict Wounds', 'Toque que inflige dano necrotico masivo.', 18, 1),
(29, 'Spiritual Weapon', 'Crea arma flotante que ataca cada turno.', 10, 2),
(30, 'Hold Person', 'Paraliza a una criatura humanoide.', 0, 2),
(31, 'Revivify', 'Revive a una criatura muerta recientemente.', 0, 3),
(32, 'Spirit Guardians', 'Inflige dano radiante o necrotico en area alrededor del lanzador.', 15, 3),
(33, 'Bestow Curse', 'Inflige penalizacion persistente a una criatura.', 0, 3),
(34, 'Death Ward', 'Previene la muerte una vez, dejando al objetivo con 1 HP.', 0, 4),
(35, 'Guardian of Faith', 'Centinela espectral que inflige dano automatico.', 20, 4),
(36, 'Raise Dead', 'Resucita a una criatura muerta hace hasta 10 dias.', 0, 5),
(37, 'Flame Strike', 'Columna de fuego y luz que inflige dano mixto.', 28, 5),
(38, 'Blade Barrier', 'Muro de cuchillas giratorias que inflige dano.', 30, 6),
(39, 'Harm', 'Inflige dano necrotico masivo.', 55, 6),
(40, 'Resurrection', 'Resucita a una criatura muerta hace hasta 100 anos.', 0, 7),
(41, 'Divine Word', 'Expulsa criaturas extraplanares con palabras divinas.', 0, 7),
(42, 'Earthquake', 'Causa terremoto masivo que derrumba estructuras.', 40, 8),
(43, 'Sunburst', 'Explosion solar que inflige dano y ciega.', 30, 8),
(44, 'Mass Heal', 'Cura hasta 700 HP distribuidos entre aliados.', 0, 9),
(45, 'Storm of Vengeance', 'Tormenta divina destructiva en area.', 50, 9),
(46, 'Shield of Faith', 'Una oracion tan formidable que te protege de golpes.', 0, 1),
(47, 'Command', 'Obliga a una criatura a obedecer una orden simple.', 0, 1),
(48, 'Zone of Truth', 'Impide mentir dentro de un area.', 0, 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inventario`
--

CREATE TABLE `inventario` (
  `ID_Inventario` int(11) NOT NULL,
  `Carga_Actual` int(11) NOT NULL,
  `Carga_Maxima` int(11) NOT NULL,
  `ID_Jugador` int(11) NOT NULL,
  `ID_Objeto` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `jugador`
--

CREATE TABLE `jugador` (
  `ID_Jugador` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `HP` int(11) NOT NULL,
  `EXP` int(11) NOT NULL,
  `Fuerza` int(11) NOT NULL,
  `Sabiduria` int(11) NOT NULL,
  `Inteligencia` int(11) NOT NULL,
  `Constitucion` int(11) NOT NULL,
  `Destreza` int(11) NOT NULL,
  `Carisma` int(11) NOT NULL,
  `Iniciativa` int(11) NOT NULL,
  `Oro` int(11) NOT NULL,
  `ID_Alineamiento` int(11) NOT NULL,
  `ID_Nivel` int(11) NOT NULL,
  `ID_Clase` int(11) NOT NULL,
  `ID_Transfondo` int(11) NOT NULL,
  `ID_Subraza` int(11) NOT NULL,
  `Contrasena` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `jugador`
--

INSERT INTO `jugador` (`ID_Jugador`, `Nombre`, `HP`, `EXP`, `Fuerza`, `Sabiduria`, `Inteligencia`, `Constitucion`, `Destreza`, `Carisma`, `Iniciativa`, `Oro`, `ID_Alineamiento`, `ID_Nivel`, `ID_Clase`, `ID_Transfondo`, `ID_Subraza`, `Contrasena`) VALUES
(18, 'a', 11, 0, 16, 15, 8, 15, 8, 10, 2, 0, 3, 1, 4, 4, 2, 'a'),
(21, 'a', 9, 0, 17, 15, 9, 17, 6, 12, -2, 0, 1, 1, 4, 1, 1, 'a1'),
(22, 'a', 7, 0, 9, 10, 17, 13, 15, 8, 4, 0, 2, 1, 2, 2, 1, 'rr');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `mapa`
--

CREATE TABLE `mapa` (
  `ID_Mapa` int(11) NOT NULL,
  `Bloqueado` tinyint(1) NOT NULL,
  `Nombre` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `misiones`
--

CREATE TABLE `misiones` (
  `ID_Misiones` int(11) NOT NULL,
  `Oro` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Bloqueado` tinyint(1) NOT NULL,
  `Completado` tinyint(1) NOT NULL,
  `ID_Objeto` int(11) NOT NULL,
  `ID_Mapa` int(11) NOT NULL,
  `ID_NPC` int(11) NOT NULL,
  `ID_Enemigo` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `nivel`
--

CREATE TABLE `nivel` (
  `ID_Nivel` int(11) NOT NULL,
  `BonusCompetencia` int(11) NOT NULL,
  `EXPNecesaria` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `nivel`
--

INSERT INTO `nivel` (`ID_Nivel`, `BonusCompetencia`, `EXPNecesaria`) VALUES
(1, 2, 0),
(2, 2, 60),
(3, 2, 180),
(4, 2, 540),
(5, 3, 1300),
(6, 3, 2800),
(7, 3, 4600),
(8, 3, 6800),
(9, 4, 9600),
(10, 4, 12800),
(11, 4, 17000),
(12, 4, 20000),
(13, 5, 24000),
(14, 5, 28000),
(15, 5, 33000),
(16, 5, 39000),
(17, 6, 45000),
(18, 6, 53000),
(19, 6, 61000),
(20, 6, 71000);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `npc`
--

CREATE TABLE `npc` (
  `ID_NPC` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `objeto`
--

CREATE TABLE `objeto` (
  `ID_Objeto` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Peso` int(11) NOT NULL,
  `Efecto` int(11) NOT NULL,
  `Afectado` varchar(20) NOT NULL,
  `Precio` int(11) NOT NULL,
  `DistanciaUso` int(11) NOT NULL,
  `Categoria` varchar(20) NOT NULL,
  `Rareza` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `objeto`
--

INSERT INTO `objeto` (`ID_Objeto`, `Nombre`, `Peso`, `Efecto`, `Afectado`, `Precio`, `DistanciaUso`, `Categoria`, `Rareza`) VALUES
(1, 'Pocion de Curacion Chica', 1, 101, 'Portador', 50, 0, 'Pocion', 'Comun'),
(2, 'Pocion de Curacion Mediana', 1, 102, 'Portador', 150, 0, 'Pocion', 'Infrecuente'),
(3, 'Pocion de Curacion Grande', 1, 103, 'Portador', 300, 0, 'Pocion', 'Rara'),
(4, 'Pocion de Resistencia Total', 1, 104, 'Portador', 800, 0, 'Pocion', 'Muy rara'),
(5, 'Pergamino de Conjuro Nivel 1', 1, 201, 'Portador', 100, 10, 'Pergamino', 'Comun'),
(6, 'Pergamino de Conjuro Nivel 5', 1, 205, 'Portador', 500, 10, 'Pergamino', 'Rara'),
(7, 'Pergamino de Conjuro Nivel 9', 1, 209, 'Portador', 1200, 10, 'Pergamino', 'Legendaria'),
(8, 'Armadura del Ultimo Aliento', 40, 301, 'Portador', 5000, 0, 'Armadura', 'Legendaria'),
(9, 'Forma de Runara', 5, 401, 'Runara', 0, 0, 'Reliquia', 'Legendaria'),
(10, 'Espina de la Tempestad', 15, 402, 'Portador', 6000, 30, 'Reliquia', 'Legendaria'),
(11, 'Colmillo de Runara', 12, 501, 'Portador', 4500, 5, 'Arma', 'Legendaria'),
(12, 'Espada de la Ira Silente', 10, 502, 'Portador', 4000, 5, 'Arma', 'Epica'),
(13, 'Lanza de la Tempestad', 14, 503, 'Portador', 4200, 30, 'Arma', 'Epica'),
(14, 'Baculo de Runara', 8, 511, 'Portador', 4800, 60, 'Arma', 'Legendaria'),
(15, 'Libro del Vacio Estelar', 6, 512, 'Portador', 5000, 0, 'Arma', 'Epica'),
(16, 'Vara de la Tempestad Rota', 7, 513, 'Portador', 4700, 40, 'Arma', 'Epica'),
(17, 'Guantes de Sinensa', 2, 521, 'Portador', 4300, 0, 'Arma', 'Epica'),
(18, 'Libro del Ultimo Rito', 5, 522, 'Portador', 4600, 0, 'Arma', 'Legendaria'),
(19, 'Cruz del Juicio Final', 4, 523, 'Portador', 4400, 0, 'Arma', 'Epica'),
(20, 'Espada de la Tempestad Final', 16, 531, 'Portador', 4900, 5, 'Arma', 'Legendaria'),
(21, 'Escudo de Runara', 18, 532, 'Portador', 4700, 0, 'Arma', 'Epica'),
(22, 'Arco del Vacio Celeste', 12, 533, 'Portador', 4600, 60, 'Arma', 'Epica'),
(23, 'Espada de Bruma', 10, 601, 'Portador', 200, 5, 'Arma', 'Comun'),
(24, 'Espada de Hueso', 11, 602, 'Portador', 250, 5, 'Arma', 'Comun'),
(25, 'Espada del Susurro Gris', 10, 603, 'Portador', 300, 5, 'Arma', 'Infrecuente'),
(26, 'Espada de la Niebla Rota', 12, 604, 'Portador', 350, 5, 'Arma', 'Infrecuente'),
(27, 'Espada de Sangre Templada', 11, 605, 'Portador', 400, 5, 'Arma', 'Rara'),
(28, 'Espada de la Caverna Hueca', 13, 606, 'Portador', 450, 5, 'Arma', 'Rara'),
(29, 'Espada de los Ecos Perdidos', 12, 607, 'Portador', 500, 5, 'Arma', 'Rara'),
(30, 'Espada de la Devocion Rota', 14, 611, 'Portador', 300, 5, 'Arma', 'Comun'),
(31, 'Espada de la Penumbra', 15, 612, 'Portador', 350, 5, 'Arma', 'Infrecuente'),
(32, 'Espada de la Sangre Noble', 16, 613, 'Portador', 400, 5, 'Arma', 'Rara'),
(33, 'Espada del Juicio Gris', 15, 614, 'Portador', 450, 5, 'Arma', 'Rara'),
(34, 'Espada de la Tormenta', 16, 615, 'Portador', 500, 5, 'Arma', 'Rara'),
(35, 'Espada de los Caidos', 14, 616, 'Portador', 550, 5, 'Arma', 'Rara'),
(36, 'Espada del Vacio Silente', 15, 617, 'Portador', 600, 5, 'Arma', 'Rara'),
(37, 'Libro de Sombras Menores', 5, 701, 'Portador', 200, 0, 'Arma', 'Comun'),
(38, 'Baculo de la Llama Silente', 8, 702, 'Portador', 250, 60, 'Arma', 'Comun'),
(39, 'Vara de los Ecos Rotos', 7, 703, 'Portador', 300, 40, 'Arma', 'Infrecuente'),
(40, 'Libro de Conjuros Marchitos', 6, 704, 'Portador', 350, 0, 'Arma', 'Infrecuente'),
(41, 'Baculo de la Niebla Interior', 8, 705, 'Portador', 400, 60, 'Arma', 'Rara'),
(42, 'Orbe de Sangre Fria', 4, 706, 'Portador', 450, 30, 'Arma', 'Rara'),
(43, 'Libro de la Lengua Muerta', 6, 707, 'Portador', 500, 0, 'Arma', 'Rara'),
(44, 'Cruz de Ceniza', 4, 801, 'Portador', 200, 0, 'Arma', 'Comun'),
(45, 'Guantes del Perdon Silente', 2, 802, 'Portador', 250, 0, 'Arma', 'Comun'),
(46, 'Libro de los Caidos', 5, 803, 'Portador', 300, 0, 'Arma', 'Infrecuente'),
(47, 'Cruz de la Penumbra', 4, 804, 'Portador', 350, 0, 'Arma', 'Infrecuente'),
(48, 'Guantes de Luz Marchita', 2, 805, 'Portador', 400, 0, 'Arma', 'Rara'),
(49, 'Libro de la Sangre Bendita', 5, 806, 'Portador', 450, 0, 'Arma', 'Rara'),
(50, 'Cruz de los Ecos Sagrados', 4, 807, 'Portador', 500, 0, 'Arma', 'Rara'),
(51, 'Oro', 0, 0, 'Portador', 1, 0, 'Recurso', 'Comun');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `raza`
--

CREATE TABLE `raza` (
  `ID_Raza` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `Velocidad` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `raza`
--

INSERT INTO `raza` (`ID_Raza`, `Nombre`, `Descripcion`, `Velocidad`) VALUES
(1, 'Elfo', 'Seres agiles y antiguos, afinados con la magia y los bosques', 30),
(2, 'Enano', 'Robustos y tenaces, maestros de la forja y la piedra', 25),
(3, 'Humano', 'Versatiles y ambiciosos, capaces de grandes logros o ruinas', 30),
(4, 'Mediano', 'Pequenos y sigilosos, expertos en evitar el peligro', 25),
(5, 'Semielfo', 'Hibridos con carisma y herencia dividida entre dos mundos', 30),
(6, 'Semiorco', 'Fuerza brutal y espiritu indomito, nacidos para la batalla', 30),
(7, 'Tiefling', 'Marcados por sangre infernal, portadores de magia oscura', 30);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `stock`
--

CREATE TABLE `stock` (
  `ID_Stock` int(11) NOT NULL,
  `Cantidad` int(11) NOT NULL,
  `MultiplicadorPrecio` float NOT NULL,
  `ID_Objeto` int(11) NOT NULL,
  `ID_Tienda` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `subraza`
--

CREATE TABLE `subraza` (
  `ID_Subraza` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `ID_Raza` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `subraza`
--

INSERT INTO `subraza` (`ID_Subraza`, `Nombre`, `Descripcion`, `ID_Raza`) VALUES
(1, 'Alto Elfo', 'Portadores de luz ancestral, dominan la magia con elegancia fria', 1),
(2, 'Elfo Oscuro', 'Criaturas de sombras y rencor, su magia nace del dolor', 1),
(3, 'Enano de las Montanas', 'Forjados en piedra dura, resistentes como las cumbres que habitan', 2),
(4, 'Enano de las Colinas', 'Sabios y firmes, guardianes de secretos bajo tierra', 2),
(5, 'Humano', 'Sin linaje arcano, pero con voluntad capaz de romper destinos', 3),
(6, 'Mediano', 'Pequenos viajeros que burlan el peligro con astucia y suerte', 4),
(7, 'Semielfo', 'Errantes entre dos mundos, con mirada nostalgica y corazon dividido', 5),
(8, 'Semiorco', 'Hijos del conflicto, su furia es su escudo y su condena', 6),
(9, 'Tiefling', 'Herederos de pactos oscuros, su sangre arde con magia prohibida', 7);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tienda`
--

CREATE TABLE `tienda` (
  `ID_Tienda` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `ID_Mapa` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `transfondo`
--

CREATE TABLE `transfondo` (
  `ID_Transfondo` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Descripcion` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `transfondo`
--

INSERT INTO `transfondo` (`ID_Transfondo`, `Nombre`, `Descripcion`) VALUES
(1, 'Erudito', 'Portador de saberes rotos, su mente arde con verdades prohibidas'),
(2, 'Soldado', 'Forjado en batallas sin gloria, su acero recuerda nombres olvidados'),
(3, 'Acolito', 'Servidor de dioses muertos, su fe es eco de un templo derrumbado'),
(4, 'Criminal', 'Marcado por pactos rotos, su pasado acecha en cada sombra'),
(5, 'Noble', 'Herencia maldita, su linaje sangra entre ruinas y traiciones'),
(6, 'Cazador', 'Errante entre bestias, su mirada conoce la furia del bosque y la tumba'),
(7, 'Exiliado', 'Desterrado por crimen o verdad, su camino es ceniza y arrepentimiento'),
(8, 'Profanador', 'Toco lo prohibido, y ahora el mundo lo rechaza como heraldo de ruina');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `trasfondo_habilidades`
--

CREATE TABLE `trasfondo_habilidades` (
  `ID_TH` int(11) NOT NULL,
  `Competente` tinyint(1) NOT NULL,
  `ID_Transfondo` int(11) NOT NULL,
  `ID_Habilidades` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `trasfondo_habilidades`
--

INSERT INTO `trasfondo_habilidades` (`ID_TH`, `Competente`, `ID_Transfondo`, `ID_Habilidades`) VALUES
(1, 1, 1, 1),
(2, 1, 1, 2),
(3, 0, 1, 3),
(4, 1, 2, 4),
(5, 1, 2, 5),
(6, 0, 2, 6),
(7, 1, 3, 7),
(8, 1, 3, 8),
(9, 0, 3, 9),
(10, 1, 4, 10),
(11, 1, 4, 11),
(12, 0, 4, 12),
(13, 1, 5, 13),
(14, 1, 5, 14),
(15, 0, 5, 15),
(16, 1, 1, 16),
(17, 1, 2, 17),
(18, 0, 3, 18),
(19, 1, 4, 19),
(20, 1, 5, 20),
(21, 0, 1, 21),
(22, 1, 2, 22),
(23, 1, 3, 23),
(24, 0, 4, 24),
(25, 1, 5, 25);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `alineamiento`
--
ALTER TABLE `alineamiento`
  ADD PRIMARY KEY (`ID_Alineamiento`);

--
-- Indices de la tabla `atributosgen`
--
ALTER TABLE `atributosgen`
  ADD PRIMARY KEY (`ID_Atributo`);

--
-- Indices de la tabla `clase`
--
ALTER TABLE `clase`
  ADD PRIMARY KEY (`ID_Clase`);

--
-- Indices de la tabla `clase_habilidades`
--
ALTER TABLE `clase_habilidades`
  ADD PRIMARY KEY (`ID_ClHa`),
  ADD KEY `ClHa_ID_Clase` (`ID_Clase`),
  ADD KEY `ClHa_ID_Habilidad` (`ID_Habilidad`);

--
-- Indices de la tabla `conjuros`
--
ALTER TABLE `conjuros`
  ADD PRIMARY KEY (`ID_Conjuro`),
  ADD KEY `Conjuros_ID_Clase_Clase_ID_Clase` (`ID_Clase`),
  ADD KEY `Conjuros_ID_Hechizo_Hechizo_ID_Hechizo` (`ID_Hechizo`);

--
-- Indices de la tabla `dialogonpc`
--
ALTER TABLE `dialogonpc`
  ADD PRIMARY KEY (`ID_DialogoNPC`),
  ADD KEY `DialogoNPC_ID_NPC_NPC_ID_NPC` (`ID_NPC`);

--
-- Indices de la tabla `enemigo`
--
ALTER TABLE `enemigo`
  ADD PRIMARY KEY (`ID_Enemigo`),
  ADD KEY `Enemigo_Nivel_Nivel_ID_Nivel` (`Nivel`),
  ADD KEY `Enemigo_ID_Transfondo_Transfondo_ID_Transfondo` (`ID_Transfondo`),
  ADD KEY `Enemigo_ID_Clase_Clase_ID_Clase` (`ID_Clase`),
  ADD KEY `Enemigo_ID_Subraza_Subraza_ID_Subraza` (`ID_Subraza`),
  ADD KEY `Enemigo_ID_Objeto_Objeto_ID_Objeto` (`ID_Objeto`);

--
-- Indices de la tabla `habilidades`
--
ALTER TABLE `habilidades`
  ADD PRIMARY KEY (`ID_Habilidades`);

--
-- Indices de la tabla `hechizo`
--
ALTER TABLE `hechizo`
  ADD PRIMARY KEY (`ID_Hechizo`),
  ADD KEY `Hechizo_ID_Nivel_Nivel_ID_Nivel` (`ID_Nivel`);

--
-- Indices de la tabla `inventario`
--
ALTER TABLE `inventario`
  ADD PRIMARY KEY (`ID_Inventario`),
  ADD KEY `Inventario_ID_Jugador_Jugador_ID_Jugador` (`ID_Jugador`),
  ADD KEY `Inventario_ID_Objeto_Objeto_ID_Objeto` (`ID_Objeto`);

--
-- Indices de la tabla `jugador`
--
ALTER TABLE `jugador`
  ADD PRIMARY KEY (`ID_Jugador`),
  ADD UNIQUE KEY `Contrasena` (`Contrasena`),
  ADD KEY `Jugador_ID_Alineamiento_Alineamiento_ID_Alineamiento` (`ID_Alineamiento`),
  ADD KEY `Jugador_ID_Nivel_Nivel_ID_Nivel` (`ID_Nivel`),
  ADD KEY `Jugador_ID_Clase_Clase_ID_Clase` (`ID_Clase`),
  ADD KEY `Jugador_ID_Transfondo_Transfondo_ID_Transfondo` (`ID_Transfondo`),
  ADD KEY `Jugador_ID_Subraza_Subraza_ID_Subraza` (`ID_Subraza`);

--
-- Indices de la tabla `mapa`
--
ALTER TABLE `mapa`
  ADD PRIMARY KEY (`ID_Mapa`);

--
-- Indices de la tabla `misiones`
--
ALTER TABLE `misiones`
  ADD PRIMARY KEY (`ID_Misiones`),
  ADD KEY `Misiones_ID_Objeto_Objeto_ID_Objeto` (`ID_Objeto`),
  ADD KEY `Misiones_ID_Mapa_Mapa_ID_Mapa` (`ID_Mapa`),
  ADD KEY `Misiones_ID_NPC_NPC_ID_NPC` (`ID_NPC`),
  ADD KEY `Misiones_ID_Enemigo_Enemigo_ID_Enemigo` (`ID_Enemigo`);

--
-- Indices de la tabla `nivel`
--
ALTER TABLE `nivel`
  ADD PRIMARY KEY (`ID_Nivel`);

--
-- Indices de la tabla `npc`
--
ALTER TABLE `npc`
  ADD PRIMARY KEY (`ID_NPC`);

--
-- Indices de la tabla `objeto`
--
ALTER TABLE `objeto`
  ADD PRIMARY KEY (`ID_Objeto`);

--
-- Indices de la tabla `raza`
--
ALTER TABLE `raza`
  ADD PRIMARY KEY (`ID_Raza`);

--
-- Indices de la tabla `stock`
--
ALTER TABLE `stock`
  ADD PRIMARY KEY (`ID_Stock`),
  ADD KEY `Stock_ID_Objeto_Objeto_ID_Objeto` (`ID_Objeto`),
  ADD KEY `Stock_ID_Tienda_Tienda_ID_Tienda` (`ID_Tienda`);

--
-- Indices de la tabla `subraza`
--
ALTER TABLE `subraza`
  ADD PRIMARY KEY (`ID_Subraza`),
  ADD KEY `Subraza_ID_Raza_Raza_ID_Raza` (`ID_Raza`);

--
-- Indices de la tabla `tienda`
--
ALTER TABLE `tienda`
  ADD PRIMARY KEY (`ID_Tienda`),
  ADD KEY `Tienda_ID_Mapa_Mapa_ID_Mapa` (`ID_Mapa`);

--
-- Indices de la tabla `transfondo`
--
ALTER TABLE `transfondo`
  ADD PRIMARY KEY (`ID_Transfondo`);

--
-- Indices de la tabla `trasfondo_habilidades`
--
ALTER TABLE `trasfondo_habilidades`
  ADD PRIMARY KEY (`ID_TH`),
  ADD KEY `TH_ID_Transfondo` (`ID_Transfondo`),
  ADD KEY `TH_ID_Habilidades` (`ID_Habilidades`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `jugador`
--
ALTER TABLE `jugador`
  MODIFY `ID_Jugador` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `clase_habilidades`
--
ALTER TABLE `clase_habilidades`
  ADD CONSTRAINT `ClHa_ID_Clase` FOREIGN KEY (`ID_Clase`) REFERENCES `clase` (`ID_Clase`),
  ADD CONSTRAINT `ClHa_ID_Habilidad` FOREIGN KEY (`ID_Habilidad`) REFERENCES `habilidades` (`ID_Habilidades`);

--
-- Filtros para la tabla `conjuros`
--
ALTER TABLE `conjuros`
  ADD CONSTRAINT `Conjuros_ID_Clase_Clase_ID_Clase` FOREIGN KEY (`ID_Clase`) REFERENCES `clase` (`ID_Clase`),
  ADD CONSTRAINT `Conjuros_ID_Hechizo_Hechizo_ID_Hechizo` FOREIGN KEY (`ID_Hechizo`) REFERENCES `hechizo` (`ID_Hechizo`);

--
-- Filtros para la tabla `dialogonpc`
--
ALTER TABLE `dialogonpc`
  ADD CONSTRAINT `DialogoNPC_ID_NPC_NPC_ID_NPC` FOREIGN KEY (`ID_NPC`) REFERENCES `npc` (`ID_NPC`);

--
-- Filtros para la tabla `enemigo`
--
ALTER TABLE `enemigo`
  ADD CONSTRAINT `Enemigo_ID_Clase_Clase_ID_Clase` FOREIGN KEY (`ID_Clase`) REFERENCES `clase` (`ID_Clase`),
  ADD CONSTRAINT `Enemigo_ID_Objeto_Objeto_ID_Objeto` FOREIGN KEY (`ID_Objeto`) REFERENCES `objeto` (`ID_Objeto`),
  ADD CONSTRAINT `Enemigo_ID_Subraza_Subraza_ID_Subraza` FOREIGN KEY (`ID_Subraza`) REFERENCES `subraza` (`ID_Subraza`),
  ADD CONSTRAINT `Enemigo_ID_Transfondo_Transfondo_ID_Transfondo` FOREIGN KEY (`ID_Transfondo`) REFERENCES `transfondo` (`ID_Transfondo`),
  ADD CONSTRAINT `Enemigo_Nivel_Nivel_ID_Nivel` FOREIGN KEY (`Nivel`) REFERENCES `nivel` (`ID_Nivel`);

--
-- Filtros para la tabla `hechizo`
--
ALTER TABLE `hechizo`
  ADD CONSTRAINT `Hechizo_ID_Nivel_Nivel_ID_Nivel` FOREIGN KEY (`ID_Nivel`) REFERENCES `nivel` (`ID_Nivel`);

--
-- Filtros para la tabla `inventario`
--
ALTER TABLE `inventario`
  ADD CONSTRAINT `Inventario_ID_Jugador_Jugador_ID_Jugador` FOREIGN KEY (`ID_Jugador`) REFERENCES `jugador` (`ID_Jugador`),
  ADD CONSTRAINT `Inventario_ID_Objeto_Objeto_ID_Objeto` FOREIGN KEY (`ID_Objeto`) REFERENCES `objeto` (`ID_Objeto`);

--
-- Filtros para la tabla `jugador`
--
ALTER TABLE `jugador`
  ADD CONSTRAINT `Jugador_ID_Alineamiento_Alineamiento_ID_Alineamiento` FOREIGN KEY (`ID_Alineamiento`) REFERENCES `alineamiento` (`ID_Alineamiento`),
  ADD CONSTRAINT `Jugador_ID_Clase_Clase_ID_Clase` FOREIGN KEY (`ID_Clase`) REFERENCES `clase` (`ID_Clase`),
  ADD CONSTRAINT `Jugador_ID_Nivel_Nivel_ID_Nivel` FOREIGN KEY (`ID_Nivel`) REFERENCES `nivel` (`ID_Nivel`),
  ADD CONSTRAINT `Jugador_ID_Subraza_Subraza_ID_Subraza` FOREIGN KEY (`ID_Subraza`) REFERENCES `subraza` (`ID_Subraza`),
  ADD CONSTRAINT `Jugador_ID_Transfondo_Transfondo_ID_Transfondo` FOREIGN KEY (`ID_Transfondo`) REFERENCES `transfondo` (`ID_Transfondo`);

--
-- Filtros para la tabla `misiones`
--
ALTER TABLE `misiones`
  ADD CONSTRAINT `Misiones_ID_Enemigo_Enemigo_ID_Enemigo` FOREIGN KEY (`ID_Enemigo`) REFERENCES `enemigo` (`ID_Enemigo`),
  ADD CONSTRAINT `Misiones_ID_Mapa_Mapa_ID_Mapa` FOREIGN KEY (`ID_Mapa`) REFERENCES `mapa` (`ID_Mapa`),
  ADD CONSTRAINT `Misiones_ID_NPC_NPC_ID_NPC` FOREIGN KEY (`ID_NPC`) REFERENCES `npc` (`ID_NPC`),
  ADD CONSTRAINT `Misiones_ID_Objeto_Objeto_ID_Objeto` FOREIGN KEY (`ID_Objeto`) REFERENCES `objeto` (`ID_Objeto`);

--
-- Filtros para la tabla `stock`
--
ALTER TABLE `stock`
  ADD CONSTRAINT `Stock_ID_Objeto_Objeto_ID_Objeto` FOREIGN KEY (`ID_Objeto`) REFERENCES `objeto` (`ID_Objeto`),
  ADD CONSTRAINT `Stock_ID_Tienda_Tienda_ID_Tienda` FOREIGN KEY (`ID_Tienda`) REFERENCES `tienda` (`ID_Tienda`);

--
-- Filtros para la tabla `subraza`
--
ALTER TABLE `subraza`
  ADD CONSTRAINT `Subraza_ID_Raza_Raza_ID_Raza` FOREIGN KEY (`ID_Raza`) REFERENCES `raza` (`ID_Raza`);

--
-- Filtros para la tabla `tienda`
--
ALTER TABLE `tienda`
  ADD CONSTRAINT `Tienda_ID_Mapa_Mapa_ID_Mapa` FOREIGN KEY (`ID_Mapa`) REFERENCES `mapa` (`ID_Mapa`);

--
-- Filtros para la tabla `trasfondo_habilidades`
--
ALTER TABLE `trasfondo_habilidades`
  ADD CONSTRAINT `TH_ID_Habilidades` FOREIGN KEY (`ID_Habilidades`) REFERENCES `habilidades` (`ID_Habilidades`),
  ADD CONSTRAINT `TH_ID_Transfondo` FOREIGN KEY (`ID_Transfondo`) REFERENCES `transfondo` (`ID_Transfondo`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
