CREATE DATABASE TeamDatabase;


USE TeamDatabase;


CREATE TABLE Team (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(255) NOT NULL,
    Points INT NOT NULL DEFAULT 0,
    Placement INT NOT NULL DEFAULT 0,
    IsActive TINYINT NOT NULL DEFAULT 1,
    CONSTRAINT PK_Team PRIMARY KEY (Id)
)

CREATE TABLE Pilot (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
    Surname VARCHAR(50) NOT NULL,
    Weight DECIMAL(5,2) NOT NULL,
    Age INT NOT NULL,
    IdentificationNumber INT NOT NULL,
    Status TINYINT NOT NULL,
    Experience DECIMAL(5,3) NOT NULL,
    Handicap DECIMAL(5,2) NOT NULL,
    Points INT NOT NULL,
    TeamId INT NOT NULL,
    Position INT NOT NULL DEFAULT 0,
    IsActive TINYINT NOT NULL DEFAULT 1,

    CONSTRAINT PK_Pilot PRIMARY KEY (Id),
    CONSTRAINT UQ_Pilot_Identification UNIQUE (IdentificationNumber),
    CONSTRAINT CK_Pilot_Identification CHECK (IdentificationNumber BETWEEN 1 AND 99),
    CONSTRAINT CK_Pilot_Weight CHECK (Weight > 0),
    CONSTRAINT CK_Pilot_Experience CHECK (Experience BETWEEN 1.000 AND 5.000),
    CONSTRAINT FK_Pilot_Team FOREIGN KEY (TeamId) REFERENCES Team(Id),
    CONSTRAINT UQ_Pilot_Position CHECK (Position BETWEEN 1 AND 22)
)

CREATE TABLE Car (
    Id INT NOT NULL IDENTITY(1,1),
    AerodynamicCoefficent DECIMAL(5,3) NOT NULL,
    PowerCoefficient DECIMAL(5,3) NOT NULL,
    Weight DECIMAL(6,2) NOT NULL,
    Model VARCHAR(5) NOT NULL,
    PilotId INT NOT NULL,
    IsActive TINYINT NOT NULL DEFAULT 1,

    CONSTRAINT PK_Car PRIMARY KEY (Id),
    CONSTRAINT UQ_Car_Pilot UNIQUE (PilotId),
    CONSTRAINT CK_Car_Aero CHECK (AerodynamicCoefficent BETWEEN 0.000 AND 10.000),
    CONSTRAINT CK_Car_Power CHECK (PowerCoefficient BETWEEN 0.000 AND 10.000),
    CONSTRAINT CK_Car_Weight CHECK (Weight > 0),
    CONSTRAINT UQ_Car_Model CHECK (LEN (Model) = 5),
    CONSTRAINT FK_Car_Pilot FOREIGN KEY (PilotId) REFERENCES Pilot(Id)
)

CREATE TABLE Engineer (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(255) NOT NULL,
    Surname VARCHAR(255) NOT NULL,
    Age INT NOT NULL,
    Experience DECIMAL(5,2) NOT NULL,
    Type TINYINT NOT NULL,
    Status VARCHAR(255) NOT NULL,
    TeamId INT NOT NULL,
    CarId INT NOT NULL,
    IsActive TINYINT NOT NULL DEFAULT 1,

    CONSTRAINT PK_Engineer PRIMARY KEY (Id),
    CONSTRAINT CK_Engineer_Experience CHECK (Experience BETWEEN 1.000 AND 5.000),
    CONSTRAINT UQ_Engineer_Team_Type UNIQUE (TeamId, Type),
    CONSTRAINT FK_Engineer_Team FOREIGN KEY (TeamId) REFERENCES Team(Id),
    CONSTRAINT FK_Engineer_Car FOREIGN KEY (CarId) REFERENCES Car(Id)
)

CREATE TABLE Boss (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(255) NOT NULL,
    Surname VARCHAR(255) NOT NULL,
    Age INT NOT NULL,
    Type VARCHAR(255) NOT NULL,
    Status TINYINT NOT NULL,
    TeamId INT NOT NULL,
    IsActive TINYINT NOT NULL DEFAULT 1,

    CONSTRAINT PK_Boss PRIMARY KEY (Id),
    CONSTRAINT FK_Boss_Team FOREIGN KEY (TeamId) REFERENCES Team(Id)
)

drop table team
drop table pilot
drop table Car
drop table Engineer
drop table boss

select * from Team;
select * from Pilot;
select * from Car;
select * from Engineer;
select * from Boss;

INSERT INTO Team (Name, Points, Placement) VALUES
('Apex Racing', 0, 0),
('Thunder Motors', 0, 0),
('Crimson Velocity', 0, 0),
('Iron Falcon GP', 0, 0),
('Nova Horizon Racing', 0, 0);


INSERT INTO Boss (Name, Surname, Age, Type, Status, TeamId) VALUES
('Marcus', 'Stone', 48, 'Team Principal', 1, 1),
('Robert', 'Hill', 55, 'Sporting Director', 1, 1),

('Laura', 'Mitchell', 45, 'Team Principal', 1, 2),
('Emma', 'Clark', 49, 'Sporting Director', 1, 2),

('Victor', 'Rossi', 52, 'Team Principal', 1, 3),
('Giovanni', 'Ricci', 58, 'Sporting Director', 1, 3),

('Helena', 'Krauss', 50, 'Team Principal', 1, 4),
('Thomas', 'Becker', 53, 'Sporting Director', 1, 4),

('Daniel', 'Okada', 46, 'Team Principal', 1, 5),
('Akira', 'Nakamura', 51, 'Sporting Director', 1, 5);


INSERT INTO Pilot
(Name, Surname, Weight, Age, IdentificationNumber, Status, Experience, Handicap, Points, TeamId, Position)
VALUES
('Lucas', 'Reed', 72.50, 26, 11, 1, 4.200, 1.50, 0, 1, 1),
('Noah', 'Turner', 73.40, 27, 12, 1, 3.900, 1.60, 0, 1, 3),

('Ethan', 'Cole', 74.00, 28, 22, 1, 3.800, 1.70, 0, 2, 5),
('Liam', 'Walker', 75.10, 29, 23, 1, 4.100, 1.50, 0, 2, 2),

('Matteo', 'Bianchi', 70.20, 24, 33, 1, 4.500, 1.30, 0, 3, 11),
('Marco', 'De Luca', 71.80, 26, 34, 1, 4.300, 1.40, 0, 3, 8),

('Jonas', 'Muller', 76.10, 30, 44, 1, 4.000, 1.60, 0, 4, 17),
('Felix', 'Schneider', 77.00, 31, 45, 1, 3.800, 1.70, 0, 4, 21),

('Kenji', 'Sato', 68.90, 25, 55, 1, 4.700, 1.20, 0, 5, 12),
('Ryo', 'Kobayashi', 69.50, 24, 56, 1, 4.600, 1.30, 0, 5, 20);


INSERT INTO Car
(AerodynamicCoefficent, PowerCoefficient, Weight, Model, PilotId)
VALUES
(7.800, 8.500, 798.50, 'ARO01', 1),
(7.600, 8.300, 801.20, 'ARO01', 2),

(7.500, 8.200, 802.00, 'TMT01', 3),
(7.400, 8.100, 804.00, 'TMT02', 4),

(8.100, 8.700, 795.30, 'CVR01', 5),
(8.000, 8.600, 797.40, 'CVR02', 6),

(7.900, 8.400, 800.10, 'IFG01', 7),
(7.700, 8.200, 803.30, 'IFG02', 8),

(8.300, 8.900, 792.80, 'NHZ01', 9),
(8.200, 8.800, 794.90, 'NHZ02', 10);



INSERT INTO Engineer
(Name, Surname, Age, Experience, Type, Status, TeamId, CarId)
VALUES
('Oliver', 'Grant', 38, 4.200, 1, 'Active', 1, 1),
('Daniel', 'Moore', 36, 3.700, 2, 'Active', 1, 2),

('Sophia', 'Brown', 35, 3.900, 1, 'Active', 2, 3),
('Isabella', 'Adams', 34, 3.500, 2, 'Active', 2, 4),

('Alessandro', 'Moretti', 41, 4.500, 1, 'Active', 3, 5),
('Stefano', 'Galli', 40, 4.200, 2, 'Active', 3, 6),

('Klaus', 'Weber', 44, 4.800, 1, 'Active', 4, 7),
('Martin', 'Fischer', 42, 4.400, 2, 'Active', 4, 8),

('Hiroshi', 'Tanaka', 39, 4.300, 1, 'Active', 5, 9),
('Yuki', 'Shimizu', 37, 4.000, 2, 'Active', 5, 10);
