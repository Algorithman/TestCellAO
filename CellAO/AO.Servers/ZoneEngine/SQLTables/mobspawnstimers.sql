CREATE TABLE  `mobspawnstimers` (
  `ID` int(10) NOT NULL,
  `Playfield` int(10) NOT NULL,
  `strain` int(10) NOT NULL,
  `timespan` int(10) NOT NULL,
  `function` blob NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;