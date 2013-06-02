CREATE TABLE `login` (
	`ID` INT(32) NOT NULL AUTO_INCREMENT,
	`CreationDate` DATETIME NOT NULL,
	`Email` VARCHAR(64) NOT NULL COLLATE 'latin1_general_ci',
	`FirstName` VARCHAR(32) NOT NULL COLLATE 'latin1_general_ci',
	`LastName` VARCHAR(32) NOT NULL COLLATE 'latin1_general_ci',
	`Username` VARCHAR(32) NOT NULL COLLATE 'latin1_general_ci',
	`Password` VARCHAR(37) NOT NULL COLLATE 'latin1_general_ci',
	`Allowed_Characters` INT(32) NOT NULL DEFAULT '6' COMMENT 'You can change this to whatever you want 0 is disabled.. no characters allowed',
	`Flags` INT(32) NOT NULL DEFAULT '0',
	`AccountFlags` INT(32) NOT NULL DEFAULT '0',
	`Expansions` INT NOT NULL DEFAULT '2047' COLLATE 'latin1_general_ci',
	`GM` INT(32) NOT NULL DEFAULT '0',
	PRIMARY KEY (`ID`),
	UNIQUE INDEX `Username` (`Username`)
)
COLLATE='latin1_general_ci'
ENGINE=InnoDB;
