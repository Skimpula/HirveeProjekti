//Tässä SQL-skriptissä luodaan tietokanta ja taulut, sekä lisätään esimerkkitietoja mökkivuokrausjärjestelmää varten. Skripti on suunniteltu MySQL Workbench -työkalua varten, ja se sisältää kaikki tarvittavat komennot tietokannan luomiseen, taulujen määrittelyyn ja tietojen syöttämiseen.
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema vn
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `vn` ;

CREATE SCHEMA IF NOT EXISTS `vn` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin ;
USE `vn` ;

-- -----------------------------------------------------
-- Table `vn`.`alue`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`alue` ;

CREATE TABLE IF NOT EXISTS `vn`.`alue` (
  `alue_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `nimi` VARCHAR(40) NULL DEFAULT NULL,
  PRIMARY KEY (`alue_id`),
  INDEX `alue_nimi_index` (`nimi` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`posti`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`posti` ;

CREATE TABLE IF NOT EXISTS `vn`.`posti` (
  `postinro` CHAR(5) NOT NULL,
  `toimipaikka` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`postinro`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`asiakas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`asiakas` ;

CREATE TABLE IF NOT EXISTS `vn`.`asiakas` (
  `asiakas_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `postinro` CHAR(5) NOT NULL,
  `etunimi` VARCHAR(20) NULL DEFAULT NULL,
  `sukunimi` VARCHAR(40) NULL DEFAULT NULL,
  `lahiosoite` VARCHAR(40) NULL DEFAULT NULL,
  `email` VARCHAR(50) NULL DEFAULT NULL,
  `puhelinnro` VARCHAR(15) NULL DEFAULT NULL,
  PRIMARY KEY (`asiakas_id`),
  INDEX `fk_as_posti1_idx` (`postinro` ASC) VISIBLE,
  INDEX `asiakas_snimi_idx` (`sukunimi` ASC) VISIBLE,
  INDEX `asiakas_enimi_idx` (`etunimi` ASC) VISIBLE,
  CONSTRAINT `fk_asiakas_posti`
    FOREIGN KEY (`postinro`)
    REFERENCES `vn`.`posti` (`postinro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`mokki`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`mokki` ;

CREATE TABLE IF NOT EXISTS `vn`.`mokki` (
  `mokki_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `alue_id` INT UNSIGNED NOT NULL,
  `postinro` CHAR(5) NOT NULL,
  `mokkinimi` VARCHAR(45) NULL DEFAULT NULL,
  `katuosoite` VARCHAR(45) NULL DEFAULT NULL,
  `hinta` DOUBLE(8,2) NOT NULL,
  `kuvaus` VARCHAR(150) NULL DEFAULT NULL,
  `henkilomaara` INT NULL DEFAULT NULL,
  `varustelu` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`mokki_id`),
  INDEX `fk_mokki_alue_idx` (`alue_id` ASC) VISIBLE,
  INDEX `fk_mokki_posti_idx` (`postinro` ASC) VISIBLE,
  CONSTRAINT `fk_mokki_alue`
    FOREIGN KEY (`alue_id`)
    REFERENCES `vn`.`alue` (`alue_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_mokki_posti`
    FOREIGN KEY (`postinro`)
    REFERENCES `vn`.`posti` (`postinro`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`varaus`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`varaus` ;

CREATE TABLE IF NOT EXISTS `vn`.`varaus` (
  `varaus_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `asiakas_id` INT UNSIGNED NOT NULL,
  `mokki_id` INT UNSIGNED NOT NULL,
  `varattu_pvm` DATETIME NULL DEFAULT NULL,
  `vahvistus_pvm` DATETIME NULL DEFAULT NULL,
  `varattu_alkupvm` DATETIME NULL DEFAULT NULL,
  `varattu_loppupvm` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`varaus_id`),
  INDEX `varaus_as_id_index` (`asiakas_id` ASC) VISIBLE,
  INDEX `fk_var_mok_idx` (`mokki_id` ASC) VISIBLE,
  CONSTRAINT `fk_varaus_mokki`
    FOREIGN KEY (`mokki_id`)
    REFERENCES `vn`.`mokki` (`mokki_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `varaus_ibfk`
    FOREIGN KEY (`asiakas_id`)
    REFERENCES `vn`.`asiakas` (`asiakas_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`lasku`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`lasku` ;

CREATE TABLE IF NOT EXISTS `vn`.`lasku` (
  `lasku_id` INT NOT NULL,
  `varaus_id` INT UNSIGNED NOT NULL,
  `summa` DOUBLE(8,2) NOT NULL,
  `alv` DOUBLE(8,2) NOT NULL,
  `maksettu` TINYINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`lasku_id`),
  INDEX `lasku_ibfk_1` (`varaus_id` ASC) VISIBLE,
  CONSTRAINT `lasku_ibfk_1`
    FOREIGN KEY (`varaus_id`)
    REFERENCES `vn`.`varaus` (`varaus_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`palvelu`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`palvelu` ;

CREATE TABLE IF NOT EXISTS `vn`.`palvelu` (
  `palvelu_id` INT UNSIGNED NOT NULL,
  `alue_id` INT UNSIGNED NOT NULL,
  `nimi` VARCHAR(40) NULL DEFAULT NULL,
  `kuvaus` VARCHAR(255) NULL DEFAULT NULL,
  `hinta` DOUBLE(8,2) NOT NULL,
  `alv` DOUBLE(8,2) NOT NULL,
  PRIMARY KEY (`palvelu_id`),
  INDEX `Palvelu_nimi_index` (`nimi` ASC) VISIBLE,
  INDEX `palv_toimip_id_ind` (`alue_id` ASC) VISIBLE,
  CONSTRAINT `palvelu_ibfk_1`
    FOREIGN KEY (`alue_id`)
    REFERENCES `vn`.`alue` (`alue_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


-- -----------------------------------------------------
-- Table `vn`.`varauksen_palvelut`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `vn`.`varauksen_palvelut` ;

CREATE TABLE IF NOT EXISTS `vn`.`varauksen_palvelut` (
  `varaus_id` INT UNSIGNED NOT NULL,
  `palvelu_id` INT UNSIGNED NOT NULL,
  `lkm` INT NOT NULL,
  PRIMARY KEY (`palvelu_id`, `varaus_id`),
  INDEX `vp_varaus_id_index` (`varaus_id` ASC) VISIBLE,
  INDEX `vp_palvelu_id_index` (`palvelu_id` ASC) VISIBLE,
  CONSTRAINT `fk_palvelu`
    FOREIGN KEY (`palvelu_id`)
    REFERENCES `vn`.`palvelu` (`palvelu_id`),
  CONSTRAINT `fk_varaus`
    FOREIGN KEY (`varaus_id`)
    REFERENCES `vn`.`varaus` (`varaus_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;

-- =====================================================
-- INSERTING SAMPLE DATA
-- =====================================================

-- Insert into `alue` (Area/Region)
INSERT INTO `vn`.`alue` (`alue_id`, `nimi`) VALUES
(1, 'Lappi'),
(2, 'Karelia'),
(3, 'Turku'),
(4, 'Helsinki'),
(5, 'Vaasa');

-- Insert into `posti` (Postal Code)
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES
('96200', 'Rovaniemi'),
('96400', 'Kolari'),
('80100', 'Joensuu'),
('80200', 'Kontiolahti'),
('20100', 'Turku'),
('20200', 'Maaria'),
('00100', 'Helsinki'),
('00200', 'Espoo'),
('65100', 'Vaasa'),
('65200', 'Mustasaari');

-- Insert into `asiakas` (Customer)
INSERT INTO `vn`.`asiakas` (`asiakas_id`, `postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`) VALUES
(1, '00100', 'Jari', 'Virtanen', 'Männikötie 5', 'jari.virtanen@email.com', '0501234567'),
(2, '00200', 'Kaisa', 'Mäkinen', 'Puusepäntie 10', 'kaisa.makinen@email.com', '0502345678'),
(3, '20100', 'Mikko', 'Korhonen', 'Kalevantie 15', 'mikko.korhonen@email.com', '0503456789'),
(4, '96200', 'Liisa', 'Nieminen', 'Ounasvaarantie 20', 'liisa.nieminen@email.com', '0504567890'),
(5, '80100', 'Pekka', 'Rantanen', 'Siikatie 25', 'pekka.rantanen@email.com', '0505678901'),
(6, '65100', 'Anna', 'Seppälä', 'Hovioikeudenk. 30', 'anna.seppala@email.com', '0506789012');

-- Insert into `mokki` (Cottage)
INSERT INTO `vn`.`mokki` (`mokki_id`, `alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`) VALUES
(1, 1, '96200', 'Lapin Talvimökki', 'Tunturintie 1', 150.00, 'Lämmin mökki poronhiihdon keskellä', 6, 'Sauna, poreallas, kaakao'),
(2, 1, '96400', 'Koillis-Lapin Lumipesä', 'Lumihiukkastie 5', 120.00, 'Pieni perhemökki lumipeitteisen maiseman äärellä', 4, 'Sauna, TV'),
(3, 2, '80100', 'Karjalan Kaunis Koti', 'Pielisniementie 10', 100.00, 'Perinteinen mökki Pielisen rannalla', 5, 'Sauna, grilli, uimarantaa'),
(4, 2, '80200', 'Joen Ranta Mökki', 'Joensuuntie 15', 110.00, 'Moderni mökki kalastajien paratiisissa', 4, 'Sauna, keittiö, pesula'),
(5, 3, '20100', 'Turun Saaristomökki', 'Saaristotie 20', 95.00, 'Kesämökki saaressa', 3, 'Grilli, terassi'),
(6, 4, '00100', 'Helsingin Lähellä', 'Metrotie 25', 130.00, 'Rauhallinen mökki kaupungin lähellä', 5, 'Sauna, pysäköinti, patio'),
(7, 5, '65100', 'Pohjanmaan Perhemökki', 'Rantatakatie 30', 105.00, 'Ystävällinen mökki perheiden suosikki', 6, 'Sauna, leikkikenttä, grilli');

-- Insert into `varaus` (Booking)
INSERT INTO `vn`.`varaus` (`varaus_id`, `asiakas_id`, `mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`) VALUES
(1, 1, 1, '2026-03-01 10:30:00', '2026-03-02 14:00:00', '2026-04-15 15:00:00', '2026-04-22 11:00:00'),
(2, 2, 3, '2026-02-15 09:00:00', '2026-02-16 11:30:00', '2026-05-01 15:00:00', '2026-05-08 11:00:00'),
(3, 3, 5, '2026-03-10 14:15:00', '2026-03-11 16:45:00', '2026-06-20 15:00:00', '2026-06-27 11:00:00'),
(4, 4, 2, '2026-01-20 11:00:00', '2026-01-21 13:30:00', '2026-04-10 15:00:00', '2026-04-17 11:00:00'),
(5, 5, 4, '2026-02-25 15:30:00', '2026-02-26 17:00:00', '2026-05-10 15:00:00', '2026-05-17 11:00:00'),
(6, 6, 6, '2026-03-05 12:00:00', '2026-03-06 14:30:00', '2026-04-25 15:00:00', '2026-05-02 11:00:00');

-- Insert into `lasku` (Invoice)
INSERT INTO `vn`.`lasku` (`lasku_id`, `varaus_id`, `summa`, `alv`, `maksettu`) VALUES
(1, 1, 1050.00, 231.00, 1),
(2, 2, 700.00, 154.00, 1),
(3, 3, 665.00, 146.30, 0),
(4, 4, 840.00, 184.80, 1),
(5, 5, 770.00, 169.40, 0),
(6, 6, 910.00, 200.20, 1);

-- Insert into `palvelu` (Service)
INSERT INTO `vn`.`palvelu` (`palvelu_id`, `alue_id`, `nimi`, `kuvaus`, `hinta`, `alv`) VALUES
(1, 1, 'Poronajelu', 'Aito poronjäristysretki tunturilla', 80.00, 17.60),
(2, 1, 'Revontuliet', 'Revontulisafari lumiautoilla', 120.00, 26.40),
(3, 2, 'Kalastusreissu', 'Opasteinen kalastusretki paikallisissa vesissä', 60.00, 13.20),
(4, 2, 'Saunapalvelu', 'Perinteinen läyly sauna yhdessä', 40.00, 8.80),
(5, 3, 'Saariston kierros', 'Veneretki Turun saariston läpi', 75.00, 16.50),
(6, 4, 'Kaupunkikierros', 'Opastettu kierros Helsingin nähtävyyksiin', 50.00, 11.00),
(7, 5, 'Uintitunnit', 'Uintitunnit paikalliset ammattilaisilta', 45.00, 9.90);

-- Insert into `varauksen_palvelut` (Booking Services - Junction Table)
INSERT INTO `vn`.`varauksen_palvelut` (`varaus_id`, `palvelu_id`, `lkm`) VALUES
(1, 1, 2),
(1, 2, 1),
(2, 3, 3),
(3, 5, 2),
(4, 1, 1),
(4, 2, 2),
(5, 3, 2),
(6, 6, 1);

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;