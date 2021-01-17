CREATE SCHEMA Vote
GO

CREATE TABLE Vote.SessionVote (
	IdSessionVote int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	NomSessionVote	nvarchar(100) NOT NULL,
	DebutSession	datetime2	NOT NULL,
	FinSession		datetime2	NULL,
	InitiateurSession	nvarchar(255) NOT NULL,
	Anonyme	bit NULL
)

CREATE TABLE Vote.Inscrit (
	LoginInscrit	varchar(255) NOT NULL,
	IdSessionVote	int NOT NULL,
	PRIMARY KEY (LoginInscrit, IdSessionVote)
)

ALTER TABLE Vote.Inscrit ADD CONSTRAINT FK_Inscrit_Session FOREIGN KEY (IdSessionVote) REFERENCES Vote.SessionVote (IdSessionVote)

CREATE TABLE Vote.Vote (
	IdVote int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	IdSessionVote int NOT NULL,
	IntituleVote nvarchar(255) NOT NULL,
	VoteOuvert bit NOT NULL DEFAULT 0,
	Anonyme	bit NOT NULL DEFAULT 0
)

ALTER TABLE Vote.Vote ADD CONSTRAINT FK_Vote_Session FOREIGN KEY (IdSessionVote) REFERENCES Vote.SessionVote (IdSessionVote)

CREATE TABLE Vote.AVoté (
	IdVote int NOT NULL,
	LoginInscrit varchar(255) NOT NULL,
	DateHeureVote datetime2 NOT NULL)

ALTER TABLE Vote.AVoté ADD CONSTRAINT FK_AVoté_Vote FOREIGN KEY (IdVote) REFERENCES Vote.Vote (IdVote)
--ALTER TABLE Vote.AVoté ADD CONSTRAINT FK_AVoté_Inscrit FOREIGN KEY (LoginInscrit) REFERENCES Vote.Inscrit (LoginInscrit)

CREATE TABLE Vote.Choix (
	IdChoix int IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	IdVote int NOT NULL,
	Ordre int NOT NULL DEFAULT 0,
	IntituleChoix nvarchar(255) NOT NULL
)

ALTER TABLE Vote.Choix ADD CONSTRAINT FK_Choix_Vote FOREIGN KEY (IdVote) REFERENCES Vote.Vote (IdVote)

CREATE TABLE Vote.SuffrageExprimé (
	IdVote int NOT NULL,
	IdChoix int NOT NULL,
	LoginInscrit varchar(255) NULL,		-- Au cas où le vote soit anonyme.
	DateHeureModif datetime2 NOT NULL,
	IdGroupeReponses int NULL,			-- Pour les modes de votes alternatifs
	CodeReponse varchar(10) NULL		-- Pour les modes de votes alternatifs
)

ALTER TABLE Vote.SuffrageExprimé ADD CONSTRAINT FK_Suffrage_Vote FOREIGN KEY (IdVote) REFERENCES Vote.Vote (IdVote)
ALTER TABLE Vote.SuffrageExprimé ADD CONSTRAINT FK_Suffrage_Choix FOREIGN KEY (IdChoix) REFERENCES Vote.Choix (IdChoix)
--ALTER TABLE Vote.SuffrageExprimé ADD CONSTRAINT FK_Suffrage_Inscrit FOREIGN KEY (LoginInscrit) REFERENCES Vote.Inscrit (LoginInscrit)
